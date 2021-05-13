using prestoMySQL.Column.Attribute;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity.Attributes;
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


    public abstract class GenericColumn<T> :  DefinableColumn<T> where T : ISQLTypeWrapper { 
        
        // INotifyPropertyChanged https://stackoverflow.com/questions/32302733/can-c-sharp-implicit-operators-be-used-to-set-a-property where T:struct

        private Type mType;
        protected PropertyInfo mPropertyInfo;
        public GenericColumn( PropertyInfo aPropertyInfo = null ) {

            mType = typeof( T );
            this.mPropertyInfo = aPropertyInfo;

            DALTable dalTable = this.mPropertyInfo?.DeclaringType?.GetCustomAttribute<DALTable>();
            if ( dalTable == null ) throw new ArgumentNullException();

            mTable = new TableReference( dalTable.TableName );

            DDColumnAttribute ddColumnAttribute = this.mPropertyInfo?.GetCustomAttribute<DDColumnAttribute>();
            if ( ddColumnAttribute == null ) throw new ArgumentNullException();

            mColumnName = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().Name;
            mNotNull = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().NotNull;
            mUnique = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().Unique;
            mDefaultValue = this.mPropertyInfo.GetCustomAttribute<DDColumnAttribute>().DefaultValue;

            DDPrimaryKey ddPrimaryKey = this.mPropertyInfo?.GetCustomAttribute<DDPrimaryKey>();

            mIsPrimaryKey = ( ddPrimaryKey == null ) ? false : true;
            mIsAutoincrement = ( ddPrimaryKey == null ) ? false : true;


        }

        private TableReference mTable;
        public TableReference Table { get => this.mTable; }


        private string mColumnName;
        public string ColumnName { get => this.mColumnName;  }


        private T mValue;
        public T Value { get => this.mValue; set => this.mValue = value; }


        bool mNotNull;
        public bool isNotNull { get => mNotNull; }
        //////////////////
        bool mUnique;
        public bool isUnique { get => mUnique;  }
        //////////////////
        object mDefaultValue;
        public object DefaultValue { get => mDefaultValue;  }
        //////////////////
        ///Todo verificare la struttura delle classi e delle interfacce

        private bool mIsPrimaryKey;
        public bool isPrimaryKey { get => mIsPrimaryKey; }
        //////////////////
        bool mIsAutoincrement;

        //public event PropertyChangedEventHandler PropertyChanged;

        public bool isAutoIncrement { get => mIsAutoincrement; }

        public abstract object ValueAsParamType();


        //Convert column's value into param value, used in query
        //object mParam;
        //protected abstract object ConvertValueToParam();
        //public object ValueToParam { get => ConvertValueToParam(); }

        //protected abstract string ConvertValueToParamAsString();

        /////////////////
    }

}