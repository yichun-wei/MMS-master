﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class dom_dist_edit : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_DIST";
    string AUTH_NAME = "內銷地區";

    /// <summary>
    /// 主版。
    /// </summary>
    IClientEditMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 功能權限。
    /// </summary>
    FunctionRight FunctionRight { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    PubCat.Info OrigInfo { get; set; }
    #endregion

    ISystemId _pubCatSId;

    int _useTgt = (int)PubCat.UseTgtOpt.DomDist;

    class AuthItem
    {

    }

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
        this.MainPage = (IClientEditMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        //權限初始
        this.FunctionRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight(this.AUTH_CODE);
        if (!this.FunctionRight.View)
        {
            Response.Redirect(SystemDefine.HomePageUrl);
            return false;
        }

        if (this.IsPostBack)
        {
            //編輯頁指定導向
            if (Session[SessionDefine.SystemBuffer] is Uri)
            {
                string redirectUrl = ((Uri)Session[SessionDefine.SystemBuffer]).ToString();
                Session.Remove(SessionDefine.SystemBuffer);
                Response.Redirect(redirectUrl);
                return false;
            }
        }

        this._pubCatSId = ConvertLib.ToSId(Request.QueryString["sid"]);

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "內銷地區";

        Returner returner = null;
        try
        {
            if (this._pubCatSId != null)
            {
                #region 修改
                if (this.SetEditData(this._pubCatSId))
                {
                    this.PageTitle = string.Format("{0}", this.OrigInfo.Name);

                    //會用在備貨單的單號, 建立了就不允許再修改了.
                    this.txtCode.ReadOnly = false;
                }
                else
                {
                    JSBuilder.ClosePage(this);
                    return false;
                }
                #endregion
            }
            else
            {
                #region 新增
                //新增
                this.PageTitle = "新增內銷地區";

                //產生一組新的系統代號並暫存
                this.hidSpecSId.Value = new SystemId().ToString();
                #endregion
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
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

        //權限操作檢查
        this.btnSend.Visible = this.btnSend.Visible ? this.FunctionRight.Maintain : this.btnSend.Visible;
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            returner = entityPubCat.GetInfo(new ISystemId[] { systemId }, IncludeScope.All);
            if (returner.IsCompletedAndContinue)
            {
                var info = PubCat.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                this.OrigInfo = info;

                this.hidSpecSId.Value = info.SId.Value; //暫存系統代號。

                this.txtCode.Text = info.Code;
                this.txtName.Text = info.Name;
                this.txtSort.Text = info.Sort.ToString();

                #region 自訂欄位
                if (!string.IsNullOrEmpty(info.CustField))
                {
                    var custFileds = CustJson.DeserializeObject<List<PubCat.CustField>>(info.CustField);

                    this.txtCompName.Text = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.CompName).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                    this.txtTel.Text = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Tel).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                    this.txtFax.Text = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Fax).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                    this.txtAddr.Text = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Addr).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                }
                #endregion

                //異動記錄
                this.ucEditDataTransLog.Visible = true;
                this.ucEditDataTransLog.SetInfo(info.CSId, info.Cdt, info.MSId, info.Mdt);

                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region btnSend_Click
    protected void btnSend_Click(object sender, EventArgs e)
    {
        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        WebUtil.TrimTextBox(this.Form.Controls, false);

        if (string.IsNullOrEmpty(this.txtCode.Text))
        {
            errMsgs.Add("請輸入「代碼」");
        }

        if (string.IsNullOrEmpty(this.txtName.Text))
        {
            errMsgs.Add("請輸入「地區」");
        }
        else if (this.txtName.Text.Length < 2)
        {
            //[20160107 by fan] 地區再細分辦事處 (例「上海-蘇州辦、上海-南京辦」), 主要用在訂單的地區識別, 在取客戶時, 使用地區前兩碼即可.
            errMsgs.Add("「地區」必須大於兩個字元以上");
        }

        if (string.IsNullOrEmpty(this.txtCompName.Text))
        {
            errMsgs.Add("請輸入「公司名稱」");
        }

        if (string.IsNullOrEmpty(this.txtTel.Text))
        {
            errMsgs.Add("請輸入「TEL」");
        }

        if (string.IsNullOrEmpty(this.txtFax.Text))
        {
            errMsgs.Add("請輸入「FAX」");
        }

        if (string.IsNullOrEmpty(this.txtAddr.Text))
        {
            errMsgs.Add("請輸入「地址」");
        }

        if (!VerificationLib.IsNumber(this.txtSort.Text))
        {
            errMsgs.Add("請輸入「排序」(或格式不正確)");
        }

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return;
        }
        #endregion

        if (this._pubCatSId == null)
        {
            this.Add();
        }
        else
        {
            this.Modify();
        }
    }
    #endregion

    #region Add
    void Add()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            if (!entityPubCat.CheckEditModeForCode(this._pubCatSId, this._useTgt, this.txtCode.Text, IncludeScope.All))
            {
                JSBuilder.AlertMessage(this, "「代碼」已經存在");
                return;
            }

            #region 自訂欄位
            var custFileds = new List<PubCat.CustField>();

            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.CompName, this.txtCompName.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Tel, this.txtTel.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Fax, this.txtFax.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Addr, this.txtAddr.Text));
            #endregion

            var input = new PubCat.Info()
            {
                SId = new SystemId(this.hidSpecSId.Value),
                Enabled = true,
                UseTgt = this._useTgt,
                PunitSId = DefVal.SId,
                ParentSId = DefVal.SId,
                Code = this.txtCode.Text.ToUpperInvariant(),
                Name = this.txtName.Text,
                ContTitle = DefVal.Str,
                Cont = DefVal.Str,
                Desc = DefVal.Str,
                MaxLayer = DefVal.Int,
                CustField = CustJson.SerializeObject(custFileds),
                Sort = Convert.ToInt32(this.txtSort.Text)
            };

            returner = entityPubCat.Add(actorSId, input.SId, input.Enabled, input.UseTgt, input.PunitSId, input.ParentSId, input.Code, input.Name, input.ContTitle, input.Cont, input.Desc, input.MaxLayer, input.CustField, input.Sort);
            if (returner.IsCompletedAndContinue)
            {
                JSBuilder.OpenerPostBack(this);

                //異動記錄
                string dataTitle = string.Format("{0}", input.Name);
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.PUB_CAT, input.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                {
                }

                //回到列表
                JSBuilder.ClosePage(this);
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region Modify
    void Modify()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            if (!entityPubCat.CheckEditModeForCode(this._pubCatSId, this._useTgt, this.txtCode.Text, IncludeScope.All))
            {
                JSBuilder.AlertMessage(this, "「代碼」已經存在");
                return;
            }

            //異動記錄
            string dataTitle = this.OrigInfo.Name;
            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.PUB_CAT, this._pubCatSId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
            {
            }

            #region 自訂欄位
            var custFileds = new List<PubCat.CustField>();

            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.CompName, this.txtCompName.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Tel, this.txtTel.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Fax, this.txtFax.Text));
            custFileds.Add(new PubCat.CustField(PubCat.CustField.DomDist.Addr, this.txtAddr.Text));
            #endregion

            var input = new PubCat.Info()
            {
                Enabled = true,
                ParentSId = DefVal.SId,
                Code = this.txtCode.Text.ToUpperInvariant(),
                Name = this.txtName.Text,
                ContTitle = DefVal.Str,
                Cont = DefVal.Str,
                Desc = DefVal.Str,
                MaxLayer = DefVal.Int,
                CustField = CustJson.SerializeObject(custFileds),
                Sort = Convert.ToInt32(this.txtSort.Text)
            };

            returner = entityPubCat.Modify(actorSId, this._pubCatSId, input.Enabled, input.ParentSId, input.Code, input.Name, input.ContTitle, input.Cont, input.Desc, input.MaxLayer, input.CustField, input.Sort);
            if (returner.IsCompletedAndContinue)
            {
                JSBuilder.OpenerPostBack(this);

                //回到列表
                JSBuilder.ClosePage(this);
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}