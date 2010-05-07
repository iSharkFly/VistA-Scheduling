using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using IndianHealthService.BMXNet;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DPatientApptDisplay.
	/// </summary>
	public class DPatientApptDisplay : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkIncludePast;
        private DataGridView dataGridView1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public void InitializeForm(CGDocumentManager docManager, int nPatientID)
		{
			try
			{	
				string sSql = "BSDX PATIENT APPT DISPLAY^" + nPatientID.ToString();
				DataTable dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");
				dataGridView1.DataSource = dtAppt;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public DPatientApptDisplay()
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkIncludePast = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkIncludePast);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(664, 32);
            this.panel1.TabIndex = 1;
            // 
            // chkIncludePast
            // 
            this.chkIncludePast.Location = new System.Drawing.Point(16, 8);
            this.chkIncludePast.Name = "chkIncludePast";
            this.chkIncludePast.Size = new System.Drawing.Size(184, 16);
            this.chkIncludePast.TabIndex = 0;
            this.chkIncludePast.Text = "Include Past Appointments";
            this.chkIncludePast.CheckedChanged += new System.EventHandler(this.chkIncludePast_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(664, 446);
            this.dataGridView1.TabIndex = 2;
            // 
            // DPatientApptDisplay
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(664, 478);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "DPatientApptDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Appointments";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void chkIncludePast_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkIncludePast.Checked == true)
			{
                //this.crViewer1.SelectionFormula = "TRUE"; //MJL 9/11/2007
			}
			else
			{
				//crViewer1.SelectionFormula = "{PatientAppts.ApptDate} >= CurrentDate";
			}
			//crViewer1.RefreshReport();
		}
	}
}
