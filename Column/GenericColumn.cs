using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.SQL.Interface;
using prestoMySQL.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace prestoMySQL.Column {

    //public class Colonna<T> where T : struct {
    //    T? Value;

    //    public Colonna( T? value ) {
    //        this.Value = value.HasValue ? value.Value : null;
    //    }
    //}

    public interface IObservableColumn {
        void Attach( IObserverColumn observer );

        // Detach an observer from the subject.
        void Detach( IObserverColumn observer );

        // Notify all observers about an event.
        void NotifyObserver();


    }



    public interface IObserverColumn {

        void Update( IObservableColumn subjectColumn );
    }


    public abstract class GenericSQLColumn<T> : ConstructibleColumn, INotifyPropertyChanged, IObservableColumn, DefinableColumn<T> where T : ISQLTypeWrapper {

        // INotifyPropertyChanged https://stackoverflow.com/questions/32302733/can-c-sharp-implicit-operators-be-used-to-set-a-property where T:struct

        private List<IObserverColumn> mObserverColumns = new List<IObserverColumn>();

        public GenericSQLColumn( PropertyInfo aPropertyInfo = null ) {
            //if ( typeof( T ).GetGenericArguments()[0].GetGenericTypeDefinition() == typeof( Nullable<> ) ) {
            //    mGenericType = typeof( T ).GetGenericArguments()[0].GetGenericArguments()[0];
            //} else {

            mGenericType = typeof( T ).GetGenericArguments()[0];
            //}

            this.mPropertyInfo = aPropertyInfo;

            this.mTypeTable = this.mPropertyInfo.DeclaringType;

            DALTable dalTable = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALTable>();
            if ( dalTable == null ) throw new ArgumentNullException();
            mTable = new TableReference( dalTable.TableName );

            DDColumnAttribute ddColumnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnAttribute>();
            if ( ddColumnAttribute == null ) throw new ArgumentNullException();

            mColumnName = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().Name;
            mAlias = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().Alias;


            mNotNull = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().NullValue == NullValue.NotNull;
            mDefaultValue = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().DefaultValue;

            DDIndexAttribute ddIndex = this.mPropertyInfo?.GetCustomAttribute<DDIndexAttribute>();
            mUnique = ( ddIndex == null ) ? false : true;

            DDPrimaryKey ddPrimaryKey = this.mPropertyInfo?.GetCustomAttribute<DDPrimaryKey>();

            mIsPrimaryKey = ( ddPrimaryKey == null ) ? false : true;
            mIsAutoincrement = ( ddPrimaryKey == null ) ? false : true;

        }

        internal TableReference mTable;
        public TableReference Table { get => this.mTable; }


        private Type mGenericType;
        public Type GenericType { get => mGenericType; }


        private Type mTypeTable;
        public Type TypeTable { get => mTypeTable; }


        protected PropertyInfo mPropertyInfo;


        private string mColumnName;
        public string ColumnName { get => this.mColumnName; }



        private T mTypeWrapperValue;
        public T TypeWrapperValue { get => mTypeWrapperValue; set => SetValue( value ); }

        private void SetValue( T value ) {

            var isChanged = false;

            if ( mTypeWrapperValue != null ) {

                if ( !mTypeWrapperValue.Equals( value ) ) {
                    OnPropertyChanged( mColumnName );
                    isChanged = true;
                }

            } else {
                OnPropertyChanged( mColumnName );
                isChanged = true;
            }

            this.mTypeWrapperValue = value;

            if ( isChanged ) NotifyObserver();
        }

        public object GetValue() {
            if ( mTypeWrapperValue is null )
                throw new NullReferenceException( nameof( mTypeWrapperValue ) + " is null" );
            if ( mTypeWrapperValue.IsNull )
                return null;
            else
                return ( ( object ) ( ( dynamic ) mTypeWrapperValue ).Value );
        }

        public T GetValue<T>() {
            if ( mTypeWrapperValue is null )
                throw new NullReferenceException( nameof( mTypeWrapperValue ) + " is null" );
            if ( mTypeWrapperValue.IsNull ) {
                return default( T );
            } else
                return ( ( T ) ( ( dynamic ) mTypeWrapperValue ).Value );
        }


        bool mNotNull;
        public bool isNotNull { get => mNotNull; }


        bool mUnique;
        public bool isUnique { get => mUnique; }


        object mDefaultValue;
        public object DefaultValue { get => mDefaultValue; }


        private bool mIsPrimaryKey;
        public bool isPrimaryKey { get => mIsPrimaryKey; }



        bool mIsAutoincrement;
        public bool isAutoIncrement { get => mIsAutoincrement; }

        private string mAlias;
        public string Alias => mAlias;

        public string ActualName => ( !String.IsNullOrWhiteSpace( mAlias ) ) ? mAlias : mColumnName;


        public abstract object ValueAsParamType();
        public abstract void AssignValue( object x );



        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged( this , new PropertyChangedEventArgs( propertyName ) );
            }
        }

        public void Attach( IObserverColumn observer ) {

            //throw new NotImplementedException();

            if ( !( mObserverColumns.Contains( observer ) ) ) {
                //if ( !IsCircularReference( ( EntityForeignKey ) observer ) ) {
                mObserverColumns.Add( observer );
                //}
            }
        }

        public void Detach( IObserverColumn observer ) {
            if ( ( mObserverColumns.Contains( observer ) ) ) {
                mObserverColumns.Remove( observer );
            }
        }

        public void NotifyObserver() {
            mObserverColumns.ForEach( o => o.Update( this ) );
        }

        public GenericSQLColumn<T> Copy() {

            return ( GenericSQLColumn<T> ) this.MemberwiseClone();
        }


    }

}