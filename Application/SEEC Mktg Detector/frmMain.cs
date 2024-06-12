using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EzCoding;

namespace Seec.Marketing
{
    public partial class frmMain : Form, IMessageLog
    {
        bool _running = false;
        const string FORM_TITLE_NAME = "士電營銷排程器";
        List<Scheduler> _schedulers = new List<Scheduler>();

        public frmMain()
        {
            InitializeComponent();

            this.Text = FORM_TITLE_NAME;
            this.lblProcessStatus.Text = "停止的排程服務";

            if (SystemDefine.EnableImportErpCusterScheduler)
            {
                this._schedulers.Add(new ImportErpCusterScheduler("ERP 客戶資料轉入排程器", new TimeSpan[] { SystemDefine.ImportErpCusterSchedulerScanTime }, this));
            }

            if (SystemDefine.EnableImportErpInvScheduler)
            {
                this._schedulers.Add(new ImportErpInvScheduler("ERP 庫存資料轉入排程器", new TimeSpan[] { SystemDefine.ImportErpInvSchedulerScanTime }, this));
            }

            if (SystemDefine.EnableImportErpDctScheduler)
            {
                this._schedulers.Add(new ImportErpDctScheduler("ERP 折扣資料轉入排程器", new TimeSpan[] { SystemDefine.ImportErpDctSchedulerScanTime }, this));
            }

            if (SystemDefine.EnableUptDomErpOdrScheduler)
            {
                this._schedulers.Add(new UptDomErpOdrScheduler("ERP 內銷訂單狀態更新排程器", SystemDefine.UptDomErpOdrSchedulerScanInterval, this));
            }

            if (SystemDefine.EnableUptExtShipOdrScheduler)
            {
                this._schedulers.Add(new UptExtShipOdrScheduler("ERP 外銷出貨單狀態更新排程器", SystemDefine.UptExtShipOdrSchedulerScanInterval, this));
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (SystemDefine.RuningAndStart)
            {
                this.StartService();
            }
            else
            {
                this.toolbarStop.Enabled = false;
            }
        }

        private void toolbarStart_Click(object sender, EventArgs e)
        {
            this.StartService();
        }

        private void toolbarStop_Click(object sender, EventArgs e)
        {
            this.StopService();
        }

        #region 檢視排程器
        private void toolStripRegisteredScheduler_Click(object sender, EventArgs e)
        {
            if (this._schedulers.Count == 0)
            {
                this.AddMessageLog("檢視排程器設定", "沒有設定任何排程器", this.txtMessageLogs);
                return;
            }

            string[] weeks = new string[] { "日", "一", "二", "三", "四", "五", "六" };
            for (int i = 0; i < this._schedulers.Count; i++)
            {
                StringBuilder builder = new StringBuilder();
                if (this._schedulers[i].SchedulingOptions == SchedulingOptions.Interval)
                {
                    builder.AppendFormat("{0}（每間隔 {1} 分鐘執行排程操作）", this._schedulers[i].Name, this._schedulers[i].ExecuteInterval);
                }
                else
                {
                    int[] executeMonthsOfYear = this._schedulers[i].GetExecuteMonthsOfYear();
                    int[] executeDaysOfMonth = this._schedulers[i].GetExecuteDaysOfMonth();
                    DayOfWeek[] executeWeeks = this._schedulers[i].GetExecuteWeeks();
                    TimeSpan[] executeTimes = this._schedulers[i].GetExecuteTimes();
                    List<string> joinStrs;

                    joinStrs = new List<string>();
                    for (int j = 0; j < executeTimes.Length; j++)
                    {
                        joinStrs.Add(string.Format("{0}:{1}", executeTimes[j].Hours.ToString().PadLeft(2, '0'), executeTimes[j].Minutes.ToString().PadLeft(2, '0')));
                    }

                    var execTimes = string.Join("、", joinStrs.ToArray());

                    if ((executeMonthsOfYear == null || executeMonthsOfYear.Length == 0) && (executeDaysOfMonth == null || executeDaysOfMonth.Length == 0) && (executeWeeks == null || executeWeeks.Length == 0))
                    {
                        builder.AppendFormat("{0}（於每天的 {1} 執行排程操作）", this._schedulers[i].Name, execTimes);
                    }
                    else
                    {
                        if (executeMonthsOfYear != null && executeMonthsOfYear.Length > 0 && executeDaysOfMonth != null && executeDaysOfMonth.Length > 0)
                        {
                            #region 每年的某月某日排程
                            joinStrs = new List<string>();
                            for (int j = 0; j < executeMonthsOfYear.Length; j++)
                            {
                                joinStrs.Add(executeMonthsOfYear[j].ToString());
                            }

                            var monthList = string.Join("、", joinStrs.ToArray());

                            joinStrs = new List<string>();
                            for (int j = 0; j < executeDaysOfMonth.Length; j++)
                            {
                                joinStrs.Add(executeDaysOfMonth[j].ToString());
                            }

                            var dayList = string.Join("、", joinStrs.ToArray());

                            builder.AppendFormat("{0}（於每年的 {1} 月中的 {2} 日 {3} 執行排程操作）", this._schedulers[i].Name, monthList, dayList, execTimes);
                            #endregion
                        }
                        else if (executeDaysOfMonth != null && executeDaysOfMonth.Length > 0)
                        {
                            #region 每月的某日排程
                            joinStrs = new List<string>();
                            for (int j = 0; j < executeDaysOfMonth.Length; j++)
                            {
                                joinStrs.Add(executeDaysOfMonth[j].ToString());
                            }

                            var dayList = string.Join("、", joinStrs.ToArray());

                            builder.AppendFormat("{0}（於每月的 {1} 日 {2} 執行排程操作）", this._schedulers[i].Name, dayList, execTimes);
                            #endregion
                        }
                        else if (executeWeeks != null && executeWeeks.Length > 0)
                        {
                            #region 週排程
                            joinStrs = new List<string>();
                            for (int j = 0; j < executeWeeks.Length; j++)
                            {
                                joinStrs.Add(string.Format("星期{0}、", weeks[(int)executeWeeks[j]]));
                            }

                            var weekList = string.Join("、", joinStrs.ToArray());

                            builder.AppendFormat("{0}（於{1}的 {2} 執行排程操作）", this._schedulers[i].Name, weekList, execTimes);
                            #endregion
                        }
                    }
                }

                this.AddMessageLog("檢視排程器設定", builder.ToString(), this.txtMessageLogs);
                Application.DoEvents();
            }
        }
        #endregion

        #region 排程器執行狀態
        private void toolStripCheckRunning_Click(object sender, EventArgs e)
        {
            if (this._schedulers.Count == 0)
            {
                this.AddMessageLog("檢視排程器執行狀態", "沒有設定任何排程器", this.txtMessageLogs);
                return;
            }

            int runningCount = 0;
            for (int i = 0; i < this._schedulers.Count; i++)
            {
                if (this._schedulers[i].IsRunning)
                {
                    runningCount++;
                    this.AddMessageLog("檢視排程器執行狀態", string.Format("{0} 排程器尚在執行中", this._schedulers[i].Name), this.txtMessageLogs);
                    Application.DoEvents();
                }
            }

            if (runningCount == 0)
            {
                this.AddMessageLog("檢視排程器執行狀態", "目前沒有執行中的排程器", this.txtMessageLogs);
            }
        }
        #endregion

        #region StartService
        /// <summary>
        /// 啟動服務。
        /// </summary>
        private void StartService()
        {
            this.toolbarStart.Enabled = false;
            this.toolbarStop.Enabled = !this.toolbarStart.Enabled;

            this.AddMessageLog("系統", "服務啟動", this.txtMessageLogs);

            this.mainTimer.Interval = SystemDefine.ScanInterval * 1000 * 30;
            this.mainTimer.Start();

            this.lblProcessStatus.Text = "排程待命中";

            //啟動時即先檢查一次。
            this.Checking();
        }
        #endregion

        #region StopService
        /// <summary>
        /// 停止服務。
        /// </summary>
        private void StopService()
        {
            this.toolbarStart.Enabled = true;
            this.toolbarStop.Enabled = !this.toolbarStart.Enabled;

            if (this.mainTimer.Enabled)
            {
                this.mainTimer.Stop();

                int runningCount = 0;
                for (int i = 0; i < this._schedulers.Count; i++)
                {
                    if (this._schedulers[i].IsRunning)
                    {
                        runningCount++;
                        Application.DoEvents();
                    }
                }

                if (runningCount > 0)
                {
                    this.AddMessageLog("系統", "尚有執行中的排程，系統並不會自動停止它，請檢視排程器執行狀態，以確保排程器已完成排程操作。", this.txtMessageLogs);
                }
            }
            this.AddMessageLog("系統", "服務已停止", this.txtMessageLogs);

            this.lblProcessStatus.Text = "停止的排程服務";
        }
        #endregion

        private void mainTimer_Tick(object sender, EventArgs e)
        {
            this.Checking();
        }

        #region Checking
        /// <summary>
        /// 開始執行檢查。
        /// </summary>
        private void Checking()
        {
            if (this._running)
            {
                //若程序正在執行中, 則略過此次檢查.
                this.AddMessageLog("系統", "檢查程序正在執行中，略過此次檢查。", this.txtMessageLogs);
            }
            else
            {
                string currentProcessStatus = this.lblProcessStatus.Text;
                try
                {
                    this._running = true;
                    this.lblProcessStatus.Text = "執行排程檢查中";
                    this.AddMessageLog("系統", "開始排程器檢查程序", this.txtMessageLogs);

                    for (int i = 0; i < this._schedulers.Count; i++)
                    {
                        this._schedulers[i].Check();
                    }

                    int runningCount = 0;
                    for (int i = 0; i < this._schedulers.Count; i++)
                    {
                        if (this._schedulers[i].IsRunning)
                        {
                            runningCount++;
                            Application.DoEvents();
                        }
                    }

                    if (this._schedulers.Count == 0)
                    {
                        this.AddMessageLog("系統", "沒有設定任何排程器", this.txtMessageLogs);
                    }
                    else if (runningCount == 0)
                    {
                        this.AddMessageLog("系統", "目前沒有執行中的排程器", this.txtMessageLogs);
                    }
                }
                catch (Exception ex)
                {
                    this.AddMessageLog("系統", ex.ToString(), this.txtMessageLogs);
                }
                finally
                {
                    this.lblProcessStatus.Text = currentProcessStatus;
                    this._running = false;
                }
            }
        }
        #endregion

        #region AddMessageLog
        /// <summary>
        /// 取得訊息文字方塊控制項。
        /// </summary>
        public TextBox MessageTextBox
        {
            get { return this.txtMessageLogs; }
        }

        int _messageLogsCurrentLine = 0;
        static object _lockMessageLog = new object();
        delegate void MessageLogsDelegate(string category, string message, TextBox messageBox);

        /// <summary>
        /// 加入一筆訊息至指定的訊息視窗。
        /// </summary>
        /// <param name="category">訊息類別。</param>
        /// <param name="message">訊息內容。</param>
        /// <param name="messageBox">System.Windows.Forms.TextBox。</param>
        public void AddMessageLog(string category, string message, TextBox messageBox)
        {
            if (this.InvokeRequired)
            {
                MessageLogsDelegate messageLogsDelegate = new MessageLogsDelegate(AddMessageLog); this.Invoke(messageLogsDelegate, category, message, messageBox);
            }
            else
            {
                lock (frmMain._lockMessageLog)
                {
                    AppUtil.WriteToFile(category, message);

                    StringBuilder builder = new StringBuilder(messageBox.Text);
                    if (this._messageLogsCurrentLine == SystemDefine.MessageLogsMaxLine)
                    {
                        builder = new StringBuilder(builder.ToString().Substring(0, messageBox.Text.LastIndexOf(Environment.NewLine)));
                    }
                    else
                    {
                        this._messageLogsCurrentLine++;
                    }

                    string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string newLine = string.Empty;

                    if (messageBox.Text.Length != 0)
                    {
                        newLine = Environment.NewLine;
                    }

                    builder.Insert(0, string.Format("[{0}] [{1}] {2}{3}", datetime, category, message, newLine));
                    messageBox.Text = builder.ToString();
                }
            }

            Application.DoEvents();
        }
        #endregion
    }
}
