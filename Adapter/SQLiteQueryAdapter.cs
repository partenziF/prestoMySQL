using MySqlConnector;
using prestoMySQL.Database.Cursor;
using prestoMySQL.Extension;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Adapter {

    public abstract class SQLiteQueryAdapter<T, X> : QueryAdapter,  IEnumerable<X> where T : SQLQuery where X : IInstantiableAdapterRow {

        public SQLiteQueryAdapter( MySQLDatabase db ) {
            mDatabase = db;
            mSqlQuery = ( T ) createSqlQuery();
        }

        public override CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get => mCursor; set => mCursor = value; }

        protected T mSqlQuery;
        public T sqlQuery { get => mSqlQuery; }

        //protected IQueryParams myParams;
        public readonly MySQLDatabase mDatabase;

        public virtual T createSqlQuery() {
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
                throw new NotImplementedException();
            }

            public bool MoveNext() {
                try {

                    if ( mAdapter.Cursor.MoveNext() ) {

                        var rs = mAdapter.Cursor.Current;
                        this.mAdapter.BindData( rs );
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

            try {

                Cursor = new CursorWrapper<MySQResultSet , MySqlDataReader>( ExecuteQuery() );
                ai = new AdapterIterator<X>( this );

            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while reading data." );
            }

            return ai;

        }
        //Count
        IEnumerator<X> IEnumerable<X>.GetEnumerator() {
            var l = new List<X>( 100 );
            return l.GetEnumerator();

        }

        public List<X> asList() {

            var result = new List<X>();

            projectionColumns?.Clear();
            projectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( sqlQuery );

            IEnumerator<X> i = ( IEnumerator<X> ) this.GetEnumerator();
            while ( i.MoveNext() )
                result.Add( i.Current );

            Cursor.Close();

            return ( List<X> ) result;
        }

        public override void BindData( MySQResultSet resultSet ) {

            foreach ( var column in projectionColumns ) {

                //need explicit conversion to work
                if ( resultSet[( string ) ( column.ColumnAlias ?? column.ColumnName )].IsDBNull() ) {

                    var o = typeof( SQLTypeWrapper<> ).MakeGenericType( column.GenericType );
                    var p = o.GetField( "NULL" , BindingFlags.Static | BindingFlags.Public );
                    column.TypeWrapperValue = p.GetValue( null );

                } else {

                    MethodInfo method = typeof( MySQResultSet ).GetMethod( nameof( MySQResultSet.getValueAs ) , new Type[] { typeof( string ) } );
                    MethodInfo generic = method.MakeGenericMethod( column.GenericType );
                    var o = generic.Invoke( resultSet , new object[] { ( string ) column.ColumnName } );
                    column.AssignValue( o );

                }

            }

        }

        public override MySQResultSet ExecuteQuery() {
            
            SQLQueryParams outparam = null;
            PrepareQuery();
            
            sqlQuery.LIMIT( Offset , RowCount );
            
            var sql = SQLBuilder.sqlSelect( sqlQuery , ref outparam , "@" );

            return mDatabase.ReadQuery( sql , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

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
    }



}