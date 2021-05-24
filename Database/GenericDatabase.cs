using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PrestoMySQL.Database.Interface;

namespace PrestoMySQ.Database {

    public record LastErrorInfo( Exception e ) : ILastErrorInfo {

        public string Message { get; init; } = e.Message;

        public override string ToString() {
            return  Message;
        }
    }


    public abstract class GenericDatabase : IDatabase {
        protected readonly string mConnectionString;
        protected readonly string mUsername;
        protected readonly string mPassword;
        protected readonly string mHost;
        protected readonly uint mPort;
        protected readonly string mDatabase;

        protected ILogger mLogger;
        public ILogger Logger { get => mLogger; set => mLogger = value; }

        protected bool mIsConnected = false;

        protected virtual bool getIsConnected() {
            return this.mIsConnected;
        }
        public bool isConnected { get { return getIsConnected(); } }


        private ILastErrorInfo mLastErrorInfo;
        public virtual ILastErrorInfo LastError { get => this.mLastErrorInfo; set => this.mLastErrorInfo =  value ; }


        private bool mIsAutoCommit;
        public bool IsAutoCommit { get => this.mIsAutoCommit; set => this.mIsAutoCommit =  value ; }


        public GenericDatabase( string aConnectionString , ILogger aLogger = null ) {
            this.mLogger = aLogger;
            this.mConnectionString = aConnectionString;
            Logger?.LogTrace( $"{nameof( GenericDatabase )} {{0}}" , new { aConnectionString } );
        }

        public GenericDatabase( string aUsername , string aPassword , string aHost , string aDatabase , uint aPort = 3306 , ILogger aLogger = null ) {

            this.mLogger = aLogger;

            this.mUsername = aUsername;
            this.mPassword = aPassword;
            this.mHost = aHost;
            this.mDatabase = aDatabase;
            this.mPort = aPort;

            Logger?.LogTrace( $"{nameof( GenericDatabase )} {{0}}" , new { aUsername , aPassword , aHost , aDatabase , aPort } );

        }

        protected abstract bool DoOpenConnection();
        protected abstract bool DoCloseConnection();

        public bool OpenConnection() {

            Logger?.LogTrace( $"{nameof( OpenConnection )} {{0}}" , new { isConnected } );

            if ( isConnected ) {

                try {
                    if ( DoCloseConnection() ) {
                        mIsConnected = false;
                    } else {
                        throw new Exception( "Error while try close connection" );
                    }
                } catch ( Exception e ) {

                    LastError = new LastErrorInfo( e );

                    Logger?.LogWarning( $"{nameof( OpenConnection )} Exception message : {{0}}" , e.Message );

                    throw new Exception( "Error while try close connection" , e );
                }
            }

            if ( !isConnected ) {

                try {

                    if ( DoOpenConnection() ) {
                        mIsConnected = true;
                    } else {
                        throw new Exception( "Error while try to open connection " );
                    }

                } catch ( Exception e ) {
                    Logger?.LogWarning( $"{nameof( OpenConnection )} Exception message : {{0}}" , e.Message );
                    throw new Exception( "Error while try to open connection" , e );
                }

            }

            Logger?.LogTrace( $"{nameof( OpenConnection )} result {{0}}" , new { isConnected } );

            return mIsConnected;
        }

        public virtual bool Close() {
            if (isConnected) {
                DoCloseConnection();
                return true;
            }

            return false;

        }

        public abstract bool Begin();
        public abstract bool Commit();
        public abstract bool Rollback();

        protected abstract Task<bool> DoOpenConnectionAsync();
    }
}