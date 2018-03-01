using Dapper;
using MessageCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MessageCenter.BLL
{
    public class AppDA
    {
        public static void CreateApp(App app)
        {
            var sql = DataCommandHelper.GetDataCommandSql("CreateApp", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, app);
            }
        }

        public static bool ExistsApp(string appId)
        {
            var sql = DataCommandHelper.GetDataCommandSql("ExistsApp", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.ExecuteScalar<bool>(sql, new { AppID=appId});
            }
        }

        public static IEnumerable<App> LoadApps()
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadApps", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<App>(sql);
            }
        }
    }
}
