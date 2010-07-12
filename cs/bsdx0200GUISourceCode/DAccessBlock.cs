namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Xml;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class DAccessBlock : Form
    {
        private ComboBox cboAccessTypeFilter;
        private Button cmdCancel;
        private Button cmdOK;
        private Container components;
        private Label label1;
        private Label label15;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private Label lblClinic;
        private Label lblEndTime;
        private Label lblStartTime;
        private ListBox lstAccessTypes;
        private DataSet m_dsGlobal;
        private DataTable m_dtTypes;
        private DataView m_dvTypes;
        private CGAppointment m_pAppt;
        private NumericUpDown nudSlots;
        private Panel panel1;
        private Panel panel2;
        private TextBox txtNote;

        public DAccessBlock()
        {
            this.InitializeComponent();
        }

        private void cboAccessTypeFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.cboAccessTypeFilter.Text == "<Show All Access Types>")
            {
                this.LoadListBox("ALL");
            }
            else
            {
                this.LoadListBox("SELECTED");
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.UpdateDialogData(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.cmdCancel = new Button();
            this.cmdOK = new Button();
            this.panel2 = new Panel();
            this.label4 = new Label();
            this.cboAccessTypeFilter = new ComboBox();
            this.lstAccessTypes = new ListBox();
            this.nudSlots = new NumericUpDown();
            this.label6 = new Label();
            this.lblEndTime = new Label();
            this.label7 = new Label();
            this.label2 = new Label();
            this.lblClinic = new Label();
            this.label15 = new Label();
            this.txtNote = new TextBox();
            this.label1 = new Label();
            this.lblStartTime = new Label();
            this.label3 = new Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.nudSlots.BeginInit();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Location = new Point(0, 0x14e);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x192, 40);
            this.panel1.TabIndex = 2;
            this.cmdCancel.Location = new Point(0x120, 8);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new Size(0x40, 0x18);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";

            this.cmdOK.Location = new Point(0xd0, 8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new Size(0x40, 0x18);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new EventHandler(this.cmdOK_Click);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cboAccessTypeFilter);
            this.panel2.Controls.Add(this.lstAccessTypes);
            this.panel2.Controls.Add(this.nudSlots);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.lblEndTime);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lblClinic);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.txtNote);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lblStartTime);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = DockStyle.Fill;
            this.panel2.Location = new Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0x192, 0x14e);
            this.panel2.TabIndex = 3;
            this.label4.Location = new Point(8, 0x88);
            this.label4.Name = "label4";
            this.label4.Size = new Size(80, 0x10);
            this.label4.TabIndex = 0x29;
            this.label4.Text = "Access Group:";
            this.label4.TextAlign = ContentAlignment.MiddleRight;
            this.cboAccessTypeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboAccessTypeFilter.Location = new Point(0x58, 0x88);
            this.cboAccessTypeFilter.Name = "cboAccessTypeFilter";
            this.cboAccessTypeFilter.Size = new Size(0x128, 0x15);
            this.cboAccessTypeFilter.TabIndex = 40;
            this.cboAccessTypeFilter.SelectionChangeCommitted += new EventHandler(this.cboAccessTypeFilter_SelectionChangeCommitted);
            this.lstAccessTypes.Location = new Point(0x58, 0xb0);
            this.lstAccessTypes.Name = "lstAccessTypes";
            this.lstAccessTypes.Size = new Size(0x128, 0x52);
            this.lstAccessTypes.TabIndex = 0x26;
            this.nudSlots.Location = new Point(0x58, 0x38);
            int[] bits = new int[4];
            bits[0] = 0x3e6;
            this.nudSlots.Maximum = new decimal(bits);
            this.nudSlots.Name = "nudSlots";
            this.nudSlots.Size = new Size(40, 20);
            this.nudSlots.TabIndex = 0x25;
            int[] numArray2 = new int[4];
            numArray2[0] = 1;
            this.nudSlots.Value = new decimal(numArray2);
            this.label6.Location = new Point(40, 0x38);
            this.label6.Name = "label6";
            this.label6.Size = new Size(40, 0x10);
            this.label6.TabIndex = 0x24;
            this.label6.Text = "Slots:";
            this.label6.TextAlign = ContentAlignment.MiddleRight;
            this.lblEndTime.BorderStyle = BorderStyle.Fixed3D;
            this.lblEndTime.Location = new Point(0x110, 8);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new Size(0x70, 0x10);
            this.lblEndTime.TabIndex = 0x22;
            this.label7.Location = new Point(0xd0, 8);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x40, 0x10);
            this.label7.TabIndex = 0x21;
            this.label7.Text = "End Time:";
            this.label7.TextAlign = ContentAlignment.MiddleRight;
            this.label2.Location = new Point(0x10, 0xb0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x48, 0x10);
            this.label2.TabIndex = 0x1d;
            this.label2.Text = "Access Type:";
            this.label2.TextAlign = ContentAlignment.MiddleRight;
            this.lblClinic.BorderStyle = BorderStyle.Fixed3D;
            this.lblClinic.Location = new Point(0x58, 0x20);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new Size(0x128, 0x10);
            this.lblClinic.TabIndex = 0x1b;
            this.label15.Location = new Point(40, 0x20);
            this.label15.Name = "label15";
            this.label15.Size = new Size(40, 0x10);
            this.label15.TabIndex = 0x1a;
            this.label15.Text = "Clinic:";
            this.label15.TextAlign = ContentAlignment.MiddleRight;
            this.txtNote.AcceptsReturn = true;
            this.txtNote.Location = new Point(0x58, 80);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new Size(0x128, 0x30);
            this.txtNote.TabIndex = 0x19;
            this.label1.Location = new Point(0x18, 0x58);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x38, 0x10);
            this.label1.TabIndex = 0x18;
            this.label1.Text = "Note:";
            this.label1.TextAlign = ContentAlignment.MiddleRight;
            this.lblStartTime.BorderStyle = BorderStyle.Fixed3D;
            this.lblStartTime.Location = new Point(0x58, 8);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new Size(120, 0x10);
            this.lblStartTime.TabIndex = 0x16;
            this.label3.Location = new Point(0x18, 8);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x40, 0x10);
            this.label3.TabIndex = 20;
            this.label3.Text = "Start Time:";
            this.label3.TextAlign = ContentAlignment.MiddleRight;
            base.AcceptButton = this.cmdOK;
            this.AutoScaleBaseSize = new Size(5, 13);
            base.CancelButton = this.cmdCancel;
            base.ClientSize = new Size(0x192, 0x176);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "DAccessBlock";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Access Block";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.nudSlots.EndInit();
            base.ResumeLayout(false);
        }

        public void InitializePage(CGAppointment pAppt, DataSet dsGlobal)
        {
            this.m_pAppt = new CGAppointment();
            this.m_pAppt.StartTime = pAppt.StartTime;
            this.m_pAppt.EndTime = pAppt.EndTime;
            this.m_pAppt.Resource = pAppt.Resource;
            this.m_pAppt.Note = pAppt.Note;
            this.m_pAppt.Slots = pAppt.Slots;
            this.m_pAppt.AccessTypeID = pAppt.AccessTypeID;
            this.m_pAppt.AccessTypeName = pAppt.AccessTypeName;
            this.m_dsGlobal = dsGlobal;
            this.LoadListBox("ALL");
            DataTable table = dsGlobal.Tables["AccessGroup"];
            DataSet set = new DataSet("dsTemp");
            set.Tables.Add(table.Copy());
            DataTable table2 = set.Tables["AccessGroup"];
            DataView view = new DataView(table2);
            view.AddNew()["ACCESS_GROUP"] = "<Show All Access Types>";
            view.Sort = "ACCESS_GROUP ASC";
            this.cboAccessTypeFilter.DataSource = view;
            this.cboAccessTypeFilter.DisplayMember = "ACCESS_GROUP";
            this.cboAccessTypeFilter.SelectedIndex = this.cboAccessTypeFilter.Items.Count - 1;
            this.cmdCancel.DialogResult = DialogResult.Cancel;
            this.cmdOK.DialogResult = DialogResult.OK;
            this.UpdateDialogData(true);
        }

        public void InitializePage(DateTime dStart, DateTime dEnd, string sClinic, string sNote, DataSet dsGlobal)
        {
            this.m_pAppt = new CGAppointment();
            this.m_pAppt.StartTime = dStart;
            this.m_pAppt.EndTime = dEnd;
            this.m_pAppt.Resource = sClinic;
            this.m_pAppt.Note = sNote;
            this.m_pAppt.Slots = 1;
            this.m_dsGlobal = dsGlobal;
            this.LoadListBox("ALL");
            DataTable table = dsGlobal.Tables["AccessGroup"];
            DataSet set = new DataSet("dsTemp");
            set.Tables.Add(table.Copy());
            DataTable table2 = set.Tables["AccessGroup"];
            DataView view = new DataView(table2);
            view.AddNew()["ACCESS_GROUP"] = "<Show All Access Types>";
            view.Sort = "ACCESS_GROUP ASC";
            this.cboAccessTypeFilter.DataSource = view;
            this.cboAccessTypeFilter.DisplayMember = "ACCESS_GROUP";
            this.cboAccessTypeFilter.SelectedIndex = this.cboAccessTypeFilter.Items.Count - 1;
            this.m_pAppt.AccessTypeID = 0;
            this.cmdCancel.DialogResult = DialogResult.Cancel;
            this.cmdOK.DialogResult = DialogResult.OK;
            this.UpdateDialogData(true);
        }

        public void LoadListBox(string sGroup)
        {
            string str = "";
            if (sGroup == "ALL")
            {
                this.m_dtTypes = this.m_dsGlobal.Tables["AccessTypes"];
                this.m_dvTypes = new DataView(this.m_dtTypes);
                str = "INACTIVE <> 'YES'";
                this.m_dvTypes.RowFilter = str;
                this.lstAccessTypes.DataSource = this.m_dvTypes;
                this.lstAccessTypes.DisplayMember = "ACCESS_TYPE_NAME";
                this.lstAccessTypes.ValueMember = "BMXIEN";
            }
            else
            {
                this.m_dtTypes = this.m_dsGlobal.Tables["AccessGroupType"];
                this.m_dvTypes = new DataView(this.m_dtTypes);
                str = "ACCESS_GROUP = '" + this.cboAccessTypeFilter.Text + "'";
                this.m_dvTypes.RowFilter = str;
                this.lstAccessTypes.DataSource = this.m_dvTypes;
                this.lstAccessTypes.DisplayMember = "ACCESS_TYPE";
                this.lstAccessTypes.ValueMember = "ACCESS_TYPE_ID";
            }
        }

        private void UpdateDialogData(bool b)
        {
            if (b)
            {
                this.lblClinic.Text = this.m_pAppt.Resource;
                this.lblEndTime.Text = this.m_pAppt.EndTime.ToShortDateString() + " " + this.m_pAppt.EndTime.ToShortTimeString();
                this.lblStartTime.Text = this.m_pAppt.StartTime.ToShortDateString() + " " + this.m_pAppt.StartTime.ToShortTimeString();
                this.txtNote.Text = this.m_pAppt.Note;
                this.nudSlots.Value = this.m_pAppt.Slots;
                if (this.m_pAppt.AccessTypeID != 0)
                {
                    this.lstAccessTypes.SelectedValue = this.m_pAppt.AccessTypeID;
                }
            }
            else
            {
                this.m_pAppt.Note = this.txtNote.Text;
                int selectedIndex = this.lstAccessTypes.SelectedIndex;
                string str = this.lstAccessTypes.SelectedValue.ToString();
                str = (str == "") ? "-1" : str;
                int num = Convert.ToInt16(str);
                this.m_pAppt.AccessTypeID = num;
                this.m_pAppt.Slots = Convert.ToInt16(this.nudSlots.Value);
            }
        }

        public int AccessTypeID
        {
            get
            {
                return this.m_pAppt.AccessTypeID;
            }
            set
            {
                this.m_pAppt.AccessTypeID = value;
            }
        }

        public CGAppointment Appointment
        {
            get
            {
                return this.m_pAppt;
            }
            set
            {
                this.m_pAppt = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.m_pAppt.EndTime;
            }
            set
            {
                this.m_pAppt.EndTime = value;
            }
        }

        public string Note
        {
            get
            {
                return this.m_pAppt.Note;
            }
            set
            {
                this.m_pAppt.Note = value;
            }
        }

        public string Resource
        {
            get
            {
                return this.m_pAppt.Resource;
            }
            set
            {
                this.m_pAppt.Resource = value;
            }
        }

        public int Slots
        {
            get
            {
                return this.m_pAppt.Slots;
            }
            set
            {
                this.m_pAppt.Slots = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.m_pAppt.StartTime;
            }
            set
            {
                this.m_pAppt.StartTime = value;
            }
        }
    }
}

