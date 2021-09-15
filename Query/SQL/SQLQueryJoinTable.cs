using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.Helper;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {

    public class JoinForeignKey {

        private AbstractEntity mTable;
        private string mColumnName;
        private AbstractEntity mReferenceTable;
        private string mReferenceColumnName;
        private object fk;

        public JoinForeignKey( ForeignKey.ForeignKeyInfo fk ) {
            this.mTable = fk.Table;
            this.mColumnName = fk.ColumnName;
            this.mReferenceTable = fk.ReferenceTable;
            this.mReferenceColumnName = fk.ReferenceColumnName;
        }

        public JoinForeignKey( AbstractEntity table , string columnName , AbstractEntity referenceTable , string referenceColumnName ) {
            this.mTable = table;
            this.mColumnName = columnName;
            this.mReferenceTable = referenceTable;
            this.mReferenceColumnName = referenceColumnName;
        }

        public AbstractEntity Table { get => this.mTable; set => this.mTable = value; }
        public AbstractEntity ReferenceTable { get => this.mReferenceTable; set => this.mReferenceTable = value; }

        public override string ToString() {

            var @left = SQLTableEntityHelper.getColumnName( Table , mColumnName , true , true );
            var @right = SQLTableEntityHelper.getColumnName( mReferenceTable , mReferenceColumnName , true , true );

            return $"{left} = {right}";

        }
    }

    public class JoinTable {

        //private string mPrimaryTable;
        //public string PrimaryTable {
        //    get {
        //        return mPrimaryTable;
        //    }
        //    set {
        //        this.mPrimaryTable = value;
        //    }
        //}

        //private string mPrimaryKey;
        //public virtual string PrimaryKey {
        //    get {
        //        return mPrimaryKey;
        //    }
        //    set {
        //        this.mPrimaryKey = value;
        //    }
        //}


        //private string mForeignTable;
        //public virtual string ForeignTable {
        //    get {
        //        return mForeignTable;
        //    }
        //    set {
        //        this.mForeignTable = value;
        //    }
        //}


        //private string mForeignKey;
        //public virtual string ForeignKey {
        //    get {
        //        return mForeignKey;
        //    }
        //    set {
        //        this.mForeignKey = value;
        //    }
        //}

        private List<JoinForeignKey> mJoinForeignKeys;

        private SQLQueryConditionExpression[] mSqlQueryConditions;
        private AbstractEntity mTable;
        public AbstractEntity Table { get => mTable; }

        public SQLQueryConditionExpression[] SqlQueryConditions {
            get {
                return mSqlQueryConditions;
            }
            set {
                this.mSqlQueryConditions = value;
            }
        }

        private JoinType mJoinType;
        public virtual JoinType JoinType {
            get {
                return mJoinType;
            }
            set {
                this.mJoinType = value;
            }
        }

        public List<JoinForeignKey> JoinForeignKeys { get => this.mJoinForeignKeys; set => this.mJoinForeignKeys = value; }

        public JoinTable( JoinType joinType , AbstractEntity table , JoinForeignKey[] foreignKey , params SQLQueryConditionExpression[] sqlQueryConditionExpressions ) {

            mJoinType = joinType;
            mJoinForeignKeys = new List<JoinForeignKey>();
            mJoinForeignKeys.AddRange( foreignKey );
            mTable = table;

            SqlQueryConditions = sqlQueryConditionExpressions.Length == 0 ? null : sqlQueryConditionExpressions.ToArray();
        }

        public JoinTable( JoinType joinType , AbstractEntity table , string primaryKey , AbstractEntity referenceTable , string foreignKey ) {
            mJoinType = joinType;
            mJoinForeignKeys = new List<JoinForeignKey>() { new JoinForeignKey( table , primaryKey , referenceTable , foreignKey ) };
        }

        public JoinTable( JoinTable joinTable , params SQLQueryConditionExpression[] constraint ) {
            mJoinType = joinTable.JoinType;
            mJoinForeignKeys = joinTable.JoinForeignKeys;
            mTable = joinTable.Table;

            SqlQueryConditions = constraint.Length == 0 ? null : constraint.ToArray();

        }

        public override string ToString() {

            try {

                StringBuilder sb;

                if ( mTable.AliasName is null ) {
                    sb = new StringBuilder( $"{JoinType.ToString()} JOIN {mTable.ActualName.QuoteTableName()} ON " );
                } else {
                    sb = new StringBuilder( $"{JoinType.ToString()} JOIN {mTable.TableName.QuoteTableName()} AS {mTable.ActualName.QuoteTableName()} ON " );
                }

                sb.AppendJoin( "\r\n\tAND " , mJoinForeignKeys );

                if ( SqlQueryConditions?.Length > 0 ) {
                    sb.Append( "\r\n\tAND " );
                    sb.AppendJoin( "\r\n\tAND " , SqlQueryConditions.Select( x => x.ToString() ).ToArray() );
                }
                return sb.ToString();


                //if ( joinTable.AliasName is null ) {
                //    sb.Append( string.Format( "{0} JOIN {1} ON " , fks.JoinType.ToString() , joinTable.ActualName.QuoteTableName() ) );
                //} else {
                //    sb.Append( string.Format( "{0} JOIN {1} {2} ON " , fks.JoinType.ToString() , joinTable.TableName.QuoteTableName() , joinTable.ActualName.QuoteTableName() ) );
                //}

                //string join = "";

                //switch ( this.mJoinType ) {
                //    case JoinType.INNER:
                //    join = "INNER";
                //    break;
                //    case JoinType.LEFT:
                //    join = "LEFT";
                //    break;
                //    case JoinType.RIGHT:
                //    join = "RIGHT";
                //    break;
                //    default:
                //    join = "";
                //    break;
                //}

                //if ( ( SqlQueryConditions != null ) && ( SqlQueryConditions.Length > 0 ) ) {
                //    string[] condition = new string[mSqlQueryConditions.Length];
                //    int i = 0;

                //    //foreach ( GenericEntityConstraint<object> c in mSqlQueryConditions ) {
                //    //    condition[i++] = c.ToString();
                //    //}

                //    //return string.Format( "{0} JOIN {1} ON {2} = {3} AND {4}" , join , mForeignTable , mForeignKey , mPrimaryKey , string.Join( " AND " , condition ) );
                //    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey , string.join( " AND " , condition ) );
                //    return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mPrimaryTable , mPrimaryTable , mPrimaryKey , mForeignTable , mForeignKey , string.Join( " AND " , this.SqlQueryConditions?.Select( x => x?.ToString() ).ToArray() ) );
                //    //return "";

                //} else {

                //    //return string.Format( "{0} JOIN {1} ON {2} = {3} " , join , mForeignTable , mForeignKey , mPrimaryKey );
                //    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey );
                //    //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mForeignTable  , mForeignTable  , mForeignKey , mPrimaryTable  , mPrimaryKey );
                //    return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mPrimaryTable , mPrimaryTable , mPrimaryKey , mForeignTable , mForeignKey );
                //    //return "";

                //}

            } catch ( System.Exception e ) {
                return "";
            }

        }



    }

    public class SQLQueryJoinTable : JoinTable {


        private SQLQuery mSQLQuery;

        public SQLQueryJoinTable( SQLQuery sQLQuery , JoinTable joinTable , params SQLQueryConditionExpression[] constraint ) : base( joinTable , constraint ) {
        }

        public SQLQueryJoinTable( SQLQuery sqlQuery , AbstractEntity table , JoinType joinType , JoinForeignKey[] foreignKey , params SQLQueryConditionExpression[] sqlQueryConditionExpressions ) : base( joinType , table , foreignKey , sqlQueryConditionExpressions ) {
            mSQLQuery = sqlQuery;
        }

        //private string mPrimaryTable;
        //public string PrimaryTable {
        //    get {
        //        return mPrimaryTable;
        //    }
        //    set {
        //        this.mPrimaryTable = value;
        //    }
        //}

        //private string mPrimaryKey;
        //public virtual string PrimaryKey {
        //    get {
        //        return mPrimaryKey;
        //    }
        //    set {
        //        this.mPrimaryKey = value;
        //    }
        //}


        //private string mForeignTable;
        //public virtual string ForeignTable {
        //    get {
        //        return mForeignTable;
        //    }
        //    set {
        //        this.mForeignTable = value;
        //    }
        //}


        //private string mForeignKey;
        //public virtual string ForeignKey {
        //    get {
        //        return mForeignKey;
        //    }
        //    set {
        //        this.mForeignKey = value;
        //    }
        //}

        //private SQLQueryConditionExpression[] mSqlQueryConditions;
        //public virtual SQLQueryConditionExpression[] SqlQueryConditions {
        //    get {
        //        return mSqlQueryConditions;
        //    }
        //    set {
        //        this.mSqlQueryConditions = value;
        //    }
        //}


        //private JoinType mJoinType;



        //public virtual JoinType JoinType {
        //    get {
        //        return mJoinType;
        //    }
        //    set {
        //        this.mJoinType = value;
        //    }
        //}

        //public override string ToString() {

        //    try {

        //        string join = "";

        //        switch ( this.JoinType ) {
        //            case JoinType.INNER:
        //            join = "INNER";
        //            break;
        //            case JoinType.LEFT:
        //            join = "LEFT";
        //            break;
        //            case JoinType.RIGHT:
        //            join = "RIGHT";
        //            break;
        //            default:
        //            join = "";
        //            break;
        //        }

        //        if ( ( SqlQueryConditions != null ) && ( SqlQueryConditions.Length > 0 ) ) {
        //            string[] condition = new string[SqlQueryConditions.Length];
        //            int i = 0;

        //            //foreach ( GenericEntityConstraint<object> c in mSqlQueryConditions ) {
        //            //    condition[i++] = c.ToString();
        //            //}

        //            //return string.Format( "{0} JOIN {1} ON {2} = {3} AND {4}" , join , mForeignTable , mForeignKey , mPrimaryKey , string.Join( " AND " , condition ) );
        //            //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey , string.join( " AND " , condition ) );
        //            return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} AND {6}" , join , PrimaryTable , PrimaryTable , PrimaryKey , ForeignTable , ForeignKey , string.Join( " AND " , this.SqlQueryConditions?.Select( x => x?.ToString() ).ToArray() ) );
        //            //return "";

        //        } else {

        //            //return string.Format( "{0} JOIN {1} ON {2} = {3} " , join , mForeignTable , mForeignKey , mPrimaryKey );
        //            //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mSQLQuery.getTableDeclaration( mForeignTable ) , mSQLQuery.getTableRealName( mForeignTable ) , mForeignKey , mSQLQuery.getTableRealName( mPrimaryTable ) , mPrimaryKey );
        //            //return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , mForeignTable  , mForeignTable  , mForeignKey , mPrimaryTable  , mPrimaryKey );
        //            return string.Format( "{0} JOIN {1} ON {2}.{3} = {4}.{5} " , join , PrimaryTable , PrimaryTable , PrimaryKey , ForeignTable , ForeignKey );
        //            //return "";

        //        }

        //    } catch ( System.Exception e ) {
        //        return "";
        //    }

        //}


    }

}