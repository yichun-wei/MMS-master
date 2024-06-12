using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seec.Marketing
{
    /// <summary>
    /// 外銷商品分類介面。
    /// </summary>
    public interface IExtItemCats
    {
        /// <summary>
        /// 產品別。
        /// </summary>
        string ExportItemType { get; set; }
        /// <summary>
        /// 分類 1。
        /// </summary>
        string Category1 { get; set; }
        /// <summary>
        /// 分類 2。
        /// </summary>
        string Category2 { get; set; }
        /// <summary>
        /// 分類 3。
        /// </summary>
        string Category3 { get; set; }
        /// <summary>
        /// 分類 4。
        /// </summary>
        string Category4 { get; set; }
        /// <summary>
        /// 分類 5。
        /// </summary>
        string Category5 { get; set; }
    }
}
