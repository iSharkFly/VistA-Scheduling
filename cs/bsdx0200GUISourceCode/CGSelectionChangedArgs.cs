namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    [Serializable]
    public class CGSelectionChangedArgs : EventArgs
    {
        private DateTime m_dEnd;
        private DateTime m_dStart;
        private string m_sResource;

        public DateTime EndTime
        {
            get
            {
                return this.m_dEnd;
            }
            set
            {
                this.m_dEnd = value;
            }
        }

        public string Resource
        {
            get
            {
                return this.m_sResource;
            }
            set
            {
                this.m_sResource = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.m_dStart;
            }
            set
            {
                this.m_dStart = value;
            }
        }
    }
}

