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
        /// ERP 幣別表。
        /// 非實際資料表名稱。
        /// </summary>
        public const string CURRENCY_LIST = "CURRENCY_LIST";
    }

    /// <summary>
    /// ERP 幣別表。
    /// </summary>
    public class CurrencyBook : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.CurrencyBook 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public CurrencyBook(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.CURRENCY_LIST))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : ICurrencyBook
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
        /// <param name="currenctCode">幣別（若為 null 或 empty 則略過條件檢查）。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(long? priceListId, string currenctCode)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT LIST_NAME, PRICE_LIST_ID, CURRENCY_CODE ");
            custEntity.Append("FROM APPS.XSEEC_PRICE_HEAD_V ");

            var sqlConds = new List<string>();

            if (priceListId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PRICE_LIST_ID", SqlOperator.EqualTo, priceListId.Value, GenericDBType.BigInt));
            }

            if (!string.IsNullOrEmpty(currenctCode))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CURRENCY_CODE", SqlOperator.EqualTo, currenctCode, GenericDBType.VarChar));
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
