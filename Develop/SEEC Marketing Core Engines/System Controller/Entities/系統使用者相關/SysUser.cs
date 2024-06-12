using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.SystemEngines;
using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 系統使用者。
        /// </summary>
        public const string SYS_USER = "SYS_USER";
    }

    /// <summary>
    /// 系統使用者的類別實作。
    /// </summary>
    public class SysUser : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.User 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SysUser(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.SYS_USER))
        {
            base.DefaultSqlOrder = new SqlOrder("ACCT", Sort.Ascending);
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            [SchemaMapping(Name = "SID", Type = ReturnType.SId)]
            public ISystemId SId { get; set; }
            /// <summary>
            /// 建立日期時間。
            /// </summary>
            [SchemaMapping(Name = "CDT", Type = ReturnType.DateTime)]
            public DateTime Cdt { get; set; }
            /// <summary>
            /// 修改日期時間。
            /// </summary>
            [SchemaMapping(Name = "MDT", Type = ReturnType.DateTime)]
            public DateTime Mdt { get; set; }
            /// <summary>
            /// 建立人。
            /// </summary>
            [SchemaMapping(Name = "CSID", Type = ReturnType.SId)]
            public ISystemId CSId { get; set; }
            /// <summary>
            /// 修改人。
            /// </summary>
            [SchemaMapping(Name = "MSID", Type = ReturnType.SId)]
            public ISystemId MSId { get; set; }
            /// <summary>
            /// 刪除標記。
            /// </summary>
            [SchemaMapping(Name = "MDELED", Type = ReturnType.Bool)]
            public bool MDeled { get; set; }
            /// <summary>
            /// 啟用。
            /// </summary>
            [SchemaMapping(Name = "ENABLED", Type = ReturnType.Bool)]
            public bool Enabled { get; set; }
            /// <summary>
            /// 是否為預設資料。
            /// </summary>
            [SchemaMapping(Name = "IS_DEF", Type = ReturnType.Bool)]
            public bool IsDef { get; set; }
            /// <summary>
            /// 雜湊鍵值。
            /// </summary>
            [SchemaMapping(Name = "HASH_KEY", Type = ReturnType.String)]
            public string HashKey { get; set; }
            /// <summary>
            /// 帳號。
            /// </summary>
            [SchemaMapping(Name = "ACCT", Type = ReturnType.String)]
            public string Acct { get; set; }
            /// <summary>
            /// 密碼。
            /// </summary>
            [SchemaMapping(Name = "PWD", Type = ReturnType.String)]
            public string Pwd { get; set; }
            /// <summary>
            /// 姓名。
            /// </summary>
            [SchemaMapping(Name = "NAME", Type = ReturnType.String)]
            public string Name { get; set; }
            /// <summary>
            /// 電子信箱。
            /// </summary>
            [SchemaMapping(Name = "EMAIL", Type = ReturnType.String)]
            public string Email { get; set; }
            /// <summary>
            /// 手機。
            /// </summary>
            [SchemaMapping(Name = "MOBILE", Type = ReturnType.String)]
            public string Mobile { get; set; }
            /// <summary>
            /// 電話。
            /// </summary>
            [SchemaMapping(Name = "TEL", Type = ReturnType.String)]
            public string Tel { get; set; }
            /// <summary>
            /// 部門。
            /// </summary>
            [SchemaMapping(Name = "DEPT", Type = ReturnType.String)]
            public string Dept { get; set; }
            /// <summary>
            /// 市場範圍（1:內銷 2:外銷）。
            /// </summary>
            [SchemaMapping(Name = "MKTG_RANGE", Type = ReturnType.String)]
            public string MktgRange { get; set; }
            /// <summary>
            /// 內銷審核權限（1:營管 2:財務 3:副總 4:倉管）。
            /// </summary>
            [SchemaMapping(Name = "DOM_AUDIT_PERMS", Type = ReturnType.String)]
            public string DomAuditPerms { get; set; }
            /// <summary>
            /// 外銷審核權限（1:外銷組）。
            /// </summary>
            [SchemaMapping(Name = "EXT_AUDIT_PERMS", Type = ReturnType.String)]
            public string ExtAuditPerms { get; set; }
            /// <summary>
            /// 系統使用者權限。
            /// </summary>
            [SchemaMapping(Name = "USER_RIGHT", Type = ReturnType.String)]
            public string UserRight { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }
            /// <summary>
            /// 系統使用者最後登入的 IP。
            /// </summary>
            [SchemaMapping(Name = "LAST_LOGIN_IP", Type = ReturnType.String)]
            public string LastLoginIP { get; set; }
            /// <summary>
            /// 最後的登入時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_LOGIN_DT", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? LastLoginDT { get; set; }
            /// <summary>
            /// 狀態旗標。
            /// </summary>
            [SchemaMapping(Name = "STATUS_FLAG", Type = ReturnType.String)]
            public string StatusFlag { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<Info>(new Info(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info[] Binding(DataTable dTable)
            {
                List<Info> infos = new List<Info>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(Info.Binding(row));
                }

                return infos.ToArray();
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info Binding(InputInfo input)
            {
                return DBUtilBox.BindingInput<Info>(new Info(), input);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info[] Binding(InputInfo[] inputs)
            {
                List<Info> infos = new List<Info>();
                foreach (var input in inputs)
                {
                    infos.Add(Info.Binding(input));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region 輸入資訊（驗證用）
        /// <summary>
        /// 輸入資訊（驗證用）。
        /// </summary>
        public class InputInfo
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 建立日期時間。
            /// </summary>
            public DateTime? Cdt { get; set; }
            /// <summary>
            /// 修改日期時間。
            /// </summary>
            public DateTime? Mdt { get; set; }
            /// <summary>
            /// 建立人。
            /// </summary>
            public ISystemId CSId { get; set; }
            /// <summary>
            /// 修改人。
            /// </summary>
            public ISystemId MSId { get; set; }
            /// <summary>
            /// 刪除標記。
            /// </summary>
            public bool? MDeled { get; set; }
            /// <summary>
            /// 啟用。
            /// </summary>
            public bool? Enabled { get; set; }
            /// <summary>
            /// 是否為預設資料。
            /// </summary>
            public bool? IsDef { get; set; }
            /// <summary>
            /// 雜湊鍵值。
            /// </summary>
            public string HashKey { get; set; }
            /// <summary>
            /// 帳號。
            /// </summary>
            public string Acct { get; set; }
            /// <summary>
            /// 密碼。
            /// </summary>
            public string Pwd { get; set; }
            /// <summary>
            /// 姓名。
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 電子信箱。
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 手機。
            /// </summary>
            public string Mobile { get; set; }
            /// <summary>
            /// 電話。
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 部門。
            /// </summary>
            public string Dept { get; set; }
            /// <summary>
            /// 市場範圍（1:內銷 2:外銷）。
            /// </summary>
            public string MktgRange { get; set; }
            /// <summary>
            /// 內銷審核權限（1:營管 2:財務 3:副總 4:倉管）。
            /// </summary>
            public string DomAuditPerms { get; set; }
            /// <summary>
            /// 外銷審核權限（1:外銷組）。
            /// </summary>
            public string ExtAuditPerms { get; set; }
            /// <summary>
            /// 系統使用者權限。
            /// </summary>
            public string UserRight { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
            /// <summary>
            /// 系統使用者最後登入的 IP。
            /// </summary>
            public string LastLoginIP { get; set; }
            /// <summary>
            /// 最後的登入時間。
            /// </summary>
            public DateTime? LastLoginDT { get; set; }
            /// <summary>
            /// 狀態旗標。
            /// </summary>
            public string StatusFlag { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="hashKey">雜湊鍵值。</param>
        /// <param name="acct">帳號。</param>
        /// <param name="pwd">密碼。</param>
        /// <param name="name">姓名。</param>
        /// <param name="email">電子信箱（null 或 empty 將自動略過操作）。</param>
        /// <param name="mobile">行動電話（null 或 empty 將自動略過操作）。</param>
        /// <param name="tel">電話（null 或 empty 將自動略過操作）。</param>
        /// <param name="dept">部門（null 或 empty 將自動略過操作）。</param>
        /// <param name="mktgRange">市場範圍（1:內銷 2:外銷; null 或 empty 將自動略過操作）。</param>
        /// <param name="domAuditPerms">內銷審核權限（1:營管 2:財務 3:副總 4:倉管; null 或 empty 將自動略過操作）。</param>
        /// <param name="extAuditPerms">外銷審核權限（1:外銷組; null 或 empty 將自動略過操作）。</param>
        /// <param name="userRight">系統使用者權限（null 或 empty 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, string hashKey, string acct, string pwd, string name, string email, string mobile, string tel, string dept, string mktgRange, string domAuditPerms, string extAuditPerms, string userRight, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("HASH_KEY", hashKey, GenericDBType.VarChar, false);
                transSet.SmartAdd("ACCT", acct, GenericDBType.VarChar, false);
                transSet.SmartAdd("PWD", pwd, GenericDBType.VarChar, false);
                transSet.SmartAdd("NAME", name, GenericDBType.NVarChar, false);
                transSet.SmartAdd("EMAIL", email, GenericDBType.NVarChar, true);
                transSet.SmartAdd("MOBILE", mobile, GenericDBType.VarChar, true);
                transSet.SmartAdd("TEL", tel, GenericDBType.VarChar, true);
                transSet.SmartAdd("DEPT", dept, GenericDBType.NVarChar, true);
                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.VarChar, true);
                transSet.SmartAdd("DOM_AUDIT_PERMS", domAuditPerms, GenericDBType.VarChar, true);
                transSet.SmartAdd("EXT_AUDIT_PERMS", extAuditPerms, GenericDBType.VarChar, true);
                transSet.SmartAdd("USER_RIGHT", userRight, GenericDBType.NVarChar, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify
        /// <summary>
        /// 依指定的參數，修改一筆系統使用者。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="sysUserSId">系統使用者系統代號。</param>
        /// <param name="acct">帳號。</param>
        /// <param name="pwd">密碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="name">姓名。</param>
        /// <param name="email">電子信箱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="mobile">行動電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="tel">電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="dept">部門（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="mktgRange">市場範圍（1:內銷 2:外銷; null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="domAuditPerms">內銷審核權限（1:營管 2:財務 3:副總 4:倉管; null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="extAuditPerms">外銷審核權限（1:外銷組; null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="userRight">系統使用者權限（null 或 empty 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 sysUserSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId sysUserSId, string acct, string pwd, string name, string email, string mobile, string tel, string dept, string mktgRange, string domAuditPerms, string extAuditPerms, string userRight, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (sysUserSId == null) { throw new ArgumentNullException("sysUserSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, sysUserSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                //更新狀態旗標
                this.UpdateStatusFlag(new ISystemId[] { sysUserSId });

                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ACCT", acct, GenericDBType.VarChar, false);
                transSet.SmartAdd("PWD", pwd, GenericDBType.VarChar, true);
                transSet.SmartAdd("NAME", name, GenericDBType.NVarChar, false);
                transSet.SmartAdd("EMAIL", email, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("MOBILE", mobile, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("TEL", tel, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DEPT", dept, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DOM_AUDIT_PERMS", domAuditPerms, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("EXT_AUDIT_PERMS", extAuditPerms, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("USER_RIGHT", userRight, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, sysUserSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdatePassword
        /// <summary>
        /// 更新密碼。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="sysUserSId">系統使用者系統代號。</param>
        /// <param name="pwd">密碼。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 sysUserSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdatePassword(ISystemId actorSId, ISystemId sysUserSId, string pwd)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (sysUserSId == null) { throw new ArgumentNullException("sysUserSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, sysUserSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                //更新狀態旗標
                this.UpdateStatusFlag(new ISystemId[] { sysUserSId });

                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("PWD", pwd, GenericDBType.VarChar, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, sysUserSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateLoginCache
        /// <summary>
        /// 更新登入快取。
        /// </summary>
        /// <param name="sysUserSId">系統使用者系統代號。</param>
        /// <param name="clientIP">系統使用者登入的 IP 位址。</param>
        /// <param name="loginDT">登入的時間。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 sysUserSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateLoginCache(ISystemId sysUserSId, string clientIP, DateTime loginDT)
        {
            if (sysUserSId == null) { throw new ArgumentNullException("sysUserSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(sysUserSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("LAST_LOGIN_IP", clientIP, GenericDBType.VarChar, false);
                transSet.SmartAdd("LAST_LOGIN_DT", loginDT, "yyyyMMddHHmmss", GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, sysUserSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateStatusFlag
        /// <summary>
        /// 更新狀態旗標。
        /// </summary>
        /// <param name="sysUserSIds">系統使用者系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 sysUserSIds 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        internal Returner UpdateStatusFlag(ISystemId[] sysUserSIds)
        {
            if (sysUserSIds == null) { throw new ArgumentNullException("sysUserSIds"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(sysUserSIds);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("STATUS_FLAG", new SystemId().ToString(), GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();

                if (sysUserSIds != null && sysUserSIds.Length > 0)
                {
                    condsMain.Add("SID", true, SystemId.ToString(sysUserSIds), GenericDBType.Char, SqlCondsSet.And);

                    returner.ChangeInto(base.Modify(transSet, condsMain, true));
                }
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion
        #endregion

        #region 一般查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class InfoConds
        {
            ///// <summary>
            ///// 初始化 InfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public InfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 InfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="sysUserSIds">系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="accts">帳號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="mktgRanges">市場範圍陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domDistSIds">內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] sysUserSIds, string[] accts, int[] mktgRanges, ISystemId[] domDistSIds)
            {
                this.SysUserSIds = sysUserSIds;
                this.Accts = accts;
                this.MktgRanges = mktgRanges;
                this.DomDistSIds = domDistSIds;
            }

            /// <summary>
            /// 系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] SysUserSIds { get; set; }
            /// <summary>
            /// 帳號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] Accts { get; set; }
            /// <summary>
            /// 市場範圍陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] MktgRanges { get; set; }
            /// <summary>
            /// 內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomDistSIds { get; set; }
        }
        #endregion

        #region GetInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetInfoBy(size, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }

        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoCount(InfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearchCount(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [SYS_USER].* ");
            custEntity.Append("FROM [SYS_USER] ");

            var sqlConds = new List<string>();

            if (qConds.SysUserSIds != null && qConds.SysUserSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.SysUserSIds), GenericDBType.Char));
            }

            if (qConds.Accts != null && qConds.Accts.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACCT", SqlOperator.EqualTo, qConds.Accts, GenericDBType.VarChar));
            }

            if (qConds.DomDistSIds != null && qConds.DomDistSIds.Length > 0)
            {
                var syntaxParams = custEntity.AddParameter("DOM_DIST_SID", SystemId.ToString(qConds.DomDistSIds), GenericDBType.Char);

                sqlConds.Add(string.Format("EXISTS (SELECT [REL_TAB].* FROM [REL_TAB] WHERE [REL_TAB].[REL_CODE] = 2 AND [REL_TAB].[USE_SID] = [SYS_USER].[SID] AND [REL_TAB].[TGT_SID] IN ({0}))", string.Join(",", syntaxParams.ToArray())));
            }

            if (qConds.MktgRanges != null && qConds.MktgRanges.Length > 0)
            {
                var conds = new List<string>();
                foreach (var mktgRange in qConds.MktgRanges)
                {
                    conds.Add(string.Format("[MKTG_RANGE] LIKE '%-{0}-%'", mktgRange));
                }
                sqlConds.Add(string.Format("({0})", string.Join(" OR ", conds.ToArray())));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion

        #region CheckEditModeForAccount
        /// <summary>
        /// 檢查在編輯時，是否允許異動帳號。
        /// </summary>
        /// <param name="sysUserSId">系統使用者系統代號（若為 null 則表示為新增模式）。</param> 
        /// <param name="acctNew">異動的帳號。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public bool CheckEditModeForAccount(ISystemId sysUserSId, string acctNew, IncludeScope includeScope)
        {
            string acctOld = string.Empty;

            if (sysUserSId != null)
            {
                using (Returner returner = base.GetInfo(new ISystemId[] { sysUserSId }, includeScope, new string[] { "ACCT" }))
                {
                    if (returner.IsCompletedAndContinue)
                    {
                        acctOld = returner.DataSet.Tables[0].Rows[0]["ACCT"].ToString();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (sysUserSId != null && string.Compare(acctOld, acctNew, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //修改時與原值相同
                return true;
            }
            else
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("ACCT", SqlCond.EqualTo, acctNew, GenericDBType.VarChar, SqlCondsSet.And);

                //新增時或修改時新舊值不相同
                return Convert.ToInt32(base.GetInfoCount(condsMain, includeScope).DataSet.Tables[0].Rows[0][0], CultureInfo.InvariantCulture) == 0;
            }
        }
        #endregion
        #endregion

        #region CheckDelete
        /// <summary> 
        /// 檢查是否允許被刪除。 
        /// </summary> 
        /// <param name="sysUserSId">系統使用者系統代號。</param>
        public bool CheckDelete(ISystemId sysUserSId)
        {
            Returner returner = new Returner();

            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            var unions = new List<string>();
            unions.Add(string.Format("SELECT [CSID] FROM [PG_ORDER] WHERE [CSID] = '{0}'", sysUserSId)); //備貨單
            unions.Add(string.Format("SELECT [CSID] FROM [DOM_ORDER] WHERE [CSID] = '{0}'", sysUserSId)); //內銷訂單
            unions.Add(string.Format("SELECT [CSID] FROM [EXT_QUOTN] WHERE [CSID] = '{0}'", sysUserSId)); //外銷報價單

            custEntity.Append("SELECT COUNT(*) AS [CNT] ");
            custEntity.Append("FROM (");
            custEntity.Append(string.Join(" UNION ", unions.ToArray()));
            custEntity.Append(") [T]");

            //var sqlConds = new List<string>();

            //custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            base.Entity.EnableCustomEntity = true;
            returner = base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, condsMain);
            base.Entity.EnableCustomEntity = false;

            return Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]) == 0;
        }
        #endregion
    }
}
