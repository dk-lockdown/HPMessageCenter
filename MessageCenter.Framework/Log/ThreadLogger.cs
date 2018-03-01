#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MessageCenter.Framework.Log
{
    /// <summary>
    /// 如果要通过此日志记录器来记录当前线程的运行过程,请在config文中的AppSettings节点配置Key="ThreadLoggerEnable"的节点,当值为"1"时表示需要记录日志,否则不记录.
    /// 操作步骤如下:1.开始记录日志时调用Start方法,2.添加日志调用Append方法,3.保存日志记录调用Save方法.
    /// 此日志记录主要是用于方法级别的性能调用
    /// </summary>
    public static class ThreadLogger
    {
        private static bool? _enable;
        private static bool Enable
        {
            get
            {
                if (!_enable.HasValue)
                {
                    _enable = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build().GetSection("AppSettings")["ThreadLoggerEnable"] == "1";
                }
                return _enable.Value;
            }
        }
        //[ThreadStatic]
        //private const string LogDataSoltName = "TimeLineLog";

        [ThreadStatic]
        private static List<ThreadLogInfo> ListContent = new List<ThreadLogInfo>();

        //[ThreadStatic]
        //private static ThreadLogInfo firstContent = null;

        /// <summary>
        /// 取得当前线程中的日志
        /// </summary>
        /// <returns></returns>
        public static List<ThreadLogInfo> GetLog()
        {
            if (ListContent == null) ListContent = new List<ThreadLogInfo>();

            return ListContent;

            //List<ThreadLogInfo> listContent = new List<ThreadLogInfo>(16);
            //LocalDataStoreSlot slot = Thread.GetNamedDataSlot(LogDataSoltName);

            //listContent = Thread.GetData(slot) as List<ThreadLogInfo>;
            //if (listContent == null)
            //{
            //    listContent = new List<ThreadLogInfo>();
            //    Thread.SetData(slot, listContent);
            //}
            //return listContent;
        }

        [ThreadStatic]
        private static bool isStart = false;

        [ThreadStatic]
        private static DateTime? startDate = DateTime.Now;

        /// <summary>
        /// 开始写日志,必须调用此方法,才能将日志写入.
        /// </summary>
        /// <param name="content">开始时写入的日志内容,当为null时不记录此条日志</param>
        /// <param name="preClear">开始前是否清除已有的日志</param>
        public static void Start(string content, bool preClear = true)
        {
            if (preClear)
            {
                Clear();
            }
            isStart = true;
            startDate = DateTime.Now;
            if (content == null)
            {
                return;
            }
            Append(content);
        }

        public static void Stop()
        {
            isStart = false;
        }

        /// <summary>
        /// 清除当前线程中的日志
        /// </summary>
        public static void Clear()
        {
            GetLog().Clear();
        }

        public static void StopAndClear()
        {
            isStart = false;
            Clear();
        }

        /// <summary>
        /// 将日志添加到当前线程,当前线程中，调用Append之前，请确认已经调用了Start方法，来启用日志写入
        /// </summary>
        /// <param name="message"></param>
        public static void Append(string content)
        {
            if (!Enable)
            {
                return;
            }
            if (!isStart)
            {
                return;
            }
            List<ThreadLogInfo> listContent = GetLog();
            listContent.Add(new ThreadLogInfo { Content = content });
            C(listContent);
        }

        private static void C(List<ThreadLogInfo> listContent)
        {
            if (listContent.Count > 1)
            {
                ThreadLogInfo f = listContent[0];
                ThreadLogInfo r = listContent[listContent.Count - 2];
                ThreadLogInfo c = listContent[listContent.Count - 1];
                c.PreviousToThisMilliseconds = (long)c.LogTime.Subtract(r.LogTime).TotalMilliseconds;
                c.FirstToThisMilliseconds = (long)c.LogTime.Subtract(f.LogTime).TotalMilliseconds;
            }
        }

        /// <summary>
        ///  保存当前线程中的日志，保存后日志的类别为“TimeLineLog[_(logCategoryTag:参数)]”
        /// </summary>
        /// <param name="logCategoryTag">不要超过20个字符</param>
        /// <param name="isStop">保存之后是否停止记录日志</param>
        /// <param name="afterClear">保存之后是否清空日志</param>
        public static void Save(string logCategoryTag = null, bool isStop = false, bool afterClear = false)
        {
            try
            {
                if (isStop)
                {
                    Stop();
                }
                List<ThreadLogInfo> listContent = new List<ThreadLogInfo>(GetLog().Count);
                listContent.AddRange(GetLog());
                if (listContent.Count == 0)
                {
                    return;
                }
                Thread th = new Thread(new ThreadStart(() =>
                {
                    string logContent = string.Join("\r\n", listContent);
                    string category = "TimeLineLog";
                    if (logCategoryTag != null)
                    {
                        category = category + "_" + logCategoryTag;
                    }
                    Logger.WriteLog(logContent, category);
                }));
                th.Start();
                if (afterClear)
                {
                    Clear();
                }
            }
            catch { }
        }

        /// <summary>
        /// 取得第一个加入到当前线程的日志，到现在的时间（毫秒）
        /// </summary>
        /// <returns></returns>
        public static long GetFistToNowMilliseconds()
        {
            if (!startDate.HasValue)
            {
                return 0L;
            }
            TimeSpan span = DateTime.Now.Subtract(startDate.Value);
            return (long)span.TotalMilliseconds;
        }

    }
    public class ThreadLogInfo
    {
        public DateTime LogTime { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 上一次到当前经历的时间
        /// </summary>
        public long PreviousToThisMilliseconds { get; set; }
        /// <summary>
        /// 第一次到当前经历的时间
        /// </summary>
        public long FirstToThisMilliseconds { get; set; }
        public ThreadLogInfo()
        {
            LogTime = DateTime.Now;
        }
        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss:fff}|{1}|{2}|{3}", LogTime, FirstToThisMilliseconds, PreviousToThisMilliseconds, Content);
        }
    }
}
#endif