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
            // setting the reference values for calculating the needed ratio of the coming page.
            //referrenceHeight and referrenceWidth for A6 size.
            double referrenceHeight = 383;
            double referrenceWidth = 213;

            //check if the paper is landscape or portrait. if it is landscape then new ratio need to be calculated.
            if (e.MarginBounds.Width > e.MarginBounds.Height)
            {
                //landscape = true;
                referrenceHeight = 213;
                referrenceWidth = 383;
                //fontRatio = ((e.MarginBounds.Height) / referrenceHeight);
            }
            
            //setting the font,width and height ratios 
            double fontRatio = (e.MarginBounds.Height / referrenceHeight);
            if (fontRatio > 1.0)
            {
                fontRatio = fontRatio * 0.75;
            }
            double widthRatio = ((e.MarginBounds.Width) / referrenceWidth);
            double heightRatio = ((e.MarginBounds.Height) / referrenceHeight);

            Graphics g = e.Graphics;
            //this is the string which is used to fill with the data and to be print on the screen. 
            string stringData;

            //in portrait we used fonts that fits with the A6 paper and changed depending on the page ratio,but in the land scape we used fonts fits with the A5 paper and counted the other pages depening on the ratios.
            Font fontTitle = new Font(FontFamily.GenericSerif, (int)(13 * fontRatio), FontStyle.Bold); //for title
            Font fontBody = new Font(FontFamily.GenericSerif, (int)(13 * fontRatio));
            Font fontGroupTitle = new Font(FontFamily.GenericSansSerif, (int)(13 * fontRatio), FontStyle.Bold);
            Font fFooter = new Font(FontFamily.GenericSerif, (int)(7 * fontRatio));

            StringFormat sfCenterNear = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            StringFormat sfCenterCenter = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            StringFormat sfCenterFar = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };

            StringFormat sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            sf3.SetDigitSubstitution(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID, StringDigitSubstitute.Traditional);


            //set the hard margin which different for each printer and multiplied with 2 
            int HardMarginX = 0;
            if ((int)(e.PageSettings.HardMarginX) > 0)
            {
                HardMarginX=((int)(e.PageSettings.HardMarginX) *2);
            }
            int HardMarginY = 0;
            if ((int)(e.PageSettings.HardMarginY) > 0)
            {
                HardMarginY = ((int)(e.PageSettings.HardMarginY) * 2);
            }

            // Draw Header 

            string division = CGDocumentManager.Current.RemoteSession.User.Division.Name;
            int divisionStringHeight = (int)g.MeasureString(division.ToString(), fontBody, e.PageBounds.Width - (int)(10 * widthRatio) - HardMarginX).Height;
            
            Rectangle headerArea = new Rectangle()
            {
                X = (int)(5 * widthRatio),
                Y = (int)(5 * heightRatio),
                Height = divisionStringHeight + (int)(5 * heightRatio),
                Width = e.PageBounds.Width - (int)(10 * widthRatio) - HardMarginX
            };
            g.DrawString(division, fontBody, Brushes.Black, headerArea, sfCenterNear);

            //use stringData to print the footer (center all the way)
            stringData = strings.Printed + ": " + DateTime.Now.ToString();
            int stringDataStringHeight = (int)g.MeasureString(stringData.ToString(), fFooter, e.MarginBounds.Width).Height;
            Rectangle footerArea = new Rectangle()
            {
                X = headerArea.X,
                Y = e.PageBounds.Height - stringDataStringHeight - HardMarginY - (int)(5 * heightRatio),
                Width = headerArea.Width,
                Height = stringDataStringHeight
            };
            g.DrawString(stringData, fFooter, Brushes.Black, footerArea, sfCenterCenter);
            
            //setting the printing area bigger than the margin bounds to use more printing space on the paper.
            Rectangle printArea = new Rectangle()
            {
                X = headerArea.X,
                Y = headerArea.Y+headerArea.Height,
                Height = footerArea.Y-(headerArea.Y+headerArea.Height) - (int)(5 * heightRatio),
                Width = headerArea.Width
            };
            
            //  if there is a need for the watermark use the following code.
            //const int watermarkLength = 75*heightratio;

            // Move down for optional form paper
            //printArea.Y += watermarkLength;
            //printArea.Height -= watermarkLength;

            //// Draw Title
            //StringFormat sfCenter = new StringFormat();
            //sfCenter.Alignment = StringAlignment.Center; //for title & header

            //string data for the appointment slip title
            stringData = strings.ApptReminderSlip;
            g.DrawString(stringData, fontTitle, Brushes.Black, printArea, sfCenterFar); //title
            
            //// move down
            int titleHeight = (int)g.MeasureString(stringData, fontTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += (int)(10 * heightRatio);
            printArea.Height -= (int)(10 * heightRatio);

            // building the string of the person information to write
            StringBuilder personInformationString = new StringBuilder(500);
            personInformationString.AppendLine(strings.Name + ":" + "\t" + appt.Patient.Name);
            //personInformationString.AppendLine();
            stringData = appt.Patient.Sex == Sex.Male ? strings.Male : strings.Female;
            personInformationString.AppendLine(strings.ID + ":" + "\t" + appt.Patient.ID + "\t" + "( " + stringData + " )");
            //personInformationString.AppendLine();
            stringData = appt.Patient.DOB.ToString("dd") + " " + appt.Patient.DOB.ToString("MMM") + ", " + appt.Patient.DOB.Year;
            personInformationString.AppendLine(strings.DOB + ":" + "\t" + stringData);
            //personInformationString.AppendLine();
            //personInformationString.AppendLine(strings.Age + ":" + "\t" + appt.Patient.UserFriendlyAge);


            // building the string of the appointment information to write
            StringBuilder appointmentInformationString = new StringBuilder(500);
            appointmentInformationString = new StringBuilder();
            appointmentInformationString.AppendLine(strings.Clinic + ":" + "\t" + appt.Resource);
            //appointmentInformationString.AppendLine();
            stringData = appt.StartTime.ToString("dddd") + " " + appt.StartTime.ToString("dd") + " " + appt.StartTime.ToString("MMM") + ", " + appt.StartTime.Year;
            appointmentInformationString.AppendLine(strings.Date + ":" + "\t" + stringData);
            //sb.AppendLine();
            //appointmentInformationString.AppendLine(strings.Day + ":" + "\t" + appt.StartTime.ToString("dddd"));
            //sb.AppendLine();
            appointmentInformationString.AppendLine(strings.Time + ":" + "\t" + appt.StartTime.ToShortTimeString());


            //counting the width and hieght of the information inner aera for the person and appointment rectangels.
            int InfoInnerRectangleWidth = (printArea.Width / 2 - printArea.X / 2) - (int)(10 * widthRatio);

            //Counting the height of the two strings (PersontInformationStringappointment & InformationStringto)to be allocated to the InfoInnerRectangleHeight later.
            int PersonInformationStringHeight = (int)g.MeasureString(personInformationString.ToString(), fontBody, InfoInnerRectangleWidth).Height;
            int appointmentInformationStringHeight = (int)g.MeasureString(appointmentInformationString.ToString(), fontBody, InfoInnerRectangleWidth).Height;
            
            int InfoInnerRectangleHeight = PersonInformationStringHeight;
            if (appointmentInformationStringHeight > PersonInformationStringHeight)
            {
                InfoInnerRectangleHeight = appointmentInformationStringHeight;
            }

            // draw curved rectangle for the person informations.
            Rectangle personalInfoRectangle = new Rectangle(printArea.X, printArea.Y, InfoInnerRectangleWidth + (int)(10 * widthRatio), InfoInnerRectangleHeight + (int)(10 * heightRatio));
            using (GraphicsPath path = GetRoundedRectPath(personalInfoRectangle, 5))
            {
                g.DrawPath(Pens.Black, path);
            }
            
            // inner rectangle for drawing strings:
            Rectangle personalInfoInnerRectangle = new Rectangle(personalInfoRectangle.X + (int)(5 * widthRatio), personalInfoRectangle.Y + (int)(5 * heightRatio), InfoInnerRectangleWidth, InfoInnerRectangleHeight);

            // Draw the preson informations.
            //sf3.SetTabStops(0, new float[] {75} );
            
            g.DrawString(personInformationString.ToString(), fontBody, Brushes.Black, personalInfoInnerRectangle, sf3);

            // draw curved rectangle for the appointment informations.
            Rectangle apptInfoRectangle = new Rectangle((int)(printArea.X + personalInfoRectangle.Width + printArea.X), personalInfoRectangle.Y, personalInfoRectangle.Width, personalInfoRectangle.Height);
            using (GraphicsPath path = GetRoundedRectPath(apptInfoRectangle, 5))
            {
                g.DrawPath(Pens.Black, path);
            }


            // Draw appointment informations in the inner rectangle.
            Rectangle apptInfoInnerRectangle = new Rectangle(apptInfoRectangle.X + (int)(5 * widthRatio), apptInfoRectangle.Y + (int)(5 * heightRatio), InfoInnerRectangleWidth, InfoInnerRectangleHeight);
            g.DrawString(appointmentInformationString.ToString(), fontBody, Brushes.Black, apptInfoInnerRectangle, sf3);

            // Move Drawing Rectangle Down
            printArea.Y += personalInfoRectangle.Height  + (int)(5 * heightRatio);
            printArea.Height -= personalInfoRectangle.Height +  (int)(5 * heightRatio);

            // Draw New Line
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += (int)(5 * heightRatio);
            printArea.Height -= (int)(5 * heightRatio);

            // Draw new Title
            stringData = strings.ClinicInstructions;
            titleHeight = (int)g.MeasureString(stringData, fontTitle, printArea.Width).Height;
            g.DrawString(stringData, fontTitle, Brushes.Black, printArea, sf3); //title
            // move down
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // Get Resource Clinic Appointment Letter Text
            DataTable resources = CGDocumentManager.Current.GlobalDataSet.Tables["Resources"];

            string ltrTxt = (from resource in resources.AsEnumerable()
                             where resource.Field<string>("RESOURCE_NAME") == appt.Resource
                             select resource.Field<string>("LETTER_TEXT")).SingleOrDefault<string>();

            if (String.IsNullOrWhiteSpace(ltrTxt)) ltrTxt = strings.NoInstructionsProvided;
            int ltrTxtHeight = (int)g.MeasureString(ltrTxt, fontBody,printArea.Width).Height;
            //setting the instructions area smaller than the printing area.
            int instructionsAreaHeight = ltrTxtHeight;
            //missingString true if there are no more printing area for the instructions and will use it to print a statement for it.
            bool missingString = false;
            string missingStringStatement = "";
            int missingStringStatementHeight = 0;
            if (ltrTxtHeight > printArea.Height)
            {
                missingStringStatement = "etc...";
                missingStringStatementHeight = (int)g.MeasureString(missingStringStatement, fFooter, printArea.Width).Height;
                instructionsAreaHeight = printArea.Height -missingStringStatementHeight- (int)(5 * heightRatio);
                missingString = true;
            }
            Rectangle instructionsArea = new Rectangle()
            {
                X = printArea.X,
                Y = printArea.Y,
                Height = instructionsAreaHeight,
                Width = printArea.Width
            };
            g.DrawString(ltrTxt, fontBody, Brushes.Black, instructionsArea, sf3);
            
            //moving down
            printArea.Y += instructionsAreaHeight + (int)(5 * heightRatio);
            printArea.Height -= instructionsAreaHeight + (int)(5 * heightRatio);

            if (missingString)
            {
                g.DrawString(missingStringStatement, fFooter, Brushes.Black, printArea, sf3); //title
                printArea.Y += missingStringStatementHeight;
                printArea.Height -= missingStringStatementHeight;
            }
            //Draing new line
            if (printArea.Height > 0)
            {
                g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
                printArea.Y += (int)(5 * heightRatio);
                printArea.Height -= (int)(5 * heightRatio);

                string noteStringData = strings.Notes;
                int noteTitleHeight = (int)g.MeasureString(noteStringData, fontTitle, printArea.Width).Height;

                //check if the remaining area height enugh for the notes 
                if (printArea.Height > ((int)(noteTitleHeight)))
                {
                    //draw the note title.
                    g.DrawString(noteStringData, fontTitle, Brushes.Black, printArea, sf3); // Notes title
                }
            

            }
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
            // setting the reference values for calculating the needed ratio of the coming page.
            //referrenceHeight and referrenceWidth for A6 size.
            double referrenceHeight = 383;
            double referrenceWidth = 213;

            //check if the paper is landscape or portrait. if it is landscape then new ratio need to be calculated.
            if (e.MarginBounds.Width > e.MarginBounds.Height)
            {
                //landscape = true;
                referrenceHeight = 213;
                referrenceWidth = 383;
                //fontRatio = ((e.MarginBounds.Height) / referrenceHeight);
            }

            //setting the font,width and height ratios 
            double fontRatio = (e.MarginBounds.Height / referrenceHeight);
            if (fontRatio > 1.0)
            {
                fontRatio = fontRatio * 0.75;
            }
            double widthRatio = ((e.MarginBounds.Width) / referrenceWidth);
            double heightRatio = ((e.MarginBounds.Height) / referrenceHeight);

            Graphics g = e.Graphics;
            //this is the string which is used to fill with the data and to be print on the screen. 
            string stringData;

            //in portrait we used fonts that fits with the A6 paper and changed depending on the page ratio,but in the land scape we used fonts fits with the A5 paper and counted the other pages depening on the ratios.
            Font fontTitle = new Font(FontFamily.GenericSerif, (int)(13 * fontRatio), FontStyle.Bold); //for title
            Font fontBody = new Font(FontFamily.GenericSerif, (int)(13 * fontRatio));
            Font fontGroupTitle = new Font(FontFamily.GenericSansSerif, (int)(13 * fontRatio), FontStyle.Bold);
            Font fFooter = new Font(FontFamily.GenericSerif, (int)(7 * fontRatio));

            StringFormat sfCenterCenter = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            //set the hard margin which different for each printer and multiplied with 2 
            int HardMarginX = 0;
            if ((int)(e.PageSettings.HardMarginX) > 0)
            {
                HardMarginX = ((int)(e.PageSettings.HardMarginX) * 2);
            }
            int HardMarginY = 0;
            if ((int)(e.PageSettings.HardMarginY) > 0)
            {
                HardMarginY = ((int)(e.PageSettings.HardMarginY) * 2);
            }

            // Draw Header
            string division = CGDocumentManager.Current.RemoteSession.User.Division.Name;
            int divisionStringHeight = (int)g.MeasureString(division.ToString(), fontBody, e.MarginBounds.Width).Height;
            Rectangle headerArea = new Rectangle()
            {
                X = (int)(5 * widthRatio),
                Y = (int)(5 * heightRatio),
                Height = divisionStringHeight + (int)(5 * heightRatio),
                Width = e.PageBounds.Width - (int)(10 * widthRatio) - HardMarginX
            };
            g.DrawString(division, fontBody, Brushes.Black, headerArea, sfCenterCenter);


            //use stringData to print the footer (center all the way)
            stringData = strings.Printed + ": " + DateTime.Now.ToString();
            int stringDataStringHeight = (int)g.MeasureString(stringData.ToString(), fFooter, e.MarginBounds.Width).Height;
            Rectangle footerArea = new Rectangle()
            {
                X = headerArea.X,
                Y = e.PageBounds.Height - stringDataStringHeight - HardMarginY - (int)(5 * heightRatio),
                Width = headerArea.Width,
                Height = stringDataStringHeight
            };
            g.DrawString(stringData, fFooter, Brushes.Black, footerArea, sfCenterCenter);



            //setting the printing area bigger than the margin bounds to use more printing space on the paper.
            Rectangle printArea = new Rectangle()
            {
                X = headerArea.X,
                Y = headerArea.Y + headerArea.Height,
                Height = footerArea.Y - (headerArea.Y + headerArea.Height) - (int)(5 * heightRatio),
                Width = headerArea.Width
            };


            //  if there is a need for the watermark use the following code.
            //const int watermarkLength = 75*heightratio;

            // Move down for optional form paper
            //printArea.Y += watermarkLength;
            //printArea.Height -= watermarkLength;

            // Draw Title
            StringFormat sfCenter = new StringFormat();
            sfCenter.Alignment = StringAlignment.Center; //for title & header
            string title = strings.RoutingSlip;
            g.DrawString(title, fontTitle, Brushes.Black, printArea, sfCenter); //title

            // move down
            int titleHeight = (int)g.MeasureString(title, fontTitle, printArea.Width).Height;
            printArea.Y += titleHeight;
            printArea.Height -= titleHeight;

            // draw underline
            g.DrawLine(Pens.Black, printArea.Location, new Point(printArea.Right, printArea.Y));
            printArea.Y += (int)(5 * heightRatio);
            printArea.Height -= (int)(5 * heightRatio);

            // Strings to write

            StringBuilder personInformationString = new StringBuilder(500);
            personInformationString.AppendLine(strings.Name + ":" + "\t" + appt.Patient.Name);
            //personInformationString.AppendLine();
            personInformationString.AppendLine(strings.ID + ":" + "\t" + appt.Patient.ID);
            //personInformationString.AppendLine();
            stringData = appt.Patient.DOB.ToString("dd") + " " + appt.Patient.DOB.ToString("MMM") + ", " + appt.Patient.DOB.Year;
            personInformationString.AppendLine(strings.DOB + ":" + "\t" + stringData);
            //personInformationString.AppendLine();
            personInformationString.AppendLine(strings.Age + ":" + "\t" + appt.Patient.UserFriendlyAge);
            //personInformationString.AppendLine();
            //personInformationString.AppendLine("Sex:" + "\t" + appt.Patient.Sex.ToString());

            // Strings to write
            StringBuilder appointmentInformationString = new StringBuilder();
            appointmentInformationString.AppendLine(strings.Clinic + ":");
            appointmentInformationString.AppendLine(appt.Resource);
            //appointmentInformationString.AppendLine();
            appointmentInformationString.AppendLine(strings.AppointmentProvider + ":");
            appointmentInformationString.AppendLine((appt.Provider == null) ? strings.none : appt.Provider.ToString());  //Appt Provider or (none) if null
            //appointmentInformationString.AppendLine();
            appointmentInformationString.AppendLine(strings.PatientOrder + ":" + "\t" + apptOrder);
            appointmentInformationString.AppendLine(strings.Date + ":" + "\t" + appt.StartTime.ToShortDateString() + " " + appt.StartTime.ToShortTimeString());
            //appointmentInformationString.AppendLine();
            appointmentInformationString.AppendLine(strings.AppointmentNote + ":");
            appointmentInformationString.AppendLine(String.IsNullOrWhiteSpace(appt.Note) ? strings.none : appt.Note);
            
            //counting the width and hieght of the information inner aera for the person and appointment rectangels.
            int InfoInnerRectangleWidth = (printArea.Width / 2 - printArea.X / 2) - (int)(10 * widthRatio);

            //Counting the height of the two strings (PersontInformationStringappointment & InformationStringto)to be allocated to the InfoInnerRectangleHeight later.
            int PersonInformationStringHeight = (int)g.MeasureString(personInformationString.ToString(), fontBody, InfoInnerRectangleWidth).Height;
            int appointmentInformationStringHeight = (int)g.MeasureString(appointmentInformationString.ToString(), fontBody, InfoInnerRectangleWidth).Height;

            int InfoInnerRectangleHeight = PersonInformationStringHeight;
            if (appointmentInformationStringHeight > PersonInformationStringHeight)
            {
                InfoInnerRectangleHeight = appointmentInformationStringHeight;
            }

            // group header for the person informations.
            stringData = strings.PtInfo;
            StringFormat sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            stringDataStringHeight = (int)g.MeasureString(stringData.ToString(), fontGroupTitle, InfoInnerRectangleWidth).Height;
            g.DrawString(stringData, fontGroupTitle, Brushes.Black, new Rectangle(printArea.X, printArea.Y, InfoInnerRectangleWidth, stringDataStringHeight), sf3);

            // group header for the appointment information
            stringData = strings.ApptInfo;
            g.DrawString(stringData, fontGroupTitle, Brushes.Black, new Rectangle((int)(printArea.X + (InfoInnerRectangleWidth + (int)(10 * widthRatio)) + printArea.X), printArea.Y, InfoInnerRectangleWidth, stringDataStringHeight), sf3);

            //moving down
            printArea.Y += stringDataStringHeight + (int)(5 * heightRatio);
            printArea.Height -= stringDataStringHeight + (int)(5 * heightRatio);

            // draw curved rectangle for the person informations.
            Rectangle personalInfoRectangle = new Rectangle(printArea.X, printArea.Y, InfoInnerRectangleWidth + (int)(10 * widthRatio), InfoInnerRectangleHeight + (int)(10 * heightRatio));
            using (GraphicsPath path = GetRoundedRectPath(personalInfoRectangle, 5))
            {
                g.DrawPath(Pens.Black, path);
            }

            // inner rectangle for drawing strings:
            Rectangle personalInfoInnerRectangle = new Rectangle(personalInfoRectangle.X + (int)(5 * widthRatio), personalInfoRectangle.Y + (int)(5 * heightRatio), InfoInnerRectangleWidth, InfoInnerRectangleHeight);

            // Draw the preson informations.
            sf3 = new StringFormat(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? StringFormatFlags.DirectionRightToLeft : 0);
            //sf3.SetTabStops(0, new float[] {75} );
            sf3.SetDigitSubstitution(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID, StringDigitSubstitute.Traditional);
            g.DrawString(personInformationString.ToString(), fontBody, Brushes.Black, personalInfoInnerRectangle, sf3);

            // draw curved rectangle for the appointment informations.
            Rectangle apptInfoRectangle = new Rectangle((int)(printArea.X + personalInfoRectangle.Width + printArea.X), personalInfoRectangle.Y, personalInfoRectangle.Width, personalInfoRectangle.Height);
            using (GraphicsPath path = GetRoundedRectPath(apptInfoRectangle, 5))
            {
                g.DrawPath(Pens.Black, path);
            }

            // Draw appointment informations in the inner rectangle.
            Rectangle apptInfoInnerRectangle = new Rectangle(apptInfoRectangle.X + (int)(5 * widthRatio), apptInfoRectangle.Y + (int)(5 * heightRatio), InfoInnerRectangleWidth, InfoInnerRectangleHeight);
            g.DrawString(appointmentInformationString.ToString(), fontBody, Brushes.Black, apptInfoInnerRectangle, sf3);
            
            // Move Drawing Rectangle Down
            printArea.Y += personalInfoRectangle.Height +  (int)(5 * heightRatio);
            printArea.Height -= personalInfoRectangle.Height + (int)(5 * heightRatio);

            // Draw New Line
            using (Pen dashpen = new Pen(Color.Black))
            {
                dashpen.DashStyle = DashStyle.Dash;
                g.DrawLine(dashpen, printArea.Location, new Point(printArea.Right, printArea.Y));
            }

            printArea.Y += (int)(5 * heightRatio);
            printArea.Height -= (int)(5 * heightRatio);

            stringData = strings.ScratchArea;
            g.DrawString(stringData, fontGroupTitle, Brushes.Black, printArea, sf3);

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

            stringData = strings.NextAppointmentInstructions;
            g.DrawString(stringData, fontGroupTitle, Brushes.Black, printArea, sf3);
            */

            g.Dispose();

        }

    }
    public abstract class PrintingCreator
    {
        public abstract Printing PrintFactory();
    }

}