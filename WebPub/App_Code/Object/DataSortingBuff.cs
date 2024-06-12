using System;

namespace Seec.Marketing
{
    /// <summary>
    /// 資料排序緩衝。
    /// </summary>
    public class DataSortingBuff
    {
        /// <summary>
        /// 系統代號。
        /// </summary>
        public string SId { get; set; }

        /// <summary>
        /// 原始的排序值。
        /// </summary>
        public string OldVal { get; set; }

        /// <summary>
        /// 新的排序值。
        /// </summary>
        public string NewVal { get; set; }
    }
}