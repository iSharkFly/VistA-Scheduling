using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using IndianHealthService.BMXNet;
using System.Collections.Generic;
using System.Linq;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Summary description for DCheckIn.
    /// </summary>
    public class DCheckIn : System.Windows.Forms.Form
    {
        private IContainer components;

        public DCheckIn()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            
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
        private System.Windows.Forms.CheckBox chkRoutingSlip;

        private string m_sPatientName;
        private DateTime m_dCheckIn;
        private string m_sProviderIEN;
        private CGDocumentManager m_DocManager;
        private DataSet m_dsGlobal;
        private DataTable m_dtProvider;
        public bool m_bPrintRouteSlip;
        private List<Provider> _providers;
        private ToolTip toolTip1;
        private bool _myCodeIsFiringIstheCheckBoxChangedEvent; // To prevent the event from firing when I set the control from code
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns Provider chosen for Check-In
        /// </summary>
        public Provider Provider
        {
            get
            {
                if (cboProvider.SelectedIndex < 1) return null; // because first item is empty placeholder
                else return this._providers[cboProvider.SelectedIndex];
            }
        }

        /// <summary>
        /// Returns 'true' if routing slip to be printed; otherwise 'false'
        /// </summary>
        public bool PrintRouteSlip
        {
            get
            {
                return m_bPrintRouteSlip;
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Fill memeber variables before showing dialog
        /// </summary>
        /// <param name="a">Appointment</param>
        /// <param name="docManager">Document Manager</param>
        public void InitializePage(CGAppointment a)
        {
            m_DocManager = CGDocumentManager.Current;
            m_dsGlobal = m_DocManager.GlobalDataSet;

            Int32? nHospLoc = (from resource in m_dsGlobal.Tables["Resources"].AsEnumerable()
                           where resource.Field<string>("RESOURCE_NAME") == a.Resource
                           select resource.Field<Int32?>("HOSPITAL_LOCATION_ID"))
                           .SingleOrDefault();

            //smh - following logic replaced with above...
            /*
            DataView rv = new DataView(this.m_DocManager.GlobalDataSet.Tables["Resources"]);
            rv.Sort = "RESOURCE_NAME ASC";
            int nFind = rv.Find((string)a.Resource);
            DataRowView drv = rv[nFind];

            string sHospLoc = drv["HOSPITAL_LOCATION_ID"].ToString();
            sHospLoc = (sHospLoc == "") ? "0" : sHospLoc;
            int nHospLoc = 0;
            try
            {
                nHospLoc = Convert.ToInt32(sHospLoc);
            }
            catch (Exception ex)
            {
                Debug.Write("CGView.AppointmentCheckIn Error: " + ex.Message);
            }
            */

            //smh new code
            //if the resource is linked to a valid hospital location, grab this locations providers
            //from the provider multiple and put them in the combo box.
            if (nHospLoc != null)
            {
                //RPC BSDX HOSP LOC PROVIDERS returns Table w/ Columns: 
                //HOSPITAL_LOCATION_ID^BMXIEN (ie Prov IEN)^NAME^DEFALUT
                string sCommandText = "BSDX HOSP LOC PROVIDERS^" + nHospLoc;
                m_dtProvider = m_DocManager.RPMSDataTable(sCommandText, "ClinicProviders");
                
                _providers = (from providerRow in m_dtProvider.AsEnumerable()
                              orderby providerRow.Field<string>("NAME")
                             select new Provider
                             {
                                 IEN = providerRow.Field<int>("BMXIEN"),
                                 Name = providerRow.Field<string>("NAME"),
                                 Default = providerRow.Field<string>("DEFAULT") == "YES" ? true : false
                             }).ToList();



                //cboProvider.DisplayMember = "NAME";
                //cboProvider.ValueMember = "BMXIEN";
                _providers.Insert(0, new Provider { Name = "<None>", IEN = -1 });
                cboProvider.DataSource = _providers;
                cboProvider.SelectedIndex = _providers.FindIndex(prov => prov.Default);
                // if no provider is default, set default to be <none> item.
                if (cboProvider.SelectedIndex == -1) cboProvider.SelectedIndex = 0;
                ////Add None to the top of the list
                //DataRow drProv = m_dtProvider.NewRow();
                //drProv.BeginEdit();
                //drProv["HOSPITAL_LOCATION_ID"] = 0;
                //drProv["NAME"] = "<None>";
                //drProv["BMXIEN"] = 0;
                //drProv.EndEdit();
                //m_dtProvider.Rows.InsertAt(drProv, 0);
                ////cboProvider.SelectedIndex = 0;

                //Find default provider--search for Yes in Field DEFAULT            
                //DataRow[] nRow = m_dtProvider.Select("DEFAULT='YES'", "NAME ASC");
                //if (nRow.Length > 0) nFind = m_dtProvider.Rows.IndexOf(nRow[0]);
                

            }
            //otherwise, just use the default provider table
            else
            {

                _providers = (from providerRow in m_dsGlobal.Tables["Provider"].AsEnumerable()
                              orderby providerRow.Field<string>("NAME")
                              select new Provider
                              {
                                  IEN = providerRow.Field<int>("BMXIEN"),
                                  Name = providerRow.Field<string>("NAME"),
                                  Default = false
                              }).ToList();

                
                /*m_dtProvider = m_dsGlobal.Tables["Provider"];
                m_dtProvider.DefaultView.Sort = "NAME ASC";*/
                _providers.Insert(0, new Provider { Name = "<None>", IEN = -1 });
                cboProvider.DataSource = _providers;
                cboProvider.SelectedIndex = 0;
                //cboProvider.DisplayMember = "NAME";
                //cboProvider.ValueMember = "BMXIEN";

                //Add None to the top of the list
                //DataRow drProv = m_dtProvider.NewRow();
                //drProv.BeginEdit();
                //drProv["NAME"] = "<None>";
                //drProv["BMXIEN"] = 0;
                //drProv.EndEdit();
                //m_dtProvider.Rows.InsertAt(drProv, 0);
                //cboProvider.SelectedIndex = 0;
            }

                            

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

            _myCodeIsFiringIstheCheckBoxChangedEvent = true;
            chkRoutingSlip.Checked = CGDocumentManager.Current.UserPreferences.PrintRoutingSlipAutomatically;
            _myCodeIsFiringIstheCheckBoxChangedEvent = false;

            UpdateDialogData(true);
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
                m_dCheckIn = this.dtpCheckIn.Value;
                m_sProviderIEN = this.cboProvider.SelectedValue.ToString();
                m_bPrintRouteSlip = chkRoutingSlip.Checked;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion Methods

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            this.chkRoutingSlip = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlPageBottom.SuspendLayout();
            this.pnlDescription.SuspendLayout();
            this.grpDescriptionResourceGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPageBottom
            // 
            this.pnlPageBottom.Controls.Add(this.cmdCancel);
            this.pnlPageBottom.Controls.Add(this.cmdOK);
            this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPageBottom.Location = new System.Drawing.Point(0, 203);
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
            this.pnlDescription.Location = new System.Drawing.Point(0, 131);
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
            this.lblDescriptionResourceGroup.Text = "Use this panel to check in an appointment. A patient may only be checked-in once." +
                "";
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
            this.lblAlready.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.label3.Text = "Check-in Time:";
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
            this.cboProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            // chkRoutingSlip
            // 
            this.chkRoutingSlip.Location = new System.Drawing.Point(380, 93);
            this.chkRoutingSlip.Name = "chkRoutingSlip";
            this.chkRoutingSlip.Size = new System.Drawing.Size(128, 16);
            this.chkRoutingSlip.TabIndex = 14;
            this.chkRoutingSlip.Text = "Print Routing Slip";
            this.toolTip1.SetToolTip(this.chkRoutingSlip, "Prints routing slip to the Windows Default Printer");
            this.chkRoutingSlip.CheckedChanged += new System.EventHandler(this.chkRoutingSlip_CheckedChanged);
            // 
            // DCheckIn
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 243);
            this.Controls.Add(this.chkRoutingSlip);
            this.Controls.Add(this.cboProvider);
            this.Controls.Add(this.lblPatientName);
            this.Controls.Add(this.lblAlready);
            this.Controls.Add(this.dtpCheckIn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlDescription);
            this.Controls.Add(this.pnlPageBottom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DCheckIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Appointment Check In";
            this.pnlPageBottom.ResumeLayout(false);
            this.pnlDescription.ResumeLayout(false);
            this.grpDescriptionResourceGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Events

        private void cmdOK_Click(object sender, System.EventArgs e)
        {
            this.UpdateDialogData(false);
        }

        /// <summary>
        /// Save this in User Preferences Object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkRoutingSlip_CheckedChanged(object sender, EventArgs e)
        {
            if (_myCodeIsFiringIstheCheckBoxChangedEvent) return;

            CGDocumentManager.Current.UserPreferences.PrintRoutingSlipAutomatically = chkRoutingSlip.Checked;
        }

        #endregion Events

    }
}
