using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Entity.Attributes {
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]

    public sealed class DALTable : Attribute {

        string mTableName;
        public string TableName { get => mTableName; }

        //string mAlias;
        //public string Alias { get => mAlias; }
        public string Charset { get => this.charset; set => this.charset = value; }

        private string charset;
        public DALTable( string aTableName ) {
            this.mTableName = aTableName;
        }


    }

}
