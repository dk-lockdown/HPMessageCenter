using System;
using System.Collections.Generic;

namespace MessageTransit.Message
{
    public interface IMessage
    {
        Dictionary<string, string> Headers { get;}
        IMessage putHeaders(string key, string value);
    }



    public static class BuiltinKeys
    {
        public static string MessageId = "MessageId";

        public static string Exchange = "Exchange";

        public static string Topic = "Topic";

        public static string Queue = "Queue";

        public static string BornTimestamp = "BornTimestamp";

        public static string BornHost = "BornHost";

        public static string StoreTimestamp = "StoreTimestamp";

        public static string StoreHost = "StoreHost";

        public static string StartTime = "StartTime";

        public static string StopTime = "StopTime";

        public static string Timeout = "Timeout";

        public static string Priority = "Priority";

        public static string Reliability = "Reliability";

        public static string SearchKey = "SearchKey";

        public static string ScheduleExpression = "ScheduleExpression";

        public static string TraceId = "TraceId";

        public static string RetryNumbers = "RetryNumbers";

        public static string RetryReason = "RetryReason";

        public static string Stream = "Stream";
    }
}