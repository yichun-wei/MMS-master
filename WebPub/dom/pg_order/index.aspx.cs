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

public partial class dom_pg_order_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_PG_ORDER";
    string AUTH_NAME = "內銷備貨單";

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

        this.PageTitle = "內銷備貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>內銷備貨單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

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

                WebUtilBox.SetListControlSelected(Request.QueryString["status"], this.lstStatusList);
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
        QueryStringParsing query = new QueryStringParsing();
        query.HttpPath = new Uri("edit.aspx", UriKind.Relative);
        this.lnkAdd.NavigateUrl = query.ToString();
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
        this.lnkAdd.Visible = this.lnkAdd.Visible ? this.FunctionRight.Maintain : this.lnkAdd.Visible;
        //this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
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
            Returner returnerTmp = null;
            try
            {
                List<string> delPaths = new List<string>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);
                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                    DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                    foreach (var sid in systemIds)
                    {
                        returner = entityPGOrder.GetInfo(new ISystemId[] { sid }, IncludeScope.OnlyNotMarkDeleted);
                        if (returner.IsCompletedAndContinue)
                        {
                            var info = PGOrder.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                            PGOrderDet.Info[] domOrderDetInfos = null;

                            #region 備貨單明細
                            if (true) //只是為了不想重複宣告區域變數
                            {
                                returnerTmp = entityPGOrderDet.GetInfo(new PGOrderDet.InfoConds(DefVal.SIds, info.SId, DefVal.Int, DefVal.Str, DefVal.Str, DefVal.Str), Int32.MaxValue, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                                if (returnerTmp.IsCompletedAndContinue)
                                {
                                    domOrderDetInfos = PGOrderDet.Info.Binding(returnerTmp.DataSet.Tables[0]);

                                    info.ChildTables.Add(new DataTransChildTable()
                                    {
                                        TableName = DBTableDefine.PG_ORDER_DET,
                                        Rows = domOrderDetInfos
                                    });

                                    foreach (var infoTmp in domOrderDetInfos)
                                    {
                                        delPaths.Add(Server.MapPath(string.Format("{0}pg_order_det/{1}/", SystemDefine.UploadRoot, infoTmp.SId)));
                                    }
                                }
                            }
                            #endregion

                            //異動記錄
                            string dataTitle = info.OdrNo;
                            entityDataTransLog.Add(actorSId, DBTableDefine.PG_ORDER, sid, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(info));

                            entityPGOrder.Delete(actorSId, new ISystemId[] { sid });

                            #region 備貨單明細
                            if (domOrderDetInfos != null && domOrderDetInfos.Length > 0)
                            {
                                entityPGOrderDet.DeleteByPGOrderSId(ConvertLib.ToSIds(info.SId));
                            }
                            #endregion
                        }
                    }

                    transaction.Complete();
                }

                #region 刪除附加資料
                foreach (string path in delPaths)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                    }
                    catch
                    {
                        //不做任何動作
                    }
                }
                #endregion
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

        public PGOrder.InfoViewConds Conds { get; set; }
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
            #region 新增-查詢專案編號 
            //2016.5.18 by Michelle
            //如果專案編號有輸入
            ISystemId[] PGSIds = DefVal.SIds;
            ISystemId PGSIdPre = DefVal.SId;
            if (!string.IsNullOrWhiteSpace(this.txtQuoteNum.Text.Trim()))
            {
                Returner returner_dtl = null;
                try
                {
                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                    returner_dtl = entityPGOrderDet.GetInfo(new PGOrderDet.InfoConds(null, null, null,  this.txtQuoteNum.Text.Trim(), null, null), Int32.MaxValue, SqlOrder.Default, IncludeScope.OnlyNotMarkDeleted, ConvertLib.ToStrs("PG_ORDER_SID"));
                    var infos = PGOrderDet.Info.Binding(returner_dtl.DataSet.Tables[0]);
                    if (returner_dtl.DataSet.Tables[0].Rows.Count > 0)
                    {
                        PGSIds = new ISystemId[infos.Length];
                        for (int i = 0; i < infos.Length; i++)
                        {
                            if (i == 0 || PGSIdPre != infos[i].PGOrderSId)
                            {
                                PGSIds[i] = infos[i].PGOrderSId;
                            }
                            PGSIdPre = infos[i].PGOrderSId;
                        }
                    }
                    else
                    {
                        //若找不到專案號碼, 則直接使其找不到.
                        PGSIds = ConvertLib.ToSIds(SystemId.MaxValue);
                    }
                }
                finally
                {
                    if (returner_dtl != null) returner_dtl.Dispose();
                }
            }
            #endregion
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

            bool? isCancel = DefVal.Bool;
            bool? hasUsed = DefVal.Bool;

            //1:未使用 2:已使用 3:已取消
            switch (ConvertLib.ToInt(this.lstStatusList.SelectedValue, DefVal.Int))
            {
                case 1:
                    isCancel = false;
                    hasUsed = false;
                    break;
                case 2:
                    isCancel = false;
                    hasUsed = true;
                    break;
                case 3:
                    isCancel = true;
                    break;
            }

            conds.Conds = new PGOrder.InfoViewConds
                (
                   PGSIds,
                   domDistSIds,
                   DefVal.Long,
                   ConvertLib.ToDateTime(this.txtBeginCdt.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndCdt.Text, DefVal.DT),
                   DefVal.DT,
                   DefVal.DT,
                   isCancel,
                   hasUsed
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
                WebUtilBox.SetListControlSelected(conds.Conds.DomDistSIds[0].Value, this.lstDomDistList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstDomDistList);
            }

            //1:未使用 2:已使用 3:已取消
            string status = string.Empty;
            if (conds.Conds.HasUsed == false)
            {
                status = "1";
            }
            else if (conds.Conds.HasUsed == true)
            {
                status = "2";
            }
            else if (conds.Conds.IsCancel == true)
            {
                status = "3";
            }
            WebUtilBox.SetListControlSelected(status, this.lstStatusList);

            this.txtBeginCdt.Text = ConvertLib.ToStr(conds.Conds.BeginCdt, string.Empty, "yyyy-MM-dd");
            this.txtEndCdt.Text = ConvertLib.ToStr(conds.Conds.EndCdt, string.Empty, "yyyy-MM-dd");

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
            htmlTh.Text = "備貨單單號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "備註";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "備貨單日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "備貨單狀態";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.OnlyNotMarkDeleted;

            var conds = this.GetSearchConds();

            returner = entityPGOrder.GetInfoViewByCompoundSearchCount(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, dataScope);

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
            returner = entityPGOrder.GetInfoViewByCompoundSearch(conds.Conds, conds.KeywordCols.ToArray(), conds.Keyword, false, flipper, sorting, dataScope);

            //this.btnDelete.Visible = (returner.IsCompletedAndContinue && returner.DataSet.Tables[0].Rows.Count > 0);
            //this.btnDelete.Attributes["onclick"] = "javascript:if(!window.confirm('確定要刪除？')){return false;}";

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = PGOrder.InfoView.Binding(returner.DataSet.Tables[0]);

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

                    //if (this.FunctionRight.Maintain)
                    //{
                    //    htmlTd = new TableCell();
                    //    htmlTd.CssClass = "no";
                    //    htmlTd.Text = string.Format("<input type='checkbox' name='sel_items[]' value='{0}' />", info.SId);
                    //    htmlTr.Cells.Add(htmlTd);
                    //}

                    query.Add("sid", info.SId.Value);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='{0}' title='{1}'>{1}</a>", query, info.OdrNo);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = WebUtilBox.ConvertNewLineToHtmlBreak(info.Rmk);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<span class='truncate' length='6' title='{0}'>{0}</span>", info.CustomerName);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.Cdt, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = PGOrderHelper.GetOrderStatusName(info);
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