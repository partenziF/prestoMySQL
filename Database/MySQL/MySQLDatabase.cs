using Microsoft.Extensions.Logging;
using MySqlConnector;
using prestoMySQL.Database;
using prestoMySQL.Extension;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

/* 
 * Use this: dotnet add package MySqlConnector
 *                              Microsoft.Extensions.Logging.Abstractions              
*/
namespace prestoMySQL.Database.MySQL {

    //https://www.thomasclaudiushuber.com/2020/09/01/c-9-0-records-work-with-immutable-data-classes/

    public record MySqlLastErrorInfo( MySqlException ex ) : LastErrorInfo( e: ex.GetBaseException() ) {

        public int Number { get; init; } = ex.Number;
        //public string Message { get; init; } = ex.Message;
        public MySqlErrorCode ErrorCode { get; init; } = ex.ErrorCode;

        public override string ToString() {
            return String.Format( "[{0}] {1} : {2}" , ErrorCode , Number , Message );
        }

    }

    public class MySQLDatabase : GenericDatabase, IDisposable {

        public readonly uint mTimeout;
        public string mCharset;

        public string ConnectionString {
            get {
                if ( string.IsNullOrWhiteSpace( this.mConnectionString ) ) {
                    MySqlConnectionStringBuilder sb;
                    if ( string.IsNullOrWhiteSpace( mCharset ) ) {

                        sb = new MySqlConnectionStringBuilder() {
                            Server = this.mHost ,
                            UserID = this.mUsername ,
                            Password = this.mPassword ,
                            Database = this.mDatabase ,
                            Port = this.mPort ,
                            ConnectionTimeout = this.mTimeout ,
                            DefaultCommandTimeout = this.mTimeout

                        };

                    } else {

                        sb = new MySqlConnectionStringBuilder() {
                            Server = this.mHost ,
                            UserID = this.mUsername ,
                            Password = this.mPassword ,
                            Database = this.mDatabase ,
                            Port = this.mPort ,
                            ConnectionTimeout = this.mTimeout ,
                            DefaultCommandTimeout = this.mTimeout ,
                            CharacterSet = this.mCharset
                        };

                    }

                    return sb.ConnectionString;


                } else {
                    return this.mConnectionString;
                }
            }
        }

        MySqlConnection mConnection;
        protected override bool getIsConnected() {
            if ( this.mConnection != null ) {
                this.mIsConnected = ( this.mIsConnected ) && ( ( this.mConnection.State == ConnectionState.Open ) || ( this.mConnection.State == ConnectionState.Fetching ) || ( this.mConnection.State == ConnectionState.Executing ) );
            } else {
                this.mIsConnected = false;
            }

            return base.getIsConnected();

        }
        public MySqlConnection Connection { get { return mConnection; } }


        private MySqlTransaction mTransaction;


        private MySqlCommand mCommand;

        public MySqlCommand Command {

            get {

                if ( this.isConnected ) {

                    if ( mCommand == null ) {
                        mCommand = new MySqlCommand();
                    }

                    //unecessary if isConnected
                    if ( mCommand.Connection == null ) {
                        mCommand.Connection = this.mConnection;
                    }

                    return mCommand;

                } else {
                    return null;
                }
            }
        }

        public MySQLDatabase( string mConnectionString , ILogger aLogger = null ) : base( mConnectionString , aLogger ) {
            this.mTimeout = 15;
        }

        public MySQLDatabase( string mConnectionString , ILogger aLogger = null , uint Timeout = 15 ) : base( mConnectionString , aLogger ) {
            this.mTimeout = Timeout;
        }

        public MySQLDatabase( string aUsername , string aPassword , string aHost , string aDatabase , uint aPort = 3306 , ILogger aLogger = null , uint Timeout = 15 ) : base( aUsername , aPassword , aHost , aDatabase , aPort , aLogger ) {
            this.mTimeout = Timeout;
        }


        public void Dispose() {

            Logger?.LogTrace( $"{nameof( Dispose )} {{0}}" , new { Connection } );

            mConnection?.Dispose();
            mCommand?.Dispose();
            mTransaction?.Dispose();

        }

