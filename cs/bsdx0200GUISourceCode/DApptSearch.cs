using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//using System.Data.OleDb;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DApptSearch.
	/// </summary>
	public class DApptSearch : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckedListBox lstAccessTypes;
		private System.Windows.Forms.ComboBox cboAccessTypeFilter;
		private System.Windows.Forms.GroupBox grpDayOfWeek;
		private System.Windows.Forms.CheckBox chkSun;
		private System.Windows.Forms.CheckBox chkSat;
		private System.Windows.Forms.CheckBox chkFri;
		private System.Windows.Forms.CheckBox chkThu;
		private System.Windows.Forms.CheckBox chkWed;
		private System.Windows.Forms.CheckBox chkTue;
		private System.Windows.Forms.CheckBox chkMon;
		private System.Windows.Forms.GroupBox grpTimeOfDay;
		private System.Windows.Forms.RadioButton rdoBoth;
		private System.Windows.Forms.RadioButton rdoPM;
		private System.Windows.Forms.RadioButton rdoAM;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MonthCalendar calStartDate;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.DataGrid grdResult;
		private System.Windows.Forms.Button cmdSearch;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DApptSearch()
		{
			InitializeComponent();
		}

		#region Fields

		private CGDocumentManager	m_DocManager;
		
		private DataSet	m_dsGlobal;
		DataTable		m_dtTypes;
		DataView		m_dvTypes;

		DateTime		m_dStart;
		DateTime		m_dEnd;
		ArrayList		m_alResources;
		ArrayList		m_alAccessTypes;
		string			m_sWeekDays;
		string			m_sAmpm;

		DataTable		m_dtResult;
		DataView		m_dvResult;

		string			m_sSelectedResource;
		DateTime		m_sSelectedDate;

		#endregion Fields

        #region Methods

        public void LoadListBox(string sGroup)
		{
			if (sGroup == "ALL")
			{
				//Load the Access Type list box with ALL access types
				m_dtTypes = m_dsGlobal.Tables["AccessTypes"];
				m_dvTypes = new DataView(m_dtTypes);
				lstAccessTypes.DataSource = m_dvTypes;
				lstAccessTypes.DisplayMember = "ACCESS_TYPE_NAME";
				lstAccessTypes.Tag = 1; //This holds the column index of the ACCESS_TYPE_NAME column
				lstAccessTypes.ValueMember = "BMXIEN";
			}
			else 
			{
				//Load the Access Type list box with active access types belonging
				//to group sGroup

				//Build AccessGroup table containing *active* AccessTypes and their Groups
				m_dtTypes = m_dsGlobal.Tables["AccessGroupType"];
				//Create a view that is filterable on Access Group
				m_dvTypes = new DataView(m_dtTypes);
				m_dvTypes.RowFilter = "ACCESS_GROUP = '" + this.cboAccessTypeFilter.Text + "'";
				lstAccessTypes.DataSource = m_dvTypes;
				lstAccessTypes.DisplayMember = "ACCESS_TYPE";
				lstAccessTypes.ValueMember = "ACCESS_TYPE_ID";
				lstAccessTypes.Tag = 4; //This holds the column index of the ACCESS_TYPE column
			}
		}

		public void InitializePage(ArrayList alResources, CGDocumentManager docManager)
		{

			this.m_DocManager = docManager;
			this.m_dsGlobal = m_DocManager.GlobalDataSet;
			System.IntPtr pHandle = this.Handle;
			LoadListBox("ALL");

			m_dStart = DateTime.Today;
			m_dEnd = new DateTime(9999);
			this.m_alResources = alResources;
			this.m_alAccessTypes = new ArrayList();
			this.m_sAmpm="both";
			this.m_sWeekDays = "";

			//Load filter combo with list of access type groups
			DataTable dtGroup = m_dsGlobal.Tables["AccessGroup"];
			DataSet dsTemp = new DataSet("dsTemp");
			dsTemp.Tables.Add(dtGroup.Copy());
			DataTable dtTemp = dsTemp.Tables["AccessGroup"];
			DataView dvGroup = new DataView(dtTemp);
			DataRowView drv = dvGroup.AddNew();
			drv["ACCESS_GROUP"]="<Show All Access Types>";
			cboAccessTypeFilter.DataSource = dvGroup;
			cboAccessTypeFilter.DisplayMember = "ACCESS_GROUP";
			cboAccessTypeFilter.SelectedText = "<Show All Access Types>";
			cboAccessTypeFilter.SelectedIndex = cboAccessTypeFilter.Items.Count - 1;
			cboAccessTypeFilter.Refresh();

			//Create DataGridTableStyle for Result grid
			DataGridTableStyle tsResult = new DataGridTableStyle();
			tsResult.MappingName = "Result";
			tsResult.ReadOnly = true;
			// Add START_TIME column style.
			DataGridTextBoxColumn colStartTime = new DataGridTextBoxColumn();
			colStartTime.MappingName = "START_TIME";
			colStartTime.HeaderText = "Start Time";
			colStartTime.Width = 200;
			colStartTime.Format = "f";
			tsResult.GridColumnStyles.Add(colStartTime);
			
			// Add END_TIME column style.
			DataGridTextBoxColumn colEndTime = new DataGridTextBoxColumn();
			colEndTime.MappingName = "END_TIME";
			colEndTime.HeaderText = "End Time";
			colEndTime.Width = 75;
			colEndTime.Format = "h:mm tt";
			tsResult.GridColumnStyles.Add(colEndTime);

			// Add RESOURCE column style.
			DataGridTextBoxColumn colResource = new DataGridTextBoxColumn();
			colResource.MappingName = "RESOURCE";
			colResource.HeaderText = "Resource";
			colResource.Width = 200;
			tsResult.GridColumnStyles.Add(colResource);

			// Add SLOTS column style.
			DataGridTextBoxColumn colSlots = new DataGridTextBoxColumn();
			colSlots.MappingName = "SLOTS";
			colSlots.HeaderText = "Slots";
			colSlots.Width = 50;
			tsResult.GridColumnStyles.Add(colSlots);

			// Add AMPM column style.
			DataGridTextBoxColumn colAccess = new DataGridTextBoxColumn();
			colAccess.MappingName = "ACCESSNAME";
			colAccess.HeaderText = "Access Type";
			colAccess.Width = 200;
			tsResult.GridColumnStyles.Add(colAccess);
			grdResult.TableStyles.Add(tsResult);

			this.UpdateDialogData(true);
		
		}

		/// <summary>
		/// If b is true, moves member vars into control data
		/// otherwise, moves control data into member vars
		/// </summary>
		/// <param name="b"></param>
		private void UpdateDialogData(bool b)
		{
			if (b == true) //move member vars into controls
			{

			}
			else //move control data into member vars
			{

				//Build AccessType list

				this.m_alAccessTypes.Clear();
				
				for (int j = 0; j < this.lstAccessTypes.CheckedItems.Count; j++)
				{
					DataRowView drv = (DataRowView) lstAccessTypes.CheckedItems[j];
					int nIndex = (int) lstAccessTypes.Tag;
					string sItem = drv.Row.ItemArray[nIndex].ToString();
					m_alAccessTypes.Add(sItem); 
				}

				//AM/PM
				this.m_sAmpm = (this.rdoAM.Checked == true) ? "AM":"BOTH";
				if (this.m_sAmpm != "AM")
					this.m_sAmpm = (this.rdoPM.Checked == true) ? "PM":"BOTH";

				
				//Weekday
				this.m_sWeekDays = ""; //any
				if (chkMon.Checked == true)
					m_sWeekDays += "Monday";
				if (chkTue.Checked == true)
					m_sWeekDays += "Tuesday";
				if (chkWed.Checked == true)
					m_sWeekDays += "Wednesday";
				if (chkThu.Checked == true)
					m_sWeekDays += "Thursday";
				if (chkFri.Checked == true)
					m_sWeekDays += "Friday";
				if (chkSat.Checked == true)
					m_sWeekDays += "Saturday";
				if (chkSun.Checked == true)
					m_sWeekDays += "Sunday";

				//Start
				this.m_dStart = this.calStartDate.SelectionStart;

				//End
				m_dEnd = calStartDate.SelectionEnd;
				m_dEnd = m_dEnd.AddHours(23);
				m_dEnd = m_dEnd.AddMinutes(59);

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

        #endregion Methods

        #region Windows Form Designer generated code
        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.grpDescription = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstAccessTypes = new System.Windows.Forms.CheckedListBox();
            this.cboAccessTypeFilter = new System.Windows.Forms.ComboBox();
            this.grpDayOfWeek = new System.Windows.Forms.GroupBox();
            this.chkSun = new System.Windows.Forms.CheckBox();
            this.chkSat = new System.Windows.Forms.CheckBox();
            this.chkFri = new System.Windows.Forms.CheckBox();
            this.chkThu = new System.Windows.Forms.CheckBox();
            this.chkWed = new System.Windows.Forms.CheckBox();
            this.chkTue = new System.Windows.Forms.CheckBox();
            this.chkMon = new System.Windows.Forms.CheckBox();
            this.grpTimeOfDay = new System.Windows.Forms.GroupBox();
            this.rdoBoth = new System.Windows.Forms.RadioButton();
            this.rdoPM = new System.Windows.Forms.RadioButton();
            this.rdoAM = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.calStartDate = new System.Windows.Forms.MonthCalendar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grdResult = new System.Windows.Forms.DataGrid();
            this.panel1.SuspendLayout();
            this.pnlDescription.SuspendLayout();
            this.grpDescription.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpDayOfWeek.SuspendLayout();
            this.grpTimeOfDay.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdSearch);
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 461);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(923, 40);
            this.panel1.TabIndex = 4;
            // 
            // cmdSearch
            // 
            this.cmdSearch.Location = new System.Drawing.Point(536, 8);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(72, 24);
            this.cmdSearch.TabIndex = 2;
            this.cmdSearch.Text = "Search";
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(616, 8);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(64, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(128, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.Visible = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.grpDescription);
            this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescription.Location = new System.Drawing.Point(0, 397);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Size = new System.Drawing.Size(923, 64);
            this.pnlDescription.TabIndex = 47;
            // 
            // grpDescription
            // 
            this.grpDescription.Controls.Add(this.lblDescription);
            this.grpDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescription.Location = new System.Drawing.Point(0, 0);
            this.grpDescription.Name = "grpDescription";
            this.grpDescription.Size = new System.Drawing.Size(923, 64);
            this.grpDescription.TabIndex = 0;
            this.grpDescription.TabStop = false;
            this.grpDescription.Text = "Description";
            // 
            // lblDescription
            // 
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 16);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(917, 45);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Search for available appointment times using this panel.  You may narrow your sea" +
                "rch by selecting an access type or by selecting specific days of the week or tim" +
                "es of day.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lstAccessTypes);
            this.groupBox1.Controls.Add(this.cboAccessTypeFilter);
            this.groupBox1.Controls.Add(this.grpDayOfWeek);
            this.groupBox1.Controls.Add(this.grpTimeOfDay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.calStartDate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(923, 208);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Parameters";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(684, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 63;
            this.label3.Text = "Access Type:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(684, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 62;
            this.label2.Text = "Access Group:";
            // 
            // lstAccessTypes
            // 
            this.lstAccessTypes.CheckOnClick = true;
            this.lstAccessTypes.HorizontalScrollbar = true;
            this.lstAccessTypes.Location = new System.Drawing.Point(661, 88);
            this.lstAccessTypes.MultiColumn = true;
            this.lstAccessTypes.Name = "lstAccessTypes";
            this.lstAccessTypes.Size = new System.Drawing.Size(250, 109);
            this.lstAccessTypes.TabIndex = 61;
            // 
            // cboAccessTypeFilter
            // 
            this.cboAccessTypeFilter.Location = new System.Drawing.Point(661, 40);
            this.cboAccessTypeFilter.Name = "cboAccessTypeFilter";
            this.cboAccessTypeFilter.Size = new System.Drawing.Size(250, 21);
            this.cboAccessTypeFilter.TabIndex = 60;
            this.cboAccessTypeFilter.Text = "cboAccessTypeFilter";
            this.cboAccessTypeFilter.SelectionChangeCommitted += new System.EventHandler(this.cboAccessTypeFilter_SelectionChangeCommitted);
            // 
            // grpDayOfWeek
            // 
            this.grpDayOfWeek.Controls.Add(this.chkSun);
            this.grpDayOfWeek.Controls.Add(this.chkSat);
            this.grpDayOfWeek.Controls.Add(this.chkFri);
            this.grpDayOfWeek.Controls.Add(this.chkThu);
            this.grpDayOfWeek.Controls.Add(this.chkWed);
            this.grpDayOfWeek.Controls.Add(this.chkTue);
            this.grpDayOfWeek.Controls.Add(this.chkMon);
            this.grpDayOfWeek.Location = new System.Drawing.Point(311, 94);
            this.grpDayOfWeek.Name = "grpDayOfWeek";
            this.grpDayOfWeek.Size = new System.Drawing.Size(240, 101);
            this.grpDayOfWeek.TabIndex = 59;
            this.grpDayOfWeek.TabStop = false;
            this.grpDayOfWeek.Text = "Day of the Week";
            // 
            // chkSun
            // 
            this.chkSun.Location = new System.Drawing.Point(176, 64);
            this.chkSun.Name = "chkSun";
            this.chkSun.Size = new System.Drawing.Size(48, 16);
            this.chkSun.TabIndex = 6;
            this.chkSun.Text = "Sun";
            // 
            // chkSat
            // 
            this.chkSat.Location = new System.Drawing.Point(128, 64);
            this.chkSat.Name = "chkSat";
            this.chkSat.Size = new System.Drawing.Size(48, 16);
            this.chkSat.TabIndex = 5;
            this.chkSat.Text = "Sat";
            // 
            // chkFri
            // 
            this.chkFri.Location = new System.Drawing.Point(72, 64);
            this.chkFri.Name = "chkFri";
            this.chkFri.Size = new System.Drawing.Size(48, 16);
            this.chkFri.TabIndex = 4;
            this.chkFri.Text = "Fri";
            // 
            // chkThu
            // 
            this.chkThu.Location = new System.Drawing.Point(16, 64);
            this.chkThu.Name = "chkThu";
            this.chkThu.Size = new System.Drawing.Size(48, 16);
            this.chkThu.TabIndex = 3;
            this.chkThu.Text = "Thu";
            // 
            // chkWed
            // 
            this.chkWed.Location = new System.Drawing.Point(128, 32);
            this.chkWed.Name = "chkWed";
            this.chkWed.Size = new System.Drawing.Size(48, 16);
            this.chkWed.TabIndex = 2;
            this.chkWed.Text = "Wed";
            // 
            // chkTue
            // 
            this.chkTue.Location = new System.Drawing.Point(72, 32);
            this.chkTue.Name = "chkTue";
            this.chkTue.Size = new System.Drawing.Size(48, 16);
            this.chkTue.TabIndex = 1;
            this.chkTue.Text = "Tue";
            // 
            // chkMon
            // 
            this.chkMon.Location = new System.Drawing.Point(16, 32);
            this.chkMon.Name = "chkMon";
            this.chkMon.Size = new System.Drawing.Size(48, 16);
            this.chkMon.TabIndex = 0;
            this.chkMon.Text = "Mon";
            // 
            // grpTimeOfDay
            // 
            this.grpTimeOfDay.Controls.Add(this.rdoBoth);
            this.grpTimeOfDay.Controls.Add(this.rdoPM);
            this.grpTimeOfDay.Controls.Add(this.rdoAM);
            this.grpTimeOfDay.Location = new System.Drawing.Point(311, 32);
            this.grpTimeOfDay.Name = "grpTimeOfDay";
            this.grpTimeOfDay.Size = new System.Drawing.Size(240, 48);
            this.grpTimeOfDay.TabIndex = 58;
            this.grpTimeOfDay.TabStop = false;
            this.grpTimeOfDay.Text = "Time of Day";
            // 
            // rdoBoth
            // 
            this.rdoBoth.Checked = true;
            this.rdoBoth.Location = new System.Drawing.Point(176, 24);
            this.rdoBoth.Name = "rdoBoth";
            this.rdoBoth.Size = new System.Drawing.Size(48, 16);
            this.rdoBoth.TabIndex = 2;
            this.rdoBoth.TabStop = true;
            this.rdoBoth.Text = "Both";
            // 
            // rdoPM
            // 
            this.rdoPM.Location = new System.Drawing.Point(96, 24);
            this.rdoPM.Name = "rdoPM";
            this.rdoPM.Size = new System.Drawing.Size(72, 16);
            this.rdoPM.TabIndex = 1;
            this.rdoPM.Text = "PM Only";
            // 
            // rdoAM
            // 
            this.rdoAM.Location = new System.Drawing.Point(16, 24);
            this.rdoAM.Name = "rdoAM";
            this.rdoAM.Size = new System.Drawing.Size(72, 16);
            this.rdoAM.TabIndex = 0;
            this.rdoAM.Text = "AM Only";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 57;
            this.label1.Text = "Date Range:";
            // 
            // calStartDate
            // 
            this.calStartDate.Location = new System.Drawing.Point(16, 40);
            this.calStartDate.MaxSelectionCount = 62;
            this.calStartDate.Name = "calStartDate";
            this.calStartDate.TabIndex = 56;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.grdResult);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 208);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(923, 189);
            this.groupBox2.TabIndex = 57;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Result";
            // 
            // grdResult
            // 
            this.grdResult.CaptionVisible = false;
            this.grdResult.DataMember = "";
            this.grdResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdResult.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdResult.Location = new System.Drawing.Point(3, 16);
            this.grdResult.Name = "grdResult";
            this.grdResult.ReadOnly = true;
            this.grdResult.Size = new System.Drawing.Size(917, 170);
            this.grdResult.TabIndex = 0;
            this.grdResult.DoubleClick += new System.EventHandler(this.grdResult_DoubleClick);
            this.grdResult.CurrentCellChanged += new System.EventHandler(this.grdResult_CurrentCellChanged);
            // 
            // DApptSearch
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(923, 501);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlDescription);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DApptSearch";
            this.Text = "Find Clinic Availability";
            this.panel1.ResumeLayout(false);
            this.pnlDescription.ResumeLayout(false);
            this.grpDescription.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grpDayOfWeek.ResumeLayout(false);
            this.grpTimeOfDay.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        #region Event Handlers
        private void cmdOK_Click(object sender, System.EventArgs e)
		{

		}

		private void cmdSearch_Click(object sender, System.EventArgs e)
		{
			//Get the control data into local vars
			UpdateDialogData(false);
			//Resource array, Begin date, Access type array, MTWTF , AM PM
			//Assemble |-delimited resource string
			string sResources = "";
			for (int j=0; j < m_alResources.Count; j++)
			{
				sResources = sResources + m_alResources[j];
				if (j < (m_alResources.Count - 1))
					sResources = sResources + "|";
			}

			//Access Types Array
			string sTypes = "";
			if (m_alAccessTypes.Count > 0) 
			{
				for (int j=0; j < m_alAccessTypes.Count; j++)
				{
					sTypes = sTypes + (string) m_alAccessTypes[j];
					if (j < (m_alAccessTypes.Count-1))
						sTypes = sTypes + "|";
				}
			}

			string sSearchInfo = "1|" + m_sAmpm + "|" + m_sWeekDays;
			m_dtResult = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_alResources, m_dStart, m_dEnd, m_alAccessTypes, ScheduleType.Resource, sSearchInfo);

			if (m_dtResult.Rows.Count == 0)
			{
				MessageBox.Show("No availability found.");
				return;
			}

			m_dtResult.TableName = "Result";
			m_dtResult.Columns.Add("AMPM", System.Type.GetType("System.String") ,"Convert(START_TIME, 'System.String')" );
			m_dtResult.Columns.Add("DAYOFWEEK", System.Type.GetType("System.String"));
			m_dtResult.Columns.Add("ACCESSNAME", System.Type.GetType("System.String"));

			DataRow drAT;
			DateTime dt;
			string sDOW;
			int nAccessTypeID;
			string sAccessType;

			foreach (DataRow dr in m_dtResult.Rows)
			{
				dt = (DateTime) dr["START_TIME"];
				sDOW = dt.DayOfWeek.ToString();
				dr["DAYOFWEEK"] = sDOW;
				if (dr["ACCESS_TYPE"].ToString() != "") 
				{
					nAccessTypeID =Convert.ToInt16(dr["ACCESS_TYPE"].ToString());
					drAT = m_dsGlobal.Tables["AccessTypes"].Rows.Find(nAccessTypeID);
					if (drAT != null)
					{
						sAccessType = drAT["ACCESS_TYPE_NAME"].ToString();
						dr["ACCESSNAME"] = sAccessType;
					}
				}
			}


			m_dvResult = new DataView(m_dtResult);

			string sFilter = "(SLOTS > 0)";
			if (m_sAmpm != "")
			{
				if (m_sAmpm == "AM")
					sFilter = sFilter + " AND (AMPM LIKE '*AM*')";
				if (m_sAmpm == "PM")
					sFilter = sFilter + " AND (AMPM LIKE '*PM*')";
			}

			bool sOr = false;
			if (m_sWeekDays != "")
			{
				sFilter += " AND (";
				if (chkMon.Checked == true)
				{
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Monday*')";
					sOr = true;
				}
				if (chkTue.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Tuesday*')";
					sOr = true;
				}
				if (chkWed.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Wednesday*')";
					sOr = true;
				}
				if (chkThu.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Thursday*')";
					sOr = true;
				}
				if (chkFri.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Friday*')";
					sOr = true;
				}
				if (chkSat.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Saturday*')";
					sOr = true;
				}
				if (chkSun.Checked == true)
				{
					sFilter = (sOr == true)?sFilter + " OR ":sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Sunday*')";
					sOr = true;
				}
				sFilter += ")";
			}

			if (m_alAccessTypes.Count > 0) 
			{
				sFilter += " AND (";
				sOr = false;
				foreach (string sType in m_alAccessTypes)
				{
					if (sOr == true)
						sFilter += " OR ";
					sOr = true;
					sFilter += "(ACCESSNAME = '" + sType + "')";
				}
				sFilter += ")";
			}	

			m_dvResult.RowFilter = sFilter;
			this.grdResult.DataSource = m_dvResult;

		}

		private void cboAccessTypeFilter_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			//Load Access Types listbox & filter
			string sGroup = cboAccessTypeFilter.Text;
			if (sGroup == "<Show All Access Types>")
			{
				LoadListBox("ALL");
			}
			else 
			{
				LoadListBox("SELECTED");
			}			
				
		}

		private void grdResult_DoubleClick(object sender, System.EventArgs e)
		{
			if (grdResult.DataSource == null)
				return;

			DataGridCell dgCell;
			dgCell = this.grdResult.CurrentCell;
			dgCell.ColumnNumber = 2;
			this.m_sSelectedResource = grdResult[dgCell.RowNumber, dgCell.ColumnNumber].ToString();
			this.m_sSelectedDate = (DateTime) grdResult[dgCell.RowNumber,0];
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void grdResult_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataGridCell dgCell;
			dgCell = this.grdResult.CurrentCell;
			this.grdResult.Select(dgCell.RowNumber);

        }

        #endregion  Event Handlers

        #region Properties
        /// <summary>
		/// Gets the resource selected by the user
		/// </summary>
		public string SelectedResource
		{
			get
			{
				return this.m_sSelectedResource;
			}
		}

		/// <summary>
		/// Gets the date selected by the user
		/// </summary>
		public DateTime SelectedDate
		{
			get
			{
				return this.m_sSelectedDate;
			}
		}
		#endregion Properties

	}
}
