using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Helper;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {

    public class SQLQuery : ISQLQuery { //IDictionary<string , object> 

        public SQLQuery() {

            var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) : false ).ToArray();

            foreach ( PropertyInfo p in InstantiableProperties ) {
                var ctors = p.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( SQLQuery ) } );
                p.SetValue( this , ctors.Invoke( new object[] { p.Name , p , this } ) , null );
            }

            // TODO Auto-generated constructor stub
            mSelectExpression = new List<string>();
            mWhereCondition = new List<SQLQueryConditionExpression>();
            mJoinTable = new List<SQLQueryJoinTable>();
            mHashOfSQLQueryTableReference = new Dictionary<string , TableReference>();
            mOrderBy = new PriorityQueue<SQLQueryOrderBy>();
            mGroupBy = new PriorityQueue<SQLQueryGroupBy>();


        }

        private int? mRowCount = null;
        private int? mOffset = null;


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
        protected virtual List<SQLQueryConditionExpression> WhereCondition {
            get {
                return mWhereCondition;
            }
            set {
                this.mWhereCondition = value;
            }
        }


        private List<SQLQueryJoinTable> mJoinTable;
        protected virtual List<SQLQueryJoinTable> JoinTable {
            get {
                return mJoinTable;
            }
            set {
                this.mJoinTable = value;
            }
        }


        public int countParam {
            get {
                int Total = 0;
                Total += mWhereCondition.Sum( x => x.countParam() );
                return Total;
            }

        }


        public QueryParam[] getParam {
            get {
                List<QueryParam> l = new List<QueryParam>();
                foreach ( var i in this.mWhereCondition ) {
                    foreach ( var j in i.getParam() ) {
                        l.Add( j );
                    }
                }
                return l.ToArray();
            }

        }

        private IDictionary<string , TableReference> mHashOfSQLQueryTableReference;

        //////////////////////////////////////////////////////////////////////////////////

        private PriorityQueue<SQLQueryOrderBy> mOrderBy;

        //////////////////////////////////////////////////////////////////////////////////

        private PriorityQueue<SQLQueryGroupBy> mGroupBy;


        public String getTableReferences() {
            throw new NotImplementedException();
            //          ArrayList<TableReference> l = SQLTableEntityHelper.getQueryTableName(this);
            //      String []
            //      s = new String[l.size()];
            //int i = 0;

            //for (TableReference e : l) {

            //	s[i++] = e.toString();
            //}

            //return String.join(",", s);

        }
        public String getmGroupByTableDeclaration( String aTableName ) {
            throw new NotImplementedException();
            //if ( mHashOfSQLQueryTableReference.isEmpty() )
            //    throw new Exception( "No tables found" );
            //if ( mHashOfSQLQueryTableReference.containsKey( aTableName ) ) {
            //    TableReference t = mHashOfSQLQueryTableReference.get( aTableName );
            //    if ( t.getTableAlias().isEmpty() ) {
            //        return t.getTableName();
            //    } else {
            //        return String.format( "%s AS %s" , t.getTableName() , t.getTableAlias() );
            //    }

            //} else {
            //    throw new Exception( "No table found in tables" );
            //}

        }
        public SQLQuery WHERE( params SQLQueryConditionExpression[] sqlQueryConditionExpressions ) {

            if ( sqlQueryConditionExpressions.Length > 0 ) {

                mWhereCondition = new List<SQLQueryConditionExpression>();

                for ( int i = 0; i < sqlQueryConditionExpressions.Length; i++ ) {

                    mWhereCondition.Add( sqlQueryConditionExpressions[i] );

                }

            }

            return this;

        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            try {


                sb.Append( String.Format( "SELECT\r\n\t{0} " , String.Join( ',' , SelectExpression ) ) );
                sb.Append( String.Format( "\r\nFROM\r\n\t{0} " , String.Join( ',' , mHashOfSQLQueryTableReference.Select( x => x.Value.ToString() ).ToArray() ) ) );

                //    if ( !mJoinTable.isEmpty() ) {
                //        String[] j = new String[mJoinTable.size()];
                //        int i = 0;
                //        for ( SQLQueryJoinTable jt : mJoinTable ) {
                //            j[i++] = jt.toString();
                //        }
                //        sb.append( String.format( "\r\n %s " , String.join( "\r\n\t" , j ) ) );

                //    }

                if ( ( mWhereCondition.Count > 0 ) ) {
                    
                    String[] c = new String[mWhereCondition.Count];
                    int i = 0;
                    foreach ( SQLQueryConditionExpression sc in mWhereCondition ) {
                        c[i++] = sc.ToString();
                    }

                    sb.Append( String.Format( "\r\nWHERE\r\n\t( {0} )" , String.Join( " AND " , c ) ) );

                }

                //    if ( !mGroupBy.isEmpty() ) {
                //        String[] c = new String[mGroupBy.size()];
                //        int i = 0;
                //        for ( SQLQueryGroupBy sc : mGroupBy ) {
                //            c[i++] = sc.toString();
                //        }

                //        sb.append( String.format( "\r\nGROUP BY\r\n\t%s " , String.join( "," , c ) ) );

                //    }

                //    if ( !mOrderBy.isEmpty() ) {
                //        String[] c = new String[mOrderBy.size()];
                //        int i = 0;
                //        for ( SQLQueryOrderBy sc : mOrderBy ) {
                //            c[i++] = sc.toString();
                //        }

                //        sb.append( String.format( "\r\nORDER BY\r\n\t%s " , String.join( "," , c ) ) );
                //    }

                if ( ( mOffset is not null ) && ( mRowCount is not null ) ) {
                    sb.Append( String.Format( $"LIMIT {mRowCount} OFFSET {mOffset}" ) );
                } else if ( ( mOffset is null ) && ( mRowCount is not null ) ) {
                    sb.Append( String.Format( $"LIMIT {mRowCount}" ) );
                } else if ( ( mOffset is not null ) && ( mRowCount is null ) ) {
                    throw new ArgumentException( "Invalid argument RowCount can't be null." );
                }

                } catch ( System.Exception e ) {
                //    // TODO Auto-generated catch block
                //    e.printStackTrace();
            }
            return sb.ToString();
        }

        public virtual string getSQLQuery() {
            return null;

        }

        public int execute() {
            return 0;
        }

        public void Initialize() { //final

            mSelectExpression.Clear();
            mWhereCondition.Clear();
            mJoinTable.Clear();
            mHashOfSQLQueryTableReference.Clear();
            foreach ( TableReference e in SQLTableEntityHelper.getQueryTableName( this.GetType() ) ) {
                mHashOfSQLQueryTableReference.Add( e.TableName , e );
            }
            //for ( TableReference e : SQLTableEntityHelper.getQueryJoinTableName( this.getClass() ) ) {
            //    mHashOfSQLQueryTableReference.put( e.getTableName() , e );
            //}

            mOrderBy?.Clear();
            mGroupBy?.Clear();

        }


        public String getTableRealName( String aTableName ) {
            throw new NotImplementedException();
            //if ( mHashOfSQLQueryTableReference.isEmpty() )
            //    throw new Exception( "No tables found" );
            //if ( mHashOfSQLQueryTableReference.containsKey( aTableName ) ) {
            //    TableReference t = mHashOfSQLQueryTableReference.get( aTableName );
            //    if ( t.getTableAlias().isEmpty() ) {
            //        return t.getTableName();
            //    } else {
            //        return t.getTableAlias();
            //    }

            //} else {
            //    throw new Exception( "No table found in tables" );
            //}

        }

        public SQLQuery INNERJOIN( String aForeignTable , String aForeignKey , String aPrimaryTable , String aPrimaryKey , params SQLQueryCondition<object>[] aSqlQueryCondition ) {
            // TODO Auto-generated method stub
            //SQLQueryCondition <?>[]
            //fSqlQueryConditions = null;

            //if ( ( aSqlQueryCondition != null ) && ( aSqlQueryCondition.length > 0 ) ) {
            //    fSqlQueryConditions = new SQLQueryCondition<?>[aSqlQueryCondition.length];
            //    int i = 0;
            //    for ( SQLQueryCondition <?> c : aSqlQueryCondition) {
            //        fSqlQueryConditions[i++] = c;
            //    }
            //}

            //mJoinTable.add( new SQLQueryJoinTable( this , JoinType.INNER , aPrimaryTable , aPrimaryKey , aForeignTable , aForeignKey , fSqlQueryConditions ) );

            return this;
        }

        public SQLQuery LEFTJOIN( String aForeignTable , String aForeignKey , String aPrimaryTable , String aPrimaryKey , params SQLQueryCondition<object>[] aSqlQueryCondition ) {
            //SQLQueryCondition <?>[]
            //fSqlQueryConditions = null;

            //if ( aSqlQueryCondition.length > 0 ) {
            //    fSqlQueryConditions = new SQLQueryCondition<?>[aSqlQueryCondition.length];
            //    int i = 0;
            //    for ( SQLQueryCondition <?> c : aSqlQueryCondition) {
            //        fSqlQueryConditions[i++] = c;
            //    }
            //}

            //mJoinTable.add( new SQLQueryJoinTable( this , JoinType.LEFT , aPrimaryTable , aPrimaryKey , aForeignTable ,
            //        aForeignKey , fSqlQueryConditions ) );

            return this;
        }

        public SQLQuery GROUPBY( params SQLQueryGroupBy[] aGroupByEntity ) {
            //// TODO Auto-generated method stub
            //for ( SQLQueryGroupBy e : aGroupByEntity ) {
            //    this.mGroupBy.add( e );
            //}

            return this;

        }

        public SQLQuery ORDERBY( params SQLQueryOrderBy[] aOrderByEntity ) {
            // TODO Auto-generated method stub
            //foreach ( SQLQueryOrderBy e in  aOrderByEntity ) {
            //    this.mOrderBy.add( e );
            //}
            return this;
        }

        public SQLQuery LIMIT( int? offset , int? rowCount ) {

            mRowCount = rowCount;
            mOffset = offset;

            return this;
        }

        public void setJoinTables( List<PropertyInfo> projectionFields ) {
            // TODO Auto-generated method stub

            //            for ( Field f : projectionFields ) {

            //                if ( f.isAnnotationPresent( DALJoinClauses.class)) {
            //				if (f.isAnnotationPresent(DALProjectionColumn.class)) {
            //					DALJoinClause[] t = f.getAnnotationsByType( DALJoinClause.class);
            //					for (DALJoinClause a : t) {

            //						DALProjectionColumn aa = f.getAnnotation( DALProjectionColumn.class);
            //						createJoin( a, aa);

            //    }
            //}

            //			} else if ( f.isAnnotationPresent( DALJoinClause.class)) {

            //    if ( f.isAnnotationPresent( DALProjectionColumn.class)) {
            //    DALJoinClause a = f.getAnnotation( DALJoinClause.class);
            //DALProjectionColumn aa = f.getAnnotation( DALProjectionColumn.class);

            //createJoin( a , aa );
            //				}

            //			}

            //		}

        }

        public SQLQueryCondition<object> createSQLQueryCondition( DALJoinConstraint c ) {
            throw new NotImplementedException();

            //            String TableName = "";
            //            if ( ( c.Table().isEmpty() ) && ( c.Entity() == void.class)){
            //			throw new Exception("In a constraint must specify Table or Entity");
            //    } else if (c.Entity()!=void.class){
            //			//TableName = c.Entity()
            //			TableName = SQLTableEntityHelper.getTableName(c.Entity());
            //		}else if ( !c.Table().isEmpty() ) {
            //    TableName = c.Table();
            //}

            //if ( ( c.TypeValue() == Byte.class) || ( c.TypeValue() == byte.class)) {
            //    return new SQLQueryCondition<Byte>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Byte.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Short.class) || ( c.TypeValue() == short.class)) {
            //    return new SQLQueryCondition<Short>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Short.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Integer.class) || ( c.TypeValue() == int.class)) {
            //    return new SQLQueryCondition<Integer>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Integer.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Long.class) || ( c.TypeValue() == long.class)) {
            //    return new SQLQueryCondition<Long>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Long.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Float.class) || ( c.TypeValue() == float.class)) {
            //    return new SQLQueryCondition<Float>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Float.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Double.class) || ( c.TypeValue() == double.class)) {
            //    return new SQLQueryCondition<Double>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Double.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Boolean.class) || ( c.TypeValue() == boolean.class)) {
            //    return new SQLQueryCondition<Boolean>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            Boolean.valueOf( c.Value() ) );
            //} else if ( ( c.TypeValue() == Character.class) || ( c.TypeValue() == char.class)) {
            //    new SQLQueryCondition<Character>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() ,
            //            c.Value().charAt( 0 ) );
            //} else if ( c.TypeValue() == String.class) {
            //    return new SQLQueryCondition<String>( this , TableName , c.Column() , SQLiteBinaryOperator.equal() , c.Value() );
            //}

            //throw new Exception( "Error type not valid" );
        }
        private void createJoin( DALJoinClause a , DALProjectionColumn aa ) {

            throw new NotImplementedException();
            //SQLQueryCondition <?>[]
            //conditions = null;

            //int i = 0;
            //if ( ( a.Constraint() != null ) && ( a.Constraint().length > 0 ) ) {
            //    conditions = new SQLQueryCondition[a.Constraint().length];
            //    for ( DALJoinConstraint c : a.Constraint() ) {

            //        conditions[i++] = createSQLQueryCondition( c );
            //    }
            //}

            //if ( a.Join() == JoinType.INNER ) {
            //    this.INNERJOIN( a.Table() , a.ForeignKey() , aa.Table() , aa.Name() , conditions );
            //} else if ( a.Join() == JoinType.LEFT ) {
            //    this.LEFTJOIN( a.Table() , a.ForeignKey() , aa.Table() , aa.Name() , conditions );
            //} else if ( a.Join() == JoinType.LEFT ) {
            //    throw new Exception( "Invalid join identifier" );
            //} else {
            //    throw new Exception( "Invalid join identifier" );
            //}

        }

        public void createJoin( DALQueryJoinEntity a , DALQueryJoinEntity aClazz ) {
            throw new NotImplementedException();
        }

        public void createJoin( DALQueryJoinEntity a , DALQueryEntity aClazz ) {
            throw new NotImplementedException();
        }

        public void setOrderBy( List<PropertyInfo> projectionFields ) {
            throw new NotImplementedException();
        }

        public void setGroupBy( List<PropertyInfo> projectionFields ) {
            throw new NotImplementedException();
        }


    }

}