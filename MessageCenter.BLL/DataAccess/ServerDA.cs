using Dapper;
using MessageCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MessageCenter.BLL
{
    public class ServerDA
    {
        public static void AddServerNode(string server)
        {
            var sql = DataCommandHelper.GetDataCommandSql("AddServerNode", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { ServerID=Guid.NewGuid(),ServerHost=server});
            }
        }

        public static bool ExistsServer(string server)
        {
            var sql = DataCommandHelper.GetDataCommandSql("ExistsServer", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.ExecuteScalar<bool>(sql, new { ServerHost = server });
            }
        }

        public static void RemoveServerNode(string server)
        {
            var sql = DataCommandHelper.GetDataCommandSql("RemoveServerNode", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { ServerHost = server });
            }
        }

        public static IEnumerable<string> LoadServerNodes()
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadServerNodes", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<string>(sql);
            }
        }
    }
}
