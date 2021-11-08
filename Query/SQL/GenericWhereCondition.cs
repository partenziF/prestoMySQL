using prestoMySQL.Column;
using prestoMySQL.Extension;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class GenericWhereCondition<T> : DefinableConstraint {


        public GenericWhereCondition( SQLProjectionColumn<SQLTypeWrapper<T>> columnDefinition , EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = "" ) {

            this.columnDefinition = columnDefinition ?? throw new ArgumentNullException( nameof( columnDefinition ) );
            this.mBinaryOperator = mBinaryOperator;
            this.mParamPlaceHolder = paramPlaceHolder;
            this.mQueryParams = ( SQLQueryParams ) ( aQueryPararms ?? throw new ArgumentNullException( nameof( aQueryPararms ) ) );
        }


        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }

        protected SQLProjectionColumn<SQLTypeWrapper<T>> columnDefinition;


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        public override string ToString() {

            //return String.Concat( "( " , columnDefinition.AsCondition() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " )" );
            return String.Concat( "( " , columnDefinition.Table.ActualName.QuoteTableName() , "." , columnDefinition.ColumnName.QuoteColumnName() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " )" );
        }


        public virtual int countParam() {
            return 1;
        }

        public virtual QueryParam[] getParam() {
            //return new QueryParam[] { ( QueryParam ) this.QueryParams[0] };

            if ( this.QueryParams.asArray().Length > 0 ) {

                if ( this.QueryParams[0]?.Value?.GetType().IsArray ?? false ) {

                    var l = ( ( Array ) this.QueryParams[0].Value ).Length;
                    var result = new QueryParam[l];
                    int i = 0;
                    foreach ( var v in ( Array ) this.QueryParams[0].Value ) {

                        //result[i] = new QueryParam( v , string.Format( "{0}_{1]" , this.QueryParams[0].Name , i )  );
                        result[i] = new MySQLQueryParam( v , String.Concat( this.QueryParams[0].Name , "_" , i.ToString() ) );
                        i++;
                    }

                    return result;

                } else {
                    if ( this.QueryParams.asArray().Length == 1 ) {
                        return new QueryParam[] { ( QueryParam ) this.QueryParams[0] };
                    } else {
                        return this.QueryParams.asArray();
                    }
                }
            } else {
                return this.QueryParams.asArray();
            }
        }

        public virtual string[] getParamAsString() {
            return new string[] { this.QueryParams[0].AsQueryParam() };
        }

        public virtual T[] ColumnValue() {
            return new T[] { ( T ) this.QueryParams[0].Value };
        }

    }



}
