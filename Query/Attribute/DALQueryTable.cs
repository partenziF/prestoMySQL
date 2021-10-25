using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

    public enum ProjectionFieldsOption {
        All,
        Entity,
        Declared
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALQueryTable : System.Attribute {
        public string Table;
        public string Alias;

        public DALQueryTable( String Table , String Alias = "" ) {
            this.Table = Table;
            this.Alias = Alias;
        }
    }
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]       
    public class DALGroupBy : System.Attribute {
        public string Property { get; set; }
        public Type Table { get; set; }
        public string ID { get; set; }

        public bool FullGroupBy { get; set; }

    }

    public class DALOrderBy : System.Attribute {
        public string Property { get; set; }
        public Type Table { get; set; }
        public OrderType Order { get; set; }

        public string ID { get; set; }
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALProjectionFieldsOption : System.Attribute {
        public ProjectionFieldsOption Option { get; set; }

    }


}
