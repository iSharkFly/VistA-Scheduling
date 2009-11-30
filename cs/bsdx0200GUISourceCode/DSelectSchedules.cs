using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DSelectSchedules.
	/// </summary>
	public class DSelectSchedules : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlOkCancel;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckedListBox lstResource;
		private System.Windows.Forms.CheckBox chkOneWindow;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboResourceGroup;
		private System.Windows.Forms.TextBox txtGroupWindow;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DSelectSchedules()
		{
			//
			// Required for Windows Form Designer support
			//
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
			this.label2 = new System.Windows.Forms.Label();
			this.lstResource = new System.Windows.Forms.CheckedListBox();
			this.chkOneWindow = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cboResourceGroup = new System.Windows.Forms.ComboBox();
			this.txtGroupWindow = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
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
			this.pnlOkCancel.Location = new System.Drawing.Point(0, 478);
			this.pnlOkCancel.Name = "pnlOkCancel";
			this.pnlOkCancel.Size = new System.Drawing.Size(512, 40);
			this.pnlOkCancel.TabIndex = 5;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(416, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 30;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(336, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 25;
			this.cmdOK.Text = "OK";
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpDescription);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDescription.Location = new System.Drawing.Point(0, 398);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(512, 80);
			this.pnlDescription.TabIndex = 48;
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
			this.lblDescription.Text = "Use this panel to open a group of resource schedules.  You can open each schedule" +
				" in a separate window or open all schedules in one single group window.  If you " +
				"use a group window, you can assign a name to identify the window.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(240, 16);
			this.label2.TabIndex = 53;
			this.label2.Text = "Resource:";
			// 
			// lstResource
			// 
			this.lstResource.HorizontalScrollbar = true;
			this.lstResource.Location = new System.Drawing.Point(16, 104);
			this.lstResource.MultiColumn = true;
			this.lstResource.Name = "lstResource";
			this.lstResource.Size = new System.Drawing.Size(448, 184);
			this.lstResource.TabIndex = 10;
			// 
			// chkOneWindow
			// 
			this.chkOneWindow.Checked = true;
			this.chkOneWindow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOneWindow.Location = new System.Drawing.Point(24, 304);
			this.chkOneWindow.Name = "chkOneWindow";
			this.chkOneWindow.Size = new System.Drawing.Size(328, 24);
			this.chkOneWindow.TabIndex = 15;
			this.chkOneWindow.Text = "Open Schedules in a Single Group Window";
			this.chkOneWindow.CheckedChanged += new System.EventHandler(this.chkOneWindow_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 16);
			this.label1.TabIndex = 58;
			this.label1.Text = "Resource Group:";
			// 
			// cboResourceGroup
			// 
			this.cboResourceGroup.Location = new System.Drawing.Point(16, 40);
			this.cboResourceGroup.Name = "cboResourceGroup";
			this.cboResourceGroup.Size = new System.Drawing.Size(448, 21);
			this.cboResourceGroup.TabIndex = 5;
			this.cboResourceGroup.SelectionChangeCommitted += new System.EventHandler(this.cboResourceGroup_SelectionChangeCommitted);
			// 
			// txtGroupWindow
			// 
			this.txtGroupWindow.Location = new System.Drawing.Point(160, 344);
			this.txtGroupWindow.Name = "txtGroupWindow";
			this.txtGroupWindow.Size = new System.Drawing.Size(304, 20);
			this.txtGroupWindow.TabIndex = 20;
			this.txtGroupWindow.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 344);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 16);
			this.label3.TabIndex = 58;
			this.label3.Text = "Group Window Name:";
			// 
			// DSelectSchedules
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(512, 518);
			this.Controls.Add(this.txtGroupWindow);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboResourceGroup);
			this.Controls.Add(this.chkOneWindow);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lstResource);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlOkCancel);
			this.Controls.Add(this.label3);
			this.Name = "DSelectSchedules";
			this.Text = "Open Selected Schedules";
			this.pnlOkCancel.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescription.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Fields
		private DataTable		m_dtGroups;
		private DataView		m_dvGroups;
		private DataTable		m_dtResources;
		private DataView		m_dvResources;
		private DataSet			m_dsGlobal;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Returns the an ArrayList of selected resource names
		/// </summary>
		public ArrayList SelectedClinics
		{

			get
			{
				System.Collections.ArrayList al = new System.Collections.ArrayList();
				foreach(DataRowView s in this.lstResource.CheckedItems)
				{
					al.Add(s["RESOURCE_NAME"].ToString());
				}
				return al;
			}
		}

		public bool SingleWindow
		{
			get
			{
				return this.chkOneWindow.Checked;
			}
		}

		public string GroupWindowName
		{
			get
			{
				return this.txtGroupWindow.Text;
			}
		}
		#endregion Properties


		public void InitializePage(DataSet dsGlobal, string sWindowText)
		{
			try
			{
				m_dsGlobal = dsGlobal;
				this.Text = sWindowText;
				m_dtResources = dsGlobal.Tables["Resources"];
				m_dvResources = new DataView(m_dtResources);
				m_dvResources.Sort = "RESOURCE_NAME ASC";
				m_dvResources.RowFilter = "INACTIVE <> 1 AND VIEW = 1";
				lstResource.DataSource = m_dvResources;
				lstResource.DisplayMember = "RESOURCE_NAME";
				lstResource.ValueMember = "RESOURCE_NAME";	
				
				m_dtGroups = dsGlobal.Tables["ResourceGroup"].Copy();
				m_dvGroups = new DataView(m_dtGroups);
				m_dvGroups.Sort = "RESOURCE_GROUP ASC";
				this.cboResourceGroup.Items.Add("<Show All Resources & Clinics>");
				foreach (DataRowView drvG in m_dvGroups)
				{
					this.cboResourceGroup.Items.Add(drvG["RESOURCE_GROUP"]);
				}
				this.cboResourceGroup.Text = "<Show All Resources & Clinics>";
				return;
			}
			catch(Exception ex)
			{
				throw ex;
			}

		}

		private void cboResourceGroup_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			string sGroup = cboResourceGroup.SelectedItem.ToString();
			if (sGroup == "<Show All Resources & Clinics>")
			{
				LoadListBox("ALL");
			}
			else 
			{
				LoadListBox("SELECTED");
			}			
				
		}

		private void LoadListBox(string sGroup)
		{
			if (sGroup == "ALL")
			{
				//Load the Resources list box with ALL resources
				m_dtResources = m_dsGlobal.Tables["Resources"];
				m_dvResources = new DataView(m_dtResources);
				m_dvResources = new DataView(m_dtResources);
				m_dvResources.Sort = "RESOURCE_NAME ASC";
				m_dvResources.RowFilter = "INACTIVE <> 1 AND VIEW = 1";
				lstResource.DataSource = m_dvResources;
				lstResource.DisplayMember = "RESOURCE_NAME";
				lstResource.ValueMember = "RESOURCE_NAME";	
			}
			else 
			{
				//Load the Resources list box with active resources belonging
				//to group sGroup

				//Build Resource Group table containing *active* Resources and their Groups
				m_dtResources = m_dsGlobal.Tables["GroupResources"];
				//Create a view that is filterable on ResourceGroup
				m_dvResources = new DataView(m_dtResources);
				m_dvResources.RowFilter = "RESOURCE_GROUP = '" + this.cboResourceGroup.SelectedItem.ToString() + "'";
				lstResource.DataSource = m_dvResources;
				lstResource.DisplayMember = "RESOURCE_NAME";
				lstResource.ValueMember = "RESOURCE_NAME";
			}
		}

		private void chkOneWindow_CheckedChanged(object sender, System.EventArgs e)
		{
			this.txtGroupWindow.Enabled = this.chkOneWindow.Checked;
		}

	}
}
