using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prestoMySQL.ForeignKey;
using prestoMySQL.Extension;

namespace prestoMySQL.Query.SQL {
    public class GenericEntityConstraint<T> : DefinableConstraint {


        public GenericEntityConstraint( MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition , EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = "" ) {

            this.columnDefinition = columnDefinition ?? throw new ArgumentNullException( nameof( columnDefinition ) );
            this.mBinaryOperator = mBinaryOperator;
            this.mParamPlaceHolder = paramPlaceHolder;
            this.mQueryParams = ( SQLQueryParams ) ( aQueryPararms ?? throw new ArgumentNullException( nameof( aQueryPararms ) ) );
        }


        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }


        protected MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition;


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();

        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        public override string ToString() {

            return String.Concat( "( " , columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " )" );
        }


        public virtual int countParam() {
            return 1;
        }

        public virtual QueryParam[] getParam() {
            return new QueryParam[] { ( QueryParam ) this.QueryParams[0] };
        }

        public virtual string[] getParamAsString() {
            return new string[] { this.QueryParams[0].AsQueryParam() };
        }

        public virtual T[] ColumnValue() {
            return new T[] { ( T ) this.QueryParams[0].Value };
        }
    }



    public class GenericForeignKeyConstraint : DefinableConstraint {

        private ForeignKey.ForeignKey mForeignKey;
        private SQLQueryParams mQueryParams;
        public GenericForeignKeyConstraint( ForeignKey.ForeignKey foreignKey ) {
            this.mForeignKey = foreignKey;
        }

        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }
        public string ParamPlaceHolder { get => ""; set => _ = ""; }
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = null; }

        public override string ToString() {

            List<string> c = new List<string>();

            foreach ( var info in mForeignKey.foreignKeyInfo ) {
                c.Add( $"( {info.Table.ActualName.QuoteTableName()}.{info.ColumnName.QuoteColumnName()} {mBinaryOperator} {info.mReferenceTable.ActualName.QuoteTableName()}.{info.ReferenceColumnName.QuoteTableName()} )" );
            }

            StringBuilder sb = new StringBuilder( String.Join( " AND " , c ) );

            return sb.ToString();
        }


        public virtual int countParam() {
            return 0;
        }

        public virtual QueryParam[] getParam() {
            return new QueryParam[] { };
        }

        public virtual string[] getParamAsString() {
            return new string[] { };
        }


    }



}
