using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace prestoMySQL.Helper {
    public static class SQLTableEntityHelper {

        #region Entity
        #region Table Name
        public static String getTableName<T>() where T : GenericEntity => GenericEntityAttributeExtension.GetAttributeDALTable<T>()?.TableName ?? typeof( T ).Name;
        public static String getTableName<T>( T aInstance ) where T : GenericEntity => aInstance.GetAttributeDALTable()?.TableName ?? typeof( T ).Name;

        #endregion

        public static List<PropertyInfo> getPropertyIfColumnDefinition<T>() where T : GenericEntity {

            var Result = new List<PropertyInfo>();
            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( DefinitionColumn<> ) ) ) {

                    Result.Add( propertyInfo );

                } else if ( ( propertyInfo.PropertyType.IsGenericType ) && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {

                }

            }

            return Result;
        }

        public static List<PropertyInfo> getPropertyIfPrimaryKey<T>() {

            List<PropertyInfo> Result = new List<PropertyInfo>();

            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType
                     && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( DefinitionColumn<> ) )
                     && ( propertyInfo.GetCustomAttribute<DDPrimaryKey>() != null )
                     && ( propertyInfo.GetCustomAttribute<DDColumnAttribute>() != null )
                   ) {

                    Result.Add( propertyInfo );

                }

            }

            return Result;

        }



        public static List<dynamic> getPrimaryKeyDefinitionColumn<T>( T aTableInstance ) where T : GenericEntity {

            //List<ColumnDefinition<?>> how to implement from java to c#
            List<dynamic> l = new List<dynamic>();

            try {

                foreach ( PropertyInfo f in getPropertyIfPrimaryKey<T>() ) {

                    var oDefinitionColumn = f.GetValue( aTableInstance );
                    if ( oDefinitionColumn != null ) l.Add( ( dynamic ) oDefinitionColumn );

                }

            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

            return l;

        }

        public static List<dynamic> getDefinitionColumn<T>( T aTableInstance , bool aIncludePrimaryKey = false ) where T : GenericEntity {

            List<dynamic> l = new List<dynamic>();

            try {

                foreach ( PropertyInfo f in getPropertyIfColumnDefinition<T>() ) {

                    if ( Attribute.IsDefined( f , typeof( DDColumnAttribute ) ) ) {
                        var oDefinitionColumn = f.GetValue( aTableInstance );
                        if ( oDefinitionColumn != null ) {
                            if ( ( Attribute.IsDefined( f , typeof( DDPrimaryKey ) ) ) ) {
                                if ( aIncludePrimaryKey ) {
                                    l.Add( ( dynamic ) oDefinitionColumn );
                                }
                            } else {
                                l.Add( ( dynamic ) oDefinitionColumn );

                            }
                        }
                    }

                }

            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

            return l;
        }


        public static List<string> getColumnName<T>( bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) where T : GenericEntity {

            List<String> result = new List<String>();

            String prefix = ( withTableNameAsPrefix ) ? getTableName<T>() : String.Empty;


            foreach ( PropertyInfo f in getPropertyIfColumnDefinition<T>() ) {

                String ColumName = "";

                ColumName = f.ColumnName( null );

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( prefix , '.' , ColumName );

                if ( aExcludePrimaryKey ) {
                    if ( !Attribute.IsDefined( f , typeof( DDPrimaryKey ) ) ) {
                        result.Add( ColumName );
                    }
                } else {
                    result.Add( ColumName );
                }

            }

            return result;

        }

        #endregion

        #region Query 
        public static IEnumerable<TableReference> getQueryTableName<T>() {

            throw new NotImplementedException();
            //List<TableReference> result = new List<TableReference>();
            //DALQueryTableReferences[] tableReferences = typeof( T ).GetCustomAttributes<DALQueryTableReferences>().ToArray();
            //if ( tableReferences != null ) {
            //    foreach( DALQueryTableReferences a in tableReferences ) {
            //        result.Add( new TableReference( a.Table , a.Alias ) );
            //    }

            //} else {
            //    //DDColumnAttribute.IsDefined(typeof(T),)
            //    throw new NotImplementedException();
            //}


            //return result;
        }
        public static IEnumerable<TableReference> getQueryJoinTableName<T>() {
            throw new NotImplementedException();
        }
        #endregion


    }
}
