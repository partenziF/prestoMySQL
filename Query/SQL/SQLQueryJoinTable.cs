using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLQueryJoinTable {


        private SQLQuery mSQLQuery;


        private string mPrimaryTable;
        public string PrimaryTable {
            get {
                return mPrimaryTable;
            }
            set {
                this.mPrimaryTable = value;
            }
        }

        private string mPrimaryKey;
        public virtual string PrimaryKey {
            get {
                return mPrimaryKey;
            }
            set {
                this.mPrimaryKey = value;
            }
        }


        private string mForeignTable;
        public virtual string ForeignTable {
            get {
                return mForeignTable;
            }
            set {
                this.mForeignTable = value;
            }
        }


        private string mForeignKey;
        public virtual string ForeignKey {
            get {
                return mForeignKey;
            }
            set {
                this.mForeignKey = value;
            }
        }
        
        private SQLQueryConditionExpression[] mSqlQueryConditions;
        public virtual SQLQueryConditionExpression[] SqlQueryConditions {
            get {
                return mSqlQueryConditions;
            }
            set {
                this.mSqlQueryConditions = value;
            }
        }


        private JoinType mJoinType;

        public SQLQueryJoinTable( SQLQuery mSQLQuery , JoinType joinType , string primaryTable , string primaryKey , string foreignTable , string foreignKey , params SQLQueryConditionExpression[] sqlQueryConditionExpressions ) {
            this.mSQLQuery = mSQLQuery;

            PrimaryTable = primaryTable;
            PrimaryKey = primaryKey;
            ForeignTable = foreignTable;
            ForeignKey = foreignKey;
            
            SqlQueryConditions = sqlQueryConditionExpressions.Length == 0 ? null : sqlQueryConditionExpressions.ToArray();
            JoinType = joinType;
        }

        public virtual JoinType JoinType {
            get {
                return mJoinType;
            }
            set {
                this.mJoinType = value;
            }
        }


        public override string ToString() {

            try {

                string join = "";

                switch ( this.mJoinType ) {
                    case JoinType.INNER:
                    join = "INNER";
                    break;
                    case JoinType.LEFT:
                    join = "LEFT";
                    break;
                    case JoinType.RIGHT:
                    join = "RIGHT";
                    break;
                    default:
                    join = "";
                    break;
                }

                if ( ( SqlQueryConditions != null ) && ( SqlQueryConditions.Length > 0 ) ) {
                    string[] condition = new string[mSqlQueryConditions.Length];
                    int i = 0;

                    //foreach ( GenericEntityConstraint<object> c in mSqlQueryConditions ) {
                    //    condition[i++] = c.ToString();
                    //}

                    //return string.Format( "{0} JOIN {1} ON {2} = {3} AND {4}" , join , mForeignTable , mForeignKey , mPrimaryKey , string.Join( " AND " , condition ) );
                    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey , string.join( " AND " , condition ) );
                    return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mPrimaryTable , mPrimaryTable , mPrimaryKey , mForeignTable , mForeignKey , string.Join( " AND " , this.SqlQueryConditions?.Select( x => x?.ToString() ).ToArray() ) );
                    //return "";

                } else {

                    //return string.Format( "{0} JOIN {1} ON {2} = {3} " , join , mForeignTable , mForeignKey , mPrimaryKey );
                    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey );
                    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mForeignTable  , mForeignTable  , mForeignKey , mPrimaryTable  , mPrimaryKey );
                    return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mPrimaryTable , mPrimaryTable , mPrimaryKey , mForeignTable , mForeignKey );
                    //return "";

                }

            } catch ( System.Exception e ) {
                return "";
            }

        }


    }

}
