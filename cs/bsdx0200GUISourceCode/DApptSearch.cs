using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
//using System.Data.OleDb;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Modal Dialog for searching for Patient Slots
	/// </summary>
	public class DApptSearch : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button btnAccept;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdSearch;
        private ListView lstResults;
        private ColumnHeader colStartTime;
        private ColumnHeader colEndTime;
        private ColumnHeader colResource;
        private ColumnHeader colSlots;
        private ColumnHeader colAccessType;
        private ColumnHeader colDate;
        private Label lblEnd;
        private Label lblStart;
        private DateTimePicker dtEnd;
        private DateTimePicker dtStart;
        private ColumnHeader colDOW;
        private ColumnHeader colID;
        private Label lblMessage;
      
        private System.ComponentModel.IContainer components;

		public DApptSearch()
		{
			InitializeComponent();
		}

		#region Fields

		private CGDocumentManager	m_DocManager;
		
		private DataSet	m_dsGlobal;
		DataTable		m_dtTypes;
		DataView		m_dvTypes;
        List<CGAvailability> lstResultantAvailabilities;
        private CGAvailability _selectedAvailability;
		DateTime		m_dStart;
		DateTime		m_dEnd;
		ArrayList		m_alResources;
		ArrayList		m_alAccessTypes;
		string			m_sWeekDays; //only practical use now is for sending to server
		string			m_sAmpm; // same here.

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

            this.Text = "Searching for Appointments in: " + string.Join(" | ", alResources.Cast<string>());
            
            this.m_DocManager = docManager;
			this.m_dsGlobal = m_DocManager.GlobalDataSet;
			
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


            /* OLD CODE 
            //Create DataGridTableStyle for Result grid
			DataGridTableStyle tsResult = new DataGridTableStyle();
			tsResult.MappingName = "Result";
			tsResult.ReadOnly = true;

			// Add START_TIME column style.
			DataGridTextBoxColumn colStartTime = new DataGridTextBoxColumn();
            colStartTime.MappingName = "StartTime";
			colStartTime.HeaderText = "Start Time";
			colStartTime.Width = 200;
			colStartTime.Format = "f";
			tsResult.GridColumnStyles.Add(colStartTime);
			
			// Add END_TIME column style.
			DataGridTextBoxColumn colEndTime = new DataGridTextBoxColumn();
            colEndTime.MappingName = "EndTime";
			colEndTime.HeaderText = "End Time";
			colEndTime.Width = 75;
			colEndTime.Format = "h:mm tt";
			tsResult.GridColumnStyles.Add(colEndTime);

			// Add RESOURCE column style.
			DataGridTextBoxColumn colResource = new DataGridTextBoxColumn();
            colResource.MappingName = "ResourceList";
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
			//grdResult.TableStyles.Add(tsResult);
            */
            
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
                this.m_dStart = this.dtStart.Value;

				//End
				this.m_dEnd = this.dtEnd.Value;
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.grpDescription = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstResults = new System.Windows.Forms.ListView();
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDOW = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStartTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEndTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colResource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSlots = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAccessType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMessage = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnlDescription.SuspendLayout();
            this.grpDescription.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpDayOfWeek.SuspendLayout();
            this.grpTimeOfDay.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.cmdSearch);
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.btnAccept);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 461);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(923, 40);
            this.panel1.TabIndex = 4;
            // 
            // cmdSearch
            // 
            this.cmdSearch.Location = new System.Drawing.Point(33, 6);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(72, 24);
            this.cmdSearch.TabIndex = 2;
            this.cmdSearch.Text = "Search";
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(828, 8);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(64, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(135, 8);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(176, 24);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Select Slot for Appointment";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
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
            this.groupBox1.Controls.Add(this.lblEnd);
            this.groupBox1.Controls.Add(this.lblStart);
            this.groupBox1.Controls.Add(this.dtEnd);
            this.groupBox1.Controls.Add(this.dtStart);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lstAccessTypes);
            this.groupBox1.Controls.Add(this.cboAccessTypeFilter);
            this.groupBox1.Controls.Add(this.grpDayOfWeek);
            this.groupBox1.Controls.Add(this.grpTimeOfDay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(923, 208);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Parameters";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(12, 124);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(153, 13);
            this.lblEnd.TabIndex = 67;
            this.lblEnd.Text = "End Date to Search (Inclusive)";
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(12, 35);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(159, 13);
            this.lblStart.TabIndex = 66;
            this.lblStart.Text = "Start Date to Search (Inclusive)";
            // 
            // dtEnd
            // 
            this.dtEnd.Location = new System.Drawing.Point(12, 141);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(200, 20);
            this.dtEnd.TabIndex = 65;
            this.dtEnd.ValueChanged += new System.EventHandler(this.dtEnd_ValueChanged);
            // 
            // dtStart
            // 
            this.dtStart.Location = new System.Drawing.Point(12, 54);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(200, 20);
            this.dtStart.TabIndex = 64;
            this.dtStart.ValueChanged += new System.EventHandler(this.dtStart_ValueChanged);
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
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 57;
            this.label1.Text = "Date Range:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstResults);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 208);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(923, 189);
            this.groupBox2.TabIndex = 57;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Result";
            // 
            // lstResults
            // 
            this.lstResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colDate,
            this.colDOW,
            this.colStartTime,
            this.colEndTime,
            this.colResource,
            this.colSlots,
            this.colAccessType});
            this.lstResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResults.FullRowSelect = true;
            this.lstResults.GridLines = true;
            this.lstResults.Location = new System.Drawing.Point(3, 16);
            this.lstResults.MultiSelect = false;
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(917, 170);
            this.lstResults.TabIndex = 0;
            this.lstResults.UseCompatibleStateImageBehavior = false;
            this.lstResults.View = System.Windows.Forms.View.Details;
            this.lstResults.DoubleClick += new System.EventHandler(this.lstResults_DoubleClick);
            // 
            // colID
            // 
            this.colID.Text = "ID";
            this.colID.Width = 0;
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.Width = 91;
            // 
            // colDOW
            // 
            this.colDOW.Text = "Day of Week";
            this.colDOW.Width = 80;
            // 
            // colStartTime
            // 
            this.colStartTime.Text = "Start Time";
            this.colStartTime.Width = 87;
            // 
            // colEndTime
            // 
            this.colEndTime.Text = "End Time";
            this.colEndTime.Width = 116;
            // 
            // colResource
            // 
            this.colResource.Text = "Resource";
            this.colResource.Width = 370;
            // 
            // colSlots
            // 
            this.colSlots.Text = "Slots";
            this.colSlots.Width = 47;
            // 
            // colAccessType
            // 
            this.colAccessType.Text = "Access Type";
            this.colAccessType.Width = 101;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(337, 16);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 16);
            this.lblMessage.TabIndex = 3;
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
            this.panel1.PerformLayout();
            this.pnlDescription.ResumeLayout(false);
            this.grpDescription.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpDayOfWeek.ResumeLayout(false);
            this.grpTimeOfDay.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        #region Event Handlers

		private void cmdSearch_Click(object sender, System.EventArgs e)
		{
			//Tell user we are processing
            this.Cursor = Cursors.WaitCursor;
            this.lblMessage.Text = String.Empty;

            //Get the control data into local vars
			UpdateDialogData(false);
			//Resource array, Begin date, Access type array, MTWTF , AM PM

            //Get Availabilities and Appointments from the DB
            //NB: m_sAmpm and m_sWeekDays don't have an effect on the M side side right now
			string sSearchInfo = "1|" + m_sAmpm + "|" + m_sWeekDays;
			DataTable m_availTable = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_alResources, m_dStart, m_dEnd, m_alAccessTypes, ScheduleType.Resource, sSearchInfo);
            DataTable m_apptTable = CGSchedLib.CreateAppointmentSchedule(m_DocManager, m_alResources, m_dStart, m_dEnd);

