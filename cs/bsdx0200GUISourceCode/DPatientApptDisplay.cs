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
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public void InitializeForm(CGDocumentManager docManager, int nPatientID)
		{
            Control UC = new UCPatientAppts(docManager, nPatientID);
            UC.Dock = DockStyle.Fill;
            this.Controls.Add(UC);
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
            // DPatientApptDisplay
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(664, 478);
            this.Name = "DPatientApptDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Appointments";
            this.ResumeLayout(false);

		}
		#endregion
	}
}
