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

public partial class popup_customer : System.Web.UI.Page
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

    ISystemId _domDistSId;
    string _domDistName = string.Empty;
    long? _priceListId;

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

        this._domDistSId = ConvertLib.ToSId(Request.QueryString["dist"]);
        this._priceListId = ConvertLib.ToLong(Request.QueryString["priceListId"], DefVal.Long);

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

        this.PageTitle = "客戶清單";

        this.btnUpdateErpCuster.OnClientClick = string.Format("javascript:if(!window.confirm('確定要更新客戶資料？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (this._domDistSId != null)
            {
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
                returner = entityPubCat.GetInfo(new ISystemId[] { this._domDistSId }, IncludeScope.All, new string[] { "NAME" });
                if (returner.IsCompletedAndContinue)
                {
                    var row = returner.DataSet.Tables[0].Rows[0];

                    //[20160107 by fan] 地區再細分辦事處 (例「上海-蘇州辦、上海-南京辦」), 主要用在訂單的地區識別, 在取客戶時, 使用地區前兩碼即可.
                    this._domDistName = row["NAME"].ToString();
                    if (this._domDistName.Length >= 2)
                    {
                        this._domDistName = this._domDistName.Substring(0, 2);
                    }
                }
                else
                {
                    JSBuilder.ClosePage(this);
                    return false;
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
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }

    protected void btnUpdateErpCuster_Click(object sender, EventArgs e)
    {
        var uptCont = Schedule.ImportErpCuster(false);

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

        public ErpCuster.InfoConds Conds { get; set; }
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

            conds.Conds = new ErpCuster.InfoConds
                (
                   DefVal.SIds,
                   DefVal.Longs,
                   ConvertLib.ToLongs(this._priceListId),
                   ConvertLib.ToInts(Request.QueryString["mr"]),
                   ConvertLib.ToStrs(this._domDistName),
                   DefVal.Str
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
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
            htmlTh.Text = "客戶編號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "選擇";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.All;

            var conds = this.GetSearchConds();

            returner = entityErpCuster.GetInfoByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityErpCuster.GetInfoByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ErpCuster.Info.Binding(returner.DataSet.Tables[0]);

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.CustomerName);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.CustomerNumber;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.CustomerName;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    //用 javascript 控制, 點 row 即選擇, 若在 button 中也加 onlick, 會執行兩次.
                    //htmlTd.Text = string.Format("<input type='button' value='選擇' class='dev-sel' customerid='{0}' onclick='returnInfo({0}); return false;' />", info.CustomerId);
                    htmlTd.Text = string.Format("<input type='button' value='選擇' class='dev-sel' customerid='{0}' />", info.CustomerId);
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