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

public partial class popup_ext_goods : System.Web.UI.Page
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

    public bool IsSingleMode { get; set; }
    public ISystemId ContainerSId { get; set; }

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

        this.IsSingleMode = ConvertLib.ToBoolean(Request.QueryString["single"]);
        this.ContainerSId = ConvertLib.ToSId(Request.QueryString["cntrSId"]);

        if (this.ContainerSId == null)
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

        this.PageTitle = "品項清單";

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);
                returner = entityErpInv.GetGroupInfo(new ErpInv.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Strs, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Bool), ConvertLib.ToStrs("SEGMENT1"), Sort.Ascending);
                if (returner.IsCompletedAndContinue)
                {
                    var rows = returner.DataSet.Tables[0].Rows;

                    foreach (DataRow row in rows)
                    {
                        this.lstCatList_1.Items.Add(new ListItem(row["SEGMENT1"].ToString(), row["SEGMENT1"].ToString()));
                    }
                }

                this.phSingleModeJS.Visible = this.IsSingleMode;
                this.phMultiModeJS.Visible = !this.IsSingleMode;
                this.phMultiAdd.Visible = !this.IsSingleMode;
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
                this.ErpInvList();
            }
        }
    }

    protected void btnInitPageInfo_Click(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.Page_DoSearch(null, EventArgs.Empty);
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }

    #region 分類切換
    protected void lstCatList_1_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.lstCatList_2.Items.Clear();
        this.lstCatList_3.Items.Clear();

        if (string.IsNullOrWhiteSpace(this.lstCatList_1.SelectedValue))
        {
            return;
        }
        else
        {
            this.lstCatList_2.Items.Add(new ListItem("請選擇", string.Empty));
        }

        Returner returner = null;
        try
        {
            ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);
            returner = entityErpInv.GetGroupInfo(new ErpInv.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Strs, this.lstCatList_1.SelectedValue, DefVal.Str, DefVal.Str, DefVal.Bool), ConvertLib.ToStrs("SEGMENT2"), Sort.Ascending);
            if (returner.IsCompletedAndContinue)
            {
                var rows = returner.DataSet.Tables[0].Rows;

                foreach (DataRow row in rows)
                {
                    this.lstCatList_2.Items.Add(new ListItem(row["SEGMENT2"].ToString(), row["SEGMENT2"].ToString()));
                }
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }

    protected void lstCatList_2_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.lstCatList_3.Items.Clear();

        if (string.IsNullOrWhiteSpace(this.lstCatList_2.SelectedValue))
        {
            return;
        }
        else
        {
            this.lstCatList_3.Items.Add(new ListItem("請選擇", string.Empty));
        }

        Returner returner = null;
        try
        {
            ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);
            returner = entityErpInv.GetGroupInfo(new ErpInv.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Strs, this.lstCatList_1.SelectedValue, this.lstCatList_2.SelectedValue, DefVal.Str, DefVal.Bool), ConvertLib.ToStrs("SEGMENT3"), Sort.Ascending);
            if (returner.IsCompletedAndContinue)
            {
                var rows = returner.DataSet.Tables[0].Rows;

                foreach (DataRow row in rows)
                {
                    this.lstCatList_3.Items.Add(new ListItem(row["SEGMENT3"].ToString(), row["SEGMENT3"].ToString()));
                }
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
        public SearchConds()
        {
            this.KeywordCols = new List<string>();
        }

        public ErpInv.InfoConds ErpInvConds { get; set; }
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
            conds.KeywordCols.Add("ITEM");
            conds.KeywordCols.Add("DESCRIPTION");

            conds.ErpInvConds = new ErpInv.InfoConds
                (
                   DefVal.SIds,
                   DefVal.Longs,
                   DefVal.Strs,
                   this.lstCatList_1.SelectedValue,
                   this.lstCatList_2.SelectedValue,
                   this.lstCatList_3.SelectedValue,
                   DefVal.Bool
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            #region 三層分類
            if (conds.ErpInvConds.Segment1 != this.lstCatList_1.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.ErpInvConds.Segment1, this.lstCatList_1);
                this.lstCatList_1_SelectedIndexChanged(null, EventArgs.Empty);
            }

            if (conds.ErpInvConds.Segment2 != this.lstCatList_2.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.ErpInvConds.Segment2, this.lstCatList_2);
                this.lstCatList_2_SelectedIndexChanged(null, EventArgs.Empty);
            }

            if (conds.ErpInvConds.Segment3 != this.lstCatList_3.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.ErpInvConds.Segment3, this.lstCatList_3);
            }
            #endregion

            this.txtKeyword.Text = conds.Keyword;
            //WebUtilBox.SetListControlSelected(conds.KeywordCols.ToArray(), this.chklKeywordCols);
            #endregion
        }

        return conds;
    }
    #endregion

    #region ERP 庫存列表
    /// <summary> 
    /// ERP 庫存列表。 
    /// </summary> 
    void ErpInvList()
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
            htmlTh.Text = "型號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "摘要";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "選擇";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.All;

            var conds = this.GetSearchConds();

            returner = entityErpInv.GetInfoByCompoundSearchCount(conds.ErpInvConds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityErpInv.GetInfoByCompoundSearch(conds.ErpInvConds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ErpInv.Info.Binding(returner.DataSet.Tables[0]);

                //已存在的品項暫存
                var existedGoodsItems = new List<string>();
                existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.Item);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Model;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Description;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Item;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    if (this.IsSingleMode)
                    {
                        //用 javascript 控制, 點 row 即選擇, 若在 button 中也加 onlick, 會執行兩次.
                        //htmlTd.Text = string.Format("<input type='button' value='選擇' class='dev-sel' customerid='{0}' onclick='returnSingleInfo({0}); return false;' />", info.Item);
                        htmlTd.Text = existedGoodsItems.Contains(info.Item) ? string.Empty : string.Format("<input type='button' value='選擇' class='dev-sel' partno='{0}' />", info.Item);
                    }
                    else
                    {
                        htmlTd.Text = existedGoodsItems.Contains(info.Item) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{0}' />", info.Item);
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