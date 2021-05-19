using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.SQL.Interface {
    public interface ISQLTypeWrapper {
        
        bool IsNull { get; }
        object ToObject();
        bool IsInteger(out TypeCode code);
        bool IsBoolean();
        bool IsDateTime();
        bool IsFloatingPoint( out TypeCode code );
        bool IsLitteral( out TypeCode code );


    }}
