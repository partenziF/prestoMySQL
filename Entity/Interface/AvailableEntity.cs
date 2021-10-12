using prestoMySQL.ForeignKey;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Entity.Interface {

    public enum EntityState {
        Undefined,
        Created,
        Set,
        Changed,
        Deleted
    }

    public interface AvailableEntity {

        EntityState State { get; set; }
        string TableName { get; }

        string AliasName { get; }

        string ActualName { get; }
        EntityPrimaryKey createPrimaryKey();
        //List<EntityForeignKey> createForeignKey();
        void createForeignKey();

        EntityUniqueIndex createUniqueIndex( [CallerMemberName] string memberName = "",params object[] values );
        AbstractEntity setCreatePrimaryKey( DelegateCreatePrimaryKey delegatorCreatePrimaryKey );
        void setPrimaryKey( EntityPrimaryKey aPrimaryKey , DelegateCreatePrimaryKey aDelegatorCreatePrimaryKey );
    }
}
