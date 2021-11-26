using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public enum JoinType {
        NONE, INNER, LEFT, RIGHT
    }

	public enum LogicOperator {
		AND,
		OR,
		NOT,
		SEPARATOR
	}

}
