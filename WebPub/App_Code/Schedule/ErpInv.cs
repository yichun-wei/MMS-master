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
        /// 匯入 ERP 庫存。
        /// </summary>
        /// <param name="allImport">是否匯入全部資料。若為 true，則會先清空後重新匯入。</param>
        /// <returns>更新的筆數。</returns>
        public static int? ImportErpInv(bool allImport)
        {
            string eventMsg =  string.Empty;
            string eventSubmsg = string.Empty;
            string catOfSysLog = "ERP 排程匯入";
            string srcOfSysLog = "匯入 ERP 庫存";
            string titleOfSysLog = string.Empty;

            Returner returner = null;
            try
            {
                eventMsg = string.Format("匯入方式: {0}", allImport ? "清空重匯" : "更新匯入");
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "排程初始", eventMsg, WebUtilBox.GetUserHostAddress());

                TalkResponse talkResp;

                var entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                DateTime? afterTime = DefVal.DT;

           // for大量更新    allImport = true;

                if (allImport)
                {
                    talkResp = ErpAgentRef.GetInventoryInfo(DefVal.DT);
                }
                else
                {
                    #region 取得更新的基準時間
                    returner = entityErpInv.GetInfo(new ErpInv.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Strs, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Bool), 1, new SqlOrder("LAST_UPDATE_DATE", Sort.Descending), IncludeScope.All, new string[] { "LAST_UPDATE_DATE" });
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ErpInv.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                        afterTime = info.LastUpdateDate.AddSeconds(1); //加 1 秒
                    }
                    #endregion

                    talkResp = ErpAgentRef.GetInventoryInfo(afterTime);
                }

                eventSubmsg = string.Format("{1}{0}介接回覆代碼: {2}{0}介接回覆描述: {3}", Environment.NewLine, eventMsg, talkResp.Code, talkResp.Desc);
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "ERP Agent 介接", eventSubmsg, WebUtilBox.GetUserHostAddress());

                if ("0000".Equals(talkResp.Code))
                {
                    IErpInv[] erpErpInvInfos = CustJson.DeserializeObject<ErpInv.Info[]>(talkResp.JsonObj);

                    if (!allImport)
                    {
                        //若資料庫中沒有任何資料, 則直接匯入.
                        returner = entityErpInv.GetInfo(1, SqlOrder.Clear);
                        allImport = !returner.IsCompletedAndContinue;
                    }

                    if (allImport)
                    {
                        //清空全部資料
                        entityErpInv.Delete();

                        #region 開始匯入
                        foreach (var info in erpErpInvInfos)
                        {
                            entityErpInv.Add(SystemId.Empty, new SystemId(), true, info.InventoryItemId, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 開始匯入
                        if (erpErpInvInfos.Length > 0)
                        {
                            //取得已存在的「料號 ID」
                            List<long> existingInventoryItemIds = new List<long>();
                            returner = entityErpInv.GetInfo(new ErpInv.InfoConds(DefVal.SIds, erpErpInvInfos.Select(q => q.InventoryItemId).ToArray(), DefVal.Strs, DefVal.Str, DefVal.Str, DefVal.Str, DefVal.Bool), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All, new string[] { "INVENTORY_ITEM_ID" });
                            if (returner.IsCompletedAndContinue)
                            {
                                var rows = returner.DataSet.Tables[0].Rows;
                                foreach (DataRow row in rows)
                                {
                                    existingInventoryItemIds.Add(Convert.ToInt64(row["INVENTORY_ITEM_ID"]));
                                }
                            }

                            foreach (var info in erpErpInvInfos)
                            {
                                if (existingInventoryItemIds.Contains(info.InventoryItemId))
                                {
                                    #region 已存在則修改
                                    entityErpInv.Modify(SystemId.Empty, info.InventoryItemId, true, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
                                    #endregion
                                }
                                else
                                {
                                    #region 不存在則新增
                                    entityErpInv.Add(SystemId.Empty, new SystemId(), true, info.InventoryItemId, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                    eventSubmsg = string.Format("{1}{0}共更新 {0} 筆資料", Environment.NewLine, eventMsg, erpErpInvInfos.Length);
                    WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "匯入完成", eventSubmsg, WebUtilBox.GetUserHostAddress());

                    return erpErpInvInfos.Length;
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
