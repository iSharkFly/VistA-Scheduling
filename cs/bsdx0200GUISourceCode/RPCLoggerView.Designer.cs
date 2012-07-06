namespace IndianHealthService.ClinicalScheduling
{
    partial class RPCLoggerView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstRPCEvents = new System.Windows.Forms.ListBox();
            this.txtRPCEvent = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstRPCEvents);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtRPCEvent);
            this.splitContainer1.Size = new System.Drawing.Size(894, 262);
            this.splitContainer1.SplitterDistance = 342;
            this.splitContainer1.TabIndex = 0;
            // 
            // lstRPCEvents
            // 
            this.lstRPCEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRPCEvents.FormattingEnabled = true;
            this.lstRPCEvents.Location = new System.Drawing.Point(0, 0);
            this.lstRPCEvents.Name = "lstRPCEvents";
            this.lstRPCEvents.Size = new System.Drawing.Size(342, 262);
            this.lstRPCEvents.TabIndex = 0;
            this.lstRPCEvents.SelectedIndexChanged += new System.EventHandler(this.lstRPCEvents_SelectedIndexChanged);
            // 
            // txtRPCEvent
            // 
            this.txtRPCEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRPCEvent.Location = new System.Drawing.Point(0, 0);
            this.txtRPCEvent.Multiline = true;
            this.txtRPCEvent.Name = "txtRPCEvent";
            this.txtRPCEvent.Size = new System.Drawing.Size(548, 262);
            this.txtRPCEvent.TabIndex = 0;
            // 
            // RPCLoggerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 262);
            this.Controls.Add(this.splitContainer1);
            this.Name = "RPCLoggerView";
            this.Text = "BMX RPC Log View";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtRPCEvent;
        private System.Windows.Forms.ListBox lstRPCEvents;
    }
}