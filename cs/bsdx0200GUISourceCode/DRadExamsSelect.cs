using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Form which displays exams for User so that the user would pick one of them
    /// for which to make an appointment
    /// </summary>
    public partial class DRadExamsSelect : Form
    {
        //return values
        public int ExamIEN { get; private set; }
        public string ProcedureName { get; private set; }
        public bool PrintAppointmentSlip { get { return chkPrint.Checked; } }
        //end return values

        //private fields
        public bool _myCodeIsFiringIstheCheckBoxChangedEvent = false;
        //private fields

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="_radExams">Strongly Typed Table of Exams</param>
        public DRadExamsSelect(List<RadiologyExam> _radExams)
        {
            InitializeComponent();

            this.lstExams.DataSource = _radExams;
            this.lstExams.SelectionMode = SelectionMode.One;

            //Set Default Checkbox
            _myCodeIsFiringIstheCheckBoxChangedEvent = true;
            chkPrint.Checked = CGDocumentManager.Current.UserPreferences.PrintAppointmentSlipAutomacially;
            _myCodeIsFiringIstheCheckBoxChangedEvent = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SharedFinishLine();
        }

        private void lstExams_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SharedFinishLine();
        }

        private void SharedFinishLine()
        {
            if (lstExams.SelectedIndex < 0)
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            ExamIEN = (lstExams.Items[lstExams.SelectedIndex] as RadiologyExam).IEN;
            ProcedureName = (lstExams.Items[lstExams.SelectedIndex] as RadiologyExam).Procedure;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Save preference for Auto Printing Appointment Slip in the DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (_myCodeIsFiringIstheCheckBoxChangedEvent) return;

            CGDocumentManager.Current.UserPreferences.PrintAppointmentSlipAutomacially = chkPrint.Checked;
        }


    }
}
