using Microsoft.Extensions.Logging;
using prestoMySQL.Adapter;
using prestoMySQL.Database.MySQL;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace prestoMySQL.Query {

    public abstract class SQLEntity<U> : SQLQuery where U : AbstractEntity {

        protected U mEntity;


        public SQLEntity( QueryAdapter queryAdapter ) : base( queryAdapter ) {
            mEntity ??= ( U ) Activator.CreateInstance( typeof( U ) );
        }

        //protected SQLEntity() {
        //    mEntity ??= ( U ) Activator.CreateInstance( typeof( U ) );
        //}

        public U Entity { get => mEntity; }

        internal override List<AbstractEntity> GetListOfEntities() {

            return new List<AbstractEntity>() { mEntity };
            //mEntities.Add( ( AbstractEntity ) Activator.CreateInstance( typeof( U ) ) );
        }

        public override List<dynamic> GetProjectionColumns() {
            return SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList();
        }


        protected override List<TableReference> GetListOfTableReference() {
            return new List<TableReference> { SQLTableEntityHelper.getTableReference<U>() };
            //mHashOfSQLQueryTableReference.Add( e.TableName , e );
        }



        internal override List<string> GetProjectionColumnName<T>( T myQuery ) {
            return SQLTableEntityHelper.getColumnName<U>( true , false );
        }


    }

    public abstract class SQLEntities<U> : SQLQuery where U : EntitiesAdapter {

        private U mEntitiesAdapter;

        public SQLEntities( QueryAdapter queryAdapter ) : base( queryAdapter ) {
            var ctor = typeof( U ).GetConstructor( new Type[] { typeof( MySQLDatabase ) , typeof( ILogger ) } );
            mEntitiesAdapter = ( U ) ( ctor?.Invoke( new object[] { queryAdapter.mDatabase , null } ) );

        }

        public U EntitiesAdapter { get => this.mEntitiesAdapter; }

        internal override List<AbstractEntity> GetListOfEntities() {
            var result = new List<AbstractEntity>();
            result.AddRange( mEntitiesAdapter.Graph.GetTopologicalOrder() );
            return result;
        }

        public override List<dynamic> GetProjectionColumns() {
            //return SQLTableEntityHelper.getDefinitionColumn( mEntitiesAdapter , true ).ToList();
            var result = new List<dynamic>();
            foreach ( var e in Graph.GetTopologicalOrder() ) {
                result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() );
                //primaryKeysValues.Add( e.ActualName , new Dictionary<Type , List<object>>() );
            }

            return result; 
        }


        protected override List<TableReference> GetListOfTableReference() {
            
            /*Graph.GetTopologicalOrder().Select( x => x.tableReference )*/;
            //return new List<TableReference> { SQLTableEntityHelper.getTableReference<U>() };
            //mHashOfSQLQueryTableReference.Add( e.TableName , e );
            //return Graph.GetTopologicalOrder().Select( x => x.tableReference ).ToList();
            return new List<TableReference>() { Graph.GetTopologicalOrder().Select( x => x.tableReference ).ToList().FirstOrDefault() };
        }



        internal override List<string> GetProjectionColumnName<T>( T myQuery ) {
            //var ccc = SQLTableEntityHelper.getColumnName<U>( true , false );
            
            //return SQLTableEntityHelper.getColumnName<U>( true , false );
            //return new List<string>();
            List<string> result = new List<string>();

             result = SQLTableEntityHelper.getProjectionColumnName<T>( myQuery );

            foreach ( var e in Graph.GetTopologicalOrder() ) {

                result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() );

            }


            return result;

        }

        //public override void Init( MySQLDatabase db ) {
        //    var ctor = typeof( U ).GetConstructor( new Type[] { typeof( MySQLDatabase ) , typeof( ILogger ) } );
        //    mEntitiesAdapter = ( U ) ( ctor?.Invoke( new object[] { db , null } ) );
        //}
    }


}
