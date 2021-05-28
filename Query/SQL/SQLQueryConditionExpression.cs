using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLQueryConditionExpression : DefinableConditionalExpression {
        public SQLQueryConditionExpression( LogicOperator logicOperator , params dynamic[] aSqlQueryConditions ) {
            this.mLogicOperator = logicOperator;
            mConditions = aSqlQueryConditions.Where( x => ( x != null ) ).ToArray() ?? null;

        }

        public SQLQueryConditionExpression( LogicOperator aLogicOperator , SQLQueryConditionExpression aSQLQueryConditionExpression1 , SQLQueryConditionExpression aSQLQueryConditionExpression2 ) {

            this.mLogicOperator = aLogicOperator;

            if ( ( aSQLQueryConditionExpression1 == null ) || ( aSQLQueryConditionExpression2 == null ) ) throw new System.Exception( "Condition must not be null" );

            this.mConditionExpressions = new SQLQueryConditionExpression[2];
            this.mConditionExpressions[0] = aSQLQueryConditionExpression1;
            this.mConditionExpressions[1] = aSQLQueryConditionExpression2;

        }

        private dynamic[] mConditions; //GenericEntityConstraint

        private SQLQueryConditionExpression[] mConditionExpressions;

        public LogicOperator mLogicOperator;


        public override string ToString() {

            if ( ( mConditions != null ) && ( mConditions.Length > 0 ) ) {

                //string[] s = new string[mConditions.Length];
                //int i = 0;
                ////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
                ////ORIGINAL LINE: for (SQLQueryCondition<?> c : mConditions)
                //foreach ( dynamic c in mConditions ) {
                //    s[i++] = c.ToString();
                //}

                var s = mConditions.Select( x => x.ToString() ).ToArray();

                switch ( mLogicOperator ) {
                    case LogicOperator.AND:
                    return ( mConditions.Length == 1 ) ? string.Join( " AND " , s ) : string.Format( "( {0} )" , string.Join( " AND " , s ) );
                    case LogicOperator.OR:
                    return ( mConditions.Length == 1 ) ? string.Join( " OR " , s ) : string.Format( "( {0} )" , string.Join( " OR " , s ) );
                    case LogicOperator.NOT:
                    return ( mConditions.Length == 1 ) ? string.Format( " NOT {0} " , s ) : throw new InvalidOperationException();
                    //return string.Format( "( {0} )" , string.Join( " OR " , s ) );
                    //throw new NotImplementedException();
                    case LogicOperator.SEPARATOR:
                    return string.Join( " , " , s );

                    default:
                    throw new ArgumentOutOfRangeException();
                    //return string.Format( "( {0} )" , string.Join( " AND " , s ) );
                }

            } else if ( ( mConditionExpressions != null ) && ( mConditionExpressions.Length > 0 ) ) {
                var exprLeft = this.mConditionExpressions[0].ToString();
                if ( this.mConditionExpressions[0].mConditions.Length > 1 ) exprLeft = String.Concat( "( " , exprLeft , " )" );

                var exprRigth = this.mConditionExpressions[1].ToString();
                if ( this.mConditionExpressions[1].mConditions.Length > 1 ) exprRigth = String.Concat( " " , exprRigth , " " );

                //var s1 = this.mConditionExpressions[0].mConditions.Length == 1 ?  string.Concat( "( " , this.mConditionExpressions[0].ToString() );
                return string.Format( "( {0}  {1}  {2} )" , exprLeft , this.mLogicOperator , exprRigth );
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
                foreach ( SQLQueryConditionExpression c in this.mConditionExpressions ) {
                    i += c.countParam();
                }
            }

            return i;
        }


        public QueryParam[] getParam() {

            List<QueryParam> result = new List<QueryParam>();

            if ( mConditions != null ) {
                foreach ( dynamic c in mConditions ) {
                    if ( c != null ) {
                        foreach ( var x in c.getParam() ) {
                            result.Add( ( QueryParam ) x );
                        }
                    }

                }

            }

            if ( mConditionExpressions?.Length == 2 ) {
                foreach ( dynamic c in mConditionExpressions ) {
                    foreach ( var x in c.getParam() ) {
                        result.Add( ( QueryParam ) x );
                    }

                }

            }

            return result.ToArray();
        }

    }
}
