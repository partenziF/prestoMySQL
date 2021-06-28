using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class EntityConditionalExpression : DefinableConditionalExpression {
        public EntityConditionalExpression( LogicOperator logicOperator , params dynamic[] aEntityConstraint ) {
            
            this.mLogicOperator = logicOperator;
            mConditions = aEntityConstraint.Where( x => ( x != null )).ToArray() ?? null;

        }

        public EntityConditionalExpression( LogicOperator aLogicOperator , EntityConditionalExpression aEntityConditionalExpressionLeft , EntityConditionalExpression aEntityConditionalExpressionRight ) {

            this.mLogicOperator = aLogicOperator;

            if ( ( aEntityConditionalExpressionLeft == null ) || ( aEntityConditionalExpressionRight == null ) ) throw new System.Exception( "Condition must not be null" );

            this.mConditionExpressions = new EntityConditionalExpression[2];
            this.mConditionExpressions[0] = aEntityConditionalExpressionLeft;
            this.mConditionExpressions[1] = aEntityConditionalExpressionRight;

        }

        //GenericEntityConstraint
        private dynamic[] mConditions; //GenericEntityConstraint
        private EntityConditionalExpression[] mConditionExpressions;
        private LogicOperator mLogicOperator;

        public LogicOperator LogicOperator { get => this.mLogicOperator; set => this.mLogicOperator = value; }
        public int Length { get => mConditions.Length; }

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
                foreach ( EntityConditionalExpression c in this.mConditionExpressions ) {
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

    public class EntityListExpression : EntityConditionalExpression {

        public EntityListExpression( params dynamic[] aSqlQueryConditions ) : base( LogicOperator.SEPARATOR , aSqlQueryConditions ) {
        }
    }

    public class EntityConstraintExpression : EntityConditionalExpression {
        public EntityConstraintExpression( params dynamic[] aSqlQueryConditions ) : base( LogicOperator.AND, aSqlQueryConditions ) {
        }
    }

}
