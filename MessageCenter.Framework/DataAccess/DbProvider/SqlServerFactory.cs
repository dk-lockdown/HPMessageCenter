using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace MessageCenter.DataAccess
{
    public class SqlServerFactory
    {
        public static DbConnection CreateConnection(string connectionString)
        {
            var conn= new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
