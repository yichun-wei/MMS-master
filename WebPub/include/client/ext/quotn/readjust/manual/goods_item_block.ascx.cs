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

public partial class include_client_ext_quotn_readjust_manual_goods_item_block : System.Web.UI.UserControl
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
        /// 商品來源（1:一般品項 2:手動新增）。
        /// </summary>
        public const string GoodsSrc = "GoodsSrc";
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

        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.changeQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

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
    public void SetInfo(ExtOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        //商品來源（1:一般品項 2:手動新增）
        this.SetPgAttr(PgAttrDefine.GoodsSrc, "2");

        if (editInfo.SId != null)
        {
            this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        }
        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.txtModel.Text = editInfo.Model;
        //this.litPartNo.Text = editInfo.PartNo;
        this.txtQty.Text = ConvertLib.ToStr(editInfo.Qty, string.Empty);
        this.txtUnitPrice.Text = ConvertLib.ToStr(editInfo.UnitPrice, string.Empty);
        if (editInfo.Discount.HasValue)
        {
            this.txtDiscount.Text = MathLib.Round(editInfo.Discount.Value * 100, 2).ToString();
        }
        else
        {
            //手動輸入商品沒有牌價
            //var unitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float);
            //if (editInfo.ListPrice.HasValue && unitPrice.HasValue)
            //{
            //    this.txtDiscount.Text = MathLib.Round(unitPrice.Value / editInfo.ListPrice.Value * 100, 2).ToString();
            //}
        }

        this.hidPaidAmt.Value = ConvertLib.ToStr(editInfo.PaidAmt, string.Empty);

        this.txtRmk.Text = editInfo.Rmk;

        //手動輸入商品沒有牌價
        //if (editInfo.ListPrice == null)
        //{
        //    //若料號比對不到價格, 要秀出錯誤訊息提示
        //    WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
        //}
    }
    #endregion

    #region 取得輸入資訊
    public ExtOrderHelper.GoodsItemEditInfo GetInfo()
    {
        return new ExtOrderHelper.GoodsItemEditInfo()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            Source = Convert.ToInt32(this.GetPgAttr(PgAttrDefine.GoodsSrc)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            Model = this.txtModel.Text,
            //PartNo = this.litPartNo.Text,
            //ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.txtQty.Text, DefVal.Int),
            //ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.txtUnitPrice.Text, DefVal.Float),
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