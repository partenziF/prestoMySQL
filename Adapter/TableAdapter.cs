using prestoMySQL.Adapter.Enum;
using prestoMySQL.Adapter.Interface;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Entity.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;

namespace prestoMySQL.Adapter {

    public abstract class TableEntity : IAdaptableTable {

        protected List<DefinableColumn<SQLTypeWrapper<object>>> definitionColumns;

        protected string mSQLQuery = null;

        public string SQLQuery { get => this.mSQLQuery; }

        protected TableEntity() {
        }

        public abstract void InitEntity();
        //protected abstract void CreateNew();
        public abstract U SelectLastInsertId<U>();
        public abstract U SelectSingleValue<U>( string aSqlQuery );
        public abstract object[] SelectSingleRow( string aSqlQuery , EntityConditionalExpression Constraint , params object[] values );


        //public abstract OperationResult DropTable( bool ifExists );
        //public abstract OperationResult CreateTable( bool ifExists );
        //public abstract OperationResult TruncateTable();        
        //protected abstract OperationResult CheckTable();
        //public abstract bool Check();

        public abstract OperationResult Read( EntityConditionalExpression Constraint = null , params Object[] aKeyValues );

        //protected abstract OperationResult Select( EntityConditionalExpression Constraint = null , params Object[] values );
        protected abstract OperationResult Insert( AbstractEntity aEntity );
        protected abstract OperationResult Update( AbstractEntity aEntity );
        protected abstract OperationResult Delete( AbstractEntity aEntity );

        public abstract bool Save();


        public abstract void SetPrimaryKey();
        public abstract void CreatePrimaryKey();
        public abstract void createForeignKey();

        public abstract void CreateUniqueKey();
        public abstract void CreateEvents();


        //public abstract OperationResult DropTable( bool ifExists );
        //public abstract OperationResult CreateTable( bool ifExists );
        //public abstract OperationResult TruncateTable();        
        //protected abstract OperationResult CheckTable();
        //public abstract bool Check();

        //public abstract OperationResult Read( EntityConditionalExpression Constraint = null , params Object[] aKeyValues );
        public abstract OperationResult New(AbstractEntity newEntity = null , bool UpdateForeignKey = true );


    }

}