using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//using System.Data.OleDb;
using System.Diagnostics;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Appointment Info Dialog
	/// </summary>
	public class DAppointPage : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPatientInfo;
		private System.Windows.Forms.TabPage tabAppointment;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtCity;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtZip;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtState;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtStreet;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtPhoneOffice;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtPhoneHome;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtSSN;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtDOB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPatientName;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label lblClinic;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtNote;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblDuration;
		private System.Windows.Forms.Label lblStartTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtHRN;
        private GroupBox groupBox4;
        private BindingSource dsPatientApptDisplay2BindingSource;
        private dsPatientApptDisplay2 dsPatientApptDisplay2;
        private BindingSource patientApptsBindingSource;
        private Label label17;
        private TextBox txtEmail;
        private Label label16;
        private TextBox txtPhoneCell;
        private Label label7;
        private TextBox txtCountry;
        private IContainer components;

		public DAppointPage()
		{
			InitializeComponent();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAppointment = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblClinic = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtHRN = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSSN = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDOB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.tabPatientInfo = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtPhoneOffice = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtPhoneHome = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtState = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtStreet = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.patientApptsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2 = new IndianHealthService.ClinicalScheduling.dsPatientApptDisplay2();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPhoneCell = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabAppointment.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPatientInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.patientApptsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAppointment);
            this.tabControl1.Controls.Add(this.tabPatientInfo);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(471, 526);
            this.tabControl1.TabIndex = 0;
            // 
            // tabAppointment
            // 
            this.tabAppointment.Controls.Add(this.groupBox4);
            this.tabAppointment.Controls.Add(this.groupBox3);
            this.tabAppointment.Controls.Add(this.groupBox1);
            this.tabAppointment.Location = new System.Drawing.Point(4, 22);
            this.tabAppointment.Name = "tabAppointment";
            this.tabAppointment.Size = new System.Drawing.Size(463, 500);
            this.tabAppointment.TabIndex = 1;
            this.tabAppointment.Text = "Appointment";
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(8, 254);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(439, 204);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other Appointments";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblClinic);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.txtNote);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblDuration);
            this.groupBox3.Controls.Add(this.lblStartTime);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(8, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(439, 141);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appointment";
            // 
            // lblClinic
            // 
            this.lblClinic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblClinic.Location = new System.Drawing.Point(200, 48);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new System.Drawing.Size(233, 16);
            this.lblClinic.TabIndex = 19;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(152, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 16);
            this.label15.TabIndex = 18;
            this.label15.Text = "Clinic:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNote
            // 
            this.txtNote.AcceptsReturn = true;
            this.txtNote.Location = new System.Drawing.Point(80, 72);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(353, 60);
            this.txtNote.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Notes:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDuration
            // 
            this.lblDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDuration.Location = new System.Drawing.Point(80, 48);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(56, 16);
            this.lblDuration.TabIndex = 15;
            // 
            // lblStartTime
            // 
            this.lblStartTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStartTime.Location = new System.Drawing.Point(80, 24);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(353, 16);
            this.lblStartTime.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Duration:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "Start Time:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtHRN);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtSSN);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtDOB);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPatientName);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 93);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Patient ID";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(56, 64);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 16);
            this.label14.TabIndex = 13;
            this.label14.Text = "HRN:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtHRN
            // 
            this.txtHRN.Location = new System.Drawing.Point(96, 64);
            this.txtHRN.Name = "txtHRN";
            this.txtHRN.ReadOnly = true;
            this.txtHRN.Size = new System.Drawing.Size(120, 20);
            this.txtHRN.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(224, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "SSN:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSSN
            // 
            this.txtSSN.Location = new System.Drawing.Point(272, 40);
            this.txtSSN.Name = "txtSSN";
            this.txtSSN.ReadOnly = true;
            this.txtSSN.Size = new System.Drawing.Size(161, 20);
            this.txtSSN.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(64, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "DOB:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDOB
            // 
            this.txtDOB.Location = new System.Drawing.Point(96, 40);
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.ReadOnly = true;
            this.txtDOB.Size = new System.Drawing.Size(120, 20);
            this.txtDOB.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(56, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPatientName
            // 
            this.txtPatientName.Location = new System.Drawing.Point(96, 16);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.ReadOnly = true;
            this.txtPatientName.Size = new System.Drawing.Size(337, 20);
            this.txtPatientName.TabIndex = 0;
            // 
            // tabPatientInfo
            // 
            this.tabPatientInfo.Controls.Add(this.groupBox2);
            this.tabPatientInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPatientInfo.Name = "tabPatientInfo";
            this.tabPatientInfo.Size = new System.Drawing.Size(463, 500);
            this.tabPatientInfo.TabIndex = 0;
            this.tabPatientInfo.Text = "Contact Information";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.txtEmail);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.txtPhoneCell);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtCountry);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtPhoneOffice);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtPhoneHome);
            this.groupBox2.Controls.Add(this.txtCity);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtZip);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtState);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtStreet);
            this.groupBox2.Location = new System.Drawing.Point(8, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(444, 198);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Address";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(2, 166);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 14);
            this.label12.TabIndex = 23;
            this.label12.Text = "Phone (Office):";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhoneOffice
            // 
            this.txtPhoneOffice.Location = new System.Drawing.Point(91, 166);
            this.txtPhoneOffice.Name = "txtPhoneOffice";
            this.txtPhoneOffice.ReadOnly = true;
            this.txtPhoneOffice.Size = new System.Drawing.Size(120, 20);
            this.txtPhoneOffice.TabIndex = 22;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(5, 118);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 16);
            this.label13.TabIndex = 21;
            this.label13.Text = "Phone (Home):";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhoneHome
            // 
            this.txtPhoneHome.Location = new System.Drawing.Point(91, 118);
            this.txtPhoneHome.Name = "txtPhoneHome";
            this.txtPhoneHome.ReadOnly = true;
            this.txtPhoneHome.Size = new System.Drawing.Size(120, 20);
            this.txtPhoneHome.TabIndex = 20;
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(91, 46);
            this.txtCity.Name = "txtCity";
            this.txtCity.ReadOnly = true;
            this.txtCity.Size = new System.Drawing.Size(342, 20);
            this.txtCity.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(52, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "City:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(224, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Zip:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(267, 70);
            this.txtZip.Name = "txtZip";
            this.txtZip.ReadOnly = true;
            this.txtZip.Size = new System.Drawing.Size(166, 20);
            this.txtZip.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(47, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "State:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(91, 70);
            this.txtState.Name = "txtState";
            this.txtState.ReadOnly = true;
            this.txtState.Size = new System.Drawing.Size(120, 20);
            this.txtState.TabIndex = 14;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(47, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 16);
            this.label11.TabIndex = 13;
            this.label11.Text = "Street:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStreet
            // 
            this.txtStreet.Location = new System.Drawing.Point(91, 22);
            this.txtStreet.Name = "txtStreet";
            this.txtStreet.ReadOnly = true;
            this.txtStreet.Size = new System.Drawing.Size(342, 20);
            this.txtStreet.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 486);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 40);
            this.panel1.TabIndex = 1;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(387, 8);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(64, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(317, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // patientApptsBindingSource
            // 
            this.patientApptsBindingSource.DataMember = "PatientAppts";
            this.patientApptsBindingSource.DataSource = this.dsPatientApptDisplay2BindingSource;
            // 
            // dsPatientApptDisplay2BindingSource
            // 
            this.dsPatientApptDisplay2BindingSource.DataSource = this.dsPatientApptDisplay2;
            this.dsPatientApptDisplay2BindingSource.Position = 0;
            // 
            // dsPatientApptDisplay2
            // 
            this.dsPatientApptDisplay2.DataSetName = "dsPatientApptDisplay2";
            this.dsPatientApptDisplay2.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(33, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 16);
            this.label7.TabIndex = 25;
            this.label7.Text = "Country:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCountry
            // 
            this.txtCountry.Location = new System.Drawing.Point(91, 94);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.ReadOnly = true;
            this.txtCountry.Size = new System.Drawing.Size(120, 20);
            this.txtCountry.TabIndex = 24;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(19, 142);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(69, 16);
            this.label16.TabIndex = 27;
            this.label16.Text = "Phone (Cell):";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhoneCell
            // 
            this.txtPhoneCell.Location = new System.Drawing.Point(91, 142);
            this.txtPhoneCell.Name = "txtPhoneCell";
            this.txtPhoneCell.ReadOnly = true;
            this.txtPhoneCell.Size = new System.Drawing.Size(120, 20);
            this.txtPhoneCell.TabIndex = 26;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(224, 94);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 16);
            this.label17.TabIndex = 29;
            this.label17.Text = "E-Mail:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(267, 94);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.ReadOnly = true;
            this.txtEmail.Size = new System.Drawing.Size(166, 20);
            this.txtEmail.TabIndex = 28;
            // 
            // DAppointPage
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(471, 526);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DAppointPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Appointment";
            this.tabControl1.ResumeLayout(false);
            this.tabAppointment.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPatientInfo.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.patientApptsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private CGDocumentManager	m_DocManager;

		private string			m_sPatientName;
		private string			m_sPatientHRN;
		private string			m_sPatientIEN;
		private string			m_sPatientDOB;
		private string			m_sPatientSSN;

		private string			m_sCity;
		private string			m_sPhoneHome;
		private string			m_sPhoneOffice;
		private string			m_sState;
		private string			m_sStreet;
		private string			m_sZip;

		private string			m_sNote;
		private	DateTime		m_dStartTime;
		private int				m_nDuration;
		private string			m_sClinic;

        private string          m_sPhoneCell;
        private string          m_sEmail;
        private string          m_sCountry;

		#endregion //fields

		#region Methods

		public void InitializePage(CGAppointment a)
		{
			InitializePage(a.PatientID.ToString(), a.StartTime, a.Duration, "", a.Note);
		}

		public void InitializePage(string sPatientIEN, DateTime dStart, int nDuration, string sClinic, string sNote)
		{
			m_dStartTime = dStart;
			m_nDuration = nDuration;
			m_sClinic = sClinic;
			m_sPatientIEN = sPatientIEN;
			m_sNote = sNote;
			try 
			{
				string sSql;
				sSql = "BSDX GET BASIC REG INFO^" + m_sPatientIEN;

				DataTable tb = m_DocManager.RPMSDataTable(sSql, "PatientRegInfo");

				Debug.Assert(tb.Rows.Count == 1);
				DataRow r = tb.Rows[0];
				this.m_sPatientName = r["NAME"].ToString();
				this.m_sPatientHRN = r["HRN"].ToString();
				this.m_sPatientIEN = r["IEN"].ToString();
				this.m_sPatientSSN = r["SSN"].ToString();
				DateTime dDob =(DateTime) r["DOB"]; //what if it's null?
				this.m_sPatientDOB = dDob.ToShortDateString();
				this.m_sStreet = r["STREET"].ToString();
				this.m_sCity = r["CITY"].ToString();
				this.m_sPhoneOffice = r["OFCPHONE"].ToString();
				this.m_sState = r["STATE"].ToString();
				this.m_sZip = r["ZIP"].ToString();
				this.m_sPhoneHome = r["HOMEPHONE"].ToString();
                this.m_sEmail = r["EMAIL ADDRESS"].ToString();
                this.m_sPhoneCell = r["PHONE NUMBER [CELLULAR]"].ToString();
                this.m_sCountry = r["COUNTRY"].ToString();
				this.UpdateDialogData(true);
                Control UC = new UCPatientAppts(m_DocManager, int.Parse(m_sPatientIEN));
                UC.Dock = DockStyle.Fill;
                groupBox4.Controls.Add(UC);
            }
			catch(Exception e)
			{
				MessageBox.Show("DAppointPage::InitializePage -- Unable to retrieve patient information from VistA.  " + e.Message);
			}

		}
		/// <summary>
		/// Move data from member variables to controls (b == true)
		/// or from controls to member variables (b == false)
		/// </summary>
		/// <param name="b"></param>
		private void UpdateDialogData(bool b)
		{
			if (b == true) //move member vars into control data
			{
				lblClinic.Text = m_sClinic;
				lblDuration.Text = m_nDuration.ToString();
				lblStartTime.Text = m_dStartTime.ToShortDateString() + " " + m_dStartTime.ToShortTimeString();

				txtCity.Text = this.m_sCity;
				txtDOB.Text = this.m_sPatientDOB;
				txtHRN.Text = this.m_sPatientHRN;
				txtNote.Text = this.m_sNote;
				txtPatientName.Text = m_sPatientName;
				txtPhoneHome.Text = this.m_sPhoneHome;
				txtPhoneOffice.Text = this.m_sPhoneOffice;
				txtSSN.Text = this.m_sPatientSSN;
				txtState.Text = this.m_sState;
				txtStreet.Text = this.m_sStreet;
				txtZip.Text = this.m_sZip;
                txtEmail.Text = this.m_sEmail;
                txtPhoneCell.Text = this.m_sPhoneCell;
                txtCountry.Text = this.m_sCountry;

			}
			else //move control data into member vars
			{
				string sNote = txtNote.Text;
				sNote = sNote.Replace("^", " ");
				this.m_sNote = sNote;
			}
			
		}
		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			this.UpdateDialogData(false);
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

		#region Properties
	
		public string Note
		{
			get
			{
				return m_sNote;
			}
			set
			{
				m_sNote = value;
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

		#endregion //Properties

	} //end Class
}
