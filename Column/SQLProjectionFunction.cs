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
using System.Linq;
using System.Reflection;


namespace prestoMySQL.Column {

    public interface IFunction {

    }
    public interface IFunctionParam {

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
                return $"{FunctionName}";
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

        public override string ToString() {
            return $"( {Left.ToString()} {Operator} {Right.ToString()} )";
        }

    }



    public class FunctionParamConstraint : IFunctionParam {
        private Type table;
        private string property;
        private object value;

        public FunctionParamConstraint( Type table , string property , object value ) {
            this.table = table;
            this.property = property;
            this.value = value;
        }

        public override string ToString() {
            if ( this.value is null ) {
                return $"{Helper.SQLTableEntityHelper.getColumnName( table , property , true )} IS NULL";
            } else {
                return $"{Helper.SQLTableEntityHelper.getColumnName( table , property , true )} = {value.ToString()}";
            }

            //return $"{mColumn.Table.ActualName.QuoteTableName()}.{mColumn.ColumnName.QuoteColumnName()} ";
        }

    }

    public class FunctionParamProperty : IFunctionParam {
        private Type table;
        private string property;

        public FunctionParamProperty( Type table , string property ) {
            this.table = table;
            this.property = property;
        }

        public override string ToString() {

            return Helper.SQLTableEntityHelper.getColumnName( table , property , true );
            //return $"{mColumn.Table.ActualName.QuoteTableName()}.{mColumn.ColumnName.QuoteColumnName()} ";
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

    }

    public class FunctionParamFunction : IFunctionParam {

        private IFunctionParam[] Expression;
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

    }


    public static class FunctionFactory {

        public static IFunction Create( DALProjectionFunction param , params IFunctionParam[] functionParams ) {
            if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_IF ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_CURDATE ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_MAX ) ) {
                return new GenericFunction( ( param as DALProjectionFunction ).Function , functionParams );
            } else if ( ( Type ) param.TypeId == typeof( DALProjectionFunction_SUM ) ) {
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

            } else if ( ( Type ) param.TypeId == typeof( DALFunctionParamConstant ) ) {

                return new FunctionParamConstant( ( param as DALFunctionParamConstant ).Value );

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
    }

    public class QueryFunction<T> : GenericQueryColumn, ValuableQueryColumn<T> where T : ISQLTypeWrapper {

        public QueryFunction( string aDeclaredVariableName , PropertyInfo aPropertyInfo ) : base( aPropertyInfo ) {
            //protected DALFunctionParam[] mParams;

            int baseIndex = 0;

            mFunctionParams = new List<IFunctionParam>();

            DALProjectionFunction dalProjectionFunction = ( this.mPropertyInfo?.GetCustomAttribute<DALProjectionFunction>() );
            DALFunctionParam[] dalFunctionParam = ( ( DALFunctionParam[] ) ( this.mPropertyInfo?.GetCustomAttributes<DALFunctionParam>() ) );

            var totalParam = dalFunctionParam.Sum( x => x.CountParam() ) + dalProjectionFunction.CountParam();

            if ( totalParam != dalFunctionParam.Length ) throw new ArgumentException( $"Invalid numer of param for function {dalProjectionFunction.Function}" );

            var functionParams = dalFunctionParam.Skip( baseIndex ).Take( dalProjectionFunction.CountParam() ).ToArray();

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
        }

        private readonly string mDeclaredVariableName;

        private readonly Type mGenericType;
        public override Type GenericType => mGenericType;

        private IFunction mFunction;
        public IFunction Function { get => mFunction; }

        //private readonly string mFunctionName;
        //public string FunctionName => mFunctionName;


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
