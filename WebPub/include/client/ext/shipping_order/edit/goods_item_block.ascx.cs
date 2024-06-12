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

public partial class include_client_ext_shipping_order_edit_goods_item_block : System.Web.UI.UserControl
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
        /// 外銷訂單明細系統代號。
        /// </summary>
        public const string ExtOrderDetSId = "ExtOrderDetSId";
        /// <summary>
        /// 牌價。
        /// 若有值，表示已檢查過料號是否存在價目表。
        /// </summary>
        public const string ListPrice = "ListPrice";
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
        WebUtilBox.AddAttribute(this.txtUnitPriceDct, "onkeyup", string.Format("orderItemHelper.changeUnitPriceDct('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        if (!this.txtUnitPrice.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtUnitPrice, "onkeyup", string.Format("orderItemHelper.changeUnitPrice('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        }

        if (!this.txtDiscount.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtDiscount, "onkeyup", string.Format("ValidateFloat1(this,value); orderItemHelper.checkDiscount($(this)); orderItemHelper.changeDiscount('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        }

        this.lblPaidAmtDisp.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidPaidAmt.Value, DefVal.Float), string.Empty);
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
    public void SetInfo(ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase editInfo)
    {
        if (this.IsPostBack && !this.Visible) { return; }

        if (editInfo == null || string.IsNullOrWhiteSpace(editInfo.PartNo))
        {
            this.Visible = false;
            return;
        }

        //外銷訂單明細系統代號
        this.SetPgAttr(PgAttrDefine.ExtOrderDetSId, editInfo.ExtOrderDetSId.Value);

        if (editInfo.SId != null)
        {
            this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        }
        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litModel.Text = editInfo.Model;
        this.litPartNo.Text = editInfo.PartNo;
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        this.txtQty.Text = ConvertLib.ToStr(editInfo.Qty, string.Empty);
        this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(editInfo.ListPrice, string.Empty));

        float? unitPrice = 0f;
        if (editInfo.UnitPrice.HasValue)
        {
            unitPrice = editInfo.UnitPrice;
        }
        else if (editInfo.ListPrice.HasValue)
        {
            //若無單價, 則牌價 * 折扣.
            unitPrice = (float)MathLib.Round(editInfo.ListPrice.Value * ConvertLib.ToSingle(editInfo.Discount, 1), 4);
        }
        this.txtUnitPrice.Text = ConvertLib.ToStr(unitPrice, string.Empty);

        this.txtUnitPriceDct.Text = ConvertLib.ToStr(editInfo.UnitPriceDct, ConvertLib.ToStr(unitPrice, string.Empty));

        if (editInfo.Discount.HasValue)
        {
            this.txtDiscount.Text = MathLib.Round(editInfo.Discount.Value * 100, 2).ToString();
        }
        else
        {
            //var unitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float);
            if (editInfo.ListPrice.HasValue && unitPrice.HasValue)
            {
                this.txtDiscount.Text = MathLib.Round(unitPrice.Value / editInfo.ListPrice.Value * 100, 2).ToString();
            }
        }

        this.hidPaidAmt.Value = ConvertLib.ToStr(editInfo.PaidAmt, string.Empty);

        this.txtRmk.Text = editInfo.Rmk;

        if (editInfo.ListPrice == null)
        {
            //若料號比對不到價格, 要秀出錯誤訊息提示
            WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
        }

        //最大值
        if (editInfo.MaxQty == 0)
        {
            int maxQty = ExtShippingOrderHelper.GetExtOrderDetMaxQty(editInfo.ExtOrderDetSId) + ConvertLib.ToInt(editInfo.Qty, 0);
            this.lblMaxQty.Text = maxQty.ToString();
        }
        else
        {
            //進來前已取過, 就毋須再取一次.
            this.lblMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
        }
    }
    #endregion

    #region 取得輸入資訊
    public ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase GetInfo()
    {
        return new ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            ExtOrderDetSId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.ExtOrderDetSId)),
            Model = this.litModel.Text,
            PartNo = this.litPartNo.Text,
            //ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.txtQty.Text, DefVal.Int),
            MaxQty = ConvertLib.ToInt(this.lblMaxQty.Text, 0),
            ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float),
            UnitPriceDct = ConvertLib.ToSingle(this.txtUnitPriceDct.Text, DefVal.Float),
            Discount = ConvertLib.ToSingle(this.txtDiscount.Text, DefVal.Float),
            PaidAmt = ConvertLib.ToSingle(this.hidPaidAmt.Value, DefVal.Float),
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