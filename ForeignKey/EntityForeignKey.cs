using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.ForeignKey.Interface;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey {
    public abstract class EntityForeignKey : ForeignKey, IForeignKey, IObserverColumn {

        private JoinType mJoinType;

        private readonly int mKeyLength;
        public int KeyLength { get => mKeyLength; }
        public JoinType JoinType { get => this.mJoinType; set => this.mJoinType =  value ; }

        private DelegateCreateForeignKey delegatorCreateForeignKey = null;

        //public Stack<EntityPrimaryKey[]> stackPrimaryKey;

        protected EntityForeignKey( AbstractEntity aTableEntity , string ForeignkeyName ) : base( KeyState.Unset ) {

            this.Table = aTableEntity;
            initialize( aTableEntity , ForeignkeyName );
            mKeyLength = this.foreignKeyColumns.Count;

        }


        private void initialize( AbstractEntity aTableEntity , string foreignkeyName ) {
            try {

                List<PropertyInfo> l = SQLTableEntityHelper.getPropertyIfForeignKey( aTableEntity.GetType() );

                foreach ( PropertyInfo p in l ) {

                    var a = p.GetCustomAttribute<DDForeignKey>();
                    if ( a != null ) {

                        if ( a.Name == foreignkeyName ) {
                            var aa = p.GetCustomAttribute<DDColumnAttribute>();
                            this.foreignKeyColumns.Add( aa.Name , p );
                            this.mColumnName = aa.Name;
                            this.mReferenceColumnName = a.Reference;
                            this.TypeRefenceTable = a.TableReferences;
                            this.JoinType = aa.NullValue == NullValue.NotNull ? JoinType.INNER : JoinType.LEFT;

                            this.mPropertyInfo = p;
                        }

                    }

                }

            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while initialize primarykey" );
            }

        }



        public virtual void doCreateForeignKey() {
            if ( delegatorCreateForeignKey != null ) {
                this.delegatorCreateForeignKey( this );
            }
        }

        public virtual void createKey() {

            keyState = KeyState.Created;

            doCreateForeignKey();

        }


        public abstract object[] getKeyValues();

        public T getKeyValue<T>( string aKey ) {

            if ( foreignKeyColumns.ContainsKey( aKey ) ) {
                try {
                    //var col = ( MySQLDefinitionColumn<SQLTypeWrapper<T>> ) p?.GetValue( this.Table );
                    PropertyInfo p = foreignKeyColumns[aKey];
                    dynamic col = p?.GetValue( this.Table );
                    //return ( T ) col.TypeWrapperValue;
                    return ( T ) col.GetValue();

                } catch ( System.Exception e ) {
                    throw new System.Exception( "Error while read key value" );
                }
            } else {
                throw new System.ArgumentException( "Invalid Key name" );
            }

        }


        //public abstract Dictionary<string , EntityPrimaryKey> peekPrimaryKey();

        public void setDoCreateForeignKey( DelegateCreateForeignKey doCreateForeignKey ) {
            this.delegatorCreateForeignKey = doCreateForeignKey;
        }

        public virtual void setKeyValues( params object[] values ) {
            this.keyState = KeyState.Set;
            if ( values.Length != KeyLength )
                throw new System.ArgumentOutOfRangeException( "Invalid key length" );

        }


        public abstract void addEntities( List<AbstractEntity> foreignKeyTables );

        public override string ToString() {

            var a = SQLTableEntityHelper.getColumnName( TypeRefenceTable , ReferenceColumnName , true , true );
            var b = SQLTableEntityHelper.getColumnName( Table.GetType() , ColumnName , true , true );

            var s = string.Format( "{0} JOIN {1} ON\r\n\t{2} = {3}" , this.JoinType.ToString() , SQLTableEntityHelper.getTableName( TypeRefenceTable ) , a , b );
            return s;

        }

        public void Update( IObservableColumn subjectColumn ) {
            //throw new NotImplementedException();
            if ( Attribute.IsDefined( this.Field , typeof( DDPrimaryKey ) ) ) {
                PropertyInfo pi = this.RefenceTable.GetType().GetProperty( this.ReferenceColumnName );
                if ( pi != null ) {
                    dynamic c = pi?.GetValue( this.RefenceTable );
                    c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                }
            } else {
                dynamic c = this.Field.GetValue( this.Table );
                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
            }

            createKey();

        }

        public void setKeyValue<T1>( string aKey , T1 aValue ) {
            //var pi = this.RefenceTable.GetType().GetProperty( this.ReferenceColumnName );
            var pi = this.RefenceTable.GetType().GetProperty( aKey );
            if ( pi == null ) new NullReferenceException( "Property " + aKey + " not found." );
            dynamic col = pi.GetValue( this.RefenceTable );
            col.AssignValue( aValue ?? ReflectionTypeHelper.SQLTypeWrapperNULL( col?.GenericType ) );
        }
    }
}
