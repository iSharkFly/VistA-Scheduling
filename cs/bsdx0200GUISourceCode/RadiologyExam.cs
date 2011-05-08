/* Licensed under LGPL */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{   
    /// <summary>
    /// Class to stand for a Radiology Exam Order from file 75.1 (RAD/NUC MED ORDERS)
    /// IEN: IEN in file 75.1
    /// Status: Text: Pending or Hold
    /// Procedure: Text
    /// RequestDate: Time procedure was requested by physician
    /// </summary>
    public class RadiologyExam
    {
        public int IEN { get; set; }
        public string Status { get; set; }
        public string Procedure { get; set; }
        public DateTime RequestDate { get; set; }

        public override string ToString()
        {
            return Procedure + "\t" + "Requested: " + RequestDate.ToString();
        }
    }
}
