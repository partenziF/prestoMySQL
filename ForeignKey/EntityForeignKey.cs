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
        public JoinType JoinType { get => this.mJoinType; set => this.mJoinType = value; }

        private string mForeignkeyName;
        public string ForeignkeyName { get => mForeignkeyName; }

        private DelegateCreateForeignKey delegatorCreateForeignKey = null;

        //public Stack<EntityPrimaryKey[]> stackPrimaryKey;

        protected EntityForeignKey( AbstractEntity aTableEntity , string ForeignkeyName ) : base( KeyState.UnsetKey ) {

            this.mForeignkeyName = ForeignkeyName;
            //this.Table = aTableEntity;
            initialize( aTableEntity , ForeignkeyName );
            mKeyLength = this.foreignKeyColumns.Count;

        }


        private void initialize( AbstractEntity aTableEntity , string foreignkeyName ) {

            try {

                List<PropertyInfo> l = SQLTableEntityHelper.getPropertyIfForeignKey( aTableEntity.GetType() );

                foreach ( PropertyInfo p in l ) {

                    foreach ( var a in p.GetCustomAttributes<DDForeignKey>() )

                        if ( a != null ) {

                            if ( a.Name == foreignkeyName ) {

                                var aa = p.GetCustomAttribute<DDColumnAttribute>();

                                this.foreignKeyColumns.Add( aa.Name , p );

                                //this.TypeReferenceTable = a.TableReferences;

                                //this.mReferenceColumnName = a.Reference;
                                //this.mColumnName = aa.Name;
                                this.JoinType = aa.NullValue == NullValue.NotNull ? JoinType.INNER : JoinType.LEFT;

                                //this.mPropertyInfo = p;


                                this.foreignKeyInfo.Add( new ForeignKeyInfo( p , a.TableReferences , a.TableAlias , a.Reference , aTableEntity , aa.Name ) );

                                //if ( !foreignKeyInfo.ContainsKey( a.TableReferences ) ) {
                                //    foreignKeyInfo.Add( a.TableReferences , new Dictionary<string , ForeignKeyInfo>() );
                                //    foreignKeyInfo[a.TableReferences].Add( foreignkeyName , new ForeignKeyInfo( p , a.TableReferences , a.Reference , aTableEntity , aa.Name ) );

                                //} else {
                                //    foreignKeyInfo[a.TableReferences].Add( foreignkeyName , new ForeignKeyInfo( p , a.TableReferences , a.Reference , aTableEntity , aa.Name ) );
                                //}

                            }

                        }

                }

            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while initialize primarykey" );
            }

        }

        public bool IsIdentifyingRelationship {
            get {
                bool? result = null;
                foreach ( var info in this.foreignKeyInfo ) {
                    if ( info.ReferenceTable != null ) {
                        if ( result is null )
                            result = info.ReferenceTable.PrimaryKey.ColumnsName.Contains( info.mReferenceColumnName );
                        else
                            result &= info.ReferenceTable.PrimaryKey.ColumnsName.Contains( info.mReferenceColumnName );
                    }
                }

                return result ?? false;
            }


        }

        public virtual void doCreateForeignKey( params string[] columnNames ) {
            if ( delegatorCreateForeignKey != null ) {
                this.delegatorCreateForeignKey( this , columnNames );
            }
        }

        public virtual void createKey( params string[] columnNames ) {

            keyState = KeyState.CreatedKey;

            doCreateForeignKey( columnNames );

        }


        public abstract object[] getKeyValues();

        public T getKeyValue<T>( string aKey ) {

            if ( foreignKeyColumns.ContainsKey( aKey ) ) {
                try {
                    foreach ( var fk in foreignKeyInfo ) {

                        PropertyInfo p = foreignKeyColumns[aKey];
                        dynamic col = p?.GetValue( fk.Table );
                        if ( col != null ) {
                            return ( T ) col.GetValue();
                        }
                    }
                } catch ( System.Exception e ) {
                    throw new System.Exception( "Error while read key value" );
                }

                throw new IndexOutOfRangeException( "Invalid key name " );

            } else {
                throw new System.ArgumentException( "Invalid Key name" );
            }

        }


        //public abstract Dictionary<string , EntityPrimaryKey> peekPrimaryKey();

        public void setDoCreateForeignKey( DelegateCreateForeignKey doCreateForeignKey ) {
            this.delegatorCreateForeignKey = doCreateForeignKey;
        }

        public virtual void setKeyValues( params object[] values ) {
            this.keyState = KeyState.SetKey;
            if ( values.Length != KeyLength )
                throw new System.ArgumentOutOfRangeException( "Invalid key length" );

        }


        public abstract void addEntities( List<AbstractEntity> foreignKeyTables );

        public override string ToString() {

            var j = new Dictionary<string , List<string>>();

            foreach ( var info in this.foreignKeyInfo ) {

                var t = SQLTableEntityHelper.getAttributeTableName( info.TypeReferenceTable );

                var a = SQLTableEntityHelper.getColumnName( info.TypeReferenceTable , info.ReferenceColumnName , true , true );
                var b = SQLTableEntityHelper.getColumnName( info.Table.GetType() , info.ColumnName , true , true );


                if ( j.ContainsKey( t ) ) {
                    j[t].Add( $"{a} = {b}" );
                } else {
                    j.Add( t , new List<string>() { $"{a} = {b}" } );
                }

            }

            var sb = new StringBuilder();

            foreach ( var (joinTable, constraint) in j ) {

                sb.Append( string.Format( "{0} JOIN {1} ON " , this.JoinType.ToString() , joinTable ) );
                sb.AppendLine( String.Join( " AND " , constraint.ToArray() ) );

            }

            return sb.ToString();

        }

        public abstract void Update( IObservableColumn subjectColumn );

        //if ( Attribute.IsDefined( this.Field , typeof( DDPrimaryKey ) ) ) {


        //    //PropertyInfo pi = this.ReferenceTable.GetType().GetProperty( this.ReferenceColumnName );
        //    //if ( pi != null ) {
        //    //    dynamic c = pi?.GetValue( this.ReferenceTable );
        //    //    c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //    //}

        //    Dictionary<string , ForeignKeyInfo> infos;
        //    if ( this.foreignKeysInfo.TryGetValue( ( subjectColumn as dynamic ).TypeTable, out infos ) ) {

        //        foreach(var(_, info) in infos ) { 

        //            if ( (info.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( info.ReferenceColumnName == (subjectColumn as dynamic).ColumnName ) ) {

        //                PropertyInfo pi = info.ReferenceTable.GetType().GetProperty( info.ReferenceColumnName );
        //                if ( pi != null ) {
        //                    dynamic c = pi?.GetValue( info.ReferenceTable );
        //                    c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //                }


        //            }
        //        }

        //    }
        //} else {
        //    dynamic c = this.Field.GetValue( this.Table );
        //    c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //}

        //createKey();

        //}

        public void setKeyValue<T1>( string aKey , T1 aValue ) {

            foreach ( var fk in foreignKeyInfo ) {

                var pi = fk.ReferenceTable.GetType().GetProperty( aKey );
                if ( pi == null ) new NullReferenceException( "Property " + aKey + " not found." );
                dynamic col = pi.GetValue( fk.ReferenceTable );
                col.AssignValue( aValue ?? ReflectionTypeHelper.SQLTypeWrapperNULL( col?.GenericType ) );
            }

        }



        //public AbstractEntity ReferenceTables() {
        //    if ( KeyLength > 0 ) {

        //        foreach ( var (t, _) in foreignKeysInfo ) {

        //            foreach ( var (name, _) in foreignKeysInfo[t] ) {
        //                if ( foreignKeysInfo[t][name].mReferenceTable != null )
        //                    return foreignKeysInfo[t][name].mReferenceTable;
        //            }
        //        }

        //        return null;

        //    } else {
        //        return mReferenceTable;
        //    }
        //}


    }
}
