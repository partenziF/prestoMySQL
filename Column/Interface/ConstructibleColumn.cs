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

        public object GetValue();

        public TableReference Table { get; }



        ///////////////////////////////////////////
        string ColumnName { get; }

        string Alias { get; }

        string ActualName { get; }

        void AssignValue( object x );

    }

}