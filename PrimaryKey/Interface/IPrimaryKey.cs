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

	public delegate object[] DelegateCreatePrimaryKey();

	public interface IPrimaryKey {

		public Object[] getKeyValues();

		void setKeyValues( params Object[] values );

		public Object[] createKey();

		public void setDoCreatePrimaryKey( DelegateCreatePrimaryKey doCreatePrimaryKey );


	}
}
