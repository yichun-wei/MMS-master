//#define 偵錯建立生產單_只看不存

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

public partial class ext_order_view : System.Web.UI.Page
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
    /// 外銷生產單功能權限。
    /// </summary>
    FunctionRight ExtProdOrderRight { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    ExtOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _extQuotnSId;

    /// <summary>
    /// 建立生產的品項資訊。
    /// (與前版本訂單相比, 只有被異動的品項才要生產.)
    /// </summary>
    List<ExtOrderDet.InfoView> _buffDetInfos = new List<ExtOrderDet.InfoView>();

    class DetInfoFlag
    {
        public enum TransFlag
        {
            /// <summary>
            /// 原品項。
            /// </summary>
            Old,
            /// <summary>
            /// 新品項。
            /// </summary>
            New,
            /// <summary>
            /// 被刪除的品項。
            /// </summary>
            Del
        }

        public DetInfoFlag(TransFlag transFlag, int transQty)
        {
            this.Flag = transFlag;
            this.TransQty = transQty;
            this.HasTrans = transQty > 0;
        }

        /// <summary>
        /// 異動旗標。
        /// </summary>
        public TransFlag Flag { get; set; }
        /// <summary>
        /// 與前版本訂單的異動差（目前品項 - 前版本品項）。
        /// </summary>
        public int TransQty { get; set; }
        /// <summary>
        /// 是否仍存在且被增加品項數量。
        /// </summary>
        public bool HasTrans { get; set; }
    }

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

        //外銷生產單 權限
        this.ExtProdOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("EXT_PROD_ORDER");

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
        this.PageTitle = "外銷訂單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>外銷訂單</a>"));

        this.btnCreateProdOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定產生生產單？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                QueryStringParsing queryTransHx = new QueryStringParsing();
                queryTransHx.HttpPath = new Uri("../../popup/ext/order/trans_hx/index.aspx", UriKind.Relative);
                queryTransHx.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                this.lnkTransHx.Attributes["onclick"] = string.Format("window.open('{0}', 'trans_hx', 'width=900, height=700, top=100, left=100', scrollbars=1);", queryTransHx);

                #region 操作切換
                if (!this.IsPostBack)
                {
                    if (this.OrigInfo.Info.IsCancel)
                    {
                    }
                    else if (this.OrigInfo.Info.IsReadjust)
                    {
                    }
                    else
                    {
                        //1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程
                        switch (this.OrigInfo.Info.Status)
                        {
                            case 1:
                                QueryStringParsing queryEdit = new QueryStringParsing();
                                queryEdit.HttpPath = new Uri("edit.aspx", UriKind.Relative);
                                queryEdit.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                                Response.Redirect(queryEdit.ToString());
                                break;
                            case 2:
                                bool hasShipped = false; //是否已出貨

                                //外銷出貨單可能包含著不同訂單, 從出貨單無法反推使用的報價單, 故而從出貨單明細去反推.
                                ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);
                                returner = entityExtShippingOrderDet.GetInfoViewCount(new ExtShippingOrderDet.InfoViewConds(DefVal.SIds, DefVal.SIds, DefVal.SIds, DefVal.Str, this.OrigInfo.Info.OdrNo), IncludeScope.OnlyNotMarkDeleted);
                                hasShipped = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) > 0;

                                //只有在還沒建立出貨單前, 能產生生產單. (一個訂單版本只會有一個生產單)
                                if (this.OrigInfo.ProdInfo == null && !hasShipped)
                                {
                                    //外銷組權限
                                    this.btnCreateProdOrder.Visible = this.ExtProdOrderRight.Maintain && this.MainPage.ActorInfoSet.CheckExtAuditPerms(1);
                                }
                                break;
                            case 3:
                            case 4:
                                //改成用生產單系統代號
                                //QueryStringParsing queryProdOrder = new QueryStringParsing();
                                //queryProdOrder.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
                                ////改成用外銷訂單系統代號
                                ////queryProdOrder.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                                //queryProdOrder.Add("sid", this.OrigInfo.Info.SId.Value);

                                //this.lnkViewProdOrder.NavigateUrl = queryProdOrder.ToString();
                                ////外銷組權限
                                //this.lnkViewProdOrder.Visible = this.ExtProdOrderRight.Maintain && this.MainPage.ActorInfoSet.CheckExtAuditPerms(1);

                                //改成用生產單系統代號
                                ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                                returner = entityExtProdOrder.GetInfo(new ExtProdOrder.InfoConds(DefVal.SIds, this.OrigInfo.Info.SId, DefVal.Int, DefVal.Bool, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted, new string[] { "SID" });
                                if (returner.IsCompletedAndContinue)
                                {
                                    QueryStringParsing queryProdOrder = new QueryStringParsing();
                                    queryProdOrder.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
                                    //改成用外銷訂單系統代號
                                    //queryProdOrder.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                                    queryProdOrder.Add("sid", returner.DataSet.Tables[0].Rows[0]["SID"].ToString());

                                    this.lnkViewProdOrder.NavigateUrl = queryProdOrder.ToString();
                                    //外銷組權限
                                    this.lnkViewProdOrder.Visible = this.ExtProdOrderRight.Maintain && this.MainPage.ActorInfoSet.CheckExtAuditPerms(1);
                                }
                                else
                                {
                                    this.lnkViewProdOrder.Visible = false;
                                }
                                break;
                            default:
                                break;
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
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, true, true);
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

                    #region 一般品項
                    if (true)
                    {
                        var editInfo = new ExtOrderHelper.GoodsEditInfo();

                        int seqNo = 0;

                        #region 以前一版本品項為主, 跑異動或刪除的品項.
                        if (this.OrigInfo.PrevVerDetInfos.Count > 0)
                        {
                            for (int i = 0; i < this.OrigInfo.PrevVerDetInfos.Count; i++)
                            {
                                var detInfo = this.OrigInfo.PrevVerDetInfos[i];

                                int beforeTransQty = detInfo.Qty;
                                var curVerDetInfo = this.OrigInfo.DetInfos.Where(q => q.Model == detInfo.Model && q.PartNo == detInfo.PartNo).DefaultIfEmpty(null).SingleOrDefault();
                                if (curVerDetInfo != null)
                                {
                                    #region 在目前版本找得到
                                    //curVerDetInfo.Tag = true;
                                    curVerDetInfo.Tag = new DetInfoFlag(DetInfoFlag.TransFlag.Old, curVerDetInfo.Qty - detInfo.Qty);

                                    //要加入生產單的品項
                                    this._buffDetInfos.Add(curVerDetInfo);

                                    var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                                    {
                                        SId = curVerDetInfo.SId,
                                        SeqNo = ++seqNo,
                                        Model = curVerDetInfo.Model,
                                        PartNo = curVerDetInfo.PartNo,
                                        Qty = curVerDetInfo.Qty,
                                        UnitPrice = curVerDetInfo.UnitPrice,
                                        Discount = curVerDetInfo.Discount,
                                        PaidAmt = curVerDetInfo.PaidAmt,
                                        Rmk = curVerDetInfo.Rmk,

                                        BeforeTransQty = beforeTransQty
                                    };

                                    //不顯示在手量
                                    //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, curVerDetInfo.PartNo);
                                    itemEditInfo.ListPrice = curVerDetInfo.ListPrice;

                                    editInfo.Items.Add(itemEditInfo);
                                    #endregion
                                }
                                else
                                {
                                    #region 在目前版本找不到 (已刪除)
                                    //要加入生產單的品項
                                    detInfo.Tag = new DetInfoFlag(DetInfoFlag.TransFlag.Del, 0 - detInfo.Qty);
                                    this._buffDetInfos.Add(detInfo);

                                    var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                                    {
                                        SId = detInfo.SId,
                                        SeqNo = ++seqNo,
                                        Model = detInfo.Model,
                                        PartNo = detInfo.PartNo,
                                        Qty = 0,
                                        UnitPrice = detInfo.UnitPrice,
                                        Discount = detInfo.Discount,
                                        PaidAmt = 0,
                                        Rmk = detInfo.Rmk,

                                        BeforeTransQty = beforeTransQty
                                    };

                                    //不顯示在手量
                                    //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                                    itemEditInfo.ListPrice = detInfo.ListPrice;

                                    editInfo.Items.Add(itemEditInfo);
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region 以目前版本品項為主, 跑新增的品項, 或是沒有前一版本的品項.
                        var detInfos = this.OrigInfo.DetInfos.Where(q => q.Tag == null).ToList();
                        for (int i = 0; i < detInfos.Count; i++)
                        {
                            var detInfo = detInfos[i];

                            //要加入生產單的品項
                            detInfo.Tag = new DetInfoFlag(DetInfoFlag.TransFlag.New, detInfo.Qty);
                            this._buffDetInfos.Add(detInfo);

                            int? beforeTransQty = null;

                            var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                            {
                                SId = detInfo.SId,
                                SeqNo = ++seqNo,
                                Model = detInfo.Model,
                                PartNo = detInfo.PartNo,
                                Qty = detInfo.Qty,
                                UnitPrice = detInfo.UnitPrice,
                                Discount = detInfo.Discount,
                                PaidAmt = detInfo.PaidAmt,
                                Rmk = detInfo.Rmk,

                                BeforeTransQty = beforeTransQty
                            };

                            //不顯示在手量
                            //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                            itemEditInfo.ListPrice = detInfo.ListPrice;

                            editInfo.Items.Add(itemEditInfo);
                        }
                        #endregion

                        #region 備份 (已註解)
                        //for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++)
                        //{
                        //    var detInfo = this.OrigInfo.DetInfos[i];

                        //    int? beforeTransQty = null;
                        //    if (this.OrigInfo.PrevVerDetInfos.Count > 0)
                        //    {
                        //        var prevVerDetInfo = this.OrigInfo.PrevVerDetInfos.Where(q => q.Model == detInfo.Model && q.PartNo == detInfo.PartNo).DefaultIfEmpty(null).SingleOrDefault();
                        //        if (prevVerDetInfo != null)
                        //        {
                        //            beforeTransQty = prevVerDetInfo.Qty;
                        //        }
                        //    }

                        //    var itemEditInfo = new ExtOrderHelper.GoodsItemEditInfo()
                        //    {
                        //        SId = detInfo.SId,
                        //        SeqNo = i + 1,
                        //        Model = detInfo.Model,
                        //        PartNo = detInfo.PartNo,
                        //        Qty = detInfo.Qty,
                        //        UnitPrice = detInfo.UnitPrice,
                        //        Discount = detInfo.Discount,
                        //        PaidAmt = detInfo.PaidAmt,
                        //        Rmk = detInfo.Rmk,

                        //        BeforeTransQty = beforeTransQty
                        //    };

                        //    //不顯示在手量
                        //    //itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);
                        //    itemEditInfo.ListPrice = detInfo.ListPrice;

                        //    editInfo.Items.Add(itemEditInfo);
                        //}
                        #endregion

                        this.ucGeneralGoods.SetInfo(editInfo);
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

    #region 產生生產單 (備份 - 已註解)
    //protected void btnCreateProdOrder_Click(object sender, EventArgs e)
    //{
    //    //網頁未完成初始時不執行
    //    if (!this.HasInitial) { return; }

    //    //沒有權限
    //    if (!this.ExtProdOrderRight.Maintain) { return; }

    //    #region 驗證檢查
    //    List<string> errMsgs = new List<string>();

    //    if (this.OrigInfo.Info.IsCancel)
    //    {
    //        errMsgs.Add("狀態已變更為「報價單已取消」");
    //    }
    //    else if (this.OrigInfo.Info.IsReadjust)
    //    {
    //        errMsgs.Add("狀態已變更為「報價單調整中」");
    //    }

    //    if (errMsgs.Count != 0)
    //    {
    //        JSBuilder.AlertMessage(this, errMsgs.ToArray());
    //        return;
    //    }
    //    #endregion

    //    Returner returner = null;
    //    try
    //    {
    //        ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

    //        //先取得所有品項的在手量
    //        var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

    //        using (var transaction = new TransactionScope(TransactionScopeOption.Required))
    //        {
    //            ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
    //            //更改訂單狀態
    //            entityExtOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 3);

    //            //若已產生過生產單編號的, 就不再產生.
    //            if (string.IsNullOrWhiteSpace(this.OrigInfo.Info.ProdOdrNo))
    //            {
    //                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
    //                var prodOrderNo = entityExtQuotn.GetNewProdOrderNo();

    //                entityExtQuotn.UpdateProdOrderNo(actorSId, this.OrigInfo.Info.ExtQuotnSId, prodOrderNo);
    //            }

    //            //先取得建議生產量資訊
    //            ExtOrderDet.RecProdInfoView[] recProdInfos = new ExtOrderDet.RecProdInfoView[0];
    //            ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);
    //            returner = entityExtOrderDet.GetRecProdRecProdInfoView(new ExtOrderDet.RecProdInfoViewConds(DefVal.SIds, this.OrigInfo.Info.SId));
    //            if (returner.IsCompletedAndContinue)
    //            {
    //                recProdInfos = ExtOrderDet.RecProdInfoView.Binding(returner.DataSet.Tables[0]);
    //            }

    //            #region 產生生產單
    //            ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
    //            ExtProdOrderDet entityExtProdOrderDet = new ExtProdOrderDet(SystemDefine.ConnInfo);

    //            ISystemId extProdOrderSId = new SystemId();

    //            entityExtProdOrder.Add(actorSId, extProdOrderSId, this.OrigInfo.Info.SId, 1, this.OrigInfo.Info.QuotnDate, this.OrigInfo.Info.Cdd, this.OrigInfo.Info.Edd, this.OrigInfo.Info.Rmk);

    //            //需要生產的品項數量
    //            int newProdDetCount = 0;
    //            foreach (var detInfo in this.OrigInfo.DetInfos)
    //            {
    //                ISystemId extProdOrderDetSId = new SystemId();

    //                /********************************************************************************
    //                 * 單一品項的計算建議生產量公式:
    //                 * 公式: 當下「在手量」- (A訂單10量 + B訂單5量 + C訂單3量) + 同一個訂單的前一個有產生生產單的版號的同品項 = 建議生產量
    //                 * ABC 為不同的訂單編號, 且不管是否有產生過生產單, 取最後一個版本.
    //                 * 若建議生產量「大於等於 0」, 則不需要生產.
    //                 * 不同訂單編號的狀態為 (還沒有出貨的品項): 正式訂單、正式訂單-未排程、正式訂單-已排程
    //                ********************************************************************************/

    //                //產生生產單時, 計算建議生產量.
    //                int recProdQty = 0;
    //                var erpOnHand = ConvertLib.ToInt(ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo), 0);
    //                //一定對應的到, 若找不到, 表示異常, 就直接拋例外唄.
    //                var recProdInfo = recProdInfos.Where(q => q.SId.Equals(detInfo.SId)).Single();
    //                //在手量 - (所有外銷訂單未出貨的同品項數量 - 目前外銷訂單未出貨的同品項數量) - 需求量 + 同一個訂單的前一個有產生生產單的版號的同品項
    //                //「所有外銷訂單未出貨的同品項數量 - 目前外銷訂單未出貨的同品項數量」的目的在還原該品項需求量
    //                recProdQty = erpOnHand - (recProdInfo.AllNotShipQty - recProdInfo.CurNotShipQty) - detInfo.Qty + recProdInfo.PrevProdQty;
    //                //若為負值, 表示需要生產.
    //                if (recProdQty < 0)
    //                {
    //                    entityExtProdOrderDet.Add(actorSId, extProdOrderDetSId, extProdOrderSId, detInfo.Source, detInfo.OrganizationCode, detInfo.Model, detInfo.PartNo, detInfo.Qty, recProdQty, DefVal.DT, detInfo.Rmk, detInfo.Sort);
    //                    newProdDetCount++;
    //                }
    //            }
    //            #endregion

    //            if (newProdDetCount > 0)
    //            {
    //                transaction.Complete();

    //                JSBuilder.AlertMessage(this, true, "資料已儲存");

    //                QueryStringParsing query = new QueryStringParsing();
    //                query.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
    //                query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
    //                JSBuilder.PageRedirect(this, true, query.ToString());
    //            }
    //            else
    //            {
    //                JSBuilder.AlertMessage(this, true, "品項數量已足夠，毋須生產。");
    //            }
    //        }
    //    }
    //    finally
    //    {
    //        if (returner != null) { returner.Dispose(); }
    //    }
    //}
    #endregion

    #region 產生生產單
    protected void btnCreateProdOrder_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //沒有權限
        if (!this.ExtProdOrderRight.Maintain) { return; }

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        if (this.OrigInfo.Info.IsCancel)
        {
            errMsgs.Add("狀態已變更為「報價單已取消」");
        }
        else if (this.OrigInfo.Info.IsReadjust)
        {
            errMsgs.Add("狀態已變更為「報價單調整中」");
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
            QueryStringParsing query;
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            //先取得所有品項的在手量
            //var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);
            var onHandInfos = ErpHelper.GetOnHandInfo(this._buffDetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

            TransactionScopeOption transScoptOption = TransactionScopeOption.Required;

#if 偵錯建立生產單_只看不存
            transScoptOption = TransactionScopeOption.Suppress;
#endif
            using (var transaction = new TransactionScope(transScoptOption))
            {
                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);
                ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                ExtProdOrderDet entityExtProdOrderDet = new ExtProdOrderDet(SystemDefine.ConnInfo);
                ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);

                #region 檢查是否已產生過生產單
                if (this.OrigInfo.ProdInfo != null)
                {
                    JSBuilder.AlertMessage(this, true, "已產生過生產單");

                    query = new QueryStringParsing();
                    query.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
                    query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                    JSBuilder.PageRedirect(this, true, query.ToString());
                    return;
                }
                #endregion

                #region 檢查是否已出貨
                bool hasShipped = false; //是否已出貨

                //外銷出貨單可能包含著不同訂單, 從出貨單無法反推使用的報價單, 故而從出貨單明細去反推.
                returner = entityExtShippingOrderDet.GetInfoViewCount(new ExtShippingOrderDet.InfoViewConds(DefVal.SIds, DefVal.SIds, DefVal.SIds, DefVal.Str, this.OrigInfo.Info.OdrNo), IncludeScope.OnlyNotMarkDeleted);
                hasShipped = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) > 0;

                if (hasShipped)
                {
                    JSBuilder.AlertMessage(this, true, "訂單已包含出貨品項");

                    query = new QueryStringParsing();
                    query.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
                    query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                    JSBuilder.PageRedirect(this, true, query.ToString());
                    return;
                }
                #endregion

                //先取得最後一次產生生產單的外銷訂單品項集合 (若有生產過的話)
                ExtOrderDet.Info[] itemOfLastProdInfos = null;
                //注意!! 就算有生產單編號也不一定取得到值, 這裡是只「已確認」的生產單.
                returner = entityExtOrderDet.GetItemOfLastProdInfoView(new ExtOrderDet.ItemOfLastProdInfoViewConds(this.OrigInfo.Info.ExtQuotnSId));
                if (returner.IsCompletedAndContinue)
                {
                    itemOfLastProdInfos = ExtOrderDet.Info.Binding(returner.DataSet.Tables[0]);
                }

                //先取得未產生過生產單且未出貨的品項 (未出貨的出貨單包含「草稿、未出貨」的出貨單狀態)
                //先取得「(未產生過生產單 AND 未產生過生產單的目前品項數量大於前一版已產生過生產單的同品項數量) AND 未出貨的品項 (未出貨的出貨單包含「草稿、未出貨」的出貨單狀態)」
                ExtOrderDet.Info[] itemOfNotProdAndShippingInfos = null;
                returner = entityExtOrderDet.GetNotProdAndShippingInfoView();
                if (returner.IsCompletedAndContinue)
                {
                    itemOfNotProdAndShippingInfos = ExtOrderDet.Info.Binding(returner.DataSet.Tables[0]);
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
#if 偵錯建立生產單_只看不存
#else
                //更改訂單狀態
                entityExtOrder.UpdateStatus(actorSId, this.OrigInfo.Info.SId, 3);
#endif

                #region 產生生產單
                ISystemId extProdOrderSId = new SystemId();
                var prodOrderNo = entityExtProdOrder.GetNewProdOrderNo();

#if 偵錯建立生產單_只看不存
#else
                entityExtProdOrder.Add(actorSId, extProdOrderSId, prodOrderNo, this.OrigInfo.Info.SId, 1, this.OrigInfo.Info.QuotnDate, this.OrigInfo.Info.Cdd, this.OrigInfo.Info.Edd, this.OrigInfo.Info.Rmk);
#endif

                //加入生產單的品項集合
                List<ExtOrderDet.InfoView> prodDetInfos = new List<ExtOrderDet.InfoView>();

                //foreach (var detInfo in this.OrigInfo.DetInfos)
                foreach (var detInfo in this._buffDetInfos)
                {
                    ISystemId extProdOrderDetSId = new SystemId();

                    var detFlag = (DetInfoFlag)detInfo.Tag;

                    /********************************************************************************
                     * [生產量] = [欄位:需求量] + [其他未產生過生產單的訂單明細數量加總(本張訂單數量除外)] - [欄位:庫存量]
                     * 
                     * 需求量定義
                     * 有產生過生產單的: 目前需求量 - 最後一次產生生產單的外銷訂單品項需求量(注意:不是生產單哦)
                     * 沒有產生過生產單的: 就是目前需求量
                     * 其他未產生生產單的需求量(草稿、未出貨, 扣掉已出貨)
                    ********************************************************************************/

                    int recProdQty = 0; //建議生產量
                    int demandQty = 0; //需求量
                    int itemOfNotProdAndShippingQty = 0; //未產生過生產單且未出貨的品項數量
                    var erpOnHand = ConvertLib.ToInt(ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo), 0); //在手量

                    if (detFlag.Flag != DetInfoFlag.TransFlag.Del)
                    {
                        #region 計算需求量
                        if (itemOfLastProdInfos == null)
                        {
                            //沒有產生過生產單的: 就是目前需求量
                            demandQty = detInfo.Qty;
                        }
                        else
                        {
                            var itemOfLastProdInfo = itemOfLastProdInfos.Where(q => q.PartNo == detInfo.PartNo).DefaultIfEmpty(null).SingleOrDefault();
                            if (itemOfLastProdInfo != null)
                            {
                                //有產生過生產單的: 目前需求量 - 最後一次產生生產單的外銷訂單品項需求量(注意:不是生產單哦)
                                demandQty = detInfo.Qty - itemOfLastProdInfo.Qty;
                                if (demandQty < 1)
                                {
                                    //若小於等於 0 則表示不用生產
                                    demandQty = 0;
                                }
                            }
                            else
                            {
                                //沒有生產過的品項, 則使用目前需求量.
                                demandQty = detInfo.Qty;
                            }
                        }
                        #endregion

                        //不包含目前訂單的其他未產生過生產單且未出貨的同品項數量
                        if (itemOfNotProdAndShippingInfos != null)
                        {
                            itemOfNotProdAndShippingQty = itemOfNotProdAndShippingInfos.Where(q => !q.ExtOrderSId.Equals(this.OrigInfo.Info.SId) && q.PartNo == detInfo.PartNo).Sum(q => q.Qty);
                        }
                        //[生產量] = [欄位:需求量] + [其他未產生過生產單的訂單明細數量加總(本張訂單數量除外)] - [欄位:庫存量]
                        recProdQty = demandQty + itemOfNotProdAndShippingQty - erpOnHand;

                        //若為負值, 表示不需要生產.
                        if (recProdQty < 0)
                        {
                            recProdQty = 0;

                            //不需要生產則略過
                            continue;
                        }

                        prodDetInfos.Add(detInfo);

#if 偵錯建立生產單_只看不存
#else
                        //[20160307 by fan] 外銷生產單明細的數量改為「需求量」
                        //entityExtProdOrderDet.Add(actorSId, extProdOrderDetSId, extProdOrderSId, detInfo.Source, detInfo.OrganizationCode, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.CumProdQty, recProdQty, DefVal.DT, detInfo.Rmk, detInfo.Sort);
                        entityExtProdOrderDet.Add(actorSId, extProdOrderDetSId, extProdOrderSId, detInfo.Source, detInfo.OrganizationCode, detInfo.Model, detInfo.PartNo, demandQty, detInfo.CumProdQty, recProdQty, DefVal.DT, detInfo.Rmk, detInfo.Sort);
#endif
                    }
                }
                #endregion

                if (prodDetInfos.Count > 0)
                {
#if 偵錯建立生產單_只看不存
#else
                    transaction.Complete();
#endif

                    JSBuilder.AlertMessage(this, true, "資料已儲存");

                    query = new QueryStringParsing();
                    query.HttpPath = new Uri("../prod_order/edit.aspx", UriKind.Relative);
                    query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                    JSBuilder.PageRedirect(this, true, query.ToString());
                }
                else
                {
                    JSBuilder.AlertMessage(this, true, "沒有需要生產的品項");
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