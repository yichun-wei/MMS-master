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

public partial class master_page_client_edit : System.Web.UI.MasterPage, IClientEditMaster
{
    #region 網頁屬性
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 目前登入的系統使用者資訊集合。
    /// </summary>
    public SysUserHelper.SysUserInfoSet ActorInfoSet { get; set; }
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
        #region 檢查是否已登入並初始化系統使用者資訊集合
        var actorInfo = SysUserHelper.CheckLoggedIn();
        if (actorInfo == null)
        {
            return false;
        }

        var actorRight = SysUserHelper.GetCurrentUserRight(actorInfo.StatusFlag);
        this.ActorInfoSet = new SysUserHelper.SysUserInfoSet(actorInfo, actorRight);
        #endregion

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
}
