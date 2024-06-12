using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Text;

using EzCoding;

namespace Seec.Marketing
{
    /// <summary>
    /// 定義系統的對應名稱操作。
    /// </summary>
    public static class SystemDefine
    {
        /// <summary> 
        /// 取得 CSV 匯入及匯出的編碼方式。 
        /// </summary> 
        public static Encoding CsvEncoding
        {
            get { return Encoding.GetEncoding(950); }
        }

        /// <summary> 
        /// 取得系統預設的編碼方式。 
        /// </summary> 
        public static Encoding DefaultEncoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary> 
        /// 資料異動記錄的 XML 根節點名稱。 
        /// </summary> 
        public static string DataTransLogXmlRootName
        {
            get { return "DataTransLog"; }
        }

        /// <summary> 
        /// 取得首頁的網址。 
        /// </summary> 
        public static string HomePageUrl
        {
            get { return SystemDefine.HttpsWebSiteRoot + "index.aspx"; }
        }

        /// <summary> 
        /// 取得系統管理首頁的網址。 
        /// </summary> 
        public static string SysMgtHomePageUrl
        {
            get { return SystemDefine.HttpsWebSiteRoot + "sys_mgt/index.aspx"; }
        }

        /// <summary> 
        /// 取得系統管理登入頁的網址。 
        /// </summary> 
        public static string SysMgtLoginUrl
        {
            get { return SystemDefine.HttpsWebSiteRoot + "sys_mgt/login.aspx"; }
        }

        /// <summary> 
        /// 取得一般使用者登入頁的網址。 
        /// </summary> 
        public static string CltLoginUrl
        {
            get { return SystemDefine.HttpsWebSiteRoot + "login.aspx"; }
        }

        /// <summary> 
        /// 取得資料庫連線資訊。 
        /// </summary> 
        public static string ConnInfo
        {
            get { return ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString; }
        }

        /// <summary> 
        /// 取得使用者介面網頁的抬頭。 
        /// </summary> 
        public static string ClientPageTitle
        {
            get { return ConfigurationManager.AppSettings["ClientPageTitle"]; }
        }

        /// <summary> 
        /// 取得系統管理介面網頁的抬頭。 
        /// </summary> 
        public static string SysMgtPageTitle
        {
            get { return ConfigurationManager.AppSettings["SysMgtPageTitle"]; }
        }

        /// <summary> 
        /// SMTP 的網域名稱。 
        /// </summary> 
        public static string SmtpDomainName
        {
            get { return ConfigurationManager.AppSettings["SmtpDomainName"]; }
        }

        /// <summary> 
        /// SMTP 的認證名稱。 
        /// </summary> 
        public static string SmtpCredentialName
        {
            get { return ConfigurationManager.AppSettings["SmtpCredentialName"]; }
        }

        /// <summary> 
        /// SMTP 的認證密碼。 
        /// </summary> 
        public static string SmtpCredentialPassword
        {
            get { return ConfigurationManager.AppSettings["SmtpCredentialPassword"]; }
        }

        /// <summary> 
        /// 網站發信使用的名稱。 
        /// </summary> 
        public static string MailFromName
        {
            get { return ConfigurationManager.AppSettings["MailFromName"]; }
        }

        /// <summary> 
        /// 網站發信使用的電子信箱。 
        /// </summary> 
        public static string MailFromAddress
        {
            get { return ConfigurationManager.AppSettings["MailFromAddress"]; }
        }

        /// <summary> 
        /// 網站的網址域名 (或 IP) 和埠號。 
        /// </summary> 
        public static string WebsiteUrlAuthority
        {
            get
            {
                return HttpContext.Current.Request.Url.Authority;
            }
        }

        /// <summary> 
        /// 指定的連結網址。 
        /// </summary> 
        public static string WebHttp
        {
            get { return string.Format("http://{0}", SystemDefine.WebsiteUrlAuthority); }
        }

