using prestoMySQL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey.Attributes {

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = true , Inherited = false )]
    public sealed class DDForeignKey : System.Attribute {

        public Type TableReferences { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }

        public string TableAlias { get; set; }

        public override string ToString() {
            return base.ToString();
        }
    }
}
