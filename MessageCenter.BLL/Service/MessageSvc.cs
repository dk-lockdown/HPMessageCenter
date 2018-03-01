using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.BLL
{
    public class MessageSvc
    {
        public static void CreateMessage(Message message)
        {
            message.HashFingerprint = ComputeHash(message.MessageText);
            MessageDA.CreateMessage(message);
        }

        public static void UpdateMessageStatusToPublishFailed(Guid messageId)
        {
            MessageDA.UpdateMessageStatusToPublishFailed(messageId);
        }

        public static bool ExistsMessage(string messageText)
        {
            string hashFingerprint = ComputeHash(messageText);
            return MessageDA.ExistsMessage(hashFingerprint);
        }

        public static void UpdateMessageStatusToPrepared(Guid messageId)
        {
            MessageDA.UpdateMessageStatusToPrepared(messageId);
        }

        public static void UpdateMessageStatusToSuccess(Guid messageId,string topic,long milliseconds)
        {
            MessageDA.UpdateMessageStatusToSuccess(messageId, topic, milliseconds);
        }

        public static void UpdateMessageStatusToFail(ProcessFailRecord record)
        {
            MessageDA.UpdateMessageStatusToFail(record.MessageId.Value,record.Topic);
            MessageDA.CreateProcessFailRecord(record);
        }

        public static IEnumerable<SubscribeMessage> LoadMessages(MessageQueryFilter filter,out int totalCount)
        {
            return MessageDA.LoadMessages(filter, out totalCount);
        }

        public static IEnumerable<Message> LoadPublishFailedMessage(int publishReloadTimeSpan)
        {
            return MessageDA.LoadPublishFailedMessage(publishReloadTimeSpan);
        }

        public static IEnumerable<SubscribeMessage> LoadFailedMessages(MessageQueryFilter filter,out int totalCount)
        {
            return MessageDA.LoadFailedMessages(filter, out totalCount);
        }

        public static SubscribeMessage LoadMessage(Guid messageId,string topic)
        {
            return MessageDA.LoadMessage(messageId,topic);
        }

        public static IEnumerable<ProcessFailRecord> LoadProcessFailRecords(Guid messageId)
        {
            return MessageDA.LoadProcessFailRecords(messageId);
        }

        private static string ComputeHash(string messageText)
        {
            HashAlgorithm algorithm = MD5.Create();
            //一天之内不能重复
            byte[] bytes = Encoding.Unicode.GetBytes(messageText+DateTime.Now.ToShortDateString());
            byte[] result = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(result);
        }
    }
}
