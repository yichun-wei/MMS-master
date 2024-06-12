using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using EzCoding.Web.UI;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    public class WebPg
    {
        Page _page;

        #region 建構子
        /// <summary>
        /// 依指定的網頁初始化 WebPg 類別的新執行個體。
        /// </summary>
        /// <param name="page">指定操作的網頁。</param>
        public WebPg(Page page)
        {
            this._page = page;
        }

        /// <summary>
        /// 依指定的網頁初始化 WebPg 類別的新執行個體。
        /// </summary>
        /// <param name="page">指定操作的網頁。</param>
        /// <param name="keepCurrentUri">是否記錄目前完整的網址。</param>
        /// <param name="operatingPosition">使用者目前在網站上操作的位置。</param>
        public WebPg(Page page, bool keepCurrentUri, OperPosition operatingPosition)
        {
            this._page = page;
            if (keepCurrentUri)
            {
                Uri baseUri;

                if (string.Compare(page.Request.Url.Scheme, "https", true) == 0)
                {
                    baseUri = new Uri(SystemDefine.WebHttps, UriKind.Absolute);
                }
                else
                {
                    baseUri = new Uri(SystemDefine.WebHttp, UriKind.Absolute);
                }

                switch (operatingPosition)
                {
                    case OperPosition.SysMgtClient:
                        this.SysMgtSrcPage = new Uri(baseUri, page.Request.RawUrl).OriginalString;
                        break;
                    default:
                        this.ClientSrcPage = new Uri(baseUri, page.Request.RawUrl).OriginalString;
                        break;
                }
            }
        }
        #endregion

        #region 網頁抬頭
        #region 設定使用者介面網頁的抬頭
        /// <summary>
        /// 設定使用者介面網頁的抬頭。
        /// </summary>
        /// <param name="pageName">網頁名稱。</param>
        public void SetClientPageTitle(string pageName)
        {
            this._page.Title = string.IsNullOrWhiteSpace(pageName) ? SystemDefine.ClientPageTitle : string.Format
            (
               "{1} | {0}",
               SystemDefine.ClientPageTitle,
               pageName
            );

            ////只顯示網站名稱
            //this._page.Title = SystemDefine.ClientPageTitle;
        }
        #endregion

        #region 設定系統管理介面網頁的抬頭
        /// <summary>
        /// 設定系統管理介面網頁的抬頭。
        /// </summary>
        /// <param name="pageName">網頁名稱。</param>
        public void SetMgtPageTitle(string pageName)
        {
            this._page.Title = string.IsNullOrWhiteSpace(pageName) ? SystemDefine.SysMgtPageTitle : string.Format
            (
               "{1} | {0}",
               SystemDefine.SysMgtPageTitle,
               pageName
            );

            ////只顯示網站名稱
            //this._page.Title = SystemDefine.SysMgtPageTitle;
        }
        #endregion
        #endregion

        #region SourcePage 相關
        #region 一般使用者界面的來源頁網址
        /// <summary>
        /// 一般使用者界面的來源頁網址。
        /// </summary>
        public string ClientSrcPage
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context.Session[SessionDefine.ClientSrcPage] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return context.Session[SessionDefine.ClientSrcPage].ToString();
                }
            }
            set
            {
                HttpContext context = HttpContext.Current;
                if (context.Session != null)
                {
                    context.Session[SessionDefine.ClientSrcPage] = value;
                }
            }
        }
        #endregion

        #region 系統管理界面的來源頁網址
        /// <summary>
        /// 系統管理界面的來源頁網址。
        /// </summary>
        public string SysMgtSrcPage
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context.Session[SessionDefine.SysMgtSrcPage] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return context.Session[SessionDefine.SysMgtSrcPage].ToString();
                }
            }
            set
            {
                HttpContext context = HttpContext.Current;
                context.Session[SessionDefine.SysMgtSrcPage] = value;
            }
        }
        #endregion
        #endregion

        #region 目前 URI 配置名稱是否為 https
        /// <summary>
        /// 目前 URI 配置名稱是否為 https。
        /// </summary>
        public static bool IsHttpsScheme
        {
            get { return "https".Equals(HttpContext.Current.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase); }
        }
        #endregion

        #region 檢查是否包含惡意的 QueryString
        /// <summary>
        /// 檢查是否包含惡意的 QueryString。
        /// </summary>
        /// <param name="page">指定操作的網頁。</param>
        public static void CheckMaliciousQueryString(Page page)
        {
            if (!page.IsPostBack)
            {
                //載入時, QueryString 參數例如「other.aspx?t=<script>alert(719)</script>」會影響 <form action="other.aspx?t=%3cscript%3ealert(719)%3c%2fscript%3e"></form> 的 action,
                //資安檢測時會出現「跨網站 Scripting」的資安議題.
                if (QueryStringParsing.ContainsMalicious())
                {
                    page.Server.Transfer(QueryStringParsing.CurrentRelativeUri.OriginalString);
                }
            }
        }
        #endregion

        #region RetouchListItem
        /// <summary>
        /// 處理 ListControl 相關物件，避免破版。
        /// </summary>
        public static ListItem RetouchListItem(string text, string value, int maxLength)
        {
            var item = new ListItem(StringLib.CutOutPartialContents(text, maxLength, "..."), value);
            item.Attributes["title"] = text;

            return item;
        }

        /// <summary>
        /// 處理 ListControl 相關物件，避免破版。
        /// </summary>
        public static void RetouchListItem(ListControl list, int maxLength)
        {
            if (list == null)
            {
                return;
            }

            foreach (ListItem item in list.Items)
            {
                item.Attributes["title"] = item.Text;
                item.Text = StringLib.CutOutPartialContents(item.Text, maxLength, "...");
            }
        }
        #endregion

        #region 腳本相關
        /// <summary>
        /// 註冊要在網頁執行的腳本。
        /// </summary>
        /// <param name="scripts">腳本陣列集合。</param>
        public void RegisterScript(params string[] scripts)
        {
            WebUtilBox.RegisterScript(this._page, scripts);
        }

        public static void RegisterClearAsyncFileUploadedPath(Page page)
        {
            Type pageType = page.GetType();
            //for Traditional
            if (!page.ClientScript.IsClientScriptBlockRegistered(pageType, "ClearAsyncFileUploadedPath"))
            {
                StringBuilder script = new StringBuilder();
                script.Append("<script type='text/javascript' language='javascript'>");
                script.Append("function ClearAsyncFileUploadedPath(clientID){");
                script.Append("var file = $get(clientID);");
                script.Append("var form = document.createElement('form');");
                script.Append("document.body.appendChild(form);");
                //記住 file 在舊表單中的位置
                script.Append("var pos = file.nextSibling;");
                script.Append("form.appendChild(file);");
                script.Append("form.reset();");
                script.Append("pos.parentNode.insertBefore(file, pos);");
                script.Append("document.body.removeChild(form);");
                script.Append("}</script>");
                page.ClientScript.RegisterClientScriptBlock(pageType, "ClearAsyncFileUploadedPath", script.ToString());
            }

            ////for Modern
            //if (!page.ClientScript.IsClientScriptBlockRegistered(pageType, "ClearAsyncFileUploadedPath"))
            //{
            //    StringBuilder script = new StringBuilder();
            //    script.Append("<script type='text/javascript' language='javascript'>");
            //    script.Append("function ClearAsyncFileUploadedPath(clientID){");
            //    script.Append("var file = $get(clientID);");
            //    script.Append("var txts = file.getElementsByTagName('input');");
            //    script.Append("for (var i = 0; i < txts.length; i++) {");
            //    script.Append("if (txts[i].type == 'text') {");
            //    script.Append("txts[i].value = '';");
            //    script.Append("txts[i].style.backgroundColor = 'white';");
            //    script.Append("}");
            //    script.Append("}");
            //    script.Append("}</script>");
            //    page.ClientScript.RegisterClientScriptBlock(pageType, "ClearAsyncFileUploadedPath", script.ToString());
            //}
        }
        #endregion

        #region AdditionalClickToDisableScript
        /// <summary>
        /// 將指定的控制項，設定為點擊之後停用的操作腳本。
        /// </summary>
        /// <param name="control">System.Web.UI.WebControls.WebControl。</param>
        public void AdditionalClickToDisableScript(WebControl control)
        {
            string onloadAdditional = string.Format("this.disabled=false;");
            string onclickAdditional = string.Format("this.disabled=true;{0};", this._page.ClientScript.GetPostBackEventReference(control, string.Empty));

            if (!string.IsNullOrEmpty(control.Attributes["onload"]))
            {
                control.Attributes["onload"] = control.Attributes["onload"].Replace(onloadAdditional, string.Empty);
            }

            if (!string.IsNullOrEmpty(control.Attributes["onclick"]))
            {
                control.Attributes["onclick"] = control.Attributes["onclick"].Replace(onclickAdditional, string.Empty);
            }

            control.Attributes["onload"] += onloadAdditional;
            control.Attributes["onclick"] += onclickAdditional;
        }
        #endregion

        #region 測試用
        #region AddEmailBcc
        public static void AddEmailBcc(MailMessage mail)
        {
            mail.Bcc.Add(new MailAddress("fan@soez.biz", "樊小乖⊙⊙ˇ"));
        }
        #endregion
        #endregion
    }
}
