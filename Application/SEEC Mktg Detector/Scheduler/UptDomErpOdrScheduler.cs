using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using Seec.Marketing.SystemEngines;
using Seec.Marketing.Erp;
using Seec.Marketing.NetTalk.WebService.Client;

namespace Seec.Marketing
{
    /// <summary>
    /// ERP 內銷訂單狀態更新排程器。
    /// </summary>
    public class UptDomErpOdrScheduler : Scheduler
    {
        public delegate void ExecuteDelegate();
        IMessageLog _messageLog;
        bool _running = false;
        IAsyncResult _asyncResult = null;

        #region 建構子
        /// <summary>
        /// 以指定的時間間隔排程方式，初始化 UptDomErpOdrScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="interval">間隔時間 (分鐘)。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public UptDomErpOdrScheduler(string name, int interval, IMessageLog messageLog)
            : base(name, interval)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 UptDomErpOdrScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public UptDomErpOdrScheduler(string name, TimeSpan[] times, IMessageLog messageLog)
            : base(name, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 UptDomErpOdrScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="dayOfWeeks">指定的星期集合陣列。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public UptDomErpOdrScheduler(string name, DayOfWeek[] dayOfWeeks, TimeSpan[] times, IMessageLog messageLog)
            : base(name, dayOfWeeks, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一個月中的哪幾天和時間的排程方式，初始化 UptDomErpOdrScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public UptDomErpOdrScheduler(string name, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
            : base(name, executeDaysOfMonth, executeTimes)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一年中的哪幾月、該月中的哪幾天和時間的排程方式，初始化 UptDomErpOdrScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeMonthsOfYear">指定的排程執行每年中的哪幾個月集合陣列。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public UptDomErpOdrScheduler(string name, int[] executeMonthsOfYear, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
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

                DomOrder.Info[] domOrderInfos = new DomOrder.Info[0];

                var entityDomOrder = new DomOrder(SystemDefine.ConnInfo);
                returner = entityDomOrder.GetToBeUptErpOdrInfo();
                if (returner.IsCompletedAndContinue)
                {
                    domOrderInfos = DomOrder.Info.Binding(returner.DataSet.Tables[0]);
                }

                if (domOrderInfos.Length == 0)
                {
                    this._messageLog.AddMessageLog(base.Name, "沒有需要更新的內銷訂單", this._messageLog.MessageTextBox);
                    return;
                }

                talkResp = ErpAgentRef.GetErpOrderInfo(DefVal.Longs, domOrderInfos.Select(q => q.OdrNo).ToArray(), DefVal.Ints);

                if ("0000".Equals(talkResp.Code))
                {
                    this._messageLog.AddMessageLog(base.Name, "已成功介接取得資料", this._messageLog.MessageTextBox);

                    var erpOrderInfos = CustJson.DeserializeObject<ErpBaseHelper.ErpOrderInfo[]>(talkResp.JsonObj);

                    //1:已輸入 2:已登錄 3:已超額 4:超額已核發 5:已取消 6:已關閉
                    SimpleDataSet<int, string> sdsErpStatuses = new SimpleDataSet<int, string>();
                    sdsErpStatuses.Add(1, "已輸入");
                    sdsErpStatuses.Add(2, "已登錄");
                    sdsErpStatuses.Add(3, "已超額");
                    sdsErpStatuses.Add(4, "超額已核發");
                    sdsErpStatuses.Add(5, "已取消");
                    sdsErpStatuses.Add(6, "已關閉");

                    int uptCnt = 0;
                    foreach (var domOrderInfo in domOrderInfos)
                    {
                        //ERP 訂單相同訂單編號只會有一筆
                        var erpOrderInfo = erpOrderInfos.Where(q => q.OriginalSystemReference == domOrderInfo.OdrNo).DefaultIfEmpty(null).SingleOrDefault();
                        if (erpOrderInfo == null)
                        {
                            this._messageLog.AddMessageLog(base.Name, string.Format("訂單編號 {0} 未取得更新", domOrderInfo.OdrNo), this._messageLog.MessageTextBox);
                            continue;
                        }

                        if (!erpOrderInfo.OrderNumber.HasValue)
                        {
                            this._messageLog.AddMessageLog(base.Name, string.Format("訂單編號 {0} 無值的「ERP 訂單號碼」(已略過)", domOrderInfo.OdrNo), this._messageLog.MessageTextBox);
                            continue;
                        }

                        var sdErpStatus = sdsErpStatuses.FindByValue(erpOrderInfo.OrderStatus);
                        if (sdErpStatus == null)
                        {
                            this._messageLog.AddMessageLog(base.Name, string.Format("訂單編號 {0} 錯誤的 ERP 訂單狀態「{1}」(已略過)", domOrderInfo.OdrNo, erpOrderInfo.OrderStatus), this._messageLog.MessageTextBox);
                            continue;
                        }

                        if (domOrderInfo.ErpStatus != sdErpStatus.Key)
                        {
                            //更改訂單資訊
                            entityDomOrder.UpdateErpInfo(actorSId, domOrderInfo.SId, erpOrderInfo.HeaderId, erpOrderInfo.OrderNumber.Value.ToString(), sdErpStatus.Key, erpOrderInfo.ShipNumber);

                            //0:草稿 1:營管部待審核 2:ERP上傳中 3:財務待審核 4:未付款待審核 5:待列印 6:已列印 7:備貨中 8:已出貨
                            //只有在訂單狀態為「2:ERP上傳中」時, 才要更新訂單狀態.
                            if (domOrderInfo.Status == 2)
                            {
                                //更改訂單狀態
                                entityDomOrder.UpdateStatus(actorSId, domOrderInfo.SId, 3);
                            }

                            uptCnt++;
                            this._messageLog.AddMessageLog(base.Name, string.Format("訂單編號 {0} ERP 訂單狀態已更新", domOrderInfo.OdrNo), this._messageLog.MessageTextBox);
                        }
                    }

                    this._messageLog.AddMessageLog(base.Name, string.Format("共更新 {0}/{1} 筆資料", uptCnt, domOrderInfos.Length), this._messageLog.MessageTextBox);
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
