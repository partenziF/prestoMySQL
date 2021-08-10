using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Column {

    public abstract class GenericQueryColumn : QueryableColumn {

        public GenericQueryColumn( PropertyInfo aPropertyInfo = null ) {

            Type entity;

            this.mPropertyInfo = aPropertyInfo;

            DALQueryEntity dalQueryEntity = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALQueryEntity>();
            if ( dalQueryEntity == null ) throw new ArgumentNullException();

            DALProjectionColumn dalProjectionColumn = this.mPropertyInfo?.GetCustomAttribute<DALProjectionColumn>();
            if ( dalProjectionColumn == null ) throw new ArgumentNullException( String.Format( "DALProjectionColumn attribute is required for {0}" , aPropertyInfo.Name ) );

            //if (!string.IsNullOrWhiteSpace(dalProjectionColumn.Table)) {
            //} else 

            if ( ( dalProjectionColumn.Entity != null ) && ( dalProjectionColumn.Entity != dalQueryEntity.Entity ) )  {
                entity = dalProjectionColumn.Entity;
            } else {
                entity = dalQueryEntity.Entity;
            }

            DALTable dalTable = entity?.GetCustomAttribute<DALTable>();
            
            if ( dalTable == null ) throw new ArgumentNullException();
            mTable = new TableReference( dalTable.TableName );

            mSQLDataType = ( MySQLDataType ) ( dalProjectionColumn as DALProjectionColumn ).DataType;

            mColumnName = dalProjectionColumn.Name;
            mColumnAlias = dalProjectionColumn.Alias;

        }


        protected PropertyInfo mPropertyInfo;


        public abstract Type GenericType { get; }

        private readonly MySQLDataType mSQLDataType;

        protected readonly TableReference mTable;

        public abstract TableReference Table { get; }


        protected readonly string mColumnName;
        public abstract string ColumnName { get; }


        internal readonly string mColumnAlias;

        public abstract string ColumnAlias { get; }

        public abstract string ActualName { get; }

    }

    public class QueryColumn<T> : GenericQueryColumn where T : ISQLTypeWrapper {
        public QueryColumn( string aDeclaredVariableName , PropertyInfo aMethodBase = null ) : base( aMethodBase ) {

            if ( string.IsNullOrEmpty( aDeclaredVariableName ) ) {
                throw new ArgumentException( $"'{nameof( aDeclaredVariableName )}' non può essere null o vuoto." , nameof( aDeclaredVariableName ) );
            }

            mDeclaredVariableName = aDeclaredVariableName;

            mGenericType = typeof( T ).GetGenericArguments()[0];

        }

        private Type mGenericType;


        private readonly string mDeclaredVariableName;


        public override TableReference Table => mTable;

        public override string ColumnName => mColumnName;

        public override Type GenericType => mGenericType;

        public override string ColumnAlias => mColumnAlias;


        private T mTypeWrapperValue;
        public T TypeWrapperValue { get => mTypeWrapperValue; set => SetValue( value ); }

        public override string ActualName => mColumnAlias ?? mColumnName;

        private void SetValue( T value ) {

            this.mTypeWrapperValue = value;

        }

        public object Value() {
            if ( mTypeWrapperValue.IsNull )
                return null;
            else
                return ( ( object ) ( ( dynamic ) mTypeWrapperValue ).Value );
        }


        public void AssignValue( object x ) {

            var genericType = typeof( T ).GetGenericArguments()[0];
            ConstructorInfo ctor = typeof( T ).GetConstructor( new Type[] { genericType } );
            TypeWrapperValue = ( T ) ctor?.Invoke( new object[] { Convert.ChangeType( x , genericType ) } );

        }

        public string AsCondition() {
            return this.Table.getResultColumn( ColumnName , "" );
            //return this.getTableReference().getResultColumn( getColumnName() , getAlias() );
        }


    }




    public class SQLProjectionColumn<T> : QueryColumn<T> where T : ISQLTypeWrapper {

        public SQLProjectionColumn( string aDeclaredVariableName , PropertyInfo aMethodBase , SQLQuery sqlQuery ) : base( aDeclaredVariableName , aMethodBase ) {

        }



        public override string ToString() {
            return this.Table.getResultColumn( ColumnName , ColumnAlias );
            //return this.getTableReference().getResultColumn( getColumnName() , getAlias() );
        }

        public static implicit operator SQLProjectionColumn<T>( MySQLDefinitionColumn<SQLTypeWrapper<uint>> v ) {
            throw new NotImplementedException();
        }
        //Se devo usarlo in una condizione non serve l'alias

    }


}
