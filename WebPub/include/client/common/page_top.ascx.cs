using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class include_client_common_page_top : System.Web.UI.UserControl
{
    #region 網頁屬性
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }
    #endregion

    #region 網頁初始的一連貫操作
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.InitVital()) { return; }
        if (!this.InitPage()) { return; }
        if (!this.LoadIncludePage()) { return; }

        this.HasInitial = true;
    }

    #region 初始化的必要操作
    /// <summary>
    /// 初始化的必要操作。
    /// </summary>
    private bool InitVital()
    {
        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        return true;
    }
    #endregion

    #region 載入使用者控制項
    /// <summary>
    /// 載入使用者控制項。
    /// </summary>
    private bool LoadIncludePage()
    {
        return true;
    }
    #endregion
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetInfo(SysUserHelper.SysUserInfoSet actorInfoSet)
    {
        if (actorInfoSet == null)
        {
            return;
        }

        this.litActor.Text = string.Format("{1} ({0})", actorInfoSet.Info.Acct, actorInfoSet.Info.Name);

        this.InitMenu(actorInfoSet);
    }

    void InitMenu(SysUserHelper.SysUserInfoSet actorInfoSet)
    {
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(HttpContext.Current.Server.MapPath("~/App_Data/client_menu.xml"));

        JQueryMenu jMenu = new JQueryMenu();
        jMenu.Items.Class = "top_menu";

        MainManu authMenu;
        if (actorInfoSet.UserRight.IsFullControl)
        {
            //完全控制
            authMenu = new MainManu(jMenu, xmldoc, SystemDefine.WebSiteRoot, true);
        }
        else
        {
            authMenu = new MainManu(jMenu, xmldoc, SystemDefine.WebSiteRoot, actorInfoSet.UserRight.GetFunctionRightByCode('V'));
        }

        authMenu.Fill();

        this.litMenu.Text = jMenu.ToHtml();
    }
}