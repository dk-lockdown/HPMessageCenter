using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MessageCenter.Framework.Encryption;

namespace MessageCenter.Portal.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult LoginValidationCode()
        {
            string code = ValidationCodeHelper.CreateValidateCode(5);
            byte[] bytes = ValidationCodeHelper.CreateValidateGraphic(code, 34);
            var options = new CookieOptions()
            {
                Domain = AppSettings.EnvironmentVariable == "pre" ? "MessageCenter.Pre" : "MessageCenter",
                HttpOnly = true
            };
            //验证码应该做加密处理，不然形同虚设
            HttpContext.Response.Cookies.Append("LoginVerifyCode", HashEncrypt.DESEncrypt(code.Trim(),"hpmcgctr"));
            return File(bytes, @"image/jpeg");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            //验证码应该做加密处理，不然形同虚设
            HttpContext.Request.Cookies.TryGetValue("LoginVerifyCode", out var verifyCode);
            verifyCode = HashEncrypt.DESDecrypt(verifyCode, "hpmcgctr");
            if (!model.VerifyCode.Equals(verifyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                return Json(new
                {
                    Success = false,
                    Message = "验证码错误！"
                });
            }
            if (model.UserName != AppSettings.DefaultUserName || model.Password != AppSettings.DefaultPassword)
            {
                return Json(new
                {
                    Success = false,
                    Message = "用户名或密码错误！"
                });
            }

            // create claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName),
            };

            // create identity
            ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationConfig.AuthenticationKey);

            // create principal
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            // sign-in
            await HttpContext.SignInAsync(
                    scheme: AuthenticationConfig.AuthenticationKey,
                    principal: principal,
                    properties: new AuthenticationProperties
                    {
                        IsPersistent = true, // for 'remember me' feature
                        ExpiresUtc = DateTime.UtcNow.AddHours(12),
                        AllowRefresh = false
                    });

            returnUrl = returnUrl ?? ViewData["ReturnUrl"] as string;
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Json(new
                {
                    Success = true,
                    Message = "登录成功！",
                    ReturnUrl = returnUrl
                });
            }
            return Json(new
            {
                Success = true,
                Message = "登录成功！",
                ReturnUrl = "/home/index"
            });
        }

        //public IActionResult Logout()
        //{
        //    await HttpContext.SignOutAsync(
        //            scheme: "MessageCenterAuth");

        //    return RedirectToAction("Login");
        //}
    }

    public class LoginModel
    {
        //不需要remember me复选框，因为我们的用户登录凭证都是放在cookie里面的，cookie都是设置了过期时间的，即默认是remember me的。

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VerifyCode { get; set; }
    }
}