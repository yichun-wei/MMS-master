using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class popup_dom_proj_quote_goods : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientPopupMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
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

    public ISystemId ContainerSId { get; set; }

    long? _customerId;
    string _quoteNumber;

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
        this.MainPage = (IClientPopupMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        this.ContainerSId = ConvertLib.ToSId(Request.QueryString["cntrSId"]);
        this._customerId = ConvertLib.ToLong(Request.QueryString["customerId"], DefVal.Long);
        this._quoteNumber = Request.QueryString["quoteNo"];

        if (this.ContainerSId == null || this._customerId == null || string.IsNullOrWhiteSpace(this._quoteNumber))
        {
            JSBuilder.ClosePage(this);
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
        this.observerPaging.Register(this.firstPaging, this.prevPaging, this.numberPaging, this.nextPaging, this.lastPaging, this.textboxPaging);

        this.PageTitle = "專案報價";

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                returner = entityPGOrderDet.GetGroupInfo(new PGOrderDet.InfoViewConds(DefVal.SIds, DefVal.SId, DefVal.Str, 2, this._quoteNumber, DefVal.Str, DefVal.Str, this._customerId.Value, false, true, IncludeScope.OnlyNotMarkDeleted), ConvertLib.ToStrs("PG_ORDER_ODR_NO"), Sort.Ascending);
                if (returner.IsCompletedAndContinue)
                {
                    var rows = returner.DataSet.Tables[0].Rows;

                    foreach (DataRow row in rows)
                    {
                        this.lstSourceList.Items.Add(new ListItem(string.Format("備貨單-{0}", row["PG_ORDER_ODR_NO"]), row["PG_ORDER_ODR_NO"].ToString()));
                    }
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

        if (!this.IsPostBack)
        {
            //第一次載入時, 要先從 opener 取得已存在的品項, 載入後品得後再建立列表.
            WebPg.RegisterScript("initPageInfo();");
        }
        else
        {
            var scriptManager = ScriptManager.GetCurrent(this);
            if (!scriptManager.IsInAsyncPostBack)
            {
                if (string.IsNullOrWhiteSpace(this.lstSourceList.SelectedValue))
                {
                    this.ProjQuoteList();
                }
                else
                {
                    this.PGOrderDetList();
                }
            }
        }
    }

    protected void btnInitPageInfo_Click(object sender, EventArgs e)
    {
        this.phPGOrderConds.Visible = "Y".Equals(this.hidContainPGOrder.Value);
    }

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

        public ProjQuote.InfoViewConds ProjQuoteConds { get; set; }
        public PGOrderDet.InfoViewConds PGOrderDetConds { get; set; }
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

            //conds.Keyword = this.txtKeyword.Text;
            //conds.KeywordCols.Add("ITEM");
            //conds.KeywordCols.Add("DESCRIPTION");

            conds.ProjQuoteConds = new ProjQuote.InfoViewConds
                (
                   ConvertLib.ToStrs(this._quoteNumber),
                   DefVal.Strs,
                   this._customerId.Value.ToString()
                );

            conds.PGOrderDetConds = new PGOrderDet.InfoViewConds
                (
                   DefVal.SIds,
                   DefVal.SId,
                   this.lstSourceList.SelectedValue,
                   2,
                   this._quoteNumber,
                   DefVal.Str,
                   DefVal.Str,
                   this._customerId.Value,
                   false,
                   true,
                   IncludeScope.OnlyNotMarkDeleted
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            WebUtilBox.SetListControlSelected(conds.PGOrderDetConds.PGOrderOdrNo, this.lstSourceList);

            //this.txtKeyword.Text = conds.Keyword;
            //WebUtilBox.SetListControlSelected(conds.KeywordCols.ToArray(), this.chklKeywordCols);
            #endregion
        }

        return conds;
    }
    #endregion

    #region 專案報價列表
    /// <summary> 
    /// 專案報價列表。 
    /// </summary> 
    void ProjQuoteList()
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
            htmlTh.Text = "摘要";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "可用量";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            //htmlTh.Text = "選擇";
            htmlTh.Text = "全選 <input type='checkbox' class='dev-sel-all' />";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityProjQuote.GetInfoViewCount(conds.ProjQuoteConds);

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

            SqlOrder sorting = new SqlOrder();
            sorting.Add("QUOTENUMBER", Sort.Ascending);
            sorting.Add("QUOTEITEMID", Sort.Ascending);
            returner = entityProjQuote.GetInfoView(conds.ProjQuoteConds, flipper, sorting);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0]);

                //已存在的品項暫存
                var existedGoodsItems = new List<string>();
                existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.ProductId);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Summary;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.ProductId;
                    htmlTr.Cells.Add(htmlTd);

                    //數量 - 備貨單使用數量 - 內銷訂單使用數量
                    var availableQty = info.Quantity - info.PGOrderUseQty - info.DomOrderUseQty;

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToAccounting(availableQty);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    if (availableQty > 0)
                    {
                        htmlTd.Text = existedGoodsItems.Contains(info.QuoteItemId) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{0}' />", info.QuoteItemId);
                    }
                    htmlTr.Cells.Add(htmlTd);
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region 專案報價備貨單列表
    /// <summary> 
    /// 專案報價備貨單列表。 
    /// </summary> 
    void PGOrderDetList()
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
            htmlTh.Text = "摘要";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "可用量";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            //htmlTh.Text = "選擇";
            htmlTh.Text = "全選 <input type='checkbox' class='dev-sel-all' />";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityPGOrderDet.GetInfoViewCount(conds.PGOrderDetConds);

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

            SqlOrder sorting = new SqlOrder();
            sorting.Add("QUOTENUMBER", Sort.Ascending);
            sorting.Add("QUOTEITEMID", Sort.Ascending);
            sorting.Add("SID", Sort.Ascending);
            returner = entityPGOrderDet.GetInfoView(conds.PGOrderDetConds, flipper, sorting);

            if (returner.IsCompletedAndContinue)
            {
                var infos = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                //已存在的品項暫存
                var existedGoodsItems = new List<string>();
                existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.PartNo);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Summary;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.PartNo;
                    htmlTr.Cells.Add(htmlTd);

                    //數量 - 內銷訂單使用數量
                    var availableQty = info.Qty - info.DomOrderUseQty;

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToAccounting(availableQty);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    if (availableQty > 0)
                    {
                        htmlTd.Text = existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.QuoteItemId, info.SId)) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{1}{0}{2}' />", SystemDefine.JoinSeparator, info.QuoteItemId, info.SId);
                    }
                    htmlTr.Cells.Add(htmlTd);
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