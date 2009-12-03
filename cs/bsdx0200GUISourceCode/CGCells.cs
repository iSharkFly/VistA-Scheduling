namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Collections;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class CGCells : IEnumerable
    {
        private Hashtable cellList = new Hashtable();

        internal CGCells()
        {
        }

        public void AddCell(CGCell r)
        {
            this.cellList.Add(r.Key, r);
        }

        public void ClearAllCells()
        {
            this.cellList.Clear();
        }

        public CGCell GetCellFromRowCol(int nRow, int nCol)
        {
            string str = CGCell.BuildKey(nRow, nCol);
            return (CGCell) this.cellList[str];
        }

        public IEnumerator GetEnumerator()
        {
            return this.cellList.GetEnumerator();
        }

        public void RemoveCell(string sKey)
        {
            this.cellList.Remove(sKey);
        }

        public int CellCount
        {
            get
            {
                return this.cellList.Count;
            }
        }

        public Hashtable CellHashTable
        {
            get
            {
                return this.cellList;
            }
        }
    }
}

