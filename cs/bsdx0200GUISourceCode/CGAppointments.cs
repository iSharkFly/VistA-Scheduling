namespace IndianHealthService.ClinicalScheduling
{
    using System;
    using System.Collections;
    /// <summary>
    /// Managers Appointment objects CGAppointment using an array list internally.
    /// </summary>
    /// <remarks>
    /// Really needs to be refactored to use generics
    /// </remarks>
    [Serializable]
    public class CGAppointments : IEnumerable, ICloneable
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

 
        /// <summary>
        /// Returns a deep copy of CGAppointments
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            CGAppointments newappts = new CGAppointments();
            foreach (DictionaryEntry d in this.apptList)
            {
                newappts.apptList.Add(d.Key, d.Value);
            }
            
            return newappts;
        }
    }
}

