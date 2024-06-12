using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.DB;
using EzCoding.SystemEngines;
using Seec.Marketing.Erp;

namespace Seec.Marketing.Erp.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// ERP 倉庫在手量。
        /// 非實際資料表名稱。
        /// </summary>
        public const string ONHAND = "ONHAND";
    }

    /// <summary>
    /// ERP 倉庫在手量。
    /// </summary>
    public class OnHand : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.OnHand 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public OnHand(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.ONHAND))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IOnHand
        {
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long)]
            public long InventoryItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT1", Type = ReturnType.String)]
            public string Segment1 { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            [SchemaMapping(Name = "SUBINVENTORY_CODE", Type = ReturnType.String)]
            public string SubinventoryCode { get; set; }
            /// <summary>
            /// 在手量。
            /// </summary>
            [SchemaMapping(Name = "ONHAND_QTY", Type = ReturnType.Int)]
            public int OnhandQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<Info>(new Info(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info[] Binding(DataTable dTable)
            {
                List<Info> infos = new List<Info>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(Info.Binding(row));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region GetInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="inventoryItemIds">料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="whse">倉庫。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(long[] inventoryItemIds, string[] items, string whse)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT X.* ");
            custEntity.Append("FROM APPS.XSEEC_ONHAND_V1 X ");

            var sqlConds = new List<string>();

            if (!string.IsNullOrEmpty(whse))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SUBINVENTORY_CODE", SqlOperator.EqualTo, whse, GenericDBType.VarChar));
            }

            if (inventoryItemIds != null && inventoryItemIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "INVENTORY_ITEM_ID", SqlOperator.EqualTo, inventoryItemIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (items != null && items.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SEGMENT1", SqlOperator.EqualTo, items, GenericDBType.VarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrEmpty(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, base.DefaultSqlOrder, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
    }
}
