using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Returner returner = null;
        try
        {
            if (SysUserHelper.IsLoggedIn)
            {
                LoginLog entityLoginLog = new LoginLog(SystemDefine.ConnInfo);

                //更新最後活動時間
                entityLoginLog.UpdateLastActDT(SysUserHelper.CurrentSysUserLoginLogSId);
                //更新登入狀態
                entityLoginLog.UpdateLoginState(SysUserHelper.CurrentSysUserLoginLogSId, true);
            }

            if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Session)
            {
            }
            else
            {
                CookiesUtil.Remove(CookiesDefine.SysUserSId);
            }
            CookiesUtil.Remove(CookiesDefine.SysUserLoginLogSId);
            CookiesUtil.Remove(CookiesDefine.SysUserStatusFlag);
            Session.Clear();
            Session.Abandon();
            Response.Redirect(SystemDefine.CltLoginUrl, false);
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
}