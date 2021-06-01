using prestoMySQL.Column.DataType;
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

    public class DALProjectionColumn : System.Attribute {

        public String Name { get; set; }

        public String Table { get; set; }//default "";
        public AbstractEntity Entity { get; set; }// () default void.class;
        public String Alias { get; set; }// () default ""; 
        public MySQLDataType DataType { get; set; }
        public int OrderBy { get; set; }// () default  0;
        public OrderType Sort { get; set; }//() default OrderType.DESC; 
        public int GroupBy { get; set; }
        //() default  0;

    }


    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public  class DALNumericProjectionColumnAttribute : DALProjectionColumn {

        public byte Size { get; set; }
        public SignType Signed { get; set; }


    }


    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public sealed class DALStringProjectionColumnAttribute : DALProjectionColumn {        

        public byte Length { get; set; }
        public string Charset { get; set; }



    }
    
    public sealed class DALFloatingPointProjectionColumnAttribute : DALProjectionColumn {

        public byte Size { get; set; }
        public byte Precision { get; set; }
    }

    public sealed class DALProjectionColumnBooleanAttribute : DALNumericProjectionColumnAttribute{

        public DALProjectionColumnBooleanAttribute() {
            DataType = MySQLDataType.dbtTinyInt;
            Size = 1;
            Signed = SignType.Unsigned;
        }
    }


}

