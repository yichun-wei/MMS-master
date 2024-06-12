using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seec.Marketing
{
    /// <summary>
    /// 介接回應。
    /// </summary>
    public class TalkResponse
    {
        /// <summary>
        /// 代碼。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 回應的 Json 物件字串。
        /// </summary>
        public string JsonObj { get; set; }
    }
}
