using prestoMySQL.SQL.Interface;
using System;

namespace prestoMySQL.Column.Interface {
    public interface DefinableColumn<T>  where T : ISQLTypeWrapper { //where T :struct 


        public T TypeWrapperValue { get; set; }

        object DefaultValue { get; }

        object ValueAsParamType();
        
        void AssignValue( object x );

    }

}