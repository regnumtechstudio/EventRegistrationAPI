using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.helper
{
    public interface Iconnection
    {
        IDbConnection MasterDBConnection { get; }
        public SqlConnection MasterDBSqlConnection { get; }
        
        string MasterDBConnectionString { get; }
        void CloseConnection();
        //void CloseMySqlConnection();
        

    }
}
