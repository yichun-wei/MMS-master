using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Seec.Marketing
{
    /// <summary>
    /// WebUtil 的摘要描述
    /// </summary>
    public class WebUtil
    {
        #region WriteSysFileLog
        static object _lockSysFileLog = new object();

        #region 系統記錄檔
        /// <summary>
        /// 系統記錄檔。
        /// </summary>
        public class SysFileLog
        {
            /// <summary>
            /// 初始化 SysFileLog 類別的新執行個體。
            /// </summary>
            /// <param name="cat">種類。</param>
            /// <param name="src">來源。</param>
            /// <param name="eventType">類型。</param>
            /// <param name="title">標題。</param>
            /// <param name="msg">訊息。</param>
            /// <param name="clientIP">使用者 IP。</param>
            public SysFileLog(string cat, string src, EventType eventType, string title, string msg)
            {
                this.Cat = cat;
                this.Src = src;
                this.EventType = eventType;
                this.Title = title;
                this.Msg = msg;
            }

            /// <summary>
            /// 種類。
            /// </summary>
            public string Cat { get; set; }
            /// <summary>
            /// 來源。
            /// </summary>
            public string Src { get; set; }
            /// <summary>
            /// 類型。
            /// </summary>
            public EventType EventType { get; set; }
            /// <summary>
            /// 標題。
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 訊息。
            /// </summary>
            public string Msg { get; set; }
        }
        #endregion

        #region WriteSysFileLog (陣列寫入)
        /// <summary>
        /// 系統記錄檔（寫入檔案）。
        /// </summary>
        /// <param name="token">Token。</param>
        /// <param name="clientIP">使用者 IP。</param>
        /// <param name="logs">系統記錄檔陣列集合。</param>
        public static void WriteSysFileLog(string token, string clientIP, SysFileLog[] logs)
        {
            if (logs == null || logs.Length == 0)
            {
                return;
            }

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"App_Data\logs\{0}", DateTime.Now.ToString("yyyyMM")));
            string path = string.Format(@"{0}\{1}_{2}_{3}.log", directoryPath, token, clientIP, logs[0].Src);

            lock (WebUtil._lockSysFileLog)
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            foreach (var log in logs)
            {
                string type = string.Empty;
                switch (log.EventType)
                {
                    case EventType.Debug:
                        type = "D";
                        break;
                    case EventType.Notice:
                        type = "N";
                        break;
                    case EventType.Warning:
                        type = "W";
                        break;
                    case EventType.Error:
                        type = "E";
                        break;
                    case EventType.Fatal:
                        type = "F";
                        break;
                }

                List<string> msgs = new List<string>();
                msgs.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                msgs.Add(log.Cat);
                msgs.Add(log.Src);
                msgs.Add(type);
                msgs.Add(log.Title);
                msgs.Add(log.Msg);
                msgs.Add(clientIP);

                using (StreamWriter writer = new StreamWriter(path, true, SystemDefine.DefaultEncoding))
                {
                    writer.WriteLine(string.Join("\t", msgs.ToArray()));
                    writer.Close();
                }
            }
        }
        #endregion

        #region WriteSysFileLog (單筆寫入)
        ///// <summary>
        ///// 系統記錄檔（寫入檔案）。
        ///// </summary>
        ///// <param name="cat">種類。</param>
        ///// <param name="src">來源。</param>
        ///// <param name="eventType">類型。</param>
        ///// <param name="title">標題。</param>
        ///// <param name="msg">訊息。</param>
        ///// <param name="clientIP">使用者 IP。</param>
        //public static void WriteSysFileLog(string cat, string src, EventType eventType, string title, string msg, string clientIP)
        //{
        //    lock (WebUtil._lockSysFileLog)
        //    {
        //        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\logs");
        //        string path = string.Format(@"{0}\{1}.log", directoryPath, DateTime.Now.ToString("yyyyMM"));
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }

        //        string type = string.Empty;
        //        switch (eventType)
        //        {
        //            case EventType.Debug:
        //                type = "D";
        //                break;
        //            case EventType.Notice:
        //                type = "N";
        //                break;
        //            case EventType.Warning:
        //                type = "W";
        //                break;
        //            case EventType.Error:
        //                type = "E";
        //                break;
        //            case EventType.Fatal:
        //                type = "F";
        //                break;
        //        }

        //        List<string> msgs = new List<string>();
        //        msgs.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        msgs.Add(cat);
        //        msgs.Add(src);
        //        msgs.Add(type);
        //        msgs.Add(title);
        //        msgs.Add(msg);
        //        msgs.Add(clientIP);

        //        using (StreamWriter writer = new StreamWriter(path, true, SystemDefine.DefaultEncoding))
        //        {
        //            writer.WriteLine(string.Join("\t", msgs.ToArray()));
        //            writer.Close();
        //        }
        //    }
        //}
        #endregion
        #endregion
    }
}
