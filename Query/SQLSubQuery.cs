using Microsoft.Extensions.Logging;
using prestoMySQL.Adapter;
using prestoMySQL.Column;
using prestoMySQL.Database.MySQL;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.Helper;
using prestoMySQL.Query.Attribute;
using prestoMySQL.SQL;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;


namespace prestoMySQL.Query {

    public abstract class AbstractSQLSubQuery<U> : SQLQuery where U : SQLQuery {

        protected U mSqlQuery;

        public AbstractSQLSubQuery( QueryAdapter queryAdapter ) : base( queryAdapter ) {
            if ( queryAdapter != null ) {
                var ctor = typeof( U ).GetConstructor( new Type[] { queryAdapter.GetType() } );
                mSqlQuery ??= ( U ) ctor?.Invoke( new object[] { queryAdapter } );
            } else {
                var ctor = typeof( U ).GetConstructor( new Type[] { typeof( QueryAdapter ) } );
                mSqlQuery ??= ( U ) ctor?.Invoke( new object[] { queryAdapter } );
            }
            //(mSqlQuery as dynamic).Entity.mAliasName = "sv";
            Dictionary<Type , List<DALQueryEntity>> queryEntity = new Dictionary<Type , List<DALQueryEntity>>();

            var entityType = ( mSqlQuery as dynamic ).Entity.GetType();
            queryEntity.Add( entityType , SQLTableEntityHelper.getQueryEntity( this.GetType() , entityType ) );

            //List<DALQueryEntity> xxx = SQLTableEntityHelper.getQueryEntity( this.GetType(), ( mSqlQuery as dynamic ).Entity.GetType() );

            if ( queryEntity[entityType].Count > 0 ) {
                SQLTableEntityHelper.SetAliasName( ( mSqlQuery as dynamic ).Entity , ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias );
            }

            var projectionFunction = SQLTableEntityHelper.getProjectionColumn( mSqlQuery );
            //List<SQLProjectionFunction<SQLTypeWrapper<DateTime>>> projectionColumns;
            foreach ( var pf in projectionFunction.Where( c => ( typeof( SQLProjectionFunction<> ).Name == ( c as object ).GetType().Name ) ) ) {
                foreach ( var p in pf.FunctionParams ) {

                    
                    SQLTableEntityHelper.SetAliasName( p , ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias, queryEntity , entityType );
                    //if ( ( p != null ) && ( p.GetType().IsAssignableTo( typeof( FunctionParamFunction ) ) ) ) {
                    //    foreach ( IFunctionParam expr in ( p as FunctionParamFunction ).Expression ) {
                    //    }
                    //}
                        //if ( ( p != null ) && ( p.GetType().IsAssignableTo( typeof( FunctionTableProperty ) ) ) ) {

                        //    if ( queryEntity.ContainsKey( ( p as FunctionTableProperty ).tableType ) ) {
                        //        if ( p.GetType().IsAssignableTo( typeof( FunctionParamConstraint ) ) ) {
                        //            //Console.WriteLine( p );
                        //            ( p as FunctionTableProperty ).mTableReference.TableAlias = ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias;
                        //        } else if ( p.GetType().IsAssignableTo( typeof( FunctionParamProperty ) ) ) {
                        //            ( p as FunctionTableProperty ).mTableReference.TableAlias = ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias;
                        //            //Console.WriteLine( p );
                        //        }
                        //    }

                        //} else if ( ( p != null ) && ( p.GetType().IsAssignableTo( typeof( FunctionParamFunction ) ) ) ) {
                        //    foreach ( IFunctionParam expr in (p as FunctionParamFunction ).Expression ) {



                        //    }
                        //}
                    }
            }

            BuildEntityGraph();
        }

        public U Entity { get => mSqlQuery; }

        internal override List<AbstractEntity> GetListOfEntities() {
            //var x = mSqlQuery.GetListOfEntities();
            //var result = new List<AbstractEntity>();
            //result.AddRange( x );
            return mSqlQuery.GetListOfEntities();
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

    public class SQLSubQuery<U> : AbstractSQLSubQuery<U> where U : SQLQuery {

        public SQLSubQuery( QueryAdapter queryAdapter ) : base( queryAdapter ) {
        }

        public override void Prepare() {

            SQLBuilder.SELECT<SQLSubQuery<U>>( this );

        }


        public override string ToString() {
            //this.Build();
            SQLQueryParams outparam = null;
            return SQLBuilder.sqlQuery<SQLSubQuery<U>>( this , ref outparam ).EncloseWith( "(" , ")" );

        }


    }


}