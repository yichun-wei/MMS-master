using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Seec.Marketing;

public partial class include_client_common_breadcrumbs : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetBreadcrumbs(string[] items)
    {
        var builder = new StringBuilder();

        if (items.Length == 0)
        {
            builder.Append(string.Format("<li class='last'><a href='{0}'>首頁</a></li>", SystemDefine.HomePageUrl));
        }
        else
        {
            builder.Append(string.Format("<li><a href='{0}'>首頁</a></li>", SystemDefine.HomePageUrl));
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (i == items.Length - 1)
            {
                builder.Append(string.Format("<li class='last'>{0}</li>", items[i]));
            }
            else
            {
                builder.Append(string.Format("<li>{0}</li>", items[i]));
            }
        }
        this.litBreadcrumbs.Text = builder.ToString();
    }
}