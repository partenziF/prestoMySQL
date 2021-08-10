using prestoMySQL.Column.DataType;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.Attribute {

    public enum DefaultValues {
        NULL,
        AUTOINCREMENT,
        CURDATE
    }

    public static class Extenders {
        public static string ToText( this SignType o ) {
            switch ( o ) {
                case SignType.Unsigned:
                return "UNSIGNED";
                case SignType.Signed:
                return "";
            }
            // other ones, just use the base method
            return o.ToString();
        }
        public static string ToText( this NullValue o ) {
            switch ( o ) {
                case NullValue.NotNull:
                return "NOT NULL";
                case NullValue.Null:
                return "NULL";
            }
            // other ones, just use the base method
            return o.ToString();
        }
    }

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
        public string Alias { get; set; }

        public MySQLDataType DataType { get; set; }
        public NullValue NullValue { get; set; }
        public object DefaultValue { get; set; }


        public DDColumnAttribute( string Name , MySQLDataType DataType , NullValue NullValue = NullValue.Null ) {
            this.Name = Name;
            this.DataType = DataType;
            this.NullValue = NullValue;

        }

        public string GetDefaultValueClause() {
            
            string result = "";

            if ( DefaultValue != default( object ) ) {

                switch ( this.NullValue ) {

                    case NullValue.NotNull:

                    if ( this.DefaultValue.GetType() == typeof( DefaultValues ) ) {

                        switch ( ( DefaultValues ) this.DefaultValue ) {

                            case DefaultValues.NULL:
                            throw new ArgumentException( "Invalid default value." );

                            case DefaultValues.AUTOINCREMENT:
                            result =  " AUTO_INCREMENT " ;
                            break;

                            case DefaultValues.CURDATE:
                            result = " DEFAULT CURDATE() ";
                            break;
                        }

                    } else if ( this.DefaultValue is null ) {
                        throw new ArgumentException( "Invalid default value." );
                    } else if ( this.DefaultValue.GetType() == typeof( string ) ) {
                        result = $" DEFAULT '{this.DefaultValue.ToString().Replace( "'" , "\\'" )}' ";
                    } else {
                        result = $" DEFAULT '{this.DefaultValue}' ";
                    }

                    break;


                    case NullValue.Null:

                    if ( this.DefaultValue.GetType() == typeof( DefaultValues ) ) {

                        switch ( ( DefaultValues ) this.DefaultValue ) {

                            case DefaultValues.NULL:
                            result = " DEFAULT NULL ";
                            break;

                            case DefaultValues.AUTOINCREMENT:
                            result = " AUTO_INCREMENT ";
                            break;

                            case DefaultValues.CURDATE:
                            result = " DEFAULT CURDATE() ";
                            break;
                        }

                    } else if ( this.DefaultValue is null ) {
                        result = " DEFAULT NULL ";
                    } else if ( this.DefaultValue.GetType() == typeof( string ) ) {
                        result = $" DEFAULT '{this.DefaultValue.ToString().Replace( "'" , "\\'" )}' ";
                    } else {
                        result = $" DEFAULT '{this.DefaultValue}' ";
                    }

                    break;

                }

            }
            
            return result;
        }

        public string BuildColumnName() {
            string columnName = ( !string.IsNullOrWhiteSpace( Name ) ) ? Name : throw new System.Exception( "Column name is empty or null" );
            return $" {SQL.SQLConstant.COLUMN_NAME_QUALIFIER}{columnName}{SQL.SQLConstant.COLUMN_NAME_QUALIFIER} ";
        }


        public string ChangeColumn(string oldColumnName ) {
            
            var sb = new StringBuilder( $"ALTER TABLE CHANGE COLUMN {SQL.SQLConstant.COLUMN_NAME_QUALIFIER}{oldColumnName}{SQL.SQLConstant.COLUMN_NAME_QUALIFIER} " );

            sb.AppendLine( this.ToString() ); 

            return sb.ToString();
            //                               ALTER TABLE `qualifiche`
            //CHANGE COLUMN `Qualifica` `Descrizione` VARCHAR( 250 ) NOT NULL COLLATE 'latin1_swedish_ci' AFTER `PkQualifica`,
            //DROP INDEX `UIQualifica`,
            //ADD UNIQUE INDEX `UIQualifica` (`Descrizione`) USING BTREE;

            //                       ALTER TABLE `qualifiche`
            //CHANGE COLUMN `Created` `Created1` DATETIME NOT NULL AFTER `Qualifica`;


        }

        protected StringBuilder BuildString() {
                        
            StringBuilder sb = new StringBuilder( this.BuildColumnName() );

            sb.Append( BulldTypeString()  );

            return sb;
        }

        public virtual string BulldTypeString() {
            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            return $" {dataType.ToString()}";
        }

        public override string ToString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );

            StringBuilder sb = new StringBuilder( this.BuildColumnName() );

            sb.Append( $" {dataType.ToString()} " );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );


            return sb.ToString();
        }
    }

    public sealed class DDColumnNumericAttribute : DDColumnAttribute {


        public DDColumnNumericAttribute( string aName , MySQLDataType aDataType , byte Size , SignType Signed , NullValue NullValue = NullValue.Null ) : base( aName , aDataType , NullValue ) {
            this.Size = Size;
            this.Signed = Signed;
        }

        public byte Size { get; set; }
        public SignType Signed { get; set; }

        public override string BulldTypeString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            StringBuilder sb = new StringBuilder( $" {dataType.ToString()}" );
            if ( this.Size > 0 ) sb.Append( $"({Size}) " );
            if ( !String.IsNullOrWhiteSpace( this.Signed.ToText() ) ) sb.Append( $" {this.Signed.ToText()} " );
            return sb.ToString();

        }
        public override string ToString() {

            StringBuilder sb = base.BuildString();

            //sb.Append( BulldTypeString() );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );

            return sb.ToString();
        }

    }

    public sealed class DDColumnBooleanAttribute : DDColumnAttribute {
        public DDColumnBooleanAttribute( string aName , NullValue NullValue = NullValue.Null ) : base( aName , MySQLDataType.dbtTinyInt , NullValue ) {
            this.Size = 1;
            this.Signed = SignType.Unsigned;
        }

        public byte Size { get; set; }
        public SignType Signed { get; set; }

        public override string BulldTypeString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            StringBuilder sb = new StringBuilder( $" {dataType.ToString()}" );
            sb.Append( $"({Size}) " );
            return sb.ToString();

        }
        public override string ToString() {

            StringBuilder sb = base.BuildString();

            //sb.Append( BulldTypeString() );

            if ( !String.IsNullOrWhiteSpace( this.Signed.ToText() ) ) sb.Append( $" {this.Signed.ToText()} " );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );

            return sb.ToString();
        }

    }

    public sealed class DDColumnStringAttribute : DDColumnAttribute {
        //Length is required
        public UInt16 Length { get; set; }
        public string Charset { get; set; }

        public DDColumnStringAttribute( string aName , MySQLDataType aDataType , UInt16 aLength = 250 , string aCharset = "" , NullValue NullValue = NullValue.Null ) : base( aName , aDataType , NullValue ) {
            this.Length = aLength;
            this.Charset = aCharset;
        }
        public override string BulldTypeString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            StringBuilder sb = new StringBuilder( $" {dataType.ToString()}" );
            sb.Append( $"({Length}) " );
            return sb.ToString();

        }
        public override string ToString() {

            StringBuilder sb = base.BuildString();

            //sb.Append( BulldTypeString() );

            if (!String.IsNullOrWhiteSpace(this.Charset)) sb.Append( $"COLLATE '{this.Charset}' " );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );

            return sb.ToString();
        }

    }

    public sealed class DDColumnFloatingPointAttribute : DDColumnAttribute {

        public byte Size { get; set; }
        public byte Precision { get; set; }

        public DDColumnFloatingPointAttribute( string aName , MySQLDataType aDataType , byte aSize , byte aPrecision , NullValue NullValue = NullValue.Null ) : base( aName , aDataType , NullValue ) {
            this.Size = aSize;
            this.Precision = aPrecision;
        }
        public override string BulldTypeString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            StringBuilder sb = new StringBuilder( $" {dataType.ToString()}" );
            if ( this.Size > 0 ) sb.Append( $"({Size}" );
            if ( this.Precision > 0 ) sb.Append( $",{Precision}" );
            sb.Append( ")" );
            return sb.ToString();

        }
        public override string ToString() {

            StringBuilder sb = base.BuildString();

            //sb.Append( BulldTypeString() );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );

            return sb.ToString();

        }


    }


    public sealed class DDColumnFixedPointAttribute : DDColumnAttribute {

        public byte Size { get; set; }
        public byte Precision { get; set; }

        public DDColumnFixedPointAttribute( string aName , MySQLDataType aDataType , byte aSize , byte aPrecision , NullValue NullValue = NullValue.Null ) : base( aName , aDataType , NullValue ) {
            this.Size = aSize;
            this.Precision = aPrecision;
        }
        public override string BulldTypeString() {

            MySQLColumnDataType dataType = new MySQLColumnDataType( ( MySQLDataType ) this.DataType );
            StringBuilder sb = new StringBuilder( $" {dataType.ToString()}" );
            if ( this.Size > 0 ) sb.Append( $"({Size}" );
            if ( this.Precision > 0 ) sb.Append( $",{Precision}" );
            sb.Append( ")" );
            return sb.ToString();

        }
        public override string ToString() {

            StringBuilder sb = base.BuildString();

            //sb.Append( BulldTypeString() );

            sb.Append( $" {this.NullValue.ToText()} " );

            sb.Append( GetDefaultValueClause() );

            return sb.ToString();

        }


    }



}
