using prestoMySQL.Column;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
    public abstract class QueryParam<T> : IQueryParam where T : notnull {

        public QueryParam( object aValue , string aName ) {
            mValue = aValue;
            mName = aName;
        }

        protected T mValue;
        public object Value { get => GetValue(); }

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

        public abstract object GetValue();
    }

}
