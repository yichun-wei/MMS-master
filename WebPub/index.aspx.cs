using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class index : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 內銷備貨單功能權限。
    /// </summary>
    FunctionRight DomPGOrderRight { get; set; }
    /// <summary>
    /// 內銷訂單功能權限。
    /// </summary>
    FunctionRight DomOrderRight { get; set; }
    /// <summary>
    /// 內銷出貨單功能權限。
    /// </summary>
    FunctionRight DomShippingOrderRight { get; set; }

    /// <summary>
    /// 外銷報價單功能權限。
    /// </summary>
    FunctionRight ExtQuotnRight { get; set; }
    /// <summary>
    /// 外銷訂單功能權限。
    /// </summary>
    FunctionRight ExtOrderRight { get; set; }
    /// <summary>
    /// 外銷生產單功能權限。
    /// </summary>
    FunctionRight ExtProdOrderRight { get; set; }
    /// <summary>
    /// 外銷出貨單功能權限。
    /// </summary>
    FunctionRight ExtShippingOrderRight { get; set; }
    #endregion

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

        //內銷備貨單功能權限
        this.DomPGOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_PG_ORDER");
        //內銷出貨單權限
        this.DomOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_ORDER");
        //內銷出貨單權限
        this.DomShippingOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("DOM_SHIPPING_ORDER");

        //外銷報價單功能權限
        this.ExtQuotnRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("EXT_QUOTN");
        //外銷訂單功能權限
        this.ExtOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("EXT_ORDER");
        //外銷生產單功能權限
        this.ExtProdOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("EXT_PROD_ORDER");
        //外銷出貨單功能權限
        this.ExtShippingOrderRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight("EXT_SHIPPING_ORDER");

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "首頁";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        Returner returner = null;
        try
        {
            PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);
            DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);

            ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
            ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
            ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);

            //系統使用者依所指定的內銷地區範圍設定
            ISystemId[] domDistSIds = this.MainPage.ActorInfoSet.DomDistInfos.Select(q => q.SId).ToArray();
            if (domDistSIds.Length == 0)
            {
                //若沒被指定的內銷地區, 則直接使其找不到.
                domDistSIds = ConvertLib.ToSIds(SystemId.MaxValue);
            }

            QueryStringParsing query;

            #region 內銷 訊息摘要
            #region 備貨單-未使用
            this.phPGOrderUnusedCnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.DomPGOrderRight.View;
            if (this.phPGOrderUnusedCnt.Visible)
            {
                returner = entityPGOrder.GetInfoViewCount(new PGOrder.InfoViewConds(DefVal.SIds, domDistSIds, DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, false, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}dom/pg_order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "1");

                this.lnkPGOrderUnusedCnt.NavigateUrl = query.ToString();
                this.lnkPGOrderUnusedCnt.Text = string.Format("備貨單-未使用<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 營管待審核
            this.phDomOrder_Status_1_Cnt.Visible =
                this.MainPage.ActorInfoSet.UserRight.IsFullControl
                || (this.DomOrderRight.View && this.MainPage.ActorInfoSet.Info.DomAuditPerms.IndexOf("-1-") != -1);

            if (this.phDomOrder_Status_1_Cnt.Visible)
            {
                returner = entityDomOrder.GetInfoViewCount(new DomOrder.InfoViewConds(DefVal.SIds, domDistSIds, DefVal.Strs, ConvertLib.ToInts(1), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}dom/order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "1");

                this.lnkDomOrder_Status_1_Cnt.NavigateUrl = query.ToString();
                this.lnkDomOrder_Status_1_Cnt.Text = string.Format("營管待審核<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 財務待審核
            this.phDomOrder_Status_3_Cnt.Visible =
                this.MainPage.ActorInfoSet.UserRight.IsFullControl
                || (this.DomOrderRight.View && this.MainPage.ActorInfoSet.Info.DomAuditPerms.IndexOf("-2-") != -1);

            if (this.phDomOrder_Status_3_Cnt.Visible)
            {
                returner = entityDomOrder.GetInfoViewCount(new DomOrder.InfoViewConds(DefVal.SIds, domDistSIds, DefVal.Strs, ConvertLib.ToInts(3), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}dom/order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "3");

                this.lnkDomOrder_Status_3_Cnt.NavigateUrl = query.ToString();
                this.lnkDomOrder_Status_3_Cnt.Text = string.Format("財務待審核<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 未付款待審核
            this.phDomOrder_Status_4_Cnt.Visible =
                this.MainPage.ActorInfoSet.UserRight.IsFullControl
                || (this.DomOrderRight.View && this.MainPage.ActorInfoSet.Info.DomAuditPerms.IndexOf("-3-") != -1);

            if (this.phDomOrder_Status_4_Cnt.Visible)
            {
                returner = entityDomOrder.GetInfoViewCount(new DomOrder.InfoViewConds(DefVal.SIds, domDistSIds, DefVal.Strs, ConvertLib.ToInts(4), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}dom/order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "4");

                this.lnkDomOrder_Status_4_Cnt.NavigateUrl = query.ToString();
                this.lnkDomOrder_Status_4_Cnt.Text = string.Format("未付款待審核<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 備貨中
            this.phDomOrder_Status_7_Cnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.DomShippingOrderRight.View;

            if (this.phDomOrder_Status_7_Cnt.Visible)
            {
                returner = entityDomOrder.GetInfoViewCount(new DomOrder.InfoViewConds(DefVal.SIds, domDistSIds, DefVal.Strs, ConvertLib.ToInts(7), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}dom/order/shipping/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "7");

                this.lnkDomOrder_Status_7_Cnt.NavigateUrl = query.ToString();
                this.lnkDomOrder_Status_7_Cnt.Text = string.Format("備貨中<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            //內銷 訊息摘要顯示判斷
            do
            {
                var visible = false;
                foreach (var ctrl in this.phDomOrderSummary.Controls)
                {
                    var ph = ctrl as PlaceHolder;
                    if (ph != null && ph.Visible)
                    {
                        //只要有一個子項顯示, 訊息摘要則顯示.
                        visible = true;
                        break;
                    }
                }
                this.phDomOrderSummary.Visible = visible;
            } while (false);
            #endregion

            #region 外銷 訊息摘要
            #region 報價單草稿
            this.phExtQuonDraftCnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.ExtQuotnRight.View;
            if (this.phExtQuonDraftCnt.Visible)
            {
                returner = entityExtQuotn.GetInfoCount(new ExtQuotn.InfoConds(DefVal.SIds, ConvertLib.ToInts(0), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}ext/quotn/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "1");

                this.lnkExtQuonDraftCnt.NavigateUrl = query.ToString();
                this.lnkExtQuonDraftCnt.Text = string.Format("報價單草稿<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 待轉為訂單
            this.phExtOrder_Status_1_Cnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.ExtOrderRight.View;
            if (this.phExtOrder_Status_1_Cnt.Visible)
            {
                returner = entityExtOrder.GetInfoViewCount(new ExtOrder.InfoViewConds(DefVal.SIds, DefVal.SIds, DefVal.SId, DefVal.Int, true, ConvertLib.ToInts(1), DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, false, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}ext/order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "1");

                this.lnkExtOrder_Status_1_Cnt.NavigateUrl = query.ToString();
                this.lnkExtOrder_Status_1_Cnt.Text = string.Format("待轉為訂單<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 正式訂單未排程
            this.phExtOrder_Status_3_Cnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.ExtOrderRight.View;
            if (this.phExtOrder_Status_3_Cnt.Visible)
            {
                returner = entityExtOrder.GetInfoViewCount(new ExtOrder.InfoViewConds(DefVal.SIds, DefVal.SIds, DefVal.SId, DefVal.Int, true, ConvertLib.ToInts(3), DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, false, false), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}ext/order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "3");

                this.lnkExtOrder_Status_3_Cnt.NavigateUrl = query.ToString();
                this.lnkExtOrder_Status_3_Cnt.Text = string.Format("正式訂單未排程<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 出貨單已確認(未出貨)
            this.phExtShippingOrder_Status_1_Cnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.ExtShippingOrderRight.View;
            if (this.phExtShippingOrder_Status_1_Cnt.Visible)
            {
                returner = entityExtShippingOrder.GetInfoViewCount(new ExtShippingOrder.InfoViewConds(DefVal.SIds, ConvertLib.ToInts(1), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}ext/shipping_order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "1");

                this.lnkExtShippingOrder_Status_1_Cnt.NavigateUrl = query.ToString();
                this.lnkExtShippingOrder_Status_1_Cnt.Text = string.Format("出貨單已確認(未出貨)<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            #region 出貨單已出貨(未上傳)
            this.phExtShippingOrder_Status_2_Cnt.Visible = this.MainPage.ActorInfoSet.UserRight.IsFullControl || this.ExtShippingOrderRight.View;
            if (this.phExtShippingOrder_Status_2_Cnt.Visible)
            {
                returner = entityExtShippingOrder.GetInfoViewCount(new ExtShippingOrder.InfoViewConds(DefVal.SIds, ConvertLib.ToInts(2), DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints), IncludeScope.OnlyNotMarkDeleted);

                query = new QueryStringParsing();
                query.HttpPath = new Uri(string.Format("{0}ext/shipping_order/index.aspx", SystemDefine.WebSiteRoot), UriKind.Relative);
                query.Add("status", "2");

                this.lnkExtShippingOrder_Status_2_Cnt.NavigateUrl = query.ToString();
                this.lnkExtShippingOrder_Status_2_Cnt.Text = string.Format("出貨單已出貨(未上傳)<span class='d_no'>{0}</span>", returner.DataSet.Tables[0].Rows[0][0]);
            }
            #endregion

            //外銷 訊息摘要顯示判斷
            do
            {
                var visible = false;
                foreach (var ctrl in this.phExtOrderSummary.Controls)
                {
                    var ph = ctrl as PlaceHolder;
                    if (ph != null && ph.Visible)
                    {
                        //只要有一個子項顯示, 訊息摘要則顯示.
                        visible = true;
                        break;
                    }
                }
                this.phExtOrderSummary.Visible = visible;
            } while (false);
            #endregion

            #region 登入資訊
            LoginLog entityLoginLog = new LoginLog(SystemDefine.ConnInfo);
            returner = entityLoginLog.GetInfo(new LoginLog.InfoConds((int)OperPosition.GeneralClient, this.MainPage.ActorInfoSet.Info.SId, DefVal.Str, true), 2, new SqlOrder("SID", Sort.Descending), IncludeScope.All, new string[] { "CDT", "CLIENT_IP" });
            if (returner.IsCompletedAndContinue)
            {
                var infos = LoginLog.Info.Binding(returner.DataSet.Tables[0]);

                if (infos.Length == 1)
                {
                    this.litLastLoginDT.Text = "無";
                    this.litLastLoginIP.Text = "無";
                }
                else
                {
                    this.litLastLoginDT.Text = infos[1].Cdt.ToString("yyyy/MM/dd HH:mm:ss");
                    this.litLastLoginIP.Text = infos[1].ClientIP;
                }
            }
            else
            {
                this.litLastLoginDT.Text = "無";
                this.litLastLoginIP.Text = "無";
            }
            #endregion
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
    }
}