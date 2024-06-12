using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using Seec.Marketing;
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

namespace Seec.Marketing
{
    /// <summary>
    /// Erp 介接 Helper 基底。
    /// </summary>
    public class ErpBaseHelper
    {
        #region ERP 倉庫
        #region ERP 倉庫表格資料
        /// <summary>
        /// ERP 倉庫表格資料。
        /// </summary>
        public class ErpWhseInfo : IErpWhse
        {
            /// <summary>
            /// 名稱。
            /// </summary>
            public string SecondaryInventoryName { get; set; }
            /// <summary>
            /// 倉庫 ID。
            /// </summary>
            public int OrganizationId { get; set; }
            /// <summary>
            /// 屬性 15。
            /// </summary>
            public string Attribute15 { get; set; }
        }
        #endregion

        #region 取得 ERP 倉庫資訊
        /// <summary>
        /// 取得 ERP 倉庫資訊。
        /// </summary>
        /// <param name="mktgRanges">市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static ErpWhseInfo[] GetErpWhseInfo(int[] mktgRanges)
        {
            var talkResp = ErpAgentRef.GetWarehouseInfo(mktgRanges);
            if ("0000".Equals(talkResp.Code))
            {
                return CustJson.DeserializeObject<ErpBaseHelper.ErpWhseInfo[]>(talkResp.JsonObj);
            }
            else
            {
                return new ErpBaseHelper.ErpWhseInfo[0];
            }
        }
        #endregion
        #endregion

