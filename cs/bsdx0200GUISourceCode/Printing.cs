using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Class that encapsulates printing functions in Clinical Scheduling
    /// </summary>
    public static class Printing
    {
        /// <summary>
        /// Print Appointments
        /// </summary>
        /// <param name="ds">Strongly Typed DataSet contains Resources and Appointments</param>
        /// <param name="e">PrintPageEventArgs from PrintDocument Print handler</param>
        /// <param name="beg">Begin Datetime to print appointments</param>
        /// <param name="end">End Datetime to print appointments</param
        /// <param name="resourceToPrint">The resouce to print</param>
        /// <param name="apptPrinting">Current Appointment printing</param>
        /// <param name="pageNumber">Current page number</param>
        /// <remarks>beg and end have no effect on operation--they are there for documentation for user only</remarks>
        public static void PrintAppointments(dsPatientApptDisplay2 ds, PrintPageEventArgs e, DateTime beg, DateTime end,
            int resourceToPrint, ref int apptPrinting, int pageNumber)
        {
            Graphics g = e.Graphics;
            //g.PageUnit = GraphicsUnit.Millimeter;
            //SizeF szVCB = g.VisibleClipBounds.Size;
            //PointF[] ptszVCB = {new PointF(szVCB.Width,szVCB.Height)};
            //g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Device, ptszVCB);
            //Create Fonts
            Font f8 = new Font(FontFamily.GenericSerif, 8);
            Font f10 = new Font(FontFamily.GenericSerif, 10);
            Font f14bold = new Font(FontFamily.GenericSerif, 14, FontStyle.Bold);
            
            //Center Alignment for some stuff
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            
            //Header
            g.DrawString("Confidential Patient Information", f8, Brushes.Black, e.PageBounds, sf);
            
            //Footer
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Far;
            g.DrawString("Page " + pageNumber, f8, Brushes.Black, e.PageBounds, sf);

            //Typical manipulable print area
            Rectangle printArea = e.MarginBounds;
            
            //resource we want to print
            dsPatientApptDisplay2.BSDXResourceRow r = ds.BSDXResource[resourceToPrint];
            
            //header
            string toprint;
            if (beg == end) toprint = "Appointments for " + r.RESOURCE_NAME + " on " + beg.ToLongDateString();
            else toprint = "Appointments for " + r.RESOURCE_NAME + " from " + beg.ToShortDateString() + " to "
                + end.ToShortDateString();
            g.DrawString(toprint, f14bold, Brushes.Black, printArea);
            
            //Move print area down
            printArea.Height -= (int)f14bold.GetHeight();
            printArea.Y += (int)f14bold.GetHeight();

            //Draw Line
            g.DrawLine(new Pen(Brushes.Black, 0), printArea.X, printArea.Y, printArea.X + printArea.Width, printArea.Y);
            
            //Move print area down
            printArea.Y += 5; 
            printArea.Height -= 5;
            
            System.Data.DataRow[] appts = r.GetChildRows(ds.Relations[0]); //ds has only one relation 

            StringFormat sf2 = new StringFormat();                 //sf to hold tab stops
            sf2.SetTabStops(50, new float[] { 100, 250, 25 });     

            //appt printed starts at zero
            while (apptPrinting < appts.Length)
            {
                dsPatientApptDisplay2.PatientApptsRow a = (dsPatientApptDisplay2.PatientApptsRow)appts[apptPrinting];
                
                StringBuilder apptPrintStr = new StringBuilder(200); 
                apptPrintStr.AppendLine(a.ApptDate.ToString() + "\t" + a.Name + "(" + a.Sex + ")" + "\t" + "DOB: " + a.DOB.ToString("dd-MMM-yyyy") + "\t" + "ID: " + a.HRN);
                apptPrintStr.AppendLine("P: " + a.HOMEPHONE + "\t" + "Address: " + a.STREET + ", " + a.CITY + ", " + a.STATE + " " + a.ZIP);
                apptPrintStr.AppendLine("Note: " + a.NOTE);
                apptPrintStr.AppendLine("Appointment made by " + a.APPT_MADE_BY + " on " + a.DATE_APPT_MADE);

                int printedApptHeight = (int)g.MeasureString(apptPrintStr.ToString(), f10, printArea.Width).Height;
                if (printedApptHeight > printArea.Height) // too much to print -- move to next page
                    // but don't increment the appointment to print since we haven't printed it yet.
                    // i.e. apptPrinting stays the same.
                {
                    e.HasMorePages = true;
                    break;
                }
   
                //otherwise print it
                g.DrawString(apptPrintStr.ToString(), f10, Brushes.Black, printArea, sf2);
                
                //Move print area down
                printArea.Y += printedApptHeight + 3;
                printArea.Height -= printedApptHeight + 3;

                //Draw a divider line
                Point pt1 = new Point((int)(printArea.X + printArea.Width * 0.25), printArea.Y);
                Point pt2 = new Point((int)(printArea.X + printArea.Width * 0.75), printArea.Y);
                g.DrawLine(Pens.Gray, pt1, pt2);

                //move down, again
                printArea.Y += 3;
                printArea.Height -= 3;

                //go to the next appointment
                apptPrinting++;
            }
        }

        /// <summary>
        /// Print Letter to be given or mailed to the patient
        /// </summary>
        /// <param name="ptrow">Strongly typed PatientApptsRow to pass (just one ApptRow)</param>
        /// <param name="e">You know what that is</param>
        /// <param name="letter">Contains letter string</param>
        /// <param name="title">Title of the letter</param>
        public static void PrintReminderLetter(dsPatientApptDisplay2.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
        {

            Rectangle printArea = e.MarginBounds;
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; //for title
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 12);
            g.DrawString(title, fTitle, Brushes.Black, printArea, sf); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;
            
            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // write appointment date
            string str = "Appointment Date: " + ptRow.ApptDate + "\n\n";
            g.DrawString(str, fBody, Brushes.Black, printArea);

            // move down
            int strHeight = (int)g.MeasureString(str, fBody, printArea.Width).Height;
            printArea.Y += strHeight;
            printArea.Height -= strHeight;

            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea);

            //print Address in lower left corner for windowed envolopes
            printArea.Location = new Point(e.MarginBounds.X, (int)(e.PageBounds.Height * 0.66));
            printArea.Height = (int)(e.MarginBounds.Height * 0.20);
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            StringBuilder address = new StringBuilder(100);
            address.AppendLine(ptRow.Name);
            address.AppendLine(ptRow.STREET);
            address.AppendLine(ptRow.CITY + ", " + ptRow.STATE + " " + ptRow.ZIP);
            g.DrawString(address.ToString(), fBody, Brushes.Black, printArea, sf);
        }

        /// <summary>
        /// Cancellation Letter to be given or mailed to the patient
        /// </summary>
        /// <param name="ptRow">Strongly typed PatientApptsRow to pass (just one ApptRow)</param>
        /// <param name="e">You know what that is</param>
        /// <param name="letter">Contains letter string</param>
        /// <param name="title">Title of the letter</param>
        public static void PrintCancelLetter(dsRebookAppts.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
        {
            Rectangle printArea = e.MarginBounds;
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; //for title
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 12);
            g.DrawString(title, fTitle, Brushes.Black, printArea, sf); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // write appointment date
            string str = "Appointment Date: " + ptRow.OldApptDate + "\n\n";
            g.DrawString(str, fBody, Brushes.Black, printArea);

            // move down
            int strHeight = (int)g.MeasureString(str, fBody, printArea.Width).Height;
            printArea.Y += strHeight;
            printArea.Height -= strHeight;

            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea);

            //print Address in lower left corner for windowed envolopes
            printArea.Location = new Point(e.MarginBounds.X, (int)(e.PageBounds.Height * 0.66));
            printArea.Height = (int)(e.MarginBounds.Height * 0.20);
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            StringBuilder address = new StringBuilder(100);
            address.AppendLine(ptRow.Name);
            address.AppendLine(ptRow.STREET);
            address.AppendLine(ptRow.CITY + ", " + ptRow.STATE + " " + ptRow.ZIP);
            g.DrawString(address.ToString(), fBody, Brushes.Black, printArea, sf);
        }

        /// <summary>
        /// Print rebook letters. Prints old and new appointments dates then the missive.
        /// </summary>
        /// <param name="ptRow">Strongly typed appointment row</param>
        /// <param name="e">etc</param>
        /// <param name="letter">Text of the letter to print</param>
        /// <param name="title">Title to print at the top of the letter</param>
        public static void PrintRebookLetter(dsRebookAppts.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
        {
            Rectangle printArea = e.MarginBounds;
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; //for title
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 12);
            g.DrawString(title, fTitle, Brushes.Black, printArea, sf); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // write old and new appointment dates
            string str = "Old Appointment Date:\t\t" + ptRow.OldApptDate + "\n";
            str += "New Appointment Date:\t\t" + ptRow.NewApptDate + "\n\n";
            g.DrawString(str, fBody, Brushes.Black, printArea);

            // move down
            int strHeight = (int)g.MeasureString(str, fBody, printArea.Width).Height;
            printArea.Y += strHeight;
            printArea.Height -= strHeight;

            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea);

            //print Address in lower left corner for windowed envolopes
            printArea.Location = new Point(e.MarginBounds.X, (int)(e.PageBounds.Height * 0.66));
            printArea.Height = (int)(e.MarginBounds.Height * 0.20);
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            StringBuilder address = new StringBuilder(100);
            address.AppendLine(ptRow.Name);
            address.AppendLine(ptRow.STREET);
            address.AppendLine(ptRow.CITY + ", " + ptRow.STATE + " " + ptRow.ZIP);
            g.DrawString(address.ToString(), fBody, Brushes.Black, printArea, sf);

        }

        /// <summary>
        /// Print message on a page; typically that there are no appointments to be found.
        /// Or just a test message to verify that printing works.
        /// </summary>
        /// <param name="msg">The exact string to print.</param>
        /// <param name="e">Print Page event args</param>
        public static void PrintMessage(string msg, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(msg, new Font(FontFamily.GenericSerif, 14),
                Brushes.Black, e.MarginBounds);
        }

        /// <summary>
        /// Print Routing Slip
        /// </summary>
        /// <param name="a">Appointment Data Structure</param>
        /// <param name="title">String to print for title</param>
        /// <param name="e">etc</param>
        public static void PrintRoutingSlip(CGAppointment a, string title, PrintPageEventArgs e)
        {
            Rectangle printArea = e.MarginBounds;
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; //for title
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 18);
            Font fBodyEm = new Font(FontFamily.GenericSerif, 18, FontStyle.Bold);
            g.DrawString(title, fTitle, Brushes.Black, printArea, sf); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            //construct what to print
            string toprint = "Patient: " + a.PatientName + "\t" + "ID: " + a.PatientID;
            toprint += "\n\n";
            toprint += "Appointment Time: " + a.StartTime.ToLongDateString() + " " + a.StartTime.ToLongTimeString();
            toprint += "\n\n";
            toprint += "Notes:\n" + a.Note;

            //print
            g.DrawString(toprint, fBody, Brushes.Black, printArea);

            // Print Date Printed
            //sf to move to bottom center
            StringFormat sf2 = new StringFormat();
            sf2.LineAlignment = StringAlignment.Far;
            sf2.Alignment = StringAlignment.Center;

            //Print
            Font fFooter = new Font(FontFamily.GenericSerif, 8);
            g.DrawString("Printed: " + DateTime.Now, fFooter, Brushes.Black, printArea, sf2);

        }
    }
}
