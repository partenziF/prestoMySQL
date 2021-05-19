using prestoMySQL.Column;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Entity.Interface;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Entity {
    // System.Collections.Generic.IDictionary<String,object> Map<String,object>
    public abstract class AbstractEntity : AvailableEntity {

        protected AbstractEntity() {

            this.mTableName ??= SQLTableEntityHelper.getTableName( this );
            this.foreignKeys = this.createForeignKey();
            this.mPrimaryKey = this.createPrimaryKey();
            mState = EntityState.Created;

            var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) : false ).ToArray();

            foreach ( PropertyInfo p in InstantiableProperties ) {
                var ctors = p.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( AbstractEntity ) } );
                p.SetValue( this , ctors.Invoke( new object[] { p.Name , p , this } ) , null );
            }

        }

        public ConcurrentQueue<EntityForeignKey> foreignKeys;

        public EntityState State { get => mState; set => mState = value; }


        protected string mTableName;
        public string TableName { get => this.mTableName; }


        #region PrimaryKey

        EntityPrimaryKey mPrimaryKey;
        public EntityPrimaryKey PrimaryKey { get => mPrimaryKey; }

        public void setPrimaryKey( EntityPrimaryKey aPrimaryKey , DelegateCreatePrimaryKey aDelegatorCreatePrimaryKey ) {
            this.mPrimaryKey = aPrimaryKey;
            this.mPrimaryKey.setDoCreatePrimaryKey( aDelegatorCreatePrimaryKey );
        }
        public AbstractEntity setCreatePrimaryKey( DelegateCreatePrimaryKey delegatorCreatePrimaryKey ) {
            this.mPrimaryKey.setDoCreatePrimaryKey( delegatorCreatePrimaryKey );
            return this;
        }
        public abstract EntityPrimaryKey createPrimaryKey();
        #endregion

        #region ForeignKey
        public abstract ConcurrentQueue<EntityForeignKey> createForeignKey();

        public virtual AbstractEntity setForeignKey( EntityForeignKey foreignKey , Delegate delegatorCreateForeignKey ) {
            throw new NotImplementedException();
            //foreignKey.setDoCreateForeignKey( delegatorCreateForeignKey );
            //this.foreignKey.add( foreignKey );
            //return this;
        }

        public virtual AbstractEntity setCreateForeignKey( Delegate delegatorCreateForeignKey ) {
            throw new NotImplementedException();
            //this.ForeignKey.setDoCreateForeignKey( delegatorCreateForeignKey );
            //return this;
        }

        
        protected void Entity_PropertyChanged( object? sender , PropertyChangedEventArgs e ) {
            mState = EntityState.Changed;
        }
        

        public virtual EntityForeignKey ForeignKey {

            get {
                if ( foreignKeys != null ) {
                    return foreignKeys.Last();
                } else {
                    return null;
                }
            }
        }

        private EntityState mState = EntityState.Undefined;
        
        #endregion

    }
}
