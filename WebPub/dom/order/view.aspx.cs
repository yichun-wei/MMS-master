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

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; //xlsx

public partial class dom_order_view : System.Web.UI.Page
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
    /// 內銷出貨單功能權限。
    /// </summary>
    FunctionRight DomShippingOrderRight { get; set; }

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

        //內銷出貨單 權限
        this.DomShippingOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_SHIPPING_ORDER");

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
        this.PageTitle = "內銷訂單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>內銷訂單</a>"));

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                #region 貨運方式
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
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

            if (this.SetEditData(this._domOrderSId))
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                if (this.OrigInfo.Info.IsCancel)
                {
                    Response.Redirect("index.aspx");
                    return false;
                }

                #region 操作切換
                #region ERP 狀態更新操作
                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                switch (this.OrigInfo.Info.Status)
                {
                    case 0:
                    case 1:
                        break;
                    case 3:
                        //營管權限
                        this.phUpdateErpOrder.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        break;
                    default:
                        //營管權限
                        //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
                        //一直到「已關閉」前都要顯示 ERP 狀態更新操作
                        this.phUpdateErpOrder.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1) && this.OrigInfo.Info.ErpStatus != 6;
                        break;
                }
                #endregion

                #region 檢視 / 編輯操作
                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                switch (this.OrigInfo.Info.Status)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        //具備訂單修改權限便可修改, 不鎖定流程審核權限.
                        this.phRcptEdit.Visible = this.FunctionRight.Maintain;
                        this.txtRmk.Visible = this.FunctionRight.Maintain;
                        this.btnUpdateInfo.Visible = this.FunctionRight.Maintain;
                        break;
                    default:
                        this.phRcptEdit.Visible = false;
                        this.txtRmk.Visible = false;
                        break;
                }

                this.phRcptView.Visible = !this.phRcptEdit.Visible;
                this.litRmk.Visible = !this.txtRmk.Visible;
                #endregion

                //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                switch (this.OrigInfo.Info.Status)
                {
                    case 1:
                        breadcrumbs.Add(string.Format("營管部待審核"));

                        //營管權限
                        this.btnToDraft.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnToDraft.OnClientClick = string.Format("javascript:if(!window.confirm('確定退回重改？')){{return false;}}");

                        //營管權限
                        this.btnCancel.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnCancel.OnClientClick = string.Format("javascript:if(!window.confirm('確定要取消？')){{return false;}}");

                        //營管權限
                        this.btnToErp.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1);
                        this.btnToErp.OnClientClick = string.Format("javascript:if(!window.confirm('確定要上傳 ERP？')){{return false;}}");
                        break;
                    case 2:
                        breadcrumbs.Add(string.Format("ERP上傳中"));
                        break;
                    case 3:
                        breadcrumbs.Add(string.Format("財務待審核"));

                        //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
                        //「已超額」時, 不允許審核.
                        switch (this.OrigInfo.Info.ErpStatus)
                        {
                            case 3:
                                break;
                            default:
                                //財務權限
                                this.btnToUnpaid.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(2);

                                //財務權限
                                this.btnToShipping.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(2);
                                break;
                        }

                        ////營管、財務、副總權限
                        //this.btnPrintSalesSlip.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(1, 2, 3);
                        this.btnPrintSalesSlip.Visible = true;
                        break;
                    case 4:
                        breadcrumbs.Add(string.Format("未付款待審核"));

                        //副總權限
                        this.btnToShipping2.Visible = this.MainPage.ActorInfoSet.CheckDomAuditPerms(3);

                        //檢視權限
                        this.btnPrintSalesSlip.Visible = true;
                        break;
                    case 5:
                        breadcrumbs.Add(string.Format("待列印"));

                        //檢視權限
                        this.btnPrintSalesSlip.Visible = true;

                        //檢視權限
                        this.btnPrintShipping.Visible = true;
                        break;
                    case 6:
                        breadcrumbs.Add(string.Format("已列印"));

                        //檢視權限
                        this.btnPrintSalesSlip.Visible = true;

                        //檢視權限
                        this.btnPrintShipping.Visible = true;
                        break;
                    case 7:
                        breadcrumbs.Add(string.Format("備貨中"));

                        //檢視權限
                        this.btnPrintSalesSlip.Visible = true;

                        //檢視權限
                        this.btnPrintShipping.Visible = true;
                        break;
                    case 8:
                        breadcrumbs.Add(string.Format("出貨單"));

                        //檢視權限
                        this.btnPrintSalesSlip.Visible = true;

                        //檢視權限
                        this.btnPrintShipping.Visible = true;
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
                this.litProdType.Text = DomOrderHelper.GetDomOrderProdTypeName(this.OrigInfo.Info.ProdType);
                this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.litStatus.Text = DomOrderHelper.GetDomOrderStatusName(this.OrigInfo.Info.Status);
                this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);
                this.txtRmk.Text = this.OrigInfo.Info.Rmk;

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

                #region 收貨人資訊 - 檢視
                this.litRcptCusterName.Text = this.OrigInfo.Info.RcptCusterName;
                this.litRcptName.Text = this.OrigInfo.Info.RcptName;
                this.litRcptTel.Text = this.OrigInfo.Info.RcptTel;
                this.litRcptFax.Text = this.OrigInfo.Info.RcptFax;
                this.litRcptAddr.Text = this.OrigInfo.Info.RcptAddr;
                this.litFreightWay.Text = this.OrigInfo.Info.FreightWayName;
                #endregion

                #region 收貨人資訊 - 編輯
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
                //this.lblDctTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.DctTotalAmt, string.Empty);
                #endregion
				#region 2016.7.28 新數字顯示寫法-By米雪,先改總金額的顯示就好，明細計算在研究
                //this.litPTTotalAmtDisp.Text = string.Format("{0:N2}", (double)Math.Round(this.OrigInfo.Info.PTTotalAmt.Value, 2));
                //this.litTaxAmtDisp.Text = string.Format("{0:N2}", (double)Math.Round(this.OrigInfo.Info.TaxAmt.Value, 2)); 
                //this.litDctAmtDisp.Text = string.Format("{0:N2}", (double)Math.Round(this.OrigInfo.Info.DctAmt.Value, 2)); 
                this.lblDctTotalAmtDisp.Text = string.Format("{0:N2}", (double)Math.Round(this.OrigInfo.Info.DctTotalAmt.Value, 2));
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

    #region 退回重改
    protected void btnToDraft_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
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
            entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 0);

            JSBuilder.AlertMessage(this, "訂單已退回");

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("index.aspx", UriKind.Relative);
            JSBuilder.PageRedirect(this, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 取消
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);
                PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);

                entityDomOrder.UpdateCancelInfo(actorSId, this._domOrderSId, true, DateTime.Now);

                DomOrderDet.Info[] domOrderDetInfos = null;

                #region 內銷訂單明細
                if (true) //只是為了不想重複宣告區域變數
                {
                    returner = entityDomOrderDet.GetInfo(new DomOrderDet.InfoConds(DefVal.SIds, this._domOrderSId, DefVal.Int, DefVal.SId, DefVal.Str, DefVal.Str, DefVal.Str), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        domOrderDetInfos = DomOrderDet.Info.Binding(returner.DataSet.Tables[0]);
                    }
                }
                #endregion

                #region 針對已取消的專案報價備貨單品項還原數量 (沒被取消的專案報價備貨單品項就不用還原了)
                var domOrderDetOfPGInfos = domOrderDetInfos.Where(q => q.Source == 2 && q.PGOrderDetSId != null).ToArray();
                foreach (var domOrderDetOfPGInfo in domOrderDetOfPGInfos)
                {
                    returner = entityPGOrderDet.GetInfoView(new PGOrderDet.InfoViewConds(ConvertLib.ToSIds(domOrderDetOfPGInfo.PGOrderDetSId), DefVal.SId, DefVal.Str, DefVal.Int, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Long, true, false, IncludeScope.All), 1, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        var pgOrderDetInfo = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

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

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "訂單已取消");

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("index.aspx", UriKind.Relative);
            JSBuilder.PageRedirect(this, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 上傳 ERP
    protected void btnToErp_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //營管權限
        if (!this.MainPage.ActorInfoSet.CheckDomAuditPerms(1))
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);

                //更改訂單狀態
                entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 2);

                var talkRespCode = new ErpHelper.ErpUploader().Upload(this.OrigInfo);
                if ("0000".Equals(talkRespCode))
                {
                    JSBuilder.AlertMessage(this, "已上傳 ERP");

                    QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
                    JSBuilder.PageRedirect(this, query.ToString());
                }
                else
                {
                    JSBuilder.AlertMessage(this, "上傳 ERP 失敗", string.Format("錯誤碼：{0}", talkRespCode));

                    QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
                    JSBuilder.PageRedirect(this, query.ToString());
                }

                if ("0000".Equals(talkRespCode))
                {
                    transaction.Complete();
                }
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

    #region 未付款待簽核 (to 副總審核)
    protected void btnToUnpaid_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            //更改訂單狀態
            entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 4);

            JSBuilder.AlertMessage(this, "已變更狀態為「未付款待簽核」");

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

    #region 財務審核通過
    protected void btnToShipping_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 5);

                //更新預計出貨日 (如果超過 14:00 則為隔天)
                //[20170627 by 儀淳] 更新時間由 15:00 修改為 14:00
                DateTime baseDT = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 14, 00, 0);
                DateTime edd = DateTime.Now < baseDT ? DateTime.Today : DateTime.Today.AddDays(1);
                entityDomOrder.UpdateEdd(actorSId, this.OrigInfo.Info.SId, edd);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "已變更狀態為「財務審核通過」");

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

    #region 列印銷貨清單
    protected void btnPrintSalesSlip_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //改用 js 執行
        //var query = new QueryStringParsing();
        //query.HttpPath = new Uri(string.Format("{0}popup/dom/print/sales_slip.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
        //query.Add("sid", this.OrigInfo.Info.SId.Value);
        //JSBuilder.OpenWindow(this, query.ToString());
    }
    #endregion

    #region 列印出貨單
    protected void btnPrintShipping_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //改用 js 執行
        //var query = new QueryStringParsing();
        //query.HttpPath = new Uri(string.Format("{0}popup/dom/print/shipping.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
        //query.Add("sid", this.OrigInfo.Info.SId.Value);
        //JSBuilder.OpenWindow(this, query.ToString());
    }
    #endregion

    #region 副總審核通過
    protected void btnToShipping2_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityDomOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 5);

                //更新預計出貨日 (如果超過 14:00 則為隔天)
                //[20170627 by 儀淳] 更新時間由 15:00 修改為 14:00
                DateTime baseDT = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 14, 00, 0);
                DateTime edd = DateTime.Now < baseDT ? DateTime.Today : DateTime.Today.AddDays(1);
                entityDomOrder.UpdateEdd(actorSId, this.OrigInfo.Info.SId, edd);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "已變更狀態為「待列印」");

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

    #region 更新收貨人資訊
    protected void btnUpdateInfo_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //具備訂單修改權限便可修改, 不鎖定流程審核權限.
        if (!this.FunctionRight.Maintain)
        {
            JSBuilder.AlertMessage(this, "您未擁有此功能的操作權限");
            return;
        }

        //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
        switch (this.OrigInfo.Info.Status)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                //略過
                break;
            default:
                JSBuilder.AlertMessage(this, "目前訂單狀態已無法再修改出貨人資訊");
                return;
        }

        #region 繫結資料
        WebUtil.TrimTextBox(this.Form.Controls, false);

        var input = new DomOrder.InputInfo();
        input.Rmk = this.txtRmk.Text;

        #region 收貨人資訊
        input.RcptName = this.txtRcptName.Text;
        input.RcptCusterName = this.txtRcptCusterName.Text;
        input.RcptTel = this.txtRcptTel.Text;
        input.RcptFax = this.txtRcptFax.Text;
        input.RcptAddr = this.txtRcptAddr.Text;
        input.FreightWaySId = ConvertLib.ToSId(this.lstFreightWayList.SelectedValue);
        #endregion
        #endregion

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        #region 收貨人資訊
        //if (string.IsNullOrEmpty(input.RcptName))
        //{
        //    errMsgs.Add("請輸入「收貨人資訊-收貨人」");
        //}

        if (input.FreightWaySId == null)
        {
            errMsgs.Add("請選擇「收貨人資訊-貨運方式」");
        }

        if (string.IsNullOrEmpty(input.RcptCusterName))
        {
            errMsgs.Add("請輸入「收貨人資訊-客戶名稱」");
        }

        if (string.IsNullOrEmpty(input.RcptAddr))
        {
            errMsgs.Add("請輸入「收貨人資訊-地址」");
        }

        //if (string.IsNullOrEmpty(input.RcptTel))
        //{
        //    errMsgs.Add("請輸入「收貨人資訊-TEL」");
        //}

        //if (string.IsNullOrEmpty(input.RcptFax))
        //{
        //    errMsgs.Add("請輸入「收貨人資訊-FAX」");
        //}
        #endregion

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return;
        }
        #endregion

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
            //更新收貨人資訊
            entityDomOrder.UpdateRcptInfo(actorSId, this.OrigInfo.Info.SId, input.RcptName, input.RcptCusterName, input.RcptTel, input.RcptFax, input.RcptAddr, input.FreightWaySId, input.Rmk);
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}