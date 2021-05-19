using prestoMySQL.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
    public abstract class GenericsQueryParam<T> : IQueryParam where T : notnull {

        protected T mValue;
        public object Value { get => GetValue(); }

        private string mName;

        protected GenericsQueryParam( T mValue , string mName ) {
            this.mValue = mValue;
            this.mName = mName;
        }

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

        protected abstract object GetValue();

    }
}
