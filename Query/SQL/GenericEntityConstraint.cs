using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Text;
using prestoMySQL.Extension;
using System.Reflection;
using prestoMySQL.Helper;

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

            if ( this.QueryParams[0].Value is null ) {
                if ( BinaryOperator.Equals( SQLBinaryOperator.equal() ) ) {
                    BinaryOperator = SQLBinaryOperator.@is();
                } else if ( BinaryOperator.Equals( SQLBinaryOperator.notEqual() ) ) {
                    BinaryOperator = SQLBinaryOperator.isNot();
                }
            }

            //return String.Concat( "( " , columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " )" );
            return $"( {columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )} )";
        }


        public virtual int countParam() {
            return 1;
        }

        public virtual QueryParam[] getParam() {

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


    public class GenericEntityConstraint : DefinableConstraint {
        public GenericEntityConstraint( PropertyInfo p , EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = "" ) {

            var sss = p.GetMethod;
            sss.Invoke( null , new object[] { } );
            this.mColumnName = SQLTableEntityHelper.getColumnName( p.DeclaringType , p.ColumnName( null ) , true , false );
            //this.columnDefinition = p.GetVale(this);
            //this.mTableName = tableName;
            this.mBinaryOperator = mBinaryOperator;
            this.mParamPlaceHolder = paramPlaceHolder;
            this.mQueryParams = ( SQLQueryParams ) ( aQueryPararms ?? throw new ArgumentNullException( nameof( aQueryPararms ) ) );
        }


        //protected MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition;
        private string mColumnName;
        public string ColumnName { get => mColumnName; }

        //private string mTableName;
        //public string TableName { get => mTableName; }

        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();

        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        public override string ToString() {

            if ( this.QueryParams[0].Value is null ) {
                if ( BinaryOperator.Equals( SQLBinaryOperator.equal() ) ) {
                    BinaryOperator = SQLBinaryOperator.@is();
                } else if ( BinaryOperator.Equals( SQLBinaryOperator.notEqual() ) ) {
                    BinaryOperator = SQLBinaryOperator.isNot();
                }
            }

            //return String.Concat( "( " , columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) , " )" );
            return $"( {ColumnName} {BinaryOperator} {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )} )";
            //return "";
        }


        public virtual int countParam() {
            return 1;
        }

        public virtual QueryParam[] getParam() {

            if ( this.QueryParams.asArray().Length > 0 ) {
                if ( this.QueryParams[0].Value.GetType().IsArray ) {

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
                    return new QueryParam[] { ( QueryParam ) this.QueryParams[0] };
                }
            } else {
                return this.QueryParams.asArray();
            }
        }

        public virtual string[] getParamAsString() {
            return new string[] { this.QueryParams[0].AsQueryParam() };
        }

        public virtual object[] ColumnValue() {
            return new object[] { ( object ) this.QueryParams[0].Value };
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

                if ( this.QueryParams is not null ) {
                    if ( this.QueryParams[0].Value is null ) {
                        if ( BinaryOperator.Equals( SQLBinaryOperator.equal() ) ) {
                            BinaryOperator = SQLBinaryOperator.@is();
                        } else if ( BinaryOperator.Equals( SQLBinaryOperator.notEqual() ) ) {
                            BinaryOperator = SQLBinaryOperator.isNot();
                        }
                    }
                }

                c.Add( $"( {info.Table.ActualName.QuoteTableName()}.{info.ColumnName.QuoteColumnName()} {BinaryOperator} {info.mReferenceTable.ActualName.QuoteTableName()}.{info.ReferenceColumnName.QuoteTableName()} )" );
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



    public class GenericEntityConstraintFunction<T> : DefinableConstraint {

        private string mColumnName;
        public string ColumnName { get => mColumnName; }


        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        protected IFunction mFunction;
        public IFunction Function { get => mFunction; set => mFunction = value; }


        protected MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition;

        public GenericEntityConstraintFunction( MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition , EvaluableBinaryOperator mBinaryOperator , IFunction aFunction ) {

            this.columnDefinition = columnDefinition ?? throw new ArgumentNullException( nameof( columnDefinition ) );
            this.mBinaryOperator = mBinaryOperator;
            this.mParamPlaceHolder = String.Empty;
            this.mFunction = aFunction;
            this.mQueryParams = null;
        }


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

            //return $"( {ColumnName} {BinaryOperator} {Function.ToString()}";
            return $"( {columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {Function.ToString()} )";
        }


    }

    public class GenericJoinFieldConstratint<T> : DefinableConstraint {

        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }


        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }

        protected MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition;
        protected MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinitionForeign;

        public GenericJoinFieldConstratint( MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinition , MySQLDefinitionColumn<SQLTypeWrapper<T>> columnDefinitionForeign ) {

            this.columnDefinition = columnDefinition ?? throw new ArgumentNullException( nameof( columnDefinition ) );
            this.columnDefinitionForeign = columnDefinitionForeign ?? throw new ArgumentNullException( nameof( columnDefinitionForeign ) );
            this.mBinaryOperator = SQLBinaryOperator.equal();
            this.mParamPlaceHolder = String.Empty;
            this.mQueryParams = null;
        }


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

            return $"( {columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {columnDefinitionForeign.Table.ActualName.QuoteTableName()}.{columnDefinitionForeign.ColumnName.QuoteColumnName()}  )";
        }
    }

}
