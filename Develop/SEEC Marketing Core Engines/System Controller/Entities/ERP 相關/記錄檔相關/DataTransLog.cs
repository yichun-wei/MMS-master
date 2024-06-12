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
        /// 資料異動記錄。
        /// </summary>
        public const string DATA_TRANS_LOG = "DATA_TRANS_LOG";
    }

    /// <summary>
    /// 資料異動記錄的類別實作。
    /// </summary>
    public class DataTransLog : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.DataTransLog 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public DataTransLog(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.DATA_TRANS_LOG))
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
            /// 表格名稱。
            /// </summary>
            [SchemaMapping(Name = "TABLE_NAME", Type = ReturnType.String)]
            public string TableName { get; set; }
            /// <summary>
            /// 表格系統代號。
            /// </summary>
            [SchemaMapping(Name = "TABLE_SID", Type = ReturnType.SId)]
            public ISystemId TableSId { get; set; }
            /// <summary>
            /// 群組代碼。
            /// </summary>
            [SchemaMapping(Name = "GROUP_CODE", Type = ReturnType.Int, AllowNull = true)]
            public int? GroupCode { get; set; }
            /// <summary>
            /// 行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除）。
            /// </summary>
            [SchemaMapping(Name = "ACT", Type = ReturnType.String)]
            public string Act { get; set; }
            /// <summary>
            /// 權限代碼。
            /// </summary>
            [SchemaMapping(Name = "AUTH_CODE", Type = ReturnType.String)]
            public string AuthCode { get; set; }
            /// <summary>
            /// 權限名稱。
            /// </summary>
            [SchemaMapping(Name = "AUTH_NAME", Type = ReturnType.String)]
            public string AuthName { get; set; }
            /// <summary>
            /// 資料標題。
            /// </summary>
            [SchemaMapping(Name = "DATA_TITLE", Type = ReturnType.String)]
            public string DataTitle { get; set; }
            /// <summary>
            /// 使用者IP。
            /// </summary>
            [SchemaMapping(Name = "CLIENT_IP", Type = ReturnType.String)]
            public string ClientIP { get; set; }
            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面）。
            /// </summary>
            [SchemaMapping(Name = "USER_MODE", Type = ReturnType.Int)]
            public int UserMode { get; set; }
            /// <summary>
            /// 異動前的資料。
            /// </summary>
            [SchemaMapping(Name = "FORMER", Type = ReturnType.String)]
            public string Former { get; set; }

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
            /// 表格名稱。
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 表格系統代號。
            /// </summary>
            public ISystemId TableSId { get; set; }
            /// <summary>
            /// 群組代碼。
            /// </summary>
            public int? GroupCode { get; set; }
            /// <summary>
            /// 行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除）。
            /// </summary>
            public string Act { get; set; }
            /// <summary>
            /// 權限代碼。
            /// </summary>
            public string AuthCode { get; set; }
            /// <summary>
            /// 權限名稱。
            /// </summary>
            public string AuthName { get; set; }
            /// <summary>
            /// 資料標題。
            /// </summary>
            public string DataTitle { get; set; }
            /// <summary>
            /// 使用者IP。
            /// </summary>
            public string ClientIP { get; set; }
            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面）。
            /// </summary>
            public int? UserMode { get; set; }
            /// <summary>
            /// 異動前的資料。
            /// </summary>
            public string Former { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary> 
        /// 依指定的參數，新增一筆資料。 
        /// </summary> 
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="tableName">表格名稱。</param>
        /// <param name="tableSId">表格系統代號。</param>
        /// <param name="groupCode">群組代碼（null 將自動略過操作）。</param>
        /// <param name="act">行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除）。</param>
        /// <param name="authCode">權限代碼。</param>
        /// <param name="authName">權限名稱。</param>
        /// <param name="dataTitle">資料標題。</param>
        /// <param name="clientIP">使用者 IP。</param>
        /// <param name="userMode">使用者模式（1:前端 9:系統管理介面）。</param>
        /// <param name="former">異動前的資料。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns> 
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception> 
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception> 
        public Returner Add(ISystemId actorSId, string tableName, ISystemId tableSId, int? groupCode, string act, string authCode, string authName, string dataTitle, string clientIP, int? userMode, string former)
        {
            if (actorSId == null) throw new ArgumentNullException("actorSId");
            if (tableSId == null) throw new ArgumentNullException("tableSId");
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, tableSId);
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
                transSet.SmartAdd("TABLE_NAME", tableName, GenericDBType.VarChar, false);
                transSet.SmartAdd("TABLE_SID", tableSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("GROUP_CODE", groupCode, GenericDBType.Int);
                transSet.SmartAdd("ACT", act, GenericDBType.VarChar, false);
                transSet.SmartAdd("AUTH_CODE", authCode, GenericDBType.VarChar, false);
                transSet.SmartAdd("AUTH_NAME", authName, GenericDBType.NVarChar, false);
                transSet.SmartAdd("DATA_TITLE", dataTitle, GenericDBType.NVarChar, false);
                transSet.SmartAdd("CLIENT_IP", clientIP, GenericDBType.VarChar, false);
                transSet.SmartAdd("USER_MODE", userMode, GenericDBType.Int);
                transSet.SmartAdd("FORMER", former, GenericDBType.NVarChar, true);

                returner.ChangeInto(base.Add(transSet, true));
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
            /// <param name="cSId">建立人系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="tableName">表格名稱（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="tableSId">表格系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="act">行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除; 若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="userMode">使用者模式（1:前端 9:系統管理介面; 若為 null  則略過條件檢查）。</param>
            public InfoConds(ISystemId cSId, string tableName, ISystemId tableSId, string act, int? userMode)
            {
                this.CSId = cSId;
                this.TableName = tableName;
                this.TableSId = tableSId;
                this.Act = act;
                this.UserMode = userMode;
            }

            /// <summary>
            /// 建立人系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId CSId { get; set; }
            /// <summary>
            /// 表格名稱（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 表格系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId TableSId { get; set; }
            /// <summary>
            /// 行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除; 若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Act { get; set; }
            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面; 若為 null  則略過條件檢查）。
            /// </summary>
            public int? UserMode { get; set; }
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
        public Returner GetInfo(InfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns
        )
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
            custEntity.Append("FROM [DATA_TRANS_LOG] ");

            var sqlConds = new List<string>();

            if (qConds.CSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CSID", SqlOperator.EqualTo, qConds.CSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.TableName))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TABLE_NAME", SqlOperator.EqualTo, qConds.TableName, GenericDBType.VarChar));
            }

            if (qConds.TableSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TABLE_SID", SqlOperator.EqualTo, qConds.TableSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.Act))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACT", SqlOperator.EqualTo, qConds.Act, GenericDBType.VarChar));
            }

            if (qConds.UserMode.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "USER_MODE", SqlOperator.EqualTo, qConds.UserMode.Value, GenericDBType.Int));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 檢視查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class InfoViewConds
        {
            /// <summary>
            /// 初始化 InfoViewConds 類別的新執行個體。
            /// 預設略過所有條件。
            /// </summary>
            public InfoViewConds()
            {

            }

            /// <summary>
            /// 初始化 InfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="csids">建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="tableName">表格名稱（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="tableSId">表格系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="act">行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除; 若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="authCode">權限代碼（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="userMode">使用者模式（1:前端 9:系統管理介面; 若為 null  則略過條件檢查）。</param>
            /// <param name="beginTransDateByRange">範圍查詢的起始異動日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endTransDateByRange">範圍查詢的結束異動日期（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] csids, string tableName, ISystemId tableSId, string act, string authCode, int? userMode, DateTime? beginTransDateByRange, DateTime? endTransDateByRange)
            {
                this.CSIds = csids;
                this.TableName = tableName;
                this.TableSId = tableSId;
                this.Act = act;
                this.AuthCode = authCode;
                this.UserMode = userMode;
                this.BeginTransDateByRange = beginTransDateByRange;
                this.EndTransDateByRange = endTransDateByRange;
            }

            /// <summary>
            /// 建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] CSIds { get; set; }
            /// <summary>
            /// 表格名稱（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 表格系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId TableSId { get; set; }
            /// <summary>
            /// 行為（A:新增 U:修改 E:啟用 DE:停用 D:刪除 MD:註解刪除; 若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Act { get; set; }
            /// <summary>
            /// 權限代碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string AuthCode { get; set; }
            /// <summary>
            /// 使用者模式（1:前端 9:系統管理介面; 若為 null  則略過條件檢查）。
            /// </summary>
            public int? UserMode { get; set; }
            /// <summary>
            /// 範圍查詢的起始異動日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginTransDateByRange { get; set; }
            /// <summary>
            /// 範圍查詢的結束異動日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndTransDateByRange { get; set; }
        }
        #endregion

        #region GetInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoView(InfoViewConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoViewCondsSet(qConds));

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
        public Returner GetInfoView(InfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoViewCount(InfoViewConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewByCompoundSearch
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
        public Returner GetInfoViewByCompoundSearch(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper,
        SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoViewCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewByCompoundSearchCount
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
        public Returner GetInfoViewByCompoundSearchCount(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoViewCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetInfoViewCondsSet(InfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [DATA_TRANS_LOG].*, ");
            custEntity.Append("       [SYS_USER].[ACCT] AS [SYS_USER_ACCT], [SYS_USER].[NAME] AS [SYS_USER_NAME]  ");
            custEntity.Append("FROM [DATA_TRANS_LOG] ");
            custEntity.Append("     INNER JOIN [SYS_USER] ON [DATA_TRANS_LOG].[CSID] = [SYS_USER].[SID] ");

            var sqlConds = new List<string>();

            if (qConds.CSIds != null && qConds.CSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "CSID", SqlOperator.EqualTo, SystemId.ToString(qConds.CSIds), GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.TableName))
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "TABLE_NAME", SqlOperator.EqualTo, qConds.TableName, GenericDBType.VarChar));
            }

            if (qConds.TableSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "TABLE_SID", SqlOperator.EqualTo, qConds.TableSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.Act))
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "ACT", SqlOperator.EqualTo, qConds.Act, GenericDBType.VarChar));
            }

            if (!string.IsNullOrEmpty(qConds.AuthCode))
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "AUTH_CODE", SqlOperator.EqualTo, qConds.AuthCode, GenericDBType.VarChar));
            }

            if (qConds.UserMode.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "USER_MODE", SqlOperator.EqualTo, qConds.UserMode.Value, GenericDBType.Int));
            }

            if (qConds.BeginTransDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "CDT", SqlOperator.GreaterEqualThan, qConds.BeginTransDateByRange.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndTransDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("DATA_TRANS_LOG", "CDT", SqlOperator.LessEqualThan, qConds.EndTransDateByRange.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion
    }
}
