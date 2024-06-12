using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 庫存介面。
    /// </summary>
    public interface IErpInv
    {
        /// <summary>
        /// 料號 ID。
        /// </summary>
        long InventoryItemId { get; set; }
        /// <summary>
        /// 料號。
        /// </summary>
        string Item { get; set; }
        /// <summary>
        /// 料號摘要。
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 料號重量。
        /// </summary>
        float? UnitWeight { get; set; }
        /// <summary>
        /// 重量單位。
        /// </summary>
        string WeightUomCode { get; set; }
        /// <summary>
        /// 預設明細折數。
        /// </summary>
        float? Discount { get; set; }
        /// <summary>
        /// 料號大分類。
        /// </summary>
        string Segment1 { get; set; }
        /// <summary>
        /// 料號中分類。
        /// </summary>
        string Segment2 { get; set; }
        /// <summary>
        /// 料號小分類。
        /// </summary>
        string Segment3 { get; set; }
        /// <summary>
        /// 料號所屬製造組織 ID。
        /// </summary>
        long OrganizationId { get; set; }
        /// <summary>
        /// 料號所屬製造組織代碼。
        /// </summary>
        string OrganizationCode { get; set; }
        /// <summary>
        /// 料號啟用。
        /// </summary>
        bool EnabledFlag { get; set; }
        /// <summary>
        /// 料號最後更新日期。
        /// </summary>
        DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// XS型號。
        /// </summary>
        string XSDItem { get; set; }
    }
}
