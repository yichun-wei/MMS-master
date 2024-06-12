
namespace Seec.Marketing
{
    /// <summary>
    /// SessionDefine 的摘要描述
    /// </summary>
    public class SessionDefine
    {
        /// <summary>
        /// 取得定義系統資料緩衝相關的 Session 記錄名稱。
        /// </summary>
        public const string SystemBuffer = "SeecMktg::SystemBuffer";
        /// <summary>
        /// 取得定義系統使用者登入後的系統代號 Session 記錄名稱。
        /// for 系統階段「SystemPhase」為「Production=11」時。
        /// </summary>
        public const string SysUserSId = "SeecMktg::SysUserSId";
        /// <summary>
        /// 取得定義使用在一般使用者界面當操作某一網頁卻因沒有權限使用時，在重新取得權限後，直接返回目的頁的 Session 記錄名稱。
        /// </summary>
        public const string ClientSrcPage = "SeecMktg::ClientSrcPage";
        /// <summary>
        /// 取得定義使用在系統管理界面當操作某一網頁卻因沒有權限使用時，在重新取得權限後，直接返回目的頁的 Session 記錄名稱。
        /// </summary>
        public const string SysMgtSrcPage = "SeecMktg::SysMgtSrcPage";
        /// <summary>
        /// 取得定義使用在系統使用者權限的 Session 記錄名稱。
        /// </summary>
        public const string UserRight = "SeecMktg::UserRight";
        /// <summary>
        /// 取得定義使用在圖形驗證碼的 Session 記錄名稱。
        /// </summary>
        public const string ImageVerifyCode = "SeecMktg::ImageVerifyCode";

        /// <summary>
        /// 取得定義查詢條件緩衝相關的 Session 記錄名稱。
        /// </summary>
        public const string SearchConds = "SeecMktg::SearchConds";

        /// <summary>
        /// 取得定義外銷型號分類查詢條件緩衝相關的 Session 記錄名稱。
        /// </summary>
        public const string ExtItemCatConds = "SeecMktg::ExtItemCatConds";
    }
}