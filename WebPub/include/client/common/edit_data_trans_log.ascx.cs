using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EzCoding;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class include_client_common_edit_data_trans_log : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 建立人。
    /// </summary>
    public string CreatorName
    {
        get { return this.litCreatorName.Text; }
        set { this.litCreatorName.Text = value; }
    }

    /// <summary>
    /// 建立時間。
    /// </summary>
    public string CreateDateTime
    {
        get { return this.litCreateDT.Text; }
        set { this.litCreateDT.Text = value; }
    }

    /// <summary>
    /// 最後修改人。
    /// </summary>
    public string LastModifierName
    {
        get { return this.litLastModifierName.Text; }
        set { this.litLastModifierName.Text = value; }
    }

    /// <summary>
    /// 最後修改時間。
    /// </summary>
    public string LastModifyDateTime
    {
        get { return this.litLastModifyDT.Text; }
        set { this.litLastModifyDT.Text = value; }
    }

    /// <summary>
    /// 設定資訊。
    /// </summary>
    /// <param name="creatorSId">建立人系統代號。</param>
    /// <param name="createDT">建立日期時間。</param>
    /// <param name="modifierSId">修改人系統代號。</param>
    /// <param name="modifyDT">修改日期時間。</param>
    public void SetInfo(ISystemId creatorSId, DateTime createDT, ISystemId modifierSId, DateTime modifyDT)
    {
        if (creatorSId == null || modifierSId == null)
        {
            return;
        }

        Returner returner = null;
        try
        {
            //異動記錄-建立
            SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);
            this.CreateDateTime = createDT.ToString("yyyy-MM-dd HH:mm:ss");
            returner = entitySysUser.GetInfo(new ISystemId[] { creatorSId }, IncludeScope.All);
            if (returner.IsCompletedAndContinue)
            {
                var row = returner.DataSet.Tables[0].Rows[0];
                this.CreatorName = string.Format("{0} ({1})", row["NAME"], row["ACCT"]);
            }

            //異動記錄-修改
            if (modifyDT != createDT)
            {
                this.LastModifyDateTime = modifyDT.ToString("yyyy-MM-dd HH:mm:ss");
                returner = entitySysUser.GetInfo(new ISystemId[] { modifierSId }, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var row = returner.DataSet.Tables[0].Rows[0];
                    this.LastModifierName = string.Format("{0} ({1})", row["NAME"], row["ACCT"]);
                }

                if (string.IsNullOrEmpty(this.LastModifierName.Replace("&nbsp;", string.Empty)))
                {
                    this.LastModifierName = "Owner";
                }
            }

            if (string.IsNullOrEmpty(this.CreatorName.Replace("&nbsp;", string.Empty)))
            {
                this.CreatorName = "Owner";
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
}