using prestoMySQL.Column.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

	public enum OrderType {
		ASC, DESC
	}

	[AttributeUsage( AttributeTargets.Property, AllowMultiple = false , Inherited = false )]
	public class DALProjectionFunction : System.Attribute {

		public string Function { get; set; }
		//public DALFunctionParam[] Params;       

		public string Alias { get; set; }
		public MySQLDataType DataType { get; set; }
		public OrderType Sort { get; set; }
		public int OrderBy { get; set; }


		public DALProjectionFunction(String Function, String Alias, MySQLDataType Type)
		{
			this.Function = Function;
			//this.Params = Params;
			this.Alias = Alias;
			this.DataType = Type;
		}
}
}
