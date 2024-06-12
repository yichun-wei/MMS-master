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
        /// ERP 倉庫。
        /// 非實際資料表名稱。
        /// </summary>
        public const string WAREHOUSE = "WAREHOUSE";
    }

    /// <summary>
    /// ERP 倉庫。
    /// </summary>
    public class Warehouse : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.Warehouse 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public Warehouse(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.WAREHOUSE))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IErpWhse
        {
            /// <summary>
            /// 名稱。
            /// </summary>
            [SchemaMapping(Name = "SECONDARY_INVENTORY_NAME", Type = ReturnType.String)]
            public string SecondaryInventoryName { get; set; }
            /// <summary>
            /// 倉庫 ID。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_ID", Type = ReturnType.Int)]
            public int OrganizationId { get; set; }
            /// <summary>
            /// 屬性 15。
            /// </summary>
            [SchemaMapping(Name = "ATTRIBUTE15", Type = ReturnType.String)]
            public string Attribute15 { get; set; }

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
        /// <param name="mktgRanges">市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetSyncInfo(int[] mktgRanges)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT X.SECONDARY_INVENTORY_NAME, X.ORGANIZATION_ID, X.ATTRIBUTE15 ");
            custEntity.Append("FROM INV.MTL_SECONDARY_INVENTORIES X ");
            //custEntity.Append("WHERE X.ORGANIZATION_ID = 228 AND X.ATTRIBUTE15 IN ('內銷', '外銷')");

            var sqlConds = new List<string>();

            sqlConds.Add(custEntity.BuildConds(string.Empty, "ORGANIZATION_ID", SqlOperator.EqualTo, 228, GenericDBType.Int));

            if (mktgRanges != null && mktgRanges.Length > 0)
            {
                var items = new List<string>();
                foreach (var mktgRange in mktgRanges)
                {
                    switch (mktgRange)
                    {
                        case 1:
                            items.Add("內銷");
                            break;
                        case 2:
                            items.Add("外銷");
                            break;
                        default:
                            //故意使其找不到
                            items.Add("N/A");
                            break;
                    }
                }

                sqlConds.Add(custEntity.BuildConds(string.Empty, "ATTRIBUTE15", SqlOperator.EqualTo, items.ToArray(), GenericDBType.VarChar));
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
