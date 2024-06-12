using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 外銷出貨單 Helper。
    /// </summary>
    public class ExtShippingOrderHelper
    {
        #region 選項相關
        #region 外銷出貨單狀態
        #region GetExtOrderStatusItems
        /// <summary>
        /// 取得外銷出貨單狀態項目。
        /// </summary>
        /// <param name="status">指定的狀態。</param>
        public static ListItem[] GetExtOrderStatusItems(params string[] statuses)
        {
            //0:草稿 1:已確認 2:已出貨 3:已上傳
            var items = new ListItem[]{
                new ListItem("草稿", "0"),
                new ListItem("已確認", "1"),
                new ListItem("已出貨", "2"),
                new ListItem("已上傳", "3")
            };

            if (statuses != null && statuses.Length > 0)
            {
                items = items.Where(q => statuses.Contains(q.Value)).ToArray();
            }

            return items;
        }
        #endregion

        #region GetExtOrderStatusName
        /// <summary>
        /// 取得外銷出貨單狀態對應的名稱。
        /// </summary>
        /// <param name="value">外銷出貨單狀態代碼。</param>
        public static string GetExtOrderStatusName(int value)
        {
            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return ExtShippingOrderHelper.GetExtOrderStatusItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
        }
        #endregion
        #endregion

        #region 外銷 ERP 訂單狀態
        #region GetExtErpOrderStatusItems
        /// <summary>
        /// 取得外銷 ERP 訂單狀態項目。
        /// </summary>
        public static ListItem[] GetExtErpOrderStatusItems()
        {
            //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
            var items = new ListItem[]{
                new ListItem("已輸入", "1"),
                new ListItem("已登錄", "2"),
                new ListItem("已超額", "3"),
                new ListItem("超額已核發", "4"),
                new ListItem("已取消", "5"),
                new ListItem("已關閉", "6")
            };

            return items;
        }
        #endregion

        #region GetExtErpOrderStatusName
        /// <summary>
        /// 取得外銷 ERP 訂單狀態對應的名稱。
        /// </summary>
        /// <param name="value">外銷 ERP 訂單狀態代碼。</param>
        public static string GetExtErpOrderStatusName(int? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return ExtShippingOrderHelper.GetExtErpOrderStatusItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
        }
        #endregion

        #region GetExtErpOrderStatusCode
        /// <summary>
        /// 取得外銷 ERP 訂單狀態對應的代碼。
        /// </summary>
        /// <param name="value">外銷 ERP 訂單狀態代碼。</param>
        public static int? GetExtErpOrderStatusCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefVal.Int;
            }

            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return ConvertLib.ToInt(ExtShippingOrderHelper.GetExtErpOrderStatusItems().Where(q => q.Text == value).DefaultIfEmpty(new ListItem() { Value = string.Empty }).SingleOrDefault().Value, DefVal.Int);
        }
        #endregion
        #endregion
        #endregion

        #region 資訊集合
        /// <summary>
        /// 資訊集合。
        /// </summary>
        public class InfoSet
        {
            /// <summary>
            /// 初始化 ExtShippingOrderHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.HeaderDiscountInfos = new List<ErpDctRel.ExtShippingHeaderDiscountInfo>();
                this.DetInfos = new List<ExtShippingOrderDet.InfoView>();
            }

            /// <summary>
            /// 外銷出貨單資訊。
            /// </summary>
            public ExtShippingOrder.InfoView Info { get; set; }
            /// <summary>
            /// ERP 客戶資訊。
            /// </summary>
            public ErpCuster.InfoView CusterInfo { get; set; }
            /// <summary>
            /// ERP 表頭折扣資訊清單。
            /// </summary>
            public List<ErpDctRel.ExtShippingHeaderDiscountInfo> HeaderDiscountInfos { get; set; }
            /// <summary>
            /// 外銷出貨單明細資訊清單。
            /// </summary>
            public List<ExtShippingOrderDet.InfoView> DetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 ExtShippingOrderHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new ExtShippingOrder.InputInfo();
                this.HeaderDiscountInfos = new List<ErpDctRel.InputInfo>();
                this.DetInfos = new List<ExtShippingOrderDet.InputInfo>();

                this.GoodsEditInfos_OdrBase = new List<GoodsEditInfo_OdrBase>();
                this.SeledDetInfos = new List<ISystemId>();

                this.GoodsEditInfos_PNBase = new List<GoodsEditInfo_PNBase>();
            }

            /// <summary>
            /// 外銷出貨單資訊。
            /// </summary>
            public ExtShippingOrder.InputInfo Info { get; set; }
            /// <summary>
            /// ERP 表頭折扣資訊清單。
            /// </summary>
            public List<ErpDctRel.InputInfo> HeaderDiscountInfos { get; set; }
            /// <summary>
            /// 外銷出貨單明細資訊清單。
            /// </summary>
            public List<ExtShippingOrderDet.InputInfo> DetInfos { get; set; }

            /// <summary>
            /// 品項區塊資訊清單（依訂單建立）。
            /// </summary>
            public List<GoodsEditInfo_OdrBase> GoodsEditInfos_OdrBase { get; set; }
            /// <summary>
            /// 選擇的外銷出貨單明細系統代號。
            /// </summary>
            public List<ISystemId> SeledDetInfos { get; set; }

            /// <summary>
            /// 品項區塊資訊清單（依品項建立）。
            /// </summary>
            public List<GoodsEditInfo_PNBase> GoodsEditInfos_PNBase { get; set; }
        }
        #endregion

        #region 繫結資料
        /// <summary>
        /// 繫結資料。
        /// </summary>
        /// <param name="extShippingOrderSId">外銷出貨單系統代號。</param>
        public static InfoSet Binding(ISystemId extShippingOrderSId)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                ExtShippingOrder entityExtShippingOrder = new ExtShippingOrder(SystemDefine.ConnInfo);
                //使用 InfoView 確保訂單與客戶是關聯的
                returner = entityExtShippingOrder.GetInfoView(new ExtShippingOrder.InfoViewConds(ConvertLib.ToSIds(extShippingOrderSId), DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                if (returner.IsCompletedAndContinue)
                {
                    var info = ExtShippingOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                    infoSet = new InfoSet();
                    infoSet.Info = info;
                }

                if (infoSet != null)
                {
                    #region ERP 客戶
                    ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
                    returner = entityErpCuster.GetInfoView(new ErpCuster.InfoViewConds(DefVal.SIds, ConvertLib.ToLongs(infoSet.Info.CustomerId), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.CusterInfo = ErpCuster.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);
                    }
                    #endregion

                    #region 表頭折扣
                    ErpDctRel entityErpDctRel = new ErpDctRel(SystemDefine.ConnInfo);
                    returner = entityErpDctRel.GetExtShippingHeaderDiscountInfo(new ErpDctRel.ExtShippingHeaderDiscountInfoConds(ConvertLib.ToSIds(infoSet.Info.SId), DefVal.Longs, IncludeScope.All), Int32.MaxValue, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.HeaderDiscountInfos.AddRange(ErpDctRel.ExtShippingHeaderDiscountInfo.Binding(returner.DataSet.Tables[0]));
                    }
                    #endregion

                    #region 外銷出貨單明細
                    ExtShippingOrderDet entityExtShippingOrderDet = new ExtShippingOrderDet(SystemDefine.ConnInfo);
                    returner = entityExtShippingOrderDet.GetInfoView(new ExtShippingOrderDet.InfoViewConds(DefVal.SIds, ConvertLib.ToSIds(infoSet.Info.SId), DefVal.SIds, DefVal.Str, DefVal.Str), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending), IncludeScope.OnlyNotMarkDeleted);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(ExtShippingOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
                    }
                    #endregion
                }

                return infoSet;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion

        #region 品項區塊資訊 - 依訂單建立
        #region 品項區塊資訊
        /// <summary>
        /// 品項區塊資訊。
        /// </summary>
        public class GoodsEditInfo_OdrBase
        {
            /// <summary>
            /// 初始化 ExtShippingOrderHelper.GoodsEditInfo_OdrBase 類別的新執行個體。
            /// </summary>
            public GoodsEditInfo_OdrBase()
            {
                this.Items = new List<GoodsItemEditInfo_OdrBase>();
            }

            /// <summary>
            /// 區塊標題。
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 外銷訂單系統代號。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }

            /// <summary>
            /// 品項編輯資訊陣列集合。
            /// </summary>
            public List<GoodsItemEditInfo_OdrBase> Items { get; set; }
        }
        #endregion

        #region 品項項目編輯資訊
        /// <summary>
        /// 品項項目編輯資訊。
        /// </summary>
        public class GoodsItemEditInfo_OdrBase
        {
            /// <summary>
            /// 外銷出貨單明細系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 序號。
            /// </summary>
            public int SeqNo { get; set; }
            /// <summary>
            /// 外銷訂單明細系統代號。
            /// </summary>
            public ISystemId ExtOrderDetSId { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// ERP 在手量。
            /// </summary>
            public int? ErpOnHand { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 品項允許的最大值。
            /// </summary>
            public int MaxQty { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            public float? ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            public float? UnitPrice { get; set; }
            /// <summary>
            /// 折扣後單價。
            /// </summary>
            public float? UnitPriceDct { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            public float? PaidAmt { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
        }
        #endregion
        #endregion

        #region 品項區塊資訊 - 依品項建立
        #region 品項區塊資訊
        /// <summary>
        /// 品項區塊資訊。
        /// </summary>
        public class GoodsEditInfo_PNBase
        {
            /// <summary>
            /// 初始化 ExtShippingOrderHelper.GoodsEditInfo_PNBase 類別的新執行個體。
            /// </summary>
            public GoodsEditInfo_PNBase()
            {
                this.Items = new List<GoodsItemEditInfo_PNBase>();
            }

            /// <summary>
            /// 品項編輯資訊陣列集合。
            /// </summary>
            public List<GoodsItemEditInfo_PNBase> Items { get; set; }
        }
        #endregion

        #region 品項項目編輯資訊
        /// <summary>
        /// 品項項目編輯資訊。
        /// </summary>
        public class GoodsItemEditInfo_PNBase
        {
            /// <summary>
            /// 初始化 ExtShippingOrderHelper.GoodsItemEditInfo_PNBase 類別的新執行個體。
            /// </summary>
            public GoodsItemEditInfo_PNBase()
            {
                this.DetItems = new List<GoodsItemDetEditInfo_PNBase>();
            }

            /// <summary>
            /// 系統代號。
            /// 項目區塊識別用。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 序號。
            /// </summary>
            public int SeqNo { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// ERP 在手量。
            /// </summary>
            public int? ErpOnHand { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            public float? ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            public float? UnitPrice { get; set; }
            /// <summary>
            /// 折扣後單價。
            /// </summary>
            public float? UnitPriceDct { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            public float? PaidAmt { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }

            /// <summary>
            /// 品項項目細項編輯資訊陣列集合。
            /// </summary>
            public List<GoodsItemDetEditInfo_PNBase> DetItems { get; set; }
        }
        #endregion

        #region 品項項目細項編輯資訊
        /// <summary>
        /// 品項項目細項編輯資訊。
        /// </summary>
        public class GoodsItemDetEditInfo_PNBase
        {
            /// <summary>
            /// 外銷出貨單明細系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 報價單編號。
            /// </summary>
            public string OdrNo { get; set; }
            /// <summary>
            /// 外銷訂單明細系統代號。
            /// </summary>
            public ISystemId ExtOrderDetSId { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 品項允許的最大值。
            /// </summary>
            public int MaxQty { get; set; }
        }
        #endregion
        #endregion

        #region 取得外銷訂單明細的目前可用數量
        /// <summary>
        /// 取得外銷訂單明細的目前可用數量。
        /// </summary>
        /// <param name="extOrderDetSId">外銷訂單明細系統代號。</param>
        public static int GetExtOrderDetMaxQty(ISystemId extOrderDetSId)
        {
            Returner returner = null;
            try
            {
                if (extOrderDetSId != null)
                {
                    ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);
                    returner = entityExtOrderDet.GetInfoView(new ExtOrderDet.InfoViewConds(ConvertLib.ToSIds(extOrderDetSId), DefVal.Long, DefVal.SIds, DefVal.Int, DefVal.Str, false, false, IncludeScope.All), 1, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                        //數量 - 出貨單使用數量
                        return info.Qty - info.ShipOdrUseQty;
                    }
                }

                return 0;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion
    }
}
