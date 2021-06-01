using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Table {
    public class TableReference {

        public string TableName { get; set; }
        public string TableAlias { get; set; }

        public TableReference( string aTableName , String aTableAlias = null ) {
            TableName = aTableName;
            TableAlias = aTableAlias;
        }

        public String getResultColumn( String aColumnName , String aColumnAlias ) {
            if ( string.IsNullOrWhiteSpace( TableAlias ) ) {
                if ( !string.IsNullOrWhiteSpace( aColumnAlias ) ) {                    
                    return string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER+" AS "+ Constant.COLUMN_NAME_QUALIFIER+"{2}" + Constant.COLUMN_NAME_QUALIFIER , this.TableName , aColumnName , aColumnAlias );
                } else {
                    return string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER , this.TableName , aColumnName );
                }
            } else {
                if ( !string.IsNullOrWhiteSpace( aColumnAlias ) ) {
                    return string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER+" AS "+Constant.COLUMN_NAME_QUALIFIER+"{2}" + Constant.COLUMN_NAME_QUALIFIER , this.TableAlias , aColumnName , aColumnAlias );
                } else {
                    return string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER , this.TableAlias , aColumnName );
                }
            }
        }

        public String getColumnName( String aColumnName ) => string.IsNullOrWhiteSpace( TableAlias )
                ? string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER , this.TableName , aColumnName )
                : string.Format( "{0}."+ Constant.COLUMN_NAME_QUALIFIER+"{1}"+ Constant.COLUMN_NAME_QUALIFIER , this.TableAlias , aColumnName );

        public String getActualName() => string.IsNullOrWhiteSpace( TableAlias ) ? TableName : TableAlias;

        public override string ToString() => string.IsNullOrWhiteSpace( TableAlias ) ? TableName : string.Format( "{0} AS {1}" , TableName , TableAlias );

    }

}
