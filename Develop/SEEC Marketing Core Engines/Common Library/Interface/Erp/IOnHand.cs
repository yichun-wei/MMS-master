using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 倉庫在手量介面。
    /// </summary>
    public interface IOnHand
    {
        /// <summary>
        /// 料號 ID。
        /// </summary>
        long InventoryItemId { get; set; }
        /// <summary>
        /// 料號。
        /// </summary>
        string Segment1 { get; set; }
        /// <summary>
        /// 倉庫。
        /// </summary>
        string SubinventoryCode { get; set; }
        /// <summary>
        /// 在手量。
        /// </summary>
        int OnhandQty { get; set; }
    }
}
