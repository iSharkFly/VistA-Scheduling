namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Drawing;
    using System.Text;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class CGCell
    {
        private Brush m_ApptTypeColor;
        public bool m_bIsSelected;
        private int m_Col;
        private Rectangle m_Rectangle;
        private int m_Row;
        private string m_sKey;

        public CGCell()
        {
            this.m_ApptTypeColor = Brushes.Cornsilk;
        }

        public CGCell(Rectangle r, int row, int col)
        {
            this.m_Rectangle = r;
            this.m_Row = row;
            this.m_Col = col;
            this.m_sKey = BuildKey(this.m_Row, this.m_Col);
            this.m_ApptTypeColor = Brushes.Cornsilk;
        }

        public static string BuildKey(int nRow, int nCol)
        {
            StringBuilder builder = new StringBuilder("r");
            builder.Append(nRow.ToString());
            builder.Append("c");
            builder.Append(nCol.ToString());
            return builder.ToString();
        }

        public Brush AppointmentTypeColor
        {
            get
            {
                return this.m_ApptTypeColor;
            }
            set
            {
                this.m_ApptTypeColor = value;
            }
        }

        public int CellColumn
        {
            get
            {
                return this.m_Col;
            }
            set
            {
                this.m_Col = value;
                this.m_sKey = BuildKey(this.m_Row, this.m_Col);
            }
        }

        public Rectangle CellRectangle
        {
            get
            {
                return this.m_Rectangle;
            }
            set
            {
                this.m_Rectangle = value;
            }
        }

        public int CellRow
        {
            get
            {
                return this.m_Row;
            }
            set
            {
                this.m_Row = value;
                this.m_sKey = BuildKey(this.m_Row, this.m_Col);
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.m_bIsSelected;
            }
            set
            {
                this.m_bIsSelected = value;
            }
        }

        public string Key
        {
            get
            {
                return this.m_sKey;
            }
        }
    }
}

