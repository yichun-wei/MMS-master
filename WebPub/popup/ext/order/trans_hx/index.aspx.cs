using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Transactions;
using System.IO;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class popup_ext_order_trans_hx_index : System.Web.UI.Page
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
    /// 是否重新執行查詢動作。
    /// </summary>
    bool DoSearch { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }
    #endregion

    ISystemId _extQuotnSId;
    string _currencyCode;

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

        this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["quotnSId"]);
        if (this._extQuotnSId == null)
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
        this.PageTitle = "修改歷程";

        Returner returner = null;
        try
        {
            ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
            returner = entityExtQuotn.GetInfo(ConvertLib.ToSIds(this._extQuotnSId), IncludeScope.All, new string[] { "CURRENCY_CODE" });
            if (returner.IsCompletedAndContinue)
            {
                DataRow row = returner.DataSet.Tables[0].Rows[0];

                this._currencyCode = row["CURRENCY_CODE"].ToString();
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

        var scriptManager = ScriptManager.GetCurrent(this);
        if (!scriptManager.IsInAsyncPostBack)
        {
            this.PageList();
        }
    }

    #region 列示頁面的主要資料
    /// <summary> 
    /// 列示頁面的主要資料。 
    /// </summary> 
    void PageList()
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
            htmlTh.Text = "版本號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "客戶名稱";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "報價單日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "修改日期";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = string.Format("總金額 ({0})", this._currencyCode);
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);

            IncludeScope dataScope = IncludeScope.OnlyNotMarkDeleted;

            var conds = new ExtOrder.InfoViewConds
                (
                   DefVal.SIds,
                   DefVal.SIds,
                   this._extQuotnSId,
                   DefVal.Int,
                   false,
                   DefVal.Ints,
                   DefVal.Long,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   DefVal.DT,
                   false,
                   DefVal.Bool
                );

            SqlOrder sorting = new SqlOrder("VERSION", Sort.Descending);
            returner = entityExtOrder.GetInfoView(conds, Int32.MaxValue, sorting, dataScope);

            QueryStringParsing query = new QueryStringParsing();
            query.HttpPath = new Uri("cont.aspx", UriKind.Relative);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtOrder.InfoView.Binding(returner.DataSet.Tables[0]);

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("版本 {0}", info.Version.ToString().PadLeft(3, '0'));

                    query.Add("sid", info.SId.Value);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Format("<a href='{0}' title='版本 {1}'>{1}</a>", query, info.Version.ToString().PadLeft(3, '0'));
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.CustomerName;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.QuotnDate, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = ConvertLib.ToStr(info.Mdt, string.Empty, "yyyy-MM-dd");
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    if (info.TotalAmt.HasValue)
                    {
                        htmlTd.Text = ConvertLib.ToAccounting(info.TotalAmt.Value);
                    }
                    else
                    {
                        htmlTd.Text = string.Empty;
                    }
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