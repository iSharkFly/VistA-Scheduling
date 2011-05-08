namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Drawing;
    /// <summary>
    /// Data Structure to Represent an Appointment
    /// </summary>
    [Serializable]
    public class CGAppointment
    {
        public int AccessTypeID { get; set; }
        public string AccessTypeName { get; set; }

        public int AppointmentKey { get; set; }

        public DateTime AuxTime { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }

        public int GridColumn { get; set; }
        public Rectangle GridRectangle { get; set; }
        
        public bool IsAccessBlock { get; set; }

        public bool NoShow { get; set; }

        public string Note { get; set; }

        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string Resource { get; set; }
        public string HealthRecordNumber { get; set; }
        
        public bool Selected { get; set; }

        public int Slots { get; set; }

        public bool WalkIn { get; set; }

        public Patient Patient { get; set; }
        public Provider Provider { get; set; }

        public int? RadiologyExamIEN { get; set; }


        public CGAppointment()
        {
            AccessTypeID = -1;
            Selected = false;
            HealthRecordNumber = "";
            }

        public void CreateAppointment(DateTime StartTime, DateTime EndTime, string Note, int Key, string sResource)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.Note = Note;
            this.AppointmentKey = Key;
            this.Resource = sResource;
            }

        public int Duration
        {
            get
            {
                TimeSpan span = (TimeSpan) (this.EndTime - this.StartTime);
                return (int) span.TotalMinutes;
            }
            }

        public override string ToString()
        {
            string patientName = "";
            if (this.IsAccessBlock)
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
            }
}

