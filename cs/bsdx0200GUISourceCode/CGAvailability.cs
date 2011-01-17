namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Drawing;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class CGAvailability
    {
        private DateTime m_EndTime;
        private int m_nAvailabilityType;
        private int m_nBlue;
        private int m_nGreen;
        private int m_nRed;
        private int m_nSlots;
        private string m_sAccessRuleList;
        private string m_sAccessTypeName;
        private string m_sDisplayColor = "Cornsilk";
        private string m_sNote;
        private string m_sResourceList;
        private DateTime m_StartTime;

        public CGAvailability()
        {
            Color color = Color.FromName("Khaki");
            this.m_nRed = color.R;
            this.m_nGreen = color.G;
            this.m_nBlue = color.B;
            this.m_sNote = "";
        }

        public void Create(DateTime StartTime, DateTime EndTime, int nSlots)
        {
            this.m_StartTime = StartTime;
            this.m_EndTime = EndTime;
            this.m_nAvailabilityType = 0;
            this.m_nSlots = nSlots;
            this.m_sResourceList = "";
            this.m_sAccessRuleList = "";
        }

        public void Create(DateTime StartTime, DateTime EndTime, int nAvailabilityType, int nSlots)
        {
            this.m_StartTime = StartTime;
            this.m_EndTime = EndTime;
            this.m_nAvailabilityType = nAvailabilityType;
            this.m_nSlots = nSlots;
            this.m_sResourceList = "";
            this.m_sAccessRuleList = "";
        }

        public void Create(DateTime StartTime, DateTime EndTime, int nAvailabilityType, int nSlots, string sResourceList)
        {
            this.m_StartTime = StartTime;
            this.m_EndTime = EndTime;
            this.m_nAvailabilityType = nAvailabilityType;
            this.m_nSlots = nSlots;
            this.m_sResourceList = sResourceList;
            this.m_sAccessRuleList = "";
        }

        public void Create(DateTime StartTime, DateTime EndTime, int nAvailabilityType, int nSlots, string sResourceList, string sAccessRuleList)
        {
            this.m_StartTime = StartTime;
            this.m_EndTime = EndTime;
            this.m_nAvailabilityType = nAvailabilityType;
            this.m_nSlots = nSlots;
            this.m_sResourceList = sResourceList;
            this.m_sAccessRuleList = sAccessRuleList;
        }

        public string AccessRuleList
        {
            get
            {
                return this.m_sAccessRuleList;
            }
            set
            {
                this.m_sAccessRuleList = value;
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

        public int AvailabilityType
        {
            get
            {
                return this.m_nAvailabilityType;
            }
            set
            {
                this.m_nAvailabilityType = value;
            }
        }

        public int Blue
        {
            get
            {
                return this.m_nBlue;
            }
            set
            {
                this.m_nBlue = value;
            }
        }

        public string DisplayColor
        {
            get
            {
                return this.m_sDisplayColor;
            }
            set
            {
                this.m_sDisplayColor = value;
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

        public int Green
        {
            get
            {
                return this.m_nGreen;
            }
            set
            {
                this.m_nGreen = value;
            }
        }

        public string Note
        {
            get
            {
                return this.m_sNote;
            }
            set
            {
                this.m_sNote = value;
            }
        }

        public int Red
        {
            get
            {
                return this.m_nRed;
            }
            set
            {
                this.m_nRed = value;
            }
        }

        public string ResourceList
        {
            get
            {
                return this.m_sResourceList;
            }
            set
            {
                this.m_sResourceList = value;
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

        public override string ToString()
        {
            return ResourceList + " (" + Slots + ") @ " + StartTime;
        }
    }
}