#if DEBUG            
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif
            lstResultantAvailabilities = (from rowAV in m_availTable.AsEnumerable()
                         
                         // Calculate the number of slots consumed in this availability by appointments
                         let slotsConsumed = (from appt in m_apptTable.AsEnumerable()
                                              //If the resource is the same and the user selection overlaps, then...
                                              where (rowAV.Field<string>("RESOURCE") == appt.Field<string>("RESOURCENAME")
                                                && CalendarGrid.TimesOverlap(rowAV.Field<DateTime>("START_TIME"), rowAV.Field<DateTime>("END_TIME"), appt.Field<DateTime>("START_TIME"), appt.Field<DateTime>("END_TIME")))
                                              // if appt starttime is before avail start time, only count against the avail starting from the availability start time
                                              let startTimeToCountAgainstBlock = appt.Field<DateTime>("START_TIME") < rowAV.Field<DateTime>("START_TIME") ? rowAV.Field<DateTime>("START_TIME") : appt.Field<DateTime>("START_TIME")
                                              // if appt endtime is after the avail ends, only count against the avail up to where the avail ends
                                              let endTimeToCountAgainstBlock = appt.Field<DateTime>("END_TIME") > rowAV.Field<DateTime>("END_TIME") ? rowAV.Field<DateTime>("END_TIME") : appt.Field<DateTime>("END_TIME")
                                              // theoretical minutes per slot for the availability
                                              let minPerSlot = (rowAV.Field<DateTime>("END_TIME") - rowAV.Field<DateTime>("START_TIME")).TotalMinutes / rowAV.Field<int>("SLOTS")
                                              // how many minutes does this appointment take away from the slot
                                              let minPerAppt = (endTimeToCountAgainstBlock - startTimeToCountAgainstBlock).TotalMinutes
                                              // how many slots the appointment takes up using this availability's scale
                                              let slotsConsumed = minPerAppt / minPerSlot
                                              select slotsConsumed).Sum()
                         
                         // Subtract the number consumed from the original ones
                         let slotsLeft = (float)rowAV.Field<int>("SLOTS") - slotsConsumed
                         // filter by that value if it is at least one slot
                         where slotsLeft >= 1
                         // Sort by Start Time, then by Resource Name
                         orderby rowAV.Field<DateTime>("START_TIME"), rowAV.Field<string>("RESOURCE")
                          //return as a CGAvailability
                          select new CGAvailability
                         {
                             ResourceList = rowAV.Field<string>("RESOURCE"),
                             StartTime = rowAV.Field<DateTime>("START_TIME"),
                             EndTime = rowAV.Field<DateTime>("END_TIME"),
                             Slots = (int)slotsLeft,
                             // AccessTypeName is grabbed from the Access Type Table using a psuedojoin syntax.
                             // "ACCESS_TYPE" is the IEN of the AcceesType.
                             // Single or default is b/c we are expecting one result.
                             AccessTypeName = (from at in m_dsGlobal.Tables["AccessTypes"].AsEnumerable() 
                                              where at.Field<int>("BMXIEN")==Int32.Parse(rowAV.Field<string>("ACCESS_TYPE"))
                                              select at.Field<string>("ACCESS_TYPE_NAME")).SingleOrDefault<string>(),
                             AvailabilityType = rowAV.Field<int>("AVAILABILITYID")
                         })
                         // convert to Generic List
                         .ToList<CGAvailability>();

            // if specific access types are chosen, filter the results based on a join against them.
            if (m_alAccessTypes.Count > 0)
                lstResultantAvailabilities = (from av in lstResultantAvailabilities
                                              join at in m_alAccessTypes.Cast<string>() on av.AccessTypeName equals at
                                              select av).ToList<CGAvailability>();

            // if user chose AM radio button, get morning appointments
            // TimeSpan.FromHours(12) gets the number of ticks since Midnight
            if (rdoAM.Checked) // less than 12 pm
            {
                lstResultantAvailabilities = (from av in lstResultantAvailabilities
                                              where av.StartTime.TimeOfDay < TimeSpan.FromHours(12)
                                              select av).ToList<CGAvailability>();
            }
            // if user chose PM radio button, get morning appointments
            if (rdoPM.Checked) // after or equal to 12 pm
            {
                lstResultantAvailabilities = (from av in lstResultantAvailabilities
                                              where av.StartTime.TimeOfDay >= TimeSpan.FromHours(12)
                                              select av).ToList<CGAvailability>();
            }

            // if any of the days of week are checked, create a new list based on them
            // and clear the original list, and join the new lists together
            if (chkMon.Checked || chkTue.Checked || chkWed.Checked || chkThu.Checked || chkFri.Checked || chkSat.Checked || chkSun.Checked)
            {

                var lstMonday = new List<CGAvailability>();
                var lstTuesday = new List<CGAvailability>();
                var lstWednesday = new List<CGAvailability>();
                var lstThursday = new List<CGAvailability>();
                var lstFriday = new List<CGAvailability>();
                var lstSaturday = new List<CGAvailability>();
                var lstSunday = new List<CGAvailability>();

                if (chkMon.Checked == true)
                {
                    lstMonday = (from av in lstResultantAvailabilities
                                 where av.StartTime.DayOfWeek == DayOfWeek.Monday
                                 select av).ToList<CGAvailability>();
                }

                if (chkTue.Checked == true)
                {
                    lstTuesday = (from av in lstResultantAvailabilities
                                  where av.StartTime.DayOfWeek == DayOfWeek.Tuesday
                                  select av).ToList<CGAvailability>();
                }

                if (chkWed.Checked == true)
                {
                    lstWednesday = (from av in lstResultantAvailabilities
                                    where av.StartTime.DayOfWeek == DayOfWeek.Wednesday
                                    select av).ToList<CGAvailability>();

                }

                if (chkThu.Checked == true)
                {
                    lstThursday = (from av in lstResultantAvailabilities
                                   where av.StartTime.DayOfWeek == DayOfWeek.Thursday
                                   select av).ToList<CGAvailability>();

                }

                if (chkFri.Checked == true)
                {
                    lstFriday = (from av in lstResultantAvailabilities
                                 where av.StartTime.DayOfWeek == DayOfWeek.Friday
                                 select av).ToList<CGAvailability>();
                }

                if (chkSat.Checked == true)
                {
                    lstSaturday = (from av in lstResultantAvailabilities
                                   where av.StartTime.DayOfWeek == DayOfWeek.Saturday
                                   select av).ToList<CGAvailability>();

                }

                if (chkSun.Checked == true)
                {
                    lstSunday = (from av in lstResultantAvailabilities
                                 where av.StartTime.DayOfWeek == DayOfWeek.Sunday
                                 select av).ToList<CGAvailability>();

                }


                lstResultantAvailabilities.Clear();
                lstResultantAvailabilities.AddRange(lstMonday);
                lstResultantAvailabilities.AddRange(lstTuesday);
                lstResultantAvailabilities.AddRange(lstWednesday);
                lstResultantAvailabilities.AddRange(lstThursday);
                lstResultantAvailabilities.AddRange(lstFriday);
                lstResultantAvailabilities.AddRange(lstSaturday);
                lstResultantAvailabilities.AddRange(lstSunday);

                lstResultantAvailabilities.OrderBy(av => av.StartTime).ThenBy(av => av.ResourceList);
            }

            

