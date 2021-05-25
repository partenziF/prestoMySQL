using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity.Attributes;
using prestoMySQL.Extension;
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


    public abstract class GenericSQLColumn<T> : ConstructibleColumn, INotifyPropertyChanged, DefinableColumn<T> where T : ISQLTypeWrapper {

        // INotifyPropertyChanged https://stackoverflow.com/questions/32302733/can-c-sharp-implicit-operators-be-used-to-set-a-property where T:struct

        public GenericSQLColumn( PropertyInfo aPropertyInfo = null ) {

            mGenericType = typeof( T ).GetGenericArguments()[0];

            this.mPropertyInfo = aPropertyInfo;

            DALTable dalTable = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALTable>();
            if ( dalTable == null ) throw new ArgumentNullException();
            mTable = new TableReference( dalTable.TableName );

            DDColumnAttribute ddColumnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnAttribute>();
            if ( ddColumnAttribute == null ) throw new ArgumentNullException();

            mColumnName = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().Name;
            mNotNull = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().NullValue == NullValue.NotNull;
            mDefaultValue = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().DefaultValue;

            DDIndexAttribute ddIndex = this.mPropertyInfo?.GetCustomAttribute<DDIndexAttribute>();
            mUnique = ( ddIndex == null ) ? false : true;

            DDPrimaryKey ddPrimaryKey = this.mPropertyInfo?.GetCustomAttribute<DDPrimaryKey>();

            mIsPrimaryKey = ( ddPrimaryKey == null ) ? false : true;
            mIsAutoincrement = ( ddPrimaryKey == null ) ? false : true;

        }

        private TableReference mTable;
        public TableReference Table { get => this.mTable; }


        private Type mGenericType;
        public Type GenericType { get => mGenericType; }


        protected PropertyInfo mPropertyInfo;


        private string mColumnName;
        public string ColumnName { get => this.mColumnName; }


        private T mTypeWrapperValue;
        public T TypeWrapperValue { get => mTypeWrapperValue; set => SetValue( value ); }


        private void SetValue( T value ) {

            if ( mTypeWrapperValue != null ) {

                if ( !mTypeWrapperValue.Equals( value ) ) {
                    OnPropertyChanged( mColumnName);
                }

            } else {
                OnPropertyChanged( mColumnName );
            }

            this.mTypeWrapperValue = value;

        }

        public object Value() {
            if ( mTypeWrapperValue.IsNull )
                return null;
            else
                return ( ( object ) ( ( dynamic ) mTypeWrapperValue ).Value );
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


        public abstract object ValueAsParamType();
        public abstract void AssignValue( object x );

        
        
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged( this , new PropertyChangedEventArgs( propertyName ) );
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged( [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "" ) {

        //    PropertyChanged?.Invoke( this , new PropertyChangedEventArgs( propertyName ) );

        //    //if ( PropertyChanged != null ) {
        //    //    PropertyChanged( this , new PropertyChangedEventArgs( propertyName ) );
        //    //}
        //}


    }

}