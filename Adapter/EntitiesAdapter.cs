using Microsoft.Extensions.Logging;
using MySqlConnector;
using prestoMySQL.Adapter.Enum;
using prestoMySQL.Column.Interface;
using prestoMySQL.Entity;
using prestoMySQL.Entity.Interface;
using prestoMySQL.Extension;
using prestoMySQL.ForeignKey;
using prestoMySQL.Helper;
using prestoMySQL.Index;
using prestoMySQL.Query;
using prestoMySQL.Query.Interface;
using prestoMySQL.Query.SQL;
using prestoMySQL.SQL;
using prestoMySQL.Utils;
using prestoMySQL.Database.Interface;
using prestoMySQL.Database.MySQL;
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
        //private Dictionary<Type , Dictionary<string,TableEntity>> mEntitiesCache;


        public EntitiesAdapter( MySQLDatabase aMySQLDatabase , ILogger logger = null ) {
            this.mDatabase = aMySQLDatabase;
            this.mLogger = logger;
            _Graph = new TableGraph();
            mEntitiesCache = new Dictionary<Type , List<TableEntity>>();
            //mEntitiesCache = new Dictionary<Type , Dictionary<string,TableEntity>>();
            //_Graph.mCache = new Dictionary<Type , List<TableEntity>>();
        }


        //public ICollection<AbstractEntity> Keys => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Keys;
        //public ICollection<List<EntityForeignKey>> Values => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Values;
        //public int Count => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Count;
        //public bool IsReadOnly => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).IsReadOnly;

        public void Create<A1, E1, A2, E2>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                             where A2 : EntityAdapter<E2> where E2 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;

            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                }

            }


            a1 = NewInstanceAdapter<A1>();
            a1.Create();

            a2 = NewInstanceAdapter<A2>();
            a2.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );

            _Graph.BuildEntityGraph( a1 , a2 );

        }

        public void Create<A1, E1, A2, E2, A3, E3>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;

            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity {


            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                   where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                   where A5 : EntityAdapter<E5> where E5 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                   where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                   where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                                                                   where A6 : EntityAdapter<E6> where E6 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;
            A6 a6 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                } else if ( ( typeof( A6 ) == adapter.GetType() ) && ( a6 is null ) ) {
                    a6 = ( A6 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            a6 ??= NewInstanceAdapter<A6>();
            if ( a6.Entity is null ) a6.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );
            mEntitiesCache.AddOrCreate( a6 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                                      where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                                      where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                                      where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                                      where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                                                                                      where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                                                                                      where A7 : EntityAdapter<E7> where E7 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;
            A6 a6 = null;
            A7 a7 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                } else if ( ( typeof( A6 ) == adapter.GetType() ) && ( a6 is null ) ) {
                    a6 = ( A6 ) adapter;
                } else if ( ( typeof( A7 ) == adapter.GetType() ) && ( a7 is null ) ) {
                    a7 = ( A7 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            a6 ??= NewInstanceAdapter<A6>();
            if ( a6.Entity is null ) a6.Create();

            a7 ??= NewInstanceAdapter<A7>();
            if ( a7.Entity is null ) a7.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );
            mEntitiesCache.AddOrCreate( a6 );
            mEntitiesCache.AddOrCreate( a7 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7, A8, E8>( params TableEntity[] adapters )
                                                                        where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                        where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                        where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                        where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                        where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                                                                        where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                                                                        where A7 : EntityAdapter<E7> where E7 : AbstractEntity
                                                                        where A8 : EntityAdapter<E8> where E8 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;
            A6 a6 = null;
            A7 a7 = null;
            A8 a8 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                } else if ( ( typeof( A6 ) == adapter.GetType() ) && ( a6 is null ) ) {
                    a6 = ( A6 ) adapter;
                } else if ( ( typeof( A7 ) == adapter.GetType() ) && ( a7 is null ) ) {
                    a7 = ( A7 ) adapter;
                } else if ( ( typeof( A8 ) == adapter.GetType() ) && ( a8 is null ) ) {
                    a8 = ( A8 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            a6 ??= NewInstanceAdapter<A6>();
            if ( a6.Entity is null ) a6.Create();

            a7 ??= NewInstanceAdapter<A7>();
            if ( a7.Entity is null ) a7.Create();

            a8 ??= NewInstanceAdapter<A8>();
            if ( a8.Entity is null ) a8.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );
            mEntitiesCache.AddOrCreate( a6 );
            mEntitiesCache.AddOrCreate( a7 );
            mEntitiesCache.AddOrCreate( a8 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 );


        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7, A8, E8, A9, E9>( params TableEntity[] adapters )
                   where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                    where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                    where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                    where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                    where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                    where A7 : EntityAdapter<E7> where E7 : AbstractEntity
                    where A8 : EntityAdapter<E8> where E8 : AbstractEntity
                    where A9 : EntityAdapter<E9> where E9 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;
            A6 a6 = null;
            A7 a7 = null;
            A8 a8 = null;
            A9 a9 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                } else if ( ( typeof( A6 ) == adapter.GetType() ) && ( a6 is null ) ) {
                    a6 = ( A6 ) adapter;
                } else if ( ( typeof( A7 ) == adapter.GetType() ) && ( a7 is null ) ) {
                    a7 = ( A7 ) adapter;
                } else if ( ( typeof( A8 ) == adapter.GetType() ) && ( a8 is null ) ) {
                    a8 = ( A8 ) adapter;
                } else if ( ( typeof( A9 ) == adapter.GetType() ) && ( a9 is null ) ) {
                    a9 = ( A9 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            a6 ??= NewInstanceAdapter<A6>();
            if ( a6.Entity is null ) a6.Create();

            a7 ??= NewInstanceAdapter<A7>();
            if ( a7.Entity is null ) a7.Create();

            a8 ??= NewInstanceAdapter<A8>();
            if ( a8.Entity is null ) a8.Create();

            a9 ??= NewInstanceAdapter<A9>();
            if ( a9.Entity is null ) a9.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );
            mEntitiesCache.AddOrCreate( a6 );
            mEntitiesCache.AddOrCreate( a7 );
            mEntitiesCache.AddOrCreate( a8 );
            mEntitiesCache.AddOrCreate( a9 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 , a9 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7, A8, E8, A9, E9, A10, E10>( params TableEntity[] adapters )
                   where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                    where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                    where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                    where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                    where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                    where A7 : EntityAdapter<E7> where E7 : AbstractEntity
                    where A8 : EntityAdapter<E8> where E8 : AbstractEntity
                    where A9 : EntityAdapter<E9> where E9 : AbstractEntity
                    where A10 : EntityAdapter<E10> where E10 : AbstractEntity {

            _Graph.mCache.Clear();
            mEntitiesCache.Clear();

            A1 a1 = null;
            A2 a2 = null;
            A3 a3 = null;
            A4 a4 = null;
            A5 a5 = null;
            A6 a6 = null;
            A7 a7 = null;
            A8 a8 = null;
            A9 a9 = null;
            A10 a10 = null;


            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                } else if ( ( typeof( A3 ) == adapter.GetType() ) && ( a3 is null ) ) {
                    a3 = ( A3 ) adapter;
                } else if ( ( typeof( A4 ) == adapter.GetType() ) && ( a4 is null ) ) {
                    a4 = ( A4 ) adapter;
                } else if ( ( typeof( A5 ) == adapter.GetType() ) && ( a5 is null ) ) {
                    a5 = ( A5 ) adapter;
                } else if ( ( typeof( A6 ) == adapter.GetType() ) && ( a6 is null ) ) {
                    a6 = ( A6 ) adapter;
                } else if ( ( typeof( A7 ) == adapter.GetType() ) && ( a7 is null ) ) {
                    a7 = ( A7 ) adapter;
                } else if ( ( typeof( A8 ) == adapter.GetType() ) && ( a8 is null ) ) {
                    a8 = ( A8 ) adapter;
                } else if ( ( typeof( A9 ) == adapter.GetType() ) && ( a9 is null ) ) {
                    a9 = ( A9 ) adapter;
                } else if ( ( typeof( A10 ) == adapter.GetType() ) && ( a10 is null ) ) {
                    a10 = ( A10 ) adapter;
                }

            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            a3 ??= NewInstanceAdapter<A3>();
            if ( a3.Entity is null ) a3.Create();

            a4 ??= NewInstanceAdapter<A4>();
            if ( a4.Entity is null ) a4.Create();

            a5 ??= NewInstanceAdapter<A5>();
            if ( a5.Entity is null ) a5.Create();

            a6 ??= NewInstanceAdapter<A6>();
            if ( a6.Entity is null ) a6.Create();

            a7 ??= NewInstanceAdapter<A7>();
            if ( a7.Entity is null ) a7.Create();

            a8 ??= NewInstanceAdapter<A8>();
            if ( a8.Entity is null ) a8.Create();

            a9 ??= NewInstanceAdapter<A9>();
            if ( a9.Entity is null ) a9.Create();

            a10 ??= NewInstanceAdapter<A10>();
            if ( a10.Entity is null ) a10.Create();

            mEntitiesCache.AddOrCreate( a1 );
            mEntitiesCache.AddOrCreate( a2 );
            mEntitiesCache.AddOrCreate( a3 );
            mEntitiesCache.AddOrCreate( a4 );
            mEntitiesCache.AddOrCreate( a5 );
            mEntitiesCache.AddOrCreate( a6 );
            mEntitiesCache.AddOrCreate( a7 );
            mEntitiesCache.AddOrCreate( a8 );
            mEntitiesCache.AddOrCreate( a9 );
            mEntitiesCache.AddOrCreate( a10 );

            _Graph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 , a9 , a10 );

        }

        //public List<EntityForeignKey> this[AbstractEntity key] { get => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key]; set => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key] = value; }



        public List<EntityForeignKey> GetForeignKeys( AbstractEntity startNode = null ) {


            if ( startNode is not null ) throw new NotImplementedException( "GetForeignKeys with start node not implemented" );

            Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
            Stack<Tuple<AbstractEntity , AbstractEntity>> visitedArc = new Stack<Tuple<AbstractEntity , AbstractEntity>>();
            List<EntityForeignKey> result = new List<EntityForeignKey>();
            //List<EntityForeignKey> edge = new List<EntityForeignKey>(); not used

            foreach ( var (e, listFK) in _Graph ) {

                if ( !visited.Contains( e ) ) {

                    visited?.Push( e );
                }

                foreach ( EntityForeignKey efk in listFK ) {

                    foreach ( var fk in efk.foreignKeyInfo ) {

                        if ( ( fk.ReferenceTable != null ) && ( CacheContainsEntityType( fk ) ) ) {

                            //if ( visited.Contains( fk.ReferenceTable ) ) continue;
                            //visited?.Push( fk.ReferenceTable );
                            //result.Add( efk );

                            var arc = new Tuple<AbstractEntity , AbstractEntity>( fk.Table , fk.ReferenceTable );
                            if ( visitedArc.Contains( arc ) ) continue;
                            visitedArc?.Push( arc );
                            visited?.Push( fk.ReferenceTable );
                            result.Add( efk );

                        }

                    }
                }

                //foreach ( EntityForeignKey fk in listFK.Where( x => x.ReferenceTables() != null ).ToList() ) {

                //    if ( ( fk.ReferenceTables() != null ) && ( CacheContainsEntityType( fk ) ) ) {

                //        if ( visited.Contains( fk.ReferenceTables() ) ) continue;

                //        visited?.Push( fk.ReferenceTables() );

                //        result.Add( fk );

                //    }
                //}

            }

            return result;


        }

        //private bool CacheContainsEntityType( EntityForeignKey fk ) {
        private bool CacheContainsEntityType( ForeignKeyInfo fk ) {


            foreach ( var (t, list) in _Graph.mCache ) {
                foreach ( var a in list ) {
                    if ( ( ( dynamic ) a ).GetType() == fk.TypeReferenceTable ) {
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
        //EntityAdapter<ProvinceEntity>
        public T Adapter<T>( T adapter ) where T : TableEntity {

            if ( mEntitiesCache.ContainsKey( typeof( T ) ) ) {

                var x = mEntitiesCache[typeof( T )].FirstOrDefault();
                if ( ( AbstractEntity ) ( adapter as dynamic ).Entity is null ) {
                    throw new ArgumentNullException( "Entity in abstract is null. Use Create method." );
                }
                _Graph.ReplaceEntityGraph( ( AbstractEntity ) ( x as dynamic ).Entity , ( AbstractEntity ) ( adapter as dynamic ).Entity );

                mEntitiesCache[typeof( T )].Remove( x );
                mEntitiesCache.AddOrCreate( adapter );


                //AbstractEntity[] entities = mEntitiesCache.Values.SelectMany( l => l.Select( e => ( AbstractEntity)( e as dynamic ).Entity ) ).ToArray();
                //_Graph.BuildEntityGraph( entities );


            } else {

                mEntitiesCache.AddOrCreate( adapter );
                AbstractEntity[] entities = mEntitiesCache.Values.SelectMany( l => l.Select( e => ( AbstractEntity ) ( e as dynamic ).Entity ) ).ToArray();
                _Graph.BuildEntityGraph( entities );

            }

            return adapter;
        }


        public T Adapter<T>() where T : TableEntity {
            return mAdapter<T>( null );
        }

        public T Adapter<T>( string name ) where T : TableEntity {
            return mAdapter<T>( name );
        }

        private T mAdapter<T>( string name ) where T : TableEntity {


            if ( mEntitiesCache.ContainsKey( typeof( T ) ) ) {
                if ( name is null ) {
                    return ( T ) mEntitiesCache[typeof( T )].FirstOrDefault();
                } else {
                    return ( T ) mEntitiesCache[typeof( T )].Where( x => ( ( x as dynamic ).Entity as AbstractEntity ).FkNames.Contains( name ) ).FirstOrDefault();
                }

            } else {
                if ( ( typeof( T ).IsGenericType ) && ( typeof( T ).GetGenericTypeDefinition() == typeof( EntityAdapter<> ) ) ) {

                    Type t = typeof( T ).GetGenericArguments().FirstOrDefault();

                    if ( t != null ) {

                        var e = Entity( t );
                        if ( name is not null ) {
                            if ( !e.FkNames.Contains( name ) )
                                e.FkNames.Add( name );
                        }
                        //    e.FkNames = ( name is not null ) ? name : null;


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

        //    //Se è presente nel grafo il tipo entity in fkey.TypeReferenceTable 
        //    // allora se ReferenceTable è nullo copia il valore in mGraph
        //    // altrimenti instazia la tabella ed aggiungila ad mGraph
        //    if ( fkey.ReferenceTable == null ) {

        //        var x = _Graph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeReferenceTable );

        //        if ( x != null ) {

        //            fkey.ReferenceTable = x;
        //            // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //            // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //            return false;
        //        } else {

        //            fkey.InstantiateReferenceTable();
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

        //        if ( tableAdapters.ToList().FirstOrDefault( a => ( ( dynamic ) a ).Entity.GetType() == fkey.TypeReferenceTable ) != null ) {

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

        //                //}

        //                //var x = mGraph.Keys.FirstOrDefault( x => x.GetType() == fkey.TypeReferenceTable );
        //                //if ( x != null ) {
        //                //    fkey.ReferenceTable = x;
        //                //    // Visto che ho trovato un tipo che già esiste non c'è bisogno di 
        //                //    // analizzare le chiavi esterne perchè si suppone che già siano state aggiunte
        //                //} else {
        //                //    fkey.InstantiateReferenceTable();
        //                //    mGraph.Add( fkey.ReferenceTable );
        //                //    var allfk = fkey.ReferenceTable.GetAllForeignkey();
        //                //    foreach ( var ffk in allfk ) {
        //                //        if ( !mGraph.IsConnected( ffk ) ) {
        //                //            foreignKeys.Push( ffk );
        //                //            InstantiateReferenceTable( ffk );
        //                //            mGraph.Connect( ffk.ReferenceTable , ffk );
        //                //        }
        //                //    }
        //                //}


        //            }
        //        }
        //    }


        //    var _Entities = mGraph.Keys.ToList();
        //    foreach ( var e in _Entities.OrderBy( a => a.mforeignKeys?.Count() ).ToList() ) {
        //        foreach ( var _fk in mGraph[e] ) {

        //            if ( _fk.ReferenceTable != null ) {
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
        //    //            fk.InstantiateReferenceTable();
        //    //            mGraph.Add( fk.ReferenceTable );
        //    //            fk.ReferenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.ReferenceTable , x ); } );
        //    //        }
        //    //    }
        //    //    EntityForeignKey fkey = null;
        //    //    while ( foreignKeys.TryPop( out fkey ) ) {
        //    //        //fkey.InstantiateReferenceTable();
        //    //        //mGraph.Add( fk.ReferenceTable );
        //    //        //fk.ReferenceTable.mforeignKeys.ForEach( x => { foreignKeys.Push( x ); mGraph.Connect( fk.ReferenceTable , x ); } );
        //    //        //foreignKeyTables.Add( fk.ReferenceTable );
        //    //    }
        //    //}

        //}


        private A1 NewInstanceAdapter<A1>() {

            return ReflectionTypeHelper.NewInstanceAdapter<A1>( this.mDatabase , this.mLogger );

            //if ( mLogger != null ) {
            //    var ctor = typeof( A1 ).GetConstructor( new Type[] { this.mDatabase.GetType() , this.mLogger.GetType() } );
            //    return ( A1 ) ctor.Invoke( new object[] { this.mDatabase , this.mLogger } );

            //} else {
            //    var ctor = typeof( A1 ).GetConstructor( new Type[] { this.mDatabase.GetType() } );
            //    return ( A1 ) ctor.Invoke( new object[] { this.mDatabase } );

            //}


        }



        private bool haveAlmostIdentifyngRelationship() {
            var AlmostIsIdentifying = false;
            foreach ( var (_, list) in _Graph ) {
                foreach ( var fk in list ) {
                    AlmostIsIdentifying = AlmostIsIdentifying || fk.IsIdentifyingRelationship;
                }
            }

            return AlmostIsIdentifying;
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

        public void BindData( IReadableResultSet rs , List<AbstractEntity> copyEntity = null ) {

            List<dynamic> definitionColumns = new List<dynamic>();

            var tables = copyEntity ?? _Graph.GetTopologicalOrder();
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

                if ( index == null ) throw new System.Exception( "Invalid index column. (" + column.ActualName + ")" );


                var v = ReflectionTypeHelper.InvokeGenericFunction( column.GenericType ,
                                                           typeof( MySQResultSet ) ,
                                                           rs ,
                                                           nameof( MySQResultSet.getValueAs ) ,
                                                           new Type[] { typeof( int ) } ,
                                                           new object[] { ( int ) index } );

                if ( column.isPrimaryKey ) {
                    tables.FirstOrDefault( x => x.GetType().Equals( ( column as dynamic ).TypeTable ) )?.PrimaryKey.createKey( v );
                } else {

                    if ( v.IsDBNull() ) {
                        ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                    } else if ( v is null ) {
                        ( column as dynamic ).TypeWrapperValue = ReflectionTypeHelper.SQLTypeWrapperNULL( column.GenericType );
                    } else {
                        column.AssignValue( v );
                    }

                }
                
            }

        }

        private OperationResult FetchResultSet( MySQResultSet rs , bool AlmostIsIdentifying = false ) {

            if ( rs != null ) {

                if ( rs.fetch() ) {

                    BindData( rs );

                    foreach ( var (_, list) in mEntitiesCache ) {

                        foreach ( var tableEntity in list ) {

                            var generic = ReflectionTypeHelper.GetClassGenericType( tableEntity );
                            var type = typeof( BindDataFromEventArgs<> ).MakeGenericType( generic );
                            dynamic bindEventArgs = Activator.CreateInstance( type );
                            bindEventArgs.Entity = ( tableEntity as dynamic ).Entity;
                            ReflectionTypeHelper.InvokeMethod( tableEntity , "OnBindDataFrom" , new object[] { bindEventArgs } );

                        }
                    }


                    while ( rs.fetch() ) {

                        if ( AlmostIsIdentifying == false ) {

                            rs?.close();
                            return OperationResult.Error;
                            throw new System.Exception( "Resultset return more than one row." );

                        }

                        var newEntities = new List<AbstractEntity>();


                        foreach ( var (_, list) in mEntitiesCache ) {

                            foreach ( var tableEntity in list ) {
                                //tableEntity.New();
                                var copy = ( tableEntity as dynamic ).Entity.Copy();
                                copy.State = EntityState.Set;
                                newEntities.Add( copy );
                            }
                        }

                        BindData( rs , newEntities );

                        foreach ( var (_, list) in mEntitiesCache ) {

                            foreach ( var tableEntity in list ) {

                                var newEntity = newEntities.Where( x => x.GetType() == tableEntity.GetType().BaseType.GetGenericArguments().FirstOrDefault() ).FirstOrDefault();

                                if ( newEntity.State == EntityState.Changed ) {
                                    tableEntity.New( newEntity );
                                    //Il bind viene fatto all'interno della funzione New
                                    //var generic = ReflectionTypeHelper.GetClassGenericType( tableEntity );
                                    //var type = typeof( BindDataFromEventArgs<> ).MakeGenericType( generic );
                                    //dynamic bindEventArgs = Activator.CreateInstance( type );
                                    //bindEventArgs.Entity = ( tableEntity as dynamic ).Entity;
                                    //ReflectionTypeHelper.InvokeMethod( tableEntity , "OnBindDataFrom" , new object[] { bindEventArgs } );

                                }

                            }

                        }

                    }


                    //Entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                    rs?.close();
                    return OperationResult.OK;
                    //this.mEntities.Clear();
                    //throw new System.Exception( "Resultset return more than one row." );

                    //} else {

                    //    //Entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                    //    rs?.close();
                    //    return OperationResult.OK;


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

        private OperationResult Read<T>( T FromTable , AbstractEntity[] PrimaryKeyTables , bool AlmostIsIdentifying , EntityConditionalExpression Constraint , params object[] KeyValues ) where T : AbstractEntity {

            if ( FromTable is null ) throw new ArgumentException( "Invalid entity " + FromTable.GetType().Name );

            if ( ( PrimaryKeyTables is null ) || ( PrimaryKeyTables?.Length == 0 ) ) throw new ArgumentException( "Invalid primary key " + FromTable.GetType().Name );

            if ( ( KeyValues.Count() == 0 ) || ( PrimaryKeyTables.Sum( x => x.PrimaryKey.KeyLength ) != KeyValues.Count() ) ) throw new ArgumentException( "Invalid key value argument" );

            try {
                int i = 0;
                foreach ( var pk in PrimaryKeyTables ) {
                    int l = pk.PrimaryKey.KeyLength;
                    var xx = KeyValues.Skip( i ).Take( l ).ToArray();
                    pk.PrimaryKey.setKeyValues( xx );
                    i += l;
                }
                //PrimaryKeyTables.FirstOrDefault().PrimaryKey.setKeyValues( KeyValues );

                SQLQueryParams outparam = null;
                var s = SQLBuilder.sqlSelect<T>( this , ref outparam , ParamPlaceholder: "@" , Constraint , PrimaryKeyTables.ToArray() );

                var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }


        }

        public OperationResult Read<T, PK>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity
                                                                                                                        where PK : AbstractEntity {

            try {

                var tables = _Graph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK pkTable = ( PK ) tables.FirstOrDefault( e => e.GetType() == typeof( PK ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable } , AlmostIsIdentifying , Constraint , KeyValues );

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

        public OperationResult Read<T, PK1, PK2>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity
                                                                                                                             where PK1 : AbstractEntity
                                                                                                                             where PK2 : AbstractEntity {

            try {

                var tables = _Graph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK1 pkTable1 = ( PK1 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK1 ) );
                PK2 pkTable2 = ( PK2 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK2 ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable1 , pkTable2 } , AlmostIsIdentifying , Constraint , KeyValues );

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


        public OperationResult Read<T>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity {

            try {

                var tables = _Graph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { fromTable } , AlmostIsIdentifying , Constraint , KeyValues );

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

        public OperationResult Read( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            if ( KeyValues.Count() == 0 ) throw new ArgumentException( "Invalid key value argument" );

            try {

                var tables = _Graph.GetTopologicalOrder();

                tables.First().PrimaryKey.setKeyValues( KeyValues );
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();
                //var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                SQLQueryParams outparam = null;
                var s = SQLBuilder.sqlSelect( this , ref outparam , ParamPlaceholder: "@" , Constraint );

                var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

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


        public OperationResult Read<T, X>( Func<T , X> delegateMethod , EntityConditionalExpression Constraint = null ) where X : TableIndex where T : AbstractEntity {

            try {
                var tables = _Graph.GetTopologicalOrder();

                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                //AbstractEntity entity = _Graph.FirstOrDefault( kvp => kvp.Key.GetType() == typeof( T ) ).Key;

                //X x = delegateMethod( ( T ) entity );

                //DefinableConstraint[] constraints = new DefinableConstraint[x.ColumnsName.Length];

                //int i = 0;
                //foreach ( string c in x.ColumnsName ) {
                //    PropertyInfo p = x[c];
                //    var col = p.GetValue( entity );
                //    DefinableConstraint o = FactoryEntityConstraint.MakeConstraintEqual( col , "@" );
                //    constraints[i++] = o;
                //}
                //Constraint: new EntityConstraintExpression( constraints ) 

                SQLQueryParams outparam = null;
                var s = SQLBuilder.sqlSelect<T , X>( delegateMethod , this , ref outparam , Constraint: Constraint );
                var rs = mDatabase.ReadQuery( s , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

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
