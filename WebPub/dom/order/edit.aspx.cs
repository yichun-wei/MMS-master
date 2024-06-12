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

public partial class dom_order_edit : System.Web.UI.Page
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
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    DomOrderHelper.InfoSet OrigInfo { get; set; }
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    DomOrderHelper.InputInfoSet InputInfo { get; set; }
    #endregion

    ISystemId _domOrderSId;

    List<ASP.include_client_dom_order_header_discount_block_ascx> _blockHeaderDiscountList = new List<ASP.include_client_dom_order_header_discount_block_ascx>();

    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_dom_order_edit_goods_block_ascx> _blockGoodsList = new List<ASP.include_client_dom_order_edit_goods_block_ascx>();

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

        this._domOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "內銷訂單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>內銷訂單</a>"));

        this.btnDelete.OnClientClick = string.Format("javascript:if(!window.confirm('確定刪除訂單？')){{return false;}}");
        this.btnToBizMgtDept.OnClientClick = string.Format("javascript:if(!window.confirm('確定提交審核？')){{return false;}}");
        this.btnCreatePGOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定儲存後同時建立備貨單？')){{return false;}}");

        Returner returner = null;
        try
        {
            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            if (!this.IsPostBack)
            {
                #region 倉庫
                var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                foreach (var erpWhseInfo in erpWhseInfos)
                {
                    this.lstWhseList.Items.Add(new ListItem(erpWhseInfo.SecondaryInventoryName, erpWhseInfo.SecondaryInventoryName));
                }
                #endregion

                //產品別
                this.lstProdTypeList.Items.AddRange(DomOrderHelper.GetDomOrderProdTypeItems());

                #region 貨運方式
                returner = entityPubCat.GetTopInfo(new PubCat.TopInfoConds(DefVal.SIds, (int)PubCat.UseTgtOpt.DomFreightWay, DefVal.SId), Int32.MaxValue, new SqlOrder("SORT", Sort.Descending), IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = PubCat.Info.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        this.lstFreightWayList.Items.Add(new ListItem(info.Name, info.SId.Value));
                    }
                }
                #endregion
            }

            if (this._domOrderSId != null)
            {
                #region 修改
                if (!this.IsPostBack)
                {
                    #region 內銷地區
                    /********************************************************************************
                     * 修改時, 由於會鎖住內銷地區的選擇, 故而取全部內銷地區.
                     * 若該系統使用者已無訂單指定的內銷地區, 則不再列入處理, 訂單一旦指定內銷地區則指定.
                     * 在進入訂單時, 若進入的系統使用者無該內銷地區的權限, 目前先不擋.
                    ********************************************************************************/
                    returner = entityPubCat.GetTopInfo(new PubCat.TopInfoConds(DefVal.SIds, (int)PubCat.UseTgtOpt.DomDist, DefVal.SId), Int32.MaxValue, new SqlOrder("SORT", Sort.Descending), IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = PubCat.Info.Binding(returner.DataSet.Tables[0]);

                        foreach (var info in infos)
                        {
                            this.lstDomDistList.Items.Add(new ListItem(info.Name, info.SId.Value));
                        }
                    }
                    #endregion
                }

                if (this.SetEditData(this._domOrderSId))
                {
                    this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                    breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                    //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                    switch (this.OrigInfo.Info.Status)
                    {
                        case 0:
                            break;
                        default:
                            QueryStringParsing query = new QueryStringParsing();
                            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                            query.Add("sid", this.OrigInfo.Info.SId.Value);
                            Response.Redirect(query.ToString());
                            return false;
                    }

                    this.lstDomDistList.Enabled = false;
                    //允許在草稿編輯時切換倉庫
                    this.lstWhseList.Enabled = this.OrigInfo.Info.Status == 0;
                    this.lnkSelCuster.Visible = false;
                    this.phProjQuoteIndex.Visible = true;

                    this.phOrderDetail.Visible = true;
                    this.phOrderGoods.Visible = true;

                    //操作控制
                    this.btnToDraft.Visible = this.OrigInfo.Info.Status == 0;
                }
                else
                {
                    Response.Redirect("index.aspx");
                    return false;
                }
                #endregion
            }
            else
            {
                #region 新增
                if (!this.IsPostBack)
                {
                    #region 內銷地區
                    //新增時, 以系統使用者被指定的內銷地區為主.
                    foreach (var domDistInfo in this.MainPage.ActorInfoSet.DomDistInfos)
                    {
                        this.lstDomDistList.Items.Add(new ListItem(domDistInfo.Name, domDistInfo.SId.Value));
                    }
                    #endregion
                }

                this.PageTitle = "新增訂單";

                breadcrumbs.Add(string.Format("<a href='{0}'>新增訂單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));

                this.litCdt.Text = DateTime.Today.ToString("yyyy-MM-dd");

                this.phCreateOrderOper.Visible = true;

                //產生一組新的系統代號並暫存
                this.hidSpecSId.Value = new SystemId().ToString();
                #endregion
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
                            var blockGoods = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(blockGoods);
                            this.phGoodsList.Controls.Add(blockGoods);
                            break;
                    }
                }
            }
        }
        #endregion
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //還原值
        this.lblPTTotalAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidPTTotalAmt.Value, DefVal.Float), string.Empty);
        this.lblTaxAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidTaxAmt.Value, DefVal.Float), string.Empty);
        this.lblDctAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidDctAmt.Value, DefVal.Float), string.Empty);
        this.lblDctTotalAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidDctTotalAmt.Value, DefVal.Float), string.Empty);

        //權限操作檢查
        this.btnCreateOrder.Visible = this.btnCreateOrder.Visible ? this.FunctionRight.Maintain : this.btnCreateOrder.Visible;
        this.btnToDraft.Visible = this.btnToDraft.Visible ? this.FunctionRight.Maintain : this.btnToDraft.Visible;
        this.btnToBizMgtDept.Visible = this.btnToBizMgtDept.Visible ? this.FunctionRight.Maintain : this.btnToBizMgtDept.Visible;
        this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;

        var rightDomPGOrder = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_PG_ORDER");
        this.btnCreatePGOrder.Visible = this.btnCreatePGOrder.Visible ? rightDomPGOrder.Maintain : this.btnCreatePGOrder.Visible;

        //重新檢查所有在手量
        WebUtilBox.RegisterScript(this, "orderItemHelper.checkAllOnHand();");
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = DomOrderHelper.Binding(systemId);
            if (this.OrigInfo != null)
            {
                this.hidSpecSId.Value = this.OrigInfo.Info.SId.Value; //暫存系統代號。

                this.litCdt.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy-MM-dd");
                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.litStatus.Text = DomOrderHelper.GetDomOrderStatusName(this.OrigInfo.Info.Status);
                this.txtRmk.Text = this.OrigInfo.Info.Rmk;

                //「價目表 ID」只是為了給品項取牌價用
                this.hidPriceListId.Value = ConvertLib.ToStr(this.OrigInfo.Info.PriceListId, string.Empty);

                #region 表頭折扣
                if (this.OrigInfo.Info.PriceListId.HasValue)
                {
                    ErpDct entityErpDct = new ErpDct(SystemDefine.ConnInfo);
                    returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, DefVal.Longs, ConvertLib.ToLongs(this.OrigInfo.Info.PriceListId.Value), ConvertLib.ToInts(1)), Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var erpDctInfos = ErpDct.Info.Binding(returner.DataSet.Tables[0]);
                        foreach (var erpDctInfo in erpDctInfos)
                        {
                            var block = (ASP.include_client_dom_order_header_discount_block_ascx)this.LoadControl("~/include/client/dom/order/header_discount_block.ascx");
                            this._blockHeaderDiscountList.Add(block);
                            this.phHeaderDiscount.Controls.Add(block);
                            block.DiscountId = erpDctInfo.DiscountId;
                            block.Name = erpDctInfo.DiscountName;

                            var headerDiscountInfo = this.OrigInfo.HeaderDiscountInfos.Where(q => q.DiscountId == erpDctInfo.DiscountId).DefaultIfEmpty(null).SingleOrDefault();
                            if (headerDiscountInfo != null)
                            {
                                block.Seled = true;
                                block.Discount = MathLib.Round(headerDiscountInfo.Discount * 100, 2).ToString();
                            }
                        }
                    }
                }
                #endregion

                #region 客戶資訊
                this.hidCustomerId.Value = this.OrigInfo.Info.CustomerId.ToString();
                this.litCustomerNumber.Text = this.OrigInfo.Info.CustomerNumber;
                this.lblCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.hidCustomerName.Value = this.OrigInfo.Info.CustomerName;
                this.lblCustomerConName.Text = this.OrigInfo.Info.CustomerConName;
                this.lblCustomerTel.Text = ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerTel);
                this.lblCustomerFax.Text = ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerFax);
                this.lblCustomerAddr.Text = this.OrigInfo.Info.CustomerAddr;
                #endregion

                #region 收貨人資訊
                this.txtRcptCusterName.Text = this.OrigInfo.Info.RcptCusterName;
                this.txtRcptName.Text = this.OrigInfo.Info.RcptName;
                this.txtRcptTel.Text = this.OrigInfo.Info.RcptTel;
                this.txtRcptFax.Text = this.OrigInfo.Info.RcptFax;
                this.txtRcptAddr.Text = this.OrigInfo.Info.RcptAddr;

                WebUtilBox.SetListControlSelected(ConvertLib.ToStr(this.OrigInfo.Info.FreightWaySId), this.lstFreightWayList);
                #endregion

                #region 品項資訊
                if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
                {
                    if (!this.IsPostBack)
                    {
                        #region 初始載入
                        //不顯示在手量 (先恢復試速度)
                        ////先取得所有品項的在手量
                        //var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);
                        //改為取得所有倉庫的在手量
                        //先取得所有倉庫
                        var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                        var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), DefVal.Str);

                        //先取得所有品項的價目表
                        var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                        if (this.OrigInfo.Info.PriceListId.HasValue)
                        {
                            priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Select(q => q.PartNo).ToArray());
                        }

                        #region 一般品項
                        if (true)
                        {
                            var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();
                            var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(block);
                            this.phGoodsList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsEditInfo()
                            {
                                Title = "一般訂單"
                            };

                            for (int i = 0; i < generalInfos.Length; i++)
                            {
                                var detInfo = generalInfos[i];

                                var itemEditInfo = new DomOrderHelper.GoodsItemEditInfo()
                                {
                                    SId = detInfo.SId,
                                    SeqNo = i + 1,
                                    PartNo = detInfo.PartNo,
                                    PGOrderOdrNo = detInfo.PGOrderOdrNo,
                                    PGOrderDetSId = detInfo.PGOrderDetSId,
                                    QuoteNumber = detInfo.QuoteNumber,
                                    QuoteItemId = detInfo.QuoteItemId,
                                    Qty = detInfo.Qty,
                                    UnitPrice = detInfo.UnitPrice,
                                    Discount = detInfo.Discount,
                                    PaidAmt = detInfo.PaidAmt,
                                    Rmk = detInfo.Rmk
                                };

                                //不顯示在手量 (先恢復試速度)
                                //改為取得所有倉庫的在手量
                                //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                                itemEditInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == this.OrigInfo.Info.Whse && q.Segment1 == detInfo.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                                itemEditInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, detInfo.PartNo);

                                itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                                editInfo.Items.Add(itemEditInfo);
                            }

                            block.SetInfo(editInfo);
                            block.IsDefault = true;
                        }
                        #endregion

                        #region 專案報價品項
                        //已存在的專案報價暫存
                        var existedProjQuotes = new List<string>();

                        //專案報價錨點索引
                        var builderProjQuoteIdxes = new StringBuilder();

                        //專案報價品項
                        var projQuoteInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).GroupBy(q => new { QuoteNumber = q.QuoteNumber }).Select(q => new { QuoteNumber = q.Key.QuoteNumber, GoodsItemList = q.ToArray() });
                        this.phProjQuoteIndex.Visible = projQuoteInfos.Count() > 0;
                        foreach (var projQuoteInfo in projQuoteInfos)
                        {
                            var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(block);
                            this.phGoodsList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsEditInfo()
                            {
                                Title = projQuoteInfo.QuoteNumber,
                                QuoteNumber = projQuoteInfo.QuoteNumber
                            };

                            for (int i = 0; i < projQuoteInfo.GoodsItemList.Length; i++)
                            {
                                var detInfo = projQuoteInfo.GoodsItemList[i];

                                var itemEditInfo = new DomOrderHelper.GoodsItemEditInfo()
                                {
                                    SId = detInfo.SId,
                                    SeqNo = i + 1,
                                    PartNo = detInfo.PartNo,
                                    PGOrderOdrNo = detInfo.PGOrderOdrNo,
                                    PGOrderDetSId = detInfo.PGOrderDetSId,
                                    QuoteNumber = detInfo.QuoteNumber,
                                    QuoteItemId = detInfo.QuoteItemId,
                                    Qty = detInfo.Qty,
                                    UnitPrice = detInfo.UnitPrice,
                                    Discount = detInfo.Discount,
                                    PaidAmt = detInfo.PaidAmt,
                                    Rmk = detInfo.Rmk
                                };

                                //不顯示在手量 (先恢復試速度)
                                //改為取得所有倉庫的在手量
                                //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                                itemEditInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == this.OrigInfo.Info.Whse && q.Segment1 == detInfo.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                                itemEditInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, detInfo.PartNo);

                                itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                                editInfo.Items.Add(itemEditInfo);
                            }

                            block.SetInfo(editInfo);

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
                    else
                    {
                        #region 重載附件清單
                        #region 一般品項
                        if (true)
                        {
                            var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();
                            var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(block);
                            this.phGoodsList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsEditInfo()
                            {
                                Title = "一般訂單"
                            };

                            for (int i = 0; i < generalInfos.Length; i++)
                            {
                                var detInfo = generalInfos[i];

                                var itemEditInfo = new DomOrderHelper.GoodsItemEditInfo()
                                {
                                    SId = detInfo.SId,
                                    SeqNo = i + 1,
                                    PartNo = detInfo.PartNo,
                                    PGOrderOdrNo = detInfo.PGOrderOdrNo,
                                    PGOrderDetSId = detInfo.PGOrderDetSId,
                                    QuoteNumber = detInfo.QuoteNumber,
                                    QuoteItemId = detInfo.QuoteItemId,
                                    Qty = detInfo.Qty,
                                    UnitPrice = detInfo.UnitPrice,
                                    Discount = detInfo.Discount,
                                    PaidAmt = detInfo.PaidAmt,
                                    Rmk = detInfo.Rmk
                                };

                                editInfo.Items.Add(itemEditInfo);
                            }

                            block.SetInfo(editInfo);
                            block.IsDefault = true;
                        }
                        #endregion

                        #region 專案報價品項
                        //已存在的專案報價暫存
                        var existedProjQuotes = new List<string>();

                        //專案報價錨點索引
                        var builderProjQuoteIdxes = new StringBuilder();

                        //專案報價品項
                        var projQuoteInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).GroupBy(q => new { QuoteNumber = q.QuoteNumber }).Select(q => new { QuoteNumber = q.Key.QuoteNumber, GoodsItemList = q.ToArray() });
                        this.phProjQuoteIndex.Visible = projQuoteInfos.Count() > 0;
                        foreach (var projQuoteInfo in projQuoteInfos)
                        {
                            var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(block);
                            this.phGoodsList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsEditInfo()
                            {
                                Title = projQuoteInfo.QuoteNumber,
                                QuoteNumber = projQuoteInfo.QuoteNumber
                            };

                            for (int i = 0; i < projQuoteInfo.GoodsItemList.Length; i++)
                            {
                                var detInfo = projQuoteInfo.GoodsItemList[i];

                                var itemEditInfo = new DomOrderHelper.GoodsItemEditInfo()
                                {
                                    SId = detInfo.SId,
                                    SeqNo = i + 1,
                                    PartNo = detInfo.PartNo,
                                    PGOrderOdrNo = detInfo.PGOrderOdrNo,
                                    PGOrderDetSId = detInfo.PGOrderDetSId,
                                    QuoteNumber = detInfo.QuoteNumber,
                                    QuoteItemId = detInfo.QuoteItemId,
                                    Qty = detInfo.Qty,
                                    UnitPrice = detInfo.UnitPrice,
                                    Discount = detInfo.Discount,
                                    PaidAmt = detInfo.PaidAmt,
                                    Rmk = detInfo.Rmk
                                };

                                editInfo.Items.Add(itemEditInfo);
                            }

                            block.SetInfo(editInfo);

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
                }
                else
                {
                    //尚未建立品項
                    #region 初始一般品項
                    var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                    this._blockGoodsList.Add(block);
                    this.phGoodsList.Controls.Add(block);

                    var editInfo = new DomOrderHelper.GoodsEditInfo()
                    {
                        Title = "一般訂單"
                    };

                    block.SetInfo(editInfo);
                    block.IsDefault = true;
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.hidPTTotalAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.PTTotalAmt, string.Empty);
                this.hidTaxAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.TaxAmt, string.Empty);
                this.hidDctAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.DctAmt, string.Empty);
                this.hidDctTotalAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.DctTotalAmt, string.Empty);
                #endregion

                WebUtilBox.SetListControlSelected(this.OrigInfo.Info.DomDistSId.Value, this.lstDomDistList);
                WebUtilBox.SetListControlSelected(this.OrigInfo.Info.Whse, this.lstWhseList);
                WebUtilBox.SetListControlSelected(this.OrigInfo.Info.ProdType.ToString(), this.lstProdTypeList);

                //ERP 相關
                this.litErpOrderNumber.Text = this.OrigInfo.Info.ErpOrderNumber;
                this.litErpStatus.Text = DomOrderHelper.GetDomErpOrderStatusName(this.OrigInfo.Info.ErpStatus);
                this.litErpShipNumber.Text = this.OrigInfo.Info.ErpShipNumber;

                return true;
            }
            else
            {
                return false;
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
                this.hidPriceListId.Value = ConvertLib.ToStr(info.PriceListId, string.Empty);
                this.litSalesName.Text = info.SalesName;
                WebPg.RegisterScript("setCustomerName();");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
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

        Returner returner = null;
        try
        {
            //是否預設載入全部品項 (一開始不預設載入全部品項, 後來改成要預設載入, 後來又改成不要預設載入... 設變數先保留預設載入的, 以免日後又...)
            bool isPreloadAllItems = false;

            if (isPreloadAllItems)
            {
                #region 預設載入全部品項
                #region 取得選擇的專案報價全部品項
                ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

                var conds = new ProjQuote.InfoViewConds
                    (
                       seledProjQuotes,
                       DefVal.Strs,
                       DefVal.Str
                    );

                ProjQuote.InfoView[] projQuoteInfos = new ProjQuote.InfoView[0];
                returner = entityProjQuote.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
                if (returner.IsCompletedAndContinue)
                {
                    projQuoteInfos = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0]);
                }
                #endregion

                #region 初始品項的相關資訊
                //不顯示在手量 (先恢復試速度)
                //先取得所有品項的在手量
                ErpHelper.ErpWhseInfo[] erpWhseInfos = new ErpHelper.ErpWhseInfo[0];
                ErpHelper.OnHandInfo[] onHandInfos = new ErpHelper.OnHandInfo[0];

                ////先取得所有品項的價目表
                //var priceBookInfos = new ErpHelper.PriceBookInfo[0];

                if (projQuoteInfos.Length > 0)
                {
                    //改為取得所有倉庫的在手量
                    //onHandInfos = ErpHelper.GetOnHandInfo(projQuoteInfos.Where(q => !string.IsNullOrWhiteSpace(q.ProductId)).Select(q => q.ProductId).ToArray(), this.OrigInfo.Info.Whse);
                    //先取得所有倉庫
                    erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                    onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), DefVal.Str);

                    //if (this.OrigInfo.Info.PriceListId.HasValue)
                    //{
                    //    priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Select(q => q.PartNo).ToArray());
                    //}
                }
                #endregion

                //已選擇的
                foreach (var seled in seledProjQuotes)
                {
                    //已存在的就不再加入
                    if (existedProjQuotes.Contains(seled))
                    {
                        continue;
                    }

                    //加入已存在的專案報價暫存 (留在外面, 就算區塊被略過, 也讓專案報價不能再選.)
                    existedProjQuotes.Add(seled);

                    //品項區塊
                    var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");

                    #region 預設帶入全部品項
                    var filterProjQuoteInfos = projQuoteInfos.Where(q => q.QuoteNumber == seled).ToArray();

                    //只有可用數量大於 0, 以及在訂單明細有該筆資料的才顯示.
                    filterProjQuoteInfos = filterProjQuoteInfos.Where(q => ((q.Quantity - q.PGOrderUseQty - q.DomOrderUseQty) > 0) || (this._domOrderSId != null && this.OrigInfo.DetInfos.Exists(det => det.QuoteNumber == q.QuoteNumber && det.QuoteItemId == q.QuoteItemId))).ToArray();
                    if (filterProjQuoteInfos.Length > 0)
                    {
                        ////加入已存在的專案報價暫存
                        //existedProjQuotes.Add(seled);

                        //專案報價錨點索引
                        builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", seled);

                        this._blockGoodsList.Add(block);
                        this.phGoodsList.Controls.Add(block);
                    }
                    else
                    {
                        //若沒有任何品項, 則略過.
                        continue;
                    }

                    var editInfo = new DomOrderHelper.GoodsEditInfo()
                    {
                        Title = seled,
                        QuoteNumber = seled
                    };

                    for (int i = 0; i < filterProjQuoteInfos.Length; i++)
                    {
                        var projQuoteInfo = filterProjQuoteInfos[i];

                        var itemEditInfo = new DomOrderHelper.GoodsItemEditInfo()
                        {
                            SeqNo = i + 1,
                            PartNo = projQuoteInfo.ProductId,
                            QuoteNumber = projQuoteInfo.QuoteNumber,
                            QuoteItemId = projQuoteInfo.QuoteItemId,
                            ListPrice = projQuoteInfo.UnitPrice,
                            Discount = projQuoteInfo.Discount
                        };

                        //不顯示在手量 (先恢復試速度)
                        //改為取得所有倉庫的在手量
                        //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, projQuoteInfo.ProductId);
                        itemEditInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == this.OrigInfo.Info.Whse && q.Segment1 == projQuoteInfo.ProductId).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                        itemEditInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, projQuoteInfo.ProductId);

                        //itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, projQuoteInfo.ProductId);

                        editInfo.Items.Add(itemEditInfo);
                    }

                    block.SetInfo(editInfo, true);

                    ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 不預設載入全部品項
                //已選擇的
                foreach (var seled in seledProjQuotes)
                {
                    //已存在的就不再加入
                    if (existedProjQuotes.Contains(seled))
                    {
                        continue;
                    }

                    var editInfo = new DomOrderHelper.GoodsEditInfo()
                    {
                        Title = seled,
                        QuoteNumber = seled
                    };

                    //品項區塊
                    var block = (ASP.include_client_dom_order_edit_goods_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_block.ascx");
                    this._blockGoodsList.Add(block);
                    this.phGoodsList.Controls.Add(block);
                    block.SetInfo(editInfo, true);

                    ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";

                    //加入已存在的專案報價暫存
                    existedProjQuotes.Add(seled);

                    //專案報價錨點索引
                    builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", seled);
                }
                #endregion
            }

            //專案報價錨點索引
            this.litProjQuoteIdxes.Text = builderProjQuoteIdxes.ToString();

            //重整已存在的專案報價暫存
            this.hidExistedProjQuotes.Value = string.Join(",", existedProjQuotes.GroupBy(q => q).Select(q => q.Key).ToArray());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 繫結輸入
    DomOrderHelper.InputInfoSet BindingInputted()
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        this.InputInfo = new DomOrderHelper.InputInfoSet();

        this.InputInfo.Info.SId = new SystemId(this.hidSpecSId.Value);
        this.InputInfo.Info.DomDistSId = ConvertLib.ToSId(this.lstDomDistList.SelectedValue);
        this.InputInfo.Info.Whse = this.lstWhseList.SelectedValue;
        this.InputInfo.Info.ProdType = ConvertLib.ToInt(this.lstProdTypeList.SelectedValue, DefVal.Int);
        this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.litEdd.Text, DefVal.DT);
        this.InputInfo.Info.CustomerId = ConvertLib.ToLong(this.hidCustomerId.Value, DefVal.Long);
        this.InputInfo.Info.Rmk = this.txtRmk.Text;

        #region 表頭折扣
        foreach (var block in this._blockHeaderDiscountList)
        {
            if (block.Seled)
            {
                var erpDctRel = new ErpDctRel.InputInfo();
                this.InputInfo.HeaderDiscountInfos.Add(erpDctRel);

                erpDctRel.SId = new SystemId();
                erpDctRel.RelCode = (int)ErpDctRel.RelCodeOpt.DomOrder_HeaderDiscount;
                erpDctRel.UseSId = this.InputInfo.Info.SId;
                erpDctRel.DiscountId = block.DiscountId;
                erpDctRel.Discount = ConvertLib.ToSingle(block.Discount, DefVal.Float);
            }
        }
        #endregion

        #region 收貨人資訊
        this.InputInfo.Info.RcptName = this.txtRcptName.Text;
        this.InputInfo.Info.RcptCusterName = this.txtRcptCusterName.Text;
        this.InputInfo.Info.RcptTel = this.txtRcptTel.Text;
        this.InputInfo.Info.RcptFax = this.txtRcptFax.Text;
        this.InputInfo.Info.RcptAddr = this.txtRcptAddr.Text;
        this.InputInfo.Info.FreightWaySId = ConvertLib.ToSId(this.lstFreightWayList.SelectedValue);
        #endregion

        #region 訂單金額
        this.InputInfo.Info.PTTotalAmt = ConvertLib.ToSingle(this.hidPTTotalAmt.Value, DefVal.Float);
        this.InputInfo.Info.TaxAmt = ConvertLib.ToSingle(this.hidTaxAmt.Value, DefVal.Float);
        this.InputInfo.Info.DctAmt = ConvertLib.ToSingle(this.hidDctAmt.Value, DefVal.Float);
        this.InputInfo.Info.DctTotalAmt = ConvertLib.ToSingle(this.hidDctTotalAmt.Value, DefVal.Float);
        #endregion

        #region 訂單品項
        for (int i = 0; i < this._blockGoodsList.Count; i++)
        {
            var block = this._blockGoodsList[i];
            if (block.Visible)
            {
                var editInfo = block.GetInfo();
                this.InputInfo.GoodsEditInfos.Add(editInfo);

                foreach (var item in editInfo.Items)
                {
                    if (item.IsSeled)
                    {
                        this.InputInfo.SeledDetInfos.Add(item.SId);
                    }

                    var detInfo = new DomOrderDet.InputInfo()
                    {
                        SId = item.SId,
                        DomOrderSId = this.InputInfo.Info.SId,
                        Source = i == 0 ? 1 : 2, //第一筆一定為一般品項
                        PGOrderDetSId = item.PGOrderDetSId,
                        QuoteNumber = item.QuoteNumber,
                        QuoteItemId = item.QuoteItemId,
                        PartNo = item.PartNo,
                        Qty = item.Qty,
                        ListPrice = item.ListPrice,
                        UnitPrice = item.UnitPrice,
                        DefDct = item.DefDct,
                        Discount = item.Discount,
                        PaidAmt = item.PaidAmt,
                        Rmk = item.Rmk,
                        Sort = item.SeqNo
                    };

                    this.InputInfo.DetInfos.Add(detInfo);
                }
            }
        }
        #endregion

        return this.InputInfo;
    }
    #endregion

    #region CheckInputted
    enum CheckMode
    {
        /// <summary>
        /// 建立。
        /// </summary>
        Create,
        /// <summary>
        /// 草稿。
        /// </summary>
        Draft,
        /// <summary>
        /// 建立備貨單。
        /// </summary>
        ToPGOrder,
        /// <summary>
        /// 提交營管部審核。
        /// </summary>
        ToBizMgtDept
    }

    bool CheckInputted(CheckMode checkMode)
    {
        this.BindingInputted();

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        if (this.InputInfo.Info.DomDistSId == null)
        {
            errMsgs.Add("請選擇「地區」");
        }

        if (string.IsNullOrEmpty(this.InputInfo.Info.Whse))
        {
            errMsgs.Add("請選擇「倉庫」");
        }

        if (this.InputInfo.Info.ProdType == null)
        {
            errMsgs.Add("請選擇「產品」");
        }

        //if (this.InputInfo.Info.Edd == null)
        //{
        //    errMsgs.Add("請輸入「預計出貨日」(或格式不正確)");
        //}

        if (this.InputInfo.Info.CustomerId == null)
        {
            errMsgs.Add("請選擇「客戶」");
        }

        switch (checkMode)
        {
            case CheckMode.Draft:
            case CheckMode.ToPGOrder:
            case CheckMode.ToBizMgtDept:
                #region 表頭折扣
                foreach (var erpDctRel in this.InputInfo.HeaderDiscountInfos)
                {
                    if (erpDctRel.Discount == null)
                    {
                        errMsgs.Add(string.Format("請輸入表頭折扣「{0}」折扣值 (或格式不正確)", this._blockHeaderDiscountList.Where(q => q.DiscountId == erpDctRel.DiscountId).Single().Name));
                    }
                }
                #endregion

                #region 收貨人資訊
                //if (string.IsNullOrEmpty(this.InputInfo.Info.RcptName))
                //{
                //    errMsgs.Add("請輸入「收貨人資訊-收貨人」");
                //}

                if (this.InputInfo.Info.FreightWaySId == null)
                {
                    errMsgs.Add("請選擇「收貨人資訊-貨運方式」");
                }

                if (string.IsNullOrEmpty(this.InputInfo.Info.RcptCusterName))
                {
                    errMsgs.Add("請輸入「收貨人資訊-客戶名稱」");
                }

                if (string.IsNullOrEmpty(this.InputInfo.Info.RcptAddr))
                {
                    errMsgs.Add("請輸入「收貨人資訊-地址」");
                }

                //if (string.IsNullOrEmpty(this.InputInfo.Info.RcptTel))
                //{
                //    errMsgs.Add("請輸入「收貨人資訊-TEL」");
                //}

                //if (string.IsNullOrEmpty(this.InputInfo.Info.RcptFax))
                //{
                //    errMsgs.Add("請輸入「收貨人資訊-FAX」");
                //}
                #endregion

                #region 訂單金額
                //只要有品項, 且品項都有輸入, 則訂單金額為自動產生.
                #endregion

                if (this.InputInfo.GoodsEditInfos.Count == 0)
                {
                    //理論上不會進來這邊, 保險起見.
                    errMsgs.Add("訂單品項初始失敗");
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
                                if (itemInfo.PGOrderDetSId != null || (!string.IsNullOrWhiteSpace(itemInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(itemInfo.QuoteItemId)))
                                {
                                    if (itemInfo.Qty > itemInfo.MaxQty)
                                    {
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 「數量」超過限定的最大值", editInfo.Title, itemInfo.SeqNo));
                                    }
                                }

                                //if (itemInfo.Qty > ConvertLib.ToInt(itemInfo.ErpOnHand, 0))
                                //{
                                //    errMsgs.Add(string.Format("{0} 序號 {1} - 「數量」超過庫存量的最大值", editInfo.Title, itemInfo.SeqNo));
                                //}
                            }

                            if (itemInfo.ListPrice == null)
                            {
                                errMsgs.Add(string.Format("{0} 序號 {1} - 找不到品項「牌價」", editInfo.Title, itemInfo.SeqNo));
                            }

                            if (itemInfo.UnitPrice == null)
                            {
                                errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「單價」", editInfo.Title, itemInfo.SeqNo));
                            }
                        }
                    }
                    #endregion
                }
                break;
        }

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return false;
        }
        #endregion

        switch (checkMode)
        {
            case CheckMode.Draft:
            case CheckMode.ToPGOrder:
            case CheckMode.ToBizMgtDept:
                #region 檢查都 pass 後再重新計算一次 (不相信 JavaScript 的浮點運算)
                List<float> headerDcts = new List<float>();
                //總金額(未稅) = 所有小計總和
                float totalAmt = 0f;
                //所有小計要被扣掉的價錢總和
                float totalDownPrice = 0;
                //所有小計總和 (表頭折扣後)
                float totalAmt_dct = 0f;
                //所有表頭折扣總和
                float totalHeaderDct = 0f;

                #region 巡覽所有表頭折扣
                foreach (var erpDctRel in this.InputInfo.HeaderDiscountInfos)
                {
                    //針對品項折扣後總計再折扣 (公式「(100 - 表頭折扣) / 100」; 假設表頭折扣為「20%」, 則表示打「80%」折.)
                    var headerDct = erpDctRel.Discount.Value / 100;
                    totalHeaderDct += headerDct;
                    headerDcts.Add(headerDct);
                }
                #endregion

                #region 巡覽所有品項區塊
                for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                {
                    var detInfo = this.InputInfo.DetInfos[i];

                    /********************************************************************************
                     * 折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.
                    ********************************************************************************/

                    //品項折扣及小計
                    //if (detInfo.PGOrderDetSId != null || (!string.IsNullOrWhiteSpace(detInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(detInfo.QuoteItemId)))
                   // {
                        //專案報價品項的折扣不重新計算
                   //     detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);
                  //  }
                  //  else
                  //  {
                 //       detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
                 //       detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);
                 //   }

					  //品項折扣及小計
                    if (detInfo.PGOrderDetSId != null || (!string.IsNullOrWhiteSpace(detInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(detInfo.QuoteItemId)))
                    {
                        //專案報價品項的折扣不重新計算
                        // detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);

                        //check 2016.8.1  BY 米雪 PARSE DECIMAL 後再計算，避免浮點數錯誤               
                        string txtPrice = detInfo.UnitPrice.Value.ToString("0.0000");
                        detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * Decimal.Parse(txtPrice),2);
                    }
                    else
                    {
                        detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
                       // detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);
                        //check 2016.8.1  BY 米雪 PARSE DECIMAL 後再計算，避免浮點數錯誤               
                        string txtPrice = detInfo.UnitPrice.Value.ToString("0.0000");
                        detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * Decimal.Parse(txtPrice), 2);


                    }
                    //品項小計
                    var subtotal = detInfo.PaidAmt.Value;

                    //小計要被扣掉的價錢
                    float downPrice = (float)MathLib.Round(subtotal * totalHeaderDct, 2);

                    //所有小計總和
                    totalAmt += subtotal;
                    //所有小計要被扣掉的價錢總和
                    totalDownPrice += downPrice;
                    //所有小計總和 (表頭折扣後)
                    totalAmt_dct += subtotal - downPrice; //「小計」-「小計要被扣掉的價錢」= 依表頭折扣計算後的小計
                }
                #endregion

                //人民幣稅率「0.17」
                var tax = DomOrderHelper.RmbTax;

                //總金額(未稅)
                this.InputInfo.Info.PTTotalAmt = (float)MathLib.Round(totalAmt, 2);

                //稅額
                this.InputInfo.Info.TaxAmt = (float)MathLib.Round(totalAmt * tax, 2);

                ////折扣金額 = (總金額(未稅) + 稅額) * (表頭折扣總和)
                //this.InputInfo.Info.DctAmt = (float)MathLib.Round((this.InputInfo.Info.PTTotalAmt.Value + this.InputInfo.Info.TaxAmt.Value) * totalHeaderDct, 2);
                //折扣金額 = 所有小計要被扣掉的價錢總和 [by fan]
                this.InputInfo.Info.DctAmt = (float)MathLib.Round(totalDownPrice, 2);

                ////折扣後總金額(含稅) = (總金額(未稅) + 稅額 - 折扣金額)
                //this.InputInfo.Info.DctTotalAmt = (float)MathLib.Round(this.InputInfo.Info.PTTotalAmt.Value + this.InputInfo.Info.TaxAmt.Value - this.InputInfo.Info.DctAmt.Value, 2);
                //折扣後總金額(含稅) = 所有小計總和 (表頭折扣後) * 1.17  [by fan]
                this.InputInfo.Info.DctTotalAmt = (float)MathLib.Round(totalAmt_dct * (1 + tax), 2);
                #endregion
                break;
        }

        return true;
    }
    #endregion

    #region 刪除訂單
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //僅註解刪除
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "MD", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                entityDomOrder.SwitchMarkDeleted(actorSId, ConvertLib.ToSIds(this.OrigInfo.Info.SId), true);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "訂單已刪除");
            JSBuilder.PageRedirect(this, "index.aspx");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 建立訂單
    protected void btnCreateOrder_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(CheckMode.Create))
        {
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);

                //取得客戶資訊. 理論上一定取得到, 若取不到, 就直接出錯唄.
                ErpCuster.Info erpCusterInfo = null;
                returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, ConvertLib.ToLongs(this.InputInfo.Info.CustomerId.Value), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    erpCusterInfo = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                }

                var orderNo = entityDomOrder.GetNewOrderNo();

                returner = entityDomOrder.Add(actorSId, this.InputInfo.Info.SId, orderNo, this.InputInfo.Info.DomDistSId, this.InputInfo.Info.Whse, this.InputInfo.Info.ProdType.Value, this.InputInfo.Info.CustomerId.Value, erpCusterInfo.CustomerNumber, erpCusterInfo.CustomerName, erpCusterInfo.ContactId, erpCusterInfo.ConName, erpCusterInfo.AreaCode, erpCusterInfo.Phone, erpCusterInfo.Fax, erpCusterInfo.ShipAddressId, erpCusterInfo.ShipAddress1, erpCusterInfo.PriceListId, erpCusterInfo.CurrencyCode, erpCusterInfo.ShipToSiteUseId, erpCusterInfo.InvoiceToSiteUseId, erpCusterInfo.OrderTypeId, erpCusterInfo.SalesRepId, erpCusterInfo.SalesName, this.InputInfo.Info.Edd, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    //異動記錄
                    string dataTitle = string.Format("{0}", orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    transaction.Complete();
                }
            }

            QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
            query.Add("sid", this.InputInfo.Info.SId.Value);

            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 儲存為稿單
    protected void btnToDraft_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //儲存訂單
                if (!this.Save(CheckMode.Draft))
                {
                    return;
                }

                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);

                transaction.Complete();
            }

            QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
            query.Add("sid", this.OrigInfo.Info.SId.Value);

            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 建立備貨單
    protected void btnCreatePGOrder_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //儲存訂單
                if (!this.Save(CheckMode.ToPGOrder))
                {
                    return;
                }

                transaction.Complete();
            }

            if (this.InputInfo.SeledDetInfos.Count == 0)
            {
                JSBuilder.AlertMessage(this, "沒有選擇任何的備貨品項");
                return;
            }

            PGOrderHelper.QuickImporter importer = new PGOrderHelper.QuickImporter();
            importer.DomOrderSId = this.OrigInfo.Info.SId;
            importer.SeledDetSIds = this.InputInfo.SeledDetInfos.ToArray();

            Session[SessionDefine.SystemBuffer] = importer;

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("../pg_order/edit.aspx", UriKind.Relative);

            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 提交營管部待審核
    protected void btnToBizMgtDept_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //儲存訂單
                if (!this.Save(CheckMode.ToBizMgtDept))
                {
                    return;
                }

                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 1);

                transaction.Complete();
            }

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
            query.Add("sid", this.OrigInfo.Info.SId.Value);
            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 儲存訂單
    bool Save(CheckMode checkMode)
    {
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //驗證檢查
                if (!this.CheckInputted(checkMode))
                {
                    return false;
                }

                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo.Info)))
                {
                }

                returner = entityDomOrder.Modify(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.DomDistSId, this.InputInfo.Info.Whse, this.InputInfo.Info.ProdType.Value, this.InputInfo.Info.Edd, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.PTTotalAmt, this.InputInfo.Info.TaxAmt, this.InputInfo.Info.DctAmt, this.InputInfo.Info.DctTotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 表頭折扣
                    var relCode = (int)ErpDctRel.RelCodeOpt.DomOrder_HeaderDiscount;

                    ErpDctRel entityErpDctRel = new ErpDctRel(SystemDefine.ConnInfo);
                    entityErpDctRel.DeleteByUseSId(relCode, ConvertLib.ToSIds(this.OrigInfo.Info.SId));
                    foreach (var erpDctRel in this.InputInfo.HeaderDiscountInfos)
                    {
                        entityErpDctRel.Add(actorSId, relCode, this.InputInfo.Info.SId, erpDctRel.DiscountId.Value, erpDctRel.Discount.Value / 100);
                    }
                    #endregion

                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        var origDetInfo = this.OrigInfo.DetInfos.Where(q => q.SId.Equals(detInfo.SId)).DefaultIfEmpty(null).SingleOrDefault();
                        if (origDetInfo == null)
                        {
                            //新增
                            entityDomOrderDet.Add(actorSId, detInfo.SId, detInfo.DomOrderSId, detInfo.Source.Value, detInfo.PGOrderDetSId, detInfo.QuoteNumber, detInfo.QuoteItemId, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice.Value, detInfo.UnitPrice.Value, detInfo.DefDct, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        else
                        {
                            //修改
                            returner = entityDomOrderDet.Modify(actorSId, detInfo.SId, detInfo.Qty.Value, detInfo.ListPrice.Value, detInfo.UnitPrice.Value, detInfo.DefDct, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(origDetInfo)))
                            {
                            }
                        }
                    }

                    //原存在, 但異動後已刪除的.
                    var delItemSIds = this.OrigInfo.DetInfos.Select(q => q.SId.Value).ToArray().Except(this.InputInfo.DetInfos.Select(q => q.SId.Value)).ToArray();
                    if (delItemSIds.Length > 0)
                    {
                        var delDetInfos = this.OrigInfo.DetInfos.Where(q => delItemSIds.Contains(q.SId.Value)).ToArray();
                        foreach (var detInfo in delDetInfos)
                        {
                            //刪除
                            returner = entityDomOrderDet.Delete(actorSId, ConvertLib.ToSIds(detInfo.SId));

                            //異動記錄
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.DOM_ORDER_DET, detInfo.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
                            {
                            }
                        }
                    }
                    #endregion

                    transaction.Complete();
                }
            }

            return true;
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}