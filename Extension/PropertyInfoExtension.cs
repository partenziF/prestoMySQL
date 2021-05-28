using prestoMySQL.Column;
using prestoMySQL.Column.Attribute;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Extension {
    public static class PropertyInfoExtension {

        public static String ColumnName( this PropertyInfo f , GenericSQLColumn<SQLTypeWrapper<object>> o ) {

            String ColumnName = "";

            //DDColumn a = f.getAnnotation( DDColumn.class);
            DDColumnAttribute a = f.GetCustomAttribute<DDColumnAttribute>();
            if ( a != null ) {
                if ( String.IsNullOrWhiteSpace( a.Name ) ) {

                    if ( o != null ) {
                        ColumnName = o.ColumnName;
                    } else {
                        ColumnName = f.Name;
                    }

                } else {
                    ColumnName = a.Name;
                }

            } else {

                DALProjectionColumn apc = f.GetCustomAttribute<DALProjectionColumn>();
                if ( apc != null ) {

                    if ( string.IsNullOrWhiteSpace( apc.Name ) ) {

                        if ( o != null ) {
                            throw new NotImplementedException();
                            //ColumnName = ( (SQLProjectionColumn<SQLTypeWrapper<object>>) o ).ColumnName;
                        } else {

                        }

                    } else {
                        ColumnName = a.Name;
                    }

                } else {
                    if ( o != null ) {
                        ColumnName = o.ColumnName;
                    } else {
                        ColumnName = f.Name;
                    }

                }

            }

            return ColumnName;

            //   if ( f.isAnnotationPresent( DALProjectionColumn.class)) {

            //        DALProjectionColumn a = f.getAnnotation( DALProjectionColumn.class);

            //           if (a.Name().isEmpty() ) {

            //            if (o != null ) {
            //             ColumName = ((SQLProjectionColumn<?>) o).getColumnName();
            //               } else {
            //             if (a.Table() != "" ) {

            //              ColumName = a.Table() + "." + f.getName();
            //             } else {

            //               ColumName = f.getName();
            //           }
            //            }

            //           } else {
            //               ColumName = a.Name();
            //           }

            //             } else {

            //               if ( o != null ) {
            //                   ColumName = ( ( DefinitionColumn <?>) o).getColumnName();
            //               } else {
            //                   ColumName = f.getName();
            //               }

            //      }

            //   return ColumName;
            //}

            //  }
        }
    }
}
