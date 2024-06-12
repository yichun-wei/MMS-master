using System;
using System.Collections.Generic;

namespace Seec.Marketing.SystemEngines
{
    /// <summary>
    /// 表格資料基底類別。
    /// </summary>
    public class InfoBase
    {
        /// <summary>
        /// 初始化 Cdri.Bkep.SystemEngines.InfoBase 類別的新執行個體。
        /// </summary>
        public InfoBase()
        {
            this.ChildTables = new List<DataTransChildTable>();
        }

        /// <summary>
        /// 子資料表。
        /// 用於資料異動時的相關子資料表內容。
        /// </summary>
        public List<DataTransChildTable> ChildTables { get; set; }

        /// <summary>
        /// 物件相關聯的使用者定義資料。
        /// 操作使用，非資料庫資料。
        /// </summary>
        public object Tag { get; set; }
    }
}
