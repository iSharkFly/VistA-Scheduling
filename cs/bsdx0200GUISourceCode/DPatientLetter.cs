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
        private System.Drawing.Printing.PrintDocument printAppts;
        private System.Drawing.Printing.PrintDocument printReminderLetters;

		#region Fields
        DateTime _dtBegin, _dtEnd; //global fields to use in passing to printing method
        
        //stuff to track what got printed and where we are -- very ugly, I know
        //but I don't know if there is a better way to do it.
        int _currentResourcePrinting = 0;
        int _currentApptPrinting = 0;
        int _pageNumber = 0;
        
        //dataset to load the results of queries into and set to print routines
        dsPatientApptDisplay2 _dsApptDisplay;
        private PrintDocument printCancelLetters;
        private PrintDocument printRebookLetters;
        dsRebookAppts _dsRebookAppts;
		#endregion Fields

		/// <summary>
		/// Print Clinic Schedules
		/// </summary>
		/// <param name="docManager">This docManger</param>
		/// <param name="sClinicList">Clinics for which to print</param>
		/// <param name="dtBegin">Beginning Date</param>
		/// <param name="dtEnd">End Date</param>
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

                _dtBegin = dtBegin; // Global variable to use in Printing method below
                _dtEnd = dtEnd; // ditto

                this.Text="Clinic Schedules";

                // Get Data
                DataTable PatientAppts = docManager.DAL.GetClinicSchedules(sClinicList, dtBegin, dtEnd);
                DataTable Resources = docManager.DAL.GetResourceLetters(sClinicList);

                // Merge tables into typed dataset
                _dsApptDisplay = new dsPatientApptDisplay2();
                _dsApptDisplay.PatientAppts.Merge(PatientAppts);
                _dsApptDisplay.BSDXResource.Merge(Resources);

                this.PrintPreviewControl.Document = printAppts;
            }
			catch (Exception ex)
			{
				throw ex;
			}
		}

        /// <summary>
        /// Print Rebook Letters by Date
        /// </summary>
        /// <param name="docManager">This docManger</param>
        /// <param name="sClinicList">Clinics for which to print</param>
        /// <param name="dtBegin">Beginning Date</param>
        /// <param name="dtEnd">End Date</param>
        /// working on moving this over...
        public void InitializeFormRebookLetters(CGDocumentManager docManager,
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

                string sBegin = dtBegin.ToString("M/d/yyyy@HH:mm");
                string sEnd = dtEnd.ToString("M/d/yyyy@HH:mm");

                //Call RPC to get list of appt ids that have been rebooked for these clinics on these dates
                string sSql1 = "BSDX REBOOK CLINIC LIST^" + sClinicList + "^" + sBegin + "^" + sEnd;				
				string sSql2 = "BSDX RESOURCE LETTERS^" + sClinicList;

                _dsRebookAppts = new dsRebookAppts();
                _dsRebookAppts.PatientAppts.Merge(docManager.RPMSDataTable(sSql1, "PatientAppts"));
                _dsRebookAppts.BSDXResource.Merge(docManager.RPMSDataTable(sSql2, "Resources"));

            }
			catch (Exception ex)
			{
				throw ex;
			}

            PrintPreviewControl.Document = printRebookLetters;

		}

        /// <summary>
        /// Print Rebook Letters by Date
        /// </summary>
        /// <param name="docManager">This docManger</param>
        /// <param name="sClinicList">Clinics for which to print</param>
        /// <param name="sApptIDList">List of appointments IENs in ^BSDXAPPT, delimited by |</param>
        public void InitializeFormRebookLetters(CGDocumentManager docManager,
            string sClinicList,
            string sApptIDList)
        {
            try
            {
                if (sClinicList == "")
                {
                    throw new Exception("At least one clinic must be selected.");
                }

                string sSql1 = "BSDX REBOOK LIST^" + sApptIDList;
                string sSql2 = "BSDX RESOURCE LETTERS^" + sClinicList;

                _dsRebookAppts = new dsRebookAppts();
                _dsRebookAppts.PatientAppts.Merge(docManager.RPMSDataTable(sSql1, "PatientAppts"));
                _dsRebookAppts.BSDXResource.Merge(docManager.RPMSDataTable(sSql2, "Resources"));


            }
            catch (Exception ex)
            {
                throw ex;
            }

            PrintPreviewControl.Document = printRebookLetters;
        }

        /// <summary>
        /// Print Cancellation letters to mail to patients
        /// </summary>
        /// <param name="docManager">This Docmanager</param>
        /// <param name="sClinicList">| delemited clinic list (IEN's)</param>
        /// <param name="dtBegin">Beginning Date</param>
        /// <param name="dtEnd">Ending Date</param>
		public void InitializeFormCancellationLetters(CGDocumentManager docManager,
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

                string sBegin = dtBegin.ToString("M/d/yyyy@HH:mm");
                string sEnd = dtEnd.ToString("M/d/yyyy@HH:mm");

                //Call RPC to get list of appt ids that have been cancelled for these clinics on these dates
                string sSql1 = "BSDX CANCEL CLINIC LIST^" + sClinicList + "^" + sBegin + "^" + sEnd;
				string sSql2 = "BSDX RESOURCE LETTERS^" + sClinicList;

                _dsRebookAppts = new dsRebookAppts();
                _dsRebookAppts.PatientAppts.Merge(docManager.RPMSDataTable(sSql1, "PatientAppts"));
                _dsRebookAppts.BSDXResource.Merge(docManager.RPMSDataTable(sSql2, "Resources"));

                PrintPreviewControl.Document = printCancelLetters;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
        /// <summary>
        /// Print Reminder Letters to give or mail to patients
        /// </summary>
        /// <param name="docManager">This docManger</param>
        /// <param name="sClinicList">Clinics for which to print</param>
        /// <param name="dtBegin">Beginning Date</param>
        /// <param name="dtEnd">End Date</param>
        public void InitializeFormPatientReminderLetters(CGDocumentManager docManager,
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
                this.Text = "Clinic Schedules";

                string sSql = "BSDX CLINIC LETTERS^" + sClinicList + "^" + sBegin + "^" + sEnd;
                string sSql2 = "BSDX RESOURCE LETTERS^" + sClinicList;

                _dsApptDisplay = new dsPatientApptDisplay2();
                _dsApptDisplay.BSDXResource.Merge(docManager.RPMSDataTable(sSql2, "Resources"));
                _dsApptDisplay.PatientAppts.Merge(docManager.RPMSDataTable(sSql, "PatientAppts"));

                this.PrintPreviewControl.Document = printReminderLetters;
				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        /// <summary>
        /// Ctor
        /// </summary>
		public DPatientLetter() : base()
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
            this.printAppts = new System.Drawing.Printing.PrintDocument();
            this.printReminderLetters = new System.Drawing.Printing.PrintDocument();
            this.printCancelLetters = new System.Drawing.Printing.PrintDocument();
            this.printRebookLetters = new System.Drawing.Printing.PrintDocument();
            this.SuspendLayout();
            // 
            // printAppts
            // 
            this.printAppts.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printAppts_PrintPage);
            // 
            // printReminderLetters
            // 
            this.printReminderLetters.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printReminderLetters_PrintPage);
            // 
            // printCancelLetters
            // 
            this.printCancelLetters.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printCancelLetters_PrintPage);
            // 
            // printRebookLetters
            // 
            this.printRebookLetters.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printRebookLetters_PrintPage);
            // 
            // DPatientLetter
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(648, 398);
            this.Name = "DPatientLetter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient Letter";
            this.ResumeLayout(false);

		}
		#endregion

        private void printAppts_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //Each time we enter here, we start off with a new page number - we start off with zero
            _pageNumber++; //becomes one first time through
            
            // _currentResourcePrinting starts with zero. There will be at least this one.
            ClinicalScheduling.Printing.PrintAppointments(_dsApptDisplay, e, _dtBegin, _dtEnd,
                    _currentResourcePrinting, ref _currentApptPrinting, _pageNumber);

            //If the printing routine says it needs more pages to print the appointments,
            //return here and have it print again.
            if (e.HasMorePages == true) return;

            // if there are more resouces to print, increment. Setting HasMorePages to true
            // calls this routine again, so printing will happen with the incremented _currentResourcePrinting
            if (_currentResourcePrinting < _dsApptDisplay.BSDXResource.Rows.Count - 1)
            {
                _currentResourcePrinting++;
                //reset _currentApptPrinting
                _currentApptPrinting = 0;
                e.HasMorePages = true;
            }            
        }

        private void printReminderLetters_PrintPage(object sender, PrintPageEventArgs e)
        {
            // no patients
            if (_dsApptDisplay.PatientAppts.Count == 0)
            {
                ClinicalScheduling.Printing.PrintMessage("No Appointments found", e);
                return;
            }
            // if there are patients
            else if (_currentApptPrinting < _dsApptDisplay.PatientAppts.Count)
            {
                dsPatientApptDisplay2.BSDXResourceRow c = (dsPatientApptDisplay2.BSDXResourceRow)
                   _dsApptDisplay.PatientAppts[_currentApptPrinting].GetParentRow(_dsApptDisplay.Relations[0]);
                ClinicalScheduling.Printing.PrintReminderLetter(_dsApptDisplay.PatientAppts[_currentApptPrinting], e, c.LETTER_TEXT, "Reminder Letter");
                _currentApptPrinting++;
                if (_currentApptPrinting < _dsApptDisplay.PatientAppts.Count)
                    e.HasMorePages = true;
            }
            
        }

        private void printCancelLetters_PrintPage(object sender, PrintPageEventArgs e)
        {
            // no patients
            if (_dsRebookAppts.PatientAppts.Count == 0)
            {
                ClinicalScheduling.Printing.PrintMessage("No Appointments found", e);
                return;
            }
            // if there are patients
            else if (_currentApptPrinting < _dsRebookAppts.PatientAppts.Count)
            {
                dsRebookAppts.BSDXResourceRow c = (dsRebookAppts.BSDXResourceRow)
                   _dsRebookAppts.PatientAppts[_currentApptPrinting].GetParentRow(_dsRebookAppts.Relations[0]);
                ClinicalScheduling.Printing.PrintCancelLetter(_dsRebookAppts.PatientAppts[_currentApptPrinting], e, c.CLINIC_CANCELLATION_LETTER, "Cancellation Letter");
                _currentApptPrinting++;
                if (_currentApptPrinting < _dsRebookAppts.PatientAppts.Count)
                    e.HasMorePages = true;
            }

        }

        private void printRebookLetters_PrintPage(object sender, PrintPageEventArgs e)
        {
            // no patients
            if (_dsRebookAppts.PatientAppts.Count == 0)
            {
                ClinicalScheduling.Printing.PrintMessage("No Appointments found", e);
                return;
            }
            // if there are patients
            else if (_currentApptPrinting < _dsRebookAppts.PatientAppts.Count)
            {
                dsRebookAppts.BSDXResourceRow c = (dsRebookAppts.BSDXResourceRow)
                   _dsRebookAppts.PatientAppts[_currentApptPrinting].GetParentRow(_dsRebookAppts.Relations[0]);
                //XXX: Rebook letter rather oddly currently stored in NO SHOW LETTER field. What gives???
                ClinicalScheduling.Printing.PrintRebookLetter(_dsRebookAppts.PatientAppts[_currentApptPrinting], e, c.NO_SHOW_LETTER, "Rebook Letter");
                _currentApptPrinting++;
                if (_currentApptPrinting < _dsRebookAppts.PatientAppts.Count)
                    e.HasMorePages = true;
            }
        }
	}
}
