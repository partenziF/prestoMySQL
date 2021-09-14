using prestoMySQL.Database.Interface;
using prestoMySQL.Database.MySQL;
using prestoMySQL.Query;

namespace prestoMySQL.Adapter {
    public interface IQueryAdapter {
        int? Offset { get; set; }
        int? RowCount { get; set; }
        int SQLCount { get; }
        MySQResultSet ExecuteQuery( out ILastErrorInfo lastErrorInfo );

    }

}