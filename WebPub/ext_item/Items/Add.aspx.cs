using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Drawing;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;
using System.Web.UI.HtmlControls;


public partial class ext_item_Items_Add : System.Web.UI.Page
{

    #region 網頁屬性
    string AUTH_CODE = "EXT_ITEMS";
    string AUTH_NAME = "外銷商品料號";
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
        this.PageTitle = "外銷商品料號：新增";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>外銷商品料號</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
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

    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString);
    SqlConnection conn_add = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString);
    SqlConnection connInit = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString);
    SqlConnection conn_update_check = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString);
    SqlConnection conn_ERP_check = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString);

    int count_type;

    string CBActiveFlag;
    string[] category = new string[12];

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查

        if (this.FunctionRight.Maintain)
        {
            if (this.lstItemType.SelectedValue != "")
            { this.btnSave.Visible = true; }

        }
        //
        if (!this.IsPostBack)
        {
            //取得產品別分類，加入下拉選單
            ExtItemTypeList();
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)//頁面一進來做這
        {
        }
        else
        {
            ExtItemList();
        }
    }
    #region 取得產品別下拉選單資料

    void ExtItemTypeList()
    {
        Returner returner = null;
        try
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(true);
            foreach (var info in extItemCatConds.ExtItemTypes)
            {
                this.lstItemType.Items.Add(new ListItem(info.ExportItemType, info.ExportItemType));
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 產品別切換
    protected void lstItemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seled = this.lstItemType.SelectedValue;

        if (!string.IsNullOrWhiteSpace(seled))
        {
            #region 重置欄位
            txtExtItem.Text = "";
            txtErpItem.Text = "";
            lblErpItemId.Text = "";
            #endregion
        }
    }
    #endregion

    protected void alert(string Msg)
    {
        ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "系統訊息", "alert('" + Msg + "');", true);
    }

    #region 列示頁面的主要資料-HtmlTable
    /// <summary> 
    /// 列示頁面的主要資料。 
    /// </summary> 
    /// 
    void ExtItemList()
    {
        Returner returner = null;

        try
        {
            if (!this.HasInitial) { return; }

            ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);

            string[] conds = new string[1] { lstItemType.SelectedValue };

            returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(conds, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5", "CATEGORY_DESC6", "CATEGORY_DESC7", "CATEGORY_DESC8", "CATEGORY_DESC9", "CATEGORY_DESC10", "CATEGORY_DESC11", "CATEGORY_DESC12", "ACTIVE_FLAG"));
            var infos = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);
            //count_type 計算這個產品分類有幾個標題
            for (int i = 1; i <= 12; i++)
            {
                if (!string.IsNullOrWhiteSpace(returner.DataSet.Tables[0].Rows[0][i].ToString()))
                {
                    count_type++;
                }
            }
            #region 列示頁面的主要資料
            /// <summary> 
            /// 列示頁面的主要資料。 
            /// </summary> 
            HtmlTableRow htmlTr = null;
            HtmlTableCell htmlTh;
            HtmlTableCell htmlTd;

            #region 標題
            HtmlTable htmlTable = new HtmlTable();

            this.phPageList.Controls.Add(htmlTable);

            htmlTable.ID = "ItemTable";
            htmlTable.CellPadding = 0;
            htmlTable.CellSpacing = 0;
            htmlTable.Align = "center";
            htmlTable.Attributes.Add("class", "ListTable");
            htmlTable.Style["width"] = "50%";
            htmlTr = new HtmlTableRow(); //產生第一個Row
            htmlTable.Rows.Add(htmlTr);

            htmlTh = new HtmlTableCell("th");//產生TH
            htmlTh.InnerHtml = "分類標題名稱";
            htmlTh.Align = "center";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new HtmlTableCell("th");//產生TH
            htmlTh.InnerHtml = "分類內容";
            htmlTh.Align = "center";
            htmlTh.Style["width"] = "60%";
            htmlTr.Cells.Add(htmlTh);

            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }
            #region 表格內容

            for (int i = 1; i <= count_type; i++)
            {
                htmlTr = new HtmlTableRow();
                htmlTable.Rows.Add(htmlTr);

                htmlTd = new HtmlTableCell();//產生TD
                htmlTd.InnerHtml = returner.DataSet.Tables[0].Rows[0][i].ToString(); //抓取產品別標題
                htmlTr.Cells.Add(htmlTd);

                htmlTd = new HtmlTableCell();//產生TD
                htmlTd.ID = "Txt_" + i;

                TextBox txt = new TextBox();
                txt.ID = "Textbox_" + i;
                txt.Style["width"] = "60%";
                txt.Text = "";
                htmlTr.Cells.Add(htmlTd);
                htmlTd.Controls.Add(txt);
            }
        }
        #endregion
        #endregion

        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
    protected void btnSave_Click(object sender, EventArgs e)
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

                ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);
                for (Int32 i = 1; i <= 12; i++)
                {
                    HtmlTableCell cell = (HtmlTableCell)phPageList.FindControl("Txt_" + i.ToString());

                    if (cell != null)

                    {
                        TextBox txtbox = (TextBox)cell.FindControl("Textbox_" + i.ToString());

                        if (txtbox != null)
                        {
                            if (!string.IsNullOrWhiteSpace(txtbox.Text))
                            { category[i - 1] = txtbox.Text.Trim(); }
                            else
                            { category[i - 1] = null; }
                        }
                    }
                    else
                    {
                        category[i - 1] = null;
                    }
                }

                if (CB_ACTIVE_FLAG.Checked)
                { CBActiveFlag = "Y"; }
                else
                { CBActiveFlag = "N"; }
                if (!string.IsNullOrWhiteSpace(lblErpItemId.Text))
                {
                    returner = entityExtItemDetails.Add(txtExtItem.Text.Trim(), Int32.Parse(lblErpItemId.Text), txtErpItem.Text.Trim(), lstItemType.SelectedValue, category[0], category[1], category[2], category[3], category[4], category[5], category[6], category[7], category[8], category[9], category[10], category[11], null, null, null, CBActiveFlag, DateTime.Now, actorSId, DateTime.Now, actorSId);
                }
                else
                {
                    returner = entityExtItemDetails.Add(txtExtItem.Text.Trim(), null, null, lstItemType.SelectedValue, category[0], category[1], category[2], category[3], category[4], category[5], category[6], category[7], category[8], category[9], category[10], category[11], null, null, null, CBActiveFlag, DateTime.Now, actorSId, DateTime.Now, actorSId);

                }
                //回到前一頁
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('外銷商品料號新增成功!!'); location.href='View.aspx';", true);

            }
            finally
            {
                if (returner != null) returner.Dispose();
            }

        }
    }

    #region CheckInputted

    bool CheckInputted()
    {
        List<string> errMsgs = new List<string>();

        //1.check外銷型號
        if (string.IsNullOrWhiteSpace(txtExtItem.Text.Trim()))
        {

            errMsgs.Add("請輸入外銷型號！");
        }
        else
        {
            if (!ExtItemCheck("export"))
            {
                errMsgs.Add("新增輸入之外銷型號「" + txtExtItem.Text + "」已重複！");
            }
            if(txtExtItem.Text.Trim().Length>100)
            {
                errMsgs.Add("新增輸入之外銷型號不可超過100個字元！");
            }
        }
        //2.check ERP料號
        if (!string.IsNullOrWhiteSpace(txtErpItem.Text.Trim().ToUpper()))
        {
            Returner returner = null;
            try
            {
                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                string[] conds = new string[1] { txtErpItem.Text.Trim().ToUpper() };
                returner = entityErpInv.GetInfo(new ErpInv.InfoConds(null, null, conds, null, null, null, null), Int32.MaxValue, SqlOrder.Default, IncludeScope.All, ConvertLib.ToStrs("Inventory_Item_Id", "Item"));
                var erpinfo = ErpInv.Info.Binding(returner.DataSet.Tables[0]);
                if (returner.DataSet.Tables[0].Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(erpinfo[0].Item))
                    {
                        if (!ExtItemCheck("erp"))
                        {
                            errMsgs.Add("新增輸入之ERP料號「" + txtErpItem.Text.Trim().ToUpper() + "」已存在於其他外銷型號！");
                        }
                        else
                        {
                            lblErpItemId.Text = erpinfo[0].InventoryItemId.ToString();
                        }
                    }
                }
                else
                {
                    errMsgs.Add("請輸入有效之ERP料號！");
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
        //3.CHECK 輸入的分類內容，至少應輸入第一個分類
        HtmlTableCell cell = (HtmlTableCell)phPageList.FindControl("Txt_1");

        if (cell != null)

        {
            TextBox txtbox = (TextBox)cell.FindControl("Textbox_1");

            if (txtbox != null)
            {
                if (string.IsNullOrWhiteSpace(txtbox.Text.Trim()))
                {

                    errMsgs.Add("請輸入分類內容！");
                }
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

    bool ExtItemCheck(string checkType)
    {
        Returner returner = null;
        try
        {
            ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);

            switch (checkType)
            {
                case "erp":
                    returner = entityExtItemDetails.GetErpInfo(this.txtErpItem.Text.Trim().ToUpper());
                    var infos = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);
                    if (returner.DataSet.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(infos[0].ErpItem) && infos[0].ExportItem != this.txtExtItem.Text.Trim())
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                case "export":
                    string[] conds = new string[1] { txtExtItem.Text.Trim() };
                    returner = entityExtItemDetails.GetInfo(new ExtItemDetails.InfoConds(conds, null, null, null, null, null, null, null, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM", "ERP_ITEM", "EXPORT_ITEM_TYPE", "CATEGORY1", "CATEGORY2", "CATEGORY3", "CATEGORY4", "CATEGORY5", "CATEGORY6", "CATEGORY7", "CATEGORY8", "CATEGORY9", "CATEGORY10", "CATEGORY11", "CATEGORY12", "ACTIVE_FLAG"));
                    var info = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);
                    if (returner.DataSet.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(info[0].ExportItem))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return true;
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }


}