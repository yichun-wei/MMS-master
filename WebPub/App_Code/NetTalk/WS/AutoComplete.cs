using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing.NetTalk.WebSerices
{
    /// <summary>
    /// 關鍵字自動完成。
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class AutoComplete : System.Web.Services.WebService
    {
        public AutoComplete()
        {

        }

        #region 取得 ERP 庫存品項清單
        /// <summary>
        /// 取得 ERP 庫存品項清單。
        /// </summary>
        [WebMethod()]
        public string[] GetGoodsList(string prefixText, int count, string contextKey)
        {
            if (string.IsNullOrWhiteSpace(prefixText))
            {
                return new string[0];
            }

            var jConds = (Newtonsoft.Json.Linq.JObject)CustJson.DeserializeObject(contextKey);

            //1:一般品項 2:專案報價品項
            int? source = ConvertLib.ToInt(jConds["Source"].ToString(), DefVal.Int);

            Returner returner = null;
            try
            {
                #region 一般品項
                if (source == 1)
                {
                    ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                    string keyword = prefixText;
                    List<string> keywordCols = new List<string>();
                    keywordCols.Add("ITEM");
                    //keywordCols.Add("DESCRIPTION");

                    var conds = new ErpInv.InfoConds
                        (
                           DefVal.SIds,
                           DefVal.Longs,
                           DefVal.Strs,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Bool
                        );

                    //returner = entityErpInv.GetInfoByCompoundSearch(conds, keywordCols.ToArray(), keyword, false, count, SqlOrder.Default, IncludeScope.All);
                    returner = entityErpInv.GetInfoByCompoundSearch(conds, keywordCols.ToArray(), keyword, SqlLikeMode.RelativeRight, SqlLikeOperator.Like, count, SqlOrder.Default, IncludeScope.All);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = ErpInv.Info.Binding(returner.DataSet.Tables[0]);

                        return infos.Select(q => new { Val = CustJson.SerializeObject(new { First = string.Format("{0}", q.Item, q.Description), Second = new { GoodsItem = q.Item } }) }).Take(count).Select(q => q.Val).ToArray();
                    }
                }
                #endregion

                #region 專案報價品項
                if (source == 2)
                {
                    ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

                    string keyword = prefixText;
                    List<string> keywordCols = new List<string>();
                    keywordCols.Add("PRODUCTID");
                    //keywordCols.Add("SUMMARY");

                    var conds = new ProjQuote.InfoViewConds
                        (
                           ConvertLib.ToStrs(jConds["QuoteNo"].ToString()),
                           DefVal.Strs,
                           jConds["CustomerId"].ToString()
                        );

                    returner = entityProjQuote.GetInfoViewByCompoundSearch(conds, keywordCols.ToArray(), keyword, false, count, SqlOrder.Default);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0]);

                        return infos.Select(q => new { Val = CustJson.SerializeObject(new { First = string.Format("{0}", q.ProductId), Second = new { GoodsItem = q.QuoteItemId } }) }).Take(count).Select(q => q.Val).ToArray();
                    }
                }
                #endregion

                return new string[0];
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion

        #region 取得外銷商品品項清單
        /// <summary>
        /// 取得外銷商品品項清單。
        /// </summary>
        [WebMethod()]
        public string[] GetExtGoodsList(string prefixText, int count, string contextKey)
        {
            if (string.IsNullOrWhiteSpace(prefixText))
            {
                return new string[0];
            }

            var jConds = (Newtonsoft.Json.Linq.JObject)CustJson.DeserializeObject(contextKey);

            ////1:一般品項 2:專案報價品項
            //int? source = ConvertLib.ToInt(jConds["Source"].ToString(), DefVal.Int);

            Returner returner = null;
            try
            {
                #region 一般品項
                ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);

                string keyword = prefixText;
                List<string> keywordCols = new List<string>();
                //keywordCols.Add("EXPORT_ITEM");
                keywordCols.Add("ERP_ITEM");

                var conds = new ExtItemDetails.InfoConds
                    (
                       DefVal.Strs,
                       DefVal.Str,
                       DefVal.Str,
                       DefVal.Str,
                       DefVal.Str,
                       DefVal.Str,
                       DefVal.Str,
                       true,
                       Convert.ToBoolean(jConds["Source"]) ? true : DefVal.Bool
                    );

                returner = entityExtItemDetails.GetInfoByCompoundSearch(conds, keywordCols.ToArray(), keyword, SqlLikeMode.RelativeRight, SqlLikeOperator.Like, count, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);

                    return infos.Select(q => new { Val = CustJson.SerializeObject(new { First = string.Format("{0}", q.ErpItem), Second = new { GoodsItem = string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(q.ExportItem), q.ErpItem) } }) }).Take(count).Select(q => q.Val).ToArray();
                }
                #endregion

                return new string[0];
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
        #endregion
    }
}
