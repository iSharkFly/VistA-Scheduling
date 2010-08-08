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
	/// Summary description for DAccessGroupItem.
	/// </summary>
	public class DAccessGroupItem : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboAccessType;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DAccessGroupItem()
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
            this.cboAccessType = new System.Windows.Forms.ComboBox();
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
            this.cmdOK.Location = new System.Drawing.Point(296, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select Access Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboAccessType
            // 
            this.cboAccessType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboAccessType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboAccessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccessType.Location = new System.Drawing.Point(152, 40);
            this.cboAccessType.Name = "cboAccessType";
            this.cboAccessType.Size = new System.Drawing.Size(248, 21);
            this.cboAccessType.TabIndex = 9;
            // 
            // DAccessGroupItem
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(472, 152);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboAccessType);
            this.Controls.Add(this.pnlPageBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DAccessGroupItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DAccessGroupItem";
            this.pnlPageBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Fields
		int		m_nAccessTypeID;
		string	m_sAccessTypeName;
		#endregion Fields


		public void InitializePage(int nSelectedATID, DataSet dsGlobal)
		{

			//Datasource the ACCESS GROUP combo box
			DataTable dtAccessType = dsGlobal.Tables["AccessTypes"];
			DataView dvAccessType = new DataView(dtAccessType);
            dvAccessType.Sort = "ACCESS_TYPE_NAME ASC";


			cboAccessType.DataSource = dvAccessType;
			cboAccessType.DisplayMember = "ACCESS_TYPE_NAME";
			cboAccessType.ValueMember = "BMXIEN";

			Debug.Assert(nSelectedATID == -1); //We're always in ADD mode

			this.Text = "Add New Access Type to Group";
			m_nAccessTypeID = 0;
			m_sAccessTypeName = "";
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
				cboAccessType.SelectedValue = m_nAccessTypeID;
			}
			else
			{
				m_nAccessTypeID = Convert.ToInt16(cboAccessType.SelectedValue);
				m_sAccessTypeName = cboAccessType.DisplayMember;
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			UpdateDialogData(false);		
		}

		#region Properties

		/// <summary>
		/// Contains the IEN of the AccessType in the BSDX_ACCESS_TYPE file
		/// </summary>
		public int AccessTypeID
		{
			get
			{
				return m_nAccessTypeID;
			}
		}

		/// <summary>
		/// Contains the name of the AccessType
		/// </summary>
		public string AccessTypeName
		{
			get
			{
				return m_sAccessTypeName;
			}
		}
		#endregion Properties


	}
}
