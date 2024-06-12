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

public partial class ext_shipping_order_view : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_SHIPPING_ORDER";
    string AUTH_NAME = "外銷出貨單";

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
    ExtShippingOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _extShippingOrderSId;

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

        this._extShippingOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._extShippingOrderSId == null)
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
        this.PageTitle = "外銷出貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>外銷出貨單</a>"));

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                #region 操作切換
                //ERP 狀態更新操作
                //0:草稿 1:已確認 2:已出貨 3:已上傳
                switch (this.OrigInfo.Info.Status)
                {
                    case 0:
                    case 1:
                        break;
                    case 3:
                        //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
                        //一直到「已關閉」前都要顯示 ERP 狀態更新操作
                        this.phUpdateErpOrder.Visible = !(this.OrigInfo.Info.ErpStatus == 6);
                        this.btnSave.Visible = this.phUpdateErpOrder.Visible;
                        break;
                }

                //0:草稿 1:已確認 2:已出貨 3:已上傳
                switch (this.OrigInfo.Info.Status)
                {
                    case 0:
                        QueryStringParsing query = new QueryStringParsing();
                        query.HttpPath = new Uri("edit.aspx", UriKind.Relative);
                        query.Add("sid", this.OrigInfo.Info.SId.Value);
                        Response.Redirect(query.ToString());
                        return false;
                    case 1:
                        breadcrumbs.Add(string.Format("已確認"));

                        this.btnConfirmShipment.Visible = true;
                        this.btnConfirmShipment.OnClientClick = string.Format("javascript:if(!window.confirm('確認出貨？')){{return false;}}");
                        break;
                    case 2:
                        breadcrumbs.Add(string.Format("已出貨"));

                        this.btnToErp.Visible = true;
                        this.btnToErp.OnClientClick = string.Format("javascript:if(!window.confirm('確定要上傳 ERP？')){{return false;}}");
                        break;
                    case 3:
                        breadcrumbs.Add(string.Format("已上傳"));

                        this.txtEta.Visible = false;
                        this.litEta.Visible = true;

                        this.phUpdateErpOrder.Visible = this.OrigInfo.Info.ErpHeaderId == null;
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

        //重置幣別
        WebUtilBox.RegisterScript(this, "resetCurrencyCode();");

        ////權限操作檢查
        //this.btnCreateOrder.Visible = this.btnCreateOrder.Visible ? this.FunctionRight.Maintain : this.btnCreateOrder.Visible;
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtShippingOrderHelper.Binding(this._extShippingOrderSId);
            if (this.OrigInfo != null)
            {
                this.litCdt.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy-MM-dd");
                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.txtEta.Text = ConvertLib.ToStr(this.OrigInfo.Info.Eta, string.Empty, "yyyy-MM-dd");
                this.litEta.Text = ConvertLib.ToStr(this.OrigInfo.Info.Eta, string.Empty, "yyyy-MM-dd");

                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
                //this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtShippingOrderHelper.GetExtOrderStatusName(this.OrigInfo.Info.Status);
                //this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);

                //幣別
                this.lblCurrencyCode.Text = this.OrigInfo.Info.CurrencyCode;

                #region 表頭折扣
                if (this.OrigInfo.Info.PriceListId.HasValue)
                {
                    ErpDct entityErpDct = new ErpDct(SystemDefine.ConnInfo);
                    returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, DefVal.Longs, ConvertLib.ToLongs(this.OrigInfo.Info.PriceListId.Value), ConvertLib.ToInts(3)), Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
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

                #region 外銷訂單品項
                //先取得所有品項的價目表
                var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                if (this.OrigInfo.Info.PriceListId.HasValue)
                {
                    priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Select(q => q.PartNo).ToArray());
                }

                //外銷訂單錨點索引
                var builderExtOrderIdxes = new StringBuilder();

                //外銷訂單品項
                var extOrderDetInfos = this.OrigInfo.DetInfos.GroupBy(q => new { ExtQuotnOdrNo = q.ExtQuotnOdrNo }).Select(q => new { ExtQuotnOdrNo = q.Key.ExtQuotnOdrNo, GoodsItemList = q.ToArray() });
                foreach (var extOrderDetInfo in extOrderDetInfos)
                {
                    var block = (ASP.include_client_ext_shipping_order_view_goods_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/view/goods_block.ascx");
                    this.phGoodsList.Controls.Add(block);

                    var editInfo = new ExtShippingOrderHelper.GoodsEditInfo_OdrBase()
                    {
                        Title = extOrderDetInfo.GoodsItemList[0].ExtQuotnOdrNo,
                        ExtOrderSId = extOrderDetInfo.GoodsItemList[0].ExtOrderSId
                    };

                    for (int i = 0; i < extOrderDetInfo.GoodsItemList.Length; i++)
                    {
                        var detInfo = extOrderDetInfo.GoodsItemList[i];

                        var itemEditInfo = new ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase()
                        {
                            SId = detInfo.SId,
                            SeqNo = i + 1,
                            ExtOrderDetSId = detInfo.ExtOrderDetSId,
                            PartNo = detInfo.PartNo,
                            Model = detInfo.Model,
                            Qty = detInfo.Qty,
                            UnitPrice = detInfo.UnitPrice,
                            Discount = detInfo.Discount,
                            PaidAmt = detInfo.PaidAmt,
                            Rmk = detInfo.Rmk
                        };

                        //不顯示在手量
                        //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                        itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                        editInfo.Items.Add(itemEditInfo);
                    }

                    block.SetInfo(editInfo);

                    //外銷訂單錨點索引
                    builderExtOrderIdxes.AppendFormat("<li class='dev-ext-order-idx' extordersid='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{1}</a></div></li>", extOrderDetInfo.GoodsItemList[0].ExtOrderSId.Value, extOrderDetInfo.GoodsItemList[0].ExtQuotnOdrNo);
                }

                //外銷訂單錨點索引
                this.litExtOrderIdxes.Text = builderExtOrderIdxes.ToString();
                #endregion

                #region 訂單金額資訊
                this.lblTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.TotalAmt, string.Empty);
                this.lblDctAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.DctAmt, string.Empty);
                this.lblDctTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.DctTotalAmt, string.Empty);
                #endregion

                //ERP 相關
                this.litErpOrderNumber.Text = this.OrigInfo.Info.ErpOrderNumber;
                this.litErpStatus.Text = ExtShippingOrderHelper.GetExtErpOrderStatusName(this.OrigInfo.Info.ErpStatus);

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

    #region 儲存 (不變更任何狀態)
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        var eta = ConvertLib.ToDateTime(this.txtEta.Text, DefVal.DT);

        //建立出貨單時, 預計到港日可能還未知.
        //if (eta == null)
        //{
        //    errMsgs.Add("請輸入「預計到港日」(或格式不正確)");
        //}

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

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                //更新預計到港日
                entityExtShippingOrder.UpdateEta(actorSId, this.OrigInfo.Info.SId, eta);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "資料已儲存");

            QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
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

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        var eta = ConvertLib.ToDateTime(this.txtEta.Text, DefVal.DT);

        if (eta == null)
        {
            errMsgs.Add("請輸入「預計到港日」(或格式不正確)");
        }

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

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                //更新預計到港日
                entityExtShippingOrder.UpdateEta(actorSId, this.OrigInfo.Info.SId, eta);
                //更改出貨單狀態
                entityExtShippingOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 2);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "出貨單已確認");

            QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
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

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        var eta = ConvertLib.ToDateTime(this.txtEta.Text, DefVal.DT);

        if (eta == null)
        {
            errMsgs.Add("請輸入「預計到港日」(或格式不正確)");
        }

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

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);

                //更新預計到港日
                entityExtShippingOrder.UpdateEta(actorSId, this.OrigInfo.Info.SId, eta);
                //更改出貨單狀態
                entityExtShippingOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 3);

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

                var erpOrderStatus = ExtShippingOrderHelper.GetExtErpOrderStatusCode(erpOrderInfo.OrderStatus);
                if (!erpOrderStatus.HasValue)
                {
                    JSBuilder.AlertMessage(this, "資料異常", "錯誤的「ERP 訂單狀態」");
                    return;
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);

                    //更改訂單資訊
                    entityExtShippingOrder.UpdateErpInfo(actorSId, this.OrigInfo.Info.SId, erpOrderInfo.HeaderId, erpOrderInfo.OrderNumber.Value.ToString(), erpOrderStatus, erpOrderInfo.ShipNumber);

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
}