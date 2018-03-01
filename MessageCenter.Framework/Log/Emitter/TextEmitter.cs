#if NETSTANDARD1_3 || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MessageCenter.Framework.Log
{
    internal class TextEmitter : ILogEmitter
    {
        private string m_LogFolderPath;
        public void Init(Dictionary<string, string> param)
        {
            string folderPath;
            if (param==null||param.TryGetValue("path", out folderPath) == false || folderPath == null || folderPath.Trim().Length <= 0)
            {
                m_LogFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
                return;
            }
            string p = Path.GetPathRoot(folderPath);
            if (p == null || p.Trim().Length <= 0) // 说明是相对路径
            {
                m_LogFolderPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
                return;
            }
            m_LogFolderPath = folderPath;
        }

        public void EmitLog(LogEntry log)
        {
            if (!Directory.Exists(m_LogFolderPath))
            {
                Directory.CreateDirectory(m_LogFolderPath);
            }

            WriteToFile(log.SerializationWithoutException(),
                Path.Combine(m_LogFolderPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"));
        }

        private static void WriteToFile(string log, string filePath)
        {
            DateTime now = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n** [" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] - Begin **************************************************************");
            sb.Append(log);
            sb.Append("\r\n** [" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] - End ****************************************************************\r\n");
            byte[] textByte = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            lock (filePath)
            {
                using (FileStream logStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    logStream.Write(textByte, 0, textByte.Length);
                    logStream.Dispose();
                }
            }
        }
    }
}
#endif