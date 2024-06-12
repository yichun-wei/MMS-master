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
        /// ERP 訂單。
        /// 非實際資料表名稱。
        /// </summary>
        public const string ERP_ORDER = "ERP_ORDER";
    }

    /// <summary>
    /// ERP 訂單。
    /// </summary>
    public class ErpOrder : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.ErpOrder 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ErpOrder(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.ERP_ORDER))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : IErpOrder
        {
            /// <summary>
            /// ERP 訂單 ID。
            /// </summary>
            [SchemaMapping(Name = "HEADER_ID", Type = ReturnType.Long)]
            public long HeaderId { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "CREATION_DATE", Type = ReturnType.DateTimeFormat)]
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "CREATED_BY", Type = ReturnType.Int)]
            public int CreatedBY { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_SOURCE_CODE", Type = ReturnType.String)]
            public string OriginalSystemSourceCode { get; set; }
            /// <summary>
            /// XS 營銷訂單號碼。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_REFERENCE", Type = ReturnType.String)]
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// ERP 訂單號碼。
            /// </summary>
            [SchemaMapping(Name = "ORDER_NUMBER", Type = ReturnType.Int, AllowNull = true)]
            public int? OrderNumber { get; set; }
            /// <summary>
            /// ERP 訂單狀態（已輸入,已登錄,已超額,超額已核發,已取消,已關閉）。
            /// </summary>
            [SchemaMapping(Name = "ORDER_STATUS", Type = ReturnType.String)]
            public string OrderStatus { get; set; }
            /// <summary>
            /// ERP 交貨號碼。
            /// </summary>
            [SchemaMapping(Name = "SHIP_NUMBER", Type = ReturnType.String)]
            public string ShipNumber { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "ATTRIBUTE1", Type = ReturnType.String)]
            public string Attribute1 { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "ATTRIBUTE2", Type = ReturnType.String)]
            public string Attribute2 { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "ATTRIBUTE3", Type = ReturnType.String)]
            public string Attribute3 { get; set; }
            /// <summary>
            /// 訂單狀態最後更新時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTimeFormat, AllowNull = true)]
            public DateTime? LastUpdateDate { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            [SchemaMapping(Name = "OPEN_FLAG", Type = ReturnType.String)]
            public string OpenFlag { get; set; }

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
        /// <param name="headerIds">ERP 訂單 ID（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="originalSystemReferences">XS 營銷訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="orderNumbers">ERP 訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(long[] headerIds, string[] originalSystemReferences, int[] orderNumbers)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT X.* ");
            custEntity.Append("FROM OE.XSEEC_ORDER_REF X ");

            var sqlConds = new List<string>();

            if (headerIds != null && headerIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "HEADER_ID", SqlOperator.EqualTo, headerIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (originalSystemReferences != null && originalSystemReferences.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ORIGINAL_SYSTEM_REFERENCE", SqlOperator.EqualTo, originalSystemReferences, GenericDBType.VarChar));
            }

            if (orderNumbers != null && orderNumbers.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ORDER_NUMBER", SqlOperator.EqualTo, orderNumbers.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrEmpty(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, new SqlOrder("CREATION_DATE", Sort.Descending), condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
    }
}
