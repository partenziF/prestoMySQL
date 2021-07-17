﻿using prestoMySQL.Adapter;
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
        public static bool IsConnected( this Dictionary<AbstractEntity , List<EntityForeignKey>> Graph , EntityForeignKey Node ) {

            foreach ( var entity in Graph.Keys.ToList() ) {

                if ( entity.GetType() == Node.Table.GetType() ) {
                    //var xxxxx = Graph[entity].FirstOrDefault( x => x.TypeRefenceTable == Node.TypeRefenceTable );
                    foreach ( var x in Graph[entity] ) {
                        if ( Node.TypeRefenceTable == x.TypeRefenceTable ) {
                            return true;
                        }
                    }

                }

            }
            return false;

        }
    }
}