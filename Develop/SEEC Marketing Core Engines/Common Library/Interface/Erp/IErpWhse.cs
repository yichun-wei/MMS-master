using System;

namespace Seec.Marketing.Erp
{
    /// <summary>
    /// ERP 倉庫介面。
    /// </summary>
    public interface IErpWhse
    {
        /// <summary>
        /// 名稱。
        /// </summary>
        string SecondaryInventoryName { get; set; }
        /// <summary>
        /// 倉庫 ID。
        /// </summary>
        int OrganizationId { get; set; }
    }
}
