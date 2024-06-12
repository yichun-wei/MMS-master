using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;
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
        /// 外銷生產單明細。
        /// </summary>
        public const string EXT_PROD_ORDER_DET = "EXT_PROD_ORDER_DET";
    }

    /// <summary>
    /// 外銷生產單明細的類別實作。
    /// </summary>
    public class ExtProdOrderDet : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtProdOrderDet(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.EXT_PROD_ORDER_DET))
        {
            base.DefaultSqlOrder = new SqlOrder("SORT", Sort.Ascending);
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
            /// 外銷生產單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_PROD_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId ExtProdOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            [SchemaMapping(Name = "SOURCE", Type = ReturnType.Int)]
            public int Source { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORG_CODE", Type = ReturnType.String)]
            public string OrgCode { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            [SchemaMapping(Name = "MODEL", Type = ReturnType.String)]
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "PART_NO", Type = ReturnType.String)]
            public string PartNo { get; set; }
            /// <summary>
            /// 需求量。
            /// </summary>
            [SchemaMapping(Name = "QTY", Type = ReturnType.Int)]
            public int Qty { get; set; }
            /// <summary>
            /// 累計生產量。
            /// </summary>
            [SchemaMapping(Name = "CUM_PROD_QTY", Type = ReturnType.Int)]
            public int CumProdQty { get; set; }
            /// <summary>
            /// 生產量。
            /// </summary>
            [SchemaMapping(Name = "PROD_QTY", Type = ReturnType.Int)]
            public int ProdQty { get; set; }
            /// <summary>
            /// 預計繳庫日。
            /// </summary>
            [SchemaMapping(Name = "EST_FPMS_DATE", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? EstFpmsDate { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            [SchemaMapping(Name = "SORT", Type = ReturnType.Int)]
            public int Sort { get; set; }

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
            /// 外銷生產單系統代號。
            /// </summary>
            public ISystemId ExtProdOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            public string OrgCode { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 需求量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 累計生產量。
            /// </summary>
            public int? CumProdQty { get; set; }
            /// <summary>
            /// 生產量。
            /// </summary>
            public int? ProdQty { get; set; }
            /// <summary>
            /// 預計繳庫日。
            /// </summary>
            public DateTime? EstFpmsDate { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            public int? Sort { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="extProdOrderDetSId">指定的外銷生產單明細系統代號。</param>
        /// <param name="extProdOrderSId">外銷生產單系統代號。</param>
        /// <param name="source">來源（1:一般品項 2:手動新增）。</param>
        /// <param name="orgCode">料號所屬製造組織代碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="model">型號（null 或 empty 將自動略過操作）。</param>
        /// <param name="partNo">料號。</param>
        /// <param name="qty">需求量。</param>
        /// <param name="cumProdQty">累計生產量。</param>
        /// <param name="prodQty">生產量。</param>
        /// <param name="estFpmsDate">預計繳庫日（null 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extProdOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId extProdOrderDetSId, ISystemId extProdOrderSId, int source, string orgCode, string model, string partNo, int qty, int cumProdQty, int prodQty, DateTime? estFpmsDate, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extProdOrderDetSId == null) { throw new ArgumentNullException("extProdOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extProdOrderDetSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", extProdOrderDetSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("EXT_PROD_ORDER_SID", extProdOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("SOURCE", source, GenericDBType.Int);
                transSet.SmartAdd("ORG_CODE", orgCode, GenericDBType.NVarChar, true);
                transSet.SmartAdd("MODEL", model, GenericDBType.NVarChar, true);
                transSet.SmartAdd("PART_NO", partNo, GenericDBType.VarChar, true);
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("CUM_PROD_QTY", cumProdQty, GenericDBType.Int);
                transSet.SmartAdd("PROD_QTY", prodQty, GenericDBType.Int);
                transSet.SmartAdd("EST_FPMS_DATE", estFpmsDate, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true);
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

        #region Modify
        /// <summary>
        /// 依指定的參數，修改一筆外銷生產單明細。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extProdOrderDetSId">外銷生產單明細系統代號。</param>
        /// <param name="model">型號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="qty">需求量。</param>
        /// <param name="cumProdQty">累計生產量。</param>
        /// <param name="prodQty">生產量。</param>
        /// <param name="estFpmsDate">預計繳庫日（null 則直接設為 DBNull）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extProdOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extProdOrderDetSId, string model, int qty, int cumProdQty, int prodQty, DateTime? estFpmsDate, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extProdOrderDetSId == null) { throw new ArgumentNullException("extProdOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extProdOrderDetSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MODEL", model, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("CUM_PROD_QTY", cumProdQty, GenericDBType.Int);
                transSet.SmartAdd("PROD_QTY", prodQty, GenericDBType.Int);
                transSet.SmartAdd("EST_FPMS_DATE", estFpmsDate, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extProdOrderDetSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region DeleteByExtProdOrderSId
        /// <summary>
        /// 依指定的外銷生產單系統代號刪除資料。
        /// </summary>
        /// <param name="extProdOrderSIds">外銷生產單系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByExtProdOrderSId(ISystemId[] extProdOrderSIds)
        {
            if (extProdOrderSIds == null || extProdOrderSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXT_PROD_ORDER_SID", true, SystemId.ToString(extProdOrderSIds), GenericDBType.Char, SqlCondsSet.And);

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
            /// <param name="extProdOrderDetSIds">外銷生產單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extProdOrderSId">外銷生產單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] extProdOrderDetSIds, ISystemId extProdOrderSId, int? source, string partNo)
            {
                this.ExtQuotnDetSIds = extProdOrderDetSIds;
                this.ExtProdOrderSId = extProdOrderSId;
                this.Source = source;
                this.PartNo = partNo;
            }

            /// <summary>
            /// 外銷生產單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 外銷生產單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtProdOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 料號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string PartNo { get; set; }
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

            custEntity.Append("SELECT [EXT_PROD_ORDER_DET].* ");
            custEntity.Append("FROM [EXT_PROD_ORDER_DET] ");

            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.ExtProdOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXT_PROD_ORDER_SID", SqlOperator.EqualTo, qConds.ExtProdOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (!string.IsNullOrWhiteSpace(qConds.PartNo))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PART_NO", SqlOperator.EqualTo, qConds.PartNo, GenericDBType.VarChar));
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
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class InfoView : Info
        {
            /// <summary>
            /// 是否已取消。
            /// </summary>
            [SchemaMapping(Name = "IS_CANCEL", Type = ReturnType.Bool)]
            public bool IsCancel { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InventoryItemId { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
            /// <summary>
            /// 料號重量。
            /// </summary>
            [SchemaMapping(Name = "UNIT_WEIGHT", Type = ReturnType.Float, AllowNull = true)]
            public float? UnitWeight { get; set; }
            /// <summary>
            /// 重量單位。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT_UOM_CODE", Type = ReturnType.String)]
            public string WeightUomCode { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
            ///// <summary>
            ///// 即時計算的累計生產量。
            ///// 同報價單所有同品項已確認生產的累計生產量。
            ///// </summary>
            //[SchemaMapping(Name = "RT_CUM_PROD_QTY", Type = ReturnType.Int)]
            //public int RTCumProdQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static InfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<InfoView>(new InfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static InfoView[] Binding(DataTable dTable)
            {
                List<InfoView> infos = new List<InfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(InfoView.Binding(row));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class InfoViewConds
        {
            ///// <summary>
            ///// 初始化 InfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public InfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 InfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extProdOrderDetSIds">外銷生產單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extProdOrderSId">外銷生產單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] extProdOrderDetSIds, ISystemId extProdOrderSId, int? source, string partNo, bool? isCancel)
            {
                this.ExtQuotnDetSIds = extProdOrderDetSIds;
                this.ExtProdOrderSId = extProdOrderSId;
                this.Source = source;
                this.PartNo = partNo;
                this.IsCancel = isCancel;
            }

            /// <summary>
            /// 外銷生產單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 外銷生產單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtProdOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 料號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 是否已取消（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsCancel { get; set; }
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
        public Returner GetInfoViewByCompoundSearch(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
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

            custEntity.Append(@"
                SELECT [PROD_DET].* 
                       , [QUOTN].[IS_CANCEL] 
                       , [INV].[INVENTORY_ITEM_ID], [INV].[DESCRIPTION] AS [SUMMARY], [INV].[UNIT_WEIGHT], [INV].[WEIGHT_UOM_CODE], [INV].[ORGANIZATION_CODE] 

	                   --累計生產量 (已確認生產), 不區分報價單所有同品項.
	                   --, (SELECT COALESCE(SUM([CUM_PROD_DET].[PROD_QTY]), 0) FROM [EXT_PROD_ORDER_DET] [CUM_PROD_DET] INNER JOIN [EXT_PROD_ORDER] [CUM_PROD] ON [CUM_PROD].[SID] = [CUM_PROD_DET].[EXT_PROD_ORDER_SID] WHERE [CUM_PROD].[STATUS] = 2 AND [CUM_PROD_DET].[PART_NO] = [PROD_DET].[PART_NO]) AS [CUM_PROD_QTY]

	                   --累計生產量 (已確認生產), 同報價單所有同品項. (即時計算的)
	                   --, (SELECT COALESCE(SUM([CUM_PROD_DET].[PROD_QTY]), 0) FROM [EXT_PROD_ORDER_DET] [CUM_PROD_DET] INNER JOIN [EXT_PROD_ORDER] [CUM_PROD] ON [CUM_PROD].[SID] = [CUM_PROD_DET].[EXT_PROD_ORDER_SID] INNER JOIN [EXT_ORDER] [CUM_ODR] ON [CUM_ODR].[SID] = [CUM_PROD].[EXT_ORDER_SID] WHERE [QUOTN].[SID] = [CUM_ODR].[EXT_QUOTN_SID] AND [CUM_PROD].[STATUS] = 2 AND [CUM_PROD_DET].[PART_NO] = [PROD_DET].[PART_NO]) AS [RT_CUM_PROD_QTY]

	                   --已被外銷出貨單的選擇的總數量 (不取已註刪的資料), 所有出貨單的同品項, 不區分訂單.
	                   --, (SELECT COALESCE(SUM([SHIPPING_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] [SHIPPING_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIPPING] ON [SHIPPING].[SID] = [SHIPPING_DET].[EXT_SHIPPING_ORDER_SID] AND [SHIPPING].[MDELED] = 'N' AND [SHIPPING_DET].[PART_NO] = [PROD_DET].[PART_NO]) AS [TOTAL_SHIPPED_QTY]

                FROM [EXT_PROD_ORDER_DET] [PROD_DET]
                     INNER JOIN [EXT_PROD_ORDER] [PROD] ON [PROD].[SID] = [PROD_DET].[EXT_PROD_ORDER_SID] 
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [PROD].[EXT_ORDER_SID] 
                     INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] 
                     INNER JOIN [ERP_INV] [INV] ON [INV].[ITEM] = [PROD_DET].[PART_NO] 
            ");

            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PROD_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.ExtProdOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD_DET", "EXT_PROD_ORDER_SID", SqlOperator.EqualTo, qConds.ExtProdOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("PROD_DET", "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (!string.IsNullOrWhiteSpace(qConds.PartNo))
            {
                sqlConds.Add(custEntity.BuildConds("PROD_DET", "PART_NO", SqlOperator.EqualTo, qConds.PartNo, GenericDBType.VarChar));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("QUOTN", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
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
