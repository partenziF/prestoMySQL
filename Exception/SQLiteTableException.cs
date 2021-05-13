using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Exception {
    public class SQLiteTableException : System.Exception {

        public SQLiteTableException( ) { }
        
        public SQLiteTableException( string message ) : base( message ) {
        }

        public SQLiteTableException( string message , System.Exception innerException ) : base( message , innerException ) {
        }

        protected SQLiteTableException( SerializationInfo info , StreamingContext context ) : base( info , context ) {
        }
    }
}
