using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class EntityConstraint<T> : GenericEntityConstraint<T> {

        public EntityConstraint( DefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator ,  IQueryParams aQueryPararm , string aParamPlaceHolder  = "") :
            base( aColumnDefinition , aOperator ,   aQueryPararm , aParamPlaceHolder ) {
        }


    }
}
