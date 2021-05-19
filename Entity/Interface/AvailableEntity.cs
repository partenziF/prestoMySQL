using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Entity.Interface {

    public enum EntityState {
        Undefined,
        Created,
        Set,
        Changed
    }

    public interface AvailableEntity {

        EntityState State { get; set; }
        string TableName { get; }
        EntityPrimaryKey createPrimaryKey();
        AbstractEntity setCreatePrimaryKey( DelegateCreatePrimaryKey delegatorCreatePrimaryKey );
        void setPrimaryKey( EntityPrimaryKey aPrimaryKey , DelegateCreatePrimaryKey aDelegatorCreatePrimaryKey );
    }
}
