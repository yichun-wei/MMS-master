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

public partial class sys_user_chg_pwd : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientMaster MainPage { get; set; }
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
        this.WebPg = new WebPg(this, true, OperPosition.GeneralClient);
        this.MainPage = (IClientMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "修改密碼";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>修改密碼</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        Returner returner = null;
        try
        {

        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

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

    protected void btnSend_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        WebUtil.TrimTextBox(this.Form.Controls, false);

        if (string.IsNullOrEmpty(this.txtOldPwd.Text))
        {
            errMsgs.Add("請輸入「原密碼」");
        }
        else if (string.CompareOrdinal(SysUserHelper.EncodingPassword(this.txtOldPwd.Text, this.MainPage.ActorInfoSet.Info.HashKey), this.MainPage.ActorInfoSet.Info.Pwd) != 0)
        {
            errMsgs.Add("「原密碼」不符合");
        }

        if (string.IsNullOrEmpty(this.txtPwd.Text))
        {
            errMsgs.Add("請輸入「新密碼」");
        }
        else if (string.CompareOrdinal(this.txtPwd.Text, this.txtPwdConfirm.Text) != 0)
        {
            errMsgs.Add("「新密碼」確認有誤");
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
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return;
        }
        #endregion

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);
            returner = entitySysUser.UpdatePassword(actorSId, this.MainPage.ActorInfoSet.Info.SId, SysUserHelper.EncodingPassword(this.txtPwd.Text, this.MainPage.ActorInfoSet.Info.HashKey));

            JSBuilder.AlertMessage(this, "您的密碼已經更新");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
}