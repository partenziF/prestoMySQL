﻿using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Exception;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace prestoMySQL.SQL {
    public static class SQLBuilder {

        public static string sqlCreate<T>( bool ifNotExists = true ) where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();

            List<String> pk = null;
            List<String> result = new List<String>();

            sb.Append( string.Format( "CREATE TABLE {0} \"{1}\" (\n" , ( ifNotExists ? "IF NOT EXISTS" : "" ) , SQLTableEntityHelper.getTableName<T>() ) );
            bool autoIncrementKey = false;
            try {

                var l = SQLTableEntityHelper.getPropertyIfColumnDefinition<T>();

                foreach ( PropertyInfo f in l ) {

                    DDColumnAttribute a = f.GetCustomAttribute<DDColumnAttribute>();

                    if ( a != null ) {
                        string column = ( !string.IsNullOrWhiteSpace( a.Name ) ) ? a.Name : throw new System.Exception( "Column name not present" );

                        SQLColumnDataType dataType = null;
                        if ( a.DataType != null ) {
                            dataType = new SQLColumnDataType( ( MySQLDataType ) a.DataType );
                        } else {
                            throw new System.Exception( "Column type not present" );
                        }

                        string sNotNull = ( a.NullValue == NullValue.NotNull ) ? " NOT NULL" : "";
                        string sUnique = ( a.Unique ) ? " UNIQUE" : "";

                        string sPrimaryKey = "";
                        string sAutoIncrement = "";

                        DDPrimaryKey ddp = f.GetCustomAttribute<DDPrimaryKey>();
                        if ( ddp != null ) {
                            sPrimaryKey = " PRIMARY KEY";
                            if ( ddp.Autoincrement ) {
                                sAutoIncrement = " AUTOINCREMENT";
                                if ( !autoIncrementKey ) {
                                    autoIncrementKey = true;
                                } else {
                                    throw new SQLiteTableException( "Only one autoincrement key is allowed." );
                                }

                                result.Add( "\t" + $"\"{column}\" {dataType.ToString()}{sNotNull}{sUnique}{sPrimaryKey}{sAutoIncrement}".Trim() );
                                //String.Format( "\"%s\" %s%s%s%s%s" , column , dataType.ToString() , sNotNull , sUnique ,sPrimaryKey , sAutoIncrement ).trim() );

                            } else {
                                result.Add( "\t" + $"\"{column}\"  {dataType.ToString()}{sNotNull}{sUnique}{sAutoIncrement}".Trim() );
                                //result.Add( "\t" + String.Format( "\"%s\" %s%s%s%s" , column , dataType.ToString() , sNotNull , sUnique , sAutoIncrement ).trim() );
                            }

                            if ( pk == null ) pk = new List<string>();
                            pk.Add( $"\"{column}\"" );

                        } else {
                            result.Add( "\t" + $"\"{column}\" {dataType.ToString()}{sNotNull}{sUnique}".Trim() );
                            //String.Format( "\"%s\" %s%s%s" , column , dataType.ToString() , sNotNull , sUnique ).Trim() 
                        }

                    }

                }

                if ( pk != null ) {
                    result.Add( String.Format( "PRIMARY KEY( {0} )" , String.Join( "," , pk ) ) );
                }

                sb.Append( String.Join( ",\n" , result ) );

                sb.Append( "\n)" );


            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }


            return sb.ToString();
        }

        public static string sqlDrop<T>( bool ifExists = true ) where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "DROP TABLE " );
            if ( ifExists ) sb.Append( "IF EXISTS " );
            sb.Append( SQLTableEntityHelper.getTableName<T>() );
            return sb.ToString();
        }

        public static string sqlDelete<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder().Append( string.Format( "DELETE FROM {0}" , SQLTableEntityHelper.getTableName<T>() ) );
            return sb.ToString();
        }

        public static string sqlDelete<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : AbstractEntity {

            List<dynamic> pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( aTableInstance );

            outParams ??= new SQLQueryParams( pk.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );

            return String.Format( "DELETE FROM {0} WHERE {1}" , SQLTableEntityHelper.getTableName( aTableInstance ) ,
                                                                new EntityConditionalExpression( LogicOperator.AND , pk.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() ).ToString() );
        }

        public static string sqlInsert<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : AbstractEntity {


            StringBuilder sb = new StringBuilder();

            List<dynamic> columnDefinition = SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , true );

            outParams ??= new SQLQueryParams( columnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );

            sb.Append( "INSERT INTO " );
            sb.Append( SQLTableEntityHelper.getTableName( aTableInstance ) );
            sb.Append( string.Concat( " ( " , string.Join( "," , columnDefinition.Select( x => ( string ) x.ColumnName ).ToList() ) , " ) " ) );
            sb.Append( " VALUES " );
            sb.Append( string.Concat( " ( " , string.Join( "," , outParams.asArray().Select( x => x.AsQueryParam( aParamPlaceholder ) ) ) , " ) " ) );

            return sb.ToString();

        }

        public static string sqlUpdate<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : AbstractEntity {

            List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( aTableInstance );
            List<dynamic> columnDefinition = SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , false );

            outParams ??= new SQLQueryParams( columnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList().Union( pkColumnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList() ).ToArray() );

            return String.Format( "UPDATE {0} SET {1} WHERE {2}" ,
                SQLTableEntityHelper.getTableName( aTableInstance ) ,
                new EntityListExpression( columnDefinition.Select( x => FactoryEntityConstraint.MakeAssignement( x , aParamPlaceholder ) ).ToArray() ).ToString() ,
                new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() ).ToString() );
        }


        //public static string sqlSelect<T>( ref SQLQueryParams outParams , params dynamic[] Constraint ) where T : AbstractEntity {
        public static string sqlSelect<T>( ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null ) where T : AbstractEntity {


            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );

            String tableName = SQLTableEntityHelper.getTableName<T>();

            if ( Constraint?.Length > 0 ) {

                EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );

                outParams ??= new SQLQueryParams( expr.getParam() );

                return string.Format(
@"SELECT
    {0}
FROM
    {1}
WHERE
    {2}" ,
                    String.Join( "," , columnsName ) ,
                    tableName ,
                    expr.ToString()
                );

            } else {
                return String.Format( "SELECT\n\t{0}\nFROM\n\t{1}" , String.Join( "," , columnsName ) , tableName );
            }

        }

        public static string sqlSelect<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) where T : AbstractEntity {

            List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( aTableInstance );
            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );

            EntityConditionalExpression constraintExpression = null;
            if ( Constraint?.Length > 0 ) {

                constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

                new EntityConditionalExpression( LogicOperator.AND ,
                     new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() ) ,
                     new EntityConditionalExpression( LogicOperator.AND , Constraint )
                    ) );

            } else {
                constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() );
            }

            outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            return string.Format(
@"SELECT
    {0}
FROM
    {1}
WHERE
    {2}" ,
                string.Join( "," , SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , true ).Select( x => ( string ) x.ToString() ).ToList() ) ,
                                   SQLTableEntityHelper.getTableName<T>() ,
                                   constraintExpression.ToString()
                );

        }




        public static string sqlMaxId<T>() where T : AbstractEntity {
            throw new NotImplementedException();
        }


        public static string sqlastInsertId<T>() where T : AbstractEntity {
            return "SELECT LAST_INSERT_ID()";
        }

        public static String sqlSelect<T>( T aQueryInstance ) where T : SQLQuery {
            throw new NotImplementedException();
        }

        public static SQLQuery SELECT( SQLQuery aQuery ) {
            throw new NotImplementedException();
        }

    }

}
