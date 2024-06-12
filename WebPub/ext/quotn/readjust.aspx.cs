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

public partial class ext_quotn_readjust : System.Web.UI.Page
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
        this.PageTitle = "外銷報價單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>外銷報價單</a>"));

        this.btnRemoveItems.OnClientClick = string.Format("javascript:if(!window.confirm('確定批次刪除指定的品項？')){{return false;}}");
        this.btnToOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定提交報價單？')){{return false;}}");
        this.btnCancel.OnClientClick = string.Format("javascript:if(!window.confirm('確定要取消？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                #region 貨運方式
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
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
            if (this.SetEditData(this._extQuotnSId))
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.PageTitle));

                //只有報價調整中才會進來這裡
                if (!this.OrigInfo.Info.IsReadjust)
                {
                    Response.Redirect("index.aspx");
                    return false;
                }

                //具備訂單修改權限便可修改, 不鎖定流程審核權限.
                this.phRcptEdit.Visible = this.FunctionRight.Maintain;
                this.phRcptView.Visible = !this.phRcptEdit.Visible;
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
        this.btnToDraft.Visible = this.btnToDraft.Visible ? this.FunctionRight.Maintain : this.btnToDraft.Visible;
        this.btnToOrder.Visible = this.btnToOrder.Visible ? this.FunctionRight.Maintain : this.btnToOrder.Visible;
        this.btnCancel.Visible = this.btnCancel.Visible ? this.FunctionRight.Maintain : this.btnCancel.Visible;
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, true, false);
            if (this.OrigInfo != null)
            {
                this.litCName.Text = this.OrigInfo.Info.CName;

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.txtQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.txtCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.txtEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");

                //this.litSalesName.Text = this.OrigInfo.Info.SalesName;
                this.litStatus.Text = ExtQuotnHelper.GetExtQuotnStatusName(this.OrigInfo.Info.ExtQuotnStatus, this.OrigInfo.Info.IsReadjust, this.OrigInfo.Info.IsCancel);
                this.txtRmk.Text = this.OrigInfo.Info.Rmk;

                //「價目表 ID」只是為了給品項取牌價用
                this.hidPriceListId.Value = ConvertLib.ToStr(this.OrigInfo.Info.PriceListId, string.Empty);

                //幣別
                this.lblCurrencyCode.Text = this.OrigInfo.Info.CurrencyCode;

                #region 客戶資訊
                this.hidCustomerId.Value = this.OrigInfo.Info.CustomerId.ToString();

                this.litCustomerNumber.Text = this.OrigInfo.Info.CustomerNumber;
                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
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
                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

                    //先取得所有品項的價目表
                    var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                    if (this.OrigInfo.Info.PriceListId.HasValue)
                    {
                        //priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Where(q => q.Source == 1).Select(q => q.PartNo).ToArray());
                        priceBookInfos = ErpHelper.GetPriceBookInfo(this.OrigInfo.Info.PriceListId.Value, this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray());
                    }

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
                            itemEditInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, detInfo.PartNo);

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

    #region 繫結輸入
    ExtOrderHelper.InputInfoSet BindingInputted()
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        Returner returner = null;
        try
        {
            this.InputInfo = new ExtOrderHelper.InputInfoSet();

            this.InputInfo.Info.SId = this.OrigInfo.Info.SId;
            this.InputInfo.Info.Status = this.OrigInfo.Info.Status;
            this.InputInfo.Info.QuotnDate = ConvertLib.ToDateTime(this.txtQuotnDate.Text, DefVal.DT);
            this.InputInfo.Info.Cdd = ConvertLib.ToDateTime(this.txtCdd.Text, DefVal.DT);
            this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
            this.InputInfo.Info.Rmk = this.txtRmk.Text;

            #region 客戶資訊
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

            #region 收貨人資訊
            if (this.phRcptEdit.Visible)
            {
                this.InputInfo.Info.RcptName = this.txtRcptName.Text;
                this.InputInfo.Info.RcptCusterName = this.txtRcptCusterName.Text;
                this.InputInfo.Info.RcptTel = this.txtRcptTel.Text;
                this.InputInfo.Info.RcptFax = this.txtRcptFax.Text;
                this.InputInfo.Info.RcptAddr = this.txtRcptAddr.Text;
                this.InputInfo.Info.FreightWaySId = ConvertLib.ToSId(this.lstFreightWayList.SelectedValue);
            }
            else
            {
                this.InputInfo.Info.RcptName = this.OrigInfo.Info.RcptName;
                this.InputInfo.Info.RcptCusterName = this.OrigInfo.Info.RcptCusterName;
                this.InputInfo.Info.RcptTel = this.OrigInfo.Info.RcptTel;
                this.InputInfo.Info.RcptFax = this.OrigInfo.Info.RcptFax;
                this.InputInfo.Info.RcptAddr = this.OrigInfo.Info.RcptAddr;
                this.InputInfo.Info.FreightWaySId = this.OrigInfo.Info.FreightWaySId;
            }
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
                    var detInfo = new ExtOrderDet.InputInfo()
                    {
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
    bool CheckInputted()
    {
        this.BindingInputted();

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
                if (!this.Save())
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

    #region Save
    bool Save()
    {
        //驗證檢查
        if (!this.CheckInputted())
        {
            return false;
        }

        return this.Modify();
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
                //同步更新報價單
                if (!this.SyncUpdateExtQuotn())
                {
                    return false;
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                returner = entityExtOrder.Modify(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.Status, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CusterSrc.Value, this.InputInfo.Info.CustomerId.Value, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConId, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerAreaCode, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddrId, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.PriceListId, this.InputInfo.Info.CurrencyCode, this.InputInfo.Info.ShipToSiteUseId, this.InputInfo.Info.InvoiceToSiteUseId, this.InputInfo.Info.OrderTypeId, this.InputInfo.Info.SalesRepId, this.InputInfo.Info.SalesName, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        if (this.OrigInfo.DetInfos.Where(q => q.SId.Equals(detInfo.SId)).Count() == 0)
                        {
                            //新增
                            entityExtOrderDet.Add(actorSId, detInfo.SId, detInfo.ExtOrderSId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                            {
                            }
                        }
                        else
                        {
                            //修改
                            returner = entityExtOrderDet.Modify(actorSId, detInfo.SId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
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
                            returner = entityExtOrderDet.Delete(actorSId, ConvertLib.ToSIds(detInfo.SId));

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_ORDER_DET, detInfo.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
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

                    returner = entityExtQuotn.Modify(actorSId, quotnInfoSet.Info.SId, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.CustomerNumber, this.InputInfo.Info.CustomerName, this.InputInfo.Info.CustomerConName, this.InputInfo.Info.CustomerTel, this.InputInfo.Info.CustomerFax, this.InputInfo.Info.CustomerAddr, this.InputInfo.Info.RcptName, this.InputInfo.Info.RcptCusterName, this.InputInfo.Info.RcptTel, this.InputInfo.Info.RcptFax, this.InputInfo.Info.RcptAddr, this.InputInfo.Info.FreightWaySId, this.InputInfo.Info.TotalAmt, this.InputInfo.Info.Rmk);
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

                        for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                        {
                            var detInfo = this.InputInfo.DetInfos[i];

                            //新增
                            entityExtQuotnDet.Add(actorSId, detInfo.SId, quotnInfoSet.Info.SId, detInfo.Source.Value, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.ListPrice, detInfo.UnitPrice.Value, ConvertLib.ToSingle(detInfo.Discount, 100) / 100, detInfo.PaidAmt.Value, detInfo.Rmk, i + 1);

                            //異動記錄
                            //一般品項時, 資料標題寫入料號; 手動品項, 資料標題寫入型號.
                            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_QUOTN_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.Source == 1 ? detInfo.PartNo : detInfo.Model, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
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
                if (!this.Save())
                {
                    return;
                }

                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                //更改報價單狀態
                entityExtQuotn.UpdateStatus(actorSId, this.OrigInfo.Info.ExtQuotnSId, 1);
                //更改報價單調整中
                entityExtQuotn.UpdateReadjustFlag(actorSId, this.OrigInfo.Info.ExtQuotnSId, false);

                transaction.Complete();

                QueryStringParsing query = new QueryStringParsing();
                query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                query.Add("quotnSId", this.OrigInfo.Info.SId.Value);
                Response.Redirect(query.ToString());
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

    protected void ucGeneralGoods_AddComplete(object sender, CustEventArgs e)
    {
        if (e.Tag == null) { return; }

        var models = (string[])e.Tag;
        var editInfos = new List<ExtOrderHelper.GoodsItemEditInfo>();
        foreach (var model in models)
        {
            editInfos.Add(new ExtOrderHelper.GoodsItemEditInfo() { Model = model });
        }

        this.ucManualGoods.Add(editInfos.ToArray());
    }
}