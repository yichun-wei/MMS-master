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

public partial class ext_prod_order_view : System.Web.UI.Page
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

        Returner returner = null;
        try
        {
            if (this.SetEditData())
            {
                this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Info.ProdOdrNo, this.OrigInfo.Info.CustomerName);

                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", new QueryStringParsing(new Uri("../order/view.aspx", UriKind.Relative), Request.QueryString), string.Format("{1} ({0})", this.OrigInfo.Info.OdrNo, this.OrigInfo.Info.CustomerName)));
                breadcrumbs.Add(string.Format("<a href='{0}'>{1}</a>", QueryStringParsing.CurrentRelativeUri.OriginalString, this.OrigInfo.Info.ProdOdrNo));
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
                this.litEdd.Text = ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy-MM-dd");
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
}