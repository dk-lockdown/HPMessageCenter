#if NETSTANDARD1_3 || NETSTANDARD2_0
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.DataAccess
{
    public class MysqlFactory
    {
        public static DbConnection CreateConnection(string connectionString)
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
#endif
