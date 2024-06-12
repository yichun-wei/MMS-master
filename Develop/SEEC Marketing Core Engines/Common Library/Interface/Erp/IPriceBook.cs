using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 價目表介面。
    /// </summary>
    public interface IPriceBook
    {
        /// <summary>
        /// 價目表名稱。
        /// </summary>
        string ListName { get; set; }
        /// <summary>
        /// 價目表 ID。
        /// </summary>
        long PriceListId { get; set; }
        /// <summary>
        /// 料號 ID。
        /// </summary>
        long InventoryItemId { get; set; }
        /// <summary>
        /// 料號。
        /// </summary>
        string Segment1 { get; set; }
        /// <summary>
        /// 單位。
        /// </summary>
        string PrimaryUnitOFMeasure { get; set; }
        /// <summary>
        /// 摘要。
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 牌價。
        /// </summary>
        float ListPrice { get; set; }
        /// <summary>
        /// 幣別。
        /// </summary>
        string CurrencyCode { get; set; }
    }
}
