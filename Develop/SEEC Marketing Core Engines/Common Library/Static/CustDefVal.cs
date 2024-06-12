using System;

using EzCoding.Collections;

namespace Seec.Marketing
{
    /// <summary>
    /// 自訂的預設值常數定義。
    /// </summary>
    public class CustDefVal
    {
        /// <summary>
        /// Null 的全文檢索查詢。
        /// </summary>
        public static SimpleDataSet<string, object> FTSearch { get { return (SimpleDataSet<string, object>)null; } }
        /// <summary>
        /// Null 的附加資料用途。
        /// </summary>
        public static AttachUse? AttachUse { get { return (Nullable<AttachUse>)null; } }
    }
}
