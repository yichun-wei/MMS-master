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
        /// 登入記錄。
        /// </summary>
        public const string LOGIN_LOG = "LOGIN_LOG";
    }

    /// <summary>
    /// 登入記錄的類別實作。
    /// </summary>
    public class LoginLog : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.LoginLog 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public LoginLog(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.LOGIN_LOG))
        {
            base.DefaultSqlOrder = new SqlOrder("SID", Sort.Descending);
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
            /// 對象系統代號。
            /// </summary>
            [SchemaMapping(Name = "TGT_SID", Type = ReturnType.SId)]
            public ISystemId TgtSId { get; set; }
            /// <summary>
            /// 登入使用的帳號。
            /// </summary>
            [SchemaMapping(Name = "ACCT", Type = ReturnType.String)]
            public string Acct { get; set; }
            /// <summary>
            /// 登入使用的密碼。
            /// </summary>
            [SchemaMapping(Name = "PWD", Type = ReturnType.String)]
            public string Pwd { get; set; }
            /// <summary>
            /// 使用者 IP。
            /// </summary>
            [SchemaMapping(Name = "CLIENT_IP", Type = ReturnType.String)]
            public string ClientIP { get; set; }
            /// <summary>
            /// 使用者模式。
            /// </summary>
            [SchemaMapping(Name = "USER_MODE", Type = ReturnType.Int)]
            public int UserMode { get; set; }
            /// <summary>
            /// 是否登入成功。
            /// </summary>
            [SchemaMapping(Name = "IS_SUCCESS", Type = ReturnType.String)]
            public string IsSuccess { get; set; }
            /// <summary>
            /// 是否已登出。
            /// </summary>
            [SchemaMapping(Name = "IS_LOGGED_OUT", Type = ReturnType.Bool)]
            public bool IsLoggedOut { get; set; }
            /// <summary>
            /// 最後活動時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_ACT_DT", Type = ReturnType.DateTime)]
            public DateTime LastActDT { get; set; }
            /// <summary>
            /// 訊息。
            /// </summary>
            [SchemaMapping(Name = "MSG", Type = ReturnType.String)]
            public string Msg { get; set; }

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
            #endregion
        }
        #endregion

        #region 異動
        #region Add
        /// <summary> 
        /// 依指定的參數，新增一筆資料。 
        /// </summary> 
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="tgtSId">對象系統代號（null 將自動略過操作）。</param>
        /// <param name="acct">登入使用的帳號。</param>
        /// <param name="pwd">登入使用的密碼。</param>
        /// <param name="clientIP">使用者 IP。</param>
        /// <param name="userMode">使用者模式（1:前端 9:系統管理介面）。</param>
        /// <param name="isSuccess">是否登入成功。</param>
        /// <param name="msg">訊息。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception> 
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception> 
        public Returner Add(ISystemId actorSId, ISystemId tgtSId, string acct, string pwd, string clientIP, int userMode, bool isSuccess, string msg)
        {
            if (actorSId == null) throw new ArgumentNullException("actorSId");
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            DateTime now = DateTime.Now;

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("TGT_SID", tgtSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("ACCT", acct, GenericDBType.VarChar, true);
                transSet.SmartAdd("PWD", pwd, GenericDBType.VarChar, true);
                transSet.SmartAdd("CLIENT_IP", clientIP, GenericDBType.VarChar, false);
                transSet.SmartAdd("USER_MODE", userMode, GenericDBType.Int);
                transSet.SmartAdd("IS_SUCCESS", isSuccess ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("LAST_ACT_DT", now, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("MSG", msg, GenericDBType.VarChar, false);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateLoginState
        /// <summary>
        /// 更新登入狀態。
        /// </summary>
        /// <param name="loginLogSId">登入記錄系統代號。</param> 
        /// <param name="isLoggedOut">是否已登出。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 courseSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateLoginState(ISystemId loginLogSId, bool isLoggedOut)
        {
            if (loginLogSId == null) { throw new ArgumentNullException("loginLogSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(loginLogSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("IS_LOGGED_OUT", isLoggedOut ? "Y" : "N", GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, loginLogSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateLastActivityDT
        /// <summary>
        /// 更新最後活動時間。
        /// </summary>
        /// <param name="loginLogSId">登入記錄系統代號。</param> 
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 courseSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateLastActDT(ISystemId loginLogSId)
        {
            if (loginLogSId == null) { throw new ArgumentNullException("loginLogSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(loginLogSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("LAST_ACT_DT", DateTime.Now.ToString("yyyyMMddHHmmss"), GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, loginLogSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
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
            /// <summary>
            /// 初始化 InfoConds 類別的新執行個體。
            /// 預設略過所有條件。
            /// </summary>
            public InfoConds()
            {

            }

            /// <summary>
            /// 初始化 InfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="userMode">使用者模式（1:前端 9:系統管理介面; 若為 null 則略過條件檢查）。</param>
            /// <param name="tgtSId">對象系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="acct">登入使用的帳號（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="isSuccess">是否登入成功（若為 null 則略過條件檢查）。</param>
            public InfoConds(int? userMode, ISystemId tgtSId, string acct, bool? isSuccess)
            {
                this.UserMode = userMode;
                this.TgtSId = tgtSId;
                this.Acct = acct;
                this.IsSuccess = isSuccess;
            }

            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? UserMode { get; set; }
            /// <summary>
            /// 對象系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId TgtSId { get; set; }
            /// <summary>
            /// 登入使用的帳號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Acct { get; set; }
            /// <summary>
            /// 是否登入成功（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsSuccess { get; set; }
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
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper,
        SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
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

            custEntity.Append("SELECT * ");
            custEntity.Append("FROM [LOGIN_LOG] ");

            var sqlConds = new List<string>();

            if (qConds.UserMode.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "USER_MODE", SqlOperator.EqualTo, qConds.UserMode.Value, GenericDBType.Int));
            }

            if (qConds.TgtSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TGT_SID", SqlOperator.EqualTo, qConds.TgtSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.Acct))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACCT", SqlOperator.EqualTo, qConds.Acct, GenericDBType.VarChar));
            }

            if (qConds.IsSuccess.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_SUCCESS", SqlOperator.EqualTo, qConds.IsSuccess.Value ? "Y" : "N", GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion

        #region IsLoginInDate
        /// <summary> 
        /// 帳號在指定日期是否已經登入。 
        /// </summary> 
        /// <param name="userMode">使用者模式（1:前端 9:系統管理介面）。</param>
        /// <param name="acct">登入使用的帳號。</param>
        /// <param name="date">指定的日期。</param>
        /// <returns>是否已經登入。</returns> 
        public bool IsLoginInDate(int userMode, string acct, DateTime date)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT * ");
            custEntity.Append("FROM [LOGIN_LOG] ");

            var sqlConds = new List<string>();

            sqlConds.Add(custEntity.BuildConds(string.Empty, "USER_MODE", SqlOperator.EqualTo, userMode, GenericDBType.Int));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_SUCCESS", SqlOperator.EqualTo, "Y", GenericDBType.Char));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "ACCT", SqlOperator.EqualTo, acct, GenericDBType.VarChar));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.GreaterEqualThan, date.ToString("yyyyMMdd000000"), GenericDBType.Char));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.LessEqualThan, date.ToString("yyyyMMdd000000"), GenericDBType.Char));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, base.DefaultSqlOrder, condsMain).IsCompletedAndContinue;
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion

        #region IsLoginAndActivity
        /// <summary> 
        /// 帳號是否已經登入且仍在系統活動中（不區分前後台）。 
        /// </summary> 
        /// <param name="loginLogSId">登入記錄系統代號。</param> 
        /// <param name="maxIdleTime">最大閒置時間（分鐘）。</param> 
        /// <returns>是否已經登入且仍在系統活動中。</returns> 
        public bool IsLoginAndActivity(ISystemId loginLogSId, int maxIdleTime)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [LOGIN_LOG].*, ");
            //已登入多久的時間 (分鐘)
            custEntity.Append("       DATEDIFF(MINUTE, CONVERT(DATETIME, (SUBSTRING([LOGIN_LOG].[CDT], 1, 8) + ' ' + SUBSTRING([LOGIN_LOG].[CDT], 9, 2) + ':' + SUBSTRING([LOGIN_LOG].[CDT], 11, 2) + ':' + SUBSTRING([LOGIN_LOG].[CDT], 13, 2))), GETDATE()) AS [LOGGED_IN_TIME],  ");
            //已停滯多久沒有活動 (分鐘)
            custEntity.Append("       DATEDIFF(MINUTE, CONVERT(DATETIME, (SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 1, 8) + ' ' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 9, 2) + ':' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 11, 2) + ':' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 13, 2))), GETDATE()) AS [IDLE_TIME]  ");
            custEntity.Append("FROM [LOGIN_LOG] ");

            var sqlConds = new List<string>();

            sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, loginLogSId.Value, GenericDBType.Char));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_SUCCESS", SqlOperator.EqualTo, "Y", GenericDBType.Char));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_LOGGED_OUT", SqlOperator.EqualTo, "N", GenericDBType.Char));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            //超過最大閒置時間一律視為已登出
            condsMain.Add("IDLE_TIME", SqlCond.LessEqualThan, maxIdleTime, GenericDBType.Int, SqlCondsSet.And);

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, base.DefaultSqlOrder, condsMain).IsCompletedAndContinue;
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
        #endregion

        #region 活動中的登入帳號查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class ActAcctInfoConds
        {
            /// <summary>
            /// 初始化 ActAcctInfoConds 類別的新執行個體。
            /// 預設略過所有條件。
            /// </summary>
            public ActAcctInfoConds()
            {

            }

            /// <summary>
            /// 初始化 ActAcctInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="loginLogSId">登入記錄系統代號（若為 null 則略過條件檢查）。</param> 
            /// <param name="userMode">使用者模式（1:前端 9:系統管理介面; 若為 null 則略過條件檢查）。</param>
            /// <param name="tgtSId">對象系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="acct">登入使用的帳號（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="loggedMinutes">已登入的指定分鐘時間內（若為 null 則略過條件檢查）。</param>
            /// <param name="maxIdleTime">最大閒置時間（分鐘）。</param> 
            public ActAcctInfoConds(ISystemId loginLogSId, int? userMode, ISystemId tgtSId, string acct, int? loggedMinutes, int maxIdleTime)
            {
                this.LoginLogSId = loginLogSId;
                this.UserMode = userMode;
                this.TgtSId = tgtSId;
                this.Acct = acct;
                this.LoggedMinutes = loggedMinutes;
                this.MaxIdleTime = maxIdleTime;
            }

            /// <summary>
            /// 登入記錄系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId LoginLogSId { get; set; }
            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? UserMode { get; set; }
            /// <summary>
            /// 對象系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId TgtSId { get; set; }
            /// <summary>
            /// 登入使用的帳號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Acct { get; set; }
            /// <summary>
            /// 已登入的指定分鐘時間內（若為 null 則略過條件檢查）。
            /// </summary>
            public int? LoggedMinutes { get; set; }
            /// <summary>
            /// 最大閒置時間（分鐘）。
            /// </summary>
            public int MaxIdleTime { get; set; }
        }
        #endregion

        #region GetActAcctInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetActAcctInfo(ActAcctInfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetActAcctInfoCondsSet(qConds));

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
        public Returner GetActAcctInfo(ActAcctInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetActAcctInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetActAcctInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetActAcctInfoCount(ActAcctInfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetActAcctInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetActAcctInfoByCompoundSearch
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
        public Returner GetActAcctInfoByCompoundSearch(ActAcctInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper,
        SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetActAcctInfoCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetActAcctInfoByCompoundSearchCount
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
        public Returner GetActAcctInfoByCompoundSearchCount(ActAcctInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetActAcctInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetActAcctInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetActAcctInfoCondsSet(ActAcctInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [LOGIN_LOG].*, ");
            //已登入多久的時間 (分鐘)
            custEntity.Append("       DATEDIFF(MINUTE, CONVERT(DATETIME, (SUBSTRING([LOGIN_LOG].[CDT], 1, 8) + ' ' + SUBSTRING([LOGIN_LOG].[CDT], 9, 2) + ':' + SUBSTRING([LOGIN_LOG].[CDT], 11, 2) + ':' + SUBSTRING([LOGIN_LOG].[CDT], 13, 2))), GETDATE()) AS [LOGGED_IN_TIME], ");
            //已停滯多久沒有活動 (分鐘)
            custEntity.Append("       DATEDIFF(MINUTE, CONVERT(DATETIME, (SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 1, 8) + ' ' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 9, 2) + ':' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 11, 2) + ':' + SUBSTRING([LOGIN_LOG].[LAST_ACT_DT], 13, 2))), GETDATE()) AS [IDLE_TIME], ");
            custEntity.Append("       [USER].[ACCT] AS [USER_ACCT], [USER].[NAME] AS [USER_NAME] ");
            custEntity.Append("FROM [LOGIN_LOG] ");
            custEntity.Append("     INNER JOIN ");
            custEntity.Append("     (");
            //custEntity.Append("     SELECT 1 AS [USER_MODE], [SID], [ACCT], [NAME] FROM [MEMBER] ");
            //custEntity.Append("     UNION ALL ");
            //custEntity.Append("     SELECT 9 AS [USER_MODE], [SID], [ACCT], [NAME] FROM [SYS_USER] ");
            //沒有後台 (前台也是用系統使用者)
            custEntity.Append("     SELECT 1 AS [USER_MODE], [SID], [ACCT], [NAME] FROM [SYS_USER] ");
            custEntity.Append("     ) [USER] ON [LOGIN_LOG].[IS_SUCCESS] = 'Y' AND [LOGIN_LOG].[IS_LOGGED_OUT] = 'N' AND [LOGIN_LOG].[TGT_SID] = [USER].[SID]");

            var sqlConds = new List<string>();

            if (qConds.LoginLogSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("LOGIN_LOG", "SID", SqlOperator.EqualTo, qConds.LoginLogSId.Value, GenericDBType.Char));
            }

            if (qConds.TgtSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("LOGIN_LOG", "TGT_SID", SqlOperator.EqualTo, qConds.TgtSId.Value, GenericDBType.Char));
            }

            if (qConds.UserMode.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("USER", "USER_MODE", SqlOperator.EqualTo, qConds.UserMode.Value, GenericDBType.Int));
            }

            if (!string.IsNullOrEmpty(qConds.Acct))
            {
                sqlConds.Add(custEntity.BuildConds("USER", "ACCT", SqlOperator.EqualTo, qConds.Acct, GenericDBType.VarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            //超過最大閒置時間一律視為已登出
            condsMain.Add("IDLE_TIME", SqlCond.LessEqualThan, qConds.MaxIdleTime, GenericDBType.Int, SqlCondsSet.And);

            if (qConds.LoggedMinutes.HasValue)
            {
                condsMain.Add("LOGGED_IN_TIME", SqlCond.LessEqualThan, qConds.LoggedMinutes.Value, GenericDBType.Int, SqlCondsSet.And);
            }

            return condsMain;
        }
        #endregion
        #endregion

        #region 是否超過登入最大嘗試次數
        /// <summary>
        /// 是否超過登入最大嘗試次數。
        /// </summary>
        /// <param name="acct">帳號。</param>
        /// <param name="maxAttemptCount">最大嘗試次數。</param>
        /// <param name="withinMinutes">在 N 分鐘內。</param>
        public bool IsOverAttemptCount(string acct, int maxAttemptCount, int withinMinutes)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [LOGIN_LOG].* ");
            custEntity.Append("FROM [LOGIN_LOG] ");

            var sqlConds = new List<string>();

            sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_SUCCESS", SqlOperator.EqualTo, "N", GenericDBType.Char));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "ACCT", SqlOperator.EqualTo, acct, GenericDBType.VarChar));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.GreaterEqualThan, DateTime.Now.AddMinutes(0 - withinMinutes).ToString("yyyyMMddHHmmss"), GenericDBType.Char));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return Convert.ToInt32(base.Entity.GetCountBy(condsMain).DataSet.Tables[0].Rows[0][0]) >= maxAttemptCount;
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
    }
}
