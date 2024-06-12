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
    /// 外銷商品 Helper。
    /// </summary>
    public class ExtItemHelper
    {
        /// <summary>
        /// 空值項目替換值。
        /// </summary>
        public const string EMPTY_ITEM_VALUE = "[---]";

        #region 外銷商品查詢條件分類內容
        /// <summary>
        /// 外銷商品查詢條件分類內容。
        /// </summary>
        public class ExtItemCatConds
        {
            /// <summary>
            /// 初始化 ExtItemHelper.ExtItemCatConds 類別的新執行個體。
            /// </summary>
            public ExtItemCatConds()
            {
                this.ExtItemTypes = new ExtItemType.Info[0];
                this.ExtItemCats = new ExtItemDetails.Info[0];
            }

            /// <summary>
            /// 外銷商品產品別及分類說明陣列集合。
            /// </summary>
            public ExtItemType.Info[] ExtItemTypes { get; set; }
            /// <summary>
            /// 外銷商品分類陣列集合。
            /// </summary>
            public IExtItemCats[] ExtItemCats { get; set; }
        }
        #endregion

        #region 取得外銷型號分類資訊
        /// <summary>
        /// 取得外銷型號分類資訊。
        /// </summary>
        /// <param name="renew">是否重新取得。</param>
        public static ExtItemCatConds GetExtItemCatConds(bool renew)
        {
            /********************************************************************************
             * 避免每次都重取資料庫, 使用快取, 記得使用者的 Session.
             * 不放在 Application 的原因為不知士電何時更新, 不能一直快取, 沒有觸發點知道士電更新資料庫而清空快取.
             * 故而每次第一次開啟查詢頁時, 皆重新取得.
            ********************************************************************************/

            var context = HttpContext.Current;
            ExtItemCatConds extItemCatConds;

            extItemCatConds = context.Session[SessionDefine.ExtItemCatConds] as ExtItemCatConds;
            if (extItemCatConds == null || !renew)
            {
                if (extItemCatConds != null)
                {
                    return extItemCatConds;
                }
            }

            extItemCatConds = new ExtItemCatConds();

            Returner returner = null;
            try
            {
                ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);
                returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(DefVal.Strs, true), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5"));
                if (returner.IsCompletedAndContinue)
                {
                    extItemCatConds.ExtItemTypes = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);
                }

                ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);
                returner = entityExtItemDetails.GetGroupCatInfo();
                if (returner.IsCompletedAndContinue)
                {
                    extItemCatConds.ExtItemCats = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);
                    //分類值改成允許空字串
                    //先將空值改為替換值
                    for (int i = 0; i < extItemCatConds.ExtItemCats.Length; i++)
                    {
                        var cat = extItemCatConds.ExtItemCats[i];
                        if (string.IsNullOrWhiteSpace(cat.Category1))
                        {
                            cat.Category1 = ExtItemHelper.EMPTY_ITEM_VALUE;
                        }

                        if (string.IsNullOrWhiteSpace(cat.Category2))
                        {
                            cat.Category2 = ExtItemHelper.EMPTY_ITEM_VALUE;
                        }

                        if (string.IsNullOrWhiteSpace(cat.Category3))
                        {
                            cat.Category3 = ExtItemHelper.EMPTY_ITEM_VALUE;
                        }

                        if (string.IsNullOrWhiteSpace(cat.Category4))
                        {
                            cat.Category4 = ExtItemHelper.EMPTY_ITEM_VALUE;
                        }

                        if (string.IsNullOrWhiteSpace(cat.Category5))
                        {
                            cat.Category5 = ExtItemHelper.EMPTY_ITEM_VALUE;
                        }
                    }
                }

                context.Session[SessionDefine.ExtItemCatConds] = extItemCatConds;
                return extItemCatConds;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion
    }
}
