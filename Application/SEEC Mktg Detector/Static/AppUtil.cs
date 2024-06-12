using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Seec.Marketing
{
    /// <summary>
    /// 應用程式工具。
    /// </summary>
    public static class AppUtil
    {
        #region WriteSystemLog
        static object _lockSystemLog = new object();

        /// <summary>
        /// 將訊息寫入檔案。
        /// </summary>
        /// <param name="category">訊息類別。</param>
        /// <param name="message">訊息。</param>
        public static void WriteToFile(string category, string message)
        {
            lock (AppUtil._lockSystemLog)
            {
                string directoryPath = SystemDefine.LogPath;
                string path = Path.Combine(directoryPath, string.Format(@"{0}.log", DateTime.Now.ToString("yyyyMMdd")));

                try
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (StreamWriter writer = new StreamWriter(path, true, Encoding.Default))
                    {
                        writer.WriteLine(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), category, message));
                        writer.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion
    }
}
