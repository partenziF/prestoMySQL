using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class BetweenEntityConstraint<T> : GenericEntityConstraint<T> {

        SQLQueryParams mColumnValues;

        public BetweenEntityConstraint( DefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , SQLQueryParams columnValues , string aParamPlaceholder = "" ) : base( aColumnDefinition , SQLBinaryOperator.between() , columnValues , aParamPlaceholder ) {

            mColumnValues = columnValues ?? throw new ArgumentNullException( nameof( columnValues ) );
            if ( mColumnValues.asArray().Length != 2 ) throw new ArgumentOutOfRangeException( "Invalid number of argument." );

        }

        public override int countParam() {
            // TODO Auto-generated method stub
            return this.QueryParams.asArray().Length;
            //return 2;
        }

        //public override object[] getParam() {
        //    return base.getParam();
        //}


        public override QueryParam[] getParam() {
            return new QueryParam[] { ( QueryParam ) this.QueryParams[0], ( QueryParam ) this.QueryParams[1] };
        }

        public override string[] getParamAsString() {
            return new string[] { this.QueryParams[0].AsQueryParam() , this.QueryParams[1].AsQueryParam() };
        }

        public override string ToString() {
            return String.Concat( "( " , columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " AND " , this.QueryParams[1].AsQueryParam( ParamPlaceHolder ) , "  )" );
        }

        public override T[] ColumnValue() {
            return new T[] { ( T ) this.QueryParams[0].Value, ( T ) this.QueryParams[1].Value };
        }

    }


    //	private String p1, p2 = null;



}
