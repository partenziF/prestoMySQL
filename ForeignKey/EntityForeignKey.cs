using prestoMySQL.Entity;
using prestoMySQL.ForeignKey.Interface;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey {
    public abstract class EntityForeignKey : ForeignKey, IForeignKey {
        public object[] createKey() {
            throw new NotImplementedException();
        }

        public KeyState getKeyState() {
            throw new NotImplementedException();
        }

        public object[] getKeyValues() {
            throw new NotImplementedException();
        }

        public IDictionary<string , EntityPrimaryKey> peekPrimaryKey() {
            throw new NotImplementedException();
        }

        public void setDoCreateForeignKey( DelegateCreateForeignKey doCreateForeignKey ) {
            throw new NotImplementedException();
        }

        public void setKeyValues( params object[] values ) {
            throw new NotImplementedException();
        }

        public abstract void addEntities( List<AbstractEntity> foreignKeyTables );
    }
}
