using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using CrystalDecisions.Windows;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using IndianHealthService.BMXNet;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DPatientApptDisplay.
	/// </summary>
	public class DPatientApptDisplay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private CrystalDecisions.Windows.Forms.CrystalReportViewer crViewer1;
		private System.Windows.Forms.CheckBox chkIncludePast;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public void InitializeForm(CGDocumentManager docManager, int nPatientID)
		{
			try
			{	
				crViewer1.DisplayGroupTree = false;

				ClinicalScheduling.crPatientApptDisplay cr = new crPatientApptDisplay();
				string sSql = "BSDX PATIENT APPT DISPLAY^" + nPatientID.ToString();

				System.Data.DataSet ds = new System.Data.DataSet();
				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");
				ds.Tables.Add(dtAppt.Copy());

				cr.Database.Tables[0].SetDataSource(ds);
				this.crViewer1.ReportSource = cr;

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public DPatientApptDisplay()
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.chkIncludePast = new System.Windows.Forms.CheckBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.crViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkIncludePast);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(664, 32);
			this.panel1.TabIndex = 1;
			// 
			// chkIncludePast
			// 
			this.chkIncludePast.Location = new System.Drawing.Point(16, 8);
			this.chkIncludePast.Name = "chkIncludePast";
			this.chkIncludePast.Size = new System.Drawing.Size(184, 16);
			this.chkIncludePast.TabIndex = 0;
			this.chkIncludePast.Text = "Include Past Appointments";
			this.chkIncludePast.CheckedChanged += new System.EventHandler(this.chkIncludePast_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.crViewer1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 32);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(664, 446);
			this.panel2.TabIndex = 2;
			// 
			// crViewer1
			// 
			this.crViewer1.ActiveViewIndex = -1;
			this.crViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.crViewer1.Location = new System.Drawing.Point(0, 0);
			this.crViewer1.Name = "crViewer1";
			this.crViewer1.ReportSource = null;
			this.crViewer1.Size = new System.Drawing.Size(664, 446);
			this.crViewer1.TabIndex = 1;
			// 
			// DPatientApptDisplay
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 478);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "DPatientApptDisplay";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Patient Appointments";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void chkIncludePast_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkIncludePast.Checked == true)
			{
                this.crViewer1.SelectionFormula = "TRUE"; //MJL 9/11/2007
			}
			else
			{
				crViewer1.SelectionFormula = "{PatientAppts.ApptDate} >= CurrentDate";
			}
			crViewer1.RefreshReport();
		}
	}
}
