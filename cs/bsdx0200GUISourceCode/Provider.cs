using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Provider puppet
    /// </summary>
    [Serializable]
    public class Provider
    {
        public int IEN { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
