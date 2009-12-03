namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    public class CGResource
    {
        private bool m_bInactive = false;
        private int m_nHospitalLocationID = 0;
        private int m_nResourceID = 0;
        private int m_nTimeScale = 15;
        private string m_sCancellationLetterText;
        private string m_sHospitalLocation = "";
        private string m_sLetterText;
        private string m_sNoShowLetterText;
        private string m_sResourceName = "";

        public string CancellationLetterText
        {
            get
            {
                return this.m_sCancellationLetterText;
            }
            set
            {
                this.m_sCancellationLetterText = value;
            }
        }

        public string HospitalLocation
        {
            get
            {
                return this.m_sHospitalLocation;
            }
            set
            {
                this.m_sHospitalLocation = value;
            }
        }

        public int HospitalLocationID
        {
            get
            {
                return this.m_nHospitalLocationID;
            }
            set
            {
                this.m_nHospitalLocationID = value;
            }
        }

        public bool Inactive
        {
            get
            {
                return this.m_bInactive;
            }
            set
            {
                this.m_bInactive = value;
            }
        }

        public string LetterText
        {
            get
            {
                return this.m_sLetterText;
            }
            set
            {
                this.m_sLetterText = value;
            }
        }

        public string NoShowLetterText
        {
            get
            {
                return this.m_sNoShowLetterText;
            }
            set
            {
                this.m_sNoShowLetterText = value;
            }
        }

        public int ResourceID
        {
            get
            {
                return this.m_nResourceID;
            }
            set
            {
                this.m_nResourceID = value;
            }
        }

        public string ResourceName
        {
            get
            {
                return this.m_sResourceName;
            }
            set
            {
                this.m_sResourceName = value;
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
                this.m_nTimeScale = value;
            }
        }
    }
}

