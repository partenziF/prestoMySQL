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

        internal string mName;
        public string Name { get => mName; }

        public string AsQueryParam( string aPlaceholder = "" ) {
            
            if ( ( mValue!=null) && ( mValue.GetType().IsArray ) ) {
            
                var l = ( ( Array ) mValue ).Length;
                string[] result = new string[l];
                for (int i = 0; i<l; i++ ) {

                    if ( aPlaceholder == "@" ) {
                        result[i] =  String.Concat( "@" , String.Concat( Name , "_" , i.ToString() ) );
                    } else if ( !String.IsNullOrWhiteSpace( aPlaceholder ) ) {
                        result[i] = String.Format( aPlaceholder );
                    } else {
                        result[i] = ToString();
                    }

                }

                return string.Join( "," , result );


            } else {
                if ( aPlaceholder == "@" ) {
                    return String.Concat( "@" , Name );
                } else if ( !String.IsNullOrWhiteSpace( aPlaceholder ) ) {
                    return String.Format( aPlaceholder );
                } else {
                    return ToString();
                }
            }

        }

        internal void rename( string name ) {
            this.mName = name;
        }
        protected abstract object GetValue();

        public static explicit operator MySqlParameter( QueryParam v ) {
            return new MySqlParameter( v.Name , v.Value );
        }

    }

}
