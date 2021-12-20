using prestoMySQL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class DALJoinConstraint : System.Attribute {
        internal Type Entity;
        internal string Table;
        internal string Column;
        internal string Value;
        internal Type TypeValue;

        //public DALJoinConstraint( String Column , String Value , Type TypeValue , Type Entity = GenericEntity, String Table = "")
        //{
        //    this.Entity = Entity;
        //    this.Table = Table;
        //    this.Column = Column;
        //    this.Value = Value;
        //    this.TypeValue = TypeValue;
        //}
    }


}
