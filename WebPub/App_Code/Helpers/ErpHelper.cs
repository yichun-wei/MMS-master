using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using Seec.Marketing;
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

namespace Seec.Marketing
{
    /// <summary>
    /// Erp 介接 Helper。
    /// </summary>
    public class ErpHelper : ErpBaseHelper
    {
        #region 上傳 ERP
        #region ErpUploader
        /// <summary>
        /// 上傳 ERP。
        /// </summary>
        new public class ErpUploader
        {
            #region 內銷上傳 ERP
            /// <summary>
            /// 內銷上傳 ERP。
            /// </summary>
            /// <param name="infoSet">內銷訂單資訊集合。</param>
            /// <returns>錯誤代碼。若「0000」則表示已成功上傳。</returns>
            public string Upload(DomOrderHelper.InfoSet infoSet)
            {
                var now = DateTime.Now;
                var custErpActor = 6214;

                ErpBaseHelper.ErpUploader uploader = new ErpBaseHelper.ErpUploader();

                using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    #region SO-HEADERS-INTERFACE-ALL
                    uploader.SOHeadersInterfaceAll = new ErpHelper.SOHeadersInterfaceAll()
                    {
                        CreationDate = now,
                        CreatedBY = custErpActor,
                        LastUpdateDate = now,
                        LastUpdatedBY = custErpActor,
                        OriginalSystemReference = infoSet.Info.OdrNo,
                        CustomerId = infoSet.Info.CustomerId,
                        OrderTypeId = infoSet.Info.OrderTypeId,
                        OrderSourceId = 1103,
                        OrderCategory = "R",
                        DateOrdered = infoSet.Info.Cdt,
                        CurrencyCode = infoSet.Info.CurrencyCode,
                        SalesRepId = infoSet.Info.SalesRepId,
                        PriceListId = infoSet.Info.PriceListId,
                        EnteredStateId = 1,
                        ShipToSiteUseId = infoSet.Info.ShipToSiteUseId,
                        InvoiceToSiteUseId = infoSet.Info.InvoiceToSiteUseId
                    };
                    #endregion

                    uploader.SOLinesInterfaceAllList = new List<SOLinesInterfaceAll>();
                    uploader.SOLineDetailsInterfaceList = new List<SOLineDetailsInterface>();
                    uploader.SOPriceAdjustmentsInterfaceList = new List<SOPriceAdjustmentsInterface>();

                    foreach (var detInfo in infoSet.DetInfos)
                    {
                        #region SO-LINES-INTERFACE-ALL
                        uploader.SOLinesInterfaceAllList.Add(new ErpHelper.SOLinesInterfaceAll()
                        {
                            CreationDate = now,
                            CreatedBY = custErpActor,
                            LastUpdateDate = now,
                            LastUpdatedBY = custErpActor,
                            OriginalSystemReference = infoSet.Info.OdrNo,
                            OriginalSystemLineReference = detInfo.Sort.ToString(),
                            LineNumber = detInfo.Sort,
                            LineType = "REGULAR",
                            UnitCode = "EA",
                            OrderedQuantity = detInfo.Qty,
                            DateRequestedCurrent = infoSet.Info.Cdt,
                            CalculatePrice = "Y",
                            Attribute2 = detInfo.QuoteNumber,
                            InventoryItemId = detInfo.InventoryItemId,
                            ShipToCustomerId = infoSet.Info.CustomerId,
                            OrderSourceId = 1103,
                            ScheduledShipmentDate = infoSet.Info.Edd,
                            WarehouseId = 228,
                            //[20160421 by 米雪]
                            ScheduleStatusCode = "DEMANDED"
                        });
                        #endregion

                        #region SO-LINE-DETAILS-INTERFACE
                        uploader.SOLineDetailsInterfaceList.Add(new ErpHelper.SOLineDetailsInterface()
                        {
                            CreationDate = now,
                            CreatedBY = custErpActor,
                            LastUpdateDate = now,
                            LastUpdatedBY = custErpActor,
                            OrderSourceId = 1103,
                            OriginalSystemReference = infoSet.Info.OdrNo,
                            OriginalSystemLineReference = detInfo.Sort.ToString(),
                            Quantity = detInfo.Qty,
                            ScheduleDate = infoSet.Info.Edd,
                            Subinventory = infoSet.Info.Whse,
                            WarehouseId = 228,
                               //[20160421 by 米雪]
                            ScheduleStatusCode = "DEMANDED"
                        });
                        #endregion

                        #region SO-PRICE-ADJUSTMENTS-INTERFACE
                        #region 表頭折扣
                        if (infoSet.HeaderDiscountInfos != null && infoSet.HeaderDiscountInfos.Count > 0)
                        {
                            foreach (var discountInfo in infoSet.HeaderDiscountInfos)
                            {
                                uploader.SOPriceAdjustmentsInterfaceList.Add(new ErpHelper.SOPriceAdjustmentsInterface()
                                {
                                    CreationDate = now,
                                    CreatedBY = custErpActor,
                                    LastUpdateDate = now,
                                    LastUpdatedBY = custErpActor,
                                    OrderSourceId = 1103,
                                    OriginalSystemReference = infoSet.Info.OdrNo,
                                    OriginalSystemLineReference = detInfo.Sort.ToString(),
                                    //(明細單價 X 現金折扣) / 牌價 X 100，EX:((10.9920 X 0.01)/18.3200)X100=0.6
                                    //(明細單價 X 獎金分攤折扣) / 牌價 X 100，EX:((10.9920 X 0.02)/18.3200)X100=1.2
                                    Percent = (float)MathLib.Round(detInfo.UnitPrice * discountInfo.Discount / detInfo.ListPrice * 100, 4),
                                    DiscountId = discountInfo.DiscountId
                                });
                            }
                        }
                        #endregion

                        #region 明細折扣
                        if (detInfo.Discount.HasValue)
                        {
                            uploader.SOPriceAdjustmentsInterfaceList.Add(new ErpHelper.SOPriceAdjustmentsInterface()
                            {
                                CreationDate = now,
                                CreatedBY = custErpActor,
                                LastUpdateDate = now,
                                LastUpdatedBY = custErpActor,
                                OrderSourceId = 1103,
                                OriginalSystemReference = infoSet.Info.OdrNo,
                                OriginalSystemLineReference = detInfo.Sort.ToString(),
                                //100 - 明細折扣， EX: 100-60=40
                                Percent = (float)MathLib.Round(100 - (ConvertLib.ToSingle(detInfo.Discount, 1f) * 100), 4),
                                DiscountId = infoSet.CusterInfo.DetDiscountId
                            });
                        }
                        #endregion
                        #endregion
                    }

                    //string jsonInput = CustJson.SerializeObject(this);
                    //return jsonInput;

                    var talkResp = ErpAgentRef.UploadErp(uploader);
                    return talkResp.Code;
                }
            }
            #endregion

