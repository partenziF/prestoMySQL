using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestoMySQL.Database.Interface {

    public interface ILastErrorInfo {

        public string Message { get; init; }

    }

    public interface IDatabase {

        ILastErrorInfo LastError { get; set; }

        ILogger Logger { get; set; }

        public bool OpenConnection();

        public bool Begin();
        public bool Commit();
        public bool Rollback();



    }
}
