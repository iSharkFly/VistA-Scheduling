using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Printing;
using System.Drawing;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DPatientLetter.
	/// </summary>
	public class DPatientLetter : System.Windows.Forms.PrintPreviewDialog
    {
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
        private System.Drawing.Printing.PrintDocument printDocument1;

		#region Fields
        DateTime _dtBegin, _dtEnd; //global fields to use in passing to printing method
        int _currentResourcePrinting = 0;
        int _currentApptPrinting = 0;
        dsPatientApptDisplay2 _ds;
		#endregion Fields

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
                _dtBegin = dtBegin;
                _dtEnd = dtEnd;
				string sBegin = dtBegin.ToShortDateString();
				string sEnd = dtEnd.ToShortDateString();
				this.Text="Clinic Schedules";

				string sSql = "BSDX CLINIC LETTERS^" + sClinicList + "^" + sBegin + "^" + sEnd;
                string sSql2 = "BSDX RESOURCE LETTERS^" + sClinicList;

                _ds = new dsPatientApptDisplay2();
                _ds.BSDXResource.Merge(docManager.RPMSDataTable(sSql2, "Resources"));
                _ds.PatientAppts.Merge(docManager.RPMSDataTable(sSql, "PatientAppts"));
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

				System.Data.DataSet ds = new System.Data.DataSet();
				ds.Tables.Add(dtLetters.Copy());
				
				string sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());				
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

				System.Data.DataSet ds = new System.Data.DataSet();
				ds.Tables.Add(dtLetters.Copy());
				
				string sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());
				
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

				string sSql = "BSDX CLINIC LETTERS^" + sClinicList + "^" + sBegin + "^" + sEnd;

				System.Data.DataSet ds = new System.Data.DataSet();
				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");				
				ds.Tables.Add(dtAppt.Copy());
				
				sSql = "BSDX RESOURCE LETTERS^" + sClinicList;
				DataTable dt = docManager.RPMSDataTable(sSql, "Resources");				
				ds.Tables.Add(dt.Copy());
				
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
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.SuspendLayout();
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // DPatientLetter
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(648, 398);
            this.Document = this.printDocument1;
            this.Name = "DPatientLetter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient Letter";
            this.ResumeLayout(false);

		}
		#endregion

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // _currentResourcePrinting starts with zero. There will be at least this one.
            ClinicalScheduling.Printing.PrintAppointments(_ds, e, _dtBegin, _dtEnd,
                    _currentResourcePrinting, ref _currentApptPrinting);

            //If the printing routine says it needs more pages to print the appointments,
            //return here and have it print again.
            if (e.HasMorePages == true) return;

            // if there are more resouces to print, increment. Setting HasMorePages to true
            // calls this routine again, so printing will happen with the incremented _currentResourcePrinting
            if (_currentResourcePrinting < _ds.BSDXResource.Rows.Count - 1)
            {
                _currentResourcePrinting++;
                e.HasMorePages = true;
            }
            
        }
	}
}
