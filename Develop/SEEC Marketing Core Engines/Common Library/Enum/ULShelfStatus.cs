
namespace Seec.Marketing
{
    /// <summary>
    /// 上下架狀態。
    /// </summary>
    public enum ULShelfStatus
    {
        /// <summary>
        /// 全部。
        /// </summary>
        All = 0,
        /// <summary>
        /// 已上架未下架。
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 已上架，但不論是否已下架。
        /// </summary>
        HasUpper = 2,
        /// <summary>
        /// 僅已下架。
        /// </summary>
        OnlyLower = 3
    }
}
