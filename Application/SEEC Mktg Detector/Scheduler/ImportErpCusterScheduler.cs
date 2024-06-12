using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

using EzCoding;
using EzCoding.DB;
using Seec.Marketing.SystemEngines;
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

namespace Seec.Marketing
{
    /// <summary>
    /// ERP 客戶資料轉入排程器。
    /// </summary>
    public class ImportErpCusterScheduler : Scheduler
    {
        public delegate void ExecuteDelegate();
        IMessageLog _messageLog;
        bool _running = false;
        IAsyncResult _asyncResult = null;

        #region 建構子
        /// <summary>
        /// 以指定的時間間隔排程方式，初始化 ImportErpCusterScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="interval">間隔時間 (分鐘)。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpCusterScheduler(string name, int interval, IMessageLog messageLog)
            : base(name, interval)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 ImportErpCusterScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpCusterScheduler(string name, TimeSpan[] times, IMessageLog messageLog)
            : base(name, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 ImportErpCusterScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="dayOfWeeks">指定的星期集合陣列。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpCusterScheduler(string name, DayOfWeek[] dayOfWeeks, TimeSpan[] times, IMessageLog messageLog)
            : base(name, dayOfWeeks, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一個月中的哪幾天和時間的排程方式，初始化 ImportErpCusterScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpCusterScheduler(string name, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
            : base(name, executeDaysOfMonth, executeTimes)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一年中的哪幾月、該月中的哪幾天和時間的排程方式，初始化 ImportErpCusterScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeMonthsOfYear">指定的排程執行每年中的哪幾個月集合陣列。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpCusterScheduler(string name, int[] executeMonthsOfYear, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
            : base(name, executeMonthsOfYear, executeDaysOfMonth, executeTimes)
        {
            this._messageLog = messageLog;
        }
        #endregion

        #region 開始執行排程
        #region Execute
        /// <summary>
        /// 開始執行排程。
        /// </summary>
        public override void Execute()
        {
            if (this._running)
            {
                this._messageLog.AddMessageLog(base.Name, "前次執行中尚未結束，略過此次執行。", this._messageLog.MessageTextBox);
            }
            else
            {
                this._running = true;
                ExecuteDelegate execute = new ExecuteDelegate(ExecuteSchedule);
                AsyncCallback callback = new AsyncCallback(FinishedCallback);
                this._asyncResult = execute.BeginInvoke(callback, execute);
            }
        }
        #endregion

        #region IsRunning
        /// <summary>
        /// 檢查排程器是否尚在執行中。
        /// </summary>
        /// <returns>若執行中回傳 true；否則回傳 false。</returns>
        public override bool IsRunning
        {
            get { return this._running; }
        }
        #endregion

        #region ExecuteSchedule
        protected void ExecuteSchedule()
        {
            #region 排程內容
            this._messageLog.AddMessageLog(base.Name, "開始執行排程操作", this._messageLog.MessageTextBox);

            Encoding encoding = Encoding.UTF8;

            Returner returner = null;
            try
            {
                ISystemId actorSId = SystemId.Empty;
                TalkResponse talkResp;

                //是否匯入全部資料. 若為 true, 則會先清空後重新匯入.
                bool allImport = false;
                //指定時間後的資料（若為 null 則表示全部）
                DateTime? afterTime = DefVal.DT;

                var entityErpCuster = new ErpCuster(SystemDefine.ConnInfo);

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

                if ("0000".Equals(talkResp.Code))
                {
                    this._messageLog.AddMessageLog(base.Name, "已成功介接取得資料", this._messageLog.MessageTextBox);
                    
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

                            entityErpCuster.Add(actorSId, new SystemId(), true, info.CustomerId, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
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

                                    entityErpCuster.Modify(actorSId, info.CustomerId, true, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
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

                                    entityErpCuster.Add(actorSId, new SystemId(), true, info.CustomerId, info.LastUpdateDate, info.CustomerNumber, info.CustomerName, info.ShipAddressId, info.ShipAddress1, info.BillAddressId, info.BillAddress1, info.CustomerCategoryCode, info.Meaning, info.ShipToSiteUseId, info.InvoiceToSiteUseId, info.OrderTypeId, info.TypeName, info.PriceListId, info.CurrencyCode, info.SalesRepId, info.SalesName, info.AreaCode, info.Phone, info.ContactId, info.ConName, info.Fax, mktgRange, areaFilter);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                    this._messageLog.AddMessageLog(base.Name, string.Format("共更新 {0} 筆資料", erpErpCusterInfos.Length), this._messageLog.MessageTextBox);
                }
                else
                {
                    this._messageLog.AddMessageLog(base.Name, string.Format("錯誤代碼: {0}", talkResp.Code), this._messageLog.MessageTextBox);
                }
            }
            catch (Exception ex)
            {
                this._messageLog.AddMessageLog(base.Name, string.Format("其他例外「{0}」", ex.Message), this._messageLog.MessageTextBox);
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
            }
            #endregion
        }
        #endregion

        #region FinishedCallback
        protected void FinishedCallback(IAsyncResult asyncResult)
        {
            if (asyncResult.IsCompleted)
            {
                this._messageLog.AddMessageLog(base.Name, "排程執行完畢", this._messageLog.MessageTextBox);

                this._running = false;
            }
        }
        #endregion
        #endregion
    }
}
