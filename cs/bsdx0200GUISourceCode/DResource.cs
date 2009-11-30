using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//using System.Data.OleDb;
using IndianHealthService.BMXNet;
using System.Diagnostics;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DResource.
	/// </summary>
	public class DResource : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.CheckBox chkInactivate;
		private System.Windows.Forms.GroupBox grpRPMSClinicLink;
		private System.Windows.Forms.Label lblReactivateDate;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lblInactivateDate;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblClinicCode;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblProvider;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lblVisitServiceCategory;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblCreateVisit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboRPMSClinic;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtResourceName;
		private System.Windows.Forms.TabControl tabResources;
		private System.Windows.Forms.TabPage tpRPMSLink;
		private System.Windows.Forms.ComboBox cboTimeInterval;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TabPage tpLetter;
		private System.Windows.Forms.TextBox txtLetter;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Panel pnlOkCancel;
		private System.Windows.Forms.TabPage tpRebookLetter;
		private System.Windows.Forms.TabPage tpCancellationLetter;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtRebookLetter;
		private System.Windows.Forms.TextBox txtCancellationLetter;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DResource()
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
			this.pnlOkCancel = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpDescription = new System.Windows.Forms.GroupBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.tabResources = new System.Windows.Forms.TabControl();
			this.tpRPMSLink = new System.Windows.Forms.TabPage();
			this.label11 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cboTimeInterval = new System.Windows.Forms.ComboBox();
			this.chkInactivate = new System.Windows.Forms.CheckBox();
			this.grpRPMSClinicLink = new System.Windows.Forms.GroupBox();
			this.lblReactivateDate = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lblInactivateDate = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lblClinicCode = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblProvider = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lblVisitServiceCategory = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblCreateVisit = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cboRPMSClinic = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtResourceName = new System.Windows.Forms.TextBox();
			this.tpLetter = new System.Windows.Forms.TabPage();
			this.label9 = new System.Windows.Forms.Label();
			this.txtLetter = new System.Windows.Forms.TextBox();
			this.tpRebookLetter = new System.Windows.Forms.TabPage();
			this.label12 = new System.Windows.Forms.Label();
			this.txtRebookLetter = new System.Windows.Forms.TextBox();
			this.tpCancellationLetter = new System.Windows.Forms.TabPage();
			this.label13 = new System.Windows.Forms.Label();
			this.txtCancellationLetter = new System.Windows.Forms.TextBox();
			this.pnlOkCancel.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpDescription.SuspendLayout();
			this.tabResources.SuspendLayout();
			this.tpRPMSLink.SuspendLayout();
			this.grpRPMSClinicLink.SuspendLayout();
			this.tpLetter.SuspendLayout();
			this.tpRebookLetter.SuspendLayout();
			this.tpCancellationLetter.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlOkCancel
			// 
			this.pnlOkCancel.Controls.Add(this.cmdCancel);
			this.pnlOkCancel.Controls.Add(this.cmdOK);
			this.pnlOkCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlOkCancel.Location = new System.Drawing.Point(0, 424);
			this.pnlOkCancel.Name = "pnlOkCancel";
			this.pnlOkCancel.Size = new System.Drawing.Size(498, 40);
			this.pnlOkCancel.TabIndex = 3;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(416, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 25;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(336, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 20;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpDescription);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDescription.Location = new System.Drawing.Point(0, 344);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(498, 80);
			this.pnlDescription.TabIndex = 46;
			// 
			// grpDescription
			// 
			this.grpDescription.Controls.Add(this.lblDescription);
			this.grpDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpDescription.Location = new System.Drawing.Point(0, 0);
			this.grpDescription.Name = "grpDescription";
			this.grpDescription.Size = new System.Drawing.Size(498, 80);
			this.grpDescription.TabIndex = 0;
			this.grpDescription.TabStop = false;
			this.grpDescription.Text = "Description";
			// 
			// lblDescription
			// 
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescription.Location = new System.Drawing.Point(3, 16);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(492, 61);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = @"Resources may optionally be linked to an RPMS Clinic.  To define the parameters for an RPMS clinic, you must log into RPMS and use the RPMS Scheduling Supervisor's menus.  The Time Interval field controls the increment of time used on the Calendar display.  The Letter Text tab contains the body text of reminder letters for this clinic.";
			// 
			// tabResources
			// 
			this.tabResources.Controls.Add(this.tpRPMSLink);
			this.tabResources.Controls.Add(this.tpLetter);
			this.tabResources.Controls.Add(this.tpRebookLetter);
			this.tabResources.Controls.Add(this.tpCancellationLetter);
			this.tabResources.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabResources.Location = new System.Drawing.Point(0, 0);
			this.tabResources.Name = "tabResources";
			this.tabResources.SelectedIndex = 0;
			this.tabResources.Size = new System.Drawing.Size(498, 344);
			this.tabResources.TabIndex = 10;
			this.tabResources.SelectedIndexChanged += new System.EventHandler(this.tabResources_SelectedIndexChanged);
			// 
			// tpRPMSLink
			// 
			this.tpRPMSLink.Controls.Add(this.label11);
			this.tpRPMSLink.Controls.Add(this.label5);
			this.tpRPMSLink.Controls.Add(this.cboTimeInterval);
			this.tpRPMSLink.Controls.Add(this.chkInactivate);
			this.tpRPMSLink.Controls.Add(this.grpRPMSClinicLink);
			this.tpRPMSLink.Controls.Add(this.label1);
			this.tpRPMSLink.Controls.Add(this.txtResourceName);
			this.tpRPMSLink.Location = new System.Drawing.Point(4, 22);
			this.tpRPMSLink.Name = "tpRPMSLink";
			this.tpRPMSLink.Size = new System.Drawing.Size(490, 318);
			this.tpRPMSLink.TabIndex = 0;
			this.tpRPMSLink.Text = "Resource Link";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(328, 40);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(80, 16);
			this.label11.TabIndex = 52;
			this.label11.Text = "minutes.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(192, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 16);
			this.label5.TabIndex = 51;
			this.label5.Text = "Time Scale:";
			// 
			// cboTimeInterval
			// 
			this.cboTimeInterval.Items.AddRange(new object[] {
																 "5",
																 "10",
																 "15",
																 "20",
																 "30",
																 "60"});
			this.cboTimeInterval.Location = new System.Drawing.Point(256, 38);
			this.cboTimeInterval.MaxDropDownItems = 6;
			this.cboTimeInterval.Name = "cboTimeInterval";
			this.cboTimeInterval.Size = new System.Drawing.Size(64, 21);
			this.cboTimeInterval.TabIndex = 10;
			// 
			// chkInactivate
			// 
			this.chkInactivate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkInactivate.Location = new System.Drawing.Point(80, 40);
			this.chkInactivate.Name = "chkInactivate";
			this.chkInactivate.Size = new System.Drawing.Size(72, 16);
			this.chkInactivate.TabIndex = 5;
			this.chkInactivate.Text = "Inactive:";
			// 
			// grpRPMSClinicLink
			// 
			this.grpRPMSClinicLink.Controls.Add(this.lblReactivateDate);
			this.grpRPMSClinicLink.Controls.Add(this.label10);
			this.grpRPMSClinicLink.Controls.Add(this.lblInactivateDate);
			this.grpRPMSClinicLink.Controls.Add(this.label8);
			this.grpRPMSClinicLink.Controls.Add(this.lblClinicCode);
			this.grpRPMSClinicLink.Controls.Add(this.label6);
			this.grpRPMSClinicLink.Controls.Add(this.lblProvider);
			this.grpRPMSClinicLink.Controls.Add(this.label7);
			this.grpRPMSClinicLink.Controls.Add(this.lblVisitServiceCategory);
			this.grpRPMSClinicLink.Controls.Add(this.label3);
			this.grpRPMSClinicLink.Controls.Add(this.lblCreateVisit);
			this.grpRPMSClinicLink.Controls.Add(this.label2);
			this.grpRPMSClinicLink.Controls.Add(this.label4);
			this.grpRPMSClinicLink.Controls.Add(this.cboRPMSClinic);
			this.grpRPMSClinicLink.Location = new System.Drawing.Point(32, 88);
			this.grpRPMSClinicLink.Name = "grpRPMSClinicLink";
			this.grpRPMSClinicLink.Size = new System.Drawing.Size(384, 208);
			this.grpRPMSClinicLink.TabIndex = 48;
			this.grpRPMSClinicLink.TabStop = false;
			this.grpRPMSClinicLink.Text = "RPMS Clinic Link";
			// 
			// lblReactivateDate
			// 
			this.lblReactivateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblReactivateDate.Location = new System.Drawing.Point(176, 168);
			this.lblReactivateDate.Name = "lblReactivateDate";
			this.lblReactivateDate.Size = new System.Drawing.Size(176, 16);
			this.lblReactivateDate.TabIndex = 57;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(56, 168);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(112, 16);
			this.label10.TabIndex = 56;
			this.label10.Text = "Reactivate Date:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblInactivateDate
			// 
			this.lblInactivateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInactivateDate.Location = new System.Drawing.Point(176, 144);
			this.lblInactivateDate.Name = "lblInactivateDate";
			this.lblInactivateDate.Size = new System.Drawing.Size(176, 16);
			this.lblInactivateDate.TabIndex = 55;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(48, 144);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(120, 16);
			this.label8.TabIndex = 54;
			this.label8.Text = "Inactivate Date:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblClinicCode
			// 
			this.lblClinicCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblClinicCode.Location = new System.Drawing.Point(176, 120);
			this.lblClinicCode.Name = "lblClinicCode";
			this.lblClinicCode.Size = new System.Drawing.Size(176, 16);
			this.lblClinicCode.TabIndex = 53;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(96, 120);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 16);
			this.label6.TabIndex = 52;
			this.label6.Text = "Clinic Code:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblProvider
			// 
			this.lblProvider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblProvider.Location = new System.Drawing.Point(176, 96);
			this.lblProvider.Name = "lblProvider";
			this.lblProvider.Size = new System.Drawing.Size(176, 16);
			this.lblProvider.TabIndex = 51;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(104, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 16);
			this.label7.TabIndex = 50;
			this.label7.Text = "Provider:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblVisitServiceCategory
			// 
			this.lblVisitServiceCategory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblVisitServiceCategory.Location = new System.Drawing.Point(176, 72);
			this.lblVisitServiceCategory.Name = "lblVisitServiceCategory";
			this.lblVisitServiceCategory.Size = new System.Drawing.Size(176, 16);
			this.lblVisitServiceCategory.TabIndex = 49;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 16);
			this.label3.TabIndex = 48;
			this.label3.Text = "Visit Sevice Category:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCreateVisit
			// 
			this.lblCreateVisit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblCreateVisit.Location = new System.Drawing.Point(176, 48);
			this.lblCreateVisit.Name = "lblCreateVisit";
			this.lblCreateVisit.Size = new System.Drawing.Size(40, 16);
			this.lblCreateVisit.TabIndex = 47;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 16);
			this.label2.TabIndex = 46;
			this.label2.Text = "Create Visit at Check-In?";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(32, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 45;
			this.label4.Text = "RPMS Clinic:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cboRPMSClinic
			// 
			this.cboRPMSClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboRPMSClinic.Location = new System.Drawing.Point(112, 16);
			this.cboRPMSClinic.Name = "cboRPMSClinic";
			this.cboRPMSClinic.Size = new System.Drawing.Size(256, 21);
			this.cboRPMSClinic.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(36, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 47;
			this.label1.Text = "Resource Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtResourceName
			// 
			this.txtResourceName.Location = new System.Drawing.Point(140, 11);
			this.txtResourceName.MaxLength = 30;
			this.txtResourceName.Name = "txtResourceName";
			this.txtResourceName.Size = new System.Drawing.Size(256, 20);
			this.txtResourceName.TabIndex = 0;
			this.txtResourceName.Text = "";
			// 
			// tpLetter
			// 
			this.tpLetter.Controls.Add(this.label9);
			this.tpLetter.Controls.Add(this.txtLetter);
			this.tpLetter.Location = new System.Drawing.Point(4, 22);
			this.tpLetter.Name = "tpLetter";
			this.tpLetter.Size = new System.Drawing.Size(490, 318);
			this.tpLetter.TabIndex = 1;
			this.tpLetter.Text = "Reminder Letter";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(32, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(416, 32);
			this.label9.TabIndex = 1;
			this.label9.Text = "Enter the text which will appear on reminder letters sent to patients with appoin" +
				"tments in this clinic.  Use CTRL+Enter to start a new line.";
			// 
			// txtLetter
			// 
			this.txtLetter.Location = new System.Drawing.Point(32, 72);
			this.txtLetter.Multiline = true;
			this.txtLetter.Name = "txtLetter";
			this.txtLetter.Size = new System.Drawing.Size(416, 216);
			this.txtLetter.TabIndex = 20;
			this.txtLetter.Text = "Dear Patient,\r\n\r\nThis letter is to remind you that you have the appointments list" +
				"ed below.\r\n\r\nPlease contact us at 555-1234 if you are unable to keep this appoin" +
				"tment.\r\n\r\nThank you,\r\n\r\nThe Clinic";
			// 
			// tpRebookLetter
			// 
			this.tpRebookLetter.Controls.Add(this.label12);
			this.tpRebookLetter.Controls.Add(this.txtRebookLetter);
			this.tpRebookLetter.Location = new System.Drawing.Point(4, 22);
			this.tpRebookLetter.Name = "tpRebookLetter";
			this.tpRebookLetter.Size = new System.Drawing.Size(490, 318);
			this.tpRebookLetter.TabIndex = 2;
			this.tpRebookLetter.Text = "Rebook Letter";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(37, 27);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(416, 32);
			this.label12.TabIndex = 21;
			this.label12.Text = "Enter the text which will appear on rebook letters sent to patients with appointm" +
				"ents in this clinic.  Use CTRL+Enter to start a new line.";
			// 
			// txtRebookLetter
			// 
			this.txtRebookLetter.Location = new System.Drawing.Point(37, 75);
			this.txtRebookLetter.Multiline = true;
			this.txtRebookLetter.Name = "txtRebookLetter";
			this.txtRebookLetter.Size = new System.Drawing.Size(416, 216);
			this.txtRebookLetter.TabIndex = 22;
			this.txtRebookLetter.Text = "Dear Patient,\r\n\r\nThis letter is to inform you that we have changed the appointmen" +
				"ts listed below.\r\n\r\nPlease contact us at 555-1234 if you are unable to keep your" +
				" appointment.\r\n\r\nThank you,\r\n\r\nThe Clinic";
			// 
			// tpCancellationLetter
			// 
			this.tpCancellationLetter.Controls.Add(this.label13);
			this.tpCancellationLetter.Controls.Add(this.txtCancellationLetter);
			this.tpCancellationLetter.Location = new System.Drawing.Point(4, 22);
			this.tpCancellationLetter.Name = "tpCancellationLetter";
			this.tpCancellationLetter.Size = new System.Drawing.Size(490, 318);
			this.tpCancellationLetter.TabIndex = 3;
			this.tpCancellationLetter.Text = "Cancellation Letter";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(37, 27);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(416, 32);
			this.label13.TabIndex = 21;
			this.label13.Text = "Enter the text which will appear on cancellation letters sent to patients with ap" +
				"pointments in this clinic.  Use CTRL+Enter to start a new line.";
			// 
			// txtCancellationLetter
			// 
			this.txtCancellationLetter.Location = new System.Drawing.Point(37, 75);
			this.txtCancellationLetter.Multiline = true;
			this.txtCancellationLetter.Name = "txtCancellationLetter";
			this.txtCancellationLetter.Size = new System.Drawing.Size(416, 216);
			this.txtCancellationLetter.TabIndex = 22;
			this.txtCancellationLetter.Text = "Dear Patient,\r\n\r\nThis letter is to inform you that the appointments listed below " +
				"have been cancelled.\r\n\r\nPlease contact us at 555-1234 if you have questions abou" +
				"t your appointments.\r\n\r\nThank you,\r\n\r\nThe Clinic";
			// 
			// DResource
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(498, 464);
			this.Controls.Add(this.tabResources);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlOkCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DResource";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Manage Resource";
			this.Activated += new System.EventHandler(this.DResource_Activated);
			this.pnlOkCancel.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpDescription.ResumeLayout(false);
			this.tabResources.ResumeLayout(false);
			this.tpRPMSLink.ResumeLayout(false);
			this.grpRPMSClinicLink.ResumeLayout(false);
			this.tpLetter.ResumeLayout(false);
			this.tpRebookLetter.ResumeLayout(false);
			this.tpCancellationLetter.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private DataTable			m_dtResources;
		private CGResource			m_pResource;
		private DataView			m_dvHospLoc;
		private DataView			m_dvClinicParams;
		
		#endregion Fields

		#region Methods

		public void InitializePage(int nSelectedResourceID, DataSet dsGlobal)
		{

			m_dtResources = dsGlobal.Tables["Resources"];

			//Datasource the HOSPITAL LOCATION combo box
			DataTable dtHospLoc = dsGlobal.Tables["HospitalLocation"];
			m_dvHospLoc = new DataView(dtHospLoc);
			m_dvHospLoc.Sort = "HOSPITAL_LOCATION_ID ASC";
			int nFind = m_dvHospLoc.Find((int) 0);
			if (nFind < 0)
			{
				DataRowView drv = m_dvHospLoc.AddNew();
				drv.BeginEdit();
				drv["HOSPITAL_LOCATION"]="<None>";
				drv["HOSPITAL_LOCATION_ID"]=0;
				drv.EndEdit();
			}

			DataTable dtClinicParams = dsGlobal.Tables["ClinicSetupParameters"];
			m_dvClinicParams = new DataView(dtClinicParams);
//			m_dvClinicParams.Sort = "HOSPITAL_LOCATION_ID ASC";
			m_dvClinicParams.Sort = "HOSPITAL_LOCATION ASC";
			string sFind = "<None>";
//			nFind = m_dvClinicParams.Find((int) 0);
			nFind = m_dvClinicParams.Find((string) sFind);

			if (nFind < 0)
			{
				DataRowView drv = m_dvClinicParams.AddNew();
				drv.BeginEdit();
				drv["HOSPITAL_LOCATION"]="<None>";
				drv["HOSPITAL_LOCATION_ID"]="0";
				drv["CREATE_VISIT"]="";
				drv["VISIT_SERVICE_CATEGORY"]="";
				drv.EndEdit();
			}

//smh       cboRPMSClinic.DataSource = m_dvClinicParams;
            cboRPMSClinic.DataSource = m_dvHospLoc;
			cboRPMSClinic.DisplayMember = "HOSPITAL_LOCATION";
			cboRPMSClinic.ValueMember = "HOSPITAL_LOCATION_ID";
//			cboRPMSClinic.SelectedItem = nFind;
			cboRPMSClinic.SelectedIndex = nFind;

			//Set databindings of the label boxes

//smh		lblCreateVisit.DataBindings.Add("Text", m_dvClinicParams, "CREATE_VISIT");
//smh   	lblClinicCode.DataBindings.Add("Text", m_dvClinicParams, "CLINIC_STOP");
            lblClinicCode.DataBindings.Add("Text", m_dvHospLoc, "STOP_CODE_NUMBER"); //smh
//smh		lblProvider.DataBindings.Add("Text", m_dvClinicParams, "PROVIDER");
            lblProvider.DataBindings.Add("Text", m_dvHospLoc, "DEFAULT_PROVIDER"); //smh
//smh		lblInactivateDate.DataBindings.Add("Text", m_dvClinicParams, "INACTIVATE_DATE");
            lblInactivateDate.DataBindings.Add("Text", m_dvHospLoc, "INACTIVATE_DATE"); //smh
//smh		lblReactivateDate.DataBindings.Add("Text", m_dvClinicParams, "REACTIVATE_DATE");
            lblReactivateDate.DataBindings.Add("Text", m_dvHospLoc, "REACTIVATE_DATE"); //smh
			//create new instance of Resource class
			m_pResource = new CGResource();

			if (nSelectedResourceID < 0) //then we're in ADD mode
			{
				this.Text = "Add New Scheduling Resource";
				m_pResource.ResourceID = 0;
				m_pResource.ResourceName = "";
				m_pResource.Inactive = false;
				m_pResource.HospitalLocationID = 0;
				m_pResource.HospitalLocation = "";
				m_pResource.TimeScale = 15;
			}
			else //we're in EDIT mode
			{
				this.Text = "Edit Scheduling Resource";
//				DataRow dr = m_dtResources.Rows[nSelectedResourceID];

				DataRow dr = m_dtResources.Rows.Find(nSelectedResourceID);
				//TODO: test dr for validity

				string sID = dr["RESOURCEID"].ToString();
				sID = (sID == "")?"0":sID;
				m_pResource.ResourceID = Convert.ToInt16(sID);
				m_pResource.ResourceName = dr["RESOURCE_NAME"].ToString();

				string sInactive = dr["INACTIVE"].ToString();
				m_pResource.Inactive = (sInactive == "1")?true:false;

				sID = dr["HOSPITAL_LOCATION_ID"].ToString();
				sID = (sID == "")?"0":sID;
				m_pResource.HospitalLocationID = Convert.ToInt16(sID);

				if (dr["TIMESCALE"].ToString() != "")
				{
					m_pResource.TimeScale = (int) dr["TIMESCALE"];
				}
				m_pResource.LetterText = dr["LETTER_TEXT"].ToString();
				m_pResource.NoShowLetterText = dr["NO_SHOW_LETTER"].ToString();
				m_pResource.CancellationLetterText = dr["CLINIC_CANCELLATION_LETTER"].ToString();

				dr = dsGlobal.Tables["HospitalLocation"].Rows.Find(m_pResource.HospitalLocationID);
				//TODO: test dr validity
				m_pResource.HospitalLocation = dr["HOSPITAL_LOCATION"].ToString();


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
				txtResourceName.Text = m_pResource.ResourceName;
				chkInactivate.Checked = m_pResource.Inactive;
				cboRPMSClinic.SelectedValue = m_pResource.HospitalLocationID;
				txtLetter.Text = m_pResource.LetterText;
				txtRebookLetter.Text = m_pResource.NoShowLetterText;
				txtCancellationLetter.Text = m_pResource.CancellationLetterText;
				cboTimeInterval.Text = m_pResource.TimeScale.ToString();

			}
			else
			{
				m_pResource.ResourceName = this.txtResourceName.Text;
				m_pResource.Inactive = this.chkInactivate.Checked;
				m_pResource.HospitalLocationID  = Convert.ToInt16(this.cboRPMSClinic.SelectedValue);
				m_pResource.LetterText = txtLetter.Text;
				m_pResource.CancellationLetterText = txtCancellationLetter.Text;
				m_pResource.NoShowLetterText = txtRebookLetter.Text;
				if (cboTimeInterval.Text != "")
					m_pResource.TimeScale = Convert.ToInt16(cboTimeInterval.Text);

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

		private void tabResources_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabResources.SelectedIndex == 0)
			{
				txtResourceName.Focus();
			}
			else
			{
				txtLetter.Focus();
			}
		}

		private void DResource_Activated(object sender, System.EventArgs e)
		{
			txtResourceName.Focus();	
		}
		
		#endregion Methods

		#region Properties
		public bool Inactive
		{
			get
			{
				return m_pResource.Inactive;
			}
			set
			{
				m_pResource.Inactive = value;
			}
		}

		public int HospitalLocationID
		{
			get
			{
				return m_pResource.HospitalLocationID;
			}
			set
			{
				m_pResource.HospitalLocationID = value;
			}
		}

		public string ResourceName
		{
			get
			{
				return m_pResource.ResourceName;
			}
			set
			{
				m_pResource.ResourceName = value;
			}
		}

		public string LetterText
		{
			get
			{
				return m_pResource.LetterText;
			}
			set
			{
				m_pResource.LetterText = value;
			}
		}

		public string NoShowLetterText
		{
			get
			{
				return m_pResource.NoShowLetterText;
			}
			set
			{
				m_pResource.NoShowLetterText = value;
			}
		}

		public string CancellationLetterText
		{
			get
			{
				return m_pResource.CancellationLetterText;
			}
			set
			{
				m_pResource.CancellationLetterText = value;
			}
		}

		public int ResourceID
		{
			get
			{
				return m_pResource.ResourceID;
			}
			set
			{
				m_pResource.ResourceID = value;
			}
		}

		public int TimeScale
		{
			get
			{
				return m_pResource.TimeScale;
			}
			set
			{
				m_pResource.TimeScale = value;
			}
		}

		#endregion Properties


	}
}
