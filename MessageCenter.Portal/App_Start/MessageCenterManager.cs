using MessageCenter.BLL;
using MessageCenter.Framework.Log;
using MessageTransit.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessageCenter.Portal
{
    public class MessageCenterManager
    {
        private const string consumerFilePath = "Configuration/Consumer.json";
        private static List<string> consumers;
        private static object sync_obj = new object();
        private static bool inited = false;
        public static List<string> Consumers
        {
            get
            {
                if (!inited)
                {
                    throw new Exception("SubscriberManager must init!");
                }
                return consumers;
            }
        }

        public static void Init()
        {
            if (!inited)
            {
                lock (sync_obj)
                {
                    Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                PublishRetry();
                            });
                            Thread.Sleep(1000 * 60 * (AppConfiguration.AppSettings.PublishRetryTimeSpan??5));
                        }
                    });


                    var cons = readConsumerFile();
                    cons.ForEach(con =>
                    {
                        var et = con.Split('&', StringSplitOptions.RemoveEmptyEntries);
                        MessageCenter.Startup.ConsumerContainer.Add(et[0],et[1]);
                    });
                    consumers = cons;
                    inited = true;
                }
            }
        }


        private static void PublishRetry()
        {
            var messages = MessageSvc.LoadPublishFailedMessage(AppConfiguration.AppSettings.PublishReloadTimeSpan ?? 2);
            foreach (var message in messages)
            {
                try
                {
                    var msg = new MessageTransit.TextMessage()
                    {
                        MessageText = message.MessageText
                    };
                    msg.putHeaders(BuiltinKeys.Exchange, message.Exchange);
                    msg.putHeaders(BuiltinKeys.Topic, message.Topic);
                    msg.putHeaders(BuiltinKeys.TraceId, message.MessageId.ToString());
                    msg.putHeaders(BuiltinKeys.SearchKey, message.HashFingerprint);

                    MessageSvc.UpdateMessageStatusToPrepared(message.MessageId.Value);
                    if (!MessageCenter.Startup.ProducerContainer.Send(msg))
                    {
                        MessageSvc.UpdateMessageStatusToPublishFailed(message.MessageId.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(ex.ToString(), "PublishRetry_Exception");
                }
            }
        }

        public static void Add(string exchange, string topic)
        {
            if (!inited)
            {
                throw new Exception("SubscriberManager must init!");
            }
            lock (sync_obj)
            {
                string et = $"{exchange}&{topic}";
                if (!consumers.Exists(con => con == et))
                {
                    MessageCenter.Startup.ConsumerContainer.Add(exchange,topic);
                    consumers.Add(et);
                    writeConsumerFile();
                }
            }
        }

        public static void Remove(string exchange, string topic)
        {
            if (!inited)
            {
                throw new Exception("SubscriberManager must init!");
            }
            lock (sync_obj)
            {
                string et = $"{exchange}&{topic}";
                MessageCenter.Startup.ConsumerContainer.Remove(exchange,topic);
                consumers.Remove(et);
                writeConsumerFile();
            }
        }

        private static void writeConsumerFile()
        {
            string consumerFile = Path.Combine(Directory.GetCurrentDirectory(), consumerFilePath);
            if (File.Exists(consumerFilePath))
            {
                File.Delete(consumerFilePath);
            }
            if (consumers != null && consumers.Count > 0)
            {
                File.WriteAllText(consumerFile, JsonConvert.SerializeObject(consumers));
            }
        }

        private static List<string> readConsumerFile()
        {
            string consumerFile = Path.Combine(Directory.GetCurrentDirectory(), consumerFilePath);
            if (File.Exists(consumerFile))
            {
                string c = File.ReadAllText(consumerFile);
                return JsonConvert.DeserializeObject<List<string>>(c);
            }
            return new List<string>();
        }

        private static bool IsConsumerFileExists()
        {
            string consumerFile = Path.Combine(Directory.GetCurrentDirectory(), consumerFilePath);
            return File.Exists(consumerFile);
        }
    }
}
