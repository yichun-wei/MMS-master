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

public partial class popup_ext_order_trans_hx_cont : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_ORDER";
    string AUTH_NAME = "外銷訂單";

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
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    ExtOrderHelper.InputInfoSet InputInfo { get; set; }
    #endregion

    ISystemId _extOrderSId;

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
            JSBuilder.ClosePage(this);
            return false;
        }

        this._extOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._extOrderSId == null)
        {
            JSBuilder.ClosePage(this);
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
        this.PageTitle = "外銷訂單";

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0}) - 版本 {2}", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName, this.OrigInfo.Info.Version.ToString().PadLeft(3, '0'));
            }
            else
            {
                JSBuilder.ClosePage(this);
                return false;
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

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //重置幣別
        WebUtilBox.RegisterScript(this, "resetCurrencyCode();");
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtOrderHelper.Binding(DefVal.SId, this._extOrderSId, DefVal.Int, DefVal.Bool);
            if (this.OrigInfo != null)
            {
                this.litCName.Text = this.OrigInfo.Info.CName;

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.litCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");

                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtOrderHelper.GetExtOrderStatusName(this.OrigInfo.Info.Status, this.OrigInfo.Info.IsReadjust, this.OrigInfo.Info.IsCancel);
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

                    switch (this.OrigInfo.Info.Status)
                    {
                        case 1:
                            //待轉訂單還有「手動新增」
                            #region 待轉訂單
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
                            break;
                        default:
                            //除了待轉外,「手動新增」皆已完成對應.
                            #region 一般品項
                            if (true)
                            {
                                var generalInfos = this.OrigInfo.DetInfos.ToArray();
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

                            //手動新增
                            this.ucManualGoods.Visible = false;
                            break;
                    }
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.lblTotalAmtDisp.Text = ConvertLib.ToStr(this.OrigInfo.Info.TotalAmt, string.Empty);
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
}