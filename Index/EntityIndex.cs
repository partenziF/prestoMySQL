using DatabaseEntity.EntityAdapter;
using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.Index;
using prestoMySQL.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Index {
    public class EntityIndex : TableIndex, ITableIndex {

        private readonly int mKeyLength;

        public EntityIndex( string Name , AbstractEntity aTableEntity ) {
            
            this.Table = aTableEntity;

            try {

                List<PropertyInfo> l = SQLTableEntityHelper.getPropertyIfIndex( aTableEntity.GetType() );

                foreach ( PropertyInfo p in l ) {
                    var a = p.GetCustomAttribute<DDIndexAttribute>();
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

        public int KeyLength { get => mKeyLength; }


        public void setKeyValue<T>( string aKey , T aValue ) {

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


        //public virtual string[] ColumnsName {
        //    get {
        //        return this.IndexColumns.Keys.ToArray();
        //    }
        //}


    }

}
