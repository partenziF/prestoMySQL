using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {

    public interface IQueryParams : IEnumerable {
        QueryParam this[int index] { get; set; }

        public void setParam( int index , QueryParam value );

        public IQueryParam getParamValue( int index );

        public void clear();

        public void setCapacity( int capacity );

        public string[] asStrings( string aPlaceholder = "" );

        public QueryParam[] asArray();

        public void setParamRange( QueryParam[] array );


    }
}
