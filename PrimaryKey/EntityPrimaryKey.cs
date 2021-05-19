using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.SQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.PrimaryKey {
    public abstract class EntityPrimaryKey : PrimaryKey, IPrimaryKey, IEnumerable {

        private readonly int mKeyLength;
        public int KeyLength { get => mKeyLength; }

        private DelegateCreatePrimaryKey delegatorCreatePrimaryKey = null;

        // Constructor ///////////////////////////////
        public EntityPrimaryKey( AbstractEntity aTableEntity ) : base( KeyState.Unset ) {

            this.Table = aTableEntity;
            initialize( aTableEntity );
            mKeyLength = primaryKeyColumns.Count;
        }
        public virtual void initialize( AbstractEntity aTableEntity ) {
            try {

                List<PropertyInfo> l = SQLTableEntityHelper.getPropertyIfPrimaryKey( aTableEntity.GetType() );

                foreach ( PropertyInfo p in l ) {

                    var a = p.GetCustomAttribute<DDPrimaryKey>();
                    var aa = p.GetCustomAttribute<DDColumnAttribute>();
                    this.primaryKeyColumns.Add( aa.Name , p );
                    this.mAutoIncrement = a.Autoincrement;
                }

            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while initialize primarykey" );
            }


        }

        public string[] getColumnsName() {
            return this.primaryKeyColumns.Keys.ToArray();
        }

        protected void setKeyValue<T>( string aKey , T aValue ) where T : notnull {

            try {

                if ( primaryKeyColumns.ContainsKey( aKey ) ) {

                    System.Reflection.PropertyInfo f = this.primaryKeyColumns[aKey];
                    
                    try {

                        var col = ( MySQLDefinitionColumn<SQLTypeWrapper<T>> ) f.GetValue( this.Table );
                        col.TypeWrapper = ( aValue );

                    } catch ( System.Exception e )  {
                        throw new System.Exception( "Error while " + nameof( setKeyValue ) );
                    }

                } else {
                    throw new System.Exception( "Field name " + aKey + " not exists" );
                }

            } catch ( System.InvalidCastException e ) {
                throw new System.InvalidCastException( "Cannot be cast " + e.ToString() );
            }

        }


        public virtual void setKeyValues( params object[] values ) {
            KeyState = KeyState.Set;
            if ( values.Length != KeyLength )
                throw new System.Exception( "Invalid key length" );
        }

        public virtual object[] doCreatePrimaryKey() {
            if ( delegatorCreatePrimaryKey != null ) {
                var x = this.delegatorCreatePrimaryKey();//.createPrimaryKey();
                return x;
            } else {
                object[] result = new object[KeyLength];
                Array.Fill( result , null );
                return result;
            }
            //		else
            //			throw new Exception("No delegate found for createPrimaryKey");
        }


        public object[] createKey() {

            object[] values;
            KeyState = KeyState.Created;

            if ( !isAutoIncrement ) {
                values = doCreatePrimaryKey();
                if ( KeyLength != values.Length ) {
                    throw new System.Exception( "Invalid key length" );
                }
            } else {
                values = new object[KeyLength];
                Array.Fill( values , null );//todo init array with default key value!
            }

            return values;
        }


        public void setDoCreatePrimaryKey( DelegateCreatePrimaryKey doCreatePrimaryKey ) {
            this.delegatorCreatePrimaryKey = doCreatePrimaryKey;
        }

        public abstract object[] getKeyValues();

        public T getKeyValue<T>(string aKey ) {

            if ( primaryKeyColumns.ContainsKey( aKey ) ) {
                try {
                    PropertyInfo p = primaryKeyColumns[aKey];
                    var col = ( MySQLDefinitionColumn<SQLTypeWrapper<T>> ) p?.GetValue( this.Table );
                    return ( T ) col.TypeWrapper;

                } catch (System.Exception) {
                    throw new System.Exception( "Error while read key value" );
                }
            } else {
                throw new System.ArgumentException( "Invalid Key name" );
            }
        }

        public IEnumerator GetEnumerator() {
            return primaryKeyColumns.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator() {
            //forces use of the non-generic implementation on the Values collection
            return ( ( IEnumerable ) primaryKeyColumns).GetEnumerator();
        }

        public virtual string[] ColumnsName {
            get {
                return this.primaryKeyColumns.Keys.ToArray();
            }
        }

    }

}
