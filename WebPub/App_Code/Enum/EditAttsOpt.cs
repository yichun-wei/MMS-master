using System;

namespace Seec.Marketing
{
    /// <summary>
    /// 編輯時欲使用的附加資料檔。
    /// </summary>
    [Flags]
    public enum EditAttsOpt
    {
        /// <summary>
        /// 超連結。
        /// </summary>
        Hyperlink = 0x01,
        /// <summary>
        /// 文繞圖內容。
        /// </summary>
        TextWrap = 0x02,
        /// <summary>
        /// 檔案下載。
        /// </summary>
        Download = 0x04,
        /// <summary>
        /// 圖片。
        /// </summary>
        Image = 0x08,
        /// <summary>
        /// 影片。
        /// </summary>
        Video = 0x10
    }
}
