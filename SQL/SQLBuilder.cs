using DatabaseEntity.EntityAdapter;
using prestoMySQL.Adapter;
using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Exception;
using prestoMySQL.ForeignKey;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace prestoMySQL.SQL {

    public static class SQLConstant {
        public static char COLUMN_NAME_QUALIFIER = '`';
        public static char TABLE_NAME_QUALIFIER = '`';
        public static char TABLE_PARAM_STRING_QUALIFIER = '"';
    }
    public static class SQLBuilder {

        public static string sqlCreate<T>( bool ifNotExists = true ) where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();

            List<String> pk = null;
            List<String> result = new List<String>();

            sb.Append( $"CREATE TABLE {( ifNotExists ? "IF NOT EXISTS" : "" )} {prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER}{SQLTableEntityHelper.getTableName<T>()}{prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER} (\n" );
            bool autoIncrementKey = false;
            try {

                var l = SQLTableEntityHelper.getPropertyIfColumnDefinition<T>();

                var IndexKey = new Dictionary<string , List<string>>();
                var UniqueKey = new Dictionary<string , List<string>>();

                foreach ( PropertyInfo f in l ) {

                    DDColumnAttribute a = f.GetCustomAttribute<DDColumnAttribute>();

                    if ( a != null ) {

                        result.Add( a.ToString() );

                        string column = a.BuildColumnName();//  ( !string.IsNullOrWhiteSpace( a.Name ) ) ? a.Name : throw new System.Exception( "Column name not present" );

                        DDPrimaryKey ddp = f.GetCustomAttribute<DDPrimaryKey>();
                        if ( ddp != null ) {

                            if ( ddp.Autoincrement ) {

                                if ( !autoIncrementKey ) {
                                    autoIncrementKey = true;
                                } else {
                                    throw new SQLiteTableException( "Only one autoincrement key is allowed." );
                                }
                            }

                            if ( pk == null ) pk = new List<string>();
                            pk.Add( column );

                        }


                        DDIndexAttribute ddia = f.GetCustomAttribute<DDIndexAttribute>();
                        if ( ddia != null ) {

                            if ( !IndexKey.ContainsKey( ddia.Name ) ) {
                                IndexKey.Add( ddia.Name , new List<string> {
                                    column
                                } );
                            } else {
                                IndexKey[ddia.Name].Add( column );

                            }

                        }


                        DDUniqueIndexAttribute ddua = f.GetCustomAttribute<DDUniqueIndexAttribute>();
                        if ( ddua != null ) {

                            if ( !UniqueKey.ContainsKey( ddua.Name ) ) {
                                UniqueKey.Add( ddua.Name , new List<string> {
                                    column
                                } );
                            } else {
                                UniqueKey[ddua.Name].Add( column );

                            }
                        }


                    }

                }

                if ( pk != null ) {
                    result.Add( String.Format( "PRIMARY KEY( {0} )" , String.Join( "," , pk ) ) );
                }

                if ( UniqueKey?.Count > 0 ) {
                    foreach ( var k in UniqueKey.Keys ) {
                        result.Add( String.Format( "UNIQUE INDEX " + SQLConstant.COLUMN_NAME_QUALIFIER + "{0}" + SQLConstant.COLUMN_NAME_QUALIFIER + " ( {1} )" , k , String.Join( "," , UniqueKey[k] ) ) );
                    }
                }


                if ( IndexKey?.Count > 0 ) {
                    foreach ( var k in UniqueKey.Keys ) {
                        result.Add( String.Format( "INDEX " + SQLConstant.COLUMN_NAME_QUALIFIER + "{0}" + SQLConstant.COLUMN_NAME_QUALIFIER + " ( {1} )" , k , String.Join( "," , UniqueKey[k] ) ) );
                    }
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
            sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlExistsTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "SHOW TABLES LIKE " );
            sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_PARAM_STRING_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_PARAM_STRING_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlDescribeTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "DESCRIBE " );
            sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER );
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

            List<dynamic> columnDefinition = SQLTableEntityHelper.getDefinitionColumn( aTableInstance , true );

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
            List<dynamic> columnDefinition = SQLTableEntityHelper.getDefinitionColumn( aTableInstance , false );

            outParams ??= new SQLQueryParams( columnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList().Union( pkColumnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToList() ).ToArray() );

            return String.Format( "UPDATE {0} SET {1} WHERE {2}" ,
                SQLTableEntityHelper.getTableName( aTableInstance ) ,
                new EntityListExpression( columnDefinition.Select( x => FactoryEntityConstraint.MakeAssignement( x , aParamPlaceholder ) ).ToArray() ).ToString() ,
                new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() ).ToString() );
        }


        public static List<EntityForeignKey> GetAllForeignkey( AbstractEntity EntityInstance ) {

            List<EntityForeignKey> fks = new List<EntityForeignKey>();

            var fieldInfoForeignKey = ReflectionTypeHelper.FieldsWhereIsAssignableFrom<EntityForeignKey>( EntityInstance.GetType() );

            foreach ( var fk in fieldInfoForeignKey ) {

                EntityForeignKey o;
                o = ( EntityForeignKey ) fk.GetValue( EntityInstance );
                if ( o == null ) {
                    ReflectionTypeHelper.InstantiateDeclaredClassToField( EntityInstance ,
                                                                          fk ,
                                                                          new Type[] { typeof( AbstractEntity ) , typeof( string ) } ,
                                                                          new object[] { EntityInstance , fk.Name } );
                    o = ( EntityForeignKey ) fk.GetValue( EntityInstance );


                }
                fks.Add( o );
                //fks.Push( o );
                //joins.Add( o.ToString() );
                //List<String> joinTableColumnsNames = SQLTableEntityHelper.getColumnName( o.TypeRefenceTable , true , false );

            }

            return fks;
        }

        public static string sqlSelect<T>( ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null ) where T : AbstractEntity {

            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );
            List<string> joins = new List<string>();

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            sb.AppendLine( string.Join( ',' , columnsName ) );
            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getTableName<T>() );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            if ( Constraint?.Length > 0 ) {

                sb.AppendLine( "WHERE" );

                EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
                outParams ??= new SQLQueryParams( expr.getParam() );

                sb.AppendLine( expr.ToString() );
            }

            return sb.ToString();
        }
        //Add Join


        public static string sqlSelect<T>( T Entities , ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null ) where T : EntitiesAdapter {

            var tables = Entities._Graph.GetTopologicalOrder();

            List<string> columnsName = new List<string>();
            List<string> joins = new List<string>();

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            foreach ( var e in tables ) {
                columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            }

            joins.AddRange( Entities.GetForeignKeys().Select( fk => fk.ToString() ).ToList() );

            sb.AppendLine( string.Join( "," , columnsName ) );

            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getTableName( tables.First().GetType() ) );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            if ( Constraint?.Length > 0 ) {

                sb.AppendLine( "WHERE" );

                EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
                outParams ??= new SQLQueryParams( expr.getParam() );

                sb.AppendLine( expr.ToString() );
            }

            return sb.ToString();

        }
        public static string sqlSelect( EntitiesAdapter Entities , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) {

            var tables = Entities._Graph.GetTopologicalOrder();

            List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( tables.First() );
            List<string> columnsName = new List<string>();
            List<string> joins = new List<string>();

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            foreach ( var e in tables ) {
                columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            }

            joins.AddRange( Entities.GetForeignKeys().Select( fk => fk.ToString() ).ToList() );

            sb.AppendLine( string.Join( "," , columnsName ) );

            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getTableName( tables.First().GetType() ) );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            EntityConditionalExpression constraintExpression = null;

            if ( Constraint?.Length > 0 ) {

                constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

                new EntityConditionalExpression( LogicOperator.AND ,
                     new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) ,
                     new EntityConditionalExpression( LogicOperator.AND , Constraint )
                    ) );

            } else {
                constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() );
            }

            outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            sb.AppendLine( "WHERE" );
            sb.AppendLine( constraintExpression.ToString() );


            return sb.ToString();
        }


        public static string sqlSelect( AbstractEntity EntityInstance , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) {

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( EntityInstance );
            List<string> columnsName = SQLTableEntityHelper.getDefinitionColumn( EntityInstance , true ).Select( x => ( string ) x.ToString() ).ToList();
            List<string> joins = new List<string>();

            sb.AppendLine( string.Join( "," , columnsName ) );

            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getTableName( EntityInstance.GetType() ) );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            EntityConditionalExpression constraintExpression = null;

            if ( Constraint?.Length > 0 ) {

                constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

                new EntityConditionalExpression( LogicOperator.AND ,
                     new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) ,
                     new EntityConditionalExpression( LogicOperator.AND , Constraint )
                    ) );

            } else {
                constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() );
            }

            outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            sb.AppendLine( "WHERE" );
            sb.AppendLine( constraintExpression.ToString() );


            return sb.ToString();
        }

        public static string sqlMaxId<T>() where T : AbstractEntity {
            throw new NotImplementedException();
        }

        public static string sqlastInsertId<T>() where T : AbstractEntity {
            return "SELECT LAST_INSERT_ID()";
        }



        public static X sqlQuery<T, X>( T Entities , X myQuery ) where T : EntitiesAdapter where X : SQLQuery { //ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null 

            //myQuery.Initialize();
            //myQuery.SelectExpression = SQLTableEntityHelper.getProjectionColumnName<X>( myQuery );

            ////var tables = Entities._Graph.GetTopologicalOrder();

            ////List<string> columnsName = new List<string>();
            ////List<string> joins = new List<string>();

            ////StringBuilder sb = new StringBuilder( "SELECT" );

            ////foreach ( var e in tables ) {
            ////    columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            ////}

            //List<DefinableConstraint> constraints = new List<DefinableConstraint>();
            //var queryConstraints = SQLTableEntityHelper.getQueryJoinConstraint( myQuery.GetType() );
            //foreach ( fk in myQuery.Graph.GetForeignKeys() ) {
            //    var constraint = queryConstraints.Where( x => x.Entity == e.GetType() );

            //    //.Where( x => ( ( ( DALQueryJoinEntityConstraint ) x ).Entity == e.GetType())   ).ToList()
            //    foreach ( DALQueryJoinEntityConstraint a in constraint ) {

            //        var fieldName = SQLTableEntityHelper.getPropertyIfColumnDefinition( e.GetType() ).FirstOrDefault( x => x.Name == a.FieldName );
            //        //MySQLDefinitionColumn( string aDeclaredVariableName , PropertyInfo aMethodBase , AbstractEntity abstractEntity )
            //        var ctor = fieldName.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( AbstractEntity ) } );


            //        var instance = ctor?.Invoke( new object[] { fieldName.Name , fieldName , e } );

            //        if ( a.ParamName != null ) {
            //            constraints.Add( FactorySQLWhereCondition.MakeColumnEqual( fieldName , myQuery[a.ParamName] , a.Placeholder ) );
            //        } else {
            //            //constraints.Add( FactorySQLWhereCondition.MakeColumnEqual( instance , a.ParamValue , a.Placeholder ) );
            //            constraints.Add( FactoryEntityConstraint.MakeEqual( instance , a.ParamValue , "@" ) );
            //            //FactorySQLWhereCondition.MakeColumnEqual( fieldName , a.ParamValue , a.Placeholder );
            //        }
            //        //a.FieldName
            //        //FactorySQLWhereCondition.MakeColumnEqual( Delete , this[nameof( pDelete )] , "@" ) ,
            //    }

            //    //new SQLQueryConditionExpression(
            //    //                    LogicOperator.AND ,
            //    //                    FactorySQLWhereCondition.MakeColumnEqual( Delete , this[nameof( pDelete )] , "@" ) ,
            //    //                    FactorySQLWhereCondition.MakeColumnEqual( FkProvincia , this[nameof( pFkProvincia )] , "@" )
            //    //                    )

            //    if ( constraints.Count > 0 ) {
            //        myQuery.JOIN( e , fk , new SQLQueryConditionExpression( LogicOperator.AND , constraints.ToArray() ) );
            //    } else {
            //        myQuery.JOIN( e , fk );
            //    }

            //}



            ////if ( Constraint?.Length > 0 ) {

            ////    sb.AppendLine( "WHERE" );

            ////    EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
            ////    outParams ??= new SQLQueryParams( expr.getParam() );

            ////    sb.AppendLine( expr.ToString() );
            ////}
            ////
            return myQuery;

        }



        public static String sqlQuery<X>( X aQueryInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where X : SQLQuery {

            //Count total params
            outParams ??= new SQLQueryParams( ( ( SQLQuery ) aQueryInstance ).getParam );

            //    return aQueryInstance.ToString();ù

            StringBuilder sb = new StringBuilder( "SELECT " );

            try {

                sb.AppendLine( String.Join( ',' , aQueryInstance.SelectExpression ) );
                sb.AppendLine( "FROM" );
                sb.AppendLine( String.Join( ',' , aQueryInstance.TablesReferences ) );

                foreach ( var jt in aQueryInstance.JoinTable.Values ) {

                    sb.AppendLine( jt.ToString() );

                }


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


                    sb.AppendLine( "WHERE" );
                    sb.AppendLine( String.Format( "\t( {0} )" , String.Join( " AND " , where.ToArray() ) ) );
                }



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
                    sb.AppendLine( String.Format( $"LIMIT {aQueryInstance.RowCount} OFFSET {aQueryInstance.Offset}" ) );
                } else if ( ( aQueryInstance.Offset is null ) && ( aQueryInstance.RowCount is not null ) ) {
                    sb.AppendLine( String.Format( $"LIMIT {aQueryInstance.RowCount}" ) );
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
            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();

            var queryConstraints = SQLTableEntityHelper.getQueryJoinConstraint( myQuery.GetType() );
            bool isReversed = false;
            foreach ( var fk in myQuery.Graph.GetForeignKeys() ) {

                List<DefinableConstraint> constraints = new List<DefinableConstraint>();
                List<DALQueryJoinEntityConstraint> constraint;
                isReversed = ( myQuery.TablesReferences.Contains( fk.Table.TableName ) || ( visited.Contains( fk.Table ) ) );
                
                constraint = isReversed
                    ? queryConstraints.Where( x => x.Entity == fk.RefenceTable.GetType() ).ToList()
                    : queryConstraints.Where( x => x.Entity == fk.Table.GetType() ).ToList();

                foreach ( DALQueryJoinEntityConstraint a in constraint ) {
                    dynamic instance = isReversed
                        ? SQLTableEntityHelper.getDefinitionColumn( fk.RefenceTable , true ).FirstOrDefault( x => x.ColumnName == a.FieldName )
                        : SQLTableEntityHelper.getDefinitionColumn( fk.Table , true ).FirstOrDefault( x => x.ColumnName == a.FieldName );
                    
                    if ( instance is null ) throw new ArgumentNullException( "invalid fieldname " + a.FieldName );


                    dynamic c = a.ParamName != null
                        ? FactoryEntityConstraint.MakeEqual( instance , ( MySQLQueryParam ) myQuery[a.ParamName] , a.Placeholder )
                        : FactoryEntityConstraint.MakeEqual( instance , a.ParamValue , a.Placeholder );

                    constraints.Add( myQuery.MakeUniqueParamName( c ) );

                }

                if ( constraints.Count > 0 ) {
                    myQuery.JOIN( isReversed , fk , new SQLQueryConditionExpression( LogicOperator.AND , constraints.ToArray() ) );
                } else {
                    myQuery.JOIN( isReversed , fk );
                }

                if ( isReversed ) {
                    visited.Push( fk.RefenceTable );
                } else {
                    visited.Push( fk.Table );
                }


            }


            //myQuery.setOrderBy( f );
            //myQuery.setGroupBy( f );

            return myQuery;
        }

    }

}
