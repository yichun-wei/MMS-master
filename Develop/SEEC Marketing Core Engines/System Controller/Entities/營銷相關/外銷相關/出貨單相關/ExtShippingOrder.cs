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
        /// 外銷出貨單。
        /// </summary>
        public const string EXT_SHIPPING_ORDER = "EXT_SHIPPING_ORDER";
    }

    /// <summary>
    /// 外銷出貨單的類別實作。
    /// </summary>
    public class ExtShippingOrder : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtShippingOrder(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.EXT_SHIPPING_ORDER))
        {
            base.DefaultSqlOrder = new SqlOrder("SID", Sort.Descending);
        }

        #region 出貨單編號
        /// <summary>
        /// 出貨單編號。
        /// </summary>
        public class OrderNo
        {
            /// <summary>
            /// 初始化 OrderNo 類別的新執行個體。
            /// </summary>
            public OrderNo()
            {
                this.Code = "DO";
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
            /// 出貨單編號代碼（固定「DO」）。
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 出貨單編號日期（西元年末兩碼）。
            /// </summary>
            public string Date { get; set; }
            /// <summary>
            /// 出貨單編號流水號。
            /// </summary>
            public int Seq { get; set; }

            /// <summary>
            /// 取得完整的出貨單編號。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                //DO + 西元年度末兩碼 + 四位流水號, 例「XD150001」.
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
            /// 出貨單編號代碼。
            /// </summary>
            [SchemaMapping(Name = "ODR_CODE", Type = ReturnType.String)]
            public string OdrCode { get; set; }
            /// <summary>
            /// 出貨單編號日期。
            /// </summary>
            [SchemaMapping(Name = "ODR_DATE", Type = ReturnType.String)]
            public string OdrDate { get; set; }
            /// <summary>
            /// 出貨單編號流水號。
            /// </summary>
            [SchemaMapping(Name = "ODR_SEQ", Type = ReturnType.Int)]
            public int OdrSeq { get; set; }
            /// <summary>
            /// 出貨單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
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
            /// 預計到港日。
            /// </summary>
            [SchemaMapping(Name = "ETA", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? Eta { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            [SchemaMapping(Name = "WHSE", Type = ReturnType.String)]
            public string Whse { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long)]
            public long CustomerId { get; set; }
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
            /// 折扣金額。
            /// </summary>
            [SchemaMapping(Name = "DCT_AMT", Type = ReturnType.Float, AllowNull = true)]
            public float? DctAmt { get; set; }
            /// <summary>
            /// 折扣後總金額。
            /// </summary>
            [SchemaMapping(Name = "DCT_TOTAL_AMT", Type = ReturnType.Float, AllowNull = true)]
            public float? DctTotalAmt { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:已確認 2:已出貨 3:已上傳）。
            /// </summary>
            [SchemaMapping(Name = "STATUS", Type = ReturnType.Int)]
            public int Status { get; set; }
            /// <summary>
            /// ERP 訂單 ID。
            /// </summary>
            [SchemaMapping(Name = "ERP_HEADER_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ErpHeaderId { get; set; }
            /// <summary>
            /// ERP 訂單號碼。
            /// </summary>
            [SchemaMapping(Name = "ERP_ORDER_NUMBER", Type = ReturnType.String)]
            public string ErpOrderNumber { get; set; }
            /// <summary>
            /// ERP 訂單狀態（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉）。
            /// </summary>
            [SchemaMapping(Name = "ERP_STATUS", Type = ReturnType.Int, AllowNull = true)]
            public int? ErpStatus { get; set; }
            /// <summary>
            /// ERP 交貨號碼。
            /// </summary>
            [SchemaMapping(Name = "SHIP_NUMBER", Type = ReturnType.String)]
            public string ErpShipNumber { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }
            /// <summary>
            /// 明細品項的加入方式（1:by 訂單 2:by 品項）。
            /// </summary>
            [SchemaMapping(Name = "DET_LAYOUT", Type = ReturnType.Int)]
            public int DetLayout { get; set; }

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
            /// 出貨單編號代碼。
            /// </summary>
            public string OdrCode { get; set; }
            /// <summary>
            /// 出貨單編號日期。
            /// </summary>
            public string OdrDate { get; set; }
            /// <summary>
            /// 出貨單編號流水號。
            /// </summary>
            public int? OdrSeq { get; set; }
            /// <summary>
            /// 出貨單編號。
            /// </summary>
            public string OdrNo { get; set; }
            /// <summary>
            /// 客戶需求日。
            /// </summary>
            public DateTime? Cdd { get; set; }
            /// <summary>
            /// 預計交貨日。
            /// </summary>
            public DateTime? Edd { get; set; }
            /// <summary>
            /// 預計到港日。
            /// </summary>
            public DateTime? Eta { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            public string Whse { get; set; }
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
            /// 折扣金額。
            /// </summary>
            public float? DctAmt { get; set; }
            /// <summary>
            /// 折扣後總金額。
            /// </summary>
            public float? DctTotalAmt { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:已確認 2:已出貨 3:已上傳）。
            /// </summary>
            public int? Status { get; set; }
            /// <summary>
            /// ERP 訂單 ID。
            /// </summary>
            public long? ErpHeaderId { get; set; }
            /// <summary>
            /// ERP 訂單號碼。
            /// </summary>
            public string ErpOrderNumber { get; set; }
            /// <summary>
            /// ERP 訂單狀態（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉）。
            /// </summary>
            public int? ErpStatus { get; set; }
            /// <summary>
            /// ERP 交貨號碼。
            /// </summary>
            public string ErpShipNumber { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
            /// <summary>
            /// 明細品項的加入方式（1:by 訂單 2:by 品項）。
            /// </summary>
            public int? DetLayout { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="extShippingOrderSId">指定的外銷出貨單系統代號。</param>
        /// <param name="orderNo">出貨單編號。</param>
        /// <param name="cdd">客戶需求日（null 將自動略過操作）。</param>
        /// <param name="edd">預計交貨日（null 將自動略過操作）。</param>
        /// <param name="eta">預計到港日（null 將自動略過操作）。</param>
        /// <param name="customerId">客戶 ID。</param>
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
        /// <param name="dctAmt">折扣金額（null 將自動略過操作）。</param>
        /// <param name="dctTotalAmt">折扣後總金額（null 將自動略過操作）。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <param name="detLayout">明細品項的加入方式（1:by 訂單 2:by 品項）。。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 orderNo 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId extShippingOrderSId, OrderNo orderNo, DateTime? cdd, DateTime? edd, DateTime? eta, long customerId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName, string rcptName, string rcptCusterName, string rcptTel, string rcptFax, string rcptAddr, ISystemId freightWaySId, float? totalAmt, float? dctAmt, float? dctTotalAmt, string rmk, int detLayout)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            if (orderNo == null) { throw new ArgumentNullException("orderNo"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", extShippingOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ODR_CODE", orderNo.Code, GenericDBType.Char, false);
                transSet.SmartAdd("ODR_DATE", orderNo.Date, GenericDBType.Char, false);
                transSet.SmartAdd("ODR_SEQ", orderNo.Seq, GenericDBType.Int);
                transSet.SmartAdd("ODR_NO", orderNo.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("ETA", eta, "yyyyMMdd", GenericDBType.Char);
                transSet.SmartAdd("WHSE", "外銷倉", GenericDBType.VarChar, false); //固定
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
                transSet.SmartAdd("DCT_AMT", dctAmt, GenericDBType.Float);
                transSet.SmartAdd("DCT_TOTAL_AMT", dctTotalAmt, GenericDBType.Float);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("DET_LAYOUT", detLayout, GenericDBType.Int);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify (不含客戶鎖定資訊)
        /// <summary>
        /// 依指定的參數，修改一筆外銷出貨單。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        /// <param name="cdd">客戶需求日（null 則直接設為 DBNull）。</param>
        /// <param name="edd">預計交貨日（null 則直接設為 DBNull）。</param>
        /// <param name="eta">預計到港日（null 則直接設為 DBNull）。</param>
        /// <param name="rcptName">收貨人（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptCusterName">收貨人客戶名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptTel">收貨人電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptFax">收貨人傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="rcptAddr">收貨人地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="freightWaySId">貨運方式系統代號（null 則直接設為 DBNull）。</param>
        /// <param name="totalAmt">總金額（null 則直接設為 DBNull）。</param>
        /// <param name="dctAmt">折扣金額（null 則直接設為 DBNull）。</param>
        /// <param name="dctTotalAmt">折扣後總金額（null 則直接設為 DBNull）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extShippingOrderSId, DateTime? cdd, DateTime? edd, DateTime? eta, string rcptName, string rcptCusterName, string rcptTel, string rcptFax, string rcptAddr, ISystemId freightWaySId, float? totalAmt, float? dctAmt, float? dctTotalAmt, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("ETA", eta, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("RCPT_NAME", rcptName, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("RCPT_CUSTER_NAME", rcptCusterName, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("RCPT_TEL", rcptTel, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("RCPT_FAX", rcptFax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("RCPT_ADDR", rcptAddr, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("FREIGHT_WAY_SID", freightWaySId, base.SystemIdVerifier, GenericDBType.Char, true, true);
                transSet.SmartAdd("TOTAL_AMT", totalAmt, GenericDBType.Float, true);
                transSet.SmartAdd("DCT_AMT", dctAmt, GenericDBType.Float, true);
                transSet.SmartAdd("DCT_TOTAL_AMT", dctTotalAmt, GenericDBType.Float, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify (完整)
        /// <summary>
        /// 依指定的參數，修改一筆外銷出貨單。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        /// <param name="cdd">客戶需求日（null 則直接設為 DBNull）。</param>
        /// <param name="edd">預計交貨日（null 則直接設為 DBNull）。</param>
        /// <param name="eta">預計到港日（null 則直接設為 DBNull）。</param>
        /// <param name="customerId">客戶 ID。</param>
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
        /// <param name="dctAmt">折扣金額（null 則直接設為 DBNull）。</param>
        /// <param name="dctTotalAmt">折扣後總金額（null 則直接設為 DBNull）。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extShippingOrderSId, DateTime? cdd, DateTime? edd, DateTime? eta, long customerId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName, string rcptName, string rcptCusterName, string rcptTel, string rcptFax, string rcptAddr, ISystemId freightWaySId, float? totalAmt, float? dctAmt, float? dctTotalAmt, string rmk)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                transSet.SmartAdd("CDD", cdd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("EDD", edd, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("ETA", eta, "yyyyMMdd", GenericDBType.Char, true);
                transSet.SmartAdd("WHSE", "外銷倉", GenericDBType.VarChar, false); //固定
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt);
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
                transSet.SmartAdd("DCT_AMT", dctAmt, GenericDBType.Float, true);
                transSet.SmartAdd("DCT_TOTAL_AMT", dctTotalAmt, GenericDBType.Float, true);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
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
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCustomerInfo(ISystemId actorSId, ISystemId extShippingOrderSId, string customerNumber, string customerName, long? customerConId, string customerConName, string customerAreaCode, string customerTel, string customerFax, long? customerAddrId, string customerAddr, long? priceListId, string currencyCode, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, long? salesRepId, string salesName)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateEta
        /// <summary>
        /// 更新預計到港日。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        /// <param name="eta">預計到港日（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateEta(ISystemId actorSId, ISystemId extShippingOrderSId, DateTime? eta)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                transSet.SmartAdd("ETA", eta, "yyyyMMdd", GenericDBType.Char, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        /// <param name="status">狀態（0:草稿 1:已確認 2:已出貨 3:已上傳）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateStatus(ISystemId actorSId, ISystemId extShippingOrderSId, int status)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateErpInfo
        /// <summary>
        /// 更新 ERP 訂單資訊。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        /// <param name="erpHeaderId">ERP 訂單 ID。</param>
        /// <param name="erpOrderNumber">ERP 訂單號碼。</param>
        /// <param name="erpStatus">ERP 訂單狀態（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉; null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="erpShipNumber">ERP 交貨號碼。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extShippingOrderSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateErpInfo(ISystemId actorSId, ISystemId extShippingOrderSId, long erpHeaderId, string erpOrderNumber, int? erpStatus, string erpShipNumber)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extShippingOrderSId == null) { throw new ArgumentNullException("extShippingOrderSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extShippingOrderSId);
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
                transSet.SmartAdd("ERP_HEADER_ID", erpHeaderId, GenericDBType.Int, true);
                transSet.SmartAdd("ERP_ORDER_NUMBER", erpOrderNumber, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("ERP_STATUS", erpStatus, GenericDBType.Int, true);
                transSet.SmartAdd("SHIP_NUMBER", erpShipNumber, GenericDBType.VarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extShippingOrderSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="extShippingOrderSIds">外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（0:草稿 1:已確認 2:已出貨 3:已上傳; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="erpStatuses">ERP 訂單狀態陣列集合（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] extShippingOrderSIds, int[] statuses, DateTime? beginCdt, DateTime? endCdt, DateTime? beginEdd, DateTime? endEdd, int[] erpStatuses)
            {
                this.DomOrderSIds = extShippingOrderSIds;
                this.Statuses = statuses;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.ErpStatuses = erpStatuses;
            }

            /// <summary>
            /// 外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderSIds { get; set; }
            /// <summary>
            /// 狀態陣列集合（0:草稿 1:已確認 2:已出貨 3:已上傳; 若為 null 或陣列長度等於 0 則略過條件檢查）。
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
            /// 範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
            /// <summary>
            /// ERP 訂單狀態陣列集合（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] ErpStatuses { get; set; }
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

            custEntity.Append("SELECT [EXT_SHIPPING_ORDER].* ");
            custEntity.Append("FROM [EXT_SHIPPING_ORDER] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderSIds != null && qConds.DomOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderSIds), GenericDBType.Char));
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

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.ErpStatuses != null && qConds.ErpStatuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ERP_STATUS", SqlOperator.EqualTo, qConds.ErpStatuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
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
        /// 取得新的出貨單編號。
        /// </summary>
        public OrderNo GetNewOrderNo()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                OrderNo orderNo = new OrderNo();

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
            /// <param name="extShippingOrderSIds">外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="statuses">狀態陣列集合（0:草稿 1:已確認 2:已出貨 3:已上傳; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginCdt">範圍查詢的起始建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="endCdt">範圍查詢的結束建立日期時間（若為 null 則略過條件檢查）。</param>
            /// <param name="beginEdd">範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="erpStatuses">ERP 訂單狀態陣列集合（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] extShippingOrderSIds, int[] statuses, DateTime? beginCdt, DateTime? endCdt, DateTime? beginEdd, DateTime? endEdd, int[] erpStatuses)
            {
                this.DomOrderSIds = extShippingOrderSIds;
                this.Statuses = statuses;
                this.BeginCdt = beginCdt;
                this.EndCdt = endCdt;
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.ErpStatuses = erpStatuses;
            }

            /// <summary>
            /// 外銷出貨單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] DomOrderSIds { get; set; }
            /// <summary>
            /// 狀態陣列集合（0:草稿 1:已確認 2:已出貨 3:已上傳; 若為 null 或陣列長度等於 0 則略過條件檢查）。
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
            /// 範圍查詢的起始預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計交貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
            /// <summary>
            /// ERP 訂單狀態陣列集合（1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] ErpStatuses { get; set; }
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

            custEntity.Append("SELECT [EXT_SHIPPING_ORDER].* ");
            custEntity.Append("       , [FREIGHT_WAY].[NAME] AS [FREIGHT_WAY_NAME] ");
            custEntity.Append("FROM [EXT_SHIPPING_ORDER] ");
            custEntity.Append("     LEFT JOIN [PUB_CAT] [FREIGHT_WAY] ON [FREIGHT_WAY].[SID] = [EXT_SHIPPING_ORDER].[FREIGHT_WAY_SID] ");

            var sqlConds = new List<string>();

            if (qConds.DomOrderSIds != null && qConds.DomOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.DomOrderSIds), GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.BeginCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "CDT", SqlOperator.GreaterEqualThan, qConds.BeginCdt.Value.ToString("yyyyMMdd000000"), GenericDBType.Char));
            }

            if (qConds.EndCdt != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "CDT", SqlOperator.LessEqualThan, qConds.EndCdt.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "EDD", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "EDD", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.ErpStatuses != null && qConds.ErpStatuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("EXT_SHIPPING_ORDER", "ERP_STATUS", SqlOperator.EqualTo, qConds.ErpStatuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region GetToBeUptErpOdrInfo
        /// <summary> 
        /// 取得待 ERP 狀態更新的訂單。
        /// </summary> 
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetToBeUptErpOdrInfo(params string[] inquireColumns)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append(@"
                SELECT [EXT_SHIPPING_ORDER].*
                FROM [EXT_SHIPPING_ORDER]
                WHERE [MDELED] = 'N' AND ([STATUS] = 3 OR [ERP_STATUS] IN (1,2,3,4,5))
            ");

            var sqlConds = new List<string>();

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion
    }
}
