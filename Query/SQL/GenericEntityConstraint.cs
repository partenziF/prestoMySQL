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
using MySqlConnector;
using System.Reflection;
using prestoMySQL.Extension;
using prestoMySQL.Helper;

namespace prestoMySQL.Query.SQL {
    public enum StatoLicenza {
        Attiva = 0,
        Sospesa = 1,
        Cessata = 2
    }
    public class Node {
        public uint id;
        public string label;
        public StatoLicenza Licenza;
        public string Qualifica;
        public bool expand;
        public List<Node> children;


        public Node( uint id , string label , string qualifica , StatoLicenza licenza , List<Node> children ) {
            this.id = id;
            this.label = label;
            this.Qualifica = qualifica;
            this.Licenza = licenza;
            this.children = children;
            expand = true;
        }
    }

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

            if ( this.QueryParams[0].Value?.GetType().IsArray ?? false ) {

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

            this.mColumnName = SQLTableEntityHelper.getColumnName( p.DeclaringType , p.ColumnName( null ) , true , false );
            //this.mTableName = tableName;
            this.mBinaryOperator = mBinaryOperator;
            this.mParamPlaceHolder = paramPlaceHolder;
            this.mQueryParams = ( SQLQueryParams ) ( aQueryPararms ?? throw new ArgumentNullException( nameof( aQueryPararms ) ) );
        }

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
        }


        public virtual int countParam() {
            return 1;
        }

        public virtual QueryParam[] getParam() {

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



}
