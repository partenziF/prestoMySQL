using prestoMySQL.SQL.Interface;

namespace prestoMySQL.Query.Interface {
    public interface IQueryParam {
		
        object Value { get; }

        object GetValue();

        string Name { get; }

        string AsQueryParam( string aPlaceholder = "" );
    }
}