#if DEBUG
            System.Diagnostics.Debug.Write("LINQ took this long: " + stopwatch.ElapsedMilliseconds + "\n");
            stopwatch = null;
#endif
            
            //Then, convert the availabilities to ListViewItems
            var items = (from item in lstResultantAvailabilities
                        let s = new string[] {item.AvailabilityType.ToString(), item.StartTime.ToShortDateString(), item.StartTime.DayOfWeek.ToString(),item.StartTime.ToShortTimeString() ,item.EndTime.ToShortTimeString() ,item.ResourceList,item.Slots.ToString(),item.AccessTypeName}
                        let lvItem = new ListViewItem(s)
                        select lvItem).ToArray<ListViewItem>();

            //--Updating Listview
            lstResults.BeginUpdate(); //tell listview to suspend drawing for now
            lstResults.Items.Clear(); //empty it from old data

            if (items.Length > 0) lstResults.Items.AddRange(items); // add new data
            else this.lblMessage.Text = "No available Appointment Slots Found!";

            lstResults.EndUpdate(); // ok done adding items, draw now.
            //--End Update Listview

            //We are done
            this.Cursor = Cursors.Default;
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


        private void lstResults_DoubleClick(object sender, EventArgs e)
        {
            ProcessChoice(sender, e);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            ProcessChoice(sender, e);
        }

        /// <summary>
        /// Shared method to process a user's choice
        /// </summary>
        /// <param name="s">sender</param>
        /// <param name="e">EventArgs</param>
        private void ProcessChoice(object s, EventArgs e)
        {
            if (lstResults.SelectedIndices.Count == 0)
            {
                this.DialogResult = DialogResult.None;
                lblMessage.Text = "No Appointment Slot selected!";
                return;
            }

            long availabilityKey = long.Parse(lstResults.SelectedItems[0].SubItems[0].Text);
            _selectedAvailability = (from av in lstResultantAvailabilities
                                     where av.AvailabilityType == availabilityKey
                                     select av).Single<CGAvailability>();
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Adjust start date based on end date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtStart_ValueChanged(object sender, EventArgs e)
        {
            if (dtEnd.Value < dtStart.Value) dtEnd.Value = dtStart.Value;
        }

        /// <summary>
        /// Adjust end date based on start date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dtStart.Value > dtEnd.Value) dtStart.Value = dtEnd.Value;
        }

        #endregion  Event Handlers

        #region Properties
        
        /// <summary>
        /// Gets the Availability Selected by the User in which to put an appointment
        /// </summary>
        public CGAvailability SelectedAvailability
        {
            get { return this._selectedAvailability; }
        }

		#endregion Properties


    }
}
