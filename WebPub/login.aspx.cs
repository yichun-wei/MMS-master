using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class login : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }
    #endregion

    #region 網頁初始的一連貫操作
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.InitVital()) { return; }
        if (!this.InitPage()) { return; }
        if (!this.LoadIncludePage()) { return; }

        this.HasInitial = true;
    }

    #region 初始化的必要操作
    /// <summary>
    /// 初始化的必要操作。
    /// </summary>
    private bool InitVital()
    {
        this.WebPg = new WebPg(this, false, OperPosition.GeneralClient);

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = string.Empty;

        //Enter 控制
        this.txtAcct.Attributes["onkeypress"] = string.Format("return EnterClick(event, '{0}');", this.btnLogin.ClientID);
        this.txtPwd.Attributes["onkeypress"] = string.Format("return EnterClick(event, '{0}');", this.btnLogin.ClientID);
        this.txtVerifyCode.Attributes["onkeypress"] = string.Format("return EnterClick(event, '{0}');", this.btnLogin.ClientID);

        if (!this.IsPostBack)
        {
            this.btnRegetVerifyCode_Click(null, EventArgs.Empty);
        }

        return true;
    }
    #endregion

    #region 載入使用者控制項
    /// <summary>
    /// 載入使用者控制項。
    /// </summary>
    private bool LoadIncludePage()
    {
        return true;
    }
    #endregion
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        this.WebPg.RegisterScript(string.Format("$get('{0}').focus();", this.txtAcct.ClientID));
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);
    }

    protected void btnRegetVerifyCode_Click(object sender, EventArgs e)
    {
        this.imgVerifyCode.ImageUrl = string.Format("~/common/verify_code.aspx?{0}", Guid.NewGuid().ToString("N"));
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        Returner returner = null;
        Returner returnerTmp = null;
        try
        {
            SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);
            LoginLog entityLoginLog = new LoginLog(SystemDefine.ConnInfo);

            #region 驗證檢查
            List<string> errMsgs = new List<string>();

            WebUtil.TrimTextBox(this.Form.Controls, false);

            if (string.IsNullOrEmpty(this.txtAcct.Text))
            {
                errMsgs.Add("請輸入「帳號」");
            }

            if (!string.IsNullOrEmpty(this.txtAcct.Text) && entityLoginLog.IsOverAttemptCount(this.txtAcct.Text, SystemDefine.LoginMaxAttemptCount, SystemDefine.LoginFailedWithinMinutes))
            {
                errMsgs.Add("您的帳號已鎖定，請稍後再嘗試登入！");
            }
            else
            {
                if (string.IsNullOrEmpty(this.txtPwd.Text))
                {
                    errMsgs.Add("請輸入「密碼」");
                }
            }

            if (Session[SessionDefine.ImageVerifyCode] == null)
            {
                errMsgs.Add("「驗證碼」已經失效 (請重新取得驗證碼)");
            }
            else if (string.Compare(this.txtVerifyCode.Text, Session[SessionDefine.ImageVerifyCode].ToString(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                errMsgs.Add("「驗證碼」驗證錯誤");
            }

            this.txtVerifyCode.Text = string.Empty;
            Session.Remove(SessionDefine.ImageVerifyCode);

            if (errMsgs.Count != 0)
            {
                JSBuilder.AlertMessage(this, true, errMsgs.ToArray());
                return;
            }
            #endregion

            returner = entitySysUser.GetInfo(new SysUser.InfoConds(DefVal.SIds, ConvertLib.ToStrs(this.txtAcct.Text), DefVal.Ints, DefVal.SIds), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeletedAndEnabledBoth);
            if (returner.IsCompletedAndContinue)
            {
                var info = Seec.Marketing.SystemEngines.SysUser.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                //檢查密碼是否相符(區分大小寫)。 
                if (string.CompareOrdinal(SysUserHelper.EncodingPassword(this.txtPwd.Text, info.HashKey), info.Pwd) != 0)
                {
                    //新增登入記錄。 
                    entityLoginLog.Add(SystemId.Empty, info.SId, this.txtAcct.Text, this.txtPwd.Text, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, false, "密碼錯誤");
                    JSBuilder.AlertMessage(this, true, "登入失敗");
                    return;
                }

                //新增登入記錄。 
                returnerTmp = entityLoginLog.Add(SystemId.Empty, info.SId, this.txtAcct.Text, this.txtPwd.Text, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, true, "success");
                if (returnerTmp.IsCompletedAndContinue)
                {
                    CookiesUtil.Write(CookiesDefine.SysUserLoginLogSId, returnerTmp.DataSet.Tables["NEW_SID"].Rows[0]["SID"].ToString());
                }

                //確保前次不是使用登出操作, 而是使用直接敲網址到登入頁, 因而仍存在前次的 Session.
                Session.Clear();

                //更新登入快取。 
                entitySysUser.UpdateLoginCache(info.SId, WebUtilBox.GetUserHostAddress(), DateTime.Now);

                if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Session)
                {
                    Session[SessionDefine.SysUserSId] = info.SId;
                }
                else
                {
                    CookiesUtil.Write(CookiesDefine.SysUserSId, info.SId.Value);
                }

                CookiesUtil.Write(CookiesDefine.SysUserStatusFlag, info.StatusFlag);
                if (SystemDefine.EnableSSL)
                {
                    if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Cookies)
                    {
                        Response.Cookies[CookiesDefine.SysUserSId].Secure = true;
                    }
                    Response.Cookies[CookiesDefine.SysUserStatusFlag].Secure = true;
                }

                if (Session[SessionDefine.ClientSrcPage] != null)
                {
                    Response.Redirect(Session[SessionDefine.ClientSrcPage].ToString(), false);
                }
                else
                {
                    Response.Redirect(SystemDefine.HomePageUrl, false);
                }
            }
            else
            {
                //新增登入記錄。
                entityLoginLog.Add(SystemId.Empty, DefVal.SId, this.txtAcct.Text, this.txtPwd.Text, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, false, "找不到指定的帳號");
                JSBuilder.AlertMessage(this, true, "登入失敗");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
            if (returnerTmp != null) { returnerTmp.Dispose(); }
        }
    }
}