using prestoMySQL.Query.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query {
    class SQLGenericsQueryParams : IQueryParams {

        private IQueryParam[] values;

        public QueryParam this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public QueryParam[] asArray() {
            throw new NotImplementedException();
        }

        public string[] asStrings( string aPlaceholder = "" ) {
            throw new NotImplementedException();
        }

        public void clear() {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }

        public IQueryParam getParamValue( int index ) {
            throw new NotImplementedException();
        }

        public void setCapacity( int capacity ) {
            throw new NotImplementedException();
        }

        public void setParam( int index , QueryParam value ) {
            throw new NotImplementedException();
        }

        public void setParamRange( QueryParam[] array ) {
            throw new NotImplementedException();
        }
    }
}
