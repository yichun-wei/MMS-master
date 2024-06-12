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

public partial class dom_order_shipping_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_SHIPPING_ORDER";
    string AUTH_NAME = "內銷出貨單";

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

        this.PageTitle = "內銷出貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>內銷出貨單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        //倉管權限
        this.btnStockUp.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(4);
        this.btnStockUp.OnClientClick = string.Format("javascript:if(!window.confirm('確認備貨？')){{return false;}}");

        //倉管權限
        this.btnDetermineShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(4);
        this.btnDetermineShipping.OnClientClick = string.Format("javascript:if(!window.confirm('確認出貨？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                #region 內銷地區
                foreach (var domDistInfo in this.MainPage.ActorInfoSet.DomDistInfos)
                {
                    this.lstDomDistList.Items.Add(new ListItem(domDistInfo.Name, domDistInfo.SId.Value));
                }
                #endregion

                #region 倉庫
                var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                foreach (var erpWhseInfo in erpWhseInfos)
                {
                    this.lstWhseList.Items.Add(new ListItem(erpWhseInfo.SecondaryInventoryName, erpWhseInfo.SecondaryInventoryName));
                }
                #endregion

                //狀態
                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                this.lstStatusList.Items.AddRange(DomOrderHelper.GetDomOrderStatusItems(ConvertLib.ToStrs("5", "6", "7", "8")));
                WebUtilBox.SetListControlSelected(Request.QueryString["status"], this.lstStatusList);

                //預計出貨日 預設為今天
                this.txtBeginEdd.Text = DateTime.Today.ToString("yyyy-MM-dd");
                this.txtEndEdd.Text = DateTime.Today.ToString("yyyy-MM-dd");
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

        //權限操作檢查
        this.btnDetermineShipping.Visible = this.btnDetermineShipping.Visible ? this.FunctionRight.Maintain : this.btnDetermineShipping.Visible;
    }

    #region 更改訂單狀態為「備貨中」
    protected void btnStockUp_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //倉管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        if (string.IsNullOrEmpty(Request.Form["sel_items[]"])) { return; }
        ISystemId[] systemIds = ConvertLib.ToSIds(Request.Form["sel_items[]"].Split(','));
        if (systemIds.Length > 0)
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            Returner returner = null;
            Returner returnerTmp = null;
            try
            {
                List<string> delPaths = new List<string>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                    DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                    //當選擇數與取出的「已列印」訂單數不同時, 則顯示失敗.
                    returner = entityDomOrder.GetInfoCount(new DomOrder.InfoConds(systemIds, DefVal.SIds, DefVal.Strs, ConvertLib.ToInts(6), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, DefVal.Bool), IncludeScope.OnlyNotMarkDeleted);
                    if (systemIds.Length != Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]))
                    {
                        JSBuilder.AlertMessage(this, true, "更新狀態失敗", "只允許狀態為「已列印」的訂單");
                        return;
                    }

                    foreach (var sid in systemIds)
                    {
                        //更改訂單狀態
                        //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                        entityDomOrder.UpdateStatus(actorSId, sid, 7);
                    }

                    transaction.Complete();
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
                if (returnerTmp != null) returnerTmp.Dispose();
            }
        }
    }
    #endregion

    #region 更改訂單狀態為「已出貨」
    protected void btnDetermineShipping_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //倉管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        if (string.IsNullOrEmpty(Request.Form["sel_items[]"])) { return; }
        ISystemId[] systemIds = ConvertLib.ToSIds(Request.Form["sel_items[]"].Split(','));
        if (systemIds.Length > 0)
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            Returner returner = null;
            Returner returnerTmp = null;
            try
            {
                List<string> delPaths = new List<string>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                    DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                    //當選擇數與取出的「備貨中」訂單數不同時, 則顯示失敗.
                    returner = entityDomOrder.GetInfoCount(new DomOrder.InfoConds(systemIds, DefVal.SIds, DefVal.Strs, ConvertLib.ToInts(7), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, DefVal.Bool), IncludeScope.OnlyNotMarkDeleted);
                    if (systemIds.Length != Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]))
                    {
                        JSBuilder.AlertMessage(this, true, "更新狀態失敗", "只允許狀態為「備貨中」的訂單");
                        return;
                    }

                    foreach (var sid in systemIds)
                    {
                        //更改訂單狀態
                        //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                        entityDomOrder.UpdateStatus(actorSId, sid, 8);
                        //更新實際出貨日
                        entityDomOrder.UpdateShipDate(actorSId, sid, DateTime.Today);
                    }

                    transaction.Complete();
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
                if (returnerTmp != null) returnerTmp.Dispose();
            }
        }
    }
    #endregion

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }

    #region 取得查詢條件
    #region 查詢條件
    class SearchConds
    {
        public SearchConds()
        {
            this.KeywordCols = new List<string>();
        }

        public DomOrder.InfoViewConds Conds { get; set; }
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
            }
            else
            {
                domDistSIds = ConvertLib.ToSIds(domDistSId);
            }

            //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
            //若選擇「全部」則只顯示「5:待列印 6:已列印 7:備貨中 8:已出貨」
            int[] status = { 5, 6, 7, 8 };
            if (VerificationLib.IsNumber(this.lstStatusList.SelectedValue))
            {
                status = ConvertLib.ToInts(Convert.ToInt32(this.lstStatusList.SelectedValue));
            }

            conds.Conds = new DomOrder.InfoViewConds
                (
                   DefVal.SIds,
                   domDistSIds,
                   ConvertLib.ToStrs(this.lstWhseList.SelectedValue),
                   status,
                   ConvertLib.ToDateTime(this.txtBeginCdt.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndCdt.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtBeginEdd.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndEdd.Text, DefVal.DT),
                   DefVal.Ints,
                   false // 2016.5.11 UPDATE BY MICHELLE 若訂單已取消，出貨單也不要顯示 
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            //因為若全選不會是空值
            if (conds.Conds.DomDistSIds == null || conds.Conds.DomDistSIds.Length > 1)
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstDomDistList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(conds.Conds.DomDistSIds[0].Value, this.lstDomDistList);
            }

            //因為若全選不會是空值
            if (conds.Conds.Statuses == null || conds.Conds.Statuses.Length > 1)
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstStatusList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Statuses[0].ToString(), this.lstStatusList);
            }

            this.txtBeginCdt.Text = ConvertLib.ToStr(conds.Conds.BeginCdt, string.Empty, "yyyy-MM-dd");
            this.txtEndCdt.Text = ConvertLib.ToStr(conds.Conds.EndCdt, string.Empty, "yyyy-MM-dd");
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

            //倉管權限
            if (this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
            {
                htmlTh = new TableHeaderCell();
                htmlTh.Text = "選擇";
                htmlTr.Cells.Add(htmlTh);
            }

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "地區";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "訂單編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "ERP 訂單編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "訂單日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "預計出貨日";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "出貨單狀態";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "總金額 (RMB)";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.OnlyNotMarkDeleted;

            var conds = this.GetSearchConds();

            returner = entityDomOrder.GetInfoViewByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityDomOrder.GetInfoViewByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            //this.btnDelete.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            //this.btnDelete.Attributes["onclick"] = "javascript:if(!window.confirm('確定要刪除？')){return false;}";

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = DomOrder.InfoView.Binding(returner.DataSet.Tables[0]);

                int seqNo = this.observerPaging.CurrentPageNumber * flipper.Size - flipper.Size + 1;
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];


                    htmlTr = new TableRow();
                    //列顏色識別
                    //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                    switch (info.Status)
                    {
                        //訂單狀態「6:已列印」才能更改為「7:備貨中」
                        //訂單狀態「7:備貨中」才能更改為「8:已出貨」
                        case 6:
                            htmlTr.CssClass = "printed";
                            break;
                        case 7:
                            htmlTr.CssClass = "stocking";
                            break;
                        case 8:
                            htmlTr.CssClass = "shipped";
                            break;
                    }
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.OdrNo);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "no";
                    htmlTd.Text = string.Format("{0}", seqNo);
                    htmlTr.Cells.Add(htmlTd);

                    //倉管權限
                    if (this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
                    {
                        htmlTd = new TableCell();
                        htmlTd.CssClass = "no";
                        //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                        switch (info.Status)
                        {
                            //訂單狀態「6:已列印」才能更改為「7:備貨中」
                            //訂單狀態「7:備貨中」才能更改為「8:已出貨」
                            case 6:
                            case 7:
                                htmlTd.Text = string.Format("<input type='checkbox' name='sel_items[]' class='dev-sel-item' value='{0}' />", info.SId);
                                break;
                        }
                        htmlTr.Cells.Add(htmlTd);
                    }

                    htmlTd = new TableCell();
                    htmlTd.Text = info.DomDistName;
                    htmlTr.Cells.Add(htmlTd);

                    query.Add("sid", info.SId.Value);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='{0}' title='{1}'>{1}</a>", query, info.OdrNo);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.ErpOrderNumber;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='6' title='{0}'>{0}</span>", info.CustomerName);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.Cdt, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.Edd, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = DomOrderHelper.GetDomOrderStatusName(info.Status);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    if (info.PTTotalAmt.HasValue)
                    {
                        htmlTd.Text = ConvertLib.ToAccounting(info.DctTotalAmt.Value);
                    }
                    else
                    {
                        htmlTd.Text = string.Empty;
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
}