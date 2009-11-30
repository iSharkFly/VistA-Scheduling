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
	/// Summary description for DAccessType.
	/// </summary>
	public class DAccessType : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdSelectColor;
		private System.Windows.Forms.TextBox txtColor;
		private System.Windows.Forms.CheckBox chkInactivate;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private DataTable	m_dtTypes;
		private string		m_sAccessIEN;
		private string		m_sAccessName;
		private string		m_sColor;
		private int			m_nRed;
		private int			m_nGreen;
		private int			m_nBlue;
		private bool		m_bInactive;
		private System.Windows.Forms.TextBox txtAccessType;
		private System.Windows.Forms.Label label1;

		public DAccessType()
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdSelectColor = new System.Windows.Forms.Button();
			this.txtColor = new System.Windows.Forms.TextBox();
			this.chkInactivate = new System.Windows.Forms.CheckBox();
			this.txtAccessType = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.cmdCancel);
			this.panel1.Controls.Add(this.cmdOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 144);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(370, 40);
			this.panel1.TabIndex = 99;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(288, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 4;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(208, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 3;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdSelectColor
			// 
			this.cmdSelectColor.Location = new System.Drawing.Point(8, 48);
			this.cmdSelectColor.Name = "cmdSelectColor";
			this.cmdSelectColor.Size = new System.Drawing.Size(96, 32);
			this.cmdSelectColor.TabIndex = 1;
			this.cmdSelectColor.Text = "Select Display Color";
			this.cmdSelectColor.Click += new System.EventHandler(this.cmdSelectColor_Click);
			// 
			// txtColor
			// 
			this.txtColor.BackColor = System.Drawing.SystemColors.Menu;
			this.txtColor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtColor.ForeColor = System.Drawing.SystemColors.Window;
			this.txtColor.Location = new System.Drawing.Point(128, 48);
			this.txtColor.Multiline = true;
			this.txtColor.Name = "txtColor";
			this.txtColor.ReadOnly = true;
			this.txtColor.Size = new System.Drawing.Size(144, 32);
			this.txtColor.TabIndex = 31;
			this.txtColor.TabStop = false;
			this.txtColor.Text = "";
			// 
			// chkInactivate
			// 
			this.chkInactivate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkInactivate.Location = new System.Drawing.Point(56, 96);
			this.chkInactivate.Name = "chkInactivate";
			this.chkInactivate.Size = new System.Drawing.Size(88, 16);
			this.chkInactivate.TabIndex = 2;
			this.chkInactivate.Text = "Inactive:";
			// 
			// txtAccessType
			// 
			this.txtAccessType.Location = new System.Drawing.Point(128, 16);
			this.txtAccessType.Name = "txtAccessType";
			this.txtAccessType.Size = new System.Drawing.Size(144, 20);
			this.txtAccessType.TabIndex = 0;
			this.txtAccessType.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 36;
			this.label1.Text = "Access Type Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// DAccessType
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(370, 184);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtAccessType);
			this.Controls.Add(this.chkInactivate);
			this.Controls.Add(this.txtColor);
			this.Controls.Add(this.cmdSelectColor);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DAccessType";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Manage Access Types";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdSelectColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog MyDialog = new ColorDialog();
			// Keeps the user from selecting a custom color.
			MyDialog.AllowFullOpen = true ;
			// Allows the user to get help. (The default is false.)
			MyDialog.ShowHelp = true ;
			// Sets the initial color select to the current text color,
			// so that if the user cancels out, the original color is restored.
			MyDialog.Color = txtColor.BackColor ;
			MyDialog.ShowDialog();
			txtColor.BackColor =  MyDialog.Color;

		}

		public void InitializePage(int nRow, DataSet dsGlobal)
		{

			m_dtTypes = dsGlobal.Tables["AccessTypes"];

			if (nRow < 0) //then we're in ADD mode
			{
				m_sAccessIEN = "";
				m_sAccessName = "";
				m_sColor = "Gray";
				Color c = Color.FromKnownColor(KnownColor.AppWorkspace);
				m_nRed = c.R;
				m_nBlue = c.B;
				m_nGreen = c.G;
				m_bInactive = false;

			}
			else //we're in EDIT mode
			{
				string sTemp;
				DataRow dr = m_dtTypes.Rows[nRow];
				m_sAccessIEN = dr["BMXIEN"].ToString();
				m_sAccessName = dr["ACCESS_TYPE_NAME"].ToString();
				m_sColor = dr["DISPLAY_COLOR"].ToString();
				sTemp = dr["RED"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				m_nRed = Convert.ToInt16(sTemp);
				sTemp = dr["GREEN"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				m_nGreen = Convert.ToInt16(sTemp);
				sTemp = dr["BLUE"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				m_nBlue = Convert.ToInt16(sTemp);
				string sInactive = dr["INACTIVE"].ToString();
				m_bInactive = (sInactive == "YES")?true:false;
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
				txtAccessType.Text = m_sAccessName;
				this.chkInactivate.Checked = m_bInactive;
				this.txtColor.BackColor = Color.FromArgb(m_nRed, m_nGreen, m_nBlue);
			}
			else
			{
				m_sAccessName = txtAccessType.Text;
				m_bInactive = this.chkInactivate.Checked;
				m_nRed = this.txtColor.BackColor.R;
				m_nGreen = this.txtColor.BackColor.G;
				m_nBlue = this.txtColor.BackColor.B;
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			this.UpdateDialogData(false);
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
		
		}

		public string AccessIEN
		{
			get
			{
				return m_sAccessIEN;
			}
		}

		public string AccessTypeName
		{
			get
			{
				return m_sAccessName;
			}
		}

		public string DisplayColor
		{
			get
			{
				return m_sColor;
			}
		}

		public bool Inactive
		{
			get
			{
				return m_bInactive;
			}
		}

		public int Red
		{
			get
			{
				return m_nRed;
			}
		}

		public int Green
		{
			get
			{
				return m_nGreen;
			}
		}

		public int Blue
		{
			get
			{
				return m_nBlue;
			}
		}

	}
}
