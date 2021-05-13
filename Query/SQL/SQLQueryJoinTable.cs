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

        private GenericEntityConstraint<object>[] mSqlQueryConditions;
        public virtual GenericEntityConstraint<object>[] SqlQueryConditions {
            get {
                return mSqlQueryConditions;
            }
            set {
                this.mSqlQueryConditions = value;
            }
        }


        private JoinType mJoinType;

        public SQLQueryJoinTable( SQLQuery mSQLQuery , string mPrimaryTable , string primaryTable , string primaryKey , string foreignTable , string foreignKey , GenericEntityConstraint<object>[] sqlQueryConditions , JoinType joinType ) {
            this.mSQLQuery = mSQLQuery;
            this.mPrimaryTable = mPrimaryTable;
            PrimaryTable = primaryTable;
            PrimaryKey = primaryKey;
            ForeignTable = foreignTable;
            ForeignKey = foreignKey;
            SqlQueryConditions = sqlQueryConditions;
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

                if ( ( mSqlQueryConditions != null ) && ( mSqlQueryConditions.Length > 0 ) ) {
                    string[] condition = new string[mSqlQueryConditions.Length];
                    int i = 0;

                    foreach ( GenericEntityConstraint<object> c in mSqlQueryConditions ) {
                        condition[i++] = c.ToString();
                    }

                    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey , string.join( " AND " , condition ) );
                    return "";
                } else {

                    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey );
                    return "";

                }

            } catch ( System.Exception e ) {
                return "";
            }

        }


    }

}
