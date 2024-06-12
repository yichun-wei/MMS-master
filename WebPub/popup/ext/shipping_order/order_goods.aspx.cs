using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class popup_ext_shipping_order_order_goods : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientPopupMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 是否重新執行查詢動作。
    /// </summary>
    bool DoSearch { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }
    #endregion

    public ISystemId ContainerSId { get; set; }
    public ISystemId ExtOrderSId { get; set; }
    public long? CustomerId { get; set; }

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
        this.MainPage = (IClientPopupMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        this.ContainerSId = ConvertLib.ToSId(Request.QueryString["cntrSId"]);
        this.ExtOrderSId = ConvertLib.ToSId(Request.QueryString["extOrderSId"]);
        this.CustomerId = ConvertLib.ToLong(Request.QueryString["customerId"], DefVal.Long);

        if (this.ContainerSId == null || this.ExtOrderSId == null || this.CustomerId == null)
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
        this.PageTitle = "品項清單";

        Returner returner = null;
        try
        {

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

        if (!this.IsPostBack)
        {
            //第一次載入時, 要先從 opener 取得已存在的品項, 載入後品得後再建立列表.
            WebPg.RegisterScript("initPageInfo();");
        }
        else
        {
            var scriptManager = ScriptManager.GetCurrent(this);
            if (!scriptManager.IsInAsyncPostBack)
            {
                this.ErpInvList();
            }
        }
    }

    protected void btnInitPageInfo_Click(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.Page_DoSearch(null, EventArgs.Empty);
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
    }

    #region ERP 庫存列表
    /// <summary> 
    /// ERP 庫存列表。 
    /// </summary> 
    void ErpInvList()
    {
        Returner returner = null;
        try
        {
            TableRow htmlTr = null;
            TableHeaderCell htmlTh;
            TableCell htmlTd;

            #region 標題
            Table htmlTable = new Table();
            htmlTable.CellPadding = 0;
            htmlTable.CellSpacing = 0;
            htmlTable.CssClass = "ListTable";
            this.phPageList.Controls.Add(htmlTable);

            htmlTr = new TableRow();
            htmlTable.Rows.Add(htmlTr);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "型號";
            htmlTr.Cells.Add(htmlTh);

            //htmlTh = new TableHeaderCell();
            //htmlTh.Text = "摘要";
            //htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "數量";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "選擇";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

            returner = entityExtOrderDet.GetInfoView(new ExtOrderDet.InfoViewConds(DefVal.SIds, this.CustomerId, ConvertLib.ToSIds(this.ExtOrderSId), DefVal.Int, DefVal.Str, DefVal.Bool, false, IncludeScope.OnlyNotMarkDeleted), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending));

            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                //已存在的品項暫存
                var existedGoodsItems = new List<string>();
                existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.PartNo);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.Model;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.PartNo;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = (info.Qty - info.ShipOdrUseQty).ToString();
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    htmlTd.Text = existedGoodsItems.Contains(info.PartNo) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{0}' />", info.PartNo, info.SId);

                    htmlTd.Text = existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.PartNo, info.SId)) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{1}{0}{2}' />", SystemDefine.JoinSeparator, info.PartNo, info.SId);
                    htmlTr.Cells.Add(htmlTd);
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}