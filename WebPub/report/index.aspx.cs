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

using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class report_index : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
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
        this.WebPg = new WebPg(this, true, OperPosition.GeneralClient);
        this.MainPage = (IClientMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.PageTitle = "報表查詢";

        //麵包屑
        List<string> breadcrumbs = new List<string>();
        breadcrumbs.Add(string.Format("<a href='{0}'>報表查詢</a>", QueryStringParsing.CurrentRelativeUri.OriginalString));
        this.ucBreadcrumbs.SetBreadcrumbs(breadcrumbs.ToArray());

        //範例: 我是登入人的帳號
        var acct = this.MainPage.ActorInfoSet.Info.Acct;
        // 2016.1.20 MICHELLE 傳給報表吃的參數 p_acct
        p_acct = acct;

        //範例: 我是登入人設定的內銷地區名稱陣列集合
        var distNames = this.MainPage.ActorInfoSet.DomDistInfos.Select(q => q.Name).ToArray();

        Returner returner = null;
        try
        {

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

    // 2016.1.20 MICHELLE 新增全域變數 p_acct
    public static string p_acct;
    // 2016.2.23 MICHELLE 新增全域變數 p_flag
    public static string p_flag;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TextBox1.Text = "";
            TextBox2.Text = "";
            CID.Text = "";
            DropDownList1.Items.Clear();
            DropDownList2.Items.Clear();
            DropDownList1.Items.Add(new ListItem("-- 請選擇報表分類 --", "0"));
            DropDownList1.DataSourceID = "SqlDataSource1";
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        TextBox1.Text = "";
        if (DropDownList1.SelectedValue != "0")
        {
            TextBox1.Text = "";
            TextBox1.Text = DropDownList1.SelectedValue;
            DropDownList2.Items.Clear();
            GetID(DropDownList1.SelectedValue);
            GetReport(CID.Text);
        }
        else
        {
            this.DropDownList2.Items.Clear();
        }
    }

    protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        ReportViewer1.Reset();
        TextBox2.Text = "";
        p_flag = "";
        TextBox2.Text = DropDownList2.SelectedValue;
        GetParm();
    }

    protected void GetID(string reportName)
    {
        CID.Text = "";
        SqlConnection Conn = new SqlConnection();
        //----上面已經事先寫好 using System.Web.Configuration ----
        Conn.ConnectionString = WebConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString;

        SqlDataReader dr = null;

        //==重點！！== 透過SQL指令解決！==
        SqlCommand cmd = new SqlCommand("SELECT [CategoryID] FROM [dbo].[XS_REPORT_CATEGORY] WHERE  [CategoryPath]='" + DropDownList1.SelectedValue + "'", Conn);

        //==== 以下程式，只放「執行期間」的指令！=====================
        try
        {
            Conn.Open();   //---- 這時候才連結DB

            dr = cmd.ExecuteReader();
            dr.Read();
            CID.Text = dr[0].ToString();
        }
        catch (Exception ex)
        {
            //---- 如果程式有錯誤或是例外狀況，將執行這一段
            Response.Write("<b>Error Message----  </b>" + ex.ToString() + "<HR/>");
        }
        finally
        {
            if (dr != null)
            {
                cmd.Cancel();
                //----關閉DataReader之前，一定要先「取消」SqlCommand

                dr.Close();
            }

            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
    }

    protected void GetReport(string categoryid)
    {
        //讀報表名稱
        SqlConnection Conn = new SqlConnection();
        //----上面已經事先寫好 using System.Web.Configuration ----
        Conn.ConnectionString = WebConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString;
        SqlDataReader dr2 = null;

        //==重點！！== 透過SQL指令解決！==
        SqlCommand cmd2 = new SqlCommand("SELECT  [ReportPath], [ReportDesc] FROM [dbo].[XS_REPORT_LIST] WHERE INACTIVE IS NULL AND CATEGORYID=" + Int32.Parse(CID.Text), Conn);
        try     //==== 以下程式，只放「執行期間」的指令！=====================
        {
            Conn.Open();   //---- 這時候才連結DB
            dr2 = cmd2.ExecuteReader();
            DropDownList2.DataTextField = "ReportDesc";  //==重點！！==
            DropDownList2.DataValueField = "ReportPath";
            DropDownList2.Items.Add(new ListItem("-- 請選擇報表名稱 --", "0"));
            DropDownList2.DataSource = dr2;
            DropDownList2.DataBind();
        }
        catch (Exception ex)
        {   //---- 如果程式有錯誤或是例外狀況，將執行這一段
            Response.Write("<b>Error Message----  </b>" + ex.ToString() + "<HR/>");
        }
        finally
        {
            if (dr2 != null)
            {
                cmd2.Cancel();
                //----關閉DataReader之前，一定要先「取消」SqlCommand

                dr2.Close();
            }

            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
    }
    protected void GetParm()
    {
        //報表是否需要傳帳號參數   
        SqlConnection Conn = new SqlConnection();
        Conn.ConnectionString = WebConfigurationManager.ConnectionStrings["ConnInfo"].ConnectionString;
        SqlDataReader dr2 = null;

        //抓取參數註記
        SqlCommand cmd2 = new SqlCommand(" SELECT  ISNULL(PARM_FLAG,'N') FROM [dbo].[XS_REPORT_LIST] WHERE INACTIVE IS NULL AND REPORTPATH='" + TextBox2.Text + "'", Conn);
        try
        {
            Conn.Open();
            dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                p_flag = dr2[0].ToString();
            }

        }
        catch (Exception ex)
        {
            Response.Write("<b>Error Message----  </b>" + ex.ToString() + "<HR/>");
        }
        finally
        {
            if (dr2 != null)
            {
                cmd2.Cancel();

                dr2.Close();
            }

            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
                Conn.Dispose();
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Microsoft.Reporting.WebForms.ReportParameter[] _params = new Microsoft.Reporting.WebForms.ReportParameter[1];

        if (DropDownList1.SelectedValue != "0" && DropDownList2.SelectedValue != "0")
        {
            // 2016.1.20 MICHELLE 傳給參數給報表 p_acct
            _params[0] = new Microsoft.Reporting.WebForms.ReportParameter("p_acct", p_acct); //參數名稱，傳帳號資料
            //_params[1] = new Microsoft.Reporting.WebForms.ReportParameter("p2", this.TextBox2.Text); //參數名稱
            //SetReportViewerAuth(ReportViewer1, "XS_ORDERAMOUNT", _params); //報表名稱

            // 2016.1.20 MICHELLE 客戶對帳報表要傳帳號參數
            // if (this.TextBox2.Text == "C02_CustomerBal" || this.TextBox2.Text == "C03_CustomerBalSum")
            //2016.2.23 MICHELLE 改成 判斷參數註記
            if (p_flag == "Y")
            {
                SetReportViewerAuth(ReportViewer1, TextBox1.Text + "/" + TextBox2.Text, _params); //報表名稱
            }
            else
            {
                SetReportViewerAuth(ReportViewer1, TextBox1.Text + "/" + TextBox2.Text);
            }
          
        }
    }

    public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        // local variable for network credential
        private string strUserName;
        private string strPassWord;
        private string strDomainName;
        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        {
            strUserName = UserName;
            strPassWord = PassWord;
            strDomainName = DomainName;
        }
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            // not use ImpersonationUser
            get { return null; }
        }
        public System.Net.ICredentials NetworkCredentials
        {
            // use NetworkCredentials
            get { return new System.Net.NetworkCredential(strUserName, strPassWord, strDomainName); }
        }
        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName, out string password, out string authority)
        {
            // not use FormsCredentials unless you have implements a custom autentication.
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }
    }
    //有參數的
    public void SetReportViewerAuth(Microsoft.Reporting.WebForms.ReportViewer sender, string ReportName, Microsoft.Reporting.WebForms.ReportParameter[] _params)
    {
        string strReportsServer = WebConfigurationManager.AppSettings["strReportsServer"]; //報表位置IP
        string strUserName = WebConfigurationManager.AppSettings["strUserName"];  //Windows驗證非SQL驗證
        string strPassword = WebConfigurationManager.AppSettings["strPassword"];

        Microsoft.Reporting.WebForms.IReportServerCredentials mycred = new CustomReportCredentials(strUserName, strPassword, strReportsServer);
        sender.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

        Uri reportUri = new Uri("http://" + strReportsServer + "/ReportServer");
        var _with1 = sender.ServerReport;
        _with1.ReportServerUrl = reportUri;
        _with1.ReportPath = "/" + ReportName;
        _with1.ReportServerCredentials = mycred;
        _with1.SetParameters(_params);
        //TextBox1.Text = ReportName;
        sender.ShowParameterPrompts = true;
        sender.Visible = true;
        sender.ZoomPercent = 75;
    }

    public void SetReportViewerAuth(Microsoft.Reporting.WebForms.ReportViewer sender, string ReportName)
    {
        string strReportsServer = WebConfigurationManager.AppSettings["strReportsServer"]; //報表位置IP
        string strUserName = WebConfigurationManager.AppSettings["strUserName"];  //Windows驗證非SQL驗證
        string strPassword = WebConfigurationManager.AppSettings["strPassword"];

        Microsoft.Reporting.WebForms.IReportServerCredentials mycred = new CustomReportCredentials(strUserName, strPassword, strReportsServer);
        sender.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

        Uri reportUri = new Uri("http://" + strReportsServer + "/ReportServer");
        var _with1 = sender.ServerReport;
        _with1.ReportServerUrl = reportUri;
        _with1.ReportPath = "/" + ReportName;
        _with1.ReportServerCredentials = mycred;
        // _with1.SetParameters(_params);
        //TextBox1.Text = ReportName;
        sender.ShowParameterPrompts = true;
        sender.Visible = true;
        sender.ZoomPercent = 75;
    }
}