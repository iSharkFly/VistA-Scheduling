using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//using System.Data.OleDb;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DPatientLookup.
	/// </summary>
	public class DPatientLookup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button cmdSearch;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ListView lvwPatients;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DPatientLookup()
		{
			this.m_sPatientName = "";
			this.m_sPatientIEN = "";
			m_nMax = 20;
			InitializeComponent();
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
			this.panel2 = new System.Windows.Forms.Panel();
			this.cmdSearch = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.lvwPatients = new System.Windows.Forms.ListView();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.cmdCancel);
			this.panel1.Controls.Add(this.cmdOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 238);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(384, 40);
			this.panel1.TabIndex = 2;
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(304, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(64, 24);
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "Cancel";
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(224, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(64, 24);
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.cmdSearch);
			this.panel2.Controls.Add(this.txtSearch);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(384, 40);
			this.panel2.TabIndex = 5;
			// 
			// cmdSearch
			// 
			this.cmdSearch.Location = new System.Drawing.Point(288, 8);
			this.cmdSearch.Name = "cmdSearch";
			this.cmdSearch.Size = new System.Drawing.Size(80, 24);
			this.cmdSearch.TabIndex = 5;
			this.cmdSearch.Text = "Search";
			this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Location = new System.Drawing.Point(16, 8);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(216, 20);
			this.txtSearch.TabIndex = 4;
			this.txtSearch.Text = "";
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.lvwPatients);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 40);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(384, 198);
			this.panel3.TabIndex = 6;
			// 
			// lvwPatients
			// 
			this.lvwPatients.AllowColumnReorder = true;
			this.lvwPatients.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvwPatients.FullRowSelect = true;
			this.lvwPatients.GridLines = true;
			this.lvwPatients.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvwPatients.Location = new System.Drawing.Point(0, 0);
			this.lvwPatients.MultiSelect = false;
			this.lvwPatients.Name = "lvwPatients";
			this.lvwPatients.Size = new System.Drawing.Size(384, 198);
			this.lvwPatients.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvwPatients.TabIndex = 5;
			this.lvwPatients.View = System.Windows.Forms.View.Details;
			this.lvwPatients.ItemActivate += new System.EventHandler(this.lvwPatients_ItemActivate);
			// 
			// DPatientLookup
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(384, 278);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "DPatientLookup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Patient";
			this.Load += new System.EventHandler(this.DPatientLookup_Load);
			this.Activated += new System.EventHandler(this.DPatientLookup_Activated);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Fields

		private string				m_sPatientName;
		private	DataTable			m_rsPatients;
		private CGDocumentManager	m_DocManager;
		private int					m_nMax;
		private string				m_sPatientHRN = "";
		private string				m_sPatientIEN;
		private string				m_sPatientDOB;
		private string				m_sPatientSSN;

		#endregion //Fields

		#region Methods

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
			if (this.txtSearch.Text == "")
			{
				this.DialogResult = DialogResult.Cancel;
				return;
			}
			m_sPatientName = this.txtSearch.Text;

			//Update spacebar lookup value
			string sSql;
			sSql = "BSDX SPACEBAR SET^AUPNPAT(^" + m_sPatientIEN;
			DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "SpaceBarValue");
			return;
		}

		private void cmdSearch_Click(object sender, System.EventArgs e)
		{
			try
			{
				string sSearch = txtSearch.Text;
				if (sSearch == "")
					return;
				this.lvwPatients.Clear();
				m_rsPatients = this.GetPatientLookupRS(sSearch, m_nMax);

				if (m_rsPatients.Rows.Count == 0)
				{
					MessageBox.Show("No matching Patients Found.");
					this.txtSearch.Focus();
					return;
				}
			
				if (m_rsPatients.Rows.Count == 1)
				{
					DataRow r = m_rsPatients.Rows[0];
					this.m_sPatientName = r["NAME"].ToString();
					txtSearch.Text = this.m_sPatientName;
					this.m_sPatientHRN = r["HRN"].ToString();
					this.m_sPatientIEN = r["IEN"].ToString();
					this.m_sPatientSSN = r["SSN"].ToString();
					this.cmdOK.Enabled = true;
					this.AcceptButton = cmdOK;
					this.cmdOK.Focus();
				}
				lvwPatients.View = View.Details;

				foreach (DataRow r in m_rsPatients.Rows)
				{
					string sPat = r["NAME"].ToString();
					ListViewItem lv = new ListViewItem(sPat);
					lv.SubItems.Add(r["HRN"].ToString());
					lv.SubItems.Add(r["SSN"].ToString());
					DateTime d = Convert.ToDateTime(r["DOB"]);
					string sFormat = "MM/dd/yyyy";
					string sDob = d.ToString(sFormat);
					lv.SubItems.Add(sDob);
					lv.SubItems.Add((r["IEN"].ToString()));
					lvwPatients.Items.Add(lv);
				}

				lvwPatients.View = View.Details;
				int w =-1;
				lvwPatients.Columns.Add("Name", w, HorizontalAlignment.Left);
				lvwPatients.Columns.Add("HRN", w, HorizontalAlignment.Left);
				lvwPatients.Columns.Add("SSN", w, HorizontalAlignment.Left);
				lvwPatients.Columns.Add("DOB",w, HorizontalAlignment.Left);

				lvwPatients.Columns[0].Width = -1;
				lvwPatients.Columns[1].Width = -1;
				lvwPatients.Columns[2].Width = -1;
				lvwPatients.Columns[3].Width = -1;
				lvwPatients.Select();
				lvwPatients.Items[0].Selected = true;
				lvwPatients.Focus();			
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private DataTable GetPatientLookupRS(string sLookup, int nMax)
		{
			string sSql;
			sSql = "BSDXPatientLookupRS^" + sLookup + "^" + nMax.ToString();
			System.Data.DataTable tb = m_DocManager.RPMSDataTable(sSql, "PatientTable");
			return tb;

		}

		private void lvwPatients_Click(object sender, System.EventArgs e)
		{
		
		}

		private void lvwPatients_ItemActivate(object sender, System.EventArgs e)
		{
			ListViewItem v = lvwPatients.SelectedItems[0]; //only one can be selected
			m_sPatientName = v.SubItems[0].Text;
			m_sPatientIEN = v.SubItems[4].Text;
			m_sPatientHRN = v.SubItems[1].Text;
			m_sPatientDOB = v.SubItems[3].Text;
			m_sPatientSSN = v.SubItems[2].Text;
			this.txtSearch.Text = m_sPatientName;
			this.cmdOK.Enabled = true;
			this.cmdOK.Focus();
		}

		private void txtSearch_TextChanged(object sender, System.EventArgs e)
		{
			this.cmdOK.Enabled = false;
			this.AcceptButton = cmdSearch;
		}

		private void DPatientLookup_Load(object sender, System.EventArgs e)
		{
		}

		private void DPatientLookup_Activated(object sender, System.EventArgs e)
		{
			System.IntPtr pHandle = this.Handle;
			this.cmdOK.Enabled = false;
			this.txtSearch.Focus();			
		
		}

		#endregion //Methods

		#region Properties

		/// <summary>
		/// Gets or sets the name of the selected patient
		/// </summary>
		public string PatientName
		{
			get
			{
				return m_sPatientName;
			}
			set
			{
				m_sPatientName = value;
			}
		}

		public CGDocumentManager DocManager
		{
			get
			{
				return m_DocManager;
			}
			set
			{
				m_DocManager = value;
			}
		}

		/// <summary>
		/// RPMS Internal Entry Number in PATIENT file (DFN)
		/// </summary>
		public string PatientIEN
		{
			get
			{
				return this.m_sPatientIEN;
			}
		}

		/// <summary>
		/// The string representation of the Health Record Number
		/// </summary>
		public string HealthRecordNumber
		{
			get
			{
				return m_sPatientHRN;
			}
			set
			{
				m_sPatientHRN = value;
			}
		}

		#endregion //Properties

	}
}
