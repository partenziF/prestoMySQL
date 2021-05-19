using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;

namespace prestoMySQL.Column.Interface {
    public interface ConstructibleColumn {
        
        Type GenericType { get; }

        bool isNotNull { get; }

        bool isUnique { get; }

        bool isPrimaryKey { get; }
        bool isAutoIncrement { get; }

        public object Value();

        public TableReference Table { get; }

        ///////////////////////////////////////////
        public string ColumnName { get; }

    }

}