            #region 外銷出貨單上傳 ERP
            /// <summary>
            /// 外銷出貨單上傳 ERP。
            /// </summary>
            /// <param name="infoSet">內銷訂單資訊集合。</param>
            /// <returns>錯誤代碼。若「0000」則表示已成功上傳。</returns>
            public string Upload(ExtShippingOrderHelper.InfoSet infoSet)
            {
                var now = DateTime.Now;
                var custErpActor = 6214;

                #region 巡覽所有表頭折扣
                //所有表頭折扣總和
                float? totalHeaderDct = DefVal.Float;
                if (infoSet.HeaderDiscountInfos != null && infoSet.HeaderDiscountInfos.Count > 0)
                {
                    //針對品項折扣後總計再折扣 (公式「(100 - 表頭折扣) / 100」; 假設表頭折扣為「20%」, 則表示打「80%」折.)
                    totalHeaderDct = infoSet.HeaderDiscountInfos.Sum(q => q.Discount / 100);
                }
                #endregion

                ErpBaseHelper.ErpUploader uploader = new ErpBaseHelper.ErpUploader();

                using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    #region SO-HEADERS-INTERFACE-ALL
                    uploader.SOHeadersInterfaceAll = new ErpHelper.SOHeadersInterfaceAll()
                    {
                        CreationDate = now,
                        CreatedBY = custErpActor,
                        LastUpdateDate = now,
                        LastUpdatedBY = custErpActor,
                        OriginalSystemReference = infoSet.Info.OdrNo,
                        CustomerId = infoSet.Info.CustomerId,
                        OrderTypeId = infoSet.Info.OrderTypeId,
                        OrderSourceId = 1103,
                        OrderCategory = "R",
                        DateOrdered = infoSet.Info.Cdt,
                        CurrencyCode = infoSet.Info.CurrencyCode,
                        ConversionTypeCode = "1020",
                        SalesRepId = infoSet.Info.SalesRepId,
                        PriceListId = infoSet.Info.PriceListId,
                        EnteredStateId = 1,
                        ShipToSiteUseId = infoSet.Info.ShipToSiteUseId,
                        InvoiceToSiteUseId = infoSet.Info.InvoiceToSiteUseId
                    };
                    #endregion

                    uploader.SOLinesInterfaceAllList = new List<SOLinesInterfaceAll>();
                    uploader.SOLineDetailsInterfaceList = new List<SOLineDetailsInterface>();
                    uploader.SOPriceAdjustmentsInterfaceList = new List<SOPriceAdjustmentsInterface>();

                    foreach (var detInfo in infoSet.DetInfos)
                    {
                        #region SO-LINES-INTERFACE-ALL
                        uploader.SOLinesInterfaceAllList.Add(new ErpHelper.SOLinesInterfaceAll()
                        {
                            CreationDate = now,
                            CreatedBY = custErpActor,
                            LastUpdateDate = now,
                            LastUpdatedBY = custErpActor,
                            OriginalSystemReference = infoSet.Info.OdrNo,
                            OriginalSystemLineReference = detInfo.Sort.ToString(),
                            LineNumber = detInfo.Sort,
                            LineType = "REGULAR",
                            UnitCode = "EA",
                            OrderedQuantity = detInfo.Qty,
                            DateRequestedCurrent = infoSet.Info.Cdt,
                            CalculatePrice = "Y",
                            Attribute2 = detInfo.ExtQuotnOdrNo,
                            InventoryItemId = detInfo.InventoryItemId,
                            ShipToCustomerId = infoSet.Info.CustomerId,
                            OrderSourceId = 1103,
                            ScheduledShipmentDate = infoSet.Info.Edd,
                            WarehouseId = 228
                        });
                        #endregion

                        #region SO-LINE-DETAILS-INTERFACE
                        uploader.SOLineDetailsInterfaceList.Add(new ErpHelper.SOLineDetailsInterface()
                        {
                            CreationDate = now,
                            CreatedBY = custErpActor,
                            LastUpdateDate = now,
                            LastUpdatedBY = custErpActor,
                            OrderSourceId = 1103,
                            OriginalSystemReference = infoSet.Info.OdrNo,
                            OriginalSystemLineReference = detInfo.Sort.ToString(),
                            Quantity = detInfo.Qty,
                            ScheduleDate = infoSet.Info.Edd,
                            Subinventory = infoSet.Info.Whse,
                            WarehouseId = 228
                        });
                        #endregion

                        #region SO-PRICE-ADJUSTMENTS-INTERFACE
                        #region 表頭折扣
                        if (infoSet.HeaderDiscountInfos != null && infoSet.HeaderDiscountInfos.Count > 0)
                        {
                            foreach (var discountInfo in infoSet.HeaderDiscountInfos)
                            {
                                uploader.SOPriceAdjustmentsInterfaceList.Add(new ErpHelper.SOPriceAdjustmentsInterface()
                                {
                                    CreationDate = now,
                                    CreatedBY = custErpActor,
                                    LastUpdateDate = now,
                                    LastUpdatedBY = custErpActor,
                                    OrderSourceId = 1103,
                                    OriginalSystemReference = infoSet.Info.OdrNo,
                                    OriginalSystemLineReference = detInfo.Sort.ToString(),
                                    //(明細單價 X 現金折扣) / 牌價 X 100，EX:((10.9920 X 0.01)/18.3200)X100=0.6
                                    //(明細單價 X 獎金分攤折扣) / 牌價 X 100，EX:((10.9920 X 0.02)/18.3200)X100=1.2
                                    Percent = (float)MathLib.Round(detInfo.UnitPrice * discountInfo.Discount / detInfo.ListPrice * 100, 4),
                                    DiscountId = discountInfo.DiscountId
                                });
                            }
                        }
                        #endregion

                        #region 明細折扣
                        if (detInfo.Discount.HasValue)
                        {
                            //公式異動:
                            float percent = 0f;
                            if (totalHeaderDct.HasValue)
                            {
                                //有表頭折扣時: ((牌價 - (牌價 * (表頭折扣總和)) - 折扣後單價) / 牌價) * 100 = 折扣比率數值
                                percent = ((detInfo.ListPrice - (detInfo.ListPrice * totalHeaderDct.Value) - detInfo.UnitPriceDct) / detInfo.ListPrice) * 100;
                            }
                            else
                            {
                                //無表頭折扣時: ((牌價 - 折扣後單價) / 牌價) * 100 = 折扣比率數值
                                percent = ((detInfo.ListPrice - detInfo.UnitPriceDct) / detInfo.ListPrice) * 100;
                            }

                            uploader.SOPriceAdjustmentsInterfaceList.Add(new ErpHelper.SOPriceAdjustmentsInterface()
                            {
                                CreationDate = now,
                                CreatedBY = custErpActor,
                                LastUpdateDate = now,
                                LastUpdatedBY = custErpActor,
                                OrderSourceId = 1103,
                                OriginalSystemReference = infoSet.Info.OdrNo,
                                OriginalSystemLineReference = detInfo.Sort.ToString(),
                                ////100 - 明細折扣， EX: 100-60=40
                                //Percent = (float)MathLib.Round(100 - (ConvertLib.ToSingle(detInfo.Discount, 1f) * 100), 4),
                                Percent = (float)MathLib.Round(percent, 4),
                                DiscountId = infoSet.CusterInfo.DetDiscountId
                            });
                        }
                        #endregion
                        #endregion
                    }

                    //string jsonInput = CustJson.SerializeObject(this);
                    //return jsonInput;

                    var talkResp = ErpAgentRef.UploadErp(uploader);
                    return talkResp.Code;
                }
            }
            #endregion
        }
        #endregion
        #endregion
    }
}
