using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class GenericEntityConstraintExpression : DefinableConditionalExpression {
        public GenericEntityConstraintExpression( LogicOperator logicOperator , params dynamic[] aSqlQueryConditions ) {
            this.mLogicOperator = logicOperator;

            if ( aSqlQueryConditions.Length > 0 ) {
                this.mConditions = new dynamic[aSqlQueryConditions.Length]; // new GenericEntityConstraint[aSqlQueryConditions.Length];
                int i = 0;
                foreach ( dynamic sc in aSqlQueryConditions ) {
                    this.mConditions[i++] = sc;
                }
            } else {
                mConditions = null;
            }

        }

        public GenericEntityConstraintExpression( LogicOperator aLogicOperator , GenericEntityConstraintExpression aSQLQueryConditionExpression1 , GenericEntityConstraintExpression aSQLQueryConditionExpression2 ) {

            this.mLogicOperator = aLogicOperator;

            if ( ( aSQLQueryConditionExpression1 == null ) || ( aSQLQueryConditionExpression2 == null ) ) throw new System.Exception( "Condition must not be null" );

            this.mConditionExpressions = new GenericEntityConstraintExpression[2];
            this.mConditionExpressions[0] = aSQLQueryConditionExpression1;
            this.mConditionExpressions[1] = aSQLQueryConditionExpression2;

        }


        private dynamic[] mConditions; //GenericEntityConstraint
        private GenericEntityConstraintExpression[] mConditionExpressions;
        private LogicOperator mLogicOperator;

        public LogicOperator LogicOperator { get => this.mLogicOperator; set => this.mLogicOperator = value; }


        public override string ToString() {

            if ( ( mConditions != null ) && ( mConditions.Length > 0 ) ) {

                string[] s = new string[mConditions.Length];
                int i = 0;
                //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
                //ORIGINAL LINE: for (SQLQueryCondition<?> c : mConditions)
                foreach ( dynamic c in mConditions ) {
                    s[i++] = c.ToString();
                }

                switch ( mLogicOperator ) {
                    case LogicOperator.AND:
                    return string.Format( "( {0} )" , string.Join( " AND " , s ) );
                    case LogicOperator.OR:
                    return string.Format( "( {0} )" , string.Join( " OR " , s ) );
                    case LogicOperator.NOT:
                    //return string.Format( "( {0} )" , string.Join( " OR " , s ) );
                    throw new NotImplementedException();
                    default:
                    throw new NotImplementedException();
                    //return string.Format( "( {0} )" , string.Join( " AND " , s ) );
                }

            } else if ( ( mConditionExpressions != null ) && ( mConditionExpressions.Length > 0 ) ) {
                return string.Format( "( {0} ) {1} ( {2} )" , this.mConditionExpressions[0].ToString() , this.mLogicOperator , this.mConditionExpressions[1].ToString() );
            } else {
                return "";
            }

        }


        public int countParam() {
            int i = 0;
            if ( mConditions != null ) {
                foreach ( dynamic c in mConditions ) {
                    i += c.countParam();
                }
            }
            if ( mConditionExpressions != null ) {
                foreach ( GenericEntityConstraintExpression c in this.mConditionExpressions ) {
                    i += c.countParam();
                }
            }

            return i;
        }

        public QueryParam[] getParam() {

            List<QueryParam> result = new List<QueryParam>();

            if ( mConditions != null ) {
                foreach ( dynamic c in mConditions ) {
                    foreach ( var x in c.getParam() ) {
                        result.Add( ( QueryParam ) x );
                    }
                    //result = result.Concat( c.getParam() ).ToArray();
                }
            }

            return result.ToArray();
        }

        public string[] getParamAsString() {
            throw new NotImplementedException();
        }
    }
}
