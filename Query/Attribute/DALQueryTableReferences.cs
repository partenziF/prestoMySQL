using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
	[AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
	public class DALQueryTableReferences : System.Attribute {
		public string Table { get; }
		public string Alias { get; set; }

		public DALQueryTableReferences(String Table)
		{
			this.Table = Table;
			this.Alias = "";
		}

}
}
