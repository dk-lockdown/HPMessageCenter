using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MessageCenter.BLL;
using MessageCenter.Framework.Log;
using MessageTransit;
using MessageTransit.Message;

namespace MessageCenter.Portal.Controllers
{
    public class PublisherController : BaseController
    {
        public PublisherController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult Publish([FromBody]TextMessageModel message)
        {
            if (string.IsNullOrWhiteSpace(message.Topic))
                return Json(new { Code = 1, Desc = "Topic不能为空" });
            var topic = TopicSvc.LoadTopicByTopicName(message.Topic);
            if (topic == null||topic.Status==-1)
                return Json(new { Code = 1, Desc = "无效的topic" });
            if (string.IsNullOrWhiteSpace(message.MessageText))
                return Json(new { Code = 1, Desc = "MessageText不能为空" });
            //应是每天相同的消息不能超过多少条
            //if(MessageSvc.ExistsMessage(message.MessageText))
            //{
            //    return Json(new { Code = 1, Desc = "重复的MessageText" });
            //}
            try
            {
                Guid messageid = Guid.NewGuid();
                var msg = new TextMessage()
                {
                    MessageText = message.MessageText
                };
                msg.putHeaders(BuiltinKeys.Exchange, topic.ExchangeName);
                msg.putHeaders(BuiltinKeys.Topic, message.Topic);
                msg.putHeaders(BuiltinKeys.TraceId, messageid.ToString());
                msg.putHeaders(BuiltinKeys.SearchKey, message.ReferenceIndentifier);

                MessageSvc.CreateMessage(new Message()
                {
                    MessageId = messageid,
                    Exchange = topic.ExchangeName,
                    Topic = message.Topic,
                    MessageText = message.MessageText,
                    ReferenceIdentifier = message.ReferenceIndentifier
                });
                if (!MessageCenter.Startup.ProducerContainer.Send(msg))
                {
                    MessageSvc.UpdateMessageStatusToPublishFailed(messageid);
                    return Json(new { Code = -1, Desc = "发送失败" });
                }
                return Json(new { Code = 0, Desc = "发送成功" });
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString(), "MessagePublish_Exception");
            }
            return Json(new { Code = -1, Desc = "发送失败" });
        }
    }

    public class TextMessageModel
    {
        public string Topic { get; set; }
        public string MessageText { get; set; }
        public string ReferenceIndentifier { get; set; }
    }
}