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
    public abstract class ForeignKey {

        protected Type mTypeRefenceTable;
        public Type TypeRefenceTable { get => this.mTypeRefenceTable; set => this.mTypeRefenceTable = value; }


        protected AbstractEntity mRefenceTable;
        public AbstractEntity RefenceTable { get => mRefenceTable; set => mRefenceTable = value; }


        protected string mColumnName;
        public string ColumnName => mColumnName;

        protected string mReferenceColumnName;
        public string ReferenceColumnName => mReferenceColumnName;

        //private T[] mValues;
        //public T[] Values => mValues;

        protected PropertyInfo mPropertyInfo;
        public PropertyInfo Field => mPropertyInfo;


        private KeyState mKeyState = KeyState.Unset;
        public KeyState keyState { get => mKeyState; set => mKeyState = value; }



        protected AbstractEntity mTable;
        public AbstractEntity Table { get => mTable; set => mTable = value; }


        protected IDictionary<string , PropertyInfo> foreignKeyColumns;

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
        }

        public void InstantiateRefenceTable() {

            if ( mRefenceTable == null ) {

                this.mRefenceTable = ( AbstractEntity ) Activator.CreateInstance( mTypeRefenceTable );
            }

        }

    }
}
