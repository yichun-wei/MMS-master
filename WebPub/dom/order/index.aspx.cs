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

public partial class dom_order_index : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_ORDER";
    string AUTH_NAME = "內銷訂單";

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

        this.PageTitle = "內銷訂單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>內銷訂單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
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

                //狀態
                this.lstStatusList.Items.AddRange(DomOrderHelper.GetDomOrderStatusItems());
                WebUtilBox.SetListControlSelected(Request.QueryString["status"], this.lstStatusList);

                //內銷 ERP 訂單狀態
                this.lstErpOrderStatuList.Items.AddRange(DomOrderHelper.GetDomErpOrderStatusItems());

                //料號更新 - 營管權限
                this.btnUpdateErpInv.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                this.btnUpdateErpInv.OnClientClick = string.Format("javascript:if(!window.confirm('確定要更新料號？')){{return false;}}");
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

        var rightDomPGOrder = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_PG_ORDER");
        this.lnkCreatePGOrder.Visible = this.lnkCreatePGOrder.Visible ? rightDomPGOrder.Maintain : this.lnkCreatePGOrder.Visible;
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
                    DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                    ErpDctRel entityErpDctRel = new ErpDctRel(SystemDefine.ConnInfo);
                    DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);
                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                    DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                    foreach (var sid in systemIds)
                    {
                        returner = entityDomOrder.GetInfo(new ISystemId[] { sid }, IncludeScope.OnlyNotMarkDeleted);
                        if (returner.IsCompletedAndContinue)
                        {
                            var info = DomOrder.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                            ErpDctRel.Info[] headerDiscountInfos = null;
                            DomOrderDet.Info[] domOrderDetInfos = null;

                            #region 表頭折扣
                            if (true) //只是為了不想重複宣告區域變數
                            {
                                returnerTmp = entityErpDctRel.GetInfo(new ErpDctRel.InfoConds(ConvertLib.ToInts((int)ErpDctRel.RelCodeOpt.DomOrder_HeaderDiscount), ConvertLib.ToSIds(info.SId), DefVal.Longs), Int32.MaxValue, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                                if (returnerTmp.IsCompletedAndContinue)
                                {
                                    headerDiscountInfos = ErpDctRel.Info.Binding(returnerTmp.DataSet.Tables[0]);

                                    info.ChildTables.Add(new DataTransChildTable()
                                    {
                                        TableName = DBTableDefine.ERP_DCT_REL,
                                        Rows = headerDiscountInfos
                                    });
                                }
                            }
                            #endregion

                            #region 內銷訂單明細
                            if (true) //只是為了不想重複宣告區域變數
                            {
                                returnerTmp = entityDomOrderDet.GetInfo(new DomOrderDet.InfoConds(DefVal.SIds, info.SId, DefVal.Int, DefVal.SId, DefVal.Str, DefVal.Str, DefVal.Str), Int32.MaxValue, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                                if (returnerTmp.IsCompletedAndContinue)
                                {
                                    domOrderDetInfos = DomOrderDet.Info.Binding(returnerTmp.DataSet.Tables[0]);

                                    info.ChildTables.Add(new DataTransChildTable()
                                    {
                                        TableName = DBTableDefine.DOM_ORDER_DET,
                                        Rows = domOrderDetInfos
                                    });

                                    foreach (var infoTmp in domOrderDetInfos)
                                    {
                                        delPaths.Add(Server.MapPath(string.Format("{0}dom_order_det/{1}/", SystemDefine.UploadRoot, infoTmp.SId)));
                                    }
                                }
                            }
                            #endregion

                            #region 針對已取消的專案報價備貨單品項還原數量 (沒被取消的專案報價備貨單品項就不用還原了)
                            var domOrderDetOfPGInfos = domOrderDetInfos.Where(q => q.Source == 2 && q.PGOrderDetSId != null).ToArray();
                            foreach (var domOrderDetOfPGInfo in domOrderDetOfPGInfos)
                            {
                                returnerTmp = entityPGOrderDet.GetInfoView(new PGOrderDet.InfoViewConds(ConvertLib.ToSIds(domOrderDetOfPGInfo.PGOrderDetSId), DefVal.SId, DefVal.Str, DefVal.Int, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Long, true, false, IncludeScope.OnlyNotMarkDeleted), 1, SqlOrder.Clear);
                                if (returnerTmp.IsCompletedAndContinue)
                                {
                                    var pgOrderDetInfo = PGOrderDet.InfoView.Binding(returnerTmp.DataSet.Tables[0].Rows[0]);

                                    var remaining = pgOrderDetInfo.Qty - domOrderDetOfPGInfo.Qty;

                                    if (remaining < 0)
                                    {
                                        //「專案報價備貨單品項數量 - 內銷訂單品項數量」為負值. 理論上不可能, 但若發生, 發出警示.
                                        JSBuilder.AlertMessage(this, string.Format("料號「{0}」還原備貨單異常 (數量為負值)", domOrderDetOfPGInfo.PartNo));
                                        return;
                                    }

                                    entityPGOrderDet.UpdateQty(actorSId, domOrderDetOfPGInfo.PGOrderDetSId, remaining);
                                }
                            }
                            #endregion

                            //異動記錄
                            string dataTitle = info.OdrNo;
                            entityDataTransLog.Add(actorSId, DBTableDefine.DOM_ORDER, sid, DefVal.Int, "MD", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(info));

                            //僅註解刪除
                            entityDomOrder.SwitchMarkDeleted(actorSId, ConvertLib.ToSIds(sid), true);

                            //僅註刪, 不真正刪除.
                            //entityDomOrder.Delete(actorSId, new ISystemId[] { sid });

                            #region 表頭折扣
                            if (headerDiscountInfos != null && headerDiscountInfos.Length > 0)
                            {
                                //僅註刪, 不真正刪除.
                                //entityErpDctRel.DeleteByUseSId((int)ErpDctRel.RelCodeOpt.DomOrder_HeaderDiscount, ConvertLib.ToSIds(info.SId));
                            }
                            #endregion

                            #region 內銷訂單明細
                            if (domOrderDetInfos != null && domOrderDetInfos.Length > 0)
                            {
                                //僅註刪, 不真正刪除.
                                //entityDomOrderDet.DeleteByDomOrderSId(ConvertLib.ToSIds(info.SId));
                            }
                            #endregion
                        }
                    }

                    transaction.Complete();
                }

                #region 刪除附加資料
                //僅註刪, 不真正刪除.
                //foreach (string path in delPaths)
                //{
                //    try
                //    {
                //        if (Directory.Exists(path))
                //        {
                //            Directory.Delete(path, true);
                //        }
                //    }
                //    catch
                //    {
                //        //不做任何動作
                //    }
                //}
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

    protected void btnUpdateErpInv_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }
        //2017.1.9 如果要料號整批清除重匯，參數要給true-->allImport
        var uptCont = Schedule.ImportErpInv(false);

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

            #region 新增-查詢專案編號 
            //2016.5.18 by Michelle
            //如果專案編號有輸入
            ISystemId[] domSIds = DefVal.SIds;
            ISystemId domSIdPre = DefVal.SId;
            if (!string.IsNullOrWhiteSpace(this.txtQuoteNum.Text.Trim()))
            {
                Returner returner_dtl = null;
                try
                {
                    DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);
                    returner_dtl = entityDomOrderDet.GetInfo(new DomOrderDet.InfoConds(null, null, null, null, this.txtQuoteNum.Text.Trim(), null, null), Int32.MaxValue, SqlOrder.Default, IncludeScope.OnlyNotMarkDeleted, ConvertLib.ToStrs("DOM_ORDER_SID"));
                    var infos = DomOrderDet.Info.Binding(returner_dtl.DataSet.Tables[0]);
                    if (returner_dtl.DataSet.Tables[0].Rows.Count > 0)
                    {
                        domSIds = new ISystemId[infos.Length];
                        for (int i = 0; i < infos.Length; i++)
                        {
                            if (i == 0 || domSIdPre != infos[i].DomOrderSId)
                            {
                               domSIds[i]= infos[i].DomOrderSId; 
                            }
                            domSIdPre = infos[i].DomOrderSId;
                        }
                    }
                    else
                    {
                        //若找不到專案號碼, 則直接使其找不到.
                        domSIds = ConvertLib.ToSIds(SystemId.MaxValue);
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

            conds.Conds = new DomOrder.InfoViewConds
                (
                   domSIds,
                   domDistSIds,
                   DefVal.Strs,
                   ConvertLib.ToInts(this.lstStatusList.SelectedValue),
                   ConvertLib.ToDateTime(this.txtBeginCdt.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndCdt.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtBeginEdd.Text, DefVal.DT),
                   ConvertLib.ToDateTime(this.txtEndEdd.Text, DefVal.DT),
                   ConvertLib.ToInts(this.lstErpOrderStatuList.SelectedValue),
                   false
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

            if (conds.Conds.Statuses != null && conds.Conds.Statuses.Length > 0)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.Statuses[0].ToString(), this.lstStatusList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstStatusList);
            }

            if (conds.Conds.ErpStatuses != null && conds.Conds.ErpStatuses.Length > 0)
            {
                WebUtilBox.SetListControlSelected(conds.Conds.ErpStatuses[0].ToString(), this.lstErpOrderStatuList);
            }
            else
            {
                WebUtilBox.SetListControlSelected(string.Empty, this.lstErpOrderStatuList);
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

            //if (this.FunctionRight.Maintain)
            //{
            //    htmlTh = new TableHeaderCell();
            //    htmlTh.Text = "選擇";
            //    htmlTr.Cells.Add(htmlTh);
            //}

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
            htmlTh.Text = "訂單狀態";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "ERP 訂單狀態";
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
            query.HttpPath = new Uri("edit.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = DomOrder.InfoView.Binding(returner.DataSet.Tables[0]);

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
                    htmlTd.Text = DomOrderHelper.GetDomOrderStatusName(info.Status);
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    switch (info.ErpStatus)
                    {
                        case 3:
                            htmlTd.Text = string.Format("<span class='red'>{0}</span>", DomOrderHelper.GetDomErpOrderStatusName(info.ErpStatus));
                            break;
                        default:
                            htmlTd.Text = DomOrderHelper.GetDomErpOrderStatusName(info.ErpStatus);
                            break;
                    }
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    if (info.DctTotalAmt.HasValue)
                    {
                         //原魁亨寫法
                        // htmlTd.Text = ConvertLib.ToAccounting(info.DctTotalAmt.Value);
                        //2016.7.28 By儀淳 將訂單金額小數位數顯示為兩位
                        //htmlTd.Text = string.Format("{0:N2}", Convert.ToDecimal(info.DctTotalAmt.Value));
                        //2016.7.28 By米雪 上述寫法沒有解決進位的問題，改成以下寫法
                        htmlTd.Text = string.Format("{0:N2}", (double)Math.Round(info.DctTotalAmt.Value, 2));

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

    protected void btnUpdateXSErpOrder_Click(object sender, EventArgs e)
    {
        Returner returner = null;
        try
        {
            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            returner = entityDomOrder.ExecErpOrderUpdate();
            if (returner.IsCompletedAndContinue)
            {
                JSBuilder.AlertMessage(this, true, "價格更新已完成");
            }
            else
            {
                JSBuilder.AlertMessage(this, true, "沒有任何價格需要更新");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
}