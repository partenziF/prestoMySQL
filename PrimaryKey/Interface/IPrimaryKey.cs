using prestoMySQL.Entity;
using prestoMySQL.ForeignKey;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL {

	public enum KeyState {
		Created,
		Set,
		Unset
	}

	public delegate void DelegateCreatePrimaryKey( AbstractEntity e );

	public interface IPrimaryKey {

		public Object[] getKeyValues();

		void AssignValues( params object[] values );

		public void createKey( dynamic value = null );

		public void setDoCreatePrimaryKey( DelegateCreatePrimaryKey doCreatePrimaryKey );


	}
}
