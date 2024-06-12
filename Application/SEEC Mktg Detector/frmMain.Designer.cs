namespace Seec.Marketing
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblProcessStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTimer = new System.Windows.Forms.Timer(this.components);
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolbarStart = new System.Windows.Forms.ToolStripButton();
            this.toolbarStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripRegisteredScheduler = new System.Windows.Forms.ToolStripButton();
            this.toolStripCheckRunning = new System.Windows.Forms.ToolStripButton();
            this.txtMessageLogs = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProcessStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblProcessStatus
            // 
            this.lblProcessStatus.Name = "lblProcessStatus";
            this.lblProcessStatus.Size = new System.Drawing.Size(128, 17);
            this.lblProcessStatus.Text = "toolStripStatusLabel1";
            // 
            // mainTimer
            // 
            this.mainTimer.Tick += new System.EventHandler(this.mainTimer_Tick);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarStart,
            this.toolbarStop,
            this.toolStripRegisteredScheduler,
            this.toolStripCheckRunning});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(784, 25);
            this.mainToolStrip.TabIndex = 6;
            this.mainToolStrip.Text = "mainToolStrip";
            // 
            // toolbarStart
            // 
            this.toolbarStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolbarStart.ImageTransparentColor = System.Drawing.SystemColors.ButtonFace;
            this.toolbarStart.Name = "toolbarStart";
            this.toolbarStart.Size = new System.Drawing.Size(35, 22);
            this.toolbarStart.Text = "啟動";
            this.toolbarStart.Click += new System.EventHandler(this.toolbarStart_Click);
            // 
            // toolbarStop
            // 
            this.toolbarStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolbarStop.ImageTransparentColor = System.Drawing.SystemColors.ButtonFace;
            this.toolbarStop.Name = "toolbarStop";
            this.toolbarStop.Size = new System.Drawing.Size(35, 22);
            this.toolbarStop.Text = "停止";
            this.toolbarStop.Click += new System.EventHandler(this.toolbarStop_Click);
            // 
            // toolStripRegisteredScheduler
            // 
            this.toolStripRegisteredScheduler.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripRegisteredScheduler.ImageTransparentColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripRegisteredScheduler.Name = "toolStripRegisteredScheduler";
            this.toolStripRegisteredScheduler.Size = new System.Drawing.Size(71, 22);
            this.toolStripRegisteredScheduler.Text = "檢視排程器";
            this.toolStripRegisteredScheduler.Click += new System.EventHandler(this.toolStripRegisteredScheduler_Click);
            // 
            // toolStripCheckRunning
            // 
            this.toolStripCheckRunning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripCheckRunning.ImageTransparentColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripCheckRunning.Name = "toolStripCheckRunning";
            this.toolStripCheckRunning.Size = new System.Drawing.Size(95, 22);
            this.toolStripCheckRunning.Text = "排程器執行狀態";
            this.toolStripCheckRunning.Click += new System.EventHandler(this.toolStripCheckRunning_Click);
            // 
            // txtMessageLogs
            // 
            this.txtMessageLogs.BackColor = System.Drawing.Color.White;
            this.txtMessageLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageLogs.Location = new System.Drawing.Point(0, 25);
            this.txtMessageLogs.MaxLength = 0;
            this.txtMessageLogs.Multiline = true;
            this.txtMessageLogs.Name = "txtMessageLogs";
            this.txtMessageLogs.ReadOnly = true;
            this.txtMessageLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessageLogs.Size = new System.Drawing.Size(784, 214);
            this.txtMessageLogs.TabIndex = 8;
            this.txtMessageLogs.WordWrap = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 261);
            this.Controls.Add(this.txtMessageLogs);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainToolStrip);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblProcessStatus;
        private System.Windows.Forms.Timer mainTimer;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton toolbarStart;
        private System.Windows.Forms.ToolStripButton toolbarStop;
        private System.Windows.Forms.ToolStripButton toolStripRegisteredScheduler;
        private System.Windows.Forms.ToolStripButton toolStripCheckRunning;
        private System.Windows.Forms.TextBox txtMessageLogs;
    }
}