        #region ERP 價目表
        #region ERP 價目表表格資料
        /// <summary>
        /// ERP 價目表表格資料。
        /// </summary>
        public class PriceBookInfo : IPriceBook
        {
            /// <summary>
            /// 價目表名稱。
            /// </summary>
            public string ListName { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            public long PriceListId { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            public long InventoryItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string Segment1 { get; set; }
            /// <summary>
            /// 單位。
            /// </summary>
            public string PrimaryUnitOFMeasure { get; set; }
            /// <summary>
            /// 摘要。
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            public float ListPrice { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            public string CurrencyCode { get; set; }
        }
        #endregion

        #region 取得 ERP 價目表資訊
        /// <summary>
        /// 取得 ERP 價目表資訊
        /// </summary>
        /// <param name="priceListId">價目表 ID（若為 null 則表示全部）。</param> 
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static PriceBookInfo[] GetPriceBookInfo(long priceListId, string[] items)
        {
            if (items == null || items.Length == 0)
            {
                return new ErpBaseHelper.PriceBookInfo[0];
            }

            var talkResp = ErpAgentRef.GetPriceBookInfo(priceListId, DefVal.Longs, items);
            if ("0000".Equals(talkResp.Code))
            {
                return CustJson.DeserializeObject<ErpBaseHelper.PriceBookInfo[]>(talkResp.JsonObj);
            }
            else
            {
                return new ErpBaseHelper.PriceBookInfo[0];
            }
        }
        #endregion

        #region 取得料號牌價
        /// <summary>
        /// 取得料號牌價。
        /// </summary>
        /// <param name="priceBookInfos">ERP 價目表表格資料。</param>
        /// <param name="item">料號。</param>
        /// <returns></returns>
        public static float? GetPriceBookListPrice(PriceBookInfo[] priceBookInfos, string item)
        {
            if (priceBookInfos == null || priceBookInfos.Length == 0 || string.IsNullOrEmpty(item))
            {
                return DefVal.Float;
            }

            var priceBookInfo = priceBookInfos.Where(q => q.Segment1 == item).DefaultIfEmpty(null).SingleOrDefault();
            if (priceBookInfo != null)
            {
                //TODO: 由於有幣別區分, 目前不知道有無影響, 先直接回傳.
                return priceBookInfo.ListPrice;
            }
            else
            {
                return DefVal.Float;
            }
        }
        #endregion
        #endregion

        #region ERP 幣別表
        #region ERP 幣別表表格資料
        /// <summary>
        /// ERP 幣別表表格資料。
        /// </summary>
        public class CurrencyBookInfo : ICurrencyBook
        {
            /// <summary>
            /// 價目表名稱。
            /// </summary>
            public string ListName { get; set; }
            /// <summary>
            /// 價目表 ID。
            /// </summary>
            public long PriceListId { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            public string CurrencyCode { get; set; }
        }
        #endregion

        #region 取得 ERP 幣別表資訊
        /// <summary>
        /// 取得 ERP 幣別表資訊
        /// </summary>
        /// <param name="priceListId">價目表 ID（若為 null 則表示全部）。</param> 
        /// <param name="currenctCode">幣別（若為 null 或 empty 則略過條件檢查）。</param>
        public static CurrencyBookInfo[] GetCurrencyBookInfo(long? priceListId, string currenctCode)
        {
            var talkResp = ErpAgentRef.GetCurrencyBookInfo(priceListId, currenctCode);
            if ("0000".Equals(talkResp.Code))
            {
                return CustJson.DeserializeObject<ErpBaseHelper.CurrencyBookInfo[]>(talkResp.JsonObj);
            }
            else
            {
                return new ErpBaseHelper.CurrencyBookInfo[0];
            }
        }
        #endregion
        #endregion

        #region ERP 倉庫在手量
        /// <summary>
        /// ERP 倉庫在手量。
        /// </summary>
        public class ErpWhseOnHand
        {
            /// <summary>
            /// 倉庫。
            /// </summary>
            public string Whse { get; set; }
            /// <summary>
            /// ERP 在手量。
            /// </summary>
            public int? ErpOnHand { get; set; }
        }
        #endregion

        #region ERP 倉庫在手量
        #region ERP 倉庫在手量表格資料
        /// <summary>
        /// ERP 倉庫在手量表格資料。
        /// </summary>
        public class OnHandInfo : IOnHand
        {
            /// <summary>
            /// 料號 ID。
            /// </summary>
            public long InventoryItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string Segment1 { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            public string SubinventoryCode { get; set; }
            /// <summary>
            /// 在手量。
            /// </summary>
            public int OnhandQty { get; set; }
        }
        #endregion

        #region 取得 ERP 倉庫在手量資訊
        /// <summary>
        /// 取得 ERP 倉庫在手量資訊
        /// </summary>
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="whse">倉庫。</param>
        public static OnHandInfo[] GetOnHandInfo(string[] items, string whse)
        {
            if (items == null || items.Length == 0)
            {
                return new ErpBaseHelper.OnHandInfo[0];
            }

            var talkResp = ErpAgentRef.GetOnHandInfo(DefVal.Longs, items, whse);
            if ("0000".Equals(talkResp.Code))
            {
                return CustJson.DeserializeObject<ErpBaseHelper.OnHandInfo[]>(talkResp.JsonObj);
            }
            else
            {
                return new ErpBaseHelper.OnHandInfo[0];
            }
        }
        #endregion

        #region 取得料號在手量
        /// <summary>
        /// 取得料號在手量。
        /// </summary>
        /// <param name="onHandInfos">ERP 倉庫在手量陣列集合。</param>
        /// <param name="whse">倉庫。</param>
        /// <param name="item">料號。</param>
        public static int? GetOnHandQty(OnHandInfo[] onHandInfos, string whse, string item)
        {
            if (onHandInfos == null || onHandInfos.Length == 0 || string.IsNullOrEmpty(whse) || string.IsNullOrEmpty(item))
            {
                return DefVal.Int;
            }

            var onHandQty = onHandInfos.Where(q => q.SubinventoryCode == whse && q.Segment1 == item).DefaultIfEmpty(null).SingleOrDefault();
            if (onHandQty != null)
            {
                return onHandQty.OnhandQty;
            }
            else
            {
                return DefVal.Int;
            }
        }

        /// <summary>
        /// 取得每個倉庫的料號在手量。
        /// </summary>
        /// <param name="onHandInfos">ERP 倉庫在手量陣列集合。</param>
        /// <param name="erpWhseInfos">ERP 倉庫陣列集合。</param>
        /// <param name="item">料號。</param>
        public static ErpBaseHelper.ErpWhseOnHand[] GetPerWhseOnHandQty(OnHandInfo[] onHandInfos, ErpWhseInfo[] erpWhseInfos, string item)
        {
            List<ErpWhseOnHand> erpWhseOnHands = new List<ErpWhseOnHand>();

            if (erpWhseInfos == null || erpWhseInfos.Length == 0)
            {
                return erpWhseOnHands.ToArray();
            }

            if (onHandInfos == null || onHandInfos.Length == 0 || string.IsNullOrEmpty(item))
            {
                foreach (var whseInfo in erpWhseInfos)
                {
                    erpWhseOnHands.Add(new ErpWhseOnHand()
                    {
                        Whse = whseInfo.SecondaryInventoryName,
                        ErpOnHand = 0
                    });
                }
                return erpWhseOnHands.ToArray();
            }

            var filterOnHandInfos = onHandInfos.Where(q => q.Segment1 == item).ToArray();
            foreach (var whseInfo in erpWhseInfos)
            {
                erpWhseOnHands.Add(new ErpWhseOnHand()
                {
                    Whse = whseInfo.SecondaryInventoryName,
                    ErpOnHand = filterOnHandInfos.Where(q => q.SubinventoryCode == whseInfo.SecondaryInventoryName).DefaultIfEmpty(new ErpBaseHelper.OnHandInfo()).SingleOrDefault().OnhandQty
                });
            }

            return erpWhseOnHands.ToArray();
        }
        #endregion
        #endregion

        #region 上傳 ERP
        #region SOHeadersInterfaceAll
        /// <summary>
        /// SO-HEADERS-INTERFACE。
        /// </summary>
        public class SOHeadersInterfaceAll : ISOHeadersInterfaceAll
        {
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int CreatedBY { get; set; }
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int LastUpdatedBY { get; set; }
            /// <summary>
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 客戶檔之 ID。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 客戶檔之 ORDER_TYPE_ID。
            /// </summary>
            public long? OrderTypeId { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 常數：R。
            /// </summary>
            public string OrderCategory { get; set; }
            /// <summary>
            /// 訂單日期。
            /// </summary>
            public DateTime DateOrdered { get; set; }
            /// <summary>
            /// 幣別。
            /// </summary>
            public string CurrencyCode { get; set; }
            /// <summary>
            /// 常數數值：1020。
            /// 外銷用。
            /// </summary>
            public string ConversionTypeCode { get; set; }
            /// <summary>
            /// 客戶檔之營業員 ID。
            /// </summary>
            public long? SalesRepId { get; set; }
            /// <summary>
            /// 客戶檔之帳單地點 ID。
            /// </summary>
            public long? InvoiceToSiteUseId { get; set; }
            /// <summary>
            /// 客戶檔之送貨地點 ID。
            /// </summary>
            public long? ShipToSiteUseId { get; set; }
            /// <summary>
            /// 客戶檔之價目表 ID。
            /// </summary>
            public long? PriceListId { get; set; }
            /// <summary>
            /// 常數數值：1。
            /// </summary>
            public int? EnteredStateId { get; set; }
        }
        #endregion

        #region SOLinesInterfaceAll
        /// <summary>
        /// SO-LINES-INTERFACE-ALL。
        /// </summary>
        public class SOLinesInterfaceAll : ISOLinesInterfaceAll
        {
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int CreatedBY { get; set; }
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int LastUpdatedBY { get; set; }
            /// <summary>
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 明細項次，ORIGINAL_SYSTEM_REFERENCE + ORIGINAL_SYSTEM_LINE_REFERENCE，兩個欄位串聯須為 UNIQUE。
            /// </summary>
            public string OriginalSystemLineReference { get; set; }
            /// <summary>
            /// 明細項次。
            /// </summary>
            public int LineNumber { get; set; }
            /// <summary>
            /// 常數：REGULAR。
            /// </summary>
            public string LineType { get; set; }
            /// <summary>
            /// 常數：EA。
            /// </summary>
            public string UnitCode { get; set; }
            /// <summary>
            /// 訂單數量。
            /// </summary>
            public int OrderedQuantity { get; set; }
            /// <summary>
            /// 訂單日期。
            /// </summary>
            public DateTime? DateRequestedCurrent { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            public long? InventoryItemId { get; set; }
            /// <summary>
            /// 預計出貨日(內外銷都相同規則)。
            /// </summary>
            public DateTime? ScheduledShipmentDate { get; set; }
            /// <summary>
            /// 常數數值：228。
            /// </summary>
            public int? WarehouseId { get; set; }
            /// <summary>
            /// 內銷-專案編號；外銷-訂單單號。
            /// </summary>
            public string Attribute2 { get; set; }
            /// <summary>
            /// 常數：Y。
            /// </summary>
            public string CalculatePrice { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 客戶 ID。
            /// </summary>
            public long? ShipToCustomerId { get; set; }
            //[20160421 by 米雪]
            /// <summary>
            /// 常數數值：DEMANDED。
            /// </summary>
            public string ScheduleStatusCode { get; set; }
        }
        #endregion

        #region SOLineDetailsInterface
        /// <summary>
        /// SO-LINE-DETAILS-INTERFACE。
        /// </summary>
        public class SOLineDetailsInterface : ISOLineDetailsInterface
        {
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int CreatedBY { get; set; }
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int LastUpdatedBY { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 明細項次，ORIGINAL_SYSTEM_REFERENCE + ORIGINAL_SYSTEM_LINE_REFERENCE 兩個欄位串聯須為 UNIQUE。
            /// </summary>
            public string OriginalSystemLineReference { get; set; }
            /// <summary>
            /// 訂單數量。
            /// </summary>
            public int Quantity { get; set; }
            /// <summary>
            /// 預計出貨日(內外銷都相同規則)。
            /// </summary>
            public DateTime? ScheduleDate { get; set; }
            /// <summary>
            /// 倉庫名稱。
            /// </summary>
            public string Subinventory { get; set; }
            /// <summary>
            /// 常數數值：228。
            /// </summary>
            public int? WarehouseId { get; set; }
            //[20160421 by 米雪]
            /// <summary>
            /// 常數數值：DEMANDED。
            /// </summary>
            public string ScheduleStatusCode { get; set; }
        }
        #endregion

        #region SOPriceAdjustmentsInterface
        /// <summary>
        /// SO-PRICE-ADJUSTMENTS-INTERFACE。
        /// </summary>
        public class SOPriceAdjustmentsInterface : ISOPriceAdjustmentsInterface
        {
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int CreatedBY { get; set; }
            /// <summary>
            /// SYSDATE。
            /// </summary>
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// 常數數值：6214。
            /// </summary>
            public int LastUpdatedBY { get; set; }
            /// <summary>
            /// 常數數值：1103。
            /// </summary>
            public int OrderSourceId { get; set; }
            /// <summary>
            /// 內銷-訂單號碼；外銷-交貨單號碼。
            /// </summary>
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// 明細項次。
            /// </summary>
            public string OriginalSystemLineReference { get; set; }
            /// <summary>
            /// 折扣 ID。
            /// </summary>
            public long? DiscountId { get; set; }
            /// <summary>
            /// 折扣比率數值。
            /// </summary>
            public float? Percent { get; set; }
        }
        #endregion

        #region ErpUploader
        /// <summary>
        /// 上傳 ERP。
        /// </summary>
        public class ErpUploader
        {
            /// <summary>
            /// SO-HEADERS-INTERFACE-ALL。
            /// </summary>
            public SOHeadersInterfaceAll SOHeadersInterfaceAll { get; set; }
            /// <summary>
            /// SO-LINES-INTERFACE-ALL。
            /// </summary>
            public List<SOLinesInterfaceAll> SOLinesInterfaceAllList { get; set; }
            /// <summary>
            /// SO-LINE-DETAILS-INTERFACE。
            /// </summary>
            public List<SOLineDetailsInterface> SOLineDetailsInterfaceList { get; set; }
            /// <summary>
            /// SO-PRICE-ADJUSTMENTS-INTERFACE。
            /// </summary>
            public List<SOPriceAdjustmentsInterface> SOPriceAdjustmentsInterfaceList { get; set; }
        }
        #endregion
        #endregion

        #region ERP 訂單
        #region ERP 訂單表格資料
        /// <summary>
        /// ERP 訂單表格資料。
        /// </summary>
        public class ErpOrderInfo : IErpOrder
        {
            /// <summary>
            /// ERP 訂單 ID。
            /// </summary>
            public long HeaderId { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public DateTime CreationDate { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public int CreatedBY { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public string OriginalSystemSourceCode { get; set; }
            /// <summary>
            /// XS 營銷訂單號碼。
            /// </summary>
            public string OriginalSystemReference { get; set; }
            /// <summary>
            /// ERP 訂單號碼。
            /// </summary>
            public int? OrderNumber { get; set; }
            /// <summary>
            /// ERP 訂單狀態（已輸入,已登錄,已超額,超額已核發,已取消,已關閉）。
            /// </summary>
            public string OrderStatus { get; set; }
            /// <summary>
            /// ERP 交貨號碼。
            /// </summary>
            public string ShipNumber { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public string Attribute1 { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public string Attribute2 { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public string Attribute3 { get; set; }
            /// <summary>
            /// 訂單狀態最後更新時間。
            /// </summary>
            public DateTime? LastUpdateDate { get; set; }
            /// <summary>
            /// 。
            /// </summary>
            public string OpenFlag { get; set; }
        }
        #endregion

        #region 取得 ERP 訂單資訊
        /// <summary>
        /// 取得 ERP 訂單資訊
        /// </summary>
        /// <param name="headerIds">ERP 訂單 ID（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="originalSystemReferences">XS 營銷訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="orderNumbers">ERP 訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static ErpOrderInfo[] GetErpOrderInfo(long[] headerIds, string[] originalSystemReferences, int[] orderNumbers)
        {
            var talkResp = ErpAgentRef.GetErpOrderInfo(headerIds, originalSystemReferences, orderNumbers);
            if ("0000".Equals(talkResp.Code))
            {
                return CustJson.DeserializeObject<ErpBaseHelper.ErpOrderInfo[]>(talkResp.JsonObj);
            }
            else
            {
                return new ErpBaseHelper.ErpOrderInfo[0];
            }
        }
        #endregion
        #endregion
    }
}
