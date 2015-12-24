using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
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
		private System.Windows.Forms.TextBox txtPID;
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
        private CheckBox chkPrint;
        private Label label18;
        private TextBox txtSex;
        private TabPage tabCloneForward;
        private GroupBox grpCloneForward;
        private Button button1;
        private ComboBox cboDuration;
        private TextBox txtFUOther;
        private Label label21;
        private RadioButton rdbFU90;
        private RadioButton rdbFU60;
        private RadioButton rdbFU30;
        private Label label19;
        private DateTimePicker dtCloneAppointment;
        private ComboBox cboSlots;
        private Label label25;
        private Label label24;
        private Label txtClinic;
        private TextBox textBox1;
        private Label label20;
        private Label label22;
        private Label label23;
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
            this.txtSex = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtHRN = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDOB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.tabCloneForward = new System.Windows.Forms.TabPage();
            this.grpCloneForward = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cboDuration = new System.Windows.Forms.ComboBox();
            this.txtFUOther = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.rdbFU90 = new System.Windows.Forms.RadioButton();
            this.rdbFU60 = new System.Windows.Forms.RadioButton();
            this.rdbFU30 = new System.Windows.Forms.RadioButton();
            this.label19 = new System.Windows.Forms.Label();
            this.dtCloneAppointment = new System.Windows.Forms.DateTimePicker();
            this.cboSlots = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.txtClinic = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tabPatientInfo = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPhoneCell = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCountry = new System.Windows.Forms.TextBox();
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
            this.chkPrint = new System.Windows.Forms.CheckBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.patientApptsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2 = new IndianHealthService.ClinicalScheduling.dsPatientApptDisplay2();
            this.tabControl1.SuspendLayout();
            this.tabAppointment.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabCloneForward.SuspendLayout();
            this.grpCloneForward.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabCloneForward);
            this.tabControl1.Controls.Add(this.tabPatientInfo);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(475, 524);
            this.tabControl1.TabIndex = 0;
            // 
            // tabAppointment
            // 
            this.tabAppointment.Controls.Add(this.groupBox4);
            this.tabAppointment.Controls.Add(this.groupBox3);
            this.tabAppointment.Controls.Add(this.groupBox1);
            this.tabAppointment.Location = new System.Drawing.Point(4, 22);
            this.tabAppointment.Name = "tabAppointment";
            this.tabAppointment.Size = new System.Drawing.Size(467, 498);
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
            this.groupBox1.Controls.Add(this.txtSex);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtHRN);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtPID);
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
            // txtSex
            // 
            this.txtSex.BackColor = System.Drawing.SystemColors.Control;
            this.txtSex.Location = new System.Drawing.Point(273, 41);
            this.txtSex.Name = "txtSex";
            this.txtSex.ReadOnly = true;
            this.txtSex.Size = new System.Drawing.Size(160, 20);
            this.txtSex.TabIndex = 15;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(238, 44);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(28, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Sex:";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(50, 64);
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
            this.label6.Location = new System.Drawing.Point(227, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "ID:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPID
            // 
            this.txtPID.Location = new System.Drawing.Point(272, 63);
            this.txtPID.Name = "txtPID";
            this.txtPID.ReadOnly = true;
            this.txtPID.Size = new System.Drawing.Size(161, 20);
            this.txtPID.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(58, 41);
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
            this.label2.Location = new System.Drawing.Point(50, 16);
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
            // tabCloneForward
            // 
            this.tabCloneForward.BackColor = System.Drawing.SystemColors.Control;
            this.tabCloneForward.Controls.Add(this.grpCloneForward);
            this.tabCloneForward.Location = new System.Drawing.Point(4, 22);
            this.tabCloneForward.Name = "tabCloneForward";
            this.tabCloneForward.Size = new System.Drawing.Size(467, 498);
            this.tabCloneForward.TabIndex = 2;
            this.tabCloneForward.Text = "Clone/Forward";
            // 
            // grpCloneForward
            // 
            this.grpCloneForward.Controls.Add(this.button1);
            this.grpCloneForward.Controls.Add(this.cboDuration);
            this.grpCloneForward.Controls.Add(this.txtFUOther);
            this.grpCloneForward.Controls.Add(this.label21);
            this.grpCloneForward.Controls.Add(this.rdbFU90);
            this.grpCloneForward.Controls.Add(this.rdbFU60);
            this.grpCloneForward.Controls.Add(this.rdbFU30);
            this.grpCloneForward.Controls.Add(this.label19);
            this.grpCloneForward.Controls.Add(this.dtCloneAppointment);
            this.grpCloneForward.Controls.Add(this.cboSlots);
            this.grpCloneForward.Controls.Add(this.label25);
            this.grpCloneForward.Controls.Add(this.label24);
            this.grpCloneForward.Controls.Add(this.txtClinic);
            this.grpCloneForward.Controls.Add(this.textBox1);
            this.grpCloneForward.Controls.Add(this.label20);
            this.grpCloneForward.Controls.Add(this.label22);
            this.grpCloneForward.Controls.Add(this.label23);
            this.grpCloneForward.Location = new System.Drawing.Point(8, 13);
            this.grpCloneForward.Name = "grpCloneForward";
            this.grpCloneForward.Size = new System.Drawing.Size(439, 206);
            this.grpCloneForward.TabIndex = 16;
            this.grpCloneForward.TabStop = false;
            this.grpCloneForward.Text = "Clone/Forward Appointment";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(357, 82);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 24);
            this.button1.TabIndex = 37;
            this.button1.Text = "Update";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboDuration
            // 
            this.cboDuration.FormattingEnabled = true;
            this.cboDuration.Location = new System.Drawing.Point(80, 51);
            this.cboDuration.Name = "cboDuration";
            this.cboDuration.Size = new System.Drawing.Size(66, 21);
            this.cboDuration.TabIndex = 36;
            this.cboDuration.SelectedIndexChanged += new System.EventHandler(this.cboDuration_SelectedIndexChanged);
            // 
            // txtFUOther
            // 
            this.txtFUOther.Location = new System.Drawing.Point(297, 84);
            this.txtFUOther.Name = "txtFUOther";
            this.txtFUOther.Size = new System.Drawing.Size(54, 20);
            this.txtFUOther.TabIndex = 35;
            this.txtFUOther.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFUOther_KeyPress);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(255, 88);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(36, 13);
            this.label21.TabIndex = 34;
            this.label21.Text = "Other:";
            // 
            // rdbFU90
            // 
            this.rdbFU90.AutoSize = true;
            this.rdbFU90.Location = new System.Drawing.Point(203, 85);
            this.rdbFU90.Name = "rdbFU90";
            this.rdbFU90.Size = new System.Drawing.Size(37, 17);
            this.rdbFU90.TabIndex = 33;
            this.rdbFU90.TabStop = true;
            this.rdbFU90.Text = "90";
            this.rdbFU90.UseVisualStyleBackColor = true;
            this.rdbFU90.CheckedChanged += new System.EventHandler(this.rdbFUCheckedChanged);
            // 
            // rdbFU60
            // 
            this.rdbFU60.AutoSize = true;
            this.rdbFU60.Location = new System.Drawing.Point(160, 85);
            this.rdbFU60.Name = "rdbFU60";
            this.rdbFU60.Size = new System.Drawing.Size(37, 17);
            this.rdbFU60.TabIndex = 32;
            this.rdbFU60.TabStop = true;
            this.rdbFU60.Text = "60";
            this.rdbFU60.UseVisualStyleBackColor = true;
            this.rdbFU60.CheckedChanged += new System.EventHandler(this.rdbFUCheckedChanged);
            // 
            // rdbFU30
            // 
            this.rdbFU30.AutoSize = true;
            this.rdbFU30.Location = new System.Drawing.Point(117, 85);
            this.rdbFU30.Name = "rdbFU30";
            this.rdbFU30.Size = new System.Drawing.Size(37, 17);
            this.rdbFU30.TabIndex = 31;
            this.rdbFU30.TabStop = true;
            this.rdbFU30.Text = "30";
            this.rdbFU30.UseVisualStyleBackColor = true;
            this.rdbFU30.CheckedChanged += new System.EventHandler(this.rdbFUCheckedChanged);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(4, 85);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(103, 16);
            this.label19.TabIndex = 30;
            this.label19.Text = "Quick Follow up:";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtCloneAppointment
            // 
            this.dtCloneAppointment.Location = new System.Drawing.Point(80, 26);
            this.dtCloneAppointment.Name = "dtCloneAppointment";
            this.dtCloneAppointment.Size = new System.Drawing.Size(136, 20);
            this.dtCloneAppointment.TabIndex = 29;
            this.dtCloneAppointment.ValueChanged += new System.EventHandler(this.dtCloneAppointment_ValueChanged);
            // 
            // cboSlots
            // 
            this.cboSlots.FormattingEnabled = true;
            this.cboSlots.Location = new System.Drawing.Point(303, 24);
            this.cboSlots.Name = "cboSlots";
            this.cboSlots.Size = new System.Drawing.Size(121, 21);
            this.cboSlots.TabIndex = 28;
            this.cboSlots.SelectedIndexChanged += new System.EventHandler(this.cboSlots_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(238, 29);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(59, 13);
            this.label25.TabIndex = 27;
            this.label25.Text = "Time Slots:";
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(152, 53);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(40, 16);
            this.label24.TabIndex = 26;
            this.label24.Text = "Clinic:";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtClinic
            // 
            this.txtClinic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.txtClinic.Location = new System.Drawing.Point(200, 53);
            this.txtClinic.Name = "txtClinic";
            this.txtClinic.Size = new System.Drawing.Size(233, 16);
            this.txtClinic.TabIndex = 25;
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Location = new System.Drawing.Point(80, 123);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 60);
            this.textBox1.TabIndex = 24;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(4, 131);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 16);
            this.label20.TabIndex = 23;
            this.label20.Text = "Notes:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(16, 53);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 16);
            this.label22.TabIndex = 21;
            this.label22.Text = "Duration:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(8, 29);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(64, 16);
            this.label23.TabIndex = 20;
            this.label23.Text = "Start Time:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPatientInfo
            // 
            this.tabPatientInfo.Controls.Add(this.groupBox2);
            this.tabPatientInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPatientInfo.Name = "tabPatientInfo";
            this.tabPatientInfo.Size = new System.Drawing.Size(467, 498);
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
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(19, 142);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(69, 16);
            this.label16.TabIndex = 27;
            this.label16.Text = "Cell/Mobile:";
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
            this.panel1.Controls.Add(this.chkPrint);
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 484);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(475, 40);
            this.panel1.TabIndex = 1;
            // 
            // chkPrint
            // 
            this.chkPrint.AutoSize = true;
            this.chkPrint.Location = new System.Drawing.Point(13, 14);
            this.chkPrint.Name = "chkPrint";
            this.chkPrint.Size = new System.Drawing.Size(139, 17);
            this.chkPrint.TabIndex = 2;
            this.chkPrint.Text = "Print Appointment Letter";
            this.chkPrint.UseVisualStyleBackColor = true;
            this.chkPrint.CheckedChanged += new System.EventHandler(this.chkPrint_CheckedChanged);
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
            // DAppointPage
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(475, 524);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DAppointPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Appointment";
            this.Load += new System.EventHandler(this.DAppointPage_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabAppointment.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabCloneForward.ResumeLayout(false);
            this.grpCloneForward.ResumeLayout(false);
            this.grpCloneForward.PerformLayout();
            this.tabPatientInfo.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.patientApptsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private CGDocumentManager	m_DocManager;

        private ArrayList alResources;

        private bool isCloning = false;

        private DateTime originalAppointment;

		private string			m_sPatientName;
        private Sex             m_enumPatientSex;
		private string			m_sPatientHRN;
		private string			m_sPatientIEN;
		private DateTime		m_dPatientDOB;
		private string			m_sPatientPID;

		private string			m_sCity;
		private string			m_sPhoneHome;
		private string			m_sPhoneOffice;
		private string			m_sState;
		private string			m_sStreet;
		private string			m_sZip;

		private string			m_sNote;
		private	DateTime		m_dStartTime;
        private DateTime        m_dEndTime;
		private int				m_nDuration;
		private string			m_sClinic;

        private string          m_sPhoneCell;
        private string          m_sEmail;
        private string          m_sCountry;
        private int          m_iAccessTypeID;
        private bool _myCodeIsFiringIstheCheckBoxChangedEvent;

		#endregion //fields

		#region

        private void SetDurations(DateTime apptDate)
        {
            DateTime m_dStart = apptDate; // move to 1200
            DateTime m_dEnd = m_dStart.Date.AddHours(23).AddMinutes(59).AddSeconds(59); //move to 235959
            string sSearchInfo = "1|both" + "|";
            ArrayList m_alResources = alResources;
            ArrayList m_alAccessTypes = new ArrayList();
            DataTable m_availTable = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_alResources, m_dStart, m_dEnd, m_alAccessTypes, ScheduleType.Resource, sSearchInfo);
            if (m_availTable.Rows.Count > 0)
            {
                foreach (DataRow row in m_availTable.Rows)
                {
                    double minPerSlot = (row.Field<DateTime>("END_TIME") - row.Field<DateTime>("START_TIME")).TotalMinutes / row.Field<int>("SLOTS");
                    int totalSlots = row.Field<int>("SLOTS");
                    double duration = 0;
                    for (int i = 0; i < totalSlots; i++)
                    {
                        duration = duration + minPerSlot;
                        cboDuration.Items.Add(duration);
                    }
                }
            }
            else
            {
                CGView v = this.DocManager.GetViewByResource(m_alResources);
                int scale = v.CGrid.TimeScale;
                //cboDuration.Items.Clear();
                cboDuration.Items.Add((double)scale);
                //cboDuration.SelectedIndex = 0;
            }
        }

        private void GetAppointmentSlots(DateTime apptDate)
        {
            DateTime m_dStart = apptDate; // move to 1200
            DateTime m_dEnd = m_dStart.Date.AddHours(23).AddMinutes(59).AddSeconds(59); //move to 235959
            string sSearchInfo = "1|both" + "|";
            ArrayList m_alResources = alResources;
            ArrayList m_alAccessTypes = new ArrayList();
            DataTable m_availTable = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_alResources, m_dStart, m_dEnd, m_alAccessTypes, ScheduleType.Resource, sSearchInfo);
            DataTable m_apptTable = CGSchedLib.CreateAppointmentSchedule(m_DocManager, m_alResources, m_dStart, m_dEnd);
            cboSlots.Items.Clear();
            if (m_availTable.Rows.Count>0)
            {
                foreach (DataRow row in m_availTable.Rows)
                {
                    string resource = row.Field<string>("RESOURCE");
                    DateTime start_time = row.Field<DateTime>("START_TIME");
                    DateTime end_time = row.Field<DateTime>("END_TIME");
                    double minPerSlot = (row.Field<DateTime>("END_TIME") - row.Field<DateTime>("START_TIME")).TotalMinutes / row.Field<int>("SLOTS");
                    double duration = minPerSlot;
                    if (cboDuration.SelectedIndex >= 0)
                    {
                        duration = (double)cboDuration.SelectedItem;
                    }
                    int totalSlots = row.Field<int>("SLOTS");
                    DateTime aStartTime = start_time;
                    DateTime aEndTime = start_time.AddMinutes(duration);
                    for (int i = 0; i < totalSlots; i++)
                    {
                        bool flag = true;
                        foreach (DataRow appt in m_apptTable.Rows)
                        {
                            if (resource == appt.Field<string>("RESOURCENAME"))
                            {
                                DateTime apptStartTime = appt.Field<DateTime>("START_TIME");
                                DateTime apptEndTime = appt.Field<DateTime>("END_TIME");
                                DateTimeRange dRange1 = new DateTimeRange();
                                dRange1.Start = aStartTime;
                                dRange1.End = aEndTime;
                                DateTimeRange dRange2 = new DateTimeRange();
                                dRange2.Start = apptStartTime;
                                dRange2.End = apptEndTime.AddSeconds(-1);
                                if (dRange1.Intersects(dRange2))
                                {
                                    flag = false;
                                }
                                /*if(aStartTime.Ticks >= apptStartTime.Ticks && aStartTime.Ticks <= apptEndTime.AddSeconds(-1).Ticks)
                                {
                                    flag = false;
                                }
                                if (aEndTime.Ticks <= apptStartTime.Ticks && aEndTime.Ticks >= apptEndTime.AddSeconds(-1).Ticks)
                                {
                                    flag = false;
                                }*/
                            }
                        }
                        if (flag)
                        {
                            AppointmentComboboxItem item = new AppointmentComboboxItem();
                            item.Value = aStartTime;
                            item.Text = aStartTime.TimeOfDay.ToString();
                            cboSlots.Items.Add(item);
                        }
                        aStartTime = aStartTime.AddMinutes(minPerSlot);
                        aEndTime = aStartTime.AddMinutes(duration);
                    }
                }
            }
            else
            {
                CGView v= this.DocManager.GetViewByResource(m_alResources);
                int scale = v.CGrid.TimeScale;
                cboSlots.Items.Clear();                
                DateTime dStartTime = apptDate.Date;
                while (dStartTime.Day == apptDate.Date.Day)
                {
                    AppointmentComboboxItem item = new AppointmentComboboxItem();
                    item.Value = dStartTime;
                    item.Text = dStartTime.TimeOfDay.ToString();
                    cboSlots.Items.Add(item);
                    dStartTime = dStartTime.AddMinutes((double)scale);
                }
            }            
        }

        public void HideCloneForwardTab() {
            tabControl1.TabPages.Remove(tabCloneForward);
        }

        public void SetCloneForwardable(ArrayList alResources, CGAppointment a) {
            originalAppointment = a.StartTime;
            //grpCloneForward.Visible = true;
            isCloning = true;
            //grpCloneForward.Top = groupBox4.Top;
            //groupBox4.Top = groupBox4.Top + grpCloneForward.Height + 10;
            txtNote.Enabled = false;
            //this.Height = this.Height + grpCloneForward.Height + 20;
            this.alResources = alResources;
            cboSlots.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDuration.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDuration.Items.Clear();
            GetAppointmentSlots(dtCloneAppointment.Value);            
            SetDurations(dtCloneAppointment.Value);
            txtClinic.Text = alResources[0].ToString();
            m_sClinic = txtClinic.Text;
            if (cboSlots.Items.Count > 0)
            {
                bool flag = true;
                cboDuration.SelectedIndex = 0;
                foreach (AppointmentComboboxItem item in cboSlots.Items)
                {
                    if (originalAppointment.TimeOfDay.ToString() == item.Text)
                    {
                        flag = false;
                        cboSlots.SelectedIndex = cboSlots.Items.IndexOf(item);
                    }
                }
                if (flag)
                {
                    cboSlots.SelectedIndex = 0;
                }                
                m_dStartTime = ((AppointmentComboboxItem)cboSlots.SelectedItem).Value;
                m_dEndTime = m_dStartTime.AddMinutes((double)cboDuration.SelectedItem);
            }
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(tabCloneForward);
        }
        
        public void InitializePage(CGAppointment a)
		{
			InitializePage(a.PatientID.ToString(), a.StartTime, a.EndTime, "", a.Note, a.AccessTypeID);
		}

		public void InitializePage(string sPatientIEN, DateTime dStart, DateTime dEnd, string sClinic, string sNote, int iAccessTypeID)
		{
			m_dStartTime = dStart;
            m_dEndTime = dEnd;
			m_nDuration = (int)(dEnd - dStart).TotalMinutes;
            m_iAccessTypeID = iAccessTypeID;
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
                this.m_enumPatientSex = r["SEX"].ToString() == "MALE" ? Sex.Male : Sex.Female;
				this.m_sPatientHRN = r["HRN"].ToString();
				this.m_sPatientIEN = r["IEN"].ToString();
                this.m_sPatientPID = r["PID"].ToString();
				this.m_dPatientDOB = (DateTime) r["DOB"];
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

                _myCodeIsFiringIstheCheckBoxChangedEvent = true;
                chkPrint.Checked = CGDocumentManager.Current.UserPreferences.PrintAppointmentSlipAutomacially;
                _myCodeIsFiringIstheCheckBoxChangedEvent = false;
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
				txtDOB.Text = this.m_dPatientDOB.ToShortDateString();
				txtHRN.Text = this.m_sPatientHRN;
				txtNote.Text = this.m_sNote;
				txtPatientName.Text = m_sPatientName;
                txtSex.Text = m_enumPatientSex.ToString();
				txtPhoneHome.Text = this.m_sPhoneHome;
				txtPhoneOffice.Text = this.m_sPhoneOffice;
				txtPID.Text = this.m_sPatientPID;
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
            if (isCloning)
            {
                if (cboSlots.Items.Count <=0)
                {
                    MessageBox.Show("Please select the Appointment Slot before submitting the form");
                    return;
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
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

        public bool PrintAppointmentSlip
        {
            get { return chkPrint.Checked; }
        }

        public CGAppointment Appointment
        {
            get
            {
                Patient pt = new Patient
                {
                    DFN = Int32.Parse(m_sPatientIEN),
                    Name = m_sPatientName,
                    Sex = m_enumPatientSex,
                    DOB = m_dPatientDOB,
                    ID = m_sPatientPID,
                    HRN = m_sPatientHRN,
                    Appointments = null, //for now
                    Street = m_sStreet,
                    City = m_sCity,
                    State = m_sState,
                    Zip = m_sZip,
                    Country = m_sCountry,
                    Email = m_sEmail,
                    HomePhone = m_sPhoneHome,
                    WorkPHone = m_sPhoneOffice,
                    CellPhone = m_sPhoneCell
                };

                CGAppointment appt = new CGAppointment()
                {
                    PatientID = Convert.ToInt32(m_sPatientIEN),
                    PatientName = m_sPatientName,
                    StartTime = m_dStartTime,
                    EndTime = m_dEndTime,
                    Resource = m_sClinic,
                    Note = m_sNote,
                    HealthRecordNumber = m_sPatientHRN,
                    AccessTypeID = m_iAccessTypeID,
                    Patient = pt
                };

                return appt;
            }
        }
		#endregion //Properties

        /// <summary>
        /// Save Print Slip preference in UserPreferences object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (_myCodeIsFiringIstheCheckBoxChangedEvent) return;

            CGDocumentManager.Current.UserPreferences.PrintAppointmentSlipAutomacially = chkPrint.Checked;
        }

        private void DAppointPage_Load(object sender, EventArgs e)
        {

        }

        private void dtCloneAppointment_ValueChanged(object sender, EventArgs e)
        {
            cboDuration.Items.Clear();
            GetAppointmentSlots(dtCloneAppointment.Value);
            SetDurations(dtCloneAppointment.Value);
            txtClinic.Text = alResources[0].ToString();
            m_sClinic = txtClinic.Text;
            if (cboSlots.Items.Count > 0)
            {
                cboDuration.SelectedIndex = 0;
                cboSlots.SelectedIndex = cboSlots.FindStringExact(originalAppointment.TimeOfDay.ToString());
                m_dStartTime = ((AppointmentComboboxItem)cboSlots.SelectedItem).Value;
                m_dEndTime = m_dStartTime.AddMinutes((double)cboDuration.SelectedItem);
            }
        }

        private void rdbFUCheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // This is the correct control.
                RadioButton rb = (RadioButton)sender;
                dtCloneAppointment.Value = originalAppointment.AddDays(Convert.ToDouble(rb.Text));
                AppointmentComboboxItem item = new AppointmentComboboxItem();
                item.Value = dtCloneAppointment.Value;
                item.Text = dtCloneAppointment.Value.TimeOfDay.ToString();
                if (cboSlots.Items.Count > 0)
                {
                    cboSlots.SelectedIndex = cboSlots.FindStringExact(dtCloneAppointment.Value.TimeOfDay.ToString());
                }
            }
        }

        private void cboDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetAppointmentSlots(dtCloneAppointment.Value);
            if (cboSlots.Items.Count >0)
            {
                cboSlots.SelectedIndex = 0;
                m_dStartTime = ((AppointmentComboboxItem)cboSlots.SelectedItem).Value;
                m_dEndTime = m_dStartTime.AddMinutes((double)cboDuration.SelectedItem);
            }
        }

        private void txtFUOther_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtFUOther.Text)>0)
            {
                if (Convert.ToInt32(txtFUOther.Text) <= 365)
                {
                    dtCloneAppointment.Value = originalAppointment.AddDays(Convert.ToInt32(txtFUOther.Text));
                    AppointmentComboboxItem item = new AppointmentComboboxItem();
                    item.Value = dtCloneAppointment.Value;
                    item.Text = dtCloneAppointment.Value.TimeOfDay.ToString();
                    if (cboSlots.Items.Count > 0)
                    {
                        cboSlots.SelectedIndex = cboSlots.FindStringExact(dtCloneAppointment.Value.TimeOfDay.ToString());
                    }                    
                }
                else
                {
                    MessageBox.Show("Please enter number less then 365.");
                }
            }
            else
            {
                MessageBox.Show("Please enter number of days before updating.");
            }
        }

        private void cboSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_dStartTime = ((AppointmentComboboxItem)cboSlots.SelectedItem).Value;
            m_dEndTime = m_dStartTime.AddMinutes((double)cboDuration.SelectedItem);
        }
    } //end Class

    public class AppointmentComboboxItem
    {
        public string Text { get; set; }
        public DateTime Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class DateTimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Intersects(DateTimeRange test)
        {
            
            if (this.Start == this.End || test.Start == test.End)
                return false; // No actual date range

            if (this.Start == test.Start || this.End == test.End)
                return true; // If any set is the same time, then by default there must be some overlap. 

            if (this.Start < test.Start)
            {
                if (this.End > test.Start && this.End < test.End)
                    return true; // Condition 1

                if (this.End > test.End)
                    return true; // Condition 3
            }
            else
            {
                if (test.End > this.Start && test.End < this.End)
                    return true; // Condition 2

                if (test.End > this.End)
                    return true; // Condition 4
            }

            return false;
        }
    }
}
