﻿using MySqlConnector;
using PrestoMySQL.Database.Interface;
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

namespace PrestoMySQL.Database.MySQL {


    public class ReadableResultSet<T> : IReadableResultSet where T : DbDataReader {

        //private Task<T> mResultSet;
        private T mResultSet;
        private Random rand;

        public object this[string name] => mResultSet[name] ?? throw new NullReferenceException( "Invalid resultset" );

        //public MySQResultSet( Task<T> aResultSet ) {
        public ReadableResultSet( T aResultSet ) {
            this.mResultSet = aResultSet;
            this.rand = new Random();
        }



        public void close() {
            //throw new NotImplementedException();
            if ( this.mResultSet != null ) {
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
                throw new Exception( "Invalid Resultset" );
            }
        }

        public float getFloat( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetFloat( i );

            } else {
                throw new Exception( "Invalid Resultset" );
            }
        }

        public int getInt( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetInt32( i );

            } else {
                throw new Exception( "Invalid Resultset" );
            }
        }

        public long getLong( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetInt64( i );

            } else {
                throw new Exception( "Invalid Resultset" );
            }
        }

        public object getObject( int i ) {

            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet[i];

            } else {
                throw new Exception( "Invalid Resultset" );
            }
        }

        public string getString( int i ) {
            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {

                return this.mResultSet.GetString( i );

            } else {
                throw new Exception( "Invalid Resultset" );
            }
        }

        public T getValueAs<T>( string aColumnName ) {

            if ( ( this.mResultSet != null ) && ( !this.mResultSet.IsClosed ) ) {
                return this.mResultSet[aColumnName].ConvertTo<T>();
            } else {
                throw new Exception( "Invalid Resultset" );
            }

        }

        public bool isEmpty() {
            return ( !mResultSet.HasRows );
        }


        public object ResultSetSchemaTable() {
            DataTable schema = mResultSet.GetSchemaTable();
            foreach ( DataRow rdrColumn in schema.Rows ) {
                String columnName = rdrColumn[schema.Columns["ColumnName"]].ToString();
                String dataType = rdrColumn[schema.Columns["DataType"]].ToString();
                string tablename = rdrColumn[schema.Columns["BaseTableName"]].ToString();

            }
            return null;
            //Dictionary<int , String> columnNames = new Dictionary<int , string>();
            //int index = 0;
            //foreach ( DataRow row in schema.Rows ) {
            //    columnNames.Add( index , row[schema.Columns["ColumnName"]].ToString() );
            //    index++;
            //}

        }

    }

    public class MySQResultSet : ReadableResultSet<MySqlDataReader>  {
        public MySQResultSet( MySqlDataReader aResultSet ) : base( aResultSet ) {
        }
    }
}
