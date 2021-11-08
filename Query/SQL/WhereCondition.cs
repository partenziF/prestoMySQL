using prestoMySQL.Column;
using prestoMySQL.Extension;
using prestoMySQL.Query.Interface;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Query.SQL {


    public static class FactorySQLWhereCondition {

        public static DefinableConstraint MakeColumnEqual( GenericQueryColumn aColumnDefinition , MySQLQueryParam p , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( WhereCondition<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { p } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;

        }

        //public static DefinableConstraint MakeBetween( GenericQueryColumn aColumnDefinition , object valueMin , object valueMax , string aParamPlaceHolder = "" ) {
        //    Type generic = aColumnDefinition.GenericType;
        //    //SQLProjectionColumn<SQLTypeWrapper<T>> aColumnDefinition , SQLQueryParams columnValues , string aParamPlaceholder = "" 

        //    Type[] types = new Type[3];
        //    types[0] = ( aColumnDefinition.GetType() );
        //    types[1] = typeof( IQueryParams );
        //    types[2] = typeof( string );

        //    Type myParameterizedSomeClass = typeof( BetweenWhereCondition<> ).MakeGenericType( generic );
        //    ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );
        //    if ( ctor is null ) throw new NullReferenceException( "Costructor not found for type " + aColumnDefinition.GetType() );
        //    var pMin = new MySQLQueryParam( valueMin , aColumnDefinition.ActualName );
        //    var pMax = new MySQLQueryParam( valueMax , aColumnDefinition.ActualName );


        //    DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , new SQLQueryParams( new[] { pMin,pMax } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

        //    return o;

        //}
        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , object value , string aParamPlaceHolder = "" ) {

            //Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];
            Type generic = aColumnDefinition.GenericType;

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( WhereCondition<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );
            if ( ctor is null ) throw new NullReferenceException( "Costructor not found for type " + aColumnDefinition.GetType() );
            var p = new MySQLQueryParam( value , aColumnDefinition.ActualName );

            //WhereCondition( SQLProjectionColumn < SQLTypeWrapper < T >> aColumnDefinition , EvaluableBinaryOperator aOperator , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :


            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { p } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;

        }

        public static DefinableConstraint MakeAllEqual( dynamic aColumnDefinition , object value , string aParamPlaceHolder = "" ) {

            //Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];
            Type generic = aColumnDefinition.GenericType;

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( WhereAllEqual<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );
            if ( ctor is null ) throw new NullReferenceException( "Costructor not found for type " + aColumnDefinition.GetType() );
            var p = new MySQLQueryParam( value , aColumnDefinition.ActualName );

            //WhereCondition( SQLProjectionColumn < SQLTypeWrapper < T >> aColumnDefinition , EvaluableBinaryOperator aOperator , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :


            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , new SQLQueryParams( new[] { p } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;

        }


    }



    public class WhereCondition<T> : GenericWhereCondition<T> where T : notnull {

        public WhereCondition( SQLProjectionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , aOperator , aQueryPararm , aParamPlaceHolder ) {
        }

    }

    public class WhereAllEqual<T> : GenericWhereCondition<T> where T : notnull {

        public WhereAllEqual( SQLProjectionColumn<SQLTypeWrapper<T>> aColumnDefinition , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , SQLBinaryOperator.@in() , aQueryPararm , aParamPlaceHolder ) {
        }

        public new string ToString() {

            return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} ( {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )} )";

        }


    }

}
