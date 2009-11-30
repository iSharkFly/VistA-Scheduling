using System;
using System.Windows.Forms;
//using RPX20Lib;
using System.Data;
//using System.Data.OleDb;
using System.Text;
using IndianHealthService.BMXNet;
using System.Reflection;
using System.Diagnostics;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Contains information about the RPMS connection
	/// </summary>
	public class CGConnectInfo
	{
		public CGConnectInfo()
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		private	bool	m_bConnected;
		string			m_sVerify;
		string			m_sAccess;
		string			m_sServerAddress;
		int				m_nServerPort;
		private	string	m_sDUZ;
		private string	m_sDUZ2;
		private	int		m_nDivisionCount = 0;
		private	string	m_sUserName;
		private	string	m_sDivision;



		public bool Connected
		{
			get
			{
				return m_bConnected;
			}
		}

		public bool LoadConnectInfo()
		{
			
			//Returns True if able to connect to RPMS


			//Step 1
			//Get RPMS Server Address and Port from Registry.
			//Prompt for them if they're not there.
			//Return False if unable to get address/port



///////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////
			
			//Old Version Below:
			//Loads and decypts M Connection info from registry
			//Tests connection
			//Sets m_bConnected based on test
			//returns m_bConnected

			//(see ConnectInfo.cpp)

			string sTempAddress;
			sTempAddress = "127.0.0.1";
//			sTempAddress = "161.223.91.10";
			int nTempPort = 0;

			// Load from registry (HKCU) 

			// Decrypt Access and Verify codes

			string sTempAccess2 = "HMWXXX8"; //TODO: Get from registry
//			sTempAccess2 = "JANXXX1"; //TODO: Get from registry
//			if (!DecryptString(pbAccessData, &lAccessSize, sTempAccess2))
//				return FALSE;
//
			string sTempVerify2 = "MOLLYB8"; //TODO: Get from registry
//			sTempVerify2 = "JANXXX2"; //TODO: Get from registry
//			if (!DecryptString(pbVerifyData, &lVerifySize, sTempVerify2))
//				return FALSE;

			m_sAccess = sTempAccess2;
			m_sVerify = sTempVerify2;
			m_sServerAddress = sTempAddress;
			if (m_sServerAddress == "") 
			{
				m_sServerAddress = "RPMSWindow";
				m_sAccess = "";
				m_sVerify = "";
			}
			m_nServerPort = nTempPort;
			if (m_nServerPort == 0)
				m_nServerPort = 9200;

//			RPX20Lib.MConnect m;
//			m = new MConnectClass();
			BMXNetLib m = new BMXNetLib();
			m.MServerPort = m_nServerPort;
			m.AppContext="BMXRPC";
			bool bRet = false;
			try 
			{
				bRet = m.OpenConnection(sTempAddress, sTempAccess2, sTempVerify2);
			}
			catch (BMXNetException exBMX)
			{
				throw exBMX;
			}
			catch (Exception bmxEx)
			{
				string sMessage =  bmxEx.Message + bmxEx.StackTrace;
				throw new BMXNetException(sMessage);
			}

			if (bRet == true){
				try {
					this.m_sAccess = sTempAccess2;
					this.m_sVerify = sTempVerify2;
					this.m_sServerAddress = sTempAddress;
					this.m_nServerPort = m.MServerPort;
					this.m_sDUZ = m.DUZ;

					string sRpc = "BMX USER";
					m_sUserName = m.TransmitRPC(sRpc, m_sDUZ);

					
					System.Data.DataTable rsDivisions;
					rsDivisions =  this.GetUserDivisions(m_sAccess, m_sVerify, m_sServerAddress, m_nServerPort);
					m_nDivisionCount = rsDivisions.Rows.Count;

					foreach (System.Data.DataRow r in rsDivisions.Rows)
					{
						string sTemp = r["MOST_RECENT_LOOKUP"].ToString();
						if (sTemp == "1")
						{
							this.m_sDivision = r["FACILITY_NAME"].ToString();
							this.m_sDUZ2 = r["FACILITY_IEN"].ToString();
							break;
						}
					}

					m_bConnected = true;
				}
				catch(Exception bmxEx)
				{
					m_bConnected = false;
					string sMessage =  bmxEx.Message + bmxEx.StackTrace;
					throw new BMXNetException(sMessage);
				}
			}

			return m_bConnected;
		}

		bool TestConnection(string sAccess, string sVerify, string sAddress, int nPort)
		{
			// Try RPMS Connection & set m_bconnected TRUE if successful
//			RPX20Lib.MConnect m;
//			m = new MConnectClass();
			BMXNetLib m = new BMXNetLib();
			bool bRet = false;
			try 
			{
				//from old MServices->Login
				m.MServerPort = nPort;
				bRet = m.OpenConnection(sAddress, sAccess, sVerify);
				this.m_sDUZ = m.DUZ;
			}
			catch(Exception ex)
			{
				Debug.Write("CConnectInfo::TestConnection: Error: " + ex.Message);
				bRet = false;
			}
			finally
			{
				m.CloseConnection();
			}
			return bRet;
		}

		private DataTable GetUserDivisions(string sTempAccess2, string sTempVerify2, string sTempAddress, int MServerPort)
		{
			try
			{
				//Connection string model:
				//"Provider=BMXODB.RPMS.1;Data source=127.0.0.1;Location=9200;Extended Properties=BMXRPC;Password=HMWXXX8^MOLLYB8"
				string sConn;
				sConn = "Data source=" + sTempAddress + ";Location=" + MServerPort.ToString() + ";Extended Properties=BMXRPC;Password=" + sTempAccess2 + "^" + sTempVerify2;
				BMXNetConnection rpmsConn = new BMXNetConnection(sConn);
				rpmsConn.Open();

				BMXNetCommand cmd = (BMXNetCommand) rpmsConn.CreateCommand();
				cmd.CommandText = "BMXGetFacRS^" + m_sDUZ;

				BMXNetDataAdapter da = new BMXNetDataAdapter();
				da.SelectCommand = cmd;

				DataSet dsDivisions = new DataSet("Divisions");
				da.Fill(dsDivisions, "DivisionTable");
				DataTable tb = dsDivisions.Tables["DivisionTable"];
				return tb;
			}
			catch (Exception bmxEx)
			{
				string sMessage =  bmxEx.Message + bmxEx.StackTrace;
				throw new BMXNetException(sMessage);

			}
		}

		public string UserName
		{
			get
			{
				return this.m_sUserName;
			}
		}

		public string DivisionName
		{
			get
			{
				return this.m_sDivision;
			}
		}

		public string GetDSN(string sAppContext)
		{
			string sDsn = "Data source=";
			if (sAppContext == "")
				sAppContext = "BMXRPC";

			if (this.m_bConnected == false)
				return sDsn.ToString();

			sDsn += this.m_sServerAddress ;
			sDsn += ";Location=";
			sDsn += this.m_nServerPort.ToString();
			sDsn += ";Extended Properties=";
			sDsn += sAppContext;
			sDsn += ";Password=";
			sDsn += this.m_sAccess;
			sDsn += "^";
			sDsn += this.m_sVerify;
			
			return sDsn;
		}

		/// <summary>
		/// String representation of DUZ
		/// </summary>
		public string DUZ
		{
			get
			{
				return m_sDUZ;
			}
		}


	}
}
