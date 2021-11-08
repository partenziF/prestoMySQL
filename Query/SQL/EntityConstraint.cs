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
            if ( p is null ) {
                throw new ArgumentNullException( nameof( p ) );
            }

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

        public static DefinableConstraint MakeCondition( dynamic aColumnDefinition , EvaluableBinaryOperator @operator , MySQLQueryParam p , string aParamPlaceHolder = "" ) {
            if ( p is null ) {
                throw new ArgumentNullException( nameof( p ) );
            }

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,
                                                                                            @operator,
                                                                                            new SQLQueryParams( new[] { p } ) ,
                                                                                            aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;

        }
        /*
        
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
*/
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

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            
            //Type myParameterizedSomeClass = typeof( EntityAssignement<> ).MakeGenericType( generic );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                SQLBinaryOperator.equal(),
                new SQLQueryParams( new[] { new MySQLQueryParam( value , aColumnDefinition.ActualName ) } ) ,
                aParamPlaceHolder }
            ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeCondition( dynamic aColumnDefinition , EvaluableBinaryOperator @operator , object value , string aParamPlaceHolder = "" ) {


            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                @operator,
                new SQLQueryParams( new[] { new MySQLQueryParam( value , aColumnDefinition.ActualName ) } ) ,
                aParamPlaceHolder }
            ) ) ?? throw new ArgumentNullException();

            return o;
        }


        public static DefinableConstraint MakeEqual<T>( dynamic aColumnDefinition , T sqlQuery ) where T : SQLQuery {
            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = sqlQuery.GetType();// typeof( SQLQuery );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntitySubQueryAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );


            //Important!
            sqlQuery.Build();


            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                sqlQuery,
                sqlQuery.getQueryParams(),
                //new SQLQueryParams( new[] { new MySQLQueryParam( "" , "test") } ) ,
                //sqlQueryParams,
                //new SQLQueryParams( new[] { new MySQLQueryParam( value , aColumnDefinition.ActualName ) } ) ,
                "" }
            ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , SQLQuery sqlQuery ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( SQLQuery );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntitySubQueryAssignement<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            //Important!
            sqlQuery.Build();

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] {
                aColumnDefinition ,
                sqlQuery,
                sqlQuery.getQueryParams(),
                //null,
                //sqlQueryParams,
                //new SQLQueryParams( new[] { new MySQLQueryParam( "" , "test") } ) ,
                "" }
            ) ) ?? throw new ArgumentNullException();

            return o;
        }


        public static DefinableConstraint MakeEqual( dynamic aColumnDefinition , string paramName , object value , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,
                SQLBinaryOperator.equal(),
                new SQLQueryParams( new[] { new MySQLQueryParam( value , paramName ) } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeBetween( dynamic aColumnDefinition , string paramName , object minValue,object maxValue , string aParamPlaceHolder = "" ) {
            //var cIntervalloOrdine = new BetweenEntityConstraint<DateTime>( EntitiesAdapter.Adapter<CustomerOrderEntityAdapter>().Entity.OrderCreated , new SQLQueryParams( new[] { new MySQLQueryParam( 1 , nameof( a.ArtistId ) + "1" ) , new MySQLQueryParam( 5 , nameof( a.ArtistId ) + "2" ) } ) , "@" )
            
            //(MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition, SQLQueryParams columnValues, string aParamPlaceholder = "" )

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[3];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( SQLQueryParams );
            types[2] = typeof( string );
            Type myParameterizedSomeClass = typeof( BetweenEntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,                
                new SQLQueryParams( new[] { new MySQLQueryParam( minValue , paramName ), new MySQLQueryParam( maxValue , paramName ) } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

            return o;
        }

        public static DefinableConstraint MakeLike( dynamic aColumnDefinition , string paramName , object value , string aParamPlaceHolder = "" ) {

            Type generic = aColumnDefinition.GetType().GetGenericArguments()[0].GetGenericArguments()[0];

            Type[] types = new Type[4];
            types[0] = ( aColumnDefinition.GetType() );
            types[1] = typeof( EvaluableBinaryOperator );
            types[2] = typeof( IQueryParams );
            types[3] = typeof( string );
            Type myParameterizedSomeClass = typeof( EntityConstraint<> ).MakeGenericType( generic );
            ConstructorInfo ctor = myParameterizedSomeClass.GetConstructor( types );

            DefinableConstraint o = ( DefinableConstraint ) ( ctor?.Invoke( new object[] { aColumnDefinition ,
                SQLBinaryOperator.like(),
                new SQLQueryParams( new[] { new MySQLQueryParam( value , paramName ) } ) , aParamPlaceHolder } ) ) ?? throw new ArgumentNullException();

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


    //public class EntityConstraint : GenericEntityConstraint {
    //    public EntityConstraint( PropertyInfo p , EvaluableBinaryOperator mBinaryOperator , IQueryParams aQueryPararms , string paramPlaceHolder = "" ) : base( p , mBinaryOperator , aQueryPararms , paramPlaceHolder ) {

    //    }
    //}


    public class EntityConstraint<T> : GenericEntityConstraint<T> where T : notnull {

        public EntityConstraint( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , aOperator , aQueryPararm , aParamPlaceHolder ) {
        }

    }


    public class ForeignkeyConstraint : GenericForeignKeyConstraint {
        public ForeignkeyConstraint( ForeignKey.ForeignKey foreignKey ) : base( foreignKey ) {
        }
    }



    public class EntityConstraintFunction<T> : GenericEntityConstraintFunction<T> where T : notnull {

        public EntityConstraintFunction( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , EvaluableBinaryOperator aOperator , IFunction aFunction ) :
            base( aColumnDefinition , aOperator , aFunction ) {
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

            //if ( this.QueryParams[0].Value is null ) {
            //    if ( BinaryOperator.Equals( SQLBinaryOperator.equal() ) ) {
            //        BinaryOperator = SQLBinaryOperator.@is();
            //    } else if ( BinaryOperator.Equals( SQLBinaryOperator.notEqual() ) ) {
            //        BinaryOperator = SQLBinaryOperator.isNot();
            //    }
            //}

            return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )}";
            //                $" columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) );
        }


    }

    public class EntitySubQueryAssignement<T> : GenericEntityConstraint<T> where T : notnull {

        public SQLQuery mSubQuery;

        public EntitySubQueryAssignement( MySQLDefinitionColumn<SQLTypeWrapper<T>> aColumnDefinition , SQLQuery subQuery , IQueryParams aQueryPararm , string aParamPlaceHolder = "" ) :
            base( aColumnDefinition , SQLBinaryOperator.assign() , aQueryPararm , aParamPlaceHolder ) {
            this.mSubQuery = subQuery;
        }

        public new string ToString() {

            //var xxxx =( (dynamic)mSubQuery).AsString();

            return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {mSubQuery.ToString()}";

            //return $"{columnDefinition.Table.ActualName.QuoteTableName()}.{columnDefinition.ColumnName.QuoteColumnName()} {BinaryOperator} {this.QueryParams[0].AsQueryParam( ParamPlaceHolder )}";
            //$" columnDefinition.ToString() , " " , BinaryOperator.ToString() , " " , this.QueryParams[0].AsQueryParam( ParamPlaceHolder ) );
        }


    }

    public class EntityExpression : DefinableConstraint {
        IFunction expression;
        public EntityExpression( IFunction expression )  {
            //this.BinaryOperator = mBinaryOperator;
            this.expression = expression;
            this.mParamPlaceHolder = String.Empty;
            this.mQueryParams = null;
        }

        private SQLQueryParams mQueryParams;
        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }
        public int countParam() {
            return 0;
        }

        public QueryParam[] getParam() {

            if ( this.expression is FunctionExpression ) {

                var q = new List<QueryParam>();

                if ( ( this.expression as FunctionExpression ).Left is FunctionParamSubQuery ) {
                    q.AddRange( ( ( ( this.expression as FunctionExpression ).Left as FunctionParamSubQuery ).SubQuery.getParam ) );

                }

                if ( ( this.expression as FunctionExpression ).Right is FunctionParamSubQuery ) {
                    q.AddRange( ( ( ( this.expression as FunctionExpression ).Right as FunctionParamSubQuery ).SubQuery.getParam ) );
                    
                }

                return q.ToArray();
                
            } else {
                return new QueryParam[] { };
            }
        }

        public string[] getParamAsString() {
            return new string[] { };
        }


        public override string ToString() {

            //return $"( {ColumnName} {BinaryOperator} {Function.ToString()}";
            return $"( {expression} )";
        }
    }


    public class EntityRawSqlQuery : DefinableConstraint {


        public string sqlQuery;
        private SQLQueryParams mQueryParams;

        public EntityRawSqlQuery( string sqlQuery ) {
            this.sqlQuery = sqlQuery;
            mQueryParams = new SQLQueryParams( new QueryParam[] { } );
        }
        public EntityRawSqlQuery( string sqlQuery , IQueryParams aQueryPararms ) {
            this.sqlQuery = sqlQuery;

            this.mQueryParams = ( SQLQueryParams ) ( aQueryPararms ?? throw new ArgumentNullException( nameof( aQueryPararms ) ) );
        }


        protected EvaluableBinaryOperator mBinaryOperator = SQLBinaryOperator.equal();
        public EvaluableBinaryOperator BinaryOperator { get => mBinaryOperator; set => mBinaryOperator = value; }


        protected string mParamPlaceHolder;
        public string ParamPlaceHolder { get => mParamPlaceHolder; set => mParamPlaceHolder = value; }

        public SQLQueryParams QueryParams { get => mQueryParams; set => mQueryParams = value; }

        public virtual int countParam() {
            return mQueryParams.asArray()?.Length ?? 0;
        }

        public virtual QueryParam[] getParam() {

            if ( this.QueryParams.asArray()?.Length > 0 ) {
                if ( this.QueryParams[0].Value.GetType().IsArray ) {

                    var l = ( ( Array ) this.QueryParams[0].Value ).Length;
                    var result = new QueryParam[l];
                    int i = 0;
                    foreach ( var v in ( Array ) this.QueryParams[0].Value ) {

                        //result[i] = new QueryParam( v , string.Format( "{0}_{1]" , this.QueryParams[0].Name , i )  );
                        result[i] = new MySQLQueryParam( v , String.Concat( this.QueryParams[0].Name , "_" , i.ToString() ) );
                        i++;
                    }

                    return result;

                } else {
                    return new QueryParam[] { ( QueryParam ) this.QueryParams[0] };
                }
            } else {
                return this.QueryParams.asArray();
            }
        }

        public virtual string[] getParamAsString() {
            if  ( this.mQueryParams.asArray()?.Length > 0 )
                return new string[] { this.QueryParams[0].AsQueryParam() };
            else
                return new string[] { };
        }

        public virtual object[] ColumnValue() {
            if ( this.mQueryParams.asArray()?.Length > 0 )
                return new object[] { ( object ) this.QueryParams[0].Value };
            else
                return new object[] { };
        }



        public override string ToString() {
            return sqlQuery;
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
