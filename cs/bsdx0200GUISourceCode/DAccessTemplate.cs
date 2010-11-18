using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DAccessTemplate.
	/// </summary>
	public class DAccessTemplate : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.GroupBox grpDescriptionResourceGroup;
		private System.Windows.Forms.Label lblDescriptionResourceGroup;
		private System.Windows.Forms.Button cmdSelectTemplate;
		private System.Windows.Forms.TextBox txtTemplate;
		private System.Windows.Forms.DateTimePicker dtpStartDate;
		private System.Windows.Forms.NumericUpDown udWeeksToApply;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		#region Methods

		public void InitializePage()
		{

			UpdateDialogData(true);
            //this.cmdSelectTemplate.Focus(); // Focus on first button on form
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
				udWeeksToApply.Value = 1;
			}
			else
			{
				//
				m_nWeeksToApply = (int) udWeeksToApply.Value;
				m_dtStart = dtpStartDate.Value;
			}
		}

		public DAccessTemplate()
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

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
            DateTime dtStart = dtpStartDate.Value;
			if 	(dtStart < DateTime.Today)
			{
				MessageBox.Show("Please select a future day.");
				m_bCancelOK = true;
				return;
			}

			if (m_bSelectedFile == false)
			{
				MessageBox.Show("Please select a valid template file.");
				m_bCancelOK = true;
				return;
			}

			if ((this.udWeeksToApply.Value > 52)||(this.udWeeksToApply.Value < 1))
			{
				MessageBox.Show("For the number of weeks to apply the template, please select a number between 1 and 52.");
				m_bCancelOK = true;
				return;
			}
			m_bCancelOK = false;

			//Send the values from the controls to the fields
			this.UpdateDialogData(false);

		}
		
		private void cmdSelectTemplate_Click(object sender, System.EventArgs e)
		{
			//Open the file dialog and pick a file
			m_bSelectedFile = false;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Schedule Template Files (*.bsdxa)|*.bsdxa|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 0 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				m_bSelectedFile = true;
				m_ofDialog = openFileDialog1;
				this.txtTemplate.Text = openFileDialog1.FileName;

			}
		}		


		#endregion Methods

		#region Fields
		private OpenFileDialog	m_ofDialog;
		private DateTime		m_dtStart;
		private int				m_nWeeksToApply;
		private bool			m_bCancelOK = false;
		private bool			m_bSelectedFile = false;

		#endregion Fields

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
            this.cmdSelectTemplate = new System.Windows.Forms.Button();
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.udWeeksToApply = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlPageBottom.SuspendLayout();
            this.pnlDescription.SuspendLayout();
            this.grpDescriptionResourceGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWeeksToApply)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPageBottom
            // 
            this.pnlPageBottom.Controls.Add(this.cmdCancel);
            this.pnlPageBottom.Controls.Add(this.cmdOK);
            this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPageBottom.Location = new System.Drawing.Point(0, 264);
            this.pnlPageBottom.Name = "pnlPageBottom";
            this.pnlPageBottom.Size = new System.Drawing.Size(440, 40);
            this.pnlPageBottom.TabIndex = 7;
            // 
            // cmdCancel
            // 
            this.cmdCancel.CausesValidation = false;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(360, 8);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(56, 24);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "Cancel";
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(280, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.grpDescriptionResourceGroup);
            this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescription.Location = new System.Drawing.Point(0, 184);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Size = new System.Drawing.Size(440, 80);
            this.pnlDescription.TabIndex = 8;
            // 
            // grpDescriptionResourceGroup
            // 
            this.grpDescriptionResourceGroup.Controls.Add(this.lblDescriptionResourceGroup);
            this.grpDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescriptionResourceGroup.Location = new System.Drawing.Point(0, 0);
            this.grpDescriptionResourceGroup.Name = "grpDescriptionResourceGroup";
            this.grpDescriptionResourceGroup.Size = new System.Drawing.Size(440, 80);
            this.grpDescriptionResourceGroup.TabIndex = 1;
            this.grpDescriptionResourceGroup.TabStop = false;
            this.grpDescriptionResourceGroup.Text = "Description";
            // 
            // lblDescriptionResourceGroup
            // 
            this.lblDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescriptionResourceGroup.Location = new System.Drawing.Point(3, 16);
            this.lblDescriptionResourceGroup.Name = "lblDescriptionResourceGroup";
            this.lblDescriptionResourceGroup.Size = new System.Drawing.Size(434, 61);
            this.lblDescriptionResourceGroup.TabIndex = 0;
            this.lblDescriptionResourceGroup.Text = "Use this panel to define an access pattern for future clinic availability.\r\nMAKE " +
                "SURE TO SELECT 5 or 7 day view first depending on which view you used to save th" +
                "e selected Access Template.";
            // 
            // cmdSelectTemplate
            // 
            this.cmdSelectTemplate.Location = new System.Drawing.Point(24, 40);
            this.cmdSelectTemplate.Name = "cmdSelectTemplate";
            this.cmdSelectTemplate.Size = new System.Drawing.Size(136, 32);
            this.cmdSelectTemplate.TabIndex = 1;
            this.cmdSelectTemplate.Text = "Select Access Template";
            this.cmdSelectTemplate.Click += new System.EventHandler(this.cmdSelectTemplate_Click);
            // 
            // txtTemplate
            // 
            this.txtTemplate.Location = new System.Drawing.Point(176, 32);
            this.txtTemplate.Multiline = true;
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.ReadOnly = true;
            this.txtTemplate.Size = new System.Drawing.Size(248, 48);
            this.txtTemplate.TabIndex = 10;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.AllowDrop = true;
            this.dtpStartDate.Checked = false;
            this.dtpStartDate.Location = new System.Drawing.Point(176, 104);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(184, 20);
            this.dtpStartDate.TabIndex = 2;
            // 
            // udWeeksToApply
            // 
            this.udWeeksToApply.Location = new System.Drawing.Point(176, 144);
            this.udWeeksToApply.Maximum = new decimal(new int[] {
            52,
            0,
            0,
            0});
            this.udWeeksToApply.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWeeksToApply.Name = "udWeeksToApply";
            this.udWeeksToApply.Size = new System.Drawing.Size(96, 20);
            this.udWeeksToApply.TabIndex = 3;
            this.udWeeksToApply.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Starting Week:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Number of Weeks to Apply:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DAccessTemplate
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(440, 304);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udWeeksToApply);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.cmdSelectTemplate);
            this.Controls.Add(this.pnlDescription);
            this.Controls.Add(this.pnlPageBottom);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DAccessTemplate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Apply Access Template";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DAccessTemplate_Closing);
            this.pnlPageBottom.ResumeLayout(false);
            this.pnlDescription.ResumeLayout(false);
            this.grpDescriptionResourceGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udWeeksToApply)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		private void DAccessTemplate_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (m_bCancelOK == true)
			{	
				e.Cancel = true;
				m_bCancelOK = false;
			}
			else
			{
				e.Cancel = false;
			}
		}


		#region Properties


		/// <summary>
		/// Returns the open file dialog object
		/// </summary>
		public OpenFileDialog FileDialog
		{
			get
			{
				return m_ofDialog;
			}
		}

		/// <summary>
		/// Sets or returns the start date to apply the template
		/// </summary>
		public DateTime StartDate
		{
			get
			{
				return m_dtStart;
			}
			set
			{
				m_dtStart = value;
			}
		}

		/// <summary>
		/// Sets or returns the number of weeks to apply the template
		/// </summary>
		public int WeeksToApply
		{
			get
			{
				return m_nWeeksToApply;
			}
			set
			{
				m_nWeeksToApply = value;
			}
		}
		#endregion Properties





	}
}
