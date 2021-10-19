using DatabaseEntity.EntityAdapter;
using prestoMySQL.Adapter;
using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Exception;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.Helper;
using prestoMySQL.Index;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.Table;
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

            sb.Append( $"CREATE TABLE {( ifNotExists ? "IF NOT EXISTS" : "" )} {prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER}{SQLTableEntityHelper.getAttributeTableName<T>()}{prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER} (\n" );
            //sb.Append( $"CREATE TABLE {( ifNotExists ? "IF NOT EXISTS" : "" )} {prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER}{SQLTableEntityHelper.getTableName<T>()}{prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER} (\n" );
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
            sb.Append( SQLTableEntityHelper.getAttributeTableName<T>() );
            return sb.ToString();
        }

        public static string sqlTruncate<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "TRUNCATE TABLE " );
            sb.Append( SQLTableEntityHelper.getAttributeTableName<T>().QuoteTableName() );
            //sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlExistsTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "SHOW TABLES LIKE " );
            sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_PARAM_STRING_QUALIFIER + SQLTableEntityHelper.getAttributeTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_PARAM_STRING_QUALIFIER );
            return sb.ToString();
        }

        public static string sqlDescribeTable<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder();
            sb.Append( "DESCRIBE " );
            sb.Append( SQLTableEntityHelper.getAttributeTableName<T>().QuoteTableName() );
            //sb.Append( prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER + SQLTableEntityHelper.getTableName<T>() + prestoMySQL.SQL.SQLConstant.TABLE_NAME_QUALIFIER );
            return sb.ToString();
        }


        public static string sqlDelete<T>() where T : AbstractEntity {
            StringBuilder sb = new StringBuilder().Append( string.Format( "DELETE FROM {0}" , SQLTableEntityHelper.getAttributeTableName<T>() ) );
            return sb.ToString();
        }

        public static string sqlDelete<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : AbstractEntity {

            List<dynamic> pk = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( aTableInstance );

            outParams ??= new SQLQueryParams( pk.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );

            return String.Format( "DELETE FROM {0} WHERE {1}" , SQLTableEntityHelper.getAttributeTableName( aTableInstance ) ,
                                                                new EntityConditionalExpression( LogicOperator.AND , pk.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , aParamPlaceholder ) ).ToArray() ).ToString() );
        }

        public static string sqlInsert<T>( T aTableInstance , ref SQLQueryParams outParams , string aParamPlaceholder = "" ) where T : AbstractEntity {


            StringBuilder sb = new StringBuilder();

            List<dynamic> columnDefinition = SQLTableEntityHelper.getDefinitionColumn( aTableInstance , true );

            outParams ??= new SQLQueryParams( columnDefinition.Select( x => ( QueryParam ) ( MySQLQueryParam ) x ).ToArray() );

            sb.Append( "INSERT INTO " );
            sb.Append( SQLTableEntityHelper.getAttributeTableName( aTableInstance ) );
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
                SQLTableEntityHelper.getAttributeTableName( aTableInstance ) ,
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
                //List<String> joinTableColumnsNames = SQLTableEntityHelper.getColumnName( o.TypeReferenceTable , true , false );

            }

            return fks;
        }

        public static string sqlSelect<T>( ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null ) where T : AbstractEntity {

            List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );
            List<string> joins = new List<string>();

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            sb.AppendLine( string.Join( ',' , columnsName ) );
            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getAttributeTableName<T>() );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            if ( Constraint?.Length > 0 ) {

                sb.AppendLine( "WHERE" );

                EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
                outParams ??= new SQLQueryParams( expr.getParam() );

                sb.AppendLine( expr.ToString() );
            }

            return sb.ToString();
        }
        public static string sqlSelect<T>( T EntityInstance , ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null ) where T : AbstractEntity {

            //List<String> columnsName = SQLTableEntityHelper.getColumnName<T>( true , false );
            List<string> columnsName = SQLTableEntityHelper.getDefinitionColumn( EntityInstance , true ).Select( x => ( string ) x.ToString() + "\r\n" ).ToList();

            List<string> joins = new List<string>();

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            sb.AppendLine( string.Join( ',' , columnsName ) );
            sb.AppendLine( "FROM" );
            //sb.AppendLine( SQLTableEntityHelper.getTableName<T>() );
            //sb.AppendLine( SQLTableEntityHelper.getAttributeTableName( EntityInstance.GetType() ) );

            sb.AppendLine( ( ( TableReference ) EntityInstance ).ToString() );

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


        public static string sqlSelect<T, X>( Func<T , X> delegateMethod , EntitiesAdapter Entities , ref SQLQueryParams outParams , string ParamPlaceholder = "@" , EntityConditionalExpression Constraint = null ) where X : TableIndex
                                                                                                                                                                                                                   where T : AbstractEntity {
            var tables = Entities.Graph.GetTopologicalOrder();

            AbstractEntity UniqueKeyEntity = tables.FirstOrDefault( x => x.GetType() == typeof( T ) );

            X x = delegateMethod( ( T ) UniqueKeyEntity );

            DefinableConstraint[] constraints = new DefinableConstraint[x.ColumnsName.Length];

            int i = 0;
            foreach ( string c in x.ColumnsName ) {
                PropertyInfo p = x[c];
                var col = p.GetValue( UniqueKeyEntity );
                DefinableConstraint o = FactoryEntityConstraint.MakeConstraintEqual( col , ParamPlaceholder );
                constraints[i++] = o;
            }

            EntityConditionalExpression constr = null;

            if ( Constraint is null )
                constr = new EntityConstraintExpression( constraints );
            else {
                constr = new EntityConditionalExpression( LogicOperator.AND ,
                                     new EntityConstraintExpression( constraints ) ,
                                     Constraint );
            }


            //var startEntity = tables.First( e => e.GetType() == typeof( T ) );
            var startEntity = tables.FirstOrDefault();
            if ( startEntity is null ) throw new ArgumentException( "Invalid entity " + typeof( T ).Name );

            AbstractEntity[] primaryKeyTables = new AbstractEntity[] { };


            //if ( ( pkTables == null ) || ( pkTables.Length == 0 ) ) {
            //    primaryKeyTables = new AbstractEntity[] { startEntity };
            //} else {
            //    primaryKeyTables = new AbstractEntity[pkTables.Length];
            //    Array.Copy( pkTables , primaryKeyTables , pkTables.Length );
            //}

            return sqlSelect( startEntity , primaryKeyTables , tables , Entities , ref outParams , ParamPlaceholder , constr );



            //List<dynamic> pkColumnDefinition = new List<dynamic>();
            //foreach ( var pk in pkTables ) {
            //    pkColumnDefinition.AddRange( SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( pk ) );
            //}

            //List<string> columnsName = new List<string>();
            //List<string> joins = new List<string>();
            //Stack<AbstractEntity> visited = new Stack<AbstractEntity>();

            //visited.Push( startEntity );

            //StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            //foreach ( var e in tables ) {


            //    columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            //    columnsName[columnsName.Count - 1] += "\r\n";

            //}

            //var listFK = Entities.GetForeignKeys();
            //foreach ( var fks in listFK ) {

            //    //foreach ( var info in fks.foreignKeyInfo ) {
            //    var join = sqlJoin( ref visited , fks );
            //    if ( !String.IsNullOrWhiteSpace( join ) ) {
            //        joins.Add( join );
            //    }

            //}


            //sb.AppendLine( string.Join( "," , columnsName ) );

            //sb.AppendLine( "FROM" );
            //sb.AppendLine( SQLTableEntityHelper.getTableName( startEntity.GetType() ) );

            //sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            //EntityConditionalExpression constraintExpression = null;

            //if ( Constraint?.Length > 0 ) {

            //    constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

            //    new EntityConditionalExpression( LogicOperator.AND ,
            //         new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) ,
            //         new EntityConditionalExpression( LogicOperator.AND , Constraint )
            //        ) );

            //} else {
            //    constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() );
            //}

            //outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            //sb.AppendLine( "WHERE" );
            //sb.AppendLine( constraintExpression.ToString() );


            //return sb.ToString();


            //var tables = Entities._Graph.GetTopologicalOrder();

            //List<string> columnsName = new List<string>();
            //List<string> joins = new List<string>();

            //StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            //foreach ( var e in tables ) {
            //    columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            //}

            //joins.AddRange( Entities.GetForeignKeys().Select( fk => fk.ToString() ).ToList() );

            //sb.AppendLine( string.Join( "," , columnsName ) );

            //sb.AppendLine( "FROM" );
            //sb.AppendLine( SQLTableEntityHelper.getTableName( tables.First().GetType() ) );

            //sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            //if ( Constraint?.Length > 0 ) {

            //    sb.AppendLine( "WHERE" );

            //    EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
            //    outParams ??= new SQLQueryParams( expr.getParam() );

            //    sb.AppendLine( expr.ToString() );
            //}

            //return sb.ToString();

        }
        public static List<JoinTable> sqlJoinTable( ref Stack<AbstractEntity> visited , EntityForeignKey fks ) {

            Dictionary<AbstractEntity , List<JoinForeignKey>> j = BuildJoinTable( ref visited , fks );

            var sb = new StringBuilder();
            var result = new List<JoinTable>();

            foreach ( var (joinTable, constraint) in j ) {

                visited?.Push( joinTable );

                var jt = new JoinTable( fks.JoinType , joinTable , constraint.ToArray() );

                //if ( joinTable.AliasName is null ) {
                //    sb.Append( string.Format( "{0} JOIN {1} ON " , fks.JoinType.ToString() , joinTable.ActualName.QuoteTableName() ) );
                //} else {
                //    sb.Append( string.Format( "{0} JOIN {1} {2} ON " , fks.JoinType.ToString() , joinTable.TableName.QuoteTableName() , joinTable.ActualName.QuoteTableName() ) );
                //}

                //sb.AppendLine( String.Join( "\r\nAND " , constraint.Select(x=>x.ToString()).ToArray() ) );
                //sb.AppendFormat( "\r\n{0}\r\n" , jt.ToString() );
                result.Add( jt );
            }

            //return sb.ToString();

            return result;

        }

        private static Dictionary<AbstractEntity , List<JoinForeignKey>> BuildJoinTable( ref Stack<AbstractEntity> visited , EntityForeignKey fks ) {



            //var j = new Dictionary<AbstractEntity , List<string>>();
            var result = new Dictionary<AbstractEntity , List<JoinForeignKey>>();
            List<JoinForeignKey> foreignKeys = new List<JoinForeignKey>();
            //AbstractEntity t = null;
            //string alias = null;
            //var a = "";
            //var b = "";

            JoinTable jt = null;
            JoinForeignKey j;

            foreach ( var info in fks.foreignKeyInfo ) {

                if ( visited.Contains( info.Table ) ) {

                    if ( ( info.ReferenceTable is not null ) && ( !visited.Contains( info.ReferenceTable ) ) ) {
                        //t = info.ReferenceTable;
                        //a = SQLTableEntityHelper.getColumnName( info.ReferenceTable , info.ReferenceColumnName , true , true );
                        //b = SQLTableEntityHelper.getColumnName( info.Table , info.ColumnName , true , true );

                        j = new JoinForeignKey( info.ReferenceTable , info.ReferenceColumnName , info.Table , info.ColumnName );
                        //visited.Push( info.ReferenceTable );
                    } else {
                        j = null;
                        //t = null;
                    }

                } else if ( ( info.ReferenceTable is not null ) && ( visited.Contains( info.ReferenceTable ) ) ) {

                    if ( !visited.Contains( info.Table ) ) {
                        //t = info.Table;
                        //b = SQLTableEntityHelper.getColumnName( info.ReferenceTable , info.ReferenceColumnName , true , true );
                        //a = SQLTableEntityHelper.getColumnName( info.Table , info.ColumnName , true , true );

                        j = new JoinForeignKey( info.Table , info.ColumnName , info.ReferenceTable , info.ReferenceColumnName );
                        visited.Push( info.Table );
                    } else {
                        j = null;
                        //t = null;
                    }
                } else {
                    j = null;
                    //t = null;
                }

                if ( j != null ) {
                    if ( result.ContainsKey( j.Table ) ) {
                        result[j.Table].Add( j );
                    } else {
                        result.Add( j.Table , new List<JoinForeignKey>() { j } );
                    }
                }

                //if ( t != null ) {
                //    if ( j.ContainsKey( t ) ) {
                //        j[t].Add( jt );
                //    } else {
                //        j.Add( t , new List<JoinTable>() { jt } );
                //    }
                //    //if ( j.ContainsKey( t ) ) {
                //    //    j[t].Add( $"{a} = {b}" );
                //    //} else {
                //    //    j.Add( t , new List<string>() { $"{a} = {b}" } );
                //    //}
                //}

            }

            return result;
        }

        private static string sqlSelect( AbstractEntity startEntity , AbstractEntity[] pkTables , List<AbstractEntity> tables , EntitiesAdapter Entities , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) {

            List<dynamic> pkColumnDefinition = new List<dynamic>();
            foreach ( var pk in pkTables ) {
                pkColumnDefinition.AddRange( SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( pk ) );
            }

            List<string> columnsName = new List<string>();
            List<string> joins = new List<string>();
            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();

            visited.Push( startEntity );

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            foreach ( var e in tables ) {

                columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() + "\r\n" ).ToList() );
                //columnsName[columnsName.Count - 1] += "\r\n";

            }

            List<ForeignkeyConstraint> fkConstraint = new List<ForeignkeyConstraint>();

            var listFK = Entities.GetForeignKeys();
            //var joinTable = new Dictionary<AbstractEntity , JoinTable>();

            //visited = NewMethod( ref visited , fkConstraint , listFK , joinTable );
            var joinTable = BuildJoinTable( ref visited , fkConstraint , listFK );

            foreach ( var (_, jj) in joinTable ) {
                joins.Add( jj.ToString() );
            }


            sb.AppendLine( string.Join( "," , columnsName ) );

            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getAttributeTableName( startEntity.GetType() ) );

            sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            EntityConditionalExpression constraintExpression = null;

            List<EntityConditionalExpression> conditionalExpression = new List<EntityConditionalExpression>();

            if ( pkColumnDefinition.Count > 0 ) {
                conditionalExpression.Add( new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) );
            }

            if ( ( Constraint is not null ) && ( !Constraint.isEmpty ) ) {
                conditionalExpression.Add( Constraint );
            }
            if ( ( fkConstraint is not null ) && ( fkConstraint.Count > 0 ) ) {
                conditionalExpression.Add( new EntityConditionalExpression( LogicOperator.AND , fkConstraint.ToArray() ) );

            }


            //if ( ( Constraint is not null ) && ( !Constraint.isEmpty ) ) {
            //    if ( pkColumnDefinition.Count > 0 ) {
            //        constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

            //        new EntityConditionalExpression( LogicOperator.AND ,
            //             new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) ,
            //             new EntityConditionalExpression( LogicOperator.AND , Constraint )
            //            ) );

            //    } else {
            //        constraintExpression = Constraint;
            //    }

            //} else {
            //    constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() );
            //}

            constraintExpression = new EntityConditionalExpression( LogicOperator.AND , conditionalExpression.ToArray() );

            outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            sb.AppendLine( "WHERE" );
            sb.AppendLine( constraintExpression.ToString() );


            return sb.ToString();

        }

        public static Dictionary<AbstractEntity , JoinTable> BuildJoinTable( ref Stack<AbstractEntity> visited , List<ForeignkeyConstraint> fkConstraint , List<EntityForeignKey> listFK ) {

            Dictionary<AbstractEntity , JoinTable> joinTable = new Dictionary<AbstractEntity , JoinTable>();

            var OrderArcs = new List<EntityForeignKey>();
            var OrderVisit = new Stack<AbstractEntity>();
            //var _listFk = new List<EntityForeignKey>();
            var l = listFK.Count;
            while ( ( listFK.Count > 0 ) && ( l > 0 ) ) {

                foreach ( var fks in listFK ) {
                    var connected = false;
                    foreach ( var info in fks.foreignKeyInfo ) {
                        if ( visited.Contains( info.ReferenceTable ) ) {
                            connected = true;
                            OrderVisit.Push( info.Table );
                        } else if ( OrderVisit.Contains( info.ReferenceTable ) ) {
                            connected = true;
                            OrderVisit.Push( info.Table );
                        } else if ( visited.Contains( info.Table ) ) {
                            connected = true;
                            OrderVisit.Push( info.ReferenceTable );
                        } else if ( OrderVisit.Contains( info.Table ) ) {
                            connected = true;
                            OrderVisit.Push( info.ReferenceTable );
                        }

                    }

                    if ( connected ) {
                        OrderArcs.Add( fks );
                        listFK.Remove( fks );
                        break;
                    }

                    l -= 1;

                }
            }

            //var Arc = new Stack<EntityForeignKey>();
            //foreach ( var fks in listFK ) {
            //    Arc.Push( fks );
            //}

            //Stack<AbstractEntity> _visited = visited;

            //var xx = listFK.OrderBy( x => x.foreignKeyInfo.Where( a => _visited.Contains( a.ReferenceTable ) ) ).ToList();


            foreach ( var fks in OrderArcs ) {

                var join = sqlJoinTable( ref visited , fks );

                if ( join.Count > 0 ) {

                    foreach ( JoinTable js in join ) {


                        if ( !joinTable.ContainsKey( js.Table ) ) {
                            joinTable[js.Table] = js;
                        } else {
                            throw new NotImplementedException( "Can't add to joinTable" );
                        }
                        //joins.Add( js.ToString() );
                    }

                } else {

                    bool isFkAdded = fks.foreignKeyInfo.Count() > 0;
                    foreach ( var info in fks.foreignKeyInfo ) {

                        var a = joinTable.Keys.ToList().IndexOf( info.Table );
                        var b = joinTable.Keys.ToList().IndexOf( info.ReferenceTable );
                        if ( ( a >= 0 ) && ( b >= 0 ) && ( a > b ) ) {
                            joinTable[joinTable.Keys.ToList()[a]].JoinForeignKeys.Add( new JoinForeignKey( info ) );
                            isFkAdded &= true;
                        } else {
                            isFkAdded &= false;
                        }

                    }

                    if ( !isFkAdded ) {
                        var isNotNull = true;
                        foreach ( var info in fks.foreignKeyInfo ) {
                            if ( info.ReferenceTable is null ) {
                                isNotNull = false;
                                break;
                            }
                        }
                        if ( isNotNull )
                            fkConstraint.Add( new ForeignkeyConstraint( fks ) );
                    }
                }


                //if ( join.Count > 0 ) {


                //    //}
                //    //if ( !String.IsNullOrWhiteSpace( join ) ) {
                //    //    joins.Add( join );
                //} else {

                //    fkConstraint.Add( new ForeignkeyConstraint( fks ) );

                //}
            }

            return joinTable;
        }

        public static string sqlSelect<T>( EntitiesAdapter Entities , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null , params AbstractEntity[] pkTables ) {

            var tables = Entities.Graph.GetTopologicalOrder();

            var startEntity = tables.First( e => e.GetType() == typeof( T ) );
            if ( startEntity is null ) throw new ArgumentException( "Invalid entity " + typeof( T ).Name );

            AbstractEntity[] primaryKeyTables = null;
            if ( ( pkTables == null ) || ( pkTables.Length == 0 ) ) {
                primaryKeyTables = new AbstractEntity[] { startEntity };
            } else {
                primaryKeyTables = new AbstractEntity[pkTables.Length];
                Array.Copy( pkTables , primaryKeyTables , pkTables.Length );
            }

            return sqlSelect( startEntity , primaryKeyTables , tables , Entities , ref outParams , ParamPlaceholder , Constraint );

            //List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( startEntity );
            //List<string> columnsName = new List<string>();
            //List<string> joins = new List<string>();
            //Stack<AbstractEntity> visited = new Stack<AbstractEntity>();

            ////var aliases = new Dictionary<string , AbstractEntity>( tables.Count );
            ////foreach ( var t in tables ) {
            ////    if ( aliases.ContainsKey( t.TableName ) ) {
            ////        if ( t.FkNames.Count == 1 ) {
            ////            if ( !aliases.ContainsKey( t.FkNames.FirstOrDefault() ) ) {
            ////                aliases.Add( t.FkNames.FirstOrDefault() , t );
            ////                t.AliasName = t.FkNames.FirstOrDefault();
            ////            }
            ////        } else {
            ////            throw new NotImplementedException( "sqlSelect t.FkNames.Count > 1" );
            ////        }
            ////    } else {
            ////        aliases.Add( t.TableName , t );
            ////    }
            ////}

            //visited.Push( startEntity );

            //StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            //foreach ( var e in tables ) {

            //    //foreach ( var c in SQLTableEntityHelper.getDefinitionColumn( e , true ) ) {
            //    //    c.Table.TableAlias = e.AliasName ?? c.Table.ActualName;
            //    //    columnsName.Add( ( string ) c.ToString() );
            //    //}

            //    columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
            //    columnsName[columnsName.Count - 1] += "\r\n";

            //}
            ////columnsName.RemoveAt( columnsName.Count() - 1 );

            ////joins.AddRange( Entities.GetForeignKeys().Select( fk => fk.ToString() ).ToList() );

            //var listFK = Entities.GetForeignKeys();
            //foreach ( var fks in listFK ) {

            //    foreach ( var info in fks.foreignKeyInfo ) {

            //        joins.Add( sqlJoin( visited , fks ) );

            //        if ( !visited.Contains( info.Table ) ) {
            //            visited?.Push( info.Table );
            //        } else if ( !visited.Contains( info.ReferenceTable ) ) {
            //            visited?.Push( info.ReferenceTable );
            //        } else {
            //            //Console.WriteLine( "provva" );
            //            throw new NotImplementedException( "sqlSelect else not implemented" );
            //        }


            //    }


            //}


            //sb.AppendLine( string.Join( "," , columnsName ) );

            //sb.AppendLine( "FROM" );
            //sb.AppendLine( SQLTableEntityHelper.getTableName( startEntity.GetType() ) );

            //sb.AppendLine( ( joins.Count > 0 ? String.Join( "\r\n" , joins ) : String.Empty ) );

            //EntityConditionalExpression constraintExpression = null;

            //if ( Constraint?.Length > 0 ) {

            //    constraintExpression = new EntityConditionalExpression( LogicOperator.AND ,

            //    new EntityConditionalExpression( LogicOperator.AND ,
            //         new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() ) ,
            //         new EntityConditionalExpression( LogicOperator.AND , Constraint )
            //        ) );

            //} else {
            //    constraintExpression = new EntityConditionalExpression( LogicOperator.AND , pkColumnDefinition.Select( x => FactoryEntityConstraint.MakeConstraintEqual( x , ParamPlaceholder ) ).ToArray() );
            //}

            //outParams ??= new SQLQueryParams( constraintExpression.getParam() );

            //sb.AppendLine( "WHERE" );
            //sb.AppendLine( constraintExpression.ToString() );


            //return sb.ToString();

        }
        public static string sqlSelect( EntitiesAdapter Entities , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) {

            var tables = Entities.Graph.GetTopologicalOrder();

            var startEntity = tables.First();
            if ( startEntity is null ) throw new ArgumentException( "Invalid entity " + startEntity.GetType().Name );

            return sqlSelect( startEntity , new AbstractEntity[] { startEntity } , tables , Entities , ref outParams , ParamPlaceholder , Constraint );

        }


        public static string sqlSelect( AbstractEntity EntityInstance , ref SQLQueryParams outParams , string ParamPlaceholder = "" , EntityConditionalExpression Constraint = null ) {

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            List<dynamic> pkColumnDefinition = SQLTableEntityHelper.getPrimaryKeyDefinitionColumn( EntityInstance );
            List<string> columnsName = SQLTableEntityHelper.getDefinitionColumn( EntityInstance , true ).Select( x => ( string ) x.ToString() ).ToList();
            List<string> joins = new List<string>();

            sb.AppendLine( string.Join( "," , columnsName ) );

            sb.AppendLine( "FROM" );
            sb.AppendLine( SQLTableEntityHelper.getAttributeTableName( EntityInstance.GetType() ) );

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



        //public static X sqlQuery<T, X>( T Entities , X myQuery ) where T : EntitiesAdapter where X : SQLQuery { //ref SQLQueryParams outParams , EntityConditionalExpression Constraint = null 

        //    //myQuery.Initialize();
        //    //myQuery.SelectExpression = SQLTableEntityHelper.getProjectionColumnName<X>( myQuery );

        //    ////var tables = Entities._Graph.GetTopologicalOrder();

        //    ////List<string> columnsName = new List<string>();
        //    ////List<string> joins = new List<string>();

        //    ////StringBuilder sb = new StringBuilder( "SELECT" );

        //    ////foreach ( var e in tables ) {
        //    ////    columnsName.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );
        //    ////}

        //    //List<DefinableConstraint> constraints = new List<DefinableConstraint>();
        //    //var queryConstraints = SQLTableEntityHelper.getQueryJoinConstraint( myQuery.GetType() );
        //    //foreach ( fk in myQuery.Graph.GetForeignKeys() ) {
        //    //    var constraint = queryConstraints.Where( x => x.Entity == e.GetType() );

        //    //    //.Where( x => ( ( ( DALQueryJoinEntityConstraint ) x ).Entity == e.GetType())   ).ToList()
        //    //    foreach ( DALQueryJoinEntityConstraint a in constraint ) {

        //    //        var fieldName = SQLTableEntityHelper.getPropertyIfColumnDefinition( e.GetType() ).FirstOrDefault( x => x.Name == a.FieldName );
        //    //        //MySQLDefinitionColumn( string aDeclaredVariableName , PropertyInfo aMethodBase , AbstractEntity abstractEntity )
        //    //        var ctor = fieldName.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( AbstractEntity ) } );


        //    //        var instance = ctor?.Invoke( new object[] { fieldName.Name , fieldName , e } );

        //    //        if ( a.ParamName != null ) {
        //    //            constraints.Add( FactorySQLWhereCondition.MakeColumnEqual( fieldName , myQuery[a.ParamName] , a.Placeholder ) );
        //    //        } else {
        //    //            //constraints.Add( FactorySQLWhereCondition.MakeColumnEqual( instance , a.ParamValue , a.Placeholder ) );
        //    //            constraints.Add( FactoryEntityConstraint.MakeEqual( instance , a.ParamValue , "@" ) );
        //    //            //FactorySQLWhereCondition.MakeColumnEqual( fieldName , a.ParamValue , a.Placeholder );
        //    //        }
        //    //        //a.FieldName
        //    //        //FactorySQLWhereCondition.MakeColumnEqual( Delete , this[nameof( pDelete )] , "@" ) ,
        //    //    }

        //    //    //new SQLQueryConditionExpression(
        //    //    //                    LogicOperator.AND ,
        //    //    //                    FactorySQLWhereCondition.MakeColumnEqual( Delete , this[nameof( pDelete )] , "@" ) ,
        //    //    //                    FactorySQLWhereCondition.MakeColumnEqual( FkProvincia , this[nameof( pFkProvincia )] , "@" )
        //    //    //                    )

        //    //    if ( constraints.Count > 0 ) {
        //    //        myQuery.JOIN( e , fk , new SQLQueryConditionExpression( LogicOperator.AND , constraints.ToArray() ) );
        //    //    } else {
        //    //        myQuery.JOIN( e , fk );
        //    //    }

        //    //}



        //    ////if ( Constraint?.Length > 0 ) {

        //    ////    sb.AppendLine( "WHERE" );

        //    ////    EntityConditionalExpression expr = new EntityConditionalExpression( LogicOperator.AND , Constraint );
        //    ////    outParams ??= new SQLQueryParams( expr.getParam() );

        //    ////    sb.AppendLine( expr.ToString() );
        //    ////}
        //    ////
        //    return myQuery;

        //}

        public static String sqlQuery<X>( X queryInstance , ref SQLQueryParams outParams , string paramPlaceholder = "" ) where X : SQLQuery {

            //Count total params
            outParams ??= new SQLQueryParams( ( ( SQLQuery ) queryInstance ).getParam );

            //    return aQueryInstance.ToString();ù

            StringBuilder sb = new StringBuilder( "SELECT\r\n" );

            try {

                sb.AppendLine( String.Join( "," , queryInstance.SelectExpression.Select( x => x + "\r\n" ).ToList() ) );
                sb.AppendLine( "FROM" );
                sb.AppendLine( String.Join( "," , queryInstance.TablesReferences ) );

                foreach ( var jt in queryInstance.JoinTable.Values ) {

                    sb.AppendLine( jt.ToString() );

                }

                List<string> where = new List<string>();

                if ( queryInstance.WhereCondition.Count > 0 ) {

                    queryInstance.WhereCondition.ForEach( x => where.Add( x.ToString() ) );
                    sb.AppendLine( "WHERE" );
                    sb.AppendLine( String.Format( "\t( {0} )" , String.Join( " AND " , where.ToArray() ) ) );
                }

                if ( queryInstance.GroupBy.Count > 0 ) {
                    sb.AppendLine( "GROUP BY " );
                    sb.AppendLine( String.Join( "," , queryInstance.GroupBy.OrderBy( x => x.order ).ToList() ) );
                }

                if ( queryInstance.OrderBy.Count > 0 ) {
                    sb.AppendLine( "ORDER BY " );
                    sb.AppendLine( String.Join( "," , queryInstance.OrderBy.OrderBy( x => x.order ).ToList() ) );
                }


                if ( ( queryInstance.Offset is not null ) && ( queryInstance.RowCount is not null ) ) {
                    sb.AppendLine( String.Format( $"LIMIT {queryInstance.RowCount} OFFSET {queryInstance.Offset}" ) );
                } else if ( ( queryInstance.Offset is null ) && ( queryInstance.RowCount is not null ) ) {
                    sb.AppendLine( String.Format( $"LIMIT {queryInstance.RowCount}" ) );
                } else if ( ( queryInstance.Offset is not null ) && ( queryInstance.RowCount is null ) ) {
                    throw new ArgumentException( "Invalid argument RowCount can't be null." );
                }

            } catch ( System.Exception e ) {
                //    // TODO Auto-generated catch block
                //    e.printStackTrace();
            }
            return sb.ToString();


        }

        public static SQLQuery SELECT<T>( T sqlQuery ) where T : SQLQuery {

            //throw new NotImplementedException();

            sqlQuery.Initialize();
            //myQuery.SelectExpression = SQLTableEntityHelper.getProjectionColumnName<T>( myQuery );
            sqlQuery.SelectExpression = sqlQuery.GetProjectionColumnName<T>( sqlQuery );
            List<string> joins = new List<string>();
            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();

            //var queryConstraints = SQLTableEntityHelper.getQueryJoinConstraint( myQuery.GetType() );
            //visited.Push( startEntity );

            List<ForeignkeyConstraint> fkConstraint = new List<ForeignkeyConstraint>();
            visited.Push( sqlQuery.Graph.GetTopologicalOrder().FirstOrDefault() );
            var listFK = sqlQuery.Graph.GetForeignKeys();

            var queryConstraints = sqlQuery.GetQueryJoinConstraint();
            var queryUnConstraints = sqlQuery.GetQueryJoinUnConstraint();

            var joinTable = BuildJoinTable( ref visited , fkConstraint , listFK );

            foreach ( var (table, jt) in joinTable ) {

                List<DefinableConstraint> constraints = new List<DefinableConstraint>();
                List<DALGenericQueryJoinConstraint> constraint = queryConstraints.Where( x => x.Entity == table.GetType() ).ToList();
                List<DALQueryJoinEntityUnConstraint> unconstraint = queryUnConstraints.Where( x => ( x.Entity == table.GetType() ) ).ToList();
                foreach ( var c in unconstraint ) {

                    var f = jt.JoinForeignKeys.FirstOrDefault( x => x.ReferenceTable.GetType() == c.JoinTable );
                    jt.JoinForeignKeys.Remove( f );

                }

                if ( constraint.Count > 0 ) {

                    constraints = CreateJoinConstraint<T>( sqlQuery , jt , constraint );
                    sqlQuery.JOIN( jt , new SQLQueryConditionExpression( LogicOperator.AND , constraints.ToArray() ) );

                } else {
                    sqlQuery.JOIN( jt );
                }

                joins.Add( jt.ToString() );
            }



            return ( T ) sqlQuery;

        }

        private static List<DefinableConstraint> CreateJoinConstraint<T>( T sqlQuery , JoinTable jt , List<DALGenericQueryJoinConstraint> constraint ) where T : SQLQuery {

            List<DefinableConstraint> result = new List<DefinableConstraint>();

            foreach ( DALGenericQueryJoinConstraint a in constraint ) {

                if ( ( Type ) a.TypeId == typeof( DALQueryJoinEntityConstraint ) ) {

                    dynamic instance = SQLTableEntityHelper.getDefinitionColumn( jt.Table , true ).FirstOrDefault( x => x.ColumnName == a.FieldName ) ?? throw new ArgumentNullException( "invalid fieldname " + a.FieldName );

                    dynamic c = ( a as DALQueryJoinEntityConstraint ).ParamName != null
                        ? FactoryEntityConstraint.MakeEqual( instance , ( MySQLQueryParam ) sqlQuery[( a as DALQueryJoinEntityConstraint ).ParamName] , ( a as DALQueryJoinEntityConstraint ).Placeholder )
                        : FactoryEntityConstraint.MakeEqual( instance , ( a as DALQueryJoinEntityConstraint ).ParamValue , ( a as DALQueryJoinEntityConstraint ).Placeholder );

                    result.Add( sqlQuery.MakeUniqueParamName( c ) );


                } else if ( ( Type ) a.TypeId == typeof( DALQueryJoinSubQueryConstraint ) ) {

                    dynamic instance = SQLTableEntityHelper.getDefinitionColumn( jt.Table , true ).FirstOrDefault( x => x.ColumnName == a.FieldName ) ?? throw new ArgumentNullException( "invalid fieldname " + a.FieldName );

                    Type subQuery = ( a as DALQueryJoinSubQueryConstraint ).SubQuery;
                    var ctor = subQuery.GetConstructor( new Type[] { typeof( QueryAdapter ) } );
                    var instanceSubQuery = ctor?.Invoke( new object[] { sqlQuery.mQueryAdapter } );
                    //FactoryEntityConstraint.MakeEqual<SQLSubQuery( instance , new StrutturaVenditaDataSubQuery( this.mQueryAdapter ) )
                    dynamic c = FactoryEntityConstraint.MakeEqual( instance , ( SQLQuery ) instanceSubQuery );
                    //FactoryEntityConstraint.MakeEqual<SQLSubQuery<StrutturaVenditaQueryAdapter.Query>>( EntitiesAdapter.Adapter<StruttureVenditeEntityAdapter>().Entity.PkData , new StrutturaVenditaDataSubQuery( this.mQueryAdapter ) )
                    result.Add( sqlQuery.MakeUniqueParamName( c ) );
                    //var xxx = c.ToString();    

                } else if ( ( Type ) a.TypeId == typeof( DALQueryJoinBetween ) ) {
                    
                } else if ( ( Type ) a.TypeId == typeof( DALQueryJoinEntityExpression ) ) {

                    int baseIndex = 0;

                    var mFunctionParams = new List<IFunctionParam>();
                    
                    DALFunctionParam[] dalFunctionParam = ( ( DALFunctionParam[] ) ( typeof( T ).GetCustomAttributes<DALFunctionParam>() ) );

                    var totalParam = dalFunctionParam.Sum( x => x.CountParam() ) + a.CountParam();

                    if ( totalParam != dalFunctionParam.Length ) throw new ArgumentException( $"Invalid numer of param for function {( ( DALQueryJoinEntityExpression ) a ).Function}" );

                    var functionParams = dalFunctionParam.Skip( baseIndex ).Take( a.CountParam() ).ToArray();

                    baseIndex = 0;

                    var subFunctionParams = dalFunctionParam.Skip( a.CountParam() ).ToArray();

                    foreach ( var param in functionParams ) {

                        if ( ( Type ) param.TypeId == typeof( DALFunctionParamSubQueryConstraint ) ) {

                            IFunctionParam fp = FunctionParamFactory.Create( ( DALFunctionParamSubQueryConstraint ) param , ref baseIndex , subFunctionParams , sqlQuery.mQueryAdapter );
                            if ( fp is not null ) mFunctionParams.Add( fp );

                        } else {

                            IFunctionParam fp = FunctionParamFactory.Create( param , ref baseIndex , subFunctionParams );
                            if ( fp is not null ) mFunctionParams.Add( fp );
                        }

                    }

                    FunctionExpression mFunction = ( FunctionExpression ) FunctionFactory.Create( ( ( DALQueryJoinEntityExpression ) a ) , mFunctionParams.ToArray() );



                    if ( mFunction.Right.GetType() == typeof( FunctionParamSubQuery ) ) {

                        IQueryParams outparam = null;

                        ( mFunction.Right as FunctionParamSubQuery ).SubQuery.Build();

                        outparam = ( mFunction.Right as FunctionParamSubQuery ).SubQuery.getQueryParams();
                        sqlQuery.MakeUniqueParamName( ( SQLQueryParams ) outparam );

                        var sqlSubQuery = ( mFunction.Right as FunctionParamSubQuery ).SubQuery.ToString();

                        if ( outparam != null )
                            result.Add( new EntityRawSqlQuery( mFunction.ToString() , outparam ) );
                        else
                            result.Add( new EntityRawSqlQuery( mFunction.ToString() ) );

                    } else
                        result.Add( new EntityRawSqlQuery( mFunction.ToString() ) );


                }


            }

            return result;
        }
    }

}
