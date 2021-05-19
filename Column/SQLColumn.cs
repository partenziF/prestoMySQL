
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
    public class SQLColumn<T> : GenericSQLColumn<T>, IConvertible where T : ISQLTypeWrapper { //where T : struct        

        public SQLColumn( string aDeclaredVariableName , PropertyInfo aMethodBase = null ) : base( aMethodBase ) {

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
            this.mSQLDataType = ( MySQLDataType ) ( columnAttribute as DDColumnAttribute ).DataType;


            
        }

        // Data Type used in database read from attribute
        private readonly MySQLDataType mSQLDataType;
        //private readonly MySqlDbType mDataType;

        public MySQLDataType SQLDataType { get => this.mSQLDataType; }

        private readonly string mDeclaredVariableName;

        private readonly byte mSize;
        public byte Size { get => mSize; }

        private readonly SignType mSigned;
        public SignType Signed { get => mSigned; }



        public override object ValueAsParamType() {

            return MySQLUtils.convertFromMySQLDataTypeToObject<T>( TypeWrapper.ToObject() , SQLDataType );

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
            return Type.GetTypeCode( TypeWrapper.GetType() );

        }

        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar( IFormatProvider provider ) {

            TypeCode code;
            if ( TypeWrapper is null ) {
                return isNotNull ? default( char ) : throw new ArgumentNullException();
            } else if ( TypeWrapper.IsInteger( out code ) ) {
                if ( code == TypeCode.Byte ) {
                    return Convert.ToChar( Convert.ChangeType( TypeWrapper , typeof( Byte ) ) );
                } else {
                    throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }
                //return ( ( long ) Convert.ChangeType( Value , typeof( long ) ) ).ToString();
            } else if ( TypeWrapper.IsFloatingPoint( out code ) ) {
                throw new InvalidCastException( "Unable to convert " + code + " to char" );
                //return ( ( decimal ) Convert.ChangeType( Value , typeof( decimal ) ) ).ToString();
            } else if ( TypeWrapper.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char:
                    return ( char ) Convert.ChangeType( TypeWrapper , typeof( char ) );
                    default:
                    if ( TypeWrapper.ToString().Length == 1 ) {
                        return ( char ) TypeWrapper.ToString()[0];
                    } else
                        throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }

            } else if ( TypeWrapper.IsDateTime() ) {

                throw new InvalidCastException( "Unable to convert DateTime to char" );

                //return ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( TypeWrapper.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( TypeWrapper , typeof( bool ) ) ) ? 'Y' : 'N';
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {

            TypeCode code;
            if ( TypeWrapper is null ) {
                return isNotNull ? default( DateTime ) : throw new ArgumentNullException();
            } else if ( TypeWrapper.IsInteger( out code ) ) {
                return DateTimeOffset.FromUnixTimeSeconds( ( long ) Convert.ChangeType( TypeWrapper , typeof( long ) ) ).DateTime;
            } else if ( TypeWrapper.IsFloatingPoint( out code ) ) {
                DateTime a = DateTime.FromOADate( ( double ) Convert.ChangeType( TypeWrapper , typeof( double ) ) );
                return new DateTime( a.Year , a.Month , a.Day , a.Hour , a.Minute , a.Second );
            } else if ( TypeWrapper.IsLitteral( out code ) ) {
                switch ( code ) {

                    case TypeCode.String: {
                        return DateTime.ParseExact( TypeWrapper.ToString() , MySQLQueryParam.MYSQL_DATE_FORMAT , CultureInfo.InvariantCulture );
                    }

                    default: {
                        throw new InvalidCastException( "unable to convert char to datetime" );
                    }
                }
            } else if ( TypeWrapper.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( TypeWrapper , typeof( DateTime ) ) );
            } else if ( TypeWrapper.IsBoolean() ) {
                throw new InvalidCastException( "unable to convert boolean to datetime" );
            } else {
                throw new InvalidCastException( "Uknow type" );
            }

        }

        decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            TypeCode code;
            if ( TypeWrapper is null ) {
                return isNotNull ? default( Decimal ) : throw new ArgumentNullException();
            } else if ( TypeWrapper.IsInteger( out code ) ) {
                return ( Decimal ) Convert.ChangeType( TypeWrapper , typeof( Decimal ) );
            } else if ( TypeWrapper.IsFloatingPoint( out code ) ) {
                return ( Decimal ) Convert.ChangeType( TypeWrapper , typeof( Decimal ) );
            } else if ( TypeWrapper.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapper , typeof( char ) );
                        return ( decimal ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !decimal.TryParse( TypeWrapper.ToString() , out decimal r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapper.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( decimal ) r;
                        }
                    }
                }

            } else if ( TypeWrapper.IsDateTime() ) {
                return ( decimal ) ( ( DateTime ) Convert.ChangeType( TypeWrapper , typeof( DateTime ) ) ).ToOADate();
            } else if ( TypeWrapper.IsBoolean() ) {
                return ( decimal ) ( ( ( bool ) Convert.ChangeType( TypeWrapper , typeof( bool ) ) ) ? 1.0 : 0.0 );
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
            if ( TypeWrapper is null ) {
                return isNotNull ? default( string ) : throw new ArgumentNullException();
            } else if ( TypeWrapper.IsInteger( out code ) ) {
                return ( ( long ) Convert.ChangeType( TypeWrapper , typeof( long ) ) ).ToString();
            } else if ( TypeWrapper.IsFloatingPoint( out code ) ) {
                return ( ( decimal ) Convert.ChangeType( TypeWrapper , typeof( decimal ) ) ).ToString();
            } else if ( TypeWrapper.IsLitteral( out code ) ) {
                return TypeWrapper.ToString();
            } else if ( TypeWrapper.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( TypeWrapper , typeof( DateTime ) ) ).ToString( MySQLQueryParam.MYSQL_DATE_FORMAT ); // ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( TypeWrapper.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( TypeWrapper , typeof( bool ) ) ) ? "True" : "False";
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
            if ( TypeWrapper is null ) {
                return isNotNull ? default( uint ) : throw new ArgumentNullException();
            } else if ( TypeWrapper.IsInteger( out code ) ) {
                return Convert.ToUInt32( TypeWrapper );
            } else if ( TypeWrapper.IsFloatingPoint( out code ) ) {
                return ( uint ) Convert.ChangeType( TypeWrapper , typeof( uint ) );
            } else if ( TypeWrapper.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapper , typeof( char ) );
                        return ( uint ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !uint.TryParse( TypeWrapper.ToString() , out uint r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapper.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( uint ) r;
                        }
                    }
                }

            } else if ( TypeWrapper.IsDateTime() ) {
                return ( uint ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapper , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
            } else if ( TypeWrapper.IsBoolean() ) {
                return ( uint ) ( ( ( bool ) Convert.ChangeType( TypeWrapper , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapper.ToString() );
            }
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            throw new NotImplementedException();
        }

        public override void AssignValue( object x ) {

            //Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            //Type[] types = new Type[4];
            //types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            //types[2] = typeof( IQueryParams );
            //types[3] = typeof( string );
            //Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            //ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            //DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { ( MySQLQueryParam ) aColumnDefinition } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            //return o;

            var genericType = typeof( T ).GetGenericArguments()[0];

            ConstructorInfo ctor = typeof( T ).GetConstructor( new Type[] { genericType } );
            TypeWrapper = ( T ) ctor?.Invoke( new object[] { Convert.ChangeType( x , genericType ) } );

            //SQLTypeWrapper<uint> b = new SQLTypeWrapper<uint>();
            //uint y = x.ConvertTo<uint>();
            //Value = new SQLTypeWrapper<uint>( y );


        }

    }
}