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

public partial class ext_shipping_order_edit : System.Web.UI.Page
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
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    ExtShippingOrderHelper.InputInfoSet InputInfo { get; set; }
    #endregion

    ISystemId _extShippingOrderSId;

    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_ext_shipping_order_header_discount_block_ascx> _blockHeaderDiscountList = new List<ASP.include_client_ext_shipping_order_header_discount_block_ascx>();
    List<ASP.include_client_ext_shipping_order_edit_goods_block_ascx> _blockGoodsList = new List<ASP.include_client_ext_shipping_order_edit_goods_block_ascx>();

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

        this.btnFixedShippingOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定出貨單確認？')){{return false;}}");
        this.btnDelete.OnClientClick = string.Format("javascript:if(!window.confirm('確定刪除出貨單？')){{return false;}}");

        Returner returner = null;
        try
        {
            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            if (!this.IsPostBack)
            {
                #region 貨運方式
                returner = entityPubCat.GetTopInfo(new PubCat.TopInfoConds(DefVal.SIds, (int)PubCat.UseTgtOpt.ExtFreightWay, DefVal.SId), Int32.MaxValue, new SqlOrder("SORT", Sort.Descending), IncludeScope.All);
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

            if (this._extShippingOrderSId != null)
            {
                #region 修改
                if (this.SetEditData(this._extShippingOrderSId))
                {
                    this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                    breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                    if (!this.IsPostBack)
                    {
                        if (this.OrigInfo.Info.DetLayout == 2)
                        {
                            QueryStringParsing query = new QueryStringParsing(Request.QueryString);
                            query.HttpPath = new Uri("edit_2.aspx", UriKind.Relative);
                            Response.Redirect(query.ToString());
                            return false;
                        }
                    }

                    //0:草稿 1:已確認 2:已出貨 3:已上傳
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

                    this.lnkSelCuster.Visible = false;

                    this.phOrderHeder.Visible = true;
                    this.phOrderDetail.Visible = true;

                    this.phCreateOrderOper.Visible = false;
                    this.phOrderGoods.Visible = true;

                    //操作控制
                    this.btnToDraft.Visible = this.OrigInfo.Info.Status == 0;
                    this.btnDelete.Visible = this.OrigInfo.Info.Status == 0;
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
                this.PageTitle = "新增出貨單";

                breadcrumbs.Add(string.Format("<a href='{0}'>新增出貨單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));

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
                            var blockGoods = (ASP.include_client_ext_shipping_order_edit_goods_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_block.ascx");
                            this._blockGoodsList.Add(blockGoods);
                            this.phGoodsList.Controls.Add(blockGoods);
                            break;
                        case "HeaderDiscountBlock":
                            var blockHeaderDiscount = (ASP.include_client_ext_shipping_order_header_discount_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/header_discount_block.ascx");
                            this._blockHeaderDiscountList.Add(blockHeaderDiscount);
                            this.phHeaderDiscount.Controls.Add(blockHeaderDiscount);
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
        this.lblTotalAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidTotalAmt.Value, DefVal.Float), string.Empty);
        this.lblDctAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidDctAmt.Value, DefVal.Float), string.Empty);
        this.lblDctTotalAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidDctTotalAmt.Value, DefVal.Float), string.Empty);
        //重置幣別
        WebUtilBox.RegisterScript(this, "resetCurrencyCode();");

        //權限操作檢查
        this.btnCreateGoods.Visible = this.btnCreateGoods.Visible ? this.FunctionRight.Maintain : this.btnCreateGoods.Visible;
        this.btnToDraft.Visible = this.btnToDraft.Visible ? this.FunctionRight.Maintain : this.btnToDraft.Visible;
        this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtShippingOrderHelper.Binding(systemId);
            if (this.OrigInfo != null)
            {
                this.hidSpecSId.Value = this.OrigInfo.Info.SId.Value; //暫存系統代號。

                this.litCdt.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy-MM-dd");
                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.txtCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.txtEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.txtEta.Text = ConvertLib.ToStr(this.OrigInfo.Info.Eta, string.Empty, "yyyy-MM-dd");

                //this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtShippingOrderHelper.GetExtOrderStatusName(this.OrigInfo.Info.Status);
                //this.txtRmk.Text = this.OrigInfo.Info.Rmk;

                //「價目表 ID」只是為了給品項取牌價用
                this.hidPriceListId.Value = ConvertLib.ToStr(this.OrigInfo.Info.PriceListId, string.Empty);

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
                        foreach (var erpDctInfo in erpDctInfos)
                        {
                            var block = (ASP.include_client_ext_shipping_order_header_discount_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/header_discount_block.ascx");
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
                    #region 初始載入
                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

                    //先取得所有品項的價目表
                    var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                    if (this.OrigInfo.Info.PriceListId.HasValue)
                    {
                        priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Select(q => q.PartNo).ToArray());
                    }

                    #region 外銷訂單品項
                    //已存在的外銷訂單暫存
                    var existedExtOrders = new List<string>();

                    //外銷訂單錨點索引
                    var builderExtOrderIdxes = new StringBuilder();

                    //外銷出貨單品項
                    var extShippingOrderDetInfos = this.OrigInfo.DetInfos.GroupBy(q => new { ExtQuotnOdrNo = q.ExtQuotnOdrNo }).Select(q => new { ExtQuotnOdrNo = q.Key.ExtQuotnOdrNo, GoodsItemList = q.ToArray() });
                    //this.phExtOrderIndex.Visible = extShippingOrderDetInfos.Count() > 0;
                    foreach (var extShippingOrderDetInfo in extShippingOrderDetInfos)
                    {
                        var block = (ASP.include_client_ext_shipping_order_edit_goods_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_block.ascx");
                        this._blockGoodsList.Add(block);
                        this.phGoodsList.Controls.Add(block);

                        var editInfo = new ExtShippingOrderHelper.GoodsEditInfo_OdrBase()
                        {
                            Title = extShippingOrderDetInfo.GoodsItemList[0].ExtQuotnOdrNo,
                            ExtOrderSId = extShippingOrderDetInfo.GoodsItemList[0].ExtOrderSId
                        };

                        for (int i = 0; i < extShippingOrderDetInfo.GoodsItemList.Length; i++)
                        {
                            var detInfo = extShippingOrderDetInfo.GoodsItemList[i];

                            var itemEditInfo = new ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase()
                            {
                                SId = detInfo.SId,
                                SeqNo = i + 1,
                                ExtOrderDetSId = detInfo.ExtOrderDetSId,
                                PartNo = detInfo.PartNo,
                                Model = detInfo.Model,
                                Qty = detInfo.Qty,
                                UnitPrice = detInfo.UnitPrice,
                                UnitPriceDct = detInfo.UnitPriceDct,
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

                        //加入已存在的外銷訂單暫存
                        existedExtOrders.Add(extShippingOrderDetInfo.GoodsItemList[0].ExtOrderSId.Value);

                        //外銷訂單錨點索引
                        builderExtOrderIdxes.AppendFormat("<li class='dev-ext-order-idx' extordersid='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{1}</a></div></li>", extShippingOrderDetInfo.GoodsItemList[0].ExtOrderSId.Value, extShippingOrderDetInfo.GoodsItemList[0].ExtQuotnOdrNo);
                    }

                    //外銷訂單錨點索引
                    this.litExtOrderIdxes.Text = builderExtOrderIdxes.ToString();

                    //重整已存在的外銷訂單暫存
                    this.hidExistedExtOrders.Value = string.Join(",", existedExtOrders.GroupBy(q => q).Select(q => q.Key).ToArray());
                    #endregion
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.hidTotalAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.TotalAmt, string.Empty);
                this.hidDctAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.DctAmt, string.Empty);
                this.hidDctTotalAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.DctTotalAmt, string.Empty);
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

                this.lblCurrencyCode.Text = info.CurrencyCode;

                this.hidCustomerName.Value = info.CustomerName;
                this.hidPriceListId.Value = ConvertLib.ToStr(info.PriceListId, string.Empty);
                WebPg.RegisterScript("setCustomerName();");
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 開始建立出貨單品項 (依訂單建立)
    protected void btnCreateGoods_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(CheckMode.InitCreate))
        {
            return;
        }

        this.litCustomerNumber.Text = this.InputInfo.Info.CustomerNumber;
        this.lblCustomerName.Text = this.InputInfo.Info.CustomerName;
        this.lblCustomerConName.Text = this.InputInfo.Info.CustomerConName;
        this.lblCustomerAddr.Text = this.InputInfo.Info.CustomerAddr;
        this.lblCustomerTel.Text = this.InputInfo.Info.CustomerTel;
        this.lblCustomerFax.Text = this.InputInfo.Info.CustomerFax;

        this.lnkSelCuster.Visible = false;

        this.phOrderHeder.Visible = true;
        this.phOrderDetail.Visible = true;

        this.phCreateOrderOper.Visible = false;
        this.phOrderGoods.Visible = true;
        this.btnToDraft.Visible = this.FunctionRight.Maintain;

        Returner returner = null;
        try
        {
            #region 表頭折扣
            if (this.InputInfo.Info.PriceListId.HasValue)
            {
                ErpDct entityErpDct = new ErpDct(SystemDefine.ConnInfo);
                returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, DefVal.Longs, ConvertLib.ToLongs(this.InputInfo.Info.PriceListId.Value), ConvertLib.ToInts(3)), Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var erpDctInfos = ErpDct.Info.Binding(returner.DataSet.Tables[0]);
                    foreach (var erpDctInfo in erpDctInfos)
                    {
                        var block = (ASP.include_client_ext_shipping_order_header_discount_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/header_discount_block.ascx");
                        this._blockHeaderDiscountList.Add(block);
                        this.phHeaderDiscount.Controls.Add(block);
                        block.DiscountId = erpDctInfo.DiscountId;
                        block.Name = erpDctInfo.DiscountName;

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[HeaderDiscountBlock]";
                    }
                }
            }
            #endregion
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 開始建立出貨單品項 (依品項建立)
    protected void btnCreateGoods2_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(CheckMode.InitCreate))
        {
            return;
        }

        Session[SessionDefine.SystemBuffer] = this.InputInfo;
        //Response.Redirect("edit_2.aspx");
        Server.Transfer("edit_2.aspx");
    }
    #endregion

    #region 加入外銷訂單區塊
    protected void btnAddExtOrder_Click(object sender, EventArgs e)
    {
        var seledExtOrders = this.hidSeledExtOrders.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledExtOrders.Length == 0)
        {
            return;
        }

        //已存在的外銷訂單暫存
        var existedExtOrders = new List<string>();
        existedExtOrders.AddRange(this.hidExistedExtOrders.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

        //外銷訂單錨點索引
        var builderExtOrderIdxes = new StringBuilder();

        //已存在的
        foreach (var existed in existedExtOrders)
        {
            //外銷訂單錨點索引
            builderExtOrderIdxes.AppendFormat("<li class='dev-ext-order-idx' extordersid='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", existed);
        }

        Returner returner = null;
        try
        {
            #region 預設帶入外銷訂單全部的品項
            ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

            ExtOrderDet.InfoView[] extOrderDetInfos = new ExtOrderDet.InfoView[0];

            var conds = new ExtOrderDet.InfoViewConds
                (
                   DefVal.SIds,
                   DefVal.Long,
                   ConvertLib.ToSIds(seledExtOrders),
                   DefVal.Int,
                   DefVal.Str,
                   false,
                   false,
                   IncludeScope.All
                );

            SqlOrder sorting = SqlOrder.Default;
            returner = entityExtOrderDet.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
            if (returner.IsCompletedAndContinue)
            {
                extOrderDetInfos = ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);
            }

            //不顯示在手量
            ////先取得所有品項的在手量
            //ErpHelper.OnHandInfo[] onHandInfos = new ErpHelper.OnHandInfo[0];

            //先取得所有品項的價目表
            var priceBookInfos = new ErpHelper.PriceBookInfo[0];

            if (extOrderDetInfos.Length > 0)
            {
                //onHandInfos = ErpHelper.GetOnHandInfo(extOrderDetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray());

                var priceListId = ConvertLib.ToLong(this.hidPriceListId.Value, DefVal.Long);
                if (priceListId.HasValue)
                {
                    priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, extOrderDetInfos.Select(q => q.PartNo).ToArray());
                }
            }
            #endregion

            //已選擇的
            foreach (var seled in seledExtOrders)
            {
                //已存在的就不再加入
                if (existedExtOrders.Contains(seled))
                {
                    continue;
                }

                //加入已存在的外銷訂單暫存 (留在外面, 就算區塊被略過, 也讓專案報價不能再選.)
                existedExtOrders.Add(seled);

                //品項區塊
                var block = (ASP.include_client_ext_shipping_order_edit_goods_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_block.ascx");

                #region 預設帶入全部品項
                var filterExtOrderInfos = extOrderDetInfos.Where(q => q.ExtOrderSId.Equals(seled)).ToArray();

                //只有可用數量大於 0, 以及在訂單明細有該筆資料的才顯示.
                filterExtOrderInfos = filterExtOrderInfos.Where(q => ((q.Qty - q.ShipOdrUseQty) > 0) || (this._extShippingOrderSId != null && this.OrigInfo.DetInfos.Exists(det => det.ExtOrderDetSId.Equals(q.SId)))).ToArray();
                if (filterExtOrderInfos.Length > 0)
                {
                    ////加入已存在的外銷訂單暫存
                    //existedExtOrders.Add(seled);

                    //外銷訂單錨點索引
                    builderExtOrderIdxes.AppendFormat("<li class='dev-ext-order-idx' extordersid='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{1}</a></div></li>", seled, filterExtOrderInfos[0].OdrNo);

                    this._blockGoodsList.Add(block);
                    this.phGoodsList.Controls.Add(block);
                }
                else
                {
                    //若沒有任何品項, 則略過.
                    continue;
                }

                var editInfo = new ExtShippingOrderHelper.GoodsEditInfo_OdrBase()
                {
                    Title = filterExtOrderInfos[0].OdrNo,
                    ExtOrderSId = ConvertLib.ToSId(seled)
                };

                for (int i = 0; i < filterExtOrderInfos.Length; i++)
                {
                    var extOrderDetInfo = filterExtOrderInfos[i];

                    var itemEditInfo = new ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase()
                    {
                        SeqNo = i + 1,
                        ExtOrderDetSId = extOrderDetInfo.SId,
                        Model = extOrderDetInfo.Model,
                        PartNo = extOrderDetInfo.PartNo,
                        MaxQty = extOrderDetInfo.Qty - extOrderDetInfo.ShipOdrUseQty,
                        Discount = extOrderDetInfo.Discount
                    };

                    //不顯示在手量
                    //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, extOrderDetInfo.PartNo);
                    itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, extOrderDetInfo.PartNo);

                    editInfo.Items.Add(itemEditInfo);
                }

                block.SetInfo(editInfo, true);

                ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsBlock]";
                #endregion
            }

            //外銷訂單錨點索引
            this.litExtOrderIdxes.Text = builderExtOrderIdxes.ToString();

            //重整已存在的外銷訂單暫存
            this.hidExistedExtOrders.Value = string.Join(",", existedExtOrders.GroupBy(q => q).Select(q => q.Key).ToArray());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 繫結輸入
    ExtShippingOrderHelper.InputInfoSet BindingInputted(bool bindingAll)
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        Returner returner = null;
        try
        {
            this.InputInfo = new ExtShippingOrderHelper.InputInfoSet();

            this.InputInfo.Info.SId = new SystemId(this.hidSpecSId.Value);
            this.InputInfo.Info.Cdd = ConvertLib.ToDateTime(this.txtCdd.Text, DefVal.DT);
            this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
            this.InputInfo.Info.Eta = ConvertLib.ToDateTime(this.txtEta.Text, DefVal.DT);
            this.InputInfo.Info.CustomerId = ConvertLib.ToLong(this.hidCustomerId.Value, DefVal.Long);
            //this.InputInfo.Info.Rmk = this.txtRmk.Text;

            //明細品項的加入方式
            this.InputInfo.Info.DetLayout = 1;

            #region 客戶資訊
            if (this._extShippingOrderSId == null)
            {
                #region 新增模式
                this.InputInfo.Info.CurrencyCode = this.lblCurrencyCode.Text;

                if (this.InputInfo.Info.CustomerId.HasValue)
                {
                    //取得客戶資訊. 理論上一定取得到, 若取不到, 就直接出錯唄.
                    ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
                    returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, ConvertLib.ToLongs(this.InputInfo.Info.CustomerId.Value), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                        this.InputInfo.Info.CustomerNumber = info.CustomerNumber;
                        this.InputInfo.Info.CustomerName = info.CustomerName;
                        this.InputInfo.Info.CustomerConId = info.ContactId;
                        this.InputInfo.Info.CustomerConName = info.ConName;
                        this.InputInfo.Info.CustomerAreaCode = info.AreaCode;
                        this.InputInfo.Info.CustomerTel = info.Phone;
                        this.InputInfo.Info.CustomerFax = info.Fax;
                        this.InputInfo.Info.CustomerAddrId = info.ShipAddressId;
                        this.InputInfo.Info.CustomerAddr = info.ShipAddress1;
                        this.InputInfo.Info.PriceListId = info.PriceListId;
                        this.InputInfo.Info.CurrencyCode = info.CurrencyCode;
                        this.InputInfo.Info.ShipToSiteUseId = info.ShipToSiteUseId;
                        this.InputInfo.Info.InvoiceToSiteUseId = info.InvoiceToSiteUseId;
                        this.InputInfo.Info.OrderTypeId = info.OrderTypeId;
                        this.InputInfo.Info.SalesRepId = info.SalesRepId;
                        this.InputInfo.Info.SalesName = info.SalesName;
                    }
                }
                #endregion
            }
            else
            {
                #region 修改模式
                this.InputInfo.Info.CustomerNumber = this.litCustomerNumber.Text;
                this.InputInfo.Info.CustomerName = this.hidCustomerName.Value;
                this.InputInfo.Info.CustomerConName = this.lblCustomerConName.Text;
                this.InputInfo.Info.CustomerTel = this.lblCustomerTel.Text;
                this.InputInfo.Info.CustomerFax = this.lblCustomerFax.Text;
                this.InputInfo.Info.CustomerAddr = this.lblCustomerAddr.Text;

                this.InputInfo.Info.CurrencyCode = this.lblCurrencyCode.Text;
                #endregion
            }
            #endregion

            if (bindingAll)
            {
                #region 表頭折扣
                foreach (var block in this._blockHeaderDiscountList)
                {
                    if (block.Seled)
                    {
                        var erpDctRel = new ErpDctRel.InputInfo();
                        this.InputInfo.HeaderDiscountInfos.Add(erpDctRel);

                        erpDctRel.SId = new SystemId();
                        erpDctRel.RelCode = (int)ErpDctRel.RelCodeOpt.ExtShippingOrder_HeaderDiscount;
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

                #region 出貨單金額
                this.InputInfo.Info.TotalAmt = ConvertLib.ToSingle(this.hidTotalAmt.Value, DefVal.Float);
                this.InputInfo.Info.DctAmt = ConvertLib.ToSingle(this.hidDctAmt.Value, DefVal.Float);
                this.InputInfo.Info.DctTotalAmt = ConvertLib.ToSingle(this.hidDctTotalAmt.Value, DefVal.Float);
                #endregion

                #region 出貨單品項
                for (int i = 0; i < this._blockGoodsList.Count; i++)
                {
                    var block = this._blockGoodsList[i];
                    if (block.Visible)
                    {
                        var editInfo = block.GetInfo();
                        this.InputInfo.GoodsEditInfos_OdrBase.Add(editInfo);

                        foreach (var item in editInfo.Items)
                        {
                            var detInfo = new ExtShippingOrderDet.InputInfo()
                            {
                                SId = item.SId,
                                ExtShippingOrderSId = this.InputInfo.Info.SId,
                                ExtOrderDetSId = item.ExtOrderDetSId,
                                Model = item.Model,
                                PartNo = item.PartNo,
                                Qty = item.Qty,
                                ListPrice = item.ListPrice,
                                UnitPrice = item.UnitPrice,
                                UnitPriceDct = item.UnitPriceDct,
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
            }

            return this.InputInfo;
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region CheckInputted
    enum CheckMode
    {
        /// <summary>
        /// 建立單的的初始資訊。
        /// </summary>
        InitCreate,
        /// <summary>
        /// 新增。
        /// </summary>
        Create,
        /// <summary>
        /// 修改。
        /// </summary>
        Modify
    }

    bool CheckInputted(CheckMode checkMode)
    {
        this.BindingInputted(checkMode != CheckMode.InitCreate);

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        switch (checkMode)
        {
            case CheckMode.Create:
            case CheckMode.Modify:
                if (this.InputInfo.Info.Edd == null)
                {
                    errMsgs.Add("請輸入「預計交貨日」(或格式不正確)");
                }

                if (this.InputInfo.Info.Cdd == null)
                {
                    errMsgs.Add("請輸入「客戶需求日」(或格式不正確)");
                }

                //建立出貨單時, 預計到港日可能還未知.
                //if (this.InputInfo.Info.Eta == null)
                //{
                //    errMsgs.Add("請輸入「預計到港日」(或格式不正確)");
                //}
                break;
        }

        if (this.InputInfo.Info.CustomerId == null)
        {
            errMsgs.Add("請選擇「客戶」");
        }

        if (string.IsNullOrWhiteSpace(this.InputInfo.Info.CurrencyCode))
        {
            errMsgs.Add("請選擇「幣別」");
        }

        switch (checkMode)
        {
            case CheckMode.Create:
            case CheckMode.Modify:
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

                #region 出貨單金額
                //只要有品項, 且品項都有輸入, 則出貨單金額為自動產生.
                #endregion

                if (this.InputInfo.GoodsEditInfos_OdrBase.Count == 0)
                {
                    //理論上不會進來這邊, 保險起見.
                    errMsgs.Add("出貨單品項初始失敗");
                }
                else
                {
                    #region 品項驗證
                    var totalGoodsCnt = 0;
                    for (int i = 0; i < this.InputInfo.GoodsEditInfos_OdrBase.Count; i++)
                    {
                        totalGoodsCnt += this.InputInfo.GoodsEditInfos_OdrBase[i].Items.Count;
                    }

                    if (totalGoodsCnt == 0)
                    {
                        errMsgs.Add("未包含任何品項");
                    }
                    else
                    {
                        for (int i = 0; i < this.InputInfo.GoodsEditInfos_OdrBase.Count; i++)
                        {
                            var editInfo = this.InputInfo.GoodsEditInfos_OdrBase[i];

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
                                    if (itemInfo.Qty > itemInfo.MaxQty)
                                    {
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 「數量」超過限定的最大值", editInfo.Title, itemInfo.SeqNo));
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

                                if (itemInfo.UnitPriceDct == null)
                                {
                                    errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「折扣後單價」", editInfo.Title, itemInfo.SeqNo));
                                }
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
            case CheckMode.Create:
            case CheckMode.Modify:
                #region 檢查都 pass 後再重新計算一次 (不相信 JavaScript 的浮點運算)
                List<float> headerDcts = new List<float>();
                //總金額(未稅) = 所有小計總和
                float totalAmt = 0f;
                //所有小計要被扣掉的價錢總和
                float totalDownPrice = 0;
                //所有小計總和 (表頭折扣後)
                float totalAmtDct = 0f;
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

                    #region 加入「折扣後單價」前 - 備份 (已註解)
                    ////品項折扣及小計
                    //detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
                    //detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);

                    ////品項小計
                    //var subtotal = detInfo.PaidAmt.Value;

                    ////小計要被扣掉的價錢
                    //float downPrice = (float)MathLib.Round(subtotal * totalHeaderDct, 2);

                    ////因為要依表頭折扣同步計算小計, 所以資料庫品項小計也要更新.
                    //detInfo.PaidAmt = subtotal - downPrice;

                    ////所有小計總和
                    //totalAmt += subtotal;
                    ////所有小計要被扣掉的價錢總和
                    //totalDownPrice += downPrice;
                    ////所有小計總和 (表頭折扣後)
                    //totalAmtDct += subtotal - downPrice; //「小計」-「小計要被扣掉的價錢」= 依表頭折扣計算後的小計 
                    #endregion

                    #region 加入「折扣後單價」後
                    //單價要被扣掉的價錢
                    float downPrice = (float)MathLib.Round(detInfo.UnitPrice.Value * totalHeaderDct, 4);
                    //折扣後單價
                    detInfo.UnitPriceDct = (float)MathLib.Round(detInfo.UnitPriceDct.Value, 4);

                    //品項折扣及小計
                    detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
                    detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPriceDct.Value, 2);

                    //品項小計 (單價)
                    var subtotal = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);
                    //品項小計 (折扣後單價)
                    var subtotalDct = detInfo.PaidAmt.Value;

                    //所有小計總和
                    totalAmt += subtotal;
                    //所有小計要被扣掉的價錢總和
                    totalDownPrice += subtotal - subtotalDct; //小計總和 - 折扣後單價小計總和
                    //所有小計總和 (折扣後單價)
                    totalAmtDct += subtotalDct; //依折扣後單價計算後的小計 
                    #endregion
                }
                #endregion

                //總金額
                this.InputInfo.Info.TotalAmt = (float)MathLib.Round(totalAmt, 2);

                //折扣金額 = 所有小計要被扣掉的價錢總和
                this.InputInfo.Info.DctAmt = totalDownPrice;

                //折扣後總金額 = 所有小計總和 (折扣後單價)
                this.InputInfo.Info.DctTotalAmt = (float)MathLib.Round(totalAmtDct, 2);
                #endregion
                break;
        }

        return true;
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
                //儲存出貨單
                if (!this.Save(this._extShippingOrderSId == null ? CheckMode.Create : CheckMode.Modify))
                {
                    return;
                }

                transaction.Complete();

                QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
                query.Add("sid", this.InputInfo.Info.SId.Value);

                Response.Redirect(query.ToString());
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 出貨單確認
    protected void btnFixedShippingOrder_Click(object sender, EventArgs e)
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
                if (!this.Save(this._extShippingOrderSId == null ? CheckMode.Create : CheckMode.Modify))
                {
                    return;
                }

                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                //更改出貨單狀態
                entityExtShippingOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 1);

                transaction.Complete();

                QueryStringParsing query = new QueryStringParsing();
                query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                query.Add("sid", this.OrigInfo.Info.SId.Value);
                Response.Redirect(query.ToString());
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 刪除出貨單
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
                //0:草稿 1:已確認 2:已出貨 3:已上傳
                if (this.OrigInfo.Info.Status != 0)
                {
                    JSBuilder.AlertMessage(this, "[操作失敗]", "僅在草稿狀態方能執行刪除操作");
                    return;
                }

                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);
                ErpDctRel entityErpDctRel = new ErpDctRel(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                entityExtShippingOrder.Delete(actorSId, new ISystemId[] { this.OrigInfo.Info.SId });
                entityExtShippingOrderDet.DeleteByExtShippingOrderSId(new ISystemId[] { this.OrigInfo.Info.SId });
                entityErpDctRel.DeleteByUseSId(2, new ISystemId[] { this.OrigInfo.Info.SId });

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "出貨單已刪除");
            JSBuilder.PageRedirect(this, "index.aspx");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region Save
    bool Save(CheckMode checkMode)
    {
        switch (checkMode)
        {
            case CheckMode.InitCreate:
                return false;
            default:
                //驗證檢查
                if (!this.CheckInputted(checkMode))
                {
                    return false;
                }

                if (this._extShippingOrderSId == null)
                {
                    return this.Add();
                }
                else
                {
                    return this.Modify();
                }
        }
    }
    #endregion

    #region Add
    bool Add()
    {
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);

                var orderNo = entityExtShippingOrder.GetNewOrderNo();

                returner = entityExtShippingOrder.Add(actorSId, this.InputInfo.Info.SId, orderNo, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.Eta, this.InputInfo.Info.CustomerId.Value, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.DctAmt, this.InputInfo.Info.DctTotalAmt, this.InputInfo.Info.Rmk, this.InputInfo.Info.DetLayout.Value);
                if (returner.IsCompletedAndContinue)
                {
                    #region 表頭折扣
                    var relCode = (int)ErpDctRel.RelCodeOpt.ExtShippingOrder_HeaderDiscount;

                    ErpDctRel entityErpDctRel = new ErpDctRel(SystemDefine.ConnInfo);
                    foreach (var erpDctRel in this.InputInfo.HeaderDiscountInfos)
                    {
                        entityErpDctRel.Add(actorSId, relCode, this.InputInfo.Info.SId, erpDctRel.DiscountId.Value, erpDctRel.Discount.Value / 100);
                    }
                    #endregion

                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        //新增
                        entityExtShippingOrderDet.Add(actorSId, detInfo.SId, detInfo.ExtShippingOrderSId, detInfo.ExtOrderDetSId, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice.Value, detInfo.UnitPrice.Value, detInfo.UnitPriceDct.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                        //異動記錄
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    //繫結一次
                    this.OrigInfo = ExtShippingOrderHelper.Binding(this.InputInfo.Info.SId);

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

    #region Modify
    bool Modify()
    {
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                returner = entityExtShippingOrder.Modify(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.Eta, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.DctAmt, this.InputInfo.Info.DctTotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 表頭折扣
                    var relCode = (int)ErpDctRel.RelCodeOpt.ExtShippingOrder_HeaderDiscount;

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

                        if (this.OrigInfo.DetInfos.Where(q => q.SId.Equals(detInfo.SId)).Count() == 0)
                        {
                            //新增
                            entityExtShippingOrderDet.Add(actorSId, detInfo.SId, detInfo.ExtShippingOrderSId, detInfo.ExtOrderDetSId, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice.Value, detInfo.UnitPrice.Value, detInfo.UnitPriceDct.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        else
                        {
                            //修改
                            returner = entityExtShippingOrderDet.Modify(actorSId, detInfo.SId, detInfo.Qty.Value, detInfo.ListPrice.Value, detInfo.UnitPrice.Value, detInfo.UnitPriceDct.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
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
                            returner = entityExtShippingOrderDet.Delete(actorSId, ConvertLib.ToSIds(detInfo.SId));

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_SHIPPING_ORDER_DET, detInfo.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
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