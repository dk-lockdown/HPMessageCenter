using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageCenter.Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MessageCenter.BLL;

namespace MessageCenter.Portal.Controllers
{
    public class HomeController : AuthController
    {
        public HomeController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult Index()
        {
            HomeIndexModel model = new HomeIndexModel()
            {
                Apps = AppSvc.LoadApps().ToList(),
                Exchanges = TopicSvc.LoadExchanges().ToList()
            };
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ErrorOccurred()
        {
            return View();
        }
    }
}
