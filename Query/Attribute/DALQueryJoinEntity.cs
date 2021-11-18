using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    public abstract class DALGenericQueryJoinConstraint : System.Attribute, IDALFunction {
        public Type Entity { get; set; }
        public string FieldName { get; set; }

        public abstract int CountParam();
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALQueryJoinEntityConstraint : DALGenericQueryJoinConstraint {
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
        public string Placeholder { get; set; }

        public DALQueryJoinEntityConstraint( Type entity , string fieldName ) {

            this.Entity = entity;
            this.FieldName = fieldName;
            this.Placeholder = "@";

        }

        public override int CountParam() {
            return 0;
        }
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]   
    public class DALQueryJoinSubQueryConstraint : DALGenericQueryJoinConstraint {
        public Type SubQuery { get; set; }
        public string FieldNameSubQuery { get; set; }

        public DALQueryJoinSubQueryConstraint( Type entity , string fieldName , Type subQuery ) {

            this.Entity = entity;
            this.FieldName = fieldName;
            this.SubQuery = subQuery;

        }

        public override int CountParam() {
            return 0;
        }
    }


    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALQueryJoinEntity : System.Attribute {
        public Type Entity;
        public string Alias;
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALQueryJoinEntityUnConstraint : System.Attribute {
        internal Type Entity;
        internal Type JoinTable;

        public DALQueryJoinEntityUnConstraint( Type entity , Type joinTable ) {
            this.Entity = entity;
            this.JoinTable = joinTable;
        }
    }


    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALQueryJoinEntityExpression : DALGenericQueryJoinConstraint {

        public string Function;
        public DALQueryJoinEntityExpression( Type entity , Type leftexpression , Type rightExpression , string @operator ) {
            this.Entity = entity;
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


    public class DALQueryJoinBetween : DALGenericQueryJoinConstraint {

        public Type Expression;
        public Type MinValue;
        public Type MaxValue;

        public DALQueryJoinBetween( Type entity , Type expression , Type minValue , Type maxValue ) {
            this.Entity = entity;
            this.Expression = expression;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public override int CountParam() {
            return 3;
        }
    }

}
