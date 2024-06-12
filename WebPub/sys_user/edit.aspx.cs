using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class sys_user_edit : System.Web.UI.Page
{
    #region 網頁屬性
    string AUTH_CODE = "SYS_USER";
    string AUTH_NAME = "系統使用者";

    /// <summary>
    /// 主版。
    /// </summary>
    IClientEditMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 功能權限。
    /// </summary>
    FunctionRight FunctionRight { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    SysUser.Info OrigInfo { get; set; }
    #endregion

    ISystemId _sysUserSId;

    SimpleDataSet<string, AuthorityItems> _checkedAuthItems;

    class AuthItem
    {

    }

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
        this.WebPg = new WebPg(this, false, OperPosition.GeneralClient);
        this.MainPage = (IClientEditMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        //權限初始
        this.FunctionRight = this.MainPage.ActorInfoSet.UserRight.GetFunctionRight(this.AUTH_CODE);
        if (!this.FunctionRight.View)
        {
            Response.Redirect(SystemDefine.HomePageUrl);
            return false;
        }

        if (this.IsPostBack)
        {
            //編輯頁指定導向
            if (Session[SessionDefine.SystemBuffer] is Uri)
            {
                string redirectUrl = ((Uri)Session[SessionDefine.SystemBuffer]).ToString();
                Session.Remove(SessionDefine.SystemBuffer);
                Response.Redirect(redirectUrl);
                return false;
            }
        }

        this._sysUserSId = ConvertLib.ToSId(Request.QueryString["sid"]);

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "權限";

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                #region 內銷地區
                PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
                returner = entityPubCat.GetTopInfo(new PubCat.TopInfoConds(DefVal.SIds, (int)PubCat.UseTgtOpt.DomDist, DefVal.SId), Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = PubCat.Info.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        this.chklDomDistList.Items.Add(new ListItem(info.Name, info.SId.Value));
                    }
                }
                #endregion
            }

            if (this._sysUserSId != null)
            {
                #region 修改
                if (this.SetEditData(this._sysUserSId))
                {
                    this.PageTitle = string.Format("{1} ({0})", this.OrigInfo.Acct, this.OrigInfo.Name);

                    this.txtAcct.ReadOnly = this.OrigInfo.IsDef;
                    this.txtName.ReadOnly = this.txtAcct.ReadOnly;

                    if (SystemId.IsDefaultValue(this._sysUserSId))
                    {
                        //預設的系統管理員永遠為 full control
                        this.phUserRight.Visible = false;
                    }
                }
                else
                {
                    JSBuilder.ClosePage(this);
                    return false;
                }
                #endregion
            }
            else
            {
                #region 新增
                //新增
                this.PageTitle = "新增權限";
                #endregion
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

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
        this._checkedAuthItems = this.GetCheckedAuthItems();
        this.FillAuthItems();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        //權限操作檢查
        this.btnSend.Visible = this.btnSend.Visible ? this.FunctionRight.Maintain : this.btnSend.Visible;
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);

            returner = entitySysUser.GetInfo(new ISystemId[] { systemId }, IncludeScope.All);
            if (returner.IsCompletedAndContinue)
            {
                var info = SysUser.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                this.OrigInfo = info;

                this.txtAcct.Text = info.Acct;
                this.txtName.Text = info.Name;

                this.litPwdNote.Text = "<span class='note'>空值表示不異動</span>";

                this.txtEmail.Text = info.Email;
                this.txtDept.Text = info.Dept;

                WebUtilBox.SetListControlSelected(info.DomAuditPerms.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries), this.chklDomAuditPerms);
                WebUtilBox.SetListControlSelected(info.ExtAuditPerms.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries), this.chklExtAuditPerms);

                //異動記錄
                this.ucEditDataTransLog.Visible = true;
                this.ucEditDataTransLog.SetInfo(info.CSId, info.Cdt, info.MSId, info.Mdt);
            }
            else
            {
                return false;
            }

            #region 內銷地區
            RelTab entityRelTab = new RelTab(SystemDefine.ConnInfo);
            returner = entityRelTab.GetInfo(new RelTab.InfoConds(ConvertLib.ToInts((int)RelTab.RelCodeOpt.SysUser_DomDist), ConvertLib.ToSIds(this.OrigInfo.SId), DefVal.SIds, DefVal.Strs), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All, new string[] { "TGT_SID" });
            if (returner.IsCompletedAndContinue)
            {
                var infos = RelTab.Info.Binding(returner.DataSet.Tables[0]);
                WebUtilBox.SetListControlSelected(infos.Select(q => q.TgtSId.Value).ToArray(), this.chklDomDistList);
            }
            #endregion

            return true;
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region GetCheckedAuthItems
    SimpleDataSet<string, AuthorityItems> GetCheckedAuthItems()
    {
        var checkedItems = new SimpleDataSet<string, AuthorityItems>();

        if (string.IsNullOrEmpty(Request.Form["authItems"])) return checkedItems;

        string[] checkedItemsSplit = Request.Form["authItems"].Split(',');
        for (int i = 0; i < checkedItemsSplit.Length; i++)
        {
            string[] itemSplit = checkedItemsSplit[i].Split('|');

            SimpleData<string, AuthorityItems> item = checkedItems.FindByKey(itemSplit[0]);
            if (item == null)
            {
                item = checkedItems[checkedItems.Add(itemSplit[0], new AuthorityItems())];
            }

            switch (itemSplit[1])
            {
                case "VIEW":
                    item.Value.View = true;
                    break;
                case "MAINTAIN":
                    item.Value.Maintain = true;
                    break;
                case "DELETE":
                    item.Value.Delete = true;
                    break;
                case "BATCH":
                    item.Value.Batch = true;
                    break;
            }
        }

        return checkedItems;
    }
    #endregion

    #region FillAuthItems
    void FillAuthItems()
    {
        if (this._sysUserSId != null && SystemId.IsDefaultValue(this._sysUserSId))
        {
            //預設的系統使用者永遠擁有最大權限
            return;
        }

        SimpleDataSet<string, AuthorityItems> checkedItems = null;
        if (this.IsPostBack)
        {
            checkedItems = new SimpleDataSet<string, AuthorityItems>();
        }

        UserRight activeUserRight = new UserRight(this._sysUserSId, UserRight.Range.OnlyUser);

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(HttpContext.Current.Server.MapPath("~/App_Data/auth_treeview.xml"));

        TreeNodeCollection items = new TreeNodeCollection();
        XmlNode rootXmlNode = xmldoc.SelectSingleNode(string.Format("AuthTable/Items"));

        var builder = new StringBuilder();
        foreach (XmlNode node in rootXmlNode.ChildNodes)
        {
            string authCode = XmlLib.GetAttributeValue(node, "AuthCode");
            if (string.IsNullOrEmpty(authCode))
            {
                continue;
            }

            string controlView = string.Empty;
            string controlMaintain = string.Empty;
            string controlDelete = string.Empty;
            string controlBatch = string.Empty;

            var nodeFunctionRight = activeUserRight.GetFunctionRight(authCode);
            var authItem = this._checkedAuthItems.FindByKey(authCode);

            if (XmlLib.GetAttributeValue(node, "Opers").IndexOf("V") != -1)
            {
                bool checkedItem = false;
                if (!this.IsPostBack) { checkedItem = nodeFunctionRight.View; }
                else if (authItem != null) { checkedItem = authItem.Value.View; }

                controlView = string.Format("<label><input type='checkbox' id='{0}_VIEW' name='authItems' value='{0}|VIEW'{1} />查詢</label>", authCode, checkedItem ? " checked" : string.Empty);
            }

            if (XmlLib.GetAttributeValue(node, "Opers").IndexOf("M") != -1)
            {
                bool checkedItem = false;
                if (!this.IsPostBack) { checkedItem = nodeFunctionRight.Maintain; }
                else if (authItem != null) { checkedItem = authItem.Value.Maintain; }

                controlMaintain = string.Format("<label><input type='checkbox' id='{0}_MAINTAIN' name='authItems' value='{0}|MAINTAIN'{1} />維護</label>", authCode, checkedItem ? " checked" : string.Empty);
            }

            if (XmlLib.GetAttributeValue(node, "Opers").IndexOf("D") != -1)
            {
                bool checkedItem = false;
                if (!this.IsPostBack) { checkedItem = nodeFunctionRight.Delete; }
                else if (authItem != null) { checkedItem = authItem.Value.Delete; }

                controlDelete = string.Format("<label><input type='checkbox' id='{0}_DELETE' name='authItems' value='{0}|DELETE'{1} />刪除</label>", authCode, checkedItem ? " checked" : string.Empty);
            }

            if (XmlLib.GetAttributeValue(node, "Opers").IndexOf("B") != -1)
            {
                bool checkedItem = false;
                if (!this.IsPostBack) { checkedItem = nodeFunctionRight.Batch; }
                else if (authItem != null) { checkedItem = authItem.Value.Batch; }

                controlBatch = string.Format("<label><input type='checkbox' id='{0}_BATCH' name='authItems' value='{0}|BATCH'{1} />批次作業</label>", authCode, checkedItem ? " checked" : string.Empty);
            }

            if (string.IsNullOrEmpty(controlView) && string.IsNullOrEmpty(controlMaintain) && string.IsNullOrEmpty(controlDelete) && string.IsNullOrEmpty(controlBatch))
            {
                //沒有任何選擇
            }
            else
            {
                builder.AppendFormat("<li><span>{0}({1}{2}{3}{4})</span></li>", XmlLib.GetAttributeValue(node, "Name"), controlView, controlMaintain, controlDelete, controlBatch);

            }
        }

        this.litAuthItems.Text = builder.ToString();
    }
    #endregion

    #region MakeUserRight
    string MakeUserRight()
    {
        var checkedItems = this.GetCheckedAuthItems();

        if (checkedItems == null || checkedItems.Count == 0) return string.Empty;

        XmlLib xml = new XmlLib();
        xml.XmlDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/sys_user_right.xml"));
        XmlNode rootXmlNode = xml.XmlDocument.SelectSingleNode("UserRight/Function");
        for (int i = 0; i < checkedItems.Count; i++)
        {
            XmlNode node = xml.XmlDocument.SelectSingleNode(checkedItems[i].Key);
            if (node == null)
            {
                node = xml.CreateNode(rootXmlNode, checkedItems[i].Key);
            }

            XmlAttribute attribute = default(XmlAttribute);

            if (checkedItems[i].Value.View)
            {
                attribute = node.Attributes["View"];
                if (attribute == null)
                {
                    attribute = xml.XmlDocument.CreateAttribute("View");
                }
                attribute.Value = "True";
                node.Attributes.Append(attribute);
            }

            if (checkedItems[i].Value.Maintain)
            {
                attribute = node.Attributes["Maintain"];
                if (attribute == null)
                {
                    attribute = xml.XmlDocument.CreateAttribute("Maintain");
                }
                attribute.Value = "True";
                node.Attributes.Append(attribute);
            }

            if (checkedItems[i].Value.Delete)
            {
                attribute = node.Attributes["Delete"];
                if (attribute == null)
                {
                    attribute = xml.XmlDocument.CreateAttribute("Delete");
                }
                attribute.Value = "True";
                node.Attributes.Append(attribute);
            }

            if (checkedItems[i].Value.Batch)
            {
                attribute = node.Attributes["Batch"];
                if (attribute == null)
                {
                    attribute = xml.XmlDocument.CreateAttribute("Batch");
                }
                attribute.Value = "True";
                node.Attributes.Append(attribute);
            }
        }

        using (MemoryStream memory = new MemoryStream())
        {
            xml.XmlDocument.Save(memory);
            return Encoding.UTF8.GetString(memory.GetBuffer());
        }
    }
    #endregion

    #region btnSend_Click
    protected void btnSend_Click(object sender, EventArgs e)
    {
        #region 驗證檢查
        List<string> errMsgs = new List<string>();

        WebUtil.TrimTextBox(this.Form.Controls, false);

        if (string.IsNullOrEmpty(this.txtName.Text))
        {
            errMsgs.Add("請輸入「姓名」");
        }

        if (string.IsNullOrEmpty(this.txtAcct.Text))
        {
            errMsgs.Add("請輸入「帳號」");
        }

        if (this._sysUserSId == null && string.IsNullOrEmpty(this.txtPwd.Text))
        {
            errMsgs.Add("請輸入「密碼」");
        }

        if (!VerificationLib.IsEmail(this.txtEmail.Text))
        {
            errMsgs.Add("請輸入「電子信箱」(或格式不正確)");
        }

        if (errMsgs.Count != 0)
        {
            JSBuilder.AlertMessage(this, errMsgs.ToArray());
            return;
        }
        #endregion

        if (this._sysUserSId == null)
        {
            this.Add();
        }
        else
        {
            this.Modify();
        }
    }
    #endregion

    #region Add
    void Add()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);

                if (!entitySysUser.CheckEditModeForAccount(this._sysUserSId, this.txtAcct.Text, IncludeScope.All))
                {
                    JSBuilder.AlertMessage(this, "「帳號」已經存在");
                    return;
                }

                #region 市場範圍
                List<string> mktgRanges = new List<string>();
                string[] mktgRangeOfAuthCodes;

                mktgRangeOfAuthCodes = new string[] { "DOM_ORDER", "DOM_PG_ORDER", "DOM_DELIVERY_ORDER" };
                foreach (var authCode in mktgRangeOfAuthCodes)
                {
                    if (this._checkedAuthItems.FindByKey(authCode) != null)
                    {
                        mktgRanges.Add("1");
                        break;
                    }
                }

                mktgRangeOfAuthCodes = new string[] { "EXT_QUOTN", "EXT_ORDER", "EXT_PROD_ORDER", "EXT_DELIVERY_ORDER", "EXT_GOODS" };
                foreach (var authCode in mktgRangeOfAuthCodes)
                {
                    if (this._checkedAuthItems.FindByKey(authCode) != null)
                    {
                        mktgRanges.Add("2");
                        break;
                    }
                }
                #endregion

                var hashKey = GenerationLib.RandomCode(10, CharacterSetOptions.Numbers | CharacterSetOptions.UppercaseLetters);
                var input = new SysUser.Info()
                {
                    HashKey = hashKey,
                    Acct = this.txtAcct.Text,
                    Pwd = SysUserHelper.EncodingPassword(this.txtPwd.Text, hashKey),
                    Name = this.txtName.Text,
                    Email = this.txtEmail.Text,
                    Mobile = DefVal.Str,
                    Tel = DefVal.Str,
                    Dept = this.txtDept.Text,
                    MktgRange = mktgRanges.Count > 0 ? string.Format("-{0}-", string.Join("-", mktgRanges.ToArray())) : string.Empty,
                    DomAuditPerms = WebUtil.GetCheckedCheckBox(this.chklDomAuditPerms),
                    ExtAuditPerms = WebUtil.GetCheckedCheckBox(this.chklExtAuditPerms),
                    UserRight = this.MakeUserRight(),
                    Rmk = DefVal.Str
                };

                returner = entitySysUser.Add(actorSId, input.HashKey, input.Acct, input.Pwd, input.Name, input.Email, input.Mobile, input.Tel, input.Dept, input.MktgRange, input.DomAuditPerms, input.ExtAuditPerms, input.UserRight, input.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    string systemId = returner.DataSet.Tables["NEW_SID"].Rows[0]["SID"].ToString();

                    #region 內銷地區
                    RelTab entityRelTab = new RelTab(SystemDefine.ConnInfo);

                    if (true)
                    {
                        var relCode = (int)RelTab.RelCodeOpt.SysUser_DomDist;
                        ISystemId[] domDistSIds = ConvertLib.ToSIds(WebUtilBox.GetListControlSelected(this.chklDomDistList));
                        foreach (var domDistSId in domDistSIds)
                        {
                            entityRelTab.Add(actorSId, relCode, ConvertLib.ToSId(systemId), domDistSId, DefVal.Str, DefVal.Str, SystemDefine.DefSortVal);
                        }
                    }
                    #endregion

                    JSBuilder.OpenerPostBack(this);

                    //異動記錄
                    string dataTitle = string.Format("{0}", this.txtAcct.Text);
                    using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.SYS_USER, new SystemId(systemId), DefVal.Int, "A", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, string.Empty))
                    {
                    }

                    transaction.Complete();

                    //回到列表
                    JSBuilder.ClosePage(this);
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion

    #region Modify
    void Modify()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        Returner returner = null;
        try
        {
            ISystemId actorSId = this.MainPage.ActorInfoSet.Info.SId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);

                if (!entitySysUser.CheckEditModeForAccount(this._sysUserSId, this.txtAcct.Text, IncludeScope.All))
                {
                    JSBuilder.AlertMessage(this, "「帳號」已經存在");
                    return;
                }

                //異動記錄
                string dataTitle = this.OrigInfo.Acct;
                using (Returner returnerDataTransLog = new DataTransLog(SystemDefine.ConnInfo).Add(actorSId, DBTableDefine.SYS_USER, this._sysUserSId, DefVal.Int, "U", this.AUTH_CODE, this.AUTH_NAME, dataTitle, WebUtilBox.GetUserHostAddress(), (int)OperPosition.GeneralClient, CustJson.SerializeObject(this.OrigInfo)))
                {
                }

                #region 市場範圍
                List<string> mktgRanges = new List<string>();
                string[] mktgRangeOfAuthCodes;

                mktgRangeOfAuthCodes = new string[] { "DOM_ORDER", "DOM_PG_ORDER", "DOM_DELIVERY_ORDER" };
                foreach (var authCode in mktgRangeOfAuthCodes)
                {
                    if (this._checkedAuthItems.FindByKey(authCode) != null)
                    {
                        mktgRanges.Add("1");
                        break;
                    }
                }

                mktgRangeOfAuthCodes = new string[] { "EXT_QUOTN", "EXT_ORDER", "EXT_PROD_ORDER", "EXT_DELIVERY_ORDER", "EXT_GOODS" };
                foreach (var authCode in mktgRangeOfAuthCodes)
                {
                    if (this._checkedAuthItems.FindByKey(authCode) != null)
                    {
                        mktgRanges.Add("2");
                        break;
                    }
                }
                #endregion

                var input = new SysUser.Info()
                {
                    Acct = this.txtAcct.Text,
                    Pwd = SysUserHelper.EncodingPassword(this.txtPwd.Text, this.OrigInfo.HashKey),
                    Name = this.txtName.Text,
                    Email = this.txtEmail.Text,
                    Mobile = DefVal.Str,
                    Tel = DefVal.Str,
                    Dept = this.txtDept.Text,
                    MktgRange = mktgRanges.Count > 0 ? string.Format("-{0}-", string.Join("-", mktgRanges.ToArray())) : string.Empty,
                    DomAuditPerms = WebUtil.GetCheckedCheckBox(this.chklDomAuditPerms),
                    ExtAuditPerms = WebUtil.GetCheckedCheckBox(this.chklExtAuditPerms),
                    UserRight = this.MakeUserRight(),
                    Rmk = this.OrigInfo.Rmk
                };

                returner = entitySysUser.Modify(actorSId, this._sysUserSId, input.Acct, input.Pwd, input.Name, input.Email, input.Mobile, input.Tel, input.Dept, input.MktgRange, input.DomAuditPerms, input.ExtAuditPerms, input.UserRight, input.Rmk);
                if (returner.IsCompletedAndContinue)
                {
                    #region 內銷地區
                    RelTab entityRelTab = new RelTab(SystemDefine.ConnInfo);

                    if (true)
                    {
                        var relCode = (int)RelTab.RelCodeOpt.SysUser_DomDist;

                        entityRelTab.DeleteByUseSId(relCode, ConvertLib.ToSIds(this._sysUserSId));

                        ISystemId[] domDistSIds = ConvertLib.ToSIds(WebUtilBox.GetListControlSelected(this.chklDomDistList));
                        foreach (var domDistSId in domDistSIds)
                        {
                            entityRelTab.Add(actorSId, relCode, this._sysUserSId, domDistSId, DefVal.Str, DefVal.Str, SystemDefine.DefSortVal);
                        }
                    }
                    #endregion

                    transaction.Complete();

                    JSBuilder.OpenerPostBack(this);

                    //回到列表
                    JSBuilder.ClosePage(this);
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}