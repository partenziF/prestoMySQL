using prestoMySQL.Adapter;
using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.DataType;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Extension;
using prestoMySQL.Query;
using prestoMySQL.Query.Attribute;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace prestoMySQL.Column {

    public interface IFunction {
        public QueryParam[] getParam();
    }

    public interface IFunctionParam {
        public QueryParam[] getParam();
    }

    public interface IFunctionTableProperty : IFunctionParam {
        public TableReference Table { get; }

    }

    public abstract class FunctionTableProperty : IFunctionTableProperty {

        internal Type tableType;
        protected string property;
        internal TableReference mTableReference;

        protected FunctionTableProperty( Type table , string property ) {
            this.tableType = table;
            this.property = property;
            this.mTableReference = Helper.SQLTableEntityHelper.getTableReference( table );
        }

        protected FunctionTableProperty( TableReference mTableReference , string property ) {
            this.tableType = null;
            this.property = property;
            this.mTableReference = mTableReference;
        }

        public TableReference Table => mTableReference;

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }
    }

    public class GenericFunction : IFunction {
        public GenericFunction( string functionName , params IFunctionParam[] functionParams ) {
            FunctionName = functionName;
            FunctionParams = functionParams;
        }

        public string FunctionName { get; }
        public IFunctionParam[] FunctionParams { get; }

        public override string ToString() {
            if ( FunctionParams.Length > 0 )
                return $"{FunctionName}({string.Join( "," , FunctionParams.ToList() )})";
            else
                return $"{FunctionName}()";
        }

        public QueryParam[] getParam() {
            List<QueryParam> result = new List<QueryParam>();
            if ( FunctionParams.Length > 0 )
                foreach ( var p in FunctionParams ) {
                    result.AddRange( p.getParam() );
                }
            else
                return new QueryParam[] { };

            return result.ToArray();
        }



    }

    public class FunctionExpression : IFunction {
        public FunctionExpression( string @operator , IFunctionParam left , IFunctionParam right ) {
            Operator = @operator;
            Left = left;
            Right = right;
        }

        public string Operator { get; }
        public IFunctionParam Left { get; }
        public IFunctionParam Right { get; }

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

        public override string ToString() {
            return $"( {Left.ToString()} {Operator} {Right.ToString()} )";
        }

    }

    public class FunctionExtract : IFunction {
        public FunctionExtract( string functionName , string unit , params IFunctionParam[] functionParams ) {

            FunctionName = functionName;
            FunctionParams = functionParams;
            Unit = unit;
        }
        public string Unit { get; }
        public string FunctionName { get; }
        public IFunctionParam[] FunctionParams { get; }

        public override string ToString() {
            if ( FunctionParams.Length > 0 )
                return $"{FunctionName}({Unit} FROM {string.Join( "," , FunctionParams.ToList() )})";
            else
                return $"{FunctionName}()";
        }

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

    }



    public class FunctionParamConstraint : FunctionTableProperty {
        //private Type table;
        //private string property;
        private object value;

        public FunctionParamConstraint( Type table , string property , object value ) : base( table , property ) {
            //this.table = table;
            //this.property = property;
            //this.mTableReference = Helper.SQLTableEntityHelper.getTableReference( table );
            this.value = value;
        }

        public override string ToString() {

            if ( this.value is null ) {
                return $"{this.mTableReference.ActualName.QuoteTableName()}.{property.QuoteColumnName()} IS NULL";
                //return $"{Helper.SQLTableEntityHelper.getColumnName( table , property , true )} IS NULL";
            } else {
                return $"{this.mTableReference.ActualName.QuoteTableName()}.{property.QuoteColumnName()} = {value.ToString()}";
                //return $"{Helper.SQLTableEntityHelper.getColumnName( table , property , true )} = {value.ToString()}";
            }

            //return $"{mColumn.Table.ActualName.QuoteTableName()}.{mColumn.ColumnName.QuoteColumnName()} ";
        }

    }

    public class FunctionParamProperty<T> : FunctionTableProperty {

        MySQLDefinitionColumn<SQLTypeWrapper<T>> property;
        public FunctionParamProperty( MySQLDefinitionColumn<SQLTypeWrapper<T>> property ) : base( property.Table.GetType() , property.ActualName ) {
            this.property = property;
            
        }


    }

    public class FunctionParamProperty : FunctionTableProperty {
        private dynamic mColumnDefinition;

        //private Type table;
        //private string property;

        public FunctionParamProperty( Type table , string property ) : base( table , property ) {
            //this.table = table;
            //this.property = property;
            //this.mTableReference = Helper.SQLTableEntityHelper.getTableReference( table );
        }

        public FunctionParamProperty( ConstructibleColumn columnDefinition  ) : base( columnDefinition.Table , columnDefinition.ActualName ) {
            this.mColumnDefinition = columnDefinition;
            //this.table = table;
            //this.property = property;
            //this.mTableReference = Helper.SQLTableEntityHelper.getTableReference( table );
        }

        public override string ToString() {
            return $"{this.mTableReference.ActualName.QuoteTableName()}.{property.QuoteColumnName()}";
            //return Helper.SQLTableEntityHelper.getColumnName( table , property , true );
            //return $"{mColumn.Table.ActualName.QuoteTableName()}.{mColumn.ColumnName.QuoteColumnName()} ";
        }

    }


    public class FunctionParamQueryParam : IFunctionParam {

        private object mValue;
        private string mName;
        private MySQLQueryParam mQueryParam;
        private string mParamPlaceHolder;

        public FunctionParamQueryParam( string mName , object mValue, string mParamPlaceHolder = "@" ) {
            this.mValue = mValue;
            this.mName = mName;
            this.mParamPlaceHolder = mParamPlaceHolder;
            mQueryParam = new MySQLQueryParam( mValue , mName );
        }

        public override string ToString() {
            //if ( mValue.IsLitteral() ) {
            //return $"'@{mValue.ToString()}'";
            return mQueryParam.AsQueryParam( mParamPlaceHolder );
            //return mQueryParam.ToString();
            //} else {
            //  return mValue.ToString();
            //}
        }

        public QueryParam[] getParam() {
            return new QueryParam[] { mQueryParam };
        }


    }

    public class FunctionParamConstant : IFunctionParam {

        private object mValue;

        public FunctionParamConstant( object mValue ) {
            this.mValue = mValue;
        }

        public override string ToString() {
            if ( mValue.IsLitteral() ) {
                return $"'{mValue.ToString()}'";
            } else {
                return mValue.ToString();
            }
        }

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

    }

    public class FunctionParamFunction : IFunctionParam {

        internal IFunctionParam[] Expression;
        private string Function;

        public FunctionParamFunction( string function , params IFunctionParam[] expression ) {
            this.Function = function;
            if ( expression is not null ) {
                this.Expression = expression;
            }
        }

        public override string ToString() {
            if ( this.Expression is not null )
                return $"{this.Function}({string.Join( "," , this.Expression.Select( s => s.ToString() ).ToArray() ) })";
            else
                return $"{this.Function}()";
        }
        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

    }

    public class FunctionParamBetween : IFunctionParam {
        private IFunctionParam expression;
        private IFunctionParam minValue;
        private IFunctionParam maxValue;

        public FunctionParamBetween( IFunctionParam expression , IFunctionParam minValue , IFunctionParam maxValue ) {
            this.expression = expression;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string ToString() {
            return $"( {this.expression} BETWEEN {this.minValue} AND {this.maxValue} )";
        }

        public QueryParam[] getParam() {

            var result = new List<QueryParam>();
            result.AddRange( expression.getParam() );
            result.AddRange( minValue.getParam() );
            result.AddRange( maxValue.getParam() );

            return result.ToArray();
        }

    }

    public class FunctionParamExpression : IFunctionParam {
        private string @operator;
        private IFunctionParam left;
        private IFunctionParam right;

        public FunctionParamExpression( IFunctionParam left , IFunctionParam right , string @operator ) {
            this.@operator = @operator;
            this.left = left;
            this.right = right;
        }

        public override string ToString() {
            return $"( {this.left} {this.@operator} {this.right} )";
        }

        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

    }

    public class FunctionParamSubQuery : IFunctionParam {

        private SQLQuery mSubQuery;

        public FunctionParamSubQuery( SQLQuery subQuery ) {
            this.mSubQuery = subQuery;
        }

        public SQLQuery SubQuery { get => this.mSubQuery; }

        public override string ToString() {
            this.mSubQuery.Build();
            return this.mSubQuery.ToString();
        }
        public QueryParam[] getParam() {
            return new QueryParam[] { };
        }

    }



    public static class FunctionFactory {

        public static IFunction Create( DALQueryJoinEntityExpression param , params IFunctionParam[] functionParams ) {

            if ( ( Type ) param.TypeId == typeof( DALQueryJoinEntityExpression ) ) {
                return new FunctionExpression( ( param as DALQueryJoinEntityExpression ).@operator , functionParams[0] , functionParams[1] );
            } else {
                throw new ArgumentException( "Invalid param " );
            }
        }


        public static IFunction Create( DALProjectionFunction param , params IFunctionParam[] functionParams ) {
            if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_IF ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_CURDATE ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_EXTRACT ) ) {
                return new FunctionExtract( ( param as DALProjectionFunction ).Function , ( param as DALProjectionFunction_EXTRACT ).Unit , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_MAX ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_MIN ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_DATE ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_COUNT ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_SUM ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_DATE_FORMAT ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunctionExpression ) ) {
                return new FunctionExpression( ( param as DALProjectionFunctionExpression ).@operator , functionParams[0] , functionParams[1] );
            } else {
                throw new ArgumentException( "Invalid param " );
            }
        }

    }



    public static class FunctionParamFactory {
        public static IFunctionParam Create( DALFunctionParam param , ref int baseIndex , DALFunctionParam[] functionParams ) {

            if ( ( Type ) param.TypeId == typeof( DALFunctionParamConstraint ) ) {

                return new FunctionParamConstraint( ( param as DALFunctionParamConstraint ).Table , ( param as DALFunctionParamConstraint ).Property , ( param as DALFunctionParamConstraint ).Value );

            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamProperty ) ) {

                return new FunctionParamProperty( ( param as DALFunctionParamProperty ).Table , ( param as DALFunctionParamProperty ).Property );

            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamQueryParam ) ) {

                var t = ( param as DALFunctionParamQueryParam ).DataType;
                
                if ( ( param as DALFunctionParamQueryParam ).Value is not null ) {
                    if ( ( param as DALFunctionParamQueryParam ).ParamPlaceHolder is null ) {
                        return new FunctionParamQueryParam( ( param as DALFunctionParamQueryParam ).ParamName , ( param as DALFunctionParamQueryParam ).Value.ConvertTo( t ) );
                    } else {
                        return new FunctionParamQueryParam( ( param as DALFunctionParamQueryParam ).ParamName , ( param as DALFunctionParamQueryParam ).Value.ConvertTo( t ), ( param as DALFunctionParamQueryParam ).ParamPlaceHolder  );
                    }
                } else {
                    if ( ( param as DALFunctionParamQueryParam ).ParamPlaceHolder is null ) {
                        return new FunctionParamQueryParam( ( param as DALFunctionParamQueryParam ).ParamName , null );
                    } else {
                        return new FunctionParamQueryParam( ( param as DALFunctionParamQueryParam ).ParamName , null , ( param as DALFunctionParamQueryParam ).ParamPlaceHolder );
                    }
                }


            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamConstant ) ) {

                return new FunctionParamConstant( ( param as DALFunctionParamConstant ).Value );

            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamExpression ) ) {

                var fp = functionParams.Skip( baseIndex ).Take( param.CountParam() ).ToArray();
                baseIndex += param.CountParam();

                var pp = new List<DALFunctionParam>();

                var leftExpression = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamExpression ).leftExpression ) && ( !pp.Contains( x ) ) );
                if ( leftExpression is not null ) pp.Add( leftExpression );

                var rightExpression = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamExpression ).rightExpression ) && ( !pp.Contains( x ) ) );
                if ( rightExpression is not null ) pp.Add( rightExpression );

                //var sOperator = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamExpression ). ) && ( !pp.Contains( x ) ) );

                var ppp = new List<IFunctionParam>();

                foreach ( var x in pp ) {
                    ppp.Add( FunctionParamFactory.Create( x , ref baseIndex , functionParams ) );
                }

                return new FunctionParamExpression( ppp[0] , ppp[1] , ( param as DALFunctionParamExpression ).@operator );


            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamFunction ) ) {

                var fp = functionParams.Skip( baseIndex ).Take( param.CountParam() ).ToArray();
                baseIndex += param.CountParam();

                var pp = new List<DALFunctionParam>();

                foreach ( var prm in fp ) {

                    var expression = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == prm.GetType() ) && ( !pp.Contains( x ) ) );
                    if ( expression is not null ) pp.Add( expression );

                }


                var ppp = new List<IFunctionParam>();

                foreach ( var x in pp ) {
                    ppp.Add( FunctionParamFactory.Create( x , ref baseIndex , functionParams ) );
                }

                return new FunctionParamFunction( ( param as DALFunctionParamFunction ).Function , ppp.ToArray() );

            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamBetween ) ) {

                var fp = functionParams.Skip( baseIndex ).Take( param.CountParam() ).ToArray();
                baseIndex += param.CountParam();

                var pp = new List<DALFunctionParam>();

                var expression = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamBetween ).Expression ) && ( !pp.Contains( x ) ) );
                if ( expression is not null ) pp.Add( expression );

                var minValue = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamBetween ).MinValue ) && ( !pp.Contains( x ) ) );
                if ( minValue is not null ) pp.Add( minValue );

                var maxValue = fp.FirstOrDefault( x => ( ( Type ) x.TypeId == ( param as DALFunctionParamBetween ).MaxValue ) && ( !pp.Contains( x ) ) );
                if ( maxValue is not null ) pp.Add( maxValue );
                if ( param.CountParam() != pp.Count ) throw new ArgumentException( "Invalid argument count" );


                var ppp = new List<IFunctionParam>();

                foreach ( var x in pp ) {
                    ppp.Add( FunctionParamFactory.Create( x , ref baseIndex , functionParams ) );
                }


                return new FunctionParamBetween( ppp[0] , ppp[1] , ppp[2] );


            }

            return null;
        }

        public static IFunctionParam Create( DALFunctionParamSubQueryConstraint param , ref int baseIndex , DALFunctionParam[] functionParams , QueryAdapter queryAdapter ) {

            Type subQuery = ( param as DALFunctionParamSubQueryConstraint ).SubQuery;
            var ctor = subQuery.GetConstructor( new Type[] { typeof( QueryAdapter ) } );
            var instanceSubQuery = ctor?.Invoke( new object[] { queryAdapter } );
            return new FunctionParamSubQuery( ( SQLQuery ) instanceSubQuery );

        }
    }

    public abstract class QueryFunction<T> : GenericQueryColumn, ValuableQueryColumn<T> where T : ISQLTypeWrapper {

        public QueryFunction( string aDeclaredVariableName , PropertyInfo aPropertyInfo ) : base( aPropertyInfo ) {
            //protected DALFunctionParam[] mParams;

            int baseIndex = 0;

            mFunctionParams = new List<IFunctionParam>();

            DALProjectionFunction dalProjectionFunction = ( this.mPropertyInfo?.GetCustomAttribute<DALProjectionFunction>() );
            DALFunctionParam[] dalFunctionParam = ( ( DALFunctionParam[] ) ( this.mPropertyInfo?.GetCustomAttributes<DALFunctionParam>() ) );

            var totalParam = dalFunctionParam.Sum( x => x.CountParam() ) + dalProjectionFunction.CountParam();

            if ( totalParam != dalFunctionParam.Length ) throw new ArgumentException( $"Invalid numer of param for function {dalProjectionFunction.Function}" );

            var functionParams = dalFunctionParam.Skip( baseIndex ).Take( dalProjectionFunction.CountParam() ).ToArray();
            //var functionParams = dalFunctionParam.Skip( baseIndex ).Take( totalParam ).ToArray();

            baseIndex = 0;

            var subFunctionParams = dalFunctionParam.Skip( dalProjectionFunction.CountParam() ).ToArray();

            foreach ( var param in functionParams ) {

                IFunctionParam p = FunctionParamFactory.Create( param , ref baseIndex , subFunctionParams );

                if ( p is not null )
                    mFunctionParams.Add( p );

            }

            mFunction = FunctionFactory.Create( dalProjectionFunction , mFunctionParams.ToArray() );

            mSQLDataType = ( MySQLDataType ) ( dalProjectionFunction ).DataType;
            //mFunctionName = dalProjectionFunction.Function;
            mColumnAlias = aDeclaredVariableName;

            if ( string.IsNullOrEmpty( aDeclaredVariableName ) ) {
                throw new ArgumentException( $"'{nameof( aDeclaredVariableName )}' non può essere null o vuoto." , nameof( aDeclaredVariableName ) );
            }

            mDeclaredVariableName = aDeclaredVariableName;

            mGenericType = typeof( T ).GetGenericArguments()[0];

            if ( dalProjectionFunction.GroupBy != -1 ) {
                GroupBy = dalProjectionFunction.GroupBy;
            }

            if ( dalProjectionFunction.OrderBy != -1 ) {
                OrderBy = dalProjectionFunction.OrderBy;
            }


        }

        private readonly string mDeclaredVariableName;

        private readonly Type mGenericType;
        public override Type GenericType => mGenericType;

        private IFunction mFunction;
        public IFunction Function { get => mFunction; }

        //private readonly string mFunctionName;
        //public string FunctionName => mFunctionName;

        public abstract QueryParam[] getParam();

        public override string ColumnAlias => mColumnAlias;
        public override string ActualName => mColumnAlias ?? mFunction.ToString();


        private T mTypeWrapperValue;
        public T TypeWrapperValue { get => mTypeWrapperValue; set => SetValue( value ); }



        private List<IFunctionParam> mFunctionParams;


        private void SetValue( T value ) {
            this.mTypeWrapperValue = value;
        }

        public object Value() {
            if ( mTypeWrapperValue.IsNull )
                return null;
            else
                return ( ( object ) ( ( dynamic ) mTypeWrapperValue ).Value );
        }


        public List<IFunctionParam> FunctionParams {
            set => mFunctionParams = value;
            get => mFunctionParams;
        }

        public override void AssignValue( object x ) {
            var genericType = typeof( T ).GetGenericArguments()[0];
            ConstructorInfo ctor = typeof( T ).GetConstructor( new Type[] { genericType } );
            TypeWrapperValue = ( T ) ctor?.Invoke( new object[] { Convert.ChangeType( x , genericType ) } );
        }

    }




    public class SQLProjectionFunction<T> : QueryFunction<T> where T : ISQLTypeWrapper {
        public SQLProjectionFunction( string aDeclaredVariableName , PropertyInfo aMethodBase ) : base( aDeclaredVariableName , aMethodBase ) {

            mTable = new TableReference( "" );
        }

        public override QueryParam[] getParam() {
            return Function.getParam();
        }


        public override string ToString() {

            return $"{Function.ToString()} AS {this.ActualName}";
            //if ( this.FunctionParams is null ) {
            //    return $"{this.FunctionName}() AS {this.ActualName}";
            //} else {
            //    return $"{this.FunctionName}({string.Join( "," , this.FunctionParams )}) AS {this.ActualName}";
            //}

        }
    }

}
