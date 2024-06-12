
namespace Seec.Marketing
{
    /// <summary>
    /// 事件類型。
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 除錯。
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 通知。
        /// </summary>
        Notice = 2,
        /// <summary>
        /// 警告。
        /// </summary>
        Warning = 3,
        /// <summary>
        /// 可預期攔截的錯誤。
        /// </summary>
        Error = 4,
        /// <summary>
        /// 不在預期攔截範圍內的錯誤。
        /// </summary>
        Fatal = 5
    }
}
