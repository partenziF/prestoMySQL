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
using DatabaseEntity.EntityAdapter;
using System.Runtime.CompilerServices;
using prestoMySQL.ForeignKey.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Index;
using prestoMySQL.Table;

namespace prestoMySQL.Entity {

    public abstract class AbstractEntity : AvailableEntity { //,IDictionary<Type , List<AbstractEntity>>

        private EntityState mState = EntityState.Undefined;
        public EntityState State { get => mState; set => mState = value; }

        private List<string> mFkNames;
        public List<string> FkNames { get => mFkNames; set => mFkNames = value; }

        //public IDictionary<Type , List<AbstractEntity>> EntityTree = new Dictionary<Type , List<AbstractEntity>>();

        public List<EntityForeignKey> mforeignKeys = new List<EntityForeignKey>();

        protected AbstractEntity() {

            this.mTableName ??= SQLTableEntityHelper.getTableName( this );
            //this.mAliasName ??= SQLTableEntityHelper.getTableAlias( this );

            mState = EntityState.Created;

            var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) : false ).ToArray();
            //Instantiate all MySQLDefinitionColumn
            foreach ( PropertyInfo p in InstantiableProperties ) {
                var ctors = p.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( AbstractEntity ) } );
                p.SetValue( this , ctors.Invoke( new object[] { p.Name , p , this } ) );
            }

            //Instantiate foreign key
            this.createForeignKey();

            //Instantiate primary key
            mPrimaryKey = this.createPrimaryKey();
            mFkNames = new List<string>();

        }


        protected string mTableName;
        public string TableName { get => this.mTableName; }

        internal string mAliasName;
        public string AliasName { get => mAliasName; }

        public string ActualName { get => mAliasName ?? mTableName; }

        #region PrimaryKey

        protected EntityPrimaryKey mPrimaryKey;
        public EntityPrimaryKey PrimaryKey { get => mPrimaryKey; }


        public TableReference tableReference { get => new TableReference( mTableName , mAliasName ); }

        //public ICollection<Type> Keys => this.EntityTree.Keys;

        //public ICollection<List<AbstractEntity>> Values => this.EntityTree.Values;

        //public int Count => this.EntityTree.Count;

        //public bool IsReadOnly => this.EntityTree.IsReadOnly;

        //public List<AbstractEntity> this[Type key] { get => this.EntityTree[key]; set => this.EntityTree[key] = value; }


        public void setPrimaryKey( EntityPrimaryKey aPrimaryKey , DelegateCreatePrimaryKey aDelegatorCreatePrimaryKey ) {
            this.mPrimaryKey = aPrimaryKey;
            this.mPrimaryKey.setDoCreatePrimaryKey( aDelegatorCreatePrimaryKey );
        }
        public AbstractEntity setCreatePrimaryKey( DelegateCreatePrimaryKey delegatorCreatePrimaryKey ) {
            this.mPrimaryKey.setDoCreatePrimaryKey( delegatorCreatePrimaryKey );
            return this;
        }

        #endregion


        public EntityUniqueIndex createUniqueIndex( [CallerMemberName] string memberName = "" , params object[] values ) {

            var u = new EntityUniqueIndex( memberName , this );
            u.setKeyValues( values );
            return u;

        }

        public EntityIndex createIndex( [CallerMemberName] string memberName = "" , params object[] values ) {

            var u = new EntityIndex( memberName , this );
            u.setKeyValues( values );
            return u;

        }

        public virtual void createForeignKey() {

            var fieldInfoForeignKey = ReflectionTypeHelper.FieldsWhereIsAssignableFrom( this.GetType() , typeof( EntityForeignKey ) );// this.GetType().GetFields().Where( x => typeof( EntityForeignKey ).IsAssignableFrom( x.FieldType ) ).ToArray();

            if ( fieldInfoForeignKey.Count > 0 ) {
                //Instantiate all Foreignkey property
                foreach ( FieldInfo fi in fieldInfoForeignKey ) {
                    ReflectionTypeHelper.InstantiateDeclaredClassToField( this ,
                                                                         fi ,
                                                                         new Type[] { typeof( AbstractEntity ) , typeof( string ) } ,
                                                                         new object[] { this , fi.Name } );

                    var _fk = ( EntityForeignKey ) fi.GetValue( this );
                    this.mforeignKeys.Add( _fk );

                }


            }

        }

        public virtual AbstractEntity setForeignKey( EntityForeignKey foreignKey , DelegateCreateForeignKey delegatorCreateForeignKey ) {

            foreignKey.setDoCreateForeignKey( delegatorCreateForeignKey );
            return this;
        }


        protected void Entity_PropertyChanged( object? sender , PropertyChangedEventArgs e ) {
            mState = EntityState.Changed;
        }

        public EntityPrimaryKey createPrimaryKey() {

            if ( this.GetType().GetProperties().Where( p => Attribute.IsDefined( p , typeof( DDPrimaryKey ) ) ).ToList().Count > 0 ) {

                var pkClass = this.GetType().GetNestedTypes( BindingFlags.NonPublic ).Where( x => typeof( EntityPrimaryKey ).IsAssignableFrom( x ) ).FirstOrDefault();
                if ( pkClass != null ) {
                    var ctor = pkClass.GetConstructor( new Type[] { typeof( AbstractEntity ) } );
                    return ( EntityPrimaryKey ) ( ctor?.Invoke( new object[] { this } ) );
                } else {
                    throw new System.Exception( "EntityPrimaryKey not declared." );

                }
            } else {
                return null;
            }
        }


        #region ForeignKey

        public List<EntityForeignKey> GetAllForeignkey() {

            List<EntityForeignKey> fks = new List<EntityForeignKey>();

            var fieldInfoForeignKey = ReflectionTypeHelper.FieldsWhereIsAssignableFrom<EntityForeignKey>( this.GetType() );

            foreach ( var fk in fieldInfoForeignKey ) {

                EntityForeignKey o;
                o = ( EntityForeignKey ) fk.GetValue( this );
                if ( o == null ) {
                    ReflectionTypeHelper.InstantiateDeclaredClassToField( this ,
                                                                          fk ,
                                                                          new Type[] { typeof( AbstractEntity ) , typeof( string ) } ,
                                                                          new object[] { this , fk.Name } );
                    o = ( EntityForeignKey ) fk.GetValue( this );


                }

                fks.Add( o );

            }

            return fks;
        }



        #endregion

        public AbstractEntity Copy() {

            AbstractEntity copy = ( AbstractEntity ) Activator.CreateInstance( this.GetType() );

            copy.mState = this.mState;
            this.mFkNames.ForEach( i => copy.mFkNames.Add( new String( i.ToCharArray() ) ) );
            copy.mAliasName = ( this.mAliasName is not null ) ? new String( this.mAliasName.ToCharArray() ) : null;


            var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) : false ).ToArray();
            //Instantiate all MySQLDefinitionColumn
            foreach ( PropertyInfo p in InstantiableProperties ) {

                var sourceCol = p.GetValue( this );
                var destCol = p.GetValue( copy );
                if ( ( sourceCol != null ) ) {

                    //ReflectionTypeHelper.SetValueToColumn( copy , p , ( sourceCol as dynamic ).TypeWrapperValue.Value );
                    ( destCol as dynamic ).TypeWrapperValue = ( sourceCol as dynamic ).TypeWrapperValue.Copy();
                    ( destCol as dynamic ).mTable = ( sourceCol as dynamic ).Table.Copy();
                }



                ////( v as dynamic ).TypeWrapperValue.Copy()
                //if ( ( v as dynamic ).TypeWrapperValue.IsNull ) {

                //    ( c as dynamic ).

                //} else {
                //    ( c as dynamic ).AssignValue( ( v as dynamic ).TypeWrapperValue.Value );
                //}

            }

            var copied = PrimaryKey.Copy();

            copy.mPrimaryKey.KeyState = PrimaryKey.KeyState;

            copy.State = EntityState.Created;
            return copy;

        }

    }
}
