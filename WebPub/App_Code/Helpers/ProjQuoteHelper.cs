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
using System.Web.UI.WebControls;

namespace Seec.Marketing
{
    /// <summary>
    /// 專案報價 Helper。
    /// </summary>
    public class ProjQuoteHelper
    {
        #region 選項相關
        #region 專案取消狀態
        #region GetProjQuoteCancelStatusItems
        /// <summary>
        /// 取得專案取消狀態。
        /// </summary>
        public static ListItem[] GetProjQuoteCancelStatusItems()
        {
            //1:已輸入 2:已取消
            var items = new ListItem[]{
                new ListItem("未取消", "N"),
                new ListItem("已取消", "Y")
            };

            return items;
        }
        #endregion
        #endregion
        #endregion

        #region 取得專案報價的目前可用數量
        /// <summary>
        /// 取得專案報價的目前可用數量。
        /// </summary>
        /// <param name="quoteNumber">報價單號碼。</param>
        /// <param name="quoteItemId">報價單明細項次。</param>
        public static int GetMaxQty(string quoteNumber, string quoteItemId)
        {
            Returner returner = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(quoteNumber) && !string.IsNullOrWhiteSpace(quoteItemId))
                {
                    ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);
                    returner = entityProjQuote.GetInfoView(new ProjQuote.InfoViewConds(ConvertLib.ToStrs(quoteNumber), ConvertLib.ToStrs(quoteItemId), DefVal.Str), 1, SqlOrder.Clear);
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0].Rows[0]);

                        //數量 - 備貨單使用數量 - 內銷訂單使用數量
                        return info.Quantity - info.PGOrderUseQty - info.DomOrderUseQty;
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
