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

public partial class ext_quotn_edit : System.Web.UI.Page
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
    ExtQuotnHelper.InfoSet OrigInfo { get; set; }
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    ExtQuotnHelper.InputInfoSet InputInfo { get; set; }
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

        this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["sid"]);

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
        this.btnRemoveItems.OnClientClick = string.Format("javascript:if(!window.confirm('確定批次刪除指定的品項？')){{return false;}}");
        this.btnDelete.OnClientClick = string.Format("javascript:if(!window.confirm('確定刪除報價單？')){{return false;}}");
        this.btnToOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定提交報價單？')){{return false;}}");

        Returner returner = null;
        try
        {
            PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);

            if (!this.IsPostBack)
            {
                #region 幣別
                var currencyBookInfos = ErpHelper.GetCurrencyBookInfo(DefVal.Long, DefVal.Str);
                foreach (var currencyBookInfo in currencyBookInfos)
                {
                    this.lstCurrencyBook.Items.Add(new ListItem(currencyBookInfo.CurrencyCode, currencyBookInfo.PriceListId.ToString()));
                }
                #endregion

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

                this.btnCopyOrder.Visible = this._extQuotnSId != null;
            }

            if (this._extQuotnSId != null)
            {
                #region 修改
                if (this.SetEditData(this._extQuotnSId))
                {
                    this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                    breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                    //0:草稿 1:待轉訂單 2:已轉訂單
                    switch (this.OrigInfo.Info.Status)
                    {
                        case 0:
                            break;
                        default:
                            if (this.OrigInfo.Info.IsReadjust)
                            {
                                QueryStringParsing query = new QueryStringParsing();
                                query.HttpPath = new Uri("readjust.aspx", UriKind.Relative);
                                query.Add("quotnSId", this.OrigInfo.Info.SId.Value);
                                Response.Redirect(query.ToString());
                                return false;
                            }
                            else
                            {
                                QueryStringParsing query = new QueryStringParsing();
                                query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                                query.Add("quotnSId", this.OrigInfo.Info.SId.Value);
                                Response.Redirect(query.ToString());
                                return false;
                            }
                    }

                    this.lnkSelCuster.Visible = false;
                    this.rbtCusterBySearch.Visible = false;
                    this.rbtCusterByManual.Visible = false;
                    this.phCreateOrderOper.Visible = false;

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
                this.PageTitle = "新增報價單";

                breadcrumbs.Add(string.Format("<a href='{0}'>新增報價單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));

                this.litCName.Text = this.MainPage.ActorInfoSet.Info.Name;
                this.txtQuotnDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                this.txtCdd.Text = DateTime.Today.ToString("yyyy-MM-dd");

                //手動輸入客戶腳本事件
                WebUtilBox.AddAttribute(this.txtInputCustomerName, "onfocus", string.Format("customerHelper.onCusterByManual();"));
                WebUtilBox.AddAttribute(this.txtInputCustomerName, "onkeydown", string.Format("customerHelper.onCusterByManual();"));

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
        this.btnCreateGoods.Visible = this.btnCreateGoods.Visible ? this.FunctionRight.Maintain : this.btnCreateGoods.Visible;
        this.btnToDraft.Visible = this.btnToDraft.Visible ? this.FunctionRight.Maintain : this.btnToDraft.Visible;
        this.btnToOrder.Visible = this.btnToOrder.Visible ? this.FunctionRight.Maintain : this.btnToOrder.Visible;
        this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtQuotnHelper.Binding(systemId);
            if (this.OrigInfo != null)
            {
                this.hidSpecSId.Value = this.OrigInfo.Info.SId.Value; //暫存系統代號。

                this.litCName.Text = this.OrigInfo.Info.CName;

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.txtQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.txtCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.txtEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");

                //this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtQuotnHelper.GetExtQuotnStatusName(this.OrigInfo.Info.Status, this.OrigInfo.Info.IsReadjust, this.OrigInfo.Info.IsCancel);
                this.txtRmk.Text = this.OrigInfo.Info.Rmk;

                //「價目表 ID」只是為了給品項取牌價用
                this.hidPriceListId.Value = ConvertLib.ToStr(this.OrigInfo.Info.PriceListId, string.Empty);

                //幣別
                WebUtilBox.SetListControlSelected(WebUtilBox.ListControlFindBy.Text, this.OrigInfo.Info.CurrencyCode, this.lstCurrencyBook);
                this.lstCurrencyBook.Enabled = false;

                #region 客戶資訊
                this.rbtCusterBySearch.Checked = this.OrigInfo.Info.CustomerId.HasValue;
                this.rbtCusterByManual.Checked = !this.rbtCusterBySearch.Checked;

                if (this.OrigInfo.Info.CustomerId.HasValue)
                {
                    //選擇客戶
                    this.hidCustomerId.Value = this.OrigInfo.Info.CustomerId.ToString();

                    this.txtInputCustomerName.Visible = false;

                    this.txtCustomerNumber.ReadOnly = true;
                    this.txtCustomerConName.ReadOnly = true;
                    this.txtCustomerAddr.ReadOnly = true;
                    this.txtCustomerTel.ReadOnly = true;
                    this.txtCustomerFax.ReadOnly = true;
                }
                else
                {
                    //手動輸入客戶腳本事件
                    WebUtilBox.AddAttribute(this.txtInputCustomerName, "onkeyup", string.Format("customerHelper.onInputCustomerName($(this));"));
                }

                this.txtCustomerNumber.Text = this.OrigInfo.Info.CustomerNumber;
                this.lblCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.hidCustomerName.Value = this.OrigInfo.Info.CustomerName;
                this.txtInputCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.txtCustomerConName.Text = this.OrigInfo.Info.CustomerConName;
                this.txtCustomerAddr.Text = this.OrigInfo.Info.CustomerAddr;
                this.txtCustomerTel.Text = this.OrigInfo.Info.CustomerTel;
                this.txtCustomerFax.Text = this.OrigInfo.Info.CustomerFax;
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
                        priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Where(q => q.Source == 1).Select(q => q.PartNo).ToArray());
                    }

                    #region 一般品項
                    if (true)
                    {
                        var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();
                        var editInfo = new ExtQuotnHelper.GoodsEditInfo();

                        for (int i = 0; i < generalInfos.Length; i++)
                        {
                            var detInfo = generalInfos[i];

                            var itemEditInfo = new ExtQuotnHelper.GoodsItemEditInfo()
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
                            itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                            editInfo.Items.Add(itemEditInfo);
                        }

                        this.ucGeneralGoods.SetInfo(editInfo);
                    }
                    #endregion

                    #region 手動新增
                    if (true)
                    {
                        var manualInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).ToArray();
                        var editInfo = new ExtQuotnHelper.GoodsEditInfo();

                        for (int i = 0; i < manualInfos.Length; i++)
                        {
                            var detInfo = manualInfos[i];

                            var itemEditInfo = new ExtQuotnHelper.GoodsItemEditInfo()
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
                            //沒有牌價
                            //itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

                            editInfo.Items.Add(itemEditInfo);
                        }

                        this.ucManualGoods.SetInfo(editInfo);
                    }
                    #endregion
                    #endregion
                }
                #endregion

                #region 報價單金額資訊
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

                WebUtilBox.SetListControlSelected(WebUtilBox.ListControlFindBy.Text, info.CurrencyCode, this.lstCurrencyBook);
                //this.lstCurrencyBook.Enabled = false;

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

    #region 開始建立報價單品項
    protected void btnCreateGoods_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        //驗證檢查
        if (!this.CheckInputted(CheckMode.InitCreate))
        {
            return;
        }

        this.txtInputCustomerName.Visible = false;
        if (this.rbtCusterBySearch.Checked)
        {
            //選擇客戶
            this.txtCustomerNumber.Text = this.InputInfo.Info.CustomerNumber;
            this.lblCustomerName.Text = this.InputInfo.Info.CustomerName;
            this.txtCustomerConName.Text = this.InputInfo.Info.CustomerConName;
            this.txtCustomerAddr.Text = this.InputInfo.Info.CustomerAddr;
            this.txtCustomerTel.Text = this.InputInfo.Info.CustomerTel;
            this.txtCustomerFax.Text = this.InputInfo.Info.CustomerFax;

            this.txtCustomerNumber.ReadOnly = true;
            this.txtCustomerConName.ReadOnly = true;
            this.txtCustomerAddr.ReadOnly = true;
            this.txtCustomerTel.ReadOnly = true;
            this.txtCustomerFax.ReadOnly = true;

            WebUtilBox.SetListControlSelected(this.hidPriceListId.Value, this.lstCurrencyBook);
        }
        else
        {
            //手動輸入客戶
            this.hidCustomerId.Value = string.Empty;
            this.hidCustomerName.Value = this.InputInfo.Info.CustomerName;
            this.lblCustomerName.Text = this.InputInfo.Info.CustomerName;

            this.hidPriceListId.Value = this.lstCurrencyBook.SelectedValue;
        }

        this.lstCurrencyBook.Enabled = false;
        this.lnkSelCuster.Visible = false;
        this.rbtCusterBySearch.Visible = false;
        this.rbtCusterByManual.Visible = false;

        this.phOrderDetail.Visible = true;
        this.phCreateOrderOper.Visible = false;
        this.phOrderGoods.Visible = true;
        this.btnToDraft.Visible = this.FunctionRight.Maintain;
    }
    #endregion

    #region 繫結輸入
    ExtQuotnHelper.InputInfoSet BindingInputted(bool bindingAll, bool fullCustInfo)
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        Returner returner = null;
        try
        {
            this.InputInfo = new ExtQuotnHelper.InputInfoSet();

            this.InputInfo.Info.SId = new SystemId(this.hidSpecSId.Value);
            this.InputInfo.Info.QuotnDate = ConvertLib.ToDateTime(this.txtQuotnDate.Text, DefVal.DT);
            this.InputInfo.Info.Cdd = ConvertLib.ToDateTime(this.txtCdd.Text, DefVal.DT);
            this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
            this.InputInfo.Info.CustomerId = ConvertLib.ToLong(this.hidCustomerId.Value, DefVal.Long);
            this.InputInfo.Info.Rmk = this.txtRmk.Text;

            #region 客戶資訊
            if (this.rbtCusterBySearch.Checked)
            {
                #region ERP 客戶
                this.InputInfo.Info.CusterSrc = 1;

                if (fullCustInfo)
                {
                    #region 繫結完整客戶資訊 (新增或複製時)
                    this.InputInfo.Info.CurrencyCode = this.lstCurrencyBook.SelectedItem.Text;

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
                    #region 繫結部份客戶資訊 (修改時)
                    this.InputInfo.Info.CustomerNumber = this.txtCustomerNumber.Text;
                    this.InputInfo.Info.CustomerName = this.hidCustomerName.Value;
                    this.InputInfo.Info.CustomerConName = this.txtCustomerConName.Text;
                    this.InputInfo.Info.CustomerTel = this.txtCustomerTel.Text;
                    this.InputInfo.Info.CustomerFax = this.txtCustomerFax.Text;
                    this.InputInfo.Info.CustomerAddr = this.txtCustomerAddr.Text;

                    this.InputInfo.Info.CurrencyCode = this.lstCurrencyBook.SelectedItem.Text;
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 手動建立
                this.InputInfo.Info.CusterSrc = 2;

                this.InputInfo.Info.CustomerNumber = this.txtCustomerNumber.Text;
                this.InputInfo.Info.CustomerName = this.txtInputCustomerName.Text;
                //this.InputInfo.Info.CustomerConId = info.ContactId;
                this.InputInfo.Info.CustomerConName = this.txtCustomerConName.Text;
                //this.InputInfo.Info.CustomerAreaCode = info.AreaCode;
                this.InputInfo.Info.CustomerTel = this.txtCustomerTel.Text;
                this.InputInfo.Info.CustomerFax = this.txtCustomerFax.Text;
                //this.InputInfo.Info.CustomerAddrId = info.ShipAddressId;
                this.InputInfo.Info.CustomerAddr = this.txtCustomerAddr.Text;
                this.InputInfo.Info.PriceListId = ConvertLib.ToLong(this.lstCurrencyBook.SelectedValue, DefVal.Long);
                if (!string.IsNullOrWhiteSpace(this.lstCurrencyBook.SelectedValue))
                {
                    this.InputInfo.Info.CurrencyCode = this.lstCurrencyBook.SelectedItem.Text;
                }
                //this.InputInfo.Info.ShipToSiteUseId = info.ShipToSiteUseId;
                //this.InputInfo.Info.InvoiceToSiteUseId = info.InvoiceToSiteUseId;
                //this.InputInfo.Info.OrderTypeId = info.OrderTypeId;
                //this.InputInfo.Info.SalesRepId = info.SalesRepId;
                //this.InputInfo.Info.SalesName = info.SalesName;
                #endregion
            }
            #endregion

            if (bindingAll)
            {
                #region 收貨人資訊
                this.InputInfo.Info.RcptName = this.txtRcptName.Text;
                this.InputInfo.Info.RcptCusterName = this.txtRcptCusterName.Text;
                this.InputInfo.Info.RcptTel = this.txtRcptTel.Text;
                this.InputInfo.Info.RcptFax = this.txtRcptFax.Text;
                this.InputInfo.Info.RcptAddr = this.txtRcptAddr.Text;
                this.InputInfo.Info.FreightWaySId = ConvertLib.ToSId(this.lstFreightWayList.SelectedValue);
                #endregion

                #region 報價單金額
                this.InputInfo.Info.TotalAmt = ConvertLib.ToSingle(this.hidTotalAmt.Value, DefVal.Float);
                #endregion

                #region 報價單品項
                if (true) //只是為了不想重複宣告區域變數
                {
                    #region 一般品項
                    var editInfo = this.ucGeneralGoods.GetInfo();
                    this.InputInfo.GoodsEditInfos.Add(editInfo);

                    foreach (var item in editInfo.Items)
                    {
                        var detInfo = new ExtQuotnDet.InputInfo()
                        {
                            SId = item.SId,
                            ExtQuotnSId = this.InputInfo.Info.SId,
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
                        var detInfo = new ExtQuotnDet.InputInfo()
                        {
                            SId = item.SId,
                            ExtQuotnSId = this.InputInfo.Info.SId,
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
        Modify,
        /// <summary>
        /// 複製。
        /// </summary>
        Copy
    }

    bool CheckInputted(CheckMode checkMode)
    {
        this.BindingInputted(checkMode != CheckMode.InitCreate, this._extQuotnSId == null || checkMode == CheckMode.Copy);

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        if (this.InputInfo.Info.QuotnDate == null)
        {
            errMsgs.Add("請輸入「報價單日期」(或格式不正確)");
        }

        if (this.InputInfo.Info.Edd == null)
        {
            errMsgs.Add("請輸入「預計交貨日」(或格式不正確)");
        }

        if (this.InputInfo.Info.Cdd == null)
        {
            errMsgs.Add("請輸入「客戶需求日」(或格式不正確)");
        }

        if (this.InputInfo.Info.CusterSrc == 1)
        {
            if (this.InputInfo.Info.CustomerId == null)
            {
                errMsgs.Add("請選擇「客戶」");
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(this.InputInfo.Info.CustomerName))
            {
                errMsgs.Add("請輸入「客戶名稱」");
            }
        }

        if (string.IsNullOrWhiteSpace(this.InputInfo.Info.CurrencyCode))
        {
            errMsgs.Add("請選擇「幣別」");
        }

        switch (checkMode)
        {
            case CheckMode.Create:
            case CheckMode.Modify:
            case CheckMode.Copy:
                #region 客戶資訊
                if (this.InputInfo.Info.CusterSrc == 2)
                {
                    if (string.IsNullOrWhiteSpace(this.InputInfo.Info.CustomerNumber))
                    {
                        errMsgs.Add("請輸入「客戶資訊-客戶編號」");
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

                #region 報價單金額
                //只要有品項, 且品項都有輸入, 則報價單金額為自動產生.
                #endregion

                if (this.InputInfo.GoodsEditInfos.Count == 0)
                {
                    //理論上不會進來這邊, 保險起見.
                    errMsgs.Add("報價單品項初始失敗");
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
                                        if (string.IsNullOrWhiteSpace(itemInfo.Model))
                                        {
                                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「型號」", editInfo.Title, itemInfo.SeqNo));
                                        }

                                        if (itemInfo.Qty == null || itemInfo.Qty < 1)
                                        {
                                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「數量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                                        }

                                        if (itemInfo.UnitPrice == null)
                                        {
                                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「單價」", editInfo.Title, itemInfo.SeqNo));
                                        }
                                        #endregion
                                        break;
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
            case CheckMode.Copy:
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
                break;
        }

        return true;
    }
    #endregion

    #region 移除品項
    protected void btnRemoveItems_Click(object sender, EventArgs e)
    {
        this.ucGeneralGoods.RemoveSeledItems();
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
                //儲存報價單
                if (!this.Save(this._extQuotnSId == null ? CheckMode.Create : CheckMode.Modify))
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

    #region 複製
    protected void btnCopyOrder_Click(object sender, EventArgs e)
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //驗證檢查
                if (!this.CheckInputted(CheckMode.Copy))
                {
                    return;
                }

                //重新產生報價單系統代號
                this.InputInfo.Info.SId = new SystemId();
                //重新產生品項系統代號
                foreach (var detInfo in this.InputInfo.DetInfos)
                {
                    detInfo.SId = new SystemId();
                    detInfo.ExtQuotnSId = this.InputInfo.Info.SId;
                }

                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);

                var orderNo = entityExtQuotn.GetNewOrderNo();

                returner = entityExtQuotn.Add(actorSId, this.InputInfo.Info.SId, orderNo, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CusterSrc.Value, this.InputInfo.Info.CustomerId, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        //新增
                        entityExtQuotnDet.Add(actorSId, detInfo.SId, detInfo.ExtQuotnSId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                        //異動記錄
                        //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    //繫結一次
                    this.OrigInfo = ExtQuotnHelper.Binding(this.InputInfo.Info.SId);

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

    #region 刪除報價單
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
                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, this.OrigInfo.Info.SId, DefVal.Int, "MD", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                entityExtQuotn.SwitchMarkDeleted(actorSId, ConvertLib.ToSIds(this.OrigInfo.Info.SId), true);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "報價單已刪除");
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

                if (this._extQuotnSId == null)
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
                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);

                var orderNo = entityExtQuotn.GetNewOrderNo();

                returner = entityExtQuotn.Add(actorSId, this.InputInfo.Info.SId, orderNo, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CusterSrc.Value, this.InputInfo.Info.CustomerId, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        //新增
                        entityExtQuotnDet.Add(actorSId, detInfo.SId, detInfo.ExtQuotnSId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                        //異動記錄
                        //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                        {
                        }
                    }
                    #endregion

                    //異動記錄
                    string dataTitle = string.Format("{0}", orderNo);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    //繫結一次
                    this.OrigInfo = ExtQuotnHelper.Binding(this.InputInfo.Info.SId);

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
                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                returner = entityExtQuotn.Modify(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        if (this.OrigInfo.DetInfos.Where(q => q.SId.Equals(detInfo.SId)).Count() == 0)
                        {
                            //新增
                            entityExtQuotnDet.Add(actorSId, detInfo.SId, detInfo.ExtQuotnSId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        else
                        {
                            //修改
                            returner = entityExtQuotnDet.Modify(actorSId, detInfo.SId, detInfo.Model, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
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
                            returner = entityExtQuotnDet.Delete(actorSId, ConvertLib.ToSIds(detInfo.SId));

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
                            {
                            }
                        }
                    }
                    #endregion

                    //繫結一次
                    this.OrigInfo = ExtQuotnHelper.Binding(this.InputInfo.Info.SId);

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

    #region 提交
    protected void btnToOrder_Click(object sender, EventArgs e)
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
                if (!this.Save(CheckMode.Modify))
                {
                    return;
                }

                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                //更改報價單狀態
                entityExtQuotn.UpdateStatus(actorSId, this.InputInfo.Info.SId, 1);

                #region 轉成訂單
                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

                ISystemId extOrderSId = new SystemId();

                entityExtOrder.Add(actorSId, extOrderSId, this.OrigInfo.Info.SId, 1, this.OrigInfo.Info.QuotnDate, this.OrigInfo.Info.Cdd, this.OrigInfo.Info.Edd, this.OrigInfo.Info.CusterSrc, this.OrigInfo.Info.CustomerId, this.OrigInfo.Info.CustomerNumber, this.OrigInfo.Info.CustomerName, this.OrigInfo.Info.CustomerConId, this.OrigInfo.Info.CustomerConName, this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerTel, this.OrigInfo.Info.CustomerFax, this.OrigInfo.Info.CustomerAddrId, this.OrigInfo.Info.CustomerAddr, this.OrigInfo.Info.PriceListId, this.OrigInfo.Info.CurrencyCode, this.OrigInfo.Info.ShipToSiteUseId, this.OrigInfo.Info.InvoiceToSiteUseId, this.OrigInfo.Info.OrderTypeId, this.OrigInfo.Info.SalesRepId, this.OrigInfo.Info.SalesName, this.OrigInfo.Info.RcptName, this.OrigInfo.Info.RcptCusterName, this.OrigInfo.Info.RcptTel, this.OrigInfo.Info.RcptFax, this.OrigInfo.Info.RcptAddr, this.OrigInfo.Info.FreightWaySId, this.OrigInfo.Info.TotalAmt, this.OrigInfo.Info.Rmk);

                foreach (var detInfo in this.OrigInfo.DetInfos)
                {
                    ISystemId extOrderDetSId = new SystemId();

                    entityExtOrderDet.Add(actorSId, extOrderDetSId, extOrderSId, detInfo.Source, detInfo.Model, detInfo.PartNo, detInfo.Qty, detInfo.ListPrice, detInfo.UnitPrice, detInfo.Discount, detInfo.PaidAmt, detInfo.Rmk, detInfo.Sort);
                }
                #endregion

                transaction.Complete();
            }

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("view.aspx", UriKind.Relative);
            query.Add("quotnSId", this.OrigInfo.Info.SId.Value);
            Response.Redirect(query.ToString());
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    protected void ucGeneralGoods_AddComplete(object sender, CustEventArgs e)
    {
        if (e.Tag == null) { return; }

        var models = (string[])e.Tag;
        var editInfos = new List<ExtQuotnHelper.GoodsItemEditInfo>();
        foreach (var model in models)
        {
            editInfos.Add(new ExtQuotnHelper.GoodsItemEditInfo() { Model = model });
        }

        this.ucManualGoods.Add(editInfos.ToArray());
    }
}