        /// <summary> 
        /// 指定的連結網址（SSL）。 
        /// </summary> 
        public static string WebHttps
        {
            get
            {
                if (SystemDefine.EnableSSL)
                {
                    return string.Format("https://{0}", SystemDefine.WebsiteUrlAuthority);
                }
                else
                {
                    return SystemDefine.WebHttp;
                }
            }
        }

        /// <summary> 
        /// 網站系統的根目錄。 
        /// </summary> 
        public static string WebSiteRoot
        {
            get { return ConfigurationManager.AppSettings["WebSiteRoot"]; }
        }

        /// <summary> 
        /// 包含網域的 HTTP 網站系統的根目錄。 
        /// </summary> 
        public static string HttpWebSiteRoot
        {
            get { return string.Format("{0}{1}", SystemDefine.WebHttp, SystemDefine.WebSiteRoot); }
        }

        /// <summary> 
        /// 包含網域的 HTTPS 網站系統的根目錄。 
        /// </summary> 
        public static string HttpsWebSiteRoot
        {
            get { return string.Format("{0}{1}", SystemDefine.WebHttps, SystemDefine.WebSiteRoot); }
        }

        /// <summary> 
        /// 上傳檔案的根目錄。 
        /// </summary> 
        public static string UploadRoot
        {
            get { return ConfigurationManager.AppSettings["UploadRoot"]; }
        }

        /// <summary> 
        /// 系統暫存檔案的根目錄。 
        /// </summary> 
        public static string SysTempFilesRoot
        {
            get { return ConfigurationManager.AppSettings["SysTempFilesRoot"]; }
        }

        /// <summary> 
        /// 上傳的 FTP 根目錄。 
        /// </summary> 
        public static string FtpRoot
        {
            get { return ConfigurationManager.AppSettings["FtpRoot"]; }
        }

        /// <summary> 
        /// 是否啟用 SSL。 
        /// </summary> 
        public static bool EnableSSL
        {
            get { return "Y".Equals(ConfigurationManager.AppSettings["EnableSSL"], StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary> 
        /// 目前系統的運作階段。 
        /// </summary> 
        public static SystemPhase SystemPhase
        {
            get { return (SystemPhase)ConvertLib.ToInt(ConfigurationManager.AppSettings["SystemPhase"], 11); }
        }

        /// <summary>
        /// 預設的排序值。
        /// </summary>
        public const int DefSortVal = 5000;

        /// <summary>
        /// 置頂的權重。
        /// </summary>
        public const int TopWeights = 50;

        /// <summary> 
        /// 登入的最大嘗試次數。 
        /// </summary> 
        public static int LoginMaxAttemptCount
        {
            get { return ConvertLib.ToInt(ConfigurationManager.AppSettings["LoginMaxAttemptCount"], 3); }
        }

        /// <summary> 
        /// 在 N 分鐘內登入嘗試失敗時，即鎖定帳號。 
        /// </summary> 
        public static int LoginFailedWithinMinutes
        {
            get { return ConvertLib.ToInt(ConfigurationManager.AppSettings["LoginFailedWithinMinutes"], 5); }
        }

        /// <summary> 
        /// 登入鍵值的保存方式（1:Session 2:Cookies）。
        /// </summary> 
        public static LoginKeepingBy LoginKeepingBy
        {
            get { return (LoginKeepingBy)ConvertLib.ToInt(ConfigurationManager.AppSettings["LoginKeepingBy"], 1); }
        }

        /// <summary> 
        /// 登入後最大閒置時間（分鐘）。超過即視為登出（適用於「LoginKeepingBy」為 Cookies）。
        /// </summary> 
        public static int MaxIdleTimeAfterLogin
        {
            get { return ConvertLib.ToInt(ConfigurationManager.AppSettings["MaxIdleTimeAfterLogin"], 30); }
        }

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

        /// <summary> 
        /// 串聯字串的分隔常數。 
        /// </summary> 
        public static string JoinSeparator { get { return "[#]"; } }
    }
}