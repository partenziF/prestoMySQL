using prestoMySQL.Adapter.Enum;
using prestoMySQL.Adapter.Interface;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;

namespace prestoMySQL.Adapter {

    public abstract class TableAdapter : AdaptableTable {

        protected List<DefinableColumn<SQLTypeWrapper<object>>> definitionColumns;

        protected TableAdapter() {
        }

        public abstract AbstractEntity CreateEntity();
        protected abstract void CreateNew();
        public abstract U SelectLastInsertId<U>();
        public abstract U SelectSingleValue<U>( string aSqlQuery );
        public abstract object[] SelectSingleRow( string aSqlQuery , EntityConditionalExpression Constraint , params object[] values );


        public abstract OperationResult DropTable( bool ifExists );
        public abstract OperationResult CreateTable( bool ifExists );

        public abstract OperationResult TruncateTable();
        public abstract OperationResult ExistsTable();
        protected abstract OperationResult CheckTable();

        public abstract OperationResult Create( EntityConditionalExpression Constraint = null , params Object[] aKeyValues );
        protected abstract OperationResult Select( EntityConditionalExpression Constraint = null , params Object[] values );
        protected abstract OperationResult Insert();
        protected abstract OperationResult Update();

        public abstract bool Save();
        public abstract bool Check();


        public abstract void SetPrimaryKey( params object[] values );
        public abstract object[] CreatePrimaryKey();
        public abstract void createForeignKey();
        public abstract void CreateEvents();
    }

}