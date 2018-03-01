using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal
{
    public class AppSettings
    {
        public string EnvironmentVariable { get; set; }
        public string DefaultUserName { get; set; }
        public string DefaultPassword { get; set; }
        public string ApiSecret { get; set; }
        public int? ProcessFailRetryCount { get; set; }
        /// <summary>
        /// 单位时间内发布失败的消息会重发，单位小时
        /// </summary>
        public int? PublishReloadTimeSpan { get; set; }
        /// <summary>
        /// 发布失败消息，重发间隔，单位分钟
        /// </summary>
        public int? PublishRetryTimeSpan { get; set; }
    }

    public class AppConfiguration
    {
        private static AppSettings appSettings;
        public static AppSettings AppSettings
        {
            get
            {
                if (appSettings != null)
                {
                    return appSettings;
                }
                else
                {
                    appSettings = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true).Build()
                        .GetSection("AppSettings")
                        .Get<AppSettings>();

                    return appSettings;
                }
            }
        }
    }
}
