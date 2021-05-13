using prestoMySQL.Entity.Attributes;
using prestoMySQL.Entity.Interface;
using prestoMySQL.Extension;
using prestoMySQL.PrimaryKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Entity {
    // System.Collections.Generic.IDictionary<String,object> Map<String,object>
    public abstract class GenericEntity : AvailableEntity {

        EntityPrimaryKey mPrimaryKey;
        public EntityPrimaryKey PrimaryKey { get => mPrimaryKey; }

        public void setPrimaryKey( EntityPrimaryKey aPrimaryKey,DelegateCreatePrimaryKey aDelegatorCreatePrimaryKey ) {
            this.mPrimaryKey = aPrimaryKey;
            this.mPrimaryKey.setDoCreatePrimaryKey( aDelegatorCreatePrimaryKey );
        }

        public GenericEntity setCreatePrimaryKey( DelegateCreatePrimaryKey delegatorCreatePrimaryKey ) {
            this.mPrimaryKey.setDoCreatePrimaryKey( delegatorCreatePrimaryKey );
            return this;
        }


        private string mTableName;
        public string TableName { get => this.mTableName; set => this.mTableName = value; }


        public GenericEntity() {
                                   
            TableName = this.GetAttributeDALTable()?.TableName ?? throw new ArgumentNullException( "DALTable" , "Table name entity not found" ); 

            /*
                    foreignKey = this.createForeignKey();
            
                    this.mPrimaryKey = this.createPrimaryKey();

            */

        }

        public abstract EntityPrimaryKey createPrimaryKey();


    }
}
