using prestoMySQL.Adapter.Interface;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;

namespace prestoMySQL.Adapter {
    public abstract class TableAdapter : AdaptableTable {

        protected List<DefinableColumn<SQLTypeWrapper<object>>> definitionColumns;

        protected TableAdapter() {
        }

        public abstract GenericEntity CreateEntity();
        protected abstract void CreateNew();
        public abstract bool Create( params Object[] aKeyValues );
        protected abstract bool Select( params Object[] values );
        protected abstract bool Insert();
        protected abstract bool Update();
        public abstract bool Save();
        public abstract U SelectLastInsertId<U>();
        public abstract U SelectSingleValue<U>( string aSqlQuery );
        public abstract object[] SelectSingleRow( string aSqlQuery );
        public abstract bool DropTable( bool ifExists );
        public abstract bool CreateTable( bool ifExists );
        public abstract void SetPrimaryKey( params object[] values );
        public abstract object[] CreatePrimaryKey();
        public abstract void createForeignKey();

    }

}