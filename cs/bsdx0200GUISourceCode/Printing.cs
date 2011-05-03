using System;
using System.Drawing.Printing;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Data;

namespace IndianHealthService.ClinicalScheduling
{
    public partial class Printing
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
        public virtual void PrintAppointments(dsPatientApptDisplay2 ds, PrintPageEventArgs e, DateTime beg, DateTime end, int resourceToPrint, ref int apptPrinting, int pageNumber)
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
            Rectangle pageArea = e.PageBounds;
            Rectangle headerArea = new Rectangle()
            {
                X = e.MarginBounds.X,
                Y = e.PageBounds.Y,
                Height = e.MarginBounds.Y - e.PageBounds.Y,
                Width = e.MarginBounds.Width
            };
            Rectangle footerArea = new Rectangle()
            {
                X = e.MarginBounds.X,
                Y = e.MarginBounds.Height + headerArea.Height,
                Width = e.MarginBounds.Width,
                Height = pageArea.Height - (e.MarginBounds.Height + headerArea.Height)
            };

            //print footer
            StringFormat sfFooter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            string s = strings.Printed + ": " + DateTime.Now.ToString();
            Font fFooter = new Font(FontFamily.GenericSerif, 7);
            g.DrawString(s, fFooter, Brushes.Black, footerArea, sfFooter);

            //resource we want to print
            dsPatientApptDisplay2.BSDXResourceRow r = ds.BSDXResource[resourceToPrint];
            
            //header
            string toprint;
            if (beg.Date == end.Date) toprint = "Appointments for " + r.RESOURCE_NAME + " on " + beg.ToLongDateString();
            else toprint = "Appointments for " + r.RESOURCE_NAME + " from " + beg.ToShortDateString() + " to "
                + end.ToShortDateString();
            g.DrawString(toprint, f14bold, Brushes.Black, printArea);
            
            //Move print area down
            int titleHeight = (int)g.MeasureString(toprint, f14bold, printArea.Size).Height;
            printArea.Height -= titleHeight;
            printArea.Y += titleHeight;

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
                apptPrintStr.AppendLine(a.ApptDate.ToString() + "\t" + a.Name + " (" + a.Sex + ")" + "\t" + "DOB: " + a.DOB.ToShortDateString() + "\t" + "ID: " + a.HRN);
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


