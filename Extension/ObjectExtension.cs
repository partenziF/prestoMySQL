using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Extension {
    public static class ObjectExtension {


        public static T ConvertTo<T>( this object source ) {

            Type t = typeof( T );

            if ( t.IsGenericType && ( t.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {
                if ( source == null ) {
                    return default( T );
                } else {
                    return (T) Convert.ChangeType( source , Nullable.GetUnderlyingType( t ) );
                }
            } else {
                return (T) Convert.ChangeType( source , t );
            }

            //return (T)Convert.ChangeType( source, typeof( T ) );
        }

        public static T ConvertTo<T>( this object source , T defaultvalue ) {

            if ( source != null ) {
                try {
                    //return (T)Convert.ChangeType( source, typeof( T ) );
                    Type t = typeof( T );

                    if ( t.IsGenericType && ( t.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {
                        if ( source == null ) {
                            //return (T)(object)null;
                            return defaultvalue;
                        } else {
                            return (T) Convert.ChangeType( source , Nullable.GetUnderlyingType( t ) );
                        }
                    } else {
                        return (T) Convert.ChangeType( source , t );
                    }

                } catch {
                    return defaultvalue;
                }
            } else {
                return defaultvalue;
            }
        }

        public static bool ConvertTo<T>( this object source , ref T result ) {

            Type t = typeof( T );

            try {
                if ( t.IsGenericType && ( t.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {
                    if ( source == null ) {
                        result = default( T );
                    } else {
                        result = (T) Convert.ChangeType( source , Nullable.GetUnderlyingType( t ) );
                    }
                } else {
                    result = (T) Convert.ChangeType( source , t );
                }

                return true;

            } catch ( System.Exception ) {

                return false;
            }


        }


        /*  TypeCode
            Empty = 0,      //Riferimento null.
            Object = 1,     //Tipo generale che rappresenta qualsiasi tipo valore o riferimento non rappresentato in modo esplicito da un altro TypeCode.
            *DBNull = 2,     //Valore (colonna) di database null
            *Boolean = 3,    //Tipo semplice che rappresenta i valori booleani true o false.

            *Char = 4,       //Tipo integrale che rappresenta interi senza segno a 16 bit con valori compresi tra 0 e 65535. Il set di possibili valori per il tipo Char corrisponde al set di caratteri Unicode.

            *SByte = 5,      //Tipo integrale che rappresenta interi con segno a 8 bit compresi tra -128 e 127.
            *Byte = 6,       //Tipo integrale che rappresenta interi a 8 bit senza segno compresi tra 0 e 255.
            *Int16 = 7,      //Tipo integrale che rappresenta interi con segno a 16 bit compresi tra -32768 e 32767.
            *UInt16 = 8,     //Tipo integrale che rappresenta interi senza segno a 16 bit con valori compresi tra 0 e 65535.
            *Int32 = 9,      //Tipo integrale che rappresenta interi con segno a 32 bit compresi tra -2147483648 e 2147483647.
            *UInt32 = 10,    //Tipo integrale che rappresenta interi senza segno a 32 bit compresi tra 0 e 4294967295.
            *Int64 = 11,     //Tipo integrale che rappresenta interi con segno a 64 bit compresi tra -9223372036854775808 e 9223372036854775807.
            *UInt64 = 12,    //Tipo integrale che rappresenta interi senza segno a 64 bit compresi tra 0 e 18446744073709551615.


            *Single = 13,    //Tipo a virgola mobile che rappresenta valori compresi tra 1,5 x 10 -45 e 3,4 x 10 38 con un'approssimazione di 7 cifre.
            *Double = 14,    //Tipo a virgola mobile che rappresenta valori compresi tra 5,0 x 10 -324 e 1,7 x 10 308 con un'approssimazione di 15-16 cifre.
            *Decimal = 15,   //Tipo semplice che rappresenta valori compresi tra 1,0 x 10 -28 e approssimativamente 7,9 x 10 28 con 28-29 cifre significative.

            *DateTime = 16,  //Tipo che rappresenta un valore di data e di ora.
            *String = 18     //Tipo di classe sealed che rappresenta stringhe di caratteri Unicode.

        */

        public static bool IsDBNull<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( code == TypeCode.DBNull );
        }
        public static bool IsDBNull( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( t == 2 );
        }

        public static bool IsBoolean<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( code == TypeCode.Boolean );
        }
        public static bool IsBoolean( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( t == 3 );
        }


        public static bool IsDateTime<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( code == TypeCode.DateTime );
        }
        public static bool IsDateTime( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( t == 16 );
        }

        public static bool IsInteger<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( ( code >= TypeCode.SByte ) && ( code <= TypeCode.UInt64 ) );
        }
        
        public static bool IsInteger( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( ( t >= 5 ) && ( t <= 12 ) );
        }

        public static bool IsFloatingPoint<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( ( code >= TypeCode.Single ) && ( code <= TypeCode.Decimal ) );
        }
        public static bool IsFloatingPoint( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( ( t >= 13 ) && ( t <= 15 ) );
        }

        public static bool IsLitteral<T>() {
            TypeCode code = Type.GetTypeCode( typeof( T ) );
            return ( ( code == TypeCode.Char ) || ( code <= TypeCode.String ) );
        }
        public static bool IsLitteral( this object o ) {
            var t = (byte) Convert.GetTypeCode( o );
            return ( ( t == 4 ) || ( t == 18 ) );
        }


    }
}
