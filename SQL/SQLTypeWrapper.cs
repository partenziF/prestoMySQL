using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.SQL {
    public class SQLTypeWrapper<T> : ISQLTypeWrapper, IConvertible where T : notnull {

        public static readonly SQLTypeWrapper<T> NULL = new SQLTypeWrapper<T>( default( T ) ) { mIsNull = true };

        public SQLTypeWrapper() {
            if ( default( T ) is null ) {
                this.mValue = default( T );
                this.mIsNull = true;

            } else {
                this.mValue = default( T );
                this.mIsNull = false;
            }
        }

        public SQLTypeWrapper( T mValue ) {
            this.mValue = mValue;
            this.mIsNull = false;
        }

        T mValue;
        public T Value { get => mValue; }

        bool mIsNull;
        public bool IsNull { get => mIsNull; }

        public bool ValueIsNull() { return mValue is null; }


        public static implicit operator SQLTypeWrapper<T>( T x ) {
            return new SQLTypeWrapper<T>( x );
        }

        public static explicit operator T( SQLTypeWrapper<T> x ) {
            if ( x is null ) throw new ArgumentNullException();
            if ( x.IsNull ) throw new InvalidOperationException();
            return x.mValue;
        }


        public SQLTypeWrapper<T> Copy() {
            return ( SQLTypeWrapper<T> ) this.MemberwiseClone();
        }


        //public static implicit operator T( SQLTypeWrapper<T> x ) {
        //    if ( x.IsNull ) throw new InvalidOperationException();
        //    return x.mValue;
        //}


        #region Conversion method

        public override string ToString() {
            return mIsNull ? "NULL" : mValue.ToString();
        }

        public bool IsInteger( out TypeCode code ) {
            code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return ( ( code >= TypeCode.SByte ) && ( code <= TypeCode.UInt64 ) );
        }

        public bool IsBoolean() {
            TypeCode code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return ( code == TypeCode.Boolean );
        }

        public bool IsDateTime() {
            TypeCode code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return ( code == TypeCode.DateTime );
        }

        public bool IsFloatingPoint( out TypeCode code ) {
            code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return ( ( code >= TypeCode.Single ) && ( code <= TypeCode.Decimal ) );
        }

        public bool IsLitteral( out TypeCode code ) {
            code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return ( ( code == TypeCode.Char ) || ( code == TypeCode.String ) );
        }

        public object ToObject() {
            return ( IsNull ) ? null : Value;
        }

        TypeCode IConvertible.GetTypeCode() {
            TypeCode code = ( !IsNull ) ? Convert.GetTypeCode( mValue ) : 0;
            return code;
        }

        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            return Convert.ToBoolean( mValue );
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            return Convert.ToByte( mValue );
        }

        char IConvertible.ToChar( IFormatProvider provider ) {
            return Convert.ToChar( mValue );
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {
            return Convert.ToDateTime( mValue );
        }

        decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            return Convert.ToDecimal( mValue );
        }

        double IConvertible.ToDouble( IFormatProvider provider ) {
            return Convert.ToDouble( mValue );
        }

        short IConvertible.ToInt16( IFormatProvider provider ) {
            return Convert.ToInt16( mValue );
        }

        int IConvertible.ToInt32( IFormatProvider provider ) {
            return Convert.ToInt32( mValue );
        }

        long IConvertible.ToInt64( IFormatProvider provider ) {
            return Convert.ToInt64( mValue );
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider ) {
            return Convert.ToSByte( mValue );
        }

        float IConvertible.ToSingle( IFormatProvider provider ) {
            return Convert.ToSingle( mValue );
        }

        string IConvertible.ToString( IFormatProvider provider ) {
            return Convert.ToString( mValue );
        }

        object IConvertible.ToType( Type conversionType , IFormatProvider provider ) {
            return Convert.ChangeType( mValue , conversionType );
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider ) {
            return Convert.ToUInt16( mValue );
        }

        uint IConvertible.ToUInt32( IFormatProvider provider ) {
            return Convert.ToUInt32( mValue );
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            return Convert.ToUInt64( mValue );
        }

        public override bool Equals( object obj ) {
            return obj is SQLTypeWrapper<T> wrapper &&
                    EqualityComparer<T>.Default.Equals( this.mValue , wrapper.mValue ) &&
                     this.mIsNull == wrapper.mIsNull;
        }

        public override int GetHashCode() {
            return HashCode.Combine( this.mValue , this.mIsNull );
        }

        //public override bool Equals( object obj ) {

        //    return ( Value.Equals(  ( obj as SQLTypeWrapper<T> ).Value ) && ( IsNull == ( obj as SQLTypeWrapper<T> ).IsNull ) );
        //}

        //public override int GetHashCode() {
        //    throw new NotImplementedException();
        //}



    }

    #endregion
    //public class IntSQLTypeWrapper : SQLTypeWrapper<int> {
    //    public IntSQLTypeWrapper( int mValue ) : base( mValue ) {
    //    }
    //}

    //public class ShortSQLTypeWrapper : SQLTypeWrapper<short> {
    //    public ShortSQLTypeWrapper( short mValue ) : base( mValue ) {
    //    }
    //}

    //public class LongSQLTypeWrapper : SQLTypeWrapper<long> {
    //    public LongSQLTypeWrapper( long mValue ) : base( mValue ) {
    //    }
    //}



}