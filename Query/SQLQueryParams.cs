using prestoMySQL.Query.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace prestoMySQL.Query {

    public class SQLQueryParams : IQueryParams {

        private QueryParam[] values;

        public QueryParam this[int index] {
            get => ( QueryParam ) getParamValue( index );
            set => setParam( index , (QueryParam) value );
        }

        public SQLQueryParams() {
            values = null;
        }

        public SQLQueryParams( int capacity ) {
            setCapacity( capacity );
        }

        public SQLQueryParams( params QueryParam[] values ) {

            setCapacity( values.Length );
            Array.Copy( values,this.values,values.Length );
        }

        public void setCapacity( int capacity ) {
            values ??= new QueryParam[capacity];
        }

        public void setCapacity() {
            values = null;
        }

        public QueryParam[] asArray() {
            return values;
        }

        public string[] asStrings( string aPlaceholder = "" ) {

            return ( (IEnumerable) values ) .Cast<QueryParam>()
                                 .Select( x => x.AsQueryParam( aPlaceholder ) )
                                 .ToArray();
        }

        public void clear() {
            GC.Collect();
            values = null;
        }


        public void setParamRange( QueryParam[] array ) {
            values = (QueryParam[]) array.Clone();
        }

        public void setParam( int index , QueryParam value ) {
            if ( ( index >= 0 ) && ( index < values.Length ) ) {
                values[index] = value;
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        public IEnumerator GetEnumerator() {
            return values.GetEnumerator();
        }


        public IQueryParam getParamValue( int index ) {
            if ( ( index >= 0 ) && ( index < values.Length ) ) {
                return values[index];
            } else {
                throw new IndexOutOfRangeException();
            }
        }

        //public static explicit operator MySQLQueryParams( QueryParam[] array ) {

        //    var r = new MySQLQueryParams();
        //    r.values = array;
        //    return r;


        //}

    }

}