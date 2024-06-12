using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Security;
using System.Security.Cryptography;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using EzCoding.Web.UI;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 系統使用者 Helper。
    /// </summary>
    public class SysUserHelper
    {
        /// <summary>
        /// 登入頁網址。
        /// </summary>
        static string LoginUrl = SystemDefine.CltLoginUrl;
        /// <summary>
        /// 登出頁網址。
        /// </summary>
        static string LogoutUrl = SystemDefine.HttpsWebSiteRoot + "logout.aspx";

        #region 系統使用者資訊集合
        /// <summary>
        /// 系統使用者資訊集合。
        /// </summary>
        public class SysUserInfoSet
        {
            /// <summary>
            /// 初始化 SysUserInfoSet 類別的新執行個體。
            /// </summary>
            /// <param name="info">表格資料。</param>
            /// <param name="userRight">使用者權限。</param>
            public SysUserInfoSet(SysUser.Info info, UserRight userRight)
            {
                this.Info = info;
                this.UserRight = userRight;
                this.DomDistInfos = new PubCat.Info[0];

                Returner returner = null;
                try
                {
                    #region 內銷地區
                    if (this.UserRight.IsFullControl)
                    {
                        PubCat entityPubCat = new PubCat(SystemDefine.ConnInfo);
                        returner = entityPubCat.GetTopInfo(new PubCat.TopInfoConds(DefVal.SIds, (int)PubCat.UseTgtOpt.DomDist, DefVal.SId), Int32.MaxValue, new SqlOrder("SORT", Sort.Descending), IncludeScope.OnlyNotMarkDeletedAndEnabledBoth);
                        if (returner.IsCompletedAndContinue)
                        {
                            this.DomDistInfos = PubCat.Info.Binding(returner.DataSet.Tables[0]);
                        }
                    }
                    else
                    {
                        RelTab entityRelTab = new RelTab(SystemDefine.ConnInfo);
                        returner = entityRelTab.GetSysUserDomDistInfo(new RelTab.SysUserDomDistInfoConds(ConvertLib.ToSIds(info.SId), DefVal.SIds, IncludeScope.OnlyNotMarkDeletedAndEnabledBoth), Int32.MaxValue, new SqlOrder("SORT", Sort.Descending));
                        if (returner.IsCompletedAndContinue)
                        {
                            this.DomDistInfos = RelTab.SysUserDomDistInfo.Binding(returner.DataSet.Tables[0]);
                        }
                    }
                    #endregion
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }
                }
            }

            /// <summary>
            /// 系統使用者資訊。
            /// </summary>
            public SysUser.Info Info { get; private set; }
            /// <summary>
            /// 系統使用者權限。
            /// </summary>
            public UserRight UserRight { get; private set; }
            /// <summary>
            /// 內銷地區資訊。
            /// </summary>
            public PubCat.Info[] DomDistInfos { get; private set; }

            #region 檢查是否擁有指定的內銷審核權限
            /// <summary>
            /// 檢查是否擁有指定的內銷審核權限。
            /// </summary>
            /// <param name="perms">內銷審核權限（1:營管 2:財務 3:副總 4:倉管）。</param>
            public bool CheckDomAuditPerms(params int[] perms)
            {
                if (this.UserRight.IsFullControl) { return true; }

                foreach (var perm in perms)
                {
                    if (this.Info.DomAuditPerms.IndexOf(string.Format("-{0}-", perm)) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            #endregion

            #region 檢查是否擁有指定的外銷審核權限
            /// <summary>
            /// 檢查是否擁有指定的外銷審核權限。
            /// </summary>
            /// <param name="perms">外銷審核權限（1:外銷組）。</param>
            public bool CheckExtAuditPerms(params int[] perms)
            {
                if (this.UserRight.IsFullControl) { return true; }

                foreach (var perm in perms)
                {
                    if (this.Info.ExtAuditPerms.IndexOf(string.Format("-{0}-", perm)) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            #endregion
        }
        #endregion

        #region 系統使用者是否已經登入
        /// <summary>
        /// 系統使用者是否已經登入。
        /// </summary>
        public static bool IsLoggedIn
        {
            get
            {
                if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Session)
                {
                    return HttpContext.Current.Session[SessionDefine.SysUserSId] is SystemId;
                }
                else
                {
                    return SystemId.MinValue.IsSystemId(CookiesUtil.Read(CookiesDefine.SysUserSId));
                }
            }
        }
        #endregion

        #region 檢查系統使用者是否已經登入，若未登入，則直接導向登入頁。
        /// <summary>
        /// 檢查系統使用者是否已經登入，若已登入，則取回系統使用者資訊；若未登入，則直接導向登入頁。
        /// </summary>
        public static SysUser.Info CheckLoggedIn()
        {
            if (!SysUserHelper.IsLoggedIn)
            {
                HttpContext.Current.Response.Redirect(SysUserHelper.LoginUrl);
                return null;
            }

            Returner returner = null;
            try
            {
                //必須找得到登入記錄
                ISystemId sysUserLoginLogSId = SysUserHelper.CurrentSysUserLoginLogSId;
                LoginLog entityLoginLog = new LoginLog(SystemDefine.ConnInfo);
                returner = entityLoginLog.GetActAcctInfo(new LoginLog.ActAcctInfoConds(sysUserLoginLogSId, 1, DefVal.SId, DefVal.Str, DefVal.Int, SystemDefine.MaxIdleTimeAfterLogin), 1, SqlOrder.Clear, IncludeScope.All);
                if (!returner.IsCompletedAndContinue)
                {
                    //只有正式環境有未活動登出的限制
                    if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Cookies)
                    {
                        if (SystemDefine.SystemPhase == SystemPhase.Production)
                        {
                            HttpContext.Current.Response.Redirect(SysUserHelper.LoginUrl);
                            return null;
                        }
                    }
                }

                //必須找得到系統使用者
                SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);
                returner = entitySysUser.GetInfo(new SysUser.InfoConds(ConvertLib.ToSIds(SysUserHelper.CurrentSysUserSId), DefVal.Strs, DefVal.Ints, DefVal.SIds), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeletedAndEnabledBoth);
                if (returner.IsCompletedAndContinue)
                {
                    //更新最後活動時間
                    entityLoginLog.UpdateLastActDT(sysUserLoginLogSId);

                    return SysUser.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                }

                HttpContext.Current.Response.Redirect(SysUserHelper.LoginUrl);
                return null;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion

        #region 取得目前登入的系統使用者系統代號
        /// <summary>
        /// 取得目前登入的系統使用者系統代號。
        /// </summary>
        public static ISystemId CurrentSysUserSId
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (!SysUserHelper.IsLoggedIn)
                {
                    context.Response.Redirect(SysUserHelper.LoginUrl);
                    return null;
                }

                if (SystemDefine.LoginKeepingBy == LoginKeepingBy.Session)
                {
                    return (SystemId)context.Session[SessionDefine.SysUserSId];
                }
                else
                {
                    return new SystemId(CookiesUtil.Read(CookiesDefine.SysUserSId));
                }
            }
        }
        #endregion

        #region 取得目前登入的系統使用者登入記錄系統代號
        /// <summary>
        /// 取得目前登入的系統使用者登入記錄系統代號。
        /// </summary>
        public static ISystemId CurrentSysUserLoginLogSId
        {
            get
            {
                ISystemId userLoginLogSId = ConvertLib.ToSId(CookiesUtil.Read(CookiesDefine.SysUserLoginLogSId));
                if (!SysUserHelper.IsLoggedIn || userLoginLogSId == null)
                {
                    HttpContext.Current.Response.Redirect(SysUserHelper.LoginUrl);
                    return null;
                }
                return userLoginLogSId;
            }
        }
        #endregion

        #region 取得目前登入的系統使用者權限
        /// <summary>
        /// 取得目前登入的系統使用者權限。
        /// </summary>
        public static UserRight GetCurrentUserRight()
        {
            HttpContext context = HttpContext.Current;

            Returner returner = null;
            try
            {
                SysUser entitySysUser = new SysUser(SystemDefine.ConnInfo);
                returner = entitySysUser.GetInfo(new ISystemId[] { SysUserHelper.CurrentSysUserSId }, IncludeScope.OnlyNotMarkDeletedAndEnabledBoth, new string[] { "STATUS_FLAG" });
                if (returner.IsCompletedAndContinue)
                {
                    DataRow row = returner.DataSet.Tables[0].Rows[0];

                    return SysUserHelper.GetCurrentUserRight(row["STATUS_FLAG"].ToString());
                }
                else
                {
                    //若找不到帳號 (包含已被註解刪除或停用) 則自動登出.
                    context.Response.Redirect(SysUserHelper.LogoutUrl);
                    return null;
                }
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }

        /// <summary>
        /// 取得目前登入的系統使用者權限。
        /// </summary>
        /// <param name="statusFlag">狀態旗標。</param>
        public static UserRight GetCurrentUserRight(string statusFlag)
        {
            HttpContext context = HttpContext.Current;

            if (string.IsNullOrEmpty(statusFlag))
            {
                //空值表示新增後沒有再異動過
            }
            else if (string.CompareOrdinal(CookiesUtil.Read(CookiesDefine.SysUserStatusFlag), statusFlag) != 0)
            {
                //若異動旗標不同, 則重取權限.
                context.Session[SessionDefine.UserRight] = null;
                CookiesUtil.Write(CookiesDefine.SysUserStatusFlag, statusFlag);
                if (SystemDefine.EnableSSL)
                {
                    context.Response.Cookies[CookiesDefine.SysUserStatusFlag].Secure = true;
                }
            }

            if (context.Session[SessionDefine.UserRight] == null)
            {
                context.Session[SessionDefine.UserRight] = new UserRight(SysUserHelper.CurrentSysUserSId, UserRight.Range.Complex);
            }
            return (UserRight)context.Session[SessionDefine.UserRight];
        }
        #endregion

        #region EncodingPassword
        /// <summary>
        /// 將指定的密碼編碼處理。
        /// </summary>
        /// <param name="password">要編碼的密碼。</param>
        /// <param name="hashKey">雜湊鍵值。</param>
        /// <returns>編碼後的密碼。</returns>
        public static string EncodingPassword(string password, string hashKey)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            HashAlgorithm hash = new SHA512Managed();
            var buffer = Encoding.Default.GetBytes(string.Format("{0}{1}", password, hashKey));
            return Convert.ToBase64String(hash.ComputeHash(buffer));
        }
        #endregion

        #region IsValidAccount
        /// <summary>
        /// 驗證是否為合法的帳號。
        /// </summary>
        /// <param name="account">驗證的帳號。</param>
        /// <returns>是否為合法的帳號。</returns>
        public static bool IsValidAccount(string account)
        {
            ////填入3至16字元小寫英文字母、數字、以及_符號，第一字元需為英文字母。

            ////null、empty、字長串度小於 3、字長串度大於 16。
            //if (string.IsNullOrEmpty(account) || account.Length < 3 || account.Length > 16)
            //{
            //    return false;
            //}

            //const string numeral = "0123456789";
            //const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYabcdefghijklmnopqrstuvwxyz";

            ////首碼必須為英文
            //if (letters.IndexOf(account.Substring(0, 1)) == -1)
            //{
            //    return false;
            //}

            return true;
        }
        #endregion

        #region IsValidPassword
        /// <summary>
        /// 驗證是否為合法的密碼。
        /// </summary>
        /// <param name="password">驗證的密碼。</param>
        /// <returns>是否為合法的密碼。</returns>
        public static bool IsValidPassword(string password)
        {
            ////密碼不得為 null、empty、字長串度小於 6、字長串度大於 12。
            //if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 20)
            //{
            //    return false;
            //}

            //const string numeral = "0123456789";
            //const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYabcdefghijklmnopqrstuvwxyz";

            //bool passed = false;
            //for (int i = 0; i < password.Length; i++)
            //{
            //    //至少一個數字。
            //    if (numeral.IndexOf(password[i]) != -1)
            //    {
            //        passed = true;
            //        break;
            //    }
            //}

            //passed = false;
            //for (int i = 0; i < password.Length; i++)
            //{
            //    //至少一個英文字母。
            //    if (letters.IndexOf(password[i]) != -1)
            //    {
            //        passed = true;
            //        break;
            //    }
            //}

            //return passed;

            return true;
        }
        #endregion
    }
}
