using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
    class _GenericEntityConstraint<T> : DefinableConstraint<T> {


        public _GenericEntityConstraint( DefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator , T aColumnValue , char aParamPlaceholder , bool valueAsParam ) {

            if ( aColumnDefinition != null ) {
                this.columnDefinition = aColumnDefinition;

                if ( valueAsParam ) {
                    this.columnDefinition.Value = aColumnValue;
                } else {
                    //this.columnValue = ( T ) aColumnDefinition.getParam();
                }

                this.BinaryOperator = ( SQLBinaryOperator ) aOperator;

                if ( aParamPlaceholder.ToString() == String.Empty ) {
                    this.mParamPlaceHolder = String.Empty;
                } else {
                    this.mParamPlaceHolder = aParamPlaceholder.ToString();
                }

            } else {
                throw new System.Exception( "Invalid Table Column" );
            }


        }

        protected DefinitionColumn<SQLTypeWrapper<T>> columnDefinition;
        protected T mColumnValue;
        public T ColumnValue { get => mColumnValue; set => mColumnValue = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }

        protected SQLBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();


        public SQLBinaryOperator BinaryOperator {
            get => mBinaryOperator;
            set => mBinaryOperator = value;
        }
        public QueryParam QueryParam { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        EvaluableBinaryOperator DefinableConstraint<T>.BinaryOperator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IQueryParams QueryParams { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        SQLQueryParams DefinableConstraint<T>.QueryParams { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual int countParam() {
            return 1;
        }

        public virtual object[] getParam() {
            throw new NotImplementedException();
            //return new Object[] { 
            //    //columnDefinition.ValueAsParamType 
            //};
        }

        public virtual string[] getParamAsString() {
            throw new NotImplementedException();
        //    return new string[] {
        //        //this.columnDefinition.ValueAsParamType.ToString();                
        //};
        }


        public override string ToString() {

            try {
                if ( !String.IsNullOrWhiteSpace( this.mParamPlaceHolder) ) {

                    return string.Format( "{0} {1} {2}" , columnDefinition.ToString() , this.BinaryOperator.ToString() , mParamPlaceHolder );
                } else {

                    return string.Format( "{0} {1} {2}" , columnDefinition.ToString() , this.BinaryOperator.ToString() , columnDefinition.ValueAsParamType() );
                }

            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }


        T[] DefinableConstraint<T>.ColumnValue() {
            throw new NotImplementedException();
        }

        QueryParam[] DefinableConstraint<T>.getParam() {
            throw new NotImplementedException();
        }
    }
}
