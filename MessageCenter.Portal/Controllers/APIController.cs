using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MessageCenter.BLL;
using Microsoft.AspNetCore.Cors;
using MessageCenter.Portal.Models;

namespace MessageCenter.Portal.Controllers
{
    public class APIController : BaseController
    {
        public APIController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult HeartBeat()
        {
            return Json(new { Success=true,Data=DateTimeOffset.Now.ToUnixTimeMilliseconds()});
        }

        public IActionResult LoadConsumers([FromBody]ApiRequestModel<string> requestModel)
        {
            if (VerifyRequestModel(requestModel))
            {
                return Json(new { Success = true, Data = from con in MessageCenterManager.Consumers select new
                {
                    Exchange = con.Split('&')[0],
                    Topic = con.Split('&')[1]
                }
                });
            }
            return Json(new { Success = false, Message = "非法请求" });
        }

        public IActionResult AddConsumer([FromBody]ApiRequestModel<string> requestModel)
        {
            if (VerifyRequestModel(requestModel))
            {
                var exchange_topic = requestModel.Data;
                var et = exchange_topic.Split('&', StringSplitOptions.RemoveEmptyEntries);
                if (string.IsNullOrWhiteSpace(exchange_topic))
                {
                    return Json(new { Success = false, Message = "请选择一个Topic！" });
                }
                MessageCenterManager.Add(et[0],et[1]);
                return Json(new { Success = true, Message = "订阅成功！" });
            }
            return Json(new { Success = false, Message = "非法请求" });
        }

        public IActionResult RemoveConsumer([FromBody]ApiRequestModel<string> requestModel)
        {
            if (VerifyRequestModel(requestModel))
            {
                var exchange_topic = requestModel.Data;
                var et = exchange_topic.Split('&', StringSplitOptions.RemoveEmptyEntries);
                MessageCenterManager.Remove(et[0], et[1]);
                return Json(new { Success = true, Message = "取消订阅成功！" });
            }
            return Json(new { Success = false, Message = "非法请求" });
        }

        private bool VerifyRequestModel<T>(ApiRequestModel<T> requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.RequestTime))
                return false;
            return VerifySignature(requestModel.RequestTime, requestModel.Sign);
        }

        private bool VerifySignature(string signString, string sign)
        {
            //var reg = new Regex("[^\"\\:,\\{\\}\\[\\]]");
            //var signStringBuilder = new StringBuilder();
            //foreach (Match mch in reg.Matches(jsonString))
            //{
            //    signStringBuilder.Append(mch.Value);
            //}
            //var signString = signStringBuilder.ToString();

            var appSecret = AppSettings.ApiSecret;
            var hashsign = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(appSecret + signString));
            return ConvertBinaryToHexValueString(hashsign) == sign;
        }

        /// <summary>
        /// Converts the binary to hex value string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        private string ConvertBinaryToHexValueString(IEnumerable<byte> bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte _byte in bytes)
            {
                string hex = _byte.ToString("x");
                if (hex.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(hex);
            }

            return sb.ToString().ToLower();
        }
    }
}