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
using Seec.Marketing.NetTalk.WebService.Client;

public partial class include_client_dom_order_view_goods_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 設定資訊
    public void SetInfo(DomOrderHelper.GoodsEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litTitle.Text = editInfo.Title;
        //原寫法
        //this.lblSubtotal.Text = ConvertLib.ToAccounting(editInfo.Items.Sum(q => ConvertLib.ToSingle(q.PaidAmt, 0)), string.Empty);
       
        #region 2016.7.28 新數字顯示寫法-By米雪 "品項區塊的合計金額"
        this.lblSubtotal.Text = string.Format("{0:N2}", (double)Math.Round(editInfo.Items.Sum(q => q.PaidAmt.Value), 2));
        #endregion
        if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber))
        {
            //設定專案報價錨點.
            this.litAnchor.Text = string.Format("<a name='GoodsBlock_{0}'></a>", editInfo.QuoteNumber);
        }

        #region 初始品項清單
        if (editInfo.Items != null && editInfo.Items.Count > 0)
        {
            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_dom_order_view_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/view/goods_item_block.ascx");
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);
            }
        }
        #endregion
    }
    #endregion
}