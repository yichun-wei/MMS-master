using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using EzCoding;
using EzCoding.Web.UI;
using Seec.Marketing;

public partial class common_verify_code : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ////移除容易混肴的數字和字母
        //string codeRange = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        //僅使用數字
        string codeRange = "0123456789";

        MemoryStream memory;
        string code = WebUtilBox.GenImageVerifyCode(4, 200, 50, 20, 40, 1000, Color.Gray, Brushes.White, codeRange, FontStyle.Bold | FontStyle.Italic, new string[] { "Arial", "Tahoma", "Comic Sans MS" }, new int[] { 24, 28, 32 }, ImageFormat.Png, out memory);
        Session[SessionDefine.ImageVerifyCode] = code;

        Response.ClearContent();
        Response.ContentType = "image/png";
        Response.BinaryWrite(memory.ToArray());

        memory.Dispose();
        Response.End();
    }
}