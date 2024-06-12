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

public partial class include_client_ext_shipping_order_edit_2_goods_item_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_ext_shipping_order_edit_2_goods_det_item_block_ascx> _blockDetItemList = new List<ASP.include_client_ext_shipping_order_edit_2_goods_det_item_block_ascx>();

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
        /// 這裡的系統代號不是「外銷出貨單明細系統代號」，僅為品項區塊的項目識別。
        /// </summary>
        public const string SId = "SId";
        ///// <summary>
        ///// 外銷訂單明細系統代號。
        ///// </summary>
        //public const string ExtOrderDetSId = "ExtOrderDetSId";
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

    string GetPgAttr(string key, Panel div)
    {
        return div.Attributes[key];
    }

    void SetPgAttr(string key, string value)
    {
        this.InitVals.Attributes[key] = value;
    }

    void SetPgAttr(string key, string value, Panel div)
    {
        div.Attributes[key] = value;
    }
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        this.btnRemove.OnClientClick = string.Format("javascript:if(!window.confirm('確定要刪除？')){{return false;}}");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!SystemId.MinValue.IsSystemId(this.GetPgAttr(PgAttrDefine.SId)))
        //{
        //    this.SetPgAttr(PgAttrDefine.SId, new SystemId().Value);
        //}

        #region 重載附件清單
        if (this.IsPostBack)
        {
            if (ViewState[USER_CONTROL_LOAD_SEQ] != null)
            {
                string[] loadSeq = StringLib.SplitSurrounded(ViewState[USER_CONTROL_LOAD_SEQ].ToString(), new LeftRightPair<char, char>('[', ']'));
                for (int i = 0; i < loadSeq.Length; i++)
                {
                    switch (loadSeq[i])
                    {
                        case "DetItem":
                            var blockDetItem = (ASP.include_client_ext_shipping_order_edit_2_goods_det_item_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit_2/goods_det_item_block.ascx");
                            this._blockDetItemList.Add(blockDetItem);
                            this.phDetItems.Controls.Add(blockDetItem);
                            break;
                    }
                }
            }
        }
        #endregion
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WebUtilBox.AddAttribute(this.htmlTr, "class", string.Format("dev-goods-item dev-goods-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
        //WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        ////WebUtilBox.AddAttribute(this.txtQty, "onchange", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        //WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.changeQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtUnitPriceDct, "onkeyup", string.Format("orderItemHelper.changeUnitPriceDct('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        if (!this.txtUnitPrice.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtUnitPrice, "onkeyup", string.Format("orderItemHelper.changeUnitPrice('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        }

        if (!this.txtDiscount.ReadOnly)
        {
            WebUtilBox.AddAttribute(this.txtDiscount, "onkeyup", string.Format("ValidateFloat1(this,value); orderItemHelper.checkDiscount($(this)); orderItemHelper.changeDiscount('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        }

        this.lblSubtotalQty.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidSubtotalQty.Value, DefVal.Int), string.Empty);
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
    public void SetInfo(ExtShippingOrderHelper.GoodsItemEditInfo_PNBase editInfo)
    {
        if (this.IsPostBack && !this.Visible) { return; }

        if (editInfo == null || string.IsNullOrWhiteSpace(editInfo.PartNo) || editInfo.DetItems.Count == 0)
        {
            this.Visible = false;
            return;
        }

        ////外銷訂單明細系統代號
        //this.SetPgAttr(PgAttrDefine.ExtOrderDetSId, editInfo.ExtOrderDetSId.Value);

        this.SetPgAttr(PgAttrDefine.SId, this.ClientID);

        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litModel.Text = editInfo.Model;
        this.litPartNo.Text = editInfo.PartNo;
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        ////this.txtQty.Text = ConvertLib.ToStr(editInfo.Qty, string.Empty);
        this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(editInfo.ListPrice, string.Empty));

        //小計數量
        this.hidSubtotalQty.Value = ConvertLib.ToStr(editInfo.DetItems.Sum(q => q.Qty), "0");

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

        foreach (var detItem in editInfo.DetItems)
        {
            var block = (ASP.include_client_ext_shipping_order_edit_2_goods_det_item_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit_2/goods_det_item_block.ascx");
            this._blockDetItemList.Add(block);
            this.phDetItems.Controls.Add(block);
            block.SetInfo(detItem, this.ClientID);

            ViewState[USER_CONTROL_LOAD_SEQ] += "[DetItem]";
        }
    }
    #endregion

    #region 取得輸入資訊
    public ExtShippingOrderHelper.GoodsItemEditInfo_PNBase GetInfo()
    {
        var editInfo = new ExtShippingOrderHelper.GoodsItemEditInfo_PNBase()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            Model = this.litModel.Text,
            PartNo = this.litPartNo.Text,
            //ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float),
            UnitPriceDct = ConvertLib.ToSingle(this.txtUnitPriceDct.Text, DefVal.Float),
            Discount = ConvertLib.ToSingle(this.txtDiscount.Text, DefVal.Float),
            PaidAmt = ConvertLib.ToSingle(this.hidPaidAmt.Value, DefVal.Float),
            Rmk = this.txtRmk.Text
        };

        foreach (var block in this._blockDetItemList)
        {
            editInfo.DetItems.Add(block.GetInfo());
        }

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