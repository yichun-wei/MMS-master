﻿using System;
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

public partial class include_client_ext_order_view_goods_item_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 設定資訊
    public void SetInfo(ExtOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litModel.Text = editInfo.Model;
        this.litPartNo.Text = editInfo.PartNo;
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");

        //修改前數量
        this.litBeforeTransQty.Text = ConvertLib.ToStr(editInfo.BeforeTransQty, "0");

        //修改量
        int transQty = ConvertLib.ToInt(editInfo.Qty, 0) - ConvertLib.ToInt(editInfo.BeforeTransQty, 0);
        if (transQty > 0)
        {
            this.litTransQty.Text = string.Format("<span class='blue font_bold'>+{0}</span>", transQty);
            WebUtilBox.AddAttribute(this.htmlTr, "class", "over");
        }
        else if (transQty < 0)
        {
            this.litTransQty.Text = string.Format("<span class='red font_bold'>{0}</span>", transQty);
            WebUtilBox.AddAttribute(this.htmlTr, "class", "lack");
        }

        this.litQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);

        if (editInfo.UnitPrice.HasValue)
        {
            this.litUnitPrice.Text = ConvertLib.ToAccounting(editInfo.UnitPrice, string.Empty);
        }
        else
        {
            this.litUnitPrice.Text = ConvertLib.ToAccounting(editInfo.ListPrice, string.Empty);
        }

        if (editInfo.Discount.HasValue)
        {
            this.litDiscount.Text = string.Format("{0}%", MathLib.Round(editInfo.Discount.Value * 100, 2));
        }
        else
        {
            this.litDiscount.Text = "無";
        }

        this.litPaidAmt.Text = ConvertLib.ToAccounting(editInfo.PaidAmt, string.Empty);

        this.lblRmk.Text = editInfo.Rmk;
        this.lblRmk.ToolTip = editInfo.Rmk;
    }
    #endregion
}