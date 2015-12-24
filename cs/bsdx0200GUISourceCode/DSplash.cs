using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Program loading splash screen. Notice the numerous remote methods intended
    /// to mickey mouse the form from another thread.
    /// 
    /// I don't know of a better way of doing this right now.
	/// </summary>
	public class DSplash : System.Windows.Forms.Form
	{
		private System.Windows.Forms.LinkLabel lnkMail;
		private System.Windows.Forms.Label lblStatus;
        private Label lblVersion;
        private ProgressBar progressBar1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DSplash()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DSplash));
            this.lnkMail = new System.Windows.Forms.LinkLabel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lnkMail
            // 
            this.lnkMail.BackColor = System.Drawing.Color.Gray;
            this.lnkMail.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lnkMail.Location = new System.Drawing.Point(489, 229);
            this.lnkMail.Name = "lnkMail";
            this.lnkMail.Size = new System.Drawing.Size(100, 23);
            this.lnkMail.TabIndex = 4;
            this.lnkMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMail_LinkClicked);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Gray;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblStatus.Location = new System.Drawing.Point(261, 202);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(328, 16);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Gray;
            this.lblVersion.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblVersion.Location = new System.Drawing.Point(239, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "lblVersion";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Gray;
            this.progressBar1.Location = new System.Drawing.Point(60, 164);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(458, 14);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 7;
            // 
            // DSplash
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(601, 403);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lnkMail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DSplash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clinical Scheduling";
            this.Load += new System.EventHandler(this.DSplash_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        public delegate void dSetStatus(string sStatus);
        public delegate void dAny();
        public delegate void dProgressBarSet(int number);
        public delegate DialogResult dMessageBox(IWin32Window owner, string message);
        public delegate DialogResult dMessageBox2(IWin32Window owner, string message, string caption, MessageBoxButtons btns);
		
        public void SetStatus(string sStatus)
		{
            if (this.InvokeRequired == true)
            {
                dSetStatus d = new dSetStatus(SetStatus);
                this.Invoke(d, new object[] { sStatus });
                return;
            }
            
			this.lblStatus.Text = sStatus;
		}

		private void DSplash_Load(object sender, System.EventArgs e)
		{
			this.lblVersion.Text = "Version " + Application.ProductVersion;
		}

        public DialogResult RemoteMsgBox(string msg)
        {
            dMessageBox d = new dMessageBox(MessageBox.Show);
            return (DialogResult)this.Invoke(d, this, msg);
        }

        public DialogResult RemoteMsgBox(string msg, string caption, MessageBoxButtons btns)
        {
            dMessageBox2 d = new dMessageBox2(MessageBox.Show);
            return (DialogResult)this.Invoke(d, this, msg, caption, btns);
        }

        public void RemoteClose()
        {
            dAny d = new dAny(this.Close);
            this.Invoke(d);
        }

        public void RemoteActivate()
        {
            dAny d = new dAny(this.Activate);
            this.Invoke(d);
        }

        public void RemoteHide()
        {
            dAny d = new dAny(this.Hide);
            this.Invoke(d);
        }

        public void RemoteProgressBarMaxSet(int max)
        {
            if (this.InvokeRequired == true)
            {
                dProgressBarSet d = new dProgressBarSet(RemoteProgressBarMaxSet);
                this.Invoke(d, new object[] { max });
                return;
            }

            this.progressBar1.Maximum = max;
        }

        public void RemoteProgressBarValueSet(int val)
        {
            if (this.InvokeRequired == true)
            {
                dProgressBarSet d = new dProgressBarSet(RemoteProgressBarValueSet);
                this.Invoke(d, new object[] { val });
                return;
            }

            this.progressBar1.Value = val;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void lnkMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }    
}
