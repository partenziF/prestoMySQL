using prestoMySQL.Column;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey {
    public class ForeignKeyLookup<T> : EntityForeignKey where T : AbstractEntity {

        List<T> leftTable = new List<T>();

        public ForeignKeyLookup( AbstractEntity e , string ForeignkeyName ) : base( e , ForeignkeyName ) {
        }

        public override void addEntities( List<AbstractEntity> foreignKeyTables ) {
            foreach ( AbstractEntity entityLeft in foreignKeyTables ) {
                if ( typeof( T ).IsAssignableFrom( entityLeft.GetType() ) ) {
                    leftTable.Add( ( T ) entityLeft );

                    foreach ( var (name, pi) in this.foreignKeyColumns ) {

                        ReflectionTypeHelper.AssignValue<T>( this , pi , ( T ) entityLeft );
                    }

                }
            }
        }

        //public override void createKey() {

        //    //List<object> result = new List<object>();

        //    foreach ( T entityLeft in leftTable ) {

        //        foreach ( var (name, pi) in this.foreignKeyColumns ) {
        //            //var col = pi.GetValue( entityLeft );
        //            //( ( IObservableColumn ) col ).Attach( this );
        //            ReflectionTypeHelper.AssignValue<T>( this , pi , entityLeft );
        //        }

        //        //                pi.SetValue( Table , v );         
        //        //EntityPrimaryKey pkLeft = entityLeft.PrimaryKey;
        //        //EntityPrimaryKey[] item = new EntityPrimaryKey[1];
        //        //item[0] = pkLeft;
        //        ////this.stackPrimaryKey.Push( item );
        //        //var v = pkLeft.getKeyValues();
        //        //result.AddRange( v );
        //    }


        //    //object[] result = base.createKey();

        //    //return result;
        //}

        public override object[] getKeyValues() {

            List<object> result = new List<object>();

            foreach ( T entityLeft in leftTable ) {

                var v = entityLeft.PrimaryKey.getKeyValues();
                result.AddRange( v );

            }

            return result.ToArray();
        }
    }
}
