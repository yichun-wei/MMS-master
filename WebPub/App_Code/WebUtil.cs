using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Transactions;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using EzCoding.Web.UI;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 網頁工具。
    /// </summary>
    public static class WebUtil
    {
        #region GetMailBase
        /// <summary>
        /// 取得已包含寄件人及基本設定的 MailMessage。
        /// </summary>
        public static MailMessage GetMailBase()
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SystemDefine.MailFromAddress, SystemDefine.MailFromName);
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = MailPriority.Normal;
            return mail;
        }
        #endregion

        #region GetSmtpClient
        /// <summary>
        /// 取得 SmtpClient。
        /// </summary>
        /// <returns>SmtpClient。</returns>
        public static SmtpClient GetSmtpClient()
        {
            return WebUtil.GetSmtpClient(SystemDefine.SmtpDomainName, SystemDefine.SmtpCredentialName, SystemDefine.SmtpCredentialPassword);
        }

        /// <summary>
        /// 取得 SmtpClient。
        /// </summary>
        /// <param name="smtpDomainName">SMTP 的網域名稱。</param>
        /// <param name="smtpCredentialName">SMTP 的認證名稱。</param>
        /// <param name="smtpCredentialPassword">SMTP 的認證密碼。</param>
        /// <returns>SmtpClient。</returns>
        public static SmtpClient GetSmtpClient(string smtpDomainName, string smtpCredentialName, string smtpCredentialPassword)
        {
            SmtpClient client = new SmtpClient(smtpDomainName);

            if (!string.IsNullOrEmpty(smtpCredentialName) && !string.IsNullOrEmpty(smtpCredentialPassword))
            {
                System.Net.NetworkCredential credential = new System.Net.NetworkCredential(smtpCredentialName, smtpCredentialPassword);
                client.UseDefaultCredentials = false;
                client.Credentials = credential;
            }

            return client;
        }
        #endregion

        #region VerifyUploadFileExtension
        /// <summary>
        /// 驗證上傳的檔案副檔名是否符合指定的副檔名。
        /// </summary>
        /// <param name="control">System.Web.UI.WebControls.FileUpload。</param>
        /// <param name="allowedExtensions">被允許的副檔名。</param>
        /// <returns>是否符合指定的副檔名</returns>
        public static bool VerifyUploadFileExtension(FileUpload control, string[] allowedExtensions)
        {
            string fileExtension = Path.GetExtension(control.FileName);
            bool isAllowedFile = false;
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (string.Compare(fileExtension, allowedExtensions[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isAllowedFile = true;
                    break;
                }
            }
            return isAllowedFile;
        }

        /// <summary>
        /// 驗證上傳的檔案副檔名是否符合指定的副檔名。
        /// </summary>
        /// <param name="fileExtension">上傳的檔案副檔名。</param>
        /// <param name="allowedExtensions">被允許的副檔名。</param>
        /// <returns>是否符合指定的副檔名</returns>
        public static bool VerifyUploadFileExtension(string fileExtension, string[] allowedExtensions)
        {
            bool isAllowedFile = false;
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (string.Compare(fileExtension, allowedExtensions[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isAllowedFile = true;
                    break;
                }
            }
            return isAllowedFile;
        }
        #endregion

        #region TrimTextBox
        /// <summary>
        /// 移除控制項 (含子控制項) 中所有輸入控制項的前後空白字元。
        /// </summary>
        /// <param name="ctrls">控制項集合。</param>
        /// <param name="filterXss">是否過濾 Xss。</param>
        public static void TrimTextBox(ControlCollection ctrls, bool filterXss)
        {
            foreach (Control ctrl in ctrls)
            {
                WebUtil.TrimTextBox(ctrl.Controls, filterXss);

                //var editor = ctrl as CKEditor.NET.CKEditorControl;
                //if (editor != null)
                //{
                //    editor.Text = editor.Text.Trim();
                //    continue;
                //}

                var txt = ctrl as TextBox;
                if (txt != null)
                {
                    if (filterXss)
                    {
                        txt.Text = HtmlEditor.FilterXss(txt.Text).Trim();
                    }
                    else
                    {
                        txt.Text = txt.Text.Trim();
                    }
                    continue;
                }
            }
        }
        #endregion

        #region FillHyperLinkTitle
        /// <summary>
        /// 填上所有超連結的 Title。
        /// </summary>
        /// <param name="ctrls"></param>
        public static void FillHyperLinkTitle(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                WebUtil.FillHyperLinkTitle(ctrl.Controls);

                var lnk = ctrl as HyperLink;
                if (lnk != null && !string.IsNullOrEmpty(lnk.Text) && string.IsNullOrEmpty(lnk.ToolTip))
                {
                    lnk.ToolTip = lnk.Text;
                }
            }
        }
        #endregion

        #region WriteSysFileLog
        static object _lockSysFileLog = new object();

        /// <summary>
        /// 系統記錄檔（寫入檔案）。
        /// </summary>
        /// <param name="message">訊息。</param>
        public static void WriteSysFileLog(string message)
        {
            lock (WebUtil._lockSysFileLog)
            {
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\logs");
                string path = string.Format(@"{0}\{1}.log", directoryPath, DateTime.Now.ToString("yyyyMM"));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.Default))
                {
                    writer.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString("yyyyMMddHHmmss"), message));
                    writer.Close();
                }
            }
        }
        #endregion

        #region WriteSysLog
        /// <summary>
        /// 系統記錄檔（寫入資料庫）。
        /// </summary>
        /// <param name="actorId">操作人代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="cat">種類。</param>
        /// <param name="src">來源。</param>
        /// <param name="eventType">類型。</param>
        /// <param name="title">標題。</param>
        /// <param name="msg">訊息。</param>
        /// <param name="clientIP">使用者 IP。</param>
        /// <param name="cond1">條件一。</param>
        /// <param name="cond2">條件二。</param>
        /// <param name="cond3">條件三。</param>
        /// <param name="cond4">條件四。</param>
        /// <param name="cond5">條件五。</param>
        public static void WriteSysLog(ISystemId actorId, string cat, string src, EventType eventType, string title, string msg, string clientIP)
        {
            WebUtil.WriteSysLog(actorId, cat, src, eventType, title, msg, clientIP, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Str);
        }

        /// <summary>
        /// 系統記錄檔（寫入資料庫）。
        /// </summary>
        /// <param name="actorId">操作人代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="cat">種類。</param>
        /// <param name="src">來源。</param>
        /// <param name="eventType">類型。</param>
        /// <param name="title">標題。</param>
        /// <param name="msg">訊息。</param>
        /// <param name="clientIP">使用者 IP。</param>
        /// <param name="cond1">條件一。</param>
        /// <param name="cond2">條件二。</param>
        /// <param name="cond3">條件三。</param>
        /// <param name="cond4">條件四。</param>
        /// <param name="cond5">條件五。</param>
        public static void WriteSysLog(ISystemId actorId, string cat, string src, EventType eventType, string title, string msg, string clientIP, string cond1, string cond2, string cond3, string cond4, string cond5)
        {
            string type = string.Empty;
            switch (eventType)
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

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    new SysLog(SystemDefine.ConnInfo).Add(actorId, cat, src, type, title, msg, clientIP, cond1, cond2, cond3, cond4, cond5);
                }
            }
            catch (Exception ex)
            {
                WebUtil.WriteSysFileLog(string.Format("[SYS LOG FAILED] {0}", ex));
            }
        }
        #endregion

        #region FilteringSqlInjection
        /// <summary>
        /// 過濾會引發 SQL Injection 的字串。
        /// </summary>
        /// <param name="value">字串值。</param>
        /// <returns></returns>
        public static string FilteringSqlInjection(string value)
        {
            //移除「or」、「and」、「--」、「;」字串
            string pattern = "(or|and|--|;)";
            return Regex.Replace(value, pattern, string.Empty, RegexOptions.IgnoreCase);
        }
        #endregion

        #region Data Util
        /// <summary>
        /// 檢查及設定被選取的 ListControl 項目。
        /// </summary>
        /// <param name="value">項目值。</param>
        /// <param name="list">System.Web.UI.WebControls.ListControl。</param>
        public static void SetListControlSelected(string val, ListControl list)
        {
            if (!string.IsNullOrEmpty(val))
            {
                WebUtilBox.SetListControlSelected(val, list);
            }
        }

        /// <summary>
        /// 檢查及設定被選取的 ListControl 項目。
        /// </summary>
        /// <param name="value">項目值。</param>
        /// <param name="list">System.Web.UI.WebControls.ListControl。</param>
        public static void SetListControlSelected(int? val, ListControl list)
        {
            if (val != null)
            {
                WebUtilBox.SetListControlSelected(val.Value.ToString(), list);
            }
        }

        /// <summary>
        /// 依定義的部份識別名稱尋找 Radio 控制項並設定核選。
        /// </summary>
        /// <param name="baseControl">指定的母控制項。</param>
        /// <param name="idPart">主要識別名稱。</param>
        /// <param name="idSeq">次要識別名稱。</param>
        public static void FindRadioButtonAndCheck(Control baseControl, string idPart, string idSeq)
        {
            var ctrl = WebUtilBox.FindControl<RadioButton>(baseControl, string.Format("{0}{1}", idPart, idSeq));
            if (ctrl != null)
            {
                ctrl.Checked = true;
            }
        }

        /// <summary>
        /// 依定義的部份識別名稱尋找 Radio 控制項並傳回核選值。
        /// </summary>
        /// <param name="baseControl">指定的母控制項。</param>
        /// <param name="idPart">主要識別名稱。</param>
        /// <param name="idSeqs">次要識別名稱。</param>
        /// <returns>若尋找到被核選的項目則回傳核選值；否則回傳 string.Empty。</returns>
        public static string FindCheckedRadioButton(Control baseControl, string idPart, string[] idSeqs)
        {
            foreach (var s in idSeqs)
            {
                var ctrl = WebUtilBox.FindControl<RadioButton>(baseControl, string.Format("{0}{1}", idPart, s));
                if (ctrl != null)
                {
                    if (ctrl.Checked)
                    {
                        return s;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 依定義的部份識別名稱尋找 CheckBox 控制項並設定核選。
        /// </summary>
        /// <param name="baseControl">指定的母控制項。</param>
        /// <param name="idPart">主要識別名稱。</param>
        /// <param name="idSeq">次要識別名稱。</param>
        public static void FindCheckBoxAndCheck(Control baseControl, string idPart, string[] idSeqs)
        {
            foreach (var s in idSeqs)
            {
                var ctrl = WebUtilBox.FindControl<CheckBox>(baseControl, string.Format("{0}{1}", idPart, s));
                if (ctrl != null)
                {
                    ctrl.Checked = true;
                }
            }
        }

        /// <summary>
        /// 取得 ListControl 被核選的項目，並組成「-Value-Value-」傳回。
        /// </summary>
        /// <param name="list">System.Web.UI.WebControls.ListControl。</param>
        /// <returns>若尋找到被核選的項目則回傳「-Value-Value-」；否則回傳 string.Empty。</returns>
        public static string GetCheckedCheckBox(ListControl list)
        {
            List<string> seledItems = new List<string>();
            foreach (ListItem item in list.Items)
            {
                if (item.Selected)
                {
                    seledItems.Add(item.Value);
                }
            }

            if (seledItems.Count > 0)
            {
                return string.Format("-{0}-", string.Join("-", seledItems.ToArray()));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 依定義的部份識別名稱尋找 CheckBox 控制項並將被核選項目組成「-Value-Value-」。
        /// </summary>
        /// <param name="baseControl">指定的母控制項。</param>
        /// <param name="idPart">主要識別名稱。</param>
        /// <param name="idSeqs">次要識別名稱。</param>
        /// <returns>若尋找到被核選的項目則回傳「-Value-Value-」；否則回傳 string.Empty。</returns>
        public static string FindCheckedCheckBox(Control baseControl, string idPart, string[] idSeqs)
        {
            List<string> seledItems = new List<string>();
            foreach (var s in idSeqs)
            {
                var ctrl = WebUtilBox.FindControl<CheckBox>(baseControl, string.Format("{0}{1}", idPart, s));
                if (ctrl != null)
                {
                    if (ctrl.Checked)
                    {
                        seledItems.Add(s);
                    }
                }
            }

            if (seledItems.Count > 0)
            {
                return string.Format("-{0}-", string.Join("-", seledItems.ToArray()));
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}