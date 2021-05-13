using prestoMySQL.Entity;
using prestoMySQL.Entity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Extension {
    public static class GenericEntityAttributeExtension {
        public static DALTable GetAttributeDALTable( this GenericEntity entity ) {

            if ( entity is null ) {
                throw new ArgumentNullException( nameof( entity ) );
            }

            return (DALTable) Attribute.GetCustomAttribute( entity.GetType() , typeof( DALTable ) );

        }

        public static DALTable GetAttributeDALTable<T>() where T : GenericEntity => (DALTable) Attribute.GetCustomAttribute( typeof( T ) , typeof( DALTable ) );
        


    }
}
