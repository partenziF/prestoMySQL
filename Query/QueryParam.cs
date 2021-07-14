using MySqlConnector;
using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
    public abstract class QueryParam : IQueryParam {

        public QueryParam( object aValue , string aName ) {
            mValue = aValue;
            mName = aName;
        }

        protected object mValue;
        public object Value { get => GetValue(); set => mValue = value; }

        private string mName;
        public string Name { get => mName; }

        public string AsQueryParam( string aPlaceholder = "" ) {
            if ( aPlaceholder == "@" ) {
                return String.Concat( "@" , Name );
            } else if ( !String.IsNullOrWhiteSpace( aPlaceholder ) ) {
                return String.Format( aPlaceholder );
            } else {
                return ToString();
            }

        }

        internal void rename(string name ) {
            this.mName = name;
        }
        protected abstract object GetValue();

        public static explicit operator MySqlParameter( QueryParam v ) {
            return new MySqlParameter( v.Name , v.Value );
        }
    }

}
