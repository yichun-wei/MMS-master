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

public partial class include_client_dom_pg_order_view_goods_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetInfo(string title, PGOrderDet.InfoView[] detInfos)
    {
        if (detInfos == null || detInfos.Length == 0)
        {
            this.Visible = false;
            return;
        }

        this.litTitle.Text = title;

        var firstDetInfo = detInfos[0];
        if (firstDetInfo.Source == 2)
        {
            //設定專案報價錨點.
            this.litAnchor.Text = string.Format("<a name='GoodsBlock_{0}'></a>", firstDetInfo.QuoteNumber);
            //顯示「最大值」
            this.phMaxQty.Visible = true;
        }

        foreach (var detInfo in detInfos)
        {
            var block = (ASP.include_client_dom_pg_order_view_goods_item_block_ascx)this.LoadControl("~/include/client/dom/pg_order/view/goods_item_block.ascx");
            this.phGoodsItemList.Controls.Add(block);
            block.SetInfo(detInfo);
        }
    }
}