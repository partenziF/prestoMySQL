using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
	[AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
	public class DALFunctionParam : System.Attribute {
		internal string Table;
		internal string Column;
		internal string Value;

		public DALFunctionParam( String Table , String Column , String Value = "" ) {
			this.Table = Table;
			this.Column = Column;
			this.Value = Value;
		}
	}
}
