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
    /// 內銷訂單 Helper。
    /// </summary>
    public class DomOrderHelper
    {
        /// <summary>
        /// 人民幣稅率。
        /// </summary>
        public const float RmbTax = 0.17f;

        #region 選項相關
        #region 內銷訂單產品別
        #region GetDomOrderProdTypeItems
        /// <summary>
        /// 取得內銷訂單產品別項目。
        /// </summary>
        /// <param name="prodTypes">指定的產品別。</param>
        public static ListItem[] GetDomOrderProdTypeItems(params string[] prodTypes)
        {
            //1:訂單 2:訂做品 3:加裝品 4:訂做加裝品
            var items = new ListItem[]{
                new ListItem("訂單", "1"),
                new ListItem("訂做品", "2"),
                new ListItem("加裝品", "3"),
                new ListItem("訂做加裝品", "4")
            };

            if (prodTypes != null && prodTypes.Length > 0)
            {
                items = items.Where(q => prodTypes.Contains(q.Value)).ToArray();
            }

            return items;
        }
        #endregion

        #region GetDomOrderProdTypeName
        /// <summary>
        /// 取得內銷訂單產品別對應的名稱。
        /// </summary>
        /// <param name="value">內銷訂單產品別代碼。</param>
        public static string GetDomOrderProdTypeName(int value)
        {
            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return DomOrderHelper.GetDomOrderProdTypeItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
        }
        #endregion
        #endregion

        #region 內銷訂單狀態
        #region GetDomOrderStatusItems
        /// <summary>
        /// 取得內銷訂單狀態項目。
        /// </summary>
        /// <param name="status">指定的狀態。</param>
        public static ListItem[] GetDomOrderStatusItems(params string[] statuses)
        {
            //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
            var items = new ListItem[]{
                new ListItem("草稿", "0"),
                new ListItem("營管部待審核", "1"),
                new ListItem("ERP上傳中", "2"),
                new ListItem("財務待審核", "3"),
                new ListItem("未付款待審核", "4"),
                new ListItem("待列印", "5"),
                new ListItem("已列印", "6"),
                new ListItem("備貨中", "7"),
                new ListItem("已出貨", "8")
            };

            if (statuses != null && statuses.Length > 0)
            {
                items = items.Where(q => statuses.Contains(q.Value)).ToArray();
            }

            return items;
        }
        #endregion

        #region GetDomOrderStatusName
        /// <summary>
        /// 取得內銷訂單狀態對應的名稱。
        /// </summary>
        /// <param name="value">內銷訂單狀態代碼。</param>
        public static string GetDomOrderStatusName(int value)
        {
            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return DomOrderHelper.GetDomOrderStatusItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
        }
        #endregion
        #endregion

        #region 內銷 ERP 訂單狀態
        #region GetDomErpOrderStatusItems
        /// <summary>
        /// 取得內銷 ERP 訂單狀態項目。
        /// </summary>
        public static ListItem[] GetDomErpOrderStatusItems()
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

        #region GetDomErpOrderStatusName
        /// <summary>
        /// 取得內銷 ERP 訂單狀態對應的名稱。
        /// </summary>
        /// <param name="value">內銷 ERP 訂單狀態代碼。</param>
        public static string GetDomErpOrderStatusName(int? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return DomOrderHelper.GetDomErpOrderStatusItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
        }
        #endregion

        #region GetDomErpOrderStatusCode
        /// <summary>
        /// 取得內銷 ERP 訂單狀態對應的代碼。
        /// </summary>
        /// <param name="value">內銷 ERP 訂單狀態代碼。</param>
        public static int? GetDomErpOrderStatusCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefVal.Int;
            }

            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return ConvertLib.ToInt(DomOrderHelper.GetDomErpOrderStatusItems().Where(q => q.Text == value).DefaultIfEmpty(new ListItem() { Value = string.Empty }).SingleOrDefault().Value, DefVal.Int);
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
            /// 初始化 DomOrderHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.HeaderDiscountInfos = new List<ErpDctRel.DomHeaderDiscountInfo>();
                this.DetInfos = new List<DomOrderDet.InfoView>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public DomOrder.InfoView Info { get; set; }
            /// <summary>
            /// ERP 客戶資訊。
            /// </summary>
            public ErpCuster.InfoView CusterInfo { get; set; }
            /// <summary>
            /// ERP 表頭折扣資訊清單。
            /// </summary>
            public List<ErpDctRel.DomHeaderDiscountInfo> HeaderDiscountInfos { get; set; }
            /// <summary>
            /// 內銷訂單明細資訊清單。
            /// </summary>
            public List<DomOrderDet.InfoView> DetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 DomOrderHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new DomOrder.InputInfo();
                this.HeaderDiscountInfos = new List<ErpDctRel.InputInfo>();
                this.DetInfos = new List<DomOrderDet.InputInfo>();
                this.GoodsEditInfos = new List<GoodsEditInfo>();

                this.SeledDetInfos = new List<ISystemId>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public DomOrder.InputInfo Info { get; set; }
            /// <summary>
            /// ERP 表頭折扣資訊清單。
            /// </summary>
            public List<ErpDctRel.InputInfo> HeaderDiscountInfos { get; set; }
            /// <summary>
            /// 內銷訂單明細資訊清單。
            /// </summary>
            public List<DomOrderDet.InputInfo> DetInfos { get; set; }
            /// <summary>
            /// 品項區塊資訊清單。
            /// </summary>
            public List<GoodsEditInfo> GoodsEditInfos { get; set; }

            /// <summary>
            /// 選擇的內銷訂單明細系統代號。
            /// </summary>
            public List<ISystemId> SeledDetInfos { get; set; }
        }
        #endregion

        #region 繫結資料
        /// <summary>
        /// 繫結資料。
        /// </summary>
        /// <param name="domOrderSId">內銷訂單系統代號。</param>
        public static InfoSet Binding(ISystemId domOrderSId)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                DomOrder entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                //使用 InfoView 確保訂單與客戶是關聯的
                returner = entityDomOrder.GetInfoView(new DomOrder.InfoViewConds(ConvertLib.ToSIds(domOrderSId), DefVal.SIds, DefVal.Strs, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Ints, DefVal.Bool), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                if (returner.IsCompletedAndContinue)
                {
                    var info = DomOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

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
                    returner = entityErpDctRel.GetDomHeaderDiscountInfo(new ErpDctRel.DomHeaderDiscountInfoConds(ConvertLib.ToSIds(infoSet.Info.SId), DefVal.Longs, IncludeScope.All), Int32.MaxValue, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.HeaderDiscountInfos.AddRange(ErpDctRel.DomHeaderDiscountInfo.Binding(returner.DataSet.Tables[0]));
                    }
                    #endregion

                    #region 內銷訂單明細
                    DomOrderDet entityDomOrderDet = new DomOrderDet(SystemDefine.ConnInfo);
                    returner = entityDomOrderDet.GetInfoView(new DomOrderDet.InfoViewConds(DefVal.SIds, infoSet.Info.SId, DefVal.Int, DefVal.SId, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Bool), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending), IncludeScope.OnlyNotMarkDeleted);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(DomOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
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

        #region 品項區塊資訊
        /// <summary>
        /// 品項區塊資訊。
        /// </summary>
        public class GoodsEditInfo
        {
            /// <summary>
            /// 初始化 DomOrderHelper.GoodsEditInfo 類別的新執行個體。
            /// </summary>
            public GoodsEditInfo()
            {
                this.Items = new List<GoodsItemEditInfo>();
            }

            /// <summary>
            /// 區塊標題。
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 報價單號碼。
            /// 若有值表示為專案報價區塊。
            /// </summary>
            public string QuoteNumber { get; set; }

            /// <summary>
            /// 品項編輯資訊陣列集合。
            /// </summary>
            public List<GoodsItemEditInfo> Items = new List<GoodsItemEditInfo>();
        }
        #endregion

        #region 品項項目編輯資訊
        /// <summary>
        /// 品項項目編輯資訊。
        /// </summary>
        public class GoodsItemEditInfo
        {
            /// <summary>
            /// 初始化 DomOrderHelper.GoodsItemEditInfo 類別的新執行個體。
            /// </summary>
            public GoodsItemEditInfo()
            {
                this.ErpWhseOnHands = new ErpHelper.ErpWhseOnHand[0];
            }

            /// <summary>
            /// 是否被選擇。
            /// for 快速建立備貨單用。
            /// </summary>
            public bool IsSeled { get; set; }

            /// <summary>
            /// 系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 序號。
            /// </summary>
            public int SeqNo { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 備貨單編號。
            /// 若有值表示為來源為備貨單。
            /// </summary>
            public string PGOrderOdrNo { get; set; }
            /// <summary>
            /// 備貨單明細系統代號。
            /// 若有值表示為來源為備貨單。
            /// </summary>
            public ISystemId PGOrderDetSId { get; set; }
            /// <summary>
            /// 報價單號碼。
            /// 若有值表示為專案報價品項。
            /// </summary>
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次。
            /// 若有值表示為專案報價品項。
            /// </summary>
            public string QuoteItemId { get; set; }

            /// <summary>
            /// ERP 在手量。
            /// </summary>
            public int? ErpOnHand { get; set; }
            /// <summary>
            /// ERP 倉庫在手量陣列集合。
            /// 檢視時要顯示全部的倉庫。
            /// </summary>
            public ErpHelper.ErpWhseOnHand[] ErpWhseOnHands { get; set; }

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
            /// 預設折扣。
            /// 若有值時，折扣不得小於預設折扣。
            /// </summary>
            public float? DefDct { get; set; }
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
    }
}
