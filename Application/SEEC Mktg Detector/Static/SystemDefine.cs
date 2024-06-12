using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.IO;
using System.Windows.Forms;

using EzCoding;

namespace Seec.Marketing
{
    /// <summary>
    /// 定義系統的對應名稱操作。
    /// </summary>
    public static class SystemDefine
    {
        #region 系統相關
        /// <summary>
        /// 是否在執行時即立即啟動。
        /// </summary>
        public static bool RuningAndStart
        {
            get
            {
                return string.Compare(ConfigurationManager.AppSettings["RuningAndStart"], "Y", StringComparison.OrdinalIgnoreCase) == 0;
            }
        }

        /// <summary>
        /// 掃描間隔時間（單位:分鐘）。
        /// </summary>
        public static int ScanInterval
        {
            get
            {
                if (VerificationLib.IsNumber(ConfigurationManager.AppSettings["ScanInterval"]))
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["ScanInterval"]);
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// LOG 的輸出路徑 (若空值則輸出至應用程式根目錄)。
        /// </summary>
        public static string LogPath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["LogPath"];
                if (string.IsNullOrEmpty(path))
                {
                    path = Path.Combine(Application.StartupPath, "Logs");
                }
                return path;
            }
        }

        /// <summary>
        /// 訊息記錄文字方塊的最大顯示行數。
        /// </summary>
        internal static int MessageLogsMaxLine
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["MessageLogsMaxLine"]); }
        }
        #endregion

        #region 資料庫相關
        /// <summary> 
        /// 取得資料庫連線資訊。 
        /// </summary> 
        public static string ConnInfo
        {
            get { return ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString; }
        }
        #endregion

        #region ERP 介接相關
        /// <summary> 
        /// ERP Agent Web Service Url。 
        /// </summary> 
        public static string ErpAgentWSUrl
        {
            get { return ConfigurationManager.AppSettings["ErpAgentWSUrl"]; }
        }

        /// <summary> 
        /// ERP Agent 客戶端 Private Key。 
        /// </summary> 
        public static string ErpAgentPrivateKey
        {
            get { return ConfigurationManager.AppSettings["ErpAgentPrivateKey"]; }
        }
        #endregion

        #region ERP 客戶資料轉入排程器
        /// <summary>
        /// 是否啟用截取 ERP 客戶資料轉入排程器。
        /// </summary>
        public static bool EnableImportErpCusterScheduler
        {
            get { return string.Compare(ConfigurationManager.AppSettings["EnableImportErpCusterScheduler"], "Y", StringComparison.OrdinalIgnoreCase) == 0; }
        }

        /// <summary>
        /// 截取 ERP 客戶資料轉入排程器的每日掃描時間 (HH:mm:ss)。
        /// </summary>
        public static TimeSpan ImportErpCusterSchedulerScanTime
        {
            get
            {
                TimeSpan scanTime;
                if (TimeSpan.TryParse(ConfigurationManager.AppSettings["ImportErpCusterSchedulerScanTime"], out scanTime))
                {
                    return scanTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        #endregion

        #region ERP 庫存資料轉入排程器
        /// <summary>
        /// 是否啟用截取 ERP 庫存資料轉入排程器。
        /// </summary>
        public static bool EnableImportErpInvScheduler
        {
            get { return string.Compare(ConfigurationManager.AppSettings["EnableImportErpInvScheduler"], "Y", StringComparison.OrdinalIgnoreCase) == 0; }
        }

        /// <summary>
        /// 截取 ERP 庫存資料轉入排程器的每日掃描時間 (HH:mm:ss)。
        /// </summary>
        public static TimeSpan ImportErpInvSchedulerScanTime
        {
            get
            {
                TimeSpan scanTime;
                if (TimeSpan.TryParse(ConfigurationManager.AppSettings["ImportErpInvSchedulerScanTime"], out scanTime))
                {
                    return scanTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        #endregion

        #region ERP 折扣資料轉入排程器
        /// <summary>
        /// 是否啟用截取 ERP 折扣資料轉入排程器。
        /// </summary>
        public static bool EnableImportErpDctScheduler
        {
            get { return string.Compare(ConfigurationManager.AppSettings["EnableImportErpDctScheduler"], "Y", StringComparison.OrdinalIgnoreCase) == 0; }
        }

        /// <summary>
        /// 截取 ERP 折扣資料轉入排程器的每日掃描時間 (HH:mm:ss)。
        /// </summary>
        public static TimeSpan ImportErpDctSchedulerScanTime
        {
            get
            {
                TimeSpan scanTime;
                if (TimeSpan.TryParse(ConfigurationManager.AppSettings["ImportErpDctSchedulerScanTime"], out scanTime))
                {
                    return scanTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        #endregion

        #region ERP 內銷訂單狀態更新排程器
        /// <summary>
        /// 是否啟用 ERP 內銷訂單狀態更新排程器。
        /// </summary>
        public static bool EnableUptDomErpOdrScheduler
        {
            get { return string.Compare(ConfigurationManager.AppSettings["EnableUptDomErpOdrScheduler"], "Y", StringComparison.OrdinalIgnoreCase) == 0; }
        }

        /// <summary>
        /// ERP 內銷訂單狀態更新排程器掃描間隔時間 (單位: 分鐘)。
        /// </summary>
        public static int UptDomErpOdrSchedulerScanInterval
        {
            get { return ConvertLib.ToInt(ConfigurationManager.AppSettings["UptDomErpOdrSchedulerScanInterval"], 20); }
        }
        #endregion

        #region ERP 外銷出貨單狀態更新排程器
        /// <summary>
        /// 是否啟用 ERP 外銷出貨單狀態更新排程器。
        /// </summary>
        public static bool EnableUptExtShipOdrScheduler
        {
            get { return string.Compare(ConfigurationManager.AppSettings["EnableUptExtShipOdrScheduler"], "Y", StringComparison.OrdinalIgnoreCase) == 0; }
        }

        /// <summary>
        /// ERP 外銷出貨單狀態更新排程器掃描間隔時間 (單位: 分鐘)。
        /// </summary>
        public static int UptExtShipOdrSchedulerScanInterval
        {
            get { return ConvertLib.ToInt(ConfigurationManager.AppSettings["UptExtShipOdrSchedulerScanInterval"], 20); }
        }
        #endregion
    }
}
