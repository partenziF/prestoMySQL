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
                    if ( a != null ) {
                        var aa = p.GetCustomAttribute<DDColumnAttribute>();
                        this.primaryKeyColumns.Add( aa.Name , p );
                        this.mAutoIncrement = a.Autoincrement;
                    }
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
                        col.TypeWrapperValue = ( aValue );

                    } catch ( System.Exception e ) {
                        throw new System.Exception( "Error while " + nameof( setKeyValue ) );
                    }

                } else {
                    throw new System.Exception( "Field name " + aKey + " not exists" );
                }

            } catch ( System.InvalidCastException e ) {
                throw new System.InvalidCastException( "Cannot be cast " + e.ToString() );
            }

        }

        internal virtual void setKeyValues(params object[] values) {
            int i = 0;
            KeyState = KeyState.Set;
            
            if ( values.Length != KeyLength )
                throw new ArgumentOutOfRangeException( "Invalid key length" );

            AssignValues( values );
        }

        public void AssignValues(params object[] values ) {
            uint i = 0;
            foreach ( KeyValuePair<string , PropertyInfo> kvp in this ) {
                dynamic column = kvp.Value.GetValue( Table );
                column.AssignValue( values[i++] );
            }
        }


        public virtual void doCreatePrimaryKey() {
        
            if ( delegatorCreatePrimaryKey != null ) {
                this.delegatorCreatePrimaryKey( this.Table );//.createPrimaryKey();            
            } else {
                foreach ( var (name, pi) in primaryKeyColumns ) {

                    ReflectionTypeHelper.SetValueToColumn( this , pi , null );
                }
            }
            //		else
            //			throw new Exception("No delegate found for createPrimaryKey");
        }


        public void createKey() {

            KeyState = KeyState.Created;

            if ( !isAutoIncrement ) {
                doCreatePrimaryKey();
                //TODO check key lenght?
                //if ( KeyLength != values.Length ) {
                //    throw new System.Exception( "Invalid key length" );
                //}
            } else {

                foreach ( var (name, pi) in primaryKeyColumns ) {

                    ReflectionTypeHelper.SetValueToColumn( this , pi , null );

                }

            }

        }


        public void setDoCreatePrimaryKey( DelegateCreatePrimaryKey doCreatePrimaryKey ) {
            this.delegatorCreatePrimaryKey = doCreatePrimaryKey;
        }

        public virtual object[] getKeyValues() {
            List<object> result = new List<object>();
            foreach ( var (columnName, pi) in this.primaryKeyColumns ) {
                if ( Attribute.IsDefined( pi , typeof( DDColumnAttribute ) ) ) {
                    result.Add( ReflectionTypeHelper.GetValueFromColumn( this , pi ) );
                } else {
                    throw new SystemException( "Invalid key." );
                }

            }

            return result?.ToArray();
            //var p = this.Table.GetType().GetProperty( nameof( RolesEntity.PkRole ) );

            //if ( Attribute.IsDefined( p , typeof( DDColumnAttribute ) ) ) {
            //    object[] result = new object[] { base.getKeyValue<uint>( p.GetCustomAttribute<DDColumnAttribute>().Name ) };
            //    return result;
            //} else {
            //    throw new SystemException( "Invalid key." );
            //}

        }

        public T getKeyValue<T>( string aKey ) {

            if ( primaryKeyColumns.ContainsKey( aKey ) ) {
                try {
                    PropertyInfo p = primaryKeyColumns[aKey];
                    var col = ( MySQLDefinitionColumn<SQLTypeWrapper<T>> ) p?.GetValue( this.Table );
                    return ( T ) col.TypeWrapperValue;

                } catch ( System.Exception e ) {
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
            return ( ( IEnumerable ) primaryKeyColumns ).GetEnumerator();
        }

        public virtual string[] ColumnsName {
            get {
                return this.primaryKeyColumns.Keys.ToArray();
            }
        }

    }

}
