using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLQueryCondition<T> : ISQLQueryCondition<T> {
        

        private SQLQuery sqlQuery;
        private String mColumnName = "";
        public string ColumnName { get => mColumnName; set => mColumnName = value; }

        private T mColumnValue;
        public T ColumnValue { get => mColumnValue; set => mColumnValue = value; }

        private String mTableName = "";
        public string TableName { get => mTableName; set => mTableName = value; }


        private SQLBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();

        public SQLBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }
        public string ParamPlaceHolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public QueryParam QueryParam { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        EvaluableBinaryOperator DefinableConstraint<T>.BinaryOperator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IQueryParams QueryParams { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        SQLQueryParams DefinableConstraint<T>.QueryParams { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        //public EvaluableBinaryOperator BinaryOperator() {
        //    throw new NotImplementedException();
        //}


        public int countParam() {
            throw new NotImplementedException();
        }

        public string getColumnName() {
            throw new NotImplementedException();
        }

        public object[] getParam() {
            throw new NotImplementedException();
        }

        public string[] getParamAsString() {
            throw new NotImplementedException();
        }

        public void setColumnName( string aColumnName ) {
            throw new NotImplementedException();
        }


        T[] DefinableConstraint<T>.ColumnValue() {
            throw new NotImplementedException();
        }

        QueryParam[] DefinableConstraint<T>.getParam() {
            throw new NotImplementedException();
        }
    }

}
