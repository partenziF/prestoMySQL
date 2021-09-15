using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {

	[AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
	public class DALQueryJoinEntityConstraint : System.Attribute {
		public Type Entity { get; set; }
		public string FieldName { get; set; }
		public string ParamName { get; set; }
		public object ParamValue { get; set; }
		public string Placeholder { get; set; }

		public DALQueryJoinEntityConstraint(Type entity,string fieldName) {

			this.Entity = entity;
			this.FieldName = fieldName;			
			this.Placeholder = "@";

		}


	}

	[AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
	public class DALQueryJoinEntity : System.Attribute {
		public Type Entity;
		public string Alias;

	}


	[AttributeUsage( AttributeTargets.Class , AllowMultiple = true , Inherited = false )]
	public class DALQueryJoinEntityUnConstraint : System.Attribute {
		internal Type Entity;
		internal Type JoinTable;

		public DALQueryJoinEntityUnConstraint( Type entity , Type joinTable ) {
			this.Entity = entity;
			this.JoinTable = joinTable;
		}
	}
	//[DALJoinUnConstraint( Entity = typeof( ContiEntity ) , JoinTable = typeof( CustomerOrderEntity )]


}