            g.Dispose();
        }

        /// <summary>
        /// Prints a single appointment slip to give to the patient
        /// </summary>
        /// <param name="appt">The Appointment to print</param>
        /// <param name="e">PrintPageEventArgs from PrintDocument Print handler</param>
        public virtual void PrintAppointmentSlip(CGAppointment appt, PrintPageEventArgs e)
        {
            // Prep
            Graphics g = e.Graphics;
            Rectangle printArea = e.MarginBounds;
            Rectangle pageArea = e.PageBounds;
            Rectangle headerArea = new Rectangle()
            {
                X = e.MarginBounds.X,
                Y = e.PageBounds.Y,
                Height = e.MarginBounds.Y - e.PageBounds.Y - 50,
                Width = e.MarginBounds.Width
            };
            Rectangle footerArea = new Rectangle()
            {
                X = e.MarginBounds.X,
                Y = e.MarginBounds.Height + headerArea.Height,
                Width = e.MarginBounds.Width,
                Height = pageArea.Height - (e.MarginBounds.Height + headerArea.Height)
            };

            string s;

            // A few fonts
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 12);
            Font fGroupTitle = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);

            StringFormat sfCenterCenter = new StringFormat()
            {
                 Alignment = StringAlignment.Center,
                 LineAlignment = StringAlignment.Center
            };
            
            // Draw Header
            string division = CGDocumentManager.Current.ConnectInfo.DivisionName;
            g.DrawString(division, fBody, Brushes.Black, headerArea, sfCenterCenter);

            const int watermarkLength = 75;

            // Move down for optional form paper
            printArea.Y += watermarkLength;
            printArea.Height -= watermarkLength;

            // Draw Title
            StringFormat sfCenter = new StringFormat();
            sfCenter.Alignment = StringAlignment.Center; //for title & header

            //string s = "Appointment Reminder Slip";
            s = strings.ApptReminderSlip;
            g.DrawString(s, fTitle, Brushes.Black, printArea, sfCenter); //title

            // move down
            int titleHeight = (int)g.MeasureString(s, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // draw curved rectangle.
            Rectangle personalInfoRectangle = new Rectangle(e.MarginBounds.X, printArea.Y + 30, 280, 300);
            using (GraphicsPath path = GetRoundedRectPath(personalInfoRectangle, 10))
            {
                g.DrawPath(Pens.Black, path);
            }

            // group header
            s = strings.PtInfo;
            StringFormat sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            g.DrawString(s, fGroupTitle, Brushes.Black, new Rectangle(personalInfoRectangle.X, personalInfoRectangle.Y - 20,personalInfoRectangle.Width, 20),sf3);
            
            // inner rectangle for drawing strings:
            Rectangle personalInfoInnerRectangle = new Rectangle(personalInfoRectangle.X + 20, personalInfoRectangle.Y + 20, 280 - 40, 300 - 40);
            
            // Strings to write
            StringBuilder sb = new StringBuilder(500);
            sb.AppendLine(strings.Name + ":" + "\t" + appt.Patient.Name);
            sb.AppendLine();
            sb.AppendLine(strings.ID + ":" + "\t" + appt.Patient.ID);
            sb.AppendLine();
            sb.AppendLine(strings.DOB + ":" + "\t" + appt.Patient.DOB.ToShortDateString());
            sb.AppendLine();
            sb.AppendLine(strings.Age + ":" + "\t" + appt.Patient.UserFriendlyAge);
            sb.AppendLine();
            s = appt.Patient.Sex == Sex.Male ? strings.Male : strings.Female;
            sb.AppendLine(strings.Sex + ":" + "\t" + s);

            // Draw them
            sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            sf3.SetTabStops(0, new float[] {75} );
            sf3.SetDigitSubstitution(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID, StringDigitSubstitute.Traditional);
            g.DrawString(sb.ToString(), fBody, Brushes.Black, personalInfoInnerRectangle, sf3);

            // draw curved rectangle
            Rectangle apptInfoRectangle = new Rectangle(e.MarginBounds.X + e.MarginBounds.Width - 280, printArea.Y + 30, 280, 300);
            using (GraphicsPath path = GetRoundedRectPath(apptInfoRectangle, 10))
            {
                g.DrawPath(Pens.Black, path);
            }

            s = strings.ApptInfo;
            // group header
            g.DrawString(s, fGroupTitle, Brushes.Black, new Rectangle(apptInfoRectangle.X, apptInfoRectangle.Y - 20,apptInfoRectangle.Width, 20), sf3);

            // Strings to write
            sb = new StringBuilder();
            sb.AppendLine(strings.Clinic + ":" + "\t" + appt.Resource);
            sb.AppendLine();
            sb.AppendLine(strings.Date + ":" + "\t" + appt.StartTime.ToShortDateString());
            sb.AppendLine();
            sb.AppendLine(strings.Day + ":" + "\t" + appt.StartTime.ToString("dddd"));
            sb.AppendLine();
            sb.AppendLine(strings.Time + ":" + "\t" + appt.StartTime.ToShortTimeString());

            // Draw them
            Rectangle apptInfoInnerRectangle = new Rectangle(apptInfoRectangle.X + 20, apptInfoRectangle.Y + 20, 280 - 40, 300 - 40);

            // Draw them
            g.DrawString(sb.ToString(), fBody, Brushes.Black, apptInfoInnerRectangle, sf3);

            // Move Drawing Rectangle Down
            printArea.Y += 300 + 30 + 20;
            printArea.Height -= 300 + 30 + 20;

            // Draw New Line
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 5;
            printArea.Height -= 5;

            // Draw new Title
            s = strings.ClinicInstructions;
            g.DrawString(s, fTitle, Brushes.Black, printArea, sfCenter); //title

            // move down
            titleHeight = (int)g.MeasureString(s, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // Draw New Line
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // Get Resource Clinic Appointment Letter Text
            DataTable resources = CGDocumentManager.Current.GlobalDataSet.Tables["Resources"];

            string ltrTxt = (from resource in resources.AsEnumerable()
                             where resource.Field<string>("RESOURCE_NAME") == appt.Resource
                             select resource.Field<string>("LETTER_TEXT")).SingleOrDefault<string>();

            if (String.IsNullOrWhiteSpace(ltrTxt)) ltrTxt = strings.NoInstructionsProvided;

            g.DrawString(ltrTxt, fBody, Brushes.Black, printArea, sf3);

            int ltrTxtHeight = (int)g.MeasureString(ltrTxt, fBody).Height;

            printArea.Y += ltrTxtHeight + 15;
            printArea.Height -= ltrTxtHeight + 15;

            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 5;
            printArea.Height -= 5;

            s = strings.Notes;
            g.DrawString(s, fTitle, Brushes.Black, printArea, sfCenter); // Notes title
            
            // move down
            titleHeight = (int)g.MeasureString(s, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // Draw New Line
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // Draw Notes Area
            using (GraphicsPath path = GetRoundedRectPath(printArea, 15))
            {
                g.DrawPath(Pens.Black, path);
            }

            //use sf0 to print the footer (center all the way)
            s = strings.Printed + ": " + DateTime.Now.ToString();
            Font fFooter = new Font(FontFamily.GenericSerif, 7);
            g.DrawString(s, fFooter, Brushes.Black, footerArea, sfCenterCenter);

            g.Dispose();
        }


        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = 2 * radius;
           
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();
            
            path.AddArc(arcRect, 180, 90); //top left
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90); // top right
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90); // bottom right
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90); // bottom left

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Print Letter to be given or mailed to the patient
        /// </summary>
        /// <param name="ptrow">Strongly typed PatientApptsRow to pass (just one ApptRow)</param>
        /// <param name="e">You know what that is</param>
        /// <param name="letter">Contains letter string</param>
        /// <param name="title">Title of the letter</param>
        public virtual void PrintReminderLetter(dsPatientApptDisplay2.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
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

            //Text Direction
            StringFormat sf2 = new StringFormat();
            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
                sf2.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea, sf2);

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

            g.Dispose();
        }
       
        /// <summary>
        /// Cancellation Letter to be given or mailed to the patient
        /// </summary>
        /// <param name="ptRow">Strongly typed PatientApptsRow to pass (just one ApptRow)</param>
        /// <param name="e">You know what that is</param>
        /// <param name="letter">Contains letter string</param>
        /// <param name="title">Title of the letter</param>
        public virtual void PrintCancelLetter(dsRebookAppts.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
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

            //Text Direction
            StringFormat sf2 = new StringFormat();
            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
                sf2.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            
            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea, sf2);

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

            g.Dispose();
        }

        /// <summary>
        /// Print rebook letters. Prints old and new appointments dates then the missive.
        /// </summary>
        /// <param name="ptRow">Strongly typed appointment row</param>
        /// <param name="e">etc</param>
        /// <param name="letter">Text of the letter to print</param>
        /// <param name="title">Title to print at the top of the letter</param>
        public virtual void PrintRebookLetter(dsRebookAppts.PatientApptsRow ptRow, PrintPageEventArgs e, string letter, string title)
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

            //Text Direction
            StringFormat sf2 = new StringFormat();
            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
                sf2.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            // write missive
            g.DrawString(letter, fBody, Brushes.Black, printArea, sf2);

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

            g.Dispose();
        }

        /// <summary>
        /// Print message on a page; typically that there are no appointments to be found.
        /// Or just a test message to verify that printing works.
        /// </summary>
        /// <param name="msg">The exact string to print.</param>
        /// <param name="e">Print Page event args</param>
        public virtual void PrintMessage(string msg, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(msg, new Font(FontFamily.GenericSerif, 14),
                Brushes.Black, e.MarginBounds);
            e.Graphics.Dispose();
        }

        /// <summary>
        /// Print Routing Slip
        /// </summary>
        /// <param name="a">Appointment Data Structure</param>
        /// <param name="apptOrder">Order of Appointment</param>
        /// <param name="e">etc</param>
        public virtual void PrintRoutingSlip(CGAppointment appt, int apptOrder, PrintPageEventArgs e)
        {
            // Prep
            Graphics g = e.Graphics;
            Rectangle printArea = e.MarginBounds;
            Rectangle pageArea = e.PageBounds;
            Rectangle headerArea = new Rectangle()
            {
                X = e.MarginBounds.X, 
                Y = e.PageBounds.Y,  //0
                Height = e.MarginBounds.Y - e.PageBounds.Y - 50, //100px - 50px
                Width = e.MarginBounds.Width
            };
            Rectangle footerArea = new Rectangle()
            {
                X = e.MarginBounds.X,
                Y = e.MarginBounds.Height + headerArea.Height,
                Width = e.MarginBounds.Width,
                Height = pageArea.Height - (e.MarginBounds.Height + headerArea.Height)
            };

            const int part1Height = 400;
            string s;

            // A few fonts
            Font fTitle = new Font(FontFamily.GenericSerif, 24, FontStyle.Bold); //for title
            Font fBody = new Font(FontFamily.GenericSerif, 12);
            Font fGroupTitle = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);

            StringFormat sf0 = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Draw Header
            string division = CGDocumentManager.Current.ConnectInfo.DivisionName;
            g.DrawString(division, fBody, Brushes.Black, headerArea, sf0);

            const int watermarkLength = 75;

            // Move down for optional form paper
            printArea.Y += watermarkLength;
            printArea.Height -= watermarkLength;

            // Draw Title
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; //for title & header
            string title = strings.RoutingSlip;
            g.DrawString(title, fTitle, Brushes.Black, printArea, sf); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += 15;
            printArea.Height -= 15;

            // draw curved rectangle.
            Rectangle personalInfoRectangle = new Rectangle(e.MarginBounds.X, printArea.Y + 30, 280, part1Height);
            using (GraphicsPath path = GetRoundedRectPath(personalInfoRectangle, 10))
            {
                g.DrawPath(Pens.Black, path);
            }

            // group header
            s = strings.PtInfo;
            StringFormat sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            g.DrawString(s, fGroupTitle, Brushes.Black, new Rectangle(personalInfoRectangle.X, personalInfoRectangle.Y - 20, personalInfoRectangle.Width, 20), sf3);

            StringFormat sf4 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            sf4.SetTabStops(0, new float[] { 75 });
            sf4.SetDigitSubstitution(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID, StringDigitSubstitute.Traditional);

            // inner rectangle for drawing strings:
            Rectangle personalInfoInnerRectangle = new Rectangle(personalInfoRectangle.X + 20, personalInfoRectangle.Y + 20, personalInfoRectangle.Width - 40, personalInfoRectangle.Height - 40);

            // Strings to write
            StringBuilder sb = new StringBuilder(500);
            sb.AppendLine(strings.Name + ":" + "\t" + appt.Patient.Name);
            sb.AppendLine();
            sb.AppendLine(strings.ID + ":" + "\t" + appt.Patient.ID);
            sb.AppendLine();
            sb.AppendLine(strings.DOB + ":" + "\t" + appt.Patient.DOB.ToShortDateString());
            sb.AppendLine();
            sb.AppendLine(strings.Age + ":" + "\t" + appt.Patient.UserFriendlyAge);
            //sb.AppendLine();
            //sb.AppendLine("Sex:" + "\t" + appt.Patient.Sex.ToString());

            // Draw them
            g.DrawString(sb.ToString(), fBody, Brushes.Black, personalInfoInnerRectangle, sf4);

            // draw curved rectangle
            Rectangle apptInfoRectangle = new Rectangle(e.MarginBounds.X + e.MarginBounds.Width - 280, printArea.Y + 30, 280, part1Height);
            using (GraphicsPath path = GetRoundedRectPath(apptInfoRectangle, 10))
            {
                g.DrawPath(Pens.Black, path);
            }

            // group header
            s = strings.ApptInfo;
            g.DrawString(s, fGroupTitle, Brushes.Black, new Rectangle(apptInfoRectangle.X, apptInfoRectangle.Y - 20, apptInfoRectangle.Width, 20), sf3);

            // Strings to write
            sb = new StringBuilder();
            sb.AppendLine(strings.Clinic + ":");
            sb.AppendLine(appt.Resource);
            sb.AppendLine();
            sb.AppendLine(strings.AppointmentProvider + ":"); 
            sb.AppendLine((appt.Provider == null) ? strings.none : appt.Provider.ToString());  //Appt Provider or (none) if null
            sb.AppendLine();
            sb.AppendLine(strings.PatientOrder + ":" + "\t" + apptOrder);
            sb.AppendLine(strings.Date + ":" + "\t" + appt.StartTime.ToShortDateString() + " " + appt.StartTime.ToShortTimeString());
            sb.AppendLine();
            sb.AppendLine(strings.AppointmentNote + ":");
            sb.AppendLine(String.IsNullOrWhiteSpace(appt.Note)? strings.none : appt.Note);

            // Draw them
            Rectangle apptInfoInnerRectangle = new Rectangle(apptInfoRectangle.X + 20, apptInfoRectangle.Y + 20, apptInfoRectangle.Width - 40, apptInfoRectangle.Height - 40);

            // Draw them
            g.DrawString(sb.ToString(), fBody, Brushes.Black, apptInfoInnerRectangle, sf4);

            // Move Drawing Rectangle Down
            printArea.Y += apptInfoRectangle.Height + 30 + 20;
            printArea.Height -= apptInfoRectangle.Height + 30 + 20;

            // Draw New Line
            using (Pen dashpen = new Pen(Color.Black))
            {
                dashpen.DashStyle = DashStyle.Dash;
                g.DrawLine(dashpen, printArea.Location, new Point(printArea.Right, printArea.Y));
            }

            printArea.Y += 5;
            printArea.Height -= 5;

            s = strings.ScratchArea;
            g.DrawString(s, fGroupTitle, Brushes.Black, printArea, sf3); 
            
            /* Per Al-Najjar, we don't want the next appointment instructions section
            // move down
            printArea.Y += 240;
            printArea.Height -= 240;

            using (Pen dashpen = new Pen(Color.Black))
            {
                dashpen.DashStyle = DashStyle.Dot;
                g.DrawLine(dashpen, printArea.Location, new Point(printArea.Right, printArea.Y));
            }

            printArea.Y += 5;
            printArea.Height -= 5;

            s = strings.NextAppointmentInstructions;
            g.DrawString(s, fGroupTitle, Brushes.Black, printArea, sf3);
            */

            // Draw Footer
            //use sf0 to print the footer (center all the way)
            s = strings.Printed + ": " + DateTime.Now.ToString();
            Font fFooter = new Font(FontFamily.GenericSerif, 7);
            g.DrawString(s, fFooter, Brushes.Black, footerArea, sf0);

            g.Dispose();

        }

    }
    public abstract class PrintingCreator
    {
        public abstract Printing PrintFactory();
    }

}