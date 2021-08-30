using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    public class DALFunctionParam : System.Attribute {
    }
    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DALValueFunctionParam : DALFunctionParam {
        internal string Value;

        public DALValueFunctionParam( String Value = "" ) {
            this.Value = Value;
        }
    }

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DALColumnFunctionParam : DALFunctionParam {
        public string Property { get; set; }
        public Type Table { get; set; }


        public DALColumnFunctionParam(  ) {
        }
    }
}
