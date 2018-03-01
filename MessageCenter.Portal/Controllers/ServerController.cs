using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MessageCenter.BLL;

namespace MessageCenter.Portal.Controllers
{
    public class ServerController : AuthController
    {
        public ServerController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult CreateApp(App app)
        {
            if(string.IsNullOrWhiteSpace(app.AppID))
            {
                return Json(new { Success = false, Message = "AppID不能位空" });
            }
            if (string.IsNullOrWhiteSpace(app.AppName))
            {
                return Json(new { Success = false, Message = "AppName不能位空" });
            }
            AppSvc.CreateApp(app);
            return Json(new { Success=true,Message="创建成功！" });
        }

        public IActionResult CreateExchange(Exchange exchange)
        {
            if (string.IsNullOrWhiteSpace(exchange.AppID)|| exchange.AppID=="-1")
            {
                return Json(new { Success = false, Message = "请选择一个App" });
            }
            if (string.IsNullOrWhiteSpace(exchange.Name))
            {
                return Json(new { Success = false, Message = "ExchangeName不能位空" });
            }
            TopicSvc.CreateExchange(exchange);
            return Json(new { Success = true, Message = "创建成功！" });
        }
    }
}