using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using prestoMySQL.Entity;

namespace prestoMySQL.Index {
    public abstract class TableIndex {

        private AbstractEntity mTable;
        public AbstractEntity Table { get => this.mTable; set => this.mTable = value; }

        protected IDictionary<string , PropertyInfo> IndexColumns;

        protected TableIndex() {
            this.IndexColumns = new Dictionary<string , PropertyInfo>();
        }

        public PropertyInfo this[string index] {

            get {
                if ( IndexColumns.ContainsKey( index ) ) {
                    return IndexColumns[index];
                } else {
                    return null;
                }
            }

        }


    }
}
