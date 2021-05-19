using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {
    public interface ISQLQueryCondition<T> : DefinableConstraint {
        String getColumnName();
        void setColumnName( String aColumnName );

    }

}
