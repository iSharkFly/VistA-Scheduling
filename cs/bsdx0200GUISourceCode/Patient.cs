using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Puppet standing for a Real Patient
    /// </summary>
    public class Patient
    {
        public int DFN { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string ID { get; set; }
        public string HRN { get; set; }
        public List<CGAppointment> Appointments { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string WorkPHone { get; set; }
        public string CellPhone { get; set; }
    }
}
