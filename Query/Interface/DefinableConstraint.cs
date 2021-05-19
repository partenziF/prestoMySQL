using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {
    public interface DefinableConstraint {
        string ParamPlaceHolder { get; set; }
        SQLQueryParams QueryParams { get; set; }
        EvaluableBinaryOperator BinaryOperator { get; set; }
        
        QueryParam[] getParam();
        String[] getParamAsString();
        int countParam();

    }       
}
