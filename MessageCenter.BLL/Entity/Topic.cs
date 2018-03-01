using System;

namespace MessageCenter.BLL
{
    public class Exchange
    {
        public string AppID { get; set; }

        public int? SysNo { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public DateTime? CreateDate { get; set; }

        public string AppName { get; set; }
    }

    public class Topic
    {
        /// <summary>
        /// 为鉴权功能预留的字段
        /// </summary>
        public string AppID { get; set; }

        public int? ExchangeSysNo { get; set; }

        public int? SysNo { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        /// <summary>
        /// 1:有效，默认值
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 0:WebAPI
        /// </summary>
        public int ProcessorType { get; set; }

        public string ProcessorConfig { get; set; }

        /// <summary>
        /// 处理失败通知邮件列表
        /// </summary>
        public string ProcessFailNotifyEmails { get; set; }

        /// <summary>
        /// 消息处理负责人
        /// </summary>
        public string ProcessorMaintainer { get; set; }

        public int? CreateUserSysNo { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? UpdateUserSysNo { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string AppName { get; set; }

        public string ExchangeName { get; set; }

        public string StatusStr
        {
            get
            {
                if(Status.HasValue&&Status.Value==1)
                {
                    return "有效";
                }
                return "无效";
            }
        }

        public string CreateDateStr
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToShortDateString();
                }
                return "";
            }
        }
    }
}
