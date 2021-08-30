using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Extension;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace prestoMySQL.Column {

    public interface IFunctionParam {

    }

    public class ColumnFunctionParam : IFunctionParam {
        private Type table;
        private string property;

        public ColumnFunctionParam( Type table , string property ) {
            this.table = table;
            this.property = property;
        }

        public override string ToString() {

            return Helper.SQLTableEntityHelper.getColumnName( table , property , true );
            //return $"{mColumn.Table.ActualName.QuoteTableName()}.{mColumn.ColumnName.QuoteColumnName()} ";
        }

    }

    public class ValueFunctionParam : IFunctionParam {

        private string mValue;

        public ValueFunctionParam( string mValue ) {
            this.mValue = mValue;
        }

        public override string ToString() {
            return mValue;
        }

    }

    public class QueryFunction<T> : GenericQueryColumn where T : ISQLTypeWrapper {

        public QueryFunction( string aDeclaredVariableName , PropertyInfo aPropertyInfo ) : base( aPropertyInfo ) {
            //protected DALFunctionParam[] mParams;


            DALProjectionFunction dalProjectionColumn = this.mPropertyInfo?.GetCustomAttribute<DALProjectionFunction>();
            if ( dalProjectionColumn == null ) throw new ArgumentNullException( String.Format( "DALProjectionColumn attribute is required for {0}" , aPropertyInfo.Name ) );

            var dalFunctionParams = ( this.mPropertyInfo?.GetCustomAttributes<DALFunctionParam>() );

            mFunctionParams = new List<IFunctionParam>();

            foreach ( var dalFunctionParam in dalFunctionParams ) {
                //Console.WriteLine( dalFunctionParam );
                if ( dalFunctionParam.GetType() == typeof( DALValueFunctionParam ) ) {
                    mFunctionParams.Add( new ValueFunctionParam( (( DALValueFunctionParam ) dalFunctionParam).Value ) );
                } else if ( dalFunctionParam.GetType() == typeof( DALColumnFunctionParam ) ) {
                    
                    mFunctionParams.Add( new ColumnFunctionParam( ( dalFunctionParam as DALColumnFunctionParam ).Table , ( dalFunctionParam as DALColumnFunctionParam ).Property ));
                }

            }


            mSQLDataType = ( MySQLDataType ) ( dalProjectionColumn as DALProjectionFunction ).DataType;
            mFunctionName = ( dalProjectionColumn as DALProjectionFunction ).Function;
            mColumnAlias = dalProjectionColumn.Alias;

            if ( string.IsNullOrEmpty( aDeclaredVariableName ) ) {
                throw new ArgumentException( $"'{nameof( aDeclaredVariableName )}' non può essere null o vuoto." , nameof( aDeclaredVariableName ) );
            }

            mDeclaredVariableName = aDeclaredVariableName;

            mGenericType = typeof( T ).GetGenericArguments()[0];
        }

        private readonly string mDeclaredVariableName;

        private readonly Type mGenericType;
        public override Type GenericType => mGenericType;


        private readonly string mFunctionName;
        public string FunctionName => mFunctionName;


        public override string ColumnAlias => mColumnAlias;

        public override string ActualName => mColumnAlias ?? mFunctionName;


        private List<IFunctionParam> mFunctionParams;


        public List<IFunctionParam> FunctionParams {
            set => mFunctionParams = value;
            get => mFunctionParams;
        }
    }




    public class SQLProjectionFunction<T> : QueryFunction<T> where T : ISQLTypeWrapper {
        public SQLProjectionFunction( string aDeclaredVariableName , PropertyInfo aMethodBase ) : base( aDeclaredVariableName , aMethodBase ) {
        }


        public override string ToString() {
            if ( this.FunctionParams is null ) {
                return $"{this.FunctionName}() AS {this.ActualName}";
            } else {
                return $"{this.FunctionName}({string.Join( "," , this.FunctionParams )}) AS {this.ActualName}";
            }

        }
    }

}
