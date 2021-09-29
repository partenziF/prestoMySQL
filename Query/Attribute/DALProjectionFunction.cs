using prestoMySQL.Column.DataType;
using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    public enum OrderType {
        ASC, DESC
    }

    public interface IDALFunction {



        public abstract int CountParam();

    }


    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public abstract class DALProjectionFunction : System.Attribute, IDALFunction {
        public string Function { get; set; }
        public MySQLDataType DataType { get; set; }
        public OrderType Sort { get; set; }
        public int OrderBy { get; set; }

        public DALProjectionFunction( String function , MySQLDataType Type ) {
            this.Function = function;
            //this.Params = Params;
            this.DataType = Type;
        }
        public DALProjectionFunction( String function ) {
            this.Function = function;
        }

        public abstract int CountParam();
    }

    public class DALProjectionFunction_CURDATE : DALProjectionFunction {
        public DALProjectionFunction_CURDATE( string Alias = null ) : base( "CURDATE" , MySQLDataType.dbtDate ) {
        }

        public override int CountParam() {
            return 0;
        }
    }

    public class DALProjectionFunction_IF : DALProjectionFunction {
        public DALProjectionFunction_IF( Type expression , Type ifTrue , Type ifFalse ) : base( "IF" , MySQLDataType.dbtTinyInt ) {
            Expression = expression;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public Type Expression { get; set; }
        public Type ifTrue { get; set; }
        public Type ifFalse { get; set; }



        public override int CountParam() {
            return 3;
        }

    }

    public class DALProjectionFunction_SUM : DALProjectionFunction {
        public DALProjectionFunction_SUM( Type expression , MySQLDataType mySQLDataType ) : base( "SUM" , mySQLDataType ) {
            Expression = expression;
        }

        public Type Expression { get; set; }



        public override int CountParam() {
            return 1;
        }

    }

    public class DALProjectionFunction_MAX : DALProjectionFunction {
        public DALProjectionFunction_MAX( Type expression , MySQLDataType mySQLDataType ) : base( "MAX" , mySQLDataType ) {
            Expression = expression;
        }

        public Type Expression { get; set; }



        public override int CountParam() {
            return 1;
        }

    }

    public class DALProjectionFunction_DATE_FORMAT : DALProjectionFunction {
        public DALProjectionFunction_DATE_FORMAT( Type expression , Type format ) : base( "DATE_FORMAT" , MySQLDataType.dbtVarChar ) {
            Expression = expression;
            Format = format;
        }

        public Type Expression { get; set; }
        public Type Format { get; set; }



        public override int CountParam() {
            return 2;
        }

    }

    public class DALProjectionFunctionExpression : DALProjectionFunction {
        public DALProjectionFunctionExpression( Type leftexpression , Type rightExpression , string @operator ) : base( "" , MySQLDataType.dbtTinyInt ) {
            this.leftexpression = leftexpression;
            this.rightExpression = rightExpression;
            this.@operator = @operator;

        }
        public Type leftexpression { get; set; }
        public Type rightExpression { get; set; }
        public string @operator { get; }

        public override int CountParam() {
            return 2;
        }

    }





}
