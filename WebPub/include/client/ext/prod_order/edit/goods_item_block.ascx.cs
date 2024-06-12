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

public partial class include_client_ext_prod_order_edit_goods_item_block : System.Web.UI.UserControl
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
    public void SetInfo(ExtProdOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        //商品來源（1:一般品項 2:手動新增）
        this.SetPgAttr(PgAttrDefine.GoodsSrc, editInfo.Source.ToString());

        this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litOrgCode.Text = editInfo.OrgCode;
        this.litPartNo.Text = editInfo.PartNo;
        this.lblQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);
        this.litCumProdQty.Text = ConvertLib.ToStr(editInfo.CumProdQty, string.Empty);
        this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        this.txtProdQty.Text = ConvertLib.ToStr(editInfo.ProdQty, "0");
        this.txtEstFpmsDate.Text = ConvertLib.ToStr(editInfo.EstFpmsDate, string.Empty, "yyyy-MM-dd");

        this.lblRmk.Text = editInfo.Rmk;
    }
    #endregion

    #region 取得輸入資訊
    public ExtProdOrderHelper.GoodsItemEditInfo GetInfo()
    {
        return new ExtProdOrderHelper.GoodsItemEditInfo()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            Source = Convert.ToInt32(this.GetPgAttr(PgAttrDefine.GoodsSrc)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            OrgCode = this.litOrgCode.Text,
            PartNo = this.litPartNo.Text,
            ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.lblQty.Text, DefVal.Int),
            CumProdQty = ConvertLib.ToInt(this.litCumProdQty.Text, DefVal.Int),
            ProdQty = ConvertLib.ToInt(this.txtProdQty.Text, DefVal.Int),
            EstFpmsDate = ConvertLib.ToDateTime(this.txtEstFpmsDate.Text, DefVal.DT),
            Rmk = this.lblRmk.Text
        };
    }
    #endregion
}