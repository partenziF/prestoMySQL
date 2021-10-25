using prestoMySQL.Adapter;
using prestoMySQL.Entity;
using prestoMySQL.ForeignKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Helper {

    public static class CacheHelper {
        public static void AddOrCreate<X>( this Dictionary<Type , List<TableEntity>> d , X a ) where X : TableEntity {
            if ( d.ContainsKey( typeof( X ) ) ) {
                d[typeof( X )].Add( a );
            } else {
                d.Add( typeof( X ) , new List<TableEntity>() { a } );
            }

        }
        public static void AddOrCreate( this Dictionary<Type , List<TableEntity>> d , TableEntity a ) {
            if ( d.ContainsKey( a.GetType() ) ) {
                d[a.GetType()].Add( a );
            } else {
                d.Add( a.GetType() , new List<TableEntity>() { a } );
            }

        }
        public static void Remove( this Dictionary<Type , List<TableEntity>> d , TableEntity a ) {
            if ( d.ContainsKey( a.GetType() ) ) {
                d[a.GetType()].Remove( a );
                if ( d[a.GetType()].Count == 0 ) {
                    d.Remove( a.GetType() );
                }
            }

        }

    }

    public static class DictHelper {
        public static void AddOrCreate<X>( this Dictionary<Type , List<AbstractEntity>> d , X a ) where X : AbstractEntity {
            if ( d.ContainsKey( typeof( X ) ) ) {
                d[typeof( X )].Add( a );
            } else {
                d.Add( typeof( X ) , new List<AbstractEntity>() { a } );
            }

        }
        public static void AddOrCreate( this Dictionary<Type , List<AbstractEntity>> d , AbstractEntity a ) {
            if ( d.ContainsKey( a.GetType() ) ) {
                d[a.GetType()].Add( a );
            } else {
                d.Add( a.GetType() , new List<AbstractEntity>() { a } );
            }

        }

        public static void Remove( this Dictionary<Type , List<AbstractEntity>> d , AbstractEntity a ) {
            if ( d.ContainsKey( a.GetType() ) ) {
                d[a.GetType()].Remove( a );
                if ( d[a.GetType()].Count == 0 ) {
                    d.Remove( a.GetType() );
                }
            }

        }


    }

    public static class GraphHelper {
        public static void Add( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , AbstractEntity Head ) {
            if ( !Graph.ContainsKey( Head ) )
                Graph.Add( Head , new List<EntityForeignKey>() );
        }
        public static void Connect( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , AbstractEntity Head , EntityForeignKey Node ) {
            if ( Graph.ContainsKey( Head ) ) {
                if ( !Graph[Head].Contains( Node ) ) {
                    Graph[Head].Add( Node );
                }
            } else {
                Graph.Add( Head , new List<EntityForeignKey>() { Node } );
            }
        }

        public static void Disconnect( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , AbstractEntity Head , EntityForeignKey Node ) {
            if ( Graph.ContainsKey( Head ) ) {
                if ( Graph[Head].Contains( Node ) ) {
                    Graph[Head].Remove( Node );
                    if (( Graph[Head].Count) == 0)
                        Graph.Remove( Head );
                }
            } else {
                
                //Graph.Add( Head , new List<EntityForeignKey>() { Node } );
            }
        }

        //public static bool IsConnected( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , EntityForeignKey Node ) {
        public static bool IsConnected( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , EntityForeignKey Node ) {

            var result = true;
            if ( Graph.Keys.Count > 0 ) {
                foreach ( var entity in Graph.Keys.ToList() ) {

                    if ( Node.foreignKeyInfo.Count > 0 ) {
                        foreach ( var info in Node.foreignKeyInfo ) {

                            if ( entity.GetType() == info.Table.GetType() ) {

                                foreach ( var x in Graph[entity] ) {
                                    foreach ( var xx in x.foreignKeyInfo ) {

                                        result = result && ( info.TypeReferenceTable == xx.TypeReferenceTable );
                                        //if ( info.TypeReferenceTable == xx.TypeReferenceTable ) {

                                        //}
                                    }
                                }
                            } else {
                                return false;
                            }

                        }
                    } else {
                        return false;
                    }

                }

            } else {
                return false;
            }
            //foreach ( var entity in Graph.Keys.ToList() ) {


            //    if ( entity.GetType() == Node.Table.GetType() ) {
            //        //var xxxxx = Graph[entity].FirstOrDefault( x => x.TypeReferenceTable == Node.TypeReferenceTable );
            //        foreach ( var x in Graph[entity] ) {
            //            if ( Node.TypeReferenceTable == x.TypeReferenceTable ) {
            //                return true;
            //            }
            //        }

            //    }

            //}
            return false;

        }
    }
}
