using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;

namespace prestoMySQL.Column.Interface {
    public interface ConstructibleColumn<T> where T : ISQLTypeWrapper { //where T: struct

        //Type of generic
        Type TypeClass => typeof( T );

        public TableReference Table { get;  }

        ///////////////////////////////////////////

        public string ColumnName { get;  }


        ////////////////////////////////////////////////////

        public T Value { get; set; }

        ////////////////////////////////////////////////////
    }

}