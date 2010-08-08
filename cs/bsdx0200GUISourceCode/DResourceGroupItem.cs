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
	public class DResourceGroupItem : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboResource;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DResourceGroupItem()
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
            this.pnlPageBottom = new System.Windows.Forms.Panel();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboResource = new System.Windows.Forms.ComboBox();
            this.pnlPageBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPageBottom
            // 
            this.pnlPageBottom.Controls.Add(this.cmdCancel);
            this.pnlPageBottom.Controls.Add(this.cmdOK);
            this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPageBottom.Location = new System.Drawing.Point(0, 112);
            this.pnlPageBottom.Name = "pnlPageBottom";
            this.pnlPageBottom.Size = new System.Drawing.Size(456, 40);
            this.pnlPageBottom.TabIndex = 5;
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
            this.cmdOK.Location = new System.Drawing.Point(296, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(40, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Select Resource:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboResource
            // 
            this.cboResource.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboResource.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboResource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResource.Location = new System.Drawing.Point(144, 40);
            this.cboResource.Name = "cboResource";
            this.cboResource.Size = new System.Drawing.Size(248, 21);
            this.cboResource.TabIndex = 7;
            // 
            // DResourceGroupItem
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(456, 152);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboResource);
            this.Controls.Add(this.pnlPageBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DResourceGroupItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DResourceGroupItem";
            this.pnlPageBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Fields
		int		m_nResourceID;
		string	m_sResourceName;
//		DataSet	m_dtResource;

		#endregion Fields


		public void InitializePage(int nSelectedRID, DataSet dsGlobal)
		{

//			m_dtResource = dsGlobal.Tables["Resources"];

			//Datasource the RESOURCE combo box
			DataTable dtResource = dsGlobal.Tables["Resources"];
			DataView dvResource = new DataView(dtResource);
            dvResource.Sort = "RESOURCE_NAME ASC";

			cboResource.DataSource = dvResource;
			cboResource.DisplayMember = "RESOURCE_NAME";
			cboResource.ValueMember = "RESOURCEID";

			if (nSelectedRID < 0) //then we're in ADD mode
			{
				this.Text = "Add New Resource to Group";
				m_nResourceID = 0;
				m_sResourceName = "";
//				this.cmdOK.Enabled = false;
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
				cboResource.SelectedValue = m_nResourceID;
			}
			else
			{
				m_nResourceID = Convert.ToInt16(cboResource.SelectedValue);
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			UpdateDialogData(false);
		}


		#region Properties


		/// <summary>
		/// Contains the IEN of the Resource in the BSDX_RESOURCE file
		/// </summary>
		public int ResourceID
		{
			get
			{
				return m_nResourceID;
			}
		}

		/// <summary>
		/// Contains the name of the Resource
		/// </summary>
		public string ResourceName
		{
			get
			{
				return m_sResourceName;
			}
		}
		#endregion Properties

	}
}
