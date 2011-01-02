using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndianHealthService.ClinicalScheduling
{
    public partial class DMassSlotDelete : Form
    {
        public DMassSlotDelete()
        {
            InitializeComponent();
        }
        
        public DateTime StartDate { get { return this.dtStart.Value; } }
        public DateTime EndDate   { get { return this.dtEnd.Value;   } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (EndDate < StartDate)
            {
                errorProvider.SetError(dtEnd, "End Date cannot be before Start Date");
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
        
    }
}
