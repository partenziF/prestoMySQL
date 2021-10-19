﻿using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Helper;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using prestoMySQL.ForeignKey;
using prestoMySQL.Utils;
using prestoMySQL.Entity;
using prestoMySQL.Column.Interface;
using prestoMySQL.Database.MySQL;
using prestoMySQL.Adapter;
using prestoMySQL.SQL;

namespace prestoMySQL.Query {

    //public abstract class AbastractSQLQuery : ISQLQuery, IDictionary<string , MySQLQueryParam> {
    //}

    public abstract class SQLQuery : ISQLQuery, IDictionary<string , MySQLQueryParam> {

        public TableGraph Graph;

        protected List<string> mParamNames;

        public List<AbstractEntity> mEntities;

        public DefinableConstraint MakeUniqueParamName( DefinableConstraint c ) {
            if ( c.QueryParams != null ) {
                foreach ( QueryParam qp in c.QueryParams ) {
                    var count = ( mParamNames.Count( c => c.StartsWith( qp.Name ) ) );
                    if ( count > 0 ) {
                        qp.rename( string.Format( "{0}_{1}" , qp.Name , count ) );
                    }
                    mParamNames.Add( qp.Name );
                }
            }
            return c;
        }
        public void MakeUniqueParamName( SQLQueryParams queryParams ) {

            if ( queryParams != null ) {

                foreach ( QueryParam qp in queryParams ) {
                    var count = ( mParamNames.Count( c => c.StartsWith( qp.Name ) ) );
                    if ( count > 0 ) {
                        qp.rename( string.Format( "{0}_{1}" , qp.Name , count ) );
                    }
                    mParamNames.Add( qp.Name );
                }

            }
        }

        public virtual void BuildEntityGraph() {
            mEntities.AddRange( GetListOfEntities() );
            Graph.BuildEntityGraph( mEntities.ToArray() );
        }

