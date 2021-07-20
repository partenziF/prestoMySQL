using MySqlConnector;
using prestoMySQL.Column;
using prestoMySQL.Database.Cursor;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace prestoMySQL.Adapter {

    public abstract class SQLQueryAdapter<T, X> : QueryAdapter, IEnumerable<X> where T : SQLQuery where X : IInstantiableAdapterRow {


        public SQLQueryAdapter( MySQLDatabase db ) {

            List<AbstractEntity> entities = new List<AbstractEntity>();
            mDatabase = db;
            mSqlQuery = ( T ) createSqlQuery();


            SQLTableEntityHelper.getQueryEntity( typeof( T ) )?.ForEach( e => {
                entities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            } );

            SQLTableEntityHelper.getQueryJoinEntity( typeof( T ) )?.ForEach( e => {
                entities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            } );


            mSqlQuery.Graph.BuildEntityGraph( entities.ToArray() );

            /*                var a = new ComuniEntity();
                            var b = new ProvinceEntity();
                            var c = new RegioniEntity();

                            Graph.BuildEntityGraph( a , b , c );
*/

            //mCache = new Dictionary<Type , List<TableEntity>>();
        }

        protected override CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get => mCursor; set => mCursor = value; }

        protected T mSqlQuery;
        public T sqlQuery { get => mSqlQuery; }
        public override int SQLCount {
            get {
                sqlQuery.UpdateValueToQueryParam();

                SQLQueryParams outparam = null;
                sqlQuery.Prepare();
                sqlQuery.SelectExpression.Clear();
                sqlQuery.SelectExpression.Add( "COUNT(*)" );
                var sql = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );
                return mDatabase.ExecuteScalar<int?>( sql , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? -1;

            }
        }

        public readonly MySQLDatabase mDatabase;

        protected virtual T createSqlQuery() {
            return ( T ) Activator.CreateInstance( typeof( T ) );
        }

        public class AdapterIterator<X> : IEnumerator<X> where X : IInstantiableAdapterRow {
            public AdapterIterator( dynamic adapter ) {
                mAdapter = adapter;
            }

            private dynamic mAdapter;

            private X mCurrent;
            public X Current => mCurrent;

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose() {
                mAdapter.Cursor.Close();
            }

            public bool MoveNext() {

                try {

                    if ( mAdapter.Cursor.MoveNext() ) {

                        var rs = mAdapter.Cursor.Current;
                        Dictionary<string , Dictionary<string , int>> s = rs.ResultSetSchemaTable();
                        this.mAdapter.BindData( s , rs );
                        Type t = typeof( T );

                        var ctor = typeof( X ).GetConstructor( new Type[] { t } );
                        mCurrent = ( X ) ctor?.Invoke( new object[] { this.mAdapter.sqlQuery } );
                        return true;
                    } else {
                        return false;
                    }

                } catch ( System.Exception e ) {
                    throw new ArgumentNullException( e.Message );
                }

            }

            public void Reset() {
                throw new NotImplementedException();
            }
        }

        public IEnumerator GetEnumerator() {

            AdapterIterator<X> ai = null;

            ILastErrorInfo message = null;

            try {

                Cursor = new CursorWrapper<MySQResultSet , MySqlDataReader>( ExecuteQuery( out message ) , message );
                ai = new AdapterIterator<X>( this );
                if ( Cursor.mResultSet is null ) {
                    throw new ArgumentNullException( "Invalid resultset. " + Cursor.LastError.ToString() );
                }

            } catch ( ArgumentNullException ex ) {
                throw new System.Exception( "Invalid resultset. " + ex.Message );
            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while reading data." );
            }

            return ai;

        }


        //public IEnumerable<X> GetAll() {
        //    var result = new List<X>();
        //    ProjectionColumns?.Clear();
        //    ProjectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( sqlQuery );            
        //    IEnumerator<X> i = ( IEnumerator<X> ) this.GetEnumerator();
        //    while ( i.MoveNext() )
        //        result.Add( i.Current );
        //    Cursor.Close();
        //    return ( List<X> ) result;
        //}

        protected override void BindData( Dictionary<string , Dictionary<string , int>> s , MySQResultSet resultSet ) {


            int? index = null;

            foreach ( GenericQueryColumn column in ProjectionColumns ) {

                index = null;

                if ( s.ContainsKey( column.Table.ActualName ) ) {

                    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                        index = s[column.Table.ActualName][column.ActualName];
                    }


                }


                if ( index == null ) throw new System.Exception( "Invalid index column." );

                var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                                                           typeof( MySQResultSet ) ,
                                                           resultSet ,
                                                           nameof( MySQResultSet.getValueAs ) ,
                                                           new Type[] { typeof( int ) } ,
                                                           new object[] { ( int ) index } );

                if ( v.IsDBNull() ) {
                    ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                } else if ( v is null ) {
                    ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                } else {
                    ( ( dynamic ) column ).AssignValue( v );
                }


                }

            }

                    public override MySQResultSet ExecuteQuery( out ILastErrorInfo Message ) {

            sqlQuery.UpdateValueToQueryParam();

            SQLQueryParams outparam = null;

            sqlQuery.Prepare();

            sqlQuery.LIMIT( Offset , RowCount );

            var sql = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );

            var result = mDatabase.ReadQuery( sql , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            Message = mDatabase.LastError;

            return result;

            //return null;

            //throw new NotImplementedException();

            //SQLQueryCondition cc = new SQLQueryCondition<string>( sqlQuery , "drinks" , "Name" , SQLiteBinaryOperator.notEqual() , ( string ) myParams.getParamValue( 0 ) , true );
            //SQLQueryConditionExpression condition = new SQLQueryConditionExpression( SQLQueryConditionExpression.LogicOperator.OR , cc );


            ////		SQLiteQueryParams myParams = new SQLiteQueryParams(1);
            ////		myParams.setParam(0, "prova");
            //SQLiteQueryParams queryParams = new SQLiteQueryParams();

            //SQLBuilder.SELECT( sqlQuery );
            ////		amyParams.setParam(0, "0");
            ///*
            //ArrayList<DALQueryEntity> entityQuery = SQLTableEntityHelper.getQueryEntity(this.getSqlQuery().getClass());
            //ArrayList<DALQueryJoinEntity> entityJoin = SQLTableEntityHelper.getQueryJoinEntity(this.getSqlQuery().getClass());

            //for (DALQueryJoinEntity a : entityJoin) {
            //    createJoin(a,entityQuery.get(0));
            //}


            //for (int i = 0; i<entityJoin.size();i++) 			
            //    for (int j=i+1;j<entityJoin.size();j++) 
            //        createJoin(entityJoin.get(i),entityJoin.get(j));

            //*/
            //sqlQuery.WHERE( condition );

            ////		String query = sqlQuery.toString();

            //string query = query = SQLBuilder.sqlSelect( sqlQuery , "?" , queryParams );

            //Console.WriteLine( query );
            //ReadableResultSet<ResultSet> rs = database.executeQuery( query , queryParams.asArray() );

            //return rs;


        }

        IEnumerator<X> IEnumerable<X>.GetEnumerator() {

            ProjectionColumns?.Clear();
            ProjectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( sqlQuery );

            return ( IEnumerator<X> ) this.GetEnumerator();
        }
    }



}