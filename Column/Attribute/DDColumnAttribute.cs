using prestoMySQL.Column.DataType;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Attribute {


    public enum SignType {
        Signed,
        Unsigned
    }

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DDColumnAttribute : System.Attribute {

        public string Name { get; set; }
        public MySQLDataType DataType { get; set; }

        public bool Unique { get; set; }
        public bool NotNull { get; set; }
        public object DefaultValue { get; set; }


        public DDColumnAttribute( string Name , MySQLDataType DataType , bool Unique = false , bool NotNull = false , object DefaultValue = null ) {
            this.Name = Name;
            this.DataType = DataType;
            this.Unique = Unique;
            this.NotNull = NotNull;
            this.DefaultValue = DefaultValue;
        }

    }

    public sealed class DDColumnNumericAttribute : DDColumnAttribute {
        public DDColumnNumericAttribute( string aName , MySQLDataType aDataType , byte Size , SignType Signed , bool Unique = false , bool NotNull = false , object DefaultValue = null ) : base( aName , aDataType , Unique , NotNull , DefaultValue ) {
            this.Size = Size;
            this.Signed = Signed;
        }

        public byte Size { get; set; }
        public SignType Signed { get; set; }
    }

    public sealed class DDColumnStringAttribute : DDColumnAttribute {

        public byte Length { get; set; }
        public string Charset { get; set; }

        public DDColumnStringAttribute( string aName , MySQLDataType aDataType , byte aLength , string aCharset = "" , bool Unique = false , bool NotNull = false , object DefaultValue = null ) : base( aName , aDataType ) {
            this.Length = aLength;
            this.Charset = aCharset;
        }
    }

    public sealed class DDColumnFloatingPointAttribute : DDColumnAttribute {

        public byte Size { get; set; }
        public byte Precision { get; set; }

        public DDColumnFloatingPointAttribute( string aName , MySQLDataType aDataType , byte aSize , byte aPrecision , bool Unique = false , bool NotNull = false , object DefaultValue = null ) : base( aName , aDataType ) {
            this.Size = aSize;
            this.Precision = aPrecision;
        }
    }


}
