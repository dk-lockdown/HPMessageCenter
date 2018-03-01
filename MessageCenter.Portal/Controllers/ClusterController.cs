using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MessageCenter.BLL;
using MessageCenter.Portal.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using MessageCenter.Framework.Log;

namespace MessageCenter.Portal.Controllers
{
    public class ClusterController : AuthController
    {
        public ClusterController(IOptions<AppSettings> appSettings)
            : base(appSettings)
        {

        }

        public IActionResult Index()
        {
            ClusterIndexModel cluster = new ClusterIndexModel()
            {
                Servers=ServerSvc.LoadServerNodes().ToList(),
                Topics=TopicSvc.LoadValidTopics().ToList()
            };
            return View(cluster);
        }

        public IActionResult AddServerNode(string server)
        {
            if(!server.StartsWith("http://")|| server.StartsWith("https://"))
            {
                return Json(new { Success = false, Message = "ServerHost格式不正确" });
            }
            ServerSvc.AddServerNode(server.TrimEnd('/'));
            return Json(new { Success = true, Message = "添加成功！" });
        }

        public IActionResult RemoveServerNode(string server)
        {
            ServerSvc.RemoveServerNode(server);
            return Json(new { Success = true, Message = "删除成功！" });
        }

        public async Task<IActionResult> LoadConsumers(string Host)
        {
            ApiRequestModel<string> apiRequestModel = new ApiRequestModel<string>()
            {
                RequestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            };
            apiRequestModel.Sign = Sign(apiRequestModel.RequestTime);

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Host);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(apiRequestModel));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(Host+"/Api/LoadConsumers", content);
                string result = await response.Content.ReadAsStringAsync();
                Logger.WriteLog(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Content(result);
                }
            }
            return Json(new { Success = false, Message = "请求失败" });
        }

        public async Task<IActionResult> AddConsumer(string Host, string Exchange,string Topic)
        {
            ApiRequestModel<string> apiRequestModel = new ApiRequestModel<string>()
            {
                RequestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                Data = $"{Exchange}&{Topic}"
            };
            apiRequestModel.Sign = Sign(apiRequestModel.RequestTime);

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Host);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(apiRequestModel));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(Host + "/Api/AddConsumer", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Content(await response.Content.ReadAsStringAsync());
                }
            }
            return Json(new { Success = false, Message = "请求失败" });
        }

        public async Task<IActionResult> RemoveConsumer(string Host,string Exchange,string Topic)
        {
            ApiRequestModel<string> apiRequestModel = new ApiRequestModel<string>()
            {
                RequestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                Data= $"{Exchange}&{Topic}"
            };
            apiRequestModel.Sign = Sign(apiRequestModel.RequestTime);

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Host);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(apiRequestModel));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(Host + "/Api/RemoveConsumer", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Content(await response.Content.ReadAsStringAsync());
                }
            }
            return Json(new { Success = false, Message = "请求失败" });
        }


        private string Sign(string signString)
        {
            var appSecret = AppSettings.ApiSecret;
            var hashsign = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(appSecret + signString));
            return ConvertBinaryToHexValueString(hashsign);
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