using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Exception {
    public class ColumnDataTypeException : System.Exception {
        public ColumnDataTypeException() {
        }

        public ColumnDataTypeException( string message ) : base( message ) {
        }

        public ColumnDataTypeException( string message , System.Exception innerException ) : base( message , innerException ) {
        }

        protected ColumnDataTypeException( SerializationInfo info , StreamingContext context ) : base( info , context ) {
        }
    }
}