        public SQLQuery( QueryAdapter queryAdapter ) {

            //var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition().IsAssignableFrom ( typeof( GenericQueryColumn ) ): false ).ToArray();


            var InstantiableProperties = this.GetType().GetProperties().Where( x => x.PropertyType.IsGenericType ? x.PropertyType.GetGenericTypeDefinition().IsAssignableTo( typeof( GenericQueryColumn ) ) : false ).ToArray();

            foreach ( PropertyInfo p in InstantiableProperties ) {
                var ctors = p.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) } );
                //var ctors = p.PropertyType.GetConstructor( new Type[] { typeof( string ) , typeof( PropertyInfo ) , typeof( SQLQuery ) } );
                p.SetValue( this , ctors.Invoke( new object[] { p.Name , p } ) , null );
            }

            Graph = new TableGraph();

            // TODO Auto-generated constructor stub
            mParamNames = new List<string>();

            mEntities = new List<AbstractEntity>();
            mSelectExpression = new List<string>();
            mWhereCondition = new List<SQLQueryConditionExpression>();
            mJoinTable = new Dictionary<string , SQLQueryJoinTable>( StringComparer.OrdinalIgnoreCase );
            mHashOfSQLQueryTableReference = new Dictionary<string , TableReference>();
            mOrderBy = new List<SQLQueryOrderBy>();
            mGroupBy = new List<SQLQueryGroupBy>();

            mDictionary = new Dictionary<string , MySQLQueryParam>();

            //mHashOfSQLQueryTableReference.Clear();

            var InstantiableFields = this.GetType().GetFields().Where( x => System.Attribute.IsDefined( x , typeof( DALQueryParamAttribute ) ) );
            InstantiableFields?.ToList().ForEach( x => this[x.Name] = new MySQLQueryParam( x.GetValue( this ) , x.GetCustomAttribute<DALQueryParamAttribute>().Name ) );

            mQueryAdapter = queryAdapter;

            //SQLTableEntityHelper.getQueryEntity( this.GetType() )?.ForEach( e => {
            //    mEntities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            //} );

            //SQLTableEntityHelper.getQueryJoinEntity( this.GetType() )?.ForEach( e => {
            //    mEntities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            //} );

            //mEntities.AddRange( GetListOfEntities() );

        }


        internal virtual List<AbstractEntity> GetListOfEntities() {
            List<AbstractEntity> result = new List<AbstractEntity>();
            SQLTableEntityHelper.getQueryEntity( this.GetType() )?.ForEach( e => {
                result.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            } );

            SQLTableEntityHelper.getQueryJoinEntity( this.GetType() )?.ForEach( e => {
                result.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            } );

            return result;
        }

        protected Dictionary<string , MySQLQueryParam> mDictionary;

        private int? mRowCount = null;
        public int? RowCount { get => this.mRowCount; set => this.mRowCount = value; }


        private int? mOffset = null;
        public int? Offset { get => this.mOffset; set => this.mOffset = value; }


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


        protected List<string> mSelectExpression;
        public virtual List<string> SelectExpression {
            get {
                return mSelectExpression;
            }
            set {
                this.mSelectExpression = value;
            }
        }


        public virtual string[] TablesReferences {
            get {
                return mHashOfSQLQueryTableReference.Select( x => x.Value.ToString() ).ToArray();
            }
        }

        protected List<SQLQueryConditionExpression> mWhereCondition;
        public virtual List<SQLQueryConditionExpression> WhereCondition {
            get {
                return mWhereCondition;
            }
            set {
                this.mWhereCondition = value;
            }
        }


        protected Dictionary<string , SQLQueryJoinTable> mJoinTable;
        public virtual Dictionary<string , SQLQueryJoinTable> JoinTable {
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
        public IQueryParams getQueryParams() {

            QueryParam[] p = getParam;
            var result = new SQLQueryParams( p );
            return result;

        }

        public QueryParam[] getParam {
            get {


                List<QueryParam> result = new List<QueryParam>();

                foreach ( SQLQueryJoinTable i in this.mJoinTable.Values ) {
                    if ( i.SqlQueryConditions != null ) {
                        foreach ( var j in i.SqlQueryConditions ) {
                            foreach ( var x in j.getParam() ) {
                                result.Add( x );
                            }
                        }
                    }
                }

                foreach ( var i in this.mWhereCondition ) {
                    foreach ( var j in i.getParam() ) {
                        result.Add( j );
                    }
                }


                ////                Console.WriteLine( "{0}.{1}" , this.GetType().Name , nameof( this.getParam ) );
                ////Console.WriteLine( "=============================" );
                ////Console.WriteLine( "SQLQuery.getParam mJoinTable {0}" , this.mJoinTable.Count );

                //for ( int i = 0; i < this.mJoinTable.Count; i++ ) {

                //    var jt = this.mJoinTable.ElementAt( i );
                //    if ( jt.Value.SqlQueryConditions != null ) {
                //        for ( int k = 0; k < jt.Value.SqlQueryConditions.Length; k++ ) {
                //            var j = jt.Value.SqlQueryConditions[k];
                //            for ( int z = 0; z < j.getParam().Length; z++ ) {
                //                //Console.WriteLine( "\tSQLQuery.getParam {0} = {1}" , j.getParam()[z].Name , j.getParam()[z].Value );
                //                result.Add( j.getParam()[z] );


                //            }

                //        }
                //    }
                //}

                ////Console.WriteLine( "SQLQuery.getParam {0}" , this.mWhereCondition.Count );
                ////for ( int x = 0; x < this.mWhereCondition.Count; x++ ) {
                ////    Console.WriteLine( "SQLQuery.getParam {0}" , this.mWhereCondition[x] );
                ////}
                ////Console.WriteLine( "SQLQuery.getParam mWhereCondition {0}" , this.mWhereCondition.Count );

                //for ( int x = 0; x < this.mWhereCondition.Count; x++ ) {

                //    for ( int z = 0; z < this.mWhereCondition[x].getParam().Length; z++ ) {
                //        var j = this.mWhereCondition[x].getParam()[z];
                //        result.Add( j );
                //        //Console.WriteLine( "\tSQLQuery.getParam {0} = {1}" , j.Name , j.Value );

                //    }
                //    //foreach ( var j in this.mWhereCondition[x].getParam() ) {
                //    //    result.Add( j );
                //    //}
                //}

                //Console.WriteLine( "SQLQuery.getParam return" );
                //Console.WriteLine( "-------------------------------------" );

                return result.ToArray();
            }

        }

        public ICollection<string> Keys => mDictionary.Keys;

        public ICollection<MySQLQueryParam> Values => mDictionary.Values;

        public int Count => mDictionary.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public QueryAdapter mQueryAdapter { get; }


        internal void UpdateValueToQueryParam() => this.GetType()
                                                       .GetFields()
                                                       .Where( x => System.Attribute.IsDefined( x , typeof( DALQueryParamAttribute ) ) )
                                                       .ToList()
                                                       .ForEach( x => {
                                                           if ( this.ContainsKey( x.Name ) ) {
                                                               MySQLQueryParam o;
                                                               if ( this.TryGetValue( x.Name , out o ) ) {
                                                                   o.Value = x.GetValue( this );
                                                               }
                                                           } else {
                                                               this.Add( x.Name , new MySQLQueryParam( x.GetValue( this ) , x.GetCustomAttribute<DALQueryParamAttribute>().Name ) );
                                                           }
                                                       } );

        public MySQLQueryParam this[string key] {
            get => mDictionary[key];
            set {
                if ( mDictionary.ContainsKey( key ) ) {
                    mDictionary[key] = value;
                } else {
                    mDictionary.Add( key , value );
                }
            }
        }

        protected IDictionary<string , TableReference> mHashOfSQLQueryTableReference;

        //////////////////////////////////////////////////////////////////////////////////

        protected List<SQLQueryOrderBy> mOrderBy;
        public List<SQLQueryOrderBy> OrderBy { get => this.mOrderBy; }

        //////////////////////////////////////////////////////////////////////////////////

        private List<SQLQueryGroupBy> mGroupBy;
        public List<SQLQueryGroupBy> GroupBy { get => this.mGroupBy; }

        //////////////////////////////////////////////////////////////////////////////////

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

        //public override string ToString() {

        //    StringBuilder sb = new StringBuilder();

        //    try {


        //        sb.Append( String.Format( "SELECT\r\n\t{0} " , String.Join( ',' , SelectExpression ) ) );
        //        sb.Append( String.Format( "\r\nFROM\r\n\t{0} " , String.Join( ',' , mHashOfSQLQueryTableReference.Select( x => x.Value.ToString() ).ToArray() ) ) );

        //        //    if ( !mJoinTable.isEmpty() ) {
        //        //        String[] j = new String[mJoinTable.size()];
        //        //        int i = 0;
        //        //        for ( SQLQueryJoinTable jt : mJoinTable ) {
        //        //            j[i++] = jt.toString();
        //        //        }
        //        //        sb.append( String.format( "\r\n %s " , String.join( "\r\n\t" , j ) ) );

        //        //    }

        //        if ( ( mWhereCondition.Count > 0 ) ) {

        //            String[] c = new String[mWhereCondition.Count];
        //            int i = 0;
        //            foreach ( SQLQueryConditionExpression sc in mWhereCondition ) {
        //                c[i++] = sc.ToString();
        //            }

        //            sb.Append( String.Format( "\r\nWHERE\r\n\t( {0} )" , String.Join( " AND " , c ) ) );

        //        }

        //        //    if ( !mGroupBy.isEmpty() ) {
        //        //        String[] c = new String[mGroupBy.size()];
        //        //        int i = 0;
        //        //        for ( SQLQueryGroupBy sc : mGroupBy ) {
        //        //            c[i++] = sc.toString();
        //        //        }

        //        //        sb.append( String.format( "\r\nGROUP BY\r\n\t%s " , String.join( "," , c ) ) );

        //        //    }

        //        //    if ( !mOrderBy.isEmpty() ) {
        //        //        String[] c = new String[mOrderBy.size()];
        //        //        int i = 0;
        //        //        for ( SQLQueryOrderBy sc : mOrderBy ) {
        //        //            c[i++] = sc.toString();
        //        //        }

        //        //        sb.append( String.format( "\r\nORDER BY\r\n\t%s " , String.join( "," , c ) ) );
        //        //    }

        //        if ( ( mOffset is not null ) && ( mRowCount is not null ) ) {
        //            sb.Append( String.Format( $"LIMIT {mRowCount} OFFSET {mOffset}" ) );
        //        } else if ( ( mOffset is null ) && ( mRowCount is not null ) ) {
        //            sb.Append( String.Format( $"LIMIT {mRowCount}" ) );
        //        } else if ( ( mOffset is not null ) && ( mRowCount is null ) ) {
        //            throw new ArgumentException( "Invalid argument RowCount can't be null." );
        //        }

        //    } catch ( System.Exception e ) {
        //        //    // TODO Auto-generated catch block
        //        //    e.printStackTrace();
        //    }
        //    return sb.ToString();
        //}


        public int execute() {
            return 0;
        }


        public virtual List<dynamic> GetProjectionColumns<T>( T myQuery ) where T : SQLQuery {

            //var ProjectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( myQuery );
            return SQLTableEntityHelper.getProjectionColumn( this );
        }

        protected virtual List<TableReference> GetListOfTableReference() {
            return SQLTableEntityHelper.getQueryTableName( this.GetType() );
            //foreach ( TableReference e in SQLTableEntityHelper.getQueryTableName( this.GetType() ) ) {
            //    mHashOfSQLQueryTableReference.Add( e.TableName , e );
            //}
        }

        public virtual void Initialize() { //final

            mParamNames.Clear();
            mSelectExpression.Clear();

            mWhereCondition.Clear();
            mJoinTable.Clear();
            mHashOfSQLQueryTableReference.Clear();
            foreach ( TableReference e in GetListOfTableReference() ) {
                mHashOfSQLQueryTableReference.Add( e.TableName , e );
            }
            //foreach ( TableReference e in SQLTableEntityHelper.getQueryTableName( this.GetType() ) ) {
            //    mHashOfSQLQueryTableReference.Add( e.TableName , e );
            //}
            ////for ( TableReference e : SQLTableEntityHelper.getQueryJoinTableName( this.getClass() ) ) {
            ////    mHashOfSQLQueryTableReference.put( e.getTableName() , e );
            ////}

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

            //mJoinTable.TryAdd( Add( new SQLQueryJoinTable( this , JoinType.INNER , aPrimaryTable , aPrimaryKey , aForeignTable , aForeignKey , null ) );

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

            //mJoinTable.Add( new SQLQueryJoinTable( this , JoinType.LEFT , aPrimaryTable , aPrimaryKey , aForeignTable , aForeignKey , null ) );
            //mJoinTable.add( new SQLQueryJoinTable( this , JoinType.LEFT , aPrimaryTable , aPrimaryKey , aForeignTable ,
            //        aForeignKey , fSqlQueryConditions ) );

            return this;
        }


        public void JOIN( JoinTable joinTable , SQLQueryConditionExpression constraint = null ) {

            if ( constraint != null )
                mJoinTable.TryAdd( joinTable.Table.ActualName , new SQLQueryJoinTable( this , joinTable , constraint ) );
            else
                mJoinTable.TryAdd( joinTable.Table.ActualName , new SQLQueryJoinTable( this , joinTable ) );



        }


        internal void JOIN( bool reverse , EntityForeignKey fk , SQLQueryConditionExpression constraint = null ) {
            //return this;
            //if ( fk.foreignKeyInfo.Count == 1 ) {

            //    var a = SQLTableEntityHelper.getColumnName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable , fk.foreignKeyInfo.FirstOrDefault().ReferenceColumnName , false );
            //    var b = SQLTableEntityHelper.getColumnName( fk.Table.GetType() , fk.foreignKeyInfo.FirstOrDefault().ColumnName , false );

            //    //Reverse visit
            //    if ( reverse ) {


            //        if ( constraint != null )
            //            mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , a , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().Table.GetType() ) , b , constraint ) );
            //        else
            //            mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , a , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().Table.GetType() ) , b ) );


            //    } else {

            //        if ( constraint != null )
            //            mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().Table.GetType() ) , b , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , a , constraint ) );
            //        else
            //            mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().Table.GetType() ) , b , SQLTableEntityHelper.getTableName( fk.foreignKeyInfo.FirstOrDefault().TypeReferenceTable ) , a ) );

            //    }


            //} else if ( fk.foreignKeyInfo.Count > 1 ) {
            //    //fk.foreignKeyInfo.Skip( 1 ).ToList();
            //    throw new NotImplementedException( "internal void JOIN( bool reverse  , EntityForeignKey fk , SQLQueryConditionExpression constraint = null ) not implemented" );
            //}


            ////var a = SQLTableEntityHelper.getColumnName( fk.TypeReferenceTable , fk.ReferenceColumnName , false );
            ////var b = SQLTableEntityHelper.getColumnName( fk.Table.GetType() , fk.ColumnName , false );

            //////[DALQueryJoinEntityConstraint( typeof( ProvinceEntity ) , nameof( ProvinceEntity.Delete ) , ParamValue = false )]

            //////Reverse visit
            ////if ( reverse ) {


            ////    if ( constraint != null )
            ////        mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , a , SQLTableEntityHelper.getTableName( fk.Table.GetType() ) , b , constraint ) );
            ////    else
            ////        mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , a , SQLTableEntityHelper.getTableName( fk.Table.GetType() ) , b ) );


            ////} else {

            ////    if ( constraint != null )
            ////        mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.Table.GetType() ) , b , SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , a , constraint ) );
            ////    else
            ////        mJoinTable.TryAdd( SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , new SQLQueryJoinTable( this , fk.JoinType , SQLTableEntityHelper.getTableName( fk.Table.GetType() ) , b , SQLTableEntityHelper.getTableName( fk.TypeReferenceTable ) , a ) );

            ////}




        }

        public SQLQuery GROUPBY( params SQLQueryGroupBy[] aGroupByEntity ) {

            //foreach ( var e in aGroupByEntity ) {
            this.mGroupBy.AddRange( aGroupByEntity );
            //}

            return this;

        }

        public SQLQuery ORDERBY( params SQLQueryOrderBy[] aOrderByEntity ) {

            this.mOrderBy.AddRange( aOrderByEntity );

            //foreach ( var e in aOrderByEntity ) {
            //    this.mOrderBy.Add( e );
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

        public abstract void Prepare();

        public void Add( string key , MySQLQueryParam value ) {
            mDictionary.Add( key , value );
        }

        public bool ContainsKey( string key ) {
            return mDictionary.ContainsKey( key );
        }

        public bool Remove( string key ) {
            return mDictionary.Remove( key );
        }

        public bool TryGetValue( string key , [MaybeNullWhen( false )] out MySQLQueryParam value ) {
            return mDictionary.TryGetValue( key , out value );
        }

        public void Add( KeyValuePair<string , MySQLQueryParam> item ) {
            mDictionary.Add( item.Key , item.Value );
        }

        public void Clear() {
            mDictionary.Clear();
        }

        public bool Contains( KeyValuePair<string , MySQLQueryParam> item ) {
            return mDictionary.Contains( item );
        }

        public void CopyTo( KeyValuePair<string , MySQLQueryParam>[] array , int arrayIndex ) {
            throw new NotImplementedException();
        }

        public bool Remove( KeyValuePair<string , MySQLQueryParam> item ) {

            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string , MySQLQueryParam>> GetEnumerator() {
            return mDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return mDictionary.GetEnumerator();
        }

        internal virtual List<string> GetProjectionColumnName<T>( T sqlQuery ) where T : SQLQuery {
            return SQLTableEntityHelper.getProjectionColumnName<T>( sqlQuery );
        }

        internal virtual IEnumerable<DALGenericQueryJoinConstraint> GetQueryJoinConstraint() {
            return SQLTableEntityHelper.getQueryJoinConstraint( this.GetType() );
        }
        internal virtual IEnumerable<DALQueryJoinEntityUnConstraint> GetQueryJoinUnConstraint() {
            return SQLTableEntityHelper.getQueryJoinUnConstraint( this.GetType() );
        }



        public void Build() {

            //Console.WriteLine( "{0}.{1}" , this.GetType().Name , nameof( this.Build ) );

            UpdateValueToQueryParam();

            //SQLQueryParams outparam = null;
            //outparam = null;

            Prepare();

            List<DALGroupBy> groupby = SQLTableEntityHelper.getQueryGroupBy( this.GetType() );
            if ( groupby.Count > 0 ) {
                List<SQLQueryGroupBy> listOfQueryGroupBy = new List<SQLQueryGroupBy>();
                var order = 0;

                foreach ( var o in groupby ) {
                    if ( Graph.Cache.ContainsKey( o.Table ) ) {
                        var cn = Helper.SQLTableEntityHelper.getColumnName( Graph.Cache[o.Table].FirstOrDefault() , o.Property , true );
                        listOfQueryGroupBy.Add( new SQLQueryGroupBy( order++ , cn ) );
                    }

                }

                GROUPBY( listOfQueryGroupBy.ToArray() );
            }


            //var xxx = GetProjectionColumns( this );
            //foreach (var c in xxx ) {
            //    Console.WriteLine( c );
            //}
            

            var orderby = SQLTableEntityHelper.getQueryOrderBy( this.GetType() );
            if ( orderby.Count > 0 ) {
                List<SQLQueryOrderBy> listOfQueryOrderBy = new List<SQLQueryOrderBy>();
                var order = 0;

                foreach ( var o in orderby ) {
                    if ( Graph.Cache.ContainsKey( o.Table ) ) {
                        var cn = Helper.SQLTableEntityHelper.getColumnName( Graph.Cache[o.Table].FirstOrDefault() , o.Property , true );
                        listOfQueryOrderBy.Add( new SQLQueryOrderBy( order++ , cn , Column.Attribute.OrderType.ASC ) );
                    }

                }
                if ( this.GroupBy.Count > 0 )
                    ORDERBY( listOfQueryOrderBy.ToArray() );
                else
                    ORDERBY( listOfQueryOrderBy.ToArray() );

            }

            LIMIT( Offset , RowCount );
        }


    }

}