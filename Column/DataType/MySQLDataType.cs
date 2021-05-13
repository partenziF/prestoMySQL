using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column.DataType {
    //
    //INT is a synonym for INTEGER,
    //DEC and FIXED are synonyms for DECIMAL
    //DOUBLE as a synonym for DOUBLE PRECISION (a nonstandard extension)
    //REAL as a synonym for DOUBLE PRECISION (a nonstandard variation), unless the REAL_AS_FLOAT SQL mode is enabled.
    //The BIT data type stores bit values 
    // BOOL. BOOLEAN These types are synonyms for TINYINT(1). A value of zero is considered false. Nonzero values are considered true:
    //When used in conjunction with the optional (nonstandard) ZEROFILL attribute, the default padding of spaces is replaced with zeros. 

    //Negative values for AUTO_INCREMENT columns are not supported.
    //Integer or floating-point data types can have the AUTO_INCREMENT attribute.When you insert a value of NULL into an indexed AUTO_INCREMENT column, the column is set to the next sequence value. Typically this is value+1, where value is the largest value for the column currently in the table. (AUTO_INCREMENT sequences begin with 1.)
    //Storing 0 into an AUTO_INCREMENT column has the same effect as storing NULL, unless the NO_AUTO_VALUE_ON_ZERO SQL mode is enabled.


    public enum MySQLDataType { 
        dbtTinyInt, dbtSmallInt, dbtMediumInt, dbtInteger, dbtBigInt, //Integer Types (Exact Value)
        dbtDecimal, dbtNumeric, //Fixed-Point Types (Exact Value) 
        dbtFloat, dbtReal, dbtDoublePrecision,  //Floating-Point Types (Approximate Value) 
        dbtBit,//Bit-Value Type
        dbtDate, dbtTime, dbtDateTime, dbtTimestamp, dbtYear, //Datetime
        //The string data types 
        dbtChar, dbtVarChar,
        dbtBinary, dbtVarBinary, //they store byte strings rather than character strings.
        dbtTinyBlob, dbtBlob, dbtMediumBlob, dbtLongBlob,
        dbtTinyText, dbtText, dbtMediumText, dbtLongText,
        dbtEnum, 
        dbtSet,//A SET column can have a maximum of 64 distinct members.
               //11.4.1 Spatial Data Types : GEOMETRY, POINT, LINESTRING, POLYGON,MULTIPOINT,MULTILINESTRING,MULTIPOLYGON,GEOMETRYCOLLECTION
        dbtJSON
    }
}
