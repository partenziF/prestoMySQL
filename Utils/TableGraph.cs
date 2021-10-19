using prestoMySQL.Entity;
using prestoMySQL.ForeignKey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using prestoMySQL.Helper;
using prestoMySQL.Table;

namespace prestoMySQL.Utils {
    public class TableGraph : IDictionary<AbstractEntity , List<EntityForeignKey>> {

        private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph;
        private Dictionary<Type , List<AbstractEntity>> mCache;

        public TableGraph() {
            Graph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();
            Cache = new Dictionary<Type , List<AbstractEntity>>();
        }

        public List<EntityForeignKey> this[AbstractEntity key] { get => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph )[key]; set => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph )[key] = value; }

        public ICollection<AbstractEntity> Keys => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).Keys;

        public ICollection<List<EntityForeignKey>> Values => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).Values;

        public int Count => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).Count;

        public bool IsReadOnly => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).IsReadOnly;

        public Dictionary<Type , List<AbstractEntity>> Cache { get => this.mCache; set => this.mCache = value; }
        public Dictionary<AbstractEntity , List<EntityForeignKey>> Graph { get => this.mGraph; set => this.mGraph = value; }

        public void Add( AbstractEntity key , List<EntityForeignKey> value ) {
            ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).Add( key , value );
        }

        public void Add( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).Add( item );
        }

        public void Clear() {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).Clear();
        }

        public bool Contains( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).Contains( item );
        }

        public bool ContainsKey( AbstractEntity key ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).ContainsKey( key );
        }

        public void CopyTo( KeyValuePair<AbstractEntity , List<EntityForeignKey>>[] array , int arrayIndex ) {
            ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).CopyTo( array , arrayIndex );
        }

        public IEnumerator<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> GetEnumerator() {
            return ( ( IEnumerable<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).GetEnumerator();
        }

        public bool Remove( AbstractEntity key ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).Remove( key );
        }

        public bool Remove( KeyValuePair<AbstractEntity , List<EntityForeignKey>> item ) {
            return ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.Graph ).Remove( item );
        }

        public bool TryGetValue( AbstractEntity key , [MaybeNullWhen( false )] out List<EntityForeignKey> value ) {
            return ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.Graph ).TryGetValue( key , out value );
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ( ( IEnumerable ) this.Graph ).GetEnumerator();
        }


        public void Add( AbstractEntity Head ) {
            if ( !Graph.ContainsKey( Head ) )
                Graph.Add( Head , new List<EntityForeignKey>() );
        }

        public void Connect( AbstractEntity Head , EntityForeignKey Node ) {
            if ( Graph.ContainsKey( Head ) ) {
                if ( !Graph[Head].Contains( Node ) ) {
                    Graph[Head].Add( Node );
                }
            } else {
                Graph.Add( Head , new List<EntityForeignKey>() { Node } );
            }
        }

        public bool IsConnected( EntityForeignKey Node ) {

            //foreach ( var entity in mGraph.Keys.ToList() ) {

            //    if ( entity.GetType() == Node.Table.GetType() ) {
            //        //var xxxxx = Graph[entity].FirstOrDefault( x => x.TypeReferenceTable == Node.TypeReferenceTable );
            //        foreach ( var x in mGraph[entity] ) {
            //            if ( Node.TypeReferenceTable == x.TypeReferenceTable ) {
            //                return true;
            //            }
            //        }

            //    }

            //}
            //return false;

            throw new NotImplementedException();
        }


        private bool InstantiateReferenceTable( string ForeignkeyName , ForeignKeyInfo foreignKeyInfo ) {

            if ( foreignKeyInfo.ReferenceTable == null ) {

                //var x = mGraph.Keys.FirstOrDefault( x => (x.GetType() == fkey.TypeReferenceTable) && ( (x.FkNames == null) || (x.FkNames == fkey.ForeignkeyName)) );
                //fkey.TypeReferenceTable ) && ( ( x.FkNames == null ) || ( x.FkNames.Count == 0 ) || ( x.FkNames.Contains( fkey.ForeignkeyName ) ) ) ).ToList();

                var listOfTypeReferenceTable = Graph.Keys.Where( x => ( x.GetType() == foreignKeyInfo.TypeReferenceTable ) ).ToList();
                AbstractEntity referencedTable = null;
                if ( listOfTypeReferenceTable.Count > 0 ) {

                    var listOfTableForeignKeyName = listOfTypeReferenceTable.Where( x => x.FkNames.Contains( ForeignkeyName ) ).ToList();
                    if ( listOfTableForeignKeyName.Count() == 0 ) {
                        referencedTable = listOfTypeReferenceTable.FirstOrDefault( x => ( ( !x.FkNames?.Contains( ForeignkeyName ) ?? false ) && ( x.mAliasName == foreignKeyInfo.mReferenceTableAlias ) ) );
                        if ( referencedTable is null )
                            referencedTable = listOfTypeReferenceTable.FirstOrDefault( x => ( ( x.FkNames.Count() == 0 ) && ( x.mAliasName is null ) ) );
                    } else {
                        referencedTable = listOfTableForeignKeyName.FirstOrDefault( x => ( x.mAliasName == foreignKeyInfo.mReferenceTableAlias ) );
                        if ( referencedTable is null )
                            referencedTable = listOfTypeReferenceTable.FirstOrDefault( x => ( x.mAliasName is null ) );
                    }

                    if ( referencedTable is not null ) {

                        referencedTable.FkNames.Add( ForeignkeyName );
                        foreignKeyInfo.ReferenceTable = referencedTable;

                        if ( foreignKeyInfo.mReferenceTableAlias is not null ) {
                            if ( string.IsNullOrWhiteSpace( referencedTable.mAliasName ) ) {
                                SQLTableEntityHelper.SetAliasName( referencedTable , foreignKeyInfo.mReferenceTableAlias );
                            }

                        }

                        return true;


                    }
                }




                /*                var xlist = Graph.Keys.Where( x => ( x.GetType() == foreignKeyInfo.TypeReferenceTable ) ).ToList();

                                var xxlist = xlist.Where( x => x.FkNames.Contains( ForeignkeyName ) ).ToList();
                                var referencedTable = xlist.FirstOrDefault( x => ( x.FkNames != null ) && ( !x.FkNames.Contains( ForeignkeyName ) ) );

                                if ( xlist.Count() > 1 ) {
                                    //&& ( ( x.FkNames == null ) || ( x.FkNames.Count == 0 ) || ( x.FkNames.Contains( ForeignkeyName )

                                    foreach ( var x in xlist ) {

                                        if ( ( x.FkNames == null ) || ( x.FkNames.Count == 0 ) || ( x.FkNames.Contains( ForeignkeyName ) ) ) {

                                            if ( x != null ) {
                                                if ( !x.FkNames.Contains( ForeignkeyName ) )
                                                    x.FkNames.Add( ForeignkeyName );
                                                foreignKeyInfo.ReferenceTable = x;
                                                if ( foreignKeyInfo.mReferenceTableAlias is not null ) {
                                                    x.mAliasName = foreignKeyInfo.mReferenceTableAlias;
                                                    foreach ( var pi in SQLTableEntityHelper.getPropertyIfColumnDefinition( x.GetType() ) ) {
                                                        var c = pi.GetValue( x );
                                                        ( ( TableReference ) ( c as dynamic ).mTable ).TableAlias = x.AliasName;
                                                    }

                                                }

                                                // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
                                                // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
                                                return false;

                                            } else {
                                                throw new NotImplementedException();
                                                //foreignKeyInfo.InstantiateReferenceTable();
                                                return true;
                                            }


                                        } else {

                                            if ( ( x.FkNames != null ) && ( x.FkNames.Count > 0 ) && ( !x.FkNames.Contains( ForeignkeyName ) ) ) {

                                                if ( foreignKeyInfo.mReferenceTableAlias is null ) {

                                                    if (( x != null )  && (x.AliasName is null)){
                                                        if ( !x.FkNames.Contains( ForeignkeyName ) )
                                                            x.FkNames.Add( ForeignkeyName );
                                                        foreignKeyInfo.ReferenceTable = x;
                                                    }



                                                } else {
                                                    if ( foreignKeyInfo.mReferenceTableAlias.Equals( x.AliasName ?? String.Empty ) ) {


                                                    } else {
                                                        continue;
                                                    }
                                                }

                                            }


                                        }

                                    }

                                } else {


                                    var result = false;

                                    foreach ( var x in xlist ) {

                                        if ( x != null ) {
                                            if ( !x.FkNames.Contains( ForeignkeyName ) )
                                                x.FkNames.Add( ForeignkeyName );
                                            foreignKeyInfo.ReferenceTable = x;
                                            if ( foreignKeyInfo.mReferenceTableAlias is not null ) {
                                                x.mAliasName = foreignKeyInfo.mReferenceTableAlias;
                                                foreach ( var pi in SQLTableEntityHelper.getPropertyIfColumnDefinition( x.GetType() ) ) {

                                                    var c = pi.GetValue( x );
                                                    ( ( TableReference ) ( c as dynamic ).mTable ).TableAlias = x.AliasName;
                                                }

                                            }


                                            // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
                                            // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte

                                        } else {
                                            result = true;
                                            throw new NotImplementedException();
                                            //foreignKeyInfo.InstantiateReferenceTable();

                                        }

                                    }

                                    return result;

                                }

                                */
                //Questo serev per la chiave ManyToMany
                //var xlist = mGraph.Keys.Where( x => ( x.GetType() == foreignKeyInfo.TypeReferenceTable ) ).ToList();


            }

            return false;

        }


        public bool InstantiateReferenceTable( EntityForeignKey fkey ) {
            throw new NotImplementedException();

            //if ( fkey.KeyLength > 1 ) {

            //bool result = false;

            //foreach ( var (t, d) in fkey.foreignKeysInfo ) {

            //    foreach ( var (name, info) in fkey.foreignKeysInfo[t] ) {

            //        if ( fkey.foreignKeysInfo[t][name].ReferenceTable is null ) {

            //            var xlist = mGraph.Keys.Where( x => ( x.GetType() == fkey.foreignKeysInfo[t][name].TypeReferenceTable ) ).ToList();

            //            foreach ( var x in xlist ) {

            //                if ( x != null ) {

            //                    x.FkNames.Add( name );
            //                    fkey.foreignKeysInfo[t][name].ReferenceTable = x;

            //                    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
            //                    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
            //                    //return false;
            //                    result = false;

            //                } else {

            //                    fkey.InstantiateReferenceTable( t , name );
            //                    //return true;
            //                    result = true;
            //                }


            //            }


            //        }

            //    }
            //}

            //return result;

            //} else {


            ////Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

            ////Se è presente nel grafo il tipo entity in fkey.TypeReferenceTable 
            //// allora se ReferenceTable è nullo copia il valore in mGraph
            //// altrimenti instazia la tabella ed aggiungila ad mGraph

            //if ( fkey.ReferenceTable == null ) {

            //    //var x = mGraph.Keys.FirstOrDefault( x => (x.GetType() == fkey.TypeReferenceTable) && ( (x.FkNames == null) || (x.FkNames == fkey.ForeignkeyName)) );
            //    //var xlist = mGraph.Keys.Where( x => ( x.GetType() == fkey.TypeReferenceTable ) && ( ( x.FkNames == null ) || ( x.FkNames.Count == 0 ) || ( x.FkNames.Contains( fkey.ForeignkeyName ) ) ) ).ToList();
            //    var xlist = mGraph.Keys.Where( x => ( x.GetType() == fkey.TypeReferenceTable ) ).ToList();

            //    foreach ( var x in xlist ) {

            //        if ( x != null ) {

            //            if ( !x.FkNames.Contains( fkey.ForeignkeyName ) )
            //                x.FkNames.Add( fkey.ForeignkeyName );
            //            fkey.ReferenceTable = x;

            //            // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
            //            // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
            //            return false;

            //        } else {

            //            fkey.InstantiateReferenceTable();
            //            return true;
            //        }

            //    }

            //}

            //return false;

            //}
        }

        public void ReplaceEntityGraph( AbstractEntity old , AbstractEntity @new ) {

            Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();
            ///old.FkNames.ForEach( x => @new.FkNames.Add( x ) );

            var tableEntity = new List<AbstractEntity>() { @new };
            foreach ( var (e, list) in Graph ) {
                foreach ( var fk in list ) {
                    foreach ( var info in fk.foreignKeyInfo ) {
                        if ( ( info.ReferenceTable != null ) && ( info.ReferenceTable == old ) ) {
                            info.ReferenceTable = null;
                            if ( info.Table != null ) tableEntity.Add( info.Table );
                        } else if ( ( info.Table != null ) && ( info.Table == old ) ) {
                            info.Table = null;
                            if ( info.ReferenceTable != null ) tableEntity.Add( info.ReferenceTable );
                        }
                    }
                }
            }

            Graph.Remove( old );

            if ( Cache.ContainsKey( old.GetType() ) )
                Cache[old.GetType()].Remove( old );

            Graph.Add( @new );
            Cache.AddOrCreate( @new );


            tableEntity.ToList().ForEach( a => {

                ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
                    foreignKeys.Push( x );
                    foreach ( var info in x.foreignKeyInfo ) {
                        Graph.Connect( info.Table , x );
                    }
                } );

            } );

            EntityForeignKey fkey = null;
            while ( foreignKeys.TryPop( out fkey ) ) {
                foreach ( var info in fkey.foreignKeyInfo ) {

                    if ( InstantiateReferenceTable( fkey.ForeignkeyName , info ) ) {
                        throw new NotImplementedException();
                    }
                }
            }


            var _Entities = Graph.Keys.ToList();
            var _order = new Dictionary<AbstractEntity , int>();

            foreach ( var e in _Entities ) {
                if ( !_order.ContainsKey( e ) )
                    _order.Add( e , 0 );
                foreach ( var fk in e.mforeignKeys ) {
                    _order[e] += fk.foreignKeyInfo.Where( x => x.ReferenceTable != null ).Count();
                }
            }


            foreach ( (AbstractEntity e, _) in _order.OrderBy( v => v.Value ) ) {
                foreach ( var fk in Graph[e] ) {
                    //foreach ( var info in fk.foreignKeyInfo.Where( x => x.ReferenceTable != null ) ) {
                    fk.addEntities( _Entities );
                    //}
                }
            }


            //Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();
            /////old.FkNames.ForEach( x => @new.FkNames.Add( x ) );

            //var tableEntity = new List<AbstractEntity>() { @new };

            //foreach ( var (e, list) in mGraph ) {
            //    foreach ( var fk in list ) {
            //        foreach ( var (_, infos) in fk.foreignKeysInfo ) {
            //            foreach ( var (_, info) in infos ) {
            //                if ( ( info.ReferenceTable != null ) && ( info.ReferenceTable == old ) ) {
            //                    info.ReferenceTable = null;
            //                    if ( info.Table != null ) tableEntity.Add( info.Table );
            //                } else if ( ( info.Table != null ) && ( info.Table == old ) ) {
            //                    info.Table = null;
            //                    if ( info.ReferenceTable != null ) tableEntity.Add( info.ReferenceTable );
            //                }
            //            }
            //        }
            //    }
            //}

            //mGraph.Remove( old );

            //if ( mCache.ContainsKey( old.GetType() ) )
            //    mCache[old.GetType()].Remove( old );

            //mGraph.Add( @new );
            //mCache.AddOrCreate( @new );


            //tableEntity.ToList().ForEach( a => {

            //    //                mGraph.Add( ( AbstractEntity ) a );
            //    //mCache.AddOrCreate( ( AbstractEntity ) a );

            //    ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
            //        foreignKeys.Push( x );
            //        mGraph.Connect( x.Table , x );
            //    } );

            //} );



            //EntityForeignKey fkey = null;
            //while ( foreignKeys.TryPop( out fkey ) ) {

            //    if ( fkey.KeyLength > 1 ) {

            //        foreach ( var (t, d) in fkey.foreignKeysInfo ) {

            //            //if ( tableEntity.ToList().FirstOrDefault( a => fkey.foreignKeysInfo.ContainsKey( a.GetType() ) ) != null ) {

            //            foreach ( var (name, info) in fkey.foreignKeysInfo[t] ) {

            //                if ( InstantiateReferenceTable( name , fkey.foreignKeysInfo[t][name] ) ) {

            //                    fkey.InstantiateReferenceTable( t , name );

            //                    mGraph.Add( fkey.foreignKeysInfo[t][name].ReferenceTable );
            //                    var allfk = fkey.foreignKeysInfo[t][name].ReferenceTable.GetAllForeignkey();

            //                    foreach ( var ffk in allfk ) {

            //                        if ( !mGraph.IsConnected( ffk ) ) {

            //                            foreignKeys.Push( ffk );
            //                            InstantiateReferenceTable( ffk );
            //                            mGraph.Connect( ffk.ReferenceTable , ffk );

            //                        }

            //                    }


            //                }

            //            }


            //            //}

            //        }

            //    } else {
            //        //E' neccessario?
            //        //if ( tableEntity.ToList().FirstOrDefault( a => a.GetType() == fkey.TypeReferenceTable ) != null ) {

            //        if ( InstantiateReferenceTable( fkey ) ) {

            //            mGraph.Add( fkey.ReferenceTable );

            //            var allfk = fkey.ReferenceTable.GetAllForeignkey();

            //            foreach ( var ffk in allfk ) {

            //                if ( !mGraph.IsConnected( ffk ) ) {

            //                    foreignKeys.Push( ffk );
            //                    InstantiateReferenceTable( ffk );
            //                    mGraph.Connect( ffk.ReferenceTable , ffk );

            //                }

            //            }
            //        }

            //    }

            //}

            //var _Entities = mGraph.Keys.ToList();

            //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Where( x => x.ReferenceTables() != null ).Count() ).ToList() ) {

            //    foreach ( var _fk in mGraph[e].Where( x => x.ReferenceTables() != null ) ) {

            //        _fk.addEntities( _Entities );

            //    }

            //}


        }

        public  void AddEntityGraph( AbstractEntity a ) {
            Graph.Add( ( AbstractEntity ) a );
            Cache.AddOrCreate( ( AbstractEntity ) a );

            ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
                
                foreach ( var info in x.foreignKeyInfo ) {
                    Graph.Connect( info.Table , x );
                }
            } );

        }


        public void BuildEntityGraph( params AbstractEntity[] tableEntity ) {

            this.Graph.Clear();
            Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();
            Stack<EntityForeignKey> visitedForeignKeys = new Stack<EntityForeignKey>();

            //Create Node -> Adiacent list of node
            tableEntity.ToList().ForEach( a => {

                Graph.Add( ( AbstractEntity ) a );
                Cache.AddOrCreate( ( AbstractEntity ) a );

                ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
                    foreignKeys.Push( x );
                    foreach ( var info in x.foreignKeyInfo ) {
                        Graph.Connect( info.Table , x );
                    }
                } );

            } );

            EntityForeignKey fkey = null;
            while ( foreignKeys.TryPop( out fkey ) ) {

                visitedForeignKeys.Push( fkey );
                foreach ( var info in fkey.foreignKeyInfo ) {

                    if ( InstantiateReferenceTable( fkey.ForeignkeyName , info ) ) {

                        Graph.Add( info.ReferenceTable );
                        var allfk = info.ReferenceTable.GetAllForeignkey();

                        foreach ( var ffk in allfk ) {

                            if ( !Graph.IsConnected( ffk ) ) {
                                if ( ( !foreignKeys.Contains( ffk ) ) && ( !visitedForeignKeys.Contains( ffk ) ) ) {
                                    foreignKeys.Push( ffk );
                                } else {
                                    visitedForeignKeys.Push( ffk );
                                }
                                //foreach ( var infoffk in ffk.foreignKeyInfo ) {

                                //    InstantiateReferenceTable( ffk );
                                //    Graph.Connect( infoffk.ReferenceTable , ffk );
                                //}

                            }

                        }


                    }

                }

            }


            var _Entities = Graph.Keys.ToList();

            var _order = new Dictionary<AbstractEntity , int>();

            foreach ( var e in tableEntity ) {
                if ( !_order.ContainsKey( e ) )
                    _order.Add( e , 0 );
                foreach ( var fk in e.mforeignKeys ) {
                    _order[e] += fk.foreignKeyInfo.Where( x => x.ReferenceTable != null ).Count();
                }
            }


            foreach ( (AbstractEntity e, _) in _order.OrderBy( v => v.Value ) ) {
                foreach ( var fk in Graph[e] ) {
                    //foreach ( var info in fk.foreignKeyInfo.Where( x => x.ReferenceTable != null ) ) {
                    fk.addEntities( _Entities );
                    //}
                }
            }

            //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Where( x => x.foreignKeyInfo.where( y => y.ReferenceTables != null ).Count() ).ToList() ) {

            //    //foreach ( var _fk in mGraph[e].Where( x => x.ReferenceTable != null ) ) {
            //    foreach ( var _fk in mGraph[e].Where( x => x.ReferenceTables() != null ) ) {

            //        //if ( _fk.ReferenceTable != null ) {
            //        _fk.addEntities( _Entities );
            //        //_fk.createKey(); inutile?

            //        //}

            //    }

            //}


            //throw new NotImplementedException();

            //this.mGraph.Clear();
            //Stack<EntityForeignKey> foreignKeys = new Stack<EntityForeignKey>();

            //tableEntity.ToList().ForEach( a => {

            //    mGraph.Add( ( AbstractEntity ) a );
            //    mCache.AddOrCreate( ( AbstractEntity ) a );

            //    ( ( AbstractEntity ) a ).GetAllForeignkey().ForEach( x => {
            //        foreignKeys.Push( x );
            //        mGraph.Connect( x.Table , x );
            //    } );

            //} );

            //EntityForeignKey fkey = null;

            //while ( foreignKeys.TryPop( out fkey ) ) {

            //    if ( fkey.KeyLength > 0 ) {

            //        foreach ( var (t, d) in fkey.foreignKeysInfo ) {

            //            if ( tableEntity.ToList().FirstOrDefault( a => fkey.foreignKeysInfo.ContainsKey( a.GetType() ) ) != null ) {

            //                foreach ( var (name, info) in fkey.foreignKeysInfo[t] ) {

            //                    if ( InstantiateReferenceTable( name , fkey.foreignKeysInfo[t][name] ) ) {

            //                        fkey.InstantiateReferenceTable( t , name );

            //                        mGraph.Add( fkey.foreignKeysInfo[t][name].ReferenceTable );
            //                        var allfk = fkey.foreignKeysInfo[t][name].ReferenceTable.GetAllForeignkey();

            //                        foreach ( var ffk in allfk ) {

            //                            if ( !mGraph.IsConnected( ffk ) ) {

            //                                foreignKeys.Push( ffk );
            //                                InstantiateReferenceTable( ffk );
            //                                mGraph.Connect( ffk.ReferenceTable , ffk );

            //                            }

            //                        }


            //                    }

            //                }

            //            }

            //        }

            //        //if ( tableEntity.ToList().FirstOrDefault( a => fkey.foreignKeysInfo.ContainsKey( a.GetType() ) ) != null ) {
            //        //    if ( InstantiateReferenceTable( fkey ) ) {
            //        //        mGraph.Add( fkey.ReferenceTable );

            //        //        var allfk = fkey.ReferenceTable.GetAllForeignkey();

            //        //        foreach ( var ffk in allfk ) {

            //        //            if ( !mGraph.IsConnected( ffk ) ) {

            //        //                foreignKeys.Push( ffk );
            //        //                InstantiateReferenceTable( ffk );
            //        //                mGraph.Connect( ffk.ReferenceTable , ffk );

            //        //            }

            //        //        }
            //        //    }
            //        //}

            //    } else {

            //        if ( tableEntity.ToList().FirstOrDefault( a => a.GetType() == fkey.TypeReferenceTable ) != null ) {

            //            //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

            //            //Se è presente nel grafo il tipo entity in fkey.TypeReferenceTable 
            //            // allora se ReferenceTable è nullo copia il valore in mGraph
            //            // altrimenti instazia la tabella ed aggiungila ad mGraph

            //            //if ( fkey.ReferenceTable == null ) {

            //            if ( InstantiateReferenceTable( fkey ) ) {

            //                mGraph.Add( fkey.ReferenceTable );

            //                var allfk = fkey.ReferenceTable.GetAllForeignkey();

            //                foreach ( var ffk in allfk ) {

            //                    if ( !mGraph.IsConnected( ffk ) ) {

            //                        foreignKeys.Push( ffk );
            //                        InstantiateReferenceTable( ffk );
            //                        mGraph.Connect( ffk.ReferenceTable , ffk );

            //                    }

            //                }

            //            }
            //        }


            //    }
            //}


            //var _Entities = mGraph.Keys.ToList();

            ////foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Where( x => x.ReferenceTable != null ).Count() ).ToList() ) {
            //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Where( x => x.ReferenceTables() != null ).Count() ).ToList() ) {

            //    //foreach ( var _fk in mGraph[e].Where( x => x.ReferenceTable != null ) ) {
            //    foreach ( var _fk in mGraph[e].Where( x => x.ReferenceTables() != null ) ) {

            //        //if ( _fk.ReferenceTable != null ) {
            //        _fk.addEntities( _Entities );
            //        //_fk.createKey(); inutile?

            //        //}

            //    }

            //}

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

        //    //    if ( tableAdapters.ToList().FirstOrDefault( a => ( ( dynamic ) a ).Entity.GetType() == fkey.TypeReferenceTable ) != null ) {

        //    //        //Non istanziare se è già presente un'istanza con lo stesso tipo nel grafo

        //    //        //Se è presente nel grafo il tipo entity in fkey.TypeReferenceTable 
        //    //        // allora se ReferenceTable è nullo copia il valore in mGraph
        //    //        // altrimenti instazia la tabella ed aggiungila ad mGraph

        //    //        //if ( fkey.ReferenceTable == null ) {

        //    //        if ( InstantiateReferenceTable( fkey ) ) {

        //    //            mGraph.Add( fkey.ReferenceTable );

        //    //            var allfk = fkey.ReferenceTable.GetAllForeignkey();

        //    //            foreach ( var ffk in allfk ) {

        //    //                if ( !mGraph.IsConnected( ffk ) ) {

        //    //                    foreignKeys.Push( ffk );
        //    //                    InstantiateReferenceTable( ffk );
        //    //                    mGraph.Connect( ffk.ReferenceTable , ffk );

        //    //                }

        //    //            }

        //    //            //}

        //    //            //var x = mGraph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeReferenceTable );
        //    //            //if ( x != null ) {
        //    //            //    fkey.ReferenceTable = x;
        //    //            //    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //    //            //    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //    //            //} else {
        //    //            //    fkey.InstantiateReferenceTable();
        //    //            //    mGraph.Add( fkey.ReferenceTable );
        //    //            //    var allfk = fkey.ReferenceTable.GetAllForeignkey();
        //    //            //    foreach ( var ffk in allfk ) {
        //    //            //        if ( !mGraph.IsConnected( ffk ) ) {
        //    //            //            foreignKeys.Push( ffk );
        //    //            //            InstantiateReferenceTable( ffk );
        //    //            //            mGraph.Connect( ffk.ReferenceTable , ffk );
        //    //            //        }
        //    //            //    }
        //    //            //}


        //    //        }
        //    //    }
        //    //}


        //    //var _Entities = mGraph.Keys.ToList();
        //    //foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
        //    //    foreach ( var _fk in mGraph[e] ) {

        //    //        if ( _fk.ReferenceTable != null ) {
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
        //    ////            fk.InstantiateReferenceTable();
        //    ////            mGraph.Add( fk.ReferenceTable );
        //    ////            fk.ReferenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.ReferenceTable , x ); } );
        //    ////        }
        //    ////    }
        //    ////    EntityForeignKey fkey = null;
        //    ////    while ( foreignKeys.TryPop( out fkey ) ) {
        //    ////        //fkey.InstantiateReferenceTable();
        //    ////        //mGraph.Add( fk.ReferenceTable );
        //    ////        //fk.ReferenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.ReferenceTable , x ); } );
        //    ////        //foreignKeyTables.Add( fk.ReferenceTable );
        //    ////    }
        //    ////}

        //}

        public List<AbstractEntity> GetTopologicalOrder() {
            List<AbstractEntity> result = new List<AbstractEntity>();
            Cache.Values.ToList().ForEach( l => l.ForEach( a => result.Add( ( ( dynamic ) a ) ) ) );
            //var x = mCache.Values.ToList().Select( l => l.Select( a => ( ( dynamic ) a ).Entity ).ToList() ).ToList();            
            return result;
        }


        private bool CacheContainsEntityType( EntityForeignKey foreignKey ) {

            //throw new NotImplementedException();
            foreach ( var (_, list) in Cache ) {

                foreach ( AbstractEntity e in list ) {

                    if ( foreignKey.foreignKeyInfo.Count == 1 ) {

                        foreach ( var info in foreignKey.foreignKeyInfo ) {

                            if ( ( info.TypeReferenceTable != null ) && ( e.GetType() == info.TypeReferenceTable ) )
                                return true;
                        }

                    } else if ( foreignKey.foreignKeyInfo.Count > 1 ) {

                        bool result = true;

                        foreach ( var info in foreignKey.foreignKeyInfo ) {

                            result &= ( ( info.TypeReferenceTable != null ) && ( e.GetType() == info.TypeReferenceTable ) );

                        }

                        return result;

                    }

                }
            }

            return false;
            //foreach ( var (t, list) in mCache ) {
            //    foreach ( var a in list ) {
            //        if ( ( ( dynamic ) a ).GetType() == fk.TypeReferenceTable ) {
            //            return true;
            //        }
            //    }
            //}

            //return false;
        }


        private bool CacheContainsEntityType( ForeignKeyInfo fk ) {


            foreach ( var (t, list) in this.mCache ) {
                foreach ( var a in list ) {
                    if ( ( ( dynamic ) a ).GetType() == fk.TypeReferenceTable ) {
                        return true;
                    }
                }
            }

            return false;
        }


        //public List<EntityForeignKey> GetForeignKeys() {

        //    //throw new NotImplementedException();

        //    //Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
        //    Dictionary<AbstractEntity , bool> visited = new Dictionary<AbstractEntity , bool>();
        //    //List<EntityForeignKey> result = new List<EntityForeignKey>();
        //    List<EntityForeignKey> result = new List<EntityForeignKey>();
        //    List<EntityForeignKey> edge = new List<EntityForeignKey>();

        //    foreach ( var (e, listFK) in this ) {

        //        if ( ( !visited.ContainsKey( e ) ) ) {

        //            visited?.TryAdd( e , ( listFK.Count > 0 ) );
        //        }

        //        foreach ( EntityForeignKey fk in listFK ) {
        //            foreach ( var info in fk.foreignKeyInfo ) {

        //                if ( info.ReferenceTable != null ) {


        //                    if ( ( info.ReferenceTable != null ) && ( CacheContainsEntityType( fk ) ) ) {

        //                        if ( visited.ContainsKey( info.ReferenceTable ) ) {

        //                            if ( visited[info.ReferenceTable] )
        //                                continue;
        //                            else {
        //                                if ( !result.Contains( fk ) )
        //                                    result.Add( fk );
        //                                visited[info.ReferenceTable] = true;
        //                            }

        //                        } else {
        //                            if ( !result.Contains( fk ) )
        //                                result.Add( fk );
        //                            visited?.TryAdd( info.ReferenceTable , true );
        //                        }

        //                    }

        //                }


        //            }

        //        }

        //    }

        //    return result;


        //}

        public bool ContainsArc( Stack<Tuple<AbstractEntity , AbstractEntity>> visitedArc , Tuple<AbstractEntity , AbstractEntity> arc ) {
            var result = visitedArc.Contains( arc );
            if ( !result ) {

                foreach ( var a in visitedArc ) {
                    if ( ( a.Item1 == arc.Item1 ) && ( a.Item2 == arc.Item2 ) ) {
                        return true;
                    }
                }

            }
            return result;
        }

        public List<EntityForeignKey> GetForeignKeys( AbstractEntity startNode = null ) {

            if ( startNode is not null ) throw new NotImplementedException( "GetForeignKeys with start node not implemented" );

            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
            Stack<Tuple<AbstractEntity , AbstractEntity>> visitedArc = new Stack<Tuple<AbstractEntity , AbstractEntity>>();
            List<EntityForeignKey> result = new List<EntityForeignKey>();

            foreach ( var (e, listFK) in this ) {

                if ( !visited.Contains( e ) ) {

                    visited?.Push( e );
                }
                //var xxx = listFK.SelectMany( fk => fk.foreignKeyInfo ).Where( fkInfo => fkInfo.ReferenceTable != null ).ToList();
                if ( listFK.SelectMany( fk => fk.foreignKeyInfo ).Where( fkInfo => fkInfo.ReferenceTable != null ).ToList().Count > 0 ) {
                    foreach ( EntityForeignKey entityfk in listFK ) {

                        //var addToResult = false;

                        foreach ( var fkInfo in entityfk.foreignKeyInfo.Where( fk => fk.ReferenceTable != null ).ToList() ) {

                            //if ( ( fkInfo.ReferenceTable != null ) && ( CacheContainsEntityType( fkInfo ) ) ) {
                            if ( ( CacheContainsEntityType( fkInfo ) ) ) {

                                var arc = new Tuple<AbstractEntity , AbstractEntity>( fkInfo.Table , fkInfo.ReferenceTable );

                                if ( visitedArc.Contains( arc ) ) continue;
                                //if ( ContainsArc(visitedArc, arc ) ) continue;
                                visitedArc?.Push( arc );
                                visited?.Push( fkInfo.ReferenceTable );

                                //addToResult |= true;
                                if ( !result.Contains( entityfk ) )
                                    result.Add( entityfk );

                                //} else {
                                //    addToResult &= false;
                            }

                        }

                        //if ( addToResult ) {
                        //    if ( !result.Contains( entityfk ) )
                        //        result.Add( entityfk );
                        //}

                    }
                }

            }

            return result;


        }

    }

}
