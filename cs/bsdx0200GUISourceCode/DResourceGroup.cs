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
	/// Summary description for DResourceGroup.
	/// </summary>
	public class DResourceGroup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtResourceGroupName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DResourceGroup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			m_sResourceGroupName = "";
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
			this.pnlPageBottom = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtResourceGroupName = new System.Windows.Forms.TextBox();
			this.pnlPageBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlPageBottom
			// 
			this.pnlPageBottom.Controls.Add(this.cmdCancel);
			this.pnlPageBottom.Controls.Add(this.cmdOK);
			this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlPageBottom.Location = new System.Drawing.Point(0, 120);
			this.pnlPageBottom.Name = "pnlPageBottom";
			this.pnlPageBottom.Size = new System.Drawing.Size(472, 40);
			this.pnlPageBottom.TabIndex = 6;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(376, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(56, 24);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Enabled = false;
			this.cmdOK.Location = new System.Drawing.Point(296, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Resource Group Name:";
			// 
			// txtResourceGroupName
			// 
			this.txtResourceGroupName.Location = new System.Drawing.Point(176, 48);
			this.txtResourceGroupName.Name = "txtResourceGroupName";
			this.txtResourceGroupName.Size = new System.Drawing.Size(256, 20);
			this.txtResourceGroupName.TabIndex = 0;
			this.txtResourceGroupName.Text = "";
			this.txtResourceGroupName.TextChanged += new System.EventHandler(this.txtResourceGroupName_TextChanged);
			// 
			// DResourceGroup
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(472, 160);
			this.Controls.Add(this.txtResourceGroupName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnlPageBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DResourceGroup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Resource Group";
			this.pnlPageBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private string	m_sResourceGroupName;

		public void InitializePage(int nSelectedRGID, DataSet dsGlobal)
		{

			if (nSelectedRGID < 0) //then we're in ADD mode
			{
				this.Text = "Add New Resource Group";
				this.cmdOK.Enabled = false;
			}
			else //we're in EDIT mode
			{
				this.Text = "Edit Resource Group";
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
				txtResourceGroupName.Text = m_sResourceGroupName;
			}
			else
			{
				m_sResourceGroupName = txtResourceGroupName.Text;
			}
		}

		private void txtResourceGroupName_TextChanged(object sender, System.EventArgs e)
		{
			string sText = txtResourceGroupName.Text;
			if ((sText.Length > 2) && (sText.Length < 30))
			{
				cmdOK.Enabled = true;
			}
			else
			{
				cmdOK.Enabled = false;
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			UpdateDialogData(false);
		}

		/// <summary>
		/// Gets the name of the resource group
		/// </summary>
		public String ResourceGroupName
		{
			get
			{
				return m_sResourceGroupName;
			}
			set
			{
				m_sResourceGroupName = value;
			}
		}

	}
}
