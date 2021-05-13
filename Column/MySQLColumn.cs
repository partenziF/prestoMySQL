using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace prestoMySQL.Column {
    public class MySQLColumn<T> : GenericColumn<T>, IConvertible where T : ISQLTypeWrapper { //where T : struct        

        public MySQLColumn( string aDeclaredVariableName , PropertyInfo aMethodBase = null ) : base( aMethodBase ) {

            if ( string.IsNullOrEmpty( aDeclaredVariableName ) ) {
                throw new ArgumentException( $"'{nameof( aDeclaredVariableName )}' non può essere null o vuoto." , nameof( aDeclaredVariableName ) );
            }

            mDeclaredVariableName = aDeclaredVariableName;

            DDColumnAttribute columnAttribute = null;

            if ( System.Attribute.IsDefined( this.mPropertyInfo , typeof( DDColumnNumericAttribute ) ) ) {
                columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnNumericAttribute>( false );
                this.mSize = ( ( DDColumnNumericAttribute ) columnAttribute ).Size;
                this.mSigned = ( ( DDColumnNumericAttribute ) columnAttribute ).Signed;
            } else if ( System.Attribute.IsDefined( this.mPropertyInfo , typeof( DDColumnStringAttribute ) ) ) {
                columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnStringAttribute>( false );
            } else if ( System.Attribute.IsDefined( this.mPropertyInfo , typeof( DDColumnFloatingPointAttribute ) ) ) {
                columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnFloatingPointAttribute>( false );
            } else if ( System.Attribute.IsDefined( this.mPropertyInfo , typeof( DDColumnAttribute ) ) ) {
                columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnAttribute>( false );
            } else {
                throw new ArgumentNullException();
            }

            //DDColumnAttribute columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnAttribute>( false );
            this.mDataType = ( MySQLDataType ) ( columnAttribute as DDColumnAttribute ).DataType;

        }

        // Data Type used in database read from attribute
        private readonly MySQLDataType mDataType;
        //private readonly MySqlDbType mDataType;

        public MySQLDataType DataType { get => this.mDataType; }

        private readonly string mDeclaredVariableName;

        private readonly byte mSize;
        public byte Size { get => mSize; }

        private readonly SignType mSigned;
        public SignType Signed { get => mSigned; }



        public override object ValueAsParamType() {

            return MySQLUtils.convertFromMySQLDataTypeToObject<T>( Value.ToObject() , DataType );

            //object Params = null;

            //switch ( DataType ) {
            //    case MySQLDataType.dbtTinyInt: return Convert.ToByte( this );
            //    case MySQLDataType.dbtInteger: return Convert.ToInt32( this );
            //}

            //switch ( DataType ) {
            //    //Integer Types (Exact Value)
            //    case MySQLDataType.dbtTinyInt:
            //    case MySQLDataType.dbtSmallInt:
            //    case MySQLDataType.dbtMediumInt:
            //    case MySQLDataType.dbtInteger:
            //    case MySQLDataType.dbtBigInt:
            //    if ( Value is null ) {
            //        return null;// SQLTypeWrapper<int>.Null;
            //    } else if ( Value.IsInteger() ) {
            //        Params = Convert.ToInt32( Value );
            //    } else if ( Value.IsFloatingPoint() ) {
            //        Params = Convert.ChangeType( Value , typeof( int ) );
            //    } else if ( Value.IsLitteral() ) {
            //        if ( !int.TryParse( Value.ToString() , out int r ) ) {
            //            throw new InvalidCastException( "Unable to convert " + Value.ToString() + " to " + DataType.ToString() );
            //        } else {
            //            Params = r;
            //        }
            //    } else if ( Value.IsDateTime() ) {
            //        Params = new DateTimeOffset( ( (DateTime) Convert.ChangeType( Value , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
            //    } else if ( Value.IsBoolean() ) {
            //        Params = ( (bool) Convert.ChangeType( Value , typeof( bool ) ) ) ? 1 : 0;
            //    } else {
            //        throw new InvalidCastException( "Unknow type for value " + Value.ToString() );
            //    }
            //    break;

            //}

            //return Params;
        }

        TypeCode IConvertible.GetTypeCode() {
            return Type.GetTypeCode( Value.GetType() );

        }

        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar( IFormatProvider provider ) {

            TypeCode code;
            if ( Value is null ) {
                return isNotNull ? default( char ) : throw new ArgumentNullException();
            } else if ( Value.IsInteger( out code ) ) {
                if ( code == TypeCode.Byte ) {
                    return Convert.ToChar( Convert.ChangeType( Value , typeof( Byte ) ) );
                } else {
                    throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }
                //return ( ( long ) Convert.ChangeType( Value , typeof( long ) ) ).ToString();
            } else if ( Value.IsFloatingPoint( out code ) ) {
                throw new InvalidCastException( "Unable to convert " + code + " to char" );
                //return ( ( decimal ) Convert.ChangeType( Value , typeof( decimal ) ) ).ToString();
            } else if ( Value.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char:
                    return ( char ) Convert.ChangeType( Value , typeof( char ) );
                    default:
                    if ( Value.ToString().Length == 1 ) {
                        return ( char ) Value.ToString()[0];
                    } else
                        throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }

            } else if ( Value.IsDateTime() ) {

                throw new InvalidCastException( "Unable to convert DateTime to char" );

                //return ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( Value.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( Value , typeof( bool ) ) ) ? 'Y' : 'N';
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {

            TypeCode code;
            if ( Value is null ) {
                return isNotNull ? default( DateTime ) : throw new ArgumentNullException();
            } else if ( Value.IsInteger( out code ) ) {
                return DateTimeOffset.FromUnixTimeSeconds( ( long ) Convert.ChangeType( Value , typeof( long ) ) ).DateTime;
            } else if ( Value.IsFloatingPoint( out code ) ) {
                DateTime a = DateTime.FromOADate( ( double ) Convert.ChangeType( Value , typeof( double ) ) );
                return new DateTime( a.Year , a.Month , a.Day , a.Hour , a.Minute , a.Second );
            } else if ( Value.IsLitteral( out code ) ) {
                switch ( code ) {

                    case TypeCode.String: {
                        return DateTime.ParseExact( Value.ToString() , MySQLQueryParam.MYSQL_DATE_FORMAT , CultureInfo.InvariantCulture );
                    }

                    default: {
                        throw new InvalidCastException( "unable to convert char to datetime" );
                    }
                }
            } else if ( Value.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) );
            } else if ( Value.IsBoolean() ) {
                throw new InvalidCastException( "unable to convert boolean to datetime" );
            } else {
                throw new InvalidCastException( "Uknow type" );
            }

        }

        decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            TypeCode code;
            if ( Value is null ) {
                return isNotNull ? default( Decimal ) : throw new ArgumentNullException();
            } else if ( Value.IsInteger( out code ) ) {
                return ( Decimal ) Convert.ChangeType( Value , typeof( Decimal ) );
            } else if ( Value.IsFloatingPoint( out code ) ) {
                return ( Decimal ) Convert.ChangeType( Value , typeof( Decimal ) );
            } else if ( Value.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( Value , typeof( char ) );
                        return ( decimal ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !decimal.TryParse( Value.ToString() , out decimal r ) ) {
                            throw new InvalidCastException( "Unable to convert " + Value.ToString() + " to " + DataType.ToString() );
                        } else {
                            return ( decimal ) r;
                        }
                    }
                }

            } else if ( Value.IsDateTime() ) {
                return ( decimal ) ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ).ToOADate();
            } else if ( Value.IsBoolean() ) {
                return ( decimal ) ( ( ( bool ) Convert.ChangeType( Value , typeof( bool ) ) ) ? 1.0 : 0.0 );
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        double IConvertible.ToDouble( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        string IConvertible.ToString( IFormatProvider provider ) {
            TypeCode code;
            if ( Value is null ) {
                return isNotNull ? default( string ) : throw new ArgumentNullException();
            } else if ( Value.IsInteger( out code ) ) {
                return ( ( long ) Convert.ChangeType( Value , typeof( long ) ) ).ToString();
            } else if ( Value.IsFloatingPoint( out code ) ) {
                return ( ( decimal ) Convert.ChangeType( Value , typeof( decimal ) ) ).ToString();
            } else if ( Value.IsLitteral( out code ) ) {
                return Value.ToString();
            } else if ( Value.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ).ToString( MySQLQueryParam.MYSQL_DATE_FORMAT ); // ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( Value.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( Value , typeof( bool ) ) ) ? "True" : "False";
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        object IConvertible.ToType( Type conversionType , IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        //sbyte => short, int, long, float, double, decimal o nint
        //byte => short, ushort, int, uint, long, ulong, float, double, decimal, nint o nuint
        //short => int, long , float , double o decimal``nint
        //ushort => int, uint , long , ulong , float , double , o decimal , nint o nuint
        //int => long, float , double o decimal , nint
        //uint => long, ulong , float , double o decimal``nuint
        //long => float, doubleo decimal
        //ulong => float, double decimal
        //float => double

        uint IConvertible.ToUInt32( IFormatProvider provider ) {
            TypeCode code;
            if ( Value is null ) {
                return isNotNull ? default( uint ) : throw new ArgumentNullException();
            } else if ( Value.IsInteger( out code ) ) {
                return Convert.ToUInt32( Value );
            } else if ( Value.IsFloatingPoint( out code ) ) {
                return ( uint ) Convert.ChangeType( Value , typeof( uint ) );
            } else if ( Value.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( Value , typeof( char ) );
                        return ( uint ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !uint.TryParse( Value.ToString() , out uint r ) ) {
                            throw new InvalidCastException( "Unable to convert " + Value.ToString() + " to " + DataType.ToString() );
                        } else {
                            return ( uint ) r;
                        }
                    }
                }

            } else if ( Value.IsDateTime() ) {
                return ( uint ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
            } else if ( Value.IsBoolean() ) {
                return ( uint ) ( ( ( bool ) Convert.ChangeType( Value , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + Value.ToString() );
            }
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            throw new NotImplementedException();
        }
    }
}