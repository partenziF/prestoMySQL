using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using prestoMySQL.Query.SQL;
using DatabaseEntity.EntityAdapter;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.SQL;

namespace prestoMySQL.Helper {
    public static class SQLTableEntityHelper {

        #region Entity
        #region Table Name
        public static String getTableName( Type T ) => GenericEntityAttributeExtension.GetAttributeDALTable( T )?.TableName ?? T.Name;
        public static String getTableName<T>() where T : AbstractEntity => GenericEntityAttributeExtension.GetAttributeDALTable<T>()?.TableName ?? typeof( T ).Name;
        public static String getTableName( AbstractEntity aInstance ) => aInstance.GetAttributeDALTable()?.TableName ?? aInstance.GetType().Name;


        #endregion

        public static List<PropertyInfo> getPropertyIfColumnDefinition(Type T)  {
            var Result = new List<PropertyInfo>();
            PropertyInfo[] props = T.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) ) ) {

                    Result.Add( propertyInfo );

                } else if ( ( propertyInfo.PropertyType.IsGenericType ) && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {

                }

            }

            return Result;
        }
        public static List<PropertyInfo> getPropertyIfColumnDefinition<T>() where T : AbstractEntity {
            
            return getPropertyIfColumnDefinition( typeof( T ) );
            //var Result = new List<PropertyInfo>();
            //PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

            //foreach ( PropertyInfo propertyInfo in props ) {

            //    if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) ) ) {

            //        Result.Add( propertyInfo );

            //    } else if ( ( propertyInfo.PropertyType.IsGenericType ) && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {

            //    }

            //}

            //return Result;
        }

        public static List<PropertyInfo> getPropertyIfPrimaryKey( Type aType ) {

            List<PropertyInfo> Result = new List<PropertyInfo>();

            PropertyInfo[] props = aType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType
                     && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) )
                     && ( propertyInfo.GetCustomAttribute<DDPrimaryKey>() != null )
                     && ( propertyInfo.GetCustomAttribute<DDColumnAttribute>() != null )
                   ) {

                    Result.Add( propertyInfo );

                }

            }

            return Result;

        }



        public static List<PropertyInfo> getPropertyIfForeignKey( Type aType ) {

            List<PropertyInfo> Result = new List<PropertyInfo>();

            PropertyInfo[] props = aType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType
                     && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) )
                     && ( propertyInfo.GetCustomAttribute<DDForeignKey>() != null )
                     && ( propertyInfo.GetCustomAttribute<DDColumnAttribute>() != null )
                   ) {

                    Result.Add( propertyInfo );

                }

            }

            return Result;

        }



        public static List<PropertyInfo> getPropertyIfIndex( Type aType ) {

            List<PropertyInfo> Result = new List<PropertyInfo>();

            PropertyInfo[] props = aType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType
                     && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) )
                     && ( propertyInfo.GetCustomAttribute<DDIndexAttribute>() != null )
                     && ( propertyInfo.GetCustomAttribute<DDColumnAttribute>() != null )
                   ) {

                    Result.Add( propertyInfo );

                }

            }

            return Result;

        }


        public static List<PropertyInfo> getPropertyIfUniqueIndex( Type aType ) {

            List<PropertyInfo> Result = new List<PropertyInfo>();

            PropertyInfo[] props = aType.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType
                     && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( MySQLDefinitionColumn<> ) )
                     && ( propertyInfo.GetCustomAttribute<DDUniqueIndexAttribute>() != null )
                     && ( propertyInfo.GetCustomAttribute<DDColumnAttribute>() != null )
                   ) {

                    Result.Add( propertyInfo );

                }

            }

            return Result;

        }


        public static List<dynamic> getPrimaryKeyDefinitionColumn( AbstractEntity aTableInstance ) {

            List<dynamic> l = new List<dynamic>();

            try {

                foreach ( PropertyInfo f in getPropertyIfPrimaryKey( aTableInstance.GetType() ) ) {

                    var oDefinitionColumn = f.GetValue( aTableInstance );
                    if ( oDefinitionColumn != null ) l.Add( ( dynamic ) oDefinitionColumn );

                }

            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

            return l;

        }

        public static List<dynamic> getDefinitionColumn( AbstractEntity aTableInstance , bool aIncludePrimaryKey = false ) {

            Type T = aTableInstance.GetType();

            List<dynamic> l = new List<dynamic>();

            try {

                foreach ( PropertyInfo f in getPropertyIfColumnDefinition(T) ) {

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


        public static string getColumnName( Type Table , string aColumn , bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) {
            string result = "";
            String prefix = ( withTableNameAsPrefix ) ? getTableName( Table ) : String.Empty;
            PropertyInfo pi = getPropertyIfColumnDefinition( Table ).FirstOrDefault( x => x.Name.Equals( aColumn , StringComparison.InvariantCultureIgnoreCase ) );
            
            if ( pi != null ) {
                
                String ColumName = "";

                ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + pi.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( SQLConstant.TABLE_NAME_QUALIFIER + prefix + SQLConstant.TABLE_NAME_QUALIFIER , '.' , ColumName );

                result = ColumName;

            } else {
                throw new ArgumentException( "Column name not found." );
            }

            return result;
        }


        public static List<string> getColumnName<T>( bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) where T : AbstractEntity {

            //List<String> result = new List<String>();

            //String prefix = ( withTableNameAsPrefix ) ? getTableName<T>() : String.Empty;

            //foreach ( PropertyInfo f in getPropertyIfColumnDefinition<T>() ) {

            //    String ColumName = "";

            //    ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + f.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;

            //    if ( withTableNameAsPrefix )
            //        ColumName = String.Concat( SQLConstant.TABLE_NAME_QUALIFIER+ prefix + SQLConstant.TABLE_NAME_QUALIFIER , '.' , ColumName );

            //    if ( aExcludePrimaryKey ) {
            //        if ( !Attribute.IsDefined( f , typeof( DDPrimaryKey ) ) ) {
            //            result.Add( ColumName );
            //        }
            //    } else {
            //        result.Add( ColumName );
            //    }

            //}

            //return result;

            return getColumnName( typeof( T ) , withTableNameAsPrefix , aExcludePrimaryKey );

        }


        public static List<string> getColumnName(Type T, bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false )  {

            List<String> result = new List<String>();

            String prefix = ( withTableNameAsPrefix ) ? getTableName(T) : String.Empty;

            foreach ( PropertyInfo f in getPropertyIfColumnDefinition(T) ) {

                String ColumName = "";

                ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + f.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( SQLConstant.TABLE_NAME_QUALIFIER + prefix + SQLConstant.TABLE_NAME_QUALIFIER , '.' , ColumName );

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

        public static List<TableReference> getQueryTableName( Type c ) {

            //Type c = typeof( T );
            List<TableReference> result = new List<TableReference>();


            if ( Attribute.IsDefined( c , typeof( DALQueryTable ) ) ) {

                //DALQueryTable a = c.getAnnotation( typeof( DALQueryTable ) );
                //result.Add( new TableReference( a.Table() , a.Alias() ) );
                c.GetCustomAttributes<DALQueryTable>()?.ToList().ForEach( a => result.Add( new TableReference( a.Table , a.Alias ) ) );


            } else if ( Attribute.IsDefined( c , typeof( DALQueryEntity ) ) ) {

                //DALQueryEntity e = c.getAnnotation( typeof( DALQueryEntity ) );
                //result.Add( new TableReference( getTableName( e.value() ) , e.Alias() ) );

                c.GetCustomAttributes<DALQueryEntity>()?.ToList().ForEach( a => result.Add( new TableReference( getTableName( a.value ) , a.Alias ) ) );


            } else {
                throw new System.Exception( "Table references annotation is missing" );
            }

            return result;

        }


        public static List<PropertyInfo> getProjectionFields<T>() where T : SQLQuery {


            var Result = new List<PropertyInfo>();
            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance ); //| BindingFlags.FlattenHierarchy 

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) ) ) {

                    Result.Add( propertyInfo );

                    //} else if ( ( propertyInfo.PropertyType.IsGenericType ) && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {

                }

            }

            return Result;

        }



        public static List<dynamic> getProjectionColumn<T>( T aQueryInstance ) where T : SQLQuery {

            //            List<string> result = new List<string>();
            List<dynamic> result = new List<dynamic>();

            foreach ( System.Reflection.PropertyInfo f in getProjectionFields<T>() ) {

                if ( Attribute.IsDefined( f , typeof( DALProjectionColumn ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    if ( o != null ) {
                        result.Add( o );
                    }

                }

                //if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) ) ) {

                //    dynamic o = f.GetValue( aQueryInstance );
                //    ColumName = ( ( GenericQueryColumn ) o ).ToString();

                //} else if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionFunction<> ) ) ) {

                //    dynamic o = f.GetValue( aQueryInstance );
                //    ColumName = ( ( GenericQueryColumn ) o ).ToString();

                //}


            }

            return result;

        }





        public static List<string> getProjectionColumnName<T>( T aQueryInstance ) where T : SQLQuery {

            List<string> result = new List<string>();

            foreach ( System.Reflection.PropertyInfo f in getProjectionFields<T>() ) {

                string ColumName = "";

                if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    ColumName = ( ( GenericQueryColumn ) o ).ToString();

                } else if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionFunction<> ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    ColumName = ( ( GenericQueryColumn ) o ).ToString();

                }

                result.Add( ColumName );

            }

            return result;

        }



        public static IEnumerable<TableReference> getQueryJoinTableName<T>() {
            throw new NotImplementedException();
        }
        #endregion


    }
}
