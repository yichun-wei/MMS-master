using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class project_view : System.Web.UI.Page
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
    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    DomOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    string _ProjQuoteSId;

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

        this._ProjQuoteSId = Request.QueryString["QUOTEID"];

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "CRM專案查詢";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>CRM專案查詢</a>"));

        Returner returner = null;
        try
        {
            if (this._ProjQuoteSId != null)
            {
                ProjQuote_dist entityProjQuote = new ProjQuote_dist(SystemDefine.ConnInfo);

                var conds = this.GetSearchConds();

                returner = entityProjQuote.QuoteContentGetInfoView(conds.Conds, 1, new SqlOrder("QUOTEITEMID", Sort.Ascending));

                if (returner.IsCompletedAndContinue)
                {
                    var infos = ProjQuote_dist.QuoteContentInfoView.Binding(returner.DataSet.Tables[0]);

                    var info = infos[0];

                    #region 報價單表頭
                    this.litQuoteNumber.Text = info.QuoteNumber;
                    this.litQuoteDate.Text = ConvertLib.ToStr(info.QuoteDate, string.Empty, "yyyy-MM-dd");
                    this.litCustomerId.Text = info.DealerCustomerId;
                    this.litQuoteTitle.Text = info.QuoteTitle;
                    this.litCustomerName.Text = info.DealerErpName;
                    this.litRemark.Text = info.Remark;
                    #endregion

                    this.PageTitle = string.Format("{1} ({0})", info.QuoteNumber, info.DealerErpName);
                    breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));                    
                }
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

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
    }

   

    #region 取得查詢條件
    #region 查詢條件
    class SearchConds
    {
        public ProjQuote_dist.QuoteContentInfoViewConds Conds { get; set; }
    }
    #endregion

    SearchConds GetSearchConds()
    {
        SearchConds conds;
        if (string.IsNullOrWhiteSpace(this.hidSearchConds.Value))
        {
            conds = new SearchConds();

            //系統使用者依所指定的內銷地區範圍設定
            ISystemId[] ProjQuoteSIds = DefVal.SIds;
            string ProjQuoteSId = this._ProjQuoteSId;
            if (ProjQuoteSId == null)
            {
                //若未選擇, 則取該系統使用者所有被指定的內銷地區.
                ProjQuoteSIds = this.MainPage.ActorInfoSet.DomDistInfos.Select(q => q.SId).ToArray();
                if (ProjQuoteSIds.Length == 0)
                {
                    //若沒被指定的內銷地區, 則直接使其找不到.
                    ProjQuoteSIds = ConvertLib.ToSIds(SystemId.MaxValue);
                }
                this.hidIsAllDomDist.Value = "Y";
            }
            else
            {
                ProjQuoteSIds = ConvertLib.ToSIds(ProjQuoteSId);
                this.hidIsAllDomDist.Value = "N";
            }

            conds.Conds = new ProjQuote_dist.QuoteContentInfoViewConds
                    (
                       this._ProjQuoteSId
                    );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);
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
            htmlTh.Text = "明細編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "數量";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "單價";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "折扣";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "專案價格";
            htmlTr.Cells.Add(htmlTh);
            #endregion            

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ProjQuote_dist entityProjQuote = new ProjQuote_dist(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityProjQuote.QuoteContentGetInfoCount(conds.Conds);

            returner = entityProjQuote.QuoteContentGetInfoView(conds.Conds, Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]), new SqlOrder("QUOTEITEMID", Sort.Ascending));

            if (returner.IsCompletedAndContinue)
            {
                var infos = ProjQuote_dist.QuoteContentInfoView.Binding(returner.DataSet.Tables[0]);

                string CusterID = "";
                #region 透過ErpCuster取「價目表 ID」
                ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
                returner = entityErpCuster.GetInfoView(new ErpCuster.InfoViewConds(DefVal.SIds, ConvertLib.ToLongs(this.litCustomerId.Text), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var info_ErpCuster = ErpCuster.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);
                    //「價目表 ID」只是為了給品項取牌價用
                    this.hidPriceListId.Value = ConvertLib.ToStr(info_ErpCuster.PriceListId, string.Empty);
                    CusterID = ConvertLib.ToStr(info_ErpCuster.CustomerId);
                }
                #endregion

                #region 透到Oracle ERP 取「料號牌價」
                //先取得所有品項的價目表
                var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                var priceListId = ConvertLib.ToLong(WebUtilBox.FindControl<HiddenField>(this.Page, "hidPriceListId").Value, DefVal.Long);
                
                if (priceListId.HasValue)
                {
                    priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, infos.Select(q => q.ITEM).ToArray());
                }
                #endregion

                string ITEM = "",price_ID="";
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    if (string.IsNullOrWhiteSpace(info.ITEM) || string.IsNullOrWhiteSpace(ConvertLib.ToStr(ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM))))
                    {
                        if (string.IsNullOrWhiteSpace(info.ITEM))
                            ITEM += info.ITEM + " ";
                        if(string.IsNullOrWhiteSpace(ConvertLib.ToStr(ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM))))
                            price_ID += info.ITEM + " ";

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", TrimBefore(info.QuoteItemId, '0'));
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", info.ProductId);
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", ConvertLib.ToStr(info.Quantity));
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", ConvertLib.ToStr(ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM)));
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", ConvertLib.ToStr(info.Discount));
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = string.Format("<span class='red'>{0}</span>", ConvertLib.ToStr( ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM) * info.Discount));
                        htmlTr.Cells.Add(htmlTd);
                    }
                    else
                    {
                        htmlTd = new TableCell();
                        htmlTd.Text = TrimBefore(info.QuoteItemId, '0');
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = info.ProductId;
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = ConvertLib.ToStr(info.Quantity);
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = ConvertLib.ToStr(ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM));
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = ConvertLib.ToStr(info.Discount);
                        htmlTr.Cells.Add(htmlTd);

                        htmlTd = new TableCell();
                        htmlTd.Text = ConvertLib.ToStr(ErpHelper.GetPriceBookListPrice(priceBookInfos, info.ITEM) * info.Discount);
                        htmlTr.Cells.Add(htmlTd);
                    }
                }

                if (string.IsNullOrWhiteSpace(CusterID))
                {
                    JSBuilder.AlertMessage(this, "查無此客戶");
                }
                else
                {
                    if (ITEM.Length>0)
                    {
                        JSBuilder.AlertMessage(this, ITEM + "無此料號");
                    }

                    if (price_ID.Length>0)
                    {
                        JSBuilder.AlertMessage(this, price_ID + "查無料號牌價");
                    }
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region 去除多餘的0
    public static string TrimBefore(string origin, char c)
    {

        StringBuilder sb = new StringBuilder(origin);

        while (sb.Length > 0 && sb[0] == c)
        {

            sb.Remove(0, 1);
        }

        return sb.ToString();

    }
    #endregion   
}