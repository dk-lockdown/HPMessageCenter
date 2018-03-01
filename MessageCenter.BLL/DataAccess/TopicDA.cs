using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MessageCenter.DataAccess;

namespace MessageCenter.BLL
{
    public class TopicDA
    {
        public static void CreateExchange(Exchange exchange)
        {
            var sql = DataCommandHelper.GetDataCommandSql("CreateExchange", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, exchange);
            }
        }

        public static bool ExistsExchange(string exchangeName)
        {
            var sql = DataCommandHelper.GetDataCommandSql("ExistsExchange", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.ExecuteScalar<bool>(sql, new { Name = exchangeName });
            }
        }

        public static IEnumerable<Exchange> LoadExchanges()
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadExchanges", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<Exchange>(sql);
            }
        }

        public static void CreateTopic(Topic topic)
        {
            var sql = DataCommandHelper.GetDataCommandSql("CreateTopic", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, topic);
            }
        }

        public static bool ExistsTopic(string topic)
        {
            var sql = DataCommandHelper.GetDataCommandSql("ExistsTopic", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.ExecuteScalar<bool>(sql, new { Name = topic });
            }
        }

        public static void EditTopic(Topic topic)
        {
            var sql = DataCommandHelper.GetDataCommandSql("EditTopic", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, topic);
            }
        }

        public static Topic LoadTopicBySysNo(int sysno)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadTopicBySysNo", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.QueryFirstOrDefault<Topic>(sql,new { SysNo = sysno });
            }
        }

        public static Topic LoadTopicByTopicName(string name)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadTopicByTopicName", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.QueryFirstOrDefault<Topic>(sql, new { Name = name });
            }
        }

        public static IEnumerable<Topic> LoadTopics()
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadTopics", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<Topic>(sql);
            }
        }

        public static IEnumerable<Topic> LoadValidTopics()
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadValidTopics", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<Topic>(sql);
            }
        }

        public static void UpdateTopicStatusToValid(int sysno)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateTopicStatusToValid", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql,new { SysNo = sysno});
            }
        }

        public static void UpdateTopicStatusToInValid(int sysno)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateTopicStatusToInValid", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { SysNo = sysno });
            }
        }
    }
}
