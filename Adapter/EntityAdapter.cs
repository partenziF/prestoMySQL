using Microsoft.Extensions.Logging;
using MySqlConnector;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Entity.Interface;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Index;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.Database.Interface;
using prestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Adapter {

    public enum KeyTypeDescribe {
        None,
        Primary,
        Unique

    }

    public struct TableDescribe {
        public TableDescribe( string Field , string Type , bool Null , string Key , object Default , string Extra ) {
            this.Field = Field;
            this.Type = Type;
            this.Null = Null;

            if ( Key.Equals( "PRI" ) ) {
                this.Key = KeyTypeDescribe.Primary;
            } else if ( Key.Equals( "UNI" ) ) {
                this.Key = KeyTypeDescribe.Unique;
            } else {
                this.Key = KeyTypeDescribe.None;
            }

            this.Default = Default;
            this.Extra = Extra;
        }

        public string Field { get; }
        public string Type { get; }
        public bool Null { get; }
        public KeyTypeDescribe Key { get; }
        public object Default { get; }

        public string Extra { get; }

    }


    public abstract class EntityAdapter<T> : TableEntity, IEnumerable<T> where T : AbstractEntity {

        private const string ERROR_EXECUTE_QUERY = "Error execute query ";
        //private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();
        public EntityAdapter( MySQLDatabase aMySQLDatabase , ILogger logger ) {

            this.mDatabase = aMySQLDatabase;
            this.mLogger = logger;

            CreateEvents();

            OnInitData( EventArgs.Empty );

        }


        #region sezione eventi
        protected event EventHandler InitData;
        protected virtual void OnInitData( EventArgs e ) {
            EventHandler handler = InitData;
            handler?.Invoke( this , e );

        }

        public void DoInitData() {
            OnInitData( EventArgs.Empty );
        }


        protected delegate void BindDataFromEventHandler( Object sender , BindDataFromEventArgs<T> e );
        protected event BindDataFromEventHandler BindDataFrom;
        protected virtual void OnBindDataFrom( BindDataFromEventArgs<T> e ) {
            BindDataFromEventHandler handler = BindDataFrom;
            handler?.Invoke( this , e );
        }


        protected delegate void BindDataToEventHandler( Object sender , BindDataToEventArgs<T> e );
        protected event BindDataToEventHandler BindDataTo;
        protected virtual void OnBindDataTo( BindDataToEventArgs<T> e ) {
            BindDataToEventHandler handler = BindDataTo;
            handler?.Invoke( this , e );
        }


        #endregion

        private List<AbstractEntity> foreignKeyTables = new List<AbstractEntity>();
        protected readonly MySQLDatabase mDatabase;
        private readonly ILogger mLogger;

        public T Entity { get => mEntities.LastOrDefault(); set => mEntities.Add( value ); }

        /////////////////////////////////////////////////////////////////////////////
        //Transazione
        //public ITransaction transaction = null;

        //public ITransaction getTransaction() {
        //    return transaction;
        //}

        //public void setTransaction( ITransaction transaction ) {
        //    this.transaction = transaction;
        //}

        /////////////////////////////////////////////////////////////////////////////

        //Implementare IEnumerable<T>?
        private List<T> mEntities = new List<T>();
        //private T mCurrentEntity;
        //private void SetEntity( T value ) {
        //    mEntities.Add( value );
        //    //mCurrentEntity = value;
        //}

        //public int Count => ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).Count;
        //public bool IsReadOnly => ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).IsReadOnly;
        //public AbstractEntity this[int index] { get => ( ( IList<AbstractEntity> ) this.foreignKeyTables )[index]; set => ( ( IList<AbstractEntity> ) this.foreignKeyTables )[index] = value; }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public string getAssociativeEntity( Type entity ) {

            new NotImplementedException( $"EntityAdapter.{nameof( getAssociativeEntity )}" );
            foreach ( AbstractEntity e in foreignKeyTables ) {

                if ( e.GetType().Equals( entity ) ) {
                    return e.TableName;
                }
                //if (entity.isInstance(e)) {
                //	return e.getTableName();
                //}

            }

            throw new System.Exception( "Invalid entity" );

        }

        public void BindData( IReadableResultSet rs ) {

            //List<dynamic> _definitionColumns = new List<dynamic>()
            //foreach ( var (t, listOfEntity) in Entity.EntityTree ) {
            //    foreach ( var e in listOfEntity ) {
            //        _definitionColumns.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() );
            //    }
            //}

            List<dynamic> _definitionColumns = SQLTableEntityHelper.getDefinitionColumn( Entity , true );
            Dictionary<string , Dictionary<Type , List<object>>> primaryKeysValues = new Dictionary<string , Dictionary<Type , List<object>>>();
            primaryKeysValues.Add( Entity.ActualName , new Dictionary<Type , List<object>>() );

            var s = rs.ResultSetSchemaTable();
            int? index = null;

            foreach ( ConstructibleColumn column in _definitionColumns ) {

                if ( s.ContainsKey( column.Table.ActualName ) ) {
                    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                        index = s[column.Table.ActualName][column.ActualName];
                    }
                }

                if ( index == null ) throw new System.Exception( "Invalid index column. (" + column.ActualName + ")" );


                var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                                                           typeof( MySQResultSet ) ,
                                                           rs ,
                                                           nameof( MySQResultSet.getValueAs ) ,
                                                           new Type[] { typeof( int ) } ,
                                                           new object[] { ( int ) index } );



                if ( column.isPrimaryKey ) {

                    if ( primaryKeysValues.ContainsKey( column.Table.ActualName ) ) {

                        if ( primaryKeysValues[column.Table.ActualName].ContainsKey( ( column as dynamic ).TypeTable ) ) {
                            ( primaryKeysValues[column.Table.ActualName][( column as dynamic ).TypeTable] as List<object> ).Add( v );
                        } else {
                            primaryKeysValues[column.Table.ActualName][( column as dynamic ).TypeTable] = new List<object>() { v };

                        }
                    } else {
                        throw new System.Exception( $"Can't find table name {column.Table.ActualName} in primarykeysvalues" );
                    }

                } else {

                    if ( v.IsDBNull() ) {
                        ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                    } else if ( v is null ) {
                        ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                    } else {
                        column.AssignValue( v );
                    }

                }


                //foreach ( var (tablename, tablenames) in primaryKeysValues ) {
                //    foreach ( var (typetable, values) in tablenames ) {
                //        if ( (Entity.ActualName == tablename) && ( Entity.GetType().Equals( typetable ) ) ) {
                //            Entity?.PrimaryKey.setKeyValues( values.ToArray() );
                //        }
                //        //tables.FirstOrDefault( x => ( ( x.ActualName == tablename ) && ( x.GetType().Equals( typetable ) ) ) )?.PrimaryKey.setKeyValues( values.ToArray() );
                //    }
                //}


                if ( primaryKeysValues.ContainsKey( Entity.ActualName ) )
                    Entity.PrimaryKey.setKeyValues( primaryKeysValues[Entity.ActualName].Values.FirstOrDefault().ToArray() );



                //index = null;

                //if ( s.ContainsKey( column.Table.ActualName ) ) {
                //    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                //        index = s[column.Table.ActualName][column.ActualName];
                //    }
                //}

                //if ( index == null ) throw new System.Exception( "Invalid index column." );

                //string columnName = ( column.Table.TableAlias is null ) ? ( string ) column.ColumnName : "{" + column.Table.TableAlias + "}." + column.ColumnName;

                ////need explicit conversion to work
                //if ( rs[columnName].IsDBNull() ) {

                //    var o = typeof( SQLTypeWrapper<> ).MakeGenericType( column.GenericType );
                //    var p = o.GetField( nameof( SQLTypeWrapper<object>.NULL ) , BindingFlags.Static | BindingFlags.Public );
                //    //dynamic xx = p.GetValue( null );
                //    //var xxx = Convert.ChangeType( xx , o );

                //    ( column as dynamic ).TypeWrapperValue = ( dynamic ) p.GetValue( null ); ;// ( SQLTypeWrapper <uint?> )p.GetValue( null );

                //} else {

                //    //var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                //    //                                       typeof( MySQResultSet ) ,
                //    //                                       rs ,
                //    //                                       nameof( MySQResultSet.getValueAs ) ,
                //    //                                       new Type[] { typeof( int ) } ,
                //    //                                       new object[] { ( int ) index } );

                //    //if ( v.IsDBNull() ) {
                //    //    ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                //    //} else {
                //    //    column.AssignValue( v );
                //    //}

                //    MethodInfo method = typeof( MySQResultSet ).GetMethod( nameof( MySQResultSet.getValueAs ) , new Type[] { typeof( string ) } );
                //    MethodInfo generic = method.MakeGenericMethod( column.GenericType );
                //    var o = generic.Invoke( rs , new object[] { ( string ) column.ColumnName } );
                //    column.AssignValue( o );

                //}

            }

        }

        public static implicit operator T( EntityAdapter<T> v ) {
            return ( T ) v.Entity;
        }


        public static OperationResult DropTable( bool ifExists , MySQLDatabase mDatabase , ILogger mLogger = null ) {

            var s = SQLBuilder.sqlDrop<T>();
            int? i = -1;
            try {

                i = mDatabase.ExecuteQuery( s ) ?? null;
                if ( i is null ) return OperationResult.Error;
                else if ( i == 0 ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {
                mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( DropTable ) );
                return OperationResult.Error;
            } catch ( System.Exception e ) {
                mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );
            }

        }
        public static OperationResult CreateTable( bool ifExists , MySQLDatabase mDatabase , ILogger mLogger = null ) {

            var s = SQLBuilder.sqlCreate<T>();
            int? i = -1;

            try {

                i = mDatabase.ExecuteQuery( s ) ?? null;
                if ( i is null ) return OperationResult.Error;
                else if ( i == 0 ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {
                mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( CreateTable ) );
                return OperationResult.Error;

            } catch ( System.Exception e ) {
                mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );
            }

        }
        public static OperationResult ExistsTable( MySQLDatabase mDatabase , ILogger mLogger = null ) {

            var s = SQLBuilder.sqlExistsTable<T>();
            string? r;

            try {

                r = mDatabase.ExecuteScalar<string>( s ) ?? null;
                //the function can return a null value even if there, not an error
                if ( r is null ) return OperationResult.Fail;
                else if ( !String.IsNullOrWhiteSpace( r ) && ( r.Equals( SQLTableEntityHelper.getAttributeTableName<T>() , StringComparison.InvariantCultureIgnoreCase ) ) ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {
                mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( ExistsTable ) );
                return OperationResult.Error;

            } catch ( System.Exception e ) {
                mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                throw new System.Exception( ERROR_EXECUTE_QUERY + e.Message );
            }

        }
        protected static OperationResult CheckTable( MySQLDatabase mDatabase , ILogger mLogger = null ) {

            var rs = mDatabase.ReadQuery( SQLBuilder.sqlDescribeTable<T>() );
            //var allFieldsExists = true;

            if ( ( bool ) !( rs?.isEmpty() ) ) {

                var properties = SQLTableEntityHelper.getPropertyIfColumnDefinition<T>();
                Dictionary<string , DDColumnAttribute> attributes = new Dictionary<string , DDColumnAttribute>();
                foreach ( var p in properties ) {

                    if ( Attribute.IsDefined( p , typeof( DDColumnFloatingPointAttribute ) ) ) {
                        attributes.Add( p.GetCustomAttribute<DDColumnFloatingPointAttribute>().Name , p.GetCustomAttribute<DDColumnFloatingPointAttribute>() );

                    } else if ( Attribute.IsDefined( p , typeof( DDColumnStringAttribute ) ) ) {
                        attributes.Add( p.GetCustomAttribute<DDColumnStringAttribute>().Name , p.GetCustomAttribute<DDColumnStringAttribute>() );

                    } else if ( Attribute.IsDefined( p , typeof( DDColumnBooleanAttribute ) ) ) {
                        attributes.Add( p.GetCustomAttribute<DDColumnBooleanAttribute>().Name , p.GetCustomAttribute<DDColumnBooleanAttribute>() );

                    } else if ( Attribute.IsDefined( p , typeof( DDColumnNumericAttribute ) ) ) {
                        attributes.Add( p.GetCustomAttribute<DDColumnNumericAttribute>().Name , p.GetCustomAttribute<DDColumnNumericAttribute>() );

                    } else if ( Attribute.IsDefined( p , typeof( DDColumnAttribute ) ) ) {
                        attributes.Add( p.GetCustomAttribute<DDColumnAttribute>().Name , p.GetCustomAttribute<DDColumnAttribute>() );
                    }

                }



                if ( rs.fetch() ) {

                    var FieldsDescribe = new List<TableDescribe>();

                    do {

                        FieldsDescribe.Add( new TableDescribe( rs.getValueAs<string>( "Field" ) ,
                            rs.getValueAs<string>( "Type" ) ,
                            rs.getValueAs<string>( "Null" ).Equals( "YES" ) ,
                            rs.getValueAs<string>( "Key" ) ,
                            rs.getValueAs<object>( "Default" ) ,
                            rs.getValueAs<string>( "Extra" ) ) );


                    } while ( rs.fetch() );

                    rs?.close();


                    if ( attributes.Count == FieldsDescribe.Count ) {
                        //Alter Table

                        for ( int i = 0; i < attributes.Count; i++ ) {

                            var FieldName = attributes.Keys.ToArray()[i];
                            if ( !( FieldsDescribe[i].Field.Equals( FieldName ) ) ) {

                                attributes[FieldName].ChangeColumn( FieldsDescribe[i].Field );

                            }
                            //if ( properties[i]. == FieldsDescribe[i] ) {

                            //}
                        }

                    } else if ( properties.Count > FieldsDescribe.Count ) {
                        //Add new field

                    } else if ( properties.Count < FieldsDescribe.Count ) {
                        //Delete field from table
                    }


                    //if ( attributes.ContainsKey( rs.getValueAs<string>( "Field" ) ) ) {
                    //    var a = attributes[rs.getValueAs<string>( "Field" )];
                    //    var sClassType = a.BulldTypeString().RemoveAllWhitespace();
                    //    var sDbType = rs.getValueAs<string>( "Type" ).RemoveAllWhitespace();
                    //    if ( sClassType.Equals( sDbType , StringComparison.InvariantCultureIgnoreCase ) ) {
                    //        //ok
                    //    } else {
                    //        //fail
                    //    }

                    //    if ( ( rs.getValueAs<string>( "Null" ).Equals( "YES" , StringComparison.InvariantCultureIgnoreCase ) ) && ( a.NullValue == NullValue.Null ) ) {
                    //        //ok
                    //    } else {
                    //        //fail
                    //    }

                    //    var sDbNull = rs.getValueAs<string>( "Null" );

                    //    if ( ( rs.getValueAs<string>( "Null" ).Equals( "NO" , StringComparison.InvariantCultureIgnoreCase ) ) && ( a.NullValue == NullValue.NotNull ) ) {
                    //        //ok
                    //    } else {
                    //        //fail
                    //    }

                    //    if ( a.GetDefaultValueClause().Equals( rs.getValueAs<string>( "Default" ) , StringComparison.InvariantCultureIgnoreCase ) ) {

                    //    }


                    //} else {
                    //    //allFieldsExists = false;
                    //}


                    return OperationResult.OK;


                } else {
                    rs?.close();
                    return OperationResult.Fail;
                }

            } else {
                return OperationResult.Error;
            }

        }
        public static OperationResult TruncateTable( MySQLDatabase mDatabase , ILogger mLogger = null ) {

            var s = SQLBuilder.sqlTruncate<T>();
            int? i = -1;
            try {

                i = mDatabase.ExecuteQuery( s ) ?? null;
                if ( i is null ) return OperationResult.Error;
                else if ( i != -1 ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {
                mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( TruncateTable ) );
                return OperationResult.Error;
            } catch ( System.Exception e ) {
                mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );
            }
        }

        public static bool Check( MySQLDatabase mDatabase , ILogger mLogger = null ) {
            if ( !( ExistsTable( mDatabase , mLogger ) == OperationResult.OK ) ) {
                return ( CreateTable( true , mDatabase , mLogger ) == OperationResult.OK );
            } else {
                //Check difference between database and class
                return CheckTable( mDatabase , mLogger ) == OperationResult.OK;
            }
        }

        public void ResetEntities() {
            this.mEntities?.Clear();
        }

        internal U CreateInstace<U>() where U : AbstractEntity {
            if ( typeof( T ) == typeof( U ) ) {
                return ( U ) Activator.CreateInstance( typeof( T ) );
            } else {
                throw new ArgumentException( "Invalid generic type." );
            }
        }

        //internal void CreateInstace<U>( bool addToEntities = false ) where U : AbstractEntity {

        //    if ( typeof( T ) == typeof( U ) ) {

        //        if ( addToEntities == true ) {
        //            this.Entity = ( T ) Activator.CreateInstance( typeof( T ) );
        //        } else {
        //            this.Entity ??= ( T ) Activator.CreateInstance( typeof( T ) );
        //        }
        //        InitEntity();
        //    } else {
        //        new ArgumentException( "Invalid generic type." );
        //    }

        //}

        internal void AssignInstance( AbstractEntity e ) {

            ResetEntities();

            if ( typeof( T ) == e.GetType() ) {
                Entity ??= ( T ) e;
            } else {
                this.Entity ??= ( T ) Activator.CreateInstance( typeof( T ) );
            }

            InitEntity();
            MapToEntity();
            Entity.State = EntityState.Created;


        }

        public OperationResult Create() {

            Entity ??= CreateInstace<T>();
            InitEntity();
            MapToEntity();
            Entity.State = EntityState.Created;

            //CreateInstace<T>();
            if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };

            Entity.PrimaryKey?.createKey();
            createForeignKey();
            CreatePrimaryKey();

            return OperationResult.OK;
        }


        public override OperationResult New( AbstractEntity newEntity = null , bool UpdateForeignKey = true ) {

            //CreateInstace<T>( true );
            Entity = ( T ) ( newEntity ?? CreateInstace<T>() );
            if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };

            InitEntity();

            if ( newEntity is null ) MapToEntity();
            else MapFromAdapter();

            Entity.State = EntityState.Created;

            if ( newEntity is null ) {
                Entity.PrimaryKey?.createKey();
            } else {
                if ( Entity.PrimaryKey?.KeyState == KeyState.CreatedKey )
                    Entity.PrimaryKey?.createKey();
            }


            createForeignKey();
            CreatePrimaryKey();

            if ( UpdateForeignKey ) {
                Entity.GetAllForeignkey().ForEach( fks => {

                    foreach ( var info in fks.foreignKeyInfo ) {

                        if ( info.ReferenceTable == null ) {

                            var f = mEntities.FirstOrDefault().GetAllForeignkey().FirstOrDefault( x => x.ForeignkeyName == fks.ForeignkeyName );

                            if ( f != null ) {
                                foreach ( var ff in f.foreignKeyInfo ) {

                                    info.ReferenceTable = ff.ReferenceTable;
                                    fks.addEntities( new List<AbstractEntity>() { ff.ReferenceTable } );

                                }
                            }

                        }

                    }

                } );
            }


            return OperationResult.OK;

        }

        //OperationResult ExecuteSelect( EntityConditionalExpression Constraint , object[] KeyValues ) {
        //    int i = 0;
        //    try {

        //        Entity.PrimaryKey.setKeyValues( KeyValues );
        //        var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

        //        if ( r == OperationResult.OK ) {
        //            Entity.PrimaryKey.KeyState = KeyState.Set;
        //        } else {
        //            Entity.PrimaryKey.KeyState = KeyState.Unset;
        //        }

        //        return r;

        //    } catch ( ArgumentOutOfRangeException e1 ) {
        //        throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
        //    } catch ( System.Exception e ) {
        //        throw new System.Exception( e.Message );
        //    }

        //    //if ( KeyValues.Length == Entity.PrimaryKey.KeyLength ) {

        //    //    Entity.PrimaryKey.setKeyValues();

        //    //    foreach ( KeyValuePair<string , PropertyInfo> kvp in this.Entity.PrimaryKey ) {
        //    //        dynamic column = kvp.Value.GetValue( Entity );
        //    //        column.AssignValue( KeyValues[i++] );
        //    //    }

        //    //    var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

        //    //    if ( r == OperationResult.OK ) {
        //    //        Entity.PrimaryKey.KeyState = KeyState.Set;
        //    //    } else {
        //    //        Entity.PrimaryKey.KeyState = KeyState.Unset;
        //    //    }

        //    //    return r;

        //    //} else {
        //    //    throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
        //    //}

        //}

        public OperationResult Refresh() {

            if ( Entity is null ) {
                throw new ArgumentNullException( "Entity can't be null." );
            };

            try {
                if ( Entity.PrimaryKey.KeyState == KeyState.SetKey ) {

                    SQLQueryParams outparam = null;
                    mSQLQuery = SQLBuilder.sqlSelect( Entity , ref outparam , ParamPlaceholder: "@" , null );
                    var rs = mDatabase.ReadQuery( SQLQuery , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                    var r = FetchResultSet( rs );

                    if ( r == OperationResult.OK ) {

                        foreach ( var e in this )
                            e.PrimaryKey.KeyState = KeyState.SetKey;

                    } else {
                        foreach ( var e in this )
                            e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    }

                    return r;

                } else {
                    throw new KeyNotFoundException( "Invalid keystate, key is not set" );
                }

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valu length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }

        public override OperationResult Read( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            //CreateInstace<T>();
            if ( Entity is null ) {
                Create();
                //new ArgumentNullException( "Entity can't be null." );
            };
            //InitEntity();

            try {

                Entity.PrimaryKey.setKeyValues( KeyValues );
                //var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                SQLQueryParams outparam = null;
                mSQLQuery = SQLBuilder.sqlSelect( Entity , ref outparam , ParamPlaceholder: "@" , Constraint );
                var rs = mDatabase.ReadQuery( SQLQuery , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs );

                if ( r == OperationResult.OK ) {

                    foreach ( var e in this )
                        e.PrimaryKey.KeyState = KeyState.SetKey;

                } else {
                    foreach ( var e in this )
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valu length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }


        }

        public OperationResult Read<X>( Func<T , X> delegateMethod , EntityConditionalExpression Constraint = null ) where X : TableIndex {

            //CreateInstace<T>();
            if ( this.Entity is null ) {
                Create();
            };
            //InitEntity();

            X x = delegateMethod( Entity );

            List<DefinableConstraint> constraints = new List<DefinableConstraint>();

            foreach ( string c in x.ColumnsName ) {
                PropertyInfo p = x[c];
                dynamic col = p.GetValue( this.Entity );
                //DefinableConstraint o2 = FactoryEntityConstraint.MakeConstraintEqual( col , "@" );
                //FactoryEntityConstraint.MakeEqual( a.Entity<QualificheEntity>().Delete , "QDelete" , false , "@" ) ,

                //DefinableConstraint o2 = FactoryEntityConstraint.MakeConstraintEqual( p, new SQLQueryParams( new[] { ( MySQLQueryParam ) ( col as dynamic ) } ) , "@" );
                DefinableConstraint o = FactoryEntityConstraint.MakeEqual( col , col.GetValue() , "@" );

                //var col = p.GetValue( this.Entity );
                //DefinableConstraint o = FactoryEntityConstraint.MakeConstraintEqual( col , "@" );

                //DefinableConstraint o2 = FactoryEntityConstraint.MakeConstraintEqual( p , "@" );
                constraints.Add( o );
            }

            SQLQueryParams outparam = null;
            EntityConditionalExpression constraintExpression;
            if ( Constraint is null ) {
                constraintExpression = new EntityConstraintExpression( constraints.ToArray() );
            } else {
                constraintExpression = new EntityConditionalExpression( LogicOperator.AND , new EntityConstraintExpression( constraints.ToArray() ) , Constraint );
            }

            var s = SQLBuilder.sqlSelect<T>( this.Entity , ref outparam , Constraint: constraintExpression );

            var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            var r = FetchResultSet( rs );

            if ( r == OperationResult.OK ) {

                foreach ( var e in this )
                    e.PrimaryKey.KeyState = KeyState.SetKey;
            } else if ( r != OperationResult.Empty ) {
                foreach ( var e in this )
                    e.PrimaryKey.KeyState = KeyState.UnsetKey;
            }

            return r;

        }

        private OperationResult FetchResultSet( MySQResultSet rs ) {

            if ( rs != null ) {

                if ( rs.fetch() ) {

                    BindData( rs );

                    OnBindDataFrom( new BindDataFromEventArgs<T>() { Entity = this.Entity } );

                    if ( rs.fetch() ) {

                        Entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        rs?.close();
                        this.mEntities.Clear();
                        throw new System.Exception( "Unique index entity violation" );

                    } else {

                        Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        rs?.close();
                        return OperationResult.OK;

                    }

                } else {
                    rs?.close();
                    if ( this.mEntities.Count() > 1 )
                        this.mEntities.Clear();
                    return OperationResult.Empty;
                }

            } else {
                rs?.close();
                if ( this.mEntities.Count() > 1 )
                    this.mEntities.Clear();
                return OperationResult.Error;
            }
        }

        //protected override OperationResult Select( EntityConditionalExpression Constraint , params object[] values ) {

        //    //SQLQueryParams outparam = null;
        //    ////this.Entity
        //    //var s = SQLBuilder.sqlSelect( Entity , ref outparam , ParamPlaceholder: "@" , Constraint );

        //    //var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

        //    if ( rs != null ) {

        //        if ( rs.fetch() ) {

        //            BindData( rs );

        //            OnBindDataFrom( new BindDataFromEventArgs<T>() { Entity = this.Entity } );

        //            if ( rs.fetch() ) {

        //                Entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
        //                rs?.close();
        //                throw new System.Exception( "Primary key entity violation" );

        //            } else {

        //                Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
        //                rs?.close();
        //                return OperationResult.OK;

        //            }

        //        } else {
        //            rs?.close();
        //            return OperationResult.Fail;
        //        }

        //    } else {
        //        rs?.close();
        //        return OperationResult.Error;
        //    }


        //}

        protected override OperationResult Insert( AbstractEntity entity ) {

            if ( entity == null ) throw new System.Exception( "entity is null, call create method." );
            SQLQueryParams outparam = null;

            if ( mEntities.Count == 1 ) {
                var args = new BindDataToEventArgs<T> { Entity = ( T ) entity };
                OnBindDataTo( args );
            }

            if ( entity.State == prestoMySQL.Entity.Interface.EntityState.Changed ) {

                mSQLQuery = SQLBuilder.sqlInsert<T>( ( T ) entity , ref outparam , "@" );

                int? rowInserted = -1;
                try {

                    rowInserted = mDatabase.ExecuteQuery( SQLQuery , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                    if ( rowInserted is null ) {

                        return OperationResult.Error;

                    } else if ( rowInserted != -1 ) {

                        object[] primaryKeyValues = null;
                        switch ( entity.PrimaryKey?.isAutoIncrement ) {

                            case true:
                                entity.PrimaryKey.doCreatePrimaryKey();
                                SetPrimaryKey();
                                entity.PrimaryKey.KeyState = KeyState.SetKey;
                                break;
                            case false:
                                primaryKeyValues = entity.PrimaryKey.getKeyValues();
                                if ( entity.PrimaryKey.isAutoIncrement ) {
                                    SetPrimaryKey();
                                }
                                entity.PrimaryKey.KeyState = KeyState.SetKey;
                                break;
                        }

                        entity.State = prestoMySQL.Entity.Interface.EntityState.Set;

                        return OperationResult.OK;

                    } else {
                        return OperationResult.Fail;

                    }

                } catch ( MySqlException ex ) {

                    mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( Insert ) );
                    return OperationResult.Exception;

                } catch ( System.Exception e ) {
                    mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                    throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );

                }

            } else {
                return OperationResult.OK; //Unchanged data
            }

        }

        protected override OperationResult Update( AbstractEntity entity ) {

            if ( entity == null ) throw new System.Exception( "entity is null, call create method." );
            SQLQueryParams outparam = null;

            var args = new BindDataToEventArgs<T> { Entity = ( T ) entity };
            OnBindDataTo( args );

            if ( entity.State == prestoMySQL.Entity.Interface.EntityState.Changed ) {

                mSQLQuery = SQLBuilder.sqlUpdate<T>( ( T ) entity , ref outparam , "@" );
                int? rowChanged = -1;
                try {

                    rowChanged = mDatabase.ExecuteQuery( SQLQuery , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                    if ( rowChanged is null ) {

                        return OperationResult.Error;

                    } else if ( rowChanged != -1 ) {

                        entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        return OperationResult.OK;

                    } else {
                        return OperationResult.Fail;
                    }


                } catch ( MySqlException ex ) {

                    mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( Update ) );
                    return OperationResult.Exception;

                } catch ( System.Exception e ) {
                    mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                    throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );
                }

            } else {
                return OperationResult.Unchange;
            }

        }

        protected override OperationResult Delete( AbstractEntity entity ) {

            if ( entity == null ) throw new System.Exception( "entity is null, call create method." );
            SQLQueryParams outparam = null;

            var args = new BindDataToEventArgs<T> { Entity = ( T ) entity };
            OnBindDataTo( args );


            mSQLQuery = SQLBuilder.sqlDelete<T>( ( T ) entity , ref outparam , "@" );
            int? rowChanged = -1;
            try {

                rowChanged = mDatabase.ExecuteQuery( SQLQuery , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                if ( rowChanged is null ) {

                    return OperationResult.Error;

                } else if ( rowChanged != -1 ) {

                    entity.State = prestoMySQL.Entity.Interface.EntityState.Deleted; ;
                    return OperationResult.OK;

                } else {
                    return OperationResult.Fail;
                }


            } catch ( MySqlException ex ) {

                mLogger?.LogError( "Exception " + ex.Message + " in " + nameof( Update ) );
                return OperationResult.Exception;

            } catch ( System.Exception e ) {
                mLogger?.LogError( "Last error : " + mDatabase.LastError?.ToString() ?? "" + " Exception " + e.Message + " in " + nameof( DropTable ) );
                throw new System.Exception( ERROR_EXECUTE_QUERY + ( ( mDatabase.LastError is null ) ? mDatabase.LastError?.ToString() ?? e.Message : e.Message ) );
            }


        }

        public override bool Save() {

            if ( this.Entity == null ) {
                throw new System.Exception( "entity is null, call create method." );
            }

            bool result = true;

            foreach ( T aEntity in mEntities ) {


                switch ( aEntity.PrimaryKey?.KeyState ) {

                    case null:
                    case KeyState.CreatedKey:

                        if ( result ) {

                            switch ( Insert( aEntity ) ) {
                                case OperationResult.OK:
                                    result &= true;
                                    break;
                                case OperationResult.Fail:
                                    result &= false;
                                    break;
                                case OperationResult.Error:
                                    result &= false;
                                    break;
                                case OperationResult.Exception:
                                    result &= false;
                                    break;
                                case OperationResult.Unchange:
                                    result &= true;
                                    break;
                            }
                        }

                        break;

                    case KeyState.SetKey:
                        //result = Update() == OperationResult.OK;
                        if ( result ) {
                            switch ( Update( aEntity ) ) {
                                case OperationResult.OK:
                                    result &= true;
                                    break;
                                case OperationResult.Fail:
                                    result &= false;
                                    break;

                                case OperationResult.Error:
                                    result &= false;
                                    break;

                                case OperationResult.Exception:
                                    result &= false;
                                    break;
                                case OperationResult.Unchange:
                                    result &= true;
                                    break;

                            }
                        }

                        break;

                    case KeyState.DeleteKey:
                        if ( result ) {
                            switch ( Delete( aEntity ) ) {
                                case OperationResult.OK:
                                    result &= true;
                                    break;
                                case OperationResult.Fail:
                                    result &= false;
                                    break;

                                case OperationResult.Error:
                                    result &= false;
                                    break;

                                case OperationResult.Exception:
                                    result &= false;
                                    break;
                                case OperationResult.Unchange:
                                    result &= true;
                                    break;

                            }
                        }

                        break;

                    case KeyState.UnsetKey:
                        result = false;
                        throw new System.Exception( "Unset primary key" );
                }
            }

            return result;
        }

        public override U SelectLastInsertId<U>() {
            return mDatabase.ExecuteScalar<U>( SQLBuilder.sqlastInsertId<T>() );
        }

        public override U SelectSingleValue<U>( string aSqlQuery ) {
            return mDatabase.ExecuteScalar<U>( aSqlQuery );
        }

        public override object[] SelectSingleRow( string aSqlQuery , EntityConditionalExpression Constraint , params object[] values ) {

            object[] result = null;
            SQLQueryParams outparam = null;
            var s = SQLBuilder.sqlSelect( Entity , ref outparam , ParamPlaceholder: "@" , Constraint );
            var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );
            if ( rs != null ) {

                if ( rs.fetch() ) {

                    int? c = rs.columnCount();
                    if ( c != null ) {
                        result = new object[( int ) c];
                        for ( int i = 0; i < c; i++ ) {
                            result[i - 1] = rs.getObject( i );
                        }
                    }
                }

                rs.close();
                return result;

            } else
                return result;

        }

        public void CreateNew() {
            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );
            OnInitData( EventArgs.Empty );
            New();
        }

        public void BindData() {

            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );
            OnInitData( EventArgs.Empty );

        }

        public U defaultValue<U>( string propertyName ) {

            var pi = SQLTableEntityHelper.getPropertyIfColumnDefinition<T>().FirstOrDefault( x => x.Name.Equals( propertyName ) );
            var a = pi.GetCustomAttribute( typeof( DDColumnAttribute ) );
            //var isPK = pi.GetCustomAttribute( typeof( DDPrimaryKey ) );
            if ( a is not null ) {

                //if ( ( isPK is null ) && ( ( ( a as DDColumnAttribute ).NullValue == NullValue.NotNull ) && ( default( T ) == null ) ) ) {
                //    if ( typeof( U ) != typeof( string ) ) {
                //        throw new NullReferenceException( $"Default value for {propertyName} can't be null." );
                //    }
                //}

                if ( ( a as DDColumnAttribute ).DefaultValue != null ) {

                    if ( ( a as DDColumnAttribute ).DefaultValue.Equals( ( DefaultValues.AUTOINCREMENT ) ) ) {
                        var value = default( U );
                        //ReflectionTypeHelper.SetValueToColumn( ( AbstractEntity ) this , pi , ( ( a as DDColumnAttribute ).NullValue == NullValue.NotNull ) ? value : null);
                        return value;

                    } else if ( ( a as DDColumnAttribute ).DefaultValue.Equals( ( DefaultValues.CURDATE ) ) ) {
                        return default( U );
                    } else if ( ( a as DDColumnAttribute ).DefaultValue.Equals( ( DefaultValues.NULL ) ) ) {
                        var value = default( U );
                        //ReflectionTypeHelper.SetValueToColumn( ( AbstractEntity ) this , pi , ( ( a as DDColumnAttribute ).NullValue == NullValue.NotNull ) ? value : null );
                        return value;
                    } else {
                        return ( U ) ( a as DDColumnAttribute ).DefaultValue.ConvertTo<U>();

                    }
                } else {
                    if ( typeof( U ) == typeof( string ) ) {
                        if ( ( a as DDColumnAttribute ).NullValue == NullValue.NotNull ) {
                            return ( dynamic ) "";
                        } else {
                            return default( U );
                        }
                    } else {
                        return default( U );
                    }

                }
            }

            return default( U );
        }

        public void MapToEntity( AbstractEntity entity = null ) {
            var entityProperty = this.GetType().GetProperties().FirstOrDefault( x => x.PropertyType.IsAssignableTo( typeof( AbstractEntity ) ) );
            AbstractEntity Entity = entity ?? ( AbstractEntity ) ( entityProperty?.GetValue( this ) );
            if ( Entity != null ) {
                var fields = this.GetType().GetFields();
                var properties = Entity.GetType().GetProperties();
                foreach ( var field in fields ) {
                    var p = properties.FirstOrDefault( p => p.Name.Equals( field.Name ) );
                    if ( p != null ) {
                        var value = field.GetValue( this );
                        ReflectionTypeHelper.SetValueToColumn( ( AbstractEntity ) Entity , p , value );
                    }

                }

            }
        }

        public void MapFromAdapter( AbstractEntity entity = null ) {

            var entityProperty = this.GetType().GetProperties().FirstOrDefault( x => x.PropertyType.IsAssignableTo( typeof( AbstractEntity ) ) );
            AbstractEntity Entity = entity ?? ( AbstractEntity ) ( entityProperty?.GetValue( this ) );
            if ( Entity != null ) {
                var fields = this.GetType().GetFields();
                var properties = Entity.GetType().GetProperties();
                foreach ( var field in fields ) {
                    var p = properties.FirstOrDefault( p => p.Name.Equals( field.Name ) );
                    if ( p != null ) {

                        dynamic type = ReflectionTypeHelper.GetValueFromColumn( ( AbstractEntity ) Entity , p );

                        if ( type.IsNull ) {
                            field.SetValue( this , null );
                        } else {
                            field.SetValue( this , type.Value );
                        }

                    }

                }

                Entity.State = EntityState.Created;
            }
        }

        public void CopyFrom( AbstractEntity entity ) {
            if ( Entity is null ) {
                Create();
            }
            MapFromAdapter( entity );
            MapToEntity();
            if ( entity is not null )
                Entity.PrimaryKey.KeyState = entity.PrimaryKey.KeyState;

        }

        public IEnumerator<T> GetEnumerator() {
            return ( ( IEnumerable<T> ) this.mEntities ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ( ( IEnumerable ) this.mEntities ).GetEnumerator();
        }
    }

    public class BindDataFromEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }

    public class BindDataToEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }




}