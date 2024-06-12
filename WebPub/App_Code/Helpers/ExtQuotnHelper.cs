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
    /// 外銷報價單 Helper。
    /// </summary>
    public class ExtQuotnHelper
    {
        #region 選項相關
        #region 外銷報價單狀態
        #region GetExtQuotnStatusItems
        /// <summary>
        /// 取得外銷報價單狀態項目。
        /// </summary>
        /// <param name="containsExtended">是否包含延伸的狀態。</param>
        /// <param name="status">指定的狀態。</param>
        public static ListItem[] GetExtQuotnStatusItems(bool containsExtended, params string[] statuses)
        {
            //0:草稿 1:待轉訂單 2:已轉訂單
            var items = new List<ListItem>();
            items.Add(new ListItem("草稿", "0"));
            items.Add(new ListItem("待轉訂單", "1"));
            items.Add(new ListItem("已轉訂單", "2"));

            if (containsExtended)
            {
                items.Add(new ListItem("已取消", "999"));
            }

            if (statuses != null && statuses.Length > 0)
            {
                items = items.Where(q => statuses.Contains(q.Value)).ToList();
            }

            return items.ToArray();
        }
        #endregion

        #region GetExtQuotnStatusName
        /// <summary>
        /// 取得外銷報價單狀態對應的名稱。
        /// </summary>
        /// <param name="value">外銷報價單狀態代碼。</param>
        /// <param name="isReadjust">是否報價單調整中。</param>
        /// <param name="isCancel">是否已取消。</param>
        public static string GetExtQuotnStatusName(int value, bool isReadjust, bool isCancel)
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
                return ExtQuotnHelper.GetExtQuotnStatusItems(false).Where(q => q.Value == value.ToString()).DefaultIfEmpty(new ListItem() { Text = string.Empty }).SingleOrDefault().Text;
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
            /// 初始化 ExtQuotnHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.DetInfos = new List<ExtQuotnDet.InfoView>();
            }

            /// <summary>
            /// 外銷報價單資訊。
            /// </summary>
            public ExtQuotn.InfoView Info { get; set; }
            /// <summary>
            /// ERP 客戶資訊。
            /// </summary>
            public ErpCuster.InfoView CusterInfo { get; set; }
            /// <summary>
            /// 外銷報價單明細資訊清單。
            /// </summary>
            public List<ExtQuotnDet.InfoView> DetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 ExtQuotnHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new ExtQuotn.InputInfo();
                this.DetInfos = new List<ExtQuotnDet.InputInfo>();
                this.GoodsEditInfos = new List<GoodsEditInfo>();
            }

            /// <summary>
            /// 外銷報價單資訊。
            /// </summary>
            public ExtQuotn.InputInfo Info { get; set; }
            /// <summary>
            /// 外銷報價單明細資訊清單。
            /// </summary>
            public List<ExtQuotnDet.InputInfo> DetInfos { get; set; }
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
        public static InfoSet Binding(ISystemId extQuotnSId)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                ExtQuotn entityExtQuotn = new ExtQuotn(SystemDefine.ConnInfo);
                //使用 InfoView 確保外銷報價單與客戶是關聯的
                returner = entityExtQuotn.GetInfoView(new ExtQuotn.InfoViewConds(ConvertLib.ToSIds(extQuotnSId), DefVal.SIds, DefVal.Ints, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool), 1, SqlOrder.Clear, IncludeScope.OnlyNotMarkDeleted);
                if (returner.IsCompletedAndContinue)
                {
                    var info = ExtQuotn.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

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

                    #region 外銷報價單明細
                    ExtQuotnDet entityExtQuotnDet = new ExtQuotnDet(SystemDefine.ConnInfo);
                    returner = entityExtQuotnDet.GetInfoView(new ExtQuotnDet.InfoViewConds(DefVal.SIds, infoSet.Info.SId, DefVal.Int, DefVal.Str, DefVal.Bool), Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending), IncludeScope.OnlyNotMarkDeleted);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(ExtQuotnDet.InfoView.Binding(returner.DataSet.Tables[0]));
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
            /// 初始化 ExtQuotnHelper.GoodsEditInfo 類別的新執行個體。
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
        }
        #endregion
    }
}
