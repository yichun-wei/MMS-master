using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;

namespace Seec.Marketing
{
    /// <summary>
    /// 定義系統的對應名稱操作。
    /// </summary>
    public static class SystemDefine
    {
        /// <summary> 
        /// 取得系統預設的編碼方式。 
        /// </summary> 
        public static Encoding DefaultEncoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary> 
        /// 取得 ERP 資料庫連線資訊。 
        /// </summary> 
        public static string ConnInfoErp
        {
            get { return ConfigurationManager.ConnectionStrings["ConnInfoErp"].ConnectionString; }
        }

        /// <summary>
        /// ERP Agent 客戶端 Private Key。
        /// </summary>
        public static string ErpAgentPrivateKey
        {
            get { return ConfigurationManager.AppSettings["ErpAgentPrivateKey"]; }
        }
    }
}
