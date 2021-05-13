using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Helper;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column {
    public class SQLProjectionColumn<T> : MySQLColumn<T>, QueryableColumn<T> where T : ISQLTypeWrapper {

        public SQLProjectionColumn( string aDeclaredVariableName ) :
            base( aDeclaredVariableName ,
            new System.Diagnostics.StackTrace()?.GetFrame( 1 )?.GetMethod().ReflectedType?.GetProperty( aDeclaredVariableName ) ) {

            DALProjectionColumn columnAttribute = this.mPropertyInfo?.GetCustomAttribute<DALProjectionColumn>( false );
            this.mAlias = columnAttribute?.Alias ?? "";

            if ( string.IsNullOrWhiteSpace( columnAttribute.Table ) ) {
                //TableReference t = 
            }

            //TableReference t = this.getTableReference( a.Table() , aClazz );
            //columnAttribute.Table = 

        }

        string mAlias;
        public string Alias { get => mAlias; set => mAlias = value; }

        bool mIsNullValue;
        public bool isNullValue { get => mIsNullValue; set => mIsNullValue = value; }

        TableReference mTable;
        public TableReference Table { get => mTable; set => mTable = value; }

        public string ColumnName => throw new NotImplementedException();

        public T Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private TableReference getTableReference<U>( String aTableName ) {

            foreach ( TableReference t in SQLTableEntityHelper.getQueryTableName<U>() ) {

                if ( t.TableName.Equals( aTableName ) ) {
                    return t;
                }
            }

            foreach ( TableReference t in SQLTableEntityHelper.getQueryJoinTableName<U>( ) ) {
                if ( t.TableName.Equals( aTableName ) ) {
                    return t;
                }
            }
            //		}

            throw new System.Exception( "Invalid table name" );

        }

    }

}
