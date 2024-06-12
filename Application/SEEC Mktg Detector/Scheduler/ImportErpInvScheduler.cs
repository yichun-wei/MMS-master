﻿using System;
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
    /// ERP 庫存資料轉入排程器。
    /// </summary>
    public class ImportErpInvScheduler : Scheduler
    {
        public delegate void ExecuteDelegate();
        IMessageLog _messageLog;
        bool _running = false;
        IAsyncResult _asyncResult = null;

        #region 建構子
        /// <summary>
        /// 以指定的時間間隔排程方式，初始化 ImportErpInvScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="interval">間隔時間 (分鐘)。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpInvScheduler(string name, int interval, IMessageLog messageLog)
            : base(name, interval)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 ImportErpInvScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpInvScheduler(string name, TimeSpan[] times, IMessageLog messageLog)
            : base(name, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定時間的排程方式，初始化 ImportErpInvScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="dayOfWeeks">指定的星期集合陣列。</param>
        /// <param name="times">指定的時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpInvScheduler(string name, DayOfWeek[] dayOfWeeks, TimeSpan[] times, IMessageLog messageLog)
            : base(name, dayOfWeeks, times)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一個月中的哪幾天和時間的排程方式，初始化 ImportErpInvScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpInvScheduler(string name, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
            : base(name, executeDaysOfMonth, executeTimes)
        {
            this._messageLog = messageLog;
        }

        /// <summary>
        /// 以指定一年中的哪幾月、該月中的哪幾天和時間的排程方式，初始化 ImportErpInvScheduler 類別的新執行個體。
        /// </summary>
        /// <param name="name">排程器名稱。</param>
        /// <param name="executeMonthsOfYear">指定的排程執行每年中的哪幾個月集合陣列。</param>
        /// <param name="executeDaysOfMonth">指定的排程執行每月中的哪幾天集合陣列。</param>
        /// <param name="executeTimes">指定的排程執行時間集合陣列。</param>
        /// <param name="messageLog">訊息記錄寫入的實作介面。</param>
        public ImportErpInvScheduler(string name, int[] executeMonthsOfYear, int[] executeDaysOfMonth, TimeSpan[] executeTimes, IMessageLog messageLog)
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

                var entityErpInv = new ErpInv(SystemDefine.ConnInfo);

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

                if ("0000".Equals(talkResp.Code))
                {
                    this._messageLog.AddMessageLog(base.Name, "已成功介接取得資料", this._messageLog.MessageTextBox);
                    
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
                            entityErpInv.Add(actorSId, new SystemId(), true, info.InventoryItemId, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
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
                                    entityErpInv.Modify(actorSId, info.InventoryItemId, true, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
                                    #endregion
                                }
                                else
                                {
                                    #region 不存在則新增
                                    entityErpInv.Add(actorSId, new SystemId(), true, info.InventoryItemId, info.Item, info.Description, info.UnitWeight, info.WeightUomCode, info.Discount, info.Segment1, info.Segment2, info.Segment3, info.OrganizationId, info.OrganizationCode, info.EnabledFlag, info.LastUpdateDate, info.XSDItem);
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                    this._messageLog.AddMessageLog(base.Name, string.Format("共更新 {0} 筆資料", erpErpInvInfos.Length), this._messageLog.MessageTextBox);
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
