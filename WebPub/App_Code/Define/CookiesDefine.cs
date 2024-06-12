
namespace Seec.Marketing
{
    /// <summary>
    /// 定義 Cookies 的對應名稱操作。
    /// </summary>
    public static class CookiesDefine
    {
        /// <summary>
        /// 取得定義系統使用者登入後的管理者系統代號 Cookies 記錄名稱。
        /// </summary>
        public const string SysUserSId = "SeecMktg::SysUserSId";
        /// <summary>
        /// 取得定義系統使用者登入後的登入記錄系統代號 Cookies 記錄名稱。
        /// </summary>
        public const string SysUserLoginLogSId = "SeecMktg::SysUserLoginLogSId";
        /// <summary>
        /// 取得定義系統使用者狀態旗標 Cookies 記錄名稱。
        /// </summary>
        public const string SysUserStatusFlag = "SeecMktg::SysUserStatusFlag";

        /// <summary>
        /// 以指定名稱檢查 Cookie 是否為有效的值。
        /// </summary>
        /// <param name="name">Cookie 名稱。</param>
        public static bool IsValid(string name)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            return context.Request.Cookies[name] != null && !string.IsNullOrEmpty(context.Request.Cookies[name].Value);
        }
    }
}