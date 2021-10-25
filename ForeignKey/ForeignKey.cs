using prestoMySQL.Entity;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey {

    public class ForeignKeyInfo {

        internal PropertyInfo mPropertyInfo;
        
        internal string mReferenceTableAlias;
        public string ReferenceTableAlias { get => mReferenceTableAlias; }

        public PropertyInfo Field => mPropertyInfo;


        internal AbstractEntity mReferenceTable;

        internal Type mTypeReferenceTable;
        //TypeReferenceTable
        public Type TypeReferenceTable { get => this.mTypeReferenceTable; set => this.mTypeReferenceTable = value; }

        public AbstractEntity ReferenceTable { get => mReferenceTable; set => mReferenceTable = value; }

        internal string mReferenceColumnName;
        public string ReferenceColumnName => mReferenceColumnName;


        internal AbstractEntity mTable;
        public AbstractEntity Table { get => mTable; set => mTable = value; }

        internal string mColumnName;
        public string ColumnName => mColumnName;

        public ForeignKeyInfo( PropertyInfo p , Type TypeReferenceTable,string referenceTableAlias , string referenceColumnName , AbstractEntity table , string columnName ) {

            mPropertyInfo = p;
            mReferenceTableAlias = referenceTableAlias;
            mTypeReferenceTable = TypeReferenceTable; //mTypeReferenceTable
            mReferenceColumnName = referenceColumnName;
            mTable = table;
            mColumnName = columnName;

        }

    }
    public abstract class ForeignKey {


        private KeyState mKeyState = KeyState.UnsetKey;
        public KeyState keyState { get => mKeyState; set => mKeyState = value; }

        public AbstractEntity Table { get => foreignKeyInfo.Select( x => x.Table ).Distinct().FirstOrDefault();  }

        //protected PropertyInfo mPropertyInfo;
        //public PropertyInfo Field => mPropertyInfo;

        //protected Type mTypeReferenceTable;
        //public Type TypeReferenceTable { get => this.mTypeReferenceTable; set => this.mTypeReferenceTable = value; }

        //protected AbstractEntity mReferenceTable;
        //public AbstractEntity ReferenceTable { get => mReferenceTable; set => mReferenceTable = value; }

        //protected string mReferenceColumnName;
        //public string ReferenceColumnName => mReferenceColumnName;

        //protected AbstractEntity mTable;
        //public AbstractEntity Table { get => mTable; set => mTable = value; }

        //protected string mColumnName;
        //public string ColumnName => mColumnName;

        protected IDictionary<string , PropertyInfo> foreignKeyColumns;
        //public Dictionary<Type , Dictionary<string, ForeignKeyInfo>> foreignKeysInfo { get; set; }

        public List<ForeignKeyInfo> foreignKeyInfo { get; set; }

        public PropertyInfo this[string index] {

            get {
                if ( foreignKeyColumns.ContainsKey( index ) ) {
                    return foreignKeyColumns[index];
                } else {
                    return null;
                }
            }

        }


        public ForeignKey( KeyState aKeyState ) {
            this.mKeyState = aKeyState;
            this.foreignKeyColumns = new Dictionary<String , PropertyInfo>();
            this.foreignKeyInfo = new List<ForeignKeyInfo>();
        }

        public void InstantiateReferenceTable() {

            foreach ( var fk in foreignKeyInfo.Where( x => x.mReferenceTable != null ).ToList() ) {

                fk.mReferenceTable = ( AbstractEntity ) Activator.CreateInstance( fk.mTypeReferenceTable );
                fk.mReferenceTable.mAliasName = fk.mReferenceTableAlias;
            }

            //if ( mReferenceTable == null ) {

            //    this.mReferenceTable = ( AbstractEntity ) Activator.CreateInstance( mTypeReferenceTable );
            //}

        }

    }


}
