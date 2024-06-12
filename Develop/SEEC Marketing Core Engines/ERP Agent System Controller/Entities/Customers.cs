using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.DB;
using EzCoding.SystemEngines;
using Seec.Marketing.Erp;

namespace Seec.Marketing.Erp.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// ERP 客戶。
        /// 非實際資料表名稱。
        /// </summary>
        public const string CUSTOMERS = "CUSTOMERS";
    }

    /// <summary>
    /// ERP 客戶。
    /// </summary>
    public class Customers : SystemBase 
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.Customers 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public Customers(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.CUSTOMERS))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IErpCuster
        {
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long)]
            public long CustomerId { get; set; }
            /// <summary>
            /// 最後修改時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTimeFormat)]
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

        #region GetSyncInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSyncInfo(DateTime? afterTime)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT CUSTOMER_ID, CUSTOMER_NUMBER, CUSTOMER_NAME, SHIP_ADDRESS_ID, BILL_ADDRESS_ID, SHIP_ADDRESS1, BILL_ADDRESS1, CUSTOMER_CATEGORY_CODE, MEANING, SHIP_TO_SITE_USE_ID, INVOICE_TO_SITE_USE_ID, ORDER_TYPE_ID, TYPE_NAME, PRICE_LIST_ID, SALESREP_ID, SALES_NAME, AREA_CODE, PHONE, CONTACT_ID, CON_NAME, FAX, CURRENCY_CODE, LAST_UPDATE_DATE ");
            custEntity.Append("FROM APPS.XSEEC_CUSTOMER_DETAILS_V ");

            var sqlConds = new List<string>();

            if (afterTime.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "LAST_UPDATE_DATE", SqlOperator.GreaterEqualThan, afterTime.Value, GenericDBType.DateTime));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrEmpty(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, base.DefaultSqlOrder, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
    }
}
