using DatabaseEntity.EntityAdapter;
using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.Index;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Entity {

    public interface ITableIndex {

        public AbstractEntity Table { get; set; }

        public PropertyInfo this[string index] { get; }

    }

    public class EntityUniqueIndex : TableIndex, ITableIndex {


        private readonly int mKeyLength;
        public int KeyLength { get => mKeyLength; }

        public EntityUniqueIndex( string Name , AbstractEntity aTableEntity ) : base() {
            this.Table = aTableEntity;

            try {

                List<PropertyInfo> l = SQLTableEntityHelper.getPropertyIfUniqueIndex( aTableEntity.GetType() );

                foreach ( PropertyInfo p in l ) {
                    var a = p.GetCustomAttribute<DDUniqueIndexAttribute>();
                    if ( ( a != null ) && ( a.Name == Name ) ) {
                        var aa = p.GetCustomAttribute<DDColumnAttribute>();
                        this.IndexColumns.Add( aa.Name , p );
                    }
                }

            } catch ( System.Exception e ) {
                throw new System.Exception( "Error while initialize primarykey" );
            }

            mKeyLength = IndexColumns.Count;
        }



        public void setKeyValue<T>( string aKey , T aValue ) where T : notnull {

            try {

                if ( IndexColumns.ContainsKey( aKey ) ) {

                    System.Reflection.PropertyInfo f = this.IndexColumns[aKey];

                    try {

                        var col = ( MySQLDefinitionColumn<SQLTypeWrapper<T>> ) f.GetValue( this.Table );
                        col.TypeWrapperValue = ( aValue );

                    } catch ( System.Exception e ) {
                        throw new System.Exception( "Error while " + nameof( setKeyValue ) );
                    }

                } else {
                    throw new System.Exception( "Field name " + aKey + " not exists" );
                }

            } catch ( System.InvalidCastException e ) {
                throw new System.InvalidCastException( "Cannot be cast " + e.ToString() );
            }

        }

        public virtual void setKeyValues( params object[] values ) {

            //MethodBase.GetCurrentMethod().DeclaringType.FullName

            if ( values.Length != KeyLength )
                throw new System.Exception( "Invalid key length" );
            int i = 0;
            foreach ( var kv in IndexColumns ) {
                var f = kv.Value;
                try {

                    var col = f.GetValue( this.Table );

                    var t = ( ( ConstructibleColumn ) col ).GenericType;
                    MethodInfo method = this.GetType().GetMethod( nameof( setKeyValue ) ); // , new Type[] { typeof( string ) , t } 
                    MethodInfo generic = method.MakeGenericMethod( t );
                    generic.Invoke( this , new object[] { kv.Key , values[i++] } );
                        

                    //var mi = typeof(setKeyValue);
                    //var r = mi.Invoke( this, new object[] { kv.Key,values[i++]} );
                    //setKeyValue<int>(kv.Key,va)

                    //col.TypeWrapperValue = values[i++];

                } catch ( System.InvalidCastException e ) {
                    throw new System.InvalidCastException( "Cannot be cast " + e.ToString() );
                }

            }
        }




        public virtual string[] ColumnsName {
            get {
                return this.IndexColumns.Keys.ToArray();
            }
        }

    }

}