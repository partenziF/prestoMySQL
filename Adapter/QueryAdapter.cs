using MySqlConnector;
using prestoMySQL.Column.Interface;
using prestoMySQL.Database.Cursor;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Adapter {
    public abstract class QueryAdapter : CollectionBase {

        private int? mRowCount = null;
        private int? mOffset = null;

        public int? RowCount { get => mRowCount; set => mRowCount = value; }
        public int? Offset { get => mOffset; set => mOffset = value; }


        public abstract new int Count { get; }



        protected CursorWrapper<MySQResultSet , MySqlDataReader> mCursor;
        public abstract CursorWrapper<MySQResultSet , MySqlDataReader> Cursor { get; set; }

        public List<dynamic> projectionColumns = null; //QueryableColumn<object>

        //public abstract ReadableResultSet<MySqlDataReader> select();
        public abstract MySQResultSet ExecuteQuery();

        public abstract void PrepareQuery( );


        //public abstract ReadableResultSet<MySqlDataReader> select( IQueryParams myParams );
        //public abstract MySQResultSet Select( IQueryParams myParams );

        public abstract void BindData( MySQResultSet resultSet );


    }


}
