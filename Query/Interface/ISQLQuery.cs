﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {
    public interface ISQLQuery {

        public String getSQLQuery();
        public int execute();

    }
}
