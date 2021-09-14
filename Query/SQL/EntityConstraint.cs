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

    public static class FactoryEntityConstraint {

        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , MySQLQueryParam p , string aParamPlaceHolder = "" ) {


            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,
                                                                                            SQLBinaryOperator.equal() ,
                                                                                            new SQLQueryParams( new[] { p } ) ,
                                                                                            aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;

        }

        public static DefinableConstraint MakeConstraintEqual( PropertyInfo aColumnDefinition , SQLQueryParams mysqlParams , string aParamPlaceHolder = "" ) {

            //Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];
            //Type table, PropertyInfo p, EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = ""

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            //var param = new MySQLQueryParam( "test" , aColumnDefinition.ColumnName( null ) );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,
                                                                                           SQLBinaryOperator.equal() ,
                                                                                           mysqlParams,
                                                                                           aParamPlaceHolder
                                                                                          } ) ) ?? throw new ArgumentNullException();

            return o;
        }
        public static DefinableConstraint MakeConstraintEqual( dynamic aColumnDefinition , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , SQLBinaryOperator.equal() , new SQLQueryParams( new[] { ( MySQLQueryParam ) aColumnDefinition } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeAssignement( dynamic aColumnDefinition , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , new SQLQueryParams( new[] { ( MySQLQueryParam ) aColumnDefinition } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , object value , string aParamPlaceHolder = "" ) {


            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                new SQLQueryParams( new[] { new MySQLQueryParam( value , aColumnDefinition.ActualName ) } ) ,
                aParamPlaceHolder }
            ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeEqual<T>( dynamic aColumnDefinition , T value ) where T: SQLQuery {
            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                new SQLQueryParams( new[] { new MySQLQueryParam( value , aColumnDefinition.ActualName ) } ) ,
                "" }
            ) ) ?? throw new ArgumentNullException();

            return o;



        }


        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , string paramName , object value , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , new SQLQueryParams( new[] { new MySQLQueryParam( value , paramName ) } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }


        public static DefinableConstraint MakeAllEqual( dynamic aColumnDefinition , string paramName , object value , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            //types[1] = typeof( EvaluableBinaryOperator );
            types[1] = typeof( IQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityAllEqual<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition , new SQLQueryParams( new[] { new MySQLQueryParam( value , paramName ) } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }



    }


    public class EntityConstraint : GenericEntityConstraint {
        public EntityConstraint( PropertyInfo p , EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = "" ) : base( p , mBinaryOperator , aQueryPararms , paramPlaceHolder ) {

        }
    }

    public class EntityConstraint<T> : GenericEntityConstraint<T> where T : notnull {

        public EntityConstraint( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , aOperator , aQueryPararm , aParamPlaceHolder ) {
        }

    }


    public class ForeignkeyConstraint : GenericForeignKeyConstraint {
        public ForeignkeyConstraint( ForeignKey.ForeignKey foreignKey ) : base( foreignKey ) {
        }
    }


    public class EntityAssignement<T> : GenericEntityConstraint<T> where T : notnull {

        public EntityAssignement( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , SQLBinaryOperator.assign() , aQueryPararm , aParamPlaceHolder ) {
        }

        public new string ToString() {
            //string.IsNullOrWhiteSpace( columnDefinition.Table.TableAlias )
            //                ? string.Format( "{0}.{1}" , this.TableName.QuoteTableName() , aColumnName.QuoteColumnName() )
            //                : string.Format( "{0}.{1} AS {2}" , this.TableAlias.QuoteTableName() , aColumnName.QuoteColumnName() , String.Concat( "{" , this.ActualName , "}" , aColumnName ).QuoteColumnName() );

            if ( this.QueryParams[0].Value is null ) {
                if ( BinaryOperator.Equals( SQLBinaryOperator.equal() ) ) {
                    BinaryOperator = SQLBinaryOperator.@is();
                } else if ( BinaryOperator.Equals( SQLBinaryOperator.notEqual() ) ) {
                    BinaryOperator = SQLBinaryOperator.isNot();
                }
            }

            return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )}";
            //                $" columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) );
        }


    }

    public class EntityAllEqual<T> : GenericEntityConstraint<T> where T : notnull {

        public EntityAllEqual( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , SQLBinaryOperator.@in() , aQueryPararm , aParamPlaceHolder ) {
        }

        public new string ToString() {
            //string.IsNullOrWhiteSpace( columnDefinition.Table.TableAlias )
            //                ? string.Format( "{0}.{1}" , this.TableName.QuoteTableName() , aColumnName.QuoteColumnName() )
            //                : string.Format( "{0}.{1} AS {2}" , this.TableAlias.QuoteTableName() , aColumnName.QuoteColumnName() , String.Concat( "{" , this.ActualName , "}" , aColumnName ).QuoteColumnName() );

            return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} ( {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )} )";
            //                $" columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) );
        }


    }



}
