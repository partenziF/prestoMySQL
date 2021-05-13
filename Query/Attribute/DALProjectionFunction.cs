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

	[AttributeUsage( AttributeTargets.Field , AllowMultiple = false , Inherited = false )]
	public class DALProjectionFunction : System.Attribute {

		public string Name;
		public DALFunctionParam[] Params;
		public string Alias;
		public MySQLDataType Type;
		public OrderType Sort;
		public int OrderBy;


		public DALProjectionFunction(String Name, DALFunctionParam[] Params, String Alias, MySQLDataType Type, OrderType Sort = OrderType.DESC, OrderType OrderBy = OrderType.ASC)
		{
			this.Name = Name;
			this.Params = Params;
			this.Alias = Alias;
			this.Type = Type;
			this.Sort = Sort;
			this.OrderBy = ( int ) OrderBy;
		}
}
}
