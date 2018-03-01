using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageCenter.Portal.Models;
using Microsoft.Extensions.Options;
using MessageCenter.BLL;
using Microsoft.AspNetCore.Cors;

namespace MessageCenter.Portal.Controllers
{
    public class SubscriberController : AuthController
    {
        public SubscriberController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult Index()
        {
            ConsumerContainerIndexModel model = new ConsumerContainerIndexModel()
            {
                Consumers = MessageCenterManager.Consumers,
                Topics = TopicSvc.LoadValidTopics().ToList()
            };
            return View(model);
        }

        public IActionResult Subscribe(string Exchange,string Topic)
        {
            if(string.IsNullOrWhiteSpace(Topic))
            {
                return Json(new { Success = false, Message = "请选择一个Topic！" });
            }
            MessageCenterManager.Add(Exchange,Topic);
            return Json(new { Success = true, Message = "订阅成功！" });
        }

        public IActionResult Unsubscribe(string Exchange, string Topic)
        {
            MessageCenterManager.Remove(Exchange,Topic);
            return Json(new { Success = true, Message = "取消订阅成功！" });
        }
    }
}