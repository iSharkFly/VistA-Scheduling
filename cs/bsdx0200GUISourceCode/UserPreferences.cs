using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Designed to keep user preferences. Gets settings from DB in constructor; Writes them back when changed.
    /// </summary>
    public class UserPreferences
    {
        bool _printAppointmentSlipAutomacially;
        bool _printRoutingSlipAutomatically;
        
        public UserPreferences()
        {
            _printAppointmentSlipAutomacially = CGDocumentManager.Current.DAL.AutoPrintAppointmentSlip;
            _printRoutingSlipAutomatically = CGDocumentManager.Current.DAL.AutoPrintRoutingSlip;
            
        }

        public bool PrintAppointmentSlipAutomacially {
            get { return _printAppointmentSlipAutomacially; }
            set
            {
                CGDocumentManager.Current.DAL.AutoPrintAppointmentSlip = _printAppointmentSlipAutomacially = value; 
            }
        }

        public bool PrintRoutingSlipAutomatically
        {
            get { return _printRoutingSlipAutomatically; }
            set 
            { 
                CGDocumentManager.Current.DAL.AutoPrintRoutingSlip = _printRoutingSlipAutomatically = value; 
            }
        }
    }
}
