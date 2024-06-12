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
        /// ERP 價目表。
        /// 非實際資料表名稱。
        /// </summary>
        public const string PRICE_BOOK = "PRICE_BOOK";
    }

    /// <summary>
    /// ERP 價目表。
    /// </summary>
    public class PriceBook : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.PriceBook 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public PriceBook(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.PRICE_BOOK))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IPriceBook
        {
            /// <summary>
            /// 價目表名稱。
            /// </summary>
            [SchemaMapping(Name = "LIST_NAME", Type = ReturnType.String)]
            public string ListName { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            [SchemaMapping(Name = "PRICE_LIST_ID", Type = ReturnType.Long)]
            public long PriceListId { get; set; }
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
            /// 單位。
            /// </summary>
            [SchemaMapping(Name = "PRIMARY_UNIT_OF_MEASURE", Type = ReturnType.String)]
            public string PrimaryUnitOFMeasure { get; set; }
            /// <summary>
            /// 摘要。
            /// </summary>
            [SchemaMapping(Name = "DESCRIPTION", Type = ReturnType.String)]
            public string Description { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            [SchemaMapping(Name = "LIST_PRICE", Type = ReturnType.Float)]
            public float ListPrice { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            [SchemaMapping(Name = "CURRENCY_CODE", Type = ReturnType.String)]
            public string CurrencyCode { get; set; }

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
        /// <param name="priceListId">價目表 ID（若為 null 則表示全部）。</param> 
        /// <param name="inventoryItemIds">料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(long? priceListId, long[] inventoryItemIds, string[] items)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //custEntity.Append("SELECT * ");
            //custEntity.Append("FROM (");
            //custEntity.Append(Properties.Resources.PriceBookSyncSql);
            //custEntity.Append(") T");

            custEntity.Append("SELECT LIST_NAME, PRICE_LIST_ID, INVENTORY_ITEM_ID, SEGMENT1, PRIMARY_UNIT_OF_MEASURE, DESCRIPTION, LIST_PRICE, CURRENCY_CODE ");
            custEntity.Append("FROM APPS.XSEEC_PRICE_DETAILS_V ");

            var sqlConds = new List<string>();

            if (priceListId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PRICE_LIST_ID", SqlOperator.EqualTo, priceListId.Value, GenericDBType.BigInt));
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
