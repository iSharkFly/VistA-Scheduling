using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DCancelAppt.
	/// </summary>
	public class DCancelAppt : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescriptionResourceGroup;
		private System.Windows.Forms.Label lblDescriptionResourceGroup;
		private System.Windows.Forms.GroupBox grpCancelledby;
		private System.Windows.Forms.RadioButton rdoClinic;
		private System.Windows.Forms.RadioButton rdoPatient;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstReason;
		private System.Windows.Forms.TextBox txtNote;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox grpAutoRebook;
		private System.Windows.Forms.CheckBox chkAutoRebook;
		private System.Windows.Forms.NumericUpDown udStart;
		private System.Windows.Forms.NumericUpDown udMax;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton rdoRebookSameType;
		private System.Windows.Forms.RadioButton rdoRebookAnyType;
		private System.Windows.Forms.RadioButton rdoRebookSelectedType;
		private System.Windows.Forms.Label lblRebookSelectedType;
		private System.Windows.Forms.Label label6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DCancelAppt()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}
		#region Fields

		private bool		m_bClinicCancelled = true;
		private int			m_nReason = 0;
		private string		m_sNote = "";
		private DataTable	m_dtCR;
		private DataView	m_dvCR;
		private bool		m_bAutoRebook = false;
		private int			m_nStart = 7;
		private int			m_nMax = 30;

		// -1: use current, -2: use any non-zero type, >0 use this access type id
		private int			m_nRebookAccessType = -1;


		#endregion Fields

		#region Methods

		public void InitializePage(CGDocumentManager DocManager)
		{
			m_bClinicCancelled = true;
			m_nReason = 0;
			m_sNote = "";

			//Load Reasons listbox

			m_dtCR = DocManager.RPMSDataTable(@"SELECT BMXIEN, NAME FROM CANCELLATION_REASONS WHERE INACTIVE=''", "CR");
			m_dvCR = new DataView(m_dtCR);
			m_dvCR.Sort = "NAME ASC";
			lstReason.DataSource = m_dvCR;
			lstReason.DisplayMember = "NAME";
			lstReason.ValueMember = "BMXIEN";

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
				rdoClinic.Checked = m_bClinicCancelled;
				if (m_nReason != 0)
				{
					lstReason.SelectedValue = m_nReason;
				}
				txtNote.Text = m_sNote;
				chkAutoRebook.Checked = m_bAutoRebook;
				udStart.Value = m_nStart;
				udMax.Value = m_nMax;

				this.rdoRebookSameType.Checked = true;
				this.rdoRebookAnyType.Checked = false;
				this.rdoRebookSelectedType.Checked = false;
			}
			else
			{
				m_bClinicCancelled = rdoClinic.Checked;
				m_nReason = (int) lstReason.SelectedValue;
				m_sNote = txtNote.Text;
				m_bAutoRebook = chkAutoRebook.Checked;
				m_nStart = (int) udStart.Value;
				m_nMax = (int) udMax.Value;
				if (this.rdoRebookSameType.Checked == true)
				{
					m_nRebookAccessType = -1;
				}
				else
				{
					m_nRebookAccessType = -2;
				}

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

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			this.UpdateDialogData(false);
		}
		#endregion Methods

		#region Properties

		/// <summary>
		/// Sets or returns the rebook access type:  -1 = use current type, -2 = use any type, 0 = prompt for a type
		/// </summary>
		public int RebookAccessType
		{
			get
			{
				return m_nRebookAccessType;
			}
			set
			{
				m_nRebookAccessType = value;
				if (m_nRebookAccessType == -1)
				{
					this.rdoRebookSameType.Checked = true;
				}
				else
				{
					this.rdoRebookAnyType.Checked = true;
				}

			}
		}
		/// <summary>
		/// Returns true if appt cancelled by Clinic, otherwise false
		/// </summary>
		public bool ClinicCancelled
		{
			get
			{
				return m_bClinicCancelled;
			}
		}

		/// <summary>
		/// Returns value of AutoRebook check box
		/// </summary>
		public bool AutoRebook
		{
			get
			{
				return m_bAutoRebook;
			}
			set
			{
				m_bAutoRebook = value;
			}
		}

		/// <summary>
		/// Returns internal entry in the CANCELLATION REASON file (409.2)
		/// </summary>
		public int CancelReason
		{
			get
			{
				return m_nReason;
			}
		}

		/// <summary>
		/// Returns cancellation remarks.
		/// </summary>
		public string CancelRemarks
		{
			get
			{
				return m_sNote;
			}
		}

		/// <summary>
		/// Sets or returns the number of days in the future to start searching for availability
		/// </summary>
		public int RebookStartDays
		{
			get
			{
				return m_nStart;
			}
			set
			{
				m_nStart = value;
			}
		}

		/// <summary>
		/// Sets and returns the maximum number of days in the future to look for rebook availability
		/// </summary>
		public int RebookMaxDays
		{
			get
			{
				return m_nMax;
			}
			set
			{
				m_nMax = value;
			}
		}

		#endregion Properties

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlPageBottom = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpDescriptionResourceGroup = new System.Windows.Forms.GroupBox();
			this.lblDescriptionResourceGroup = new System.Windows.Forms.Label();
			this.grpCancelledby = new System.Windows.Forms.GroupBox();
			this.rdoPatient = new System.Windows.Forms.RadioButton();
			this.rdoClinic = new System.Windows.Forms.RadioButton();
			this.lstReason = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtNote = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.grpAutoRebook = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.lblRebookSelectedType = new System.Windows.Forms.Label();
			this.rdoRebookSameType = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.udMax = new System.Windows.Forms.NumericUpDown();
			this.udStart = new System.Windows.Forms.NumericUpDown();
			this.chkAutoRebook = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.rdoRebookAnyType = new System.Windows.Forms.RadioButton();
			this.rdoRebookSelectedType = new System.Windows.Forms.RadioButton();
			this.pnlPageBottom.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescriptionResourceGroup.SuspendLayout();
			this.grpCancelledby.SuspendLayout();
			this.grpAutoRebook.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.udMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udStart)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlPageBottom
			// 
			this.pnlPageBottom.Controls.Add(this.cmdCancel);
			this.pnlPageBottom.Controls.Add(this.cmdOK);
			this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlPageBottom.Location = new System.Drawing.Point(0, 488);
			this.pnlPageBottom.Name = "pnlPageBottom";
			this.pnlPageBottom.Size = new System.Drawing.Size(594, 40);
			this.pnlPageBottom.TabIndex = 6;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(512, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(56, 24);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(432, 8);
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
			this.pnlDescription.Location = new System.Drawing.Point(0, 416);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(594, 72);
			this.pnlDescription.TabIndex = 7;
			// 
			// grpDescriptionResourceGroup
			// 
			this.grpDescriptionResourceGroup.Controls.Add(this.lblDescriptionResourceGroup);
			this.grpDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescriptionResourceGroup.Location = new System.Drawing.Point(0, 0);
			this.grpDescriptionResourceGroup.Name = "grpDescriptionResourceGroup";
			this.grpDescriptionResourceGroup.Size = new System.Drawing.Size(594, 72);
			this.grpDescriptionResourceGroup.TabIndex = 1;
			this.grpDescriptionResourceGroup.TabStop = false;
			this.grpDescriptionResourceGroup.Text = "Description";
			// 
			// lblDescriptionResourceGroup
			// 
			this.lblDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescriptionResourceGroup.Location = new System.Drawing.Point(3, 16);
			this.lblDescriptionResourceGroup.Name = "lblDescriptionResourceGroup";
			this.lblDescriptionResourceGroup.Size = new System.Drawing.Size(588, 53);
			this.lblDescriptionResourceGroup.TabIndex = 0;
			this.lblDescriptionResourceGroup.Text = @"Use this panel to cancel an appointment. Indicate whether the appointment was cancelled by the clinic or by the patient.  Select a reason for the cancellation.  Enter remarks in the text box.  To automatically rebook cancelled appointments, check the Auto Rebook box.  The Start Time in Days and Maximum Days values control the time window for rebooked appointments.";
			// 
			// grpCancelledby
			// 
			this.grpCancelledby.Controls.Add(this.rdoPatient);
			this.grpCancelledby.Controls.Add(this.rdoClinic);
			this.grpCancelledby.Location = new System.Drawing.Point(24, 24);
			this.grpCancelledby.Name = "grpCancelledby";
			this.grpCancelledby.Size = new System.Drawing.Size(256, 80);
			this.grpCancelledby.TabIndex = 8;
			this.grpCancelledby.TabStop = false;
			this.grpCancelledby.Text = "Appointment Cancelled By";
			// 
			// rdoPatient
			// 
			this.rdoPatient.Location = new System.Drawing.Point(24, 48);
			this.rdoPatient.Name = "rdoPatient";
			this.rdoPatient.Size = new System.Drawing.Size(160, 16);
			this.rdoPatient.TabIndex = 1;
			this.rdoPatient.Text = "Cancelled by Patient";
			// 
			// rdoClinic
			// 
			this.rdoClinic.Location = new System.Drawing.Point(24, 24);
			this.rdoClinic.Name = "rdoClinic";
			this.rdoClinic.Size = new System.Drawing.Size(160, 16);
			this.rdoClinic.TabIndex = 0;
			this.rdoClinic.Text = "Cancelled by Clinic";
			// 
			// lstReason
			// 
			this.lstReason.ColumnWidth = 250;
			this.lstReason.Location = new System.Drawing.Point(24, 136);
			this.lstReason.Name = "lstReason";
			this.lstReason.Size = new System.Drawing.Size(256, 264);
			this.lstReason.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(248, 16);
			this.label1.TabIndex = 10;
			this.label1.Text = "Reason for Cancellation (Select one)";
			// 
			// txtNote
			// 
			this.txtNote.Location = new System.Drawing.Point(312, 304);
			this.txtNote.Multiline = true;
			this.txtNote.Name = "txtNote";
			this.txtNote.Size = new System.Drawing.Size(272, 96);
			this.txtNote.TabIndex = 11;
			this.txtNote.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(312, 280);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(280, 64);
			this.label2.TabIndex = 10;
			this.label2.Text = "Remarks (Optional)";
			// 
			// grpAutoRebook
			// 
			this.grpAutoRebook.Controls.Add(this.label6);
			this.grpAutoRebook.Controls.Add(this.lblRebookSelectedType);
			this.grpAutoRebook.Controls.Add(this.rdoRebookSameType);
			this.grpAutoRebook.Controls.Add(this.label3);
			this.grpAutoRebook.Controls.Add(this.udMax);
			this.grpAutoRebook.Controls.Add(this.udStart);
			this.grpAutoRebook.Controls.Add(this.chkAutoRebook);
			this.grpAutoRebook.Controls.Add(this.label4);
			this.grpAutoRebook.Controls.Add(this.rdoRebookAnyType);
			this.grpAutoRebook.Controls.Add(this.rdoRebookSelectedType);
			this.grpAutoRebook.Location = new System.Drawing.Point(312, 24);
			this.grpAutoRebook.Name = "grpAutoRebook";
			this.grpAutoRebook.Size = new System.Drawing.Size(272, 248);
			this.grpAutoRebook.TabIndex = 13;
			this.grpAutoRebook.TabStop = false;
			this.grpAutoRebook.Text = "Auto Rebook";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(232, 16);
			this.label6.TabIndex = 19;
			this.label6.Text = "Access Type for Rebooked Appointment:";
			// 
			// lblRebookSelectedType
			// 
			this.lblRebookSelectedType.Enabled = false;
			this.lblRebookSelectedType.Location = new System.Drawing.Point(64, 224);
			this.lblRebookSelectedType.Name = "lblRebookSelectedType";
			this.lblRebookSelectedType.Size = new System.Drawing.Size(168, 16);
			this.lblRebookSelectedType.TabIndex = 18;
			// 
			// rdoRebookSameType
			// 
			this.rdoRebookSameType.Location = new System.Drawing.Point(24, 152);
			this.rdoRebookSameType.Name = "rdoRebookSameType";
			this.rdoRebookSameType.Size = new System.Drawing.Size(160, 16);
			this.rdoRebookSameType.TabIndex = 17;
			this.rdoRebookSameType.Text = "Same as Current";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(88, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 16;
			this.label3.Text = "Start time in Days";
			// 
			// udMax
			// 
			this.udMax.Increment = new System.Decimal(new int[] {
																	7,
																	0,
																	0,
																	0});
			this.udMax.Location = new System.Drawing.Point(16, 88);
			this.udMax.Maximum = new System.Decimal(new int[] {
																  730,
																  0,
																  0,
																  0});
			this.udMax.Minimum = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  0});
			this.udMax.Name = "udMax";
			this.udMax.Size = new System.Drawing.Size(56, 20);
			this.udMax.TabIndex = 15;
			this.udMax.Value = new System.Decimal(new int[] {
																30,
																0,
																0,
																0});
			// 
			// udStart
			// 
			this.udStart.Location = new System.Drawing.Point(16, 54);
			this.udStart.Maximum = new System.Decimal(new int[] {
																	730,
																	0,
																	0,
																	0});
			this.udStart.Name = "udStart";
			this.udStart.Size = new System.Drawing.Size(56, 20);
			this.udStart.TabIndex = 14;
			this.udStart.Value = new System.Decimal(new int[] {
																  14,
																  0,
																  0,
																  0});
			// 
			// chkAutoRebook
			// 
			this.chkAutoRebook.Location = new System.Drawing.Point(16, 24);
			this.chkAutoRebook.Name = "chkAutoRebook";
			this.chkAutoRebook.Size = new System.Drawing.Size(120, 16);
			this.chkAutoRebook.TabIndex = 13;
			this.chkAutoRebook.Text = "Auto Rebook";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(88, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 16;
			this.label4.Text = "Maximum Days";
			// 
			// rdoRebookAnyType
			// 
			this.rdoRebookAnyType.Location = new System.Drawing.Point(24, 176);
			this.rdoRebookAnyType.Name = "rdoRebookAnyType";
			this.rdoRebookAnyType.Size = new System.Drawing.Size(160, 16);
			this.rdoRebookAnyType.TabIndex = 17;
			this.rdoRebookAnyType.Text = "Any Access Type";
			// 
			// rdoRebookSelectedType
			// 
			this.rdoRebookSelectedType.Enabled = false;
			this.rdoRebookSelectedType.Location = new System.Drawing.Point(24, 200);
			this.rdoRebookSelectedType.Name = "rdoRebookSelectedType";
			this.rdoRebookSelectedType.Size = new System.Drawing.Size(136, 16);
			this.rdoRebookSelectedType.TabIndex = 17;
			this.rdoRebookSelectedType.Text = "Selected Access Type:";
			// 
			// DCancelAppt
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(594, 528);
			this.ControlBox = false;
			this.Controls.Add(this.grpAutoRebook);
			this.Controls.Add(this.txtNote);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lstReason);
			this.Controls.Add(this.grpCancelledby);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlPageBottom);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DCancelAppt";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Cancel Appointment";
			this.pnlPageBottom.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescriptionResourceGroup.ResumeLayout(false);
			this.grpCancelledby.ResumeLayout(false);
			this.grpAutoRebook.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.udMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udStart)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion





	}
}
