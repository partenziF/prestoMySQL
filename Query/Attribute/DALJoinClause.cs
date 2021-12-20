using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.Attribute {
    [AttributeUsage( AttributeTargets.Field , AllowMultiple = false , Inherited = false )]
    public class DALJoinClause : System.Attribute {
        internal JoinType Join;
        internal string Table;
        internal string ForeignKey;
        //    internal DALJoinConstraint[] Constraint;

        public DALJoinClause( JoinType Join , String Table , String ForeignKey )
    {
        this.Join = Join;
        this.Table = Table;
        this.ForeignKey = ForeignKey;
        //this.Constraint = Constraint;
    }
}
}
