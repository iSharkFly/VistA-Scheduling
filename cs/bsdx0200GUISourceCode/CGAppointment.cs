namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Drawing;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    [Serializable]
    public class CGAppointment
    {
        private bool m_bAccessBlock;
        private bool m_bNoShow;
        private bool m_bSelected = false;
        private bool m_bWalkIn;
        public DateTime m_dAuxTime;
        public DateTime m_dCheckIn;
        private DateTime m_EndTime;
        public int m_nAccessTypeID = -1;
        private int m_nColumn;
        public int m_nKey;
        private string m_Note;
        public int m_nPatientID;
        public int m_nSlots;
        private Rectangle m_rectangle;
        public string m_sAccessTypeName;
        private string m_sHRN = "";
        private string m_sPatientName;
        public string m_sResource;
        private DateTime m_StartTime;
        private string m_Text;

        public void CreateAppointment(DateTime StartTime, DateTime EndTime, string Note, int Key, string sResource)
        {
            this.m_StartTime = StartTime;
            this.m_EndTime = EndTime;
            this.m_Note = Note;
            this.m_nKey = Key;
            this.m_sResource = sResource;
        }

        public override string ToString()
        {
            string patientName = "";
            if (this.m_bAccessBlock)
            {
                string str2 = (this.Slots == 1) ? " Slot, " : " Slots, ";
                return ((((this.AccessTypeName + ": ") + this.Slots.ToString() + str2) + this.Duration.ToString() + " Minutes. ") + this.Note);
            }
            patientName = this.PatientName;
            if (this.HealthRecordNumber != "")
            {
                patientName = patientName + " #" + this.HealthRecordNumber;
            }
            return (patientName + " " + this.Note);
        }

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

        public string AccessTypeName
        {
            get
            {
                return this.m_sAccessTypeName;
            }
            set
            {
                this.m_sAccessTypeName = value;
            }
        }

        public int AppointmentKey
        {
            get
            {
                return this.m_nKey;
            }
            set
            {
                this.m_nKey = value;
            }
        }

        public DateTime AuxTime
        {
            get
            {
                return this.m_dAuxTime;
            }
            set
            {
                this.m_dAuxTime = value;
            }
        }

        public DateTime CheckInTime
        {
            get
            {
                return this.m_dCheckIn;
            }
            set
            {
                this.m_dCheckIn = value;
            }
        }

        public int Duration
        {
            get
            {
                TimeSpan span = (TimeSpan) (this.EndTime - this.StartTime);
                return (int) span.TotalMinutes;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.m_EndTime;
            }
            set
            {
                this.m_EndTime = value;
            }
        }

        public int GridColumn
        {
            get
            {
                return this.m_nColumn;
            }
            set
            {
                this.m_nColumn = value;
            }
        }

        public Rectangle GridRectangle
        {
            get
            {
                return this.m_rectangle;
            }
            set
            {
                this.m_rectangle = value;
            }
        }

        public string HealthRecordNumber
        {
            get
            {
                return this.m_sHRN;
            }
            set
            {
                this.m_sHRN = value;
            }
        }

        public bool IsAccessBlock
        {
            get
            {
                return this.m_bAccessBlock;
            }
            set
            {
                this.m_bAccessBlock = value;
            }
        }

        public bool NoShow
        {
            get
            {
                return this.m_bNoShow;
            }
            set
            {
                this.m_bNoShow = value;
            }
        }

        public string Note
        {
            get
            {
                return this.m_Note;
            }
            set
            {
                this.m_Note = value;
            }
        }

        public int PatientID
        {
            get
            {
                return this.m_nPatientID;
            }
            set
            {
                this.m_nPatientID = value;
            }
        }

        public string PatientName
        {
            get
            {
                return this.m_sPatientName;
            }
            set
            {
                this.m_sPatientName = value;
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

        public bool Selected
        {
            get
            {
                return this.m_bSelected;
            }
            set
            {
                this.m_bSelected = value;
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
                return this.m_StartTime;
            }
            set
            {
                this.m_StartTime = value;
            }
        }

        public string Text
        {
            get
            {
                this.m_Text = this.m_sPatientName;
                return this.m_Text;
            }
        }

        public bool WalkIn
        {
            get
            {
                return this.m_bWalkIn;
            }
            set
            {
                this.m_bWalkIn = value;
            }
        }
    }
}

