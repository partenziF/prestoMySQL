using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {
    public interface DefinableConstraint<T> {
        string ParamPlaceHolder { get; set; }
        SQLQueryParams QueryParams { get; set; }
        EvaluableBinaryOperator BinaryOperator { get; set; }

        T[] ColumnValue();
        QueryParam[] getParam();
        String[] getParamAsString();
        int countParam();

    }       
}
