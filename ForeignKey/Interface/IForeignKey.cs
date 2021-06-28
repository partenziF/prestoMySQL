using prestoMySQL.Entity;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey.Interface {

	public delegate void DelegateCreateForeignKey(EntityForeignKey e);

	public interface IForeignKey {

		public KeyState keyState { get; set; }
		
		public Object[] getKeyValues();

		void setKeyValues( params Object[] values );

		public void createKey();

		public void setDoCreateForeignKey( DelegateCreateForeignKey doCreateForeignKey );

		//Dictionary<String , EntityPrimaryKey> peekPrimaryKey();

	}
}
