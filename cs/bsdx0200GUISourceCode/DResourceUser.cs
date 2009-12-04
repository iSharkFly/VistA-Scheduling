using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using IndianHealthService.BMXNet;
using System.Diagnostics;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DResourceUser.
	/// </summary>
	public class DResourceUser : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescriptionResourceGroup;
		private System.Windows.Forms.Label lblDescriptionResourceGroup;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.ComboBox cboScheduleUser;
		private System.Windows.Forms.CheckBox chkModifySchedule;
		private System.Windows.Forms.CheckBox chkOverbook;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.CheckBox chkAppointments;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DResourceUser()
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
			this.cboScheduleUser = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlPageBottom = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpDescriptionResourceGroup = new System.Windows.Forms.GroupBox();
			this.lblDescriptionResourceGroup = new System.Windows.Forms.Label();
			this.chkModifySchedule = new System.Windows.Forms.CheckBox();
			this.chkOverbook = new System.Windows.Forms.CheckBox();
			this.chkAppointments = new System.Windows.Forms.CheckBox();
			this.pnlPageBottom.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescriptionResourceGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// cboScheduleUser
			// 
			this.cboScheduleUser.Location = new System.Drawing.Point(144, 32);
			this.cboScheduleUser.Name = "cboScheduleUser";
			this.cboScheduleUser.Size = new System.Drawing.Size(248, 21);
			this.cboScheduleUser.TabIndex = 0;
			this.cboScheduleUser.Text = "cboScheduleUser";
			this.cboScheduleUser.SelectedIndexChanged += new System.EventHandler(this.cboScheduleUser_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select Resource User:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pnlPageBottom
			// 
			this.pnlPageBottom.Controls.Add(this.cmdCancel);
			this.pnlPageBottom.Controls.Add(this.cmdOK);
			this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlPageBottom.Location = new System.Drawing.Point(0, 254);
			this.pnlPageBottom.Name = "pnlPageBottom";
			this.pnlPageBottom.Size = new System.Drawing.Size(448, 40);
			this.pnlPageBottom.TabIndex = 4;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(376, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(56, 24);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(296, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpDescriptionResourceGroup);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDescription.Location = new System.Drawing.Point(0, 182);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(448, 72);
			this.pnlDescription.TabIndex = 5;
			// 
			// grpDescriptionResourceGroup
			// 
			this.grpDescriptionResourceGroup.Controls.Add(this.lblDescriptionResourceGroup);
			this.grpDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescriptionResourceGroup.Location = new System.Drawing.Point(0, 0);
			this.grpDescriptionResourceGroup.Name = "grpDescriptionResourceGroup";
			this.grpDescriptionResourceGroup.Size = new System.Drawing.Size(448, 72);
			this.grpDescriptionResourceGroup.TabIndex = 1;
			this.grpDescriptionResourceGroup.TabStop = false;
			this.grpDescriptionResourceGroup.Text = "Description";
			// 
			// lblDescriptionResourceGroup
			// 
			this.lblDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescriptionResourceGroup.Location = new System.Drawing.Point(3, 16);
			this.lblDescriptionResourceGroup.Name = "lblDescriptionResourceGroup";
			this.lblDescriptionResourceGroup.Size = new System.Drawing.Size(442, 53);
			this.lblDescriptionResourceGroup.TabIndex = 0;
			this.lblDescriptionResourceGroup.Text = "Use this panel to assign user access to Resources.  Only users who have the BSDXZ" +
				"MENU key in VistA will appear in the selection list.  If Modify Schedule is check" +
				"ed, then the user will be able to add and edit the resource\'s availability.";
			// 
			// chkModifySchedule
			// 
			this.chkModifySchedule.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkModifySchedule.Location = new System.Drawing.Point(96, 136);
			this.chkModifySchedule.Name = "chkModifySchedule";
			this.chkModifySchedule.Size = new System.Drawing.Size(152, 16);
			this.chkModifySchedule.TabIndex = 7;
			this.chkModifySchedule.Text = "Modify Clinic Availability:";
			this.chkModifySchedule.CheckedChanged += new System.EventHandler(this.chkModifySchedule_CheckedChanged);
			// 
			// chkOverbook
			// 
			this.chkOverbook.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkOverbook.Location = new System.Drawing.Point(160, 104);
			this.chkOverbook.Name = "chkOverbook";
			this.chkOverbook.Size = new System.Drawing.Size(88, 16);
			this.chkOverbook.TabIndex = 8;
			this.chkOverbook.Text = "Overbook:";
			this.chkOverbook.CheckedChanged += new System.EventHandler(this.chkOverbook_CheckedChanged);
			// 
			// chkAppointments
			// 
			this.chkAppointments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkAppointments.Location = new System.Drawing.Point(40, 72);
			this.chkAppointments.Name = "chkAppointments";
			this.chkAppointments.Size = new System.Drawing.Size(208, 16);
			this.chkAppointments.TabIndex = 9;
			this.chkAppointments.Text = "Add, Edit and Delete appointments:";
			this.chkAppointments.CheckedChanged += new System.EventHandler(this.chkAppointments_CheckedChanged);
			// 
			// DResourceUser
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(448, 294);
			this.Controls.Add(this.chkAppointments);
			this.Controls.Add(this.chkOverbook);
			this.Controls.Add(this.chkModifySchedule);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlPageBottom);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboScheduleUser);
			this.Name = "DResourceUser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DResourceUser";
			this.pnlPageBottom.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescriptionResourceGroup.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private DataTable	m_dtResourceUser;
		private int			m_nUserID;
		private bool		m_bModifySchedule;
		private bool		m_bOverbook;
		private bool		m_bAppointments;

		#endregion Fields

		#region Properties

		/// <summary>
		/// The ID of the Resource User in the NEW PERSON file.
		/// </summary>
		public int UserID
		{
			get
			{
				return m_nUserID;
			}
		}

		/// <summary>
		/// True if the user is allowed to modify the resource's scheduled availability
		/// </summary>
		public bool ModifySchedule
		{
			get
			{
				return m_bModifySchedule;
			}
		}

		/// <summary>
		/// True if the user is allowed to overbook beyond the resource's scheduled availability
		/// </summary>
		public bool Overbook
		{
			get
			{
				return m_bOverbook;
			}
		}

		/// <summary>
		/// True if the user is allowed to create, edit and delete appointments
		/// </summary>
		public bool Appoinmtments
		{
			get
			{
				return m_bAppointments;
			}
		}		
		#endregion Properties


		public void InitializePage(int nSelectedRUID, DataSet dsGlobal)
		{

			m_dtResourceUser = dsGlobal.Tables["ResourceUser"];

			//Datasource the SCHEDULE USER combo box
			DataTable dtSchedUser = dsGlobal.Tables["ScheduleUser"];
			DataView dvSchedUser = new DataView(dtSchedUser);
			dvSchedUser.Sort = "USERNAME ASC";


			cboScheduleUser.DataSource = dvSchedUser;
			cboScheduleUser.DisplayMember = "USERNAME";
			cboScheduleUser.ValueMember = "USERID";

			if (nSelectedRUID < 0) //then we're in ADD mode
			{
				this.Text = "Add New Resource User";
				m_nUserID = 0;
				m_bModifySchedule = false;
				m_bOverbook = false;
				m_bAppointments = false;
				this.cmdOK.Enabled = false;
			}
			else //we're in EDIT mode
			{
				this.Text = "Edit Scheduling Resource";
				this.cboScheduleUser.Enabled = false;
				DataRow dr = m_dtResourceUser.Rows.Find(nSelectedRUID);	
				m_nUserID = Convert.ToInt16(dr["USERID"]);//CHANGED FROM USERID1
				string sOverbook = dr["OVERBOOK"].ToString();
				m_bOverbook = (sOverbook == "YES")?true:false;
				string sModify = dr["MODIFY_SCHEDULE"].ToString();
				m_bModifySchedule = (sModify == "YES")?true:false;
				string sAppts = dr["MODIFY_APPOINTMENTS"].ToString();
				m_bAppointments = (sAppts == "YES")?true:false;

			}
			UpdateDialogData(true);
		}

		/// <summary>
		/// If b is true, moves member vars into control data
		/// otherwise, moves control data into member vars
		/// </summary>
		/// <param name="b"></param>
		private void UpdateDialogData(bool b)
		{
			if (b == true)
			{
				this.chkOverbook.Checked = m_bOverbook;
				this.chkModifySchedule.Checked = m_bModifySchedule;
				this.cboScheduleUser.SelectedValue = m_nUserID;
				this.chkAppointments.Checked = m_bAppointments;
			}
			else
			{
				m_bOverbook = this.chkOverbook.Checked;
				m_bModifySchedule = this.chkModifySchedule.Checked;
				m_bAppointments = this.chkAppointments.Checked;
				m_nUserID = Convert.ToInt16(this.cboScheduleUser.SelectedValue);
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			this.UpdateDialogData(false);
		}

		private void cboScheduleUser_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.cmdOK.Enabled = true;
		}

		private void chkModifySchedule_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkModifySchedule.Checked == true)
			{
				this.chkAppointments.Checked = true;
				this.chkOverbook.Checked = true;
			}
		}

		private void chkOverbook_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkOverbook.Checked == true)
			{
				chkAppointments.Checked = true;
			}
			if (chkOverbook.Checked == false)
			{
				chkModifySchedule.Checked = false;
			}
		}

		private void chkAppointments_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkAppointments.Checked == false)
			{
				chkOverbook.Checked = false;
				chkModifySchedule.Checked = false;
			}
		}

	}
}
