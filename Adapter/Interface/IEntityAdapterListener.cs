using prestoMySQL.Entity;

namespace prestoMySQL.Adapter.Interface {
    public interface IEntityAdapterListener<U> where U : GenericEntity {
        void onBindDataFrom( U entity );

        void onBindDataTo( U entity );

        void onInitData();

    }
}