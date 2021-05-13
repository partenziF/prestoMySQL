using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
	[AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
	public class DALQueryJoinEntity : System.Attribute {
		public Type Entity;
		public string Alias;

	}
}
