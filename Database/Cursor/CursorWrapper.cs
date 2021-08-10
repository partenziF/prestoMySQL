using MySqlConnector;
using prestoMySQL.Database.Interface;
using prestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


//IReadableResultSet<MySqlDataReader>
//MySQResultSet
namespace prestoMySQL.Database.Cursor {
    public class CursorWrapper<T, U> : ICursorWrapper, IEnumerator<T>, IEnumerable where T : ReadableResultSet<U> where U : DbDataReader {

        private ILastErrorInfo mLastErrorInfo;
        public virtual ILastErrorInfo LastError { get => this.mLastErrorInfo; set => this.mLastErrorInfo = value; }


        public T mResultSet;

        public CursorWrapper( T aResultSet, ILastErrorInfo lastErrorInfo = null ) {

            mLastErrorInfo = lastErrorInfo;
            this.mResultSet = aResultSet;

        }

        public T Current => mResultSet;

        object IEnumerator.Current => mResultSet;

        public void Dispose() {
            Close();
        }

        public IEnumerator GetEnumerator() {
            return ( IEnumerator ) this;
        }

        public bool MoveNext() {
            return ( bool ) ( this.mResultSet?.fetch() );
        }

        public void Reset() {
            this.mResultSet.close();
        }

        public bool isEmpty() {
            return ( bool ) ( mResultSet?.isEmpty() );
        }

        public void Close() {
            mResultSet?.close();
        }

    }


    public class CursorWrapperAsync<T, U> : ICursorWrapper, IAsyncEnumerator<T>, IAsyncEnumerable<T> where T : ReadableResultSet<U> where U : DbDataReader {

        public T mResultSet;

        public CursorWrapperAsync( T aResultSet ) {
            mResultSet = aResultSet;
        }

        public T Current => mResultSet;


        public void Close() {
            mResultSet?.close();
        }

        public ValueTask DisposeAsync() {
            Close();
            return new ValueTask();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken cancellationToken = default ) {
            return ( IAsyncEnumerator<T> ) this;
        }

        public bool isEmpty() {
            return ( bool ) ( mResultSet?.isEmpty() );
        }

        public ValueTask<bool> MoveNextAsync() {
            //.ConfigureAwait(false)
            //Task.Delay();
            var b = this.mResultSet?.fetchAsync();
            return new ValueTask<bool>( b );
        }
    }
}

