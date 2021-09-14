using System;

namespace prestoMySQL.Query.Attribute {

    public class DALQueryParamAttribute : System.Attribute {
        public string Name;
    }

    [AttributeUsage( AttributeTargets.All )]
    public class DALJoinTargetAttribute : System.Attribute {
        public string Name;
    }

}