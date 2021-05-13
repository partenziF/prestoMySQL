using prestoMySQL.SQL.Interface;
using System;

namespace prestoMySQL.Column.Interface {
    public interface DefinableColumn<T> : ConstructibleColumn<T> where T : ISQLTypeWrapper { //where T :struct 

        bool isNotNull { get; }

        bool isUnique { get; }

        object DefaultValue { get; }

        //object ValueToParam { get; }
        //public string ValueToParamAsString { get; }
        object ValueAsParamType();
        //public object ValueToParam { get => ConvertValueToParam(); }


        bool isPrimaryKey { get; }
        bool isAutoIncrement { get; }


    }
}