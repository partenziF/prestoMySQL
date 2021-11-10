
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

            return MySQLUtils.convertFromMySQLDataTypeToObject<T>( TypeWrapperValue.ToObject() , SQLDataType );

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
            return Type.GetTypeCode( TypeWrapperValue.GetType() );

        }

        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToBoolean )}" );
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( byte ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return Convert.ToByte( TypeWrapperValue );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                //return ( sbyte ) Convert.ChangeType( TypeWrapperValue , typeof( sbyte ) );
                throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToByte )}" );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( byte ) Convert.ToByte( c );
                    }

                    default: {
                        //if ( !uint.TryParse( TypeWrapperValue.ToString() , out uint r ) ) {
                        //    throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        //} else {
                        //    return ( sbyte ) r;
                        //}
                        throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToByte )}" );
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                //return ( sbyte ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
                throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToByte )}" );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( byte ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapperValue.ToString() );
            }

        }

        char IConvertible.ToChar( IFormatProvider provider ) {

            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( char ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                if ( code == TypeCode.Byte ) {
                    return Convert.ToChar( Convert.ChangeType( TypeWrapperValue , typeof( Byte ) ) );
                } else {
                    throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }
                //return ( ( long ) Convert.ChangeType( Value , typeof( long ) ) ).ToString();
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                throw new InvalidCastException( "Unable to convert " + code + " to char" );
                //return ( ( decimal ) Convert.ChangeType( Value , typeof( decimal ) ) ).ToString();
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char:
                    return ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                    default:
                    if ( TypeWrapperValue.ToString().Length == 1 ) {
                        return ( char ) TypeWrapperValue.ToString()[0];
                    } else
                        throw new InvalidCastException( "Unable to convert " + code + " to char" );
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {

                throw new InvalidCastException( "Unable to convert DateTime to char" );

                //return ( ( DateTime ) Convert.ChangeType( Value , typeof( DateTime ) ) ).ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 'Y' : 'N';
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {

            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( DateTime ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return DateTimeOffset.FromUnixTimeSeconds( ( long ) Convert.ChangeType( TypeWrapperValue , typeof( long ) ) ).DateTime;
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                DateTime a = DateTime.FromOADate( ( double ) Convert.ChangeType( TypeWrapperValue , typeof( double ) ) );
                return new DateTime( a.Year , a.Month , a.Day , a.Hour , a.Minute , a.Second );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {

                    case TypeCode.String: {
                        return DateTime.ParseExact( TypeWrapperValue.ToString() , MySQLQueryParam.MYSQL_DATE_FORMAT , CultureInfo.InvariantCulture );
                    }

                    default: {
                        throw new InvalidCastException( "unable to convert char to datetime" );
                    }
                }
            } else if ( TypeWrapperValue.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                throw new InvalidCastException( "unable to convert boolean to datetime" );
            } else {
                throw new InvalidCastException( "Uknow type" );
            }

        }

        decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( Decimal ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return ( Decimal ) Convert.ChangeType( TypeWrapperValue , typeof( Decimal ) );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                return ( Decimal ) Convert.ChangeType( TypeWrapperValue , typeof( Decimal ) );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {

                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( decimal ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !decimal.TryParse( TypeWrapperValue.ToString() , out decimal r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( decimal ) r;
                        }
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                return ( decimal ) ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ).ToOADate();
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( decimal ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1.0 : 0.0 );
            } else {
                throw new InvalidCastException( "Uknow type" );
            }
        }

        double IConvertible.ToDouble( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToDouble )}" );
        }

        short IConvertible.ToInt16( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToInt16 )}" );
        }

        int IConvertible.ToInt32( IFormatProvider provider ) {
            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( int ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return Convert.ToInt32( TypeWrapperValue );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                return ( int ) Convert.ChangeType( TypeWrapperValue , typeof( int ) );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( int ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !uint.TryParse( TypeWrapperValue.ToString() , out uint r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( int ) r;
                        }
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                return ( int ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( int ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapperValue.ToString() );
            }
        }

        long IConvertible.ToInt64( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToInt64 )}" );
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider ) {


            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( sbyte ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return Convert.ToSByte( TypeWrapperValue );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                //return ( sbyte ) Convert.ChangeType( TypeWrapperValue , typeof( sbyte ) );
                throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToSByte )}" );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( sbyte ) Convert.ToByte( c );
                    }

                    default: {
                        //if ( !uint.TryParse( TypeWrapperValue.ToString() , out uint r ) ) {
                        //    throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        //} else {
                        //    return ( sbyte ) r;
                        //}
                        throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToSByte )}" );
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                //return ( sbyte ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
                throw new InvalidCastException( $"{this.ColumnName} not converted {nameof( IConvertible.ToSByte )}" );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( sbyte ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapperValue.ToString() );
            }

            //
        }

        float IConvertible.ToSingle( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToSingle )}" );
        }

        string IConvertible.ToString( IFormatProvider provider ) {
            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( string ) : throw new ArgumentNullException();
            } else if (TypeWrapperValue.ToObject().GetType().IsEnum ) {
                return TypeWrapperValue.ToString();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return ( ( long ) Convert.ChangeType( TypeWrapperValue , typeof( long ) ) ).ToString();
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                return ( ( decimal ) Convert.ChangeType( TypeWrapperValue , typeof( decimal ) ) ).ToString();
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                return TypeWrapperValue.ToString();
            } else if ( TypeWrapperValue.IsDateTime() ) {
                return ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ).ToString( MySQLQueryParam.MYSQL_DATE_FORMAT ); // ToString( CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? "True" : "False";
            } else if ( TypeWrapperValue.ValueIsNull() ) {
                throw new ArgumentNullException();
            } else {
                throw new InvalidCastException( "Uknown type for column " + ColumnName + " defined in " + mPropertyInfo.Name );
            }
        }

        object IConvertible.ToType( Type conversionType , IFormatProvider provider ) {

            //if ( conversionType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) ) {                
            //    return new SQLProjectionColumn<SQLTypeWrapper<uint>>();
            //}


            throw new InvalidCastException( $"{this.ColumnName} not converted in {conversionType.GetGenericTypeDefinition()}" );
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider ) {

            TypeCode code;
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( ushort ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return Convert.ToUInt16( TypeWrapperValue );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                return ( ushort ) Convert.ChangeType( TypeWrapperValue , typeof( ushort ) );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( ushort ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !ushort.TryParse( TypeWrapperValue.ToString() , out ushort r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( ushort ) r;
                        }
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                //return ( ushort ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
                throw new InvalidCastException( "Uknown type for column " + ColumnName + " defined in " + mPropertyInfo.Name );
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( ushort ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapperValue.ToString() );
            }


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
            if ( TypeWrapperValue is null ) {
                return isNotNull ? default( uint ) : throw new ArgumentNullException();
            } else if ( TypeWrapperValue.IsInteger( out code ) ) {
                return Convert.ToUInt32( TypeWrapperValue );
            } else if ( TypeWrapperValue.IsFloatingPoint( out code ) ) {
                return ( uint ) Convert.ChangeType( TypeWrapperValue , typeof( uint ) );
            } else if ( TypeWrapperValue.IsLitteral( out code ) ) {
                switch ( code ) {
                    case TypeCode.Char: {
                        var c = ( char ) Convert.ChangeType( TypeWrapperValue , typeof( char ) );
                        return ( uint ) Convert.ToByte( c );
                    }

                    default: {
                        if ( !uint.TryParse( TypeWrapperValue.ToString() , out uint r ) ) {
                            throw new InvalidCastException( "Unable to convert " + TypeWrapperValue.ToString() + " to " + SQLDataType.ToString() );
                        } else {
                            return ( uint ) r;
                        }
                    }
                }

            } else if ( TypeWrapperValue.IsDateTime() ) {
                return ( uint ) new DateTimeOffset( ( ( DateTime ) Convert.ChangeType( TypeWrapperValue , typeof( DateTime ) ) ) ).ToUnixTimeSeconds();
            } else if ( TypeWrapperValue.IsBoolean() ) {
                return ( uint ) ( ( ( bool ) Convert.ChangeType( TypeWrapperValue , typeof( bool ) ) ) ? 1 : 0 );
            } else {
                throw new InvalidCastException( "Unknow type for value " + TypeWrapperValue.ToString() );
            }
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            throw new NotImplementedException( $"{this.ColumnName} not converted {nameof( IConvertible.ToUInt64 )}" );
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
            if ( Nullable.GetUnderlyingType( genericType ) != null ) {

                Type NullableType = Nullable.GetUnderlyingType( genericType );
                var notNullableValue = Convert.ChangeType( x , NullableType );
                if ( NullableType.IsValueType ) {

                    var nctor = typeof( Nullable<> ).MakeGenericType( NullableType )?.GetConstructor( new Type[] { NullableType } );
                    //Nullable<uint> xxx = new Nullable<uint>( (uint) notNullableValue );
                    TypeWrapperValue = ( T ) ctor?.Invoke( new object[] { nctor.Invoke( new object[] { notNullableValue } ) } );
                } else {
                    throw new NotImplementedException( "not implemented on SQLColumn line 449" );
                }

            } else {
                TypeWrapperValue = ( T ) ctor?.Invoke( new object[] { Convert.ChangeType( x , genericType ) } );
            }

            //SQLTypeWrapper<uint> b = new SQLTypeWrapper<uint>();
            //uint y = x.ConvertTo<uint>();
            //Value = new SQLTypeWrapper<uint>( y );


        }


        public SQLColumn<T> Copy() {

            return ( SQLColumn<T> ) base.Copy();
        }

    }
}