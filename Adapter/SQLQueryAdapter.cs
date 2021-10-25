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
using prestoMySQL.Database.Interface;
using prestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using prestoMySQL.Column.Interface;
using prestoMySQL.Query.SQL;

namespace prestoMySQL.Adapter {


    public abstract class SQLQueryAdapter<T, X> : QueryAdapter, IEnumerable<X> where T : SQLQuery where X : IInstantiableAdapterRow {

        internal string mSQLQueryString;
        public string SQLQueryString { get => mSQLQueryString; }

        public SQLQueryAdapter( MySQLDatabase database ) : base( database ) {

            mSqlQuery = ( T ) createSqlQuery();

            //SQLTableEntityHelper.getQueryEntity( typeof( T ) )?.ForEach( e => {
            //    entities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            //} );
            //SQLTableEntityHelper.getQueryJoinEntity( typeof( T ) )?.ForEach( e => {
            //    entities.Add( ( AbstractEntity ) Activator.CreateInstance( e ) );
            //} );


            mSqlQuery.BuildEntityGraph();

        }

        public override CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get => mCursor; set => mCursor = value; }

        public T mSqlQuery;
        public T sqlQuery { get => mSqlQuery; }
        public override int SQLCount {
            get {
                //sqlQuery.UpdateValueToQueryParam();

                //SQLQueryParams outparam = null;
                //sqlQuery.Prepare();
                //sqlQuery.SelectExpression.Clear();

                //var RowCount = sqlQuery.RowCount;
                //var Offset = sqlQuery.Offset;
                //sqlQuery.LIMIT( null , null );
                //sqlQuery.SelectExpression.Add( "COUNT(*)" );

                SQLQueryParams outparam = null;

                sqlQuery.Build();
                sqlQuery.SelectExpression.Clear();
                sqlQuery.SelectExpression.Add( "COUNT(*)" );

                var RowCount = sqlQuery.RowCount;
                var Offset = sqlQuery.Offset;
                sqlQuery.LIMIT( null , null );

                mSQLQueryString = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );

                sqlQuery.LIMIT( Offset , RowCount );

                return mDatabase.ExecuteScalar<int?>( mSQLQueryString , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? -1;

            }
        }

