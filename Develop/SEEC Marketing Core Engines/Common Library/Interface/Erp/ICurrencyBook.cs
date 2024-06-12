using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 幣別表介面。
    /// </summary>
    public interface ICurrencyBook
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
        /// 幣別。
        /// </summary>
        string CurrencyCode { get; set; }
    }
}
