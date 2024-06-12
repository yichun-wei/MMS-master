using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.DB;
using EzCoding.SystemEngines;
using Seec.Marketing.Erp;

namespace Seec.Marketing.Erp.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// SO-PRICE-ADJUSTMENTS-INTERFACE。
        /// </summary>
        public const string SO_PRICE_ADJUSTMENTS_INTERFACE = "OE.SO_PRICE_ADJUSTMENTS_INTERFACE";
    }

    /// <summary>
    /// SO-PRICE-ADJUSTMENTS-INTERFACE。
    /// </summary>
    public class SOPriceAdjustmentsInterface : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.SOPriceAdjustmentsInterface 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SOPriceAdjustmentsInterface(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.SO_PRICE_ADJUSTMENTS_INTERFACE))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : ISOPriceAdjustmentsInterface
        {
            /// <summary>
            /// SYSDATE。
            /// </summary>
            [SchemaMapping(Name = "CREATION_DATE", Type = ReturnType.DateTimeFormat)]
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            [SchemaMapping(Name = "CREATED_BY", Type = ReturnType.Int)]
            public int CreatedBY { get; set; }
            /// <summary>
            /// SYSDATE。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTimeFormat)]
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATED_BY", Type = ReturnType.Int)]
            public int LastUpdatedBY { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            [SchemaMapping(Name = "ORDER_SOURCE_ID", Type = ReturnType.Int)]
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_REFERENCE", Type = ReturnType.String)]
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 明細項次。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_LINE_REFERENCE", Type = ReturnType.String)]
            public string OriginalSystemLineReference { get; set; }
            /// <summary>
            /// 折扣 ID。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? DiscountId { get; set; }
            /// <summary>
            /// 折扣比率數值。
            /// </summary>
            [SchemaMapping(Name = "PERCENT", Type = ReturnType.Float, AllowNull = true)]
            public float? Percent { get; set; }

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

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="info">輸入資訊。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner Add(ISOPriceAdjustmentsInterface info)
        {
            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("CREATION_DATE", info.CreationDate, string.Empty, GenericDBType.DateTime);
                transSet.SmartAdd("CREATED_BY", info.CreatedBY, GenericDBType.Int);
                transSet.SmartAdd("LAST_UPDATE_DATE", info.LastUpdateDate, string.Empty, GenericDBType.DateTime);
                transSet.SmartAdd("LAST_UPDATED_BY", info.LastUpdatedBY, GenericDBType.Int);
                transSet.SmartAdd("ORDER_SOURCE_ID", info.OrderSourceId, GenericDBType.Int);
                transSet.SmartAdd("ORIGINAL_SYSTEM_REFERENCE", info.OriginalSystemReference, GenericDBType.VarChar, false);
                transSet.SmartAdd("ORIGINAL_SYSTEM_LINE_REFERENCE", info.OriginalSystemLineReference, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DISCOUNT_ID", info.DiscountId, GenericDBType.BigInt, true);
                transSet.SmartAdd("PERCENT", info.Percent, GenericDBType.Float, true);

                returner.ChangeInto(base.Add(transSet, true));

                //TODO: 測試用
                //returner.Feedback.Add(new FeedbackMessage("DEBUG", EzCoding.DB.OracleClient.OracleDataHandler.ToSqlStatement(base.SqlSyntaxCommander), string.Empty, Priority.High, TraceLevel.None));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region DeleteByOriginalSystemReference
        /// <summary>
        /// 依指定的「內銷訂單號碼」或「外銷交貨單號碼」刪除資料。
        /// </summary>
        /// <param name="originalSystemReference">「內銷訂單號碼」或「外銷交貨單號碼」。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByOriginalSystemReference(string originalSystemReference)
        {
            if (string.IsNullOrEmpty(originalSystemReference))
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("ORIGINAL_SYSTEM_REFERENCE", SqlCond.EqualTo, originalSystemReference, GenericDBType.VarChar, SqlCondsSet.And);

                returner.ChangeInto(base.Delete(condsMain, true));

                return returner;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion
        #endregion
    }
}