        protected virtual T createSqlQuery() {
            //T result = ( T ) Activator.CreateInstance( typeof( T ) );            
            var ctor = typeof( T ).GetConstructor( new Type[] { this.GetType() } );
            T result = ( T ) ctor?.Invoke( new object[] { this } );
            return result;
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
                        if ( ctor is null ) throw new System.Exception( String.Format( "Can't find constructor for type {0} with parameter {1}" , typeof( X ).FullName ,t.FullName));
                        mCurrent = ( X ) ctor?.Invoke( new object[] { this.mAdapter.sqlQuery } );
                        return true;
                    } else {
                        return false;
                    }

                } catch ( System.Exception e ) {
                    throw;
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

        protected override void BindData( Dictionary<string , Dictionary<string , int>> s , IReadableResultSet resultSet ) {


            int? index = null;
            if ( ProjectionColumns is null ) throw new System.Exception( "Undefined ProjectionColumns" );

            //foreach ( GenericQueryColumn column in ProjectionColumns ) {
            foreach ( dynamic column in ProjectionColumns ) {

                index = null;

                if ( s.ContainsKey( column.Table.ActualName ) ) {

                    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                        index = s[column.Table.ActualName][column.ActualName];
                    }


                }


                if ( index == null ) throw new System.Exception( "Invalid index column." );

                object v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
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

            SQLQueryParams outparam = null;

            sqlQuery.Build();

            this.mSQLQueryString = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );

            var result = mDatabase.ReadQuery( this.mSQLQueryString , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            Message = mDatabase.LastError;

            return result;

        }

        IEnumerator<X> IEnumerable<X>.GetEnumerator() {

            ProjectionColumns?.Clear();
            //ProjectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( sqlQuery );
            ProjectionColumns = sqlQuery.GetProjectionColumns<T>( sqlQuery );

            return ( IEnumerator<X> ) this.GetEnumerator();
        }
    }



    //public abstract class SQLQueryAdapter<T, U, X> : QueryAdapter, IEnumerable<X> where T : EntityAdapter<U> where U : AbstractEntity where X : IInstantiableAdapterRow {

    //    public SQLQueryAdapter( MySQLDatabase db ) {

    //        List<AbstractEntity> entities = new List<AbstractEntity>();
    //        mDatabase = db;
    //        //mSqlQuery = ( T ) createSqlQuery();

    //        mAdapter = ReflectionTypeHelper.NewInstanceAdapter<T>( mDatabase );
    //        mAdapter.Create();
    //    }

    //    public readonly MySQLDatabase mDatabase;

    //    private T mAdapter;

    //    public override int SQLCount {
    //        get {

    //            return 0;
    //            //sqlQuery.UpdateValueToQueryParam();

    //            //SQLQueryParams outparam = null;
    //            //sqlQuery.Prepare();
    //            //sqlQuery.SelectExpression.Clear();
    //            //sqlQuery.SelectExpression.Add( "COUNT(*)" );
    //            //var sql = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );
    //            //return mDatabase.ExecuteScalar<int?>( sql , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() ) ?? -1;

    //        }
    //    }

    //    public class AdapterIterator<X> : IEnumerator<X> where X : IInstantiableAdapterRow {
    //        public AdapterIterator( dynamic queryAdapter ) {
    //            mQueryAdapter = queryAdapter;
    //        }

    //        private dynamic mQueryAdapter;

    //        private X mCurrent;
    //        public X Current => mCurrent;

    //        object IEnumerator.Current => throw new NotImplementedException();

    //        public void Dispose() {
    //            mQueryAdapter.Cursor.Close();
    //        }

    //        public bool MoveNext() {

    //            try {

    //                if ( mQueryAdapter.Cursor.MoveNext() ) {

    //                    var rs = mQueryAdapter.Cursor.Current;
    //                    Dictionary<string , Dictionary<string , int>> s = rs.ResultSetSchemaTable();
    //                    this.mQueryAdapter.BindData( s , rs );
    //                    Type t = typeof( T );

    //                    var ctor = typeof( X ).GetConstructor( new Type[] { t } );
    //                    this.mQueryAdapter.mAdapter.MapFromAdapter();
    //                    mCurrent = ( X ) ctor?.Invoke( new object[] { this.mQueryAdapter.mAdapter } );
    //                    return true;
    //                } else {
    //                    return false;
    //                }

    //            } catch ( System.Exception e ) {
    //                throw new ArgumentNullException( e.Message );
    //            }

    //        }

    //        public void Reset() {
    //            throw new NotImplementedException();
    //        }
    //    }


    //    public IEnumerator GetEnumerator() {

    //        AdapterIterator<X> ai = null;

    //        ILastErrorInfo message = null;

    //        try {

    //            Cursor = new CursorWrapper<MySQResultSet , MySqlDataReader>( ExecuteQuery( out message ) , message );
    //            ai = new AdapterIterator<X>( this );
    //            if ( Cursor.mResultSet is null ) {
    //                throw new ArgumentNullException( "Invalid resultset. " + Cursor.LastError.ToString() );
    //            }

    //        } catch ( ArgumentNullException ex ) {
    //            throw new System.Exception( "Invalid resultset. " + ex.Message );
    //        } catch ( System.Exception e ) {
    //            throw new System.Exception( "Error while reading data." );
    //        }

    //        return ai;

    //    }

    //    IEnumerator<X> IEnumerable<X>.GetEnumerator() {

    //        ProjectionColumns?.Clear();

    //        //var definitionColumn = SQLTableEntityHelper.getDefinitionColumn( mAdapter.Entity , true );
    //        //foreach ( var column in definitionColumn ) {

    //        //    SQLTypeWrapper<uint> b = null;
    //        //    MySQLDefinitionColumn<SQLTypeWrapper<uint>> a = null;

    //        //    var ttt = ( column as dynamic ).GenericType;


    //        //    var t = typeof( SQLTypeWrapper<> ).MakeGenericType( ( column as dynamic ).GenericType );
    //        //    var tt = typeof( MySQLDefinitionColumn<> ).MakeGenericType( t );
    //        //    var x = Convert.ChangeType( column ,tt);
    //        //    //SQLProjectionColumn < SQLTypeWrapper<uint> > x = column;

    //        //    ProjectionColumns?.Add( x );

    //        //}
    //        //ProjectionColumns = SQLTableEntityHelper.getProjectionColumn<T>( sqlQuery );

    //        return ( IEnumerator<X> ) this.GetEnumerator();
    //    }


    //    protected override void BindData( Dictionary<string , Dictionary<string , int>> s , IReadableResultSet resultSet ) {


    //        int? index = null;

    //        List<dynamic> definitionColumns = new List<dynamic>();
    //        definitionColumns.AddRange( SQLTableEntityHelper.getDefinitionColumn( mAdapter.Entity , true ).ToList() );
    //        foreach ( ConstructibleColumn column in definitionColumns ) {

    //            index = null;

    //            if ( s.ContainsKey( column.Table.ActualName ) ) {
    //                if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
    //                    index = s[column.Table.ActualName][column.ActualName];
    //                }
    //            }


    //            if ( index == null ) throw new System.Exception( "Invalid index column. (" + column.ActualName + ")" );


    //            var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
    //                                                       typeof( MySQResultSet ) ,
    //                                                       resultSet ,
    //                                                       nameof( MySQResultSet.getValueAs ) ,
    //                                                       new Type[] { typeof( int ) } ,
    //                                                       new object[] { ( int ) index } );

    //            if ( v.IsDBNull() ) {
    //                ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
    //            } else if ( v is null ) {
    //                ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
    //            } else {
    //                column.AssignValue( v );
    //            }


    //        }


    //    }


    //    protected override CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get => mCursor; set => mCursor = value; }

    //    public override MySQResultSet ExecuteQuery( out ILastErrorInfo Message ) {

    //        SQLQueryParams outparam = null;

    //        //sqlQuery.Prepare();

    //        //sqlQuery.LIMIT( Offset , RowCount );

    //        //var sql = SQLBuilder.sqlQuery<T>( sqlQuery , ref outparam , "@" );
    //        var sql = SQLBuilder.sqlSelect<U>( ref outparam );

    //        var result = mDatabase.ReadQuery( sql , outparam?.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

    //        Message = mDatabase.LastError;

    //        return result;


    //        /*
    //         *             SQLQueryParams outparam = null;

    //                    MySQLDatabase db = new MySQLDatabase( "root" , "root" , "localhost" , "multilevelmarketing" , 3306 , logger );

    //                    var a = new EntityAdapterAlbums.AlbumsEntity();

    //                    EntityConstraint<dynamic>[] c = new EntityConstraint<dynamic>[1];
    //                    MySQLQueryParam p1 = new MySQLQueryParam( 1 , nameof( a.AlbumId ) );
    //                    MySQLQueryParam p2 = new MySQLQueryParam( "ciao" , nameof( a.Title ) );

    //                    EntityConstraint<uint> Key1 = new EntityConstraint<uint>( a.AlbumId , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { p1 } ) , "@" );
    //                    EntityConstraint<string> Key2 = new EntityConstraint<string>( a.Title , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { p2 } ) , "@" );
    //                    BetweenEntityConstraint<short> Key3 = new BetweenEntityConstraint<short>( a.ArtistId , new SQLQueryParams( new[] { new MySQLQueryParam( 1 , nameof( a.ArtistId ) + "1" ) , new MySQLQueryParam( 5 , nameof( a.ArtistId ) + "2" ) } ) , "@" );


    //                    var s = SQLBuilder.sqlSelect<EntityAdapterAlbums.AlbumsEntity>( ref outparam , Constraint: new EntityConstraintExpression( Key1 , Key2 , Key3 ) );

    //                    if ( db.OpenConnection() ) {

    //                        var rs = db.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

    //                        var cursor = new CursorWrapper<MySQResultSet , MySqlDataReader>( rs );
    //                        foreach ( PrestoMySQL.Database.MySQL.MySQResultSet row in cursor ) Console.WriteLine( row.getString( 1 ) );

    //                        if ( rs != null ) {

    //                            Console.WriteLine( s );

    //                            foreach ( MySQLQueryParam x in outparam )
    //                                Console.WriteLine( x.Name + " " + x );

    //                            while ( rs.fetch() ) {

    //                                Console.WriteLine( rs.getInt( 0 ) + " " + rs.getString( 1 ) );

    //                            }

    //                        }

    //                    }

    //                    db.Close();
    //        */

    //    }


    //}



}