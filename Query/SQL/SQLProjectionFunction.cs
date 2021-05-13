using prestoMySQL.Column;
using prestoMySQL.Query.Attribute;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {
    public class SQLProjectionFunction<T> : MySQLColumn<T> where T : ISQLTypeWrapper {
        public SQLProjectionFunction( string aDeclaredVariableName , PropertyInfo aMethodBase = null ) : base( aDeclaredVariableName , aMethodBase ) {

			//// TODO Auto-generated constructor stub			
			//if ( f == null ) {
			//	throw new Exception( "Field is null" );
			//}

			//tables.clear();

			//if ( f.isAnnotationPresent( typeof( DALProjectionFunction ) ) ) {

			//	DALProjectionFunction a = f.getAnnotation( typeof( DALProjectionFunction ) );

			//	if ( a.Name() != "" ) {
			//		this.mName = a.Name();
			//	} else {
			//		throw new Exception( "Function name not present" );
			//	}

			//	if ( a.Params() != null ) {
			//		this.mParams = a.Params().clone();

			//		foreach ( TableReference t in SQLTableEntityHelper.getQueryTableName( aClazz ) ) {
			//			tables.put( t.getTableName() , t );
			//		}
			//		foreach ( TableReference t in SQLTableEntityHelper.getQueryJoinTableName( aClazz ) ) {
			//			tables.put( t.getTableName() , t );
			//		}

			//		if ( tables.isEmpty() ) {
			//			throw new Exception( "No tables found" );
			//		}
			//		bool found = false;
			//		foreach ( DALFunctionParam p in this.mParams ) {
			//			if ( !p.Table().isEmpty() ) {
			//				if ( tables.containsKey( p.Table() ) ) {
			//					found = true;
			//					break;
			//				}
			//			}
			//		}
			//		if ( !found ) {
			//			throw new Exception( "No table found" );
			//		}



			//	} else {
			//		this.mParams = null;
			//	}


			//	if ( a.Type() != null ) {
			//		this.sqliteDataType = new SQLColumnDataType( a.Type() );
			//	} else {
			//		throw new Exception( "Column type not present" );
			//	};

			//	if ( a.Alias() != "" ) {
			//		this.mAlias = a.Alias();
			//	} else {
			//		throw new Exception( "Column alias not present" );
			//	}


			//}


		}

		protected String mName;
        protected DALFunctionParam[] mParams;
        protected String mAlias;
        private IDictionary<string , TableReference> tables = new Dictionary<string , TableReference>();



        public override string ToString() {
			//string result = "";
			//if ( this.mParams != null ) {

			//	string[] Params = new string[this.mParams.length];
			//	int i = 0;
			//	foreach ( DALFunctionParam p in this.mParams ) {

			//		if ( p.Value().Equals( "" ) ) {
			//			string sTable = p.Table();
			//			TableReference table = tables.get( sTable );
			//			if ( table != null ) {
			//				if ( table.getTableAlias().isEmpty() ) {
			//					Params[i++] = string.Format( "{0}.{1}" , table.getTableName() , p.Column() );
			//				} else {
			//					Params[i++] = string.Format( "{0}.{1}" , table.getTableAlias() , p.Column() );
			//				}
			//			} else {
			//				//throw new Exception("Invialid function param");						  
			//			}
			//		} else if ( p.Column().Equals( "" ) && ( !p.Value().Equals( "" ) ) ) {
			//			Params[i++] = string.Format( "{0}" , p.Value() );
			//		} else {
			//			//throw new Exception("Invialid function param");
			//		}

			//	}

			//	result = string.Format( "{0}( {1} ) {2}" , this.mName , string.join( "," , Params ) , " AS " + this.mAlias );

			//} else {
			//	result = string.Format( "{0}( {1} ) {2}" , this.mName , " AS " + this.mAlias );
			//}

			////

			//return result;

			throw new NotImplementedException();

		}

		public override object ValueAsParamType() {
            return base.ValueAsParamType();
        }
    }
}
