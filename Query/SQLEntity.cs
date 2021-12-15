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




    public abstract class SQLEntity<U> : SQLQuery where U : AbstractEntity {

        protected U mEntity;



        public SQLEntity( QueryAdapter queryAdapter ) : base( queryAdapter ) {
            mEntity ??= ( U ) Activator.CreateInstance( typeof( U ) );
            //mEntity.mAliasName = "vs";
        }

        //protected SQLEntity() {
        //    mEntity ??= ( U ) Activator.CreateInstance( typeof( U ) );
        //}

        public U Entity { get => mEntity; }

        internal override List<AbstractEntity> GetListOfEntities() {

            return new List<AbstractEntity>() { mEntity };
            //mEntities.Add( ( AbstractEntity ) Activator.CreateInstance( typeof( U ) ) );
        }

        public override List<dynamic> GetProjectionColumns<T>( T myQuery ) {



            var result = new List<dynamic>();
            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );

            if ( option != default( ProjectionFieldsOption ) ) {

                List<dynamic> ProjectionColumns = new List<dynamic>();
                switch ( option ) {
                    case ProjectionFieldsOption.All:
                        var aa = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                        result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                        result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                        break;
                    case ProjectionFieldsOption.Declared:
                        result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                        break;
                    case ProjectionFieldsOption.Entity:
                        result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                        break;

                }

            } else {
                result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
            }
            return result;

        }

        protected override List<TableReference> GetListOfTableReference() {
            return new List<TableReference> { SQLTableEntityHelper.getTableReference<U>() };
            //mHashOfSQLQueryTableReference.Add( e.TableName , e );
        }

        internal override List<string> GetProjectionColumnName<T>( T myQuery ) {
            List<string> result = new List<string>();

            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );
            if ( option != default( ProjectionFieldsOption ) ) {
                switch ( option ) {
                    case ProjectionFieldsOption.All:
                        result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                        result.AddRange( SQLTableEntityHelper.getColumnName<U>( true , false ) );
                        break;
                    case ProjectionFieldsOption.Declared:
                        result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                        //result.AddRange( SQLTableEntityHelper.getColumnName<U>( true , false ) );
                        break;
                    case ProjectionFieldsOption.Entity:
                        result.AddRange( SQLTableEntityHelper.getColumnName<U>( true , false ) );
                        break;

                }

            } else {
                result = SQLTableEntityHelper.getProjectionColumnName<T>( myQuery );
                result.AddRange( SQLTableEntityHelper.getColumnName<U>( true , false ) );
            }

            return result;

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
            result.AddRange( mEntitiesAdapter.TableGraph.GetTopologicalOrder() );
            return result;
        }

        public override List<dynamic> GetProjectionColumns<T>( T myQuery ) {

            var result = new List<dynamic>();
            string[] entitiesNames;
            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );

            if ( option != default( ProjectionFieldsOption ) ) {

                List<dynamic> ProjectionColumns = new List<dynamic>();

                switch ( option ) {

                    case ProjectionFieldsOption.All:

                        entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                        result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                        if ( entitiesNames is null ) {
                            Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                        } else {
                            Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        }

                        //Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                        //result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                        break;

                    case ProjectionFieldsOption.Declared:
                        result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                        break;

                    case ProjectionFieldsOption.Entity:
                        //result.AddRange( SQLTableEntityHelper.getDefinitionColumn( mEntity , true ).ToList() );
                        entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                        //Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                        if ( entitiesNames is null ) {
                            Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                        } else {
                            Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                        }

                        break;

                }

            } else {
                entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                result.AddRange( SQLTableEntityHelper.getProjectionColumn<T>( myQuery ) );
                //Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                if ( entitiesNames is null ) {
                    Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                } else {
                    Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() ) );
                }

            }



            return result;

        }


        protected override List<TableReference> GetListOfTableReference() {

            /*Graph.GetTopologicalOrder().Select( x => x.tableReference )*/
            //return new List<TableReference> { SQLTableEntityHelper.getTableReference<U>() };
            //mHashOfSQLQueryTableReference.Add( e.TableName , e );
            //return Graph.GetTopologicalOrder().Select( x => x.tableReference ).ToList();
            return new List<TableReference>() { Graph.GetTopologicalOrder().Select( x => x.tableReference ).ToList().FirstOrDefault() };
        }

        internal override List<string> GetProjectionColumnName<T>( T myQuery ) {

            List<string> result = new List<string>();

            var option = SQLTableEntityHelper.getQueryTableOptions( myQuery );
            string[] entitiesNames;
            if ( option != default( ProjectionFieldsOption ) ) {
                List<dynamic> ProjectionColumns = new List<dynamic>();
                switch ( option ) {

                    case ProjectionFieldsOption.All:
                        entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                        result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                        //Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        if ( entitiesNames is null ) {
                            Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        } else {
                            Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        }

                        break;

                    case ProjectionFieldsOption.Declared:
                        result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                        break;

                    case ProjectionFieldsOption.Entity:
                        entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                        if ( entitiesNames is null ) {
                            Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        } else {
                            Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                        }

                        break;

                }

            } else {

                entitiesNames = SQLTableEntityHelper.getQueryTableProjectionField( myQuery );
                result.AddRange( SQLTableEntityHelper.getProjectionColumnName<T>( myQuery ) );
                if ( entitiesNames is null ) {
                    Graph.GetTopologicalOrder().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                } else {
                    Graph.GetTopologicalOrder().Where( e => entitiesNames.Contains( e.TableName ) ).ToList().ForEach( e => result.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).Select( x => ( string ) x.ToString() ).ToList() ) );
                }

            }

            return result;

        }

        //public override void Init( MySQLDatabase db ) {
        //    var ctor = typeof( U ).GetConstructor( new Type[] { typeof( MySQLDatabase ) , typeof( ILogger ) } );
        //    mEntitiesAdapter = ( U ) ( ctor?.Invoke( new object[] { db , null } ) );
        //}
    }


}
