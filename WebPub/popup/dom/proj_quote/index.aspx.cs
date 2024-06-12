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
using System.Transactions;

public partial class popup_dom_proj_quote_index : System.Web.UI.Page
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

    long? _customerId;

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

        this._customerId = ConvertLib.ToInt(Request.QueryString["customerId"], DefVal.Int);
        if (this._customerId == null)
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
            WebPg.RegisterScript("getExistedProjQuotes();");
        }
        else
        {
            var scriptManager = ScriptManager.GetCurrent(this);
            if (!scriptManager.IsInAsyncPostBack)
            {
                this.PageList();
            }
        }
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

        public ProjQuote.GrpQuoteInfoViewConds Conds { get; set; }
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

            conds.Conds = new ProjQuote.GrpQuoteInfoViewConds
                (
                   DefVal.Str,
                   DefVal.Str,
                   this._customerId.Value.ToString(),
                   true
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            //this.txtKeyword.Text = conds.Keyword;
            //WebUtilBox.SetListControlSelected(conds.KeywordCols.ToArray(), this.chklKeywordCols);
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
            htmlTh.Text = "專案編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "專案報價日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "報價抬頭";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "報價備註";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "選擇";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityProjQuote.GetGrpQuoteInfoViewCount(conds.Conds);

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
            sorting.Add("QUOTEDATE", Sort.Descending);
            sorting.Add("QUOTENUMBER", Sort.Ascending);

            returner = entityProjQuote.GetGrpQuoteInfoView(conds.Conds, flipper, sorting);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ProjQuote.GrpQuoteInfoView.Binding(returner.DataSet.Tables[0]);

                //已存在的專案報價暫存
                var existedProjQuotes = new List<string>();
                existedProjQuotes.AddRange(this.hidExistedProjQuotes.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.QuoteNumber);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.QuoteNumber;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.QuoteDate, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.QuoteTitle;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Remark;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    htmlTd.Text = existedProjQuotes.Contains(info.QuoteNumber) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{0}' />", info.QuoteNumber);
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