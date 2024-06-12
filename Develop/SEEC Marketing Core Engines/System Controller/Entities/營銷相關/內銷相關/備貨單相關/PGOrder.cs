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
        /// 備貨單。
        /// </summary>
        public const string PG_ORDER = "PG_ORDER";
    }

    /// <summary>
    /// 備貨單的類別實作。
    /// </summary>
    public class PGOrder : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public PGOrder(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.PG_ORDER))
        {
            base.DefaultSqlOrder = new SqlOrder("SID", Sort.Descending);
        }

        #region 備貨單編號
        /// <summary>
        /// 備貨單編號。
        /// </summary>
        public class OrderNo
        {
            /// <summary>
            /// 初始化 OrderNo 類別的新執行個體。
            /// </summary>
            public OrderNo()
            {
                this.Date = StringLib.LastSubstring(DateTime.Today.Year.ToString(), 2);
            }

            /// <summary>
            /// 初始化 OrderNo 類別的新執行個體。
            /// </summary>
            public OrderNo(string code, string date, int seq)
            {
                this.Code = code;
                this.Date = date;
                this.Seq = seq;
            }

            /// <summary>
            /// 備貨單編號代碼（內銷地區代碼）。
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 備貨單編號日期（西元年末兩碼）。
            /// </summary>
            public string Date { get; set; }
            /// <summary>
            /// 備貨單編號流水號。
            /// </summary>
            public int Seq { get; set; }

            /// <summary>
            /// 取得完整的備貨單編號。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                //內銷地區代碼 + 西元年度末兩碼 + 五位流水號, 例「XBJB1500001」.
                return string.Format("{0}{1}{2}", this.Code, this.Date, this.Seq.ToString().PadLeft(5, '0'));
            }
        }
        #endregion

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
            /// 備貨單編號代碼。
            /// </summary>
            [SchemaMapping(Name = "ODR_CODE", Type = ReturnType.String)]
            public string OdrCode { get; set; }
            /// <summary>
            /// 備貨單編號日期。
            /// </summary>
            [SchemaMapping(Name = "ODR_DATE", Type = ReturnType.String)]
            public string OdrDate { get; set; }
            /// <summary>
            /// 備貨單編號流水號。
            /// </summary>
            [SchemaMapping(Name = "ODR_SEQ", Type = ReturnType.Int)]
            public int OdrSeq { get; set; }
            /// <summary>
            /// 備貨單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
            /// <summary>
            /// 對象代碼（1:內銷）。
            /// </summary>
            [SchemaMapping(Name = "TGT_CODE", Type = ReturnType.Int)]
            public int TgtCode { get; set; }
            /// <summary>
            /// 內銷地區系統代號。
            /// </summary>
            [SchemaMapping(Name = "DOM_DIST_SID", Type = ReturnType.SId)]
            public ISystemId DomDistSId { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long)]
            public long CustomerId { get; set; }
            /// <summary>
            /// 預計出貨日。
            /// </summary>
            [SchemaMapping(Name = "EDD", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? Edd { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }
            /// <summary>
            /// 是否已取消。
            /// </summary>
            [SchemaMapping(Name = "IS_CANCEL", Type = ReturnType.Bool)]
            public bool IsCancel { get; set; }
            /// <summary>
            /// 取消時間。
            /// </summary>
            [SchemaMapping(Name = "CANCEL_DT", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? CancelDT { get; set; }

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
            /// 備貨單編號代碼。
            /// </summary>
            public string OdrCode { get; set; }
            /// <summary>
            /// 備貨單編號日期。
            /// </summary>
            public string OdrDate { get; set; }
            /// <summary>
            /// 備貨單編號流水號。
            /// </summary>
            public int? OdrSeq { get; set; }
            /// <summary>
            /// 備貨單編號。
            /// </summary>
            public string OdrNo { get; set; }
            /// <summary>
            /// 對象代碼（1:內銷）。
            /// </summary>
            public int? TgtCode { get; set; }
            /// <summary>
            /// 內銷地區系統代號。
            /// </summary>
            public ISystemId DomDistSId { get; set; }
            /// <summary>
            /// 預計出貨日。
            /// </summary>
            public DateTime? Edd { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="pgOrderSId">指定的備貨單系統代號。</param>
        /// <param name="orderNo">訂單編號。</param>
        /// <param name="tgtCode">對象代碼（1:內銷）。</param>
        /// <param name="domDistSId">內銷地區系統代號。</param>
        /// <param name="customerId">客戶 ID。</param>
        /// <param name="edd">預計出貨日（null 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pgOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 orderNo 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 domDistSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId pgOrderSId, OrderNo orderNo, int tgtCode, ISystemId domDistSId, long customerId, DateTime? edd, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pgOrderSId == null) { throw new ArgumentNullException("pgOrderSId"); }
            if (orderNo == null) { throw new ArgumentNullException("orderNo"); }
            if (domDistSId == null) { throw new ArgumentNullException("domDistSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pgOrderSId, domDistSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", pgOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ODR_CODE", orderNo.Code, GenericDBType.Char, false);
                transSet.SmartAdd("ODR_DATE", orderNo.Date, GenericDBType.Char, false);
                transSet.SmartAdd("ODR_SEQ", orderNo.Seq, GenericDBType.Int);
                transSet.SmartAdd("ODR_NO", orderNo.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("TGT_CODE", tgtCode, GenericDBType.Int);
                transSet.SmartAdd("DOM_DIST_SID", domDistSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char);
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
        /// 依指定的參數，修改一筆備貨單。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="pgOrderSId">備貨單系統代號。</param>
        /// <param name="domDistSId">內銷地區系統代號。</param>
        /// <param name="edd">預計出貨日（null 則直接設為 DBNull）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pgOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 domDistSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId pgOrderSId, ISystemId domDistSId, DateTime? edd, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pgOrderSId == null) { throw new ArgumentNullException("pgOrderSId"); }
            if (domDistSId == null) { throw new ArgumentNullException("domDistSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pgOrderSId);
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
                transSet.SmartAdd("DOM_DIST_SID", domDistSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pgOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateCancelInfo
        /// <summary>
        /// 更新取消資訊。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="pgOrderSId">備貨單系統代號。</param>
        /// <param name="isCancel">是否已取消。</param>
        /// <param name="cancelDT">取消時間（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pgOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCancelInfo(ISystemId actorSId, ISystemId pgOrderSId, bool isCancel, DateTime? cancelDT)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pgOrderSId == null) { throw new ArgumentNullException("pgOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pgOrderSId);
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
                transSet.SmartAdd("IS_CANCEL", isCancel ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("CANCEL_DT", cancelDT, "yyyyMMddHHmmss", GenericDBType.Char, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pgOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="pgOrderSIds">備貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domDistSIds">內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] pgOrderSIds, ISystemId[] domDistSIds, long? customerId, DateTime? beginCdt, DateTime? endCdt, DateTime? beginEdd, DateTime? endEdd, bool? isCancel)
            {
                this.DomOrderSIds = pgOrderSIds;
                this.DomDistSIds = domDistSIds;
                this.CustomerId = customerId;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.IsCancel = isCancel;
            }

            /// <summary>
            /// 備貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderSIds { get; set; }
            /// <summary>
            /// 內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomDistSIds { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdt { get; set; }
            /// <summary>
            /// 範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdt { get; set; }
            /// <summary>
            /// 範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
            /// <summary>
            /// 是否已取消（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsCancel { get; set; }
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

            custEntity.Append("SELECT [PG_ORDER].* ");
            custEntity.Append("FROM [PG_ORDER] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderSIds != null && qConds.DomOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderSIds), GenericDBType.Char));
            }

            if (qConds.DomDistSIds != null && qConds.DomDistSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DOM_DIST_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomDistSIds), GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion

        #region GetNewOrderNo
        /// <summary>
        /// 取得新的訂單編號。
        /// </summary>
        /// <param name="odrCode">備貨單編號代碼。</param>
        public OrderNo GetNewOrderNo(string odrCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                OrderNo orderNo = new OrderNo();
                orderNo.Code = odrCode;

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("ODR_CODE", SqlCond.EqualTo, orderNo.Code, GenericDBType.Char, SqlCondsSet.And);
                condsMain.Add("ODR_DATE", SqlCond.EqualTo, orderNo.Date, GenericDBType.Char, SqlCondsSet.And);

                Returner returner = null;
                try
                {
                    returner = base.Entity.GetInfoBy(1, new SqlOrder("ODR_SEQ", Sort.Descending), condsMain, new string[] { "ODR_SEQ" });
                    if (returner.IsCompletedAndContinue)
                    {
                        int seq = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]);
                        orderNo.Seq = ++seq;
                    }
                    else
                    {
                        orderNo.Seq = 1;
                    }

                    transaction.Complete();

                    return orderNo;
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }
                }
            }
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
            /// 內銷地區名稱。
            /// </summary>
            [SchemaMapping(Name = "DOM_DIST_NAME", Type = ReturnType.String)]
            public string DomDistName { get; set; }
            /// <summary>
            /// 客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_NAME", Type = ReturnType.String)]
            public string CustomerName { get; set; }
            /// <summary>
            /// 營業員姓名。
            /// </summary>
            [SchemaMapping(Name = "SALES_NAME", Type = ReturnType.String)]
            public string SalesName { get; set; }
            /// <summary>
            /// 使用量。
            /// </summary>
            [SchemaMapping(Name = "USE_QTY", Type = ReturnType.Int)]
            public int UseQty { get; set; }

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
            /// <param name="pgOrderSIds">備貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="domDistSIds">內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            /// <param name="hasUsed">是否已被使用（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] pgOrderSIds, ISystemId[] domDistSIds, long? customerId, DateTime? beginCdt, DateTime? endCdt, DateTime? beginEdd, DateTime? endEdd, bool? isCancel, bool? hasUsed)
            {
                this.DomOrderSIds = pgOrderSIds;
                this.DomDistSIds = domDistSIds;
                this.CustomerId = customerId;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.IsCancel = isCancel;
                this.HasUsed = hasUsed;
            }

            /// <summary>
            /// 備貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderSIds { get; set; }
            /// <summary>
            /// 內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomDistSIds { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdt { get; set; }
            /// <summary>
            /// 範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdt { get; set; }
            /// <summary>
            /// 範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
            /// <summary>
            /// 是否已取消（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsCancel { get; set; }
            /// <summary>
            /// 是否已被使用（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? HasUsed { get; set; }
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

            custEntity.Append("SELECT [PG_ORDER].* ");
            custEntity.Append("       , [ERP_CUSTER].[CUSTOMER_NAME], [ERP_CUSTER].[SALES_NAME] ");
            custEntity.Append("       , [PUB_CAT].[NAME] AS [DOM_DIST_NAME] ");
            //取內銷訂單的使用量 (不取已註刪或已取消的資料)
            custEntity.Append("       , (SELECT COALESCE(SUM([PG_ORDER_DET].[QTY]), 0) AS [CNT] FROM [PG_ORDER_DET] INNER JOIN [DOM_ORDER_DET] ON [PG_ORDER_DET].[SID] = [DOM_ORDER_DET].[PG_ORDER_DET_SID] INNER JOIN [DOM_ORDER] ON [DOM_ORDER].[SID] = [DOM_ORDER_DET].[DOM_ORDER_SID] AND [DOM_ORDER].[MDELED] = 'N' AND [DOM_ORDER].[IS_CANCEL] = 'N' WHERE [PG_ORDER_DET].[PG_ORDER_SID] = [PG_ORDER].[SID]) AS [USE_QTY] ");
            custEntity.Append("FROM [PG_ORDER] ");
            custEntity.Append("     INNER JOIN [ERP_CUSTER] ON [ERP_CUSTER].[CUSTOMER_ID] = [PG_ORDER].[CUSTOMER_ID] ");
            custEntity.Append("     INNER JOIN [PUB_CAT] ON [PUB_CAT].[SID] = [PG_ORDER].[DOM_DIST_SID] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderSIds != null && qConds.DomOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderSIds), GenericDBType.Char));
            }

            if (qConds.DomDistSIds != null && qConds.DomDistSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "DOM_DIST_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomDistSIds), GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PG_ORDER", "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            switch (qConds.HasUsed)
            {
                case true:
                    condsMain.Add("USE_QTY", SqlCond.GreaterThan, 0, GenericDBType.Int, SqlCondsSet.And);
                    break;
                case false:
                    condsMain.Add("USE_QTY", SqlCond.EqualTo, 0, GenericDBType.Int, SqlCondsSet.And);
                    break;
            }

            return condsMain;
        }
        #endregion
        #endregion
    }
}