        protected override bool DoOpenConnection() {

            Logger?.LogTrace( $"{nameof( DoOpenConnection ) } {{0}}" , new { mConnection } );

            try {

                if ( mConnection == null )
                    mConnection = new MySqlConnection( this.ConnectionString );

                if ( ( mConnection.State == ConnectionState.Closed ) || ( mConnection.State == ConnectionState.Broken ) )
                    mConnection.Open();

                this.LastError = null;

                return true;

            } catch ( MySqlException e ) {

                LastError = new MySqlLastErrorInfo( e );
                Logger?.LogWarning( $"{ nameof( DoOpenConnection )} Exception message : {{0}} " , LastError );
                return false;
            }
        }

        protected override async Task<bool> DoOpenConnectionAsync() {
            Logger?.LogTrace( $"{nameof( DoOpenConnection ) } {{0}}" , new { mConnection } );

            try {

                if ( mConnection == null )
                    mConnection = new MySqlConnection( this.ConnectionString );

                if ( ( mConnection.State == ConnectionState.Closed ) || ( mConnection.State == ConnectionState.Broken ) )
                    await mConnection.OpenAsync();

                return true;

            } catch ( MySqlException e ) {

                LastError = new MySqlLastErrorInfo( e );
                Logger?.LogWarning( $"{ nameof( DoOpenConnection )} Exception message : {{0}} " , LastError );
                return false;
            }
        }


        protected override bool DoCloseConnection() {

            Logger?.LogTrace( $"{nameof( DoCloseConnection )} {{0}}" , new { mConnection } );

            try {
                if ( ( mConnection != null ) && ( mConnection.State == ConnectionState.Open ) )
                    mConnection.Close();
                return true;
            } catch ( MySqlException e ) {

                Logger?.LogWarning( $"{nameof( DoOpenConnection )} Exception message : {{0}} : {{1}}" , e.ErrorCode , e.Message );
                LastError = new MySqlLastErrorInfo( e );
                return false;
            }

        }


        protected override async Task<bool> DoCloseConnectionAsync() {

            Logger?.LogTrace( $"{nameof( DoCloseConnection )} {{0}}" , new { mConnection } );

            try {
                if ( ( mConnection != null ) && ( mConnection.State == ConnectionState.Open ) )
                    await mConnection.CloseAsync();
                return true;
            } catch ( MySqlException e ) {

                Logger?.LogWarning( $"{nameof( DoOpenConnection )} Exception message : {{0}} : {{1}}" , e.ErrorCode , e.Message );
                LastError = new MySqlLastErrorInfo( e );
                return false;
            }
        }

        public int? ExecuteQuery( string aSQLQuery , params MySqlParameter[] args ) {

            int? result = null;

            if ( isConnected ) {

                try {

                    this.LastError = null;

                    Command.CommandText = aSQLQuery;

                    if ( mTransaction != null )
                        this.mCommand.Transaction = mTransaction;

                    mCommand.Parameters.Clear();

                    if ( args.Length > 0 ) {
                        mCommand.Parameters.AddRange( args );
                    }


#if DEBUG
                    Console.WriteLine( "[{0}]\r\n{1}\r\n{2}" , DateTime.Now.ToString( "G" ) , Command.CommandText , String.Join( "\r\n" , args.ToList().Select( a => $"{a.DbType} {a.ParameterName} = {a.Value}" ).ToArray() ) );
#endif


                    result = Command.ExecuteNonQuery();

                } catch ( MySqlException ex ) {
                    Logger?.LogWarning( $"{nameof( ExecuteQueryAsync )} {{0}}" , ex.Message );
                    this.LastError = new LastErrorInfo( ex );
                }

            } else {
                throw new System.Exception( "Connection is closed" );
            }

            return result;

        }

