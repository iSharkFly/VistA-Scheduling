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
        DataTable dtAppt; // Main table
        DataView dvAppt; // Manipulated view of table
        int rowToPrint; // Used in printing
        /// <summary>
        /// Ctor - Creates control and populates data into datagridview
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
            else dvAppt.RowFilter = "ApptDate > " + "#" + DateTime.Today.ToShortDateString() + "#";
        }

        private void chkPastAppts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPastAppts.Checked) SetPastFilter(true);
            else SetPastFilter(false);
        }

        private void PrintPtAppts_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font Serif12Bold = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Font Serif12 = new Font(FontFamily.GenericSerif, 12);
            Font Serif14 = new Font(FontFamily.GenericSerif, 14);
            Rectangle startDrawRectangle = e.MarginBounds;
            int widthPerColumn = e.MarginBounds.Width/dgAppts.Columns.Count;
            int Serif12Height = (int)Serif12.GetHeight();
            
            //Draw Header
            StringFormat sf1 = new StringFormat();
            sf1.Alignment = StringAlignment.Center;
            g.DrawString("Appointment Listing", Serif14, Brushes.Black, startDrawRectangle, sf1);

            startDrawRectangle.Y += (int)Serif14.GetHeight();

            g.DrawString("Confidential Patient Information", Serif12, Brushes.Black, startDrawRectangle, sf1);
            
            startDrawRectangle.Y += Serif12Height * 2;

            //Patient Name + Sex + DOB
            string identifier = "Patient Name: " + dtAppt.Rows[0]["Name"] + "\tSex: " + dtAppt.Rows[0]["Sex"]
                + "\tDate of Birth: " + dtAppt.Rows[0]["DOB"];
            g.DrawString(identifier, Serif12, Brushes.Black, startDrawRectangle);

            startDrawRectangle.Y += Serif12Height * 2;

            foreach (DataGridViewColumn col in dgAppts.Columns)
            {
                g.DrawString(col.HeaderText, Serif12Bold, Brushes.Black, startDrawRectangle);
                startDrawRectangle.X += widthPerColumn;
            }
            startDrawRectangle.Y += Serif12Height;

            // rowToPrint initialized in print button handler. Helps us keep track of which row we
            // are on, so that, just in case we need an extra page to print, we would know where
            // we left off. Royal we of course.
            for ( ; rowToPrint<dgAppts.Rows.Count; rowToPrint++)
            {
                // Post facto statement -- This is starting to look like Mumps...
                // Start drawing a new page if you hit the bottom margin...
                // Y incremented at the bottom of the for loop; but checked here
                // because I need for statement stuff to happen first
                if (startDrawRectangle.Y > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    break;
                } 

                startDrawRectangle.X = e.MarginBounds.X;

                foreach (DataGridViewCell cell in dgAppts.Rows[rowToPrint].Cells)
                {
                    g.DrawString(cell.Value.ToString(), Serif12, Brushes.Black, startDrawRectangle);
                    startDrawRectangle.X += widthPerColumn;
                }

                startDrawRectangle.Y += Serif12Height;

            }             
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            rowToPrint = 0; //reset row to print
            DialogResult res = printDialog1.ShowDialog();
            if (res == DialogResult.OK) this.printDialog1.Document.Print();
        }

        private void PrintPtAppts_QueryPageSettings(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e)
        {
            e.PageSettings.Landscape = true;
        }

    }
}
