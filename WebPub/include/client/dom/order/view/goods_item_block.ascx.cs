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

public partial class include_client_dom_order_view_goods_item_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 設定資訊
    public void SetInfo(DomOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litPartNo.Text = editInfo.PartNo;

        //改為取得所有倉庫的在手量
        //this.lblErpOnHand.Text = ConvertLib.ToAccounting(editInfo.ErpOnHand, "0");
        this.lblErpOnHand.Text = string.Join("<br />", editInfo.ErpWhseOnHands.Select(q => string.Format("{0}：{1}", q.Whse, ConvertLib.ToAccounting(q.ErpOnHand, "0"))).ToArray());
    
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

        if (ConvertLib.ToInt(editInfo.ErpOnHand, 0) < ConvertLib.ToInt(editInfo.Qty, 0))
        {
            //若在手量小於數量, 要秀出錯誤訊息提示
            WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
        }

        //若為備貨單品項或專案報價品項, 則顯示「數值最大值」
        this.phMaxQtyContainer.Visible = editInfo.PGOrderDetSId != null || (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId));

        if (editInfo.PGOrderDetSId != null && !string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            #region 來自於備貨單的專案報價品項
            this.litSource.Text = string.Format("備貨單 {0}", editInfo.PGOrderOdrNo);

            //若為備貨單的專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.litMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.litMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }
            #endregion
        }
        else if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber) && !string.IsNullOrWhiteSpace(editInfo.QuoteItemId))
        {
            #region 直接來自於專案報價品項
            //若為專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = ProjQuoteHelper.GetMaxQty(editInfo.QuoteNumber, editInfo.QuoteItemId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.litMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.litMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }
            #endregion
        }
        else if (editInfo.PGOrderDetSId != null)
        {
            #region 來自於備貨單一般品項
            this.litSource.Text = string.Format("備貨單 {0}", editInfo.PGOrderOdrNo);

            //若為備貨單的專案報價品項, 則顯示「數值最大值」
            if (editInfo.MaxQty == 0)
            {
                int maxQty = PGOrderHelper.GetPGOrderDetMaxQty(editInfo.PGOrderDetSId) + ConvertLib.ToInt(editInfo.Qty, 0);
                this.litMaxQty.Text = maxQty.ToString();
            }
            else
            {
                //進來前已取過, 就毋須再取一次.
                this.litMaxQty.Text = (editInfo.MaxQty + ConvertLib.ToInt(editInfo.Qty, 0)).ToString();
            }
            #endregion
        }
        else
        {
            //一般品項
            this.phMaxQtyContainer.Visible = false;
        }
    }
    #endregion
}