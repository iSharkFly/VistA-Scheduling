namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class CGRange
    {
        private CGCells m_Cells;
        private CGCell m_gcEnd;
        private CGCell m_gcStart;

        public CGRange()
        {
            this.m_Cells = new CGCells();
        }

        public CGRange(CGCells gridCells, CGCell gcStart, CGCell gcEnd)
        {
            this.CreateRange(gridCells, gcStart, gcEnd);
        }

        public void AppendCell(CGCells gridCells, CGCell aCell)
        {
            if ((aCell != this.StartCell) && (aCell.CellColumn == this.StartCell.CellColumn))
            {
                CGCell startCell = this.StartCell;
                this.m_Cells.ClearAllCells();
                this.CreateRange(gridCells, startCell, aCell);
            }
        }

        public bool CellIsInRange(CGCell cgCell)
        {
            return this.m_Cells.CellHashTable.ContainsKey(cgCell.Key);
        }

        public void CreateRange(CGCells gridCells, CGCell sCell, CGCell eCell)
        {
            this.m_Cells.ClearAllCells();
            this.m_Cells.AddCell(sCell);
            this.m_gcStart = sCell;
            this.m_gcEnd = eCell;
            if (sCell != eCell)
            {
                int num;
                CGCell r = null;
                if (sCell.CellRow < eCell.CellRow)
                {
                    for (num = sCell.CellRow + 1; num <= eCell.CellRow; num++)
                    {
                        r = gridCells.GetCellFromRowCol(num, eCell.CellColumn);
                        this.m_Cells.AddCell(r);
                    }
                }
                else
                {
                    for (num = sCell.CellRow - 1; num >= eCell.CellRow; num--)
                    {
                        r = gridCells.GetCellFromRowCol(num, eCell.CellColumn);
                        this.m_Cells.AddCell(r);
                    }
                }
            }
        }

        public void SubtractCell(CGCells gridCells, CGCell aCell, bool bUp)
        {
            int nRow = bUp ? (this.m_gcEnd.CellRow - 1) : (this.m_gcEnd.CellRow + 1);
            int cellColumn = this.m_gcEnd.CellColumn;
            this.Cells.RemoveCell(this.m_gcEnd.Key);
            this.m_gcEnd = gridCells.GetCellFromRowCol(nRow, cellColumn);
        }

        public CGCells Cells
        {
            get
            {
                return this.m_Cells;
            }
        }

        public CGCell EndCell
        {
            get
            {
                return this.m_gcEnd;
            }
            set
            {
                this.m_gcEnd = value;
            }
        }

        public CGCell StartCell
        {
            get
            {
                return this.m_gcStart;
            }
            set
            {
                this.m_gcStart = value;
            }
        }
    }
}

