using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
	public class SQLQuery : ISQLQuery, IDictionary<string , object> {

		internal Dictionary<string , SQLProjectionColumn<SQLTypeWrapper<object>>> getter;

        public SQLQuery( Dictionary<string , SQLProjectionColumn<SQLTypeWrapper<object>>> getter ) {
            this.getter = getter;
        }


        public class OrderByEntityComparator : IComparer<SQLQueryOrderBy> {

            public virtual int Compare( SQLQueryOrderBy arg0 , SQLQueryOrderBy arg1 ) {
                // TODO Auto-generated method stub
                return arg0.order - arg1.order;
            }

        }

        public class GroupByEntityComparator : IComparer<SQLQueryGroupBy> {

            public virtual int Compare( SQLQueryGroupBy arg0 , SQLQueryGroupBy arg1 ) {
                // TODO Auto-generated method stub
                return arg0.order - arg1.order;
            }

        }


        private List<string> mSelectExpression;
        public virtual List<string> SelectExpression {
            get {
                return mSelectExpression;
            }
            set {
                this.mSelectExpression = value;
            }
        }


        private List<SQLQueryConditionExpression> mWhereCondition;

        public virtual List<SQLQueryConditionExpression> WhereCondition {
            get {
                return mWhereCondition;
            }
            set {
                this.mWhereCondition = value;
            }
        }


        private List<SQLQueryJoinTable> mJoinTable;
        public virtual List<SQLQueryJoinTable> JoinTable {
            get {
                return mJoinTable;
            }
            set {
                this.mJoinTable = value;
            }
        }

        private IDictionary<string , TableReference> mHashOfSQLQueryTableReference;

        //////////////////////////////////////////////////////////////////////////////////

        private PriorityQueue<SQLQueryOrderBy> mOrderBy;

        //////////////////////////////////////////////////////////////////////////////////

        private PriorityQueue<SQLQueryGroupBy> mGroupBy;


        public SQLQuery() {
            // TODO Auto-generated constructor stub
            mSelectExpression = new List<string>();
            mWhereCondition = new List<SQLQueryConditionExpression>();
            mJoinTable = new List<SQLQueryJoinTable>();
            mHashOfSQLQueryTableReference = new Dictionary<string , TableReference>();
            //mOrderBy = new PriorityQueue<SQLQueryOrderBy>( 1 , new OrderByEntityComparator() );
            //mGroupBy = new PriorityQueue<SQLQueryGroupBy>( 1 , new GroupByEntityComparator() );
        }



        object IDictionary<string , object>.this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ICollection<string> IDictionary<string , object>.Keys => throw new NotImplementedException();

        ICollection<object> IDictionary<string , object>.Values => throw new NotImplementedException();

        int ICollection<KeyValuePair<string , object>>.Count => throw new NotImplementedException();

        bool ICollection<KeyValuePair<string , object>>.IsReadOnly => throw new NotImplementedException();

        public int execute() {
            throw new NotImplementedException();
        }

        public string getSQLQuery() {
            throw new NotImplementedException();
        }

        void IDictionary<string , object>.Add( string key , object value ) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string , object>>.Add( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string , object>>.Clear() {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string , object>>.Contains( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        bool IDictionary<string , object>.ContainsKey( string key ) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string , object>>.CopyTo( KeyValuePair<string , object>[] array , int arrayIndex ) {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string , object>> IEnumerable<KeyValuePair<string , object>>.GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        bool IDictionary<string , object>.Remove( string key ) {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string , object>>.Remove( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        bool IDictionary<string , object>.TryGetValue( string key , out object value ) {
            throw new NotImplementedException();
        }
    }
}
