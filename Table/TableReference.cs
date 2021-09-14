using prestoMySQL.Entity;
using prestoMySQL.Extension;
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

        public TableReference( AbstractEntity entityInstance ) {
            TableName = entityInstance.TableName;
            TableAlias = entityInstance.AliasName;
        }

        public String getResultColumn( String aColumnName , String aColumnAlias ) {
            if ( string.IsNullOrWhiteSpace( TableAlias ) ) {
                if ( !string.IsNullOrWhiteSpace( aColumnAlias ) ) {
                    return string.Format( "{0}.{1} AS {2}" , this.TableName.QuoteTableName() , aColumnName.QuoteColumnName() , aColumnAlias.QuoteColumnName() );
                } else {
                    return string.Format( "{0}.{1}" , this.TableName.QuoteTableName() , aColumnName.QuoteColumnName() );
                }
            } else {
                if ( !string.IsNullOrWhiteSpace( aColumnAlias ) ) {
                    return string.Format( "{0}.{1} AS {2}" , this.TableAlias.QuoteTableName() , aColumnName.QuoteColumnName() , aColumnAlias.QuoteColumnName() );
                } else {
                    return string.Format( "{0}.{1}" , this.TableAlias.QuoteTableName() , aColumnName.QuoteColumnName() );
                }
            }
        }

        public String getColumnName( String aColumnName ) => string.IsNullOrWhiteSpace( TableAlias )
                ? string.Format( "{0}.{1}" , this.TableName.QuoteTableName() , aColumnName.QuoteColumnName() )
                : string.Format( "{0}.{1} AS {2}" , this.TableAlias.QuoteTableName() , aColumnName.QuoteColumnName() , String.Concat( "{" , this.ActualName , "}" , aColumnName ).QuoteColumnName() );
        //? string.Format( "{0}."+ SQLConstant.COLUMN_NAME_QUALIFIER+"{1}"+ SQLConstant.COLUMN_NAME_QUALIFIER , this.TableName , aColumnName )
        //: string.Format( "{0}."+ SQLConstant.COLUMN_NAME_QUALIFIER+"{1}"+ SQLConstant.COLUMN_NAME_QUALIFIER , this.TableAlias , aColumnName );

        public String ActualName => string.IsNullOrWhiteSpace( TableAlias ) ? TableName : TableAlias;

        public override string ToString() => string.IsNullOrWhiteSpace( TableAlias ) ? TableName : string.Format( "{0} AS {1}" , TableName.QuoteTableName() , TableAlias.QuoteTableName() );

        public TableReference Copy() {
            return ( TableReference ) this.MemberwiseClone();
        }

    }


}
