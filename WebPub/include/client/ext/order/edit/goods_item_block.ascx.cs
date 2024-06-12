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
using Seec.Marketing.Erp;

public partial class include_client_ext_order_edit_goods_item_block : System.Web.UI.UserControl
{
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WebUtilBox.AddAttribute(this.htmlTr, "class", string.Format("dev-goods-item dev-goods-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
    }

    #region 設定資訊
    public void SetInfo(ExtOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null || string.IsNullOrWhiteSpace(editInfo.PartNo))
        {
            this.Visible = false;
            return;
        }

        //商品來源（1:一般品項 2:手動新增）
        this.SetPgAttr(PgAttrDefine.GoodsSrc, "1");
        this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);

        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litModel.Text = editInfo.Model;
        this.litPartNo.Text = editInfo.PartNo;
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        this.lblQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);
        this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(editInfo.ListPrice, string.Empty));

        if (editInfo.UnitPrice.HasValue)
        {
            this.lblUnitPrice.Text = ConvertLib.ToAccounting(editInfo.UnitPrice, string.Empty);
        }
        else
        {
            this.lblUnitPrice.Text = ConvertLib.ToAccounting(editInfo.ListPrice, string.Empty);
        }

        if (editInfo.Discount.HasValue)
        {
            this.lblDiscount.Text = string.Format("{0}", MathLib.Round(editInfo.Discount.Value * 100, 2));
        }
        else
        {
            this.lblDiscount.Text = "100";
        }

        this.lblPaidAmt.Text = ConvertLib.ToAccounting(editInfo.PaidAmt, string.Empty);

        this.lblRmk.Text = editInfo.Rmk;
        this.lblRmk.ToolTip = editInfo.Rmk;

        //if (ConvertLib.ToInt(editInfo.ErpOnHand, 0) < ConvertLib.ToInt(editInfo.Qty, 0))
        //{
        //    //若在手量小於數量, 要秀出錯誤訊息提示
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
            Model = this.litModel.Text,
            PartNo = this.litPartNo.Text,
            //ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.lblQty.Text, DefVal.Int),
            ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.lblUnitPrice.Text, DefVal.Float),
            Discount = ConvertLib.ToSingle(this.lblDiscount.Text, DefVal.Float),
            PaidAmt = ConvertLib.ToSingle(this.lblPaidAmt.Text, DefVal.Float),
            Rmk = this.lblRmk.Text
        };
    }
    #endregion
}