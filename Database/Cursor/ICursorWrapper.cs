using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestoMySQL.Database.Cursor {
    public interface ICursorWrapper {

        bool isEmpty();

        void Close();

    }
}
