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
        /// SO-HEADERS-INTERFACE-ALL。
        /// </summary>
        public const string SO_HEADERS_INTERFACE_ALL = "OE.SO_HEADERS_INTERFACE_ALL";
    }

    /// <summary>
    /// SO-HEADERS-INTERFACE-ALL。
    /// </summary>
    public class SOHeadersInterfaceAll : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.Erp.SystemEngines.SOHeadersInterfaceAll 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SOHeadersInterfaceAll(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.SO_HEADERS_INTERFACE_ALL))
        {

        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : ISOHeadersInterfaceAll
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
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            [SchemaMapping(Name = "ORIGINAL_SYSTEM_REFERENCE", Type = ReturnType.String)]
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 客戶檔之 ID。
            /// </summary>
            [SchemaMapping(Name = "CUSTOMER_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? CustomerId { get; set; }
            /// <summary>
            /// 客戶檔之 ORDER_TYPE_ID。
            /// </summary>
            [SchemaMapping(Name = "ORDER_TYPE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? OrderTypeId { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            [SchemaMapping(Name = "ORDER_SOURCE_ID", Type = ReturnType.Int)]
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 常數：R。
            /// </summary>
            [SchemaMapping(Name = "ORDER_CATEGORY", Type = ReturnType.String)]
            public string OrderCategory { get; set; }
            /// <summary>
            /// 訂單日期。
            /// </summary>
            [SchemaMapping(Name = "DATE_ORDERED", Type = ReturnType.DateTimeFormat)]
            public DateTime DateOrdered { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            [SchemaMapping(Name = "CURRENCY_CODE", Type = ReturnType.String)]
            public string CurrencyCode { get; set; }
            /// <summary>
            /// 常數數值：1020。
            /// 外銷用。
            /// </summary>
            [SchemaMapping(Name = "CONVERSION_TYPE_CODE", Type = ReturnType.String)]
            public string ConversionTypeCode { get; set; }
            /// <summary>
            /// 客戶檔之營業員 ID。
            /// </summary>
            [SchemaMapping(Name = "SALESREP_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? SalesRepId { get; set; }
            /// <summary>
            /// 客戶檔之帳單地點 ID。
            /// </summary>
            [SchemaMapping(Name = "INVOICE_TO_SITE_USE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InvoiceToSiteUseId { get; set; }
            /// <summary>
            /// 客戶檔之送貨地點 ID。
            /// </summary>
            [SchemaMapping(Name = "SHIP_TO_SITE_USE_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ShipToSiteUseId { get; set; }
            /// <summary>
            /// 客戶檔之價目表 ID。
            /// </summary>
            [SchemaMapping(Name = "PRICE_LIST_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? PriceListId { get; set; }
            /// <summary>
            /// 常數數值：1。
            /// </summary>
            [SchemaMapping(Name = "ENTERED_STATE_ID", Type = ReturnType.Int, AllowNull = true)]
            public int? EnteredStateId { get; set; }

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
        public Returner Add(ISOHeadersInterfaceAll info)
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
                transSet.SmartAdd("ORIGINAL_SYSTEM_REFERENCE", info.OriginalSystemReference, GenericDBType.VarChar, false);
                transSet.SmartAdd("CUSTOMER_ID", info.CustomerId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_TYPE_ID", info.OrderTypeId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ORDER_SOURCE_ID", info.OrderSourceId, GenericDBType.Int);
                transSet.SmartAdd("ORDER_CATEGORY", info.OrderCategory, GenericDBType.VarChar, false);
                transSet.SmartAdd("DATE_ORDERED", info.DateOrdered, string.Empty, GenericDBType.DateTime);
                transSet.SmartAdd("CURRENCY_CODE", info.CurrencyCode, GenericDBType.VarChar, false);
                transSet.SmartAdd("CONVERSION_TYPE_CODE", info.ConversionTypeCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("SALESREP_ID", info.SalesRepId, GenericDBType.BigInt, true);
                transSet.SmartAdd("INVOICE_TO_SITE_USE_ID", info.InvoiceToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("SHIP_TO_SITE_USE_ID", info.ShipToSiteUseId, GenericDBType.BigInt, true);
                transSet.SmartAdd("PRICE_LIST_ID", info.PriceListId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ENTERED_STATE_ID", info.EnteredStateId, GenericDBType.Int, true);

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
