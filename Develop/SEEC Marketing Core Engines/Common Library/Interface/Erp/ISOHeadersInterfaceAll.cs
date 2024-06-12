using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// SO-PRICE-ADJUSTMENTS-INTERFACE 介面。
    /// </summary>
    public interface ISOHeadersInterfaceAll
    {
        /// <summary>
        /// SYSDATE。
        /// </summary>
        DateTime CreationDate { get; set; }
        /// <summary>
        /// 常數數值：6214。
        /// </summary>
        int CreatedBY { get; set; }
        /// <summary>
        /// SYSDATE。
        /// </summary>
        DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// 常數數值：6214。
        /// </summary>
        int LastUpdatedBY { get; set; }
        /// <summary>
        /// 內銷-訂單號碼；外銷-交貨單號碼。
        /// </summary>
        string OriginalSystemReference { get; set; }
        /// <summary>
        /// 客戶檔之 ID。
        /// </summary>
        long? CustomerId { get; set; }
        /// <summary>
        /// 客戶檔之 ORDER_TYPE_ID。
        /// </summary>
        long? OrderTypeId { get; set; }
        /// <summary>
        /// 常數數值：1103。
        /// </summary>
        int OrderSourceId { get; set; }
        /// <summary>
        /// 常數：R。
        /// </summary>
        string OrderCategory { get; set; }
        /// <summary>
        /// 訂單日期。
        /// </summary>
        DateTime DateOrdered { get; set; }
        /// <summary>
        /// 幣別。
        /// </summary>
        string CurrencyCode { get; set; }
        /// <summary>
        /// 常數數值：1020。
        /// 外銷用。
        /// </summary>
        string ConversionTypeCode { get; set; }
        /// <summary>
        /// 客戶檔之營業員 ID。
        /// </summary>
        long? SalesRepId { get; set; }
        /// <summary>
        /// 客戶檔之帳單地點 ID。
        /// </summary>
        long? InvoiceToSiteUseId { get; set; }
        /// <summary>
        /// 客戶檔之送貨地點 ID。
        /// </summary>
        long? ShipToSiteUseId { get; set; }
        /// <summary>
        /// 客戶檔之價目表 ID。
        /// </summary>
        long? PriceListId { get; set; }
        /// <summary>
        /// 常數數值：1。
        /// </summary>
        int? EnteredStateId { get; set; }
    }
}
