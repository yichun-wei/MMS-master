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
        /// 外銷生產單。
        /// </summary>
        public const string EXT_PROD_ORDER = "EXT_PROD_ORDER";
    }

    /// <summary>
    /// 外銷生產單的類別實作。
    /// </summary>
    public class ExtProdOrder : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtProdOrder(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.EXT_PROD_ORDER))
        {
            SqlOrder sqlOrder = new SqlOrder();
            sqlOrder.Add("PROD_ODR_NO", Sort.Descending);
            //sqlOrder.Add("EXT_ORDER_SID", Sort.Descending);
            //sqlOrder.Add("VERSION", Sort.Descending);

            base.DefaultSqlOrder = sqlOrder;
        }

        #region 生產單編號
        /// <summary>
        /// 生產單編號。
        /// </summary>
        public class ProdOrderNo
        {
            /// <summary>
            /// 初始化 ProdOrderNo 類別的新執行個體。
            /// </summary>
            public ProdOrderNo()
            {
                this.Code = "MO";
                this.Date = StringLib.LastSubstring(DateTime.Today.Year.ToString(), 2);
            }

            /// <summary>
            /// 初始化 ProdOrderNo 類別的新執行個體。
            /// </summary>
            public ProdOrderNo(string code, string date, int seq)
            {
                this.Code = code;
                this.Date = date;
                this.Seq = seq;
            }

            /// <summary>
            /// 生產單編號代碼（固定「MO」）。
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 生產單編號日期（西元年末兩碼）。
            /// </summary>
            public string Date { get; set; }
            /// <summary>
            /// 生產單編號流水號。
            /// </summary>
            public int Seq { get; set; }

            /// <summary>
            /// 取得完整的生產單編號。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                //MO + 西元年度末兩碼 + 四位流水號, 例「MO150001」.
                return string.Format("{0}{1}{2}", this.Code, this.Date, this.Seq.ToString().PadLeft(4, '0'));
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
            /// 生產單編號代碼。
            /// </summary>
            [SchemaMapping(Name = "PROD_ODR_CODE", Type = ReturnType.String)]
            public string ProdOdrCode { get; set; }
            /// <summary>
            /// 生產單編號日期。
            /// </summary>
            [SchemaMapping(Name = "PROD_ODR_DATE", Type = ReturnType.String)]
            public string ProdOdrDate { get; set; }
            /// <summary>
            /// 生產單編號流水號。
            /// </summary>
            [SchemaMapping(Name = "PROD_ODR_SEQ", Type = ReturnType.Int, AllowNull = true)]
            public int? ProdOdrSeq { get; set; }
            /// <summary>
            /// 生產單編號。
            /// </summary>
            [SchemaMapping(Name = "PROD_ODR_NO", Type = ReturnType.String)]
            public string ProdOdrNo { get; set; }
            /// <summary>
            /// 外銷訂單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 版本。
            /// </summary>
            [SchemaMapping(Name = "VERSION", Type = ReturnType.Int)]
            public int Version { get; set; }
            /// <summary>
            /// 是否作用中。
            /// </summary>
            [SchemaMapping(Name = "ACTIVE_FLAG", Type = ReturnType.Bool)]
            public bool ActiveFlag { get; set; }
            /// <summary>
            /// 狀態（1:未確認 2:已確認）。
            /// </summary>
            [SchemaMapping(Name = "STATUS", Type = ReturnType.Int)]
            public int Status { get; set; }
            /// <summary>
            /// 報價單日期。
            /// </summary>
            [SchemaMapping(Name = "QUOTN_DATE", Type = ReturnType.DateTime)]
            public DateTime QuotnDate { get; set; }
            /// <summary>
            /// 客戶需求日。
            /// </summary>
            [SchemaMapping(Name = "CDD", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? Cdd { get; set; }
            /// <summary>
            /// 預計交貨日。
            /// </summary>
            [SchemaMapping(Name = "EDD", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? Edd { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            [SchemaMapping(Name = "WHSE", Type = ReturnType.String)]
            public string Whse { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }

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
            /// 外銷訂單系統代號。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 版本。
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// 是否作用中。
            /// </summary>
            public bool ActiveFlag { get; set; }
            /// <summary>
            /// 狀態（1:未確認 2:已確認）。
            /// </summary>
            public int Status { get; set; }
            /// <summary>
            /// 報價單日期。
            /// </summary>
            public DateTime? QuotnDate { get; set; }
            /// <summary>
            /// 客戶需求日。
            /// </summary>
            public DateTime? Cdd { get; set; }
            /// <summary>
            /// 預計交貨日。
            /// </summary>
            public DateTime? Edd { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            public string Whse { get; set; }
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
        /// <param name="extProdOrderSId">指定的外銷生產單系統代號。</param>
        /// <param name="prodOrderNo">生產單編號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <param name="status">狀態（1:未確認 2:已確認）。</param>
        /// <param name="quotnDate">報價單日期。</param>
        /// <param name="cdd">客戶需求日（null 將自動略過操作）。</param>
        /// <param name="edd">預計交貨日（null 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extProdOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId extProdOrderSId, ProdOrderNo prodOrderNo, ISystemId extOrderSId, int status, DateTime quotnDate, DateTime? cdd, DateTime? edd, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extProdOrderSId == null) { throw new ArgumentNullException("extProdOrderSId"); }
            if (extOrderSId == null) { throw new ArgumentNullException("extOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extProdOrderSId, extOrderSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                //取新版號
                var version = this.GetNewVerNo(extOrderSId);
                //停用作用中的資料
                this.LetNotActive(actorSId, extOrderSId);

                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", extProdOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("PROD_ODR_CODE", prodOrderNo.Code, GenericDBType.Char, false);
                transSet.SmartAdd("PROD_ODR_DATE", prodOrderNo.Date, GenericDBType.Char, false);
                transSet.SmartAdd("PROD_ODR_SEQ", prodOrderNo.Seq, GenericDBType.Int);
                transSet.SmartAdd("PROD_ODR_NO", prodOrderNo.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("EXT_ORDER_SID", extOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("VERSION", version, GenericDBType.Int);
                transSet.SmartAdd("ACTIVE_FLAG", "Y", GenericDBType.Char, false);
                transSet.SmartAdd("STATUS", status, GenericDBType.Int);
                transSet.SmartAdd("QUOTN_DATE", quotnDate, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("WHSE", "外銷倉", GenericDBType.VarChar, false); //固定
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
        /// 依指定的參數，修改一筆外銷生產單。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extProdOrderSId">外銷生產單系統代號。</param>
        /// <param name="edd">預計交貨日（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extProdOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extProdOrderSId, DateTime? edd)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extProdOrderSId == null) { throw new ArgumentNullException("extProdOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extProdOrderSId);
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
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char,true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extProdOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region LetNotActive
        /// <summary>
        /// 依外銷訂單系統代號停用作用中的資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        Returner LetNotActive(ISystemId actorSId, ISystemId extOrderSId)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extOrderSId == null) { throw new ArgumentNullException("extOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extOrderSId);
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
                transSet.SmartAdd("ACTIVE_FLAG", "N", GenericDBType.VarChar, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXT_ORDER_SID", SqlCond.EqualTo, extOrderSId.ToString(), GenericDBType.Char, SqlCondsSet.And);
                condsMain.Add("ACTIVE_FLAG", SqlCond.EqualTo, "Y", GenericDBType.Char, SqlCondsSet.And);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateStatus
        /// <summary>
        /// 更新狀態。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extProdOrderSId">外銷生產單系統代號。</param>
        /// <param name="status">狀態（1:未確認 2:已確認）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extProdOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateStatus(ISystemId actorSId, ISystemId extProdOrderSId, int status)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extProdOrderSId == null) { throw new ArgumentNullException("extProdOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extProdOrderSId);
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
                transSet.SmartAdd("STATUS", status, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extProdOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="extProdOrderSIds">外銷生產單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extOrderSId">外銷訂單系統代號。</param>
            /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（1:未確認 2:已確認; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginQuotnDate">範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endQuotnDate">範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdd">範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdd">範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] extProdOrderSIds, ISystemId extOrderSId, int? version, bool? activeFlag, int[] statuses, DateTime? beginCdt, DateTime? endCdt, DateTime? beginQuotnDate, DateTime? endQuotnDate, DateTime? beginCdd, DateTime? endCdd, DateTime? beginEdd, DateTime? endEdd)
            {
                this.ExtProdOrderSIds = extProdOrderSIds;
                this.ExtOrderSId = extOrderSId;
                this.Version = version;
                this.ActiveFlag = activeFlag;
                this.Statuses = statuses;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginQuotnDate = beginQuotnDate;
                this.EndQuotnDate = endQuotnDate;
                this.BeginCdd = beginCdd;
                this.EndCdd = endCdd;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
            }

            /// <summary>
            /// 外銷生產單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtProdOrderSIds { get; set; }
            /// <summary>
            /// 外銷訂單系統代號。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 版本（若為 null 則略過條件檢查）。
            /// </summary>
            public int? Version { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
            /// <summary>
            /// 狀態陣列集合（1:未確認 2:已確認; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] Statuses { get; set; }
            /// <summary>
            /// 範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdt { get; set; }
            /// <summary>
            /// 範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdt { get; set; }
            /// <summary>
            /// 範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginQuotnDate { get; set; }
            /// <summary>
            /// 範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndQuotnDate { get; set; }
            /// <summary>
            /// 範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdd { get; set; }
            /// <summary>
            /// 範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
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

            custEntity.Append("SELECT [EXT_PROD_ORDER].* ");
            custEntity.Append("FROM [EXT_PROD_ORDER] ");

            var sqlConds = new List<string>();

            if (qConds.ExtProdOrderSIds != null && qConds.ExtProdOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtProdOrderSIds), GenericDBType.Char));
            }

            if (qConds.ExtOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXT_ORDER_SID", SqlOperator.EqualTo, qConds.ExtOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Version.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "VERSION", SqlOperator.EqualTo, qConds.Version.Value, GenericDBType.Int));
            }

            if (qConds.ActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTN_DATE", SqlOperator.GreaterEqualThan, qConds.BeginQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTN_DATE", SqlOperator.LessEqualThan, qConds.EndQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.BeginCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDD", SqlOperator.GreaterEqualThan, qConds.BeginCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CDD", SqlOperator.LessEqualThan, qConds.EndCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
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

        #region GetNewProdOrderNo
        /// <summary>
        /// 取得新的生產單編號。
        /// </summary>
        public ProdOrderNo GetNewProdOrderNo()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                ProdOrderNo prodOrderNo = new ProdOrderNo();

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("PROD_ODR_CODE", SqlCond.EqualTo, prodOrderNo.Code, GenericDBType.Char, SqlCondsSet.And);
                condsMain.Add("PROD_ODR_DATE", SqlCond.EqualTo, prodOrderNo.Date, GenericDBType.Char, SqlCondsSet.And);

                Returner returner = null;
                try
                {
                    returner = base.Entity.GetInfoBy(1, new SqlOrder("PROD_ODR_SEQ", Sort.Descending), condsMain, new string[] { "PROD_ODR_SEQ" });
                    if (returner.IsCompletedAndContinue)
                    {
                        int seq = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]);
                        prodOrderNo.Seq = ++seq;
                    }
                    else
                    {
                        prodOrderNo.Seq = 1;
                    }

                    transaction.Complete();

                    return prodOrderNo;
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }
                }
            }
        }
        #endregion

        #region GetNewVerNo
        /// <summary>
        /// 取得新版號。
        /// </summary>
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <returns>新版號。</returns>
        int GetNewVerNo(ISystemId extOrderSId)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add("ACTIVE_FLAG", SqlCond.EqualTo, "Y", GenericDBType.Char, SqlCondsSet.And);
            condsMain.Add("EXT_ORDER_SID", SqlCond.EqualTo, extOrderSId.ToString(), GenericDBType.Char, SqlCondsSet.And);

            Returner returner = null;
            try
            {
                returner = base.Entity.GetInfoBy(1, new SqlOrder("VERSION", Sort.Descending), condsMain, new string[] { "VERSION" });
                if (returner.IsCompletedAndContinue)
                {
                    int version = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0]);
                    return ++version;
                }
                else
                {
                    return 1;
                }
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
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
            /// 外銷報價單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_QUOTN_SID", Type = ReturnType.SId)]
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 外銷訂單版本。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_VERSION", Type = ReturnType.Int)]
            public int ExtOrderVersion { get; set; }
            /// <summary>
            /// 外銷訂單是否作用中。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_ACTIVE_FLAG", Type = ReturnType.Bool)]
            public bool ExtOrderActiveFlag { get; set; }

            /// <summary>
            /// 客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_NAME", Type = ReturnType.String)]
            public string CustomerName { get; set; }
            /// <summary>
            /// 報價單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
            /// <summary>
            /// 外銷報價單狀態（0:草稿 1:待轉訂單 2:已轉訂單）。
            /// </summary>
            [SchemaMapping(Name = "EXT_QUOTN_STATUS", Type = ReturnType.Int)]
            public int ExtQuotnStatus { get; set; }
            /// <summary>
            /// 是否已取消。
            /// </summary>
            [SchemaMapping(Name = "IS_CANCEL", Type = ReturnType.Bool)]
            public bool IsCancel { get; set; }
            /// <summary>
            /// 是否報價單調整中。
            /// </summary>
            [SchemaMapping(Name = "IS_READJUST", Type = ReturnType.Bool)]
            public bool IsReadjust { get; set; }

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
            /// <param name="extProdOrderSIds">外銷生產單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="cSIds">建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extQuotnSId">外銷報價單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderSId">外銷訂單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderVersion">外銷訂單版本（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderActiveFlag">外銷訂單是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（1:未確認 2:已確認; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginQuotnDate">範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endQuotnDate">範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdd">範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdd">範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] extProdOrderSIds, ISystemId[] cSIds, ISystemId extQuotnSId, ISystemId extOrderSId, int? extOrderVersion, bool? extOrderActiveFlag, int? version, bool? activeFlag, int[] statuses, DateTime? beginCdt, DateTime? endCdt, DateTime? beginQuotnDate, DateTime? endQuotnDate, DateTime? beginCdd, DateTime? endCdd, DateTime? beginEdd, DateTime? endEdd, bool? isCancel)
            {
                this.ExtProdOrderSIds = extProdOrderSIds;
                this.CSIds = cSIds;
                this.ExtQuotnSId = extQuotnSId;
                this.ExtOrderSId = extOrderSId;
                this.ExtOrderVersion = version;
                this.ExtOrderActiveFlag = activeFlag;
                this.Version = version;
                this.ActiveFlag = activeFlag;
                this.Statuses = statuses;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginQuotnDate = beginQuotnDate;
                this.EndQuotnDate = endQuotnDate;
                this.BeginCdd = beginCdd;
                this.EndCdd = endCdd;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.IsCancel = isCancel;
            }

            /// <summary>
            /// 外銷生產單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtProdOrderSIds { get; set; }
            /// <summary>
            /// 建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] CSIds { get; set; }
            /// <summary>
            /// 外銷報價單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 外銷訂單版本（若為 null 則略過條件檢查）。
            /// </summary>
            public int? ExtOrderVersion { get; set; }
            /// <summary>
            /// 外銷訂單是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ExtOrderActiveFlag { get; set; }
            /// <summary>
            /// 版本（若為 null 則略過條件檢查）。
            /// </summary>
            public int? Version { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
            /// <summary>
            /// 狀態陣列集合（1:未確認 2:已確認; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] Statuses { get; set; }
            /// <summary>
            /// 範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdt { get; set; }
            /// <summary>
            /// 範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdt { get; set; }
            /// <summary>
            /// 範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginQuotnDate { get; set; }
            /// <summary>
            /// 範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndQuotnDate { get; set; }
            /// <summary>
            /// 範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginCdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndCdd { get; set; }
            /// <summary>
            /// 範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
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
                SELECT [PROD].* 
                       , [ODR].[EXT_QUOTN_SID], [ODR].[VERSION] AS [EXT_ORDER_VERSION], [ODR].[ACTIVE_FLAG] AS [EXT_ORDER_ACTIVE_FLAG], [ODR].[CUSTOMER_NAME] 
                       , [QUOTN].[ODR_NO], [QUOTN].[STATUS] AS [EXT_QUOTN_STATUS], [QUOTN].[IS_CANCEL], [QUOTN].[IS_READJUST] 
                FROM [EXT_PROD_ORDER] [PROD]
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [PROD].[EXT_ORDER_SID] 
                     INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] 
            ");

            var sqlConds = new List<string>();

            if (qConds.ExtProdOrderSIds != null && qConds.ExtProdOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtProdOrderSIds), GenericDBType.Char));
            }

            if (qConds.CSIds != null && qConds.CSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "CSID", SqlOperator.EqualTo, SystemId.ToString(qConds.CSIds), GenericDBType.Char));
            }

            if (qConds.ExtQuotnSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "EXT_QUOTN_SID", SqlOperator.EqualTo, qConds.ExtQuotnSId.Value, GenericDBType.Char));
            }

            if (qConds.ExtOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "EXT_ORDER_SID", SqlOperator.EqualTo, qConds.ExtOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.ExtOrderVersion.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "VERSION", SqlOperator.EqualTo, qConds.ExtOrderVersion.Value, GenericDBType.Int));
            }

            if (qConds.ExtOrderActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ExtOrderActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Version.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "VERSION", SqlOperator.EqualTo, qConds.Version.Value, GenericDBType.Int));
            }

            if (qConds.ActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("QUOTN", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "QUOTN_DATE", SqlOperator.GreaterEqualThan, qConds.BeginQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "QUOTN_DATE", SqlOperator.LessEqualThan, qConds.EndQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.BeginCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "CDD", SqlOperator.GreaterEqualThan, qConds.BeginCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "CDD", SqlOperator.LessEqualThan, qConds.EndCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("PROD", "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
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
