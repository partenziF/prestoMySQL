using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALQueryEntity : System.Attribute {
        public Type value;
        public string Alias;


        //public DALQueryEntity( Type value , String Alias = "" ) {
        //    this.value = value;
        //    this.Alias = Alias;
        //}
    }

}
