using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.IO;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class dom_order_shipping_view : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_SHIPPING_ORDER";
    string AUTH_NAME = "內銷出貨單";

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
    #endregion

    ISystemId _domOrderSId;

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
        if (this._domOrderSId == null)
        {
            Response.Redirect("index.aspx");
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
        this.PageTitle = "內銷出貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>內銷出貨單</a>"));

        Returner returner = null;
        try
        {
            if (this.SetEditData(this._domOrderSId))
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                if (this.OrigInfo.Info.IsCancel)
                {
                    Response.Redirect("index.aspx");
                    return false;
                }

                #region 權限初始
                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                switch (this.OrigInfo.Info.Status)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        break;
                    default:
                        Response.Redirect("../view.aspx");
                        return false;
                }
                #endregion

                #region 操作切換
                //營管權限
                //ERP 狀態更新操作
                //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
                //一直到「已關閉」前都要顯示 ERP 狀態更新操作
                this.phUpdateErpOrder.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1) && this.OrigInfo.Info.ErpStatus != 6;

                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                switch (this.OrigInfo.Info.Status)
                {
                    case 5:
                        breadcrumbs.Add(string.Format("待列印"));

                        //營管權限
                        this.btnPrintSalesSlip.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnPrintShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        //this.btnPrintShipping.OnClientClick = string.Format("javascript:if(!window.confirm('確定列印出貨單？')){{return false;}}");
                        break;
                    case 6:
                        breadcrumbs.Add(string.Format("已列印"));

                        //倉管權限
                        this.btnStockUp.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(4);
                        this.btnStockUp.OnClientClick = string.Format("javascript:if(!window.confirm('確認備貨？')){{return false;}}");

                        //營管權限
                        //就算已列印出貨單或已出貨, 仍要可以列印「銷貨清單」、「出貨單」.
                        this.btnPrintSalesSlip.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnPrintShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        //this.btnPrintShipping.OnClientClick = string.Format("javascript:if(!window.confirm('確定列印出貨單？')){{return false;}}");
                        break;
                    case 7:
                        breadcrumbs.Add(string.Format("備貨中"));

                        //倉管權限
                        this.btnConfirmShipment.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(4);
                        this.btnConfirmShipment.OnClientClick = string.Format("javascript:if(!window.confirm('確認出貨？')){{return false;}}");

                        //營管權限
                        //就算已列印出貨單或已出貨, 仍要可以列印「銷貨清單」、「出貨單」.
                        this.btnPrintSalesSlip.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnPrintShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        //this.btnPrintShipping.OnClientClick = string.Format("javascript:if(!window.confirm('確定列印出貨單？')){{return false;}}");
                        break;
                    case 8:
                        breadcrumbs.Add(string.Format("出貨單"));

                        //營管權限
                        //就算已列印出貨單或已出貨, 仍要可以列印「銷貨清單」、「出貨單」.
                        this.btnPrintSalesSlip.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnPrintShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        //this.btnPrintShipping.OnClientClick = string.Format("javascript:if(!window.confirm('確定列印出貨單？')){{return false;}}");
                        break;
                    default:
                        Response.Redirect("index.aspx");
                        return false;
                }
                #endregion
            }
            else
            {
                Response.Redirect("index.aspx");
                return false;
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
                this.litCdt.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy-MM-dd");
                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litDomDistName.Text = this.OrigInfo.Info.DomDistName;
                this.litWhse.Text = this.OrigInfo.Info.Whse;
                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = DomOrderHelper.GetDomOrderStatusName(this.OrigInfo.Info.Status);
                this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);

                //營管權限
                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                //只有訂單狀態為「待列印」之後, 才能修改「預計出貨日」.
                this.txtEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                switch (this.OrigInfo.Info.Status)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        this.txtEdd.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        break;
                }
                this.litEdd.Visible = !this.txtEdd.Visible;
                this.btnUpdateEdd.Visible = this.txtEdd.Visible;


                #region 表頭折扣
                if (this.OrigInfo.Info.PriceListId.HasValue)
                {
                    ErpDct entityErpDct = new ErpDct(SystemDefine.ConnInfo);
                    returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, DefVal.Longs, ConvertLib.ToLongs(this.OrigInfo.Info.PriceListId.Value), ConvertLib.ToInts(1)), Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var erpDctInfos = ErpDct.Info.Binding(returner.DataSet.Tables[0]);

                        var builder = new StringBuilder();
                        foreach (var erpDctInfo in erpDctInfos)
                        {
                            var headerDiscountInfo = this.OrigInfo.HeaderDiscountInfos.Where(q => q.DiscountId == erpDctInfo.DiscountId).DefaultIfEmpty(null).SingleOrDefault();
                            if (headerDiscountInfo != null)
                            {
                                builder.AppendFormat("<div>{0} {1}%</div>", erpDctInfo.DiscountName, MathLib.Round(headerDiscountInfo.Discount * 100, 2));
                            }
                        }
                        this.litHeaderDiscountList.Text = builder.ToString();
                    }
                }
                #endregion

                #region 客戶資訊
                this.litCustomerNumber.Text = this.OrigInfo.Info.CustomerNumber;
                this.litCustomerName2.Text = this.OrigInfo.Info.CustomerName;
                this.litCustomerConName.Text = this.OrigInfo.Info.CustomerConName;
                this.litCustomerTel.Text = ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerTel);
                this.litCustomerFax.Text = ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerFax);
                this.litCustomerAddr.Text = this.OrigInfo.Info.CustomerAddr;
                #endregion

                #region 收貨人資訊
                this.litRcptCusterName.Text = this.OrigInfo.Info.RcptCusterName;
                this.litRcptName.Text = this.OrigInfo.Info.RcptName;
                this.litRcptTel.Text = this.OrigInfo.Info.RcptTel;
                this.litRcptFax.Text = this.OrigInfo.Info.RcptFax;
                this.litRcptAddr.Text = this.OrigInfo.Info.RcptAddr;
                this.litFreightWay.Text = this.OrigInfo.Info.FreightWayName;
                #endregion

                #region 品項資訊
                if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
                {
                    #region 初始載入
                    //先取得所有品項的在手量
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
                        var block = (ASP.include_client_dom_order_view_goods_block_ascx)this.LoadControl("~/include/client/dom/order/view/goods_block.ascx");
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

                            //改為取得所有倉庫的在手量
                            //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                            itemEditInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == this.OrigInfo.Info.Whse && q.Segment1 == detInfo.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            itemEditInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, detInfo.PartNo);

                            itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                            editInfo.Items.Add(itemEditInfo);
                        }

                        block.SetInfo(editInfo);
                    }
                    #endregion

                    #region 專案報價品項
                    //專案報價錨點索引
                    var builderProjQuoteIdxes = new StringBuilder();

                    //專案報價品項
                    var projQuoteInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).GroupBy(q => new { QuoteNumber = q.QuoteNumber }).Select(q => new { QuoteNumber = q.Key.QuoteNumber, GoodsItemList = q.ToArray() });
                    this.phProjQuoteIndex.Visible = projQuoteInfos.Count() > 0;
                    foreach (var projQuoteInfo in projQuoteInfos)
                    {
                        var block = (ASP.include_client_dom_order_view_goods_block_ascx)this.LoadControl("~/include/client/dom/order/view/goods_block.ascx");
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

                            //改為取得所有倉庫的在手量
                            //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                            itemEditInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == this.OrigInfo.Info.Whse && q.Segment1 == detInfo.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            itemEditInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, detInfo.PartNo);

                            itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                            editInfo.Items.Add(itemEditInfo);
                        }

                        block.SetInfo(editInfo);

                        //專案報價錨點索引
                        builderProjQuoteIdxes.AppendFormat("<li><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", projQuoteInfo.QuoteNumber);
                    }

                    //專案報價錨點索引
                    this.litProjQuoteIdxes.Text = builderProjQuoteIdxes.ToString();
                    #endregion
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.litPTTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.PTTotalAmt, string.Empty);
                this.litTaxAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.TaxAmt, string.Empty);
                this.litDctAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.DctAmt, string.Empty);
                this.lblDctTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.DctTotalAmt, string.Empty);
                #endregion

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

    #region ERP 訂單狀態更新
    protected void btnUpdateErpOrder_Click(object sender, EventArgs e)
    {
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            var erpOrderInfos = ErpHelper.GetErpOrderInfo(DefVal.Longs, ConvertLib.ToStrs(this.OrigInfo.Info.OdrNo), DefVal.Ints);
            if (erpOrderInfos.Length > 0)
            {
                var erpOrderInfo = erpOrderInfos[0];

                if (!erpOrderInfo.OrderNumber.HasValue)
                {
                    JSBuilder.AlertMessage(this, "資料異常", "無值的「ERP 訂單號碼」");
                    return;
                }

                var erpOrderStatus = DomOrderHelper.GetDomErpOrderStatusCode(erpOrderInfo.OrderStatus);
                if (!erpOrderStatus.HasValue)
                {
                    JSBuilder.AlertMessage(this, "資料異常", "錯誤的「ERP 訂單狀態」");
                    return;
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);

                    //更改訂單資訊
                    entityDomOrder.UpdateErpInfo(actorSId, this.OrigInfo.Info.SId, erpOrderInfo.HeaderId, erpOrderInfo.OrderNumber.Value.ToString(), erpOrderStatus, erpOrderInfo.ShipNumber);

                    //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                    //只有在訂單狀態為「2:ERP上傳中」時, 才要更新訂單狀態.
                    if (this.OrigInfo.Info.Status == 2)
                    {
                        //更改訂單狀態
                        entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 3);
                    }

                    JSBuilder.AlertMessage(this, "ERP 訂單狀態已更新");

                    QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
                    JSBuilder.PageRedirect(this, query.ToString());

                    transaction.Complete();
                }
            }
            else
            {
                JSBuilder.AlertMessage(this, "未取得「ERP 訂單」資料");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 列印銷貨清單
    protected void btnPrintSalesSlip_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        //改用 js 執行
        //var query = new QueryStringParsing();
        //query.HttpPath = new Uri(string.Format("{0}popup/dom/print/sales_slip.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
        //query.Add("sid", this.OrigInfo.Info.SId.Value);
        //JSBuilder.OpenWindow(this, query.ToString());
    }
    #endregion

    #region 待列印 (儲存預計出貨日)
    protected void btnUpdateEdd_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        var edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
        if (edd == null)
        {
            JSBuilder.AlertMessage(this, "請輸入「預計出貨日」(或格式不正確)");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            //更新預計出貨日
            entityDomOrder.UpdateEdd(actorSId, this.OrigInfo.Info.SId, edd);
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 列印出貨單
    protected void btnPrintShipping_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        var edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
        if (edd == null)
        {
            JSBuilder.AlertMessage(this, "請輸入「預計出貨日」(或格式不正確)");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            if (this.OrigInfo.Info.Status == 5)
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 6);
                //更新預計出貨日
                entityDomOrder.UpdateEdd(actorSId, this.OrigInfo.Info.SId, edd);
            }

            //改用 js 執行
            //var query = new QueryStringParsing();
            //query.HttpPath = new Uri(string.Format("{0}popup/dom/print/shipping.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
            //query.Add("sid", this.OrigInfo.Info.SId.Value);
            //JSBuilder.OpenWindow(this, query.ToString());

            JSBuilder.PageRedirect(this, QueryStringParsing.CurrentRelativeUri.OriginalString);
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 確認備貨
    protected void btnStockUp_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //倉管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            //更改訂單狀態
            entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 7);

            JSBuilder.AlertMessage(this, "已變更狀態為「備貨中」");

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
            query.Add("sid", this.OrigInfo.Info.SId.Value);
            JSBuilder.PageRedirect(this, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 確認出貨
    protected void btnConfirmShipment_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //倉管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(4))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            //更改訂單狀態
            entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 8);

            JSBuilder.AlertMessage(this, "已變更狀態為「已出貨」");

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
            query.Add("sid", this.OrigInfo.Info.SId.Value);
            JSBuilder.PageRedirect(this, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}