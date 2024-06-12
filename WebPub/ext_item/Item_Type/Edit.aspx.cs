using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class ext_item_Item_Type_Edit : System.Web.UI.Page
{

    #region 網頁屬性
    string AUTH_CODE = "EXT_ITEM_TYPE";
    string AUTH_NAME = "外銷商品產品別";
    /// <summary>
    /// 主版。
    /// </summary>
    IClientMaster MainPage { get; set; }
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
    /// 是否重新執行查詢動作。
    /// </summary>
    bool DoSearch { get; set; }
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
        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "外銷商品產品別：維護";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>外銷商品產品別</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        //範例: 我是登入人的帳號
        var acct = this.MainPage.ActorInfoSet.Info.Acct;
        lblUserSid.Text = this.MainPage.ActorInfoSet.Info.SId.ToString();

        //範例: 我是登入人設定的內銷地區名稱陣列集合
        var distNames = this.MainPage.ActorInfoSet.DomDistInfos.Select(q => q.Name).ToArray();

        Returner returner = null;
        try
        {

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


    string ItemType,  CBActiveFlag;


    protected void Page_Load(object sender, EventArgs e)
    {
        ItemType = Request.QueryString["ItemType"];
        lbltxtItemType.Text = ItemType;

            if (!IsPostBack)
            {
             ExtItemTypeList();
            }
      
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查

        if (this.FunctionRight.Maintain)
        { this.btnSave.Visible = true; }

        //  this.btnSave.Visible = this.btnSave.Visible ? this.FunctionRight.Maintain : this.btnSave.Visible;
        //權限控制範例 

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
       
            Save();
    }


    protected void alert(string Msg)
    {
        ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "系統訊息", "alert('" + Msg + "');", true);
    }

    private void Save()
    {

        if (!CheckInputted())
        {
            return;
        }
        else
        {
       
            Returner returner = null;
            try
            {
                ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;
                if (CB_ACTIVE_FLAG.Checked)
                { CBActiveFlag = "Y"; }
                else
                { CBActiveFlag = "N"; }

                ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);
                returner = entityExtItemType.Modify(ItemType, txt_1.Text, txt_2.Text, txt_3.Text, txt_4.Text, txt_5.Text, txt_6.Text, txt_7.Text, txt_8.Text, txt_9.Text, txt_10.Text, txt_11.Text, txt_12.Text, CBActiveFlag, DateTime.Now, actorSId);
                //回到前一頁
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('外銷商品產品別修改成功!!'); location.href='View.aspx';", true);

            }
            finally
            {
                if (returner != null) returner.Dispose();
            }

            //
        }

    }
    #region 列示頁面：外銷產品別資料
    /// <summary> 
    /// 列示頁面：外銷產品別資料
    /// </summary> 
    void ExtItemTypeList()
    {
        Returner returner = null;
        try
        {

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);

            string[] conds = new string[1] { ItemType };
            returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(conds, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5", "CATEGORY_DESC6", "CATEGORY_DESC7", "CATEGORY_DESC8", "CATEGORY_DESC9", "CATEGORY_DESC10", "CATEGORY_DESC11", "CATEGORY_DESC12", "ACTIVE_FLAG"));

            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);

                if (infos[0].ActiveFlag == "Y")
                {
                    CB_ACTIVE_FLAG.Checked = true;
                }
                for (int i = 1; i <= 12; i++)
                {
                    TextBox txt = (TextBox)lblItemType.FindControl("txt_" + i.ToString());
                    txt.Text=returner.DataSet.Tables[0].Rows[0][i].ToString();
                }

            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
    #region CheckInputted

    bool CheckInputted()
    {
        List<string> errMsgs = new List<string>();

        if (!CB_ACTIVE_FLAG.Checked)
        {
            Returner returner = null;
            try
            {
                ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);

                returner = entityExtItemDetails.GetInfoCount(new ExtItemDetails.InfoConds(null,lbltxtItemType.Text,null,null,null,null,null,true,false));
                if(Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) > 0)
                {
                    errMsgs.Add("產品別「" + lbltxtItemType.Text+ "」包含有效外銷商品料號，不可失效！");

                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
        int a = 0;
        int[] brk = new int[12];

        for (int i = 1; i <= 12; i++)
        {
            TextBox txt = (TextBox)this.Master.FindControl("phHtmlBody").FindControl("txt_" + i.ToString());
            if (txt != null)
            {
                if (!string.IsNullOrWhiteSpace(txt.Text))
                {
                    brk[a] = i;
                    a++;
                }
                else
                {
                    if (i == 1)
                    {
                        errMsgs.Add("請輸入標題內容！");
                        break;
                    }
                }
            }

        }
        if (a > 0)
        {
            if (brk[a - 1] != a)
            {
                errMsgs.Add("請連續輸入標題內容！");
            }
        }

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return false;
        }
        else
            return true;
    }
    #endregion
  

}