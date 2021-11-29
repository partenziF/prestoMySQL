using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DALSchemaBind : System.Attribute {
        public string Table;
    }
}
