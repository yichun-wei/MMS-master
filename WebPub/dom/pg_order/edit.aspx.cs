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

public partial class dom_pg_order_edit : System.Web.UI.Page
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
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    PGOrderHelper.InfoSet OrigInfo { get; set; }
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    PGOrderHelper.InputInfoSet InputInfo { get; set; }
    #endregion

    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_dom_pg_order_edit_goods_block_ascx> _blockGoodsList = new List<ASP.include_client_dom_pg_order_edit_goods_block_ascx>();

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
        this.PageTitle = "內銷備貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>內銷備貨單</a>"));

        Returner returner = null;
        try
        {
            //備貨單一旦建立了就不能修改
            #region 新增
            if (!this.IsPostBack)
            {
                #region 內銷地區
                //新增時, 以系統使用者被指定的內銷地區為主.
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
                foreach (var domDistInfo in this.MainPage.ActorInfoSet.DomDistInfos)
                {
                    this.lstDomDistList.Items.Add(new ListItem(domDistInfo.Name, domDistInfo.SId.Value));
                }
                #endregion
            }

            this.PageTitle = "新增備貨單";

            breadcrumbs.Add(string.Format("<a href='{0}'>新增備貨單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));

            this.litCdt.Text = DateTime.Today.ToString("yyyy-MM-dd");

            //產生一組新的系統代號並暫存
            this.hidSpecSId.Value = new SystemId().ToString();
            #endregion
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
        #region 重載附件清單
        if (this.IsPostBack)
        {
            if (ViewState[USER_CONTROL_LOAD_SEQ] != null)
            {
                string[] loadSeq = StringLib.SplitSurrounded(ViewState[USER_CONTROL_LOAD_SEQ].ToString(), new LeftRightPair<char, char>('[', ']'));
                for (int i = 0; i < loadSeq.Length; i++)
                {
                    switch (loadSeq[i])
                    {
                        case "GoodsBlock":
                            var blockGoods = (ASP.include_client_dom_pg_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(blockGoods);
                            this.phGoodsList.Controls.Add(blockGoods);
                            break;
                    }
                }
            }
        }
        #endregion

        if (!this.IsPostBack)
        {
            //匯入內銷訂單備貨資訊
            this.SetImportData();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查
        this.btnCreateGoods.Visible = this.btnCreateGoods.Visible ? this.FunctionRight.Maintain : this.btnCreateGoods.Visible;
        this.btnSave.Visible = this.btnSave.Visible ? this.FunctionRight.Maintain : this.btnSave.Visible;
    }

    #region 匯入內銷訂單備貨資訊
    void SetImportData()
    {
        #region 初始匯入資訊
        ISystemId domOrderSId = null;
        ISystemId[] domOrderDetSIds = null;

        var importer = Session[SessionDefine.SystemBuffer] as PGOrderHelper.QuickImporter;
        if (importer == null)
        {
            return;
        }

        domOrderSId = importer.DomOrderSId;
        domOrderDetSIds = importer.SeledDetSIds;

        Session.Remove(SessionDefine.SystemBuffer);
        #endregion

        Returner returner = null;
        try
        {
            var domOrderInfo = DomOrderHelper.Binding(domOrderSId);
            if (domOrderInfo != null)
            {
                this.hidCustomerId.Value = domOrderInfo.Info.CustomerId.ToString();
                this.hidCustomerName.Value = domOrderInfo.Info.CustomerName;
                this.txtEdd.Text = ConvertLib.ToStr(domOrderInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.txtRmk.Text = domOrderInfo.Info.Rmk;

                WebUtilBox.SetListControlSelected(domOrderInfo.Info.DomDistSId.Value, this.lstDomDistList);

                #region 品項資訊
                if (domOrderInfo.DetInfos != null && domOrderInfo.DetInfos.Count > 0)
                {
                    #region 初始載入
                    #region 一般品項
                    if (true)
                    {
                        var generalInfos = domOrderInfo.DetInfos.Where(q => q.Source == 1 && domOrderDetSIds.Contains(q.SId)).ToArray();
                        var block = (ASP.include_client_dom_pg_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_block.ascx");
                        this._blockGoodsList.Add(block);
                        this.phGoodsList.Controls.Add(block);

                        var editInfo = new PGOrderHelper.GoodsEditInfo()
                        {
                            Title = "一般備貨訂單"
                        };

                        var grpDetInfos = generalInfos.GroupBy(q => new { PartNo = q.PartNo }).Select(q => new { DetInfos = q.First() }).Select(q => q.DetInfos).ToArray();

                        block.SetInfo(editInfo);
                        block.IsDefault = true;
                        block.Add(grpDetInfos.Select(q => q.PartNo).ToArray());

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";
                    }
                    #endregion

                    #region 專案報價品項
                    //已存在的專案報價暫存
                    var existedProjQuotes = new List<string>();

                    //專案報價錨點索引
                    var builderProjQuoteIdxes = new StringBuilder();

                    //專案報價品項
                    var projQuoteInfos = domOrderInfo.DetInfos.Where(q => q.Source == 2 && domOrderDetSIds.Contains(q.SId)).GroupBy(q => new { QuoteNumber = q.QuoteNumber }).Select(q => new { QuoteNumber = q.Key.QuoteNumber, GoodsItemList = q.ToArray() });
                    this.phProjQuoteIndex.Visible = projQuoteInfos.Count() > 0;
                    foreach (var projQuoteInfo in projQuoteInfos)
                    {
                        var block = (ASP.include_client_dom_pg_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_block.ascx");
                        this._blockGoodsList.Add(block);
                        this.phGoodsList.Controls.Add(block);

                        var editInfo = new PGOrderHelper.GoodsEditInfo()
                        {
                            Title = projQuoteInfo.QuoteNumber,
                            QuoteNumber = projQuoteInfo.QuoteNumber
                        };

                        var grpDetInfos = projQuoteInfo.GoodsItemList.GroupBy(q => new { PartNo = q.PartNo }).Select(q => new { DetInfos = q.First() }).Select(q => q.DetInfos).ToArray();

                        block.SetInfo(editInfo);
                        block.Add(grpDetInfos.Select(q => q.QuoteItemId).ToArray());

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";

                        //加入已存在的專案報價暫存
                        existedProjQuotes.Add(projQuoteInfo.QuoteNumber);

                        //專案報價錨點索引
                        builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", projQuoteInfo.QuoteNumber);
                    }

                    //專案報價錨點索引
                    this.litProjQuoteIdxes.Text = builderProjQuoteIdxes.ToString();

                    //重整已存在的專案報價暫存
                    this.hidExistedProjQuotes.Value = string.Join(",", existedProjQuotes.GroupBy(q => q).Select(q => q.Key).ToArray());
                    #endregion
                    #endregion
                }
                #endregion

                this.lstDomDistList.Enabled = false;
                this.lnkSelCuster.Visible = false;
                this.phProjQuoteIndex.Visible = true;

                this.phCreateOrderOper.Visible = false;
                this.phOrderGoods.Visible = true;
                this.btnSave.Visible = this.FunctionRight.Maintain;
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 更新客戶資料
    protected void btnResetCustomer_Click(object sender, EventArgs e)
    {
        if (!VerificationLib.IsNumber(this.hidCustomerId.Value))
        {
            this.hidCustomerId.Value = string.Empty;
            this.hidCustomerName.Value = string.Empty;
        }

        Returner returner = null;
        try
        {
            var entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
            returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, ConvertLib.ToLongs(this.hidCustomerId.Value), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
            if (returner.IsCompletedAndContinue)
            {
                var info = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                this.hidCustomerName.Value = info.CustomerName;
                WebPg.RegisterScript("setCustomerName();");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 開始建立備貨單品項
    protected void btnCreateGoods_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(false))
        {
            return;
        }

        this.lstDomDistList.Enabled = false;
        this.lnkSelCuster.Visible = false;
        this.phProjQuoteIndex.Visible = true;

        this.phCreateOrderOper.Visible = false;
        this.phOrderGoods.Visible = true;
        this.btnSave.Visible = this.FunctionRight.Maintain;

        #region 初始一般品項
        var block = (ASP.include_client_dom_pg_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_block.ascx");
        this._blockGoodsList.Add(block);
        this.phGoodsList.Controls.Add(block);

        var editInfo = new PGOrderHelper.GoodsEditInfo()
        {
            Title = "一般備貨訂單"
        };

        block.SetInfo(editInfo);
        block.IsDefault = true;

        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";
        #endregion
    }
    #endregion

    #region 加入專案報價區塊
    protected void btnAddProjQuote_Click(object sender, EventArgs e)
    {
        var seledProjQuotes = this.hidSeledProjQuotes.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledProjQuotes.Length == 0)
        {
            return;
        }

        //已存在的專案報價暫存
        var existedProjQuotes = new List<string>();
        existedProjQuotes.AddRange(this.hidExistedProjQuotes.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

        //專案報價錨點索引
        var builderProjQuoteIdxes = new StringBuilder();

        //已存在的
        foreach (var existed in existedProjQuotes)
        {
            //專案報價錨點索引
            builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", existed);
        }

        //已選擇的
        foreach (var seled in seledProjQuotes)
        {
            //已存在的就不再加入
            if (existedProjQuotes.Contains(seled))
            {
                continue;
            }

            //加入已存在的專案報價暫存
            existedProjQuotes.Add(seled);

            //專案報價錨點索引
            builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", seled);

            //品項區塊
            var block = (ASP.include_client_dom_pg_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_block.ascx");
            this._blockGoodsList.Add(block);
            this.phGoodsList.Controls.Add(block);

            var editInfo = new PGOrderHelper.GoodsEditInfo()
            {
                Title = seled,
                QuoteNumber = seled
            };

            block.SetInfo(editInfo);

            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";
        }

        //專案報價錨點索引
        this.litProjQuoteIdxes.Text = builderProjQuoteIdxes.ToString();

        //重整已存在的專案報價暫存
        this.hidExistedProjQuotes.Value = string.Join(",", existedProjQuotes.GroupBy(q => q).Select(q => q.Key).ToArray());
    }
    #endregion

    #region 繫結輸入
    PGOrderHelper.InputInfoSet BindingInputted(bool bindingAll)
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        this.InputInfo = new PGOrderHelper.InputInfoSet();

        this.InputInfo.Info.SId = new SystemId(this.hidSpecSId.Value);
        this.InputInfo.Info.DomDistSId = ConvertLib.ToSId(this.lstDomDistList.SelectedValue);
        this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
        this.InputInfo.Info.CustomerId = ConvertLib.ToLong(this.hidCustomerId.Value, DefVal.Long);
        this.InputInfo.Info.Rmk = this.txtRmk.Text;

        if (bindingAll)
        {
            #region 備貨單品項
            for (int i = 0; i < this._blockGoodsList.Count; i++)
            {
                var block = this._blockGoodsList[i];
                if (block.Visible)
                {
                    var editInfo = block.GetInfo();
                    this.InputInfo.GoodsEditInfos.Add(editInfo);

                    foreach (var item in editInfo.Items)
                    {
                        this.InputInfo.DetInfos.Add(new PGOrderDet.InputInfo()
                        {
                            SId = item.SId,
                            PGOrderSId = this.InputInfo.Info.SId,
                            Source = i == 0 ? 1 : 2, //第一筆一定為一般品項
                            QuoteNumber = item.QuoteNumber,
                            QuoteItemId = item.QuoteItemId,
                            PartNo = item.PartNo,
                            Qty = item.Qty,
                            Rmk = item.Rmk,
                            Sort = item.SeqNo
                        });
                    }
                }
            }
            #endregion
        }

        return this.InputInfo;
    }
    #endregion

    #region CheckInputted
    bool CheckInputted(bool checkAll)
    {
        this.BindingInputted(checkAll);

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        if (this.InputInfo.Info.DomDistSId == null)
        {
            errMsgs.Add("請選擇「地區」");
        }

        if (this.InputInfo.Info.Edd == null)
        {
            errMsgs.Add("請輸入「預計出貨日」(或格式不正確)");
        }

        if (this.InputInfo.Info.CustomerId == null)
        {
            errMsgs.Add("請選擇「客戶」");
        }

        if (checkAll)
        {
            if (this.InputInfo.GoodsEditInfos.Count == 0)
            {
                //理論上不會進來這邊, 保險起見.
                errMsgs.Add("備貨品項初始失敗");
            }
            else
            {
                #region 品項驗證
                for (int i = 0; i < this.InputInfo.GoodsEditInfos.Count; i++)
                {
                    var editInfo = this.InputInfo.GoodsEditInfos[i];

                    if (i == 0)
                    {
                        //若只有預設品項區塊, 則檢查預設品項區塊中是包含品項; 若有包含專案報價品項, 則預設品項區塊允許沒有品項.
                        if (this.InputInfo.GoodsEditInfos.Count == 1)
                        {
                            //只有預設品項區塊
                            if (editInfo.Items.Count == 0)
                            {
                                errMsgs.Add("未包含任何品項");
                            }
                        }
                    }

                    foreach (var itemInfo in editInfo.Items)
                    {
                        if (string.IsNullOrWhiteSpace(itemInfo.PartNo))
                        {
                            //理論上不會進來這邊, 保險起見.
                            errMsgs.Add(string.Format("{0} 序號 {1} - 未包含料號", editInfo.Title, itemInfo.SeqNo));
                        }

                        if (itemInfo.Qty == null || itemInfo.Qty < 1)
                        {
                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「數量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                        }
                        else
                        {
                            //專案報價品項
                            if (!string.IsNullOrWhiteSpace(itemInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(itemInfo.QuoteItemId))
                            {
                                if (itemInfo.Qty > itemInfo.MaxQty)
                                {
                                    errMsgs.Add(string.Format("{0} 序號 {1} - 「數量」超過限定的最大值", editInfo.Title, itemInfo.SeqNo));
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return false;
        }
        #endregion

        return true;
    }
    #endregion

    #region 儲存備貨單
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(true))
        {
            return;
        }

        JSBuilder.AlertMessage(this, "檢查通過");

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);
                PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

                string odrCode = string.Empty;
                returner = entityPubCat.GetInfo(new ISystemId[] { this.InputInfo.Info.DomDistSId }, IncludeScope.OnlyNotMarkDeleted, new string[] { "CODE" });
                if (returner.IsCompletedAndContinue)
                {
                    odrCode = returner.DataSet.Tables[0].Rows[0]["CODE"].ToString();
                }
                else
                {
                    //理論上不會進來這裡, 保險起見.
                    JSBuilder.AlertMessage(this, "找不到地區資料");
                    return;
                }

                var input = new
                {
                    pgOrderSId = this.InputInfo.Info.SId,
                    orderNo = entityPGOrder.GetNewOrderNo(odrCode),
                    tgtCode = 1,
                    domDistSId = this.InputInfo.Info.DomDistSId,
                    customerId = this.InputInfo.Info.CustomerId.Value,
                    edd = this.InputInfo.Info.Edd,
                    rmk = this.InputInfo.Info.Rmk
                };

                returner = entityPGOrder.Add(actorSId, input.pgOrderSId, input.orderNo, input.tgtCode, input.domDistSId, input.customerId, input.edd, input.rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 備貨單品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        entityPGOrderDet.Add(actorSId, detInfo.SId, detInfo.PGOrderSId, detInfo.Source.Value, detInfo.QuoteNumber, detInfo.QuoteItemId, detInfo.PartNo, detInfo.Qty.Value, detInfo.Rmk, i + 1);

                        //異動記錄
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.PG_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", input.orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.PG_ORDER, input.pgOrderSId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    transaction.Complete();

                    QueryStringParsing query = new QueryStringParsing();
                    query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                    query.Add("sid", input.pgOrderSId.Value);

                    Response.Redirect(query.ToString());
                }
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}