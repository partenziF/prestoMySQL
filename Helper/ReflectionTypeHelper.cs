using prestoMySQL.Column;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.ForeignKey;
using prestoMySQL.PrimaryKey;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Helper {
    public static class ReflectionTypeHelper {


        public static Type GetColumnDefinitionGenericType( dynamic aColumnDefinition ) {
            return aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];
        }

        public static Type GetClassGenericType( dynamic aClass ) {
            if ( aClass.GetType().GenericTypeArguments.Length > 0 ) {
                return aClass.GetType().GetGenericArguments()[0];
            } else if ( aClass.GetType().BaseType.GenericTypeArguments.Length > 0 ) {
                return aClass.GetType().BaseType.GetGenericArguments()[0];
            } else {
                return null;
            }
        }


        //public static void AssignValue<T>( EntityForeignKey key , T entityLeft ) where T : AbstractEntity {
        public static void AssignValue<T>( EntityForeignKey key , PropertyInfo pi , T entityLeft ) where T : AbstractEntity {

            //var pi = key.Table.GetType().GetProperty( key.Column );
            dynamic colPK = ( ConstructibleColumn ) ( pi?.GetValue( key.Table ) );
            bool usePK = true;
            PropertyInfo piFK = null;
            if ( entityLeft.PrimaryKey[key.ReferenceColumnName] != null ) {
                piFK = entityLeft.PrimaryKey[key.ReferenceColumnName];
                usePK = false;
            } else {
                usePK = true;
                piFK = entityLeft.GetType().GetProperty( key.ReferenceColumnName );

            }
            dynamic colFK = piFK?.GetValue( entityLeft );
            //dynamic vv = colPK.TypeWrapperValue;
            //var mi = entityLeft.PrimaryKey.GetType().GetMethod( nameof( entityLeft.PrimaryKey.getKeyValue ) );
            //MethodInfo migeneric = mi.MakeGenericMethod( new Type[] { col.GenericType } );
            //object v = migeneric.Invoke( entityLeft.PrimaryKey , new object[] { key.Reference } );

            colPK.AssignValue( colFK?.TypeWrapperValue ?? SQLTypeWrapperNULL( colFK?.GenericType ) );
            if ( usePK ) {
                ( ( IObservableColumn ) colPK ).Attach( key );
            } else {
                ( ( IObservableColumn ) colFK ).Attach( key );
            }

            //            ( col as dynamic ).TypeWrapperValue = v;
            /*
             *                     foreach ( KeyValuePair<string , PropertyInfo> kvp in this.Entity.PrimaryKey ) {
                        dynamic x = kvp.Value.GetValue( Entity );
                        x.AssignValue( KeyValues[i++] );                    
                    }
*/
        }


        public static void SetValueToColumn( EntityPrimaryKey key , PropertyInfo pi , dynamic value ) {

            //TODO oppure usare il metodo AssignValue
            ConstructibleColumn col = ( ConstructibleColumn ) ( pi?.GetValue( key.Table ) );
            var p2 = col.GetType().GetProperty( nameof( MySQLDefinitionColumn<SQLTypeWrapper<dynamic>>.TypeWrapperValue ) );
            if ( value != null ) {
                var setter = p2.GetSetMethod( nonPublic: true );
                var ctors = p2.PropertyType.GetConstructor( new Type[] { col.GenericType } );
                var x = setter.Invoke( col , new object?[] { ctors.Invoke( new object?[] { Convert.ChangeType( value , col.GenericType ) } ) } );
            } else {

                p2.SetValue( col , p2.PropertyType.GetField( "NULL" ).GetValue( value ) );
            }

        }

        public static object GetValueFromColumn( EntityPrimaryKey key , PropertyInfo pi ) {

            ConstructibleColumn col = ( ConstructibleColumn ) ( pi?.GetValue( key.Table ) );
            var p2 = col.GetType().GetProperty( nameof( MySQLDefinitionColumn<SQLTypeWrapper<dynamic>>.TypeWrapperValue ) );
            var getter = p2.GetGetMethod( nonPublic: true );
            //var ctors = p2.PropertyType.GetConstructor( new Type[] { col.GenericType } );
            var x = getter.Invoke( col , new object?[] { } );

            return x;
        }


        public static List<FieldInfo> FieldsWhereIsAssignableFrom( Type FromType , Type SearchType ) => FromType.GetFields().Where( fi => SearchType.IsAssignableFrom( fi.FieldType ) ).ToList();

        public static List<FieldInfo> FieldsWhereIsAssignableFrom<T>( Type FromType ) {
            return FieldsWhereIsAssignableFrom( FromType , typeof( T ) );
        }


        public static void InstantiateDeclaredClassToField( AbstractEntity self , FieldInfo fieldInfo , Type[] ctorTypes , object[] ctorValues ) {

            var ctors = fieldInfo.FieldType.GetConstructor( ctorTypes );
            if ( ctors is not null ) {
                fieldInfo.SetValue( self , ctors.Invoke( ctorValues ) );
            } else {
                throw new ArgumentException( "Invalid declared class" );
            }


        }

        public static object? InvokeGenericFunction( Type GenericType , Type ClassTypeOfMethod , object InstanceClassTypeOfMethod , string MethodName , Type[] MethodTypeParams , object[] MethodParams ) {

            MethodInfo method = ClassTypeOfMethod.GetMethod( MethodName , MethodTypeParams );
            MethodInfo generic = method.MakeGenericMethod( GenericType );
            return generic?.Invoke( InstanceClassTypeOfMethod , MethodParams );      
        }

        public static object InvokeMethod( object obj , string methodName , params object[] methodParams ) {
            var methodParamTypes = methodParams?.Select( p => p.GetType() ).ToArray() ?? new Type[] { };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            MethodInfo method = null;
            var type = obj.GetType();
            while ( method == null && type != null ) {
                method = type.GetMethod( methodName , bindingFlags , Type.DefaultBinder , methodParamTypes , null );
                type = type.BaseType;
            }

            return method?.Invoke( obj , methodParams );
        }


        public static dynamic SQLTypeWrapperNULL( Type GenericType ) {

            var o = typeof( SQLTypeWrapper<> ).MakeGenericType( GenericType );
            var p = o.GetField( nameof( SQLTypeWrapper<object>.NULL ) , BindingFlags.Static | BindingFlags.Public );
            return p.GetValue( null );

        }

    }

}
