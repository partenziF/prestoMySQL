using prestoMySQL.Entity;
using System;

namespace prestoMySQL.Adapter.Interface {
    public interface IEntityAdapterListener<U> where U : AbstractEntity {

        public void onBindDataFrom( object sender , BindDataFromEventArgs<U> e );

        public void onBindDataTo( object sender ,  BindDataToEventArgs<U> e );

        public void onInitData( object sender , EventArgs e );


    }
}