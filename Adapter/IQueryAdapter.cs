using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;

namespace prestoMySQL.Adapter {
    public interface IQueryAdapter {
        int? Offset { get; set; }
        int? RowCount { get; set; }
        int SQLCount { get; }

        MySQResultSet ExecuteQuery( out ILastErrorInfo lastErrorInfo );

    }

}