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

public partial class ext_quotn_view : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_QUOTN";
    string AUTH_NAME = "外銷報價單";

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
    ExtOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _extQuotnSId;

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

        this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["quotnSId"]);
        if (this._extQuotnSId == null)
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
        this.PageTitle = "外銷報價單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>外銷報價單</a>"));

        this.btnCopyOrder.OnClientClick = string.Format("javascript:if(!window.confirm('將複製此操作產生一份新的報價單，但此操作不會被儲存。\\n\\n確定複製？')){{return false;}}");
        this.btnReadjust.OnClientClick = string.Format("javascript:if(!window.confirm('確定要修改？')){{return false;}}");
        this.btnCancel.OnClientClick = string.Format("javascript:if(!window.confirm('確定要取消？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                #region 操作切換
                if (!this.IsPostBack)
                {
                    if (this.OrigInfo.Info.IsCancel)
                    {
                        //已取消
                    }
                    else
                    {
                        bool hasProded = false; //是否已產生生產單 (同一報價單中所有訂單版本的生產單, 不論是否已確認.)
                        bool hasShipped = false; //是否已出貨

                        ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                        returner = entityExtProdOrder.GetInfoViewCount(new ExtProdOrder.InfoViewConds(DefVal.SIds, DefVal.SIds, this.OrigInfo.Info.ExtQuotnSId, DefVal.SId, DefVal.Int, DefVal.Bool, DefVal.Int, DefVal.Bool, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, false), IncludeScope.OnlyNotMarkDeleted);
                        hasProded = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) > 0;

                        //外銷出貨單可能包含著不同訂單, 從出貨單無法反推使用的報價單, 故而從出貨單明細去反推.
                        ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);
                        returner = entityExtShippingOrderDet.GetInfoViewCount(new ExtShippingOrderDet.InfoViewConds(DefVal.SIds, DefVal.SIds, DefVal.SIds, DefVal.Str, this.OrigInfo.Info.OdrNo), IncludeScope.OnlyNotMarkDeleted);
                        hasShipped = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) > 0;

                        //未產生生產單及未出貨時, 才允許取消.
                        this.btnCancel.Visible = !hasProded && !hasShipped;

                        if (!this.OrigInfo.Info.IsReadjust && !hasShipped)
                        {
                            //1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程
                            switch (this.OrigInfo.Info.Status)
                            {
                                case 1:
                                case 3:
                                    //「待轉訂單」、「正式訂單-未排程」也不能修改報價單.
                                    break;
                                default:
                                    //只有在狀態為「正式訂單」以後, 及還沒建立出貨單前, 能修改報價單.
                                    this.btnReadjust.Visible = true;
                                    break;
                            }
                        }
                    }
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

        //權限操作檢查
        this.btnReadjust.Visible = this.btnReadjust.Visible ? this.FunctionRight.Maintain : this.btnReadjust.Visible;
        this.btnCancel.Visible = this.btnCancel.Visible ? this.FunctionRight.Maintain : this.btnCancel.Visible;
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, true, false);
            if (this.OrigInfo != null)
            {
                this.litCName.Text = this.OrigInfo.Info.CName;

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.litCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");

                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
                //this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                //this.litStatus.Text = ExtOrderHelper.GetExtOrderStatusName(this.OrigInfo.Info.Status);
                this.litStatus.Text = ExtQuotnHelper.GetExtQuotnStatusName(this.OrigInfo.Info.ExtQuotnStatus, this.OrigInfo.Info.IsReadjust, this.OrigInfo.Info.IsCancel);
                this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);

                //幣別
                this.lblCurrencyCode.Text = this.OrigInfo.Info.CurrencyCode;

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
                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

                    #region 一般品項
                    if (true)
                    {
                        var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();
                        var editInfo = new ExtOrderHelper.GoodsEditInfo();

                        for (int i = 0; i < generalInfos.Length; i++)
                        {
                            var detInfo = generalInfos[i];

                            var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                            {
                                SId = detInfo.SId,
                                SeqNo = i + 1,
                                Model = detInfo.Model,
                                PartNo = detInfo.PartNo,
                                Qty = detInfo.Qty,
                                UnitPrice = detInfo.UnitPrice,
                                Discount = detInfo.Discount,
                                PaidAmt = detInfo.PaidAmt,
                                Rmk = detInfo.Rmk
                            };

                            //不顯示在手量
                            //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                            itemEditInfo.ListPrice = detInfo.ListPrice;

                            editInfo.Items.Add(itemEditInfo);
                        }

                        this.ucGeneralGoods.SetInfo(editInfo);
                    }
                    #endregion

                    #region 手動新增
                    if (true)
                    {
                        var manualInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).ToArray();
                        var editInfo = new ExtOrderHelper.GoodsEditInfo();

                        for (int i = 0; i < manualInfos.Length; i++)
                        {
                            var detInfo = manualInfos[i];

                            var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                            {
                                SId = detInfo.SId,
                                SeqNo = i + 1,
                                Model = detInfo.Model,
                                PartNo = detInfo.PartNo,
                                Qty = detInfo.Qty,
                                UnitPrice = detInfo.UnitPrice,
                                Discount = detInfo.Discount,
                                PaidAmt = detInfo.PaidAmt,
                                Rmk = detInfo.Rmk
                            };

                            //不顯示在手量
                            //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                            itemEditInfo.ListPrice = detInfo.ListPrice;

                            editInfo.Items.Add(itemEditInfo);
                        }

                        this.ucManualGoods.SetInfo(editInfo);
                    }
                    #endregion
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.lblTotalAmtDisp.Text = ConvertLib.ToAccounting(this.OrigInfo.Info.TotalAmt, string.Empty);
                #endregion

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

    #region 複製
    protected void btnCopyOrder_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            ExtQuotnHelper.InfoSet quotnInfo;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);

                //取得報價單資訊
                quotnInfo = ExtQuotnHelper.Binding(this.OrigInfo.Info.ExtQuotnSId);

                //重新產生報價單系統代號
                quotnInfo.Info.SId = new SystemId();
                //重新產生品項系統代號
                foreach (var detInfo in quotnInfo.DetInfos)
                {
                    detInfo.SId = new SystemId();
                    detInfo.ExtQuotnSId = quotnInfo.Info.SId;
                }

                var orderNo = entityExtQuotn.GetNewOrderNo();

                returner = entityExtQuotn.Add(actorSId, quotnInfo.Info.SId, orderNo, quotnInfo.Info.QuotnDate, quotnInfo.Info.Cdd, quotnInfo.Info.Edd, quotnInfo.Info.CusterSrc, quotnInfo.Info.CustomerId, quotnInfo.Info.CustomerNumber, quotnInfo.Info.CustomerName, quotnInfo.Info.CustomerConId, quotnInfo.Info.CustomerConName, quotnInfo.Info.CustomerAreaCode, quotnInfo.Info.CustomerTel, quotnInfo.Info.CustomerFax, quotnInfo.Info.CustomerAddrId, quotnInfo.Info.CustomerAddr, quotnInfo.Info.PriceListId, quotnInfo.Info.CurrencyCode, quotnInfo.Info.ShipToSiteUseId, quotnInfo.Info.InvoiceToSiteUseId, quotnInfo.Info.OrderTypeId, quotnInfo.Info.SalesRepId, quotnInfo.Info.SalesName, quotnInfo.Info.RcptName, quotnInfo.Info.RcptCusterName, quotnInfo.Info.RcptTel, quotnInfo.Info.RcptFax, quotnInfo.Info.RcptAddr, quotnInfo.Info.FreightWaySId, quotnInfo.Info.TotalAmt, quotnInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項 (使用報價單品項) - 已註解
                    //for (int i = 0; i < quotnInfo.DetInfos.Count; i++)
                    //{
                    //    var detInfo = quotnInfo.DetInfos[i];

                    //    //新增
                    //    entityExtQuotnDet.Add(actorSId, detInfo.SId, detInfo.ExtQuotnSId, detInfo.Source, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt, detInfo.Rmk, i + 1);

                    //    //異動記錄
                    //    //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                    //    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    //    {
                    //    }
                    //}
                    #endregion

                    #region 品項 (使用訂單品項)
                    for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.OrigInfo.DetInfos[i];

                        var detSId = new SystemId();

                        //新增
                        entityExtQuotnDet.Add(actorSId, detSId, quotnInfo.Info.SId, 1, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt, detInfo.Rmk, i + 1);

                        //異動記錄
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detSId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, quotnInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    transaction.Complete();
                }
            }

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("edit.aspx", UriKind.Relative);
            query.Add("sid", quotnInfo.Info.SId.Value);

            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 修改 (報價調整中)
    protected void btnReadjust_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //同步更新報價單
                if (!this.SyncUpdateExtQuotn())
                {
                    return;
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

                #region 建立新版本
                var extOrderSId = new SystemId();
                returner = entityExtOrder.Add(actorSId, extOrderSId, this.OrigInfo.Info.ExtQuotnSId, 1, this.OrigInfo.Info.QuotnDate, this.OrigInfo.Info.Cdd, this.OrigInfo.Info.Edd, this.OrigInfo.Info.CusterSrc, this.OrigInfo.Info.CustomerId, this.OrigInfo.Info.CustomerNumber, this.OrigInfo.Info.CustomerName, this.OrigInfo.Info.CustomerConId, this.OrigInfo.Info.CustomerConName, this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerTel, this.OrigInfo.Info.CustomerFax, this.OrigInfo.Info.CustomerAddrId, this.OrigInfo.Info.CustomerAddr, this.OrigInfo.Info.PriceListId, this.OrigInfo.Info.CurrencyCode, this.OrigInfo.Info.ShipToSiteUseId, this.OrigInfo.Info.InvoiceToSiteUseId, this.OrigInfo.Info.OrderTypeId, this.OrigInfo.Info.SalesRepId, this.OrigInfo.Info.SalesName, this.OrigInfo.Info.RcptName, this.OrigInfo.Info.RcptCusterName, this.OrigInfo.Info.RcptTel, this.OrigInfo.Info.RcptFax, this.OrigInfo.Info.RcptAddr, this.OrigInfo.Info.FreightWaySId, this.OrigInfo.Info.TotalAmt, this.OrigInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項
                    for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.OrigInfo.DetInfos[i];

                        //新增
                        //entityExtOrderDet.Add(actorSId, new SystemId(), extOrderSId, detInfo.Source, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, detInfo.Discount, detInfo.PaidAmt, detInfo.Rmk, detInfo.Sort);
                        //調整報價單後, 品項來源全改為「1:一般品項」.
                        entityExtOrderDet.Add(actorSId, new SystemId(), extOrderSId, 1, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, detInfo.Discount, detInfo.PaidAmt, detInfo.Rmk, detInfo.Sort);

                        //異動記錄
                        //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", this.OrigInfo.Info.OdrNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER, extOrderSId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                    //更改報價單調整中
                    entityExtQuotn.UpdateReadjustFlag(actorSId, this.OrigInfo.Info.ExtQuotnSId, true);

                    transaction.Complete();
                }
                #endregion
            }

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("readjust.aspx", UriKind.Relative);
            query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 同步更新報價單
    bool SyncUpdateExtQuotn()
    {
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtQuotnHelper.InfoSet quotnInfoSet = ExtQuotnHelper.Binding(this.OrigInfo.Info.ExtQuotnSId);
                if (quotnInfoSet != null)
                {
                    ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                    ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);

                    //異動記錄
                    string dataTitle = quotnInfoSet.Info.OdrNo;
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, quotnInfoSet.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(quotnInfoSet)))
                    {
                    }

                    returner = entityExtQuotn.Modify(actorSId, quotnInfoSet.Info.SId, this.OrigInfo.Info.QuotnDate, this.OrigInfo.Info.Cdd, this.OrigInfo.Info.Edd, this.OrigInfo.Info.CustomerNumber, this.OrigInfo.Info.CustomerName, this.OrigInfo.Info.CustomerConName, this.OrigInfo.Info.CustomerTel, this.OrigInfo.Info.CustomerFax, this.OrigInfo.Info.CustomerAddr, this.OrigInfo.Info.RcptName, this.OrigInfo.Info.RcptCusterName, this.OrigInfo.Info.RcptTel, this.OrigInfo.Info.RcptFax, this.OrigInfo.Info.RcptAddr, this.OrigInfo.Info.FreightWaySId, this.OrigInfo.Info.TotalAmt, this.OrigInfo.Info.Rmk);
                    if (returner.IsCompletedAndContinue)
                    {
                        #region 品項
                        //品項全部刪除重建
                        foreach (var detInfo in quotnInfoSet.DetInfos)
                        {
                            //刪除
                            returner = entityExtQuotnDet.Delete(actorSId, ConvertLib.ToSIds(detInfo.SId));

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
                            {
                            }
                        }

                        for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++)
                        {
                            var detInfo = this.OrigInfo.DetInfos[i];

                            //新增
                            //entityExtQuotnDet.Add(actorSId, detInfo.SId, quotnInfoSet.Info.SId, detInfo.Source, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt, detInfo.Rmk, i + 1);
                            //調整報價單後, 品項來源全改為「1:一般品項」.
                            entityExtQuotnDet.Add(actorSId, detInfo.SId, quotnInfoSet.Info.SId, 1, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            //using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            //{
                            //}
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        #endregion

                        transaction.Complete();
                    }

                    return true;
                }
                else
                {
                    //理論上不可能找不到, 保險起見.
                    JSBuilder.AlertMessage(this, true, "找不到原始報價單");
                    return false;
                }
            }
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
        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                returner = entityExtQuotn.UpdateCancelInfo(actorSId, this.OrigInfo.Info.ExtQuotnSId, true, DateTime.Now);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "報價單已取消");
            JSBuilder.PageRedirect(this, QueryStringParsing.CurrentRelativeUri.OriginalString);
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}