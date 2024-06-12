using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using EzCoding.SystemEngines;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 關聯表。
        /// </summary>
        public const string REL_TAB = "REL_TAB";
    }

    /// <summary>
    /// 關聯表的類別實作。
    /// </summary>
    public class RelTab : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.RelTab 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public RelTab(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.REL_TAB))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("SORT", Sort.Descending);
            sorting.Add("SID", Sort.Descending);

            base.DefaultSqlOrder = sorting;
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
            /// 關聯代碼（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區）。
            /// </summary>
            [SchemaMapping(Name = "REL_CODE", Type = ReturnType.Int)]
            public int RelCode { get; set; }
            /// <summary>
            /// 使用系統代號。
            /// </summary>
            [SchemaMapping(Name = "USE_SID", Type = ReturnType.SId)]
            public ISystemId UseSId { get; set; }
            /// <summary>
            /// 對象系統代號。
            /// </summary>
            [SchemaMapping(Name = "TGT_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId TgtSId { get; set; }
            /// <summary>
            /// 對象代碼。
            /// </summary>
            [SchemaMapping(Name = "TGT_CODE", Type = ReturnType.String)]
            public string TgtCode { get; set; }
            /// <summary>
            /// 關聯資訊。
            /// </summary>
            [SchemaMapping(Name = "REL_INFO", Type = ReturnType.String)]
            public string RelInfo { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            [SchemaMapping(Name = "SORT", Type = ReturnType.Int)]
            public int Sort { get; set; }

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
        }
        #endregion

        #region 關聯代碼列舉
        /// <summary>
        /// 關聯代碼列舉。
        /// </summary>
        public enum RelCodeOpt
        {
            /// <summary>
            /// 系統使用者/系統使用者群組。
            /// </summary>
            SysUser_SysUserGrp = 1,
            /// <summary>
            /// 系統使用者/內銷地區。
            /// </summary>
            SysUser_DomDist = 2
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="relCode">關聯代碼（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區）。</param> 
        /// <param name="useSId">使用系統代號。</param> 
        /// <param name="tgtSId">對象系統代號（null 將自動略過操作）。</param> 
        /// <param name="tgtCode">對象代碼（null 或 empty 將自動略過操作）。</param> 
        /// <param name="relInfo">關聯資訊。</param> 
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 useSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, int relCode, ISystemId useSId, ISystemId tgtSId, string tgtCode, string relInfo, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (useSId == null) { throw new ArgumentNullException("useSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, useSId);
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
                transSet.SmartAdd("REL_CODE", relCode, GenericDBType.Int);
                transSet.SmartAdd("USE_SID", useSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("TGT_SID", tgtSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("TGT_CODE", tgtCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("REL_INFO", relInfo, GenericDBType.NVarChar, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateCustSort
        /// <summary>
        /// 更新自訂排序。
        /// </summary>
        /// <param name="relTabSId">關聯表系統代號。</param> 
        /// <param name="val">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 relTabSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCustSort(ISystemId relTabSId, int val)
        {
            if (relTabSId == null) { throw new ArgumentNullException("relTabSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(relTabSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SORT", val, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, relTabSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// 依指定的參數刪除資料。
        /// </summary>
        /// <param name="relCode">關聯代碼（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區）。</param> 
        /// <param name="useSId">使用系統代號。</param> 
        /// <param name="tgtSId">對象系統代號。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner Delete(int relCode, ISystemId useSId, ISystemId tgtSId)
        {
            if (useSId == null || tgtSId == null)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("REL_CODE", SqlCond.EqualTo, relCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("USE_SID", SqlCond.EqualTo, useSId.ToString(), GenericDBType.Char, SqlCondsSet.And);
                condsMain.Add("TGT_SID", SqlCond.EqualTo, tgtSId.ToString(), GenericDBType.Char, SqlCondsSet.And);

                returner.ChangeInto(base.Delete(condsMain, true));

                return returner;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion

        #region DeleteByUseSId
        /// <summary>
        /// 依指定的使用系統代號刪除資料。
        /// </summary>
        /// <param name="relCode">關聯代碼（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區）。</param> 
        /// <param name="useSIds">使用系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByUseSId(int relCode, ISystemId[] useSIds)
        {
            if (useSIds == null || useSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("REL_CODE", SqlCond.EqualTo, relCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("USE_SID", true, SystemId.ToString(useSIds), GenericDBType.Char, SqlCondsSet.And);

                returner.ChangeInto(base.Delete(condsMain, true));

                return returner;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion

        #region DeleteByTgtSId
        /// <summary>
        /// 依指定的對象系統代號刪除資料。
        /// </summary>
        /// <param name="relCode">關聯代碼（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區）。</param> 
        /// <param name="tgtSIds">對象系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByTgtSId(int relCode, ISystemId[] tgtSIds)
        {
            if (tgtSIds == null || tgtSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("REL_CODE", SqlCond.EqualTo, relCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("TGT_SID", true, SystemId.ToString(tgtSIds), GenericDBType.Char, SqlCondsSet.And);

                returner.ChangeInto(base.Delete(condsMain, true));

                return returner;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
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
            /// <param name="relCodes">關聯代碼陣列集合（1:系統使用者/系統使用者群組 2:系統使用者/內銷地區; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="useSIds">使用系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="tgtSIds">對象系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="tgtCodes">對象代碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            public InfoConds(int[] relCodes, ISystemId[] useSIds, ISystemId[] tgtSIds, string[] tgtCodes)
            {
                this.RelCodes = relCodes;
                this.UseSIds = useSIds;
                this.TgtSIds = tgtSIds;
                this.TgtCodes = tgtCodes;
            }

            /// <summary>
            /// 關聯代碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] RelCodes { get; set; }
            /// <summary>
            /// 使用系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] UseSIds { get; set; }
            /// <summary>
            /// 對象系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] TgtSIds { get; set; }
            /// <summary>
            /// 對象代碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] TgtCodes { get; set; }
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

        #region GetGroupInfo
        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupInfo(InfoConds qConds, string[] fieldNames, Sort sort)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoCondsSet(qConds));

            SqlGroup grouping = new SqlGroup();
            SqlOrder sorting = new SqlOrder();

            for (int i = 0; i < fieldNames.Length; i++)
            {
                grouping.Add(fieldNames[i]);
                sorting.Add(fieldNames[i], sort);
            }

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetGroupBy(grouping, sorting, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion

        #region GetInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.AppendFormat("SELECT [REL_TAB].* ");
            custEntity.AppendFormat("FROM [REL_TAB] ");

            var sqlConds = new List<string>();

            if (qConds.RelCodes != null && qConds.RelCodes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "REL_CODE", SqlOperator.EqualTo, qConds.RelCodes.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.UseSIds != null && qConds.UseSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "USE_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.UseSIds), GenericDBType.Char));
            }

            if (qConds.TgtSIds != null && qConds.TgtSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TGT_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.TgtSIds), GenericDBType.Char));
            }

            if (qConds.TgtCodes != null && qConds.TgtCodes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "TGT_CODE", SqlOperator.EqualTo, qConds.TgtCodes, GenericDBType.VarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 系統使用者/系統使用者群組
        #region 系統使用者群組表格資料
        /// <summary>
        /// 系統使用者群組表格資料。
        /// </summary>
        public class SysUserGrpInfo : SysUserGrp.Info
        {
            /// <summary>
            /// 關聯表系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_SID", Type = ReturnType.SId)]
            public ISystemId RelTabSId { get; set; }
            /// <summary>
            /// 關聯表使用系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_USE_SID", Type = ReturnType.SId)]
            public ISystemId RelTabUseSId { get; set; }
            /// <summary>
            /// 關聯表對象系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_TGT_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId RelTabTgtSId { get; set; }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static SysUserGrpInfo Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<SysUserGrpInfo>(new SysUserGrpInfo(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static SysUserGrpInfo[] Binding(DataTable dTable)
            {
                List<SysUserGrpInfo> infos = new List<SysUserGrpInfo>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(SysUserGrpInfo.Binding(row));
                }

                return infos.ToArray();
            }
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class SysUserGrpInfoConds
        {
            ///// <summary>
            ///// 初始化 SysUserGrpInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public SysUserGrpInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 SysUserGrpInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="sysUserSIds">系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="sysUserGrpSIds">系統使用者群組系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
            public SysUserGrpInfoConds(ISystemId[] sysUserSIds, ISystemId[] sysUserGrpSIds, IncludeScope includeScope)
            {
                this.SysUserSIds = sysUserSIds;
                this.SysUserGrpSIds = sysUserGrpSIds;
                this.IncludeScope = includeScope;
            }

            /// <summary>
            /// 系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] SysUserSIds { get; set; }
            /// <summary>
            /// 系統使用者群組系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] SysUserGrpSIds { get; set; }
            /// <summary>
            /// 資料取得所包含的範圍（是否註解刪除或啟用中）。
            /// </summary>
            public IncludeScope IncludeScope { get; set; }
        }
        #endregion

        #region GetSysUserGrpInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserGrpInfo(SysUserGrpInfoConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserGrpInfoCondsSet(qConds));

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
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserGrpInfo(SysUserGrpInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserGrpInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserGrpInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserGrpInfoCount(SysUserGrpInfoConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserGrpInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserGrpInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserGrpInfoByCompoundSearch(SysUserGrpInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetSysUserGrpInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserGrpInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserGrpInfoByCompoundSearchCount(SysUserGrpInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetSysUserGrpInfoCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserGrpInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetSysUserGrpInfoCondsSet(SysUserGrpInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //以系統使用者群組的角度
            custEntity.AppendFormat("SELECT [SYS_USER_GRP].*, ");
            custEntity.AppendFormat("       [REL_TAB].[SID] AS [REL_TAB_SID], [REL_TAB].[USE_SID] AS [REL_TAB_USE_SID], [REL_TAB].[TGT_SID] AS [REL_TAB_TGT_SID] ");
            custEntity.AppendFormat("FROM [REL_TAB] ");
            custEntity.AppendFormat("     INNER JOIN [SYS_USER] ON [REL_TAB].[REL_CODE] = 1 AND [REL_TAB].[USE_SID] = [SYS_USER].[SID] ");
            custEntity.AppendFormat("     INNER JOIN [SYS_USER_GRP] ON [REL_TAB].[TGT_SID] = [SYS_USER_GRP].[SID] ");

            var sqlConds = new List<string>();

            if (qConds.SysUserSIds != null && qConds.SysUserSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("SYS_USER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.SysUserSIds), GenericDBType.Char));
            }

            if (qConds.SysUserGrpSIds != null && qConds.SysUserGrpSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("SYS_USER_GRP", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.SysUserGrpSIds), GenericDBType.Char));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "SYS_USER"));
            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "SYS_USER_GRP"));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 系統使用者/內銷地區
        #region 系統使用者內銷地區表格資料
        /// <summary>
        /// 系統使用者內銷地區表格資料。
        /// </summary>
        public class SysUserDomDistInfo : PubCat.Info
        {
            /// <summary>
            /// 關聯表系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_SID", Type = ReturnType.SId)]
            public ISystemId RelTabSId { get; set; }
            /// <summary>
            /// 關聯表使用系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_USE_SID", Type = ReturnType.SId)]
            public ISystemId RelTabUseSId { get; set; }
            /// <summary>
            /// 關聯表對象系統代號。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_TGT_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId RelTabTgtSId { get; set; }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static SysUserDomDistInfo Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<SysUserDomDistInfo>(new SysUserDomDistInfo(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static SysUserDomDistInfo[] Binding(DataTable dTable)
            {
                List<SysUserDomDistInfo> infos = new List<SysUserDomDistInfo>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(SysUserDomDistInfo.Binding(row));
                }

                return infos.ToArray();
            }
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class SysUserDomDistInfoConds
        {
            ///// <summary>
            ///// 初始化 SysUserDomDistInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public SysUserDomDistInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 SysUserDomDistInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="sysUserSIds">系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domDistSIds">內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
            public SysUserDomDistInfoConds(ISystemId[] sysUserSIds, ISystemId[] domDistSIds, IncludeScope includeScope)
            {
                this.SysUserSIds = sysUserSIds;
                this.DomDistSIds = domDistSIds;
                this.IncludeScope = includeScope;
            }

            /// <summary>
            /// 系統使用者系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] SysUserSIds { get; set; }
            /// <summary>
            /// 內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomDistSIds { get; set; }
            /// <summary>
            /// 資料取得所包含的範圍（是否註解刪除或啟用中）。
            /// </summary>
            public IncludeScope IncludeScope { get; set; }
        }
        #endregion

        #region GetSysUserDomDistInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserDomDistInfo(SysUserDomDistInfoConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserDomDistInfoCondsSet(qConds));

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
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserDomDistInfo(SysUserDomDistInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserDomDistInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserDomDistInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserDomDistInfoCount(SysUserDomDistInfoConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetSysUserDomDistInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserDomDistInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserDomDistInfoByCompoundSearch(SysUserDomDistInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetSysUserDomDistInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserDomDistInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSysUserDomDistInfoByCompoundSearchCount(SysUserDomDistInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetSysUserDomDistInfoCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetSysUserDomDistInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetSysUserDomDistInfoCondsSet(SysUserDomDistInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //以通用分類的角度
            custEntity.AppendFormat("SELECT [PUB_CAT].*, ");
            custEntity.AppendFormat("       [REL_TAB].[SID] AS [REL_TAB_SID], [REL_TAB].[USE_SID] AS [REL_TAB_USE_SID], [REL_TAB].[TGT_SID] AS [REL_TAB_TGT_SID] ");
            custEntity.AppendFormat("FROM [REL_TAB] ");
            custEntity.AppendFormat("     INNER JOIN [SYS_USER] ON [REL_TAB].[REL_CODE] = 2 AND [REL_TAB].[USE_SID] = [SYS_USER].[SID] ");
            custEntity.AppendFormat("     INNER JOIN [PUB_CAT] ON [REL_TAB].[TGT_SID] = [PUB_CAT].[SID] ");

            var sqlConds = new List<string>();

            if (qConds.SysUserSIds != null && qConds.SysUserSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("SYS_USER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.SysUserSIds), GenericDBType.Char));
            }

            if (qConds.DomDistSIds != null && qConds.DomDistSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PUB_CAT", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomDistSIds), GenericDBType.Char));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "SYS_USER"));
            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "PUB_CAT"));

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
