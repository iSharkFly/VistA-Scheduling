using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{

    /// <summary>
    /// You guessed it.
    /// </summary>
    public enum Sex
    {
        Male, Female
    };

    /// <summary>
    /// Puppet standing for a Real Patient
    /// </summary>
    [Serializable]
    public class Patient
    {
        public int DFN { get; set; }
        public string Name { get; set; }
        public Sex Sex;
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
        public TimeSpan Age
        {
            get 
            { 
                return (DateTime.Today - this.DOB); 
            }
        }

        /// <summary>
        /// Returns User Friendly Age. If Age < 5, then Years and Months
        /// If Age > 5, then only Years.
        /// Humans tend to round down their ages. So I follow the same rule here.
        /// </summary>
        public string UserFriendlyAge
        {
            get
            {
                if (Age.TotalDays / 365.24 > 5)
                    return Math.Floor((Age.TotalDays / 365.24)).ToString() + " " + strings.years;
                else
                    return Math.Floor((Age.TotalDays / 365.24)).ToString() + " " + strings.years + " " + strings.and + " "
                     + Math.Floor(Age.TotalDays % 365.24 / 30).ToString() + " " + strings.months;
            }
        }
    }
}
