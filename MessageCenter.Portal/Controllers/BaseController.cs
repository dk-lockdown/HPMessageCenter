using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace MessageCenter.Portal.Controllers
{
    public class BaseController : Controller
    {
        public AppSettings AppSettings;

        public BaseController(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }
    }

    [Authorize]
    public class AuthController : BaseController
    {

        public AuthController(IOptions<AppSettings> appSettings)
            :base(appSettings)
        {
        }
    }
}