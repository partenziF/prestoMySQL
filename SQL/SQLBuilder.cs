using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
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

        public static string sqlCreate<T>( bool ifNotExists = true ) where T : GenericEntity {
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

                        string sNotNull = ( a.NotNull ) ? " NOT NULL" : "";
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

        public static string sqlDrop<T>( bool ifExists = true ) where T : GenericEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "DROP TABLE " );
            if ( ifExists ) sb.Append( "IF EXISTS " );
            sb.Append( SQLTableEntityHelper.getTableName<T>() );
            return sb.ToString();
        }

        public static string sqlDelete<T>() where T : GenericEntity {
            StringBuilder sb = new StringBuilder().Append( string.Format( "DELETE FROM {0}" , SQLTableEntityHelper.getTableName<T>() ) );
            return sb.ToString();
        }

        public static string sqlDelete<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : GenericEntity {

            outParams ??= new SQLQueryParams();
            StringBuilder sb = new StringBuilder();

            List<dynamic> pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn<T>( aTableInstance );
            String[] pkc = new String[pk.Count];

            if ( outParams != null ) {
                outParams.setCapacity( pk.Count );
            }

            int i = 0;
            foreach ( dynamic o in pk ) {

                outParams[i++] = ( MySQLQueryParam ) o;

            }

            sb.Append( String.Format( "DELETE FROM {0} WHERE {1}" ,
                    SQLTableEntityHelper.getTableName( aTableInstance ) ,
                    String.Join( "," , outParams.asStrings( aParamPlaceholder ) ) ) );

            return sb.ToString();

        }

        public static string sqlInsert<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : GenericEntity {


            StringBuilder sb = new StringBuilder();
            //List<string> fieldsName = new List<string>(); //= SQLTableEntityHelper.getColumnName<T>( false , false );

            List<dynamic> listDefinitionColumns = SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , true );

            //            outParams.setCapacity( listDefinitionColumns.Count );
            //string[] Params = new string[listDefinitionColumns.Count];
            //int i = 0;
            //foreach ( dynamic DefinitionColumn in listDefinitionColumns ) {
            //    outParams[i++] = ( QueryParam ) ( MySQLQueryParam ) DefinitionColumn;
            //}
            //var xxx = listDefinitionColumns.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray();

            outParams ??= new SQLQueryParams( listDefinitionColumns.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );


            //QueryParam queryParam = ( MySQLQueryParam ) DefinitionColumn;  // cast definition column into query param
            //String.Concat( DefinitionColumn , "=" , ( QueryParam ) ( MySQLQueryParam ) DefinitionColumn.AsQueryParam( aParamPlaceholder ) );
            //outParams[i] = queryParam;
            //fieldsName.Add( DefinitionColumn );
            //  i++;
            //outParams[i] = (MySQLQueryParam) o;
            //Params[i] = ( ( outParams != null ) && ( !String.IsNullOrWhiteSpace( aParamPlaceholder ) ) ) ? aParamPlaceholder : outParams[i].ToString();
            //}
            //sb.Append( string.Format( "INSERT INTO  {0} ( {1} ) VALUES ({2})" ,
            //            SQLTableEntityHelper.getTableName( aTableInstance ) ,
            //            string.Join( "," , listDefinitionColumns.Select( x => ( string ) x.ToString() ).ToList() ) ,
            //            string.Join( "," , outParams.asArray().Select( x => x.AsQueryParam(aParamPlaceholder) ) ) ));

            sb.Append( "INSERT INTO " );
            sb.Append( SQLTableEntityHelper.getTableName( aTableInstance ) );
            sb.Append( string.Concat( " ( " , string.Join( "," , listDefinitionColumns.Select( x => ( string ) x.ColumnName ).ToList() ) , " ) " ) );
            sb.Append( " VALUES " );
            sb.Append( string.Concat( " ( " , string.Join( "," , outParams.asArray().Select( x => x.AsQueryParam( aParamPlaceholder ) ) ) , " ) " ) );

            return sb.ToString();

        }

        public static string sqlUpdate<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : GenericEntity {

            //outParams ??= new MySQLQueryParams();
            StringBuilder sb = new StringBuilder();

            List<dynamic> pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn<T>( aTableInstance );
            List<dynamic> p = SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , false );

            outParams ??= new SQLQueryParams( p.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList().Union( pk.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList() ).ToArray() );

            //String[] pkc = new String[pk.Count];
            //String[] c = new String[p.Count];

            //outParams.setCapacity( pkc.Length + c.Length );

            //int i = 0;
            //int k = 0;
            //foreach ( dynamic o in p ) {
            //    outParams[k++] = ( MySQLQueryParam ) o;
            //}
            //int j = k;
            //foreach ( dynamic o in pk ) {
            //    outParams[k++] = ( MySQLQueryParam ) o;
            //}

            //Array.Copy( outParams.asStrings( aParamPlaceholder ) , c , j );
            //Array.Copy( outParams.asStrings( aParamPlaceholder ) , j , pkc , 0 , k - j );

            sb.Append( String.Format( "UPDATE {0} SET {1} WHERE {2}" ,
                SQLTableEntityHelper.getTableName( aTableInstance ) ,
                String.Join( ", " , p.Select( x => String.Concat( ( string ) x.ColumnName , " = " , ( ( MySQLQueryParam ) x ).AsQueryParam() ) ).ToList() ) ,
                String.Join( " AND " , pk.Select( x => String.Concat( x.ToString() , " = " , ( ( MySQLQueryParam ) x ).AsQueryParam() ) ).ToArray() ) )
            );

            return sb.ToString();

        }



        public static string sqlSelect<T>( ref SQLQueryParams outParams , params dynamic[] definableConstraint ) where T : GenericEntity {


            StringBuilder sb = new StringBuilder();
            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );
            String tableName = SQLTableEntityHelper.getTableName<T>();

            //int valuesLength = 0;

            //string[] selectionArray;

            //if ( definableConstraint != null ) {

            //    foreach ( dynamic c in definableConstraint ) {
            //        valuesLength += c.countParam();
            //    }

            //    selectionArray = new String[definableConstraint.Length];

            //} else {
            //    selectionArray = new String[] { };
            //}

            //outParams.setCapacity( valuesLength );

            //int index = 0;
            //if ( valuesLength > 0 ) {
            //    for ( int i = 0; i < definableConstraint.Length; i++ ) {
            //        selectionArray[i] = definableConstraint[i].ToString();
            //        if ( outParams != null ) {
            //            QueryParam[] v = definableConstraint[i].getParam();
            //            for ( int j = 0; j < definableConstraint[i].countParam(); j++ ) {
            //                outParams[index++] = v[j];
            //            }
            //        }
            //    }
            //}


            //if ( selectionArray.Length > 0 ) {
            if ( definableConstraint != null ) {

                GenericEntityConstraintExpression expr = new GenericEntityConstraintExpression( LogicOperator.AND , definableConstraint );
                outParams ??= new SQLQueryParams( expr.getParam() );

                sb.Append( string.Format(
@"SELECT
    {0}
FROM
    {1}
WHERE
    {2}" ,
                    String.Join( "," , columnsName ) ,
                    tableName ,
                    //string.Join( " AND " , selectionArray ) 
                    expr.ToString()
                    ) );
            } else {
                sb.Append( String.Format( "SELECT\n\t{0}\nFROM\n\t{1}" , String.Join( "," , columnsName ) , tableName ) );
            }

            return sb.ToString();

        }

        public static string sqlSelect<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" , params DefinableConstraint<object>[] values ) where T : GenericEntity {


            StringBuilder sb = new StringBuilder();

            List<dynamic> pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn<T>( aTableInstance );

            outParams ??= new SQLQueryParams( pk.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );

            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( false , false );
            //String tableName = SQLTableEntityHelper.getTableName<T>();

            //            dynamic[] pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn<T>( aTableInstance ).ToArray();// .toArray( new DefinableColumn<?>[0] )
            //int delta = pk.Length;
            //int valuesLength = 0;
            //if ( ( values != null ) && ( values.Length > 0 ) ) {
            //    foreach ( DefinableConstraint<object> c in values ) {
            //        valuesLength += c.countParam();
            //    }
            //}

            //String[] selectionArray;
            //selectionArray = new String[delta + values?.Length ?? 0];

            //            outParams.setCapacity( selectionArray.Length );

            //int index = 0;
            //for ( int i = 0; i < pk.Length; i++ ) {

            //    var o = Convert.ChangeType( pk[i] , ( pk[i].GetType() ) );
            //    outParams[index++] = ( MySQLQueryParam ) o;
            //}


            //if ( valuesLength > 0 ) {

            //    for ( int i = 0; i < values.Length; i++ ) {

            //        if ( !String.IsNullOrEmpty( aParamPlaceholder ) ) {
            //            ( ( GenericEntityConstraint<T> ) values[i] ).ParamPlaceHolder = aParamPlaceholder;
            //        }

            //        selectionArray[i + delta] = ( ( GenericEntityConstraint<T> ) values[i] ).ToString();

            //        if ( outParams != null ) {
            //            object[] v = ( ( GenericEntityConstraint<T> ) values[i] ).getParam();

            //            for ( int j = 0; j < ( ( GenericEntityConstraint<T> ) values[i] ).countParam(); j++ ) {
            //                outParams[index++] = v[j];
            //            }

            //        }

            //    }

            //}

            sb.Append(
                string.Format(
@"SELECT
    {0}
FROM
    {1}
WHERE
    {2}" ,
                string.Join( "," , SQLTableEntityHelper.getDefinitionColumn<T>( aTableInstance , true ).Select( x => ( string ) x.ToString() ).ToList() ) ,
                SQLTableEntityHelper.getTableName<T>() ,
                string.Join( " AND " , String.Join( " AND " , pk.Select( x => String.Concat( x.ToString() , " = " , ( ( MySQLQueryParam ) x ).AsQueryParam() ) ).ToArray() ) ) )
                );


            return sb.ToString();
        }




        public static string sqlMaxId<T>() where T : GenericEntity {
            throw new NotImplementedException();
        }


        public static String sqlSelect<T>( T aQueryInstance ) where T : SQLQuery {
            throw new NotImplementedException();
        }

        public static SQLQuery SELECT( SQLQuery aQuery ) {
            throw new NotImplementedException();
        }
    }
}
