using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Database.Interface {
    public interface IReadableResultSet {


        public object this[string name] { get; }
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

        public U getValueAs<U>( int Index );

        public Object getObject( int i );
        bool isEmpty();

        Dictionary<string , Dictionary<string , int>> ResultSetSchemaTable();
        }
}