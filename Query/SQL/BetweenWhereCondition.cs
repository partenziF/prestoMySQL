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
    public class BetweenWhereCondition<T> : GenericWhereCondition<T> {

        SQLQueryParams mColumnValues;

        public BetweenWhereCondition( SQLProjectionColumn<SQLTypeWrapper<T>> aColumnDefinition , SQLQueryParams columnValues , string aParamPlaceholder = "" ) : base( aColumnDefinition , SQLBinaryOperator.between() , columnValues , aParamPlaceholder ) {

            mColumnValues = columnValues ?? throw new ArgumentNullException( nameof( columnValues ) );
            if ( mColumnValues.asArray().Length != 2 ) throw new ArgumentOutOfRangeException( "Invalid number of argument." );

        }

        public override int countParam() {
            // TODO Auto-generated method stub
            return this.QueryParams.asArray().Length;
            //return 2;
        }

        public override QueryParam[] getParam() {
            return new QueryParam[] { ( QueryParam ) this.QueryParams[0] , ( QueryParam ) this.QueryParams[1] };
        }

        public override string[] getParamAsString() {
            return new string[] { this.QueryParams[0].AsQueryParam() , this.QueryParams[1].AsQueryParam() };
        }

        public override string ToString() {
            return String.Concat( "( " , columnDefinition.Table.ActualName.QuoteTableName() , "." , columnDefinition.ColumnName.QuoteColumnName() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " AND " , this.QueryParams[1].AsQueryParam( ParamPlaceHolder ) , "  )" );
        }

        public override T[] ColumnValue() {
            return new T[] { ( T ) this.QueryParams[0].Value , ( T ) this.QueryParams[1].Value };
        }

    }


    public class BetweenFunctionWhereCondition : DefinableConstraint {
        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }



        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.between();

        //        public BetweenFunctionWhereCondition( IFunction function , MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinitionMinValue , MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinitionMaxValue ) {
        public BetweenFunctionWhereCondition( IFunction function , dynamic aColumnDefinitionMinValue , dynamic aColumnDefinitionMaxValue ) {
            Function = function;
            ColumnDefinitionMinValue = aColumnDefinitionMinValue;
            ColumnDefinitionMaxValue = aColumnDefinitionMaxValue;
        }

        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }
        public IFunction Function { get; }
        public dynamic ColumnDefinitionMinValue { get; }
        public dynamic ColumnDefinitionMaxValue { get; }

        public int countParam() {
            return 0;
        }

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

        public string[] getParamAsString() {
            return new string[] { };
        }

        public override string ToString() {

            string s = "".QuoteTableName();
            return $"( {Function} {BinaryOperator}  {( ( string ) ColumnDefinitionMinValue.Table.ActualName ).QuoteTableName() }.{( ( string ) ColumnDefinitionMinValue.ColumnName).QuoteColumnName()} AND {( ( string ) ColumnDefinitionMinValue.Table.ActualName).QuoteTableName() }.{( ( string ) ColumnDefinitionMaxValue.ColumnName).QuoteColumnName()} )"; 
        }
    }

}
