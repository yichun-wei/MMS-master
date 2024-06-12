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
        /// 內銷訂單明細。
        /// </summary>
        public const string DOM_ORDER_DET = "DOM_ORDER_DET";
    }

    /// <summary>
    /// 內銷訂單明細的類別實作。
    /// </summary>
    public class DomOrderDet : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public DomOrderDet(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.DOM_ORDER_DET))
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
            /// 內銷訂單系統代號。
            /// </summary>
            [SchemaMapping(Name = "DOM_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId DomOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:專案報價品項）。
            /// </summary>
            [SchemaMapping(Name = "SOURCE", Type = ReturnType.Int)]
            public int Source { get; set; }
            /// <summary>
            /// 備貨單明細系統代號。
            /// </summary>
            [SchemaMapping(Name = "PG_ORDER_DET_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId PGOrderDetSId { get; set; }
            /// <summary>
            /// 報價單號碼。
            /// </summary>
            [SchemaMapping(Name = "QUOTENUMBER", Type = ReturnType.String)]
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次。
            /// </summary>
            [SchemaMapping(Name = "QUOTEITEMID", Type = ReturnType.String)]
            public string QuoteItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "PART_NO", Type = ReturnType.String)]
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            [SchemaMapping(Name = "QTY", Type = ReturnType.Int)]
            public int Qty { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            [SchemaMapping(Name = "LIST_PRICE", Type = ReturnType.Float)]
            public float ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            [SchemaMapping(Name = "UNIT_PRICE", Type = ReturnType.Float)]
            public float UnitPrice { get; set; }
            /// <summary>
            /// 預設明細折扣。
            /// </summary>
            [SchemaMapping(Name = "DEF_DCT", Type = ReturnType.Float, AllowNull = true)]
            public float? DefDct { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float, AllowNull = true)]
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            [SchemaMapping(Name = "PAID_AMT", Type = ReturnType.Float)]
            public float PaidAmt { get; set; }
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
            /// 內銷訂單系統代號。
            /// </summary>
            public ISystemId DomOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:專案報價品項）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 備貨單明細系統代號。
            /// </summary>
            public ISystemId PGOrderDetSId { get; set; }
            /// <summary>
            /// 報價單號碼。
            /// </summary>
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次。
            /// </summary>
            public string QuoteItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            public float? ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            public float? UnitPrice { get; set; }
            /// <summary>
            /// 預設明細折扣。
            /// </summary>
            public float? DefDct { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            public float? PaidAmt { get; set; }
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
        /// <param name="domOrderDetSId">指定的內銷訂單明細系統代號。</param>
        /// <param name="domOrderSId">內銷訂單系統代號。</param>
        /// <param name="source">來源（1:一般品項 2:專案報價品項）。</param>
        /// <param name="pgOrderDetSId">備貨單明細系統代號（null 將自動略過操作）。</param>
        /// <param name="quoteNumber">報價單號碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="quoteItemId">報價單明細項次（null 或 empty 將自動略過操作）。</param>
        /// <param name="partNo">料號。</param>
        /// <param name="qty">數量。</param>
        /// <param name="listPrice">牌價。</param>
        /// <param name="unitPrice">單價。</param>
        /// <param name="defDct">預設明細折扣（null 將自動略過操作）。</param>
        /// <param name="discount">折扣（null 將自動略過操作）。</param>
        /// <param name="paidAmt">實付金額。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 domOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId domOrderDetSId, ISystemId domOrderSId, int source, ISystemId pgOrderDetSId, string quoteNumber, string quoteItemId, string partNo, int qty, float listPrice, float unitPrice, float? defDct, float? discount, float paidAmt, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (domOrderDetSId == null) { throw new ArgumentNullException("domOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, domOrderDetSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", domOrderDetSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("DOM_ORDER_SID", domOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("SOURCE", source, GenericDBType.Int);
                transSet.SmartAdd("PG_ORDER_DET_SID", pgOrderDetSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("QUOTENUMBER", quoteNumber, GenericDBType.NVarChar, true);
                transSet.SmartAdd("QUOTEITEMID", quoteItemId, GenericDBType.NVarChar, true);
                transSet.SmartAdd("PART_NO", partNo, GenericDBType.VarChar, false);
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("LIST_PRICE", listPrice, GenericDBType.Float);
                transSet.SmartAdd("UNIT_PRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("DEF_DCT", defDct, GenericDBType.Float);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);
                transSet.SmartAdd("PAID_AMT", paidAmt, GenericDBType.Float);
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
        /// 依指定的參數，修改一筆內銷訂單明細。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="domOrderDetSId">內銷訂單明細系統代號。</param>
        /// <param name="qty">數量。</param>
        /// <param name="listPrice">牌價。</param>
        /// <param name="unitPrice">單價。</param>
        /// <param name="defDct">預設明細折扣（null 將自動略過操作）。</param>
        /// <param name="discount">折扣（null 則直接設為 DBNull）。</param>
        /// <param name="paidAmt">實付金額。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 domOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId domOrderDetSId, int qty, float listPrice, float unitPrice, float? defDct, float? discount, float paidAmt, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (domOrderDetSId == null) { throw new ArgumentNullException("domOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, domOrderDetSId);
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
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("LIST_PRICE", listPrice, GenericDBType.Float);
                transSet.SmartAdd("UNIT_PRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("DEF_DCT", defDct, GenericDBType.Float, true);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float, true);
                transSet.SmartAdd("PAID_AMT", paidAmt, GenericDBType.Float);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, domOrderDetSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region DeleteByDomOrderSId
        /// <summary>
        /// 依指定的內銷訂單系統代號刪除資料。
        /// </summary>
        /// <param name="domOrderSIds">內銷訂單系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByDomOrderSId(ISystemId[] domOrderSIds)
        {
            if (domOrderSIds == null || domOrderSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("DOM_ORDER_SID", true, SystemId.ToString(domOrderSIds), GenericDBType.Char, SqlCondsSet.And);

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
            /// <param name="domOrderDetSIds">內銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domOrderSId">內銷訂單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:專案報價品項; 若為 null 則略過條件檢查）。</param>
            /// <param name="pgOrderDetSId">備貨單明細系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="quoteNumber">報價單號碼（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="quoteItemId">報價單明細項次（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] domOrderDetSIds, ISystemId domOrderSId, int? source, ISystemId pgOrderDetSId, string quoteNumber, string quoteItemId, string partNo)
            {
                this.DomOrderDetSIds = domOrderDetSIds;
                this.DomOrderSId = domOrderSId;
                this.Source = source;
                this.PGOrderDetSId = pgOrderDetSId;
                this.QuoteNumber = quoteNumber;
                this.QuoteItemId = quoteItemId;
                this.PartNo = partNo;
            }

            /// <summary>
            /// 內銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderDetSIds { get; set; }
            /// <summary>
            /// 內銷訂單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId DomOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:專案報價品項; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 備貨單明細系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PGOrderDetSId { get; set; }
            /// <summary>
            /// 報價單號碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteItemId { get; set; }
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

            custEntity.Append("SELECT [DOM_ORDER_DET].* ");
            custEntity.Append("FROM [DOM_ORDER_DET] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderDetSIds != null && qConds.DomOrderDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderDetSIds), GenericDBType.Char));
            }

            if (qConds.DomOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DOM_ORDER_SID", SqlOperator.EqualTo, qConds.DomOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (qConds.PGOrderDetSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PG_ORDER_DET_SID", SqlOperator.EqualTo, qConds.PGOrderDetSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrWhiteSpace(qConds.QuoteNumber))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumber, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.QuoteItemId))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEITEMID", SqlOperator.EqualTo, qConds.QuoteItemId, GenericDBType.NVarChar));
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
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long)]
            public long InventoryItemId { get; set; }
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
            /// 備貨單系統代號。
            /// </summary>
            [SchemaMapping(Name = "PG_ORDER_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId PGOrderSId { get; set; }
            /// <summary>
            /// 備貨單編號。
            /// </summary>
            [SchemaMapping(Name = "PG_ORDER_ODR_NO", Type = ReturnType.String)]
            public string PGOrderOdrNo { get; set; }
            /// <summary>
            /// XS型號。
            /// </summary>
            [SchemaMapping(Name = "XSD_ITEM", Type = ReturnType.String)]
            public string XSDItem { get; set; }

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
            /// <param name="domOrderDetSIds">內銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domOrderSId">內銷訂單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:專案報價品項; 若為 null 則略過條件檢查）。</param>
            /// <param name="pgOrderDetSId">備貨單明細系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="quoteNumber">報價單號碼（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="quoteItemId">報價單明細項次（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] domOrderDetSIds, ISystemId domOrderSId, int? source, ISystemId pgOrderDetSId, string quoteNumber, string quoteItemId, string partNo, bool? isCancel)
            {
                this.DomOrderDetSIds = domOrderDetSIds;
                this.DomOrderSId = domOrderSId;
                this.Source = source;
                this.PGOrderDetSId = pgOrderDetSId;
                this.QuoteNumber = quoteNumber;
                this.QuoteItemId = quoteItemId;
                this.PartNo = partNo;
                this.IsCancel = isCancel;
            }

            /// <summary>
            /// 內銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderDetSIds { get; set; }
            /// <summary>
            /// 內銷訂單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId DomOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:專案報價品項; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 備貨單明細系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PGOrderDetSId { get; set; }
            /// <summary>
            /// 報價單號碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteItemId { get; set; }
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

            custEntity.Append("SELECT [DOM_ORDER_DET].* ");
            custEntity.Append("       , [DOM_ORDER].[IS_CANCEL] ");
            custEntity.Append("       , [ERP_INV].[INVENTORY_ITEM_ID], [ERP_INV].[DESCRIPTION] AS [SUMMARY], [ERP_INV].[UNIT_WEIGHT], [ERP_INV].[WEIGHT_UOM_CODE], [ERP_INV].[XSD_ITEM] ");
            custEntity.Append("       , [PG_ORDER_DET].[PG_ORDER_SID] ");
            custEntity.Append("       , [PG_ORDER].[ODR_NO] AS [PG_ORDER_ODR_NO] ");
            custEntity.Append("FROM [DOM_ORDER_DET] ");
            custEntity.Append("     INNER JOIN [DOM_ORDER] ON [DOM_ORDER].[SID] = [DOM_ORDER_DET].[DOM_ORDER_SID] ");
            custEntity.Append("     INNER JOIN [ERP_INV] ON [ERP_INV].[ITEM] = [DOM_ORDER_DET].[PART_NO] ");
            custEntity.Append("     LEFT JOIN [PG_ORDER_DET] ON [PG_ORDER_DET].[SID] = [DOM_ORDER_DET].[PG_ORDER_DET_SID] ");
            custEntity.Append("     LEFT JOIN [PG_ORDER] ON [PG_ORDER].[SID] = [PG_ORDER_DET].[PG_ORDER_SID] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderDetSIds != null && qConds.DomOrderDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderDetSIds), GenericDBType.Char));
            }

            if (qConds.DomOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "DOM_ORDER_SID", SqlOperator.EqualTo, qConds.DomOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (qConds.PGOrderDetSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "PG_ORDER_DET_SID", SqlOperator.EqualTo, qConds.PGOrderDetSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrWhiteSpace(qConds.QuoteNumber))
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumber, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.QuoteItemId))
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "QUOTEITEMID", SqlOperator.EqualTo, qConds.QuoteItemId, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.PartNo))
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER_DET", "PART_NO", SqlOperator.EqualTo, qConds.PartNo, GenericDBType.VarChar));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("DOM_ORDER", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
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
