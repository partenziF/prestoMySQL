using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using prestoMySQL.Entity;
using prestoMySQL.Index;

namespace prestoMySQL.PrimaryKey {
    public abstract class PrimaryKey : TableIndex, ITableIndex {

  //      private prestoMySQL.Entity.AbstractEntity mTable;
//        public AbstractEntity Table { get => this.mTable; set => this.mTable = value; }

        public KeyState mKeyState = KeyState.Unset;
        public KeyState KeyState { get => mKeyState; set => mKeyState = value; }

        
        //protected IDictionary<string , PropertyInfo> IndexColumns;        
        
        //public PropertyInfo this[string index] {

        //    get {
        //        if ( primaryKeyColumns.ContainsKey( index ) ) {
        //            return primaryKeyColumns[index];
        //        } else {
        //            return null;
        //        }
        //    }

        //}



        protected bool mAutoIncrement = true;
        public bool isAutoIncrement { get => mAutoIncrement; }

        protected PrimaryKey( KeyState aKeyState ) {
            this.mKeyState = aKeyState;
            this.IndexColumns = new Dictionary<String , PropertyInfo>();
        }

    }
}
