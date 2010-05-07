namespace IndianHealthService.ClinicalScheduling
{
    partial class UCPatientAppts
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dgAppts = new System.Windows.Forms.DataGridView();
            this.apptDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clinicDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aPPTMADEBYDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dATEAPPTMADEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nOTEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientApptsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatientApptDisplay2 = new IndianHealthService.ClinicalScheduling.dsPatientApptDisplay2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkPastAppts = new System.Windows.Forms.CheckBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.PrintPtAppts = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.dgAppts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.patientApptsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgAppts
            // 
            this.dgAppts.AllowUserToAddRows = false;
            this.dgAppts.AllowUserToDeleteRows = false;
            this.dgAppts.AutoGenerateColumns = false;
            this.dgAppts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAppts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.apptDateDataGridViewTextBoxColumn,
            this.clinicDataGridViewTextBoxColumn,
            this.aPPTMADEBYDataGridViewTextBoxColumn,
            this.dATEAPPTMADEDataGridViewTextBoxColumn,
            this.nOTEDataGridViewTextBoxColumn});
            this.dgAppts.DataSource = this.patientApptsBindingSource;
            this.dgAppts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgAppts.Location = new System.Drawing.Point(0, 32);
            this.dgAppts.Name = "dgAppts";
            this.dgAppts.ReadOnly = true;
            this.dgAppts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgAppts.ShowEditingIcon = false;
            this.dgAppts.Size = new System.Drawing.Size(544, 171);
            this.dgAppts.TabIndex = 2;
            // 
            // apptDateDataGridViewTextBoxColumn
            // 
            this.apptDateDataGridViewTextBoxColumn.DataPropertyName = "ApptDate";
            this.apptDateDataGridViewTextBoxColumn.HeaderText = "Date";
            this.apptDateDataGridViewTextBoxColumn.Name = "apptDateDataGridViewTextBoxColumn";
            this.apptDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // clinicDataGridViewTextBoxColumn
            // 
            this.clinicDataGridViewTextBoxColumn.DataPropertyName = "Clinic";
            this.clinicDataGridViewTextBoxColumn.HeaderText = "Clinic";
            this.clinicDataGridViewTextBoxColumn.Name = "clinicDataGridViewTextBoxColumn";
            this.clinicDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // aPPTMADEBYDataGridViewTextBoxColumn
            // 
            this.aPPTMADEBYDataGridViewTextBoxColumn.DataPropertyName = "APPT_MADE_BY";
            this.aPPTMADEBYDataGridViewTextBoxColumn.HeaderText = "Made By";
            this.aPPTMADEBYDataGridViewTextBoxColumn.Name = "aPPTMADEBYDataGridViewTextBoxColumn";
            this.aPPTMADEBYDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dATEAPPTMADEDataGridViewTextBoxColumn
            // 
            this.dATEAPPTMADEDataGridViewTextBoxColumn.DataPropertyName = "DATE_APPT_MADE";
            this.dATEAPPTMADEDataGridViewTextBoxColumn.HeaderText = "Made on";
            this.dATEAPPTMADEDataGridViewTextBoxColumn.Name = "dATEAPPTMADEDataGridViewTextBoxColumn";
            this.dATEAPPTMADEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nOTEDataGridViewTextBoxColumn
            // 
            this.nOTEDataGridViewTextBoxColumn.DataPropertyName = "NOTE";
            this.nOTEDataGridViewTextBoxColumn.HeaderText = "Note";
            this.nOTEDataGridViewTextBoxColumn.Name = "nOTEDataGridViewTextBoxColumn";
            this.nOTEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // patientApptsBindingSource
            // 
            this.patientApptsBindingSource.DataMember = "PatientAppts";
            this.patientApptsBindingSource.DataSource = this.dsPatientApptDisplay2BindingSource;
            // 
            // dsPatientApptDisplay2BindingSource
            // 
            this.dsPatientApptDisplay2BindingSource.DataSource = this.dsPatientApptDisplay2;
            this.dsPatientApptDisplay2BindingSource.Position = 0;
            // 
            // dsPatientApptDisplay2
            // 
            this.dsPatientApptDisplay2.DataSetName = "dsPatientApptDisplay2";
            this.dsPatientApptDisplay2.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.chkPastAppts);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 32);
            this.panel1.TabIndex = 3;
            // 
            // chkPastAppts
            // 
            this.chkPastAppts.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkPastAppts.AutoSize = true;
            this.chkPastAppts.Location = new System.Drawing.Point(389, 3);
            this.chkPastAppts.Name = "chkPastAppts";
            this.chkPastAppts.Size = new System.Drawing.Size(152, 17);
            this.chkPastAppts.TabIndex = 0;
            this.chkPastAppts.Text = "Include Past Appointments";
            this.chkPastAppts.UseVisualStyleBackColor = true;
            this.chkPastAppts.CheckedChanged += new System.EventHandler(this.chkPastAppts_CheckedChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(3, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.Document = this.PrintPtAppts;
            this.printDialog1.UseEXDialog = true;
            // 
            // PrintPtAppts
            // 
            this.PrintPtAppts.DocumentName = "Print Patient Appointments";
            this.PrintPtAppts.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.PrintPtAppts_PrintPage);
            // 
            // UCPatientAppts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgAppts);
            this.Controls.Add(this.panel1);
            this.Name = "UCPatientAppts";
            this.Size = new System.Drawing.Size(544, 203);
            ((System.ComponentModel.ISupportInitialize)(this.dgAppts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.patientApptsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptDisplay2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgAppts;
        private System.Windows.Forms.DataGridViewTextBoxColumn apptDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clinicDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn aPPTMADEBYDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dATEAPPTMADEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nOTEDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource patientApptsBindingSource;
        private System.Windows.Forms.BindingSource dsPatientApptDisplay2BindingSource;
        private dsPatientApptDisplay2 dsPatientApptDisplay2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkPastAppts;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument PrintPtAppts;
    }
}
