using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Transactions;
using System.IO;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class ext_order_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_ORDER";
    string AUTH_NAME = "外銷訂單";

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
        this.observerPaging.Register(this.firstPaging, this.prevPaging, this.numberPaging, this.nextPaging, this.lastPaging, this.textboxPaging);

        this.PageTitle = "外銷訂單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>外銷訂單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                //狀態
                this.lstStatusList.Items.AddRange(ExtOrderHelper.GetExtOrderStatusItems(true));
                WebUtilBox.SetListControlSelected(Request.QueryString["status"], this.lstStatusList);

                //外銷組權限
                if (!this.MainPage.ActorInfoSet.CheckExtAuditPerms(1))
                {
                    this.btnUpdateErpInv.Visible = false;
                    this.btnUpdateErpCuster.Visible = false;
                }
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

        var scriptManager = ScriptManager.GetCurrent(this);
        if (!scriptManager.IsInAsyncPostBack)
        {
            this.PageList();
        }

        //this.WebPg.AdditionalClickToDisableScript(this.btnDelete);

        //權限操作檢查
        //this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }

    protected void btnUpdateErpInv_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //外銷組權限
        if (!this.MainPage.ActorInfoSet.CheckExtAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        var uptCont = Schedule.ImportErpInv(false);

        if (uptCont.HasValue)
        {
            if (uptCont == 0)
            {
                JSBuilder.AlertMessage(this, "沒有任何更新");
            }
            else
            {
                JSBuilder.AlertMessage(this, string.Format("共成功更新 {0} 筆", uptCont));
            }
        }
        else
        {
            JSBuilder.AlertMessage(this, "資料更新時發生錯誤！請洽系統管理員。");
        }
    }

    protected void btnUpdateErpCuster_Click(object sender, EventArgs e)
    {
        var uptCont = Schedule.ImportErpCuster(false);

        //外銷組權限
        if (!this.MainPage.ActorInfoSet.CheckExtAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        if (uptCont.HasValue)
        {
            if (uptCont == 0)
            {
                JSBuilder.AlertMessage(this, "沒有任何更新");
            }
            else
            {
                JSBuilder.AlertMessage(this, string.Format("共成功更新 {0} 筆", uptCont));
            }
        }
        else
        {
            JSBuilder.AlertMessage(this, "資料更新時發生錯誤！請洽系統管理員。");
        }
    }

    #region 取得查詢條件
    #region 查詢條件
    class SearchConds
    {
        public SearchConds()
        {
            this.KeywordCols = new List<string>();
        }

        public ExtOrder.InfoViewConds Conds { get; set; }
        public List<string> KeywordCols { get; set; }
        public string Keyword { get; set; }
    }
    #endregion

    SearchConds GetSearchConds()
    {
        SearchConds conds;
        if (string.IsNullOrWhiteSpace(this.hidSearchConds.Value))
        {
            conds = new SearchConds();

            conds.Keyword = this.txtKeyword.Text;
            foreach (ListItem item in this.chklKeywordCols.Items)
            {
                if (item.Selected)
                {
                    conds.KeywordCols.Add(item.Value);
                }
            }

            var statuses = new int[0];
            bool? isCancel = false;
            bool? isReadjust = false;

            //1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程
            switch (ConvertLib.ToInt(this.lstStatusList.SelectedValue, DefVal.Int))
            {
                case 200:
                    isReadjust = true;
                    break;
                case 999:
                    isCancel = true;
                    break;
                case null:
                    isCancel = DefVal.Bool;
                    isReadjust = DefVal.Bool;
                    break;
                default:
                    statuses = ConvertLib.ToInts(this.lstStatusList.SelectedValue);
                    break;
            }

            var seledStatus = ConvertLib.ToInt(this.lstStatusList.SelectedValue, DefVal.Int);

            conds.Conds = new ExtOrder.InfoViewConds
                (
                   DefVal.SIds,
                   DefVal.SIds,
                   DefVal.SId,
                   DefVal.Int,
                   true,
                   statuses,
                   DefVal.Long,
                   DefVal.DT,
                   DefVal.DT,
                   ConvertLib.ToDateTime(this.txtBeginQuotnDate.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndQuotnDate.Text, DefVal.DT),
                   DefVal.DT,
                   DefVal.DT,
                   ConvertLib.ToDateTime(this.txtBeginEdd.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndEdd.Text, DefVal.DT),
                   isCancel,
                   isReadjust
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            if (conds.Conds.Statuses != null && conds.Conds.Statuses.Length > 0)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Statuses[0].ToString(), this.lstStatusList);
            }
            else if (conds.Conds.IsCancel == true)
            {
                WebUtilBox.SetListControlSelected("999", this.lstStatusList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstStatusList);
            }

            this.txtBeginQuotnDate.Text = ConvertLib.ToStr(conds.Conds.BeginQuotnDate, string.Empty, "yyyy-MM-dd");
            this.txtEndQuotnDate.Text = ConvertLib.ToStr(conds.Conds.EndQuotnDate, string.Empty, "yyyy-MM-dd");
            this.txtBeginEdd.Text = ConvertLib.ToStr(conds.Conds.BeginEdd, string.Empty, "yyyy-MM-dd");
            this.txtEndEdd.Text = ConvertLib.ToStr(conds.Conds.EndEdd, string.Empty, "yyyy-MM-dd");

            this.txtKeyword.Text = conds.Keyword;
            WebUtilBox.SetListControlSelected(conds.KeywordCols.ToArray(), this.chklKeywordCols);
            #endregion
        }

        return conds;
    }
    #endregion

    #region 列示頁面的主要資料
    /// <summary> 
    /// 列示頁面的主要資料。 
    /// </summary> 
    void PageList()
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
            this.phPageList.Controls.Add(htmlTable);

            htmlTr = new TableRow();
            htmlTable.Rows.Add(htmlTr);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "序號";
            htmlTr.Cells.Add(htmlTh);

            //if (this.FunctionRight.Maintain)
            //{
            //    htmlTh = new TableHeaderCell();
            //    htmlTh.Text = "選擇";
            //    htmlTr.Cells.Add(htmlTh);
            //}

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "訂單編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "訂單日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "預計交貨日";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "訂單狀態";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "修改次數";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "修改歷程";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "總金額";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "幣別";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.OnlyNotMarkDeleted;

            var conds = this.GetSearchConds();

            returner = entityExtOrder.GetInfoViewByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

            //分頁初始
            PagingFlipper flipper = new PagingFlipper()
            {
                Size = Convert.ToInt32(this.listCountOfPage.SelectedValue),
                Total = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0])
            };

            this.observerPaging.TotalPageCount = this.observerPaging.CalculateTotalPageCount(flipper.Size, flipper.Total);
            this.observerPaging.PagingInit();

            if (this.DoSearch)
            {
                this.observerPaging.CurrentPageNumber = 1;
            }

            this.observerPaging.PagingActing();
            this.phPaging.Visible = this.observerPaging.TotalPageCount > 1;

            flipper.Page = this.observerPaging.CurrentPageNumber;
            if (flipper.Total == 0)
            {
                //this.litCurrentPageNumber.Text = "0";
                this.litTotalPageNumber.Text = "0";
                this.litSearchResulted.Text = "0";
            }
            else
            {
                //this.litCurrentPageNumber.Text = this.observerPaging.CurrentPageNumber.ToString();
                this.litTotalPageNumber.Text = this.observerPaging.TotalPageCount.ToString();
                this.litSearchResulted.Text = flipper.Total.ToString();
            }

            SqlOrder sorting = SqlOrder.Default;
            returner = entityExtOrder.GetInfoViewByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            //this.btnDelete.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            //this.btnDelete.Attributes["onclick"] = "javascript:if(!window.confirm('確定要刪除？')){return false;}";

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("edit.aspx", UriKind.Relative);

            QueryStringParsing queryTransHx = new QueryStringParsing();
            queryTransHx.HttpPath = new Uri("../../popup/ext/order/trans_hx/index.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtOrder.InfoView.Binding(returner.DataSet.Tables[0]);

                int seqNo = this.observerPaging.CurrentPageNumber * flipper.Size - flipper.Size + 1;
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.OdrNo);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "no";
                    htmlTd.Text = string.Format("{0}", seqNo);
                    htmlTr.Cells.Add(htmlTd);

                    query.Add("quotnSId", info.ExtQuotnSId.Value);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='{0}' title='{1}'>{1}</a>", query, info.OdrNo);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='20' title='{0}'>{0}</span>", info.CustomerName);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.QuotnDate, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.Edd, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ExtOrderHelper.GetExtOrderStatusName(info.Status, info.IsReadjust, info.IsCancel);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = (info.Version - 1).ToString();
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "course";
                    if (info.Version == 1)
                    {
                        htmlTd.Text = "<span></span>";
                    }
                    else
                    {
                        queryTransHx.Add("quotnSId", info.ExtQuotnSId.Value);

                        htmlTd.Text = string.Format("<a href='javascript:;' onclick=\"window.open('{0}', 'trans_hx', 'width=900, height=700, top=100, left=100', scrollbars=1);\">修改歷程</a>", queryTransHx);
                    }
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    if (info.TotalAmt.HasValue)
                    {
                        htmlTd.Text = ConvertLib.ToAccounting(info.TotalAmt.Value);
                    }
                    else
                    {
                        htmlTd.Text = string.Empty;
                    }
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.CurrencyCode;
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
}