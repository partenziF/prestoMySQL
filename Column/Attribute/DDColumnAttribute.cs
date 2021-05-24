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

    public enum NullValue {
        NotNull,
        Null
    }

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DDColumnAttribute : System.Attribute {

        public string Name { get; set; }
        public MySQLDataType DataType { get; set; }

        public bool Unique { get; set; }
        public NullValue NullValue { get; set; }
        public object DefaultValue { get; set; }


        public DDColumnAttribute( string Name , MySQLDataType DataType , bool Unique = false , NullValue NullValue = NullValue.Null , object DefaultValue = null ) {
            this.Name = Name;
            this.DataType = DataType;
            this.Unique = Unique;
            this.NullValue = NullValue;
            this.DefaultValue = DefaultValue;
        }

    }

    public sealed class DDColumnNumericAttribute : DDColumnAttribute {
        public DDColumnNumericAttribute( string aName , MySQLDataType aDataType , byte Size , SignType Signed , bool Unique = false , NullValue NullValue = NullValue.Null , object DefaultValue = null ) : base( aName , aDataType , Unique ,  NullValue , DefaultValue ) {
            this.Size = Size;
            this.Signed = Signed;
        }

        public byte Size { get; set; }
        public SignType Signed { get; set; }
    }

    public sealed class DDColumnBooleanAttribute : DDColumnAttribute {
        public DDColumnBooleanAttribute( string aName , bool Unique = false , NullValue NullValue = NullValue.Null , object DefaultValue = null ) : base( aName , MySQLDataType.dbtTinyInt ,  Unique , NullValue , DefaultValue ) {
            this.Size = 1;
            this.Signed = SignType.Unsigned;
        }

        public byte Size { get; set; }
        public SignType Signed { get; set; }
    }


    public sealed class DDColumnStringAttribute : DDColumnAttribute {

        public byte Length { get; set; }
        public string Charset { get; set; }

        public DDColumnStringAttribute( string aName , MySQLDataType aDataType , byte aLength , string aCharset = "" , bool Unique = false , NullValue NullValue = NullValue.Null , object DefaultValue = null ) : base( aName , aDataType ) {
            this.Length = aLength;
            this.Charset = aCharset;
        }
    }

    public sealed class DDColumnFloatingPointAttribute : DDColumnAttribute {

        public byte Size { get; set; }
        public byte Precision { get; set; }

        public DDColumnFloatingPointAttribute( string aName , MySQLDataType aDataType , byte aSize , byte aPrecision , bool Unique = false , NullValue NullValue = NullValue.Null , object DefaultValue = null ) : base( aName , aDataType ) {
            this.Size = aSize;
            this.Precision = aPrecision;
        }
    }


}
