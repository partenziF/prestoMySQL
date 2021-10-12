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
        public static String getAttributeTableName( Type T ) => GenericEntityAttributeExtension.GetAttributeDALTable( T )?.TableName ?? T.Name;
        public static String getAttributeTableName<T>() where T : AbstractEntity => GenericEntityAttributeExtension.GetAttributeDALTable<T>()?.TableName ?? typeof( T ).Name;
        public static String getAttributeTableName( AbstractEntity aInstance ) => aInstance.GetAttributeDALTable()?.TableName ?? aInstance.GetType().Name;

        public static TableReference getTableReference<T>() where T : AbstractEntity => new TableReference( GenericEntityAttributeExtension.GetAttributeDALTable<T>()?.TableName ?? typeof( T ).Name );
        public static TableReference getTableReference( Type type ) => new TableReference( GenericEntityAttributeExtension.GetAttributeDALTable( type )?.TableName ?? type.Name );


        //public static String getTableAlias( Type T ) => GenericEntityAttributeExtension.GetAttributeDALTable( T )?.Alias;
        //public static String getTableAlias<T>() where T : AbstractEntity => GenericEntityAttributeExtension.GetAttributeDALTable<T>()?.Alias;
        //public static String getTableAlias( AbstractEntity aInstance ) => aInstance.GetAttributeDALTable()?.Alias;

        #endregion

        public static List<PropertyInfo> getPropertyIfColumnDefinition( Type T ) {

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
                     && ( propertyInfo.GetCustomAttributes<DDForeignKey>().FirstOrDefault() != null )
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

                foreach ( PropertyInfo f in getPropertyIfColumnDefinition( T ) ) {

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

        public static string getColumnName( AbstractEntity Table , string aColumn , bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) {

            string result = "";
            //String prefix = ( withTableNameAsPrefix ) ? ( getTableAlias( Table ) ?? getTableName( Table ) ) : String.Empty;
            string prefix = withTableNameAsPrefix ? ( Table.ActualName ) : String.Empty;
            PropertyInfo pi = getPropertyIfColumnDefinition( Table.GetType() ).FirstOrDefault( x => x.Name.Equals( aColumn , StringComparison.InvariantCultureIgnoreCase ) );

            if ( pi != null ) {

                String ColumName = "";

                //ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + pi.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;
                ColumName = pi.ColumnName( null ).QuoteColumnName();

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( prefix.QuoteTableName() , '.' , ColumName );

                result = ColumName;

            } else {
                throw new ArgumentException( "Column name not found." );
            }

            return result;
        }

        public static string getColumnName( Type Table , string aColumn , bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) {

            string result = "";
            //String prefix = ( withTableNameAsPrefix ) ? ( getTableAlias( Table ) ?? getTableName( Table ) ) : String.Empty;
            string prefix = withTableNameAsPrefix ? ( getAttributeTableName( Table ) ) : String.Empty;
            PropertyInfo pi = getPropertyIfColumnDefinition( Table ).FirstOrDefault( x => x.Name.Equals( aColumn , StringComparison.InvariantCultureIgnoreCase ) );

            if ( pi != null ) {

                String ColumName = "";

                //ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + pi.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;
                ColumName = pi.ColumnName( null ).QuoteColumnName();

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( prefix.QuoteTableName() , '.' , ColumName );
                //ColumName = String.Concat( SQLConstant.TABLE_NAME_QUALIFIER + prefix + SQLConstant.TABLE_NAME_QUALIFIER , '.' , ColumName );

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
        public static List<string> getColumnName( Type T , bool withTableNameAsPrefix = false , bool aExcludePrimaryKey = false ) {

            List<String> result = new List<String>();

            String prefix = ( withTableNameAsPrefix ) ? getAttributeTableName( T ) : String.Empty;

            foreach ( PropertyInfo f in getPropertyIfColumnDefinition( T ) ) {

                String ColumName = "";

                //ColumName = SQLConstant.COLUMN_NAME_QUALIFIER + f.ColumnName( null ) + SQLConstant.COLUMN_NAME_QUALIFIER;
                ColumName = f.ColumnName( null ).QuoteColumnName();

                if ( withTableNameAsPrefix )
                    ColumName = String.Concat( prefix.QuoteTableName() , '.' , ColumName );
                //ColumName = String.Concat( SQLConstant.TABLE_NAME_QUALIFIER + prefix + SQLConstant.TABLE_NAME_QUALIFIER , '.' , ColumName );

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

        public static void SetAliasName( IFunctionParam p , string AliasName , Dictionary<Type , List<DALQueryEntity>> queryEntity , Type entityType ) {

            if ( ( p != null ) && ( p.GetType().IsAssignableTo( typeof( FunctionTableProperty ) ) ) ) {

                if ( queryEntity.ContainsKey( ( p as FunctionTableProperty ).tableType ) ) {
                    if ( p.GetType().IsAssignableTo( typeof( FunctionParamConstraint ) ) ) {
                        //Console.WriteLine( p );
                        ( p as FunctionTableProperty ).mTableReference.TableAlias = ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias;
                    } else if ( p.GetType().IsAssignableTo( typeof( FunctionParamProperty ) ) ) {
                        ( p as FunctionTableProperty ).mTableReference.TableAlias = ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias;
                        //Console.WriteLine( p );
                    }
                }

            } else if ( ( p != null ) && ( p.GetType().IsAssignableTo( typeof( FunctionParamFunction ) ) ) ) {
                foreach ( IFunctionParam expr in ( p as FunctionParamFunction ).Expression ) {
                    SQLTableEntityHelper.SetAliasName( expr , ( queryEntity[entityType] as List<DALQueryEntity> ).FirstOrDefault().Alias , queryEntity , entityType );
                }

            }

        }
        public static void SetAliasName( AbstractEntity entity , string AliasName ) {
            entity.mAliasName = AliasName;
            foreach ( var pi in SQLTableEntityHelper.getPropertyIfColumnDefinition( entity.GetType() ) ) {
                var c = pi.GetValue( entity );
                ( ( TableReference ) ( c as dynamic ).mTable ).TableAlias = entity.AliasName;
            }
        }

        #endregion

        #region Query         

        public static List<DALQueryEntity> getQueryEntity<U>( Type entity ) {
            var result = new List<DALQueryEntity>();

            if ( Attribute.IsDefined( typeof( U ) , typeof( DALQueryEntity ) ) ) {

                result = entity.GetCustomAttributes<DALQueryEntity>()?.Where( a => a.Entity == entity ).ToList();

            }

            return result;
        }

        public static List<DALQueryEntity> getQueryEntity( Type Query , Type entity ) {
            var result = new List<DALQueryEntity>();

            if ( Attribute.IsDefined( Query , typeof( DALQueryEntity ) ) ) {

                result = Query.GetCustomAttributes<DALQueryEntity>()?.Where( a => a.Entity == entity ).ToList();

            }

            return result;
        }

        public static List<Type> getQueryEntity( Type c ) {
            var result = new List<Type>();

            if ( Attribute.IsDefined( c , typeof( DALQueryEntity ) ) ) {

                result = c.GetCustomAttributes<DALQueryEntity>()?.ToList().Select( x => ( ( DALQueryEntity ) x ).Entity ).ToList();
            }

            return result;
        }

        public static List<Type> getQueryJoinEntity( Type c ) {
            var result = new List<Type>();

            if ( Attribute.IsDefined( c , typeof( DALQueryJoinEntity ) ) ) {

                result = c.GetCustomAttributes<DALQueryJoinEntity>()?.ToList().Select( x => ( ( DALQueryJoinEntity ) x ).Entity ).ToList();
            }

            return result;
        }

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

                c.GetCustomAttributes<DALQueryEntity>()?.ToList().ForEach( a => result.Add( new TableReference( getAttributeTableName( a.Entity ) , a.Alias ) ) );

            } else {
                throw new System.Exception( "Table references annotation is missing" );
            }

            return result;

        }

        public static List<DALGroupBy> getQueryGroupBy( Type c ) {

            List<DALGroupBy> result = new List<DALGroupBy>();

            if ( Attribute.IsDefined( c , typeof( DALGroupBy ) ) ) {

                result = c.GetCustomAttributes<DALGroupBy>()?.ToList();

            }

            return result;

        }

        public static List<DALOrderBy> getQueryOrderBy( Type c ) {

            List<DALOrderBy> result = new List<DALOrderBy>();

            if ( Attribute.IsDefined( c , typeof( DALOrderBy ) ) ) {

                result = c.GetCustomAttributes<DALOrderBy>()?.ToList();

            }

            return result;

        }

        public static List<PropertyInfo> getProjectionFields<T>() where T : SQLQuery {


            var Result = new List<PropertyInfo>();
            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance ); //| BindingFlags.FlattenHierarchy 

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition().IsAssignableTo( typeof( GenericQueryColumn ) ) ) ) {

                    Result.Add( propertyInfo );

                    //} else if ( ( propertyInfo.PropertyType.IsGenericType ) && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) ) {

                }

            }

            return Result;

        }

        public static List<PropertyInfo> getProjectionFields( SQLQuery aInstance ) {


            var Result = new List<PropertyInfo>();
            PropertyInfo[] props = aInstance.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance ); //| BindingFlags.FlattenHierarchy 

            foreach ( PropertyInfo propertyInfo in props ) {

                if ( propertyInfo.PropertyType.IsGenericType && ( propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) ) ) {

                    Result.Add( propertyInfo );

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

                    //}

                    //if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionColumn<> ) ) ) {

                    //    dynamic o = f.GetValue( aQueryInstance );
                    //    ColumName = ( ( GenericQueryColumn ) o ).ToString();

                } else if ( f.PropertyType.IsGenericType && ( f.PropertyType.GetGenericTypeDefinition() == typeof( SQLProjectionFunction<> ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    //ColumName = ( ( GenericQueryColumn ) o ).ToString();
                    if ( o != null ) {
                        result.Add( o );

                    }

                }
            }

            return result;

        }

        public static ProjectionFieldsOption getQueryTableOptions( SQLQuery aQueryInstance ) {

            if ( Attribute.IsDefined( aQueryInstance.GetType() , typeof( DALProjectionFieldsOption ) ) ) {
                return ( aQueryInstance.GetType().GetCustomAttribute( typeof( DALProjectionFieldsOption ) ) as DALProjectionFieldsOption ).Option;
            } else {
                return default;
            }
        }

        public static List<dynamic> getProjectionColumn( SQLQuery aQueryInstance ) {

            List<dynamic> result = new List<dynamic>();

            foreach ( System.Reflection.PropertyInfo f in getProjectionFields( aQueryInstance ) ) {

                if ( Attribute.IsDefined( f , typeof( DALProjectionColumn ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    if ( o != null ) {
                        result.Add( o );
                    }

                } else if ( Attribute.IsDefined( f , typeof( DALProjectionFunction ) ) ) {

                    dynamic o = f.GetValue( aQueryInstance );
                    if ( o != null ) {
                        result.Add( o );
                    }

                }

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


        public static IEnumerable<DALGenericQueryJoinConstraint> getQueryJoinConstraint( Type t ) {

            var result = new List<DALGenericQueryJoinConstraint>();

            if ( Attribute.IsDefined( t , typeof( DALQueryJoinEntityConstraint ) ) ) {

                result.AddRange( t.GetCustomAttributes<DALQueryJoinEntityConstraint>()?.ToList() );

            }
            if ( Attribute.IsDefined( t , typeof( DALQueryJoinSubQueryConstraint ) ) ) {

                result.AddRange( t.GetCustomAttributes<DALQueryJoinSubQueryConstraint>()?.ToList() );

            }
            if ( Attribute.IsDefined( t , typeof( DALQueryJoinEntityExpression ) ) ) {

                result.AddRange( t.GetCustomAttributes<DALQueryJoinEntityExpression>()?.ToList() );

            }

            if ( Attribute.IsDefined( t , typeof( DALQueryJoinBetween ) ) ) {

                result.AddRange( t.GetCustomAttributes<DALQueryJoinBetween>()?.ToList() );

            }

            return result;
        }

        public static IEnumerable<DALQueryJoinEntityUnConstraint> getQueryJoinUnConstraint( Type t ) {

            var result = new List<DALQueryJoinEntityUnConstraint>();

            if ( Attribute.IsDefined( t , typeof( DALQueryJoinEntityUnConstraint ) ) ) {

                result = t.GetCustomAttributes<DALQueryJoinEntityUnConstraint>()?.ToList();

            }

            return result;
        }


        #endregion

    }

}