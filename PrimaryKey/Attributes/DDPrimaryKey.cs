using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.PrimaryKey.Attributes {
    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public sealed class DDPrimaryKey : System.Attribute {

        public bool Autoincrement;

        public DDPrimaryKey( bool Autoincrement = false ) {
            this.Autoincrement = Autoincrement;
        }
    }
}
