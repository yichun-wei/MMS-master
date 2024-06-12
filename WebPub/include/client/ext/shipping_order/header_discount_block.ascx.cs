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

public partial class include_client_ext_shipping_order_header_discount_block : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public long DiscountId
    {
        get { return Convert.ToInt64(this.hidDiscountId.Value); }
        set { this.hidDiscountId.Value = value.ToString(); }
    }

    public string Name
    {
        get { return this.chkSel.Text; }
        set { this.chkSel.Text = value; }
    }

    public bool Seled
    {
        get { return this.chkSel.Checked; }
        set { this.chkSel.Checked = value; }
    }

    public string Discount
    {
        get { return this.txtDiscount.Text; }
        set { this.txtDiscount.Text = value; }
    }
}