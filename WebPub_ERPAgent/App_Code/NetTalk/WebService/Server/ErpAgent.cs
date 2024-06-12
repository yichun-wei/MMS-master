using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using EzCoding;
using EzCoding.Web.UI;
using Seec.Marketing.Erp;
using Seec.Marketing.Erp.SystemEngines;

namespace Seec.Marketing.NetTalk.WebService.Server
{
    /// <summary>
    /// ERP Agent。
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ErpAgent : System.Web.Services.WebService
    {
        /// <summary>
        /// 初始化 Seec.Marketing.NetTalk.WebService.ErpAgent 類別的新執行個體。
        /// </summary>
        public ErpAgent()
        {

        }

        /// <summary>
        /// 私密金鑰。
        /// </summary>
        static string PRIVATE_KEY
        {
            get { return SystemDefine.ErpAgentPrivateKey; }
        }

        #region 轉換為 Json 格式的介接回應
        /// <summary>
        /// 轉換為 Json 格式的介接回應。
        /// </summary>
        /// <param name="code">回應代碼。</param>
        /// <param name="desc">回應說明。</param>
        /// <returns>Json 格式的介接回應。</returns>
        static string ToTalkResponse(string code, string desc)
        {
            return ErpAgent.ToTalkResponse(code, desc, string.Empty);
        }

        /// <summary>
        /// 轉換為 Json 格式的介接回應。
        /// </summary>
        /// <param name="code">回應代碼。</param>
        /// <param name="desc">回應說明。</param>
        /// <param name="jsonObj">回應的 JSON 物件字串（若需要的話）。</param>
        /// <returns>Json 格式的介接回應。</returns>
        static string ToTalkResponse(string code, string desc, string jsonObj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new TalkResponse() { Code = code, Desc = desc, JsonObj = jsonObj });
        }
        #endregion

        #region NetTalkChecker
        /// <summary>
        /// 介接驗證資訊。
        /// </summary>
        public class NetTalkChecker
        {
            /// <summary>
            /// 初始化 Seec.Marketing.NetTalk.WebService.NetTalkChecker 類別的新執行個體。
            /// </summary>
            public NetTalkChecker()
            {
                this.Message = string.Empty;
            }

            /// <summary>
            /// 驗證結果代碼（0:驗證通過 1:雜湊值不符合 9:必要值為空值）。
            /// </summary>
            public int Code { get; set; }
            /// <summary>
            /// 驗證結果訊息。
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 傳回拋回叫用端的錯誤訊息。
            /// 若為空字串則表示驗證通過。
            /// </summary>
            /// <returns></returns>
            public string ToErrMsg()
            {
                switch (this.Code)
                {
                    case 0:
                        return string.Empty;
                    case 1:
                        return ErpAgent.ToTalkResponse("900", "未授權使用");
                    case 9:
                        return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                    default:
                        return ErpAgent.ToTalkResponse("999", "發生未預期的結果");
                }
            }
        }
        #endregion

        #region ApiSecurity
        /// <summary>
        /// API 介接安全相關。
        /// </summary>
        public static class ApiSecurity
        {
            #region 驗證雜湊值
            /// <summary>
            /// 驗證雜湊值。
            /// </summary>
            /// <param name="talkVal">介接值。</param>
            /// <param name="hashVal">雜湊值。</param>
            /// <returns>介接驗證資訊。</returns>
            public static NetTalkChecker CheckHashSumWithoutAcct(string talkVal, string hashVal)
            {
                NetTalkChecker checker = new NetTalkChecker();

                if (string.IsNullOrEmpty(talkVal) || string.IsNullOrEmpty(hashVal))
                {
                    //理論上在進來前都要先檢查, 這裡保險起見.
                    checker.Code = 9;
                    return checker;
                }

                string checkHashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", PRIVATE_KEY, talkVal));
                if (hashVal.Equals(checkHashVal, StringComparison.OrdinalIgnoreCase))
                {
                    checker.Code = 0;
                    return checker;
                }
                else
                {
                    checker.Code = 1;
                    return checker;
                }
            }
            #endregion
        }
        #endregion

        #region ERP 客戶相關
        #region 條件參數
        public class GetCustomersInfoConds
        {
            /// <summary>
            /// 指定時間後的資料（若為 null 則表示全部）。
            /// </summary>
            public DateTime? AfterTime { get; set; }
        }
        #endregion

