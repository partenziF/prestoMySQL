using Microsoft.Extensions.Logging;
using prestoMySQL.Adapter;
using prestoMySQL.Database.MySQL;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;


namespace prestoMySQL.Query {

    public abstract class SQLSubQuery<U> : SQLQuery where U : SQLQuery {

        protected U mSqlQuery;

        public SQLSubQuery( QueryAdapter queryAdapter ) : base( queryAdapter ) {
            //mEntity ??= ( U ) Activator.CreateInstance( typeof( U ) );
            var ctor = typeof( U ).GetConstructor( new Type[] { queryAdapter.GetType() } );
            mSqlQuery ??= ( U ) ctor?.Invoke( new object[] { queryAdapter } );
            (mSqlQuery as dynamic).Entity.mAliasName = "sv";
        }

        public U Entity { get => mSqlQuery; }

        internal override List<AbstractEntity> GetListOfEntities() {
            var x = mSqlQuery.GetListOfEntities();
            return new List<AbstractEntity>() { };
            //mEntities.Add( ( AbstractEntity ) Activator.CreateInstance( typeof( U ) ) );
        }

        public override List<dynamic> GetProjectionColumns<T>( T myQuery ) {

            var result = new List<dynamic>();
            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );
            if ( option != default( ProjectionFieldsOption ) ) {
                List<dynamic> ProjectionColumns = new List<dynamic>();
                switch ( option ) {
                    case ProjectionFieldsOption.All:
                    result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                    //result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                    break;
                    case ProjectionFieldsOption.Declared:
                    result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                    break;
                    case ProjectionFieldsOption.Entity:
                    //result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                    break;

                }

            } else {
                result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                //result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
            }
            return result;

        }

        protected override List<TableReference> GetListOfTableReference() {

            return SQLTableEntityHelper.getQueryTableName( this.GetType() );
            //return new List<TableReference> { new TableReference( "StruttureVendite" , "sv" ) };
            //return new List<TableReference> { SQLTableEntityHelper.getTableReference<U>() };
            //mHashOfSQLQueryTableReference.Add( e.TableName , e );
        }

        internal override List<string> GetProjectionColumnName<T>( T myQuery ) {
            List<string> result = new List<string>();

            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );
            if ( option != default( ProjectionFieldsOption ) ) {
                List<dynamic> ProjectionColumns = new List<dynamic>();
                switch ( option ) {
                    case ProjectionFieldsOption.All:
                    result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                    result.AddRange( SQLTableEntityHelper.getProjectionColumnName<U>( mSqlQuery ) );
                    break;
                    case ProjectionFieldsOption.Declared:
                    result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                    break;
                    case ProjectionFieldsOption.Entity:
                    result.AddRange( SQLTableEntityHelper.getProjectionColumnName<U>( mSqlQuery ) );
                    break;

                }

            } else {
                result = SQLTableEntityHelper.getProjectionColumnName<T>( myQuery );
                result.AddRange( SQLTableEntityHelper.getProjectionColumnName<U>( mSqlQuery ) );
                //result.AddRange( SQLTableEntityHelper.getColumnName<U>( true , false ) );
            }

            return result;
        }

    }

}