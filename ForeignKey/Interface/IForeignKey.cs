using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey.Interface {

	public delegate object[] DelegateCreateForeignKey();

	public interface IForeignKey {

		public KeyState getKeyState();

		public Object[] getKeyValues();

		void setKeyValues( params Object[] values );

		public Object[] createKey();

		public void setDoCreateForeignKey( DelegateCreateForeignKey doCreateForeignKey );

		IDictionary<String , EntityPrimaryKey> peekPrimaryKey();

	}
}