        #region GetCustomersInfo
        /// <summary>
        /// 取得 ERP 客戶資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCustomersInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetCustomersInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetCustomersInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCustomersInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    Customers entityCustomers = new Customers(SystemDefine.ConnInfoErp);
                    returner = entityCustomers.GetSyncInfo(conds.AfterTime);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(Customers.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new Customers[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 倉庫相關
        #region 條件參數
        public class GetWarehouseInfoConds
        {
            /// <summary>
            /// 市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] MktgRanges { get; set; }
        }
        #endregion

        #region GetWarehouseInfo
        /// <summary>
        /// 取得 ERP 倉庫資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWarehouseInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetWarehouseInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                //「jsonConds」雖然未定義條件, 但仍需傳「{}」.
                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetWarehouseInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetWarehouseInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    Warehouse entityWarehouse = new Warehouse(SystemDefine.ConnInfoErp);
                    returner = entityWarehouse.GetSyncInfo(conds.MktgRanges);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(Warehouse.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new Warehouse[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 庫存相關
        #region 條件參數
        public class GetInventoryInfoConds
        {
            /// <summary>
            /// 指定時間後的資料（若為 null 則表示全部）。
            /// </summary>
            public DateTime? AfterTime { get; set; }
        }
        #endregion

        #region GetInventoryInfo
        /// <summary>
        /// 取得 ERP 庫存資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetInventoryInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetInventoryInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetInventoryInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetInventoryInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    Inventory entityInventory = new Inventory(SystemDefine.ConnInfoErp);
                    returner = entityInventory.GetSyncInfo(conds.AfterTime);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(Inventory.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new Inventory[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 倉庫在手量相關
        #region 條件參數
        public class GetOnHandInfoConds
        {
            /// <summary>
            /// 料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] InventoryItemIds { get; set; }
            /// <summary>
            /// 料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] Items { get; set; }
            /// <summary>
            /// 倉庫。
            /// </summary>
            public string Whse { get; set; }
        }
        #endregion

        #region GetOnHandInfo
        /// <summary>
        /// 取得 ERP 倉庫在手量資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOnHandInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetOnHandInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetOnHandInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetOnHandInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    OnHand entityOnHand = new OnHand(SystemDefine.ConnInfoErp);
                    returner = entityOnHand.GetInfo(conds.InventoryItemIds, conds.Items, conds.Whse);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(OnHand.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new OnHand[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 價目表相關
        #region 條件參數
        public class GetPriceBookInfoConds
        {
            /// <summary>
            /// 價目表 ID（若為 null 則表示全部）。
            /// </summary>
            public long? PriceListId { get; set; }
            /// <summary>
            /// 料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] InventoryItemIds { get; set; }
            /// <summary>
            /// 料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] Items { get; set; }
        }
        #endregion

        #region GetPriceBookInfo
        /// <summary>
        /// 取得 ERP 價目表資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPriceBookInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetPriceBookInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetPriceBookInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetPriceBookInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    PriceBook entityPriceBook = new PriceBook(SystemDefine.ConnInfoErp);
                    returner = entityPriceBook.GetInfo(conds.PriceListId, conds.InventoryItemIds, conds.Items);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(PriceBook.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new PriceBook[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 幣別表相關
        #region 條件參數
        public class GetCurrencyBookInfoConds
        {
            /// <summary>
            /// 價目表 ID（若為 null 則表示全部）。
            /// </summary>
            public long? PriceListId { get; set; }
            /// <summary>
            /// 幣別（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string CurrenctCode { get; set; }
        }
        #endregion

        #region GetCurrencyBookInfo
        /// <summary>
        /// 取得 ERP 幣別表資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCurrencyBookInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetCurrencyBookInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetCurrencyBookInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurrencyBookInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    CurrencyBook entityCurrencyBook = new CurrencyBook(SystemDefine.ConnInfoErp);
                    returner = entityCurrencyBook.GetInfo(conds.PriceListId, conds.CurrenctCode);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(CurrencyBook.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new CurrencyBook[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 折扣相關
        #region 條件參數
        public class GetDiscountsInfoConds
        {
            /// <summary>
            /// 指定時間後的資料（若為 null 則表示全部）。
            /// </summary>
            public DateTime? AfterTime { get; set; }
        }
        #endregion

        #region GetDiscountsInfo
        /// <summary>
        /// 取得 ERP 折扣資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDiscountsInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetDiscountsInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetDiscountsInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetDiscountsInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    Discounts entityDiscounts = new Discounts(SystemDefine.ConnInfoErp);
                    returner = entityDiscounts.GetSyncInfo(conds.AfterTime);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(Discounts.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new Discounts[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region 上傳 ERP 相關
        #region 條件參數
        public class UploadErpInputted
        {
            /// <summary>
            /// SO-HEADERS-INTERFACE-ALL。
            /// </summary>
            public SOHeadersInterfaceAll.Info SOHeadersInterfaceAll { get; set; }
            /// <summary>
            /// SO-LINES-INTERFACE-ALL。
            /// </summary>
            public SOLinesInterfaceAll.Info[] SOLinesInterfaceAllList { get; set; }
            /// <summary>
            /// SO-LINE-DETAILS-INTERFACE。
            /// </summary>
            public SOLineDetailsInterface.Info[] SOLineDetailsInterfaceList { get; set; }
            /// <summary>
            /// SO-PRICE-ADJUSTMENTS-INTERFACE。
            /// </summary>
            public SOPriceAdjustmentsInterface.Info[] SOPriceAdjustmentsInterfaceList { get; set; }
        }
        #endregion

        #region UploadErp
        /// <summary>
        /// 上傳 ERP。
        /// </summary>
        /// <param name="jsonInput">輸入參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UploadErp(string jsonInput, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 201  「內銷訂單號碼」或「外銷交貨單號碼」不一致
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "UploadErp";

                eventMsg = string.Format("hashVal: {1}{0}jsonInput: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonInput) ? jsonInput : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonInput))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                UploadErpInputted inputted;

                try
                {
                    inputted = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadErpInputted>(jsonInput);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                //檢查「內銷訂單號碼」或「外銷交貨單號碼」是否為空值
                if (
                    (inputted.SOHeadersInterfaceAll == null || string.IsNullOrEmpty(inputted.SOHeadersInterfaceAll.OriginalSystemReference))
                    || (inputted.SOLinesInterfaceAllList == null || inputted.SOLinesInterfaceAllList.Where(q => string.IsNullOrEmpty(q.OriginalSystemReference)).Count() > 0)
                    || (inputted.SOLineDetailsInterfaceList == null || inputted.SOLineDetailsInterfaceList.Where(q => string.IsNullOrEmpty(q.OriginalSystemReference)).Count() > 0)
                    || (inputted.SOPriceAdjustmentsInterfaceList == null || inputted.SOPriceAdjustmentsInterfaceList.Where(q => string.IsNullOrEmpty(q.OriginalSystemReference)).Count() > 0)
                   )
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                var originalSystemReference = inputted.SOHeadersInterfaceAll.OriginalSystemReference;

                //檢查「內銷訂單號碼」或「外銷交貨單號碼」是否一致
                if (
                    inputted.SOLinesInterfaceAllList.Where(q => q.OriginalSystemReference != originalSystemReference).Count() > 0
                    || inputted.SOLineDetailsInterfaceList.Where(q => q.OriginalSystemReference != originalSystemReference).Count() > 0
                    || inputted.SOPriceAdjustmentsInterfaceList.Where(q => q.OriginalSystemReference != originalSystemReference).Count() > 0
                   )
                {
                    return ErpAgent.ToTalkResponse("201", "「內銷訂單號碼」或「外銷交貨單號碼」不一致");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonInput, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                SOHeadersInterfaceAll entitySOHeadersInterfaceAll = new SOHeadersInterfaceAll(SystemDefine.ConnInfoErp);
                SOLinesInterfaceAll entitySOLinesInterfaceAll = new SOLinesInterfaceAll(SystemDefine.ConnInfoErp);
                SOLineDetailsInterface entitySOLineDetailsInterface = new SOLineDetailsInterface(SystemDefine.ConnInfoErp);
                SOPriceAdjustmentsInterface entitySOPriceAdjustmentsInterface = new SOPriceAdjustmentsInterface(SystemDefine.ConnInfoErp);

                Returner returner = null;
                try
                {

                    foreach (var soLinesInterfaceAllList in inputted.SOLinesInterfaceAllList)
                    {
                        logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Debug, "寫入 SOLinesInterfaceAllList", Newtonsoft.Json.JsonConvert.SerializeObject(soLinesInterfaceAllList)));

                        entitySOLinesInterfaceAll.Add(soLinesInterfaceAllList);
                    }

                    foreach (var soLineDetailsInterface in inputted.SOLineDetailsInterfaceList)
                    {
                        logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Debug, "寫入 SOLineDetailsInterfaceList", Newtonsoft.Json.JsonConvert.SerializeObject(soLineDetailsInterface)));

                        entitySOLineDetailsInterface.Add(soLineDetailsInterface);
                    }

                    foreach (var soPriceAdjustmentsInterface in inputted.SOPriceAdjustmentsInterfaceList)
                    {
                        logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Debug, "寫入 SOPriceAdjustmentsInterfaceList", Newtonsoft.Json.JsonConvert.SerializeObject(soPriceAdjustmentsInterface)));

                        entitySOPriceAdjustmentsInterface.Add(soPriceAdjustmentsInterface);
                    }

                    //主檔上傳移到最後
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Debug, "寫入 SOHeadersInterfaceAll", Newtonsoft.Json.JsonConvert.SerializeObject(inputted.SOHeadersInterfaceAll)));

                    entitySOHeadersInterfaceAll.Add(inputted.SOHeadersInterfaceAll);

                    return ErpAgent.ToTalkResponse("0000", string.Empty);
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    try
                    {
                        //若發生例外, 則刪掉所有已寫入的資料.
                        entitySOHeadersInterfaceAll.DeleteByOriginalSystemReference(originalSystemReference);
                        entitySOLinesInterfaceAll.DeleteByOriginalSystemReference(originalSystemReference);
                        entitySOLineDetailsInterface.DeleteByOriginalSystemReference(originalSystemReference);
                        entitySOPriceAdjustmentsInterface.DeleteByOriginalSystemReference(originalSystemReference);
                    }
                    catch (Exception exDel)
                    {
                        eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, exDel);
                        logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));
                    }

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion

        #region ERP 訂單相關
        #region 條件參數
        public class GetErpOrderInfoConds
        {
            /// <summary>
            /// ERP 訂單 ID（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] HeaderIds { get; set; }
            /// <summary>
            /// XS 營銷訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] OriginalSystemReferences { get; set; }
            /// <summary>
            /// ERP 訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] OrderNumbers { get; set; }
        }
        #endregion

        #region GetErpOrderInfo
        /// <summary>
        /// 取得 ERP 訂單資訊。
        /// </summary>
        /// <param name="jsonConds">條件參數。</param>
        /// <param name="hashVal">雜湊值。</param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetErpOrderInfo(string jsonConds, string hashVal)
        {
            /********************************************************************************
             * [錯誤代碼總覽]
             * 代碼 說明
             * --------------------------------------------------------------------------------
             * 200  必要欄位為空值或格式不正確
             * 900  未授權使用
             * 999  發生未預期的例外
            ********************************************************************************/

            string token = new SystemId().Value;
            string clientIP = WebUtilBox.GetUserHostAddress();
            List<WebUtil.SysFileLog> logs = new List<WebUtil.SysFileLog>();

            try
            {
                #region 前置處理
                string eventMsg = string.Empty;
                string eventSubmsg = string.Empty;
                string catOfSysLog = "NetTalk.WS.ErpAgent";
                string srcOfSysLog = "GetErpOrderInfo";

                eventMsg = string.Format("hashVal: {1}{0}jsonConds: {2}"
                    , Environment.NewLine
                    , !string.IsNullOrEmpty(hashVal) ? hashVal : string.Empty
                    , !string.IsNullOrEmpty(jsonConds) ? jsonConds : string.Empty
                    );

                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接初始", eventMsg));
                #endregion

                if (string.IsNullOrEmpty(hashVal) || string.IsNullOrEmpty(jsonConds))
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                GetErpOrderInfoConds conds;

                try
                {
                    conds = Newtonsoft.Json.JsonConvert.DeserializeObject<GetErpOrderInfoConds>(jsonConds);
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return ErpAgent.ToTalkResponse("200", "必要欄位為空值或格式不正確");
                }

                #region 驗證通關密語
                NetTalkChecker checker = ApiSecurity.CheckHashSumWithoutAcct(jsonConds, hashVal);

                eventSubmsg = string.Format("{1}{0}驗證結果: {2}", Environment.NewLine, eventMsg, Newtonsoft.Json.JsonConvert.SerializeObject(checker));
                logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接驗證", eventSubmsg));

                switch (checker.Code)
                {
                    case 0:
                        break;
                    default:
                        return checker.ToErrMsg();
                }
                #endregion

                Returner returner = null;
                try
                {
                    ErpOrder entityErpOrder = new ErpOrder(SystemDefine.ConnInfoErp);
                    returner = entityErpOrder.GetInfo(conds.HeaderIds, conds.OriginalSystemReferences, conds.OrderNumbers);
                    if (returner.IsCompletedAndContinue)
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(ErpOrder.Info.Binding(returner.DataSet.Tables[0])));
                    }
                    else
                    {
                        return ErpAgent.ToTalkResponse("0000", string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(new ErpOrder[0]));
                    }
                }
                catch (Exception ex)
                {
                    eventSubmsg = string.Format("{1}{0}{0}例外訊息: {0}{2}", Environment.NewLine, eventMsg, ex);
                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Fatal, "其他例外", eventSubmsg));

                    return ErpAgent.ToTalkResponse("999", "發生未預期的例外");
                }
                finally
                {
                    if (returner != null) { returner.Dispose(); }

                    logs.Add(new WebUtil.SysFileLog(catOfSysLog, srcOfSysLog, EventType.Notice, "介接結束", eventMsg));
                }
            }
            finally
            {
                WebUtil.WriteSysFileLog(token, clientIP, logs.ToArray());
            }
        }
        #endregion
        #endregion
    }
}
