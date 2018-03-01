using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MessageCenter.Portal.Models;
using MessageCenter.BLL;

namespace MessageCenter.Portal.Controllers
{
    public class TopicController : AuthController
    {
        public TopicController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult List()
        {
            TopicListModel model = new TopicListModel()
            {
                Apps = AppSvc.LoadApps().ToList(),
                Exchanges = TopicSvc.LoadExchanges().ToList()
            };
            return View(model);
        }

        public IActionResult LoadTopics()
        {
            var topics = TopicSvc.LoadTopics();
            var data = new
            {
                Success = true,
                sEcho = 0,
                iTotalRecords = topics.Count(),
                iTotalDisplayRecords = topics.Count(),
                aaData = topics
            };
            return Json(data);
        }

        public IActionResult SaveTopic(Topic topic)
        {
            if (topic.SysNo.HasValue && topic.SysNo.Value > 0)
            {
                if (string.IsNullOrWhiteSpace(topic.Name))
                {
                    return Json(new { Success = false, Message = "TopicName不能为空" });
                }
                if (string.IsNullOrWhiteSpace(topic.ProcessorConfig))
                {
                    return Json(new { Success = false, Message = "ProcessorConfig不能为空" });
                }
                TopicSvc.EditTopic(topic);
                return Json(new { Success = true, Message = "保存成功！" });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(topic.AppID) || topic.AppID == "-1")
                {
                    return Json(new { Success = false, Message = "请选择一个App" });
                }
                if (!topic.ExchangeSysNo.HasValue || topic.ExchangeSysNo.Value <= 0)
                {
                    return Json(new { Success = false, Message = "请选择一个Exchange" });
                }
                if (string.IsNullOrWhiteSpace(topic.Name))
                {
                    return Json(new { Success = false, Message = "TopicName不能为空" });
                }
                if (string.IsNullOrWhiteSpace(topic.ProcessorConfig))
                {
                    return Json(new { Success = false, Message = "ProcessorConfig不能为空" });
                }
                topic.ProcessorType = 1;
                TopicSvc.CreateTopic(topic);
                return Json(new { Success = true, Message = "创建成功！" });
            }
        }

        public IActionResult ToggleTopicStatus(int sysno)
        {
            var topic = TopicSvc.LoadTopicBySysNo(sysno);
            if(topic.Status.HasValue&&topic.Status.Value==1)
            {
                TopicSvc.UpdateTopicStatusToInValid(sysno);
            }
            else
            {
                TopicSvc.UpdateTopicStatusToValid(sysno);
            }
            return Json(new { Success = true, Message = "更新成功！" });
        }
    }
}