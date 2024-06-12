using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using EzCoding;
using EzCoding.DB;
using EzCoding.Web.UI;
using Seec.Marketing.SystemEngines;
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

namespace Seec.Marketing
{
    /// <summary>
    /// 排程。
    /// </summary>
    public partial class Schedule
    {
        /// <summary>
        /// 匯入 ERP 折扣。
        /// </summary>
        /// <param name="allImport">是否匯入全部資料。若為 true，則會先清空後重新匯入。</param>
        /// <returns>更新的筆數。</returns>
        public static int? ImportErpDct(bool allImport)
        {
            string eventMsg = string.Empty;
            string eventSubmsg = string.Empty;
            string catOfSysLog = "ERP 排程匯入";
            string srcOfSysLog = "匯入 ERP 折扣";
            string titleOfSysLog = string.Empty;

            Returner returner = null;
            try
            {
                eventMsg = string.Format("匯入方式: {0}", allImport ? "清空重匯" : "更新匯入");
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "排程初始", eventMsg, WebUtilBox.GetUserHostAddress());

                TalkResponse talkResp;

                var entityErpDct = new ErpDct(SystemDefine.ConnInfo);

                DateTime? afterTime = DefVal.DT;

                if (allImport)
                {
                    talkResp = ErpAgentRef.GetDiscountsInfo(DefVal.DT);
                }
                else
                {
                    #region 取得更新的基準時間
                    returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Longs, DefVal.Ints), 1, new SqlOrder("LAST_UPDATE_DATE", Sort.Descending), IncludeScope.All, new string[] { "LAST_UPDATE_DATE" });
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ErpDct.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                        afterTime = info.LastUpdateDate.AddSeconds(1); //加 1 秒
                    }
                    #endregion

                    talkResp = ErpAgentRef.GetDiscountsInfo(afterTime);
                }

                eventSubmsg = string.Format("{1}{0}介接回覆代碼: {2}{0}介接回覆描述: {3}", Environment.NewLine, eventMsg, talkResp.Code, talkResp.Desc);
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "ERP Agent 介接", eventSubmsg, WebUtilBox.GetUserHostAddress());

                if ("0000".Equals(talkResp.Code))
                {
                    IErpDct[] erpErpDctInfos = CustJson.DeserializeObject<ErpDct.Info[]>(talkResp.JsonObj);

                    if (!allImport)
                    {
                        //若資料庫中沒有任何資料, 則直接匯入.
                        returner = entityErpDct.GetInfo(1, SqlOrder.Clear);
                        allImport = !returner.IsCompletedAndContinue;
                    }

                    if (allImport)
                    {
                        //清空全部資料
                        entityErpDct.Delete();

                        #region 開始匯入
                        foreach (var info in erpErpDctInfos)
                        {
                            entityErpDct.Add(SystemId.Empty, new SystemId(), true, info.DiscountId, info.DiscountName, info.AutomaticDiscountFlag, info.OverrideAllowedFlag, info.PriceListId, info.ListName, info.DiscountType, info.LastUpdateDate);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 開始匯入
                        if (erpErpDctInfos.Length > 0)
                        {
                            //取得已存在的「料號 ID」
                            List<long> existingDiscountsItemIds = new List<long>();
                            returner = entityErpDct.GetInfo(new ErpDct.InfoConds(DefVal.SIds, erpErpDctInfos.Select(q => q.DiscountId).ToArray(), DefVal.Longs, DefVal.Ints), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All, new string[] { "DISCOUNT_ID" });
                            if (returner.IsCompletedAndContinue)
                            {
                                var rows = returner.DataSet.Tables[0].Rows;
                                foreach (DataRow row in rows)
                                {
                                    existingDiscountsItemIds.Add(Convert.ToInt64(row["DISCOUNT_ID"]));
                                }
                            }

                            foreach (var info in erpErpDctInfos)
                            {
                                if (existingDiscountsItemIds.Contains(info.DiscountId))
                                {
                                    #region 已存在則修改
                                    entityErpDct.Modify(SystemId.Empty, info.DiscountId, true, info.DiscountName, info.AutomaticDiscountFlag, info.OverrideAllowedFlag, info.PriceListId, info.ListName, info.DiscountType, info.LastUpdateDate);
                                    #endregion
                                }
                                else
                                {
                                    #region 不存在則新增
                                    entityErpDct.Add(SystemId.Empty, new SystemId(), true, info.DiscountId, info.DiscountName, info.AutomaticDiscountFlag, info.OverrideAllowedFlag, info.PriceListId, info.ListName, info.DiscountType, info.LastUpdateDate);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                    eventSubmsg = string.Format("{1}{0}共更新 {0} 筆資料", Environment.NewLine, eventMsg, erpErpDctInfos.Length);
                    WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "匯入完成", eventSubmsg, WebUtilBox.GetUserHostAddress());

                    return erpErpDctInfos.Length;
                }
                else
                {
                    eventSubmsg = string.Format("{1}{0}錯誤代碼: {2}", Environment.NewLine, eventMsg, talkResp.Code);
                    WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Warning, "介接失敗", eventSubmsg, WebUtilBox.GetUserHostAddress());

                    return DefVal.Int;
                }
            }
            catch (Exception ex)
            {
                eventSubmsg = string.Format("{1}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg, WebUtilBox.GetUserHostAddress());

                return DefVal.Int;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
        }
    }
}
