using Microsoft.Extensions.Logging;
using MySqlConnector;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.Utils;
using PrestoMySQL.Database.Interface;
using PrestoMySQL.Database.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace prestoMySQL.Adapter {



    public class EntitiesAdapter {//: IDictionary<AbstractEntity , List<EntityForeignKey>> 

        public TableGraph _Graph;

        //private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();

        public readonly MySQLDatabase mDatabase;
        private readonly ILogger mLogger;
        private Dictionary<Type , List<TableEntity>> mEntitiesCache;

        //public ICollection<AbstractEntity> Keys => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Keys;
        //public ICollection<List<EntityForeignKey>> Values => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Values;
        //public int Count => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Count;
        //public bool IsReadOnly => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).IsReadOnly;

        public void Create<A1, E1, A2, E2>() where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                             where A2 : EntityAdapter<E2> where E2 : AbstractEntity {

            _Graph.mCache.Clear();

            A1 a1;
            A2 a2;

            a1 = NewInstanceAdapter<A1>();
            a1.Create();

            a2 = NewInstanceAdapter<A2>();
            a2.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );

            _Graph.BuildEntityGraph( a1.Entity , a2.Entity );

        }

        public void Create<A1, E1, A2, E2, A3, E3>() where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity {

            _Graph.mCache.Clear();

            A1 a1;
            A2 a2;
            A3 a3;

            a1 = NewInstanceAdapter<A1>();
            a1.Create();

            a2 = NewInstanceAdapter<A2>();
            a2.Create();

            a3 = NewInstanceAdapter<A3>();
            a3.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );

            _Graph.BuildEntityGraph( a1.Entity , a2.Entity , a3.Entity );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4>() where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity {

            _Graph.mCache.Clear();

            A1 a1;
            A2 a2;
            A3 a3;
            A4 a4;

            a1 = NewInstanceAdapter<A1>();
            a1.Create();

            a2 = NewInstanceAdapter<A2>();
            a2.Create();

            a3 = NewInstanceAdapter<A3>();
            a3.Create();

            a4 = NewInstanceAdapter<A4>();
            a4.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );

            _Graph.BuildEntityGraph( a1.Entity , a2.Entity , a3.Entity , a4.Entity );

        }

        //public List<EntityForeignKey> this[AbstractEntity key] { get => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key]; set => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key] = value; }

        public EntitiesAdapter( MySQLDatabase aMySQLDatabase , ILogger logger = null ) {
            this.mDatabase = aMySQLDatabase;
            this.mLogger = logger;
            _Graph = new TableGraph();
            mEntitiesCache = new Dictionary<Type , List<TableEntity>>();
            //_Graph.mCache = new Dictionary<Type , List<TableEntity>>();
        }


        public List<EntityForeignKey> GetForeignKeys() {

            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
            List<EntityForeignKey> result = new List<EntityForeignKey>();
            List<EntityForeignKey> edge = new List<EntityForeignKey>();

            foreach ( var (e, listFK) in _Graph ) {

                if ( !visited.Contains( e ) ) {

                    visited?.Push( e );
                }

                foreach ( EntityForeignKey fk in listFK.Where( x => x.RefenceTable != null ).ToList() ) {

                    //bool add = true;
                    //if ( edge.Count > 0 ) {
                    //    foreach ( EntityForeignKey f in edge ) {
                    //        if ( ( ( ( f.RefenceTable == fk.RefenceTable ) && ( f.Table == fk.Table ) ) || ( ( f.RefenceTable == fk.Table ) && ( f.Table == fk.RefenceTable ) ) ) ) {
                    //            add = false;
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (add) edge.Add( fk ); 

                    if ( ( fk.RefenceTable != null ) && ( CacheContainsEntityType( fk ) ) ) {

                        if ( visited.Contains( fk.RefenceTable ) ) continue;

                        visited?.Push( fk.RefenceTable );

                        result.Add( fk );

                    }
                }

            }

            return result;


        }

        private bool CacheContainsEntityType( EntityForeignKey fk ) {

            foreach ( var (t, list) in _Graph.mCache ) {
                foreach ( var a in list ) {
                    if ( ( ( dynamic ) a ).GetType() == fk.TypeRefenceTable ) {
                        return true;
                    }
                }
            }

            return false;
        }

        //public List<AbstractEntity> GetTopologicalOrder() {
        //    List<AbstractEntity> result = new List<AbstractEntity>();
        //    _Graph.mCache.Values.ToList().ForEach( l => l.ForEach( a => result.Add( ( ( dynamic ) a ).Entity ) ) );
        //    //var x = mCache.Values.ToList().Select( l => l.Select( a => ( ( dynamic ) a ).Entity ).ToList() ).ToList();            
        //    return result;
        //}

        public T Entity<T>() where T : AbstractEntity {

            return ( T ) _Graph.Keys.FirstOrDefault( x => x.GetType().IsAssignableFrom( typeof( T ) ) );

        }

        public AbstractEntity Entity( Type t ) {

            return _Graph.Keys.FirstOrDefault( x => x.GetType().IsAssignableFrom( t ) );

        }

        public T Adapter<T>() where T : TableEntity {


            if ( _Graph.mCache.ContainsKey( typeof( T ) ) ) {
                return ( T ) mEntitiesCache[typeof( T )].FirstOrDefault();
            } else {
                if ( ( typeof( T ).IsGenericType ) && ( typeof( T ).GetGenericTypeDefinition() == typeof( EntityAdapter<> ) ) ) {

                    Type t = typeof( T ).GetGenericArguments().FirstOrDefault();

                    if ( t != null ) {

                        var e = Entity( t );

                        if ( e != null ) {
                            T Adapter;
                            InstantiateAdapter<T>( out Adapter , e );
                            mEntitiesCache.AddOrCreate( Adapter );
                            return Adapter;
                        } else {
                            new ArgumentException( "Invalid generic argument" );
                        }

                    } else {
                        new ArgumentException( "Invalid generic argument" );
                    }

                    return default;

                } else if ( ( typeof( T ).BaseType.IsGenericType ) && ( typeof( T ).BaseType.GetGenericTypeDefinition() == typeof( EntityAdapter<> ) ) ) {

                    Type t = typeof( T ).BaseType.GetGenericArguments().FirstOrDefault();

                    if ( t != null ) {

                        var e = Entity( t );

                        if ( e != null ) {

                            T Adapter;
                            InstantiateAdapter<T>( out Adapter , e );

                            if ( Adapter != null ) {
                                mEntitiesCache.AddOrCreate( Adapter );
                                return Adapter;
                            }

                        } else {
                            new ArgumentException( "Invalid generic argument" );
                        }

                    } else {
                        new ArgumentException( "Invalid generic argument" );
                    }

                    return default;
                } else {
                    return default;
                }

            }

            //return ( T ) mAdapters.FirstOrDefault( x => x.GetType() == typeof( T ) );
        }

        //private bool InstantiateReferenceTable( EntityForeignKey fkey ) {

        //    //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

        //    //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
        //    // allora se RefenceTable è nullo copia il valore in mGraph
        //    // altrimenti instazia la tabella ed aggiungila ad mGraph
        //    if ( fkey.RefenceTable == null ) {

        //        var x = _Graph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeRefenceTable );

        //        if ( x != null ) {

        //            fkey.RefenceTable = x;
        //            // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //            // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //            return false;
        //        } else {

        //            fkey.InstantiateRefenceTable();
        //            return true;
        //        }

        //    }

        //    return false;

        //}


        //private void BuildEntityGraph( params TableEntity[] tableAdapters ) {

        //    this.mGraph.Clear();
        //    Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();

        //    tableAdapters.ToList().ForEach( a => {

        //        mGraph.Add( ( AbstractEntity ) ( ( dynamic ) a ).Entity );
        //        mCache.AddOrCreate( a );

        //        ( ( AbstractEntity ) ( ( dynamic ) a ).Entity ).GetAllForeignkey().ForEach( x => {
        //            foreignKeys.Push( x );
        //            mGraph.Connect( x.Table , x );
        //        } );

        //    } );

        //    EntityForeignKey fkey = null;

        //    while ( foreignKeys.TryPop( out fkey ) ) {

        //        if ( tableAdapters.ToList().FirstOrDefault( a => ( ( dynamic ) a ).Entity.GetType() == fkey.TypeRefenceTable ) != null ) {

        //            //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

        //            //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
        //            // allora se RefenceTable è nullo copia il valore in mGraph
        //            // altrimenti instazia la tabella ed aggiungila ad mGraph

        //            //if ( fkey.RefenceTable == null ) {

        //            if ( InstantiateReferenceTable( fkey ) ) {

        //                mGraph.Add( fkey.RefenceTable );

        //                var allfk = fkey.RefenceTable.GetAllForeignkey();

        //                foreach ( var ffk in allfk ) {

        //                    if ( !mGraph.IsConnected( ffk ) ) {

        //                        foreignKeys.Push( ffk );
        //                        InstantiateReferenceTable( ffk );
        //                        mGraph.Connect( ffk.RefenceTable , ffk );

        //                    }

        //                }

        //                //}

        //                //var x = mGraph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeRefenceTable );
        //                //if ( x != null ) {
        //                //    fkey.RefenceTable = x;
        //                //    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //                //    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //                //} else {
        //                //    fkey.InstantiateRefenceTable();
        //                //    mGraph.Add( fkey.RefenceTable );
        //                //    var allfk = fkey.RefenceTable.GetAllForeignkey();
        //                //    foreach ( var ffk in allfk ) {
        //                //        if ( !mGraph.IsConnected( ffk ) ) {
        //                //            foreignKeys.Push( ffk );
        //                //            InstantiateReferenceTable( ffk );
        //                //            mGraph.Connect( ffk.RefenceTable , ffk );
        //                //        }
        //                //    }
        //                //}


        //            }
        //        }
        //    }


        //    var _Entities = mGraph.Keys.ToList();
        //    foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
        //        foreach ( var _fk in mGraph[e] ) {

        //            if ( _fk.RefenceTable != null ) {
        //                _fk.addEntities( _Entities );
        //                //_fk.createKey(); inutile?

        //            }
        //        }
        //    }

        //    //foreach ( dynamic a in tableAdapters ) {
        //    //    if ( a.Entity != null ) {
        //    //        mGraph.Add( (AbstractEntity) a.Entity );
        //    //        foreach ( EntityForeignKey fk in a.Entity.GetAllForeignkey() ) {
        //    //            Console.WriteLine( fk );
        //    //            mGraph.Connect( ( AbstractEntity ) a.Entity , fk );
        //    //            fk.InstantiateRefenceTable();
        //    //            mGraph.Add( fk.RefenceTable );
        //    //            fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );
        //    //        }
        //    //    }
        //    //    EntityForeignKey fkey = null;
        //    //    while ( foreignKeys.TryPop( out fkey ) ) {
        //    //        //fkey.InstantiateRefenceTable();
        //    //        //mGraph.Add( fk.RefenceTable );
        //    //        //fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );
        //    //        //foreignKeyTables.Add( fk.RefenceTable );
        //    //    }
        //    //}

        //}


        private A1 NewInstanceAdapter<A1>() {

            if ( mLogger != null ) {
                var ctor = typeof( A1 ).GetConstructor( new Type[] { this.mDatabase.GetType() , this.mLogger.GetType() } );
                return ( A1 ) ctor.Invoke( new object[] { this.mDatabase , this.mLogger } );

            } else {
                var ctor = typeof( A1 ).GetConstructor( new Type[] { this.mDatabase.GetType() } );
                return ( A1 ) ctor.Invoke( new object[] { this.mDatabase } );

            }


        }

        //void InstantiateAdapter<U1, U2>( out U1 adapter , out U2 entity ) where U1 : EntityAdapter<U2>
        //    where U2 : AbstractEntity {

        //    entity = Entity<U2>();

        //    if ( mLogger != null ) {
        //        var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() , this.mLogger.GetType() } );
        //        adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase , this.mLogger } );

        //    } else {
        //        var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() } );
        //        adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase } );

        //    }


        //}

        public void BindData( IReadableResultSet rs ) {

            List<dynamic> definitionColumns = new List<dynamic>();
            var tables = _Graph.GetTopologicalOrder();
            foreach ( var e in tables ) {
                definitionColumns.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() );
            }
            var s = rs.ResultSetSchemaTable();
            int? index = null;
            foreach ( ConstructibleColumn column in definitionColumns ) {

                index = null;

                if ( s.ContainsKey( column.Table.ActualName ) ) {
                    if ( s[column.Table.ActualName].ContainsKey( column.ActualName ) ) {
                        index = s[column.Table.ActualName][column.ActualName];
                    }
                }

                if ( index == null ) throw new System.Exception( "Invalid index column." );


                var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                                                           typeof( MySQResultSet ) ,
                                                           rs ,
                                                           nameof( MySQResultSet.getValueAs ) ,
                                                           new Type[] { typeof( int ) } ,
                                                           new object[] { ( int ) index } );

                if ( v.IsDBNull() ) {
                    ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                } else {
                    column.AssignValue( v );
                }

            }

        }

        private OperationResult FetchResultSet( MySQResultSet rs ) {

            if ( rs != null ) {

                if ( rs.fetch() ) {

                    BindData( rs );

                    foreach ( var (t, list) in  mEntitiesCache ) {

                        foreach ( var a in list ) {

                            var generic = ReflectionTypeHelper.GetClassGenericType( a );
                            var type = typeof( BindDataFromEventArgs<> ).MakeGenericType( generic );
                            dynamic o = Activator.CreateInstance( type );
                            o.Entity = ( a as dynamic ).Entity;
                            var m = ReflectionTypeHelper.InvokeMethod( a , "OnBindDataFrom" , new object[] { o } );

                            //( a as dynamic ).OnBindDataFrom( o );

                        }
                        //    a.OnBi
                        //    OnBindDataFrom( new BindDataFromEventArgs<T>() { Entity = this.Entity } );
                    }

                    if ( rs.fetch() ) {

                        //Entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        rs?.close();
                        //this.mEntities.Clear();
                        throw new System.Exception( "Resultset return more than one row." );

                    } else {

                        //Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        rs?.close();
                        return OperationResult.OK;

                    }

                } else {
                    rs?.close();
                    //this.mEntities.Clear();
                    return OperationResult.Empty;
                }

            } else {
                rs?.close();
                //this.mEntities.Clear();
                return OperationResult.Error;
            }
        }

        public OperationResult Read( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            if ( KeyValues.Count() == 0 ) throw new ArgumentException( "Invalid key value argument" );

            try {

                var tables = _Graph.GetTopologicalOrder();

                tables.First().PrimaryKey.setKeyValues( KeyValues );
                //var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                SQLQueryParams outparam = null;
                var s = SQLBuilder.sqlSelect( this , ref outparam , ParamPlaceholder: "@" , Constraint );
                var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.Set;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.Unset;
                    }
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }


        public OperationResult Read<T, X>( Func<T , X> delegateMethod ) where X : EntityUniqueIndex where T : AbstractEntity {

            AbstractEntity entity = _Graph.FirstOrDefault( kvp => kvp.Key.GetType() == typeof( T ) ).Key;

            X x = delegateMethod( ( T ) entity );

            DefinableConstraint[] constraints = new DefinableConstraint[x.ColumnsName.Length];

            int i = 0;
            foreach ( string c in x.ColumnsName ) {
                PropertyInfo p = x[c];
                var col = p.GetValue( entity );
                DefinableConstraint o = FactoryEntityConstraint.MakeConstraintEqual( col , "@" );
                constraints[i++] = o;
            }

            SQLQueryParams outparam = null;
            var s = SQLBuilder.sqlSelect<EntitiesAdapter>( this , ref outparam , new EntityConstraintExpression( constraints ) );
            var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

            var r = FetchResultSet( rs );

            if ( r == OperationResult.OK ) {
                entity.PrimaryKey.KeyState = KeyState.Set;
            } else {
                entity.PrimaryKey.KeyState = KeyState.Unset;
            }

            return r;

        }

        void InstantiateAdapter<U1, U2>( out U1 adapter , out U2 entity ) where U1 : EntityAdapter<U2>
            where U2 : AbstractEntity {

            entity = Entity<U2>();

            if ( mLogger != null ) {
                var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() , this.mLogger.GetType() } );
                adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase , this.mLogger } );

            } else {
                var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() } );
                adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase } );

            }


        }

        void InstantiateAdapter<U1>( out U1 adapter , AbstractEntity entity ) where U1 : TableEntity {

            if ( mLogger != null ) {
                var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() , this.mLogger.GetType() } );
                adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase , this.mLogger } );

            } else {
                var ctor = typeof( U1 ).GetConstructor( new Type[] { this.mDatabase.GetType() } );
                adapter = ( U1 ) ctor.Invoke( new object[] { this.mDatabase } );

            }

            ( adapter as dynamic ).AssignInstance( entity );

        }

        //public void Add( AbstractEntity key , List<EntityForeignKey> value ) {
        //    ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Add( key , value );
        //}

        //public bool ContainsKey( AbstractEntity key ) {
        //    return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).ContainsKey( key );
        //}

        //public bool Remove( AbstractEntity key ) {
        //    return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Remove( key );
        //}

        //public bool TryGetValue( AbstractEntity key , [MaybeNullWhen( false )] out List<EntityForeignKey> value ) {
        //    return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).TryGetValue( key , out value );
        //}

        //public void Add( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
        //    ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Add( item );
        //}

        //public void Clear() {
        //    ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Clear();
        //}

        //public bool Contains( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
        //    return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Contains( item );
        //}

        //public void CopyTo( KeyValuePair<AbstractEntity , List<EntityForeignKey>>[] array , int arrayIndex ) {
        //    ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).CopyTo( array , arrayIndex );
        //}

        //public bool Remove( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
        //    return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Remove( item );
        //}

        //public IEnumerator<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> GetEnumerator() {
        //    return ( ( IEnumerable<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator() {
        //    return ( ( IEnumerable ) this.mGraph ).GetEnumerator();
        //}

    }

}
