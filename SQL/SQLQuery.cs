﻿using prestoMySQL.Column;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.SQL {
    public class SQLQuery : IDictionary<String , Object> {

        Dictionary<String , SQLProjectionColumn<SQLTypeWrapper<Object>>> getter = null;

        ///// Implements Map<String,Object> /////
        ///// /////////////////////////////////////////////////////

        public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<object> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add( string key , object value ) {
            throw new NotImplementedException();
        }

        public void Add( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        public bool ContainsKey( string key ) {
            throw new NotImplementedException();
        }

        public void CopyTo( KeyValuePair<string , object>[] array , int arrayIndex ) {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string , object>> GetEnumerator() {
            throw new NotImplementedException();
        }

        public bool Remove( string key ) {
            throw new NotImplementedException();
        }

        public bool Remove( KeyValuePair<string , object> item ) {
            throw new NotImplementedException();
        }

        public bool TryGetValue( string key , [MaybeNullWhen( false )] out object value ) {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
