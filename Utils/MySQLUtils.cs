using prestoMySQL.Column.DataType;
using prestoMySQL.Extension;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Utils {
    public static class MySQLUtils {

        public static DateTime DateMinValue = new DateTime( 1000 , 1 , 1 );
        //Todo deprecated
        public static object convertFromMySQLDataTypeToObject<T>( object value , MySQLDataType aSQLDataType )  {

            object Params = null;

            switch ( aSQLDataType ) {
                //Integer Types (Exact Value)
                case MySQLDataType.dbtTinyInt:
                case MySQLDataType.dbtSmallInt:
                case MySQLDataType.dbtMediumInt:
                case MySQLDataType.dbtInteger:
                case MySQLDataType.dbtBigInt:
                if ( value is null ) {
                    Params = SQLTypeWrapper<int>.NULL;
                } else if ( value.IsInteger() ) {
                    Params = value;
                } else if ( value.IsFloatingPoint() ) {
                    Params = Convert.ChangeType( value , typeof( int ) );
                } else if ( value.IsLitteral() ) {
                    if ( !int.TryParse( value.ToString() , out int r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r;
                    }
                } else if ( value.IsDateTime() ) {
                    Params = new DateTimeOffset( ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
                } else if ( value.IsBoolean() ) {
                    Params = ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 1 : 0;
                } else {
                    throw new InvalidCastException( "Unknow type for value " + value.ToString() );
                }
                break;
                //Fixed-Point Types (Exact Value) 
                case MySQLDataType.dbtDecimal:
                case MySQLDataType.dbtNumeric:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = (Decimal) Convert.ChangeType( value , typeof( Decimal ) );
                } else if ( value.IsFloatingPoint() ) {
                    Params = value;
                } else if ( value.IsLitteral() ) {
                    if ( !decimal.TryParse( value.ToString() , out decimal r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r;
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToOADate();
                } else if ( value.IsBoolean() ) {
                    Params = (decimal) ( ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 1.0 : 0.0 );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                //Floating-Point Types (Approximate Value) 
                case MySQLDataType.dbtFloat:
                case MySQLDataType.dbtReal:
                case MySQLDataType.dbtDoublePrecision:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = (Decimal) Convert.ChangeType( value , typeof( Decimal ) );
                } else if ( value.IsFloatingPoint() ) {
                    Params = value;
                } else if ( value.IsLitteral() ) {
                    if ( !decimal.TryParse( value.ToString() , out decimal r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r;
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToOADate();
                } else if ( value.IsBoolean() ) {
                    Params = (decimal) ( ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 1.0 : 0.0 );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                //Bit-Value Type
                case MySQLDataType.dbtBit:
                byte[] byteArray = null;
                TypeCode code = Type.GetTypeCode( typeof( T ) );

                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    switch ( code ) {
                        case TypeCode.SByte:
                        byteArray = BitConverter.GetBytes( (sbyte) Convert.ChangeType( value , typeof( sbyte ) ) );
                        break;
                        case TypeCode.Byte:
                        byteArray = BitConverter.GetBytes( (byte) Convert.ChangeType( value , typeof( byte ) ) );
                        break;
                        case TypeCode.Int16:
                        byteArray = BitConverter.GetBytes( (short) Convert.ChangeType( value , typeof( short ) ) );
                        break;
                        case TypeCode.UInt16:
                        byteArray = BitConverter.GetBytes( (ushort) Convert.ChangeType( value , typeof( ushort ) ) );
                        break;
                        case TypeCode.Int32:
                        byteArray = BitConverter.GetBytes( (int) Convert.ChangeType( value , typeof( int ) ) );
                        break;
                        case TypeCode.UInt32:
                        byteArray = BitConverter.GetBytes( (uint) Convert.ChangeType( value , typeof( uint ) ) );
                        break;
                        case TypeCode.Int64:
                        byteArray = BitConverter.GetBytes( (long) Convert.ChangeType( value , typeof( long ) ) );
                        break;
                        case TypeCode.UInt64:
                        byteArray = BitConverter.GetBytes( (ulong) Convert.ChangeType( value , typeof( ulong ) ) );
                        break;
                        default:
                        throw new InvalidCastException();
                    }
                } else if ( value.IsFloatingPoint() ) {
                    switch ( code ) {
                        case TypeCode.Single:
                        byteArray = BitConverter.GetBytes( (float) Convert.ChangeType( value , typeof( float ) ) );
                        break;
                        case TypeCode.Double:
                        byteArray = BitConverter.GetBytes( (double) Convert.ChangeType( value , typeof( double ) ) );
                        break;
                        default:
                        throw new InvalidCastException();
                    }
                } else if ( value.IsLitteral() ) {

                    switch ( code ) {
                        case TypeCode.Char:
                        byteArray = BitConverter.GetBytes( (char) Convert.ChangeType( value , typeof( char ) ) );
                        break;
                        case TypeCode.String:
                        string s = value.ToString();
                        byteArray = new byte[s.ToString().Length * 2];
                        for ( int j = 0; j < s.Length; j++ )
                            Array.Copy( BitConverter.GetBytes( s[j] ) , 0 , byteArray , j * 2 , 2 );
                        break;
                        default:
                        throw new InvalidCastException();
                    }

                } else if ( value.IsDateTime() ) {
                    byteArray = BitConverter.GetBytes( ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).Millisecond );
                } else if ( value.IsBoolean() ) {
                    byteArray = BitConverter.GetBytes( (bool) Convert.ChangeType( value , typeof( bool ) ) );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                if ( byteArray == null ) { Params = DBNull.Value; } else {
                    if ( BitConverter.IsLittleEndian ) { Array.Reverse( byteArray ); };
                    //hexvalue = ;
                    Params = String.Join( String.Empty , BitConverter.ToString( byteArray ).Replace( "-" , "" ).Select( c => Convert.ToString( Convert.ToUInt32( c.ToString() , 16 ) , 2 ).PadLeft( 4 , '0' ) ) );



                }
                break;
                //Datetime
                case MySQLDataType.dbtDate:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = DateTimeOffset.FromUnixTimeSeconds( (long) Convert.ChangeType( value , typeof( long ) ) ).ToString( "yyyy-MM-dd" );
                } else if ( value.IsFloatingPoint() ) {
                    Params = DateTime.FromOADate( ( (double) Convert.ChangeType( value , typeof( double ) ) ) ).ToString( "yyyy-MM-dd" );
                } else if ( value.IsLitteral() ) {
                    if ( !DateTime.TryParse( value.ToString() , out DateTime r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r.ToString( "yyyy-MM-dd" );
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( "yyyy-MM-dd" );
                } else if ( value.IsBoolean() ) {
                    throw new InvalidCastException( "Can't convert boolean to date time" );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtTime:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = TimeSpan.FromSeconds( (double) Convert.ChangeType( value , typeof( int ) ) ).ToString( "HH:mm:ss" );
                } else if ( value.IsFloatingPoint() ) {
                    Params = TimeSpan.FromMilliseconds( (double) Convert.ChangeType( value , typeof( int ) ) ).ToString( "HH:mm:ss" );
                } else if ( value.IsLitteral() ) {
                    if ( !TimeSpan.TryParse( value.ToString() , out TimeSpan r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r.ToString( "HH:mm:ss" );
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( "HH:mm:ss" );
                } else if ( value.IsBoolean() ) {
                    throw new InvalidCastException( "Can't convert boolean to time" );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                case MySQLDataType.dbtYear:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = (byte) Convert.ChangeType( value , typeof( byte ) );
                } else if ( value.IsFloatingPoint() ) {
                    Params = (byte) Convert.ChangeType( value , typeof( float ) );
                } else if ( value.IsLitteral() ) {
                    if ( !byte.TryParse( value.ToString() , out byte r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r;
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( "YYYY" );
                } else if ( value.IsBoolean() ) {
                    throw new InvalidCastException( "Can't convert boolean to year" );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtTimestamp:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = ( (long) Convert.ChangeType( value , typeof( long ) ) );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                case MySQLDataType.dbtDateTime:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = DateTimeOffset.FromUnixTimeSeconds( (long) Convert.ChangeType( value , typeof( long ) ) ).LocalDateTime.ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsFloatingPoint() ) {
                    Params = DateTime.FromOADate( ( (double) Convert.ChangeType( value , typeof( double ) ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsLitteral() ) {
                    if ( !DateTime.TryParse( value.ToString() , out DateTime r ) ) {
                        throw new InvalidCastException( "Unable to convert " + value.ToString() + " to " + aSQLDataType.ToString() );
                    } else {
                        Params = r.ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                    }
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsBoolean() ) {
                    throw new InvalidCastException( "Can't convert boolean to date time" );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtChar:
                case MySQLDataType.dbtVarChar:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = ( (long) Convert.ChangeType( value , typeof( long ) ) ).ToString();
                } else if ( value.IsFloatingPoint() ) {
                    Params = ( (decimal) Convert.ChangeType( value , typeof( decimal ) ) ).ToString();
                } else if ( value.IsLitteral() ) {
                    Params = value.ToString();
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsBoolean() ) {
                    Params = ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 'Y' : 'N';
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                //they store byte strings rather than character strings.
                case MySQLDataType.dbtBinary:
                case MySQLDataType.dbtVarBinary:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = ( (long) Convert.ChangeType( value , typeof( long ) ) ).ToString();
                } else if ( value.IsFloatingPoint() ) {
                    Params = ( (decimal) Convert.ChangeType( value , typeof( decimal ) ) ).ToString();
                } else if ( value.IsLitteral() ) {
                    Params = value.ToString();
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsBoolean() ) {
                    Params = ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 'Y' : 'N';
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtTinyBlob:
                case MySQLDataType.dbtBlob:
                case MySQLDataType.dbtMediumBlob:
                case MySQLDataType.dbtLongBlob:
                byteArray = null;
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value is byte[] ) {
                    Params = (byte[]) Convert.ChangeType( value , typeof( byte[] ) );
                } else if ( value.IsInteger() ) {
                    byteArray = BitConverter.GetBytes( (long) Convert.ChangeType( value , typeof( long ) ) );
                    if ( BitConverter.IsLittleEndian ) { Array.Reverse( byteArray ); };
                    Params = BitConverter.ToString( byteArray ).Replace( "-" , "" );
                } else if ( value.IsFloatingPoint() ) {
                    switch ( Type.GetTypeCode( typeof( T ) ) ) {
                        case TypeCode.Single:
                        byteArray = BitConverter.GetBytes( (float) Convert.ChangeType( value , typeof( float ) ) );
                        break;
                        case TypeCode.Double:
                        byteArray = BitConverter.GetBytes( (double) Convert.ChangeType( value , typeof( double ) ) );
                        break;
                        default:
                        throw new InvalidCastException();
                    }
                    if ( BitConverter.IsLittleEndian ) { Array.Reverse( byteArray ); };
                    Params = BitConverter.ToString( byteArray ).Replace( "-" , "" );
                } else if ( value.IsLitteral() ) {
                    Params = BitConverter.ToString( Encoding.Default.GetBytes( value.ToString() ) ).Replace( "-" , "" );
                } else if ( value.IsDateTime() ) {
                    Params = BitConverter.GetBytes( new DateTimeOffset( ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ) ).ToUnixTimeSeconds() );
                } else if ( value.IsBoolean() ) {
                    Params = BitConverter.GetBytes( (bool) Convert.ChangeType( value , typeof( bool ) ) );
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtTinyText:
                case MySQLDataType.dbtText:
                case MySQLDataType.dbtMediumText:
                case MySQLDataType.dbtLongText:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    Params = ( (long) Convert.ChangeType( value , typeof( long ) ) ).ToString();
                } else if ( value.IsFloatingPoint() ) {
                    Params = ( (decimal) Convert.ChangeType( value , typeof( decimal ) ) ).ToString();
                } else if ( value.IsLitteral() ) {
                    Params = value.ToString();
                } else if ( value.IsDateTime() ) {
                    Params = ( (DateTime) Convert.ChangeType( value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
                } else if ( value.IsBoolean() ) {
                    Params = ( (bool) Convert.ChangeType( value , typeof( bool ) ) ) ? 'Y' : 'N';
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;

                case MySQLDataType.dbtEnum:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    throw new NotImplementedException();
                } else if ( value.IsFloatingPoint() ) {
                    throw new NotImplementedException();
                } else if ( value.IsLitteral() ) {
                    throw new NotImplementedException();
                } else if ( value.IsDateTime() ) {
                    throw new NotImplementedException();
                } else if ( value.IsBoolean() ) {
                    throw new NotImplementedException();
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                //A SET column can have a maximum of 64 distinct members.
                case MySQLDataType.dbtSet:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    throw new NotImplementedException();
                } else if ( value.IsFloatingPoint() ) {
                    throw new NotImplementedException();
                } else if ( value.IsLitteral() ) {
                    throw new NotImplementedException();
                } else if ( value.IsDateTime() ) {
                    throw new NotImplementedException();
                } else if ( value.IsBoolean() ) {
                    throw new NotImplementedException();
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;
                //       //11.4.1 Spatial Data Types : GEOMETRY, POINT, LINESTRING, POLYGON,MULTIPOINT,MULTILINESTRING,MULTIPOLYGON,GEOMETRYCOLLECTION
                //            dynamic jobject = JsonConvert.DeserializeObject( jsonStr );


                case MySQLDataType.dbtJSON:
                if ( value == null ) {
                    Params = DBNull.Value;
                } else if ( value.IsInteger() ) {
                    throw new NotImplementedException();
                } else if ( value.IsFloatingPoint() ) {
                    throw new NotImplementedException();
                } else if ( value.IsLitteral() ) {
                    throw new NotImplementedException();
                } else if ( value.IsDateTime() ) {
                    throw new NotImplementedException();
                } else if ( value.IsBoolean() ) {
                    throw new NotImplementedException();
                    //} else if ( value.IsDBNull() ) {
                    //    Params = DBNull.Value;
                } else {
                    throw new InvalidCastException( "Uknow type" );
                }
                break;


            }


            return  Params;
        }
    
    }

}