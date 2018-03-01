using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MessageCenter.DataAccess;

namespace MessageCenter.BLL
{
    public class MessageDA
    {
        public static void CreateMessage(Message message)
        {
            var sql = DataCommandHelper.GetDataCommandSql("CreateMessage", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, message);
            }
        }

        public static void UpdateMessageStatusToPublishFailed(Guid messageId)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateMessageStatusToPublishFailed", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { MessageId = messageId });
            }
        }

        public static bool ExistsMessage(string hashFingerprint)
        {
            var sql = DataCommandHelper.GetDataCommandSql("ExistsMessage", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.ExecuteScalar<bool>(sql, new { HashFingerprint = hashFingerprint });
            }
        }

        public static void UpdateMessageStatusToPrepared(Guid messageId)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateMessageStatusToPrepared", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { MessageId = messageId });
            }
        }

        public static void UpdateMessageStatusToSuccess(Guid messageId,string topic,long milliseconds)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateMessageStatusToSuccess", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { MessageId = messageId,Topic=topic, TimePeriod=milliseconds });
            }
        }

        public static void UpdateMessageStatusToFail(Guid messageId, string topic)
        {
            var sql = DataCommandHelper.GetDataCommandSql("UpdateMessageStatusToFail", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, new { MessageId = messageId, Topic = topic });
            }
        }

        public static void CreateProcessFailRecord(ProcessFailRecord record)
        {
            var sql = DataCommandHelper.GetDataCommandSql("CreateProcessFailRecord", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                conn.Execute(sql, record);
            }
        }

        public static IEnumerable<SubscribeMessage> LoadMessages(MessageQueryFilter filter,out int totalCount)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadMessages", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                string whereStr = "WHERE 1 = 1";
                if(!string.IsNullOrWhiteSpace(filter.Topic))
                {
                    whereStr += " AND `subscribemessage`.`Topic`=@Topic";
                }
                if (!string.IsNullOrWhiteSpace(filter.ReferenceIdentifier))
                {
                    whereStr += " AND `message`.`ReferenceIdentifier`=@ReferenceIdentifier";
                }
                if (filter.CreateDateFrom.HasValue)
                {
                    whereStr += " AND `message`.`CreateDate`>=@CreateDateFrom";
                }
                if (filter.CreateDateTo.HasValue)
                {
                    whereStr += " AND `message`.`CreateDate`<@CreateDateTo";
                }
                whereStr += " Order By `message`.`CreateDate` DESC";
                sql = sql.Replace("#StrWhere#", whereStr);
                var result = conn.QueryMultiple(sql,filter);
                var messages = result.Read<SubscribeMessage>();
                totalCount = result.ReadFirstOrDefault<int>();
                return messages;
            }
        }

        /// <summary>
        /// 获取单位时间内发布失败的消息，单位小时
        /// </summary>
        /// <param name="publishReloadTimeSpan"></param>
        /// <returns></returns>
        public static IEnumerable<Message> LoadPublishFailedMessage(int publishReloadTimeSpan)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadPublishFailedMessage", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<Message>(sql, new { PublishReloadTimeSpan = publishReloadTimeSpan });
            }
        }

        public static IEnumerable<SubscribeMessage> LoadFailedMessages(MessageQueryFilter filter,out int totalCount)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadMessages", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                string whereStr = "WHERE `subscribemessage`.`Status` <> 1";
                if (!string.IsNullOrWhiteSpace(filter.Topic))
                {
                    whereStr += " AND `subscribemessage`.`Topic`=@Topic";
                }
                if (!string.IsNullOrWhiteSpace(filter.ReferenceIdentifier))
                {
                    whereStr += " AND `message`.`ReferenceIdentifier`=@ReferenceIdentifier";
                }
                if (filter.CreateDateFrom.HasValue)
                {
                    whereStr += " AND `message`.`CreateDate`>=@CreateDateFrom";
                }
                if (filter.CreateDateTo.HasValue)
                {
                    whereStr += " AND `message`.`CreateDate`<@CreateDateTo";
                }
                whereStr += " Order By `message`.`CreateDate` DESC,`subscribemessage`.`ProcessSuccessDate` DESC";
                sql = sql.Replace("#StrWhere#", whereStr);
                var result = conn.QueryMultiple(sql, filter);
                var messages = result.Read<SubscribeMessage>();
                totalCount = result.ReadFirstOrDefault<int>();
                return messages;
            }
        }

        public static SubscribeMessage LoadMessage(Guid messageId,string topic)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadMessage", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                var result = conn.QueryMultiple(sql,new { MessageId = messageId,Topic = topic });
                var message = result.ReadFirstOrDefault<SubscribeMessage>();
                message.ProcessFailRecords = result.Read<ProcessFailRecord>().AsList();
                return message;
            }
        }

        public static IEnumerable<ProcessFailRecord> LoadProcessFailRecords(Guid messageId)
        {
            var sql = DataCommandHelper.GetDataCommandSql("LoadProcessFailRecords", out string connectionString);
            using (IDbConnection conn = MysqlFactory.CreateConnection(connectionString))
            {
                return conn.Query<ProcessFailRecord>(sql, new { MessageId= messageId });
            }
        }
    }
}
