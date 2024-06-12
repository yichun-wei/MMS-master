using System;
using System.Collections.Generic;
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
    /// 備貨單 Helper。
    /// </summary>
    public class PGOrderHelper
    {
        #region 資訊集合
        /// <summary>
        /// 資訊集合。
        /// </summary>
        public class InfoSet
        {
            /// <summary>
            /// 初始化 PGOrderHelper.InfoSet 類別的新執行個體。
            /// </summary>
            public InfoSet()
            {
                this.DetInfos = new List<PGOrderDet.InfoView>();
            }

            /// <summary>
            /// 備貨單資訊。
            /// </summary>
            public PGOrder.InfoView Info { get; set; }
            /// <summary>
            /// ERP 客戶資訊。
            /// </summary>
            public ErpCuster.Info CusterInfo { get; set; }
            /// <summary>
            /// 備貨單明細資訊清單。
            /// </summary>
            public List<PGOrderDet.InfoView> DetInfos { get; set; }
        }
        #endregion

        #region 輸入資訊集合
        /// <summary>
        /// 輸入資訊集合。
        /// </summary>
        public class InputInfoSet
        {
            /// <summary>
            /// 初始化 PGOrderHelper.InputInfoSet 類別的新執行個體。
            /// </summary>
            public InputInfoSet()
            {
                this.Info = new PGOrder.InputInfo();
                this.DetInfos = new List<PGOrderDet.InputInfo>();
                this.GoodsEditInfos = new List<GoodsEditInfo>();
            }

            /// <summary>
            /// 備貨單資訊。
            /// </summary>
            public PGOrder.InputInfo Info { get; set; }
            /// <summary>
            /// 備貨單明細資訊清單。
            /// </summary>
            public List<PGOrderDet.InputInfo> DetInfos { get; set; }
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
        /// <param name="pgOrderSId">備貨單系統代號。</param>
        public static InfoSet Binding(ISystemId pgOrderSId)
        {
            return PGOrderHelper.Binding(pgOrderSId, IncludeScope.All);
        }

        /// <summary>
        /// 繫結資料。
        /// </summary>
        /// <param name="pgOrderSId">備貨單系統代號。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        public static InfoSet Binding(ISystemId pgOrderSId, IncludeScope includeScope)
        {
            Returner returner = null;
            try
            {
                InfoSet infoSet = null;

                PGOrder entityPGOrder = new PGOrder(SystemDefine.ConnInfo);
                //使用 InfoView 確保備貨單與客戶是關聯的
                returner = entityPGOrder.GetInfoView(new PGOrder.InfoViewConds(ConvertLib.ToSIds(pgOrderSId), DefVal.SIds, DefVal.Long, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.DT, DefVal.Bool, DefVal.Bool), 1, SqlOrder.Clear, includeScope);
                if (returner.IsCompletedAndContinue)
                {
                    var info = PGOrder.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                    infoSet = new InfoSet();
                    infoSet.Info = info;
                }

                if (infoSet != null)
                {
                    #region ERP 客戶
                    ErpCuster entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);
                    returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, ConvertLib.ToLongs(infoSet.Info.CustomerId), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, SqlOrder.Clear, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.CusterInfo = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                    }
                    #endregion

                    #region 備貨單明細
                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                    returner = entityPGOrderDet.GetInfoView(new PGOrderDet.InfoViewConds(DefVal.SIds, infoSet.Info.SId, DefVal.Str, DefVal.Int, DefVal.Str, DefVal.Str, DefVal.Str, infoSet.Info.CustomerId, DefVal.Bool, false, includeScope), Int32.MaxValue, SqlOrder.Default);
                    if (returner.IsCompletedAndContinue)
                    {
                        infoSet.DetInfos.AddRange(PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0]));
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

        #region 取得備貨單狀態
        /// <summary>
        /// 取得備貨單狀態。
        /// </summary>
        public static int GetOrderStatus(PGOrder.InfoView info)
        {
            int status = -1;

            //1:未使用 2:已使用 3:已取消
            if (info != null)
            {
                if (info.IsCancel)
                {
                    return 3;
                }
                else if (info.UseQty > 0)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }

            return status;
        }

        /// <summary>
        /// 取得備貨單狀態名稱。
        /// </summary>
        public static string GetOrderStatusName(PGOrder.InfoView info)
        {
            string status = string.Empty;

            if (info != null)
            {
                if (info.IsCancel)
                {
                    status = "已取消";
                }
                else if (info.UseQty > 0)
                {
                    status = "已使用";
                }
                else
                {
                    status = "未使用";
                }
            }

            return status;
        }
        #endregion

        #region 品項區塊資訊
        /// <summary>
        /// 品項區塊資訊。
        /// </summary>
        public class GoodsEditInfo
        {
            /// <summary>
            /// 初始化 PGOrderHelper.GoodsEditInfo 類別的新執行個體。
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
            /// 摘要。
            /// </summary>
            public string Summary { get; set; }
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
            /// 數量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 品項允許的最大值。
            /// </summary>
            public int MaxQty { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
        }
        #endregion

        #region 取得備貨單明細的目前可用數量
        /// <summary>
        /// 取得備貨單明細的目前可用數量。
        /// </summary>
        /// <param name="pgOrderDetSId">備貨單明細系統代號。</param>
        public static int GetPGOrderDetMaxQty(ISystemId pgOrderDetSId)
        {
            Returner returner = null;
            try
            {
                if (pgOrderDetSId != null)
                {
                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);
                    returner = entityPGOrderDet.GetInfoView(new PGOrderDet.InfoViewConds(ConvertLib.ToSIds(pgOrderDetSId), DefVal.SId, DefVal.Str, DefVal.Int, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Long, false, false, IncludeScope.All), 1, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                        //數量 - 內銷訂單使用數量
                        return info.Qty - info.DomOrderUseQty;
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

        /// <summary>
        /// 快速匯入。
        /// </summary>
        public class QuickImporter
        {
            /// <summary>
            /// 內銷訂單系統代號。
            /// </summary>
            public ISystemId DomOrderSId { get; set; }
            /// <summary>
            /// 選擇的內銷訂單明細系統代號。
            /// </summary>
            public ISystemId[] SeledDetSIds { get; set; }
        }
    }
}
