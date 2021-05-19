using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Interface {
    public interface QueryableColumn<T> : ConstructibleColumn where T : ISQLTypeWrapper {
        public T Value { get; set; }

        string Alias { get; set; }
        bool isNullValue { get; set; }

    }
}
