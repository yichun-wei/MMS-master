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

public partial class include_client_ext_prod_order_view_goods_item_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 設定資訊
    public void SetInfo(ExtProdOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litOrgCode.Text = editInfo.OrgCode;
        this.litPartNo.Text = editInfo.PartNo;
        this.lblQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);
        this.litCumProdQty.Text = ConvertLib.ToStr(editInfo.CumProdQty, string.Empty);
        this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        this.litProdQty.Text = ConvertLib.ToStr(editInfo.ProdQty, "0");
        this.litEstFpmsDate.Text = ConvertLib.ToStr(editInfo.EstFpmsDate, string.Empty, "yyyy-MM-dd");

        this.lblRmk.Text = editInfo.Rmk;
    }
    #endregion
}