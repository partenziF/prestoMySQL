﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLQueryGroupBy {
        public int order;
        String column;

        public SQLQueryGroupBy( int order , string column ) {
            this.order = order;
            this.column = column;
        }

        public override string ToString() {
            return column;
        }
    }
}
