using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Query;
using prestoMySQL.SQL;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace prestoMySQL.Adapter {
    public abstract class EntityAdapter<T> : TableAdapter, IList<T> where T : GenericEntity {


        public EntityAdapter( MySQLDatabase aMySQLDatabase ) {
            this.mDatabase = aMySQLDatabase;

            //if ( this.listener != null ) {
            //    listener.onInitData();
            //}

        }

        private List<GenericEntity> foreignKeyTables = new List<GenericEntity>();

        //delegati
        //protected CreatePrimaryKey<T> createPrimaryKey;

        //protected CreateForeignKey<T> createForeignKey;

        public MySQLDatabase mDatabase;


        //////////////////////////////////////////////////////////////////////////////
        // Gestore degli eventi
        //public EntityAdapterListener<T> listener;

        //public void setEntityAdapterListener( EntityAdapterListener<T> aListener ) {
        //    this.listener = aListener;
        //}

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
            foreach ( GenericEntity e in foreignKeyTables ) {

                if ( e.GetType().Equals( entity ) ) {
                    return e.TableName;
                }
                //if (entity.isInstance(e)) {
                //	return e.getTableName();
                //}

            }

            throw new System.Exception( "Invalid entity" );


        }

        public abstract GenericEntity createEntity();

        public void bindData( IReadableResultSet<DbDataReader> rs ) {

            new NotImplementedException();

            //if (definitionColumns == null)
            //	definitionColumns = SQLTableEntityHelper.getColumnDefinitionInstance(this.getEntity());

            foreach ( DefinableColumn<SQLTypeWrapper<Object>> column in definitionColumns ) {
                //column.TypeClass
                //object p = rs.getValueAs<>( column.ColumnName );
                //this.getEntity().put( column.ColumnName, rs.getValueAs< column.TypeClass>( column.TypeClass, column.ColumnName));
            }

        }


        protected override bool Select( params object[] values ) {
            
            SQLQueryParams outparam = null;
            
            String sqlQuery = SQLBuilder.sqlSelect<T>( Entity , ref  outparam );
            MySQResultSet rs = mDatabase.ExecuteQuery( sqlQuery , outparam );
            if (rs != null) {
                if ( rs.fetch() ) {

                } else {
                    return false;
                }
            } else {
                return false;
            }
            



            return false;


        }


        protected override bool Insert() {
            throw new NotImplementedException();
        }

        protected override bool Update() {
            throw new NotImplementedException();
        }

        public override bool Save() {
            throw new NotImplementedException();
        }

        public override U SelectLastInsertId<U>() {
            throw new NotImplementedException();
        }

        public override U SelectSingleValue<U>( string aSqlQuery ) {
            throw new NotImplementedException();
        }

        public override object[] SelectSingleRow( string aSqlQuery ) {
            throw new NotImplementedException();
        }

        public override bool DropTable( bool ifExists ) {
            throw new NotImplementedException();
        }

        public override bool CreateTable( bool ifExists ) {
            throw new NotImplementedException();
        }

        public override void SetPrimaryKey( params object[] values ) {
            throw new NotImplementedException();
        }

        public override object[] CreatePrimaryKey() {
            throw new NotImplementedException();
        }

        public override void createForeignKey() {
            throw new NotImplementedException();
        }

        public override GenericEntity CreateEntity() {
            throw new NotImplementedException();
        }

        protected override void CreateNew() {
            throw new NotImplementedException();
        }

        public override bool Create( params object[] aKeyValues ) {
            throw new NotImplementedException();
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // List Implementation
        /////////////////////////////////////////////////////////////////////////////////////////////////////

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
    }
}