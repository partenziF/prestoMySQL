using prestoMySQL.Column.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {

    public class SQLQueryOrderBy {
        public int order;
        internal string column;
        internal OrderType sort;

        public SQLQueryOrderBy( int order , string column , OrderType sort ) : base() {
            this.order = order;
            this.column = column;
            this.sort = sort;
        }

        public override string ToString() {

            if ( this.sort == OrderType.ASC ) {
                return string.Format( "{0} {1}" , column , "ASC" );
            } else if ( this.sort == OrderType.DESC ) {
                return string.Format( "{0} {1}" , column , "DESC" );
            } else {
                return "";
            }
        }

    }


}
