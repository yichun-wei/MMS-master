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
        /// 匯入 ERP 客戶。
        /// </summary>
        /// <param name="allImport">是否匯入全部資料。若為 true，則會先清空後重新匯入。</param>
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        /// <returns>更新的筆數。</returns>
        public static int? ImportErpCuster(bool allImport)
        {
            string eventMsg = string.Empty;
            string eventSubmsg = string.Empty;
            string catOfSysLog = "ERP 排程匯入";
            string srcOfSysLog = "匯入 ERP 客戶";
            string titleOfSysLog = string.Empty;

            Returner returner = null;
            try
            {
                eventMsg = string.Format("匯入方式: {0}", allImport ? "清空重匯" : "更新匯入");
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "排程初始", eventMsg, WebUtilBox.GetUserHostAddress());

                TalkResponse talkResp;

                var entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);

                DateTime? afterTime = DefVal.DT;

                if (allImport)
                {
                    talkResp = ErpAgentRef.GetCustomersInfo(DefVal.DT);
                }
                else
                {
                    #region 取得更新的基準時間
                    returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, DefVal.Longs, DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), 1, new SqlOrder("LAST_UPDATE_DATE", Sort.Descending), IncludeScope.All, new string[] { "LAST_UPDATE_DATE" });
                    if (returner.IsCompletedAndContinue)
                    {
                        var info = ErpCuster.Info.Binding(returner.DataSet.Tables[0].Rows[0]);
                        afterTime = info.LastUpdateDate.AddSeconds(1); //加 1 秒
                    }
                    #endregion

                    talkResp = ErpAgentRef.GetCustomersInfo(afterTime);
                }

                eventSubmsg = string.Format("{1}{0}介接回覆代碼: {2}{0}介接回覆描述: {3}", Environment.NewLine, eventMsg, talkResp.Code, talkResp.Desc);
                WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "ERP Agent 介接", eventSubmsg, WebUtilBox.GetUserHostAddress());

                if ("0000".Equals(talkResp.Code))
                {
                    IErpCuster[] erpErpCusterInfos = CustJson.DeserializeObject<ErpCuster.Info[]>(talkResp.JsonObj);

                    if (!allImport)
                    {
                        //若資料庫中沒有任何資料, 則直接匯入.
                        returner = entityErpCuster.GetInfo(1, SqlOrder.Clear);
                        allImport = !returner.IsCompletedAndContinue;
                    }

                    if (allImport)
                    {
                        //清空全部資料
                        entityErpCuster.Delete();

                        #region 開始匯入
                        foreach (var info in erpErpCusterInfos)
                        {
                            int mktgRange = 0;
                            string areaFilter = string.Empty;

                            if (!string.IsNullOrWhiteSpace(info.Meaning))
                            {
                                var kwIdx = info.Meaning.IndexOf("分公司");
                                if (kwIdx != -1)
                                {
                                    mktgRange = 1;
                                    areaFilter = info.Meaning.Substring(0, kwIdx);
                                }
                                else if (info.Meaning.IndexOf("外銷") != -1)
                                {
                                    mktgRange = 2;
                                    areaFilter = "外銷";
                                }
                            }

                            entityErpCuster.Add(SystemId.Empty, new SystemId(), true, info.CustomerId, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 開始匯入
                        if (erpErpCusterInfos.Length > 0)
                        {
                            //取得已存在的「客戶 ID」
                            List<long> existingCustomerIds = new List<long>();
                            returner = entityErpCuster.GetInfo(new ErpCuster.InfoConds(DefVal.SIds, erpErpCusterInfos.Select(q => q.CustomerId).ToArray(), DefVal.Longs, DefVal.Ints, DefVal.Strs, DefVal.Str), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All, new string[] { "CUSTOMER_ID" });
                            if (returner.IsCompletedAndContinue)
                            {
                                var rows = returner.DataSet.Tables[0].Rows;
                                foreach (DataRow row in rows)
                                {
                                    existingCustomerIds.Add(Convert.ToInt64(row["CUSTOMER_ID"]));
                                }
                            }

                            foreach (var info in erpErpCusterInfos)
                            {
                                if (existingCustomerIds.Contains(info.CustomerId))
                                {
                                    #region 已存在則修改
                                    int mktgRange = 0;
                                    string areaFilter = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(info.Meaning))
                                    {
                                        var kwIdx = info.Meaning.IndexOf("分公司");
                                        if (kwIdx != -1)
                                        {
                                            mktgRange = 1;
                                            areaFilter = info.Meaning.Substring(0, kwIdx);
                                        }
                                        else if (info.Meaning.IndexOf("外銷") != -1)
                                        {
                                            mktgRange = 2;
                                            areaFilter = "外銷";
                                        }
                                    }

                                    entityErpCuster.Modify(SystemId.Empty, info.CustomerId, true, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
                                    #endregion
                                }
                                else
                                {
                                    #region 不存在則新增
                                    int mktgRange = 0;
                                    string areaFilter = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(info.Meaning))
                                    {
                                        var kwIdx = info.Meaning.IndexOf("分公司");
                                        if (kwIdx != -1)
                                        {
                                            mktgRange = 1;
                                            areaFilter = info.Meaning.Substring(0, kwIdx);
                                        }
                                        else if (info.Meaning.IndexOf("外銷") != -1)
                                        {
                                            mktgRange = 2;
                                            areaFilter = "外銷";
                                        }
                                    }

                                    entityErpCuster.Add(SystemId.Empty, new SystemId(), true, info.CustomerId, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                    eventSubmsg = string.Format("{1}{0}共更新 {0} 筆資料", Environment.NewLine, eventMsg, erpErpCusterInfos.Length);
                    WebUtil.WriteSysLog(SystemId.Empty, catOfSysLog, srcOfSysLog, EventType.Notice, "匯入完成", eventSubmsg, WebUtilBox.GetUserHostAddress());

                    return erpErpCusterInfos.Length;
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
