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
	/// Summary description for DAccessGroup.
	/// </summary>
	public class DAccessGroup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.TextBox txtAccessGroupName;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DAccessGroup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			m_sAccessGroupName = "";
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
			this.txtAccessGroupName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlPageBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlPageBottom
			// 
			this.pnlPageBottom.Controls.Add(this.cmdCancel);
			this.pnlPageBottom.Controls.Add(this.cmdOK);
			this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlPageBottom.Location = new System.Drawing.Point(0, 158);
			this.pnlPageBottom.Name = "pnlPageBottom";
			this.pnlPageBottom.Size = new System.Drawing.Size(496, 40);
			this.pnlPageBottom.TabIndex = 7;
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
			// txtAccessGroupName
			// 
			this.txtAccessGroupName.Location = new System.Drawing.Point(184, 72);
			this.txtAccessGroupName.Name = "txtAccessGroupName";
			this.txtAccessGroupName.Size = new System.Drawing.Size(256, 20);
			this.txtAccessGroupName.TabIndex = 0;
			this.txtAccessGroupName.Text = "";
			this.txtAccessGroupName.TextChanged += new System.EventHandler(this.txtAccessGroupName_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Access Group Name:";
			// 
			// DAccessGroup
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(496, 198);
			this.Controls.Add(this.txtAccessGroupName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnlPageBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DAccessGroup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Access Group";
			this.pnlPageBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private string	m_sAccessGroupName;

		public void InitializePage(int nSelectedRGID, DataSet dsGlobal)
		{

			if (nSelectedRGID < 0) //then we're in ADD mode
			{
				this.Text = "Add New Access Group";
				this.cmdOK.Enabled = false;
			}
			else //we're in EDIT mode
			{
				this.Text = "Edit Access Group";
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
				txtAccessGroupName.Text = m_sAccessGroupName;
			}
			else
			{
				m_sAccessGroupName = txtAccessGroupName.Text;
			}
		}

		private void txtAccessGroupName_TextChanged(object sender, System.EventArgs e)
		{
			string sText = txtAccessGroupName.Text;
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
		/// Gets the name of the Access Group;
		/// </summary>
		public string AccessGroupName
		{
			get
			{
				return m_sAccessGroupName;
			}
			set
			{
				m_sAccessGroupName = value;
			}
		}
	}
}
