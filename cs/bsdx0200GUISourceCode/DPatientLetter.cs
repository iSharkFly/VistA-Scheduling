using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using CrystalDecisions.Windows;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DPatientLetter.
	/// </summary>
	public class DPatientLetter : System.Windows.Forms.Form
	{
		private CrystalDecisions.Windows.Forms.CrystalReportViewer crViewer1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Fields

		private string	m_sBodyText;
		#endregion Fields

		#region Properties

		public string BodyText
		{
			get
			{
				return m_sBodyText;
			}
			set
			{
				m_sBodyText = value;
			}
		}

		#endregion Properties

		public void InitializeForm(CGDocumentManager docManager, int nPatientID)
		{
			try
			{	
				crViewer1.DisplayGroupTree = true;

				ClinicalScheduling.crPatientLetter cr = new crPatientLetter();
				string sSql = "BSDX PATIENT APPT DISPLAY^" + nPatientID.ToString();
				System.Data.DataSet ds = new System.Data.DataSet();
				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");
				ds.Tables.Add(dtAppt.Copy());

				System.Data.DataTable dt = 
					docManager.GlobalDataSet.Tables["Resources"].Copy();
				ds.Tables.Add(dt);

				cr.Database.Tables["PatientAppts"].SetDataSource(ds.Tables["PatientAppts"]);
				cr.Database.Tables["BSDXResource"].SetDataSource(ds.Tables["Resources"]);

				crViewer1.SelectionFormula = "{PatientAppts.ApptDate} >= CurrentDate";
				this.crViewer1.ReportSource = cr;
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void InitializeFormClinicSchedule(CGDocumentManager docManager,
			string sClinicList,
			DateTime dtBegin,
			DateTime dtEnd)
		{
			try
			{
				if (sClinicList == "")
				{
					throw new Exception("At least one clinic must be selected.");
				}
				string sBegin = dtBegin.ToShortDateString();
				string sEnd = dtEnd.ToShortDateString();
				crViewer1.DisplayGroupTree = true;
				this.Text="Clinic Schedules";

				ClinicalScheduling.crAppointmentList cr = new crAppointmentList();
				string sSql = "BSDX CLINIC LETTERS^" + sClinicList + "^" + sBegin + "^" + sEnd;

				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");				
				cr.Database.Tables["PatientAppts"].SetDataSource(dtAppt);

				this.crViewer1.ReportSource = cr;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void InitializeFormRebookLetters(CGDocumentManager docManager,
			string sClinicList,
			DataTable dtLetters)
		{
			try
			{	
				if (sClinicList == "")
				{
					throw new Exception("At least one clinic must be selected.");
				}
				crViewer1.DisplayGroupTree = true;

				ClinicalScheduling.crRebookLetter cr = new crRebookLetter();

				System.Data.DataSet ds = new System.Data.DataSet();
				ds.Tables.Add(dtLetters.Copy());
				
				string sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());

				cr.Database.Tables["PatientAppts"].SetDataSource(ds.Tables["PatientAppts"]);
				cr.Database.Tables["BSDXResource"].SetDataSource(ds.Tables["Resources"]);

				this.crViewer1.ReportSource = cr;
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void InitializeFormCancellationLetters(CGDocumentManager docManager,
			string sClinicList,
			DataTable dtLetters)
		{
			try
			{	
				if (sClinicList == "")
				{
					throw new Exception("At least one clinic must be selected.");
				}
				crViewer1.DisplayGroupTree = true;

				ClinicalScheduling.crCancelLetter cr = new crCancelLetter();

				System.Data.DataSet ds = new System.Data.DataSet();
				ds.Tables.Add(dtLetters.Copy());
				
				string sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());

				cr.Database.Tables["PatientAppts"].SetDataSource(ds.Tables["PatientAppts"]);
				cr.Database.Tables["BSDXResource"].SetDataSource(ds.Tables["Resources"]);

				this.crViewer1.ReportSource = cr;
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void InitializeFormClinicLetters(CGDocumentManager docManager,
			string sClinicList,
			DateTime dtBegin,
			DateTime dtEnd)
		{
			try
			{	
				if (sClinicList == "")
				{
					throw new Exception("At least one clinic must be selected.");
				}
				
				string sBegin = dtBegin.ToShortDateString();
				string sEnd = dtEnd.ToShortDateString();
				crViewer1.DisplayGroupTree = true;

				ClinicalScheduling.crPatientLetter cr = new crPatientLetter();
				string sSql = "BSDX CLINIC LETTERS^" + sClinicList + "^" + sBegin + "^" + sEnd;

				System.Data.DataSet ds = new System.Data.DataSet();
				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");				
				ds.Tables.Add(dtAppt.Copy());
				
				sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());

				cr.Database.Tables["PatientAppts"].SetDataSource(ds.Tables["PatientAppts"]);
				cr.Database.Tables["BSDXResource"].SetDataSource(ds.Tables["Resources"]);

				this.crViewer1.ReportSource = cr;
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		public DPatientLetter()
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
			this.crViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			this.SuspendLayout();
			// 
			// crViewer1
			// 
			this.crViewer1.ActiveViewIndex = -1;
			this.crViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.crViewer1.Location = new System.Drawing.Point(0, 0);
			this.crViewer1.Name = "crViewer1";
			this.crViewer1.ReportSource = null;
			this.crViewer1.Size = new System.Drawing.Size(648, 398);
			this.crViewer1.TabIndex = 0;
			// 
			// DPatientLetter
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(648, 398);
			this.Controls.Add(this.crViewer1);
			this.Name = "DPatientLetter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Patient Letter";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
