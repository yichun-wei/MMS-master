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
using System.Data.SqlClient;
using System.Configuration;

public partial class project_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "PROJ_QUOTE";
    string AUTH_NAME = "CRM專案查詢";

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

        this.PageTitle = "CRM專案查詢";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}project/index.aspx'>報表查詢</a>", SystemDefine.WebSiteRoot));
        breadcrumbs.Add(string.Format("<a href='{0}'>CRM專案查詢</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                #region 地區
                foreach (var domDistInfo in this.MainPage.ActorInfoSet.DomDistInfos)
                {
                    this.lstDomDistList.Items.Add(new ListItem(domDistInfo.Name, domDistInfo.SId.Value));
                }
                #endregion

                #region 狀態
                //[20170509 by 儀淳] CRM專案取消功能新增_查詢條件：未取消/已取消 選單
                //內銷 ERP 訂單狀態
                this.lstProjQuoteCancelStatuList.Items.AddRange(ProjQuoteHelper.GetProjQuoteCancelStatusItems());
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

        var scriptManager = ScriptManager.GetCurrent(this);
        if (!scriptManager.IsInAsyncPostBack)
        {
            this.PageList();
        }
                
        //[20170509 by 儀淳] CRM專案取消功能新增_未取消/已取消資料切換
        if (this.lstProjQuoteCancelStatuList.SelectedValue == "N")
        {
            //權限操作檢查
            this.btnCancel_PROJ_QUOTE.Visible = true;
            this.btnCancel_PROJ_QUOTE.Visible = this.btnCancel_PROJ_QUOTE.Visible ? this.FunctionRight.Maintain : this.btnCancel_PROJ_QUOTE.Visible;
            btnValid_PROJ_QUOTE.Visible = false;
        }
        else if (this.lstProjQuoteCancelStatuList.SelectedValue == "Y")
        {
            btnCancel_PROJ_QUOTE.Visible = false;
            //[20170509 by 儀淳] CRM專案取消功能新增_隱藏回復已取消報價單
            btnValid_PROJ_QUOTE.Visible = false;
        }
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }
    #region CRM專案報價明細狀態更新
    //[20170804 by 儀淳] CRM專案報價明細狀態更新
    protected void btnUpdateCRMQuote_Click(object sender, EventArgs e)
    {
        Returner returner = null;
        try
        {
            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            returner = entityDomOrder.ExecProjQuoteUpdate();
            if (returner.IsCompletedAndContinue)
            {
                JSBuilder.AlertMessage(this, true, "CRM專案報價更新已完成");
            }
            else
            {
                JSBuilder.AlertMessage(this, true, "沒有任何CRM專案報價需要更新");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 取消專案編號
    //[20170509 by 儀淳] CRM專案取消功能新增
    protected void btnCancel_PROJ_QUOTE_Click(object sender, EventArgs e)
    {
        var seledProjQuotes = this.hidSeledProjQuotes.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledProjQuotes.Length == 0)
        {
            this.lstProjQuoteCancelStatuList.SelectedValue = "N";
            return;
        }

        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);
                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now;
                ProjQuote.InfoView[] projQuoteInfos = new ProjQuote.InfoView[0];
                //已選擇的
                foreach (var seled in seledProjQuotes)
                {
                    returner = entityProjQuote.UpdateCancelInfo(seled, true, currentTime);
                }
                transaction.Complete();
                Response.Write("<script>alert('報價單已取消完成');</script>");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 有效專案編號(隱藏)
    //[20170509 by 儀淳] CRM專案取消功能新增__隱藏回復已取消報價單
    protected void btnValid_PROJ_QUOTE_Click(object sender, EventArgs e)
    {
        var seledProjQuotes = this.hidSeledProjQuotes.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledProjQuotes.Length == 0)
        {
            this.lstProjQuoteCancelStatuList.SelectedValue = "Y";
            return;
        }

        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);
                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now;
                ProjQuote.InfoView[] projQuoteInfos = new ProjQuote.InfoView[0];
                //已選擇的
                foreach (var seled in seledProjQuotes)
                {
                    returner = entityProjQuote.UpdateCancelInfo(seled, false, currentTime);
                }
                transaction.Complete();
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion 

    #region 取得查詢條件
    #region 查詢條件
    class SearchConds
    {
        public ProjQuote_dist.InfoConds Conds { get; set; }
    }
    #endregion

    SearchConds GetSearchConds()
    {
        SearchConds conds;
        if (string.IsNullOrWhiteSpace(this.hidSearchConds.Value))
        {
            conds = new SearchConds();

            //系統使用者依所指定的內銷地區範圍設定
            ISystemId[] domDistSIds = DefVal.SIds;
            ISystemId domDistSId = ConvertLib.ToSId(this.lstDomDistList.SelectedValue);
            if (domDistSId == null)
            {
                //若未選擇, 則取該系統使用者所有被指定的內銷地區.
                domDistSIds = this.MainPage.ActorInfoSet.DomDistInfos.Select(q => q.SId).ToArray();
                if (domDistSIds.Length == 0)
                {
                    //若沒被指定的內銷地區, 則直接使其找不到.
                    domDistSIds = ConvertLib.ToSIds(SystemId.MaxValue);
                }
                this.hidIsAllDomDist.Value = "Y";
            }
            else
            {
                domDistSIds = ConvertLib.ToSIds(domDistSId);
                this.hidIsAllDomDist.Value = "N";
            }

            bool isCancel;
            if (this.lstProjQuoteCancelStatuList.SelectedValue == "N")
            {
                isCancel = false;
            }
            else
            {
                isCancel = true;
            }

            conds.Conds = new ProjQuote_dist.InfoConds
                    (                                             
                       ConvertLib.ToDateTime(this.txtBeginEdd.Text, DefVal.DT),
                       ConvertLib.ToDateTime(this.txtEndEdd.Text, DefVal.DT),                       
                       ConvertLib.ToStrs(domDistSIds),
                       ConvertLib.ToStrs(this.txtKeyword.Text),
                       //2017-05-25 CRM專案取消 客戶名稱
                       ConvertLib.ToStrs(this.txtDEALER_ERP_NAME.Text),
                       isCancel,
                       //2017/05/24 CRM專案取消 使用完畢之報價單不顯示
                       null
                    );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            //因為地區會依系統使用者依所指定的內銷地區範圍設定, 若為「全部」時, 會篩選全部可用的地區, 而不是只有一筆, 故而另加一個「hidIsAllDomDist」來記錄是否為「全部」.
            if (conds.Conds.DomDistSIds != null && conds.Conds.DomDistSIds.Length > 0 && "N".Equals(this.hidIsAllDomDist.Value))
            {
                WebUtilBox.SetListControlSelected(conds.Conds.DomDistSIds[0].ToString(), this.lstDomDistList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstDomDistList);
            }
            
            this.txtBeginEdd.Text = ConvertLib.ToStr(conds.Conds.BeginEdd, string.Empty, "yyyy-MM-dd");
            this.txtEndEdd.Text = ConvertLib.ToStr(conds.Conds.EndEdd, string.Empty, "yyyy-MM-dd");
            this.txtKeyword.Text="";
            //2017-05-25 CRM專案取消 客戶名稱
            this.txtDEALER_ERP_NAME.Text = "";

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

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "地區";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "專案編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "專案報價日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "報價單抬頭";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "報價單備註";
            htmlTr.Cells.Add(htmlTh);

            //20170509 by 儀淳 CRM專案取消功能新增
            if (this.lstProjQuoteCancelStatuList.SelectedValue == "N")
            {
                htmlTh = new TableHeaderCell();
                htmlTh.Text = "全選 <input type='checkbox' class='dev-sel-all' />";
                htmlTr.Cells.Add(htmlTh);
            }
            else
            {
                htmlTh = new TableHeaderCell();
                htmlTh.Text = "選擇";                
                htmlTr.Cells.Add(htmlTh);
            }
            #endregion            

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ProjQuote_dist entityProjQuote = new ProjQuote_dist(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityProjQuote.GetInfoCount(conds.Conds);

            #region 分頁初始
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
                this.litTotalPageNumber.Text = "0";
                this.litSearchResulted.Text = "0";
            }
            else
            {
                this.litTotalPageNumber.Text = this.observerPaging.TotalPageCount.ToString();
                this.litSearchResulted.Text = flipper.Total.ToString();
            }
            #endregion

            SqlOrder sorting = SqlOrder.Default;

            returner = entityProjQuote.GetInfo(conds.Conds, flipper, sorting);

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ProjQuote_dist.InfoView.Binding(returner.DataSet.Tables[0]);

                int seqNo = this.observerPaging.CurrentPageNumber * flipper.Size - flipper.Size + 1;
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
                    htmlTd.Text = info.DomDistName;
                    htmlTr.Cells.Add(htmlTd);

                    query.Add("QUOTEID", info.QuoteId);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='{0}' title='{1}'>{1}</a>", query, info.QuoteNumber);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='6' title='{0}'>{0}</span>", info.DealerErpName);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.QuoteDate, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='6' title='{0}'>{0}</span>", info.QuoteTitle);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='12' title='{0}'>{0}</span>", info.Remark);
                    htmlTr.Cells.Add(htmlTd);

                    //[20170509 by 儀淳] CRM專案取消功能新增
                    if (this.lstProjQuoteCancelStatuList.SelectedValue == "N")
                    {
                        htmlTd = new TableCell();
                        htmlTd.CssClass = "dev-sel-cell";
                        htmlTd.Text = string.Format("<input type='checkbox' class='dev-sel' value='{0}' />", info.QuoteNumber);
                        htmlTr.Cells.Add(htmlTd);
                    }
                    else
                    {
                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='truncate' length='12' title='{0}'>{0}</span>", "已取消");
                        htmlTr.Cells.Add(htmlTd);
                    }

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