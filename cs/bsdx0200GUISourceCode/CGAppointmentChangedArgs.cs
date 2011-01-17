namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// Custom event args for CGAppointment events when changing appointments.
    /// More documentation when I totally understand it.
    /// I changed it to automatic properties.
    /// </summary>
    [Serializable]
    public class CGAppointmentChangedArgs : EventArgs
    {
        public int AccessTypeID {get; set;}

        public CGAppointment Appointment {get; set;}

        public DateTime EndTime {get; set;}

        public string OldResource {get; set;}

        public string Resource {get; set;}

        public int Slots {get; set;}

        public DateTime StartTime {get; set;}
    }
}

