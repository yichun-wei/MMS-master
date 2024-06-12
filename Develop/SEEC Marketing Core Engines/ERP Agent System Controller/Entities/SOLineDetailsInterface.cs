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
        /// SO-LINE-DETAILS-INTERFACE。
        /// </summary>
        public const string SO_LINE_DETAILS_INTERFACE = "OE.SO_LINE_DETAILS_INTERFACE";
    }

    /// <summary>
    /// SO-LINE-DETAILS-INTERFACE。
    /// </summary>
    public class SOLineDetailsInterface : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.SOLineDetailsInterface 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SOLineDetailsInterface(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.SO_LINE_DETAILS_INTERFACE))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : ISOLineDetailsInterface
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
            /// 明細項次，ORIGINAL_SYSTEM_REFERENCE + ORIGINAL_SYSTEM_LINE_REFERENCE 兩個欄位串聯須為 UNIQUE。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_LINE_REFERENCE", Type = ReturnType.String)]
            public string OriginalSystemLineReference { get; set; }
            /// <summary>
            /// 訂單數量。
            /// </summary>
            [SchemaMapping(Name = "QUANTITY", Type = ReturnType.Int)]
            public int Quantity { get; set; }
            /// <summary>
            /// 預計出貨日(內外銷都相同規則)。
            /// </summary>
            [SchemaMapping(Name = "SCHEDULE_DATE", Type = ReturnType.DateTimeFormat, AllowNull = true)]
            public DateTime? ScheduleDate { get; set; }
            /// <summary>
            /// 倉庫名稱。
            /// </summary>
            [SchemaMapping(Name = "SUBINVENTORY", Type = ReturnType.String)]
            public string Subinventory { get; set; }
            /// <summary>
            /// 常數數值：228。
            /// </summary>
            [SchemaMapping(Name = "WAREHOUSE_ID", Type = ReturnType.Int, AllowNull = true)]
            public int? WarehouseId { get; set; }
            //[20160421 by 米雪]
            /// <summary>
            /// 常數數值：DEMANDED。
            /// </summary>
            [SchemaMapping(Name = "SCHEDULE_STATUS_CODE", Type = ReturnType.String)]
            public string ScheduleStatusCode { get; set; }


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
        public Returner Add(ISOLineDetailsInterface info)
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
                transSet.SmartAdd("ORIGINAL_SYSTEM_LINE_REFERENCE", info.OriginalSystemLineReference, GenericDBType.VarChar, false);
                transSet.SmartAdd("QUANTITY", info.Quantity, GenericDBType.Int);
                transSet.SmartAdd("SCHEDULE_DATE", info.ScheduleDate, string.Empty, GenericDBType.DateTime, true);
                transSet.SmartAdd("SUBINVENTORY", info.Subinventory, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("WAREHOUSE_ID", info.WarehouseId, GenericDBType.Int, true);
                //[20160421 by 米雪]
                transSet.SmartAdd("SCHEDULE_STATUS_CODE", info.ScheduleStatusCode, GenericDBType.VarChar, true);

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
