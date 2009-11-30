using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DSelectLetterClinics.
	/// </summary>
	public class DSelectLetterClinics : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlOkCancel;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.CheckedListBox lstResource;
		private System.Windows.Forms.ComboBox cboResourceGroup;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dtBegin;
		private System.Windows.Forms.DateTimePicker dtEnd;
		private System.Windows.Forms.Label lblRange;
		private System.Windows.Forms.CheckBox chkSelectAll;
		private System.ComponentModel.Container components = null;

		public DSelectLetterClinics()
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
			this.pnlOkCancel = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpDescription = new System.Windows.Forms.GroupBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lstResource = new System.Windows.Forms.CheckedListBox();
			this.cboResourceGroup = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.dtBegin = new System.Windows.Forms.DateTimePicker();
			this.dtEnd = new System.Windows.Forms.DateTimePicker();
			this.lblRange = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.chkSelectAll = new System.Windows.Forms.CheckBox();
			this.pnlOkCancel.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescription.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlOkCancel
			// 
			this.pnlOkCancel.Controls.Add(this.cmdCancel);
			this.pnlOkCancel.Controls.Add(this.cmdOK);
			this.pnlOkCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlOkCancel.Location = new System.Drawing.Point(0, 430);
			this.pnlOkCancel.Name = "pnlOkCancel";
			this.pnlOkCancel.Size = new System.Drawing.Size(512, 40);
			this.pnlOkCancel.TabIndex = 4;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(416, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(336, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "OK";
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpDescription);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDescription.Location = new System.Drawing.Point(0, 350);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(512, 80);
			this.pnlDescription.TabIndex = 47;
			// 
			// grpDescription
			// 
			this.grpDescription.Controls.Add(this.lblDescription);
			this.grpDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescription.Location = new System.Drawing.Point(0, 0);
			this.grpDescription.Name = "grpDescription";
			this.grpDescription.Size = new System.Drawing.Size(512, 80);
			this.grpDescription.TabIndex = 0;
			this.grpDescription.TabStop = false;
			this.grpDescription.Text = "Description";
			// 
			// lblDescription
			// 
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescription.Location = new System.Drawing.Point(3, 16);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(506, 61);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "Use this panel to select resources and to specify the time period for patient rem" +
				"inder letters.  Check each resource (clinic) for which to print letters.  Letter" +
				"s will be printed for appointments between the beginning and ending dates, inclu" +
				"sive.";
			// 
			// lstResource
			// 
			this.lstResource.HorizontalScrollbar = true;
			this.lstResource.Location = new System.Drawing.Point(24, 96);
			this.lstResource.MultiColumn = true;
			this.lstResource.Name = "lstResource";
			this.lstResource.Size = new System.Drawing.Size(448, 124);
			this.lstResource.TabIndex = 48;
			// 
			// cboResourceGroup
			// 
			this.cboResourceGroup.Location = new System.Drawing.Point(24, 40);
			this.cboResourceGroup.Name = "cboResourceGroup";
			this.cboResourceGroup.Size = new System.Drawing.Size(448, 21);
			this.cboResourceGroup.TabIndex = 49;
			this.cboResourceGroup.SelectedIndexChanged += new System.EventHandler(this.cboResourceGroup_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 16);
			this.label1.TabIndex = 50;
			this.label1.Text = "Resource Group:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 51;
			this.label2.Text = "Resource:";
			// 
			// dtBegin
			// 
			this.dtBegin.Location = new System.Drawing.Point(24, 312);
			this.dtBegin.Name = "dtBegin";
			this.dtBegin.TabIndex = 52;
			// 
			// dtEnd
			// 
			this.dtEnd.Location = new System.Drawing.Point(280, 312);
			this.dtEnd.Name = "dtEnd";
			this.dtEnd.Size = new System.Drawing.Size(192, 20);
			this.dtEnd.TabIndex = 52;
			// 
			// lblRange
			// 
			this.lblRange.Location = new System.Drawing.Point(24, 272);
			this.lblRange.Name = "lblRange";
			this.lblRange.Size = new System.Drawing.Size(192, 16);
			this.lblRange.TabIndex = 53;
			this.lblRange.Text = "Date range for appointment letters:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 296);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(152, 16);
			this.label4.TabIndex = 54;
			this.label4.Text = "Beginning Date:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(280, 296);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(152, 16);
			this.label5.TabIndex = 54;
			this.label5.Text = "Ending  Date:";
			// 
			// chkSelectAll
			// 
			this.chkSelectAll.Location = new System.Drawing.Point(32, 232);
			this.chkSelectAll.Name = "chkSelectAll";
			this.chkSelectAll.Size = new System.Drawing.Size(168, 24);
			this.chkSelectAll.TabIndex = 55;
			this.chkSelectAll.Text = "Select All Resources";
			this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
			// 
			// DSelectLetterClinics
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(512, 470);
			this.Controls.Add(this.chkSelectAll);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblRange);
			this.Controls.Add(this.dtBegin);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboResourceGroup);
			this.Controls.Add(this.lstResource);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlOkCancel);
			this.Controls.Add(this.dtEnd);
			this.Controls.Add(this.label5);
			this.Name = "DSelectLetterClinics";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Print Apppointment Letters";
			this.pnlOkCancel.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescription.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Methods

		public void SetupForReports()
		{
			lblRange.Text = "Date Range for Appointment List:";
			lblDescription.Text = "Use this panel to select resources and to specify the time period for appointment lists.  Check each resource (clinic) for which to print lists.  Lists will be printed for appointments between the beginning and ending dates, inclusive.";
			this.Text = "Print Clinic Schedules";
		}

		public void InitializePage(DataSet dsGlobal, string sWindowText)
		{
			try
			{
				m_bInitialized = false;
				this.Text = sWindowText;
				m_dtResources = dsGlobal.Tables["GroupResources"];
				m_dvResources = new DataView(m_dtResources);
				m_dvResources.Sort = "RESOURCE_NAME ASC";
				lstResource.DataSource = m_dvResources;
				lstResource.DisplayMember = "RESOURCE_NAME";
				lstResource.ValueMember = "RESOURCEID";	
				
				m_dtGroups = dsGlobal.Tables["ResourceGroup"];
				m_dvGroups = new DataView(m_dtGroups);
				m_dvGroups.Sort = "RESOURCE_GROUP ASC";
				cboResourceGroup.DataSource = m_dvGroups;
				cboResourceGroup.DisplayMember = "RESOURCE_GROUP";
				cboResourceGroup.ValueMember = "RESOURCE_GROUPID";

				m_dvResources.RowFilter = "RESOURCE_GROUPID = " + cboResourceGroup.SelectedValue;
				m_bInitialized = true;
				return;
			}
			catch(Exception ex)
			{
				throw ex;
			}

		}

		private void cboResourceGroup_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_bInitialized == true)
				m_dvResources.RowFilter = "RESOURCE_GROUPID = " + cboResourceGroup.SelectedValue;
			chkSelectAll.Checked = false;
		}

		private void chkSelectAll_CheckedChanged(object sender, System.EventArgs e)
		{
			for(int i=0; i < lstResource.Items.Count; i++)
			{
				lstResource.SetItemChecked(i, chkSelectAll.Checked);
			}
		}

		#endregion Methods

		#region Fields
		private DataTable		m_dtGroups;
		private DataView		m_dvGroups;
		private DataTable		m_dtResources;
		private DataView		m_dvResources;
		private bool			m_bInitialized;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Returns the |-delimited string of selected resource id's
		/// </summary>
		public string SelectedClinics
		{
			get
			{
				string sRet = "";
				foreach(DataRowView s in this.lstResource.CheckedItems)
				{
					sRet = sRet + s["RESOURCEID"].ToString() + "|";
				}
				return sRet;
			}
		}

		public DateTime BeginDate
		{
			get
			{
				return dtBegin.Value;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return dtEnd.Value;
			}
		}
		#endregion Properties


	}
}
