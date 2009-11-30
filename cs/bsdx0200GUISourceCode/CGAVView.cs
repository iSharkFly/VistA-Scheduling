using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using IndianHealthService.BMXNet;


namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for CGAVView.
	/// </summary>
	public class CGAVView : System.Windows.Forms.Form
	{


		public CGAVView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.panelRight.Visible = false;
			this.tvSchedules.Visible = false;
			m_ClipList = new CGAppointments();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CGAVView));
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelClip = new System.Windows.Forms.Panel();
            this.lstClip = new System.Windows.Forms.ListBox();
            this.ctxApptClipMenu = new System.Windows.Forms.ContextMenu();
            this.mnuRemoveClipItem = new System.Windows.Forms.MenuItem();
            this.mnuClearClipItems = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.panelTop = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblResource = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.calendarGrid1 = new IndianHealthService.ClinicalScheduling.CalendarGrid();
            this.ctxCalendarGrid = new System.Windows.Forms.ContextMenu();
            this.ctxCalGridAdd = new System.Windows.Forms.MenuItem();
            this.ctxCalGridEdit = new System.Windows.Forms.MenuItem();
            this.ctxCalGridDelete = new System.Windows.Forms.MenuItem();
            this.tvSchedules = new System.Windows.Forms.TreeView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuLoadTemplate = new System.Windows.Forms.MenuItem();
            this.mnuSaveTemplate = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnuSchedulingManagment = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuClose = new System.Windows.Forms.MenuItem();
            this.mnuAvailability = new System.Windows.Forms.MenuItem();
            this.mnuAddNewAV = new System.Windows.Forms.MenuItem();
            this.mnuRemoveAV = new System.Windows.Forms.MenuItem();
            this.mnuEditAV = new System.Windows.Forms.MenuItem();
            this.mnuCalendar = new System.Windows.Forms.MenuItem();
            this.mnu1Day = new System.Windows.Forms.MenuItem();
            this.mnu5Day = new System.Windows.Forms.MenuItem();
            this.mnu7Day = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.mnuTimeScale = new System.Windows.Forms.MenuItem();
            this.mnu10Minute = new System.Windows.Forms.MenuItem();
            this.mnu15Minute = new System.Windows.Forms.MenuItem();
            this.mnu20Minute = new System.Windows.Forms.MenuItem();
            this.mnu30Minute = new System.Windows.Forms.MenuItem();
            this.mnuViewRightPanel = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelRight.SuspendLayout();
            this.panelClip.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.panelClip);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(728, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(120, 450);
            this.panelRight.TabIndex = 1;
            // 
            // panelClip
            // 
            this.panelClip.Controls.Add(this.lstClip);
            this.panelClip.Controls.Add(this.label1);
            this.panelClip.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelClip.Location = new System.Drawing.Point(0, 0);
            this.panelClip.Name = "panelClip";
            this.panelClip.Size = new System.Drawing.Size(120, 448);
            this.panelClip.TabIndex = 1;
            // 
            // lstClip
            // 
            this.lstClip.AllowDrop = true;
            this.lstClip.ContextMenu = this.ctxApptClipMenu;
            this.lstClip.DisplayMember = "ToString()";
            this.lstClip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClip.Location = new System.Drawing.Point(0, 32);
            this.lstClip.Name = "lstClip";
            this.lstClip.Size = new System.Drawing.Size(120, 407);
            this.lstClip.TabIndex = 0;
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
            this.label1.Size = new System.Drawing.Size(120, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Access Block Clipboard";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.statusBar1);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 426);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(720, 24);
            this.panelBottom.TabIndex = 2;
            // 
            // statusBar1
            // 
            this.statusBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBar1.Location = new System.Drawing.Point(0, 0);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(720, 24);
            this.statusBar1.SizingGrip = false;
            this.statusBar1.TabIndex = 1;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.dateTimePicker1);
            this.panelTop.Controls.Add(this.lblResource);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(8, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(720, 24);
            this.panelTop.TabIndex = 3;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Dock = System.Windows.Forms.DockStyle.Right;
            this.dateTimePicker1.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(592, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(128, 20);
            this.dateTimePicker1.TabIndex = 4;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // lblResource
            // 
            this.lblResource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResource.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblResource.Location = new System.Drawing.Point(0, 0);
            this.lblResource.Name = "lblResource";
            this.lblResource.Size = new System.Drawing.Size(720, 24);
            this.lblResource.TabIndex = 3;
            this.lblResource.Text = "lblResource";
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.calendarGrid1);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(8, 24);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(712, 402);
            this.panelCenter.TabIndex = 4;
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
            this.calendarGrid1.GridBackColor = "blue";
            this.calendarGrid1.GridEnter = false;
            this.calendarGrid1.Location = new System.Drawing.Point(0, 0);
            this.calendarGrid1.Name = "calendarGrid1";
            this.calendarGrid1.Resources = ((System.Collections.ArrayList)(resources.GetObject("calendarGrid1.Resources")));
            this.calendarGrid1.SelectedAppointment = 0;
            this.calendarGrid1.Size = new System.Drawing.Size(712, 402);
            this.calendarGrid1.StartDate = new System.DateTime(2003, 1, 27, 0, 0, 0, 0);
            this.calendarGrid1.TabIndex = 2;
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
            this.ctxCalGridDelete});
            this.ctxCalendarGrid.Popup += new System.EventHandler(this.ctxCalendarGrid_Popup);
            // 
            // ctxCalGridAdd
            // 
            this.ctxCalGridAdd.Index = 0;
            this.ctxCalGridAdd.Text = "Add New Access Block";
            this.ctxCalGridAdd.Click += new System.EventHandler(this.ctxCalGridAdd_Click);
            // 
            // ctxCalGridEdit
            // 
            this.ctxCalGridEdit.Index = 1;
            this.ctxCalGridEdit.Text = "Edit Access Block";
            this.ctxCalGridEdit.Click += new System.EventHandler(this.ctxCalGridEdit_Click);
            // 
            // ctxCalGridDelete
            // 
            this.ctxCalGridDelete.Index = 2;
            this.ctxCalGridDelete.Text = "Delete Access Block";
            this.ctxCalGridDelete.Click += new System.EventHandler(this.ctxCalGridDelete_Click);
            // 
            // tvSchedules
            // 
            this.tvSchedules.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tvSchedules.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvSchedules.HotTracking = true;
            this.tvSchedules.Location = new System.Drawing.Point(0, 0);
            this.tvSchedules.Name = "tvSchedules";
            this.tvSchedules.Size = new System.Drawing.Size(8, 450);
            this.tvSchedules.Sorted = true;
            this.tvSchedules.TabIndex = 5;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.mnuAvailability,
            this.mnuCalendar,
            this.mnuHelp});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuLoadTemplate,
            this.mnuSaveTemplate,
            this.menuItem6,
            this.mnuSchedulingManagment,
            this.menuItem5,
            this.mnuClose});
            this.menuItem1.Text = "&File";
            // 
            // mnuLoadTemplate
            // 
            this.mnuLoadTemplate.Index = 0;
            this.mnuLoadTemplate.Text = "&Apply Template";
            this.mnuLoadTemplate.Click += new System.EventHandler(this.mnuLoadTemplate_Click);
            // 
            // mnuSaveTemplate
            // 
            this.mnuSaveTemplate.Index = 1;
            this.mnuSaveTemplate.Text = "&Save Template";
            this.mnuSaveTemplate.Click += new System.EventHandler(this.mnuSaveTemplate_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 2;
            this.menuItem6.Text = "-";
            // 
            // mnuSchedulingManagment
            // 
            this.mnuSchedulingManagment.Index = 3;
            this.mnuSchedulingManagment.Text = "Scheduling &Management";
            this.mnuSchedulingManagment.Click += new System.EventHandler(this.mnuSchedulingManagment_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // mnuClose
            // 
            this.mnuClose.Index = 5;
            this.mnuClose.Text = "&Close";
            this.mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
            // 
            // mnuAvailability
            // 
            this.mnuAvailability.Index = 1;
            this.mnuAvailability.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAddNewAV,
            this.mnuRemoveAV,
            this.mnuEditAV});
            this.mnuAvailability.Text = "&Access Blocks";
            this.mnuAvailability.Popup += new System.EventHandler(this.mnuAvailability_Popup);
            // 
            // mnuAddNewAV
            // 
            this.mnuAddNewAV.Index = 0;
            this.mnuAddNewAV.Text = "&Add New Block";
            this.mnuAddNewAV.Click += new System.EventHandler(this.mnuAddNewAV_Click);
            // 
            // mnuRemoveAV
            // 
            this.mnuRemoveAV.Index = 1;
            this.mnuRemoveAV.Text = "&Remove Block";
            this.mnuRemoveAV.Click += new System.EventHandler(this.mnuRemoveAV_Click);
            // 
            // mnuEditAV
            // 
            this.mnuEditAV.Index = 2;
            this.mnuEditAV.Text = "&Edit Block";
            this.mnuEditAV.Click += new System.EventHandler(this.mnuEditAV_Click);
            // 
            // mnuCalendar
            // 
            this.mnuCalendar.Index = 2;
            this.mnuCalendar.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnu1Day,
            this.mnu5Day,
            this.mnu7Day,
            this.menuItem7,
            this.mnuTimeScale,
            this.mnuViewRightPanel});
            this.mnuCalendar.Text = "&View";
            // 
            // mnu1Day
            // 
            this.mnu1Day.Index = 0;
            this.mnu1Day.Text = "&1-Day View";
            this.mnu1Day.Click += new System.EventHandler(this.mnu1Day_Click);
            // 
            // mnu5Day
            // 
            this.mnu5Day.Index = 1;
            this.mnu5Day.Text = "&5-Day View";
            this.mnu5Day.Click += new System.EventHandler(this.mnu5Day_Click);
            // 
            // mnu7Day
            // 
            this.mnu7Day.Index = 2;
            this.mnu7Day.Text = "&7-Day View";
            this.mnu7Day.Click += new System.EventHandler(this.mnu7Day_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 3;
            this.menuItem7.Text = "-";
            // 
            // mnuTimeScale
            // 
            this.mnuTimeScale.Index = 4;
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
            this.mnu15Minute.Text = "&15-Minute";
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
            // mnuViewRightPanel
            // 
            this.mnuViewRightPanel.Index = 5;
            this.mnuViewRightPanel.Text = "&Access Block Clipboard";
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
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(720, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 402);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // CGAVView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(848, 450);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.tvSchedules);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "CGAVView";
            this.Text = "CGAVView";
            this.Load += new System.EventHandler(this.CGAVView_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CGAVView_Closing);
            this.panelRight.ResumeLayout(false);
            this.panelClip.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        private IContainer components;

		#region Member Variables

        private	CGAVDocument			m_Document;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelCenter;
		private System.Windows.Forms.Label lblResource;
		private System.Windows.Forms.TreeView tvSchedules;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private IndianHealthService.ClinicalScheduling.CalendarGrid calendarGrid1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuAvailability;
		private System.Windows.Forms.MenuItem mnuAddNewAV;
		private System.Windows.Forms.MenuItem mnuRemoveAV;
		private System.Windows.Forms.MenuItem mnuEditAV;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuSchedulingManagment;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mnuClose;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.MenuItem mnuLoadTemplate;
		private System.Windows.Forms.MenuItem mnuSaveTemplate;
		private System.Windows.Forms.Panel panelClip;
		private System.Windows.Forms.ListBox lstClip;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuItem mnuCalendar;
		private System.Windows.Forms.MenuItem mnuViewRightPanel;
		private System.Windows.Forms.MenuItem mnuTimeScale;
		private System.Windows.Forms.MenuItem mnu10Minute;
		private System.Windows.Forms.MenuItem mnu15Minute;
		private System.Windows.Forms.MenuItem mnu20Minute;
		private System.Windows.Forms.MenuItem mnu30Minute;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem mnu1Day;
		private System.Windows.Forms.MenuItem mnu5Day;
		private System.Windows.Forms.MenuItem mnu7Day;
		private System.Windows.Forms.Splitter splitter1;
		private CGDocumentManager		m_DocManager;
		private CGAppointments		m_ClipList;
		private System.Windows.Forms.ContextMenu ctxApptClipMenu;
		private System.Windows.Forms.MenuItem mnuRemoveClipItem;
		private System.Windows.Forms.MenuItem mnuClearClipItems;
		private System.Windows.Forms.ContextMenu ctxCalendarGrid;
		private System.Windows.Forms.MenuItem ctxCalGridAdd;
		private System.Windows.Forms.MenuItem ctxCalGridEdit;
		private System.Windows.Forms.MenuItem ctxCalGridDelete;
		private bool				m_bDragDropStart = false;

		#endregion

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
		public CGAVDocument Document
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

		public CGAppointments AVBlocks
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
		
		#endregion //Properties

		#region Methods

		private void RaiseRPMSEvent(string sEvent, string sParams)
		{
			try
			{
				//Signal RPMS to raise an event
//				string sSql;
//				sSql = "BSDX RAISE EVENT^" + sEvent + "^" + sParams + "^^";
//				DataTable dtAppt =m_DocManager.RPMSDataTable(sSql, "RaiseEvent");
				this.m_DocManager.ConnectInfo.RaiseEvent(sEvent, sParams, true);
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		void UpdateStatusBar(DateTime dStart, DateTime dEnd)
		{
			string sMsg =  dStart.ToShortTimeString() + " to " + dEnd.ToShortTimeString();
			this.statusBar1.Text = sMsg;
		}

		private void AvailabilityEdit()
		{
			try
			{
				int nApptID = this.calendarGrid1.SelectedAppointment;
				if (nApptID != 0)
				{

					CGAppointment pAppt = this.Document.AVBlocks.GetAppointment(nApptID);
					DAccessBlock dA = new DAccessBlock();
					dA.InitializePage(pAppt, m_DocManager.GlobalDataSet);
					calendarGrid1.CGToolTip.Active = false;
					if (dA.ShowDialog(this) == DialogResult.Cancel)
					{
						calendarGrid1.CGToolTip.Active = true;
						return;
					}
					calendarGrid1.CGToolTip.Active = true;
					this.Document.DeleteAvailability(nApptID);
					this.Document.CreateAppointment(dA.Appointment);
					this.calendarGrid1.Invalidate();
					this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add new access block  " +  ex.Message, "IHS Clinical Scheduling");
				return;
			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		private void AvailabilityAddNew()
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

				foreach (CGAppointment a in this.Document.m_AVBlocks.AppointmentTable.Values)
				{
					DateTime sStart2 = a.StartTime;
					DateTime sEnd2 = a.EndTime;
					if (CGSchedLib.TimesOverlap(dStart, dEnd, a.StartTime, a.EndTime))
					{
						MessageBox.Show("Access blocks may not overlap.","IHS Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}
			
				string sNote = "";
				DAccessBlock dA = new DAccessBlock();
				dA.InitializePage(dStart, dEnd, sResource, sNote, this.m_DocManager.GlobalDataSet);
				if (dA.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}
				appt.StartTime = dStart;
				appt.EndTime = dEnd;
				appt.Resource = sResource;
				appt.Note = " " + dA.Note; // + ": " + dA.Slots.ToString()+ " Slots";
				appt.AccessTypeID = dA.AccessTypeID;
				appt.Slots = dA.Slots;

				//Call Document to add a new appointment
				this.Document.CreateAppointment(appt);
				this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add new access block  " +  ex.Message, "IHS Clinical Scheduling");
				return;
			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}//End AvailabilityAddNew

		private void AppointmentDelete() 
		{
			calendarGrid1.CGToolTip.Active = false;
			string sMsg = " this access block?";
			if (calendarGrid1.SelectedAppointments.AppointmentTable.Count > 1)
				sMsg = " these access blocks?";

			if (MessageBox.Show("Are you sure you want to delete" + sMsg, "IHS Clinical Scheduling",  MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				calendarGrid1.CGToolTip.Active = true;
				return;
			}
			calendarGrid1.CGToolTip.Active = true;

			bool bDeleted = false;
			foreach (CGAppointment a in this.calendarGrid1.SelectedAppointments.AppointmentTable.Values)
			{
				int nApptID = a.AppointmentKey;
				Debug.Assert(nApptID != 0);
				try
				{
					Document.DeleteAvailability(nApptID);
					bDeleted = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unable to delete access block" +  ex.Message, "IHS Clinical Scheduling");
				}
			}
			if (bDeleted == true)
			{
				try
				{
					this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
					RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
				this.calendarGrid1.Invalidate();
			}		
		}

		private void AccessTypeManage()
		{
//			DAccessType dAT = new DAccessType();
//			dAT.InitializePage(0, this.m_DocManager.GlobalDataSet, this.m_DocManager.ADOConnection);
//			if (dAT.ShowDialog(this) == DialogResult.Cancel)
//			{
//				return;
//			}
		}

		private void SchedulingManagement()
		{
			try
			{
				bool bLock = DocManager.ConnectInfo.bmxNetLib.Lock("^BSDXMGR", "+");
				if (bLock == false)
				{
					throw new Exception("Another user is currently in Scheduling Management.  Try later.");
				}

				DManagement dMgm = new DManagement();
				dMgm.InitializeDialog(this.m_DocManager);
			
				if (dMgm.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}
				bLock = DocManager.ConnectInfo.bmxNetLib.Lock("^BSDXMGR", "-");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Scheduling Management Error:  " +  ex.Message, "IHS Clinical Scheduling");
			}
		}

		public void UpdateArrays()
		{
			//TODO:  Create these components
			this.calendarGrid1.Resources = this.m_Document.Resources;
			this.calendarGrid1.OnUpdateArrays();
			this.lblResource.Text = this.m_Document.DocName;
			
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
		#endregion //Methods

		#region Events

		private void dateTimePicker1_ValueChanged(object sender, System.EventArgs e)
		{
			DateTime dDate = dateTimePicker1.Value;
			dDate = dDate.Date;
			this.Document.SelectedDate = dDate;
			if (this.calendarGrid1.Columns > 1)
			{
				this.StartDate = this.Document.StartDate;
			}
			else
			{
				this.StartDate = this.Document.SelectedDate;
			}
			this.Document.UpdateAllViews();
			this.calendarGrid1.Invalidate();
		}

		private void CGAVView_Load(object sender, System.EventArgs e)
		{
			Debug.Assert (this.Document != null);

			//Register the view
			CGDocumentManager.Current.RegisterAVDocumentView(this.Document, this);
            this.mnu5Day.Click += new System.EventHandler(this.dateTimePicker1_ValueChanged); // MJL 1/17/2007
            this.mnu7Day.Click += new System.EventHandler(this.dateTimePicker1_ValueChanged);

		}

		private void CGAVView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.calendarGrid1.CloseGrid();
			DocManager.ConnectInfo.bmxNetLib.Lock("^BSDXRES(" + Document.ResourceID.ToString() + ")", "-");
		}

		private void calendarGrid1_CGSelectionChanged(object sender, IndianHealthService.ClinicalScheduling.CGSelectionChangedArgs e)
		{
			UpdateStatusBar(e.StartTime, e.EndTime);
		}

		private void calendarGrid1_CGAppointmentChanged(object sender, IndianHealthService.ClinicalScheduling.CGAppointmentChangedArgs e)
		{
			try
			{
//				this.DocManager.EnableAutoRefresh(false);

				if (MessageBox.Show("Are you sure you want to move this access block?", "IHS Clinical Scheduling",  MessageBoxButtons.YesNo) != DialogResult.Yes)
					return;

				m_Document.ChangeAppointmentTime(e.Appointment, e.StartTime, e.EndTime, e.Resource);
				this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to change access block  " +  ex.Message, "IHS Clinical Scheduling");
				this.m_DocManager.UpdateViews();
				return;
			}
			finally
			{
//				this.DocManager.EnableAutoRefresh(true);
			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		private void mnuAddNewAV_Click(object sender, System.EventArgs e)
		{
			AvailabilityAddNew();
		}

		private void mnuRemoveAV_Click(object sender, System.EventArgs e)
		{
			this.AppointmentDelete();
		}

		private void mnuManageAT_Click(object sender, System.EventArgs e)
		{
			AccessTypeManage();
		}

		private void mnuEditAV_Click(object sender, System.EventArgs e)
		{
			AvailabilityEdit();
		}

		private void mnuAvailability_Popup(object sender, System.EventArgs e)
		{
			this.mnuAddNewAV.Enabled = (this.calendarGrid1.SelectedRange.Cells.CellCount > 0);	
			this.mnuEditAV.Enabled = (this.calendarGrid1.SelectedAppointment > 0);
			this.mnuRemoveAV.Enabled = (this.calendarGrid1.SelectedAppointment > 0);
		}

		private void mnuSchedulingManagment_Click(object sender, System.EventArgs e)
		{
			SchedulingManagement();
		}

		#endregion //Events

		private void mnuClose_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = MessageBox.Show("Are you sure you want to close this schedule?", Application.ProductName, MessageBoxButtons.OKCancel);
			if (dr != DialogResult.OK)
				return;

			this.Close();
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("IHS Clinical Scheduling Version " + Application.ProductVersion, "IHS Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Information);	
		}

		private void ImplementMsg()
		{
			MessageBox.Show("IHS Clinical Scheduling", "TODO: Implement this function");
		}

		private void mnuLoadTemplate_Click(object sender, System.EventArgs e)
		{
			/*
			 * Display dialog to collect:
			 * - Number of weeks to apply template
			 * - Starting week Monday
			 * - Template path & filename
			 * 
			 * for each week,
			 *	Delete all availability during that week
			 *  apply the template
			 * 
			 */
			DAccessTemplate dlg = new DAccessTemplate();
			dlg.InitializePage();
			if (dlg.ShowDialog(this) == DialogResult.Cancel)
			{
				return;
			}

			try
			{
				OpenFileDialog openFileDialog1 = dlg.FileDialog;
				Stream streamFile;
				if((streamFile = openFileDialog1.OpenFile())== null)
				{
					MessageBox.Show("Unable to open template file.");
					return;
				}

				BinaryFormatter formatter = new BinaryFormatter();
				CGAppointments cgaTemp = (CGAppointments) formatter.Deserialize(streamFile);
				streamFile.Close();

				DateTime dtStart = dlg.StartDate;
				int nWeeksToApply = dlg.WeeksToApply;
				DateTime dtEnd = dtStart.AddDays(6); // or 7?
				string sResourceID = this.m_Document.ResourceID.ToString();
				DataTable dt;

				for (int j=1; j < nWeeksToApply + 1; j++)
				{
					//Convert start and end to string
					string sStart = dtStart.ToString("M/d/yyyy");
					string sEnd = dtEnd.ToString("M/d/yyyy");

					//Cancel all existing access blocks in the date range
					string sSql = "BSDX CANCEL AV BY DATE^" + sResourceID + "^" + sStart + "^" + sEnd;
					dt = this.m_DocManager.RPMSDataTable(sSql, "Cancelled");

					//for each CGAppointment in AVBlocks, call AddNew
					string sResource = "";
					sResource = (string) this.Document.Resources[0];
					foreach (CGAppointment a in cgaTemp.AppointmentTable.Values)
					{
						//Change the resource to the current one
						a.Resource = sResource;

						//Change the date to correspond to the GridColumn member
						int col = a.GridColumn;
						col--;
						DateTime dBuildDate = dtStart.Date;
						dBuildDate = dBuildDate.AddDays(col);
						dBuildDate = dBuildDate.AddHours(a.StartTime.Hour);
						dBuildDate = dBuildDate.AddMinutes(a.StartTime.Minute);
						a.StartTime = dBuildDate;
						dBuildDate = dtStart.Date;
						dBuildDate = dBuildDate.AddDays(col);
						dBuildDate = dBuildDate.AddHours(a.EndTime.Hour);
						dBuildDate = dBuildDate.AddMinutes(a.EndTime.Minute);
						a.EndTime = dBuildDate;

						//Call Document to add a new appointment
						this.Document.CreateAppointmentAuto(a);
					}

					//Increment start and end
					dtStart = dtStart.AddDays(7);
					dtEnd = dtStart.AddDays(6);

				}//end for
				try
				{
					RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				}
				catch (Exception ex)
				{
					Debug.Write(ex.Message);
				}
				this.calendarGrid1.Invalidate();
				this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error loading template:  " + ex.Message, "IHS Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

				//Check that there are no blocks in document
//				if (this.Document.AVBlocks.AppointmentTable.Count > 0)
//				{
//					Exception bmxex = new Exception("You may not load a template if there are already access blocks defined for the week.");
//					throw bmxex;
//				}
//				OpenFileDialog openFileDialog1 = new OpenFileDialog();
//				string sPath = "";
//				sPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
//
//				openFileDialog1.InitialDirectory = "c:\\" ;
//				openFileDialog1.InitialDirectory = sPath ;
//				openFileDialog1.Filter = "Schedule Template Files (*.bsdxa)|*.bsdxa|All files (*.*)|*.*" ;
//				openFileDialog1.FilterIndex = 0 ;
//				openFileDialog1.RestoreDirectory = true ;

//				Stream streamFile;
//				if(openFileDialog1.ShowDialog() == DialogResult.OK)
//				{
//					if((streamFile = openFileDialog1.OpenFile())!= null)
//					{
//						BinaryFormatter formatter = new BinaryFormatter();
//						CGAppointments cgaTemp = (CGAppointments) formatter.Deserialize(streamFile);
//						streamFile.Close();
//
//						//for each CGAppointment in AVBlocks, call AddNew
//						string sResource = "";
//						sResource = (string) this.Document.Resources[0];
//						foreach (CGAppointment a in cgaTemp.AppointmentTable.Values)
//						{
//							//Change the resource to the current one
//							a.Resource = sResource;
//
//							//Change the date to correspond to the GridColumn member
//							int col = a.GridColumn;
//							Rectangle r = new Rectangle(0,0,0,0);
//							int row = 5;
//							CGCell c = new CGCell(r, row, col);
//							DateTime dTemp = this.calendarGrid1.GetTimeFromCell(c);
//							DateTime dBuildDate = dTemp.Date;
//							dBuildDate = dBuildDate.AddHours(a.StartTime.Hour);
//							dBuildDate = dBuildDate.AddMinutes(a.StartTime.Minute);
//							a.StartTime = dBuildDate;
//							dBuildDate = dTemp.Date;
//							dBuildDate = dBuildDate.AddHours(a.EndTime.Hour);
//							dBuildDate = dBuildDate.AddMinutes(a.EndTime.Minute);
//							a.EndTime = dBuildDate;
//
//							//Call Document to add a new appointment
//							this.Document.CreateAppointment(a);
//						}
//						try
//						{
//							RaiseRPMSEvent("SCHEDULE-" + m_Document.DocName, "");
//						}
//						catch (Exception ex)
//						{
//							Debug.Write(ex.Message);
//						}
//						this.calendarGrid1.Invalidate();
//						this.m_DocManager.UpdateViews((string) this.m_Document.Resources[0], "");
//					}
//				}
//			}//end try
//			catch (Exception ex)
//			{
//				MessageBox.Show(this, "Error loading template:  " + ex.Message, "IHS Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//			}
//		}

		private void mnuSaveTemplate_Click(object sender, System.EventArgs e)
		{
			try
			{
				//Check that there is at least one block in document
//				if (this.Document.AVBlocks.AppointmentTable.Count == 0)
//				{
//					Exception bmxex = new Exception("You may not save a template if there are no access blocks defined for the week.");
//					throw bmxex;
//				}

				Stream streamFile ;
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
				saveFileDialog1.Filter = "Schedule Template Files (*.bsdxa)|*.bsdxa|All files (*.*)|*.*" ;
				saveFileDialog1.FilterIndex = 0 ;
				saveFileDialog1.RestoreDirectory = true ;
				saveFileDialog1.AddExtension = true;
				saveFileDialog1.DefaultExt = "bsdxa";
 
				//Iterate through AVBlocks and set the GridColumn member based on the date
				foreach (CGAppointment a in this.Document.AVBlocks.AppointmentTable.Values)
				{
					//Get the column by subtracting the grid's Start day from dStart.		
					int col =(int) a.StartTime.DayOfWeek - (int) this.calendarGrid1.StartDate.DayOfWeek + 1;
					a.GridColumn = col;
				}

				if(saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					if((streamFile = saveFileDialog1.OpenFile()) != null)
					{
						BinaryFormatter formatter = new BinaryFormatter();

						formatter.Serialize(streamFile, this.Document.AVBlocks);

						streamFile.Close();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error saving template:  " + ex.Message, "IHS Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuViewRightPanel_Click(object sender, System.EventArgs e)
		{
			this.mnuViewRightPanel.Checked = this.panelRight.Visible;
			this.panelRight.Visible = !(this.panelRight.Visible);
			this.mnuViewRightPanel.Checked = !(this.mnuViewRightPanel.Checked);
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
			//CalendarGrid Event raised when appointment is dragdropped onto the grid

			try
			{
				CGAppointment aCopy = new CGAppointment();
				aCopy.CreateAppointment(e.StartTime, e.EndTime, e.Appointment.Note, 0, e.Resource);
				aCopy.AccessTypeID = e.AccessTypeID;
				aCopy.Slots = e.Slots;

				m_Document.CreateAppointment(aCopy);
				//			RaiseRPMSEvent("SCHEDULE-" + e.Resource , "");
				this.m_DocManager.UpdateViews(e.Resource, e.OldResource);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to add new access block  " +  ex.Message, "IHS Clinical Scheduling");
				return;
			}
			try
			{
				RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				if (e.Resource != e.OldResource)
					RaiseRPMSEvent("BSDX SCHEDULE", m_Document.DocName);
				this.calendarGrid1.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		#region ctxApptClipMenu Handlers

		private void ctxApptClipMenu_Popup(object sender, System.EventArgs e)
		{
			mnuClearClipItems.Enabled = (m_ClipList.AppointmentTable.Count > 0);
			mnuRemoveClipItem.Enabled = (lstClip.SelectedIndex > -1);
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

		private void mnuClearClipItems_Click(object sender, System.EventArgs e)
		{
			this.m_ClipList.ClearAllAppointments();
			lstClip.Items.Clear();
		}

		#endregion ctxApptClipMenu Handlers

		#region ctxCalGridMenu Handlers

		private void ctxCalGridAdd_Click(object sender, System.EventArgs e)
		{
			AvailabilityAddNew();
		}

		private void ctxCalGridEdit_Click(object sender, System.EventArgs e)
		{
			AvailabilityEdit();
		}

		private void ctxCalGridDelete_Click(object sender, System.EventArgs e)
		{
			AppointmentDelete();
		}

		private void ctxCalendarGrid_Popup(object sender, System.EventArgs e)
		{
			//Toggle availability of make, edit and delete appointments
			//based on whether appropriate element is selected.
			ctxCalGridAdd.Enabled = ((calendarGrid1.SelectedRange.Cells.CellCount > 0) );	
			ctxCalGridDelete.Enabled = (calendarGrid1.SelectedAppointment > 0);
			ctxCalGridEdit.Enabled = (calendarGrid1.SelectedAppointment > 0);
		}

		private void calendarGrid1_DoubleClick(object sender, System.EventArgs e)
		{
			if (calendarGrid1.SelectedAppointment > 0)
				AvailabilityEdit();
		}

		#endregion ctxCalGridMenu Handlers

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
			this.Document.UpdateAllViews();
		}

		private void mnu7Day_Click(object sender, System.EventArgs e)
		{
			this.calendarGrid1.Columns = 7;
		}

		private void PositionGrid(CalendarGrid cg, int nHour)
		{
			//Position grid to nHour
			int nRow = 0, nCol = 0;
			DateTime dStart = DateTime.Today;
			dStart = dStart.AddHours(nHour);
			cg.GetCellFromTime(dStart, ref nRow, ref nCol, false, "");
			int nHeight = cg.CellHeight;
			nHeight *= nRow;
			cg.AutoScrollPosition = new Point(50, nHeight);
			cg.Invalidate();
		}
	}
}
