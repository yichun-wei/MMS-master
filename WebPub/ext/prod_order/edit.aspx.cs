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

public partial class ext_prod_order_edit : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "EXT_PROD_ORDER";
    string AUTH_NAME = "外銷生產單";

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
    ExtProdOrderHelper.InfoSet OrigInfo { get; set; }
    /// <summary>
    /// 輸入資訊。
    /// </summary>
    ExtProdOrderHelper.InputInfoSet InputInfo { get; set; }
    #endregion

    //改成用生產單系統代號
    //ISystemId _extQuotnSId;
    ISystemId _extProdOrderSId;

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

        //改成用生產單系統代號
        //this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["quotnSId"]);
        //if (this._extQuotnSId == null)
        //{
        //    Response.Redirect("index.aspx");
        //    return false;
        //}
        this._extProdOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._extProdOrderSId == null)
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
        this.PageTitle = "外銷生產單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>外銷生產單</a>"));

        this.btnFixedProdOrder.OnClientClick = string.Format("javascript:if(!window.confirm('確定確認？')){{return false;}}");
        this.btnDelete.OnClientClick = string.Format("javascript:if(!window.confirm('確定刪除？')){{return false;}}");

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.ProdOdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", new QueryStringParsing(new Uri("../order/view.aspx", UriKind.Relative), Request.QueryString), string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName)));
                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.OrigInfo.Info.ProdOdrNo));

                #region 操作切換
                //1:未確認 2:已確認
                switch (this.OrigInfo.Info.Status)
                {
                    case 2:
                        QueryStringParsing queryProdOrder = new QueryStringParsing();
                        queryProdOrder.HttpPath = new Uri("view.aspx", UriKind.Relative);
                        //改成用生產單系統代號
                        //queryProdOrder.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
                        queryProdOrder.Add("sid", this.OrigInfo.Info.SId.Value);
                        Response.Redirect(queryProdOrder.ToString());
                        break;
                    default:
                        //外銷組權限
                        this.btnFixedProdOrder.Visible = this.MainPage.ActorInfoSet.CheckExtAuditPerms(1);
                        break;
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

        //權限操作檢查
        this.btnDelete.Visible = this.btnDelete.Visible ? this.FunctionRight.Delete : this.btnDelete.Visible;
    }

    #region SetEditData
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            //改成用生產單系統代號
            //this.OrigInfo = ExtProdOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, DefVal.Bool, DefVal.SId, DefVal.Int, true);
            this.OrigInfo = ExtProdOrderHelper.Binding(DefVal.SId, DefVal.SId, DefVal.Int, DefVal.Bool, this._extProdOrderSId, DefVal.Int, DefVal.Bool);
            if (this.OrigInfo != null)
            {
                this.litCdt.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy-MM-dd");
                this.litProdOdrNo.Text = this.OrigInfo.Info.ProdOdrNo;
                this.litIsProdFixed.Text = ExtProdOrderHelper.GetExtProdOrderStatusName(this.OrigInfo.Info.Status);

                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litQuotnDate.Text = ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy-MM-dd");
                this.litCdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy-MM-dd");
                this.txtEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;

                #region 品項資訊
                if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
                {
                    #region 初始載入
                    //先取得所有品項的在手量
                    var onHandInfos = ErpHelper.GetOnHandInfo(this.OrigInfo.DetInfos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), this.OrigInfo.Info.Whse);

                    #region 一般品項
                    if (true)
                    {
                        var generalInfos = this.OrigInfo.DetInfos.ToArray();
                        var editInfo = new ExtProdOrderHelper.GoodsEditInfo();

                        for (int i = 0; i < generalInfos.Length; i++)
                        {
                            var detInfo = generalInfos[i];

                            var itemEditInfo = new ExtProdOrderHelper.GoodsItemEditInfo()
                            {
                                SId = detInfo.SId,
                                Source = detInfo.Source,
                                SeqNo = i + 1,
                                OrgCode = detInfo.OrgCode,
                                Model = detInfo.Model,
                                PartNo = detInfo.PartNo,
                                Qty = detInfo.Qty,
                                CumProdQty = detInfo.CumProdQty,
                                ProdQty = detInfo.ProdQty,
                                EstFpmsDate = detInfo.EstFpmsDate,
                                Rmk = detInfo.Rmk
                            };

                            itemEditInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, this.OrigInfo.Info.Whse, detInfo.PartNo);

                            editInfo.Items.Add(itemEditInfo);
                        }

                        this.ucGeneralGoods.SetInfo(editInfo);
                    }
                    #endregion
                    #endregion
                }
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
    ExtProdOrderHelper.InputInfoSet BindingInputted(CheckMode checkMode)
    {
        WebUtil.TrimTextBox(this.Form.Controls, false);

        Returner returner = null;
        try
        {
            this.InputInfo = new ExtProdOrderHelper.InputInfoSet();

            this.InputInfo.Info.ExtOrderSId = this.OrigInfo.Info.ExtOrderSId;

            //this.InputInfo.Info.SId = checkMode == CheckMode.FixedProdOrder ? new SystemId() : this.OrigInfo.Info.SId;
            this.InputInfo.Info.SId = this.OrigInfo.Info.SId;
            this.InputInfo.Info.QuotnDate = this.OrigInfo.Info.QuotnDate;
            this.InputInfo.Info.Cdd = this.OrigInfo.Info.Cdd;
            this.InputInfo.Info.Edd = ConvertLib.ToDateTime(this.txtEdd.Text, DefVal.DT);
            this.InputInfo.Info.Rmk = this.OrigInfo.Info.Rmk;

            #region 生產單品項
            var editInfo = this.ucGeneralGoods.GetInfo();
            this.InputInfo.GoodsEditInfos.Add(editInfo);

            foreach (var item in editInfo.Items)
            {
                var detInfo = new ExtProdOrderDet.InputInfo()
                {
                    //SId = checkMode == CheckMode.FixedProdOrder ? new SystemId() : item.SId,
                    SId = item.SId,
                    ExtProdOrderSId = this.InputInfo.Info.SId,
                    Source = item.Source,
                    OrgCode = item.OrgCode,
                    Model = item.Model,
                    PartNo = item.PartNo,
                    Qty = item.Qty,
                    CumProdQty = item.CumProdQty,
                    ProdQty = item.ProdQty,
                    EstFpmsDate = item.EstFpmsDate,
                    Rmk = item.Rmk,
                    Sort = item.SeqNo
                };

                this.InputInfo.DetInfos.Add(detInfo);
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
    enum CheckMode
    {
        /// <summary>
        /// 僅儲存操作（狀態不變）。
        /// </summary>
        SaveOnly,
        /// <summary>
        /// 確認生產單。
        /// </summary>
        FixedProdOrder
    }

    bool CheckInputted(CheckMode checkMode)
    {
        this.BindingInputted(checkMode);

        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        if (this.InputInfo.Info.Edd == null)
        {
            errMsgs.Add("請輸入「預計交貨日」(或格式不正確)");
        }

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
                        #region 生產單品項
                        if (string.IsNullOrWhiteSpace(itemInfo.PartNo))
                        {
                            //理論上不會進來這邊, 保險起見.
                            errMsgs.Add(string.Format("{0} 序號 {1} - 未包含料號", editInfo.Title, itemInfo.SeqNo));
                        }

                        //值是自動計算的, 理論上是有值的, 保險起見.
                        if (itemInfo.Qty == null || itemInfo.Qty < 1)
                        {
                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「需求量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                        }

                        //值是自動計算的, 理論上是有值的, 保險起見.
                        if (itemInfo.CumProdQty == null)
                        {
                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「累計生產量」", editInfo.Title, itemInfo.SeqNo));
                        }

                        //[by fan] 改為允許為 0
                        //if (itemInfo.ProdQty == null || itemInfo.ProdQty < 1)
                        //{
                        //    errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「生產量」 (或不得為 0)", editInfo.Title, itemInfo.SeqNo));
                        //}
                        if (itemInfo.ProdQty == null)
                        {
                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「生產量」", editInfo.Title, itemInfo.SeqNo));
                        }

                        if (itemInfo.EstFpmsDate == null)
                        {
                            errMsgs.Add(string.Format("{0} 序號 {1} - 請輸入「預計繳庫日」", editInfo.Title, itemInfo.SeqNo));
                        }
                        #endregion
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
                ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                ExtProdOrderDet entityExtProdOrderDet = new ExtProdOrderDet(SystemDefine.ConnInfo);

                #region 儲存但不遞增版號
                //確認生產單也不遞增版號
                //if (checkMode == CheckMode.SaveOnly)
                //{
                //異動記錄
                string dataTitle = this.OrigInfo.Info.ProdOdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_PROD_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                returner = entityExtProdOrder.Modify(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.Edd);
                if (returner.IsCompletedAndContinue)
                {
                    //更改生產單狀態
                    entityExtProdOrder.UpdateStatus(actorSId, this.InputInfo.Info.SId, status);

                    #region 品項
                    for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                    {
                        var detInfo = this.InputInfo.DetInfos[i];

                        returner = entityExtProdOrderDet.Modify(actorSId, detInfo.SId, detInfo.Model, detInfo.Qty.Value, detInfo.CumProdQty.Value, detInfo.ProdQty.Value, detInfo.EstFpmsDate, detInfo.Rmk, i + 1);

                        //異動記錄
                        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_PROD_ORDER_DET, detInfo.SId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(detInfo)))
                        {
                        }
                    }
                    #endregion

                    transaction.Complete();
                }
                //}
                #endregion

                #region 儲存且遞增版號 (已註解)
                //if (checkMode == CheckMode.FixedProdOrder)
                //{
                //    returner = entityExtProdOrder.Add(actorSId, this.InputInfo.Info.SId, this.InputInfo.Info.ExtOrderSId, status, this.InputInfo.Info.QuotnDate.Value, this.InputInfo.Info.Cdd, this.InputInfo.Info.Edd, this.InputInfo.Info.Rmk);
                //    if (returner.IsCompletedAndContinue)
                //    {
                //        #region 品項
                //        for (int i = 0; i < this.InputInfo.DetInfos.Count; i++)
                //        {
                //            var detInfo = this.InputInfo.DetInfos[i];

                //            //新增
                //            entityExtProdOrderDet.Add(actorSId, detInfo.SId, detInfo.ExtProdOrderSId, detInfo.Source.Value, detInfo.OrgCode, detInfo.Model, detInfo.PartNo, detInfo.Qty.Value, detInfo.CumProdQty.Value, detInfo.ProdQty.Value, detInfo.EstFpmsDate, detInfo.Rmk, i + 1);

                //            //異動記錄
                //            using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_PROD_ORDER_DET, detInfo.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, detInfo.PartNo, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                //            {
                //            }
                //        }
                //        #endregion

                //        //異動記錄
                //        string dataTitle = string.Format("{0}", this.OrigInfo.Info.ProdOdrNo);
                //        using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.EXT_PROD_ORDER, this.InputInfo.Info.SId, DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                //        {
                //        }

                //        transaction.Complete();
                //    }
                //}
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

    #region 刪除外銷生產單
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
                ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                ExtProdOrderDet entityExtProdOrderDet = new ExtProdOrderDet(SystemDefine.ConnInfo);
                DataTransLog entityDataTransLog = new DataTransLog(SystemDefine.ConnInfo);

                //刪除外銷訂單下的所有外銷生產單
                returner = entityExtProdOrder.GetInfo(new ExtProdOrder.InfoConds(DefVal.SIds, this.OrigInfo.Info.ExtOrderSId, DefVal.Int, DefVal.Bool, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT), Int32.MaxValue, new SqlOrder("VERSION", Sort.Descending), IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = ExtProdOrder.Info.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        ExtProdOrderDet.Info[] extProdOrderDetInfos = null;

                        #region 外銷生產單明細
                        if (true) //只是為了不想重複宣告區域變數
                        {
                            returner = entityExtProdOrderDet.GetInfo(new ExtProdOrderDet.InfoConds(DefVal.SIds, info.SId, DefVal.Int, DefVal.Str), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All);
                            if (returner.IsCompletedAndContinue)
                            {
                                extProdOrderDetInfos = ExtProdOrderDet.Info.Binding(returner.DataSet.Tables[0]);

                                info.ChildTables.Add(new DataTransChildTable()
                                {
                                    TableName = DBTableDefine.EXT_PROD_ORDER_DET,
                                    Rows = extProdOrderDetInfos
                                });
                            }
                        }
                        #endregion

                        //異動記錄
                        string dataTitle = this.OrigInfo.Info.ProdOdrNo;
                        entityDataTransLog.Add(actorSId, DBTableDefine.EXT_PROD_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "D", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(info));

                        entityExtProdOrder.Delete(actorSId, new ISystemId[] { info.SId });

                        #region 外銷生產單明細
                        if (extProdOrderDetInfos != null && extProdOrderDetInfos.Length > 0)
                        {
                            entityExtProdOrderDet.DeleteByExtProdOrderSId(ConvertLib.ToSIds(info.SId));
                        }
                        #endregion
                    }
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityExtOrder.UpdateStatus(actorSId, this.OrigInfo.Info.ExtOrderSId, 2);

                transaction.Complete();
            }

            Response.Redirect("index.aspx");
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

                JSBuilder.AlertMessage(this, true, "資料已儲存");

                QueryStringParsing query = new QueryStringParsing(QueryStringParsing.CurrentRelativeUri);
                JSBuilder.PageRedirect(this, true, query.ToString());
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 確認
    protected void btnFixedProdOrder_Click(object sender, EventArgs e)
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
                if (!this.Save(2, CheckMode.FixedProdOrder))
                {
                    return;
                }

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                //更改訂單狀態
                entityExtOrder.UpdateStatus(actorSId, this.OrigInfo.Info.ExtOrderSId, 4);

                transaction.Complete();

                QueryStringParsing query = new QueryStringParsing();
                query.HttpPath = new Uri("view.aspx", UriKind.Relative);
                //改成用生產單系統代號
                //query.Add("quotnSId", this.OrigInfo.Info.ExtQuotnSId.Value);
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
}