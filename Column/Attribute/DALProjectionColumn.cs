using prestoMySQL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Attribute {

    public enum OrderType {
        ASC, DESC
    }

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]

    public sealed class DALProjectionColumn : System.Attribute {

        public String Name { get; set; }

        public String Table { get; set; }//default "";
        public AbstractEntity Entity { get; set; }// () default void.class;
        public String Alias { get; set; }// () default ""; 
        public SQLiteDataType Type { get; set; }
        public int OrderBy { get; set; }// () default  0;
        public OrderType Sort { get; set; }//() default OrderType.DESC; 
        public int GroupBy { get; set; }
        //() default  0;

    }
}
