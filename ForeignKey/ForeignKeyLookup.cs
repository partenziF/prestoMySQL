using prestoMySQL.Column;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Entity.Interface;
using prestoMySQL.ForeignKey.Attributes;
using prestoMySQL.Helper;
using prestoMySQL.PrimaryKey;
using prestoMySQL.PrimaryKey.Attributes;
using prestoMySQL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.ForeignKey {
    public class ForeignKeyLookup<T> : EntityForeignKey where T : AbstractEntity {

        List<T> leftTable = new List<T>();

        public ForeignKeyLookup( AbstractEntity e , string ForeignkeyName ) : base( e , ForeignkeyName ) {
        }

        public override void addEntities( List<AbstractEntity> foreignKeyTables ) {


            foreach ( var info in this.foreignKeyInfo ) {

                foreach ( AbstractEntity entity in foreignKeyTables.Where( x => ( (x is not null) && ( x.GetType() == info.TypeReferenceTable ) && ( info.ReferenceTable != null ) ) ) ) {

                    if ( typeof( T ).IsAssignableFrom( entity.GetType() ) ) {
                        if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {
                            leftTable.Add( ( T ) entity );

                            var piList = this.foreignKeyColumns.Where( x => x.Value.Name == info.ColumnName ).Select( k => k.Value ).ToList();
                            foreach ( var pi in piList )
                                ReflectionTypeHelper.AssignValue<T>( this , pi , ( T ) entity );
                        }
                        //} else if ( typeof( U ).IsAssignableFrom( entity.GetType() ) ) {
                        //    if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {
                        //        rightTable.Add( ( U ) entity );

                        //        var piList = this.foreignKeyColumns.Where( x => x.Value.Name == info.ColumnName ).Select( k => k.Value ).ToList();
                        //        foreach ( var pi in piList )
                        //            ReflectionTypeHelper.AssignValue<U>( this , pi , ( U ) entity );
                        //    }
                        //}

                        //foreach ( AbstractEntity entityLeft in foreignKeyTables ) {

                        //    if ( typeof( T ).IsAssignableFrom( entityLeft.GetType() ) ) {

                        //        //if ( entityLeft.FkNames == this.ForeignkeyName ) {
                        //        if ( entityLeft.FkNames.Contains( this.ForeignkeyName ) ) {

                        //            leftTable.Add( ( T ) entityLeft );

                        //            foreach ( var (name, pi) in this.foreignKeyColumns ) {

                        //                //var x = pi.GetCustomAttribute<DDForeignKey>()?.Name;
                        //                //foreach(var x in pi.get)
                        //                ReflectionTypeHelper.AssignValue<T>( this , pi , ( T ) entityLeft );

                        //            }

                        //        }

                        //    }

                        //}


                    }
                }
            }

            if ( Table.State == EntityState.Changed ) {
                string[] @params = foreignKeyColumns.Keys.ToArray();
                createKey( @params );
            }

        }


        //public override void createKey() {

        //    //List<object> result = new List<object>();

        //    foreach ( T entityLeft in leftTable ) {

        //        foreach ( var (name, pi) in this.foreignKeyColumns ) {
        //            //var col = pi.GetValue( entityLeft );
        //            //( ( IObservableColumn ) col ).Attach( this );
        //            ReflectionTypeHelper.AssignValue<T>( this , pi , entityLeft );
        //        }

        //        //                pi.SetValue( Table , v );         
        //        //EntityPrimaryKey pkLeft = entityLeft.PrimaryKey;
        //        //EntityPrimaryKey[] item = new EntityPrimaryKey[1];
        //        //item[0] = pkLeft;
        //        ////this.stackPrimaryKey.Push( item );
        //        //var v = pkLeft.getKeyValues();
        //        //result.AddRange( v );
        //    }


        //    //object[] result = base.createKey();

        //    //return result;
        //}

        public override object[] getKeyValues() {

            List<object> result = new List<object>();

            foreach ( T entityLeft in leftTable ) {

                var v = entityLeft.PrimaryKey.getKeyValues();
                result.AddRange( v );

            }

            return result.ToArray();
        }


        //    public override void Update( IObservableColumn subjectColumn ) {
        //        //throw new NotImplementedException();

        //        if ( Attribute.IsDefined( this.Field , typeof( DDPrimaryKey ) ) ) {

        //            PropertyInfo pi = this.ReferenceTable.GetType().GetProperty( this.ReferenceColumnName );
        //            if ( pi != null ) {
        //                dynamic c = pi?.GetValue( this.ReferenceTable );
        //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //            } else {
        //                dynamic c = this.Field.GetValue( this.Table );
        //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //            }


        //        } else {

        //            Dictionary<string , ForeignKeyInfo> infos;
        //            if ( this.foreignKeysInfo.TryGetValue( ( subjectColumn as dynamic ).TypeTable , out infos ) ) {

        //                foreach ( var (_, info) in infos ) {

        //                    if ( ( info.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( info.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) ) {

        //                        dynamic c = this.Field.GetValue( this.Table );
        //                        c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        //                    }
        //                }
        //            }
        //        }

        //        createKey();

        //    }

        //}


        public override void Update( IObservableColumn subjectColumn ) {
            //throw new NotImplementedException();

            List<string> ColumnName = new List<string>();

            foreach ( var fk in this.foreignKeyInfo ) {
                if ( Attribute.IsDefined( fk.Field , typeof( DDPrimaryKey ) ) ) {

                    PropertyInfo pi = fk.ReferenceTable.GetType().GetProperty( fk.ReferenceColumnName );
                    if ( pi != null ) {
                        dynamic c = pi?.GetValue( fk.ReferenceTable );
                        c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                        ColumnName.Add( fk.ColumnName );
                    }

                } else {

                    if ( ( fk.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( fk.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) ) {

                        dynamic c = fk.Field.GetValue( fk.Table );
                        c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                        ColumnName.Add( fk.ColumnName );

                    }

                }


            }

            if ( ColumnName.Count > 0 )
                createKey( ColumnName.ToArray() );

            //Dictionary<string , ForeignKeyInfo> infos;
            //if ( this.foreignKeysInfo.TryGetValue( ( subjectColumn as dynamic ).TypeTable , out infos ) ) {
            //    foreach ( var (_, info) in infos ) {

            //        if ( Attribute.IsDefined( info.Field , typeof( DDPrimaryKey ) ) ) {
            //            PropertyInfo pi = info.ReferenceTable.GetType().GetProperty( info.ReferenceColumnName );
            //            if ( pi != null ) {
            //                dynamic c = pi?.GetValue( info.ReferenceTable );
            //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
            //            }

            //        } else {

            //            if ( ( info.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( info.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) ) {

            //                dynamic c = info.Field.GetValue( info.Table );
            //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
            //                ColumnName.Add( info.ColumnName );

            //            }

            //        }

            //    }

            //    createKey( ColumnName.ToArray() );
            //}

            //if ( Attribute.IsDefined( this.Field , typeof( DDPrimaryKey ) ) ) {
            //    PropertyInfo pi = this.ReferenceTable.GetType().GetProperty( this.ReferenceColumnName );
            //    if ( pi != null ) {
            //        dynamic c = pi?.GetValue( this.ReferenceTable );
            //        c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
            //    }
            //} else {

            //    Dictionary<string , ForeignKeyInfo> infos;
            //    if ( this.foreignKeysInfo.TryGetValue( ( subjectColumn as dynamic ).TypeTable , out infos ) ) {
            //        foreach ( var ( _, info) in infos ) {

            //            if ( ( info.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( info.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) ) {

            //                dynamic c = info.Field.GetValue( info.Table );
            //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
            //                ColumnName.Add( info.ColumnName );

            //            }
            //        }
            //    }

            //dynamic c = this.Field.GetValue( this.Table );
            //c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
        }



    }



    public class ForeignKeyManyToMany<T, U> : EntityForeignKey where T : AbstractEntity where U : AbstractEntity {
        List<T> leftTable = new List<T>();
        List<U> rightTable = new List<U>();
        public ForeignKeyManyToMany( AbstractEntity aTableEntity , string ForeignkeyName ) : base( aTableEntity , ForeignkeyName ) {
        }

        public override void addEntities( List<AbstractEntity> foreignKeyTables ) {

            foreach ( var info in this.foreignKeyInfo ) {

                foreach ( AbstractEntity entity in foreignKeyTables.Where( x => ( (x is not null) && ( x.GetType() == info.TypeReferenceTable) ) ) ) {

                    if ( typeof( T ).IsAssignableFrom( entity.GetType() ) ) {
                        if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {
                            leftTable.Add( ( T ) entity );

                            var piList = this.foreignKeyColumns.Where( x => x.Value.Name == info.ColumnName ).Select( k => k.Value ).ToList();
                            foreach ( var pi in piList )
                                ReflectionTypeHelper.AssignValue<T>( this , pi , ( T ) entity );
                        }
                    } else if ( typeof( U ).IsAssignableFrom( entity.GetType() ) ) {
                        if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {
                            rightTable.Add( ( U ) entity );

                            var piList = this.foreignKeyColumns.Where( x => x.Value.Name == info.ColumnName ).Select( k => k.Value ).ToList();
                            foreach ( var pi in piList )
                                ReflectionTypeHelper.AssignValue<U>( this , pi , ( U ) entity );
                        }
                    }



                }

            }

            if ( Table.State == EntityState.Changed ) {
                string[] @params = foreignKeyColumns.Keys.ToArray();
                createKey( @params );
            }

            //foreach ( AbstractEntity entity in foreignKeyTables ) {

            //    if ( typeof( T ).IsAssignableFrom( entity.GetType() ) ) {

            //        //if ( entityLeft.FkNames == this.ForeignkeyName ) {
            //        if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {

            //            leftTable.Add( ( T ) entity );
            //            //Qui togliere il doppio foreach che non serve
            //            foreach ( var (_, pi) in this.foreignKeyColumns ) {

            //                //var x = pi.GetCustomAttribute<DDForeignKey>()?.Name;
            //                //foreach(var x in pi.get)
            //                //foreach ( var (_name, _) in this.foreignKeysInfo[typeof( T )] ) {
            //                ReflectionTypeHelper.AssignValue<T>( this , pi , ( T ) entity );
            //                //}

            //            }

            //        }

            //    } else if ( typeof( U ).IsAssignableFrom( entity.GetType() ) ) {

            //        //if ( entityLeft.FkNames == this.ForeignkeyName ) {
            //        if ( entity.FkNames.Contains( this.ForeignkeyName ) ) {

            //            rightTable.Add( ( U ) entity );

            //            foreach ( var (_, pi) in this.foreignKeyColumns ) {
            //                //foreach ( var (_name, _) in this.foreignKeysInfo[typeof( U )] ) {

            //                //var x = pi.GetCustomAttribute<DDForeignKey>()?.Name;
            //                //foreach(var x in pi.get)
            //                ReflectionTypeHelper.AssignValue<U>( this , pi , ( U ) entity );
            //                //}
            //            }
            //        }
            //    }
            //}

        }

        public override object[] getKeyValues() {
            throw new NotImplementedException();
        }


        public override void Update( IObservableColumn subjectColumn ) {
            //throw new NotImplementedException();

            List<string> ColumnName = new List<string>();

            foreach ( var fk in this.foreignKeyInfo ) {

                if ( Attribute.IsDefined( fk.Field , typeof( DDPrimaryKey ) ) ) {
                    if ( fk.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) {

                        PropertyInfo pi = fk.Table.GetType().GetProperty( fk.ColumnName );
                        if ( pi != null ) {
                            dynamic c = pi?.GetValue( fk.Table );
                            c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                            ColumnName.Add( fk.ColumnName );
                        }

                    }

                    //PropertyInfo pi = fk.ReferenceTable.GetType().GetProperty( fk.ReferenceColumnName );
                    //if ( pi != null ) {
                    //    dynamic c = pi?.GetValue( fk.ReferenceTable );
                    //    c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                    //}

                    //Dictionary<string , ForeignKeyInfo> infos;
                    //if ( this.foreignKeysInfo.TryGetValue( ( subjectColumn as dynamic ).TypeTable , out infos ) ) {

                    //    foreach ( var (_, info) in infos ) {

                    //        if ( ( info.TypeReferenceTable == ( subjectColumn as dynamic ).TypeTable ) && ( info.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) ) {

                    //            PropertyInfo pi = info.ReferenceTable.GetType().GetProperty( info.ReferenceColumnName );
                    //            if ( pi != null ) {
                    //                //dynamic c = pi?.GetValue( info.ReferenceTable );
                    //                dynamic c = info.Field.GetValue( this.Table );
                    //                c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                    //                ColumnName.Add( info.ColumnName );
                    //            }


                    //        }
                    //    }

                    //}
                } else {
                    if ( fk.ReferenceColumnName == ( subjectColumn as dynamic ).ColumnName ) {

                        PropertyInfo pi = fk.Table.GetType().GetProperty( fk.ColumnName );
                        if ( pi != null ) {
                            dynamic c = pi?.GetValue( fk.Table );
                            c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                            ColumnName.Add( fk.ColumnName );
                        }

                    }
                    //dynamic c = this.Field.GetValue( this.Table );
                    //c.AssignValue( ( subjectColumn as ConstructibleColumn ).GetValue() ?? ReflectionTypeHelper.SQLTypeWrapperNULL( c?.GenericType ) );
                }


            }

            if ( ColumnName.Count > 0 )
                createKey( ColumnName.ToArray() );

        }


    }

}
