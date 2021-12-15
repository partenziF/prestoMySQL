using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Struct , AllowMultiple = false , Inherited = false )]
    public class SchemaBindTable : System.Attribute {
        public string Table;
    }
}
