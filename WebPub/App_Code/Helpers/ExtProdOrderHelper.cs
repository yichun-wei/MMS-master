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
    /// 外銷生產單 Helper。
    /// </summary>
    public class ExtProdOrderHelper
    {
        #region 選項相關
        #region 外銷生產單狀態
        #region GetExtProdOrderStatusItems
        /// <summary>
        /// 取得外銷生產單狀態項目。
        /// </summary>
        /// <param name="status">指定的狀態。</param>
        public static ListItem[] GetExtProdOrderStatusItems(params string[] statuses)
        {
            //1:未確認 2:已確認
            var items = new List<ListItem>();
            items.Add(new ListItem("未確認", "1"));
            items.Add(new ListItem("已確認", "2"));

            if (statuses != null && statuses.Length > 0)
            {
                items = items.Where(q => statuses.Contains(q.Value)).ToList();
            }

            return items.ToArray();
        }
        #endregion

        #region GetExtProdOrderStatusName
        /// <summary>
        /// 取得外銷生產單狀態對應的名稱。
        /// </summary>
        /// <param name="value">外銷生產單狀態代碼。</param>
        public static string GetExtProdOrderStatusName(int value)
        {
            //以列表會一次顯示多筆來講, 每一次都 new 一次或許有點蠢, 但目前看來還好, 先不將取得值 cache 在 Application (或其他).
            return ExtProdOrderHelper.GetExtProdOrderStatusItems().Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
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
            /// 初始化 ExtProdOrderHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.DetInfos = new List<ExtProdOrderDet.InfoView>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public ExtProdOrder.InfoView Info { get; set; }
            /// <summary>
            /// 外銷生產單明細資訊清單。
            /// </summary>
            public List<ExtProdOrderDet.InfoView> DetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 ExtProdOrderHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new ExtProdOrder.InputInfo();
                this.DetInfos = new List<ExtProdOrderDet.InputInfo>();
                this.GoodsEditInfos = new List<GoodsEditInfo>();
            }

            /// <summary>
            /// 訂單資訊。
            /// </summary>
            public ExtProdOrder.InputInfo Info { get; set; }
            /// <summary>
            /// 外銷生產單明細資訊清單。
            /// </summary>
            public List<ExtProdOrderDet.InputInfo> DetInfos { get; set; }
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
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <param name="extOrderVersion">外銷訂單版本（若為 null 則略過條件檢查）。</param>
        /// <param name="extOrderActiveFlag">外銷訂單是否作用中（若為 null 則略過條件檢查）。</param>
        /// <param name="extProdOrderSId">外銷生產單系統代號（若為 null 則略過條件檢查）。</param>
        /// <param name="version">版本（若為 null 則略過條件檢查）。</param>
        /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
        /// <param name="containsPrevVer">是否包含前一個版本的資料。</param>
        public static InfoSet Binding(ISystemId extQuotnSId, ISystemId extOrderSId, int? extOrderVersion, bool? extOrderActiveFlag, ISystemId extProdOrderSId, int? version, bool? activeFlag)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                ExtProdOrder entityExtProdOrder = new ExtProdOrder(SystemDefine.ConnInfo);
                //改成用生產單系統代號
                ////取得最新的版本
                //returner = entityExtProdOrder.GetInfoView(new ExtProdOrder.InfoViewConds(ConvertLib.ToSIds(extProdOrderSId), DefVal.SIds, extQuotnSId, extOrderSId, extOrderVersion, extOrderActiveFlag, version, activeFlag, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool), 1, new SqlOrder("VERSION", Sort.Descending), IncludeScope.OnlyNotMarkDeleted);
                returner = entityExtProdOrder.GetInfoView(new ExtProdOrder.InfoViewConds(ConvertLib.ToSIds(extProdOrderSId), DefVal.SIds, extQuotnSId, extOrderSId, extOrderVersion, extOrderActiveFlag, version, activeFlag, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool), 1, new SqlOrder("VERSION", Sort.Descending), IncludeScope.OnlyNotMarkDeleted);
                if (returner.IsCompletedAndContinue)
                {
                    var info = ExtProdOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                    infoSet = new InfoSet();
                    infoSet.Info = info;
                }

                if (infoSet != null)
                {
                    #region 外銷生產單明細
                    ExtProdOrderDet entityExtProdOrderDet = new ExtProdOrderDet(SystemDefine.ConnInfo);
                    returner = entityExtProdOrderDet.GetInfoView(new ExtProdOrderDet.InfoViewConds(DefVal.SIds, infoSet.Info.SId, DefVal.Int, DefVal.Str, DefVal.Bool), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending), IncludeScope.OnlyNotMarkDeleted);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(ExtProdOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
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
            /// 初始化 ExtProdOrderHelper.GoodsEditInfo 類別的新執行個體。
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
            /// 料號所屬製造組織代碼。
            /// </summary>
            public string OrgCode { get; set; }
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
            /// 需求量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 累計生產量。
            /// </summary>
            public int? CumProdQty { get; set; }
            /// <summary>
            /// 生產量。
            /// </summary>
            public int? ProdQty { get; set; }
            /// <summary>
            /// 預計繳庫日。
            /// </summary>
            public DateTime? EstFpmsDate { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
        }
        #endregion
    }
}
