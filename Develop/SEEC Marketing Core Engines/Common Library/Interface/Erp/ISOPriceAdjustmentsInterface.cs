using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// SO-PRICE-ADJUSTMENTS-INTERFACE 介面。
    /// </summary>
    public interface ISOPriceAdjustmentsInterface
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
        /// 明細項次。
        /// </summary>
        string OriginalSystemLineReference { get; set; }
        /// <summary>
        /// 折扣 ID。
        /// </summary>
        long? DiscountId { get; set; }
        /// <summary>
        /// 折扣比率數值。
        /// </summary>
        float? Percent { get; set; }
    }
}
