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
        /// 系統記錄。
        /// </summary>
        public const string SYS_LOG = "SYS_LOG";
    }

    /// <summary>
    /// 系統記錄的類別實作。
    /// </summary>
    public class SysLog : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.SysLog 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SysLog(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.SYS_LOG))
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
            /// 種類。
            /// </summary>
            [SchemaMapping(Name = "CAT", Type = ReturnType.String)]
            public string Cat { get; set; }
            /// <summary>
            /// 來源。
            /// </summary>
            [SchemaMapping(Name = "SRC", Type = ReturnType.String)]
            public string Src { get; set; }
            /// <summary>
            /// 類型（D:Debug N:Notice W:Warning E:Error F:Fatal）。
            /// </summary>
            [SchemaMapping(Name = "TYPE", Type = ReturnType.String)]
            public string Type { get; set; }
            /// <summary>
            /// 標題。
            /// </summary>
            [SchemaMapping(Name = "TITLE", Type = ReturnType.String)]
            public string Title { get; set; }
            /// <summary>
            /// 訊息。
            /// </summary>
            [SchemaMapping(Name = "MSG", Type = ReturnType.String)]
            public string Msg { get; set; }
            /// <summary>
            /// 使用者IP。
            /// </summary>
            [SchemaMapping(Name = "CLIENT_IP", Type = ReturnType.String)]
            public string ClientIP { get; set; }
            /// <summary>
            /// 條件一。
            /// </summary>
            [SchemaMapping(Name = "COND_1", Type = ReturnType.String)]
            public string Cond1 { get; set; }
            /// <summary>
            /// 條件二。
            /// </summary>
            [SchemaMapping(Name = "COND_2", Type = ReturnType.String)]
            public string Cond2 { get; set; }
            /// <summary>
            /// 條件三。
            /// </summary>
            [SchemaMapping(Name = "COND_3", Type = ReturnType.String)]
            public string Cond3 { get; set; }
            /// <summary>
            /// 條件四。
            /// </summary>
            [SchemaMapping(Name = "COND_4", Type = ReturnType.String)]
            public string Cond4 { get; set; }
            /// <summary>
            /// 條件五。
            /// </summary>
            [SchemaMapping(Name = "COND_5", Type = ReturnType.String)]
            public string Cond5 { get; set; }

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
        /// <param name="cat">種類。</param>
        /// <param name="src">來源。</param>
        /// <param name="type">類型（D:Debug N:Notice W:Warning E:Error F:Fatal）。</param>
        /// <param name="title">標題。</param>
        /// <param name="msg">訊息。</param>
        /// <param name="clientIP">使用者 IP（null 或 empty 將自動略過操作）。</param>
        /// <param name="cond1">條件一（null 或 empty 將自動略過操作）。</param>
        /// <param name="cond2">條件二（null 或 empty 將自動略過操作）。</param>
        /// <param name="cond3">條件三（null 或 empty 將自動略過操作）。</param>
        /// <param name="cond4">條件四（null 或 empty 將自動略過操作）。</param>
        /// <param name="cond5">條件五（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception> 
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception> 
        public Returner Add(ISystemId actorSId, string cat, string src, string type, string title, string msg, string clientIP, string cond1, string cond2, string cond3, string cond4, string cond5)
        {
            if (actorSId == null) throw new ArgumentNullException("actorSId");
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
                transSet.SmartAdd("CAT", cat, GenericDBType.VarChar, false);
                transSet.SmartAdd("SRC", src, GenericDBType.VarChar, false);
                transSet.SmartAdd("TYPE", type, GenericDBType.Char, false);
                transSet.SmartAdd("TITLE", title, GenericDBType.VarChar, false);
                transSet.SmartAdd("MSG", msg, GenericDBType.NVarChar, false);
                transSet.SmartAdd("CLIENT_IP", clientIP, GenericDBType.VarChar, true);
                transSet.SmartAdd("COND_1", cond1, GenericDBType.NVarChar, true);
                transSet.SmartAdd("COND_2", cond2, GenericDBType.NVarChar, true);
                transSet.SmartAdd("COND_3", cond3, GenericDBType.NVarChar, true);
                transSet.SmartAdd("COND_4", cond4, GenericDBType.NVarChar, true);
                transSet.SmartAdd("COND_5", cond5, GenericDBType.NVarChar, true);

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

        #region 查詢
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
            /// <param name="actorSId">操作人系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="cat">種類（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="src">來源（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="type">類型（D:Debug N:Notice W:Warning E:Error F:Fatal；若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="cond1">條件一（null 或 empty 則略過條件檢查）。</param>
            /// <param name="cond2">條件二（null 或 empty 則略過條件檢查）。</param>
            /// <param name="cond3">條件三（null 或 empty 則略過條件檢查）。</param>
            /// <param name="cond4">條件四（null 或 empty 則略過條件檢查）。</param>
            /// <param name="cond5">條件五（null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId actorSId, string cat, string src, string type, string cond1, string cond2, string cond3, string cond4, string cond5)
            {
                this.ActorSId = actorSId;
                this.Cat = cat;
                this.Src = src;
                this.Type = type;
                this.Cond1 = cond1;
                this.Cond2 = cond2;
                this.Cond3 = cond3;
                this.Cond4 = cond4;
                this.Cond5 = cond5;
            }

            /// <summary>
            /// 系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId ActorSId { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cat { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Src { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cond1 { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cond2 { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cond3 { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cond4 { get; set; }
            /// <summary>
            /// 帳號陣列集合。
            /// </summary>
            public string Cond5 { get; set; }
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
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder,
        IncludeScope includeScope, params string[] inquireColumns)
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
            custEntity.Append("FROM [SYS_LOG] ");

            var sqlConds = new List<string>();

            if (qConds.ActorSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CSID", SqlOperator.EqualTo, qConds.ActorSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.Cat))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CAT", SqlOperator.EqualTo, qConds.Cat, GenericDBType.VarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Src))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SRC", SqlOperator.EqualTo, qConds.Src, GenericDBType.VarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Type))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TYPE", SqlOperator.EqualTo, qConds.Type, GenericDBType.Char));
            }

            if (!string.IsNullOrEmpty(qConds.Cond1))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "COND_1", SqlOperator.EqualTo, qConds.Cond1, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Cond2))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "COND_2", SqlOperator.EqualTo, qConds.Cond2, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Cond3))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "COND_3", SqlOperator.EqualTo, qConds.Cond3, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Cond4))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "COND_4", SqlOperator.EqualTo, qConds.Cond4, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrEmpty(qConds.Cond5))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "COND_5", SqlOperator.EqualTo, qConds.Cond5, GenericDBType.NVarChar));
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
