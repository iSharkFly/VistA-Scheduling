using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DCheckIn.
	/// </summary>
	public class DCheckIn : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DCheckIn()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}


		#region Fields
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescriptionResourceGroup;
		private System.Windows.Forms.Label lblDescriptionResourceGroup;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtpCheckIn;
		private System.Windows.Forms.Label lblAlready;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblPatientName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboProvider;
		private System.Windows.Forms.ComboBox cboStopCode;
		private System.Windows.Forms.Label lblStopCode;
		private System.Windows.Forms.CheckBox chkRoutingSlip;
		private System.Windows.Forms.GroupBox grpPCCPlus;
		private System.Windows.Forms.ComboBox cboPCCPlusClinic;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboPCCPlusForm;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox chkPCCOutGuide;

		private string					m_sPatientName;
		private DateTime				m_dCheckIn;
		private string					m_sProvider;
		private string					m_sProviderIEN;
		private string					m_sStopCode;
		private string					m_sStopCodeIEN;
		private CGDocumentManager		m_DocManager;
		private DataSet					m_dsGlobal;
		private DataView				m_dvProvider;
		private DataView				m_dvCS;
		private bool					m_bProviderRequired;
		private DataTable				m_dtClinic;
		private DataTable				m_dtForm;
		private DataView				m_dvClinic;
		private DataView				m_dvForm;
		private bool					m_bInit;
		public bool						m_bPrintRouteSlip;
        private DateTime                m_dAuxTime;

		/*
		 * PCC Variables
		 */
		private bool					m_bPCC;
		private string					m_sPCCClinicIEN;
		private string					m_sPCCFormIEN;
		private bool					m_bPCCOutGuide;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Returns string representation of internal entry number of Provider in PROVIDER File
		/// </summary>
		public string ProviderIEN
		{
			get
			{
				return this.m_sProviderIEN;
			}
		}

		/// <summary>
		/// Returns string representation of IEN of Clinic in VEN EHP CLINIC file
		/// </summary>
		public string PCCClinicIEN
		{
			get
			{
				return this.m_sPCCClinicIEN;
			}
		}

		/// <summary>
		/// Returns string representation of IEN of template entry in VEN PCC TEMPLATE
		/// </summary>
		public string PCCFormIEN
		{
			get
			{
				return m_sPCCFormIEN;
			}
		}

		/// <summary>
		/// Returns 'true' if outguide to be printed; otherwise returns 'false'
		/// </summary>
		public string PCCOutGuide
		{
			get
			{
				string sRet = (this.m_bPCCOutGuide == true)?"true":"false";
				return sRet;
			}
		}

		/// <summary>
		/// Returns string representation of IEN of CLINIC STOP
		/// </summary>
		public string ClinicStopIEN
		{
			get
			{
				return this.m_sStopCodeIEN;
			}
		}

		/// <summary>
		/// Returns 'true' if routing slip to be printed; otherwise 'false'
		/// </summary>
		public string PrintRouteSlip
		{
			get
			{
				string sRet = (this.m_bPrintRouteSlip == true)?"true":"false";
				return sRet;
			}
		}

		/// <summary>
		/// Appointment checkin time
		/// </summary>
		public DateTime CheckInTime
		{
			get
			{
				return m_dCheckIn;
			}
			set
			{
				m_dCheckIn = value;
			}
		}

		/// <summary>
		/// Appointment end time
		/// </summary>
		public DateTime AuxTime
		{
			get
			{
				return m_dAuxTime;
			}
			set
			{
				m_dAuxTime = value;
			}
		}
		#endregion Properties

		#region Methods

		public void InitializePage(CGAppointment a, CGDocumentManager docManager, 
			string sDefaultProvider, bool bProviderRequired, bool bGeneratePCCPlus,
			bool bMultCodes, string sStopCode)
		{
			m_bInit = true;
			m_DocManager = docManager;
			m_dsGlobal = m_DocManager.GlobalDataSet;
			int nFind = 0;

			//Provider processing
			m_bProviderRequired = bProviderRequired;
			m_dvProvider = new DataView(m_dsGlobal.Tables["Provider"]);
			m_dvProvider.Sort = "NAME ASC";
			cboProvider.DataSource = m_dvProvider;
			cboProvider.DisplayMember = "NAME";
			cboProvider.ValueMember = "BMXIEN";

			nFind = m_dvProvider.Find((string) "<None>");
			if (nFind < 0)
			{
				DataRowView drvProv = m_dvProvider.AddNew();
				drvProv.BeginEdit();
				drvProv["NAME"]="<None>";
				drvProv["BMXIEN"]="0";
				drvProv.EndEdit();
			}
			cboProvider.SelectedIndex = 0;
			
			if (sDefaultProvider != "")
			{
				nFind = m_dvProvider.Find((string) sDefaultProvider);
				cboProvider.SelectedIndex = nFind;
				m_sProvider = sDefaultProvider;
			}

			//Stop code processing
			this.lblStopCode.Visible = false;
			this.cboStopCode.Visible = false;
			m_dvCS = new DataView(m_dsGlobal.Tables["ClinicStop"]);
			m_dvCS.Sort = "NAME ASC";
			m_sStopCode = sStopCode;
			m_sStopCodeIEN = "";
			if (m_sStopCode != "")
			{
				//Get the IEN of the clinic stop code
				nFind = m_dvCS.Find((string) m_sStopCode);
				Debug.Assert(nFind > -1);
				if (nFind > -1)
				{
					m_sStopCodeIEN = m_dvCS[nFind].Row["BMXIEN"].ToString();
				}
			}
			
			if (bMultCodes == true)
			{
				this.lblStopCode.Visible = true;
				this.cboStopCode.Visible = true;
				cboStopCode.DataSource = m_dvCS;
				cboStopCode.DisplayMember = "NAME";
				cboStopCode.ValueMember = "BMXIEN";
				if (m_sStopCode != "")
				{
					nFind = m_dvCS.Find((string) m_sStopCode);
					cboStopCode.SelectedIndex = nFind;
				}
			}

				m_bPCC = bGeneratePCCPlus;
				PCCPlus();

				m_sPatientName = a.PatientName;
				if (a.CheckInTime.Ticks != 0)
				{
					m_dCheckIn = a.CheckInTime;
					dtpCheckIn.Enabled = false;
					this.cboProvider.Enabled = false;
					lblAlready.Visible = true;
				}
				else
				{
					m_dCheckIn = DateTime.Now;
				}
				UpdateDialogData(true);
				m_bInit = false;
				
				//Synchronize PCCForm with Clinic
				if (m_bPCC == true)
				{	
					cboPCCPlusClinic_SelectedIndexChanged(this, new System.EventArgs());
				}
			}

		private void PCCPlus()
		{
			//PCCPlus processing
			/*Can't do PCCPlus if no stop code
				* or if PRINT PCC PLUS FORM field in CLINIC SETUP PARAMETERS is false
				*/
			if ((m_bPCC == false) ||(m_sStopCode == ""))
			{
				grpPCCPlus.Enabled = false;
				return;
			}
			else
			{
				grpPCCPlus.Enabled = true;
				//Populate combo box with recordset of clinics based on m_sStopCode
                string sCmd = "SELECT BMXIEN, NAME, DEPARTMENT, DEFAULT_ENCOUNTER_FORM, NEVER_PRINT_OUTGUIDE FROM VEN_EHP_CLINIC WHERE DEPARTMENT = '" + m_sStopCode + "'";
                m_dtClinic = m_DocManager.ConnectInfo.RPMSDataTable(sCmd, "CLINIC");
				m_dvClinic = new DataView(m_dtClinic);
				m_dvClinic.Sort = "NAME ASC";

				cboPCCPlusClinic.DataSource = m_dvClinic;
				cboPCCPlusClinic.DisplayMember = "NAME";
				cboPCCPlusClinic.ValueMember = "BMXIEN";


				//Populate combo box with recordset of all forms
                sCmd = "SELECT BMXIEN, TEMPLATE FROM VEN_EHP_EF_TEMPLATES";
                m_dtForm = m_DocManager.ConnectInfo.RPMSDataTable(sCmd, "FORM");
				m_dvForm = new DataView(m_dtForm);
				m_dvForm.Sort = "TEMPLATE ASC";

				cboPCCPlusForm.DataSource = m_dvForm;
				cboPCCPlusForm.DisplayMember = "TEMPLATE";
				cboPCCPlusForm.ValueMember = "BMXIEN";

				if ((m_dtClinic.Rows.Count == 0) ||(m_dtForm.Rows.Count == 0))
				{
					//No PCCPlus clinics for current stop code
					//or no forms available
					grpPCCPlus.Enabled = false;
					return;
				}

				cboPCCPlusClinic.SelectedIndex = 0;
				cboPCCPlusClinic_SelectedIndexChanged(this, new System.EventArgs());

			}
		}

		/// <summary>
		/// If b is true, moves member vars into control data
		/// otherwise, moves control data into member vars
		/// </summary>
		/// <param name="b"></param>
		private void UpdateDialogData(bool b)
		{
			if (b == true) //Move data to dialog controls from member variables
			{
				this.lblPatientName.Text = m_sPatientName;
				this.dtpCheckIn.Value = m_dCheckIn;
			}
			else //Move data to member variables from dialog controls
			{
				
				/*
				 * Need to return Provider, ClinicStop, PrintRouteSlip, 
				 * PCC Clinic, PCC Form, Print OutGuide
				 */

				m_dCheckIn = this.dtpCheckIn.Value;
				m_sProviderIEN = this.cboProvider.SelectedValue.ToString();
				m_bPrintRouteSlip = chkRoutingSlip.Checked;

				/*
				 * Don't get value from CLINIC STOP combo since
				 * it may not be enabled, and
				 * it updates the member variable whenever the selection changes
				 */

				/*
				 * PCCPlus
				 */
					
				if (grpPCCPlus.Enabled == false)
				{
					m_bPCC = false;
					m_sPCCClinicIEN = "";
					m_sPCCFormIEN = "";
					m_bPCCOutGuide = false;
				}
				else
				{
					m_bPCC = true;
					m_sPCCClinicIEN = this.cboPCCPlusClinic.SelectedValue.ToString();
					m_sPCCFormIEN = this.cboPCCPlusForm.SelectedValue.ToString();
					if (chkPCCOutGuide.Enabled == false)
					{
						m_bPCCOutGuide = false;
					}
					else
					{
						m_bPCCOutGuide = this.chkPCCOutGuide.Checked;
					}
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
		#endregion Methods

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
			this.label1 = new System.Windows.Forms.Label();
			this.dtpCheckIn = new System.Windows.Forms.DateTimePicker();
			this.lblAlready = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblPatientName = new System.Windows.Forms.Label();
			this.cboProvider = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cboStopCode = new System.Windows.Forms.ComboBox();
			this.lblStopCode = new System.Windows.Forms.Label();
			this.chkRoutingSlip = new System.Windows.Forms.CheckBox();
			this.grpPCCPlus = new System.Windows.Forms.GroupBox();
			this.chkPCCOutGuide = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cboPCCPlusClinic = new System.Windows.Forms.ComboBox();
			this.cboPCCPlusForm = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.pnlPageBottom.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescriptionResourceGroup.SuspendLayout();
			this.grpPCCPlus.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlPageBottom
			// 
			this.pnlPageBottom.Controls.Add(this.cmdCancel);
			this.pnlPageBottom.Controls.Add(this.cmdOK);
			this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlPageBottom.Location = new System.Drawing.Point(0, 360);
			this.pnlPageBottom.Name = "pnlPageBottom";
			this.pnlPageBottom.Size = new System.Drawing.Size(520, 40);
			this.pnlPageBottom.TabIndex = 5;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(440, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(56, 24);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(360, 8);
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
			this.pnlDescription.Location = new System.Drawing.Point(0, 288);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(520, 72);
			this.pnlDescription.TabIndex = 6;
			// 
			// grpDescriptionResourceGroup
			// 
			this.grpDescriptionResourceGroup.Controls.Add(this.lblDescriptionResourceGroup);
			this.grpDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescriptionResourceGroup.Location = new System.Drawing.Point(0, 0);
			this.grpDescriptionResourceGroup.Name = "grpDescriptionResourceGroup";
			this.grpDescriptionResourceGroup.Size = new System.Drawing.Size(520, 72);
			this.grpDescriptionResourceGroup.TabIndex = 1;
			this.grpDescriptionResourceGroup.TabStop = false;
			this.grpDescriptionResourceGroup.Text = "Description";
			// 
			// lblDescriptionResourceGroup
			// 
			this.lblDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescriptionResourceGroup.Location = new System.Drawing.Point(3, 16);
			this.lblDescriptionResourceGroup.Name = "lblDescriptionResourceGroup";
			this.lblDescriptionResourceGroup.Size = new System.Drawing.Size(514, 53);
			this.lblDescriptionResourceGroup.TabIndex = 0;
			this.lblDescriptionResourceGroup.Text = "Use this panel to check in an appointment. A PCC visit will automatically be crea" +
				"ted for this patient at the check in date and time if the clinic is set up to cr" +
				"eate a visit at checkin.  A patient may only be checked-in once.";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Patient Name:";
			// 
			// dtpCheckIn
			// 
			this.dtpCheckIn.AllowDrop = true;
			this.dtpCheckIn.CustomFormat = "MMMM dd yyyy H:mm";
			this.dtpCheckIn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpCheckIn.Location = new System.Drawing.Point(96, 48);
			this.dtpCheckIn.Name = "dtpCheckIn";
			this.dtpCheckIn.ShowUpDown = true;
			this.dtpCheckIn.Size = new System.Drawing.Size(176, 20);
			this.dtpCheckIn.TabIndex = 9;
			// 
			// lblAlready
			// 
			this.lblAlready.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblAlready.ForeColor = System.Drawing.Color.Green;
			this.lblAlready.Location = new System.Drawing.Point(288, 40);
			this.lblAlready.Name = "lblAlready";
			this.lblAlready.Size = new System.Drawing.Size(192, 32);
			this.lblAlready.TabIndex = 10;
			this.lblAlready.Text = "This Patient is already checked in.";
			this.lblAlready.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 7;
			this.label3.Text = "CheckIn Time:";
			// 
			// lblPatientName
			// 
			this.lblPatientName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPatientName.Location = new System.Drawing.Point(96, 16);
			this.lblPatientName.Name = "lblPatientName";
			this.lblPatientName.Size = new System.Drawing.Size(256, 16);
			this.lblPatientName.TabIndex = 11;
			// 
			// cboProvider
			// 
			this.cboProvider.Location = new System.Drawing.Point(96, 88);
			this.cboProvider.Name = "cboProvider";
			this.cboProvider.Size = new System.Drawing.Size(240, 21);
			this.cboProvider.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Visit Provider:";
			// 
			// cboStopCode
			// 
			this.cboStopCode.Location = new System.Drawing.Point(96, 128);
			this.cboStopCode.Name = "cboStopCode";
			this.cboStopCode.Size = new System.Drawing.Size(240, 21);
			this.cboStopCode.TabIndex = 13;
			this.cboStopCode.SelectedIndexChanged += new System.EventHandler(this.cboStopCode_SelectedIndexChanged);
			// 
			// lblStopCode
			// 
			this.lblStopCode.Location = new System.Drawing.Point(16, 128);
			this.lblStopCode.Name = "lblStopCode";
			this.lblStopCode.Size = new System.Drawing.Size(80, 16);
			this.lblStopCode.TabIndex = 7;
			this.lblStopCode.Text = "Stop Code:";
			// 
			// chkRoutingSlip
			// 
			this.chkRoutingSlip.Location = new System.Drawing.Point(368, 88);
			this.chkRoutingSlip.Name = "chkRoutingSlip";
			this.chkRoutingSlip.Size = new System.Drawing.Size(128, 16);
			this.chkRoutingSlip.TabIndex = 14;
			this.chkRoutingSlip.Text = "Print Routing Slip";
			// 
			// grpPCCPlus
			// 
			this.grpPCCPlus.Controls.Add(this.chkPCCOutGuide);
			this.grpPCCPlus.Controls.Add(this.label4);
			this.grpPCCPlus.Controls.Add(this.cboPCCPlusClinic);
			this.grpPCCPlus.Controls.Add(this.cboPCCPlusForm);
			this.grpPCCPlus.Controls.Add(this.label5);
			this.grpPCCPlus.Location = new System.Drawing.Point(8, 168);
			this.grpPCCPlus.Name = "grpPCCPlus";
			this.grpPCCPlus.Size = new System.Drawing.Size(488, 104);
			this.grpPCCPlus.TabIndex = 15;
			this.grpPCCPlus.TabStop = false;
			this.grpPCCPlus.Text = "PCC Plus";
			// 
			// chkPCCOutGuide
			// 
			this.chkPCCOutGuide.Location = new System.Drawing.Point(360, 24);
			this.chkPCCOutGuide.Name = "chkPCCOutGuide";
			this.chkPCCOutGuide.Size = new System.Drawing.Size(96, 16);
			this.chkPCCOutGuide.TabIndex = 15;
			this.chkPCCOutGuide.Text = "Print Outguide";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "PCC+ Clinic:";
			// 
			// cboPCCPlusClinic
			// 
			this.cboPCCPlusClinic.Location = new System.Drawing.Point(88, 24);
			this.cboPCCPlusClinic.Name = "cboPCCPlusClinic";
			this.cboPCCPlusClinic.Size = new System.Drawing.Size(240, 21);
			this.cboPCCPlusClinic.TabIndex = 0;
			this.cboPCCPlusClinic.SelectedIndexChanged += new System.EventHandler(this.cboPCCPlusClinic_SelectedIndexChanged);
			// 
			// cboPCCPlusForm
			// 
			this.cboPCCPlusForm.Location = new System.Drawing.Point(88, 64);
			this.cboPCCPlusForm.Name = "cboPCCPlusForm";
			this.cboPCCPlusForm.Size = new System.Drawing.Size(240, 21);
			this.cboPCCPlusForm.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 16);
			this.label5.TabIndex = 8;
			this.label5.Text = "PCC+ Form:";
			// 
			// DCheckIn
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 400);
			this.Controls.Add(this.grpPCCPlus);
			this.Controls.Add(this.chkRoutingSlip);
			this.Controls.Add(this.cboStopCode);
			this.Controls.Add(this.cboProvider);
			this.Controls.Add(this.lblPatientName);
			this.Controls.Add(this.lblAlready);
			this.Controls.Add(this.dtpCheckIn);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlPageBottom);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblStopCode);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DCheckIn";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Appointment Check In";
			this.pnlPageBottom.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescriptionResourceGroup.ResumeLayout(false);
			this.grpPCCPlus.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Events

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			this.UpdateDialogData(false);		
		}

		private void cboStopCode_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			/*
			 * Whenever the stop code changes, the set of PCCPlus Clinic Selections change
			 * except during init.
			 */
			if (m_bInit == true)
				return;

			//Change the value of m_sStopCode
			DataRowView drv = (DataRowView) this.cboStopCode.SelectedItem;
			string sStopCode = drv.Row["NAME"].ToString();
			m_sStopCode = sStopCode;
			m_sStopCodeIEN = drv.Row["BMXIEN"].ToString();
			PCCPlus();
		}

		private void cboPCCPlusClinic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			/*
			 * Whenever the PCCPlus Clinic changes, the default EF TEMPLATE changes
			 */
			if (m_bInit == true)
				return;

			if (this.cboPCCPlusClinic.SelectedItem == null)
				return;

			DataRowView drv = (DataRowView) this.cboPCCPlusClinic.SelectedItem;
			string sDefaultForm = drv.Row["DEFAULT_ENCOUNTER_FORM"].ToString();
		
			int nFind = this.m_dvForm.Find(sDefaultForm);
			if (nFind > -1)
			{
				this.cboPCCPlusForm.SelectedIndex = nFind;
			}
		}
		#endregion Events


	}
}
