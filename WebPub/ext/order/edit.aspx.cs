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

public partial class ext_order_edit : System.Web.UI.Page
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

        this.btnToFormalOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定轉存為訂單？')){{return false;}}");

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

            #region 修改
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                if (this.OrigInfo.Info.IsCancel)
                {
                }
                else if (this.OrigInfo.Info.IsReadjust)
                {
                    QueryStringParsing query = new QueryStringParsing();
                    query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                    query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                    Response.Redirect(query.ToString());
                    return false;
                }
                else
                {
                    //1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程
                    switch (this.OrigInfo.Info.Status)
                    {
                        case 1:
                            break;
                        default:
                            QueryStringParsing query = new QueryStringParsing();
                            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                            query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                            Response.Redirect(query.ToString());
                            return false;
                    }
                }

                this.lnkSelCuster.Visible = this.OrigInfo.Info.CustomerId == null;

                //操作控制
                this.btnSaveOnly.Visible = this.OrigInfo.Info.Status == 1;
                //外銷組權限
                this.btnToFormalOrder.Visible = this.OrigInfo.Info.Status == 1 && this.MainPage.ActorInfoSet.CheckExtAuditPerms(1);
            }
            else
            {
                Response.Redirect("index.aspx");
                return false;
            }
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

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //還原值
        this.lblTotalAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidTotalAmt.Value, DefVal.Float), string.Empty);
        //重置幣別
        WebUtilBox.RegisterScript(this, "resetCurrencyCode();");

        //權限操作檢查
        //this.btnCreateGoods.Visible = this.btnCreateGoods.Visible ? this.FunctionRight.Maintain : this.btnCreateGoods.Visible;
        //this.btnToDraft.Visible = this.btnToDraft.Visible ? this.FunctionRight.Maintain : this.btnToDraft.Visible;
        //this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, true);
            if (this.OrigInfo != null)
            {
                this.litCName.Text = this.OrigInfo.Info.CName;

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.litCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");

                this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtOrderHelper.GetExtOrderStatusName(this.OrigInfo.Info.Status, this.OrigInfo.Info.IsReadjust, this.OrigInfo.Info.IsCancel);
                this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);

                //「價目表 ID」只是為了給品項取牌價用
                this.hidPriceListId.Value = ConvertLib.ToStr(this.OrigInfo.Info.PriceListId, string.Empty);

                //幣別
                this.lblCurrencyCode.Text = this.OrigInfo.Info.CurrencyCode;

                #region 客戶資訊
                this.hidCustomerName.Value = this.OrigInfo.Info.CustomerName;

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

                    //已存在的品項暫存
                    var existedGoodsItems = new List<string>();

                    ////一般品項暫存的是「料號」
                    //existedGoodsItems.AddRange(this.OrigInfo.DetInfos.Select(q => q.PartNo).ToArray());
                    //[by fan] 加入型號「型號[#]料號」
                    existedGoodsItems.AddRange(this.OrigInfo.DetInfos.Select(q => string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(q.Model), q.PartNo)).ToArray());

                    this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
                    #endregion
                }
                #endregion

                #region 訂單金額資訊
                this.hidTotalAmt.Value = ConvertLib.ToStr(this.OrigInfo.Info.TotalAmt, string.Empty);
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

                this.hidCustomerId.Value = info.CustomerId.ToString();
                this.litCustomerNumber.Text = info.CustomerNumber;
                this.lblCustomerName.Text = info.CustomerName;
                this.hidCustomerName.Value = info.CustomerName;
                this.lblCustomerConName.Text = info.ConName;
                this.lblCustomerTel.Text = ComUtil.ToDispTel(info.AreaCode, info.Phone);
                this.lblCustomerFax.Text = ComUtil.ToDispTel(info.AreaCode, info.Fax);
                this.lblCustomerAddr.Text = info.ShipAddress1;

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

    #region 繫結輸入
    ExtOrderHelper.InputInfoSet BindingInputted()
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        Returner returner = null;
        try
        {
            this.InputInfo = new ExtOrderHelper.InputInfoSet();

            this.InputInfo.Info.ExtQuotnSId = this.OrigInfo.Info.ExtQuotnSId;

            //不產生新版本
            //this.InputInfo.Info.SId = new SystemId();
            this.InputInfo.Info.SId = this.OrigInfo.Info.SId;

            this.InputInfo.Info.QuotnDate = this.OrigInfo.Info.QuotnDate;
            this.InputInfo.Info.Cdd = this.OrigInfo.Info.Cdd;
            this.InputInfo.Info.Edd = this.OrigInfo.Info.Edd;
            this.InputInfo.Info.Rmk = this.OrigInfo.Info.Rmk;

            #region 客戶資訊
            //先賦值一次幣別, 以免在手動建立客戶, 但尚未對應 ERP 客戶時, 仍跳出未選擇幣別的訊息.
            this.InputInfo.Info.CurrencyCode = this.lblCurrencyCode.Text;

            if (this.OrigInfo.Info.CusterSrc == 1)
            {
                #region ERP 客戶
                this.InputInfo.Info.CusterSrc = this.OrigInfo.Info.CusterSrc;

                this.InputInfo.Info.CustomerId = this.OrigInfo.Info.CustomerId;
                this.InputInfo.Info.CustomerNumber = this.OrigInfo.Info.CustomerNumber;
                this.InputInfo.Info.CustomerName = this.OrigInfo.Info.CustomerName;
                this.InputInfo.Info.CustomerConId = this.OrigInfo.Info.CustomerConId;
                this.InputInfo.Info.CustomerConName = this.OrigInfo.Info.CustomerConName;
                this.InputInfo.Info.CustomerAreaCode = this.OrigInfo.Info.CustomerAreaCode;
                this.InputInfo.Info.CustomerTel = this.OrigInfo.Info.CustomerTel;
                this.InputInfo.Info.CustomerFax = this.OrigInfo.Info.CustomerFax;
                this.InputInfo.Info.CustomerAddrId = this.OrigInfo.Info.CustomerAddrId;
                this.InputInfo.Info.CustomerAddr = this.OrigInfo.Info.CustomerAddr;
                this.InputInfo.Info.PriceListId = this.OrigInfo.Info.PriceListId;
                this.InputInfo.Info.CurrencyCode = this.OrigInfo.Info.CurrencyCode;
                this.InputInfo.Info.ShipToSiteUseId = this.OrigInfo.Info.ShipToSiteUseId;
                this.InputInfo.Info.InvoiceToSiteUseId = this.OrigInfo.Info.InvoiceToSiteUseId;
                this.InputInfo.Info.OrderTypeId = this.OrigInfo.Info.OrderTypeId;
                this.InputInfo.Info.SalesRepId = this.OrigInfo.Info.SalesRepId;
                this.InputInfo.Info.SalesName = this.OrigInfo.Info.SalesName;
                #endregion
            }
            else
            {
                #region 手動建立
                this.InputInfo.Info.CustomerId = ConvertLib.ToLong(this.hidCustomerId.Value, DefVal.Long);
                if (this.InputInfo.Info.CustomerId.HasValue)
                {
                    //取得客戶資訊. 理論上一定取得到, 若取不到, 就直接出錯唄.
                    ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
                    returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, ConvertLib.ToLongs(this.InputInfo.Info.CustomerId.Value), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                        //將手動建立的客戶來源改為 ERP 客戶
                        this.InputInfo.Info.CusterSrc = 1;

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
            this.InputInfo.Info.TotalAmt = ConvertLib.ToSingle(this.hidTotalAmt.Value, DefVal.Float);
            #endregion

            #region 訂單品項
            if (true) //只是為了不想重複宣告區域變數
            {
                #region 一般品項
                #region 原本想說因為本頁沒有修改操作, 故而最原本的資訊即可, 想想避免以後可能會調整, 還是取使用者介面的好了. (已註解)
                //var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();

                //var editInfo = new ExtOrderHelper.GoodsEditInfo()
                //{
                //    Title = this.ucGeneralGoods.Title
                //};
                //this.InputInfo.GoodsEditInfos.Add(editInfo);

                //for (int i = 0; i < generalInfos.Length; i++)
                //{
                //    var detInfo = generalInfos[i];

                //    this.InputInfo.DetInfos.Add(new ExtOrderDet.InputInfo()
                //    {
                //        //不產生新版本
                //        //SId = new SystemId(),
                //        SId = detInfo.SId,

                //        ExtOrderSId = this.InputInfo.Info.SId,
                //        Source = detInfo.Source,
                //        Model = detInfo.Model,
                //        PartNo = detInfo.PartNo,
                //        Qty = detInfo.Qty,
                //        ListPrice = detInfo.ListPrice,
                //        UnitPrice = detInfo.UnitPrice,
                //        //先轉為百分比, 因為儲存時還會再轉回小數.
                //        Discount = ConvertLib.ToSingle(detInfo.Discount, 1) * 100,
                //        PaidAmt = detInfo.PaidAmt,
                //        Rmk = detInfo.Rmk,
                //        Sort = detInfo.Sort
                //    });
                //}
                #endregion

                var editInfo = this.ucGeneralGoods.GetInfo();
                this.InputInfo.GoodsEditInfos.Add(editInfo);

                foreach (var item in editInfo.Items)
                {
                    var detInfo = new ExtOrderDet.InputInfo()
                    {
                        //不產生新版本
                        //SId = new SystemId(),
                        SId = item.SId,

                        ExtOrderSId = this.InputInfo.Info.SId,
                        Source = item.Source,
                        Model = item.Model,
                        PartNo = item.PartNo,
                        Qty = item.Qty,
                        ListPrice = item.ListPrice,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        PaidAmt = item.PaidAmt,
                        Rmk = item.Rmk,
                        Sort = item.SeqNo
                    };

                    this.InputInfo.DetInfos.Add(detInfo);
                }
                #endregion
            }

            if (true) //只是為了不想重複宣告區域變數
            {
                #region 手動新增
                var editInfo = this.ucManualGoods.GetInfo();
                this.InputInfo.GoodsEditInfos.Add(editInfo);

                foreach (var item in editInfo.Items)
                {
                    var detInfo = new ExtOrderDet.InputInfo()
                    {
                        //不產生新版本
                        //SId = new SystemId(),
                        SId = item.SId,

                        ExtOrderSId = this.InputInfo.Info.SId,
                        //先維持各自的來源, 在轉為正式訂單後, 手動新增品項要不要轉為一般品項由儲存時決定.
                        Source = item.Source,
                        Model = item.Model,
                        PartNo = item.PartNo,
                        Qty = item.Qty,
                        ListPrice = item.ListPrice,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        PaidAmt = item.PaidAmt,
                        Rmk = item.Rmk,
                        Sort = item.SeqNo
                    };

                    this.InputInfo.DetInfos.Add(detInfo);
                }
                #endregion
            }

            #region 檢查都 pass 後再重新計算一次 (不相信 JavaScript 的浮點運算)
            //總金額(未稅) = 所有小計總和
            float totalAmt = 0f;

            #region 巡覽所有品項區塊
            for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
            {
                var detInfo = this.InputInfo.DetInfos[i];

                /********************************************************************************
                 * 折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.
                ********************************************************************************/

                //品項折扣及小計
                if (detInfo.ListPrice.HasValue)
                {
                    detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
                }
                detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);

                //品項小計
                var subtotal = detInfo.PaidAmt;

                //所有小計總和
                totalAmt += (float)subtotal;
            }
            #endregion

            //總金額(未稅)
            this.InputInfo.Info.TotalAmt = (float)MathLib.Round(totalAmt, 2);
            #endregion
            #endregion

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
        /// 僅儲存操作（狀態不變）。
        /// </summary>
        SaveOnly,
        /// <summary>
        /// 確認訂單。
        /// </summary>
        FixedOrder
    }

    bool CheckInputted(CheckMode checkMode)
    {
        this.BindingInputted();

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
        else
        {
            if (this.InputInfo.Info.QuotnDate == null)
            {
                errMsgs.Add("請輸入「訂單日期」(或格式不正確)");
            }

            if (this.InputInfo.Info.Edd == null)
            {
                errMsgs.Add("請輸入「預計交貨日」(或格式不正確)");
            }

            if (this.InputInfo.Info.Cdd == null)
            {
                errMsgs.Add("請輸入「客戶需求日」(或格式不正確)");
            }

            if (this.InputInfo.Info.CustomerId == null)
            {
                errMsgs.Add("請選擇「客戶」");
            }

            if (string.IsNullOrWhiteSpace(this.InputInfo.Info.CurrencyCode))
            {
                errMsgs.Add("請選擇「幣別」");
            }

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
                var totalGoodsCnt = 0;
                for (int i = 0; i < this.InputInfo.GoodsEditInfos.Count; i++)
                {
                    totalGoodsCnt += this.InputInfo.GoodsEditInfos[i].Items.Count;
                }

                if (totalGoodsCnt == 0)
                {
                    errMsgs.Add("未包含任何品項");
                }
                else
                {
                    for (int i = 0; i < this.InputInfo.GoodsEditInfos.Count; i++)
                    {
                        var editInfo = this.InputInfo.GoodsEditInfos[i];

                        foreach (var itemInfo in editInfo.Items)
                        {
                            switch (itemInfo.Source)
                            {
                                case 1:
                                    #region 一般品項
                                    if (string.IsNullOrWhiteSpace(itemInfo.PartNo))
                                    {
                                        //理論上不會進來這邊, 保險起見.
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 未包含料號", editInfo.Title, itemInfo.SeqNo));
                                    }

                                    if (itemInfo.Qty == null || itemInfo.Qty < 1)
                                    {
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「數量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                                    }

                                    if (itemInfo.ListPrice == null)
                                    {
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 找不到品項「牌價」", editInfo.Title, itemInfo.SeqNo));
                                    }

                                    if (itemInfo.UnitPrice == null)
                                    {
                                        errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「單價」", editInfo.Title, itemInfo.SeqNo));
                                    }
                                    #endregion
                                    break;
                                case 2:
                                    #region 手動新增
                                    if (string.IsNullOrWhiteSpace(itemInfo.PartNo))
                                    {
                                        switch (checkMode)
                                        {
                                            case CheckMode.FixedOrder:
                                                errMsgs.Add(string.Format("{0} 序號 {1} - 請選擇對應的品項", editInfo.Title, itemInfo.SeqNo));
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        if (itemInfo.Qty == null || itemInfo.Qty < 1)
                                        {
                                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「數量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                                        }

                                        switch (checkMode)
                                        {
                                            case CheckMode.FixedOrder:
                                                if (itemInfo.ListPrice == null)
                                                {
                                                    errMsgs.Add(string.Format("{0} 序號 {1} - 找不到品項「牌價」", editInfo.Title, itemInfo.SeqNo));
                                                }
                                                break;
                                        }

                                        if (itemInfo.UnitPrice == null)
                                        {
                                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「單價」", editInfo.Title, itemInfo.SeqNo));
                                        }
                                    }
                                    #endregion
                                    break;
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

        #region 檢查都 pass 後再重新計算一次 (不相信 JavaScript 的浮點運算)
        //總金額(未稅) = 所有小計總和
        float totalAmt = 0f;

        #region 巡覽所有品項區塊
        for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
        {
            var detInfo = this.InputInfo.DetInfos[i];

            /********************************************************************************
             * 折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.
            ********************************************************************************/

            //品項折扣及小計
            if (detInfo.ListPrice.HasValue)
            {
                detInfo.Discount = (float)MathLib.Round(detInfo.UnitPrice.Value / detInfo.ListPrice.Value * 100, 2);
            }
            detInfo.PaidAmt = (float)MathLib.Round(detInfo.Qty.Value * detInfo.UnitPrice.Value, 2);

            //品項小計
            var subtotal = detInfo.PaidAmt;

            //所有小計總和
            totalAmt += (float)subtotal;
        }
        #endregion

        //總金額(未稅)
        this.InputInfo.Info.TotalAmt = (float)MathLib.Round(totalAmt, 2);
        #endregion

        return true;
    }
    #endregion

    #region Save
    bool Save(int status, CheckMode checkMode)
    {
        //驗證檢查
        if (!this.CheckInputted(checkMode))
        {
            return false;
        }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

                #region 建立新版本 (改為修改報價單時, 才產生新版本.)
                if (false) //不執行
                {
                    returner = entityExtOrder.Add(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.ExtQuotnSId, status, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CusterSrc.Value, this.InputInfo.Info.CustomerId, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                    if (returner.IsCompletedAndContinue)
                    {
                        #region 品項
                        for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                        {
                            var detInfo = this.InputInfo.DetInfos[i];

                            //新增
                            entityExtOrderDet.Add(actorSId, detInfo.SId, detInfo.ExtOrderSId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        #endregion

                        //異動記錄
                        string dataTitle = string.Format("{0}", this.OrigInfo.Info.OdrNo);
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }

                        transaction.Complete();
                    }
                }
                #endregion

                #region 儲存 (不產生新版本)
                if (true)
                {
                    //異動記錄
                    string dataTitle = this.OrigInfo.Info.OdrNo;
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo.Info)))
                    {
                    }

                    returner = entityExtOrder.Modify(actorSId, this.InputInfo.Info.SId, status, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CusterSrc.Value, this.InputInfo.Info.CustomerId, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                    if (returner.IsCompletedAndContinue)
                    {
                        #region 品項
                        for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                        {
                            var detInfo = this.InputInfo.DetInfos[i];

                            switch (checkMode)
                            {
                                case CheckMode.FixedOrder:
                                    //轉為訂單後, 因已對應品項, 一律轉成一般商品. (不再歸類為「手動新增」)
                                    detInfo.Source = 1;
                                    break;
                            }

                            //修改
                            entityExtOrderDet.Modify(actorSId, detInfo.SId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo.DetInfos.Where(q => q.SId.Equals(detInfo.SId)).DefaultIfEmpty(null).SingleOrDefault())))
                            {
                            }
                        }
                        #endregion

                        transaction.Complete();
                    }
                }
                #endregion
            }

            return true;
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 僅儲存操作 (狀態不變)
    protected void btnSaveOnly_Click(object sender, EventArgs e)
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
                if (!this.Save(this.OrigInfo.Info.Status, CheckMode.SaveOnly)) //只是單純儲存, 原先什麼狀態就什麼狀態.
                {
                    return;
                }

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, true, "資料已儲存");

            QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
            JSBuilder.PageRedirect(this, true, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 轉存為訂單
    protected void btnToFormalOrder_Click(object sender, EventArgs e)
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
                if (!this.Save(this.OrigInfo.Info.Status, CheckMode.FixedOrder))
                {
                    return;
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityExtOrder.UpdateStatus(actorSId, this.InputInfo.Info.SId, 2);

                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                //更改報價單狀態
                entityExtQuotn.UpdateStatus(actorSId, this.InputInfo.Info.ExtQuotnSId, 2);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, true, "資料已儲存");

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
            query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
            JSBuilder.PageRedirect(this, true, query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion
}