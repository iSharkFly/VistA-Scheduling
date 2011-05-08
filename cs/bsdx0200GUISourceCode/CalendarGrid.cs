namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Linq;

    /// <summary>
    /// This class is reponsible for rendering the Calendar Grid.
    /// </summary>
    public class CalendarGrid : ScrollableControl
    {
        private IContainer components;
        private Font fontArial10;
        private Font fontArial8;
        private CGAppointments m_Appointments;
        private Hashtable m_ApptOverlapTable;
        private bool m_bAutoDrag = true;
        private bool m_bDragDropStart;
        private bool m_bDrawWalkIns = true;
        private bool m_bGridEnter;
        //private bool m_bInitialUpdate;
        private bool m_bMouseDown;
        private bool m_bScroll;
        private bool m_bScrollDown;
        private bool m_bSelectingRange;
        private int m_cellHeight;
        private int m_cellWidth;
        private int m_col0Width;
        private Hashtable m_ColumnInfoTable;
        private CGCell m_currentCell;
        private DateTime m_dtStart;
        private Font m_fCell;
        private string m_GridBackColor;
        private CGCells m_gridCells;
        private int m_nColumns = 5;
        private int m_nSelectID;
        private int m_nTimeScale = 20;
        private ArrayList m_pAvArray;
        private string m_sDragSource;
        private CGAppointments m_SelectedAppointments;
        private CGRange m_selectedRange;
        private StringFormat m_sf;
        private StringFormat m_sfHour;
        private StringFormat m_sfRight;
        private ArrayList m_sResourcesArray;
        private Timer m_Timer;                  // Timer used in Drag and Drop Operations
        private ToolTip m_toolTip;
        private const int WM_HSCROLL = 0x114;       // Horizontal Scrolling Windows Message
        private const int WM_VSCROLL = 0x115;       // Vertical Scrolling Windows Message
        private const int WM_MOUSEWHEEL = 0x20a;    // Windows Mouse Scrolling Message
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();


        public delegate void CGAppointmentChangedHandler(object sender, CGAppointmentChangedArgs e);
        public event CGAppointmentChangedHandler CGAppointmentChanged;
        public event CGAppointmentChangedHandler CGAppointmentAdded;

        public delegate void CGSelectionChangedHandler(object sender, CGSelectionChangedArgs e);
        public event CGSelectionChangedHandler CGSelectionChanged;

        public CalendarGrid()
        {
            this.InitializeComponent();
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            this.m_nColumns = 5;
            this.m_gridCells = new CGCells();
            this.m_selectedRange = new CGRange();
            this.m_SelectedAppointments = new CGAppointments();
            //this.m_Appointments = new CGAppointments();
            this.m_dtStart = new DateTime(2003, 1, 27);
            this.m_ApptOverlapTable = new Hashtable();
            this.m_ColumnInfoTable = new Hashtable();
            this.m_sResourcesArray = new ArrayList();
            base.ResizeRedraw = true;
            this.m_col0Width = 100;
            this.fontArial8 = new Font("Arial", 8f);
            this.fontArial10 = new Font("Arial", 10f);
            this.m_fCell = this.fontArial10;
            this.m_sf = new StringFormat();
            this.m_sfRight = new StringFormat();
            this.m_sfHour = new StringFormat();
            this.m_sf.LineAlignment = StringAlignment.Center;
            this.m_sfRight.LineAlignment = StringAlignment.Center;
            this.m_sfRight.Alignment = StringAlignment.Far;
            this.m_sfHour.LineAlignment = StringAlignment.Center;
            this.m_sfHour.Alignment = StringAlignment.Far;
            // this.m_bInitialUpdate = false;
        }

        private Rectangle AdjustRectForOverlap()
        {
            return new Rectangle();
        }

        private void AutoDragStart()
        {
            this.m_bAutoDrag = true;
            this.m_Timer = new Timer();
            this.m_Timer.Interval = 5;
            this.m_Timer.Tick += new EventHandler(this.tickEventHandler);
            this.m_Timer.Start();
        }

        private void AutoDragStop()
        {
            this.m_bAutoDrag = false;
            if (this.m_Timer != null)
            {
                this.m_Timer.Stop();
                this.m_Timer.Tick -= new EventHandler(this.tickEventHandler);
                this.m_Timer.Dispose();
                this.m_Timer = null;
            }
        }

        private void BuildGridCellsArray(Graphics g)
        {
            try
            {
                //calculate each cell's height
                SizeF ef = g.MeasureString("Test", this.m_fCell);
                this.m_cellHeight = ((int) ef.Height) + 4;

                int nColumns = this.m_nColumns; // columns set via property
                int slotsPerHour = 60 / this.m_nTimeScale; //time scale set via property
                int slotsPerDay = 24 * slotsPerHour;
                nColumns++; // add extra column for time display
                slotsPerDay++; // not sure here why that's don't

                //calculate each cell's height
                this.m_cellWidth = 600 / nColumns; // base size is 600 pixels
                // if larger:
                if (base.ClientRectangle.Width > 600)
                {
                    this.m_cellWidth = (base.ClientRectangle.Width - this.m_col0Width) / (nColumns - 1);
                }
                // if only one column
                if (this.m_nColumns == 1)
                {
                    this.m_cellWidth = base.ClientRectangle.Width - this.m_col0Width;
                }
                //next line seems to be useless (we don't use X and Y below)
                g.TranslateTransform((float) base.AutoScrollPosition.X, (float) base.AutoScrollPosition.Y);
                
                //now, build the grid cells
                for (int i = slotsPerDay; i > -1; i--)
                {
                    for (int j = 1; j < nColumns; j++)
                    {
                        int x = 0;
                        if (j == 1)
                        {
                            x = this.m_col0Width;
                        }
                        if (j > 1)
                        {
                            x = this.m_col0Width + (this.m_cellWidth * (j - 1));
                        }
                        Point point = new Point(x, i * this.m_cellHeight);
                        Rectangle r = new Rectangle(point.X, point.Y, this.m_cellWidth, this.m_cellHeight);
                        if (i != 0)
                        {
                            CGCell cell = null;
                            cell = new CGCell(r, i, j);
                            this.m_gridCells.AddCell(cell);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void CalendarGrid_DragDrop(object Sender, DragEventArgs e)
        {
            CGAppointment data = (CGAppointment) e.Data.GetData(typeof(CGAppointment));
            Point point = base.PointToClient(new Point(e.X, e.Y));
            int x = point.X - base.AutoScrollPosition.X;
            int y = point.Y - base.AutoScrollPosition.Y;
            Point pt = new Point(x, y);
            foreach (DictionaryEntry entry in this.m_gridCells.CellHashTable)
            {
                CGCell cgCell = (CGCell) entry.Value;
                if (cgCell.CellRectangle.Contains(pt))
                {
                    DateTime timeFromCell = this.GetTimeFromCell(cgCell);
                    string resourceFromColumn = this.GetResourceFromColumn(cgCell.CellColumn);
                    int duration = data.Duration;
                    TimeSpan span = new TimeSpan(0, duration, 0);
                    DateTime time2 = timeFromCell + span;
                    data.Selected = false;
                    this.m_nSelectID = 0;
                    CGAppointmentChangedArgs args = new CGAppointmentChangedArgs();
                    args.Appointment = data;
                    args.StartTime = timeFromCell;
                    args.EndTime = time2;
                    args.Resource = resourceFromColumn;
                    args.OldResource = data.Resource;
                    args.AccessTypeID = data.AccessTypeID;
                    args.Slots = data.Slots;
                    if (this.ApptDragSource == "grid")
                    {
                        this.CGAppointmentChanged(this, args);
                    }
                    else
                    {
                        this.CGAppointmentAdded(this, args);
                    }
                    break;
                }
            }
            this.SetOverlapTable();
            base.Invalidate();
        }

        private void CalendarGrid_DragEnter(object Sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CGAppointment)))
            {
                if ((e.KeyState & 8) == 8)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void CalendarGrid_DragOver(object sender, DragEventArgs e)
        {
            //Translate point to client point
            Point pt = this.PointToClient(new Point(e.X, e.Y));
            
            //clear selections
            foreach (DictionaryEntry entry in this.m_gridCells.CellHashTable)
            {
                CGCell cell = (CGCell)entry.Value;
                cell.IsSelected = false;
            }
            this.m_selectedRange.Cells.ClearAllCells();

            //select a cell based on current drag position to visually assist the user
            int nRow = -1;
            int nCol = -1;
            if (this.HitTest(pt.X, pt.Y, ref nRow, ref nCol))
            {
                CGCell cellFromRowCol = this.m_gridCells.GetCellFromRowCol(nRow, nCol);
                if (cellFromRowCol != null)
                {
                    this.m_currentCell = cellFromRowCol;
                    this.m_selectedRange.StartCell = null;
                    this.m_selectedRange.EndCell = null;
                    this.m_selectedRange.CreateRange(this.m_gridCells, cellFromRowCol, cellFromRowCol);

                    cellFromRowCol.IsSelected = true;
                }

                base.Invalidate();
            }

            //if Y axis is outside the top or bottom

            if ((pt.Y + 40 >= this.ClientRectangle.Bottom) || (pt.Y - 40 <= this.ClientRectangle.Top))
            {
                //start auto scrolling. m_bScrollDown decides whether we scroll up or down.
                this.m_bScrollDown = (pt.Y + 40) >= this.ClientRectangle.Bottom;
                AutoDragStart();
            }

            //if Y axis within client rectagle, stop dragging (whether you started or not)
            if ((pt.Y + 40 < this.ClientRectangle.Bottom) && (pt.Y - 40 > this.ClientRectangle.Top))
            {
                AutoDragStop();
            }
        }

        private void CalendarGrid_MouseDown(object sender, MouseEventArgs e)
        {
            //watch.Restart();
            if (e.Button == MouseButtons.Left)
            {
                foreach (DictionaryEntry entry in this.m_gridCells.CellHashTable)
                {
                    CGCell cell = (CGCell) entry.Value;
                    cell.IsSelected = false;
                }
                this.m_selectedRange.Cells.ClearAllCells();
                this.m_bMouseDown = true;
                this.OnLButtonDown(e.X, e.Y, true);
            }
            //new code!!! smh 4/13/2011 -- refactor later
            
            else if (e.Button == MouseButtons.Right)
            {
                // clear all selected cells, but ONLY if the the pointer is NOT over one of the cells in
                // the selected range

                int nRow = -1;
                int nCol = -1;
                CGCell cellFromRowCol = null;
                bool _isCellInRange = false;
                if (this.HitTest(e.X, e.Y, ref nRow, ref nCol))
                {
                    cellFromRowCol = this.m_gridCells.GetCellFromRowCol(nRow, nCol);
                }

                if (cellFromRowCol != null)
                    _isCellInRange = this.m_selectedRange.CellIsInRange(cellFromRowCol);

                if (!_isCellInRange)
                {
                    foreach (DictionaryEntry entry in this.m_gridCells.CellHashTable)
                    {
                        CGCell cell = (CGCell)entry.Value;
                        cell.IsSelected = false;
                    }
                    this.m_selectedRange.Cells.ClearAllCells();
                }

                // clear all selected appointments
                this.m_SelectedAppointments.ClearAllAppointments();
                foreach (CGAppointment a in this.m_Appointments.AppointmentTable.Values) a.Selected = false;
                this.m_nSelectID = 0;

                OnRButtonDown(e.X, e.Y, _isCellInRange);
            }
             
            //end new code!!! /smh 4/13/2011
        }

        private void CalendarGrid_MouseMove(object Sender, MouseEventArgs e)
        {

            //if the left mouse button is down and we are moving the mouse...
            if (this.m_bMouseDown)
            {
                //if Y axis is outside the top or bottom
                if ((e.Y >= base.ClientRectangle.Bottom) || (e.Y <= base.ClientRectangle.Top))
                {
                    //start auto scrolling. m_bScrollDown decides whether we scroll up or down.
                    this.m_bScrollDown = e.Y >= base.ClientRectangle.Bottom;
                    AutoDragStart();
                }

                //if Y axis within client rectagle, stop dragging (whether you started or not)
                if ((e.Y < base.ClientRectangle.Bottom) && (e.Y > base.ClientRectangle.Top))
                {
                    AutoDragStop();
                }
                if (this.m_bSelectingRange)
                {
                    this.OnLButtonDown(e.X, e.Y, false);
                }
                if (this.m_nSelectID != 0)
                {
                    if (this.m_bGridEnter)
                    {
                        this.m_bGridEnter = false;
                    }
                    else if (!this.m_bDragDropStart)
                    {
                        CGAppointment data = (CGAppointment) this.m_Appointments.AppointmentTable[this.m_nSelectID];
                        this.ApptDragSource = "grid";
                        base.DoDragDrop(data, DragDropEffects.Move);
                        this.m_bDragDropStart = true;
                    }
               }
            }
            else
            {
            
                AutoDragStop(); //is this needed?  //just in case maybe

                //this code below displays the tooltip if we are moving the mouse over an appointment
                int y = e.Y - base.AutoScrollPosition.Y;
                int x = e.X - base.AutoScrollPosition.X;
                Point pt = new Point(x, y);
                foreach (CGAppointment appointment2 in this.m_Appointments.AppointmentTable.Values)
                {
                    if (appointment2.GridRectangle.Contains(pt))
                    {
                        this.m_toolTip.SetToolTip(this, appointment2.ToString());
                        return;
                    }
                }
                this.m_toolTip.RemoveAll();
            }
        }

        private void CalendarGrid_MouseUp(object Sender, MouseEventArgs e)
        {
            if (this.m_bAutoDrag)
            {
                this.m_bAutoDrag = false;
                this.AutoDragStop();
            }
            this.m_bMouseDown = false;
            if (this.m_bSelectingRange)
            {
                CGSelectionChangedArgs args = new CGSelectionChangedArgs();
                args.StartTime = this.GetTimeFromCell(this.m_selectedRange.StartCell);
                args.EndTime = this.GetTimeFromCell(this.m_selectedRange.EndCell);
                args.Resource = this.GetResourceFromColumn(this.m_selectedRange.StartCell.CellColumn);
                if (args.EndTime < args.StartTime)
                {
                    DateTime startTime = args.StartTime;
                    args.StartTime = args.EndTime;
                    args.EndTime = startTime;
                }
                TimeSpan span = new TimeSpan(0, 0, this.m_nTimeScale, 0, 0);
                args.EndTime += span;
                this.CGSelectionChanged(this, args);
                this.m_bSelectingRange = false;
            }
        }

        private void CalendarGrid_Paint(object sender, PaintEventArgs e)
        {
            if (e.Graphics != null)
            {
                this.DrawGrid(e.Graphics);
                /*
                if (!this.m_bInitialUpdate)
                {
                    this.SetAppointmentTypes();
                    base.Invalidate();
                    this.m_bInitialUpdate = true;
                }
                 */
            }
        }

        public void CloseGrid()
        {
            foreach (CGAppointment appointment in this.m_Appointments.AppointmentTable.Values)
            {
                appointment.Selected = false;
            }
            this.m_nSelectID = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawAppointments(Graphics g, int col0Width, int cellWidth, int cellHeight)
        {
            if (!base.DesignMode && (this.m_Appointments != null))
            {
                int num = 0;
                int num2 = 0;
                int x = 0;
                ArrayList list = new ArrayList();
                foreach (CGAppointment appointment in this.m_Appointments.AppointmentTable.Values)
                {
                    bool bRet = false;
                    Rectangle rect = this.GetAppointmentRect(appointment, col0Width, cellWidth, cellHeight, out bRet);
                    if (bRet && (!appointment.WalkIn || this.m_bDrawWalkIns))
                    {
                        rect.Inflate(-10, 0);
                        num = (int) this.m_ApptOverlapTable[appointment.AppointmentKey];
                        num2 = rect.Right - rect.Left;
                        x = num2 / (num + 1);
                        rect.Width = x;
                        if (num > 0)
                        {
                            foreach (object obj2 in list)
                            {
                                Rectangle rectangle2 = (Rectangle) obj2;
                                if (rect.IntersectsWith(rectangle2))
                                {
                                    rect.Offset(x, 0);
                                }
                            }
                        }
                        appointment.GridRectangle = rect;
                        if (appointment.Selected)
                        {
                            Pen pen = new Pen(Brushes.Black, 5f);
                            g.DrawRectangle(pen, rect);
                            pen.Dispose();
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Blue, rect);
                        }
                        string s = appointment.ToString();
                        Rectangle rectangle3 = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
                        g.FillRectangle(Brushes.White, rectangle3);
                        Brush black = Brushes.Black;
                        if (appointment.CheckInTime.Ticks > 0L)
                        {
                            black = Brushes.Green;
                            g.FillRectangle(Brushes.LightGreen, rectangle3);
                        }
                        if (appointment.NoShow)
                        {
                            black = Brushes.Red;
                            g.FillRectangle(Brushes.LightPink, rectangle3);
                        }
                        if (appointment.WalkIn)
                        {
                            black = Brushes.Blue;
                            g.FillRectangle(Brushes.LightSteelBlue, rectangle3);
                        }
                        g.DrawString(s, this.fontArial8, black, rectangle3);
                        list.Add(rect);
                    }
                }
            }
        }

        private void DrawGrid(Graphics g)
        {
            //Default color of grid lines is black
            Pen pen = new Pen(Color.Black);

            //each cell's height is Height of Arial Font 10pt + 10 pixels (by default 26 pixels)
            SizeF ef = g.MeasureString("Test", this.m_fCell);
            int num = 10;
            this.m_cellHeight = ((int) ef.Height) + num;

            // Number of columns is dynamic based on user of Grid. See Property Columns. Default 5 in init.
            int nColumns = this.m_nColumns;

            //Time scale is also dynamic. Property TimeScale. Default 20 (minutes)
            //num3 stands for number of cells per hour
            int num3 = 60 / this.m_nTimeScale;
            //num4 stands for number of cells per day (aka rows in the grid)
            int num4 = 24 * num3;
            //Add extra column to hold time in the left hand corner
            nColumns++;
            //add extra row to represent dates or resources (depending on which view we are in)
            //Not sure of which variable controls view yet.
            num4++;

            // 100 px is reserved no matter our column sizes for displaying the time scale

            // Minimum cell width is 600/columns (100 px by default)
            this.m_cellWidth = 600 / nColumns;

            // if we happen to have more than 600 pixels in our Client Window then cell
            // is (Width-100) / (number of date columns)
            if (base.ClientRectangle.Width > 600)
            {
                this.m_cellWidth = (base.ClientRectangle.Width - this.m_col0Width) / (nColumns - 1);
            }

            // If we have one column, the cell width is the itself - 100
            if (this.m_nColumns == 1)
            {
                this.m_cellWidth = base.ClientRectangle.Width - this.m_col0Width;
            }

            // Our rectangle will start scrolling if width is less than 600 and height less than  height of all cells comb
            // Of course Height will scroll all the time unless you have a humungous screen
            base.AutoScrollMinSize = new Size(600, this.m_cellHeight * num4);

            // Default Rectangle is Gray
            g.FillRectangle(Brushes.LightGray, base.ClientRectangle);
            
            int num5 = 0; //Minutes (start at 0)
            int num6 = 0; //Hour (starts at 0)
            
            // flag is true only if there are no cells what so ever in the screen
            // Only true when no resource is selected.
            bool noCellsFlag = this.m_gridCells.CellCount == 0;

            // Move the base point from the client screen to the scrolling region top-left corner.
            g.TranslateTransform((float) base.AutoScrollPosition.X, (float) base.AutoScrollPosition.Y);

            // This for loop draws the time scale (although I haven't completely traced it out)
            // For each row except the first one (i starts from 1 rather than zero)
            for (int i = 1; i < num4; i++)
            {
                int x = 0;
                //point is (0, 1st Cell Start) then (0, 2nd Cell Start) until we run out
                Point point = new Point(x, i * this.m_cellHeight);
                //rectangle2 represents each cell rectangle
                Rectangle rectangle2 = new Rectangle(point.X, point.Y, this.m_cellWidth, this.m_cellHeight);
                //rect stands for the time scale rectangle; width is 100px; Height is length of the hour on grid
                Rectangle rect = new Rectangle(0, rectangle2.Y, this.m_col0Width, rectangle2.Height * num3);
                //height is length of hour
                int height = rect.Height;
                //Min font height is 25 pixels (100/4)--see below where it's used
                height = (height > (this.m_col0Width / 4)) ? (this.m_col0Width / 4) : height;

                //if we are the top of the time scale (at hour:00) -- num5 is min
                if (num5 == 0)
                {
                    // Fill time scale triangle with Gray (remember, this is the whole hour!)
                    g.FillRectangle(Brushes.LightGray, rect);
                    // Draw Rectangle
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    //Pad time with at least one zero to make it 2 digits
                    string str = string.Format("{0}", num6).PadLeft(2, '0');
                    //Font height using pixels. Min is 25 pixels
                    Font font = new Font("Arial", (float) height, FontStyle.Bold, GraphicsUnit.Pixel);
                    // rectangle3 is the left half of the time rectangle
                    Rectangle rectangle3 = new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
                    // In this left half, draw the hours (m_sfHour is the stringformat:
                    // Horizontal Center and Right Justified to rectangle3
                    g.DrawString(str, font, Brushes.Black, rectangle3, this.m_sfHour);
                    // Increment hour
                    num6++;
                    font.Dispose();
                }
                
                // Pad minutes with zeros to be 2 digits long
                string s = string.Format("{0}", num5);
                s = ":" + s.PadLeft(2, '0');
                // Rectangle starts at 
                // X: 2/3rds of width of Time Rectangle
                // Y: Top of the current time slot cell
                // Width: 1/3rd of the width of the Time Rectangle
                // Height: Height of a time slot
                Rectangle layoutRectangle = new Rectangle(rect.X + ((rect.Width * 2) / 3), rectangle2.Top, rect.Width / 3, rectangle2.Height);
                // At in this rectangle, write the minutes. Horizontal Ctr and Right Justified to Rectangle
                g.DrawString(s, this.m_fCell, Brushes.Black, layoutRectangle, this.m_sfRight);
                // Draw Line from two points, just under the time we have just written
                Point point2 = new Point(rect.X + ((rect.Width * 2) / 3), rectangle2.Bottom);
                Point point3 = new Point(rect.Right, rectangle2.Bottom);
                g.DrawLine(pen, point2, point3);
                // Increment the minutes with the time scale
                num5 += this.m_nTimeScale;
                // If miniutes reaches 60, reset to zero
                num5 = (num5 >= 60) ? 0 : num5;
                // When we reach the bottom (num4 - 1 is # of rows) and we are not scrolling
                if ((i == (num4 - 1)) && !this.m_bScroll)
                {
                    // Fill the last cell with Gray (?)
                    g.TranslateTransform((float) -base.AutoScrollPosition.X, (float) -base.AutoScrollPosition.Y);
                    rect = new Rectangle(0, 0, this.m_col0Width, this.m_cellHeight);
                    g.FillRectangle(Brushes.LightGray, rect);
                    g.DrawRectangle(pen, rect);
                    g.TranslateTransform((float) base.AutoScrollPosition.X, (float) base.AutoScrollPosition.Y);
                }
            }
            
            //This for loop draws the cells
            //Start from the bottom (num4 is # of rows) and go down to the zeroth row (ie date row/resource row)
            for (int j = num4; j > -1; j--)
            {
                // For each column - 1 (we start at 1, not zero-->We drew the first column anyways in the 1st loop)
                for (int k = 1; k < nColumns; k++)
                {
                    int num12 = 0;  // X-axis position
                    if (k == 1)     // If we are at the first column, start at 100px (default)
                    {
                        num12 = this.m_col0Width;
                    }
                    if (k > 1)      // if we are subsequent columns, adjust accordingly
                    {
                        num12 = this.m_col0Width + (this.m_cellWidth * (k - 1));
                    }
                    //make a rectangle for the cell
                    Point point4 = new Point(num12, j * this.m_cellHeight);
                    Rectangle r = new Rectangle(point4.X, point4.Y, this.m_cellWidth, this.m_cellHeight);
                    if (j != 0) // if we are not at the top (we are starting from the bottom)
                    {
                        CGCell cellFromRowCol = null; 
                        if (noCellsFlag)  //if there are no cells, create the cell
                        {
                            cellFromRowCol = new CGCell(r, j, k);
                            this.m_gridCells.AddCell(cellFromRowCol);
                        }
                        else // otherwise, get the cell from the m_gridCells array
                        {
                            cellFromRowCol = this.m_gridCells.GetCellFromRowCol(j, k);
                            cellFromRowCol.CellRectangle = r;
                        }
                        if (this.m_sResourcesArray.Count > 0) // if we have any resources open
                        {
                            //IMP
                            //this is the place where we the selected cells are drawn in Light Light Blue.
                            //IMP
                            // if cell is selected, draw it in Aquamarine (light light blue)
                            if (this.m_selectedRange.CellIsInRange(cellFromRowCol))
                            {
                                g.FillRectangle(Brushes.Aquamarine, r);
                                //g.FillRectangle(Brushes.AntiqueWhite, r);
                            }
                            // otherwise, draw it from Appointment Type Color set by BuildGridCellsArray()
                            else
                            {
                                g.FillRectangle(cellFromRowCol.AppointmentTypeColor, r);
                            }
                            // finally the drawing
                            g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
                            // once done with availabilities, draw the appointments
                            if (j == 1)
                            {
                                this.DrawAppointments(g, this.m_col0Width, this.m_cellWidth, this.m_cellHeight);
                            }
                        }
                        continue;
                    }
                    
                    //Below draws the top column either containing the dates or resources
                    if (!this.m_bScroll)
                    {
                        g.TranslateTransform(0f, (float) -base.AutoScrollPosition.Y);
                        Rectangle rectangle6 = r;
                        g.FillRectangle(Brushes.LightBlue, rectangle6);
                        g.DrawRectangle(pen, rectangle6.X, rectangle6.Y, rectangle6.Width, rectangle6.Height);
                        string str3 = "";
                        if (this.m_sResourcesArray.Count > 1)
                        {
                            foreach (DictionaryEntry entry in this.m_ColumnInfoTable)
                            {
                                int num13 = (int) entry.Value;
                                num13++;
                                if (num13 == k)
                                {
                                    str3 = entry.Key.ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            DateTime dtStart = this.m_dtStart;
                            if (k > 1)
                            {
                                dtStart = dtStart.AddDays((double) (k - 1));
                            }
                            string format = "ddd, MMM d";
                            str3 = dtStart.ToString(format, DateTimeFormatInfo.InvariantInfo);
                        }
                        g.DrawString(str3, this.m_fCell, Brushes.Black, rectangle6, this.m_sf);
                        g.TranslateTransform(0f, (float) base.AutoScrollPosition.Y);
                    }
                }
            }
            this.m_bScroll = false;
            pen.Dispose();
        }

        public Rectangle GetAppointmentRect(CGAppointment a, int col0Width, int cellWidth, int cellHeight, out bool bRet)
        {
            DateTime startTime = a.StartTime;
            DateTime endTime = a.EndTime;
            string resource = a.Resource;
            int originX = 0;
            int originY = 0;
            int recHeight = 0;
            int recWidth = 0;
            int columnToPutAppt = 0;
            Rectangle rectangle = new Rectangle();
            int startTotalMinutesoffset = (int) startTime.TimeOfDay.TotalMinutes;
            int endTotalMinutesoffset = (int) endTime.TimeOfDay.TotalMinutes;
            
            // To fix a bug with 1 day view: if the start time of appt is before Calendar Start Date, don't draw anything.
            if (startTime < this.m_dtStart)
            {
                bRet = false;
                return rectangle;
            }
            
            // if grid has more than one reource
            if (this.m_sResourcesArray.Count > 1)
            {
                // get zero based index
                columnToPutAppt = (int) this.m_ColumnInfoTable[resource];
                // increment to 1 based index
                columnToPutAppt++;
            }
            else
            {
                columnToPutAppt = (startTime - this.m_dtStart).Days + 1;
            }
            // this if should not get tripped; it did the same function as the new first if check. 
            //if (columnToPutAppt < 1)
            //{
            //    bRet = false;
            //    return rectangle;
            //}
            originX = col0Width + (cellWidth * (columnToPutAppt - 1));
            int num8 = startTotalMinutesoffset + this.m_nTimeScale;
            int num9 = (endTotalMinutesoffset > 0) ? endTotalMinutesoffset : 0x5a0;
            num9 -= startTotalMinutesoffset;
            originY = (cellHeight * num8) / this.m_nTimeScale;
            recHeight = (cellHeight * num9) / this.m_nTimeScale;
            recWidth = cellWidth;
            rectangle.X = originX;
            rectangle.Y = originY;
            rectangle.Width = recWidth;
            rectangle.Height = recHeight;
            bRet = true;
            return rectangle;
        }

        /// <summary>
        /// Translates a StartTime into a Cell, for coloring
        /// </summary>
        /// <param name="dDate"></param>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="bStartCell"></param>
        /// <param name="sResource"></param>
        /// <returns></returns>
        public bool GetCellFromTime(DateTime dDate, ref int nRow, ref int nCol, bool bStartCell, string sResource)
        {
            int num = (dDate.Hour * 60) + dDate.Minute;
            nRow = num / this.m_nTimeScale;
            if (bStartCell)
            {
                nRow++;
            }
            if (this.m_sResourcesArray.Count > 1)
            {
                if (sResource == "")
                {
                    sResource = this.m_sResourcesArray[0].ToString();
                }
                nCol = (int) this.m_ColumnInfoTable[sResource];
                nCol++;
                return true;
            }
            DateTime time = new DateTime(dDate.Year, dDate.Month, dDate.Day);
            TimeSpan span = (TimeSpan) (time - this.StartDate);
            int totalDays = 0;
            totalDays = (int) span.TotalDays;
            nCol = totalDays;
            nCol++;
            return true;
        }

        private string GetResourceFromColumn(int nCol)
        {
            if (this.m_sResourcesArray.Count == 1)
            {
                return this.m_sResourcesArray[0].ToString();
            }
            foreach (DictionaryEntry entry in this.m_ColumnInfoTable)
            {
                int num = (int) entry.Value;
                num++;
                if (num == nCol)
                {
                    return entry.Key.ToString();
                }
            }
            return "";
        }

        public bool GetSelectedTime(out DateTime dStart, out DateTime dEnd, out string sResource)
        {
            if (this.m_selectedRange.Cells.CellCount == 0)
            {
                dEnd = new DateTime();
                dStart = dEnd;
                sResource = "";
                return false;
            }
            CGCell startCell = this.m_selectedRange.StartCell;
            CGCell endCell = this.m_selectedRange.EndCell;
            if (startCell.CellRow > endCell.CellRow)
            {
                CGCell cell3 = startCell;
                startCell = endCell;
                endCell = cell3;
            }
            dStart = this.GetTimeFromCell(startCell);
            dEnd = this.GetTimeFromCell(endCell);
            dEnd = dEnd.AddMinutes((double) this.m_nTimeScale);
            sResource = this.GetResourceFromColumn(startCell.CellColumn);
            return true;
        }

        public bool GetSelectedType(out int nAccessTypeID)
        {
            nAccessTypeID = 0;
            if (this.m_selectedRange.Cells.CellCount == 0)
            {
                return false;
            }
            CGCell startCell = this.m_selectedRange.StartCell;
            CGCell endCell = this.m_selectedRange.EndCell;
            if (startCell.CellRow > endCell.CellRow)
            {
                CGCell cell3 = startCell;
                startCell = endCell;
                endCell = cell3;
            }
            DateTime timeFromCell = this.GetTimeFromCell(startCell);
            DateTime time2 = this.GetTimeFromCell(endCell).AddMinutes((double) this.m_nTimeScale);
            foreach (CGAvailability availability in this.m_pAvArray)
            {
                if (TimesOverlap(availability.StartTime, availability.EndTime, timeFromCell, time2))
                {
                    nAccessTypeID = availability.AvailabilityType;
                    break;
                }
            }
            return (nAccessTypeID > 0);
        }

        public DateTime GetTimeFromCell(CGCell cgCell)
        {
            int cellRow = cgCell.CellRow;
            int cellColumn = cgCell.CellColumn;
            DateTime dtStart = this.m_dtStart;
            int num3 = (cellRow - 1) * this.m_nTimeScale;
            int num4 = num3 / 60;
            if (num4 > 0)
            {
                num3 = num3 % (num4 * 60);
            }
            dtStart = dtStart.AddHours((double) num4).AddMinutes((double) num3);
            if (this.m_sResourcesArray.Count == 1)
            {
                dtStart = dtStart.AddDays((double) (cellColumn - 1));
            }
            return dtStart;
        }

        public bool GetTypeFromCell(CGCell cgCell, out int nAccessTypeID)
        {
            nAccessTypeID = 0;
            CGCell cell = cgCell;
            CGCell cell2 = cgCell;
            if (cell.CellRow > cell2.CellRow)
            {
                CGCell cell3 = cell;
                cell = cell2;
                cell2 = cell3;
            }
            DateTime timeFromCell = this.GetTimeFromCell(cell);
            DateTime time2 = this.GetTimeFromCell(cell2).AddMinutes((double) this.m_nTimeScale);
            foreach (CGAvailability availability in this.m_pAvArray)
            {
                if (TimesOverlap(availability.StartTime, availability.EndTime, timeFromCell, time2))
                {
                    nAccessTypeID = availability.AvailabilityType;
                    break;
                }
            }
            return (nAccessTypeID > 0);
        }

        private bool HitTest(int X, int Y, ref int nRow, ref int nCol)
        {
            Y -= base.AutoScrollPosition.Y;
            X -= base.AutoScrollPosition.X;
            foreach (DictionaryEntry entry in this.m_gridCells)
            {
                CGCell cell = (CGCell) entry.Value;
                if (cell.CellRectangle.Contains(X, Y))
                {
                    nRow = cell.CellRow;
                    nCol = cell.CellColumn;
                    return true;
                }
            }
            return false;
        }

        public void InitializeCalendarGrid()
        {
            this.AllowDrop = true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // CalendarGrid
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(600, 400);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CalendarGrid_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CalendarGrid_MouseMove);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CalendarGrid_DragDrop);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CalendarGrid_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CalendarGrid_MouseUp);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CalendarGrid_DragEnter);
            this.DragOver += new DragEventHandler(CalendarGrid_DragOver);
            this.ResumeLayout(false);

        }

 
        private static int MinSince80(DateTime d)
        {
            DateTime time = new DateTime(1980, 1, 1, 0, 0, 0);
            TimeSpan span = (TimeSpan) (d - time);
            return (int) span.TotalMinutes;
        }

        //new code1!! smh 4/14/2011
        private void OnRButtonDown(int X, int Y, bool RangeAlreadySelected)
        {
            //if right mouse button is clicked, select an appointment if mouse hovers over one
            foreach (CGAppointment appointment3 in this.m_Appointments.AppointmentTable.Values)
            {
                int y = Y - base.AutoScrollPosition.Y;
                int x = X - base.AutoScrollPosition.X;
                Point pt = new Point(x, y);

                if (!appointment3.GridRectangle.Contains(pt))
                {
                    continue;
                }
                this.m_bMouseDown = false;

                this.m_SelectedAppointments.AddAppointment(appointment3);
                appointment3.Selected = true;
                this.m_nSelectID = appointment3.AppointmentKey;
                //this.m_bGridEnter = true;
            }

            // if we find an appointment, redraw the grid
            if (this.m_SelectedAppointments.AppointmentCount > 0)
            {
                base.Invalidate();
                return;
            }

            // Otherwise, select a cell, but only if we don't don't have an existing range
            if (RangeAlreadySelected) return;

            // Ok, select cell here
            int nRow = -1;
            int nCol = -1;
            if (this.HitTest(X, Y, ref nRow, ref nCol))
            {
                CGCell cellFromRowCol = this.m_gridCells.GetCellFromRowCol(nRow, nCol);
                if (cellFromRowCol != null)
                {
                    this.m_currentCell = cellFromRowCol;
                    this.m_selectedRange.StartCell = null;
                    this.m_selectedRange.EndCell = null;
                    this.m_selectedRange.CreateRange(this.m_gridCells, cellFromRowCol, cellFromRowCol);

                    cellFromRowCol.IsSelected = true;
                }

                base.Invalidate();
                return;
            }
        }

        private void OnLButtonDown(int X, int Y, bool bStart)
        {
            this.m_bDragDropStart = false;
            this.m_nSelectID = 0;
            if (!this.m_bSelectingRange)
            {
                int y = Y - base.AutoScrollPosition.Y;
                int x = X - base.AutoScrollPosition.X;
                Point pt = new Point(x, y);
                if (Control.ModifierKeys == Keys.Control)
                {
                    this.m_bMouseDown = false;
                    foreach (CGAppointment appointment in this.m_Appointments.AppointmentTable.Values)
                    {
                        if (!appointment.GridRectangle.Contains(pt))
                        {
                            continue;
                        }
                        if (this.m_SelectedAppointments.AppointmentTable.ContainsKey(appointment.AppointmentKey))
                        {
                            this.m_SelectedAppointments.RemoveAppointment(appointment.AppointmentKey);
                            if (this.m_SelectedAppointments.AppointmentTable.Count == 0)
                            {
                                this.m_nSelectID = 0;
                            }
                            else
                            {
                                foreach (CGAppointment appointment2 in this.m_Appointments.AppointmentTable.Values)
                                {
                                    this.m_nSelectID = appointment2.AppointmentKey;
                                }
                            }
                        }
                        else
                        {
                            this.m_SelectedAppointments.AddAppointment(appointment);
                            this.m_nSelectID = appointment.AppointmentKey;
                        }
                        appointment.Selected = !appointment.Selected;
                        break;
                    }
                    base.Invalidate();
                    return;
                }
                foreach (CGAppointment appointment3 in this.m_Appointments.AppointmentTable.Values)
                {
                    if (!appointment3.GridRectangle.Contains(pt))
                    {
                        continue;
                    }
                    this.m_bMouseDown = false;
                    if (appointment3.Selected)
                    {
                        appointment3.Selected = false;
                        this.m_SelectedAppointments.ClearAllAppointments();
                        this.m_nSelectID = 0;
                    }
                    else
                    {
                        foreach (CGAppointment appointment4 in this.m_Appointments.AppointmentTable.Values)
                        {
                            appointment4.Selected = false;
                        }
                        this.m_SelectedAppointments.ClearAllAppointments();
                        this.m_SelectedAppointments.AddAppointment(appointment3);
                        appointment3.Selected = true;
                        this.m_nSelectID = appointment3.AppointmentKey;
                        this.m_bMouseDown = true;
                        this.m_bGridEnter = true;
                    }
                    base.Invalidate();
                    return;
                }
            }
            int nRow = -1;
            int nCol = -1;
            if (this.HitTest(X, Y, ref nRow, ref nCol))
            {
                CGCell cellFromRowCol = this.m_gridCells.GetCellFromRowCol(nRow, nCol);
                if (cellFromRowCol != null)
                {
                    if (bStart)
                    {
                        this.m_currentCell = cellFromRowCol;
                        this.m_selectedRange.StartCell = null;
                        this.m_selectedRange.EndCell = null;
                        this.m_selectedRange.CreateRange(this.m_gridCells, cellFromRowCol, cellFromRowCol);
                        bStart = false;
                        this.m_bMouseDown = true;
                        this.m_bSelectingRange = true;
                    }
                    else if (cellFromRowCol != this.m_currentCell)
                    {
                        if (!this.m_selectedRange.Cells.CellHashTable.ContainsKey(cellFromRowCol.Key))
                        {
                            this.m_selectedRange.AppendCell(this.m_gridCells, cellFromRowCol);
                        }
                        else
                        {
                            bool bUp = cellFromRowCol.CellRow < this.m_currentCell.CellRow;
                            this.m_selectedRange.SubtractCell(this.m_gridCells, cellFromRowCol, bUp);
                        }
                        this.m_currentCell = cellFromRowCol;
                    }
                    cellFromRowCol.IsSelected = true;
                    base.Invalidate();
                }
            }
        }

        public void OnUpdateArrays()
        {
            try
            {
                this.m_gridCells.ClearAllCells();
                this.SetColumnInfo();
                this.SetOverlapTable();
                Graphics g = base.CreateGraphics();
                this.BuildGridCellsArray(g);
                this.SetAppointmentTypes();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        /// <summary>
        /// Draws Availabilities. Draws Some of the Empty cells (don't know where the rest go) with the Khaki color
        /// </summary>
        private void SetAppointmentTypes()
        {
            if (this.m_gridCells.CellCount != 0)
            {
                // this happens for the CGAVView Grid
                foreach (DictionaryEntry entry in this.m_gridCells.CellHashTable)
                {
                    CGCell cell = (CGCell) entry.Value;
                    cell.AppointmentTypeColor = (this.m_GridBackColor == "blue") ? Brushes.CornflowerBlue : Brushes.Khaki;
                }
                // won't happen for CGAVView Grid b/c it has no availabilites
                // BUT, will happen for normal CGView Grid if there any availabilies
                if ((this.m_pAvArray != null) && (this.m_pAvArray.Count != 0))
                {
                    foreach (CGAvailability availability in this.m_pAvArray)
                    {
                        int nRow = 0;
                        int nCol = 0;
                        int num3 = 0;
                        int num4 = 0;
                        // pick the color from the availability
                        Brush brush = new SolidBrush(Color.FromArgb(availability.Red, availability.Green, availability.Blue));
                        // get starting and ending cell
                        this.GetCellFromTime(availability.StartTime, ref nRow, ref nCol, true, availability.ResourceList);
                        this.GetCellFromTime(availability.EndTime, ref num3, ref num4, false, availability.ResourceList);
                        // for each of the range cells between starting and ending, change their color
                        for (int i = nCol; i <= num4; i++)
                        {
                            for (int j = nRow; (i == num4) && (j <= num3); j++)
                            {
                                string str = "r" + j.ToString() + "c" + i.ToString();
                                CGCell cell2 = (CGCell) this.m_gridCells.CellHashTable[str];
                                if (cell2 != null)
                                {
                                    cell2.AppointmentTypeColor = brush;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetColumnInfo()
        {
            this.m_ColumnInfoTable.Clear();
            for (int i = 0; i < this.m_sResourcesArray.Count; i++)
            {
                this.m_ColumnInfoTable.Add(this.m_sResourcesArray[i], i);
            }
            if (this.m_sResourcesArray.Count > 1)
            {
                this.m_nColumns = this.m_sResourcesArray.Count;
            }
        }

        public void SetOverlapTable()
        {
            Hashtable hashtable = new Hashtable();
            int y = 0;
            int num2 = 0;
            int x = 0;
            foreach (CGAppointment appointment in this.m_Appointments.AppointmentTable.Values)
            {
                if (!appointment.WalkIn || this.m_bDrawWalkIns)
                {
                    string resource = appointment.Resource;
                    y = appointment.StartTime.Minute + (60 * appointment.StartTime.Hour);
                    num2 = appointment.EndTime.Minute + (60 * appointment.EndTime.Hour);
                    x = (this.m_sResourcesArray.Count > 1) ? (((int) this.m_ColumnInfoTable[resource]) + 1) : appointment.StartTime.DayOfYear;
                    Rectangle rectangle = new Rectangle(x, y, 1, num2 - y);
                    hashtable.Add(appointment.AppointmentKey, rectangle);
                }
            }
            this.m_ApptOverlapTable.Clear();
            foreach (int num4 in hashtable.Keys)
            {
                this.m_ApptOverlapTable.Add(num4, 0);
            }
            // Here it draws the Dates on Top
            if (this.m_ApptOverlapTable.Count != 0)
            {
                int num5 = (this.m_sResourcesArray.Count > 1) ? 1 : this.StartDate.DayOfYear;
                int num6 = (this.m_sResourcesArray.Count > 1) ? (this.m_sResourcesArray.Count + 1) : (this.Columns + this.StartDate.DayOfYear);
                for (int i = num5; i < num6; i++)
                {
                    ArrayList list = new ArrayList();
                    for (int j = 1; j < this.Rows; j++)
                    {
                        Rectangle rectangle2 = new Rectangle(i, j * this.m_nTimeScale, 1, this.m_nTimeScale);
                        int num9 = -1;
                        list.Clear();
                        foreach (int num10 in hashtable.Keys)
                        {
                            Rectangle rect = (Rectangle) hashtable[num10];
                            if (rectangle2.IntersectsWith(rect))
                            {
                                num9++;
                                list.Add(num10);
                            }
                        }
                        if (num9 > 0)
                        {
                            foreach (object obj2 in list)
                            {
                                int num11 = (int) obj2;
                                if (((int) this.m_ApptOverlapTable[num11]) < num9)
                                {
                                    this.m_ApptOverlapTable[num11] = num9;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles scrolling when the mouse button is down
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void tickEventHandler(object o, EventArgs e)
        {
            //if there are still WM_TIME messages in the Queue after the timer is dead, don't do anything.
            if (this.m_Timer == null) return;

            Point point = new Point(base.AutoScrollPosition.X, base.AutoScrollPosition.Y);
            int x = point.X;
            int num = point.Y * -1;
            num = this.m_bScrollDown ? (num + 2) : (num - 2);
            point.Y = num;
            base.AutoScrollPosition = point;
            base.Invalidate();
        }

        /// <summary>
        /// Do 2 time ranges overlap each other?
        /// </summary>
        /// <param name="dStart1">First Start Time</param>
        /// <param name="dEnd1">First End Time</param>
        /// <param name="dStart2">Second Start Time</param>
        /// <param name="dEnd2">Second End Time</param>
        /// <returns>True or False</returns>
        public static bool TimesOverlap(DateTime dStart1, DateTime dEnd1, DateTime dStart2, DateTime dEnd2)
        {
            long ticks = dEnd1.Ticks - dStart1.Ticks;
            TimeSpan ts = new TimeSpan(ticks);
            ticks = dEnd2.Ticks - dStart2.Ticks;
            new TimeSpan(ticks).Subtract(ts);
            Rectangle rect = new Rectangle();
            Rectangle rectangle2 = new Rectangle();
            rect.X = 0;
            rectangle2.X = 0;
            rect.Width = 1;
            rectangle2.Width = 1;
            rect.Y = MinSince80(dStart1);
            rect.Height = MinSince80(dEnd1) - rect.Y;
            rectangle2.Y = MinSince80(dStart2);
            rectangle2.Height = MinSince80(dEnd2) - rectangle2.Y;
            return rectangle2.IntersectsWith(rect);
        }

        public void PositionGrid(int nHour)
        {
            //Position grid to nHour
            int nRow = 0, nCol = 0;
            DateTime dStart = DateTime.Today;
            dStart = dStart.AddHours(nHour);
            this.GetCellFromTime(dStart, ref nRow, ref nCol, false, "");
            int nHeight = this.CellHeight + 10;
            nHeight *= nRow;
            this.AutoScrollPosition = new Point(50, nHeight);
            this.Invalidate();
        }


        /// <summary>
        /// The purpose of this is to properly draw the date boxes at the top of the calendar grid.
        /// Otherwise, when scrolling, it gets garbled.
        /// </summary>
        /// <param name="msg">Handles three messages:
        /// WM_VSCROLL (0x115 - Vertical Scrolling)
        /// WM_HSCROLL (0x114 - Horizontal Scrolling)
        /// WM_MOUSEWHEEL (0x20a - Mouse Wheel Movement)
        /// </param>
        protected override void WndProc(ref Message msg)
        {
            try
            {
                if (msg.Msg == WM_VSCROLL || msg.Msg == WM_MOUSEWHEEL)
                {
                    this.m_bScroll = true;
                    base.Invalidate(false);
                    this.m_bScroll = false;
                }
                if (msg.Msg == WM_HSCROLL)
                {
                    base.Invalidate(false);
                }
                base.WndProc(ref msg);
            }
            catch (Exception exception)
            {
                MessageBox.Show("CalendarGrid::WndProc:  " + exception.Message + "\nStack: " + exception.StackTrace);
            }
        }

        public CGAppointments Appointments
        {
            get
            {
                return this.m_Appointments;
            }
            set
            {
                this.m_Appointments = value;
            }
        }

        public string ApptDragSource
        {
            get
            {
                return this.m_sDragSource;
            }
            set
            {
                this.m_sDragSource = value;
            }
        }

        public ArrayList AvailabilityArray
        {
            get
            {
                return this.m_pAvArray;
            }
            set
            {
                this.m_pAvArray = value;
            }
        }

        public int CellHeight
        {
            get
            {
                return this.m_cellHeight;
            }
        }

        public ToolTip CGToolTip
        {
            get
            {
                return this.m_toolTip;
            }
        }

        public int Columns
        {
            get
            {
                return this.m_nColumns;
            }
            set
            {
                if ((value > 0) && (value < 11))
                {
                    this.m_nColumns = value;
                    //new line
                    this.SetColumnInfo();  // redoes the columns if we have multiple resources
                    //end new line
                    this.m_gridCells.ClearAllCells();               //remove all cells
                    this.m_selectedRange.Cells.ClearAllCells();     //remove selected range
                    Graphics g = base.CreateGraphics();
                    this.BuildGridCellsArray(g);                    //rebuild the cells
                    this.SetAppointmentTypes();                     //set the colors on the cells for availabilities
                    base.Invalidate();                              //Fire paint to call DrawGrid
                }
            }
        }

        public bool DrawWalkIns
        {
            get
            {
                return this.m_bDrawWalkIns;
            }
            set
            {
                this.m_bDrawWalkIns = value;
            }
        }

        public string GridBackColor
        {
            get
            {
                return this.m_GridBackColor;
            }
            set
            {
                this.m_GridBackColor = value;
            }
        }

        public bool GridEnter
        {
            get
            {
                return this.m_bGridEnter;
            }
            set
            {
                this.m_bGridEnter = value;
            }
        }

        public ArrayList Resources
        {
            get
            {
                return this.m_sResourcesArray;
            }
            set
            {
                this.m_sResourcesArray = value;
            }
        }

        public int Rows
        {
            get
            {
                return (0x5a0 / this.m_nTimeScale);
            }
        }

        public int SelectedAppointment
        {
            get
            {
                return this.m_nSelectID;
            }
            set
            {
                this.m_nSelectID = value;
            }
        }

        public CGAppointments SelectedAppointments
        {
            get
            {
                return this.m_SelectedAppointments;
            }
        }

        public CGRange SelectedRange
        {
            get
            {
                return this.m_selectedRange;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.m_dtStart;
            }
            set
            {
                //this.m_dtStart = value;
                this.m_dtStart = value.Date; // only date portion!!!//smh
            }
        }

        public int TimeScale
        {
            get
            {
                return this.m_nTimeScale;
            }
            set
            {
                if ((((value == 5) || (value == 10)) || ((value == 15) || (value == 20))) || ((value == 30) || (value == 60)))
                {
                    this.m_nTimeScale = value;
                    this.m_gridCells.ClearAllCells();
                    this.m_selectedRange.Cells.ClearAllCells();
                    Graphics g = base.CreateGraphics();
                    this.BuildGridCellsArray(g);
                    this.SetAppointmentTypes();
                    base.Invalidate();
                }
            }
        }

    }
}

