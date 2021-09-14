using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLBinaryOperator : EvaluableBinaryOperator {

        private readonly string @operator;

        public SQLBinaryOperator( string @operator ) : base() {
            this.@operator = @operator;
        }

        public override string ToString() {
            return this.@operator;
        }

        public static SQLBinaryOperator assign() {
            return new SQLBinaryOperator( "=" );
        }

        public static SQLBinaryOperator equal() {
            return new SQLBinaryOperator( "=" );
        }

        public static SQLBinaryOperator notEqual() {
            return new SQLBinaryOperator( "<>" );
        }

        public static SQLBinaryOperator lessThan() {
            return new SQLBinaryOperator( "<" );
        }

        public static SQLBinaryOperator lessThanEqual() {
            return new SQLBinaryOperator( "<=" );
        }

        public static SQLBinaryOperator greatThan() {
            return new SQLBinaryOperator( ">" );
        }

        public static SQLBinaryOperator greatThanEqual() {
            return new SQLBinaryOperator( ">=" );
        }

        public static SQLBinaryOperator @in() {
            return new SQLBinaryOperator( "IN" );
        }

        public static SQLBinaryOperator notIn() {
            return new SQLBinaryOperator( "NOT IN" );
        }

        public static SQLBinaryOperator like() {
            return new SQLBinaryOperator( "LIKE" );
        }


        public static SQLBinaryOperator @is() {
            return new SQLBinaryOperator( "IS" );
        }

        public static SQLBinaryOperator isNot() {
            return new SQLBinaryOperator( "IS NOT" );
        }

        public static SQLBinaryOperator between() {
            return new SQLBinaryOperator( "BETWEEN" );
        }

        public override bool Equals( object obj ) {
            return obj is SQLBinaryOperator @operator &&
                     this.@operator == @operator.@operator;
        }

        public override int GetHashCode() {
            return HashCode.Combine( this.@operator );
        }
    }
}
