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
        /// ERP 折扣。
        /// 非實際資料表名稱。
        /// </summary>
        public const string DISCOUNTS = "DISCOUNTS";
    }

    /// <summary>
    /// ERP 折扣。
    /// </summary>
    public class Discounts : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.Discounts 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public Discounts(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.DISCOUNTS))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IErpDct
        {
            /// <summary>
            /// 折扣 ID。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_ID", Type = ReturnType.Long)]
            public long DiscountId { get; set; }
            /// <summary>
            /// 折扣名稱。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_NAME", Type = ReturnType.String)]
            public string DiscountName { get; set; }
            /// <summary>
            /// 自動計算折扣。
            /// </summary>
            [SchemaMapping(Name = "AUTOMATIC_DISCOUNT_FLAG", Type = ReturnType.Bool, AllowNull = true)]
            public bool? AutomaticDiscountFlag { get; set; }
            /// <summary>
            /// 允許修訂。
            /// </summary>
            [SchemaMapping(Name = "OVERRIDE_ALLOWED_FLAG", Type = ReturnType.Bool, AllowNull = true)]
            public bool? OverrideAllowedFlag { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            [SchemaMapping(Name = "PRICE_LIST_ID", Type = ReturnType.Long)]
            public long PriceListId { get; set; }
            /// <summary>
            /// 價目表名稱。
            /// </summary>
            [SchemaMapping(Name = "LIST_NAME", Type = ReturnType.String)]
            public string ListName { get; set; }
            /// <summary>
            /// 折扣型態（1:內銷表頭 2:內銷明細 3:外銷表頭 4:外銷明細）。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_TYPE", Type = ReturnType.Int)]
            public int DiscountType { get; set; }
            /// <summary>
            /// 折扣最後更新日期。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTimeFormat)]
            public DateTime LastUpdateDate { get; set; }

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

            custEntity.Append("SELECT * ");
            custEntity.Append("FROM (");
            custEntity.Append(Properties.Resources.DiscountsSyncSql);
            custEntity.Append(") T");

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
