using prestoMySQL.Column.Attribute;
using System;

namespace DatabaseEntity.EntityAdapter {

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public class DDUniqueIndexAttribute : System.Attribute {

        public string Name;

        public IndexType? IndexType = null;


    }
}