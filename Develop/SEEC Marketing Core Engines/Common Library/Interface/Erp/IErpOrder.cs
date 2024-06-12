using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 訂單介面。
    /// </summary>
    public interface IErpOrder
    {
        /// <summary>
        /// ERP 訂單 ID。
        /// </summary>
        long HeaderId { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        DateTime CreationDate { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        int CreatedBY { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        string OriginalSystemSourceCode { get; set; }
        /// <summary>
        /// XS 營銷訂單號碼。
        /// </summary>
        string OriginalSystemReference { get; set; }
        /// <summary>
        /// ERP 訂單號碼。
        /// </summary>
        int? OrderNumber { get; set; }
        /// <summary>
        /// ERP 訂單狀態（已輸入,已登錄,已超額,超額已核發,已取消,已關閉）。
        /// </summary>
        string OrderStatus { get; set; }
        /// <summary>
        /// ERP 交貨號碼。
        /// </summary>
        string ShipNumber { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        string Attribute1 { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        string Attribute2 { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        string Attribute3 { get; set; }
        /// <summary>
        /// 訂單狀態最後更新時間。
        /// </summary>
        DateTime? LastUpdateDate { get; set; }
        /// <summary>
        /// 。
        /// </summary>
        string OpenFlag { get; set; }
    }
}
