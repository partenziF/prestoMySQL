using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALJoinTable : System.Attribute {
        public string Table;
        public string Alias;

        public DALJoinTable( String Table, String Alias = "")
        {
            this.Table = Table;
            this.Alias = Alias;
        }
}
}