        public MySQResultSet ReadQuery( string aSQLQuery , params MySqlParameter[] args ) {

            if ( isConnected ) {

                try {
                    this.LastError = null;

                    Command.CommandText = aSQLQuery;

#if DEBUG
                    Console.WriteLine( "[{0}]\r\n{1}\r\n{2}" , DateTime.Now.ToString( "G" ) , Command.CommandText , String.Join( "\r\n" , args.ToList().Select( a => $"{a.DbType} {a.ParameterName} = {a.Value}" ).ToArray() ) );
#endif


                    if ( mTransaction != null )
                        this.mCommand.Transaction = mTransaction;

                    mCommand.Parameters.Clear();

                    if ( args?.Length > 0 ) {
                        mCommand.Parameters.AddRange( args );
                    }



                    var rs = Command.ExecuteReader();



                    if ( ( Logger is not null ) && ( args?.Length > 0 ) ) {
                        args.ToList().ForEach( a => Logger?.LogDebug( a.ParameterName + " " + a.Value ) );
                    }


                    return new MySQResultSet( rs );

                } catch ( MySqlException ex ) {
                    Logger?.LogWarning( $"{nameof( ExecuteQueryAsync )} {{0}}" , ex.Message );
                    this.LastError = new LastErrorInfo( ex );
                }

            } else {
                throw new System.Exception( "Connection is closed" );
            }

            return null;
        }

        public async Task<MySQResultSet> ExecuteQueryAsync( string aSQLQuery , params MySqlParameter[] args ) {

            if ( isConnected ) {

                try {

                    this.LastError = null;

                    Command.CommandText = aSQLQuery;

                    if ( mTransaction != null )
                        this.mCommand.Transaction = mTransaction;

                    mCommand.Parameters.Clear();

                    if ( args.Length > 0 ) {
                        //for ( int i = 0; i < args.Length; i++ ) {
                        mCommand.Parameters.AddRange( args );
                        //}
                    }

#if DEBUG
                    Console.WriteLine( "[{0}]\r\n{1}\r\n{2}" , DateTime.Now.ToString( "G" ) , Command.CommandText , String.Join( "\r\n" , args.ToList().Select( a => $"{a.DbType} {a.ParameterName} = {a.Value}" ).ToArray() ) );
#endif

                    var rs = await Command.ExecuteReaderAsync();

                    return new MySQResultSet( rs );

                    //Task<MySqlDataReader> r =  Command.ExecuteReaderAsync();
                    //var x = (await r).ReadAsync();
                    //return o;

                    /* SqlCommand command = connection.CreateCommand();
                             SqlTransaction transaction = null;

                             // Start a local transaction.
                             transaction = await Task.Run<SqlTransaction>(
                                 () => connection.BeginTransaction("SampleTransaction")
                                 );

                             // Must assign both transaction object and connection
                             // to Command object for a pending local transaction
                             command.Connection = connection;
                             command.Transaction = transaction;
                    */

                    //async ... yield return and await foreach

                    //var x = new MySQLiteResultSet<MySqlDataReader>( r );





                    //while ( rdr.Read() ) {
                    //    Console.WriteLine( rdr[0] + " -- " + rdr[1] );
                    //}
                    //rdr.Close();

                } catch ( MySqlException ex ) {
                    Logger?.LogWarning( $"{nameof( ExecuteQueryAsync )} {{0}}" , ex.Message );
                    this.LastError = new LastErrorInfo( ex );

                    //Console.WriteLine( ex.ToString() );
                }

            } else {
                throw new System.Exception( "Connection is closed" );
            }

            return null;

        }


        public override bool Begin() {

            if ( this.mLogger != null ) Logger.LogTrace( $"{nameof( Begin )} {{0}}" , new { isConnected } );

            if ( isConnected ) {
                try {
                    this.LastError = null;
                    this.mTransaction = this.Connection.BeginTransaction();
                    return ( this.mTransaction != null );
                } catch ( System.Exception e ) {
                    if ( this.mLogger != null ) Logger.LogWarning( $"{nameof( Begin )} {{0}}" , e.Message );
                    new System.Exception( "Error in begin transaction : " + e.Message );
                }
            }

            return false;

        }

