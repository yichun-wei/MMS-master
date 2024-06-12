using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// SO-LINE-DETAILS-INTERFACE 介面。
    /// </summary>
    public interface ISOLineDetailsInterface
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
        /// 常數數值：1103。
        /// </summary>
        int OrderSourceId { get; set; }
        /// <summary>
        /// 內銷-訂單號碼；外銷-交貨單號碼。
        /// </summary>
        string OriginalSystemReference { get; set; }
        /// <summary>
        /// 明細項次，ORIGINAL_SYSTEM_REFERENCE + ORIGINAL_SYSTEM_LINE_REFERENCE 兩個欄位串聯須為 UNIQUE。
        /// </summary>
        string OriginalSystemLineReference { get; set; }
        /// <summary>
        /// 訂單數量。
        /// </summary>
        int Quantity { get; set; }
        /// <summary>
        /// 預計出貨日(內外銷都相同規則)。
        /// </summary>
        DateTime? ScheduleDate { get; set; }
        /// <summary>
        /// 倉庫名稱。
        /// </summary>
        string Subinventory { get; set; }
        /// <summary>
        /// 常數數值：228。
        /// </summary>
        int? WarehouseId { get; set; }
        //[20160421 by 米雪]
        /// <summary>
        /// 常數：DEMANDED。
        /// </summary>
        string ScheduleStatusCode { get; set; }
    }
}
