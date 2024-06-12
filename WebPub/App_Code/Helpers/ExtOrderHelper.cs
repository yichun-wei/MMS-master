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
    /// 外銷訂單 Helper。
    /// </summary>
    public class ExtOrderHelper
    {
        #region 選項相關
        #region 外銷訂單狀態
        #region GetExtOrderStatusItems
        /// <summary>
        /// 取得外銷訂單狀態項目。
        /// </summary>
        /// <param name="containsExtended">是否包含延伸的狀態。</param>
        /// <param name="status">指定的狀態。</param>
        public static ListItem[] GetExtOrderStatusItems(bool containsExtended, params string[] statuses)
        {
            //1:待轉訂單 2:正式訂單 3:正式訂單-未排程 4:正式訂單-已排程
            var items = new List<ListItem>();
            items.Add(new ListItem("待轉訂單", "1"));
            items.Add(new ListItem("正式訂單", "2"));
            items.Add(new ListItem("正式訂單-未排程", "3"));
            items.Add(new ListItem("正式訂單-已排程", "4"));

            if (containsExtended)
            {
                items.Add(new ListItem("已取消", "999"));
                items.Add(new ListItem("報價調整中", "200"));
            }

            if (statuses != null && statuses.Length > 0)
            {
                items = items.Where(q => statuses.Contains(q.Value)).ToList();
            }

            return items.ToArray();
        }
        #endregion

        #region GetExtOrderStatusName
        /// <summary>
        /// 取得外銷訂單狀態對應的名稱。
        /// </summary>
        /// <param name="value">外銷訂單狀態代碼。</param>
        public static string GetExtOrderStatusName(int value, bool isReadjust, bool isCancel)
        {
            if (isCancel)
            {
                return "已取消";
            }
            else if (isReadjust)
            {
                return "報價調整中";
            }
            else
            {
                //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
                return ExtOrderHelper.GetExtOrderStatusItems(false).Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
            }
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
            /// 初始化 ExtOrderHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.DetInfos = new List<ExtOrderDet.InfoView>();
                this.PrevVerDetInfos = new List<ExtOrderDet.InfoView>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public ExtOrder.InfoView Info { get; set; }
            /// <summary>
            /// ERP 客戶資訊。
            /// </summary>
            public ErpCuster.InfoView CusterInfo { get; set; }
            /// <summary>
            /// 外銷訂單明細資訊清單。
            /// </summary>
            public List<ExtOrderDet.InfoView> DetInfos { get; set; }

            /// <summary>
            /// 外銷生產單。
            /// </summary>
            public ExtProdOrder.Info ProdInfo { get; set; }

            //正式訂單使用
            /// <summary>
            /// 外銷訂單明細資訊清單（前一個版本）。
            /// </summary>
            public List<ExtOrderDet.InfoView> PrevVerDetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 ExtOrderHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new ExtOrder.InputInfo();
                this.DetInfos = new List<ExtOrderDet.InputInfo>();
                this.GoodsEditInfos = new List<GoodsEditInfo>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public ExtOrder.InputInfo Info { get; set; }
            /// <summary>
            /// 外銷訂單明細資訊清單。
            /// </summary>
            public List<ExtOrderDet.InputInfo> DetInfos { get; set; }
            /// <summary>
            /// 品項區塊資訊清單。
            /// </summary>
            public List<GoodsEditInfo> GoodsEditInfos { get; set; }
        }
        #endregion

        #region 繫結資料
        /// <summary>
        /// 繫結資料。
        /// </summary>
        /// <param name="extQuotnSId">外銷報價單系統代號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號（若為 null 則略過條件檢查）。</param>
        /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
        /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
        public static InfoSet Binding(ISystemId extQuotnSId, ISystemId extOrderSId, int? version, bool? activeFlag)
        {
            return ExtOrderHelper.Binding(extQuotnSId, extOrderSId, version, activeFlag, false);
        }

        /// <summary>
        /// 繫結資料。
        /// </summary>
        /// <param name="extQuotnSId">外銷報價單系統代號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號（若為 null 則略過條件檢查）。</param>
        /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
        /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
        /// <param name="containsPrevVer">是否包含前一個版本的資料。</param>
        public static InfoSet Binding(ISystemId extQuotnSId, ISystemId extOrderSId, int? version, bool? activeFlag, bool containsPrevVer)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                ExtOrder entityExtOrder = new ExtOrder(SystemDefine.ConnInfo);
                //取得最新的版本
                returner = entityExtOrder.GetInfoView(new ExtOrder.InfoViewConds(ConvertLib.ToSIds(extOrderSId), DefVal.SIds, extQuotnSId, version, activeFlag, DefVal.Ints, DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool, DefVal.Bool), 1, new SqlOrder("VERSION", Sort.Descending), IncludeScope.OnlyNotMarkDeleted);
                if (returner.IsCompletedAndContinue)
                {
                    var info = ExtOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

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

                    #region 外銷訂單明細
                    ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);
                    returner = entityExtOrderDet.GetInfoView(new ExtOrderDet.InfoViewConds(DefVal.SIds, DefVal.Long, ConvertLib.ToSIds(infoSet.Info.SId), DefVal.Int, DefVal.Str, DefVal.Bool, false, IncludeScope.OnlyNotMarkDeleted), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending));
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
                    }
                    #endregion

                    #region 外銷生產單
                    ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                    returner = entityExtProdOrder.GetInfo(new ExtProdOrder.InfoConds(DefVal.SIds, infoSet.Info.SId, DefVal.Int, true, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.ProdInfo = ExtProdOrder.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                    }
                    #endregion

                    #region 是否包含前一個版本的資料
                    if (containsPrevVer)
                    {
                        if (infoSet.Info.Version > 1)
                        {
                            returner = entityExtOrder.GetInfoView(new ExtOrder.InfoViewConds(DefVal.SIds, DefVal.SIds, extQuotnSId, infoSet.Info.Version - 1, DefVal.Bool, DefVal.Ints, DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool, DefVal.Bool), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                            if (returner.IsCompletedAndContinue)
                            {
                                var prevVerOrderInfo = ExtOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                                #region 外銷訂單明細（前一個版本）
                                returner = entityExtOrderDet.GetInfoView(new ExtOrderDet.InfoViewConds(DefVal.SIds, DefVal.Long, ConvertLib.ToSIds(prevVerOrderInfo.SId), DefVal.Int, DefVal.Str, DefVal.Bool, false, IncludeScope.OnlyNotMarkDeleted), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending));
                                if (returner.IsCompletedAndContinue)
                                {
                                    infoSet.PrevVerDetInfos.AddRange(ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
                                }
                                #endregion
                            }
                        }
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
            /// 初始化 ExtOrderHelper.GoodsEditInfo 類別的新執行個體。
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
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            public int Source { get; set; }

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
            /// 系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 序號。
            /// </summary>
            public int SeqNo { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            public int Source { get; set; }
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

            //正式訂單使用
            /// <summary>
            /// 修改前數量。
            /// </summary>
            public int? BeforeTransQty { get; set; }
        }
        #endregion
    }
}
