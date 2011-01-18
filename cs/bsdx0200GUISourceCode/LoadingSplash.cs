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
    public partial class LoadingSplash : Form
    {
        public LoadingSplash()
        {
            InitializeComponent();
        }

        //Generic Delegate for fancy remote invokations
        public delegate void dAny();

        /// <summary>
        /// Close a form from another thread
        /// </summary>
        public void RemoteClose()
        {
            if (InvokeRequired == true)
            {
                dAny d = new dAny(this.Close);
                this.Invoke(d);
            }
            else
                this.Close();
        }
    }
}
