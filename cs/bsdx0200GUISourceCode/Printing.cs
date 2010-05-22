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
        /// <param name="end">End Datetime to print appointments</param>
        /// <remarks>beg and end have no effect on operation--they are there for documentation for user only</remarks>
        public static void PrintAppointments(dsPatientApptDisplay2 ds, PrintPageEventArgs e, DateTime beg, DateTime end,
            int resourceToPrint, ref int apptPrinted)
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
            
            g.DrawString("Confidential Patient Information", f8, Brushes.Black, e.PageBounds, sf);
            
            //Typical manipulable print area
            Rectangle printArea = e.MarginBounds;
            
            dsPatientApptDisplay2.BSDXResourceRow r = ds.BSDXResource[resourceToPrint];
            
            string toprint;
            if (beg == end) toprint = "Appointments for " + r.RESOURCE_NAME + " on " + beg.ToLongDateString();
            else toprint = "Appointments for " + r.RESOURCE_NAME + " from " + beg.ToShortDateString() + " to "
                + end.ToShortDateString();
            g.DrawString(toprint, f14bold, Brushes.Black, printArea);
            
            printArea.Y += (int)f14bold.GetHeight();
            g.DrawLine(new Pen(Brushes.Black, 0), printArea.X, printArea.Y, printArea.X + printArea.Width, printArea.Y);
            printArea.Y += 5;
            
            System.Data.DataRow[] appts = r.GetChildRows(ds.Relations[0]); //only one relation

            toprint = "";
            StringFormat sf2 = new StringFormat();
            sf2.SetTabStops(50, new float[] { 100, 200, 200 });

            foreach (dsPatientApptDisplay2.PatientApptsRow a in appts)
            {
                toprint += a.ApptDate.ToString() + "\t" + a.Name +"(" + a.Sex + ")" + "\t" + "DOB: " + a.DOB.ToString("dd-MMM-yyyy") + "\t" + "ID: " + a.HRN;
                toprint += "\n";
                toprint += "Home Phone: " + a.HOMEPHONE + "\t" + "Address: " + a.STREET + ", " + a.CITY + ", " + a.STATE + " " + a.ZIP;
                toprint += "\n";
                toprint += "Note: " + a.NOTE;
                toprint += "\n";
                toprint += "Appointment made by " + a.APPT_MADE_BY + " on " + a.DATE_APPT_MADE;
                toprint += "\n\n";
            }
            g.DrawString(toprint, f10, Brushes.Black, printArea, sf2);
        }
    }
}
