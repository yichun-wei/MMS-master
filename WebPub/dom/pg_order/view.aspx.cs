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

public partial class dom_pg_order_view : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "DOM_PG_ORDER";
    string AUTH_NAME = "內銷備貨單";

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
    PGOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _pgOrderSId;

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

        this._pgOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._pgOrderSId == null)
        {
            Response.Redirect(SystemDefine.HomePageUrl);
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
        this.PageTitle = "檢視備貨單";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='index.aspx'>內銷備貨單</a>"));
        breadcrumbs.Add(string.Format("<a href='{0}'>檢視備貨單</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));

        this.btnDelete.OnClientClick = string.Format("javascript:if(!window.confirm('確定要刪除？')){{return false;}}");
        this.btnCancel.OnClientClick = string.Format("javascript:if(!window.confirm('確定要取消？')){{return false;}}");

        Returner returner = null;
        try
        {
            this.OrigInfo = PGOrderHelper.Binding(this._pgOrderSId, IncludeScope.OnlyNotMarkDeleted);
            if (this.OrigInfo != null)
            {
                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.OrigInfo.Info.OdrNo));

                this.litCdt.Text = this.OrigInfo.Info.Cdt.ToString("yyyy-MM-dd");
                this.litOdrNo.Text = this.OrigInfo.Info.OdrNo;
                this.litDomDistName.Text = this.OrigInfo.Info.DomDistName;
                this.litCustomerName.Text = this.OrigInfo.Info.CustomerName;
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
                this.litRmk.Text = WebUtilBox.ConvertNewLineToHtmlBreak(this.OrigInfo.Info.Rmk);
                this.litStatus.Text = PGOrderHelper.GetOrderStatusName(this.OrigInfo.Info);

                #region 品項資訊
                if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
                {
                    //一般品項
                    if (true)
                    {
                        var generalInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 1).ToArray();
                        var block = (ASP.include_client_dom_pg_order_view_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/view/goods_block.ascx");
                        this.phGoodsList.Controls.Add(block);
                        block.SetInfo("一般備貨訂單", generalInfos);
                    }

                    //專案報價錨點索引
                    var builderProjQuoteIdxes = new StringBuilder();

                    //專案報價品項
                    var projQuoteInfos = this.OrigInfo.DetInfos.Where(q => q.Source == 2).GroupBy(q => new { QuoteNumber = q.QuoteNumber }).Select(q => new { QuoteNumber = q.Key.QuoteNumber, GoodsItemList = q.ToArray() });
                    this.phProjQuoteIndex.Visible = projQuoteInfos.Count() > 0;
                    foreach (var projQuoteInfo in projQuoteInfos)
                    {
                        var block = (ASP.include_client_dom_pg_order_view_goods_block_ascx)this.LoadControl("~/include/client/dom/pg_order/view/goods_block.ascx");
                        this.phGoodsList.Controls.Add(block);
                        block.SetInfo(projQuoteInfo.QuoteNumber, projQuoteInfo.GoodsItemList);

                        //專案報價錨點索引
                        builderProjQuoteIdxes.AppendFormat("<li class='dev-proj-quote-idx' quoteno='{0}'><div><a href='#GoodsBlock_{0}' class='tag'>{0}</a></div></li>", projQuoteInfo.QuoteNumber);
                    }

                    //專案報價錨點索引
                    this.litProjQuoteIdxes.Text = builderProjQuoteIdxes.ToString();
                }
                #endregion

                //1:未使用 2:已使用 3:已取消
                switch (PGOrderHelper.GetOrderStatus(this.OrigInfo.Info))
                {
                    case 1:
                        //未使用 - 才能刪除
                        this.btnDelete.Visible = true;
                        break;
                    case 2:
                        //已使用 - 只能取消, 已使用的就已使用.
                        this.btnCancel.Visible = true;
                        break;
                    default:
                        //已取消 - 都不顯示
                        break;
                }
            }
            else
            {
                Response.Redirect(SystemDefine.HomePageUrl);
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
        this.btnCancel.Visible = this.btnCancel.Visible ? this.FunctionRight.Maintain : this.btnCancel.Visible;
    }

    #region 刪除
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
                PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);

                //異動記錄
                string dataTitle = this.OrigInfo.Info.OdrNo;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.PG_ORDER, this.OrigInfo.Info.SId, DefVal.Int, "MD", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                entityPGOrder.SwitchMarkDeleted(actorSId, ConvertLib.ToSIds(this.OrigInfo.Info.SId), true);

                transaction.Complete();
            }

            JSBuilder.AlertMessage(this, "備貨單已刪除");
            JSBuilder.PageRedirect(this, "index.aspx");
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

        ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

        Returner returner = null;
        try
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);
                PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);

                entityPGOrder.UpdateCancelInfo(actorSId, this.OrigInfo.Info.SId, true, DateTime.Now);

                #region 將專案報價品項未被使用的「還原回去」
                //備貨單的取消操作, 來自於已有訂單使用備貨單品項.
                //若取消時, 將專案報價品項未被使用的「還原回去」, 也就是將備貨單中的專案報價品項數量讓他等於已被使用的量.
                returner = entityPGOrderDet.GetInfoView(new PGOrderDet.InfoViewConds(DefVal.SIds, this.OrigInfo.Info.SId, DefVal.Str, 2, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Long, DefVal.Bool, false, IncludeScope.OnlyNotMarkDeleted), Int32.MaxValue, SqlOrder.Clear);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        var remaining = info.Qty - info.DomOrderUseQty;

                        if (remaining < 0)
                        {
                            //「備貨單品項數量 - 內銷訂單使用量」為負值. 理論上不可能, 但若發生, 發出警示.
                            JSBuilder.AlertMessage(this, string.Format("料號「{0}」還原異常 (數量為負值)", info.PartNo));
                            return;
                        }

                        entityPGOrderDet.UpdateQty(actorSId, info.PGOrderSId, remaining);
                    }
                }
                #endregion

                transaction.Complete();
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

        JSBuilder.AlertMessage(this, "備貨單已取消");
        JSBuilder.PageRedirect(this, QueryStringParsing.CurrentRelativeUri.OriginalString);
    }
    #endregion
}