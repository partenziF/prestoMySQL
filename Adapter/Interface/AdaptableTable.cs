using prestoMySQL.Adapter.Enum;
using prestoMySQL.Entity;
using prestoMySQL.Query.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Adapter.Interface {

    public interface AdaptableTable {

        AbstractEntity CreateEntity();

        OperationResult Create( EntityConditionalExpression Constraint = null , params object[] aKeyValues );

        bool Save();

        U SelectSingleValue<U>( string aSqlQuery );

        U SelectLastInsertId<U>();

        //Object[] SelectSingleRow( String aSqlQuery );
        object[] SelectSingleRow( string aSqlQuery , EntityConditionalExpression Constraint , params object[] values );

        OperationResult DropTable( bool ifExists );

        OperationResult CreateTable( bool ifExists );

        void SetPrimaryKey( params Object[] values );

        Object[] CreatePrimaryKey();

        public interface Creator<T> {
            public T createEntity();
        }

        void createForeignKey();
        void CreateEvents();
    }
}
