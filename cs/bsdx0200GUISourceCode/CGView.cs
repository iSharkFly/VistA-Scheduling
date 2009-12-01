using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using IndianHealthService.BMXNet;
using System.Reflection;


namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for CGView.
	/// </summary>
	public class CGView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuTest;
		private System.Windows.Forms.MenuItem mnuAppointment;
		private System.Windows.Forms.MenuItem mnuNewAppointment;
		private System.Windows.Forms.MenuItem mnu1Day;
		private System.Windows.Forms.MenuItem mnu7Day;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnu5Day;
		private System.Windows.Forms.MenuItem mnu10Minute;
		private System.Windows.Forms.MenuItem mnu20Minute;
		private System.Windows.Forms.MenuItem mnu30Minute;
		private System.Windows.Forms.MenuItem mnuTimeScale;
		private System.Windows.Forms.MenuItem mnu15Minute;
		private System.Windows.Forms.MenuItem mnuOpenSchedule;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.TreeView tvSchedules;
		private System.Windows.Forms.MenuItem mnuViewScheduleTree;
		private System.Windows.Forms.MenuItem mnuDeleteAppointment;
		private System.Windows.Forms.MenuItem mnuTest1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.MenuItem mnuViewRightPanel;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelCenter;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Label lblResource;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem ctxOpenSchedule;
		private System.Windows.Forms.MenuItem ctxEditAvailability;
		private System.Windows.Forms.MenuItem ctxProperties;
		private System.Windows.Forms.MenuItem mnuSchedulingManagment;
		private System.Windows.Forms.MenuItem ctxFindAppt;
		private System.Windows.Forms.MenuItem mnuFindAppt;
		internal System.Windows.Forms.MenuItem mnuRPMSServer;
		internal System.Windows.Forms.MenuItem mnuRPMSLogin;
		private System.Windows.Forms.MenuItem mnuCheckIn;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.MenuItem mnuCalendar;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuClose;
		private System.Windows.Forms.MenuItem mnuViewPatientAppts;
		private IndianHealthService.ClinicalScheduling.CalendarGrid calendarGrid1;
		private System.Windows.Forms.MenuItem mnuCopyAppointment;
		private System.Windows.Forms.Panel panelClip;
		private System.Windows.Forms.ListBox lstClip;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenu ctxApptClipMenu;
		private System.Windows.Forms.MenuItem mnuRemoveClipItem;
		private System.Windows.Forms.MenuItem mnuClearClipItems;
		private System.Windows.Forms.MenuItem mnuEditAppointment;
		private System.Windows.Forms.ContextMenu ctxCalendarGrid;
		private System.Windows.Forms.MenuItem ctxCalGridAdd;
		private System.Windows.Forms.MenuItem ctxCalGridEdit;
		private System.Windows.Forms.MenuItem ctxCalGridDelete;
		private System.Windows.Forms.MenuItem ctxCalGridCheckIn;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem mnuPrintClinicLetters;
		private System.Windows.Forms.MenuItem mnuPrintPatientLetter;
		private System.Windows.Forms.MenuItem mnuPrintClinicSchedules;
		private System.Windows.Forms.MenuItem ctxCalGridNoShow;
		private System.Windows.Forms.MenuItem ctxCalGridNoShowUndo;
		private System.Windows.Forms.MenuItem mnuNoShow;
		private System.Windows.Forms.MenuItem mnuNoShowUndo;
		private System.Windows.Forms.MenuItem mnuPrintRebookLetters;
		private System.Windows.Forms.MenuItem mnuPrintCancellationLetters;
		private System.Windows.Forms.MenuItem mnuWalkIn;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem ctxCalGridWalkin;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem mnuOpenMultipleSchedules;
		private System.Windows.Forms.MenuItem mnuDisplayWalkIns;
        private System.Windows.Forms.MenuItem mnuRPMSDivision;
        private IContainer components;

        #region Initialization
        public CGView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_nSlots = 0;
			m_alSelectedTreeResourceArray = new ArrayList();
			m_ClipList = new CGAppointments();

		}

		public void InitializeDocView(string sText)
		{
			this.Text = this.DocManager.ConnectInfo.UserName;
			if (sText != null)
				this.Text += " - " + sText;
			if (DocManager.ConnectInfo.DivisionName != null)
				this.Text += " - " + DocManager.ConnectInfo.DivisionName;
		}

		public void InitializeDocView(CGDocument doc, 
			CGDocumentManager docMgr,
			DateTime dStartDate,
			CGAppointments cgAppts,
			string sText)
		{
			System.IntPtr pHandle = this.Handle;
			this.DocManager = docMgr;
			this.StartDate = dStartDate;
			this.Document = doc;
			this.Appointments = cgAppts;
			this.Text = this.DocManager.ConnectInfo.UserName;
			if (sText != null)
				this.Text += " - " + sText;
			if (DocManager.ConnectInfo.DivisionName != null)
				this.Text += " - " + DocManager.ConnectInfo.DivisionName;

			this.m_ConnectInfo = m_DocManager.ConnectInfo;
			m_bmxDelegate = new BMXNetConnectInfo.BMXNetEventDelegate(BMXNetEventHandler);
			m_ConnectInfo.BMXNetEvent += m_bmxDelegate;
		}

		private BMXNetConnectInfo.BMXNetEventDelegate m_bmxDelegate;
		delegate void OnUpdateScheduleDelegate();

		private void BMXNetEventHandler(Object obj, BMXNet.BMXNetEventArgs e)
		{
			try
			{
				if (e.BMXEvent == "BMXNet AutoFire")
				{
					Debug.Write("CGView caught AutoFire event.\n");
					if (this == null)
						return;
					OnUpdateScheduleDelegate ousd = new OnUpdateScheduleDelegate(OnUpdateSchedule);
					this.BeginInvoke(ousd);
					return;
				}

				if (e.BMXEvent != "BSDX SCHEDULE")
				{
					return;
				}
				string sResourceName;
				for (int j=0; j < m_Document.m_sResourcesArray.Count; j++)
				{
					sResourceName = m_Document.m_sResourcesArray[j].ToString();
					if (e.BMXParam == sResourceName)
					{
						OnUpdateScheduleDelegate ousd = new OnUpdateScheduleDelegate(OnUpdateSchedule);
						if (this == null)
							return;
						this.BeginInvoke(ousd);
						Debug.Write("CGView caught BSDX SCHEDULE event.\n");
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
        }

        #endregion initialization

        #region Windows Form Designer generated code
        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CGView));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuOpenSchedule = new System.Windows.Forms.MenuItem();
            this.mnuOpenMultipleSchedules = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuRPMSServer = new System.Windows.Forms.MenuItem();
            this.mnuRPMSLogin = new System.Windows.Forms.MenuItem();
            this.mnuRPMSDivision = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuSchedulingManagment = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnuPrintClinicSchedules = new System.Windows.Forms.MenuItem();
            this.mnuPrintClinicLetters = new System.Windows.Forms.MenuItem();
            this.mnuPrintRebookLetters = new System.Windows.Forms.MenuItem();
            this.mnuPrintCancellationLetters = new System.Windows.Forms.MenuItem();
            this.mnuPrintPatientLetter = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.mnuClose = new System.Windows.Forms.MenuItem();
            this.mnuAppointment = new System.Windows.Forms.MenuItem();
            this.mnuNewAppointment = new System.Windows.Forms.MenuItem();
            this.mnuEditAppointment = new System.Windows.Forms.MenuItem();
            this.mnuDeleteAppointment = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuNoShow = new System.Windows.Forms.MenuItem();
            this.mnuNoShowUndo = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.mnuCopyAppointment = new System.Windows.Forms.MenuItem();
            this.mnuWalkIn = new System.Windows.Forms.MenuItem();
            this.mnuFindAppt = new System.Windows.Forms.MenuItem();
            this.mnuCheckIn = new System.Windows.Forms.MenuItem();
            this.mnuViewPatientAppts = new System.Windows.Forms.MenuItem();
            this.mnuCalendar = new System.Windows.Forms.MenuItem();
            this.mnuDisplayWalkIns = new System.Windows.Forms.MenuItem();
            this.mnu1Day = new System.Windows.Forms.MenuItem();
            this.mnu5Day = new System.Windows.Forms.MenuItem();
            this.mnu7Day = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnuTimeScale = new System.Windows.Forms.MenuItem();
            this.mnu10Minute = new System.Windows.Forms.MenuItem();
            this.mnu15Minute = new System.Windows.Forms.MenuItem();
            this.mnu20Minute = new System.Windows.Forms.MenuItem();
            this.mnu30Minute = new System.Windows.Forms.MenuItem();
            this.mnuViewScheduleTree = new System.Windows.Forms.MenuItem();
            this.mnuViewRightPanel = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.mnuTest = new System.Windows.Forms.MenuItem();
            this.mnuTest1 = new System.Windows.Forms.MenuItem();
            this.tvSchedules = new System.Windows.Forms.TreeView();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.ctxOpenSchedule = new System.Windows.Forms.MenuItem();
            this.ctxEditAvailability = new System.Windows.Forms.MenuItem();
            this.ctxProperties = new System.Windows.Forms.MenuItem();
            this.ctxFindAppt = new System.Windows.Forms.MenuItem();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelClip = new System.Windows.Forms.Panel();
            this.lstClip = new System.Windows.Forms.ListBox();
            this.ctxApptClipMenu = new System.Windows.Forms.ContextMenu();
            this.mnuRemoveClipItem = new System.Windows.Forms.MenuItem();
            this.mnuClearClipItems = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblResource = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.calendarGrid1 = new IndianHealthService.ClinicalScheduling.CalendarGrid();
            this.ctxCalendarGrid = new System.Windows.Forms.ContextMenu();
            this.ctxCalGridAdd = new System.Windows.Forms.MenuItem();
            this.ctxCalGridEdit = new System.Windows.Forms.MenuItem();
            this.ctxCalGridDelete = new System.Windows.Forms.MenuItem();
            this.ctxCalGridCheckIn = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.ctxCalGridNoShow = new System.Windows.Forms.MenuItem();
            this.ctxCalGridNoShowUndo = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.ctxCalGridWalkin = new System.Windows.Forms.MenuItem();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelRight.SuspendLayout();
            this.panelClip.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuAppointment,
            this.mnuCalendar,
            this.mnuHelp,
            this.mnuTest});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuOpenSchedule,
            this.mnuOpenMultipleSchedules,
            this.menuItem1,
            this.mnuRPMSServer,
            this.mnuRPMSLogin,
            this.mnuRPMSDivision,
            this.menuItem3,
            this.mnuSchedulingManagment,
            this.menuItem6,
            this.mnuPrintClinicSchedules,
            this.mnuPrintClinicLetters,
            this.mnuPrintRebookLetters,
            this.mnuPrintCancellationLetters,
            this.mnuPrintPatientLetter,
            this.menuItem7,
            this.mnuClose});
            this.mnuFile.Text = "&File";
            this.mnuFile.Popup += new System.EventHandler(this.mnuFile_Popup);
            // 
            // mnuOpenSchedule
            // 
            this.mnuOpenSchedule.Enabled = false;
            this.mnuOpenSchedule.Index = 0;
            this.mnuOpenSchedule.Text = "&Open Schedule";
            this.mnuOpenSchedule.Visible = false;
            this.mnuOpenSchedule.Click += new System.EventHandler(this.mnuOpenSchedule_Click);
            // 
            // mnuOpenMultipleSchedules
            // 
            this.mnuOpenMultipleSchedules.Index = 1;
            this.mnuOpenMultipleSchedules.Text = "Open M&ultiple Schedules";
            this.mnuOpenMultipleSchedules.Click += new System.EventHandler(this.mnuOpenMultipleSchedules_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // mnuRPMSServer
            // 
            this.mnuRPMSServer.Index = 3;
            this.mnuRPMSServer.Text = "Change RPMS &Server";
            this.mnuRPMSServer.Click += new System.EventHandler(this.mnuRPMSServer_Click);
            // 
            // mnuRPMSLogin
            // 
            this.mnuRPMSLogin.Index = 4;
            this.mnuRPMSLogin.Text = "Change RPMS &Login";
            this.mnuRPMSLogin.Click += new System.EventHandler(this.mnuRPMSLogin_Click);
            // 
            // mnuRPMSDivision
            // 
            this.mnuRPMSDivision.Index = 5;
            this.mnuRPMSDivision.Text = "Change RPMS &Division";
            this.mnuRPMSDivision.Click += new System.EventHandler(this.mnuRPMSDivision_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 6;
            this.menuItem3.Text = "-";
            // 
            // mnuSchedulingManagment
            // 
            this.mnuSchedulingManagment.Index = 7;
            this.mnuSchedulingManagment.Text = "Scheduling &Management";
            this.mnuSchedulingManagment.Click += new System.EventHandler(this.mnuSchedulingManagment_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 8;
            this.menuItem6.Text = "-";
            // 
            // mnuPrintClinicSchedules
            // 
            this.mnuPrintClinicSchedules.Index = 9;
            this.mnuPrintClinicSchedules.Text = "&Print Clinic Schedules";
            this.mnuPrintClinicSchedules.Click += new System.EventHandler(this.mnuPrintClinicSchedules_Click);
            // 
            // mnuPrintClinicLetters
            // 
            this.mnuPrintClinicLetters.Index = 10;
            this.mnuPrintClinicLetters.Text = "Print Rem&inder Letters";
            this.mnuPrintClinicLetters.Click += new System.EventHandler(this.mnuPrintClinicLetters_Click);
            // 
            // mnuPrintRebookLetters
            // 
            this.mnuPrintRebookLetters.Index = 11;
            this.mnuPrintRebookLetters.Text = "Print &Rebook Letters";
            this.mnuPrintRebookLetters.Click += new System.EventHandler(this.mnuPrintRebookLetters_Click);
            // 
            // mnuPrintCancellationLetters
            // 
            this.mnuPrintCancellationLetters.Index = 12;
            this.mnuPrintCancellationLetters.Text = "Print C&ancellation Letters";
            this.mnuPrintCancellationLetters.Click += new System.EventHandler(this.mnuPrintCancellationLetters_Click);
            // 
            // mnuPrintPatientLetter
            // 
            this.mnuPrintPatientLetter.Index = 13;
            this.mnuPrintPatientLetter.Text = "Print Patient Le&tter";
            this.mnuPrintPatientLetter.Click += new System.EventHandler(this.mnuPrintPatientLetter_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 14;
            this.menuItem7.Text = "-";
            // 
            // mnuClose
            // 
            this.mnuClose.Index = 15;
            this.mnuClose.Text = "&Close Schedule";
            this.mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
            // 
            // mnuAppointment
            // 
            this.mnuAppointment.Index = 1;
            this.mnuAppointment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuNewAppointment,
            this.mnuEditAppointment,
            this.mnuDeleteAppointment,
            this.menuItem5,
            this.mnuNoShow,
            this.mnuNoShowUndo,
            this.menuItem8,
            this.mnuCopyAppointment,
            this.mnuWalkIn,
            this.mnuFindAppt,
            this.mnuCheckIn,
            this.mnuViewPatientAppts});
            this.mnuAppointment.Text = "&Appointment";
            this.mnuAppointment.Popup += new System.EventHandler(this.mnuAppointment_Popup);
            // 
            // mnuNewAppointment
            // 
            this.mnuNewAppointment.Index = 0;
            this.mnuNewAppointment.Text = "&New Appointment";
            this.mnuNewAppointment.Click += new System.EventHandler(this.mnuNewAppointment_Click);
            // 
            // mnuEditAppointment
            // 
            this.mnuEditAppointment.Index = 1;
            this.mnuEditAppointment.Text = "&Edit Appointment";
            this.mnuEditAppointment.Click += new System.EventHandler(this.mnuEditAppointment_Click);
            // 
            // mnuDeleteAppointment
            // 
            this.mnuDeleteAppointment.Index = 2;
            this.mnuDeleteAppointment.Text = "Cance&l Appointment";
            this.mnuDeleteAppointment.Click += new System.EventHandler(this.mnuDeleteAppointment_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // mnuNoShow
            // 
            this.mnuNoShow.Index = 4;
            this.mnuNoShow.Text = "Mark as No Sho&w";
            this.mnuNoShow.Click += new System.EventHandler(this.mnuNoShow_Click);
            // 
            // mnuNoShowUndo
            // 
            this.mnuNoShowUndo.Index = 5;
            this.mnuNoShowUndo.Text = "&Undo No Show";
            this.mnuNoShowUndo.Click += new System.EventHandler(this.mnuNoShowUndo_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 6;
            this.menuItem8.Text = "-";
            // 
            // mnuCopyAppointment
            // 
            this.mnuCopyAppointment.Index = 7;
            this.mnuCopyAppointment.Text = "&Copy  Appointment to Clipboard";
            this.mnuCopyAppointment.Click += new System.EventHandler(this.mnuCopyAppointment_Click);
            // 
            // mnuWalkIn
            // 
            this.mnuWalkIn.Index = 8;
            this.mnuWalkIn.Text = "Create Wal&k-In Appointment";
            this.mnuWalkIn.Click += new System.EventHandler(this.mnuWalkIn_Click);
            // 
            // mnuFindAppt
            // 
            this.mnuFindAppt.Index = 9;
            this.mnuFindAppt.Text = "&Find Available Appointment";
            this.mnuFindAppt.Click += new System.EventHandler(this.mnuFindAppt_Click);
            // 
            // mnuCheckIn
            // 
            this.mnuCheckIn.Index = 10;
            this.mnuCheckIn.Text = "Check &In Patient";
            this.mnuCheckIn.Click += new System.EventHandler(this.mnuCheckIn_Click);
            // 
            // mnuViewPatientAppts
            // 
            this.mnuViewPatientAppts.Index = 11;
            this.mnuViewPatientAppts.Text = "&View Patient Appointments";
            this.mnuViewPatientAppts.Click += new System.EventHandler(this.mnuViewPatientAppts_Click);
            // 
            // mnuCalendar
            // 
            this.mnuCalendar.Index = 2;
            this.mnuCalendar.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuDisplayWalkIns,
            this.mnu1Day,
            this.mnu5Day,
            this.mnu7Day,
            this.menuItem4,
            this.mnuTimeScale,
            this.mnuViewScheduleTree,
            this.mnuViewRightPanel});
            this.mnuCalendar.Text = "&View";
            // 
            // mnuDisplayWalkIns
            // 
            this.mnuDisplayWalkIns.Checked = true;
            this.mnuDisplayWalkIns.Index = 0;
            this.mnuDisplayWalkIns.Text = "&Display Walk-Ins";
            this.mnuDisplayWalkIns.Click += new System.EventHandler(this.mnuDisplayWalkIns_Click);
            // 
            // mnu1Day
            // 
            this.mnu1Day.Index = 1;
            this.mnu1Day.Text = "&1-Day View";
            this.mnu1Day.Click += new System.EventHandler(this.mnu1Day_Click);
            // 
            // mnu5Day
            // 
            this.mnu5Day.Index = 2;
            this.mnu5Day.Text = "&5-Day View";
            this.mnu5Day.Click += new System.EventHandler(this.mnu5Day_Click);
            // 
            // mnu7Day
            // 
            this.mnu7Day.Index = 3;
            this.mnu7Day.Text = "&7-Day View";
            this.mnu7Day.Click += new System.EventHandler(this.mnu7Day_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "-";
            // 
            // mnuTimeScale
            // 
            this.mnuTimeScale.Index = 5;
            this.mnuTimeScale.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnu10Minute,
            this.mnu15Minute,
            this.mnu20Minute,
            this.mnu30Minute});
            this.mnuTimeScale.Text = "&Time Scale";
            // 
            // mnu10Minute
            // 
            this.mnu10Minute.Index = 0;
            this.mnu10Minute.Text = "&10-Minute";
            this.mnu10Minute.Click += new System.EventHandler(this.mnu10Minute_Click);
            // 
            // mnu15Minute
            // 
            this.mnu15Minute.Index = 1;
            this.mnu15Minute.Text = "1&5-Minute";
            this.mnu15Minute.Click += new System.EventHandler(this.mnu15Minute_Click);
            // 
            // mnu20Minute
            // 
            this.mnu20Minute.Index = 2;
            this.mnu20Minute.Text = "&20-Minute";
            this.mnu20Minute.Click += new System.EventHandler(this.mnu20Minute_Click);
            // 
            // mnu30Minute
            // 
            this.mnu30Minute.Index = 3;
            this.mnu30Minute.Text = "&30-Minute";
            this.mnu30Minute.Click += new System.EventHandler(this.mnu30Minute_Click);
            // 
            // mnuViewScheduleTree
            // 
            this.mnuViewScheduleTree.Index = 6;
            this.mnuViewScheduleTree.Text = "&Schedule Tree";
            this.mnuViewScheduleTree.Click += new System.EventHandler(this.mnuViewScheduleTree_Click);
            // 
            // mnuViewRightPanel
            // 
            this.mnuViewRightPanel.Index = 7;
            this.mnuViewRightPanel.Text = "&Appointment Clipboard";
            this.mnuViewRightPanel.Click += new System.EventHandler(this.mnuViewRightPanel_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 3;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 0;
            this.mnuHelpAbout.Text = "&About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnuTest
            // 
            this.mnuTest.Enabled = false;
            this.mnuTest.Index = 4;
            this.mnuTest.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuTest1});
            this.mnuTest.Text = "&Test";
            this.mnuTest.Visible = false;
            // 
            // mnuTest1
            // 
            this.mnuTest1.Index = 0;
            this.mnuTest1.Text = "Test1";
            this.mnuTest1.Click += new System.EventHandler(this.mnuTest1_Click);
            // 
            // tvSchedules
            // 
            this.tvSchedules.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tvSchedules.ContextMenu = this.contextMenu1;
            this.tvSchedules.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvSchedules.HotTracking = true;
            this.tvSchedules.Location = new System.Drawing.Point(0, 0);
            this.tvSchedules.Name = "tvSchedules";
            this.tvSchedules.Size = new System.Drawing.Size(128, 473);
            this.tvSchedules.Sorted = true;
            this.tvSchedules.TabIndex = 1;
            this.tvSchedules.DoubleClick += new System.EventHandler(this.tvSchedules_DoubleClick);
            this.tvSchedules.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSchedules_AfterSelect);
            this.tvSchedules.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvSchedules_BeforeSelect);
            this.tvSchedules.Click += new System.EventHandler(this.tvSchedules_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxOpenSchedule,
            this.ctxEditAvailability,
            this.ctxProperties,
            this.ctxFindAppt});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // ctxOpenSchedule
            // 
            this.ctxOpenSchedule.DefaultItem = true;
            this.ctxOpenSchedule.Index = 0;
            this.ctxOpenSchedule.Text = "&Open Schedule";
            this.ctxOpenSchedule.Click += new System.EventHandler(this.ctxOpenSchedule_Click);
            // 
            // ctxEditAvailability
            // 
            this.ctxEditAvailability.Index = 1;
            this.ctxEditAvailability.Text = "&Edit Resource Availability";
            this.ctxEditAvailability.Click += new System.EventHandler(this.ctxEditAvailability_Click);
            // 
            // ctxProperties
            // 
            this.ctxProperties.Index = 2;
            this.ctxProperties.Text = "&Properties";
            this.ctxProperties.Click += new System.EventHandler(this.ctxProperties_Click);
            // 
            // ctxFindAppt
            // 
            this.ctxFindAppt.Index = 3;
            this.ctxFindAppt.Text = "Find Available Appointment";
            this.ctxFindAppt.Click += new System.EventHandler(this.ctxFindAppt_Click);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.panelClip);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(676, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(128, 473);
            this.panelRight.TabIndex = 3;
            this.panelRight.Visible = false;
            // 
            // panelClip
            // 
            this.panelClip.Controls.Add(this.lstClip);
            this.panelClip.Controls.Add(this.label1);
            this.panelClip.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelClip.Location = new System.Drawing.Point(0, 0);
            this.panelClip.Name = "panelClip";
            this.panelClip.Size = new System.Drawing.Size(128, 448);
            this.panelClip.TabIndex = 0;
            // 
            // lstClip
            // 
            this.lstClip.AllowDrop = true;
            this.lstClip.ContextMenu = this.ctxApptClipMenu;
            this.lstClip.DisplayMember = "PatientName";
            this.lstClip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClip.Location = new System.Drawing.Point(0, 32);
            this.lstClip.Name = "lstClip";
            this.lstClip.Size = new System.Drawing.Size(128, 407);
            this.lstClip.TabIndex = 0;
            this.lstClip.SelectedIndexChanged += new System.EventHandler(this.lstClip_SelectedIndexChanged);
            this.lstClip.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstClip_DragDrop);
            this.lstClip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstClip_MouseMove);
            this.lstClip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstClip_MouseDown);
            this.lstClip.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstClip_DragEnter);
            // 
            // ctxApptClipMenu
            // 
            this.ctxApptClipMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuRemoveClipItem,
            this.mnuClearClipItems});
            this.ctxApptClipMenu.Popup += new System.EventHandler(this.ctxApptClipMenu_Popup);
            // 
            // mnuRemoveClipItem
            // 
            this.mnuRemoveClipItem.Index = 0;
            this.mnuRemoveClipItem.Text = "Remove Item";
            this.mnuRemoveClipItem.Click += new System.EventHandler(this.mnuRemoveClipItem_Click);
            // 
            // mnuClearClipItems
            // 
            this.mnuClearClipItems.Index = 1;
            this.mnuClearClipItems.Text = "Clear All";
            this.mnuClearClipItems.Click += new System.EventHandler(this.mnuClearClipItems_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Appointment Clipboard";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.dateTimePicker1);
            this.panelTop.Controls.Add(this.lblResource);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(128, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(548, 24);
            this.panelTop.TabIndex = 6;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Dock = System.Windows.Forms.DockStyle.Right;
            this.dateTimePicker1.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(420, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(128, 20);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // lblResource
            // 
            this.lblResource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResource.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblResource.Location = new System.Drawing.Point(8, 5);
            this.lblResource.Name = "lblResource";
            this.lblResource.Size = new System.Drawing.Size(456, 19);
            this.lblResource.TabIndex = 2;
            this.lblResource.Text = "lblResource";
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.calendarGrid1);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(136, 24);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(537, 425);
            this.panelCenter.TabIndex = 7;
            // 
            // calendarGrid1
            // 
            this.calendarGrid1.AllowDrop = true;
            this.calendarGrid1.Appointments = null;
            this.calendarGrid1.ApptDragSource = null;
            this.calendarGrid1.AutoScroll = true;
            this.calendarGrid1.AutoScrollMinSize = new System.Drawing.Size(600, 1898);
            this.calendarGrid1.AvailabilityArray = null;
            this.calendarGrid1.BackColor = System.Drawing.SystemColors.Window;
            this.calendarGrid1.Columns = 5;
            this.calendarGrid1.ContextMenu = this.ctxCalendarGrid;
            this.calendarGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calendarGrid1.DrawWalkIns = true;
            this.calendarGrid1.GridBackColor = null;
            this.calendarGrid1.GridEnter = false;
            this.calendarGrid1.Location = new System.Drawing.Point(0, 0);
            this.calendarGrid1.Name = "calendarGrid1";
            this.calendarGrid1.Resources = ((System.Collections.ArrayList)(resources.GetObject("calendarGrid1.Resources")));
            this.calendarGrid1.SelectedAppointment = 0;
            this.calendarGrid1.Size = new System.Drawing.Size(537, 425);
            this.calendarGrid1.StartDate = new System.DateTime(2003, 1, 27, 0, 0, 0, 0);
            this.calendarGrid1.TabIndex = 0;
            this.calendarGrid1.TimeScale = 20;
            this.calendarGrid1.DoubleClick += new System.EventHandler(this.calendarGrid1_DoubleClick);
            this.calendarGrid1.CGSelectionChanged += new IndianHealthService.ClinicalScheduling.CGSelectionChangedHandler(this.calendarGrid1_CGSelectionChanged);
            this.calendarGrid1.CGAppointmentChanged += new IndianHealthService.ClinicalScheduling.CGAppointmentChangedHandler(this.calendarGrid1_CGAppointmentChanged);
            this.calendarGrid1.CGAppointmentAdded += new IndianHealthService.ClinicalScheduling.CGAppointmentChangedHandler(this.calendarGrid1_CGAppointmentAdded);
            // 
            // ctxCalendarGrid
            // 
            this.ctxCalendarGrid.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxCalGridAdd,
            this.ctxCalGridEdit,
            this.ctxCalGridDelete,
            this.ctxCalGridCheckIn,
            this.menuItem2,
            this.ctxCalGridNoShow,
            this.ctxCalGridNoShowUndo,
            this.menuItem9,
            this.ctxCalGridWalkin});
            this.ctxCalendarGrid.Popup += new System.EventHandler(this.ctxCalendarGrid_Popup);
            // 
            // ctxCalGridAdd
            // 
            this.ctxCalGridAdd.Index = 0;
            this.ctxCalGridAdd.Text = "Add Appointment";
            this.ctxCalGridAdd.Click += new System.EventHandler(this.ctxCalGridAdd_Click);
            // 
            // ctxCalGridEdit
            // 
            this.ctxCalGridEdit.Index = 1;
            this.ctxCalGridEdit.Text = "Edit Appointment";
            this.ctxCalGridEdit.Click += new System.EventHandler(this.ctxCalGridEdit_Click);
            // 
            // ctxCalGridDelete
            // 
            this.ctxCalGridDelete.Index = 2;
            this.ctxCalGridDelete.Text = "Cancel Appointment";
            this.ctxCalGridDelete.Click += new System.EventHandler(this.ctxCalGridDelete_Click);
            // 
            // ctxCalGridCheckIn
            // 
            this.ctxCalGridCheckIn.Index = 3;
            this.ctxCalGridCheckIn.Text = "Check In Patient";
            this.ctxCalGridCheckIn.Click += new System.EventHandler(this.ctxCalGridCheckIn_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            this.menuItem2.Text = "-";
            // 
            // ctxCalGridNoShow
            // 
            this.ctxCalGridNoShow.Index = 5;
            this.ctxCalGridNoShow.Text = "Mark as No Show";
            this.ctxCalGridNoShow.Click += new System.EventHandler(this.ctxCalGridNoShow_Click);
            // 
            // ctxCalGridNoShowUndo
            // 
            this.ctxCalGridNoShowUndo.Index = 6;
            this.ctxCalGridNoShowUndo.Text = "Undo NoShow";
            this.ctxCalGridNoShowUndo.Click += new System.EventHandler(this.ctxCalGridNoShowUndo_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 7;
            this.menuItem9.Text = "-";
            // 
            // ctxCalGridWalkin
            // 
            this.ctxCalGridWalkin.Index = 8;
            this.ctxCalGridWalkin.Text = "Create Wal&k-In Appointment";
            this.ctxCalGridWalkin.Click += new System.EventHandler(this.ctxCalGridWalkin_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.statusBar1);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(136, 449);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(537, 24);
            this.panelBottom.TabIndex = 8;
            // 
            // statusBar1
            // 
            this.statusBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBar1.Location = new System.Drawing.Point(0, 0);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(537, 24);
            this.statusBar1.SizingGrip = false;
            this.statusBar1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(128, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 449);
            this.splitter1.TabIndex = 9;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(673, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 449);
            this.splitter2.TabIndex = 10;
            this.splitter2.TabStop = false;
            // 
            // CGView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(804, 473);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.tvSchedules);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "CGView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CGView";
            this.CursorChanged += new System.EventHandler(this.CGView_CursorChanged);
            this.Load += new System.EventHandler(this.CGView_Load);
            this.Activated += new System.EventHandler(this.CGView_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CGView_Closing);
            this.panelRight.ResumeLayout(false);
            this.panelClip.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private	CGDocument			m_Document;
		private CGDocumentManager	m_DocManager;
		private int					m_nSlots;
		bool						bSchedulesClicked = false;
		private ArrayList			m_alSelectedTreeResourceArray = new ArrayList();
		private string				m_sDocName;
		private CGAppointments		m_ClipList;
		private bool				m_bDragDropStart = false;
		private Hashtable			m_htOverbook;
		private Hashtable			m_htModifySchedule;
		private Hashtable			m_htChangeAppts;
		private BMXNetConnectInfo	m_ConnectInfo = null;
		public BMXNetConnectInfo.BMXNetEventDelegate	BMXNetEvent;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Access the CalendarGrid associated with this view
		/// </summary>
		public CalendarGrid CGrid
		{
			get
			{
				return this.calendarGrid1;
			}
		}

		/// <summary>
		/// Accesses the document associated with this view
		/// </summary>
		public CGDocument Document
		{
			get
			{
				return this.m_Document;
			}
			set
			{
				this.m_Document = value;
			}
		}

		public CGDocumentManager DocManager
		{
			get
			{
				return m_DocManager;
			}
			set
			{
				m_DocManager = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return this.calendarGrid1.StartDate;
			}
			set
			{
				this.calendarGrid1.StartDate = value;
			}
		}

		public CGAppointments Appointments
		{
			get
			{
				return this.calendarGrid1.Appointments;
			}
			set
			{
				this.calendarGrid1.Appointments = value;
			}
		}


		#endregion

		#region AppointmentMenu Handlers

		private void mnuAppointment_Popup(object sender, System.EventArgs e)
		{
			bool bEnabled = (this.Document.Resources.Count > 0)? true : false ;
			this.mnuFindAppt.Enabled = bEnabled;

			//Toggle availability of make, edit, checkin and delete appointments
			//based on whether a range is selected.

			mnuNewAppointment.Enabled = AddAppointmentEnabled();
			this.mnuWalkIn.Enabled = mnuNewAppointment.Enabled;
			bool bEditAppointments = this.EditAppointmentEnabled();

			mnuDeleteAppointment.Enabled = bEditAppointments;
			mnuCheckIn.Enabled = bEditAppointments;
			mnuEditAppointment.Enabled = bEditAppointments;
			mnuNoShow.Enabled = bEditAppointments;
			mnuNoShowUndo.Enabled = bEditAppointments;
		}

		private void mnuCheckIn_Click(object sender, System.EventArgs e)
		{
			AppointmentCheckIn();
		}

		private void mnuCopyAppointment_Click(object sender, System.EventArgs e)
		{
			//For each appointment in the grid's selected list,
			//add to the clip list
			//and add to m_ClipList
			try
			{
				foreach (CGAppointment a in this.calendarGrid1.SelectedAppointments.AppointmentTable.Values)
				{
					if (m_ClipList.AppointmentTable.Contains((int) a.AppointmentKey))
					{
						return;
					}
					m_ClipList.AddAppointment(a);
					lstClip.Items.Add(a.PatientName);
				}
			}
			catch (Exception ex)
			{
				string s = ex.Message;
				Debug.Write(s);
				return;
			}
		}

		private void mnuDeleteAppointment_Click(object sender, System.EventArgs e)
		{
			AppointmentDelete();
		}

		private void mnuEditAppointment_Click(object sender, System.EventArgs e)
		{
			AppointmentEdit();
		}

		private void mnuNewAppointment_Click(object sender, System.EventArgs e)
		{
			AppointmentAddNew();
		}

		private void mnuNoShow_Click(object sender, System.EventArgs e)
		{
			AppointmentNoShow(true);
		}

		private void mnuNoShowUndo_Click(object sender, System.EventArgs e)
		{
			AppointmentNoShow(false);
		}

		#endregion AppointmentMenu Handlers

		#region ContextMenu1 Handlers

		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			//Enable/disable OpenSchedule and Find Appointment options
			bool bEnabled = (m_alSelectedTreeResourceArray.Count > 0)? true : false ;
			this.ctxOpenSchedule.Enabled = bEnabled;
			this.ctxFindAppt.Enabled = bEnabled;

			//properties not supported now
			this.ctxProperties.Enabled = false;
			this.ctxProperties.Visible = false;

			//Enable/disable Availability menu option
			if (m_alSelectedTreeResourceArray.Count != 1)
			{
				this.ctxEditAvailability.Enabled = false;
				return;
			}
			
			if (this.DocManager.ScheduleManager == true)
			{
				ctxEditAvailability.Enabled = true;
				return;
			}

			string sResource = (string) m_alSelectedTreeResourceArray[0];
			DataTable dt = this.DocManager.GlobalDataSet.Tables["ResourceUser"];
			DataView dv = new DataView(dt, "", "RESOURCENAME ASC", DataViewRowState.OriginalRows);
			string sDuz = this.DocManager.ConnectInfo.DUZ;
			bool bModSchedule = false;
			DataRowView[] drvA = dv.FindRows(sResource);
			if (drvA.Length == 0)
			{
				this.ctxEditAvailability.Enabled = false;
			}
			else
			{
				string sModSchedule = "NO";
				foreach (DataRowView drv in drvA)
				{
					if (drv["USERID"].ToString() == sDuz)
					{
						sModSchedule = drv["MODIFY_SCHEDULE"].ToString();
						break;
					}
				}

				bModSchedule = (sModSchedule == "YES")?true:false;
				this.ctxEditAvailability.Enabled = bModSchedule;
			}
		}

		private void ctxEditAvailability_Click(object sender, System.EventArgs e)
		{
			this.EditScheduleAvailability();
		}

		private void ctxOpenSchedule_Click(object sender, System.EventArgs e)
		{
			OpenSelectedSchedule(m_alSelectedTreeResourceArray, DateTime.Today);
		}

		private void ctxProperties_Click(object sender, System.EventArgs e)
		{
			//TODO: Implement Properties dialog
			MessageBox.Show("TODO: Implement Properties dialog");
		}

		private void ctxFindAppt_Click(object sender, System.EventArgs e)
		{
			FindAvailableAppointment(m_alSelectedTreeResourceArray);
		}

		#endregion ContextMenu1 Handlers

		#region ctxApptClipMenu Handlers

		private void mnuClearClipItems_Click(object sender, System.EventArgs e)
		{
			this.m_ClipList.ClearAllAppointments();
			lstClip.Items.Clear();
		}

		private void mnuRemoveClipItem_Click(object sender, System.EventArgs e)
		{
			int i = lstClip.SelectedIndex;
			CGAppointment a = (CGAppointment) lstClip.SelectedItem;
			int nKey = a.AppointmentKey;
			if (i > -1)
			{
				m_ClipList.RemoveAppointment(nKey);
				lstClip.Items.RemoveAt(i);
			}
		}

		private void ctxApptClipMenu_Popup(object sender, System.EventArgs e)
		{
			mnuClearClipItems.Enabled = (m_ClipList.AppointmentTable.Count > 0);
			mnuRemoveClipItem.Enabled = (lstClip.SelectedIndex > -1);
		}

		#endregion ctxApptClipMenu Handlers

		#region ctxCalGridMenu Handlers

		private void ctxCalendarGrid_Popup(object sender, System.EventArgs e)
		{
			//Toggle availability of make, edit, checkin and delete appointments
			//based on whether appropriate element is selected.
			ctxCalGridAdd.Enabled = AddAppointmentEnabled();
			bool bEditAppointments = (EditAppointmentEnabled() && (calendarGrid1.SelectedAppointment > 0)) ;
			ctxCalGridDelete.Enabled = bEditAppointments;
			ctxCalGridEdit.Enabled = bEditAppointments;
			ctxCalGridCheckIn.Enabled = bEditAppointments;
			ctxCalGridNoShow.Enabled = bEditAppointments;
			ctxCalGridNoShowUndo.Enabled = bEditAppointments;
			ctxCalGridWalkin.Enabled = ctxCalGridAdd.Enabled;
		}

		private void ctxCalGridAdd_Click(object sender, System.EventArgs e)
		{
			AppointmentAddNew();
		}

		private void calendarGrid1_DoubleClick(object sender, System.EventArgs e)
		{
			if (calendarGrid1.SelectedAppointment > 0)
				AppointmentEdit();
		}

		private void ctxCalGridEdit_Click(object sender, System.EventArgs e)
		{
			AppointmentEdit();
		}

		private void ctxCalGridDelete_Click(object sender, System.EventArgs e)
		{
			AppointmentDelete();
		}

		private void ctxCalGridCheckIn_Click(object sender, System.EventArgs e)
		{
			AppointmentCheckIn();
		}

		private void ctxCalGridNoShow_Click(object sender, System.EventArgs e)
		{
			AppointmentNoShow(true);
		}

		private void ctxCalGridNoShowUndo_Click(object sender, System.EventArgs e)
		{
			AppointmentNoShow(false);
		}

		#endregion ctxCalGridMenu Handlers

		#region Methods

        private bool EditAppointmentEnabled()
        {
            try
            {
                //Call here if there is a selected appointment in the grid
                if (calendarGrid1.SelectedAppointment < 1)
                    return false;
                CGAppointment appt = (CGAppointment)this.Appointments.AppointmentTable[calendarGrid1.SelectedAppointment];
                string sResource = appt.Resource;
                return EditAppointmentEnabled(sResource);

            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
                return false;
            }
        }

        private bool EditAppointmentEnabled(string sResource)
        {

            bool bManager = this.DocManager.ScheduleManager;
            if (bManager == true)
            {
                return (true);
            }
            else
            {
                bool bModAppts;
                bModAppts = (bool)this.m_htChangeAppts[sResource];
                return bModAppts;
            }
        }

        private bool AddAppointmentEnabled()
        {
            if (this.calendarGrid1.SelectedRange.Cells.CellCount < 1)
                return false;

            bool bManager = this.DocManager.ScheduleManager;
            if (bManager == true)
            {
                return (true);
            }
            else
            {
                DateTime dStart = DateTime.Today;
                DateTime dEnd = DateTime.Today;
                string sResource = "";
                bool bRet = this.calendarGrid1.GetSelectedTime(out dStart, out dEnd, out sResource);
                if (bRet == false)
                {
                    return false;
                }
                bool bSlotsAvailable;
                bool bOverbook;
                bool bModSchedule;
                bool bModAppts;
                bOverbook = (bool)this.m_htOverbook[sResource];
                bModSchedule = (bool)this.m_htModifySchedule[sResource];
                bModAppts = (bool)this.m_htChangeAppts[sResource];
                if (bModAppts == false)
                    return false;

                bSlotsAvailable = (this.m_nSlots > 0);
                return ((bSlotsAvailable) || (bModSchedule) || (bOverbook));
            }
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

		void UpdateStatusBar(DateTime dStart, DateTime dEnd, string sAccessType, string sAvailabilityMessage)
		{
			string sMsg =  dStart.ToShortTimeString() + " to " + dEnd.ToShortTimeString();
			if (m_nSlots > 0)
			{
				sMsg = sMsg + ": " + m_nSlots.ToString() + " slot";
				sMsg = sMsg + ((m_nSlots > 1)?"s " : " ");
				sMsg = sMsg + "available";
				if (sAccessType != "")
				{
					sMsg = sMsg + " for " + sAccessType;
				}
				sMsg = sMsg + ".";
				if (sAvailabilityMessage != "")
				{
					sMsg = sMsg + "  Note: " + sAvailabilityMessage;
				}
			}
			else
			{
				sMsg += ": No appointment slots available.";
			}

			this.statusBar1.Text = sMsg;
		}

		private void EditScheduleAvailability()
		{
			CGAVDocument doc = new CGAVDocument();
			try 
			{
				//If resource already open, then navigate to its window
				CGAVView v =this.DocManager.GetAVViewByResource(m_alSelectedTreeResourceArray);
			
				if (v != null) 
				{
					v.Activate();
				}
				else 
				{
					//If not already open, get a lock and open it
					doc.DocManager = this.DocManager;
					for (int j=0; j < m_alSelectedTreeResourceArray.Count; j++)
					{
						doc.AddResource((string) m_alSelectedTreeResourceArray[j]);
					}
					doc.DocName = this.m_sDocName;

					//Get preferred time scale from resource info

					DataTable dt = this.DocManager.GlobalDataSet.Tables["Resources"];
					DataView dv = new DataView(dt, "", "RESOURCE_NAME ASC", DataViewRowState.OriginalRows);
					int nScale = 60;
					int nTest=0;
					string sResource;
					int nDataRow;
					DataRowView drv;
					string sResourceID="";
					for (int j=0; j < m_alSelectedTreeResourceArray.Count; j++)
					{
						sResource = (string) m_alSelectedTreeResourceArray[j];
						nDataRow = dv.Find(sResource);
						Debug.Assert(nDataRow != -1);
						drv = dv[nDataRow];
						if (drv["TIMESCALE"].ToString() == "")
						{
							nTest = 15; //15 minute default
						}
						else
						{
							nTest = (int) drv["TIMESCALE"];
						}
						nScale = (nTest < nScale)?nTest : nScale ;
						sResourceID = drv["RESOURCEID"].ToString();
					}
					
					doc.ResourceID = Convert.ToInt32(sResourceID);

					bool bLock = DocManager.ConnectInfo.bmxNetLib.Lock("^BSDXRES(" + sResourceID + ")", "+");
					if (bLock == false)
					{
						throw new BMXNetException("Another user is currently editing availability for this resource.  Try later.");
					}					
					
					doc.OnOpenDocument();
					v =this.DocManager.GetAVViewByResource(m_alSelectedTreeResourceArray);
					CalendarGrid cg = v.CGrid;

					cg.TimeScale = nScale;

					//Position grid to 0700
					PositionGrid(cg, 7);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to edit availability for " + m_sDocName + " schedule.  " +  ex.Message, "Clinical Scheduling");
				this.m_DocManager.CloseAllViews(doc);
				return;
			}
		}

		private void OpenSelectedSchedule(ArrayList sSelectedTreeResourceArray, DateTime dDate)
		{
			//If resource already open, then navigate to its window
			CGDocument doc;
			CGView v =this.DocManager.GetViewByResource(sSelectedTreeResourceArray);
			if (v != null) 
			{
				v.Activate();
				v.dateTimePicker1.Value = dDate;
			}
			else 
			{
				//If not already open, open it
				//If current document has a resource, then open a new window 
				//with the selected resource.
				//Otherwise just use the current document.
				if (this.Document.m_sResourcesArray.Count > 0)
				{
					doc = new CGDocument();
					doc.DocManager = this.DocManager;
				}
				else 
				{
					doc = this.Document;
				}
				for (int j=0; j < sSelectedTreeResourceArray.Count; j++)
				{
					doc.AddResource((string) sSelectedTreeResourceArray[j]);
				}
				doc.DocName = this.m_sDocName;
				try
				{
					doc.OnOpenDocument();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unable to open " + m_sDocName + " schedule.  " +  ex.Message, "Clinical Scheduling");
					this.m_DocManager.CloseAllViews(doc);
					return;
				}
				v =this.DocManager.GetViewByResource(sSelectedTreeResourceArray);
				v.dateTimePicker1.Value = dDate;

				//Get preferred time scale from resource info
				//If more than one resource, get smallest time scale
				CalendarGrid cg = v.CGrid;
				DataTable dt = this.DocManager.GlobalDataSet.Tables["Resources"];
				DataView dv = new DataView(dt, "", "RESOURCE_NAME ASC", DataViewRowState.OriginalRows);
				int nScale = 60;
				int nTest=0;
				string sResource;
				int nDataRow;
				DataRowView drv;
				for (int j=0; j < sSelectedTreeResourceArray.Count; j++)
				{
					sResource = (string) sSelectedTreeResourceArray[j];
					nDataRow = dv.Find(sResource);
					Debug.Assert(nDataRow != -1);
					drv = dv[nDataRow];
					if (drv["TIMESCALE"].ToString() == "")
					{
						nTest = 15; //15 minute default
					}
					else
					{
						nTest = (int) drv["TIMESCALE"];
					}
					nScale = (nTest < nScale)?nTest : nScale ;
				}

				cg.TimeScale = nScale;

				PositionGrid(cg, 7);

				//Get the OverBook and ModifySchedule permissions from ResourceUser table
				//and populate the hashtables
				string	sOverbook;
				string	sModSchedule;
				string	sModAppts;
				bool	bOverbook;
				bool	bModSchedule;
				bool	bModAppts;
				v.m_htOverbook = new Hashtable(sSelectedTreeResourceArray.Count);
				v.m_htModifySchedule = new Hashtable(sSelectedTreeResourceArray.Count);
				v.m_htChangeAppts = new Hashtable(sSelectedTreeResourceArray.Count);
				dt = this.DocManager.GlobalDataSet.Tables["ResourceUser"];
				dv = new DataView(dt, "", "RESOURCENAME ASC", DataViewRowState.OriginalRows);
				dv.RowFilter = "USERNAME = '" + this.DocManager.ConnectInfo.UserName + "'";
				for (int j=0; j < dv.Count; j++)
				{
					drv = dv[j];
					sResource = drv["RESOURCENAME"].ToString();
					sOverbook = drv["OVERBOOK"].ToString();
					bOverbook = (sOverbook == "YES")?true:false;
					sModSchedule = drv["MODIFY_SCHEDULE"].ToString();
					bModSchedule = (sModSchedule == "YES")?true:false;
					sModAppts = drv["MODIFY_APPOINTMENTS"].ToString();
					bModAppts = (sModAppts == "YES")?true:false;
					v.m_htOverbook[sResource] = bOverbook;
					v.m_htModifySchedule[sResource] = bModSchedule;
					v.m_htChangeAppts[sResource] = bModAppts;
				}

				//For programmers and scheduling managers, set all permissions for all resources
				if (this.DocManager.ScheduleManager == true)
				{
					dt = this.DocManager.GlobalDataSet.Tables["Resources"];
					foreach (DataRow dr in dt.Rows)
					{
						sResource = dr["RESOURCE_NAME"].ToString();
						v.m_htOverbook[sResource] = true;
						v.m_htModifySchedule[sResource] = true;
						v.m_htChangeAppts[sResource] = true;
					}
				}

				v.calendarGrid1.SetOverlapTable();
				v.calendarGrid1.Refresh();
			}
		}

		private void PositionGrid(CalendarGrid cg, int nHour)
		{
				//Position grid to nHour
				int nRow = 0, nCol = 0;
				DateTime dStart = DateTime.Today;
				dStart = dStart.AddHours(nHour);
				cg.GetCellFromTime(dStart, ref nRow, ref nCol, false, "");
				int nHeight = cg.CellHeight + 10;
				nHeight *= nRow;
				cg.AutoScrollPosition = new Point(50, nHeight);
				cg.Invalidate();
		}

		private void LoadTree()
		{
			//Navigate from ResourceGroup table to Resources table
			DataRow[] arrRows;
			DataRelation dr = DocManager.GlobalDataSet.Relations["GroupResource"];
			string sGroup;
			string sResource;
			int nIndex = 0;
			foreach (DataRow r in DocManager.GlobalDataSet.Tables["ResourceGroup"].Rows)
			{
				sGroup = r["RESOURCE_GROUP"].ToString();
				TreeNode deptNode = new TreeNode(sGroup);
				nIndex = this.tvSchedules.Nodes.Add(deptNode);
				tvSchedules.Nodes[nIndex].Tag = "Dept";
				arrRows = r.GetChildRows(dr);
				for (int i=0; i< arrRows.Length; i++) 
				{
					sResource = arrRows[i]["RESOURCE_NAME"].ToString();
					TreeNode resNode = new TreeNode(sResource);
					int nResIndex = deptNode.Nodes.Add(resNode);
					deptNode.Nodes[nResIndex].Tag = "Resource";
				}
			}
		}

		public void CreateNewSchedule()
		{				
			//Create a new document and open it
			CGDocument doc = new CGDocument();
			doc.DocManager = this.DocManager;
			try
			{
				doc.OnOpenDocument();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to open " + m_sDocName + " schedule.  " +  ex.Message, "Clinical Scheduling");
				this.m_DocManager.CloseAllViews(doc);
				return;
			}
		}

		private void AppointmentEdit()
		{
			try
			{
				int nApptID = this.calendarGrid1.SelectedAppointment;
				Debug.Assert(nApptID != 0);
			
				CGAppointment a = (CGAppointment) this.Appointments.AppointmentTable[nApptID];

				DAppointPage dAppt = new DAppointPage();			
				dAppt.DocManager = this.m_DocManager;
				dAppt.InitializePage(a);

				calendarGrid1.CGToolTip.Active = false;

				if (dAppt.ShowDialog(this) == DialogResult.Cancel)
				{
					calendarGrid1.CGToolTip.Active = true;
					return;
				}
				calendarGrid1.CGToolTip.Active = true;

				string sNote = dAppt.Note;

				//Call Document to edit appointment
				this.Document.EditAppointment(a, sNote);

			}
			catch (Exception ex)
			{
				Debug.Write("CGView.AppointmentEdit Failed:  " + ex.Message);
			}
		}

		/// <summary>
		/// Marks all selected appointments as No Show
		/// </summary>
		/// <param name="nApptID"></param>
		/// <returns></returns>		
		private void AppointmentNoShow(bool bNoShow)
		{

			//bNoShow indicates whether to mark or un-mark as noshow
			bool			bMarked = false;	//Indicates at least one attempt to mark as noshow succeeded
			bool			bRebook = false;	//Stores user's response to auto-rebook dialog question
			CGAppointments	alRebookList = new CGAppointments();
			DNoShow dlg = new DNoShow();;
			if (bNoShow == true)
			{
				if (dlg.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}
			}

			bRebook = dlg.AutoRebook;

			foreach (CGAppointment a in this.calendarGrid1.SelectedAppointments.AppointmentTable.Values)
			{
				int nApptID = a.AppointmentKey;
				Debug.Assert(nApptID != 0);
				try
				{
					if ((bNoShow == true)
						&&
						(a.StartTime.Date > DateTime.Today.Date)
						&&
						(MessageBox.Show(this, "The appointment for " + a.PatientName + " is in the future.  Are you sure you want to No-Show?", "Windows Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK))
					{
					}
					else
					{
						string sError = Document.AppointmentNoShow(nApptID, bNoShow);
						if (sError != "1")
							throw new Exception(sError);

						bMarked = true;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unable to mark appointment No Show: " +  ex.Message, "Clinical Scheduling");
				}
				if (bRebook == true)
				{
					try
					{
						CGAppointment aRebook;
						int nMinimumdays = dlg.RebookStartDays;
						int nMaximumdays = dlg.RebookMaxDays;
						int nAccessType = dlg.RebookAccessType;
						//-1 means use current type

						if (nAccessType == -1)
						{
							//Get access type from grid
							int nRow = 0;
							int nCol = 0;
							CGCell cgCell = new CGCell();
							this.calendarGrid1.GetCellFromTime(a.StartTime, ref nRow, ref nCol, true , a.Resource);
							cgCell.CellColumn = nCol;
							cgCell.CellRow = nRow;
							this.calendarGrid1.GetTypeFromCell(cgCell, out nAccessType);
							a.AccessTypeID = nAccessType;
						}
						string sResult = Document.AutoRebook(a, nAccessType, nMinimumdays, nMaximumdays, out aRebook);
						if (sResult == "1")
						{
							//Add appointment to list of rebooked appointments
							alRebookList.AddAppointment(a);
						}
						else
						{
							MessageBox.Show("Unable to rebook this patient: " + a.PatientName);
						}

					}
					catch (Exception ex)
					{
						MessageBox.Show("Unable to rebook: " + ex.Message);
					}
				}
			}
			
			if (bMarked == true)
			{
				//Notify other scheduling users that this schedule has changed
				try
				{
					this.Document.RefreshDocument();
					RaiseRPMSEvent("BSDX SCHEDULE" , m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
				this.calendarGrid1.Invalidate();
				
				AutoRebookFromList(alRebookList);
			}			
		}
		
		private void AutoRebookFromList(CGAppointments alRebookList)
		{
			//Print AutoRebook letters.
			if (alRebookList.AppointmentCount > 0)
			{
				//build |-delimited list of ApptIDs to pass to BSDX REBOOK LIST
				string sApptIDList = "";

				System.Collections.ArrayList a = new ArrayList();

				foreach (CGAppointment appt in alRebookList.AppointmentTable.Values)
				{
					string sApptID = appt.AppointmentKey.ToString() + "|";
					sApptIDList += sApptID;
					if (a.Contains(appt.Resource) == false)
						a.Add(appt.Resource);
				}
				//Call rebook letters RPC to get datatable
				string sClinicList = "";
				foreach (string sRes in a)
				{
					sClinicList = sClinicList + sRes + "|";	
				}
				string sSql = "BSDX REBOOK LIST^" + sApptIDList;
				DataTable dtLetters = m_DocManager.RPMSDataTable(sSql, "PatientAppts");
				DPatientLetter dpl = new DPatientLetter();
					
				dpl.InitializeFormRebookLetters(this.DocManager, sClinicList, dtLetters);
				dpl.ShowDialog(this);
			}		
		}

		/// <summary>
		/// Delete appointment ApptID
		/// </summary>
		/// <param name="nApptID"></param>
		/// <returns></returns>
		private string AppointmentDeleteOne(int nApptID)
		{
			return Document.DeleteAppointment(nApptID);
		}

		/// <summary>
		/// Delete all selected appointments
		/// </summary>
		private void AppointmentDelete() 
		{
			calendarGrid1.CGToolTip.Active = false;
			CGAppointments	alRebookList = new CGAppointments();

			DCancelAppt dCancel = new DCancelAppt();
			dCancel.InitializePage(this.m_DocManager);
			if (dCancel.ShowDialog(this) != DialogResult.OK)
			{
				calendarGrid1.CGToolTip.Active = true;
				return;
			}

			bool bClinic = dCancel.ClinicCancelled;
			int nReason = dCancel.CancelReason;
			string sRemarks = dCancel.CancelRemarks;
			bool bRebook = dCancel.AutoRebook;
			int nRebookStart = dCancel.RebookStartDays;
			int nRebookMax = dCancel.RebookMaxDays;
			int nRebookAccessType = dCancel.RebookAccessType;

			calendarGrid1.CGToolTip.Active = true;

			bool bDeleted = false;
			foreach (CGAppointment a in this.calendarGrid1.SelectedAppointments.AppointmentTable.Values)
			{
				int nApptID = a.AppointmentKey;
				Debug.Assert(nApptID != 0);
				try
				{
					string sError = Document.DeleteAppointment(nApptID,bClinic, nReason, sRemarks);
					if (sError != "")
						throw new Exception(sError);

					bDeleted = true;
					if (bRebook == true)
					{
						try
						{
							//TODO: Parameterize  or dialogize the minum and maximum rebook days
							CGAppointment aRebook;
							int nMinimumdays = nRebookStart;
							int nMaximumdays = nRebookMax;
							string sResult = Document.AutoRebook(a, nRebookAccessType, nMinimumdays, nMaximumdays, out aRebook);
							if (sResult == "1")
							{
								//Add appointment to list of rebooked appointments
								alRebookList.AddAppointment(a);
							}

						}
						catch (Exception ex)
						{
							MessageBox.Show("Unable to rebook: " + ex.Message);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unable to delete appointment.  " +  ex.Message, "Clinical Scheduling");
				}

			}
			if (bDeleted == true)
			{
				try
				{
					RaiseRPMSEvent("BSDX SCHEDULE" , m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
				this.calendarGrid1.Invalidate();
				if (bRebook == true)
				{
					AutoRebookFromList(alRebookList);
				}
			}

		}

		private void AppointmentCheckIn()
		{
			bool bDeleted = false;
			int nApptID = this.calendarGrid1.SelectedAppointment;
			Debug.Assert(nApptID != 0);

			CGAppointment a = (CGAppointment) this.Appointments.AppointmentTable[nApptID];

			try
			{

				bool bAlreadyCheckedIn = false;
				if (a.CheckInTime.Ticks > 0)
					bAlreadyCheckedIn = true;

				if ((bAlreadyCheckedIn == false)
					&&
					(a.StartTime.Date > DateTime.Today.Date))
				{
					MessageBox.Show(this, "It is too early to check in " + a.PatientName, "Windows Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				//Find the default provider for the resource & load into combo box
				DataView rv = new DataView(this.m_DocManager.GlobalDataSet.Tables["Resources"]);
				rv.Sort="RESOURCE_NAME ASC";
				int nFind = rv.Find((string) a.Resource);
				DataRowView drv = rv[nFind];
				
				string sHospLoc = drv["HOSPITAL_LOCATION_ID"].ToString();
				sHospLoc = (sHospLoc == "")?"0":sHospLoc;
				int nHospLoc = 0;
				try
				{
					nHospLoc = Convert.ToInt32(sHospLoc);
				}
				catch(Exception ex)
				{
					Debug.Write("CGView.AppointmentCheckIn Error: " + ex.Message);
				}

				
				string sProv = "";
				string sProvReqd = "NO";
				string sPCC = "NO";
				string sMultCodes = "NO";
				string sStopCode = "";
				bool bProvReqd = false;
				bool bPCC = false;
				bool bMultCodes = false;
				if (nHospLoc > 0)
				{
					DataRow dr = drv.Row;
					DataRow drHL = dr.GetParentRow(m_DocManager.GlobalDataSet.Relations["HospitalLocationResource"]);
					sProv = drHL["DEFAULT_PROVIDER"].ToString();
					sStopCode = drHL["STOP_CODE_NUMBER"].ToString();

					DataRow[] draCS = drHL.GetChildRows(m_DocManager.GlobalDataSet.Relations["HospitalLocationClinic"]);
					if (draCS.GetLength(0) > 0)
					{
						DataRow drCS = draCS[0];
						sProvReqd = drCS["VISIT_PROVIDER_REQUIRED"].ToString();
						sPCC = drCS["GENERATE_PCCPLUS_FORMS?"].ToString();
						sMultCodes = drCS["MULTIPLE_CLINIC_CODES_USED?"].ToString();
					}
					bProvReqd = (sProvReqd == "YES")?true:false;
					bPCC = (sPCC == "YES")?true:false;
					bMultCodes = (sMultCodes == "YES")?true:false;
				}

				DCheckIn dlgCheckin = new DCheckIn();
				dlgCheckin.InitializePage(a, this.m_DocManager, sProv, bProvReqd, bPCC, bMultCodes, sStopCode);
				calendarGrid1.CGToolTip.Active = false;
				if (dlgCheckin.ShowDialog(this) != DialogResult.OK)
				{
					calendarGrid1.CGToolTip.Active = true;
					return;
				}
				calendarGrid1.CGToolTip.Active = true;

				if (bAlreadyCheckedIn == true)
					return;

				DateTime dtCheckIn = dlgCheckin.CheckInTime;

				/*
				 * Need to pass Provider, ClinicStop, PrintRouteSlip, 
				 * PCC Clinic, PCC Form, Print OutGuide
				 */

				this.Document.CheckInAppointment(nApptID, dtCheckIn,
			 dlgCheckin.ClinicStopIEN,
			 dlgCheckin.ProviderIEN,
			 dlgCheckin.PrintRouteSlip,
			 dlgCheckin.PCCClinicIEN,
			 dlgCheckin.PCCFormIEN,
			 dlgCheckin.PCCOutGuide
					);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error checking in patient:  " +  ex.Message, "Clinical Scheduling");
			}

			if (bDeleted == true)
			{
				try
				{
					RaiseRPMSEvent("BSDX SCHEDULE" , m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
				this.calendarGrid1.Invalidate();
			}		
		}

		private void AppointmentAddWalkin()
		{
			try
			{
				CGAppointment appt = new CGAppointment();
			
				//Get Time and Resource from Selected Cell
				DateTime dStart = DateTime.Today;
				DateTime dEnd = DateTime.Today;
				string sResource = "";
				bool bRet = this.calendarGrid1.GetSelectedTime(out dStart, out dEnd, out sResource);
				if (bRet == false)
					return;

				TimeSpan tsDuration = dEnd - dStart;
				int nDuration = (int) tsDuration.TotalMinutes;
				Debug.Assert(nDuration > 0);

				/*
				 * 8-10-05 Added check to prevent walkin from being created
				 * on a date later than today.
				 */

				if (dStart.Date > DateTime.Today.Date)
				{
					MessageBox.Show(this, "You cannot create a walk-in appointment for a date in the future.\n Select today's date and try again.", "Windows Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				/*
				 * 8-10-05 Added overbook prompt for walkin
				*/
				this.Document.RefreshDocument();
				string sAccessType = "";
				string sAvailabilityMessage = "";
				m_nSlots = m_Document.SlotsAvailable(dStart, dEnd, sResource, out sAccessType, out sAvailabilityMessage);

				if (m_nSlots < 1)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				//Display a dialog to collect Patient Name
				DPatientLookup dPat = new DPatientLookup();
				dPat.DocManager = m_DocManager;
				
				int nAccessTypeID = 0;
				bRet = calendarGrid1.GetSelectedType(out nAccessTypeID);

				if (dPat.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				appt.PatientID = Convert.ToInt32(dPat.PatientIEN);
				appt.PatientName = dPat.PatientName;
				appt.StartTime = dStart;
				appt.EndTime = dEnd;
				appt.Resource = sResource;
				appt.HealthRecordNumber = dPat.HealthRecordNumber;

				/*
				 * 8-10-05 Copied overbook prompt for walkin
				 * to this position in order to check just prior
				 * to calling CreateAppointment
				*/
				this.Document.RefreshDocument();
				m_nSlots = m_Document.SlotsAvailable(dStart, dEnd, sResource, out sAccessType, out sAvailabilityMessage);

				if (m_nSlots < 1)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				//Call Document to add a walkin appointment
				int nApptID = this.Document.CreateAppointment(appt, true);

				//Now check them in.
				calendarGrid1.SelectedAppointment = nApptID;

				AppointmentCheckIn();

				try
				{
					RaiseRPMSEvent("BSDX SCHEDULE" , m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
			
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add walk-in appointment  " +  ex.Message, "Clinical Scheduling");
				return;

			}
		}

		private void AppointmentAddNew() 
		{
			try
			{
				CGAppointment appt = new CGAppointment();
			
				//Get Time and Resource from Selected Cell
				DateTime dStart = DateTime.Today;
				DateTime dEnd = DateTime.Today;
				string sResource = "";
				bool bRet = this.calendarGrid1.GetSelectedTime(out dStart, out dEnd, out sResource);
				if (bRet == false)
					return;
				
				//Test dStart for Holiday
				DataView dvHoliday = new DataView(this.DocManager.GlobalDataSet.Tables["HOLIDAY"]);
				dvHoliday.Sort="DATE ASC";
				int nFind = dvHoliday.Find(dStart.Date);
				if (nFind > -1)
				{
					string sHoliday = "";
					DataRowView drv = dvHoliday[nFind];
					sHoliday = drv["NAME"].ToString();
					if (MessageBox.Show(this, dStart.ToShortDateString() + " is a holiday (" + sHoliday + ").  Are you sure you want to make this appointment?","Clinical Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
						return;
				}
				
				TimeSpan tsDuration = dEnd - dStart;
				int nDuration = (int) tsDuration.TotalMinutes;
				Debug.Assert(nDuration > 0);

				/*
				 * 7-19-05 Added overbook prompt
				 * 8-10-05 Copied code block prior to patient selection
				*/
				this.Document.RefreshDocument();
				string sAccessType = "";
				string sAvailabilityMessage = "";
				m_nSlots = m_Document.SlotsAvailable(dStart, dEnd, sResource, out sAccessType, out sAvailabilityMessage);

				if (m_nSlots < 1)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				//Display a dialog to collect Patient Name
				DPatientLookup dPat = new DPatientLookup();
				dPat.DocManager = m_DocManager;
				
				int nAccessTypeID = 0;
				bRet = calendarGrid1.GetSelectedType(out nAccessTypeID);

				if (dPat.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call the appointment dialog to collect the appointment info
				Debug.Assert(dPat.PatientIEN != "");
				DAppointPage dAppt = new DAppointPage();			
				dAppt.DocManager = this.m_DocManager;
				string sNote = "";
				dAppt.InitializePage(dPat.PatientIEN, dStart, nDuration, sResource, sNote);

				if (dAppt.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				appt.PatientID = Convert.ToInt32(dPat.PatientIEN);
				appt.PatientName = dPat.PatientName;
				appt.StartTime = dStart;
				appt.EndTime = dEnd;
				appt.Resource = sResource;
				appt.Note = dAppt.Note;
				appt.HealthRecordNumber = dPat.HealthRecordNumber;
				appt.AccessTypeID = nAccessTypeID;

				/*
				 * 7-19-05 Added overbook prompt
				 * 8-10-05 Copied code block prior to patient selection
				*/
				this.Document.RefreshDocument();
				m_nSlots = m_Document.SlotsAvailable(dStart, dEnd, sResource, out sAccessType, out sAvailabilityMessage);

				if (m_nSlots < 1)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				//Call Document to add a new appointment
				this.Document.CreateAppointment(appt);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add new appointment  " +  ex.Message, "Clinical Scheduling");
				return;

			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE" , m_Document.DocName);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		public void OnUpdateSchedule()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				m_Document.RefreshDocument();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to refresh document " +  ex.Message, "Clinical Scheduling");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		public void UpdateArrays()
		{
			Debug.Assert(this.InvokeRequired == false,"CGView.UpdateArrays InvokeRequired");
			try 
			{
				this.calendarGrid1.AvailabilityArray = this.m_Document.AvailabilityArray;
				this.calendarGrid1.Resources = this.m_Document.Resources;
				this.calendarGrid1.OnUpdateArrays();
				this.lblResource.Text = this.m_Document.DocName;
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to update arrays " +  ex.Message, "Clinical Scheduling");
			}
		}

		public void RaiseRPMSEvent(string sEvent, string sParams)
		{
			try
			{
				//Signal RPMS to raise an event
				m_ConnectInfo.RaiseEvent(sEvent, sParams, true);
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

        private void SchedulingManagement()
        {
            try
            {
                bool bLock = DocManager.ConnectInfo.Lock("^BSDXMGR", "+", "");
                if (bLock == false)
                {
                    throw new Exception("Another user is currently in Scheduling Management.  Try later.");
                }

                DManagement dMgm = new DManagement();
                dMgm.InitializeDialog(this.m_DocManager);

                if (dMgm.ShowDialog(this) == DialogResult.Cancel)
                {
                }

                m_DocManager.GlobalDataSet.Tables["ResourceUser"].Clear();
                m_DocManager.LoadResourceUserTable(false);
                bLock = DocManager.ConnectInfo.bmxNetLib.Lock("^BSDXMGR", "-");
            }
            catch (ApplicationException aex)
            {
                string sMsg = aex.Message;
                MessageBox.Show("Unable to acquire transmit lock.  Try later.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Scheduling Management Error:  " + ex.Message, "Clinical Scheduling");
            }
        }

        public void UpdateTree()
        {
            this.tvSchedules.Nodes.Clear();
            this.LoadTree();
        }

        public void ViewPatientAppointments()
        {
            try
            {
                //Display a dialog to collect Patient Name
                DPatientLookup dPat = new DPatientLookup();
                dPat.DocManager = m_DocManager;
                if (dPat.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }

                Debug.Assert(dPat.PatientIEN != "");
                int nPatientID = Convert.ToInt32(dPat.PatientIEN);
                ViewPatientAppointments(nPatientID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void ViewPatientAppointments(int PatientID)
        {
            DPatientApptDisplay dPa = new DPatientApptDisplay();

            dPa.InitializeForm(this.DocManager, PatientID);


            if (dPa.ShowDialog(this) != DialogResult.Cancel)
            {

            }

        }

        private void FindAvailableAppointment(ArrayList alResourceArray)
        {
            DApptSearch dSearch = new DApptSearch();
            dSearch.InitializePage(alResourceArray, this.m_DocManager);
            if (dSearch.ShowDialog(this) == DialogResult.Cancel)
                return;

            string sResource = dSearch.SelectedResource;
            ArrayList alResource = new ArrayList();
            alResource.Add(sResource);
            DateTime sDate = dSearch.SelectedDate;
            m_sDocName = sResource;
            OpenSelectedSchedule(alResource, sDate);

        }

        private void PrintPatientLetter()
        {
            //Prompt for patient and
            //Print letter for individual patient
            try
            {
                //Display a dialog to collect Patient Name
                DPatientLookup dPat = new DPatientLookup();
                dPat.DocManager = m_DocManager;
                if (dPat.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }

                Debug.Assert(dPat.PatientIEN != "");
                int nPatientID = Convert.ToInt32(dPat.PatientIEN);
                PrintPatientLetter(nPatientID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void PrintPatientLetter(int PatientID)
        {
            //Print letter for individual patient
            try
            {
                DPatientLetter dPa = new DPatientLetter();
                dPa.InitializeForm(DocManager, PatientID);

                if (dPa.ShowDialog(this) != DialogResult.Cancel)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Methods

		#region Events

		private void CGView_Load(object sender, System.EventArgs e)
		{
			Debug.Assert (this.Document != null);

			//Register the view
			CGDocumentManager.Current.RegisterDocumentView(this.Document, this);
            this.mnu5Day.Click += new System.EventHandler(this.dateTimePicker1_ValueChanged); // MJL 1/17/2007
            this.mnu7Day.Click += new System.EventHandler(this.dateTimePicker1_ValueChanged);

			//Load the Group-Resource treeview
			LoadTree();

			this.SetDesktopLocation(this.DesktopLocation.X + 10, this.DesktopLocation.Y + 10);
		}

		private void mnuOpenSchedule_Click(object sender, System.EventArgs e)
		{
			CreateNewSchedule();
		}

		private void mnu1Day_Click(object sender, System.EventArgs e)
		{
			DateTime dtPicker = dateTimePicker1.Value;
			DateTime DayOnly = new DateTime(dtPicker.Year, dtPicker.Month, dtPicker.Day);
			this.calendarGrid1.StartDate = DayOnly;
			this.calendarGrid1.Columns = 1;
		}

		private void mnu5Day_Click(object sender, System.EventArgs e)
		{
			if (this.calendarGrid1.Columns == 1)
			{
				this.StartDate = this.Document.StartDate;
			}

			this.calendarGrid1.Columns = 5;
            this.Document.m_nColumnCount = 5; // MJL 1/17/2007
            //this.Document.UpdateAllViews();
		}

		private void mnu7Day_Click(object sender, System.EventArgs e)
		{
			this.calendarGrid1.Columns = 7;
            this.Document.m_nColumnCount = 7; // MJL 1/17/2007
        }

		private void mnu10Minute_Click(object sender, System.EventArgs e)
		{
			CalendarGrid cg = this.calendarGrid1;
			cg.TimeScale = 10;
			PositionGrid(cg, 7);
		}

		private void mnu15Minute_Click(object sender, System.EventArgs e)
		{
			CalendarGrid cg = this.calendarGrid1;
			cg.TimeScale = 15;
			PositionGrid(cg, 7);
		}

		private void mnu20Minute_Click(object sender, System.EventArgs e)
		{
			CalendarGrid cg = this.calendarGrid1;
			cg.TimeScale = 20;
			PositionGrid(cg, 7);
		}

		private void mnu30Minute_Click(object sender, System.EventArgs e)
		{
			CalendarGrid cg = this.calendarGrid1;
			cg.TimeScale = 30;
			PositionGrid(cg, 7);
		}

		private void mnuViewScheduleTree_Click(object sender, System.EventArgs e)
		{
			this.mnuViewScheduleTree.Checked = this.tvSchedules.Visible;
			this.tvSchedules.Visible = !(this.tvSchedules.Visible);
			this.mnuViewScheduleTree.Checked = !(this.mnuViewScheduleTree.Checked);
		}

		private void tvSchedules_Click(object sender, System.EventArgs e)
		{
			bSchedulesClicked = true;
		}

		private void tvSchedules_DoubleClick(object sender, System.EventArgs e)
		{
			if (m_alSelectedTreeResourceArray == null)
				return;
			if (m_alSelectedTreeResourceArray.Count < 1)
			{
				if (this.tvSchedules.SelectedNode.Text != "")
				{
					SetResourceArrayFromGroup(tvSchedules.SelectedNode.Text);
				}
				else
				{
					return;
				}
			}
			OpenSelectedSchedule(m_alSelectedTreeResourceArray, DateTime.Today);
		}

		//20041109 Added
		private void SetResourceArrayFromGroup(string sGroup)
		{
			//Navigate from ResourceGroup table to Resources table
			DataRow[] arrRows;
			DataRelation dr = DocManager.GlobalDataSet.Relations["GroupResource"];
			DataRow r = DocManager.GlobalDataSet.Tables["ResourceGroup"].Rows.Find(sGroup);
			arrRows = r.GetChildRows(dr);
			for (int i=0; i< arrRows.Length; i++) 
			{
				string sResource = arrRows[i]["RESOURCE_NAME"].ToString();
				m_alSelectedTreeResourceArray.Add(sResource);
			}
			m_sDocName = sGroup;
		}

		public void SyncTree()
		{

        }

		private void tvSchedules_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{

		}

		private void tvSchedules_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (bSchedulesClicked == false)
				return;
			bSchedulesClicked = false;
			
			m_alSelectedTreeResourceArray = new ArrayList();
			string sResource = e.Node.FullPath;
			string[] ss = sResource.Split((char) 92);
			int l = ss.GetUpperBound(0);

			if (l == 0) //a resource group was checked, so get all underying resources 
			{
				SetResourceArrayFromGroup(ss[0]);
			}
			else 
			{
				sResource = ss[l];
				m_alSelectedTreeResourceArray.Add(ss[1]);
			}

			m_sDocName = ss[l];
			return;

		}

		private void mnuTest1_Click(object sender, System.EventArgs e)
		{
			ReaderWriterLock m_rwl = this.DocManager.ConnectInfo.bmxNetLib.BMXRWL;
			try
			{
				m_rwl.AcquireWriterLock(50);
				Debug.Write("\nTest Button 1 Acquired first lock\n");
				m_rwl.AcquireWriterLock(50);
				Debug.Write("Test Button 1 Acquired second lock\n");
				this.DocManager.ViewRefresh();
				Thread.Sleep(5000);
				try
				{
				}
				catch
				{
				}
				finally
				{
					m_rwl.ReleaseWriterLock();
					Debug.Write ("Test Button 1 released first lock.\n");
					m_rwl.ReleaseWriterLock();
					Debug.Write ("Test Button 1 released second lock.\n");
				}

				return;
			}
			catch (Exception ex)
			{
				Debug.Write("Test Button 1 exception: " + ex.Message + "\n");
			}
		}

		private void CGView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				m_ConnectInfo.BMXNetEvent -= m_bmxDelegate;
				this.calendarGrid1.CloseGrid();
			}
			catch (Exception ex)
			{
				Debug.Write("CGView_Closing exception: " + ex.Message + "\n");
			}
		}

		private void mnuViewRightPanel_Click(object sender, System.EventArgs e)
		{
			this.mnuViewRightPanel.Checked = this.panelRight.Visible;
			this.panelRight.Visible = !(this.panelRight.Visible);
			this.mnuViewRightPanel.Checked = !(this.mnuViewRightPanel.Checked);
		}

		private void dateTimePicker1_ValueChanged(object sender, System.EventArgs e)
		{
			DateTime dDate = dateTimePicker1.Value;
			dDate = dDate.Date;
			this.Document.SelectedDate = dDate;
			if (this.Document.Resources.Count == 1) 
			{
				if (this.calendarGrid1.Columns > 1)
				{
					this.StartDate = this.Document.StartDate;
				}
				else
				{
					this.StartDate = this.Document.SelectedDate;
				}
			}
			else
			{
					this.StartDate = this.Document.SelectedDate;
			}
			this.Document.UpdateAllViews();
			this.calendarGrid1.Invalidate();
		}

		private void calendarGrid1_CGSelectionChanged(object sender, IndianHealthService.ClinicalScheduling.CGSelectionChangedArgs e)
		{
			string sAccessType = "";
			string sAvailabilityMessage = "";
			m_nSlots = m_Document.SlotsAvailable(e.StartTime, e.EndTime, e.Resource, out sAccessType, out sAvailabilityMessage);
			UpdateStatusBar(e.StartTime, e.EndTime, sAccessType, sAvailabilityMessage);
		}

		private void calendarGrid1_CGAppointmentChanged(object sender, IndianHealthService.ClinicalScheduling.CGAppointmentChangedArgs e)
		{
			try
			{
				if (e.Appointment.CheckInTime.Ticks > 0)
				{
					MessageBox.Show("You cannot change the appointment time because the patient has already checked in.", "Clinical Scheduling",  MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
					return;
				}

				if (EditAppointmentEnabled(e.Resource) == false)
					return;
				if (EditAppointmentEnabled(e.Appointment.Resource) == false)
					return;

				if (MessageBox.Show("Are you sure you want to move this appointment?", "Clinical Scheduling",  MessageBoxButtons.YesNo) != DialogResult.Yes)
					return;

				//20040909 Cherokee Replaced this block with following
				string sAccessType = "";
				string sAvailabilityMessage = "";
				//				if (m_Document.SlotsAvailable(e.StartTime, e.EndTime, e.Resource, out sAccessType, out sAvailabilityMessage) < 1)
				//				{
				//					MessageBox.Show("There are no appointment slots available for the selected time.");
				//					return;
				//				}
				bool bOverbook =false;
				if (m_htOverbook.Count > 0)
				{
					bOverbook = (bool) this.m_htOverbook[e.Resource.ToString()];
				}
				bool bModSchedule =false;
				if (m_htModifySchedule.Count > 0)
				{
					bModSchedule =  (bool) this.m_htModifySchedule[e.Resource.ToString()];
				}
				bool bSlotsAvailable = (m_Document.SlotsAvailable(e.StartTime, e.EndTime, e.Resource, out sAccessType, out sAvailabilityMessage) > 0);
				if (!((bSlotsAvailable) || (bModSchedule) || (bOverbook) ))
				{
					MessageBox.Show("There are no appointment slots available for the selected time.");
					return;
				}

				/*
				 * 7-19-05 Added overbook prompt
				*/
				if (bSlotsAvailable == false)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				e.Appointment.StartTime = e.StartTime;
				e.Appointment.EndTime = e.EndTime;
				e.Appointment.Resource = e.Resource;
				e.Appointment.AccessTypeID = e.AccessTypeID;
				m_Document.CreateAppointment(e.Appointment);
			
				string sError = AppointmentDeleteOne(e.Appointment.AppointmentKey);
				if (sError != "")
				{
					MessageBox.Show(sError);
					return;
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to change appointment  " +  ex.Message, "Clinical Scheduling");
				this.m_DocManager.UpdateViews();
				return;
			}
			finally
			{

            }
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE"  , e.Resource);
				if (e.Resource != e.OldResource)
					RaiseRPMSEvent("BSDX SCHEDULE", e.OldResource);
				this.m_DocManager.UpdateViews(e.Resource, e.OldResource);
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		private void mnuSchedulingManagment_Click(object sender, System.EventArgs e)
		{
			SchedulingManagement();
		}

		private void mnuFile_Popup(object sender, System.EventArgs e)
		{
			this.mnuSchedulingManagment.Enabled = DocManager.ScheduleManager;
		}

		private void mnuFindAppt_Click(object sender, System.EventArgs e)
		{
			FindAvailableAppointment(this.Document.Resources);
		}

		private void mnuRPMSServer_Click(object sender, System.EventArgs e)
		{
			//Handled by DocumentManager class
		}

		private void mnuRPMSLogin_Click(object sender, System.EventArgs e)
		{
			//Handled by DocumentManager class
		}

		private void CGView_Activated(object sender, System.EventArgs e)
		{
			calendarGrid1.GridEnter = true;
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("Clinical Scheduling Version " + Application.ProductVersion, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Information);	
		}

		private void ImplementMsg()
		{
			MessageBox.Show("Clinical Scheduling", "TODO: Implement this function");
		}

		private void mnuClose_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = MessageBox.Show("Are you sure you want to close this schedule?", Application.ProductName, MessageBoxButtons.OKCancel);
			if (dr != DialogResult.OK)
				return;

			this.Close();
		}

		private void mnuViewPatientAppts_Click(object sender, System.EventArgs e)
		{
			ViewPatientAppointments();
		}

		private void lstClip_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			bool b = e.Data.GetDataPresent(typeof(CGAppointment));
			if (b == true)
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}

		}

		private void lstClip_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			try
			{
				CGAppointment a = (CGAppointment) e.Data.GetData(typeof(CGAppointment));
				if (m_ClipList.AppointmentTable.Contains((int) a.AppointmentKey))
				{
					return;
				}
				m_ClipList.AddAppointment(a);
				lstClip.Items.Add(a);
			}
			catch(Exception ex)
			{
				Debug.Write(ex.Message);
			}

		}

		private void lstClip_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_bDragDropStart = false;
		}

		private void lstClip_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if ((m_bDragDropStart == false)&&(lstClip.SelectedIndex > -1))
				{
					CGAppointment a = (CGAppointment) lstClip.Items[lstClip.SelectedIndex];
					this.calendarGrid1.ApptDragSource = "list";
					DragDropEffects effect = DoDragDrop(a, DragDropEffects.Move);
					m_bDragDropStart = true;
				}
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		private void calendarGrid1_CGAppointmentAdded(object sender, IndianHealthService.ClinicalScheduling.CGAppointmentChangedArgs e)
		{
			try
			{
				string sAccessType = "";
				string sAvailabilityMessage = "";
				bool	bSlotsAvailable;
				bool	bOverbook;
				bool	bModSchedule;
				bool	bModAppts;

				if (this.EditAppointmentEnabled(e.Appointment.Resource) == false)
					return;
				
				bModAppts = (bool) this.m_htChangeAppts[e.Resource.ToString()];
				if (bModAppts == false)
					return;

				bOverbook = (bool) this.m_htOverbook[e.Resource.ToString()];
				bModSchedule =  (bool) this.m_htModifySchedule[e.Resource.ToString()];
				bSlotsAvailable = (m_Document.SlotsAvailable(e.StartTime, e.EndTime, e.Resource, out sAccessType, out sAvailabilityMessage) > 0);

				if (!((bSlotsAvailable) || (bModSchedule) || (bOverbook) ))
				{
					MessageBox.Show("There are no appointment slots available for the selected time.");
					return;
				}

				/*
				 * 7-19-05 Added overbook prompt
				*/
				if (bSlotsAvailable == false)
				{
					DialogResult dr = MessageBox.Show(this, "There are no slots available at the selected time.  Do you want to overbook this appointment?", "Clinical Scheduling",MessageBoxButtons.YesNo);
					if (dr != DialogResult.Yes)
					{
						return;
					}
				}

				e.Appointment.StartTime = e.StartTime;
				e.Appointment.EndTime = e.EndTime;
				e.Appointment.Resource = e.Resource;
				e.Appointment.AccessTypeID = e.AccessTypeID;
				m_Document.CreateAppointment(e.Appointment);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add new appointment  " +  ex.Message, "Clinical Scheduling");
				return;
			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE"  , e.Resource);
				if (e.Resource != e.OldResource)
					RaiseRPMSEvent("BSDX SCHEDULE", e.OldResource);
				this.m_DocManager.UpdateViews(e.Resource, e.OldResource);
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		private void lstClip_SelectedIndexChanged(object sender, System.EventArgs e)
		{

		}

		private void mnuPrintClinicSchedules_Click(object sender, System.EventArgs e)
		{
			try
			{
				DSelectLetterClinics ds = new DSelectLetterClinics();
				ds.InitializePage(this.m_DocManager.GlobalDataSet, "Print Clinic Schedules");
				ds.SetupForReports();
				if (ds.ShowDialog(this) != DialogResult.OK)
					return;

				//get the resource names and call the letter printer

				string sClinics = ds.SelectedClinics;
				DateTime dtBegin = ds.BeginDate;
				DateTime dtEnd = ds.EndDate;

				DPatientLetter dpl = new DPatientLetter();
				dpl.InitializeFormClinicSchedule(this.DocManager, sClinics, dtBegin, dtEnd);
				dpl.ShowDialog(this);

			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuPrintClinicLetters_Click(object sender, System.EventArgs e)
		{
			try
			{
				DSelectLetterClinics ds = new DSelectLetterClinics();
				ds.InitializePage(this.m_DocManager.GlobalDataSet, "Print Reminder Letters");
				if (ds.ShowDialog(this) != DialogResult.OK)
					return;

				//get the resource names and call the letter printer

				string sClinics = ds.SelectedClinics;
				DateTime dtBegin = ds.BeginDate;
				DateTime dtEnd = ds.EndDate;

				DPatientLetter dpl = new DPatientLetter();
				dpl.InitializeFormClinicLetters(this.DocManager, sClinics, dtBegin, dtEnd);
				dpl.ShowDialog(this);

			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			
		}

		private void mnuPrintRebookLetters_Click(object sender, System.EventArgs e)
		{
			try
			{
				DSelectLetterClinics ds = new DSelectLetterClinics();
				ds.InitializePage(this.m_DocManager.GlobalDataSet, "Print Clinic Rebook Letters");
				if (ds.ShowDialog(this) != DialogResult.OK)
					return;

				//get the resource names and call the letter printer

				string sClinics = ds.SelectedClinics;
				DateTime dtBegin = ds.BeginDate;
				DateTime dtEnd = ds.EndDate;
				string sBegin = dtBegin.ToString("M/d/yyyy@HH:mm");
				string sEnd = dtEnd.ToString("M/d/yyyy@HH:mm");

				DPatientLetter dpl = new DPatientLetter();
				
				//Call RPC to get list of appt ids that have been rebooked for these clinics on these dates
				string sSql = "BSDX REBOOK CLINIC LIST^" + sClinics + "^" + sBegin + "^" + sEnd;
				DataTable dtLetters = DocManager.RPMSDataTable(sSql, "PatientAppts");

				dpl.InitializeFormRebookLetters(this.DocManager, sClinics, dtLetters);
				dpl.ShowDialog(this);

			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}		
		}

		private void mnuPrintPatientLetter_Click(object sender, System.EventArgs e)
		{
			PrintPatientLetter();
		}

		private void mnuRPMSDivision_Click(object sender, System.EventArgs e)
		{
			this.DocManager.ChangeDivision(this);
		}

		private void CGView_CursorChanged(object sender, System.EventArgs e)
		{

		}

		private void mnuDisplayWalkIns_Click(object sender, System.EventArgs e)
		{
			calendarGrid1.DrawWalkIns = !(calendarGrid1.DrawWalkIns);
			mnuDisplayWalkIns.Checked = calendarGrid1.DrawWalkIns;
			calendarGrid1.SetOverlapTable();
			calendarGrid1.Refresh();
		}

		private void mnuOpenMultipleSchedules_Click(object sender, System.EventArgs e)
		{
			
			try
			{
				DSelectSchedules ds = new DSelectSchedules();
				ds.InitializePage(this.m_DocManager.GlobalDataSet, "Open Multiple Schedules");
				if (ds.ShowDialog(this) != DialogResult.OK)
					return;

				//get the resource names and open schedules

				ArrayList sResources = ds.SelectedClinics;
				if (ds.SingleWindow == true)
				{
					m_sDocName = (ds.GroupWindowName == "")?"Multiple Selected Schedules":ds.GroupWindowName;
					OpenSelectedSchedule( sResources, DateTime.Today);
				}
				else
				{
					foreach (string sResource in sResources)
					{
						ArrayList alSingle = new ArrayList(1);
						alSingle.Add(sResource);
						m_sDocName = sResource;
						OpenSelectedSchedule( alSingle, DateTime.Today);
					}
				}
				return;

			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

		}

		private void ctxCalGridWalkin_Click(object sender, System.EventArgs e)
		{
			AppointmentAddWalkin();
		}

		private void mnuWalkIn_Click(object sender, System.EventArgs e)
		{
			AppointmentAddWalkin();
		}

		private void mnuPrintCancellationLetters_Click(object sender, System.EventArgs e)
		{
			try
			{
				DSelectLetterClinics ds = new DSelectLetterClinics();
				ds.InitializePage(this.m_DocManager.GlobalDataSet, "Print Clinic Cancellation Letters");
				if (ds.ShowDialog(this) != DialogResult.OK)
					return;

				//get the resource names and call the letter printer

				string sClinics = ds.SelectedClinics;
				DateTime dtBegin = ds.BeginDate;
				DateTime dtEnd = ds.EndDate;
				string sBegin = dtBegin.ToString("M/d/yyyy@HH:mm");
				string sEnd = dtEnd.ToString("M/d/yyyy@HH:mm");

				DPatientLetter dpl = new DPatientLetter();
				
				//Call RPC to get list of appt ids that have been cancelled for these clinics on these dates
				string sSql = "BSDX CANCEL CLINIC LIST^" + sClinics + "^" + sBegin + "^" + sEnd;
				DataTable dtLetters = DocManager.RPMSDataTable(sSql, "PatientAppts");

				dpl.InitializeFormCancellationLetters(this.DocManager, sClinics, dtLetters);
				dpl.ShowDialog(this);

			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
        }

        #endregion events




    }//End class
}
