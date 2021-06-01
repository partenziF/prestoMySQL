using prestoMySQL.Column;
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

    public static class Constant {
        public static char COLUMN_NAME_QUALIFIER = '`';
        public static char TABLE_NAME_QUALIFIER = '`';
        public static char TABLE_PARAM_STRING_QUALIFIER = '"';
    }
    public static class SQLBuilder {

        public static string sqlCreate<T>( bool ifNotExists = true ) where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();

            List<String> pk = null;
            List<String> result = new List<String>();

            sb.Append( $"CREATE TABLE {( ifNotExists ? "IF NOT EXISTS" : "" )} {prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER}{SQLTableEntityHelper.getTableName<T>()}{prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER} (\n" );
            bool autoIncrementKey = false;
            try {

                var l = SQLTableEntityHelper.getPropertyIfColumnDefinition<T>();

                foreach ( PropertyInfo f in l ) {

                    DDColumnAttribute a = f.GetCustomAttribute<DDColumnAttribute>();

                    if ( a != null ) {

                        //var s = a.ToString();

                        result.Add( a.ToString() );

                        string column = ( !string.IsNullOrWhiteSpace( a.Name ) ) ? a.Name : throw new System.Exception( "Column name not present" );

                        //SQLColumnDataType dataType = null;
                        //if ( a.DataType != null ) {
                        //    dataType = new SQLColumnDataType( ( MySQLDataType ) a.DataType );
                        //} else {
                        //    throw new System.Exception( "Column type not present" );
                        //}) ? " NOT NULL" : "";
                        //string sUnique = ( a.Unique ) ? " UNIQUE" : "";

                        string sPrimaryKey = "";
                        string sAutoIncrement = "";

                        DDPrimaryKey ddp = f.GetCustomAttribute<DDPrimaryKey>();
                        if ( ddp != null ) {

                            //string sNotNull = ( a.NullValue == NullValue.NotNull 
                            sPrimaryKey = " PRIMARY KEY";
                            if ( ddp.Autoincrement ) {
                                sAutoIncrement = " AUTOINCREMENT";
                                if ( !autoIncrementKey ) {
                                    autoIncrementKey = true;
                                } else {
                                    throw new SQLiteTableException( "Only one autoincrement key is allowed." );
                                }

                                //result.Add( "\t" + $"\"{column}\" {dataType.ToString()}{sNotNull}{sUnique}{sPrimaryKey}{sAutoIncrement}".Trim() );
                                ////String.Format( "\"%s\" %s%s%s%s%s" , column , dataType.ToString() , sNotNull , sUnique ,sPrimaryKey , sAutoIncrement ).trim() );

                            } else {
                                //result.Add( "\t" + $"\"{column}\"  {dataType.ToString()}{sNotNull}{sUnique}{sAutoIncrement}".Trim() );
                                ////result.Add( "\t" + String.Format( "\"%s\" %s%s%s%s" , column , dataType.ToString() , sNotNull , sUnique , sAutoIncrement ).trim() );
                            }

                            if ( pk == null ) pk = new List<string>();
                            pk.Add( $"`{column}`" );

                        } else {
                            //result.Add( "\t" + $"\"{column}\" {dataType.ToString()}{sNotNull}{sUnique}".Trim() );
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

        public static string sqlTruncate<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "TRUNCATE TABLE " );
            sb.Append( prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlExistsTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "SHOW TABLES LIKE " );
            sb.Append( prestoMySQL.SQL.Constant.TABLE_PARAM_STRING_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.Constant.TABLE_PARAM_STRING_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlDescribeTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "DESCRIBE " );
            sb.Append( prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.Constant.TABLE_NAME_QUALIFIER );
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
            sb.Append( string.Concat( " ( " , string.Join( "," , columnDefinition.Select( x => String.Concat( '`' , ( string ) x.ColumnName , '`' ) ).ToList() ) , " ) " ) );
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


        public static String sqlQuery<X>( X aQueryInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where X : SQLQuery {

            //Count total params
            outParams ??= new SQLQueryParams( ( ( SQLQuery ) aQueryInstance ).getParam );

            //    return aQueryInstance.ToString();ù

            StringBuilder sb = new StringBuilder();

            try {


                sb.Append( String.Format( "SELECT\r\n\t{0} " , String.Join( ',' , aQueryInstance.SelectExpression ) ) );
                sb.Append( String.Format( "\r\nFROM\r\n\t{0} " , String.Join( ',' , aQueryInstance.TablesReferences ) ) );

                //    if ( !mJoinTable.isEmpty() ) {
                //        String[] j = new String[mJoinTable.size()];
                //        int i = 0;
                //        for ( SQLQueryJoinTable jt : mJoinTable ) {
                //            j[i++] = jt.toString();
                //        }
                //        sb.append( String.format( "\r\n %s " , String.join( "\r\n\t" , j ) ) );

                //    }


                //SQLQueryConditionExpression

                /*            
                 *          EntityConditionalExpression constraintExpression = null;
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
                */

                List<string> where = new List<string>();

                if ( aQueryInstance.WhereCondition.Count > 0 ) {

                    aQueryInstance.WhereCondition.ForEach( x => where.Add( x.ToString() ) );

                    //int i = 0;
                    //foreach( SQLQueryConditionExpression sc in aQueryInstance.WhereCondition) {
                    //    i += sc.countParam();

                    //    var p = sc.getParam();

                    //    var c = sc.ToString();

                    //}



                    //EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );

                    //outParams ??= new SQLQueryParams( expr.getParam() );


                    //String[] c = new string[aQueryInstance.WhereCondition.Count];
                    //int i = 0;
                    //foreach ( SQLQueryConditionExpression sc in aQueryInstance.WhereCondition ) {
                    //    c[i++] = sc.ToString();
                    //}

                    //sb.Append( String.Format( "\r\nWHERE\r\n\t( {0} )" , String.Join( " AND " , c ) ) );

                }

                sb.Append( String.Format( "\r\nWHERE\r\n\t( {0} )" , String.Join( " AND " , where.ToArray() ) ) );

                //    if ( !mGroupBy.isEmpty() ) {
                //        String[] c = new String[mGroupBy.size()];
                //        int i = 0;
                //        for ( SQLQueryGroupBy sc : mGroupBy ) {
                //            c[i++] = sc.toString();
                //        }

                //        sb.append( String.format( "\r\nGROUP BY\r\n\t%s " , String.join( "," , c ) ) );

                //    }

                //    if ( !mOrderBy.isEmpty() ) {
                //        String[] c = new String[mOrderBy.size()];
                //        int i = 0;
                //        for ( SQLQueryOrderBy sc : mOrderBy ) {
                //            c[i++] = sc.toString();
                //        }

                //        sb.append( String.format( "\r\nORDER BY\r\n\t%s " , String.join( "," , c ) ) );
                //    }

                if ( ( aQueryInstance.Offset is not null ) && ( aQueryInstance.RowCount is not null ) ) {
                    sb.Append( String.Format( $"LIMIT {aQueryInstance.RowCount} OFFSET {aQueryInstance.Offset}" ) );
                } else if ( ( aQueryInstance.Offset is null ) && ( aQueryInstance.RowCount is not null ) ) {
                    sb.Append( String.Format( $"LIMIT {aQueryInstance.RowCount}" ) );
                } else if ( ( aQueryInstance.Offset is not null ) && ( aQueryInstance.RowCount is null ) ) {
                    throw new ArgumentException( "Invalid argument RowCount can't be null." );
                }

            } catch ( System.Exception e ) {
                //    // TODO Auto-generated catch block
                //    e.printStackTrace();
            }
            return sb.ToString();


        }


        public static SQLQuery SELECT<T>( T myQuery ) where T : SQLQuery {

            myQuery.Initialize();
            myQuery.SelectExpression = SQLTableEntityHelper.getProjectionColumnName<T>( myQuery );

            //List<Field> f = SQLTableEntityHelper.getProjectionFields( myQuery.getClass() );
            //myQuery.setJoinTables( f );

            ////Auto Join tables 
            //ArrayList<DALQueryEntity> entityQuery = SQLTableEntityHelper.getQueryEntity( myQuery.getClass() );
            //ArrayList<DALQueryJoinEntity> entityJoin = SQLTableEntityHelper.getQueryJoinEntity( myQuery.getClass() );

            //for ( DALQueryJoinEntity a : entityJoin ) {
            //    myQuery.createJoin( a , entityQuery.get( 0 ) );
            //}


            //for ( int i = 0; i < entityJoin.size(); i++ )
            //    for ( int j = i + 1; j < entityJoin.size(); j++ )
            //        myQuery.createJoin( entityJoin.get( i ) , entityJoin.get( j ) );

            ////end 

            //myQuery.setOrderBy( f );
            //myQuery.setGroupBy( f );

            return myQuery;
        }

    }

}
