using Microsoft.Extensions.Logging;
using MySqlConnector;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Adapter {

    public abstract class EntityAdapter<T> : TableAdapter where T : AbstractEntity {

        private const string ERROR_EXECUTE_QUERY = "Error execute query ";
        //private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();
        public EntityAdapter( MySQLDatabase aMySQLDatabase , ILogger logger ) {

            this.mDatabase = aMySQLDatabase;
            this.mLogger = logger;

            CreateEvents();

            //this.BuildEntityGraph();

        }

        #region sezione eventi
        public event EventHandler InitData;
        public virtual void OnInitData( EventArgs e ) {
            EventHandler handler = InitData;
            handler?.Invoke( this , e );
        }


        public delegate void BindDataFromEventHandler( Object sender , BindDataFromEventArgs<T> e );
        public event BindDataFromEventHandler BindDataFrom;
        protected virtual void OnBindDataFrom( BindDataFromEventArgs<T> e ) {
            BindDataFromEventHandler handler = BindDataFrom;
            handler?.Invoke( this , e );
        }


        public delegate void BindDataToEventHandler( Object sender , BindDataToEventArgs<T> e );
        public event BindDataToEventHandler BindDataTo;
        protected virtual void OnBindDataTo( BindDataToEventArgs<T> e ) {
            BindDataToEventHandler handler = BindDataTo;
            handler?.Invoke( this , e );
        }


        #endregion

        private List<AbstractEntity> foreignKeyTables = new List<AbstractEntity>();
        public readonly MySQLDatabase mDatabase;
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

            //var s = rs.ResultSetSchemaTable();
            //int? index = null;

            foreach ( ConstructibleColumn column in _definitionColumns ) {

                //index = null;

                //if ( s.ContainsKey( column.Table.ActualName ) ) {
                //    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                //        index = s[column.Table.ActualName][column.ActualName];
                //    }
                //}

                //if ( index == null ) throw new System.Exception( "Invalid index column." );

                //need explicit conversion to work
                if ( rs[( string ) column.ColumnName].IsDBNull() ) {

                    var o = typeof( SQLTypeWrapper<> ).MakeGenericType( column.GenericType );
                    var p = o.GetField( nameof( SQLTypeWrapper<object>.NULL ) , BindingFlags.Static | BindingFlags.Public );
                    //dynamic xx = p.GetValue( null );
                    //var xxx = Convert.ChangeType( xx , o );

                    ( column as dynamic ).TypeWrapperValue = ( dynamic ) p.GetValue( null ); ;// ( SQLTypeWrapper <uint?> )p.GetValue( null );

                } else {

                    //var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                    //                                       typeof( MySQResultSet ) ,
                    //                                       rs ,
                    //                                       nameof( MySQResultSet.getValueAs ) ,
                    //                                       new Type[] { typeof( int ) } ,
                    //                                       new object[] { ( int ) index } );

                    //if ( v.IsDBNull() ) {
                    //    ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                    //} else {
                    //    column.AssignValue( v );
                    //}

                    MethodInfo method = typeof( MySQResultSet ).GetMethod( nameof( MySQResultSet.getValueAs ) , new Type[] { typeof( string ) } );
                    MethodInfo generic = method.MakeGenericMethod( column.GenericType );
                    var o = generic.Invoke( rs , new object[] { ( string ) column.ColumnName } );
                    column.AssignValue( o );

                }

            }

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
                else if ( !String.IsNullOrWhiteSpace( r ) && ( r.Equals( SQLTableEntityHelper.getTableName<T>() , StringComparison.InvariantCultureIgnoreCase ) ) ) return OperationResult.OK;
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

            if ( rs != null ) {

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

                    do {

                        if ( attributes.ContainsKey( rs.getValueAs<string>( "Field" ) ) ) {
                            var a = attributes[rs.getValueAs<string>( "Field" )];
                            var sClassType = a.BulldTypeString().RemoveAllWhitespace();
                            var sDbType = rs.getValueAs<string>( "Type" ).RemoveAllWhitespace();
                            if ( sClassType.Equals( sDbType , StringComparison.InvariantCultureIgnoreCase ) ) {
                                //ok
                            } else {
                                //fail
                            }

                            if ( ( rs.getValueAs<string>( "Null" ).Equals( "YES" , StringComparison.InvariantCultureIgnoreCase ) ) && ( a.NullValue == NullValue.Null ) ) {
                                //ok
                            } else {
                                //fail
                            }

                            var sDbNull = rs.getValueAs<string>( "Null" );

                            if ( ( rs.getValueAs<string>( "Null" ).Equals( "NO" , StringComparison.InvariantCultureIgnoreCase ) ) && ( a.NullValue == NullValue.NotNull ) ) {
                                //ok
                            } else {
                                //fail
                            }

                            if ( a.GetDefaultValueClause().Equals( rs.getValueAs<string>( "Default" ) , StringComparison.InvariantCultureIgnoreCase ) ) {

                            }


                        } else {
                            //allFieldsExists = false;
                        }


                    } while ( rs.fetch() );

                    rs?.close();
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

        }

        public OperationResult Create() {

            Entity ??= CreateInstace<T>();
            InitEntity();

            //CreateInstace<T>();
            if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };

            Entity.PrimaryKey.createKey();
            createForeignKey();
            CreatePrimaryKey();

            return OperationResult.OK;
        }


        public override OperationResult New() {

            //CreateInstace<T>( true );
            Entity = CreateInstace<T>();
            InitEntity();
            if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };

            Entity.PrimaryKey?.createKey();
            createForeignKey();
            CreatePrimaryKey();

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

        public override OperationResult Read( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            //CreateInstace<T>();
            if ( Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };
            //InitEntity();

            try {

                Entity.PrimaryKey.setKeyValues( KeyValues );
                //var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                SQLQueryParams outparam = null;
                var s = SQLBuilder.sqlSelect( Entity , ref outparam , ParamPlaceholder: "@" , Constraint );
                var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs );

                if ( r == OperationResult.OK ) {
                    Entity.PrimaryKey.KeyState = KeyState.Set;
                } else {
                    Entity.PrimaryKey.KeyState = KeyState.Unset;
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }


        }

        public OperationResult Read<X>( Func<T , X> delegateMethod ) where X : EntityUniqueIndex {

            //CreateInstace<T>();
            if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };
            //InitEntity();

            X x = delegateMethod( Entity );

            DefinableConstraint[] constraints = new DefinableConstraint[x.ColumnsName.Length];

            int i = 0;
            foreach ( string c in x.ColumnsName ) {

                PropertyInfo p = x[c];
                var col = p.GetValue( this.Entity );
                DefinableConstraint o = FactoryEntityConstraint.MakeConstraintEqual( col , "@" );
                constraints[i++] = o;
            }

            SQLQueryParams outparam = null;
            var s = SQLBuilder.sqlSelect<T>( ref outparam , new EntityConstraintExpression( constraints ) );
            var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            var r = FetchResultSet( rs );

            if ( r == OperationResult.OK ) {
                Entity.PrimaryKey.KeyState = KeyState.Set;
            } else {
                Entity.PrimaryKey.KeyState = KeyState.Unset;
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

                var s = SQLBuilder.sqlInsert<T>( ( T ) entity , ref outparam , "@" );

                int? rowInserted = -1;
                try {

                    rowInserted = mDatabase.ExecuteQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                    if ( rowInserted is null ) {

                        return OperationResult.Error;

                    } else if ( rowInserted != -1 ) {

                        object[] primaryKeyValues = null;
                        switch ( entity.PrimaryKey?.isAutoIncrement ) {

                            case true:
                            entity.PrimaryKey.doCreatePrimaryKey();
                            SetPrimaryKey();
                            entity.PrimaryKey.KeyState = KeyState.Set;
                            break;
                            case false:
                            primaryKeyValues = entity.PrimaryKey.getKeyValues();
                            if ( entity.PrimaryKey.isAutoIncrement ) {
                                SetPrimaryKey();
                            }
                            entity.PrimaryKey.KeyState = KeyState.Set;
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

                var s = SQLBuilder.sqlUpdate<T>( ( T ) entity , ref outparam , "@" );
                int? rowChanged = -1;
                try {

                    rowChanged = mDatabase.ExecuteQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

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
                return OperationResult.Fail;
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
                    case KeyState.Created:

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
                        }
                    }

                    break;

                    case KeyState.Set:
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

                        }
                    }

                    break;

                    case KeyState.Unset:
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


        //protected override void CreateNew() {
        //    this.BuildEntityGraph();
        //    Entity.PrimaryKey.createKey();
        //    createForeignKey();
        //    CreatePrimaryKey();
        //}

        public void BindData() {


            //if ( this.Entity is null ) { new ArgumentNullException( "Entity can't be null." ); };
            //CreateNew();
            //return OperationResult.OK;

            //this.Entity = ( T ) Activator.CreateInstance( typeof( T ) );
            //CreateEntity();

            //CreateNew();

            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );
            OnInitData( EventArgs.Empty );

        }


        //private void BuildEntityGraph() {

        //    //mGraph.Clear();

        //    //if ( ( this.Entity.mforeignKeys != null ) && ( this.Entity.mforeignKeys.Count > 0 ) ) {

        //    //    Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();
        //    //    this.Entity.mforeignKeys.ForEach( fk => {

        //    //        mGraph.Connect( this.Entity , fk );

        //    //        fk.InstantiateRefenceTable();
        //    //        mGraph.Add( fk.RefenceTable );
        //    //        fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );


        //    //    } );

        //    //    EntityForeignKey fk = null;
        //    //    while ( foreignKeys.TryPop( out fk ) ) {

        //    //        //Avoid circular refernce
        //    //        if ( fk.TypeRefenceTable != this.Entity.GetType() ) {
        //    //            //this.Entity.mforeignKeys.Add( fk );
        //    //            fk.InstantiateRefenceTable();
        //    //            mGraph.Add( fk.RefenceTable );
        //    //            fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );

        //    //            foreignKeyTables.Add( fk.RefenceTable );
        //    //        }                    

        //    //    }

        //    //    //var _Entities = mGraph.Keys.ToList();
        //    //    //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
        //    //    //    foreach ( var _fk in mGraph[e] ) {

        //    //    //        if ( _fk.RefenceTable != null ) {
        //    //    //            _fk.addEntities( _Entities );
        //    //    //            _fk.createKey();

        //    //    //        }
        //    //    //    }
        //    //    //}

        //    //} else {
        //    //    mGraph.Add( this.Entity );
        //    //}
        //}


        //public int IndexOf( AbstractEntity item ) {
        //    return ( ( IList<AbstractEntity> ) this.foreignKeyTables ).IndexOf( item );
        //}

        //public void Insert( int index , AbstractEntity item ) {
        //    ( ( IList<AbstractEntity> ) this.foreignKeyTables ).Insert( index , item );
        //}

        //public void RemoveAt( int index ) {
        //    ( ( IList<AbstractEntity> ) this.foreignKeyTables ).RemoveAt( index );
        //}

        //public void Add( AbstractEntity item ) {
        //    ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).Add( item );
        //}

        //public void Clear() {
        //    ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).Clear();
        //}

        //public bool Contains( AbstractEntity item ) {
        //    return ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).Contains( item );
        //}

        //public void CopyTo( AbstractEntity[] array , int arrayIndex ) {
        //    ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).CopyTo( array , arrayIndex );
        //}

        //public bool Remove( AbstractEntity item ) {
        //    return ( ( ICollection<AbstractEntity> ) this.foreignKeyTables ).Remove( item );
        //}

        //public IEnumerator<AbstractEntity> GetEnumerator() {
        //    return ( ( IEnumerable<AbstractEntity> ) this.foreignKeyTables ).GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator() {
        //    return ( ( IEnumerable ) this.foreignKeyTables ).GetEnumerator();
        //}


    }


    public class BindDataFromEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }

    public class BindDataToEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }


}