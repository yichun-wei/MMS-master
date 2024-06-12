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

public partial class ext_item_Items_Edit : System.Web.UI.Page
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
        this.PageTitle = "外銷商品料號：維護";

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
    string ExtItem, ItemType, CBActiveFlag;

    int count_type;

    string[] category = new string[12];
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查

        if (this.FunctionRight.Maintain)
        { this.btnSave.Visible = true; }

        //  this.btnSave.Visible = this.btnSave.Visible ? this.FunctionRight.Maintain : this.btnSave.Visible;
        //權限控制範例 
        //   this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
        //   this.btnUpdateSort.Visible = this.btnUpdateSort.Visible ? this.FunctionRight.Maintain : this.btnUpdateSort.Visible;

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
        ExtItem = Request.QueryString["ExtItem"];
        lbltxtextItem.Text = ExtItem;

        ExtItemList();
    }

    #region 列示頁面的主要資料-HtmlTable
    /// <summary> 
    /// 列示頁面的主要資料。 
    /// </summary> 
    /// 
    void ExtItemList()
    {
        Returner returner = null;
        Returner returner_dtl = null;

        try
        {
            if (!this.HasInitial) { return; }

            ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);
            ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);


            string[] conds = new string[1] { lbltxtextItem.Text };
            returner_dtl = entityExtItemDetails.GetInfo(new ExtItemDetails.InfoConds(conds, null, null, null, null, null, null, null, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM", "ERP_ITEM", "EXPORT_ITEM_TYPE", "CATEGORY1", "CATEGORY2", "CATEGORY3", "CATEGORY4", "CATEGORY5", "CATEGORY6", "CATEGORY7", "CATEGORY8", "CATEGORY9", "CATEGORY10", "CATEGORY11", "CATEGORY12", "ACTIVE_FLAG"));
            var infos = ExtItemDetails.Info.Binding(returner_dtl.DataSet.Tables[0]);

            string[] typeconds = new string[1] { infos[0].ExportItemType };
            returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(typeconds, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5", "CATEGORY_DESC6", "CATEGORY_DESC7", "CATEGORY_DESC8", "CATEGORY_DESC9", "CATEGORY_DESC10", "CATEGORY_DESC11", "CATEGORY_DESC12", "ACTIVE_FLAG"));
            var typeinfos = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);
            //count_type 計算這個產品分類有幾個標題
            for (int i = 1; i < 13; i++)
            {
                if (!string.IsNullOrWhiteSpace(returner.DataSet.Tables[0].Rows[0][i].ToString()))

                {
                    count_type++;
                }

            }

            //   
            #region 產品別欄位顯示
            if (!IsPostBack)
            {
                lbltxtItemType.Text = infos[0].ExportItemType;

                if (infos[0].ActiveFlag == "Y")
                {
                    CB_ACTIVE_FLAG.Checked = true;
                }
                else
                {
                    CB_ACTIVE_FLAG.Checked = false;
                }

                if (!string.IsNullOrWhiteSpace(infos[0].ErpItem))
                {
                    txtErpItem.Text = infos[0].ErpItem;
                    lblErpChanged.Text = infos[0].ErpItem;
                }
            }
            #endregion

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
                txt.Text = returner_dtl.DataSet.Tables[0].Rows[0][i + 2].ToString();
                htmlTr.Cells.Add(htmlTd);
                htmlTd.Controls.Add(txt);

            }



            #endregion
            #endregion
            //
        }

        finally
        {
            if (returner != null) returner.Dispose();
            if (returner_dtl != null) returner_dtl.Dispose();
        }
        #region 原HTML寫法
        //
        //conn.Open();
        //        string cmd = "SELECT * ";
        //        cmd += "FROM EXT_ITEM_DETAILS d, EXT_ITEM_TYPE t ";
        //        cmd += "WHERE d.EXPORT_ITEM_TYPE=t.EXPORT_ITEM_TYPE ";
        //        cmd += "AND d.EXPORT_ITEM=@EXPORT_ITEM ";
        //        SqlCommand MS_cmd = new SqlCommand(cmd, conn);
        //        MS_cmd.Parameters.Add("@EXPORT_ITEM", SqlDbType.NVarChar).Value = lbltxtextItem.Text;

        //        SqlDataReader MS_dr = MS_cmd.ExecuteReader();

        //        if (MS_dr.HasRows)
        //        {
        //            while (MS_dr.Read())
        //            {
        //                if (!IsPostBack)
        //               {
        //                    lbltxtItemType.Text = MS_dr["EXPORT_ITEM_TYPE"].ToString();

        //                    if (MS_dr["ACTIVE_FLAG"].ToString() == "Y")
        //                {
        //                    CB_ACTIVE_FLAG.Checked = true;
        //                }

        //                if (MS_dr["ERP_ITEM"].ToString().Length > 0)
        //                {
        //                    txtErpItem.Text = MS_dr["ERP_ITEM"].ToString();
        //                    lblErpChanged.Text = txtErpItem.Text;
        //                }
        //              }

        //                //count 計算這個產品分類有幾個標題
        //                for (int i = 1; i < 13; i++)
        //                {
        //                    if (MS_dr["CATEGORY_DESC" + i + ""].ToString().Length > 0)
        //                    {
        //                        count++;
        //                    }

        //                }

        //                if (count > 0)
        //                {
        //                    lblTxtCount.Text = count.ToString();

        //                }



        //                #region 列示頁面的主要資料
        //                /// <summary> 
        //                /// 列示頁面的主要資料。 
        //                /// </summary> 
        //                HtmlTableRow htmlTr = null;
        //                HtmlTableCell htmlTh;
        //                HtmlTableCell htmlTd;


        //                #region 標題
        //                HtmlTable htmlTable = new HtmlTable();

        //                this.phPageList.Controls.Add(htmlTable);

        //                htmlTable.ID = "ItemTable";
        //                htmlTable.CellPadding = 0;
        //                htmlTable.CellSpacing = 0;
        //                htmlTable.Align = "center";
        //                htmlTable.Attributes.Add("class", "ListTable");
        //                htmlTable.Style["width"] = "50%";
        //                htmlTr = new HtmlTableRow(); //產生第一個Row
        //                htmlTable.Rows.Add(htmlTr);

        //                htmlTh = new HtmlTableCell("th");//產生TH
        //                htmlTh.InnerHtml = "標題序號";
        //                htmlTh.Align = "center";
        //                htmlTr.Cells.Add(htmlTh);

        //                htmlTh = new HtmlTableCell("th");//產生TH
        //                htmlTh.InnerHtml = "分類標題名稱";
        //                htmlTh.Align = "center";
        //                htmlTh.Style["width"] = "50%";
        //                htmlTr.Cells.Add(htmlTh);

        //                #endregion

        //                //網頁未完成初始時不執行
        //                if (!this.HasInitial) { return; }
        //                #region 表格內容
        //                if (!MS_dr.HasRows)
        //                {

        //                }
        //                else
        //                {
        //                    for (int i = 1; i <= count; i++)
        //                    {
        //                        htmlTr = new HtmlTableRow();
        //                        htmlTable.Rows.Add(htmlTr);

        //                        htmlTd = new HtmlTableCell();//產生TD
        //                        htmlTd.InnerHtml = MS_dr["CATEGORY_DESC" + i + ""].ToString(); //抓取產品別標題
        //                        htmlTr.Cells.Add(htmlTd);

        //                        htmlTd = new HtmlTableCell();//產生TD
        //                        htmlTd.ID = "Txt_" + i;

        //                        TextBox txt = new TextBox();
        //                        txt.ID = "Textbox_" + i;
        //                        txt.Style["width"] = "60%";
        //                        txt.Text = MS_dr["CATEGORY" + i + ""].ToString();
        //                        htmlTr.Cells.Add(htmlTd);
        //                        htmlTd.Controls.Add(txt);

        //                    }

        //                }

        //                #endregion
        //                #endregion


        //            }

        //        }
        //        MS_cmd.Dispose();
        //        conn.Close();
        //        conn.Dispose();
        #endregion
    }


    #endregion

    //protected void ErpItemCheck(out bool ITEM)
    //{
    //    ITEM = false;

    //    if (txtErpItem.Text.Trim() != "")
    //    {
    //        conn_update_check.Open();
    //        string sqlselect = "select ITEM From ERP_INV ";
    //        sqlselect += "Where ITEM=@ITEM ";

    //        SqlCommand cmd_select = new SqlCommand(sqlselect, conn_update_check);

    //        cmd_select.Parameters.Add("@ITEM", SqlDbType.Char).Value = txtErpItem.Text.TrimStart().TrimEnd();
    //        SqlDataReader MS_dr = cmd_select.ExecuteReader();
    //        if (MS_dr.HasRows)
    //        {
    //            ITEM = true;
    //            MS_dr.Close();
    //        }
    //        conn_update_check.Close();
    //    }
    //    else
    //    {
    //        ITEM = true;
    //        txtErpItem.Text = "";
    //    }

    //    if (ITEM == true)
    //    {
    //        if (txtErpItem.Text.Trim().Length > 0)
    //        {
    //            conn_ERP_check.Open();
    //            string ERP_select = "SELECT * ";
    //            ERP_select += "FROM EXT_ITEM_DETAILS ";
    //            ERP_select += "WHERE ERP_ITEM=@ERP_ITEM AND EXPORT_ITEM !=@EXPORT_ITEM";
    //            SqlCommand ERP_cmd = new SqlCommand(ERP_select, conn_ERP_check);
    //            ERP_cmd.Parameters.Add("@EXPORT_ITEM", SqlDbType.NVarChar).Value = lbltxtextItem.Text.TrimStart().TrimEnd();
    //            ERP_cmd.Parameters.Add("@ERP_ITEM", SqlDbType.NVarChar).Value = txtErpItem.Text.TrimStart().TrimEnd();
    //            SqlDataReader ERP_dr = ERP_cmd.ExecuteReader();
    //            if (!ERP_dr.HasRows)
    //            {
    //                ITEM = true;
    //            }
    //            else
    //            {
    //                JSBuilder.AlertMessage(this, "料號重覆！外銷商品料號中已有「" + txtErpItem.Text + "」ERP料號");
    //                ITEM = false;
    //                txtErpItem.Focus();
    //            }
    //            ERP_cmd.Cancel();
    //            conn_ERP_check.Close();
    //        }
    //        else
    //        {
    //            ITEM = true;
    //            txtErpItem.Text = "";
    //        }

    //    }
    //    else
    //    {
    //        JSBuilder.AlertMessage(this, "您輸入的ERP料號「" + txtErpItem.Text + "」無效");
    //        ITEM = false;
    //        txtErpItem.Focus();
    //    }

    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //
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
                    returner = entityExtItemDetails.Modify(lbltxtextItem.Text, Int32.Parse(lblErpItemId.Text), txtErpItem.Text.Trim().ToUpper(), lbltxtItemType.Text, category[0], category[1], category[2], category[3], category[4], category[5], category[6], category[7], category[8], category[9], category[10], category[11], null, null, null, CBActiveFlag, DateTime.Now, actorSId);
                }
                else
                {
                    returner = entityExtItemDetails.Modify(lbltxtextItem.Text, null, null, lbltxtItemType.Text, category[0], category[1], category[2], category[3], category[4], category[5], category[6], category[7], category[8], category[9], category[10], category[11], null, null, null, CBActiveFlag, DateTime.Now, actorSId);

                }
                //回到前一頁
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('外銷商品料號修改成功!!'); location.href='View.aspx';", true);

            }
            finally
            {
                if (returner != null) returner.Dispose();
            }

        }
        #region 原save寫法

        //bool ItemValid = false;

        //if (lblErpChanged.Text == txtErpItem.Text )
        //{ ItemValid = true; }
        //else
        //{ ErpItemCheck(out ItemValid); }


        //if (ItemValid)
        //{
        //    conn_update.Open();
        //    string txt_string;

        //    if (lblTxtCount.Text != "")
        //    {
        //        count_d = int.Parse(lblTxtCount.Text);
        //    }
        //    string sqlupdate = "UPDATE EXT_ITEM_DETAILS SET ";
        //    for (int i = 1; i <= count_d; i++)
        //    {
        //        if (i == 1)
        //            sqlupdate += "CATEGORY" + i + "=@CATEGORY" + i;
        //        else
        //            sqlupdate += " ,CATEGORY" + i + "=@CATEGORY" + i;
        //    }
        //    sqlupdate += "  ,ERP_ITEM=@ERP_ITEM ";
        //    sqlupdate += "  ,ACTIVE_FLAG=@ACTIVE_FLAG";
        //    sqlupdate += " WHERE EXPORT_ITEM=@EXPORT_ITEM ";
        //    SqlCommand update = new SqlCommand(sqlupdate, conn_update);
        //    update.Parameters.Add("@EXPORT_ITEM", SqlDbType.NVarChar).Value = lbltxtextItem.Text;

        //    if (string.IsNullOrWhiteSpace(txtErpItem.Text))
        //    { update.Parameters.Add("@ERP_ITEM", SqlDbType.NVarChar).Value = DBNull.Value; }
        //    else
        //    { update.Parameters.Add("@ERP_ITEM", SqlDbType.NVarChar).Value = txtErpItem.Text; }

        //    for (Int32 Forvar = 1; Forvar <= count_d; Forvar++)
        //    {
        //        HtmlTableCell cell = (HtmlTableCell)phPageList.FindControl("Txt_" + Forvar.ToString());

        //        if (cell != null)

        //        {
        //            TextBox txtbox = (TextBox)cell.FindControl("Textbox_" + Forvar.ToString());

        //            if (txtbox != null)
        //            {
        //                if (string.IsNullOrWhiteSpace(txtbox.Text))
        //                { update.Parameters.Add("@CATEGORY" + Forvar.ToString() + "", SqlDbType.NVarChar).Value = DBNull.Value; }
        //                else
        //                { update.Parameters.Add("@CATEGORY" + Forvar.ToString() + "", SqlDbType.NVarChar).Value = txtbox.Text; }

        //            }
        //        }

        //    }

        //    if (CB_ACTIVE_FLAG.Checked == false)
        //    {
        //        txt_string = "N";
        //    }
        //    else
        //        txt_string = "Y";

        //    update.Parameters.Add("@ACTIVE_FLAG", SqlDbType.Char).Value = txt_string.ToString();
        //    update.Parameters.Add("@LAST_UPDATED_DATE", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    update.Parameters.Add("@LAST_UPDATED_BY", SqlDbType.NVarChar).Value = lblUserSid.Text;

        //    update.ExecuteNonQuery();
        //    conn_update.Close();

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('更新成功!!'); location.href='View.aspx';", true);
        //}
        #endregion
    }


    #region 網路參考資料-不用了
    //Table item = (Table)Page.FindControl("phHtmlBody_ItemTable");
    //int j = 1;
    //string txt = "";
    //foreach (TableRow row in item.Rows)
    //{
    //    foreach (TableCell cell in row.Cells)
    //    {
    //        foreach (Control ctl in cell.Controls)
    //        {
    //            if (ctl.GetType().ToString() == "System.Web.UI.WebControls.TextBox")
    //            {
    //                txt = ctl.ToString();
    //                if (string.IsNullOrWhiteSpace(txt))
    //                { update.Parameters.Add("@CATEGORY" + j.ToString() + "", SqlDbType.NVarChar).Value = DBNull.Value; }
    //                else
    //                { update.Parameters.Add("@CATEGORY" + j.ToString() + "", SqlDbType.NVarChar).Value = txt; }
    //            }
    //        }
    //    }
    //}
    #endregion


    protected void alert(string Msg)
    {
        ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "系統訊息", "alert('" + Msg + "');", true);

    }

    #region CheckInputted

    bool CheckInputted()
    {
        List<string> errMsgs = new List<string>();

        //1.check erp料號欄位
        if (!string.IsNullOrWhiteSpace(txtErpItem.Text) && (txtErpItem.Text != lblErpChanged.Text))
        {
            Returner returner = null;
            try
            {
                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                string[] conds = new string[1] { txtErpItem.Text.Trim().ToUpper() };
                returner = entityErpInv.GetInfo(new ErpInv.InfoConds(null, null, conds, null, null, null, null), Int32.MaxValue, SqlOrder.Default, IncludeScope.All, ConvertLib.ToStrs("Inventory_Item_Id", "Item"));
                var erpinfo = ErpInv.Info.Binding(returner.DataSet.Tables[0]);
               
                    if (returner.DataSet.Tables[0].Rows.Count>0)
                    {

                        if (!ErpItemDup())
                        {
                            errMsgs.Add("輸入之ERP料號「" + txtErpItem.Text.Trim().ToUpper() + "」已存在於其他外銷型號！");
                        }
                        else
                        {
                            lblErpItemId.Text = erpinfo[0].InventoryItemId.ToString();
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
        //2.check 分類內容欄位，至少第一個分類要輸入
        HtmlTableCell cell = (HtmlTableCell)phPageList.FindControl("Txt_1");

        if (cell != null)

        {
            TextBox txtbox = (TextBox)cell.FindControl("Textbox_1");

            if (txtbox != null)
            {
                if (string.IsNullOrWhiteSpace(txtbox.Text))
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

    bool ErpItemDup()
    {

        Returner returner = null;
        try
        {
            ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);

            returner = entityExtItemDetails.GetErpInfo(this.txtErpItem.Text);
            
                var infos = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);
            if (returner.DataSet.Tables[0].Rows.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(infos[0].ErpItem) && infos[0].ExportItem != this.lbltxtextItem.Text)
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
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }

}