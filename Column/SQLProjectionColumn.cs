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

    public interface ValuableQueryColumn<T> {

        //private T mTypeWrapperValue;
        public T TypeWrapperValue { get; set; }

        public object Value();

    }

    public abstract class GenericQueryColumn : QueryableColumn {

        public GenericQueryColumn( PropertyInfo aPropertyInfo = null ) {

            //Type entity;

            this.mPropertyInfo = aPropertyInfo;

            //DALQueryEntity dalQueryEntity = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALQueryEntity>();
            //if ( dalQueryEntity == null ) throw new ArgumentNullException();

            //DALProjectionColumn dalProjectionColumn = this.mPropertyInfo?.GetCustomAttribute<DALProjectionColumn>();
            //if ( dalProjectionColumn == null ) throw new ArgumentNullException( String.Format( "DALProjectionColumn attribute is required for {0}" , aPropertyInfo.Name ) );

            //if ( ( dalProjectionColumn.Entity != null ) && ( dalProjectionColumn.Entity != dalQueryEntity.Entity ) ) {
            //    entity = dalProjectionColumn.Entity;
            //} else {
            //    entity = dalQueryEntity.Entity;
            //}


            //DALTable dalTable = entity?.GetCustomAttribute<DALTable>();
            //if ( dalTable == null ) throw new ArgumentNullException();
            //mTable = new TableReference( dalTable.TableName );

            //DALQueryEntity dalQueryEntity = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALQueryEntity>();
            //if ( dalQueryEntity == null ) throw new ArgumentNullException();

            //DALProjectionColumn dalProjectionColumn = this.mPropertyInfo?.GetCustomAttribute<DALProjectionColumn>();
            //if ( dalProjectionColumn == null ) throw new ArgumentNullException( String.Format( "DALProjectionColumn attribute is required for {0}" , aPropertyInfo.Name ) );


            //if ( ( dalProjectionColumn.Entity != null ) && ( dalProjectionColumn.Entity != dalQueryEntity.Entity ) )  {
            //    entity = dalProjectionColumn.Entity;
            //} else {
            //    entity = dalQueryEntity.Entity;
            //}

            //DALTable dalTable = entity?.GetCustomAttribute<DALTable>();

            //if ( dalTable == null ) throw new ArgumentNullException();
            //mTable = new TableReference( dalTable.TableName );

            //mSQLDataType = ( MySQLDataType ) ( dalProjectionColumn as DALProjectionColumn ).DataType;

            //mColumnName = dalProjectionColumn.Name;
            //mColumnAlias = dalProjectionColumn.Alias;

        }


        protected PropertyInfo mPropertyInfo;


        public abstract Type GenericType { get; }

        //internal readonly MySQLDataType mSQLDataType;
        internal MySQLDataType mSQLDataType;

        //protected readonly TableReference mTable;

        //public abstract TableReference Table { get; }


        //protected readonly string mColumnName;
        //public abstract string ColumnName { get; }


        internal string mColumnAlias;
        //internal readonly string mColumnAlias;

        public abstract string ColumnAlias { get; }

        public abstract string ActualName { get; }


        protected  TableReference mTable;
        public TableReference Table => mTable;

        public abstract void AssignValue( object x );


    }

    public class QueryColumn<T> : GenericQueryColumn, ValuableQueryColumn<T> where T : ISQLTypeWrapper {
        public QueryColumn( string aDeclaredVariableName , PropertyInfo aPropertyInfo = null ) : base( aPropertyInfo ) {

            Type entity;

            //this.mPropertyInfo = aPropertyInfo;

            DALQueryEntity dalQueryEntity = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALQueryEntity>();
            if ( dalQueryEntity == null ) throw new ArgumentNullException();

            DALProjectionColumn dalProjectionColumn = this.mPropertyInfo?.GetCustomAttribute<DALProjectionColumn>();
            if ( dalProjectionColumn == null ) throw new ArgumentNullException( String.Format( "DALProjectionColumn attribute is required for {0}" , aPropertyInfo.Name ) );


            if ( ( dalProjectionColumn.Entity != null ) && ( dalProjectionColumn.Entity != dalQueryEntity.Entity ) ) {
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

            if ( string.IsNullOrEmpty( aDeclaredVariableName ) ) {
                throw new ArgumentException( $"'{nameof( aDeclaredVariableName )}' non può essere null o vuoto." , nameof( aDeclaredVariableName ) );
            }

            mDeclaredVariableName = aDeclaredVariableName;
            if ( typeof( T ).GetGenericArguments()[0].IsGenericType)
                mGenericType = typeof( T ).GetGenericArguments()[0].GetGenericArguments()[0];
            else
            mGenericType = typeof( T ).GetGenericArguments()[0];

        }

        private Type mGenericType;


        private readonly string mDeclaredVariableName;


        //public abstract TableReference Table { get; }


        protected readonly string mColumnName;
        //public abstract string ColumnName { get; }

        //public override TableReference Table => mTable;

        //protected readonly TableReference mTable;
        //public TableReference Table => mTable;

        public string ColumnName => mColumnName;
        //public override string ColumnName => mColumnName;

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


        public override void AssignValue( object x ) {

            //var genericType = typeof( T ).GetGenericArguments()[0];
            var genericType = mGenericType;
            ConstructorInfo ctor = typeof( T ).GetConstructor( new Type[] { genericType } );
            TypeWrapperValue = ( T ) ctor?.Invoke( new object[] { Convert.ChangeType( x , genericType ) } );

        }

        public string AsCondition() {
            return this.Table.getResultColumn( ColumnName , "" );
            //return this.getTableReference().getResultColumn( getColumnName() , getAlias() );
        }


    }




    public class SQLProjectionColumn<T> : QueryColumn<T> where T : ISQLTypeWrapper {

        public SQLProjectionColumn( string aDeclaredVariableName , PropertyInfo aMethodBase ) : base( aDeclaredVariableName , aMethodBase ) {
            
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
