using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using IndianHealthService.BMXNet;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DCopyAppts.
	/// </summary>
	public class DCopyAppts : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlOKCancel;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblSummary;
		private System.Windows.Forms.Label lblProgress;
		private System.ComponentModel.IContainer components;

        #region Fields
        private DateTime			m_dtBegin;
		private DateTime			m_dtEnd;
		private string				m_HospLocationID;
		private string				m_HospLocationName;
		private string				m_ResourceID;
		private string				m_ResourceName;
		private string				m_sTask;
		private CGDocumentManager	m_DocManager;

		private System.Windows.Forms.Timer timerPoll;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;

		//protected delegate void UpdateDisplayDelegate(string sText);
		//protected delegate void RegisterEventDelegate(string sPort, string sEvent);

        #endregion Fields

        public DCopyAppts()
		{
			InitializeComponent();
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
			this.components = new System.ComponentModel.Container();
			this.pnlOKCancel = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpDescription = new System.Windows.Forms.GroupBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblSummary = new System.Windows.Forms.Label();
			this.lblProgress = new System.Windows.Forms.Label();
			this.timerPoll = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.pnlOKCancel.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescription.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlOKCancel
			// 
			this.pnlOKCancel.Controls.Add(this.cmdCancel);
			this.pnlOKCancel.Controls.Add(this.cmdOK);
			this.pnlOKCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlOKCancel.Location = new System.Drawing.Point(0, 286);
			this.pnlOKCancel.Name = "pnlOKCancel";
			this.pnlOKCancel.Size = new System.Drawing.Size(376, 40);
			this.pnlOKCancel.TabIndex = 4;
			// 
			// cmdCancel
			// 
			this.cmdCancel.Location = new System.Drawing.Point(288, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(208, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "OK";
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpDescription);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDescription.Location = new System.Drawing.Point(0, 222);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(376, 64);
			this.pnlDescription.TabIndex = 47;
			// 
			// grpDescription
			// 
			this.grpDescription.Controls.Add(this.lblDescription);
			this.grpDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescription.Location = new System.Drawing.Point(0, 0);
			this.grpDescription.Name = "grpDescription";
			this.grpDescription.Size = new System.Drawing.Size(376, 64);
			this.grpDescription.TabIndex = 0;
			this.grpDescription.TabStop = false;
			this.grpDescription.Text = "Description";
			// 
			// lblDescription
			// 
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescription.Location = new System.Drawing.Point(3, 16);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(370, 45);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "This panel displays the progress of the appointment copy process.  Press the \'Can" +
				"cel\' button to stop copying appointments.";
			// 
			// lblSummary
			// 
			this.lblSummary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblSummary.Location = new System.Drawing.Point(32, 32);
			this.lblSummary.Name = "lblSummary";
			this.lblSummary.Size = new System.Drawing.Size(312, 64);
			this.lblSummary.TabIndex = 48;
			this.lblSummary.Text = "lblSummary";
			// 
			// lblProgress
			// 
			this.lblProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblProgress.Location = new System.Drawing.Point(32, 128);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(312, 72);
			this.lblProgress.TabIndex = 49;
			this.lblProgress.Text = "lblProgress";
			// 
			// timerPoll
			// 
			this.timerPoll.Tick += new System.EventHandler(this.timerPoll_Tick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 50;
			this.label1.Text = "Status:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 16);
			this.label2.TabIndex = 51;
			this.label2.Text = "Job Summary:";
			// 
			// DCopyAppts
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 326);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.lblSummary);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlOKCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "DCopyAppts";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Copy Appointments";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DCopyAppts_Closing);
			this.Load += new System.EventHandler(this.DCopyAppts_Load);
			this.pnlOKCancel.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescription.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        #region Methods and Handlers

		public void InitializePage(DateTime StartDate, DateTime EndDate, 
			string HospLocationID, string HospLocationName, 
			string ResourceID, string ResourceName, 
			CGDocumentManager DocManager)
		{
			string sMsg = "Copying appointments from " + HospLocationName + " to " + ResourceName + ", ";
			sMsg += "beginning with apppointments on " + StartDate.ToLongDateString();
			sMsg += " and going through " + EndDate.ToLongDateString() + ".";
			lblSummary.Text = sMsg;
			m_dtBegin = StartDate;
			m_dtEnd = EndDate;
			m_HospLocationID = HospLocationID;
			m_HospLocationName = HospLocationName;
			m_ResourceID = ResourceID;
			m_ResourceName = ResourceName;
			m_DocManager = DocManager;
		}

		private void DCopyAppts_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Start M copy job and get the ZTSK number
				this.timerPoll.Stop();
				lblProgress.Text = "Starting Process";

				string sSql = "BSDX COPY APPOINTMENTS^" + m_ResourceID + "^" + m_HospLocationID + "^" + m_dtBegin.ToShortDateString() + "^" + m_dtEnd.ToShortDateString();
				DataTable dt = m_DocManager.RPMSDataTable(sSql, "ApptCopy");
				Debug.Assert(dt.Rows.Count == 1);


				DataRow dr = dt.Rows[0];
				m_sTask = "0";
				Object oTask = dr["TASK_NUMBER"];
				m_sTask = oTask.ToString();

				Object oError = dr["ERRORID"];
				string sError = oError.ToString();
				if (sError != "OK")
				{
					timerPoll.Stop();
					lblProgress.Text = sError;
					cmdOK.Enabled = true;
					cmdCancel.Enabled = false;
				}
				else
				{
					lblProgress.Text = "RPMS Job queued as Task #" + m_sTask;
					;//this.timerPoll.Start();
					cmdOK.Enabled = false;
					cmdCancel.Enabled = true;
				}

			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.Message);
			}
			
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				//Check status and update progress control
				string sSql = "BSDX COPY APPOINTMENT CANCEL^" + m_sTask;
				DataTable dt = m_DocManager.RPMSDataTable(sSql, "ApptCopyCancel");
				Debug.Assert(dt.Rows.Count == 1);
				DataRow dr = dt.Rows[0];
				Object oCount = dr["RECORD_COUNT"];
				string sCount = oCount.ToString();

				lblProgress.Text = "Cancelling job...";
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.Message);
			}		
		}

		private void DCopyAppts_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{

        }

		private void timerPoll_Tick(object sender, System.EventArgs e)
		{
			try
			{
                return;
				//Check status and update progress control
                //string sSql = "BSDX COPY APPOINTMENT STATUS^" + m_sTask;
                //DataTable dt = m_DocManager.RPMSDataTable(sSql, "ApptCopyStatus");
                //Debug.Assert(dt.Rows.Count == 1);
                //DataRow dr = dt.Rows[0];
                //Object oCount = dr["RECORD_COUNT"];
                //string sCount = oCount.ToString();
                //Object oError = dr["ERRORID"];
                //string sError = oError.ToString();
                //if (sError != "OK")
                //{
                //    timerPoll.Stop();
                //    lblProgress.Text = sError;
                //}
                //else if ((sCount.StartsWith("Finished"))||(sCount.StartsWith("Cancelled")))
                //{
                //    timerPoll.Stop();
                //    lblProgress.Text = sCount;
                //    cmdOK.Enabled = true;
                //    cmdCancel.Enabled = false;
                //}
                //else
                //{
                //    lblProgress.Text = "RPMS Job queued as Task #" + m_sTask + ".  " + sCount; // + " records copied so far.";
                //}
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.Message);
			}
        }

        #endregion Methods and Handlers
    }
}
