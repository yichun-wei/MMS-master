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

public partial class include_client_ext_shipping_order_edit_2_goods_det_item_block : System.Web.UI.UserControl
{
    #region 網頁屬性
    /// <summary>
    /// 屬性常數定義。
    /// </summary>
    struct PgAttrDefine
    {
        /// <summary>
        /// 外銷出貨單明細系統代號。
        /// </summary>
        public const string SId = "SId";
        /// <summary>
        /// 外銷訂單明細系統代號。
        /// </summary>
        public const string ExtOrderDetSId = "ExtOrderDetSId";
        /// <summary>
        /// 報價單編號。
        /// </summary>
        public const string OdrNo = "OdrNo";
        /// <summary>
        /// 上層區塊識別 ID。
        /// </summary>
        public const string ParentBlockId = "ParentBlockId";
    }

    string GetPgAttr(string key)
    {
        return this.divDetItemBlock.Attributes[key];
    }

    void SetPgAttr(string key, string value)
    {
        this.divDetItemBlock.Attributes[key] = value;
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SystemId.MinValue.IsSystemId(this.GetPgAttr(PgAttrDefine.SId)))
        {
            this.SetPgAttr(PgAttrDefine.SId, new SystemId().Value);
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WebUtilBox.AddAttribute(this.divDetItemBlock, "class", string.Format("dev-goods-det-item dev-goods-det-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        //WebUtilBox.AddAttribute(this.txtQty, "onchange", string.Format("orderItemHelper.checkMaxQty('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        WebUtilBox.AddAttribute(this.txtQty, "onkeyup", string.Format("orderItemHelper.changeQty('{0}');", this.GetPgAttr(PgAttrDefine.ParentBlockId)));
    }

    public void SetInfo(ExtShippingOrderHelper.GoodsItemDetEditInfo_PNBase editInfo, string parentBlockId)
    {
        //上層區塊識別 ID
        this.SetPgAttr(PgAttrDefine.ParentBlockId, parentBlockId);

        //外銷訂單明細系統代號
        this.SetPgAttr(PgAttrDefine.ExtOrderDetSId, editInfo.ExtOrderDetSId.Value);
        //報價單編號
        this.SetPgAttr(PgAttrDefine.OdrNo, editInfo.OdrNo);
        
        if (editInfo.SId != null)
        {
            this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        }

        this.litOdrNo.Text = editInfo.OdrNo;
        this.txtQty.Text = ConvertLib.ToStr(editInfo.Qty, string.Empty);

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

    #region 取得輸入資訊
    public ExtShippingOrderHelper.GoodsItemDetEditInfo_PNBase GetInfo()
    {
        return new ExtShippingOrderHelper.GoodsItemDetEditInfo_PNBase()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            ExtOrderDetSId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.ExtOrderDetSId)),
            OdrNo = this.GetPgAttr(PgAttrDefine.OdrNo),
            Qty = ConvertLib.ToInt(this.txtQty.Text, DefVal.Int),
            MaxQty = ConvertLib.ToInt(this.lblMaxQty.Text, 0)
        };
    }
    #endregion
}