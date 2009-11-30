using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DSplash.
	/// </summary>
	public class DSplash : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
        //private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.LinkLabel lnkMail;
		private System.Windows.Forms.Label lblStatus;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DSplash()
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
			this.label1 = new System.Windows.Forms.Label();
            //this.lblVersion = new System.Windows.Forms.Label();
			this.lnkMail = new System.Windows.Forms.LinkLabel();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(448, 40);
			this.label1.TabIndex = 0;
			this.label1.Text = "IHS Clinical Scheduling";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblVersion
			// 
            //this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            //this.lblVersion.Location = new System.Drawing.Point(88, 88);
            //this.lblVersion.Name = "lblVersion";
            //this.lblVersion.Size = new System.Drawing.Size(328, 32);
            //this.lblVersion.TabIndex = 1;
            //this.lblVersion.Text = "Version ";
            //this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lnkMail
			// 
            //this.lnkMail.Location = new System.Drawing.Point(328, 224);
            //this.lnkMail.Name = "lnkMail";
            //this.lnkMail.Size = new System.Drawing.Size(144, 16);
            //this.lnkMail.TabIndex = 2;
            //this.lnkMail.TabStop = true;
            //this.lnkMail.Text = "Horace.Whitt@mail.ihs.gov";
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Location = new System.Drawing.Point(88, 160);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(328, 16);
			this.lblStatus.TabIndex = 3;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// DSplash
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 252);
			this.ControlBox = false;
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.lnkMail);
            //this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "DSplash";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IHS Clinical Scheduling";
			this.Load += new System.EventHandler(this.DSplash_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public void SetStatus(string sStatus)
		{
			this.Status = sStatus;
		}

		private void DSplash_Load(object sender, System.EventArgs e)
		{
			//this.lblVersion.Text = "Version " + Application.ProductVersion;
		}

		#region Properties
		/// <summary>
		/// Gets or sets the value of the Status displayed on the splash screen
		/// </summary>
		public String Status
		{
			get
			{
				return lblStatus.Text;
			}
			set
			{
				lblStatus.Text = value;
			}
		}
		#endregion Properties
	}
}
