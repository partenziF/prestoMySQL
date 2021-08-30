using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Interface {
    public interface QueryableColumn  {

        Type GenericType { get; }

        //public TableReference Table { get; }

        /////////////////////////////////////////////
        //public string ColumnName { get; }

        public string ColumnAlias { get; }

        public abstract string ActualName { get; }


    }

}
