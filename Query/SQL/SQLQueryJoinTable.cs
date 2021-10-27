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

    public interface IJoin {
        public SQLQueryConditionExpression[] SqlQueryConditions { get; set; }
        public string ID { get; }
    }

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



    public class JoinTable : IJoin {

        private List<JoinForeignKey> mJoinForeignKeys;

        private SQLQueryConditionExpression[] mSqlQueryConditions;
        internal AbstractEntity mTable;
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

        public string ID => this.Table.ActualName;

        public JoinTable() {

        }
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

    }


    public class RawQueryJoinTable : IJoin {
        private string id;
        private string mSQLQuery;

        public string ID { get => id;  }
        public RawQueryJoinTable(string id, string sQLQuery ) {
            //this.mTable = new 
            this.id = id;
            this.mSQLQuery = sQLQuery;
        }

        public SQLQueryConditionExpression[] SqlQueryConditions { get => new SQLQueryConditionExpression[] { }; set => SqlQueryConditions = value; }

        public override string ToString() {
            return mSQLQuery;
        }

    }

}