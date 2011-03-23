using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Designed to keep user preferences. Very basic for now.
    /// </summary>
    public class UserPreferences
    {
        public UserPreferences()
        {
            PrintAppointmentSlipAutomacially = false;
            PrintRoutingSlipAutomatically = false;
        }

        public bool PrintAppointmentSlipAutomacially { get; set; }
        public bool PrintRoutingSlipAutomatically { get; set; }
    }
}
