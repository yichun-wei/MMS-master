using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.SystemEngines;
using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;
using Seec.Marketing.Erp;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// ERP 折扣。
        /// </summary>
        public const string ERP_DCT = "ERP_DCT";
    }

    /// <summary>
    /// ERP 折扣的類別實作。
    /// </summary>
    public class ErpDct : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ErpDct 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ErpDct(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ERP_DCT))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("DISCOUNT_ID", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase, IErpDct
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
            /// 折扣 ID。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_ID", Type = ReturnType.Long)]
            public long DiscountId { get; set; }
            /// <summary>
            /// 折扣名稱。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_NAME", Type = ReturnType.String)]
            public string DiscountName { get; set; }
            /// <summary>
            /// 自動計算折扣。
            /// </summary>
            [SchemaMapping(Name = "AUTOMATIC_DISCOUNT_FLAG", Type = ReturnType.Bool, AllowNull = true)]
            public bool? AutomaticDiscountFlag { get; set; }
            /// <summary>
            /// 允許修訂。
            /// </summary>
            [SchemaMapping(Name = "OVERRIDE_ALLOWED_FLAG", Type = ReturnType.Bool, AllowNull = true)]
            public bool? OverrideAllowedFlag { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            [SchemaMapping(Name = "PRICE_LIST_ID", Type = ReturnType.Long)]
            public long PriceListId { get; set; }
            /// <summary>
            /// 價目表名稱。
            /// </summary>
            [SchemaMapping(Name = "LIST_NAME", Type = ReturnType.String)]
            public string ListName { get; set; }
            /// <summary>
            /// 折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_TYPE", Type = ReturnType.Int)]
            public int DiscountType { get; set; }
            /// <summary>
            /// 折扣最後更新日期。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTime)]
            public DateTime LastUpdateDate { get; set; }

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
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param> 
        /// <param name="erpDctSId">指定的 ERP 折扣系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="discountId">折扣 ID。</param>
        /// <param name="discountName">折扣名稱。</param>
        /// <param name="automaticDiscountFlag">自動計算折扣（null 將自動略過操作）。</param>
        /// <param name="overrideAllowedFlag">允許修訂（null 將自動略過操作）。</param>
        /// <param name="priceListId">價目表 ID。</param>
        /// <param name="listName">價目表名稱。</param>
        /// <param name="discountType">折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。</param>
        /// <param name="lastUpdateDate">折扣最後更新日期。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpDctSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId erpDctSId, bool enabled, long discountId, string discountName, bool? automaticDiscountFlag, bool? overrideAllowedFlag, long priceListId, string listName, int discountType, DateTime lastUpdateDate)
        {
            if (erpDctSId == null) { throw new ArgumentNullException("erpDctSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(erpDctSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", erpDctSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("DISCOUNT_ID", discountId, GenericDBType.BigInt);
                transSet.SmartAdd("DISCOUNT_NAME", discountName, GenericDBType.VarChar, false);
                if (automaticDiscountFlag.HasValue)
                {
                    transSet.SmartAdd("AUTOMATIC_DISCOUNT_FLAG", automaticDiscountFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                if (overrideAllowedFlag.HasValue)
                {
                    transSet.SmartAdd("OVERRIDE_ALLOWED_FLAG", overrideAllowedFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt);
                transSet.SmartAdd("LIST_NAME", listName, GenericDBType.VarChar, false);
                transSet.SmartAdd("DISCOUNT_TYPE", discountType, GenericDBType.Int);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify (僅匯入用)
        /// <summary>
        /// 依指定的參數，修改一筆 ERP 折扣。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param>
        /// <param name="discountId">折扣 ID。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="discountName">折扣名稱。</param>
        /// <param name="automaticDiscountFlag">自動計算折扣（null 則直接設為 DBNull）。</param>
        /// <param name="overrideAllowedFlag">允許修訂（null 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID。</param>
        /// <param name="listName">價目表名稱。</param>
        /// <param name="discountType">折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。</param>
        /// <param name="lastUpdateDate">折扣最後更新日期。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, long discountId, bool enabled, string discountName, bool? automaticDiscountFlag, bool? overrideAllowedFlag, long priceListId, string listName, int discountType, DateTime lastUpdateDate)
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
                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("DISCOUNT_ID", discountId, GenericDBType.BigInt);
                transSet.SmartAdd("DISCOUNT_NAME", discountName, GenericDBType.VarChar, false);
                if (automaticDiscountFlag.HasValue)
                {
                    transSet.SmartAdd("AUTOMATIC_DISCOUNT_FLAG", automaticDiscountFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                if (overrideAllowedFlag.HasValue)
                {
                    transSet.SmartAdd("OVERRIDE_ALLOWED_FLAG", overrideAllowedFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt);
                transSet.SmartAdd("LIST_NAME", listName, GenericDBType.VarChar, false);
                transSet.SmartAdd("DISCOUNT_TYPE", discountType, GenericDBType.Int);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("DISCOUNT_ID", SqlCond.EqualTo, discountId, GenericDBType.BigInt, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
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
        /// 依指定的參數，修改一筆 ERP 折扣。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param>
        /// <param name="erpDctSId">ERP 折扣系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="discountId">折扣 ID。</param>
        /// <param name="discountName">折扣名稱。</param>
        /// <param name="automaticDiscountFlag">自動計算折扣（null 則直接設為 DBNull）。</param>
        /// <param name="overrideAllowedFlag">允許修訂（null 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID。</param>
        /// <param name="listName">價目表名稱。</param>
        /// <param name="discountType">折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。</param>
        /// <param name="lastUpdateDate">折扣最後更新日期。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpDctSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId erpDctSId, bool enabled, long discountId, string discountName, bool? automaticDiscountFlag, bool? overrideAllowedFlag, long priceListId, string listName, int discountType, DateTime lastUpdateDate)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (erpDctSId == null) { throw new ArgumentNullException("erpDctSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, erpDctSId);
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
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("DISCOUNT_ID", discountId, GenericDBType.BigInt);
                transSet.SmartAdd("DISCOUNT_NAME", discountName, GenericDBType.VarChar, false);
                if (automaticDiscountFlag.HasValue)
                {
                    transSet.SmartAdd("AUTOMATIC_DISCOUNT_FLAG", automaticDiscountFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                if (overrideAllowedFlag.HasValue)
                {
                    transSet.SmartAdd("OVERRIDE_ALLOWED_FLAG", overrideAllowedFlag.Value ? "Y" : "N", GenericDBType.Char, false);
                }
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt);
                transSet.SmartAdd("LIST_NAME", listName, GenericDBType.VarChar, false);
                transSet.SmartAdd("DISCOUNT_TYPE", discountType, GenericDBType.Int);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, erpDctSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="erpDctSIds">ERP 折扣系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="discountIds">折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="priceListIds">價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="discountTypes">折扣型態陣列集合（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] erpDctSIds, long[] discountIds, long[] priceListIds, int[] discountTypes)
            {
                this.ErpCusterSIds = erpDctSIds;
                this.DiscountIds = discountIds;
                this.PriceListIds = priceListIds;
                this.DiscountTypes = discountTypes;
            }

            /// <summary>
            /// ERP 折扣系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ErpCusterSIds { get; set; }
            /// <summary>
            /// 折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] DiscountIds { get; set; }
            /// <summary>
            /// 價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] PriceListIds { get; set; }
            /// <summary>
            /// 折扣型態陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] DiscountTypes { get; set; }
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
        SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [ERP_DCT].* ");
            custEntity.Append("FROM [ERP_DCT] ");

            var sqlConds = new List<string>();

            if (qConds.ErpCusterSIds != null && qConds.ErpCusterSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ErpCusterSIds), GenericDBType.Char));
            }

            if (qConds.DiscountIds != null && qConds.DiscountIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DISCOUNT_ID", SqlOperator.EqualTo, qConds.DiscountIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.PriceListIds != null && qConds.PriceListIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PRICE_LIST_ID", SqlOperator.EqualTo, qConds.PriceListIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.DiscountTypes != null && qConds.DiscountTypes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DISCOUNT_TYPE", SqlOperator.EqualTo, qConds.DiscountTypes.Select(q => (object)q).ToArray(), GenericDBType.Int));
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
