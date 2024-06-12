using System;

namespace Seec.Marketing.SystemEngines
{
    /// <summary>
    /// 資料異動子資料表。
    /// </summary>
    public class DataTransChildTable
    {
        /// <summary>
        /// 資料表名稱。
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 以 JSON 格式表示的資料列集合。
        /// </summary>
        public object[] Rows { get; set; }
    }
}