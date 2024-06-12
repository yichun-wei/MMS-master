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

public partial class dom_dist_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_DIST";
    string AUTH_NAME = "內銷地區";

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

    int _useTgt = (int)PubCat.UseTgtOpt.DomDist;

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

        this.PageTitle = "內銷地區";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}sys_user/index.aspx'>後端管理</a>", SystemDefine.WebSiteRoot));
        breadcrumbs.Add(string.Format("<a href='{0}'>內銷地區</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

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
        QueryStringParsing query = new QueryStringParsing();
        query.HttpPath = new Uri("edit.aspx", UriKind.Relative);
        this.btnAdd.Attributes["onclick"] = string.Format("window.open('{0}', '', 'width=900, height=700, top=100, left=100, scrollbars=yes'); return false;", query);

        if (this.IsPostBack)
        {
            this.UpdateSort();
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

        this.WebPg.AdditionalClickToDisableScript(this.btnDelete);

        //權限操作檢查
        this.btnAdd.Visible = this.btnAdd.Visible ? this.FunctionRight.Maintain : this.btnAdd.Visible;
        this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
        this.btnUpdateSort.Visible = this.btnUpdateSort.Visible ? this.FunctionRight.Maintain : this.btnUpdateSort.Visible;
    }

    #region 刪除
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        if (string.IsNullOrEmpty(Request.Form["sel_items[]"])) { return; }
        ISystemId[] systemIds = ConvertLib.ToSIds(Request.Form["sel_items[]"].Split(','));
        if (systemIds.Length > 0)
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            Returner returner = null;
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
                    DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                    #region 檢查資料已否已被使用
                    List<string> existItems = new List<string>();
                    foreach (var sid in systemIds)
                    {
                        returner = entityPubCat.GetDomDistUseInfo(sid);
                        if (returner.IsCompletedAndContinue)
                        {
                            var rows = returner.DataSet.Tables[0].Rows;

                            int totalUseCnt = 0;
                            foreach (DataRow row in rows)
                            {
                                switch (Convert.ToInt32(row["USE_BY"]))
                                {
                                    case 1:
                                        //系統使用者/內銷地區 關聯表
                                        totalUseCnt += Convert.ToInt32(row["USE_CNT"]);
                                        break;
                                    case 2:
                                        //內銷訂單
                                        totalUseCnt += Convert.ToInt32(row["USE_CNT"]);
                                        break;
                                }
                            }

                            if (totalUseCnt > 0)
                            {
                                returner = entityPubCat.GetInfo(new ISystemId[] { sid }, IncludeScope.All, new string[] { "NAME" });
                                if (returner.IsCompletedAndContinue)
                                {
                                    var row = returner.DataSet.Tables[0].Rows[0];
                                    existItems.Add(row["NAME"].ToString());
                                }
                            }
                        }
                    }

                    if (existItems.Count > 0)
                    {
                        JSBuilder.AlertMessage(this, string.Format("{0} 已包含使用中的資料，無法刪除。", string.Join("、", existItems.ToArray())));
                        return;
                    }
                    #endregion

                    foreach (var sid in systemIds)
                    {
                        returner = entityPubCat.GetInfo(new ISystemId[] { sid }, IncludeScope.All);
                        if (returner.IsCompletedAndContinue)
                        {
                            var info = PubCat.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                            //異動記錄
                            string dataTitle = info.Name;
                            entityDataTransLog.Add(actorSId, DBTableDefine.PUB_CAT, sid, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(info));

                            entityPubCat.Delete(actorSId, new ISystemId[] { sid });
                        }
                    }

                    transaction.Complete();
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
    }
    #endregion

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
    }

    #region 修改排序
    void UpdateSort()
    {
        var sortingBuff = this.ucDataSorting.GetReSortList();
        if (sortingBuff == null || sortingBuff.Length == 0)
        {
            //若沒有任何異動則略過.
            return;
        }

        Returner returner = null;
        try
        {
            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
            foreach (var buff in sortingBuff)
            {
                entityPubCat.UpdateCustSort(new SystemId(buff.SId), Convert.ToInt32(buff.NewVal));
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

        public PubCat.InfoConds Conds { get; set; }
        public List<string> KeywordCols { get; set; }
        public string Keyword { get; set; }
    }
    #endregion

    SearchConds GetSearchConds()
    {
        SearchConds conds = new SearchConds();
        conds.Conds = new PubCat.InfoConds
            (
                DefVal.SIds,
                ConvertLib.ToInts(this._useTgt),
                DefVal.SId,
                DefVal.SId,
                DefVal.Str
            );

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

            if (this.FunctionRight.Maintain)
            {
                htmlTh = new TableHeaderCell();
                htmlTh.Text = "選擇";
                htmlTr.Cells.Add(htmlTh);
            }

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "排序";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "代碼";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "地區";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "最後更新時間";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.All;

            var conds = this.GetSearchConds();

            returner = entityPubCat.GetInfoByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityPubCat.GetInfoByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            this.btnDelete.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            this.btnDelete.Attributes["onclick"] = "javascript:if(!window.confirm('確定要刪除？')){return false;}";

            this.btnUpdateSort.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            this.btnUpdateSort.Attributes["onclick"] = "javascript:if(window.confirm('確定要修改排序？')){ChangingSort();}return false;";

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("edit.aspx", UriKind.Relative);

            List<DataSortingBuff> dataSortingBuff = new List<DataSortingBuff>();

            if (returner.IsCompletedAndContinue)
            {
                var infos = PubCat.Info.Binding(returner.DataSet.Tables[0]);

                int seqNo = this.observerPaging.CurrentPageNumber * flipper.Size - flipper.Size + 1;
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.Name);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "no";
                    htmlTd.Text = string.Format("{0}", seqNo);
                    htmlTr.Cells.Add(htmlTd);

                    if (this.FunctionRight.Maintain)
                    {
                        htmlTd = new TableCell();
                        htmlTd.CssClass = "no";
                        htmlTd.Text = info.IsDef ? string.Empty : string.Format("<input type='checkbox' name='sel_items[]' value='{0}' />", info.SId);
                        htmlTr.Cells.Add(htmlTd);
                    }

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "sort";
                    htmlTd.Text = string.Format("<input type='text' id='txtSort_{0}' name='txtSort_{0}' value='{1}' oldVal='{1}' maxlength='4' class='txt_number' onkeypress=\"this.className='txt_number txt_changed';\" onkeyup='ValidateNumber(this,value);' style='width:50px;'>", info.SId, info.Sort);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Code;
                    htmlTr.Cells.Add(htmlTd);

                    query.Add("sid", info.SId.Value);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='javascript:;' title='{1}' onclick=\"window.open('{0}', '', 'width=900, height=700, top=100, left=100, scrollbars=yes'); return false;\">{1}</a>", query, info.Name);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "updated";
                    htmlTd.Text = ConvertLib.ToStr(info.Mdt, string.Empty, "yyyy-MM-dd HH:mm:ss");
                    htmlTr.Cells.Add(htmlTd);

                    seqNo++;

                    dataSortingBuff.Add(new DataSortingBuff()
                    {
                        SId = info.SId.Value,
                        OldVal = info.Sort.ToString()
                    });
                }
            }

            WebUtilBox.RegisterScript(this, string.Format("sortData = {0};", CustJson.SerializeObject(dataSortingBuff)));
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}