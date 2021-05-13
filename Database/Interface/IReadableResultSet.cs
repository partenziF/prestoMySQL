using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestoMySQL.Database.Interface {
    public interface IReadableResultSet<T> where T : DbDataReader {

        public Task<bool> fetchAsync();
        public bool fetch();

        public int? columnCount();

        public void close();

        public int getInt( int i );

        public String getString( int i );

        public long getLong( int i );

        public double getDouble( int i );

        public float getFloat( int i );

        public U getValueAs<U>( String aColumnName );


        public Object getObject( int i );
        bool isEmpty();
    }
}