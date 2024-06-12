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

public partial class include_client_dom_pg_order_view_goods_item_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetInfo(PGOrderDet.InfoView detInfo)
    {
        if (detInfo == null || string.IsNullOrWhiteSpace(detInfo.PartNo))
        {
            this.Visible = false;
            return;
        }

        this.litSeqNo.Text = detInfo.Sort.ToString();
        this.litSummary.Text = detInfo.Summary;
        this.litPartNo.Text = detInfo.PartNo;
        this.litQty.Text = ConvertLib.ToStr(detInfo.Qty, string.Empty);
        
        this.lblRmk.Text = detInfo.Rmk;
        this.lblRmk.ToolTip = detInfo.Rmk;

        if (!string.IsNullOrWhiteSpace(detInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(detInfo.QuoteItemId))
        {
            this.phMaxQty.Visible = true;

            //若為專案報價品項, 則顯示「數值最大值」
            int maxQty = ProjQuoteHelper.GetMaxQty(detInfo.QuoteNumber, detInfo.QuoteItemId) + detInfo.Qty;
            this.litMaxQty.Text = maxQty.ToString();
        }
    }
}