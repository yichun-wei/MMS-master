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
        /// 外銷訂單。
        /// </summary>
        public const string EXT_ORDER = "EXT_ORDER";
    }

    /// <summary>
    /// 外銷訂單的類別實作。
    /// </summary>
    public class ExtOrder : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtOrder(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.EXT_ORDER))
        {
            SqlOrder sqlOrder = new SqlOrder();
            sqlOrder.Add("EXT_QUOTN_SID", Sort.Descending);
            sqlOrder.Add("VERSION", Sort.Descending);

            base.DefaultSqlOrder = sqlOrder;
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
            /// 外銷報價單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_QUOTN_SID", Type = ReturnType.SId)]
            public ISystemId ExtQuotnSId { get; set; }
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
            /// 狀態（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程）。
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
            /// 客戶建立來源（1:ERP 客戶 2:手動建立）。
            /// </summary>
            [SchemaMapping(Name = "CUSTER_SRC", Type = ReturnType.Int)]
            public int CusterSrc { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? CustomerId { get; set; }
            /// <summary>
            /// 客戶編號。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_NUMBER", Type = ReturnType.String)]
            public string CustomerNumber { get; set; }
            /// <summary>
            /// 客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_NAME", Type = ReturnType.String)]
            public string CustomerName { get; set; }
            /// <summary>
            /// 客戶聯絡人 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_CON_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? CustomerConId { get; set; }
            /// <summary>
            /// 客戶聯絡人。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_CON_NAME", Type = ReturnType.String)]
            public string CustomerConName { get; set; }
            /// <summary>
            /// 客戶電話區碼。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_AREA_CODE", Type = ReturnType.String)]
            public string CustomerAreaCode { get; set; }
            /// <summary>
            /// 客戶電話。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_TEL", Type = ReturnType.String)]
            public string CustomerTel { get; set; }
            /// <summary>
            /// 客戶傳真。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_FAX", Type = ReturnType.String)]
            public string CustomerFax { get; set; }
            /// <summary>
            /// 客戶地址 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ADDR_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? CustomerAddrId { get; set; }
            /// <summary>
            /// 客戶地址。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ADDR", Type = ReturnType.String)]
            public string CustomerAddr { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            [SchemaMapping(Name = "PRICE_LIST_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? PriceListId { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            [SchemaMapping(Name = "CURRENCY_CODE", Type = ReturnType.String)]
            public string CurrencyCode { get; set; }
            /// <summary>
            /// 送貨地點 ID。
            /// </summary>
            [SchemaMapping(Name = "SHIP_TO_SITE_USE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ShipToSiteUseId { get; set; }
            /// <summary>
            /// 帳單地點 ID。
            /// </summary>
            [SchemaMapping(Name = "INVOICE_TO_SITE_USE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InvoiceToSiteUseId { get; set; }
            /// <summary>
            /// 訂單型態 ID。
            /// </summary>
            [SchemaMapping(Name = "ORDER_TYPE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? OrderTypeId { get; set; }
            /// <summary>
            /// 營業員 ID。
            /// </summary>
            [SchemaMapping(Name = "SALESREP_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? SalesRepId { get; set; }
            /// <summary>
            /// 營業員姓名。
            /// </summary>
            [SchemaMapping(Name = "SALES_NAME", Type = ReturnType.String)]
            public string SalesName { get; set; }
            /// <summary>
            /// 收貨人。
            /// </summary>
            [SchemaMapping(Name = "RCPT_NAME", Type = ReturnType.String)]
            public string RcptName { get; set; }
            /// <summary>
            /// 收貨人客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "RCPT_CUSTER_NAME", Type = ReturnType.String)]
            public string RcptCusterName { get; set; }
            /// <summary>
            /// 收貨人電話。
            /// </summary>
            [SchemaMapping(Name = "RCPT_TEL", Type = ReturnType.String)]
            public string RcptTel { get; set; }
            /// <summary>
            /// 收貨人傳真。
            /// </summary>
            [SchemaMapping(Name = "RCPT_FAX", Type = ReturnType.String)]
            public string RcptFax { get; set; }
            /// <summary>
            /// 收貨人地址。
            /// </summary>
            [SchemaMapping(Name = "RCPT_ADDR", Type = ReturnType.String)]
            public string RcptAddr { get; set; }
            /// <summary>
            /// 貨運方式系統代號。
            /// </summary>
            [SchemaMapping(Name = "FREIGHT_WAY_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId FreightWaySId { get; set; }
            /// <summary>
            /// 總金額。
            /// </summary>
            [SchemaMapping(Name = "TOTAL_AMT", Type = ReturnType.Float, AllowNull = true)]
            public float? TotalAmt { get; set; }
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
            /// 外銷報價單系統代號。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 版本。
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// 是否作用中。
            /// </summary>
            public bool ActiveFlag { get; set; }
            /// <summary>
            /// 狀態（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程）。
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
            /// 客戶建立來源（1:ERP 客戶 2:手動建立）。
            /// </summary>
            public int? CusterSrc { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 客戶編號。
            /// </summary>
            public string CustomerNumber { get; set; }
            /// <summary>
            /// 客戶名稱。
            /// </summary>
            public string CustomerName { get; set; }
            /// <summary>
            /// 客戶聯絡人 ID。
            /// </summary>
            public long? CustomerConId { get; set; }
            /// <summary>
            /// 客戶聯絡人。
            /// </summary>
            public string CustomerConName { get; set; }
            /// <summary>
            /// 客戶電話區碼。
            /// </summary>
            public string CustomerAreaCode { get; set; }
            /// <summary>
            /// 客戶電話。
            /// </summary>
            public string CustomerTel { get; set; }
            /// <summary>
            /// 客戶傳真。
            /// </summary>
            public string CustomerFax { get; set; }
            /// <summary>
            /// 客戶地址 ID。
            /// </summary>
            public long? CustomerAddrId { get; set; }
            /// <summary>
            /// 客戶地址。
            /// </summary>
            public string CustomerAddr { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            public long? PriceListId { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            public string CurrencyCode { get; set; }
            /// <summary>
            /// 送貨地點 ID。
            /// </summary>
            public long? ShipToSiteUseId { get; set; }
            /// <summary>
            /// 帳單地點 ID。
            /// </summary>
            public long? InvoiceToSiteUseId { get; set; }
            /// <summary>
            /// 訂單型態 ID。
            /// </summary>
            public long? OrderTypeId { get; set; }
            /// <summary>
            /// 營業員 ID。
            /// </summary>
            public long? SalesRepId { get; set; }
            /// <summary>
            /// 營業員姓名。
            /// </summary>
            public string SalesName { get; set; }
            /// <summary>
            /// 收貨人。
            /// </summary>
            public string RcptName { get; set; }
            /// <summary>
            /// 收貨人客戶名稱。
            /// </summary>
            public string RcptCusterName { get; set; }
            /// <summary>
            /// 收貨人電話。
            /// </summary>
            public string RcptTel { get; set; }
            /// <summary>
            /// 收貨人傳真。
            /// </summary>
            public string RcptFax { get; set; }
            /// <summary>
            /// 收貨人地址。
            /// </summary>
            public string RcptAddr { get; set; }
            /// <summary>
            /// 貨運方式系統代號。
            /// </summary>
            public ISystemId FreightWaySId { get; set; }
            /// <summary>
            /// 總金額。
            /// </summary>
            public float? TotalAmt { get; set; }
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
        /// <param name="extOrderSId">指定的外銷訂單系統代號。</param>
        /// <param name="extQuotnSId">外銷報價單系統代號。</param>
        /// <param name="status">狀態（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程）。</param>
        /// <param name="quotnDate">報價單日期。</param>
        /// <param name="cdd">客戶需求日（null 將自動略過操作）。</param>
        /// <param name="edd">預計交貨日（null 將自動略過操作）。</param>
        /// <param name="custerSrc">客戶建立來源（1:ERP 客戶 2:手動建立）。</param>
        /// <param name="customerId">客戶 ID（null 將自動略過操作）。</param>
        /// <param name="customerNumber">客戶編號（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerName">客戶名稱（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerConId">客戶聯絡人 ID（null 將自動略過操作）。</param>
        /// <param name="customerConName">客戶聯絡人（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerAreaCode">客戶電話區碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerTel">客戶電話（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerFax">客戶傳真（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerAddrId">客戶地址 ID（null 將自動略過操作）。</param>
        /// <param name="customerAddr">客戶地址（null 或 empty 將自動略過操作）。</param>
        /// <param name="priceListId">價目表 ID（null 將自動略過操作）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 將自動略過操作）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 將自動略過操作）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 將自動略過操作）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 將自動略過操作）。</param>
        /// <param name="salesRepId">營業員 ID（null 將自動略過操作）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 將自動略過操作）。</param>
        /// <param name="rcptName">收貨人（null 或 empty 將自動略過操作）。</param>
        /// <param name="rcptCusterName">收貨人客戶名稱（null 或 empty 將自動略過操作）。</param>
        /// <param name="rcptTel">收貨人電話（null 或 empty 將自動略過操作）。</param>
        /// <param name="rcptFax">收貨人傳真（null 或 empty 將自動略過操作）。</param>
        /// <param name="rcptAddr">收貨人地址（null 或 empty 將自動略過操作）。</param>
        /// <param name="freightWaySId">貨運方式系統代號（null 將自動略過操作）。</param>
        /// <param name="totalAmt">總金額（null 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extQuotnSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId extOrderSId, ISystemId extQuotnSId, int status, DateTime quotnDate, DateTime? cdd, DateTime? edd, int custerSrc, long? customerId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName, string rcptName, string rcptCusterName, string rcptTel, string rcptFax, string rcptAddr, ISystemId freightWaySId, float? totalAmt, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extOrderSId == null) { throw new ArgumentNullException("extOrderSId"); }
            if (extQuotnSId == null) { throw new ArgumentNullException("extQuotnSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extOrderSId, extQuotnSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                //取新版號
                var version = this.GetNewVerNo(extQuotnSId);
                //停用作用中的資料
                this.LetNotActive(actorSId, extQuotnSId);

                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", extOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("EXT_QUOTN_SID", extQuotnSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("VERSION", version, GenericDBType.Int);
                transSet.SmartAdd("ACTIVE_FLAG", "Y", GenericDBType.Char, false);
                transSet.SmartAdd("STATUS", status, GenericDBType.Int);
                transSet.SmartAdd("QUOTN_DATE", quotnDate, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("WHSE", "外銷倉", GenericDBType.VarChar, false); //固定
                transSet.SmartAdd("CUSTER_SRC", custerSrc, GenericDBType.Int);
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt);
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_CON_ID", customerConId, GenericDBType.BigInt);
                transSet.SmartAdd("CUSTOMER_CON_NAME", customerConName, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_AREA_CODE", customerAreaCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_TEL", customerTel, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_FAX", customerFax, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_ADDR_ID", customerAddrId, GenericDBType.BigInt);
                transSet.SmartAdd("CUSTOMER_ADDR", customerAddr, GenericDBType.VarChar, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true);
                transSet.SmartAdd("RCPT_NAME", rcptName, GenericDBType.NVarChar, true);
                transSet.SmartAdd("RCPT_CUSTER_NAME", rcptCusterName, GenericDBType.NVarChar, true);
                transSet.SmartAdd("RCPT_TEL", rcptTel, GenericDBType.VarChar, true);
                transSet.SmartAdd("RCPT_FAX", rcptFax, GenericDBType.VarChar, true);
                transSet.SmartAdd("RCPT_ADDR", rcptAddr, GenericDBType.NVarChar, true);
                transSet.SmartAdd("FREIGHT_WAY_SID", freightWaySId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("TOTAL_AMT", totalAmt, GenericDBType.Float);
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
        /// 依指定的參數，修改一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="extOrderSId">指定的外銷訂單系統代號。</param>
        /// <param name="status">狀態（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程）。</param>
        /// <param name="quotnDate">報價單日期。</param>
        /// <param name="cdd">客戶需求日（null 則直接設為 DBNull）。</param>
        /// <param name="edd">預計交貨日（null 則直接設為 DBNull）。</param>
        /// <param name="custerSrc">客戶建立來源（1:ERP 客戶 2:手動建立）。</param>
        /// <param name="customerId">客戶 ID（null 則直接設為 DBNull）。</param>
        /// <param name="customerNumber">客戶編號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerName">客戶名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerConId">客戶聯絡人 ID（null 則直接設為 DBNull）。</param>
        /// <param name="customerConName">客戶聯絡人（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerAreaCode">客戶電話區碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerTel">客戶電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerFax">客戶傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerAddrId">客戶地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="customerAddr">客戶地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID（null 則直接設為 DBNull）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesRepId">營業員 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptName">收貨人（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptCusterName">收貨人客戶名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptTel">收貨人電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptFax">收貨人傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptAddr">收貨人地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="freightWaySId">貨運方式系統代號（null 則直接設為 DBNull）。</param>
        /// <param name="totalAmt">總金額（null 則直接設為 DBNull）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extQuotnSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extOrderSId,  int status, DateTime quotnDate, DateTime? cdd, DateTime? edd, int custerSrc, long? customerId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName, string rcptName, string rcptCusterName, string rcptTel, string rcptFax, string rcptAddr, ISystemId freightWaySId, float? totalAmt, string rmk)
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
                transSet.SmartAdd("STATUS", status, GenericDBType.Int);
                transSet.SmartAdd("QUOTN_DATE", quotnDate, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("WHSE", "外銷倉", GenericDBType.VarChar, false); //固定
                transSet.SmartAdd("CUSTER_SRC", custerSrc, GenericDBType.Int);
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_CON_ID", customerConId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CUSTOMER_CON_NAME", customerConName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_AREA_CODE", customerAreaCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_TEL", customerTel, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_FAX", customerFax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_ADDR_ID", customerAddrId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CUSTOMER_ADDR", customerAddr, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("RCPT_NAME", rcptName, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("RCPT_CUSTER_NAME", rcptCusterName, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("RCPT_TEL", rcptTel, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("RCPT_FAX", rcptFax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("RCPT_ADDR", rcptAddr, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("FREIGHT_WAY_SID", freightWaySId, base.SystemIdVerifier, GenericDBType.Char, true, true);
                transSet.SmartAdd("TOTAL_AMT", totalAmt, GenericDBType.Float, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateCustomerInfo
        /// <summary>
        /// 更新客戶資訊。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <param name="customerNumber">客戶編號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerName">客戶名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerConId">客戶聯絡人 ID（null 則直接設為 DBNull）。</param>
        /// <param name="customerConName">客戶聯絡人（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerAreaCode">客戶電話區碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerTel">客戶電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerFax">客戶傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerAddrId">客戶地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="customerAddr">客戶地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID（null 則直接設為 DBNull）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesRepId">營業員 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCustomerInfo(ISystemId actorSId, ISystemId extOrderSId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName)
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
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_CON_ID", customerConId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CUSTOMER_CON_NAME", customerConName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_AREA_CODE", customerAreaCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_TEL", customerTel, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_FAX", customerFax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_ADDR_ID", customerAddrId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CUSTOMER_ADDR", customerAddr, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
        /// 依外銷報價單系統代號停用作用中的資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extQuotnSId">外銷報價單系統代號。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        Returner LetNotActive(ISystemId actorSId, ISystemId extQuotnSId)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extQuotnSId == null) { throw new ArgumentNullException("extQuotnSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extQuotnSId);
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
                condsMain.Add("EXT_QUOTN_SID", SqlCond.EqualTo, extQuotnSId.ToString(), GenericDBType.Char, SqlCondsSet.And);
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
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <param name="status">狀態（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateStatus(ISystemId actorSId, ISystemId extOrderSId, int status)
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
                transSet.SmartAdd("STATUS", status, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extQuotnSId">外銷報價單系統代號。</param>
            /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginQuotnDate">範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endQuotnDate">範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdd">範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdd">範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] extOrderSIds, ISystemId extQuotnSId, int? version, bool? activeFlag, int[] statuses, DateTime? beginCdt, DateTime? endCdt, DateTime? beginQuotnDate, DateTime? endQuotnDate, DateTime? beginCdd, DateTime? endCdd, DateTime? beginEdd, DateTime? endEdd)
            {
                this.ExtOrderSIds = extOrderSIds;
                this.ExtQuotnSId = extQuotnSId;
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
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 外銷報價單系統代號。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 版本（若為 null 則略過條件檢查）。
            /// </summary>
            public int? Version { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
            /// <summary>
            /// 狀態陣列集合（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程; 若為 null 或陣列長度等於 0 則略過條件檢查）。
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

            custEntity.Append("SELECT [EXT_ORDER].* ");
            custEntity.Append("FROM [EXT_ORDER] ");

            var sqlConds = new List<string>();

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.ExtQuotnSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXT_QUOTN_SID", SqlOperator.EqualTo, qConds.ExtQuotnSId.Value, GenericDBType.Char));
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

        #region GetNewVerNo
        /// <summary>
        /// 取得新版號。
        /// </summary>
        /// <param name="extQuotnSId">外銷報價單系統代號。</param>
        /// <returns>新版號。</returns>
        int GetNewVerNo(ISystemId extQuotnSId)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add("ACTIVE_FLAG", SqlCond.EqualTo, "Y", GenericDBType.Char, SqlCondsSet.And);
            condsMain.Add("EXT_QUOTN_SID", SqlCond.EqualTo, extQuotnSId.ToString(), GenericDBType.Char, SqlCondsSet.And);

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
            /// 建立人姓名。
            /// </summary>
            [SchemaMapping(Name = "C_NAME", Type = ReturnType.String)]
            public string CName { get; set; }
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
            /// <summary>
            /// 貨運方式名稱。
            /// </summary>
            [SchemaMapping(Name = "FREIGHT_WAY_NAME", Type = ReturnType.String)]
            public string FreightWayName { get; set; }

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
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="cSIds">建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extQuotnSId">外銷報價單系統代號。</param>
            /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginQuotnDate">範圍查詢的起始報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endQuotnDate">範圍查詢的結束報價單日期（若為 null 則略過條件檢查）。</param>
            /// <param name="beginCdd">範圍查詢的起始客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdd">範圍查詢的結束客戶需求日（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            /// <param name="isReadjust">是否報價單調整中（若為 null 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] extOrderSIds, ISystemId[] cSIds, ISystemId extQuotnSId, int? version, bool? activeFlag, int[] statuses, long? customerId, DateTime? beginCdt, DateTime? endCdt, DateTime? beginQuotnDate, DateTime? endQuotnDate, DateTime? beginCdd, DateTime? endCdd, DateTime? beginEdd, DateTime? endEdd, bool? isCancel, bool? isReadjust)
            {
                this.ExtOrderSIds = extOrderSIds;
                this.CSIds = cSIds;
                this.ExtQuotnSId = extQuotnSId;
                this.Version = version;
                this.ActiveFlag = activeFlag;
                this.Statuses = statuses;
                this.CustomerId = customerId;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginQuotnDate = beginQuotnDate;
                this.EndQuotnDate = endQuotnDate;
                this.BeginCdd = beginCdd;
                this.EndCdd = endCdd;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.IsCancel = isCancel;
                this.IsReadjust = isReadjust;
            }

            /// <summary>
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] CSIds { get; set; }
            /// <summary>
            /// 外銷報價單系統代號。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 版本（若為 null 則略過條件檢查）。
            /// </summary>
            public int? Version { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
            /// <summary>
            /// 狀態陣列集合（1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] Statuses { get; set; }
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
            /// <summary>
            /// 是否報價單調整中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsReadjust { get; set; }
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

            custEntity.Append("SELECT [EXT_ORDER].* ");
            custEntity.Append("       , [EXT_QUOTN].[ODR_NO], [EXT_QUOTN].[STATUS] AS [EXT_QUOTN_STATUS], [EXT_QUOTN].[IS_CANCEL], [EXT_QUOTN].[IS_READJUST] ");
            custEntity.Append("       , [C_USER].[NAME] AS [C_NAME] ");
            custEntity.Append("       , [FREIGHT_WAY].[NAME] AS [FREIGHT_WAY_NAME] ");
            custEntity.Append("FROM [EXT_ORDER] ");
            custEntity.Append("     INNER JOIN [EXT_QUOTN] ON [EXT_QUOTN].[SID] = [EXT_ORDER].[EXT_QUOTN_SID] ");
            custEntity.Append("     INNER JOIN [SYS_USER] [C_USER] ON [C_USER].[SID] = [EXT_ORDER].[CSID] ");
            custEntity.Append("     LEFT JOIN [PUB_CAT] [FREIGHT_WAY] ON [FREIGHT_WAY].[SID] = [EXT_ORDER].[FREIGHT_WAY_SID] ");

            var sqlConds = new List<string>();

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.CSIds != null && qConds.CSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CSID", SqlOperator.EqualTo, SystemId.ToString(qConds.CSIds), GenericDBType.Char));
            }

            if (qConds.ExtQuotnSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "EXT_QUOTN_SID", SqlOperator.EqualTo, qConds.ExtQuotnSId.Value, GenericDBType.Char));
            }

            if (qConds.Version.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "VERSION", SqlOperator.EqualTo, qConds.Version.Value, GenericDBType.Int));
            }

            if (qConds.ActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_QUOTN", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.IsReadjust.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_QUOTN", "IS_READJUST", SqlOperator.EqualTo, qConds.IsReadjust.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "QUOTN_DATE", SqlOperator.GreaterEqualThan, qConds.BeginQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndQuotnDate != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "QUOTN_DATE", SqlOperator.LessEqualThan, qConds.EndQuotnDate.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.BeginCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CDD", SqlOperator.GreaterEqualThan, qConds.BeginCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndCdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CDD", SqlOperator.LessEqualThan, qConds.EndCdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 待出貨訂單檢視查詢
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class ReadyShipInfoView : Info
        {
            /// <summary>
            /// 建立人姓名。
            /// </summary>
            [SchemaMapping(Name = "C_NAME", Type = ReturnType.String)]
            public string CName { get; set; }
            /// <summary>
            /// 報價單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
            /// <summary>
            /// 外銷訂單明細的總數量。
            /// </summary>
            [SchemaMapping(Name = "TOTAL_DET_QTY", Type = ReturnType.Int)]
            public int TotalDetQty { get; set; }
            /// <summary>
            /// 外銷訂單明細的總數量。
            /// </summary>
            [SchemaMapping(Name = "TOTAL_SHIPPED_QTY", Type = ReturnType.Int)]
            public string TotalShippedQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static ReadyShipInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<ReadyShipInfoView>(new ReadyShipInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static ReadyShipInfoView[] Binding(DataTable dTable)
            {
                List<ReadyShipInfoView> infos = new List<ReadyShipInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(ReadyShipInfoView.Binding(row));
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
        public class ReadyShipInfoViewConds
        {
            ///// <summary>
            ///// 初始化 ReadyShipInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public ReadyShipInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 ReadyShipInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="cSIds">建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extQuotnSId">外銷報價單系統代號。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="onlyAvailable">是否僅取得尚有可用數量的外銷訂單，若否則取得全部。</param>
            public ReadyShipInfoViewConds(ISystemId[] extOrderSIds, ISystemId[] cSIds, ISystemId extQuotnSId, long? customerId, bool onlyAvailable)
            {
                this.ExtOrderSIds = extOrderSIds;
                this.CSIds = cSIds;
                this.ExtQuotnSId = extQuotnSId;
                this.CustomerId = customerId;
                this.OnlyAvailable = onlyAvailable;
            }

            /// <summary>
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 建立人系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] CSIds { get; set; }
            /// <summary>
            /// 外銷報價單系統代號。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 是否僅取得尚有可用數量的專案報價，若否則取得全部。
            /// </summary>
            public bool OnlyAvailable { get; set; }
        }
        #endregion

        #region GetReadyShipInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetReadyShipInfoView(ReadyShipInfoViewConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetReadyShipInfoViewCondsSet(qConds));

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
        public Returner GetReadyShipInfoView(ReadyShipInfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetReadyShipInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetReadyShipInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetReadyShipInfoViewCount(ReadyShipInfoViewConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetReadyShipInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetReadyShipInfoViewByCompoundSearch
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
        public Returner GetReadyShipInfoViewByCompoundSearch(ReadyShipInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetReadyShipInfoViewCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetReadyShipInfoViewByCompoundSearchCount
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
        public Returner GetReadyShipInfoViewByCompoundSearchCount(ReadyShipInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetReadyShipInfoViewCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetReadyShipInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetReadyShipInfoViewCondsSet(ReadyShipInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append(@"
                SELECT [EXT_ORDER].*
                       , [EXT_QUOTN].[ODR_NO]
                       , [C_USER].[NAME] AS [C_NAME]
	                   -- 外銷訂單明細的總數量
	                   , (SELECT COALESCE(SUM([EXT_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_ORDER_DET] WHERE [EXT_ORDER_DET].[EXT_ORDER_SID] = [EXT_ORDER].[SID]) AS [TOTAL_DET_QTY]
                       --已被外銷出貨單的選擇的總數量 (不取已註刪的資料)
                       , (SELECT COALESCE(SUM([EXT_SHIPPING_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] INNER JOIN [EXT_SHIPPING_ORDER] ON [EXT_SHIPPING_ORDER].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_SHIPPING_ORDER_SID] AND [EXT_SHIPPING_ORDER].[MDELED] = 'N' INNER JOIN [EXT_ORDER_DET] ON [EXT_ORDER_DET].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_ORDER_DET_SID] AND [EXT_ORDER_DET].[EXT_ORDER_SID] = [EXT_ORDER].[SID]) AS [TOTAL_SHIPPED_QTY]
                FROM [EXT_ORDER]
                     INNER JOIN [EXT_QUOTN] ON [EXT_QUOTN].[SID] = [EXT_ORDER].[EXT_QUOTN_SID]
                     INNER JOIN [SYS_USER] [C_USER] ON [C_USER].[SID] = [EXT_ORDER].[CSID]
                WHERE [EXT_ORDER].[ACTIVE_FLAG] = 'Y' AND [EXT_ORDER].[STATUS] IN (2,4)
                      AND [EXT_QUOTN].[IS_CANCEL] = 'N' AND [EXT_QUOTN].[IS_READJUST] = 'N'
            ");

            var sqlConds = new List<string>();

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.CSIds != null && qConds.CSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CSID", SqlOperator.EqualTo, SystemId.ToString(qConds.CSIds), GenericDBType.Char));
            }

            if (qConds.ExtQuotnSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "EXT_QUOTN_SID", SqlOperator.EqualTo, qConds.ExtQuotnSId.Value, GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_ORDER", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            if (qConds.OnlyAvailable)
            {
                condsMain.Add(new LeftRightPair<string, string>("TOTAL_DET_QTY", "TOTAL_SHIPPED_QTY"), SqlCond.GreaterThan, SqlCondsSet.And);
            }

            return condsMain;
        }
        #endregion
        #endregion
    }
}
