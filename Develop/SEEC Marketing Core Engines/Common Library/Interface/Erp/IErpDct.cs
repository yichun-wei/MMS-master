using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 折扣介面。
    /// </summary>
    public interface IErpDct
    {
        /// <summary>
        /// 折扣 ID。
        /// </summary>
        long DiscountId { get; set; }
        /// <summary>
        /// 折扣名稱。
        /// </summary>
        string DiscountName { get; set; }
        /// <summary>
        /// 自動計算折扣。
        /// </summary>
        bool? AutomaticDiscountFlag { get; set; }
        /// <summary>
        /// 允許修訂。
        /// </summary>
        bool? OverrideAllowedFlag { get; set; }
        /// <summary>
        /// 價目表 ID。
        /// </summary>
        long PriceListId { get; set; }
        /// <summary>
        /// 價目表名稱。
        /// </summary>
        string ListName { get; set; }
        /// <summary>
        /// 折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。
        /// </summary>
        int DiscountType { get; set; }
        /// <summary>
        /// 折扣最後更新日期。
        /// </summary>
        DateTime LastUpdateDate { get; set; }
    }
}
