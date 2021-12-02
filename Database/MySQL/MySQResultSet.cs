using MySqlConnector;
using prestoMySQL.Database.Interface;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using prestoMySQL.Extension;
using System.Collections;
using System.Data;
using System.Reflection;
using prestoMySQL.Query.Attribute;

namespace prestoMySQL.Database.MySQL {


    public class ReadableResultSet<T> : IReadableResultSet,IDisposable where T : DbDataReader {

        //private Task<T> mResultSet;
        private T mResultSet;
        private bool disposedValue;
        protected Dictionary<string , Dictionary<string , int>> schema;

        //private Random rand;

        public object this[string name] => mResultSet[name] ?? throw new NullReferenceException( "Invalid resultset" );

        //public MySQResultSet( Task<T> aResultSet ) {
        public ReadableResultSet( T aResultSet ) {
            this.mResultSet = aResultSet;
            schema = null;
            //this.rand = new Random();
        }



        public void close() {
            //throw new NotImplementedException();
            if ( this.mResultSet != null ) {


#if DEBUG
                Console.WriteLine( "ResultSet.Close()" );
#endif


                mResultSet.Close();
            }
        }

        public int? columnCount() {

            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {
                return this.mResultSet.FieldCount;
            } else {
                return null;
            }
        }

        public async Task<bool> fetchAsync() {

            //Thread.Sleep( rand.Next( 500 , 1000 ) ); ;

            return await this.mResultSet.ReadAsync();

            //return await ( await this.mResultSet ).ReadAsync();
            //var x = ( await this.mResultSet );

            //Thread.Sleep( rand.Next( 50 , 100 ) ); ;

            //return await x.ReadAsync();

        }

        public bool fetch() {

            //Thread.Sleep( rand.Next( 500 , 1000 ) ); ;
            if ( !this.mResultSet.IsClosed )
                return this.mResultSet.Read();
            else
                return false;

            //return await ( await this.mResultSet ).ReadAsync();
            //var x = ( await this.mResultSet );

            //Thread.Sleep( rand.Next( 50 , 100 ) ); ;

            //return await x.ReadAsync();

        }


        public double getDouble( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetDouble( i );

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public float getFloat( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetFloat( i );

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public int getInt( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetInt32( i );

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public long getLong( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetInt64( i );

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public object getObject( int i ) {

            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet[i];

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public string getString( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {
                return this.mResultSet.GetString( i );

            } else {
                throw new System.Exception( "Invalid Resultset" );
            }
        }

        public T getValueAs<T>( string aColumnName ) {

            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {
                return this.mResultSet[aColumnName].ConvertTo<T>();
            } else {
                throw new System.Exception( "Invalid Resultset" );
            }

        }

        public bool isEmpty() {
            return ( !mResultSet.HasRows );
        }


        public Dictionary<string , Dictionary<string , int>> ResultSetSchemaTable() {

            Dictionary<string , Dictionary<string , int>> result = new Dictionary<string , Dictionary<string , int>>( StringComparer.OrdinalIgnoreCase );
            DataTable schema = mResultSet.GetSchemaTable();

            foreach ( DataRow rdrColumn in schema.Rows ) {

                String columnName = rdrColumn[schema.Columns["ColumnName"]].ToString();
                //String dataType = rdrColumn[schema.Columns["DataType"]].ToString();
                string tablename = rdrColumn[schema.Columns["BaseTableName"]].ToString();
                int index = ( int ) rdrColumn[schema.Columns["ColumnOrdinal"]];
                string BaseColumnName = rdrColumn[schema.Columns["BaseColumnName"]].ToString();

                if ( !columnName.Equals( BaseColumnName ) ) {

                    var beginChar = columnName.IndexOf( '{' );
                    if ( beginChar != -1 ) {
                        var endChar = columnName.IndexOf( '}' , beginChar );

                        if ( ( beginChar != -1 ) && ( endChar != -1 ) ) {

                            var aliasTable = columnName.Substring( beginChar + 1 , ( endChar - beginChar ) - 1 );
                            tablename = aliasTable;
                            columnName = BaseColumnName;
                        } else {
                            throw new System.Exception( "Invalid alias column format " + BaseColumnName );
                        }
                    }
                }

                if ( result.ContainsKey( tablename ) ) {
                    if ( result[tablename].ContainsKey( columnName ) ) {
                        result[tablename][columnName] = index;
                    } else {
                        result[tablename].Add( columnName , index );
                    }
                } else {
                    result.Add( tablename , new Dictionary<string , int>( StringComparer.OrdinalIgnoreCase ) );
                    result[tablename].Add( columnName , index );
                }

            }

            return result;

            //Dictionary<int , String> columnNames = new Dictionary<int , string>();
            //int index = 0;
            //foreach ( DataRow row in schema.Rows ) {
            //    columnNames.Add( index , row[schema.Columns["ColumnName"]].ToString() );
            //    index++;
            //}

        }

        public U getValueAs<U>( int Index ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {
                return this.mResultSet[Index].ConvertTo<U>();
            } else {
                throw new System.Exception( "Invalid Resultset" );
            }

        }

        protected virtual void Dispose( bool disposing ) {
            if ( !disposedValue ) {
                if ( disposing ) {
                    // TODO: eliminare lo stato gestito (oggetti gestiti)
                    schema?.Clear();
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                // TODO: impostare campi di grandi dimensioni su Null
                disposedValue = true;
            }
        }

        // // TODO: eseguire l'override del finalizzatore solo se 'Dispose(bool disposing)' contiene codice per liberare risorse non gestite
        // ~ReadableResultSet()
        // {
        //     // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
            Dispose( disposing: true );
            GC.SuppressFinalize( this );
        }
    }

    public class MySQResultSet : ReadableResultSet<MySqlDataReader> {

        
        public MySQResultSet( MySqlDataReader aResultSet ) : base( aResultSet ) {
            schema = null;
        }

        public T MapTo<T>( string tableName = null ) where T : struct {

            schema ??= this.ResultSetSchemaTable();
            object result = Activator.CreateInstance<T>();
            //objectResult = ( object ) result;

            MethodInfo miGetValueAs = this.GetType().GetMethod( nameof( this.getValueAs ) , new Type[] { typeof( int ) } );

            foreach ( var p in typeof( T ).GetProperties() ) {
                var a = p.GetCustomAttribute<SchemaBindTable>();

                var schemaTable = a?.Table ?? tableName ?? typeof( T ).Name;

                if ( ( schemaTable is not null ) && ( schema.ContainsKey( schemaTable ) ) ) {

                    if ( schema[schemaTable].ContainsKey( p.Name ) ) {

                        MethodInfo getValueAs = miGetValueAs.MakeGenericMethod( p.PropertyType );
                        var v = getValueAs.Invoke( this , new object[] { schema[schemaTable][p.Name] } );
                        p.SetValue( result , v , null );

                    }

                }

            }
            
            return ( T ) result;

        }
    }
}
