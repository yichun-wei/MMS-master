using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seec.Marketing
{
    /// <summary>
    /// 自定的通用 EventArgs。
    /// </summary>
    public class CustEventArgs : EventArgs
    {
        /// <summary>
        /// 物件相關聯的使用者定義資料。
        /// </summary>
        public object Tag { get; set; }
    }
}
