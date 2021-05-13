using prestoMySQL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Adapter.Interface {
    public interface AdaptableTable {

        GenericEntity CreateEntity();

        bool Create( params object[] aKeyValues );

        bool Save();

        U SelectSingleValue<U>( string aSqlQuery );

        U SelectLastInsertId<U>();

		Object[] SelectSingleRow( String aSqlQuery );

        bool DropTable( bool ifExists );

        bool CreateTable( bool ifExists );

        void SetPrimaryKey( params Object[] values );

        Object[] CreatePrimaryKey();

        public interface Creator<T> {
            public T createEntity();
        }

        void createForeignKey();


    }
}
