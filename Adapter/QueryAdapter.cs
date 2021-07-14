using MySqlConnector;
using prestoMySQL.Column.Interface;
using prestoMySQL.Database.Cursor;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Adapter {
    public abstract class QueryAdapter : IQueryAdapter {

        private int? mRowCount = null;
        private int? mOffset = null;

        public int? RowCount { get => mRowCount; set => mRowCount = value; }
        public int? Offset { get => mOffset; set => mOffset = value; }

        public abstract int SQLCount { get; }

        protected CursorWrapper<MySQResultSet , MySqlDataReader> mCursor;
        protected abstract CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get; set; }

        public List<dynamic> ProjectionColumns = null; //QueryableColumn<object>

        public abstract MySQResultSet ExecuteQuery( out ILastErrorInfo Message );

        protected abstract void BindData( Dictionary<string , Dictionary<string , int>> s,MySQResultSet resultSet );


    }


}
