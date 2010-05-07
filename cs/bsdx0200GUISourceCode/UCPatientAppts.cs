using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// User Control that shows patient's appointments and allows printing
    /// </summary>
    public partial class UCPatientAppts : UserControl
    {
        DataTable dtAppt;
        DataView dvAppt;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="docManager">Document Manager from main context</param>
        /// <param name="nPatientID">Patient IEN</param>
        public UCPatientAppts(CGDocumentManager docManager, int nPatientID)
        {
            InitializeComponent();
            try
            {
                string sSql = "BSDX PATIENT APPT DISPLAY^" + nPatientID.ToString();
                dtAppt = docManager.RPMSDataTable(sSql, "PatientAppts");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            dvAppt = new DataView(dtAppt);
            dvAppt.Sort = "ApptDate ASC";
            SetPastFilter(false);
            dgAppts.DataSource = dvAppt;

        }
        /// <summary>
        /// Sets the filter for the DataView on whether to show past appointments or not
        /// </summary>
        /// <param name="ShowPastAppts">boolean - self explanatory</param>
        void SetPastFilter(bool ShowPastAppts)
        {
            if (ShowPastAppts) dvAppt.RowFilter = "";
            else dvAppt.RowFilter = "ApptDate > " + "#" + DateTime.Today.ToShortDateString() + "#"; ;
        }

        private void chkPastAppts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPastAppts.Checked) SetPastFilter(true);
            else SetPastFilter(false);
        }

        private void PrintPtAppts_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Font font = new Font("Lucida Console", 72))
            {
                g.DrawString("Hello,\nPrinter", font, Brushes.Black, e.MarginBounds);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DialogResult res = printDialog1.ShowDialog();
            if (res == DialogResult.OK) this.printDialog1.Document.Print();
        }

    }
}
