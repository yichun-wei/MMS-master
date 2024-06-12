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
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

public partial class include_client_dom_pg_order_edit_goods_item_block : System.Web.UI.UserControl
{
    public delegate void RemoveEventHandler(object seder, EventArgs e);
    public event RemoveEventHandler Remove;

    protected virtual void OnRemove(EventArgs e)
    {
        if (this.Remove != null)
        {
            this.Remove(this, EventArgs.Empty);
        }
    }

    #region 網頁屬性
    /// <summary>
    /// 屬性常數定義。
    /// </summary>
    struct PgAttrDefine
    {
        /// <summary>
        /// 系統代號。
        /// </summary>
        public const string SId = "SId";
        /// <summary>
        /// 報價單號碼。
        /// </summary>
        public const string QuoteNumber = "QuoteNumber";
        /// <summary>
        /// 報價單明細項次。
        /// </summary>
        public const string QuoteItemId = "QuoteItemId";
    }

    string GetPgAttr(string key)
    {
        return this.InitVals.Attributes[key];
    }

    void SetPgAttr(string key, string value)
    {
        this.InitVals.Attributes[key] = value;
    }
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        this.btnRemove.OnClientClick = string.Format("javascript:if(!window.confirm('確定要刪除？')){{return false;}}");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SystemId.MinValue.IsSystemId(this.GetPgAttr(PgAttrDefine.SId)))
        {
            this.SetPgAttr(PgAttrDefine.SId, new SystemId().Value);
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WebUtilBox.AddAttribute(this.htmlTr, "class", string.Format("dev-goods-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtQty, "onchange", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
    }

    /// <summary>
    /// 序號。
    /// </summary>
    public int SeqNo
    {
        get { return Convert.ToInt32(this.litSeqNo.Text); }
        set { this.litSeqNo.Text = value.ToString(); }
    }

    #region 設定資訊
    public void SetInfo(PGOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null || string.IsNullOrWhiteSpace(editInfo.PartNo))
        {
            this.Visible = false;
            return;
        }

        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litSummary.Text = editInfo.Summary;
        this.litPartNo.Text = editInfo.PartNo;

        if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            this.SetPgAttr(PgAttrDefine.QuoteNumber, editInfo.QuoteNumber);
            this.SetPgAttr(PgAttrDefine.QuoteItemId, editInfo.QuoteItemId);

            //若為專案報價品項, 則顯示「數值最大值」
            //不需要編輯, 直接使用傳進來的最大值即可.
            //int maxQty = ProjQuoteHelper.GetMaxQty(editInfo.QuoteNumber, editInfo.QuoteItemId);
            //this.lblMaxQty.Text = maxQty.ToString();
            this.lblMaxQty.Text = editInfo.MaxQty.ToString();
        }
        else
        {
            this.lblMaxQtyContainer.Visible = false;
        }
    }
    #endregion

    #region 取得輸入資訊
    public PGOrderHelper.GoodsItemEditInfo GetInfo()
    {
        return new PGOrderHelper.GoodsItemEditInfo()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            PartNo = this.litPartNo.Text,
            Summary = this.litSummary.Text,
            QuoteNumber = this.GetPgAttr(PgAttrDefine.QuoteNumber),
            QuoteItemId = this.GetPgAttr(PgAttrDefine.QuoteItemId),
            Qty = ConvertLib.ToInt(this.txtQty.Text, DefVal.Int),
            MaxQty = ConvertLib.ToInt(this.lblMaxQty.Text, 0),
            Rmk = this.txtRmk.Text
        };
    }
    #endregion

    #region 移除
    protected void btnRemove_Click(object sender, EventArgs e)
    {
        this.Visible = false;

        if (this.Remove != null)
        {
            this.Remove(this, EventArgs.Empty);
        }
    }
    #endregion
}