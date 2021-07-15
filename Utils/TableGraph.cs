using prestoMySQL.Entity;
using prestoMySQL.ForeignKey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using prestoMySQL.Helper;

namespace prestoMySQL.Utils {
    public class TableGraph : IDictionary<AbstractEntity , List<EntityForeignKey>> {

        private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph;
        public Dictionary<Type , List<AbstractEntity>> mCache;

        public TableGraph() {
            mGraph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();
            mCache = new Dictionary<Type , List<AbstractEntity>>();
        }

        public List<EntityForeignKey> this[AbstractEntity key] { get => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key]; set => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key] = value; }

        public ICollection<AbstractEntity> Keys => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Keys;

        public ICollection<List<EntityForeignKey>> Values => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Values;

        public int Count => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Count;

        public bool IsReadOnly => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).IsReadOnly;

        public void Add( AbstractEntity key , List<EntityForeignKey> value ) {
            ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Add( key , value );
        }

        public void Add( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Add( item );
        }

        public void Clear() {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Clear();
        }

        public bool Contains( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Contains( item );
        }

        public bool ContainsKey( AbstractEntity key ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).ContainsKey( key );
        }

        public void CopyTo( KeyValuePair<AbstractEntity , List<EntityForeignKey>>[] array , int arrayIndex ) {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).CopyTo( array , arrayIndex );
        }

        public IEnumerator<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> GetEnumerator() {
            return ( ( IEnumerable<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).GetEnumerator();
        }

        public bool Remove( AbstractEntity key ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Remove( key );
        }

        public bool Remove( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Remove( item );
        }

        public bool TryGetValue( AbstractEntity key , [MaybeNullWhen( false )] out List<EntityForeignKey> value ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).TryGetValue( key , out value );
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ( ( IEnumerable ) this.mGraph ).GetEnumerator();
        }


        public void Add( AbstractEntity Head ) {
            if ( !mGraph.ContainsKey( Head ) )
                mGraph.Add( Head , new List<EntityForeignKey>() );
        }

        public void Connect( AbstractEntity Head , EntityForeignKey Node ) {
            if ( mGraph.ContainsKey( Head ) ) {
                if ( !mGraph[Head].Contains( Node ) ) {
                    mGraph[Head].Add( Node );
                }
            } else {
                mGraph.Add( Head , new List<EntityForeignKey>() { Node } );
            }
        }

        public bool IsConnected( EntityForeignKey Node ) {

            foreach ( var entity in mGraph.Keys.ToList() ) {

                if ( entity.GetType() == Node.Table.GetType() ) {
                    //var xxxxx = Graph[entity].FirstOrDefault( x => x.TypeRefenceTable == Node.TypeRefenceTable );
                    foreach ( var x in mGraph[entity] ) {
                        if ( Node.TypeRefenceTable == x.TypeRefenceTable ) {
                            return true;
                        }
                    }

                }

            }
            return false;

        }


        public bool InstantiateReferenceTable( EntityForeignKey fkey ) {

            //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

            //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
            // allora se RefenceTable è nullo copia il valore in mGraph
            // altrimenti instazia la tabella ed aggiungila ad mGraph
            if ( fkey.RefenceTable == null ) {

                var x = mGraph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeRefenceTable );

                if ( x != null ) {

                    fkey.RefenceTable = x;
                    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
                    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
                    return false;
                } else {

                    fkey.InstantiateRefenceTable();
                    return true;
                }

            }

            return false;

        }

        public void ReplaceEntityGraph( AbstractEntity old , AbstractEntity @new ) {

            Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();


            mGraph.Remove( old );

            if ( mCache.ContainsKey( old.GetType() ) )
                mCache[old.GetType()].Remove( old );

            mGraph.Add( @new );
            mCache.AddOrCreate( @new );

            //            AbstractEntity[] e =



            //            BuildEntityGraph( e)

            ( ( AbstractEntity ) @new ).GetAllForeignkey().ForEach( x => {
                foreignKeys.Push( x );
                mGraph.Connect( x.Table , x );
            } );


            EntityForeignKey fkey = null;
            while ( foreignKeys.TryPop( out fkey ) ) {

                if ( InstantiateReferenceTable( fkey ) ) {

                    mGraph.Add( fkey.RefenceTable );

                    var allfk = fkey.RefenceTable.GetAllForeignkey();

                    foreach ( var ffk in allfk ) {

                        if ( !mGraph.IsConnected( ffk ) ) {

                            foreignKeys.Push( ffk );
                            InstantiateReferenceTable( ffk );
                            mGraph.Connect( ffk.RefenceTable , ffk );

                        }

                    }

                }

            }

            var _Entities = mGraph.Keys.ToList();            
            foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
                foreach ( var _fk in mGraph[e] ) {

                    if ( _fk.RefenceTable != null ) {
                        _fk.addEntities( _Entities );
                    }
                }
            }

            //    //foreach ( var (e, l) in mGraph ) {
            //    //    if ( ( fkey.RefenceTable == null ) && ( fkey.TypeRefenceTable == e.GetType() ) ) {

            //    //        if ( InstantiateReferenceTable( fkey ) ) {
            //    //            Console.WriteLine( "" );
            //    //        }
            //    //    }
            //    //}

            //    //if ( mCache.Values.ToList().FirstOrDefault( a => a.GetType() == fkey.TypeRefenceTable ) != null ) {

            //    //    //if ( @new.GetType() == fkey.TypeRefenceTable ) {

            //    //    if ( InstantiateReferenceTable( fkey ) ) {

            //    //        mGraph.Add( fkey.RefenceTable );

            //    //        var allfk = fkey.RefenceTable.GetAllForeignkey();

            //    //        foreach ( var ffk in allfk ) {

            //    //            if ( !mGraph.IsConnected( ffk ) ) {

            //    //                foreignKeys.Push( ffk );
            //    //                InstantiateReferenceTable( ffk );
            //    //                mGraph.Connect( ffk.RefenceTable , ffk );

            //    //            }

            //    //        }

            //    //    }

            //    //}

            //    //if ( tableEntity.ToList().FirstOrDefault( a => a.GetType() == fkey.TypeRefenceTable ) != null ) {

            //    //    //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

            //    //    //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
            //    //    // allora se RefenceTable è nullo copia il valore in mGraph
            //    //    // altrimenti instazia la tabella ed aggiungila ad mGraph

            //    //    //if ( fkey.RefenceTable == null ) {

            //    //    if ( InstantiateReferenceTable( fkey ) ) {

            //    //        mGraph.Add( fkey.RefenceTable );

            //    //        var allfk = fkey.RefenceTable.GetAllForeignkey();

            //    //        foreach ( var ffk in allfk ) {

            //    //            if ( !mGraph.IsConnected( ffk ) ) {

            //    //                foreignKeys.Push( ffk );
            //    //                InstantiateReferenceTable( ffk );
            //    //                mGraph.Connect( ffk.RefenceTable , ffk );

            //    //            }

            //    //        }

            //    //    }
            //    //}
            //}


            //var _Entities = mGraph.Keys.ToList();
            //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
            //    foreach ( var _fk in mGraph[e] ) {

            //        if ( _fk.RefenceTable != null ) {
            //            _fk.addEntities( _Entities );
            //        }
            //    }
            //}



        }

        public void BuildEntityGraph( params AbstractEntity[] tableEntity ) {

            this.mGraph.Clear();
            Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();

            tableEntity.ToList().ForEach( a => {

                mGraph.Add( ( AbstractEntity ) a );
                mCache.AddOrCreate( ( AbstractEntity ) a );

                ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
                    foreignKeys.Push( x );
                    mGraph.Connect( x.Table , x );
                } );

            } );

            EntityForeignKey fkey = null;

            while ( foreignKeys.TryPop( out fkey ) ) {

                if ( tableEntity.ToList().FirstOrDefault( a => a.GetType() == fkey.TypeRefenceTable ) != null ) {

                    //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

                    //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
                    // allora se RefenceTable è nullo copia il valore in mGraph
                    // altrimenti instazia la tabella ed aggiungila ad mGraph

                    //if ( fkey.RefenceTable == null ) {

                    if ( InstantiateReferenceTable( fkey ) ) {

                        mGraph.Add( fkey.RefenceTable );

                        var allfk = fkey.RefenceTable.GetAllForeignkey();

                        foreach ( var ffk in allfk ) {

                            if ( !mGraph.IsConnected( ffk ) ) {

                                foreignKeys.Push( ffk );
                                InstantiateReferenceTable( ffk );
                                mGraph.Connect( ffk.RefenceTable , ffk );

                            }

                        }

                    }
                }
            }


            var _Entities = mGraph.Keys.ToList();
            foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
                foreach ( var _fk in mGraph[e] ) {

                    if ( _fk.RefenceTable != null ) {
                        _fk.addEntities( _Entities );
                        //_fk.createKey(); inutile?

                    }
                }
            }


        }

        //public void BuildEntityGraph( params TableEntity[] tableAdapters ) {

        //    AbstractEntity[] x = tableAdapters.Select( x => ( ( ( dynamic ) x ).Entity as AbstractEntity) ).ToArray();
        //    BuildEntityGraph( x );


        //    //this.mGraph.Clear();
        //    //Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();

        //    //tableAdapters.ToList().ForEach( a => {

        //    //    mGraph.Add( ( AbstractEntity ) ( ( dynamic ) a ).Entity );
        //    //    mCache.AddOrCreate( a );

        //    //    ( ( AbstractEntity ) ( ( dynamic ) a ).Entity ).GetAllForeignkey().ForEach( x => {
        //    //        foreignKeys.Push( x );
        //    //        mGraph.Connect( x.Table , x );
        //    //    } );

        //    //} );

        //    //EntityForeignKey fkey = null;

        //    //while ( foreignKeys.TryPop( out fkey ) ) {

        //    //    if ( tableAdapters.ToList().FirstOrDefault( a => ( ( dynamic ) a ).Entity.GetType() == fkey.TypeRefenceTable ) != null ) {

        //    //        //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

        //    //        //Se è presente nel grafo il tipo entity in fkey.TypeRefenceTable 
        //    //        // allora se RefenceTable è nullo copia il valore in mGraph
        //    //        // altrimenti instazia la tabella ed aggiungila ad mGraph

        //    //        //if ( fkey.RefenceTable == null ) {

        //    //        if ( InstantiateReferenceTable( fkey ) ) {

        //    //            mGraph.Add( fkey.RefenceTable );

        //    //            var allfk = fkey.RefenceTable.GetAllForeignkey();

        //    //            foreach ( var ffk in allfk ) {

        //    //                if ( !mGraph.IsConnected( ffk ) ) {

        //    //                    foreignKeys.Push( ffk );
        //    //                    InstantiateReferenceTable( ffk );
        //    //                    mGraph.Connect( ffk.RefenceTable , ffk );

        //    //                }

        //    //            }

        //    //            //}

        //    //            //var x = mGraph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeRefenceTable );
        //    //            //if ( x != null ) {
        //    //            //    fkey.RefenceTable = x;
        //    //            //    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //    //            //    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //    //            //} else {
        //    //            //    fkey.InstantiateRefenceTable();
        //    //            //    mGraph.Add( fkey.RefenceTable );
        //    //            //    var allfk = fkey.RefenceTable.GetAllForeignkey();
        //    //            //    foreach ( var ffk in allfk ) {
        //    //            //        if ( !mGraph.IsConnected( ffk ) ) {
        //    //            //            foreignKeys.Push( ffk );
        //    //            //            InstantiateReferenceTable( ffk );
        //    //            //            mGraph.Connect( ffk.RefenceTable , ffk );
        //    //            //        }
        //    //            //    }
        //    //            //}


        //    //        }
        //    //    }
        //    //}


        //    //var _Entities = mGraph.Keys.ToList();
        //    //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
        //    //    foreach ( var _fk in mGraph[e] ) {

        //    //        if ( _fk.RefenceTable != null ) {
        //    //            _fk.addEntities( _Entities );
        //    //            //_fk.createKey(); inutile?

        //    //        }
        //    //    }
        //    //}

        //    ////foreach ( dynamic a in tableAdapters ) {
        //    ////    if ( a.Entity != null ) {
        //    ////        mGraph.Add( (AbstractEntity) a.Entity );
        //    ////        foreach ( EntityForeignKey fk in a.Entity.GetAllForeignkey() ) {
        //    ////            Console.WriteLine( fk );
        //    ////            mGraph.Connect( ( AbstractEntity ) a.Entity , fk );
        //    ////            fk.InstantiateRefenceTable();
        //    ////            mGraph.Add( fk.RefenceTable );
        //    ////            fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );
        //    ////        }
        //    ////    }
        //    ////    EntityForeignKey fkey = null;
        //    ////    while ( foreignKeys.TryPop( out fkey ) ) {
        //    ////        //fkey.InstantiateRefenceTable();
        //    ////        //mGraph.Add( fk.RefenceTable );
        //    ////        //fk.RefenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.RefenceTable , x ); } );
        //    ////        //foreignKeyTables.Add( fk.RefenceTable );
        //    ////    }
        //    ////}

        //}

        public List<AbstractEntity> GetTopologicalOrder() {
            List<AbstractEntity> result = new List<AbstractEntity>();
            mCache.Values.ToList().ForEach( l => l.ForEach( a => result.Add( ( ( dynamic ) a ) ) ) );
            //var x = mCache.Values.ToList().Select( l => l.Select( a => ( ( dynamic ) a ).Entity ).ToList() ).ToList();            
            return result;
        }


        private bool CacheContainsEntityType( EntityForeignKey fk ) {

            foreach ( var (t, list) in mCache ) {
                foreach ( var a in list ) {
                    if ( ( ( dynamic ) a ).GetType() == fk.TypeRefenceTable ) {
                        return true;
                    }
                }
            }

            return false;
        }


        public List<EntityForeignKey> GetForeignKeys() {

            //Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
            Dictionary<AbstractEntity , bool> visited = new Dictionary<AbstractEntity , bool>();
            //List<EntityForeignKey> result = new List<EntityForeignKey>();
            List<EntityForeignKey> result = new List<EntityForeignKey>();
            List<EntityForeignKey> edge = new List<EntityForeignKey>();

            foreach ( var (e, listFK) in this ) {

                if ( ( !visited.ContainsKey( e ) ) ) {

                    visited?.TryAdd( e , ( listFK.Count > 0 ) );
                }

                foreach ( EntityForeignKey fk in listFK.Where( x => x.RefenceTable != null ).ToList() ) {

                    if ( ( fk.RefenceTable != null ) && ( CacheContainsEntityType( fk ) ) ) {

                        if ( visited.ContainsKey( fk.RefenceTable ) ) {

                            if ( visited[fk.RefenceTable] )
                                continue;
                            else {
                                result.Add( fk );
                                visited[fk.RefenceTable] = true;
                            }

                        } else {
                            result.Add( fk );
                            visited?.TryAdd( fk.RefenceTable , true );
                        }

                        //if (( visited.ContainsKey( fk.RefenceTable ) ) && ( visited[fk.RefenceTable] )) {
                        //    //if ( visited[fk.RefenceTable])
                        //    continue;
                        //}
                        //if ( ( !visited.Contains( fk.RefenceTable ) ) ) 
                        //    visited?.Push( fk.RefenceTable );


                    }


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

                }

            }

            return result;


        }


    }




}
