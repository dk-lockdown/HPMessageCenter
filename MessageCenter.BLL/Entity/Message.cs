using MessageCenter.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter.BLL
{
    public class Message
    {
        public Guid? MessageId { get; set; }

        public string Exchange { get; set; }

        public string Topic { get; set; }

        public string MessageText { get; set; }

        /// <summary>
        /// MessageText的hash值
        /// </summary>
        public string HashFingerprint { get; set; }

        public string ReferenceIdentifier { get; set; }

        public DateTime? CreateDate { get; set; }
    }

    public class SubscribeMessage
    {
        public string SubscribeTopic { get; set; }

        public Guid? MessageId { get; set; }

        public string SourceTopic { get; set; }

        public string MessageText { get; set; }

        public string ReferenceIdentifier { get; set; }

        /// <summary>
        /// 0：尚未处理，1：处理成功，-1：处理失败，-2：发送失败
        /// </summary>
        public int? Status { get; set; }

        public int RetryCount { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ProcessSuccessDate { get; set; }

        public long? TimePeriod { get; set; }

        public string ProcessorMaintainer { get; set; }

        public List<ProcessFailRecord> ProcessFailRecords { get; set; }

        public string StatusStr
        {
            get
            {
                if (Status.HasValue && Status.Value == 1)
                {
                    return "处理成功";
                }
                else if (Status.HasValue && Status.Value == 0)
                {
                    return "尚未处理";
                }
                else if (Status.HasValue && Status.Value == -1)
                {
                    return "处理失败";
                }
                else if (Status.HasValue && Status.Value == -2)
                {
                    return "发送失败";
                }
                return "";
            }
        }

        public string CreateDateStr
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString();
                }
                return "";
            }
        }

        public string ProcessSuccessDateStr
        {
            get
            {
                if (ProcessSuccessDate.HasValue)
                {
                    return ProcessSuccessDate.Value.ToString();
                }
                return "";
            }
        }
    }

    public class MessageQueryFilter: QueryFilter
    {
        public string Topic { get; set; }

        public string ReferenceIdentifier { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public bool? OnlyFailedMessage { get; set; }
    }

    public class ProcessFailRecord
    {
        public Guid? MessageId { get; set; }

        public string Topic { get; set; }

        public string FailRecord { get; set; }

        public long? TimePeriod { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateDateStr
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString();
                }
                return "";
            }
        }
    }
}
