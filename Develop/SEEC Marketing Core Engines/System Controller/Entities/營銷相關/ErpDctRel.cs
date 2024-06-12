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
        /// ERP 折扣關聯。
        /// </summary>
        public const string ERP_DCT_REL = "ERP_DCT_REL";
    }

    /// <summary>
    /// ERP 折扣關聯的類別實作。
    /// </summary>
    public class ErpDctRel : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ErpDctRel 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ErpDctRel(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ERP_DCT_REL))
        {
            base.DefaultSqlOrder = new SqlOrder("SID", Sort.Ascending);
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
            /// 關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。
            /// </summary>
            [SchemaMapping(Name = "REL_CODE", Type = ReturnType.Int)]
            public int RelCode { get; set; }
            /// <summary>
            /// 使用系統代號。
            /// </summary>
            [SchemaMapping(Name = "USE_SID", Type = ReturnType.SId)]
            public ISystemId UseSId { get; set; }
            /// <summary>
            /// 折扣 ID。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_ID", Type = ReturnType.Long)]
            public long DiscountId { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float)]
            public float Discount { get; set; }

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
            /// 關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。
            /// </summary>
            public int? RelCode { get; set; }
            /// <summary>
            /// 使用系統代號。
            /// </summary>
            public ISystemId UseSId { get; set; }
            /// <summary>
            /// 折扣 ID。
            /// </summary>
            public long? DiscountId { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            public float? Discount { get; set; }
        }
        #endregion

        #region 關聯代碼列舉
        /// <summary>
        /// 關聯代碼列舉。
        /// </summary>
        public enum RelCodeOpt
        {
            /// <summary>
            /// 內銷訂單/表頭折扣。
            /// </summary>
            DomOrder_HeaderDiscount = 1,
            /// <summary>
            /// 外銷出貨單/表頭折扣。
            /// </summary>
            ExtShippingOrder_HeaderDiscount = 2
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="relCode">關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。</param>
        /// <param name="useSId">使用系統代號。</param>
        /// <param name="discountId">折扣 ID。</param>
        /// <param name="discount">折扣。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 useSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, int relCode, ISystemId useSId, long discountId, float discount)
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
                transSet.SmartAdd("USE_SID", useSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("DISCOUNT_ID", discountId, GenericDBType.BigInt);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);

                returner.ChangeInto(base.Add(transSet, true));
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
        /// <param name="relCode">關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。</param> 
        /// <param name="useSId">使用系統代號。</param> 
        /// <param name="discountId">折扣 ID。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner Delete(int relCode, ISystemId useSId, long? discountId)
        {
            if (useSId == null || discountId == null)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("REL_CODE", SqlCond.EqualTo, relCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("USE_SID", SqlCond.EqualTo, useSId.ToString(), GenericDBType.Char, SqlCondsSet.And);
                if (discountId.HasValue)
                {
                    condsMain.Add("TGT_SID", SqlCond.EqualTo, discountId.Value, GenericDBType.BigInt, SqlCondsSet.And);
                }

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
        /// <param name="relCode">關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。</param> 
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

        #region DeleteByDiscountId
        /// <summary>
        /// 依指定的折扣 ID 刪除資料。
        /// </summary>
        /// <param name="relCode">關聯代碼（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣）。</param> 
        /// <param name="discountIds">折扣 ID 陣列集合。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByDiscountId(int relCode, long[] discountIds)
        {
            if (discountIds == null || discountIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("REL_CODE", SqlCond.EqualTo, relCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("DISCOUNT_ID", true, discountIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt, SqlCondsSet.And);

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
            /// <param name="relCodes">關聯代碼陣列集合（1:內銷訂單/表頭折扣 2:外銷出貨單/表頭折扣; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="useSIds">使用系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="discountIds">折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoConds(int[] relCodes, ISystemId[] useSIds, long[] discountIds)
            {
                this.RelCodes = relCodes;
                this.UseSIds = useSIds;
                this.DiscountIds = discountIds;
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
            /// 折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] DiscountIds { get; set; }
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

            custEntity.AppendFormat("SELECT [ERP_DCT_REL].* ");
            custEntity.AppendFormat("FROM [ERP_DCT_REL] ");

            var sqlConds = new List<string>();

            if (qConds.RelCodes != null && qConds.RelCodes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "REL_CODE", SqlOperator.EqualTo, qConds.RelCodes.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.UseSIds != null && qConds.UseSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "USE_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.UseSIds), GenericDBType.Char));
            }

            if (qConds.DiscountIds != null && qConds.DiscountIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DISCOUNT_ID", SqlOperator.EqualTo, qConds.DiscountIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 內銷訂單/表頭折扣
        #region ERP 折扣 (表頭折扣) 表格資料
        /// <summary>
        /// ERP 折扣 (表頭折扣) 表格資料。
        /// </summary>
        public class DomHeaderDiscountInfo : ErpDct.Info
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
            /// 關聯表折扣。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_DISCOUNT", Type = ReturnType.Float)]
            public float Discount { get; set; }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static DomHeaderDiscountInfo Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<DomHeaderDiscountInfo>(new DomHeaderDiscountInfo(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static DomHeaderDiscountInfo[] Binding(DataTable dTable)
            {
                List<DomHeaderDiscountInfo> infos = new List<DomHeaderDiscountInfo>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(DomHeaderDiscountInfo.Binding(row));
                }

                return infos.ToArray();
            }
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class DomHeaderDiscountInfoConds
        {
            ///// <summary>
            ///// 初始化 DomHeaderDiscountInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public DomHeaderDiscountInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 DomHeaderDiscountInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="domOrderSIds">內銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="discountIds">折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
            public DomHeaderDiscountInfoConds(ISystemId[] domOrderSIds, long[] discountIds, IncludeScope includeScope)
            {
                this.DomOrderSIds = domOrderSIds;
                this.DiscountIds = discountIds;
                this.IncludeScope = includeScope;
            }

            /// <summary>
            /// 內銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderSIds { get; set; }
            /// <summary>
            /// 折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] DiscountIds { get; set; }
            /// <summary>
            /// 資料取得所包含的範圍（是否註解刪除或啟用中）。
            /// </summary>
            public IncludeScope IncludeScope { get; set; }
        }
        #endregion

        #region GetDomHeaderDiscountInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetDomHeaderDiscountInfo(DomHeaderDiscountInfoConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetDomHeaderDiscountInfoCondsSet(qConds));

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
        public Returner GetDomHeaderDiscountInfo(DomHeaderDiscountInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetDomHeaderDiscountInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetDomHeaderDiscountInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetDomHeaderDiscountInfoCount(DomHeaderDiscountInfoConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetDomHeaderDiscountInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetDomHeaderDiscountInfoByCompoundSearch
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
        public Returner GetDomHeaderDiscountInfoByCompoundSearch(DomHeaderDiscountInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetDomHeaderDiscountInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetDomHeaderDiscountInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetDomHeaderDiscountInfoByCompoundSearchCount(DomHeaderDiscountInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetDomHeaderDiscountInfoCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetDomHeaderDiscountInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetDomHeaderDiscountInfoCondsSet(DomHeaderDiscountInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //以 ERP 折扣的角度
            custEntity.AppendFormat("SELECT [ERP_DCT].*, ");
            custEntity.AppendFormat("       [ERP_DCT_REL].[SID] AS [REL_TAB_SID], [ERP_DCT_REL].[USE_SID] AS [REL_TAB_USE_SID], [ERP_DCT_REL].[DISCOUNT] AS [REL_TAB_DISCOUNT] ");
            custEntity.AppendFormat("FROM [ERP_DCT_REL] ");
            custEntity.AppendFormat("     INNER JOIN [DOM_ORDER] ON [ERP_DCT_REL].[REL_CODE] = 1 AND [ERP_DCT_REL].[USE_SID] = [DOM_ORDER].[SID] ");
            custEntity.AppendFormat("     INNER JOIN [ERP_DCT] ON [ERP_DCT_REL].[DISCOUNT_ID] = [ERP_DCT].[DISCOUNT_ID] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderSIds != null && qConds.DomOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderSIds), GenericDBType.Char));
            }

            if (qConds.DiscountIds != null && qConds.DiscountIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_DCT", "DISCOUNT_ID", SqlOperator.EqualTo, qConds.DiscountIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "DOM_ORDER"));
            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "ERP_DCT"));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 外銷出貨單/表頭折扣
        #region ERP 折扣 (表頭折扣) 表格資料
        /// <summary>
        /// ERP 折扣 (表頭折扣) 表格資料。
        /// </summary>
        public class ExtShippingHeaderDiscountInfo : ErpDct.Info
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
            /// 關聯表折扣。
            /// </summary>
            [SchemaMapping(Name = "REL_TAB_DISCOUNT", Type = ReturnType.Float)]
            public float Discount { get; set; }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static ExtShippingHeaderDiscountInfo Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<ExtShippingHeaderDiscountInfo>(new ExtShippingHeaderDiscountInfo(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static ExtShippingHeaderDiscountInfo[] Binding(DataTable dTable)
            {
                List<ExtShippingHeaderDiscountInfo> infos = new List<ExtShippingHeaderDiscountInfo>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(ExtShippingHeaderDiscountInfo.Binding(row));
                }

                return infos.ToArray();
            }
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class ExtShippingHeaderDiscountInfoConds
        {
            ///// <summary>
            ///// 初始化 ExtShippingHeaderDiscountInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public ExtShippingHeaderDiscountInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 ExtShippingHeaderDiscountInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="extShippingOrders">外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="discountIds">折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
            public ExtShippingHeaderDiscountInfoConds(ISystemId[] extShippingOrders, long[] discountIds, IncludeScope includeScope)
            {
                this.ExtShippingOrderSIds = extShippingOrders;
                this.DiscountIds = discountIds;
                this.IncludeScope = includeScope;
            }

            /// <summary>
            /// 外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtShippingOrderSIds { get; set; }
            /// <summary>
            /// 折扣 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] DiscountIds { get; set; }
            /// <summary>
            /// 資料取得所包含的範圍（是否註解刪除或啟用中）。
            /// </summary>
            public IncludeScope IncludeScope { get; set; }
        }
        #endregion

        #region GetExtShippingHeaderDiscountInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetExtShippingHeaderDiscountInfo(ExtShippingHeaderDiscountInfoConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetExtShippingHeaderDiscountInfoCondsSet(qConds));

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
        public Returner GetExtShippingHeaderDiscountInfo(ExtShippingHeaderDiscountInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetExtShippingHeaderDiscountInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetExtShippingHeaderDiscountInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetExtShippingHeaderDiscountInfoCount(ExtShippingHeaderDiscountInfoConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetExtShippingHeaderDiscountInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetExtShippingHeaderDiscountInfoByCompoundSearch
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
        public Returner GetExtShippingHeaderDiscountInfoByCompoundSearch(ExtShippingHeaderDiscountInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetExtShippingHeaderDiscountInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetExtShippingHeaderDiscountInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetExtShippingHeaderDiscountInfoByCompoundSearchCount(ExtShippingHeaderDiscountInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetExtShippingHeaderDiscountInfoCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetExtShippingHeaderDiscountInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetExtShippingHeaderDiscountInfoCondsSet(ExtShippingHeaderDiscountInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //以 ERP 折扣的角度
            custEntity.AppendFormat("SELECT [ERP_DCT].*, ");
            custEntity.AppendFormat("       [ERP_DCT_REL].[SID] AS [REL_TAB_SID], [ERP_DCT_REL].[USE_SID] AS [REL_TAB_USE_SID], [ERP_DCT_REL].[DISCOUNT] AS [REL_TAB_DISCOUNT] ");
            custEntity.AppendFormat("FROM [ERP_DCT_REL] ");
            custEntity.AppendFormat("     INNER JOIN [EXT_SHIPPING_ORDER] ON [ERP_DCT_REL].[REL_CODE] = 2 AND [ERP_DCT_REL].[USE_SID] = [EXT_SHIPPING_ORDER].[SID] ");
            custEntity.AppendFormat("     INNER JOIN [ERP_DCT] ON [ERP_DCT_REL].[DISCOUNT_ID] = [ERP_DCT].[DISCOUNT_ID] ");

            var sqlConds = new List<string>();

            if (qConds.ExtShippingOrderSIds != null && qConds.ExtShippingOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtShippingOrderSIds), GenericDBType.Char));
            }

            if (qConds.DiscountIds != null && qConds.DiscountIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_DCT", "DISCOUNT_ID", SqlOperator.EqualTo, qConds.DiscountIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "EXT_SHIPPING_ORDER"));
            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "ERP_DCT"));

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
