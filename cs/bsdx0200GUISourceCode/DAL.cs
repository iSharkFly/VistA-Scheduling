using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Diagnostics;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Data Access Layer
    /// </summary>
    public class DAL
    {
        private BMXNetConnectInfo _thisConnection; // set in constructor
        delegate DataTable RPMSDataTableDelegate(string CommandString, string TableName); // for use in calling (Sync and Async)

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn">The current connection to use</param>
        public DAL(BMXNetConnectInfo conn)
        {
            this._thisConnection = conn;
        }
        
        public DataTable GetVersion(string nmsp)
        {
            string cmd = String.Format("BMX VERSION INFO^{0}", nmsp);
            return RPMSDataTable(cmd, "");
        }

        public DataTable GetUserInfo(string DUZ)
        {
            string cmd = String.Format("BSDX SCHEDULING USER INFO^{0}", DUZ);
            return RPMSDataTable(cmd, "");
        }

        public DataTable GetAccessTypes()
        {
            string sCommandText = "SELECT * FROM BSDX_ACCESS_TYPE";
            DataTable table = RPMSDataTable(sCommandText, "");
            DataColumn dcKey = table.Columns["BMXIEN"];
            DataColumn[] dcKeys = new DataColumn[1];
            dcKeys[0] = dcKey;
            table.PrimaryKey = dcKeys;
            return table;
        }



        /// <summary>
        /// Workhorse
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="sTableName"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        private DataTable RPMSDataTable(string sSQL, string sTableName)
        {
            //Retrieves a recordset from RPMS
            string sErrorMessage = "";
            DataTable dtOut;

#if TRACE
            DateTime sendTime = DateTime.Now;
#endif
            try
            {
                RPMSDataTableDelegate rdtd = new RPMSDataTableDelegate(_thisConnection.RPMSDataTable);
                dtOut = (DataTable)rdtd.Invoke(sSQL, sTableName);
            }

            catch (Exception ex)
            {
                sErrorMessage = "CGDocumentManager.RPMSDataTable error: " + ex.Message;
                throw ex;
            }

#if TRACE
            DateTime receiveTime = DateTime.Now;
            TimeSpan executionTime = receiveTime - sendTime;
            Debug.Write("RPMSDataTable Execution Time: " + executionTime.Milliseconds + " ms.\n");
#endif

            return dtOut;

        }


    }
}
    

