using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Globalization;

using EzCoding;
using EzCoding.DB;
using EzCoding.Web.UI;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class include_common_data_sorting : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string buff = Request.Form["hidSortingBuff"];
    }

    public DataSortingBuff[] GetReSortList()
    {
        string buff = Request.Form["hidSortingBuff"];

        if (string.IsNullOrEmpty(buff))
        {
            return null;
        }

        return CustJson.DeserializeObject<DataSortingBuff[]>(buff).Where(q => SystemId.MinValue.IsSystemId(q.SId) && VerificationLib.IsNumber(q.NewVal) && !q.OldVal.Equals(q.NewVal)).ToArray();
    }
}