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
        /// ERP 客戶。
        /// </summary>
        public const string ERP_CUSTER = "ERP_CUSTER";
    }

    /// <summary>
    /// ERP 客戶的類別實作。
    /// </summary>
    public class ErpCuster : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ErpCuster 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ErpCuster(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ERP_CUSTER))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("CUSTOMER_NUMBER", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase, IErpCuster
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
            /// 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long)]
            public long CustomerId { get; set; }
            /// <summary>
            /// 最後修改時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTime)]
            public DateTime LastUpdateDate { get; set; }
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
            /// 送貨地址 ID。
            /// </summary>
            [SchemaMapping(Name = "SHIP_ADDRESS_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ShipAddressId { get; set; }
            /// <summary>
            /// 送貨地址。
            /// </summary>
            [SchemaMapping(Name = "SHIP_ADDRESS1", Type = ReturnType.String)]
            public string ShipAddress1 { get; set; }
            /// <summary>
            /// 帳單地址 ID。
            /// </summary>
            [SchemaMapping(Name = "BILL_ADDRESS_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? BillAddressId { get; set; }
            /// <summary>
            /// 帳單地址。
            /// </summary>
            [SchemaMapping(Name = "BILL_ADDRESS1", Type = ReturnType.String)]
            public string BillAddress1 { get; set; }
            /// <summary>
            /// 客戶種類代碼。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_CATEGORY_CODE", Type = ReturnType.String)]
            public string CustomerCategoryCode { get; set; }
            /// <summary>
            /// 客戶種類。
            /// </summary>
            [SchemaMapping(Name = "MEANING", Type = ReturnType.String)]
            public string Meaning { get; set; }
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
            /// 訂單型態。
            /// </summary>
            [SchemaMapping(Name = "TYPE_NAME", Type = ReturnType.String)]
            public string TypeName { get; set; }
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
            /// 電話區碼。
            /// </summary>
            [SchemaMapping(Name = "AREA_CODE", Type = ReturnType.String)]
            public string AreaCode { get; set; }
            /// <summary>
            /// 電話。
            /// </summary>
            [SchemaMapping(Name = "PHONE", Type = ReturnType.String)]
            public string Phone { get; set; }
            /// <summary>
            /// 聯絡人 ID。
            /// </summary>
            [SchemaMapping(Name = "CONTACT_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ContactId { get; set; }
            /// <summary>
            /// 聯絡人姓名。
            /// </summary>
            [SchemaMapping(Name = "CON_NAME", Type = ReturnType.String)]
            public string ConName { get; set; }
            /// <summary>
            /// 傳真。
            /// </summary>
            [SchemaMapping(Name = "FAX", Type = ReturnType.String)]
            public string Fax { get; set; }
            /// <summary>
            /// 市場範圍（0:未知 1:內銷 2:外銷）。
            /// </summary>
            [SchemaMapping(Name = "MKTG_RANGE", Type = ReturnType.Int)]
            public int MktgRange { get; set; }
            /// <summary>
            /// 地區篩選。
            /// </summary>
            [SchemaMapping(Name = "DIST_FILTER", Type = ReturnType.String)]
            public string DistFilter { get; set; }

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
        /// <param name="erpCusterSId">指定的 ERP 客戶系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="customerId">客戶 ID。</param>
        /// <param name="lastUpdateDate">最後修改時間。</param>
        /// <param name="customerNumber">客戶編號。</param>
        /// <param name="customerName">客戶名稱。</param>
        /// <param name="shipAddressId">送貨地址 ID（null 將自動略過操作）。</param>
        /// <param name="shipAddress1">送貨地址（null 或 empty 將自動略過操作）。</param>
        /// <param name="billAddressId">帳單地址 ID（null 將自動略過操作）。</param>
        /// <param name="billAddress1">帳單地址（null 或 empty 將自動略過操作）。</param>
        /// <param name="customerCategoryCode">客戶種類代碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="meaning">客戶種類（null 或 empty 將自動略過操作）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 將自動略過操作）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 將自動略過操作）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 將自動略過操作）。</param>
        /// <param name="typeName">訂單型態（null 或 empty 將自動略過操作）。</param>
        /// <param name="priceListId">價目表 ID（null 將自動略過操作）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 將自動略過操作）。</param>
        /// <param name="salesRepId">營業員 ID（null 將自動略過操作）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 將自動略過操作）。</param>
        /// <param name="areaCode">電話區碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="phone">電話（null 或 empty 將自動略過操作）。</param>
        /// <param name="contactId">聯絡人 ID（null 將自動略過操作）。</param>
        /// <param name="conName">聯絡人姓名（null 或 empty 將自動略過操作）。</param>
        /// <param name="fax">傳真（null 或 empty 將自動略過操作）。</param>
        /// <param name="mktgRange">市場範圍（0:未知 1:內銷 2:外銷）。</param>
        /// <param name="distFilter">地區篩選（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpCusterSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId erpCusterSId, bool enabled, long customerId, DateTime lastUpdateDate, string customerNumber, string customerName, long? shipAddressId, string shipAddress1, long? billAddressId, string billAddress1, string customerCategoryCode, string meaning, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, string typeName, long? priceListId, string currencyCode, long? salesRepId, string salesName, string areaCode, string phone, long? contactId, string conName, string fax, int mktgRange, string distFilter)
        {
            if (erpCusterSId == null) { throw new ArgumentNullException("erpCusterSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(erpCusterSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", erpCusterSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, false);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, false);
                transSet.SmartAdd("SHIP_ADDRESS_ID", shipAddressId, GenericDBType.BigInt);
                transSet.SmartAdd("SHIP_ADDRESS1", shipAddress1, GenericDBType.VarChar, true);
                transSet.SmartAdd("BILL_ADDRESS_ID", billAddressId, GenericDBType.BigInt);
                transSet.SmartAdd("BILL_ADDRESS1", billAddress1, GenericDBType.VarChar, true);
                transSet.SmartAdd("CUSTOMER_CATEGORY_CODE", customerCategoryCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("MEANING", meaning, GenericDBType.VarChar, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt);
                transSet.SmartAdd("TYPE_NAME", typeName, GenericDBType.VarChar, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true);
                transSet.SmartAdd("AREA_CODE", areaCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("PHONE", phone, GenericDBType.VarChar, true);
                transSet.SmartAdd("CONTACT_ID", contactId, GenericDBType.BigInt);
                transSet.SmartAdd("CON_NAME", conName, GenericDBType.VarChar, true);
                transSet.SmartAdd("FAX", fax, GenericDBType.VarChar, true);
                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.Int);
                transSet.SmartAdd("DIST_FILTER", distFilter, GenericDBType.NVarChar, true);

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
        /// 依指定的參數，修改一筆 ERP 客戶。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="customerId">客戶 ID。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="lastUpdateDate">最後修改時間。</param>
        /// <param name="customerNumber">客戶編號。</param>
        /// <param name="customerName">客戶名稱。</param>
        /// <param name="shipAddressId">送貨地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="shipAddress1">送貨地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="billAddressId">帳單地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="billAddress1">帳單地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerCategoryCode">客戶種類代碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="meaning">客戶種類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 則直接設為 DBNull）。</param>
        /// <param name="typeName">訂單型態（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID（null 則直接設為 DBNull）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="salesRepId">營業員 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="areaCode">電話區碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="phone">電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="contactId">聯絡人 ID（null 則直接設為 DBNull）。</param>
        /// <param name="conName">聯絡人姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="fax">傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="mktgRange">市場範圍（0:未知 1:內銷 2:外銷）。</param>
        /// <param name="distFilter">地區篩選（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, long customerId, bool enabled, DateTime lastUpdateDate, string customerNumber, string customerName, long? shipAddressId, string shipAddress1, long? billAddressId, string billAddress1, string customerCategoryCode, string meaning, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, string typeName, long? priceListId, string currencyCode, long? salesRepId, string salesName, string areaCode, string phone, long? contactId, string conName, string fax, int mktgRange, string distFilter)
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
                transSet.SmartAdd("CUSTOMER_ID", customerId, GenericDBType.BigInt);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, false);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, false);
                transSet.SmartAdd("SHIP_ADDRESS_ID", shipAddressId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SHIP_ADDRESS1", shipAddress1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("BILL_ADDRESS_ID", billAddressId, GenericDBType.BigInt, true);
                transSet.SmartAdd("BILL_ADDRESS1", billAddress1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_CATEGORY_CODE", customerCategoryCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("MEANING", meaning, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt, true);
                transSet.SmartAdd("TYPE_NAME", typeName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("AREA_CODE", areaCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PHONE", phone, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CONTACT_ID", contactId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CON_NAME", conName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("FAX", fax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.Int);
                transSet.SmartAdd("DIST_FILTER", distFilter, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("CUSTOMER_ID", SqlCond.EqualTo, customerId, GenericDBType.BigInt, string.Empty);

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
        /// 依指定的參數，修改一筆 ERP 客戶。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="erpCusterSId">ERP 客戶系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="lastUpdateDate">最後修改時間。</param>
        /// <param name="customerNumber">客戶編號。</param>
        /// <param name="customerName">客戶名稱。</param>
        /// <param name="shipAddressId">送貨地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="shipAddress1">送貨地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="billAddressId">帳單地址 ID（null 則直接設為 DBNull）。</param>
        /// <param name="billAddress1">帳單地址（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="customerCategoryCode">客戶種類代碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="meaning">客戶種類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="shipToSiteUseId">送貨地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="invoiceToSiteUseId">帳單地點 ID（null 則直接設為 DBNull）。</param>
        /// <param name="orderTypeId">訂單型態 ID（null 則直接設為 DBNull）。</param>
        /// <param name="typeName">訂單型態（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="priceListId">價目表 ID（null 則直接設為 DBNull）。</param>
        /// <param name="currencyCode">幣別（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="salesRepId">營業員 ID（null 則直接設為 DBNull）。</param>
        /// <param name="salesName">營業員姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="areaCode">電話區碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="phone">電話（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="contactId">聯絡人 ID（null 則直接設為 DBNull）。</param>
        /// <param name="conName">聯絡人姓名（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="fax">傳真（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="mktgRange">市場範圍（0:未知 1:內銷 2:外銷）。</param>
        /// <param name="distFilter">地區篩選（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpCusterSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId erpCusterSId, bool enabled, DateTime lastUpdateDate, string customerNumber, string customerName, long? shipAddressId, string shipAddress1, long? billAddressId, string billAddress1, string customerCategoryCode, string meaning, long? shipToSiteUseId, long? invoiceToSiteUseId, long? orderTypeId, string typeName, long? priceListId, string currencyCode, long? salesRepId, string salesName, string areaCode, string phone, long? contactId, string conName, string fax, int mktgRange, string distFilter)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (erpCusterSId == null) { throw new ArgumentNullException("erpCusterSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, erpCusterSId);
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
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CUSTOMER_NUMBER", customerNumber, GenericDBType.VarChar, false);
                transSet.SmartAdd("CUSTOMER_NAME", customerName, GenericDBType.VarChar, false);
                transSet.SmartAdd("SHIP_ADDRESS_ID", shipAddressId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SHIP_ADDRESS1", shipAddress1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("BILL_ADDRESS_ID", billAddressId, GenericDBType.BigInt, true);
                transSet.SmartAdd("BILL_ADDRESS1", billAddress1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CUSTOMER_CATEGORY_CODE", customerCategoryCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("MEANING", meaning, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", shipToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", invoiceToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_TYPE_ID", orderTypeId, GenericDBType.BigInt, true);
                transSet.SmartAdd("TYPE_NAME", typeName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PRICE_LIST_ID", priceListId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CURRENCY_CODE", currencyCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SALESREP_ID", salesRepId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SALES_NAME", salesName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("AREA_CODE", areaCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("PHONE", phone, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("CONTACT_ID", contactId, GenericDBType.BigInt, true);
                transSet.SmartAdd("CON_NAME", conName, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("FAX", fax, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.Int);
                transSet.SmartAdd("DIST_FILTER", distFilter, GenericDBType.NVarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, erpCusterSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="erpCusterSIds">ERP 客戶系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerIds">客戶 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="priceListIds">價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="mktgRanges">市場範圍陣列集合（0:未知 1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="distFilters">地區篩選陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="keyword">關鍵字（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] erpCusterSIds, long[] customerIds, long[] priceListIds, int[] mktgRanges, string[] distFilters, string keyword)
            {
                this.ErpCusterSIds = erpCusterSIds;
                this.CustomerIds = customerIds;
                this.PriceListIds = priceListIds;
                this.MktgRanges = mktgRanges;
                this.DistFilters = distFilters;
                this.Keyword = keyword;
            }

            /// <summary>
            /// ERP 客戶系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ErpCusterSIds { get; set; }
            /// <summary>
            /// 客戶 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] CustomerIds { get; set; }
            /// <summary>
            /// 價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] PriceListIds { get; set; }
            /// <summary>
            /// 市場範圍陣列集合（0:未知 1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] MktgRanges { get; set; }
            /// <summary>
            /// 地區篩選陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] DistFilters { get; set; }
            /// <summary>
            /// 關鍵字（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Keyword { get; set; }
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

            custEntity.Append("SELECT [ERP_CUSTER].* ");
            custEntity.Append("FROM [ERP_CUSTER] ");

            var sqlConds = new List<string>();

            if (qConds.ErpCusterSIds != null && qConds.ErpCusterSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ErpCusterSIds), GenericDBType.Char));
            }

            if (qConds.CustomerIds != null && qConds.CustomerIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.PriceListIds != null && qConds.PriceListIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PRICE_LIST_ID", SqlOperator.EqualTo, qConds.PriceListIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.MktgRanges != null && qConds.MktgRanges.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "MKTG_RANGE", SqlOperator.EqualTo, qConds.MktgRanges.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.DistFilters != null && qConds.DistFilters.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DIST_FILTER", SqlOperator.EqualTo, qConds.DistFilters, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Keyword))
            {
                var conds = new List<string>();

                string syntaxParam = custEntity.AddParameter("Keyword", qConds.Keyword, GenericDBType.NVarChar);

                conds.Add(custEntity.BuildConds(string.Empty, "CUSTOMER_NUMBER", SqlOperator.LikeThan, syntaxParam));
                conds.Add(custEntity.BuildConds(string.Empty, "CUSTOMER_NAME", SqlOperator.LikeThan, syntaxParam));

                sqlConds.Add(string.Format("({0})", string.Join(" OR ", conds.ToArray())));
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
            /// 明細折扣 ID。
            /// </summary>
            [SchemaMapping(Name = "DET_DISCOUNT_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? DetDiscountId { get; set; }

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
            /// <param name="erpCusterSIds">ERP 客戶系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerIds">客戶 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="priceListIds">價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="mktgRanges">市場範圍陣列集合（0:未知 1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="distFilters">地區篩選陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="keyword">關鍵字（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoViewConds(ISystemId[] erpCusterSIds, long[] customerIds, long[] priceListIds, int[] mktgRanges, string[] distFilters, string keyword)
            {
                this.ErpCusterSIds = erpCusterSIds;
                this.CustomerIds = customerIds;
                this.PriceListIds = priceListIds;
                this.MktgRanges = mktgRanges;
                this.DistFilters = distFilters;
                this.Keyword = keyword;
            }

            /// <summary>
            /// ERP 客戶系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ErpCusterSIds { get; set; }
            /// <summary>
            /// 客戶 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] CustomerIds { get; set; }
            /// <summary>
            /// 價目表 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] PriceListIds { get; set; }
            /// <summary>
            /// 市場範圍陣列集合（0:未知 1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] MktgRanges { get; set; }
            /// <summary>
            /// 地區篩選陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] DistFilters { get; set; }
            /// <summary>
            /// 關鍵字（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Keyword { get; set; }
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

        #region GetGroupInfoView
        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupInfoView(InfoViewConds qConds, string[] fieldNames, Sort sort)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoViewCondsSet(qConds));

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

            custEntity.Append("SELECT [ERP_CUSTER].* ");
            custEntity.Append("       , [DCT_ERP_DCT].[DISCOUNT_ID] AS [DET_DISCOUNT_ID] ");
            custEntity.Append("FROM [ERP_CUSTER] ");
            custEntity.Append("     LEFT JOIN [ERP_DCT] [DCT_ERP_DCT] ON [DCT_ERP_DCT].[PRICE_LIST_ID] = [ERP_CUSTER].[PRICE_LIST_ID] AND [DCT_ERP_DCT].[DISCOUNT_TYPE] = CASE [ERP_CUSTER].[MKTG_RANGE] WHEN 1 THEN 2 WHEN 2 THEN 4 END ");

            var sqlConds = new List<string>();

            if (qConds.ErpCusterSIds != null && qConds.ErpCusterSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_CUSTER", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ErpCusterSIds), GenericDBType.Char));
            }

            if (qConds.CustomerIds != null && qConds.CustomerIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_CUSTER", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.PriceListIds != null && qConds.PriceListIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_CUSTER", "PRICE_LIST_ID", SqlOperator.EqualTo, qConds.PriceListIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.MktgRanges != null && qConds.MktgRanges.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_CUSTER", "MKTG_RANGE", SqlOperator.EqualTo, qConds.MktgRanges.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.DistFilters != null && qConds.DistFilters.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ERP_CUSTER", "DIST_FILTER", SqlOperator.EqualTo, qConds.DistFilters, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Keyword))
            {
                var conds = new List<string>();

                string syntaxParam = custEntity.AddParameter("Keyword", qConds.Keyword, GenericDBType.NVarChar);

                conds.Add(custEntity.BuildConds("ERP_CUSTER", "CUSTOMER_NUMBER", SqlOperator.LikeThan, syntaxParam));
                conds.Add(custEntity.BuildConds("ERP_CUSTER", "CUSTOMER_NAME", SqlOperator.LikeThan, syntaxParam));

                sqlConds.Add(string.Format("({0})", string.Join(" OR ", conds.ToArray())));
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
