using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Attribute {

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    class DDIndexAttribute : System.Attribute {

        public bool Unique { get; set; }

        public DDIndexAttribute( bool unique ) {
            this.Unique = unique;
        }
    }
}
