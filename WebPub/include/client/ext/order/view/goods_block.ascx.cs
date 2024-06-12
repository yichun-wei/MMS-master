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

public partial class include_client_ext_order_view_goods_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 區塊標題。
    /// </summary>
    public string Title
    {
        get { return this.litTitle.Text; }
        set { this.litTitle.Text = value; }
    }

    #region 設定資訊
    public void SetInfo(ExtOrderHelper.GoodsEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.lblSubtotal.Text = ConvertLib.ToAccounting(editInfo.Items.Sum(q => ConvertLib.ToSingle(q.PaidAmt, 0)), string.Empty);

        #region 初始品項清單
        if (editInfo.Items != null && editInfo.Items.Count > 0)
        {
            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_ext_order_view_goods_item_block_ascx)this.LoadControl("~/include/client/ext/order/view/goods_item_block.ascx");
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);
            }
        }
        #endregion
    }
    #endregion
}