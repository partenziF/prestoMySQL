using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLQueryConditionExpression : DefinableConditionalExpression {
        public SQLQueryConditionExpression() {
        }

        private GenericEntityConstraint<object>[] mConditions;
        private SQLQueryConditionExpression[] mConditionExpressions;

        public LogicOperator mLogicOperator;


        public enum LogicOperator {
            AND,
            OR
        }

        public int countParam() {
            throw new NotImplementedException();
        }

        public object[] getParam() {
            throw new NotImplementedException();
        }

        public string[] getParamAsString() {
            throw new NotImplementedException();
        }

        QueryParam[] DefinableConditionalExpression.getParam() {
            throw new NotImplementedException();
        }
    }
}
