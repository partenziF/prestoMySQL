using prestoMySQL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.PrimaryKey {
    public abstract class EntityPrimaryKey : PrimaryKey, IPrimaryKey {

        internal readonly int mKeyLengh;
        private DelegateCreatePrimaryKey delegatorCreatePrimaryKey = null;


        // Constructor ///////////////////////////////
        public EntityPrimaryKey( GenericEntity aTableEntity ) : base( KeyState.Unset ) {

            this.Table = aTableEntity;
            initialize( aTableEntity.GetType() );
            mKeyLengh = primaryKeyColumns.Count;
        }


		public virtual void initialize( Type aTable ) {

			//try {
			//	IList<System.Reflection.FieldInfo> l = SQLTableEntityHelper.getPrimaryKeyFields( aTable );
			//	foreach ( System.Reflection.FieldInfo f in l ) {
			//		f.setAccessible( true );
			//		DDPrimaryKey a = f.getAnnotation( typeof( DDPrimaryKey ) );
			//		DDColumn aa = f.getAnnotation( typeof( DDColumn ) );
			//		if ( ( a != null ) && ( aa != null ) ) {

			//			//					DefinitionColumn<?> o = null;
			//			//					o = (DefinitionColumn<?>) f.get(this.table);

			//			this.primaryKeyColumns[aa.Name()] = f;
			//			this.autoincrement( a.Autoincrement() );
			//		}
			//	}
			//} catch ( Exception e ) {
			//	Console.WriteLine( e.ToString() );
			//	Console.Write( e.StackTrace );
			//}

		}

        public KeyState getKeyState() {
            throw new NotImplementedException();
        }

        public object[] getKeyValues() {
            throw new NotImplementedException();
        }

        public void setKeyValues( params object[] values ) {
            throw new NotImplementedException();
        }

        public object[] createKey() {
            throw new NotImplementedException();
        }

        public void setDoCreatePrimaryKey( DelegateCreatePrimaryKey doCreatePrimaryKey ) {
            throw new NotImplementedException();
        }

        public virtual string[] ColumnsName {
			get {
				return this.primaryKeyColumns.Keys.ToArray();
			}
		}




	}
}
