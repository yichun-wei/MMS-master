using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seec.Marketing
{
    /// <summary>
    /// 前台主版頁面供子頁叫用的介面實作。
    /// </summary>
    public interface IClientMaster
    {
        /// <summary>
        /// 目前登入的系統使用者資訊集合。
        /// </summary>
        SysUserHelper.SysUserInfoSet ActorInfoSet { get; set; }
    }
}
