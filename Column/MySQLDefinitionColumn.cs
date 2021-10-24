using MySqlConnector;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Query;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace prestoMySQL.Column {

    public class MySQLDefinitionColumn<T> : SQLColumn<T> where T : ISQLTypeWrapper { //where T : struct 

        public MySQLDefinitionColumn( string aDeclaredVariableName, PropertyInfo aMethodBase , AbstractEntity abstractEntity ) : base( aDeclaredVariableName , aMethodBase ) {
            
            MethodInfo m1 = typeof( AbstractEntity ).GetMethod( "Entity_PropertyChanged" , BindingFlags.NonPublic | BindingFlags.Instance );
            Delegate dlg = Delegate.CreateDelegate( typeof( PropertyChangedEventHandler ) , abstractEntity , m1 );

            EventInfo evClick = this.GetType().GetEvent( "PropertyChanged" );
            MethodInfo addHandler = evClick.GetAddMethod();
            Object[] addHandlerArgs = { dlg };
            addHandler.Invoke( this , addHandlerArgs );

        }

        public uint ResultSetIndex { get; set; }

        public override string ToString() {
            return Table.getColumnName( this.ColumnName );
        }

        public static implicit operator QueryParam( MySQLDefinitionColumn<T> a ) {

            return new MySQLQueryParam( ( ISQLTypeWrapper ) a.ValueAsParamType() , a.ColumnName );

        }

        //https://www.tutorialspoint.com/What-is-the-Chash-Equivalent-of-SQL-Server-DataTypes
        public static implicit operator MySQLQueryParam( MySQLDefinitionColumn<T> a ) {

            object value;


            if ( a.TypeWrapperValue?.IsNull ?? true ) {

                return new MySQLQueryParam( null , a.ColumnName );

            } else {


                switch ( a.SQLDataType ) {

                    //Type      ( Bytes)    Signed                  Maximum Unsigned
                    //TINYINT       1       - 128           127             255
                    //SMALLINT      2       - 32768         32767           65535
                    //MEDIUMINT     3       - 8388608       8388607         16777215
                    //INT           4       - 2147483648    2147483647      4294967295
                    //BIGINT        8       - 2^63          2^63 - 1        2^64 - 1

                    //Integer Types (Exact Value)
                    case MySQLDataType.dbtTinyInt:
                    value = Convert.ChangeType( a , a.Signed == SignType.Signed ? typeof( sbyte ) : typeof( byte ) );
                    break;
                    case MySQLDataType.dbtSmallInt:
                    value = Convert.ChangeType( a , a.Signed == SignType.Signed ? typeof( short ) : typeof( ushort ) );
                    break;
                    case MySQLDataType.dbtMediumInt:
                    value = Convert.ChangeType( a , a.Signed == SignType.Signed ? typeof( int ) : typeof( uint ) );
                    break;
                    case MySQLDataType.dbtInteger:
                    value = Convert.ChangeType( a , a.Signed == SignType.Signed ? typeof( int ) : typeof( uint ) );
                    break;
                    case MySQLDataType.dbtBigInt:
                    value = Convert.ChangeType( a , a.Signed == SignType.Signed ? typeof( long ) : typeof( ulong ) );
                    break;


                    //C# Alias	.NET Type	    Size	    Precision	                Range
                    //float     System.Single   4 bytes     7 digits                    + -1.5 x 10^- 45 to + -3.4 x 10^38
                    //double    System.Double   8 bytes     15 - 16 digits              + -5.0 x 10^- 324 to + -1.7 x 10^308
                    //decimal   System.Decimal  16 bytes    28 - 29 decimal places      +-1.0 x 10^-28 to + -7.9 x 10^28

                    //Fixed-Point Types (Exact Value) 
                    case MySQLDataType.dbtDecimal:
                    //A fixed precision and scale numeric value between -1038 -1 and 10 38 -1.
                    value = Convert.ChangeType( a , typeof( decimal ) );
                    break;
                    case MySQLDataType.dbtNumeric:
                    value = Convert.ChangeType( a , typeof( decimal ) );
                    break;

                    //Floating-Point Types (Approximate Value) 
                    case MySQLDataType.dbtFloat:
                    //A small (single-precision) floating-point number. Allowable values are -3.402823466E+38 to -1.175494351E-38, 0, and 1.175494351E-38 to 3.402823466E+38.
                    value = Convert.ChangeType( a , typeof( float ) );
                    break;
                    case MySQLDataType.dbtReal:
                    value = Convert.ChangeType( a , typeof( float ) );
                    break;
                    case MySQLDataType.dbtDoublePrecision:
                    //A normal-size (double-precision) floating-point number. Allowable values are -1.7976931348623157E+308 to -2.2250738585072014E-308, 0, and 2.2250738585072014E-308 to 1.7976931348623157E+308.
                    value = Convert.ChangeType( a , typeof( double ) );
                    break;

                    case MySQLDataType.dbtBit:
                    value = Convert.ChangeType( a , typeof( byte[] ) );
                    break;//Bit-Value Type

                    case MySQLDataType.dbtDate:
                    value = Convert.ChangeType( a , typeof( DateTime ) );
                    break;
                    case MySQLDataType.dbtTime:
                    value = Convert.ChangeType( a , typeof( DateTime ) );
                    break;
                    case MySQLDataType.dbtDateTime:
                    value = Convert.ChangeType( a , typeof( DateTime ) );
                    break;
                    case MySQLDataType.dbtTimestamp:
                    value = Convert.ChangeType( a , typeof( long ) );
                    break;
                    case MySQLDataType.dbtYear:
                    value = Convert.ChangeType( a , typeof( sbyte ) );
                    break; //Datetime


                    //The string data types 
                    case MySQLDataType.dbtChar:
                    value = Convert.ChangeType( a , typeof( char ) );
                    break;
                    case MySQLDataType.dbtVarChar:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtBinary:
                    value = Convert.ChangeType( a , typeof( char ) );
                    break;
                    case MySQLDataType.dbtVarBinary:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break; //they store byte strings rather than character strings.

                    case MySQLDataType.dbtTinyBlob:
                    value = Convert.ChangeType( a , typeof( byte[] ) );
                    break;
                    case MySQLDataType.dbtBlob:
                    value = Convert.ChangeType( a , typeof( byte[] ) );
                    break;
                    case MySQLDataType.dbtMediumBlob:
                    value = Convert.ChangeType( a , typeof( byte[] ) );
                    break;
                    case MySQLDataType.dbtLongBlob:
                    value = Convert.ChangeType( a , typeof( byte[] ) );
                    break;

                    case MySQLDataType.dbtTinyText:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtText:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtMediumText:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtLongText:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtEnum:
                    value = Convert.ChangeType( a , typeof( string ) );
                    break;
                    case MySQLDataType.dbtSet:
                    throw new NotImplementedException();
                    //break;//A SET column can have a maximum of 64 distinct members.
                    //11.4.1 Spatial Data Types : GEOMETRY, POINT, LINESTRING, POLYGON,MULTIPOINT,MULTILINESTRING,MULTIPOLYGON,GEOMETRYCOLLECTION
                    case MySQLDataType.dbtJSON:
                    throw new NotImplementedException();
                    default:
                    throw new NotImplementedException();
                }

                return new MySQLQueryParam( value , a.ColumnName );

            }

        }

        public MySQLDefinitionColumn<T> Copy() {

            return ( MySQLDefinitionColumn<T> ) base.Copy();
        }




    }

}