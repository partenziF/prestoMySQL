using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Interface {
    public interface DefinableConditionalExpression {
		
		public QueryParam[] getParam();

		public string[] getParamAsString();

		public int countParam();

	}
}
