using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MessageCenter.Portal.Models;
using MessageTransit;
using System.Diagnostics;
using MessageCenter.BLL;
using MessageCenter.Framework.Extension;
using MessageTransit.Message;
using MessageTransit.Monitor;

namespace MessageCenter.Portal.Controllers
{
    public class MessageController : AuthController
    {
        public MessageController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult Index()
        {
            MessageIndexModel model = new MessageIndexModel()
            {
                Topics = TopicSvc.LoadValidTopics().ToList()
            };
            return View(model);
        }

        public IActionResult LoadMessages(MessageQueryFilter filter, DataTableQueryRequest request)
        {
            filter.BuildQueryFilter(request);
            IEnumerable<SubscribeMessage> messages;
            int totalCount = 0;
            if (filter.OnlyFailedMessage.HasValue && filter.OnlyFailedMessage.Value)
            {
                messages = MessageSvc.LoadFailedMessages(filter, out totalCount);
            }
            else
            {
                messages = MessageSvc.LoadMessages(filter, out totalCount);
            }

            var data = new
            {
                Success = true,
                sEcho = 0,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = totalCount,
                aaData = messages
            };
            return Json(data);
        }

        public IActionResult LoadMessage(Guid messageId,string topic)
        {
            var message = MessageSvc.LoadMessage(messageId,topic);

            var data = new
            {
                Success = true,
                Data= message
            };
            return Json(data);
        }

        public IActionResult ManaualProcess(Guid messageId, string topic)
        {
            var message = MessageSvc.LoadMessage(messageId, topic);
            var msg = new TextMessage()
            {
                MessageText = message.MessageText
            };
            msg.putHeaders(BuiltinKeys.TraceId, messageId.ToString());
            msg.putHeaders(BuiltinKeys.Topic, topic);
            Stopwatch stopwatch = new Stopwatch();
            IMonitor monitor = new SqlMonitor();
            try
            {
                var processor = new RestApiProcessor();
                stopwatch.Restart();
                var result = (processor.Process(msg)).Result;
                stopwatch.Stop();
                MessageSuccessEventArgs args = new MessageSuccessEventArgs(msg, stopwatch.ElapsedMilliseconds);                
                monitor.onEvent(args);
                return Json(new { Success = true, Message = "处理成功！" });
            }
            catch(Exception ex)
            {
                stopwatch.Stop();
                MessageExceptionEventArgs args = new MessageExceptionEventArgs(msg, ex, stopwatch.ElapsedMilliseconds);
                monitor.onEvent(args);
            }
            return Json(new { Success = true, Message = "处理失败！" });
        }
    }
}