namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    [Serializable]
    public class CGAppointmentChangedArgs : EventArgs
    {
        private DateTime m_dEnd;
        private DateTime m_dStart;
        private int m_nAccessTypeID;
        private int m_nSlots;
        private CGAppointment m_pAppt;
        private string m_sOldResource;
        private string m_sResource;

        public int AccessTypeID
        {
            get
            {
                return this.m_nAccessTypeID;
            }
            set
            {
                this.m_nAccessTypeID = value;
            }
        }

        public CGAppointment Appointment
        {
            get
            {
                return this.m_pAppt;
            }
            set
            {
                this.m_pAppt = value;
            }
        }

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

        public string OldResource
        {
            get
            {
                return this.m_sOldResource;
            }
            set
            {
                this.m_sOldResource = value;
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

        public int Slots
        {
            get
            {
                return this.m_nSlots;
            }
            set
            {
                this.m_nSlots = value;
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

