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

public partial class ext_item_Item_Type_View : System.Web.UI.Page
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
    /// <summary>
    /// 外銷商品產品別及分類說明陣列集合。
    /// </summary>
    public ExtItemType.Info[] ExtItemTypes { get; set; }
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
        this.PageTitle = "外銷商品產品別";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>外銷商品產品別</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        //範例: 我是登入人的帳號
        var acct = this.MainPage.ActorInfoSet.Info.Acct;
       
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           
        }
        QueryStringParsing query = new QueryStringParsing();
        query.HttpPath = new Uri("Add.aspx", UriKind.Relative);
        this.lnkAdd.NavigateUrl = query.ToString();
       
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查
        this.lnkAdd.Visible = this.lnkAdd.Visible ? this.FunctionRight.Maintain : this.lnkAdd.Visible;
     //權限控制範例 
     //   this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
     //   this.btnUpdateSort.Visible = this.btnUpdateSort.Visible ? this.FunctionRight.Maintain : this.btnUpdateSort.Visible;

        var scriptManager = ScriptManager.GetCurrent(this);
        if (!scriptManager.IsInAsyncPostBack)
        {
            ExtItemTypeList();
        }
    }

    protected void btn_Prepage_Click(object sender, EventArgs e)
    {
        Response.Redirect("View.aspx");
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
            TableRow htmlTr = null;
            TableHeaderCell htmlTh;
            TableCell htmlTd;

            #region 標題
            Table htmlTable = new Table();
            htmlTable.CellPadding = 0;
            htmlTable.CellSpacing = 0;
            htmlTable.CssClass = "ListTable";
            htmlTable.Width = Unit.Percentage(60);
            htmlTable.HorizontalAlign = HorizontalAlign.Center;
            this.phPageList.Controls.Add(htmlTable);

            htmlTr = new TableRow();
            htmlTable.Rows.Add(htmlTr);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "序號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "產品別";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "有效註記";
            htmlTr.Cells.Add(htmlTh);

            #endregion
            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("Edit.aspx", UriKind.Relative);

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);

            returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(DefVal.Strs, null), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5", "CATEGORY_DESC6", "CATEGORY_DESC7", "CATEGORY_DESC8", "CATEGORY_DESC9", "CATEGORY_DESC10", "CATEGORY_DESC11", "CATEGORY_DESC12","ACTIVE_FLAG"));


            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);

                //
                int seqNo = 1;

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];
                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "no";
                    htmlTd.Text = string.Format("{0}", seqNo);

                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    query.Add("ItemType", info.ExportItemType);
                    htmlTd.Text = string.Format("<a href='{0}' title='{1}'>{1}</a>", query, info.ExportItemType);
                    htmlTr.Cells.Add(htmlTd);


                    htmlTd = new TableCell();
                    if (info.ActiveFlag == "Y")
                    {
                        htmlTd.Text = string.Format("<input type='checkbox' name='valid[]' value='{0}' checked='checked'  />", info.ActiveFlag);
                    }
                    else
                    {
                        htmlTd.Text = string.Format("<input type='checkbox' name='valid[]' value='{0}'  />", info.ActiveFlag);
                    }
                    htmlTr.Cells.Add(htmlTd);


                    seqNo++;

                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region 原組html的method by儀淳，改用魁亨的patten
    //private void datatable(DataTable dt)
    //{
    //    Label2.Text = "<form action='HTML.aspx' style='width: 80 % '>";
    //    Label2.Text += "<div id='Table2' align=center style='width: 80 % '>";
    //    Label2.Text += "<table class='ListTable' cellspacing='0' cellpadding='0' style='border - collapse:collapse;width: 80%;' >";

    //    //add header row
    //    Label2.Text += "<tr>";
    //    for (int i = 0; i < dt.Columns.Count; i++)
    //        Label2.Text += "<td>" + dt.Columns[i].ColumnName + "</td>";
    //    Label2.Text += "</tr>";

    //    if (dt.Rows.Count == 1)
    //    {
    //        Label2.Text += "<tr>";
    //        for (int j = 0; j < dt.Columns.Count; j++)
    //        {
    //            if (j == 1)
    //            {
    //                Label2.Text += "<td  style='text - align: left'><a style='text - align: left' href='Edit.aspx?ItemType=" + dt.Rows[0][j].ToString() + "' style='display: inline - block; margin: 5px; width: 100px; cursor: pointer; text-align: center; text-decoration: underline; color: #0000FF'>" + dt.Rows[0][j].ToString() + " </ a></td>";
    //            }
    //            else if(j == 2)
    //            {
    //                if (dt.Rows[0][2].ToString() == "Y")
    //                {
    //                    Label2.Text += "<td><input type='checkbox' checked=true/></td>";
    //                }
    //                else
    //                    Label2.Text += "<td><input type='checkbox' checked=false/></td>";
    //            }
    //            else
    //                Label2.Text += "<td>" + dt.Rows[0][j].ToString() + "</td>";
    //        }
    //        Label2.Text += "</tr>";
    //    }
    //    else
    //    {
    //        //add rows
    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            Label2.Text += "<tr>";
    //            for (int j = 0; j < dt.Columns.Count; j++)
    //            {
    //                if (j == 1)
    //                    Label2.Text += "<td style='text - align: left'><a style='text - align: left' href='Edit.aspx?ItemType=" + dt.Rows[i][j].ToString() + "' style='display: inline - block; margin: 5px; width: 100px; cursor: pointer; text-align: center; text-decoration: underline; color: #0000FF'>" + dt.Rows[i][j].ToString() + " </ a></td>";
    //                else if (j == 2)
    //                {
    //                    if (dt.Rows[i][2].ToString() == "Y")
    //                    {
    //                        Label2.Text += "<td><input type='checkbox' checked=true/></td>";
    //                    }
    //                    else if (dt.Rows[i][2].ToString() == "N")
    //                    {
    //                        Label2.Text += "<td><input type='checkbox' /></td>";
    //                    }
    //                }
    //                else
    //                    Label2.Text += "<td>" + dt.Rows[i][j].ToString() + "</td>";
    //            }
    //            Label2.Text += "</tr>";
    //        }
    //    }

    //    Label2.Text += "</table>";
    //    Label2.Text += "</div>";
    //    Label2.Text += "</form>";
    //}
    #endregion
}