
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//cmd.CommandText = "INSERT INTO blogEntry (entryText) VALUE ('"+MySql.Data.MySqlClient.MySqlHelper.EscapeString(entryText)+"');";


namespace prestoMySQL.Query {
    public class MySQLQueryParam : QueryParam {

        public static readonly string MYSQL_DATE_FORMAT = "yyyy-MM-dd H:mm:ss";

        public MySQLQueryParam( object aValue , string aName ) : base( aValue , aName ) {
        }


        public override string ToString() {
            if ( mValue is null )
                return "NULL";
            else if ( mValue is string )

                //  Quando possibile, rifiutare l'input contenente i caratteri seguenti.
                //  CONVALIDARE TUTTI GLI INPUT
                //  Carattere di input Significato in Transact - SQL
                //  ;               Delimitatore di query
                //  '	            Delimitatore di stringhe di dati di tipo carattere
                //  --              Delimitatore di commento a riga singola. Il testo --che segue fino alla fine della riga non viene valutato dal server.
                //  /*_ ... _*/	    Delimitatori di commento.Il testo compreso fra /* _ e _ */ non viene valutato dal server.
                return String.Concat( "'" , MySqlConnector.MySqlHelper.EscapeString( ( string ) mValue ) , "'" );
            else if ( mValue is DateTime )
                return String.Concat( "'" , MySqlConnector.MySqlHelper.EscapeString( ( ( DateTime ) mValue ).ToString( MySQLQueryParam.MYSQL_DATE_FORMAT ) ) , "'" );
            else
                return mValue.ToString();

        }

        protected override object GetValue() {
            if ( mValue is string ) {
                return MySqlConnector.MySqlHelper.EscapeString( ( string ) mValue );
            } else
                return mValue;
        }

    }

}