using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 客戶介面。
    /// </summary>
    public interface IErpCuster
    {
        /// <summary>
        /// 客戶 ID。
        /// </summary>
        long CustomerId { get; set; }
        /// <summary>
        /// 最後修改時間。
        /// </summary>
        DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// 客戶編號。
        /// </summary>
        string CustomerNumber { get; set; }
        /// <summary>
        /// 客戶名稱。
        /// </summary>
        string CustomerName { get; set; }
        /// <summary>
        /// 送貨地址 ID。
        /// </summary>
        long? ShipAddressId { get; set; }
        /// <summary>
        /// 送貨地址。
        /// </summary>
        string ShipAddress1 { get; set; }
        /// <summary>
        /// 帳單地址 ID。
        /// </summary>
        long? BillAddressId { get; set; }
        /// <summary>
        /// 帳單地址。
        /// </summary>
        string BillAddress1 { get; set; }
        /// <summary>
        /// 客戶種類代碼。
        /// </summary>
        string CustomerCategoryCode { get; set; }
        /// <summary>
        /// 客戶種類。
        /// </summary>
        string Meaning { get; set; }
        /// <summary>
        /// 送貨地點 ID。
        /// </summary>
        long? ShipToSiteUseId { get; set; }
        /// <summary>
        /// 帳單地點 ID。
        /// </summary>
        long? InvoiceToSiteUseId { get; set; }
        /// <summary>
        /// 訂單型態 ID。
        /// </summary>
        long? OrderTypeId { get; set; }
        /// <summary>
        /// 訂單型態。
        /// </summary>
        string TypeName { get; set; }
        /// <summary>
        /// 價目表 ID。
        /// </summary>
        long? PriceListId { get; set; }
        /// <summary>
        /// 幣別。
        /// </summary>
        string CurrencyCode { get; set; }
        /// <summary>
        /// 營業員 ID。
        /// </summary>
        long? SalesRepId { get; set; }
        /// <summary>
        /// 營業員姓名。
        /// </summary>
        string SalesName { get; set; }
        /// <summary>
        /// 電話區碼。
        /// </summary>
        string AreaCode { get; set; }
        /// <summary>
        /// 電話。
        /// </summary>
        string Phone { get; set; }
        /// <summary>
        /// 聯絡人 ID。
        /// </summary>
        long? ContactId { get; set; }
        /// <summary>
        /// 聯絡人姓名。
        /// </summary>
        string ConName { get; set; }
        /// <summary>
        /// 傳真。
        /// </summary>
        string Fax { get; set; }
    }
}
