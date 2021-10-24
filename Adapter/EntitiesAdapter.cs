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

        private string mSQLQueryString = null;
        public string SQLQueryString { get => this.mSQLQueryString; }


        private TableGraph mGraph;

        //private Dictionary<AbstractEntity , List<EntityForeignKey>> mGraph = new Dictionary<AbstractEntity , List<EntityForeignKey>>();

        public readonly MySQLDatabase mDatabase;
        private readonly ILogger mLogger;
        private Dictionary<Type , List<TableEntity>> mTableEntityCache;
        protected List<string> mParamNames;


        public TableGraph TableGraph { get => this.mGraph; set => this.mGraph = value; }
        //internal Dictionary<Type , List<TableEntity>> TableEntityCache { get => this.mTableEntityCache; set => this.mTableEntityCache = value; }

        //private Dictionary<Type , Dictionary<string,TableEntity>> mEntitiesCache;


        public EntitiesAdapter( MySQLDatabase aMySQLDatabase , ILogger logger = null ) {
            this.mDatabase = aMySQLDatabase;
            this.mLogger = logger;
            TableGraph = new TableGraph();

            mParamNames = new List<string>();
            mTableEntityCache = new Dictionary<Type , List<TableEntity>>();

            //mEntitiesCache = new Dictionary<Type , Dictionary<string,TableEntity>>();
            //_Graph.mCache = new Dictionary<Type , List<TableEntity>>();
        }

        public void Add<A1, E1>( TableEntity adapter = null ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity {

            A1 a1 = null;

            if ( ( typeof( A1 ) == adapter?.GetType() ) && ( a1 is null ) ) {
                a1 = ( A1 ) adapter;
            }

            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            mTableEntityCache.AddOrCreate( a1 );

            TableGraph.AddEntityGraph( a1 );

        }



        //public ICollection<AbstractEntity> Keys => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Keys;
        //public ICollection<List<EntityForeignKey>> Values => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph ).Values;
        //public int Count => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).Count;
        //public bool IsReadOnly => ( ( ICollection<KeyValuePair<AbstractEntity , List<EntityForeignKey>>> ) this.mGraph ).IsReadOnly;

        public void Create<A1, E1, A2, E2>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                             where A2 : EntityAdapter<E2> where E2 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

            A1 a1 = null;
            A2 a2 = null;

            foreach ( var adapter in adapters ) {

                if ( ( typeof( A1 ) == adapter.GetType() ) && ( a1 is null ) ) {
                    a1 = ( A1 ) adapter;
                } else if ( ( typeof( A2 ) == adapter.GetType() ) && ( a2 is null ) ) {
                    a2 = ( A2 ) adapter;
                }

            }


            a1 ??= NewInstanceAdapter<A1>();
            if ( a1.Entity is null ) a1.Create();

            a2 ??= NewInstanceAdapter<A2>();
            if ( a2.Entity is null ) a2.Create();

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );

            TableGraph.BuildEntityGraph( a1 , a2 );

        }

        public void Create<A1, E1, A2, E2, A3, E3>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity {


            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                   where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                   where A5 : EntityAdapter<E5> where E5 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                   where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                   where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                   where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                   where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                                                                   where A6 : EntityAdapter<E6> where E6 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 );

        }


        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7>( params TableEntity[] adapters ) where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                                                                                      where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                                                                                      where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                                                                                      where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                                                                                      where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                                                                                      where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                                                                                      where A7 : EntityAdapter<E7> where E7 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );
            mTableEntityCache.AddOrCreate( a7 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 );

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

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );
            mTableEntityCache.AddOrCreate( a7 );
            mTableEntityCache.AddOrCreate( a8 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 );


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

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );
            mTableEntityCache.AddOrCreate( a7 );
            mTableEntityCache.AddOrCreate( a8 );
            mTableEntityCache.AddOrCreate( a9 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 , a9 );

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

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );
            mTableEntityCache.AddOrCreate( a7 );
            mTableEntityCache.AddOrCreate( a8 );
            mTableEntityCache.AddOrCreate( a9 );
            mTableEntityCache.AddOrCreate( a10 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 , a9 , a10 );

        }

        public void Create<A1, E1, A2, E2, A3, E3, A4, E4, A5, E5, A6, E6, A7, E7, A8, E8, A9, E9, A10, E10, A11, E11>( params TableEntity[] adapters )
                   where A1 : EntityAdapter<E1> where E1 : AbstractEntity
                    where A2 : EntityAdapter<E2> where E2 : AbstractEntity
                    where A3 : EntityAdapter<E3> where E3 : AbstractEntity
                    where A4 : EntityAdapter<E4> where E4 : AbstractEntity
                    where A5 : EntityAdapter<E5> where E5 : AbstractEntity
                    where A6 : EntityAdapter<E6> where E6 : AbstractEntity
                    where A7 : EntityAdapter<E7> where E7 : AbstractEntity
                    where A8 : EntityAdapter<E8> where E8 : AbstractEntity
                    where A9 : EntityAdapter<E9> where E9 : AbstractEntity
                    where A10 : EntityAdapter<E10> where E10 : AbstractEntity
                    where A11 : EntityAdapter<E11> where E11 : AbstractEntity {

            TableGraph.Cache.Clear();
            mTableEntityCache.Clear();

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
            A11 a11 = null;


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
                } else if ( ( typeof( A11 ) == adapter.GetType() ) && ( a11 is null ) ) {
                    a11 = ( A11 ) adapter;
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

            a11 ??= NewInstanceAdapter<A11>();
            if ( a11.Entity is null ) a11.Create();

            mTableEntityCache.AddOrCreate( a1 );
            mTableEntityCache.AddOrCreate( a2 );
            mTableEntityCache.AddOrCreate( a3 );
            mTableEntityCache.AddOrCreate( a4 );
            mTableEntityCache.AddOrCreate( a5 );
            mTableEntityCache.AddOrCreate( a6 );
            mTableEntityCache.AddOrCreate( a7 );
            mTableEntityCache.AddOrCreate( a8 );
            mTableEntityCache.AddOrCreate( a9 );
            mTableEntityCache.AddOrCreate( a10 );
            mTableEntityCache.AddOrCreate( a11 );

            TableGraph.BuildEntityGraph( a1 , a2 , a3 , a4 , a5 , a6 , a7 , a8 , a9 , a10 , a11 );

        }

        //public List<EntityForeignKey> this[AbstractEntity key] { get => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key]; set => ( ( IDictionary<AbstractEntity , List<EntityForeignKey>> ) this.mGraph )[key] = value; }



        public List<EntityForeignKey> GetForeignKeys( AbstractEntity startNode = null ) {

            return TableGraph.GetForeignKeys( startNode );

            //if ( startNode is not null ) throw new NotImplementedException( "GetForeignKeys with start node not implemented" );

            //Stack<AbstractEntity> visited = new Stack<AbstractEntity>();
            //Stack<Tuple<AbstractEntity , AbstractEntity>> visitedArc = new Stack<Tuple<AbstractEntity , AbstractEntity>>();
            //List<EntityForeignKey> result = new List<EntityForeignKey>();
            ////List<EntityForeignKey> edge = new List<EntityForeignKey>(); not used

            //foreach ( var (e, listFK) in Graph ) {

            //    if ( !visited.Contains( e ) ) {

            //        visited?.Push( e );
            //    }

            //    foreach ( EntityForeignKey efk in listFK ) {

            //        foreach ( var fk in efk.foreignKeyInfo ) {

            //            if ( ( fk.ReferenceTable != null ) && ( CacheContainsEntityType( fk ) ) ) {

            //                //if ( visited.Contains( fk.ReferenceTable ) ) continue;
            //                //visited?.Push( fk.ReferenceTable );
            //                //result.Add( efk );

            //                var arc = new Tuple<AbstractEntity , AbstractEntity>( fk.Table , fk.ReferenceTable );
            //                if ( visitedArc.Contains( arc ) ) continue;
            //                visitedArc?.Push( arc );
            //                visited?.Push( fk.ReferenceTable );
            //                result.Add( efk );

            //            }

            //        }
            //    }

            //    //foreach ( EntityForeignKey fk in listFK.Where( x => x.ReferenceTables() != null ).ToList() ) {

            //    //    if ( ( fk.ReferenceTables() != null ) && ( CacheContainsEntityType( fk ) ) ) {

            //    //        if ( visited.Contains( fk.ReferenceTables() ) ) continue;

            //    //        visited?.Push( fk.ReferenceTables() );

            //    //        result.Add( fk );

            //    //    }
            //    //}

            //}

            //return result;


        }

        //private bool CacheContainsEntityType( EntityForeignKey fk ) {
        //private bool CacheContainsEntityType( ForeignKeyInfo fk ) {


        //    foreach ( var (t, list) in Graph.Cache ) {
        //        foreach ( var a in list ) {
        //            if ( ( ( dynamic ) a ).GetType() == fk.TypeReferenceTable ) {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public List<AbstractEntity> GetTopologicalOrder() {
        //    List<AbstractEntity> result = new List<AbstractEntity>();
        //    _Graph.mCache.Values.ToList().ForEach( l => l.ForEach( a => result.Add( ( ( dynamic ) a ).Entity ) ) );
        //    //var x = mCache.Values.ToList().Select( l => l.Select( a => ( ( dynamic ) a ).Entity ).ToList() ).ToList();            
        //    return result;
        //}

        public T Entity<T>() where T : AbstractEntity {

            return ( T ) TableGraph.Keys.FirstOrDefault( x => x.GetType().IsAssignableFrom( typeof( T ) ) );

        }

        public AbstractEntity Entity( Type t ) {

            return TableGraph.Keys.FirstOrDefault( x => x.GetType().IsAssignableFrom( t ) );

        }
        //EntityAdapter<ProvinceEntity>
        public T Adapter<T>( T adapter ) where T : TableEntity {

            if ( mTableEntityCache.ContainsKey( typeof( T ) ) ) {

                var x = mTableEntityCache[typeof( T )].FirstOrDefault();
                if ( ( AbstractEntity ) ( adapter as dynamic ).Entity is null ) {
                    throw new ArgumentNullException( "Entity in abstract is null. Use Create method." );
                }
                TableGraph.ReplaceEntityGraph( ( AbstractEntity ) ( x as dynamic ).Entity , ( AbstractEntity ) ( adapter as dynamic ).Entity );

                mTableEntityCache[typeof( T )].Remove( x );
                mTableEntityCache.AddOrCreate( adapter );


                //AbstractEntity[] entities = mEntitiesCache.Values.SelectMany( l => l.Select( e => ( AbstractEntity)( e as dynamic ).Entity ) ).ToArray();
                //_Graph.BuildEntityGraph( entities );


            } else {

                mTableEntityCache.AddOrCreate( adapter );
                AbstractEntity[] entities = mTableEntityCache.Values.SelectMany( l => l.Select( e => ( AbstractEntity ) ( e as dynamic ).Entity ) ).ToArray();
                TableGraph.BuildEntityGraph( entities );

            }

            return adapter;
        }


        //public T SelectAdapter<T>(Func<List<TableEntity>,T> selector ) {
        //    var r = selector( mTableEntityCache[typeof( T )] );
        //    return r;

        //}

        // use in mapping
        public T Adapter<T>() where T : TableEntity {
            return mAdapter<T>( null , null );
        }

        public T Adapter<T>( Func<List<TableEntity> , T> selector = null ) where T : TableEntity {
            return mAdapter<T>( null , selector );
        }

        public T Adapter<T>( string name ) where T : TableEntity {
            return mAdapter<T>( name , null );
        }

        private T mAdapter<T>( string name , Func<List<TableEntity> , T> selector = null ) where T : TableEntity {


            if ( mTableEntityCache.ContainsKey( typeof( T ) ) ) {
                if ( selector is null ) {
                    if ( name is null ) {
                        return ( T ) mTableEntityCache[typeof( T )].FirstOrDefault();
                    } else {
                        return ( T ) mTableEntityCache[typeof( T )].Where( x => ( ( x as dynamic ).Entity as AbstractEntity ).FkNames.Contains( name ) ).FirstOrDefault();
                    }
                } else {
                    var r = selector( mTableEntityCache[typeof( T )] );
                    return r;


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

                        if ( e != null ) {
                            T Adapter;
                            InstantiateAdapter<T>( out Adapter , e );
                            mTableEntityCache.AddOrCreate( Adapter );
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
                                mTableEntityCache.AddOrCreate( Adapter );
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


        protected A1 NewInstanceAdapter<A1>() {

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
            foreach ( var (_, list) in TableGraph ) {
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
            Dictionary<string , Dictionary<Type , List<object>>> primaryKeysValues = new Dictionary<string , Dictionary<Type , List<object>>>();

            var tables = copyEntity ?? TableGraph.GetTopologicalOrder();
            foreach ( var e in tables ) {
                definitionColumns.AddRange( SQLTableEntityHelper.getDefinitionColumn( e , true ).ToList() );
                primaryKeysValues.Add( e.ActualName , new Dictionary<Type , List<object>>() );
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
                    if ( primaryKeysValues.ContainsKey( column.Table.ActualName ) ) {

                        if ( primaryKeysValues[column.Table.ActualName].ContainsKey( ( column as dynamic ).TypeTable ) ) {
                            ( primaryKeysValues[column.Table.ActualName][( column as dynamic ).TypeTable] as List<object> ).Add( v );
                        } else {
                            primaryKeysValues[column.Table.ActualName][( column as dynamic ).TypeTable] = new List<object>() { v };

                        }
                    } else {
                        throw new System.Exception( $"Can't find table name {column.Table.ActualName} in primarykeysvalues" );
                    }

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

            foreach ( var (tablename, tablenames) in primaryKeysValues ) {
                foreach ( var (typetable, values) in tablenames ) {
                    tables.FirstOrDefault( x => ( ( x.ActualName == tablename ) && ( x.GetType().Equals( typetable ) ) ) )?.PrimaryKey.setKeyValues( values.ToArray() );
                }
            }

        }

        private OperationResult FetchResultSet( MySQResultSet rs , bool AlmostIsIdentifying = false ) {

            if ( rs != null ) {

                if ( rs.fetch() ) {

                    BindData( rs );

                    foreach ( var (_, list) in mTableEntityCache ) {

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


                        foreach ( var (_, list) in mTableEntityCache ) {

                            foreach ( var tableEntity in list ) {
                                //tableEntity.New();
                                var copy = ( tableEntity as dynamic ).Entity.Copy();
                                copy.State = EntityState.Created;
                                newEntities.Add( copy );
                            }
                        }

                        BindData( rs , newEntities );

                        foreach ( var (_, list) in mTableEntityCache ) {

                            foreach ( var tableEntity in list ) {

                                var newEntity = newEntities.Where( x => x.GetType() == tableEntity.GetType().BaseType.GetGenericArguments().FirstOrDefault() ).FirstOrDefault();

                                if ( newEntity.State == EntityState.Changed ) {
                                    tableEntity.New( newEntity , false );
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
                    //if ( !( xx[i].GetType().IsArray ) )
                    //    pk.PrimaryKey.setKeyValues( xx );
                    //else
                    ////pk.PrimaryKey.setKeyValues( xx[0]. );
                    i += pk.PrimaryKey.KeyLength;
                }
                //PrimaryKeyTables.FirstOrDefault().PrimaryKey.setKeyValues( KeyValues );

                SQLQueryParams outparam = null;

                mParamNames.Clear();
                MakeUniqueParamName( Constraint );

                mSQLQueryString = SQLBuilder.sqlSelect<T>( this , ref outparam , ParamPlaceholder: "@" , Constraint , PrimaryKeyTables.ToArray() );

                var rs = mDatabase.ReadQuery( SQLQueryString , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }


        }

        public OperationResult Read<T, PK>( Func<IEnumerable<AbstractEntity> , PK> selector = null , EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity
                                                                                                                        where PK : AbstractEntity {

            try {

                var tables = TableGraph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK pkTable = null;
                if ( selector is null )
                    pkTable = ( PK ) tables.FirstOrDefault( e => e.GetType() == typeof( PK ) );
                else
                    pkTable = selector( tables );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable } , AlmostIsIdentifying , Constraint , KeyValues );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
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

                var tables = TableGraph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK1 pkTable1 = ( PK1 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK1 ) );
                PK2 pkTable2 = ( PK2 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK2 ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable1 , pkTable2 } , AlmostIsIdentifying , Constraint , KeyValues );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    }
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }


        public OperationResult Read<T, PK1, PK2, PK3>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity
                                                                                                                             where PK1 : AbstractEntity
                                                                                                                             where PK2 : AbstractEntity
                                                                                                                             where PK3 : AbstractEntity {

            try {

                var tables = TableGraph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK1 pkTable1 = ( PK1 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK1 ) );
                PK2 pkTable2 = ( PK2 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK2 ) );
                PK3 pkTable3 = ( PK3 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK3 ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable1 , pkTable2 , pkTable3 } , AlmostIsIdentifying , Constraint , KeyValues );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    }
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }

        public OperationResult Read<T, PK1, PK2, PK3, PK4>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity
                                                                                                                             where PK1 : AbstractEntity
                                                                                                                             where PK2 : AbstractEntity
                                                                                                                             where PK3 : AbstractEntity
                                                                                                                             where PK4 : AbstractEntity {

            try {

                var tables = TableGraph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );
                PK1 pkTable1 = ( PK1 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK1 ) );
                PK2 pkTable2 = ( PK2 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK2 ) );
                PK3 pkTable3 = ( PK3 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK3 ) );
                PK4 pkTable4 = ( PK4 ) tables.FirstOrDefault( e => e.GetType() == typeof( PK4 ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { pkTable1 , pkTable2 , pkTable3 , pkTable4 } , AlmostIsIdentifying , Constraint , KeyValues );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    }
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }


        public void Read<T>( Func<T , ConstructibleColumn> p , params object[] KeyValues ) where T : TableEntity {
            var x = this.Adapter<T>();
            var xx = p( x );

            var constratint = FactoryEntityConstraint.MakeEqual( xx , KeyValues , "@" );

            Console.WriteLine( p );
            //throw new NotImplementedException();
        }


        public OperationResult Read<T>( EntityConditionalExpression Constraint = null , params object[] KeyValues ) where T : AbstractEntity {

            try {

                var tables = TableGraph.GetTopologicalOrder();
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();

                T fromTable = ( T ) tables.FirstOrDefault( e => e.GetType() == typeof( T ) );

                var r = Read<T>( fromTable , new AbstractEntity[] { fromTable } , AlmostIsIdentifying , Constraint , KeyValues );

                if ( r == OperationResult.OK ) {

                    //foreach ( var e in tables ) {
                    //    e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                    //    e.PrimaryKey.KeyState = KeyState.SetKey;
                    //}

                    foreach ( var (_, adapters) in mTableEntityCache ) {
                        foreach ( var adapter in adapters ) {
                            foreach ( var entity in ( adapter as dynamic ) ) {
                                entity.State = prestoMySQL.Entity.Interface.EntityState.Set;
                                entity.PrimaryKey.KeyState = KeyState.SetKey;
                            }
                        }
                    }

                } else {

                    foreach ( var (_, adapters) in mTableEntityCache ) {
                        foreach ( var adapter in adapters ) {
                            foreach ( var entity in ( adapter as dynamic ) ) {
                                entity.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                                entity.PrimaryKey.KeyState = KeyState.UnsetKey;
                            }
                        }
                    }

                    //foreach ( var e in tables ) {
                    //    e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                    //    e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    //}

                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }



        public void MakeUniqueParamName( EntityConditionalExpression c ) {
            if ( ( c != null ) && ( c.countParam() > 0 ) ) {
                foreach ( QueryParam qp in c.getParam() ) {
                    var count = ( mParamNames.Count( c => c.StartsWith( qp.Name ) ) );
                    if ( count > 0 ) {
                        qp.rename( string.Format( "{0}_{1}" , qp.Name , count ) );
                    }
                    mParamNames.Add( qp.Name );
                }
            }

        }

        public OperationResult Read( EntityConditionalExpression Constraint = null , params object[] KeyValues ) {

            if ( KeyValues.Count() == 0 ) throw new ArgumentException( "Invalid key value argument" );

            try {



                var tables = TableGraph.GetTopologicalOrder();

                tables.First().PrimaryKey.setKeyValues( KeyValues );
                bool AlmostIsIdentifying = haveAlmostIdentifyngRelationship();
                //var r = this.Select( Constraint , Entity.PrimaryKey.getKeyValues() );

                SQLQueryParams outparam = null;

                mParamNames.Clear();
                MakeUniqueParamName( Constraint );


                mSQLQueryString = SQLBuilder.sqlSelect( this , ref outparam , ParamPlaceholder: "@" , Constraint );

                var rs = mDatabase.ReadQuery( mSQLQueryString , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
                    }
                }

                return r;

            } catch ( ArgumentOutOfRangeException e1 ) {
                throw new ArgumentOutOfRangeException( "Invalid key valus length for primary key" );
            } catch ( System.Exception e ) {
                throw new System.Exception( e.Message );
            }

        }

        //Used with UniqueIndex or Index
        public OperationResult Read<T, X>( Func<T , X> delegateMethod , EntityConditionalExpression Constraint = null ) where X : TableIndex where T : AbstractEntity {

            try {
                var tables = TableGraph.GetTopologicalOrder();

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

                mParamNames.Clear();
                MakeUniqueParamName( Constraint );

                mSQLQueryString = SQLBuilder.sqlSelect<T , X>( delegateMethod , this , ref outparam , Constraint: Constraint );
                var rs = mDatabase.ReadQuery( SQLQueryString , outparam.asArray().Select( x => ( MySqlParameter ) x ).ToArray() );

                var r = FetchResultSet( rs , AlmostIsIdentifying );

                if ( r == OperationResult.OK ) {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Set;
                        e.PrimaryKey.KeyState = KeyState.SetKey;
                    }
                } else {
                    foreach ( var e in tables ) {
                        e.State = prestoMySQL.Entity.Interface.EntityState.Undefined;
                        e.PrimaryKey.KeyState = KeyState.UnsetKey;
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
