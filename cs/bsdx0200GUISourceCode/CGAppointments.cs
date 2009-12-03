namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Collections;
    /// <summary>
    /// This class was regenerated from Calendargrid.dll using Reflector.exe
    /// by Sam Habiel for WorldVista. The original source code is lost.
    /// </summary>
    [Serializable]
    public class CGAppointments : IEnumerable
    {
        private Hashtable apptList = new Hashtable();

        public void AddAppointment(CGAppointment appt)
        {
            if (this.apptList.ContainsKey(appt.AppointmentKey))
            {
                this.apptList.Remove(appt.AppointmentKey);
            }
            this.apptList.Add(appt.AppointmentKey, appt);
        }

        public void ClearAllAppointments()
        {
            this.apptList.Clear();
        }

        public CGAppointment GetAppointment(int nKey)
        {
            return (CGAppointment) this.apptList[nKey];
        }

        public IEnumerator GetEnumerator()
        {
            return this.apptList.GetEnumerator();
        }

        public void RemoveAppointment(int nKey)
        {
            this.apptList.Remove(nKey);
        }

        public int AppointmentCount
        {
            get
            {
                return this.apptList.Count;
            }
        }

        public Hashtable AppointmentTable
        {
            get
            {
                return this.apptList;
            }
        }
    }
}

