﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    public interface ICountableParam {
        int CountParam();
    }


    public abstract class DALFunctionParam : System.Attribute, ICountableParam {

        public abstract int CountParam();
    }



    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALFunctionParamConstant : DALFunctionParam {

        public object Value { get; set; }

        public override int CountParam() {
            return 0;
        }

        //public DALValueFunctionParamConstant( object Value ) {
        //    this.Value = Value;
        //}
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALFunctionParamSubQueryConstraint : DALFunctionParam {

        public Type Table { get; set; }
        public string Property { get; set; }
        public Type SubQuery { get; set; }


        public override int CountParam() {
            return 0;
        }

        //public DALValueFunctionParamConstant( object Value ) {
        //    this.Value = Value;
        //}
    }


    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALFunctionParamConstraint : DALFunctionParam {
        public string Property { get; set; }
        public Type Table { get; set; }
        public object Value { get; set; }

        public override int CountParam() {
            return 0;
        }

    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALFunctionParamBetween : DALFunctionParam {
        public Type Expression;
        public Type MinValue;
        public Type MaxValue;

        public override int CountParam() {
            return 3;
        }

    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALFunctionParamExpression : DALFunctionParam {
        public Type leftExpression;
        public Type rightExpression;
        public string @operator;

        public override int CountParam() {
            return 2;
        }

    }


    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALFunctionParamProperty : DALFunctionParam {

        public string Property { get; set; }
        public Type Table { get; set; }

        public override int CountParam() {
            return 0;
        }

    }




    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]    
    public class DALFunctionParamFunction : DALFunctionParam {

        public Type Expression { get; set; }
        public string Function { get; set; }
        public Type[] Params { get; set; }

        public override int CountParam() {
            return ( Params is null ) ? 0 : Params.Length;
        }

    }


    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]    
    public class DALFunctionParamQueryParam : DALFunctionParam {

        //public Type Expression { get; set; }
        public Type DataType { get; set; }
        public string Value { get; set; }
        public string ParamName { get; set; }
        public string ParamPlaceHolder { get; set; }


        public override int CountParam() {
            return 0;
        }

    }


    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
    public class DALAlias : DALFunctionParam {
        public override int CountParam() {
            throw new NotImplementedException();
        }
    }





}