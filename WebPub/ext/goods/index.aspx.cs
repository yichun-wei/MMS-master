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

public partial class ext_goods_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_GOODS";
    string AUTH_NAME = "外銷商品";

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

        this.PageTitle = "外銷商品";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}sys_user/index.aspx'>後端管理</a>", SystemDefine.WebSiteRoot));
        breadcrumbs.Add(string.Format("<a href='{0}'>外銷商品</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

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
        if (this.IsPostBack)
        {
            this.UpdateModel();
        }
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

        this.WebPg.AdditionalClickToDisableScript(this.btnUpdateModel);

        //權限操作檢查
        this.btnUpdateModel.Visible = this.btnUpdateModel.Visible ? this.FunctionRight.Maintain : this.btnUpdateModel.Visible;
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

    #region 修改型號
    ListChangeBuff[] GetReModelList()
    {
        string buff = Request.Form["hidModelBuff"];

        if (string.IsNullOrEmpty(buff))
        {
            return null;
        }

        return CustJson.DeserializeObject<ListChangeBuff[]>(buff).Where(q => SystemId.MinValue.IsSystemId(q.SId) && !q.OldVal.Equals(q.NewVal)).ToArray();
    }

    void UpdateModel()
    {
        var modelBuff = this.GetReModelList();
        if (modelBuff == null || modelBuff.Length == 0)
        {
            //若沒有任何異動則略過.
            return;
        }

        Returner returner = null;
        try
        {
            ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);
            foreach (var buff in modelBuff)
            {
                entityErpInv.UpdateMode(new SystemId(buff.SId), buff.NewVal);
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

        public ErpInv.InfoConds Conds { get; set; }
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

            conds.Conds = new ErpInv.InfoConds
                (
                   DefVal.SIds,
                   DefVal.Longs,
                   DefVal.Strs,
                   this.lstCatList_1.SelectedValue,
                   this.lstCatList_2.SelectedValue,
                   this.lstCatList_3.SelectedValue,
                   ConvertLib.ToBoolean(this.lstModelStatus.SelectedValue, DefVal.Bool)
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            #region 三層分類
            if (conds.Conds.Segment1 != this.lstCatList_1.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Segment1, this.lstCatList_1);
                this.lstCatList_1_SelectedIndexChanged(null, EventArgs.Empty);
            }

            if (conds.Conds.Segment2 != this.lstCatList_2.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Segment2, this.lstCatList_2);
                this.lstCatList_2_SelectedIndexChanged(null, EventArgs.Empty);
            }

            if (conds.Conds.Segment3 != this.lstCatList_3.SelectedValue)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Segment3, this.lstCatList_3);
            }
            #endregion

            if (conds.Conds.IsModelExists.HasValue)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.IsModelExists.Value ? "Y" : "N", this.lstModelStatus);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstModelStatus);
            }

            this.txtKeyword.Text = conds.Keyword;
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
            htmlTh.Text = "序號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "摘要";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "型號";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.All;

            var conds = this.GetSearchConds();

            returner = entityErpInv.GetInfoByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityErpInv.GetInfoByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            this.btnUpdateModel.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            this.btnUpdateModel.Attributes["onclick"] = "javascript:if(window.confirm('確定要儲存修改？')){ChangingModel();}return false;";

            List<ListChangeBuff> modelBuff = new List<ListChangeBuff>();

            if (returner.IsCompletedAndContinue)
            {
                var infos = ErpInv.Info.Binding(returner.DataSet.Tables[0]);

                int seqNo = this.observerPaging.CurrentPageNumber * flipper.Size - flipper.Size + 1;
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.Item);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "no";
                    htmlTd.Text = string.Format("{0}", seqNo);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Description;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Item;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "sort";
                    htmlTd.Text = string.Format("<textarea id='txtModel_{0}' name='txtModel_{0}' oldVal='{1}' class='dev-model' onkeypress=\"this.className='dev-model txt_changed';\" onkeydown='if(event.keyCode == 13){{return false;}}'>{1}</textarea>", info.SId, info.Model);
                    htmlTr.Cells.Add(htmlTd);

                    seqNo++;

                    modelBuff.Add(new ListChangeBuff()
                    {
                        SId = info.SId.Value,
                        OldVal = info.Model
                    });
                }
            }

            WebUtilBox.RegisterScript(this, string.Format("modelData = {0};", CustJson.SerializeObject(modelBuff)));
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}