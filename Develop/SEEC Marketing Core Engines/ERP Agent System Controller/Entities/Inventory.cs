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
        /// ERP 庫存。
        /// 非實際資料表名稱。
        /// </summary>
        public const string INVENTORY = "INVENTORY";
    }

    /// <summary>
    /// ERP 庫存。
    /// </summary>
    public class Inventory : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.Inventory 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public Inventory(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.INVENTORY))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IErpInv
        {
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long)]
            public long InventoryItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "ITEM", Type = ReturnType.String)]
            public string Item { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "DESCRIPTION", Type = ReturnType.String)]
            public string Description { get; set; }
            /// <summary>
            /// 料號重量。
            /// </summary>
            [SchemaMapping(Name = "UNIT_WEIGHT", Type = ReturnType.Float, AllowNull = true)]
            public float? UnitWeight { get; set; }
            /// <summary>
            /// 重量單位。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT_UOM_CODE", Type = ReturnType.String)]
            public string WeightUomCode { get; set; }
            /// <summary>
            /// 預設明細折數。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float, AllowNull = true)]
            public float? Discount { get; set; }
            /// <summary>
            /// 料號大分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT1", Type = ReturnType.String)]
            public string Segment1 { get; set; }
            /// <summary>
            /// 料號中分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT2", Type = ReturnType.String)]
            public string Segment2 { get; set; }
            /// <summary>
            /// 料號小分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT3", Type = ReturnType.String)]
            public string Segment3 { get; set; }
            /// <summary>
            /// 料號所屬製造組織 ID。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_ID", Type = ReturnType.Long)]
            public long OrganizationId { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
            /// <summary>
            /// 料號啟用。
            /// </summary>
            [SchemaMapping(Name = "ENABLED_FLAG", Type = ReturnType.Bool)]
            public bool EnabledFlag { get; set; }
            /// <summary>
            /// 料號最後更新日期。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTimeFormat)]
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// XS型號。
            /// </summary>
            [SchemaMapping(Name = "XSD_ITEM", Type = ReturnType.String)]
            public string XSDItem { get; set; }

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

        #region GetSyncInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSyncInfo(DateTime? afterTime)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT INVENTORY_ITEM_ID, ITEM, DESCRIPTION, UNIT_WEIGHT, WEIGHT_UOM_CODE, DISCOUNT, SEGMENT1, SEGMENT2, SEGMENT3, ORGANIZATION_ID, ORGANIZATION_CODE, ENABLED_FLAG, LAST_UPDATE_DATE, XSD_ITEM ");
            custEntity.Append("FROM APPS.XSEEC_ITEMS_V ");

            var sqlConds = new List<string>();

            if (afterTime.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "LAST_UPDATE_DATE", SqlOperator.GreaterEqualThan, afterTime.Value, GenericDBType.DateTime));
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
