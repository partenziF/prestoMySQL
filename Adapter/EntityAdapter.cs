using MySqlConnector;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Adapter {

    public abstract class EntityAdapter<T> : TableAdapter, IList<T> where T : AbstractEntity {

        public EntityAdapter( MySQLDatabase aMySQLDatabase ) {

            this.mDatabase = aMySQLDatabase;

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

        public MySQLDatabase mDatabase;

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
        private T mCurrentEntity;
        private void SetEntity( T value ) {
            mEntities.Add( value );
            mCurrentEntity = value;
        }
        protected T Entity { get => mCurrentEntity; set => SetEntity( value ); }

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

            //if (definitionColumns == null)

            var _definitionColumns = SQLTableEntityHelper.getDefinitionColumn<T>( Entity , true );


            //_ = rs.ResultSetSchemaTable();
            foreach ( var column in _definitionColumns ) {

                //need explicit conversion to work
                if ( rs[( string ) column.ColumnName].IsDBNull() ) {

                    var o = typeof( SQLTypeWrapper<> ).MakeGenericType( column.GenericType );
                    var p = o.GetField( "NULL" , BindingFlags.Static | BindingFlags.Public );
                    column.TypeWrapper = p.GetValue( null );

                } else {

                    MethodInfo method = typeof( MySQResultSet ).GetMethod( nameof( MySQResultSet.getValueAs ) , new Type[] { typeof( string ) } );
                    MethodInfo generic = method.MakeGenericMethod( column.GenericType );
                    var o = generic.Invoke( rs , new object[] { ( string ) column.ColumnName } );
                    column.AssignValue( o );

                }

            }

        }



        public override OperationResult DropTable( bool ifExists ) {

            var s = SQLBuilder.sqlDrop<T>();
            int? i = -1;
            try {

                i = mDatabase.ExecuteQuery( s ) ?? null;

                if ( i != -1 ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {
                return OperationResult.Error;
            } catch ( System.Exception e ) {

                throw new System.Exception( "Error insert query " + mDatabase.LastError?.ToString() ?? e.Message );
            }

            //return ( i != -1 ) ? true : false;

        }

        public override OperationResult CreateTable( bool ifExists ) {

            var s = SQLBuilder.sqlCreate<T>();
            int? i = -1;

            try {

                i = mDatabase.ExecuteQuery( s ) ?? null;

                if ( i != -1 ) return OperationResult.OK;
                else return OperationResult.Fail;

            } catch ( MySqlException ex ) {

                return OperationResult.Error;

            } catch ( System.Exception e ) {
                i = -1;
                throw new System.Exception( "Error insert query " + mDatabase.LastError?.ToString() ?? e.Message );
            }
        }

        public override OperationResult Create( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            this.Entity = ( T ) CreateEntity();

            if ( KeyValues?.Length == 0 ) {

                CreateNew();
                return OperationResult.OK;

            } else {

                int i = 0;
                if ( KeyValues.Length == Entity.PrimaryKey.KeyLength ) {

                    foreach ( KeyValuePair<string , PropertyInfo> kvp in this.Entity.PrimaryKey ) {

                        dynamic x = kvp.Value.GetValue( Entity );
                        x.AssignValue( KeyValues[i++] );
                    }

                    return this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                } else {
                    throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
                }

            }

        }


        protected override OperationResult Select( EntityConditionalExpression Constraint , params object[] values ) {

            SQLQueryParams outparam = null;
            //this.Entity
            var s = SQLBuilder.sqlSelect<T>( Entity , ref outparam , "@" , Constraint );
            var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            if ( rs != null ) {

                if ( rs.fetch() ) {

                    BindData( rs );

                    OnBindDataFrom( new BindDataFromEventArgs<T>() { Entity = this.Entity } );

                    if ( rs.fetch() ) {
                        Entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        throw new System.Exception( "Primary key entity violation" );
                    } else {
                        Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        return OperationResult.OK;
                    }


                } else {
                    return OperationResult.Fail;
                }

            } else {
                return OperationResult.Error;
            }


        }

        protected override OperationResult Insert() {

            if ( Entity == null ) throw new System.Exception( "entity is null, call create method." );
            SQLQueryParams outparam = null;

            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );

            if ( Entity.State == prestoMySQL.Entity.Interface.EntityState.Changed ) {

                var s = SQLBuilder.sqlInsert<T>( Entity , ref outparam , "@" );

                int? rowInserted = -1;
                try {

                    rowInserted = mDatabase.ExecuteQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                } catch ( MySqlException ex ) {
                    return OperationResult.Exception;
                } catch ( System.Exception e ) {
                    rowInserted = -1;
                    throw new System.Exception( "Error insert query " + mDatabase.LastError?.ToString() ?? e.Message );
                }

                if ( rowInserted is null ) {

                    return OperationResult.Error;

                } else if ( rowInserted != -1 ) {

                    object[] primaryKeyValues = null;
                    if ( Entity.PrimaryKey.isAutoIncrement ) {
                        primaryKeyValues = this.Entity.PrimaryKey.doCreatePrimaryKey();
                    } else {
                        primaryKeyValues = this.Entity.PrimaryKey.getKeyValues();
                    }

                    SetPrimaryKey( primaryKeyValues );

                    Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;

                    return OperationResult.OK;

                } else {
                    return OperationResult.Fail;
                        
                }

            } else {
                return OperationResult.OK; //Unchanged data
            }

        }

        protected override OperationResult Update() {

            if ( Entity == null ) throw new System.Exception( "entity is null, call create method." );
            SQLQueryParams outparam = null;

            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );

            if ( Entity.State == prestoMySQL.Entity.Interface.EntityState.Changed ) {

                var s = SQLBuilder.sqlUpdate<T>( Entity , ref outparam , "@" );
                int? rowChanged = -1;
                try {

                    rowChanged = mDatabase.ExecuteQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? null;

                } catch ( MySqlException ex ) {
                
                    return OperationResult.Exception;
                
                } catch ( System.Exception e ) {

                    throw new System.Exception( "Error insert query " + mDatabase.LastError?.ToString() ?? e.Message );
                }


                if ( rowChanged is null ) {

                    return OperationResult.Error;

                } else if ( rowChanged != -1 ) {

                    Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                    return OperationResult.OK;
                } else {
                    return OperationResult.Fail;
                }

            } else {
                return OperationResult.Fail;
            }

        }

        public override bool Save() {

            this.mEntities.First();
            bool result = false;

            if ( this.Entity == null ) {
                throw new System.Exception( "entity is null, call create method." );

            }

            foreach ( T aEntity in this.mEntities ) {

                switch ( aEntity.PrimaryKey.KeyState ) {

                    case KeyState.Created:
                    result = Insert() == OperationResult.OK;

                    break;

                    case KeyState.Set:
                    result = Update() == OperationResult.OK;
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
            var s = SQLBuilder.sqlSelect<T>( Entity , ref outparam , "@" , Constraint );
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


        public override void SetPrimaryKey( params object[] values ) {
            this.Entity.PrimaryKey.setKeyValues( values );
        }

        public override object[] CreatePrimaryKey() {
            return Entity.PrimaryKey.createKey();
        }

        public override void createForeignKey() {
            if ( this.Entity.ForeignKey != null ) {
                foreach ( EntityForeignKey fk in this.Entity.foreignKeys ) {
                    fk.addEntities( foreignKeyTables );
                }
            }
        }

        protected override void CreateNew() {
            createForeignKey();
            CreatePrimaryKey();
        }

        public void newEntity() {

            var args = new BindDataToEventArgs<T> { Entity = this.Entity };
            OnBindDataTo( args );

            Clear();

            OnInitData( EventArgs.Empty );

        }


        #region List implementation
        public T this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int Count => throw new System.NotImplementedException();

        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add( T item ) {
            throw new System.NotImplementedException();
        }

        public void Clear() {
            throw new System.NotImplementedException();
        }

        public bool Contains( T item ) {
            throw new System.NotImplementedException();
        }

        public void CopyTo( T[] array , int arrayIndex ) {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public int IndexOf( T item ) {
            throw new System.NotImplementedException();
        }

        public void Insert( int index , T item ) {
            throw new System.NotImplementedException();
        }

        public bool Remove( T item ) {
            throw new System.NotImplementedException();
        }

        public void RemoveAt( int index ) {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }
        #endregion
    }


    public class BindDataFromEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }

    public class BindDataToEventArgs<U> : EventArgs {
        public U Entity { get; set; }
    }

}