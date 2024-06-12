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

public partial class include_client_dom_order_edit_goods_item_block : System.Web.UI.UserControl
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
        /// 原始數量。
        /// 適用在編輯時記錄的原始數量。
        /// </summary>
        public const string OrigQty = "OrigQty";
        /// <summary>
        /// 備貨單編號。
        /// </summary>
        public const string PGOrderOdrNo = "PGOrderOdrNo";
        /// <summary>
        /// 備貨單明細系統代號。
        /// </summary>
        public const string PGOrderDetSId = "PGOrderDetSId";
        /// <summary>
        /// 報價單號碼。
        /// </summary>
        public const string QuoteNumber = "QuoteNumber";
        /// <summary>
        /// 報價單明細項次。
        /// </summary>
        public const string QuoteItemId = "QuoteItemId";
        /// <summary>
        /// 牌價。
        /// 若有值，表示已檢查過料號是否存在價目表。
        /// </summary>
        public const string ListPrice = "ListPrice";
        /// <summary>
        /// 預設折扣。
        /// 若有值時，折扣不得小於預設折扣。
        /// </summary>
        public const string DefDct = "DefDct";
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
        WebUtilBox.AddAttribute(this.htmlTr, "class", string.Format("dev-goods-item dev-goods-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        //WebUtilBox.AddAttribute(this.txtQty, "onchange", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.changeQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.checkOnHand('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        if (!this.txtUnitPrice.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtUnitPrice, "onkeyup", string.Format("orderItemHelper.changeUnitPrice('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        }

        if (!this.txtDiscount.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtDiscount, "onkeyup", string.Format("ValidateFloat1(this,value); orderItemHelper.checkDiscount($(this),{1}); orderItemHelper.changeDiscount('{0}');", this.GetPgAttr(PgAttrDefine.SId), ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.DefDct), 0)));
        }
        #region 2016.7.28 test-By米雪

         this.lblPaidAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidPaidAmt.Value, DefVal.Float), string.Empty);
        //this.lblPaidAmtDisp.Text = string.Format("{0:N2}", this.hidPaidAmt.Value);
        #endregion



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
    public void SetInfo(DomOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null || string.IsNullOrWhiteSpace(editInfo.PartNo))
        {
            this.Visible = false;
            return;
        }

        if (editInfo.SId != null)
        {
            this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        }
        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litPartNo.Text = editInfo.PartNo;

        //改為取得所有倉庫的在手量
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        //this.lblErpOnHand.Text = string.Join("<br />", editInfo.ErpWhseOnHands.Select(q => string.Format("{0}：{1}", q.Whse, ConvertLib.ToAccounting(q.ErpOnHand, "0"))).ToArray());
        this.lblErpOnHand.Text = string.Join("<br />", editInfo.ErpWhseOnHands.Select(q => string.Format("<span whse='{0}' onhand='{1}'>{0}</span>：{2}", q.Whse, ConvertLib.ToInt(q.ErpOnHand, 0), ConvertLib.ToAccounting(q.ErpOnHand, "0"))).ToArray());

        this.txtQty.Text = ConvertLib.ToStr(editInfo.Qty, string.Empty);
        this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(editInfo.ListPrice, string.Empty));
        this.SetPgAttr(PgAttrDefine.DefDct, ConvertLib.ToStr(editInfo.DefDct, string.Empty));

        if (!this.IsPostBack)
        {
            this.SetPgAttr(PgAttrDefine.OrigQty, ConvertLib.ToStr(editInfo.Qty, "0"));
        }

        if (editInfo.UnitPrice.HasValue)
        {
            this.txtUnitPrice.Text = editInfo.UnitPrice.ToString();
        }
        else if (editInfo.ListPrice.HasValue)
        {
            //若無單價, 則牌價 * 折扣.
            this.txtUnitPrice.Text = MathLib.Round(editInfo.ListPrice.Value * ConvertLib.ToSingle(editInfo.Discount, 1), 4).ToString();
        }

        if (editInfo.Discount.HasValue)
        {
            this.txtDiscount.Text = MathLib.Round(editInfo.Discount.Value * 100, 2).ToString();
        }
        else
        {
            var unitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float);
            if (editInfo.ListPrice.HasValue && unitPrice.HasValue)
            {
                this.txtDiscount.Text = MathLib.Round(unitPrice.Value / editInfo.ListPrice.Value * 100, 2).ToString();
            }
        }

        this.hidPaidAmt.Value = ConvertLib.ToStr(editInfo.PaidAmt, string.Empty);

        this.txtRmk.Text = editInfo.Rmk;

        //if (editInfo.ListPrice == null)
        //{
        //    //若料號比對不到價格, 要秀出錯誤訊息提示
        //    WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
        //}

        //若為備貨單品項或專案報價品項, 則顯示「數值最大值」
        this.lblMaxQtyContainer.Visible = editInfo.PGOrderDetSId != null || (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId));

        if (editInfo.PGOrderDetSId != null && !string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            #region 來自於備貨單的專案報價品項
            this.SetPgAttr(PgAttrDefine.PGOrderOdrNo, editInfo.PGOrderOdrNo);
            this.SetPgAttr(PgAttrDefine.PGOrderDetSId, editInfo.PGOrderDetSId.Value);
            this.SetPgAttr(PgAttrDefine.QuoteNumber, editInfo.QuoteNumber);
            this.SetPgAttr(PgAttrDefine.QuoteItemId, editInfo.QuoteItemId);

            this.litSource.Text = string.Format("備貨單 {0}", editInfo.PGOrderOdrNo);

            //若為備貨單的專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.lblMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.lblMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }

            //專案報價不顯示建立備貨單的核選方塊
            this.chkSel.Visible = false;
            #endregion
        }
        else if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            #region 直接來自於專案報價品項
            this.SetPgAttr(PgAttrDefine.QuoteNumber, editInfo.QuoteNumber);
            this.SetPgAttr(PgAttrDefine.QuoteItemId, editInfo.QuoteItemId);

            //若為專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = ProjQuoteHelper.GetMaxQty(editInfo.QuoteNumber, editInfo.QuoteItemId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.lblMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.lblMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }

            //專案報價不顯示建立備貨單的核選方塊
            this.chkSel.Visible = false;
            #endregion
        }
        else if (editInfo.PGOrderDetSId != null)
        {
            #region 來自於備貨單一般品項
            this.SetPgAttr(PgAttrDefine.PGOrderOdrNo, editInfo.PGOrderOdrNo);
            this.SetPgAttr(PgAttrDefine.PGOrderDetSId, editInfo.PGOrderDetSId.Value);

            this.litSource.Text = string.Format("備貨單 {0}", editInfo.PGOrderOdrNo);

            //若為備貨單的專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.lblMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.lblMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }
            #endregion
        }
        else
        {
            //一般品項
            this.lblMaxQtyContainer.Visible = false;
        }

        //專案報價品項
        if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            //不能更改單價和折扣
            this.txtUnitPrice.ReadOnly = true;
            this.txtDiscount.ReadOnly = true;
        }
    }
    #endregion

    #region 取得輸入資訊
    public DomOrderHelper.GoodsItemEditInfo GetInfo()
    {
        var editInfo = new DomOrderHelper.GoodsItemEditInfo()
        {
            IsSeled = this.chkSel.Checked,

            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            PartNo = this.litPartNo.Text,
            PGOrderOdrNo = this.GetPgAttr(PgAttrDefine.PGOrderOdrNo),
            PGOrderDetSId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.PGOrderDetSId)),
            QuoteNumber = this.GetPgAttr(PgAttrDefine.QuoteNumber),
            QuoteItemId = this.GetPgAttr(PgAttrDefine.QuoteItemId),
            ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.txtQty.Text, DefVal.Int),
            //MaxQty = ConvertLib.ToInt(this.lblMaxQty.Text, 0),
            ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float),
            DefDct = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.DefDct), DefVal.Float),
            Discount = ConvertLib.ToSingle(this.txtDiscount.Text, DefVal.Float),
            PaidAmt = ConvertLib.ToSingle(this.hidPaidAmt.Value, DefVal.Float),
            Rmk = this.txtRmk.Text
        };

        #region 先重整一次品項允許的最大值
        if (editInfo.PGOrderDetSId != null && !string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            //來自於備貨單的專案報價品項
            editInfo.MaxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(this.GetPgAttr(PgAttrDefine.OrigQty), 0);
            this.lblMaxQty.Text = editInfo.MaxQty.ToString();
        }
        else if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            //直接來自於專案報價品項
            editInfo.MaxQty = ProjQuoteHelper.GetMaxQty(editInfo.QuoteNumber, editInfo.QuoteItemId) + ConvertLib.ToInt(this.GetPgAttr(PgAttrDefine.OrigQty), 0);
            this.lblMaxQty.Text = editInfo.MaxQty.ToString();
        }
        else if (editInfo.PGOrderDetSId != null)
        {
            //來自於備貨單一般品項
            editInfo.MaxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(this.GetPgAttr(PgAttrDefine.OrigQty), 0);
            this.lblMaxQty.Text = editInfo.MaxQty.ToString();
        }
        else
        {
            //一般品項
            editInfo.MaxQty = ConvertLib.ToInt(this.lblMaxQty.Text, 0);
        }
        #endregion

        return editInfo;
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