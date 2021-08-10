using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using prestoMySQL.Entity;

namespace prestoMySQL.Index {

    public interface ITableIndex {

        public AbstractEntity Table { get; set; }

        public PropertyInfo this[string index] { get; }

    }

    public abstract class TableIndex {

        private AbstractEntity mTable;
        public AbstractEntity Table { get => this.mTable; set => this.mTable = value; }

        public virtual string[] ColumnsName {
            get {
                return this.IndexColumns.Keys.ToArray();
            }
        }

        protected TableIndex() {
            this.IndexColumns = new Dictionary<string , PropertyInfo>();
        }


        protected IDictionary<string , PropertyInfo> IndexColumns;

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
