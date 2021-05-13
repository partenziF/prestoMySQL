using prestoMySQL.DataType.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.DataType {
    public class SQLColumnDataType : DefinableDataType {

        private MySQLDataType mSqlType;


        public MySQLDataType SqlType { get => this.mSqlType; set => this.mSqlType = value; }

        public SQLColumnDataType( MySQLDataType aSQLType ) {
            this.mSqlType = aSQLType;
        }

        public SQLColumnDataType( String aType ) {

            int state = 0;
            int length = 0;
            int start = 0;
            String result = "";
            String size = "";

            for ( int i = 0; i < aType.Length; i++ ) {
                switch ( aType.ToLower()[i] ) {
                    case '(':
                    state = ( state == 1 ) ? 2 : -1;
                    break;
                    case ')':
                    state = ( state == 10 ) ? 3 : -2;
                    break;
                    case ',':
                    state = ( state == 10 ) ? 4 : -3;
                    break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    state = ( ( state == 2 ) || ( state == 4 ) || ( state == 10 ) ) ? 10 : -4;
                    break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    state = ( ( state == 0 ) || ( state == 1 ) ) ? 1 : -5;
                    start = ( state == 0 ) ? i : start;
                    break;

                }
                if ( state == 1 ) {
                    length++;
                } else if ( state == 2 ) {
                    result = aType.Substring( start , start + length );
                    start = i + 1;
                    length = 0;
                } else if ( state == 3 ) {
                    size = aType.Substring( start , start + length );
                    start = i;
                    length = 0;
                } else if ( state == 4 ) {
                    size = aType.Substring( start , start + length );
                    start = i;
                    length = 0;
                } else if ( state == 10 ) {
                    length++;
                } else if ( state < 0 ) {
                    throw new System.Exception( String.Format( "error code {0} '{1}'" , state , aType ) );
                }

            }

            if ( ( state == 1 ) && String.IsNullOrWhiteSpace( result ) ) {
                result = aType.Substring( start , start + length );
            }

            if ( ( state == 3 ) || ( state == 1 ) ) {
                
                if ( result.Equals( "INTEGER" ,StringComparison.OrdinalIgnoreCase) ) {
                    this.SqlType = MySQLDataType.dbtInteger ;
                } else if ( result.Equals( "NVARCHAR" , StringComparison.OrdinalIgnoreCase ) ) {
                    this.SqlType = MySQLDataType.dbtText ;
                } else if ( result.Equals( "NUMERIC" , StringComparison.OrdinalIgnoreCase ) ) {
                    this.SqlType = MySQLDataType.dbtNumeric ;
                } else {
                    throw new System.Exception( "invali type name" );
                }
            }

        }


        public override String ToString() {

            switch ( mSqlType ) {


                case MySQLDataType.dbtInteger: return "INTEGER";
                case MySQLDataType.dbtText: return "TEXT";
                case MySQLDataType.dbtBlob: return "BLOB";
                case MySQLDataType.dbtReal: return "REAL";
                case MySQLDataType.dbtNumeric: return "NUMERIC";

            }
            return "";

        }

        public String toLanguageType() {

            switch ( mSqlType ) {

                case MySQLDataType.dbtInteger: return "int";
                case MySQLDataType.dbtText: return "string";
                case MySQLDataType.dbtBlob: return "byte[]";
                case MySQLDataType.dbtReal: return "double";
                case MySQLDataType.dbtNumeric: return "float";

            }
            return "";

        }

        public String toLanguageCode() {
            switch ( mSqlType ) {

                case MySQLDataType.dbtInteger: return "MySQLDataType.dbtInteger";
                case MySQLDataType.dbtText: return "MySQLDataType.dbtText";
                case MySQLDataType.dbtBlob: return "MySQLDataType.dbtBlob";
                case MySQLDataType.dbtReal: return "MySQLDataType.dbtReal";
                case MySQLDataType.dbtNumeric: return "MySQLDataType.dbtNumeric";
            }

            return "";

        }

    }

}
