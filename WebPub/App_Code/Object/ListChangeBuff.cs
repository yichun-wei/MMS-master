using System;

namespace Seec.Marketing
{
    /// <summary>
    /// 列表頁異動緩衝。
    /// </summary>
    public class ListChangeBuff
    {
        /// <summary>
        /// 系統代號。
        /// </summary>
        public string SId { get; set; }

        /// <summary>
        /// 原始值。
        /// </summary>
        public string OldVal { get; set; }

        /// <summary>
        /// 異動值。
        /// </summary>
        public string NewVal { get; set; }
    }
}