        public override bool Commit() {

            if ( this.mLogger != null ) Logger.LogTrace( $"{nameof( Commit )} {{0}}" , new { isConnected } );

            if ( ( isConnected ) && ( this.mTransaction != null ) ) {
                try {
                    this.LastError = null;
                    this.mTransaction.Commit();
                    //this.mCommand = null;
                    //this.mTransaction = null;
                    return true;
                } catch ( System.Exception e ) {
                    if ( this.mLogger != null ) Logger.LogWarning( $"{nameof( Commit )} {{0}}" , e.Message );
                    new System.Exception( "Error in begin transaction : " + e.Message );
                }
            }

            return false;
        }

        public override bool Rollback() {
            if ( this.mLogger != null ) Logger.LogTrace( $"{nameof( Rollback )} {{0}}" , new { isConnected } );

            if ( ( isConnected ) && ( this.mTransaction != null ) ) {
                try {
                    this.mTransaction.Rollback();
                    //this.mCommand = this.Connection.CreateCommand();
                    //this.mTransaction = null;
                    return true;
                } catch ( System.Exception e ) {
                    if ( this.mLogger != null ) Logger.LogWarning( $"{nameof( Rollback )} {{0}}" , e.Message );
                    new System.Exception( "Error in begin transaction : " + e.Message );
                }
            }

            return false;
        }

        public override bool Close() {


            if ( isConnected ) {

                if ( mTransaction != null ) {
                    mTransaction.Rollback();
                }
                if ( mCommand != null ) {
                    //mCommand.Close();
                }

                return base.Close();
            }

            return false;

        }


        public T? ExecuteScalar<T>( string aSQLQuery , params MySqlParameter[] args ) where T : notnull {

            T? result = default;

            if ( isConnected ) {
                this.LastError = null;

                try {

                    Command.CommandText = aSQLQuery;

                    if ( mTransaction != null )
                        this.mCommand.Transaction = mTransaction;

                    mCommand.Parameters.Clear();

                    if ( args.Length > 0 ) {
                        //    for ( int i = 0; i < args.Length; i++ ) {
                        mCommand.Parameters.AddRange( args );
                        //  }
                    }


#if DEBUG
                    Console.WriteLine( "[{0}]\r\n{1}\r\n{2}" , DateTime.Now.ToString( "G" ) , Command.CommandText , String.Join( "\r\n" , args.ToList().Select( a => $"{a.DbType} {a.ParameterName} = {a.Value}" ).ToArray() ) );
#endif

                    var value = Command.ExecuteScalar();
                    if ( value is not null ) result = value.ConvertTo<T>();
                    else return default( T );

                    //return new MySQResultSet( rs );

                } catch ( MySqlException ex ) {
                    Logger?.LogWarning( $"{nameof( ExecuteQueryAsync )} {{0}}" , ex.Message );
                    this.LastError = new LastErrorInfo( ex );
                }

            } else {
                throw new System.Exception( "Connection is closed" );
            }

            return result;

        }


        public async Task<T?> ExecuteScalarAsync<T>( string aSQLQuery , params MySqlParameter[] args ) where T : notnull {

            T? result = default;

            if ( isConnected ) {

                try {
                    this.LastError = null;

                    Command.CommandText = aSQLQuery;

                    if ( mTransaction != null )
                        this.mCommand.Transaction = mTransaction;

                    mCommand.Parameters.Clear();

                    if ( args.Length > 0 ) {
                        //    for ( int i = 0; i < args.Length; i++ ) {
                        mCommand.Parameters.AddRange( args );
                        //  }
                    }


#if DEBUG
                    Console.WriteLine( "[{0}]\r\n{1}\r\n{2}" , DateTime.Now.ToString( "G" ) , Command.CommandText , String.Join( "\r\n" , args.ToList().Select( a => $"{a.DbType} {a.ParameterName} = {a.Value}" ).ToArray() ) );
#endif

                    var id = await Command.ExecuteScalarAsync();
                    result = id.ConvertTo<T>();

                    //return new MySQResultSet( rs );

                } catch ( MySqlException ex ) {
                    Logger?.LogWarning( $"{nameof( ExecuteQueryAsync )} {{0}}" , ex.Message );
                    this.LastError = new LastErrorInfo( ex );
                }

            } else {
                throw new System.Exception( "Connection is closed" );
            }

            return result;

        }


    }

}