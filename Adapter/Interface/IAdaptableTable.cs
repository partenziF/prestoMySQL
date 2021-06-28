using Microsoft.Extensions.Logging;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Entity;
using prestoMySQL.Query.SQL;
using PrestoMySQL.Database.MySQL;
using System;

namespace prestoMySQL.Adapter.Interface {

    public interface IAdaptableTable {

        //AbstractEntity CreateEntity();
        void InitEntity();

        OperationResult New();
        OperationResult Read( EntityConditionalExpression Constraint = null , params object[] aKeyValues );

        bool Save();

        U SelectSingleValue<U>( string aSqlQuery );

        U SelectLastInsertId<U>();

        //Object[] SelectSingleRow( String aSqlQuery );
        object[] SelectSingleRow( string aSqlQuery , EntityConditionalExpression Constraint , params object[] values );

        public static OperationResult DropTable( bool ifExists , MySQLDatabase mDatabase , ILogger mLogger = null ) => throw new NotImplementedException();
        public static OperationResult CheckTable( MySQLDatabase mDatabase , ILogger mLogger = null ) => throw new NotImplementedException();
        public static OperationResult CreateTable( bool ifExists , MySQLDatabase mDatabase , ILogger mLogger = null ) => throw new NotImplementedException();
        public static OperationResult ExistsTable( MySQLDatabase mDatabase , ILogger mLogger = null ) => throw new NotImplementedException();

        public static bool Check( MySQLDatabase mDatabase , ILogger mLogger = null ) => throw new NotImplementedException();

        void SetPrimaryKey();
        void CreatePrimaryKey();
        void createForeignKey();
        void CreateEvents();

    }

}
