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
    /// <summary>
    /// This form displays the RPC Events found in RPCLogger
    /// </summary>
    public partial class RPCLoggerView : Form
    {
        public RPCLoggerView()
        {
            InitializeComponent();
            lstRPCEvents.BeginUpdate();     // Stop redrawing
            foreach (var eventItem in CGDocumentManager.Current.RPCLogger.Logger) lstRPCEvents.Items.Add(eventItem); // Add the stuff
            lstRPCEvents.EndUpdate();       // Draw Again

            //We are interested in event HaveMoreData. Each time it happens, it means we have an extra item we need to add.
            CGDocumentManager.Current.RPCLogger.HaveMoreData += new EventHandler<RPCLogger.EventToLog>(RPCLogger_HaveMoreData);
        }

        // Dummmy delegate for the method below to use in this.Invoke
        delegate void dAny(object s, RPCLogger.EventToLog e);

        /// <summary>
        /// Adds the new RPC event to Listbox
        /// </summary>
        /// <param name="sender">this is the RPCLogger Object. It's not used</param>
        /// <param name="e">That's the custom logged event.</param>
        void RPCLogger_HaveMoreData(object sender, RPCLogger.EventToLog e)
        {
            if (this.InvokeRequired)
            {
                dAny d = new dAny(this.RPCLogger_HaveMoreData);
                this.Invoke(d, new object[] { sender, e });
                return;
            }

            lstRPCEvents.Items.Add(e);
        }

        /// <summary>
        /// Puts the text of the event in the text box
        /// </summary>
        /// <param name="sender">useless</param>
        /// <param name="e">useless</param>
        private void lstRPCEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            RPCLogger.EventToLog l = lstRPCEvents.SelectedItem as RPCLogger.EventToLog;
            if (l == null) return;
            txtRPCEvent.Text = l.Lines + "\r\n" + l.Exception ?? ""; 
        }
    }
}
