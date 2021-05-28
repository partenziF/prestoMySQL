using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Adapter.Interface {
    public interface QueryAdapterListener<U> {
        public void onBindDataFrom( object sender , BindDataFromEventArgs<U> e );
    }
}
