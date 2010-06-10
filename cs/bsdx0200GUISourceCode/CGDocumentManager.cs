using System;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using System.Diagnostics;
using IndianHealthService.BMXNet;
using Mono.Options;
using System.Runtime.InteropServices;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DocumentManager.
	/// </summary>
	public class CGDocumentManager : System.Windows.Forms.Form
	{
		#region Member Variables

		private static CGDocumentManager	_current;
		private Hashtable					_views = new Hashtable();
		private Hashtable					m_AVViews = new Hashtable();
		private string						m_sWindowText = "Clinical Scheduling"; //Default Window Text
		private bool						m_bSchedManager;
		private bool						m_bExitOK = true;
        public string                       m_sHandle = "0";
        private string                      m_AccessCode="";
        private string                      m_VerifyCode="";
        private string                      m_Server="";
        private int                         m_Port=0;

		//M Connection member variables
		private DataSet									m_dsGlobal = null;
		private System.ComponentModel.IContainer		components = null;
		private BMXNetConnectInfo						m_ConnectInfo = null;
		private BMXNetConnectInfo.BMXNetEventDelegate	CDocMgrEventDelegate;

		#endregion

		public CGDocumentManager()
		{
			InitializeComponent();
			m_ConnectInfo = new BMXNetConnectInfo();
            //m_ConnectInfo.bmxNetLib.StartLog();    //This line turns on logging of messages
            m_bSchedManager = false;
			CDocMgrEventDelegate = new BMXNetConnectInfo.BMXNetEventDelegate(CDocMgrEventHandler);
			m_ConnectInfo.BMXNetEvent += CDocMgrEventDelegate;
			m_ConnectInfo.EventPollingEnabled = false;
        }

        #region BMXNet Event Handler
        private void CDocMgrEventHandler(Object obj, BMXNet.BMXNetEventArgs e)
		{
			if (e.BMXEvent == "BSDX CALL WORKSTATIONS")
			{
				string sParam = "";
				string sDelim="~";
				sParam += this.m_ConnectInfo.UserName + sDelim;
				sParam += this.m_sHandle + sDelim;
				sParam += Application.ProductVersion + sDelim;
				sParam += this._views.Count.ToString();
				_current.m_ConnectInfo.RaiseEvent("BSDX WORKSTATION REPORT", sParam, true);
			}
			if (e.BMXEvent == "BSDX ADMIN MESSAGE")
			{
				string sMsg = e.BMXParam;
				ShowAdminMsgDelegate samd = new ShowAdminMsgDelegate(ShowAdminMsg);
				this.Invoke(samd, new object [] {sMsg});
			}
			if (e.BMXEvent == "BSDX ADMIN SHUTDOWN")
			{
				string sMsg = e.BMXParam;
				CloseAllDelegate cad = new CloseAllDelegate(CloseAll);
				this.Invoke(cad, new object [] {sMsg});
			}
		}
		
		delegate void ShowAdminMsgDelegate(string sMsg);

		private void ShowAdminMsg(string sMsg)
		{
			MessageBox.Show(sMsg, "Message from Scheduling Administrator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion  BMXNet Event Handler

        #region Properties

        /// <summary>
		/// Returns the document manager's BMXNetConnectInfo member
		/// </summary>
		public BMXNetConnectInfo ConnectInfo
		{
			get
			{
				return m_ConnectInfo;
			}
		}

		/// <summary>
		/// True if the current user holds the BSDXZMGR or XUPROGMODE keys in RPMS
		/// </summary>
		public bool ScheduleManager
		{
			get
			{
				return m_bSchedManager;
			}
		}

		/// <summary>
		/// Holds the user and division
		/// </summary>
		public string WindowText
		{
			get
			{
				return m_sWindowText;
			}
		}

		/// <summary>
		/// This dataset contains tables used by the entire application
		/// </summary>		
		public DataSet GlobalDataSet
		{
			get
			{
				return m_dsGlobal;
			}
			set
			{
				m_dsGlobal = value;
			}
		}
        //public BMXNetConnection ADOConnection 
        //{
        //    get
        //    {
        //        return m_ADOConnection;
        //    }
        //}

		/// <summary>
		/// Returns the single CGDocumentManager object
		/// </summary>
		public static CGDocumentManager Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Returns the list of currently opened documents
		/// </summary>
		public Hashtable Views
		{
			get
			{
				return _views;
			}
		}

		/// <summary>
		/// Returns the list of currently opened CGAVViews
		/// </summary>
		public Hashtable AvailabilityViews
		{
			get
			{
				return this.m_AVViews;
			}
		}


		#endregion

		#region Methods & Events
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (m_ConnectInfo != null)
				{
					m_ConnectInfo.EventPollingEnabled = false;
					m_ConnectInfo.UnSubscribeEvent("BSDX SCHEDULE");
					m_ConnectInfo.UnSubscribeEvent("BSDX CALL WORKSTATIONS");
					m_ConnectInfo.CloseConnection();
				}
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		private void InitializeComponent()
		{
			// 
			// CGDocumentManager
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "CGDocumentManager";

		}


		private DSplash m_ds;
		public void StartSplash()
		{
			m_ds = new DSplash();
			m_ds.ShowDialog();
		}

		private void InitializeApp()
		{
			InitializeApp(false);
		}

		private void InitializeApp(bool bReLogin)
		{
			try
			{
				//Set M connection info
				//Show a splash screen while initializing
				m_ds = new DSplash();
                m_ds.Show(this);
				m_ds.SetStatus("Loading Configuration Settings...");
                m_ds.Refresh();
				this.Activate();
                System.Configuration.ConfigurationManager.GetSection("appSettings");
                m_ds.SetStatus("Connecting to VistA Server...");
                m_ds.Refresh();
				bool bRetry = true;

                //Try to connect using supplied values for Server and Port
                //Why am I doing this? The library BMX net uses prompts for access and verify code
                //whether you can connect or not. Not good. So I test first whether
                //we can connect at all by doing a simple connection and disconnect.
                //TODO: Make this more robust by sending a TCPConnect message and seeing if you get a response.

                if (m_Server != "" && m_Port != 0)
                {
                    System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
                    try
                    {
                        tcpClient.Connect(m_Server, m_Port); // open it
                        tcpClient.Close();                  // then close it
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        throw ex;
                    }
                }
				do
				{
                    try
                    {
                        if (bReLogin == true)
                        {
                            //Prompt for Access and Verify codes
                            _current.m_ConnectInfo.LoadConnectInfo("", "");
                        }
                        else
                        {
                            if (m_Server != String.Empty && m_Port != 0 && m_AccessCode != String.Empty
                                && m_VerifyCode != String.Empty)
                            {
                                m_ConnectInfo.LoadConnectInfo(m_Server, m_Port, m_AccessCode, m_VerifyCode);
                            }
                            else if (m_Server != String.Empty && m_Port != 0)
                                m_ConnectInfo.LoadConnectInfo(m_Server, m_Port, "", "");
                            else
                                m_ConnectInfo.LoadConnectInfo();
                        }
                        bRetry = false;
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        MessageBox.Show("Cannot connect to VistA. ");
                    }
                    catch (Exception ex)
                    {
                        m_ds.Close();
                        if (MessageBox.Show("Unable to connect to VistA.  " + ex.Message, "Clinical Scheduling", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                        {
                            bRetry = true;
                            _current.m_ConnectInfo.ChangeServerInfo();
                        }
                        else
                        {
                            bRetry = false;
                            throw ex;
                        }
                    }
				}while (bRetry == true);

				//Create global dataset
				_current.m_dsGlobal = new DataSet("GlobalDataSet");

				//Version info
				m_ds.SetStatus("Getting Version Info...");
                m_ds.Refresh();
                String sCmd = "BMX VERSION INFO^BSDX^";
                this.m_ConnectInfo.RPMSDataTable(sCmd, "VersionInfo", m_dsGlobal);

				//Keep the following commented code for future use:
				//How to extract the version numbers:
                //DataTable dtVersion = m_dsGlobal.Tables["VersionInfo"];
                //Debug.Assert(dtVersion.Rows.Count == 1);
                //DataRow rVersion = dtVersion.Rows[0];
                //string sMajor = rVersion["MAJOR_VERSION"].ToString();
                //string sMinor = rVersion["MINOR_VERSION"].ToString();
                //string sBuild = rVersion["BUILD"].ToString();
                //decimal fBuild = Convert.ToDecimal(sBuild);

				//Set application context
				m_ds.SetStatus("Setting Application Context to BSDXRPC...");
                m_ds.Refresh();
				m_ConnectInfo.AppContext = "BSDXRPC";
	
				//Load global recordsets
				m_ds.SetStatus("Loading VistA data tables...");
                m_ds.Refresh();
				if (_current.LoadGlobalRecordsets() == false)
				{
					MessageBox.Show("Unable to create VistA recordsets"); //TODO Improve this message
					m_ds.Close();
					return;
				}
				
				System.IntPtr pHandle = this.Handle;
				System.IntPtr pConnHandle = this.ConnectInfo.Handle;
                this.m_sHandle = pHandle.ToString();

                _current.m_ConnectInfo.ReceiveTimeout = 30000; //30-second timeout

#if DEBUG
                _current.m_ConnectInfo.ReceiveTimeout = 600000; //longer timeout for debugging
#endif 
				_current.m_ConnectInfo.SubscribeEvent("BSDX SCHEDULE");
				_current.m_ConnectInfo.SubscribeEvent("BSDX CALL WORKSTATIONS");
				_current.m_ConnectInfo.SubscribeEvent("BSDX ADMIN MESSAGE");
				_current.m_ConnectInfo.SubscribeEvent("BSDX ADMIN SHUTDOWN");

				_current.m_ConnectInfo.EventPollingInterval = 5000; //in milliseconds
				_current.m_ConnectInfo.EventPollingEnabled = true;
				_current.m_ConnectInfo.AutoFire = 12; //AutoFire every 12*5 seconds

				m_ds.Close();
			}
			catch (Exception ex)
			{
				m_ds.Close();
				Debug.Write(ex.Message);
				MessageBox.Show(ex.Message + ex.StackTrace, "Clinical Scheduling Error -- Closing Application");
				throw ex;
			}
		}

        //To write to the console
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

		[STAThread()] 
        static void Main(string[] args)
		{
#if DEBUG
            // Print console messages to console if launched from console
            AttachConsole(ATTACH_PARENT_PROCESS);
#endif
            try
            {
                 //Store the current manager
                _current = new CGDocumentManager();
                
                //Get command line options; store in private variables
                var opset = new OptionSet () {
                    { "s=", s => _current.m_Server = s },
                    { "p=", p => _current.m_Port = int.Parse(p) },
                    { "a=", a => _current.m_AccessCode = a },
                    { "v=", v => _current.m_VerifyCode = v }
                };

                opset.Parse(args);
                
                try
                {
                    _current.InitializeApp();
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    return;
                }

                //Create the first empty document
                CGDocument doc = new CGDocument();
                doc.DocManager = _current;
                doc.OnNewDocument();
                Application.DoEvents();

                //Run the application
                Application.Run();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                MessageBox.Show(ex.Message + ex.StackTrace, "CGDocumentManager.Main(): Clinical Scheduling Error -- Closing Application");
                return;
            }
		}

		public void LoadAccessTypesTable()
		{
			string sCommandText = "SELECT * FROM BSDX_ACCESS_TYPE";
			ConnectInfo.RPMSDataTable(sCommandText, "AccessTypes", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- AccessTypes loaded\n");
		}

		public void LoadAccessGroupsTable()
		{
			string sCommandText = "SELECT * FROM BSDX_ACCESS_GROUP";
			ConnectInfo.RPMSDataTable(sCommandText, "AccessGroup", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- AccessGroups loaded\n");
		}

		public void LoadAccessGroupTypesTable()
		{
			string sCommandText = "BSDX GET ACCESS GROUP TYPES";
			ConnectInfo.RPMSDataTable(sCommandText, "AccessGroupType", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- AccessGroupTypes loaded\n");
		}

        //TODO:REMOVE THIS
		public void LoadClinicSetupTable()
		{
			string sCommandText = "BSDX CLINIC SETUP";
			ConnectInfo.RPMSDataTable(sCommandText, "ClinicSetupParameters", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ClinicSetupParameters loaded\n");
		}

		public void LoadBSDXResourcesTable()
		{
			string sCommandText = "BSDX RESOURCES^" + m_ConnectInfo.DUZ;
			ConnectInfo.RPMSDataTable(sCommandText, "Resources", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- Resources loaded\n");
		}
		
		public void LoadResourceGroupTable()
		{
			//ResourceGroup Table (Resource Groups by User)
			//Table "ResourceGroup" contains all resource group names
			//to which user has access
			//Fields are: RESOURCE_GROUPID, RESOURCE_GROUP
			string sCommandText = "BSDX RESOURCE GROUPS BY USER^" + m_ConnectInfo.DUZ;
			ConnectInfo.RPMSDataTable(sCommandText, "ResourceGroup", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ResourceGroup loaded\n");
		}

		public void LoadGroupResourcesTable()
		{
			//Table "GroupResources" contains all active GROUP/RESOURCE combinations
			//to which user has access based on entries in BSDX RESOURCE USER file
			//If user has BSDXZMGR or XUPROGMODE keys, then ALL Group/Resource combinstions
			//are returned.
			//Fields are: RESOURCE_GROUPID, RESOURCE_GROUP, RESOURCE_GROUP_ITEMID, RESOURCE_NAME, RESOURCE_ID
			string sCommandText = "BSDX GROUP RESOURCE^" + m_ConnectInfo.DUZ;
			ConnectInfo.RPMSDataTable(sCommandText, "GroupResources", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- GroupResources loaded\n");
		}

		public void LoadScheduleUserTable()
		{
			//Table "ScheduleUser" contains an entry for each user in File 200 (NEW PERSON)
			//who possesses the BSDXZMENU security key.
			string sCommandText = "BSDX SCHEDULE USER";
			ConnectInfo.RPMSDataTable(sCommandText, "ScheduleUser", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ScheduleUser loaded\n");
		}

		public void LoadResourceUserTable()
		{
			//Table "ResourceUser" duplicates the BSDX RESOURCE USER File.
			//NOTE: Column names are RESOURCEUSER_ID, RESOURCEID, 
			//						 OVERBOOK, MODIFY_SCHEDULE, USERID, USERID1
			//string sCommandText = "SELECT BMXIEN RESOURCEUSER_ID, INTERNAL[RESOURCENAME] RESOURCEID, OVERBOOK, MODIFY_SCHEDULE, USERNAME USERID, INTERNAL[USERNAME] FROM BSDX_RESOURCE_USER";
			LoadResourceUserTable(false);
		}

		public void LoadResourceUserTable(bool bAllUsers)
		{
			string sCommandText = "SELECT BMXIEN RESOURCEUSER_ID, RESOURCENAME, INTERNAL[RESOURCENAME] RESOURCEID, OVERBOOK, MODIFY_SCHEDULE, MODIFY_APPOINTMENTS, USERNAME, INTERNAL[USERNAME] USERID FROM BSDX_RESOURCE_USER";
			ConnectInfo.RPMSDataTable(sCommandText, "ResourceUser", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ResourceUser loaded\n");
		}

		private bool LoadGlobalRecordsets() 
		{
			//Schedule User Info
			string sCommandText = "BSDX SCHEDULING USER INFO^" + m_ConnectInfo.DUZ;
			DataTable dtUser = ConnectInfo.RPMSDataTable(sCommandText, "SchedulingUser", m_dsGlobal);

			Debug.Assert(dtUser.Rows.Count == 1);
			DataRow rUser = dtUser.Rows[0];
			Object oUser = rUser["MANAGER"];
			string sUser = oUser.ToString();
			m_bSchedManager = (sUser == "YES")?true:false;

			//AccessTypes
			LoadAccessTypesTable();

			//Build Primary Key for AccessTypes table
			DataTable dtTypes = m_dsGlobal.Tables["AccessTypes"];
			DataColumn dcKey = dtTypes.Columns["BMXIEN"];
			DataColumn[] dcKeys = new DataColumn[1];
			dcKeys[0] = dcKey;
			dtTypes.PrimaryKey = dcKeys;

			//AccessGroups
			LoadAccessGroupsTable();

			//Build Primary Key for AccessGroup table
			DataTable dtGroups = m_dsGlobal.Tables["AccessGroup"];
			dcKey = dtGroups.Columns["ACCESS_GROUP"];
			dcKeys = new DataColumn[1];
			dcKeys[0] = dcKey;
			dtGroups.PrimaryKey = dcKeys;

			//AccessGroupType
			LoadAccessGroupTypesTable();

			//Build Primary Key for AccessGroupType table
			DataTable dtAGTypes = m_dsGlobal.Tables["AccessGroupType"];
			DataColumn dcGTKey = dtAGTypes.Columns["ACCESS_GROUP_TYPEID"];
			DataColumn[] dcGTKeys = new DataColumn[1];
			dcGTKeys[0] = dcGTKey;
			dtAGTypes.PrimaryKey = dcGTKeys;

			//Build Data Relationship between AccessGroupType and AccessTypes tables
			DataRelation dr = new DataRelation("AccessGroupType",	//Relation Name
				m_dsGlobal.Tables["AccessGroup"].Columns["BMXIEN"],	//Parent
				m_dsGlobal.Tables["AccessGroupType"].Columns["ACCESS_GROUP_ID"]);	//Child
			m_dsGlobal.Relations.Add(dr);

			//ResourceGroup Table (Resource Groups by User)
			LoadResourceGroupTable();
			
			//Resources by user
			LoadBSDXResourcesTable();

			//Build Primary Key for Resources table
			DataColumn[] dc = new DataColumn[1];
			dc[0] = m_dsGlobal.Tables["Resources"].Columns["RESOURCEID"];
			m_dsGlobal.Tables["Resources"].PrimaryKey = dc;

			//GroupResources table
			LoadGroupResourcesTable();

			//Build Primary Key for ResourceGroup table
			dc = new DataColumn[1];
			dc[0] = m_dsGlobal.Tables["ResourceGroup"].Columns["RESOURCE_GROUP"];
			m_dsGlobal.Tables["ResourceGroup"].PrimaryKey = dc;
			
			//Build Data Relationships between ResourceGroup and GroupResources tables
			dr = new DataRelation("GroupResource",	//Relation Name
				m_dsGlobal.Tables["ResourceGroup"].Columns["RESOURCE_GROUP"],	//Parent
				m_dsGlobal.Tables["GroupResources"].Columns["RESOURCE_GROUP"]);	//Child
			CGSchedLib.OutputArray(m_dsGlobal.Tables["GroupResources"], "GroupResources");
			m_dsGlobal.Relations.Add(dr);

			//HospitalLocation table
			//cmd.CommandText = "SELECT BMXIEN 'HOSPITAL_LOCATION_ID', NAME 'HOSPITAL_LOCATION', DEFAULT_PROVIDER, STOP_CODE_NUMBER, INACTIVATE_DATE, REACTIVATE_DATE FROM HOSPITAL_LOCATION";
			sCommandText = "BSDX HOSPITAL LOCATION";
			ConnectInfo.RPMSDataTable(sCommandText, "HospitalLocation", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- HospitalLocation loaded\n");

			//Build Primary Key for HospitalLocation table
			dc = new DataColumn[1];
			DataTable dtTemp = m_dsGlobal.Tables["HospitalLocation"];
			dc[0] = dtTemp.Columns["HOSPITAL_LOCATION_ID"];
			m_dsGlobal.Tables["HospitalLocation"].PrimaryKey = dc;

			LoadClinicSetupTable();

			//Build Primary Key for ClinicSetupParameters table
			dc = new DataColumn[1];
			dtTemp = m_dsGlobal.Tables["ClinicSetupParameters"];
			dc[0] = dtTemp.Columns["HOSPITAL_LOCATION_ID"];
			m_dsGlobal.Tables["ClinicSetupParameters"].PrimaryKey = dc;

			//Build Data Relationships between ClinicSetupParameters and HospitalLocation tables
			dr = new DataRelation("HospitalLocationClinic",	//Relation Name
				m_dsGlobal.Tables["HospitalLocation"].Columns["HOSPITAL_LOCATION_ID"],	//Parent
				m_dsGlobal.Tables["ClinicSetupParameters"].Columns["HOSPITAL_LOCATION_ID"], false);	//Child
			m_dsGlobal.Relations.Add(dr);

			dtTemp.Columns.Add("PROVIDER", System.Type.GetType("System.String"), "Parent.DEFAULT_PROVIDER");
			dtTemp.Columns.Add("CLINIC_STOP", System.Type.GetType("System.String"), "Parent.STOP_CODE_NUMBER");
			dtTemp.Columns.Add("INACTIVATE_DATE", System.Type.GetType("System.String"), "Parent.INACTIVATE_DATE");
			dtTemp.Columns.Add("REACTIVATE_DATE", System.Type.GetType("System.String"), "Parent.REACTIVATE_DATE");

			//Build Data Relationships between Resources and HospitalLocation tables
			dr = new DataRelation("HospitalLocationResource",	//Relation Name
				m_dsGlobal.Tables["HospitalLocation"].Columns["HOSPITAL_LOCATION_ID"],	//Parent
				m_dsGlobal.Tables["Resources"].Columns["HOSPITAL_LOCATION_ID"], false);	//Child
			m_dsGlobal.Relations.Add(dr);

			//Build ScheduleUser table
			this.LoadScheduleUserTable();

			//Build Primary Key for ScheduleUser table
			dc = new DataColumn[1];
			dtTemp = m_dsGlobal.Tables["ScheduleUser"];
			dc[0] = dtTemp.Columns["USERID"];
			m_dsGlobal.Tables["ScheduleUser"].PrimaryKey = dc;

			//Build ResourceUser table
			this.LoadResourceUserTable();

			//Build Primary Key for ResourceUser table
			dc = new DataColumn[1];
			dtTemp = m_dsGlobal.Tables["ResourceUser"];
			dc[0] = dtTemp.Columns["RESOURCEUSER_ID"];
			m_dsGlobal.Tables["ResourceUser"].PrimaryKey = dc;

			//Create relation between BSDX Resource and BSDX Resource User tables
			dr = new DataRelation("ResourceUser",	//Relation Name
				m_dsGlobal.Tables["Resources"].Columns["RESOURCEID"],	//Parent
				m_dsGlobal.Tables["ResourceUser"].Columns["RESOURCEID"]);	//Child
			m_dsGlobal.Relations.Add(dr);

			//Build active provider table
			sCommandText = "SELECT BMXIEN, NAME FROM NEW_PERSON WHERE INACTIVE_DATE = ''";
			ConnectInfo.RPMSDataTable(sCommandText, "Provider", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- Provider loaded\n");

			//Build the CLINIC_STOP table
			// sCommandText = "SELECT BMXIEN, CODE, NAME FROM CLINIC_STOP"; //SMH
            sCommandText = "SELECT BMXIEN, AMIS_REPORTING_STOP_CODE, NAME FROM CLINIC_STOP";
			ConnectInfo.RPMSDataTable(sCommandText, "ClinicStop", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ClinicStop loaded\n");

			//Build the HOLIDAY table
			sCommandText = "SELECT NAME, DATE FROM HOLIDAY WHERE DATE > '" + DateTime.Today.ToShortDateString() + "'";
			ConnectInfo.RPMSDataTable(sCommandText, "HOLIDAY", m_dsGlobal);
            Debug.Write("LoadingGlobalRecordsets -- Holidays loaded\n");

            sCommandText = @"SELECT HOSPITAL_LOCATION.BMXIEN 'BMXIEN', HOSPITAL_LOCATION.PROVIDER.PROVIDER 'PROVIDER', HOSPITAL_LOCATION.PROVIDER.DEFAULT_PROVIDER 'DEFAULT' FROM HOSPITAL_LOCATION";
            ConnectInfo.RPMSDataTable(sCommandText, "ClinicProviders", m_dsGlobal);
            Debug.Write("LoadingGlobalRecordsets -- ClinicProviders loaded\n");

			//Save the xml schema
			//m_dsGlobal.WriteXmlSchema(@"..\..\csSchema20060526.xsd");

			return true;
		}

		public void RegisterDocumentView(CGDocument doc, CGView view)
		{			
			//Store the view in the list of views
			this.Views.Add(view, doc);

			//Hook into the view's 'closed' event
			view.Closed += new EventHandler(ViewClosed);

			//Hook into the view's mnuRPMSServer.Click event
			view.mnuRPMSServer.Click += new EventHandler(mnuRPMSServer_Click);

			//Hook into the view's mnuRPMSLogin.Click event
			view.mnuRPMSLogin.Click += new EventHandler(mnuRPMSLogin_Click);

		}

		public void RegisterAVDocumentView(CGAVDocument doc, CGAVView view)
		{
			//Store the view in the list of views
			this.AvailabilityViews.Add(view, doc);

			//Hook into the view's 'closed' event
			view.Closed += new EventHandler(AVViewClosed);
		}

		public CGAVView GetAVViewByResource(ArrayList sResourceArray)
		{
			if (sResourceArray == null)
				return null;

			bool bEqual = true;
			foreach (CGAVView v in m_AVViews.Keys)
			{
				CGAVDocument d = v.Document;

				bEqual = false;
				if (d.Resources.Count == sResourceArray.Count)
				{
					bEqual = true;
					for (int j = 0; j < sResourceArray.Count; j++)
					{
						if (sResourceArray.Contains(d.Resources[j]) == false)
						{
							bEqual = false;
							break;
						}
						if (d.Resources.Contains(sResourceArray[j]) == false)
						{
							bEqual = false;
							break;
						}
					}					
					if (bEqual == true)
						return v;
				}
			}
			return null;
		}
		/// <summary>
		/// Return the first view having a resource array matching sResourceArray
		/// </summary>
		/// <param name="sResourceArray"></param>
		/// <returns></returns>
		public CGView GetViewByResource(ArrayList sResourceArray)
		{
			if (sResourceArray == null)
				return null;

			bool bEqual = true;
			foreach (CGView v in _views.Keys)
			{
				CGDocument d = v.Document;

				bEqual = false;
				if (d.Resources.Count == sResourceArray.Count)
				{
					bEqual = true;
					for (int j = 0; j < sResourceArray.Count; j++)
					{
						if (sResourceArray.Contains(d.Resources[j]) == false)
						{
							bEqual = false;
							break;
						}
						if (d.Resources.Contains(sResourceArray[j]) == false)
						{
							bEqual = false;
							break;
						}
					}					
					if (bEqual == true)
						return v;
				}
			}
			return null;
		}

		private void ViewClosed(object sender, EventArgs e)
		{
			//Remove the sender from our document list
			Views.Remove(sender);

			//If no documents left, then close RPMS connection & exit the application
			if ((Views.Count == 0)&&(this.AvailabilityViews.Count == 0)&&(m_bExitOK == true))
			{
				m_ConnectInfo.EventPollingEnabled = false;
				m_ConnectInfo.UnSubscribeEvent("BSDX SCHEDULE");
				m_ConnectInfo.CloseConnection();
				Application.Exit();
			}
		}

		private void AVViewClosed(object sender, EventArgs e)
		{
			//Remove the sender from our document list
			this.AvailabilityViews.Remove(sender);

			//If no documents left, then close RPMS connection & exit the application
			if ((Views.Count == 0)&&(this.AvailabilityViews.Count == 0)&&(m_bExitOK == true))
			{
				m_ConnectInfo.bmxNetLib.CloseConnection();
				Application.Exit();
			}
		}

		private void KeepAlive()
		{
			foreach (CGView v in _views.Keys)
			{
				CGDocument d = v.Document;
				DateTime dNow = DateTime.Now;
				DateTime dLast = d.LastRefreshed;
				TimeSpan tsDiff = dNow - dLast;
				if (tsDiff.Seconds > 180)
				{				
					for (int j = 0; j < d.Resources.Count; j++)
					{
						v.RaiseRPMSEvent("SCHEDULE-" + d.Resources[j].ToString(), "");
					}

					break;
				}
			}		
		}

		/// <summary>
		/// Propogate availability updates to all sRresource's doc/views 
		/// </summary>
		public void UpdateViews(string sResource, string sOldResource)
		{
			if (sResource == null)
				return;
			foreach (CGView v in _views.Keys)
			{
				CGDocument d = v.Document;
				for (int j = 0; j < d.Resources.Count; j++)
				{
					if ((sResource == "") || (sResource == ((string) d.Resources[j])) || (sOldResource == ((string) d.Resources[j])))
					{
						d.RefreshDocument();
						break;
					}
				}
				v.UpdateTree();
			}
		}
		
		/// <summary>
		/// Propogate availability updates to all doc/views 
		/// </summary>
		public void UpdateViews()
		{
			UpdateViews("","");
			foreach (CGView v in _views.Keys)
			{
				v.UpdateTree();
			}
		}

		/// <summary>
		/// Calls each view associated with document Doc and closes it.
		/// </summary>		
		public void CloseAllViews(CGDocument doc)
		{
			//iterate through all views and call update.
			Hashtable h = CGDocumentManager.Current.Views;
			
			CGDocument d;
			int nTempCount = h.Count;
			do
			{
				nTempCount = h.Count;
				foreach (CGView v in h.Keys)
				{
					d = (CGDocument) h[v];
					if (d == doc)
					{
						v.Close();
						break;
					}
				}
			} while ((h.Count > 0) && (nTempCount != h.Count));
		}

		/// <summary>
		/// Calls each view associated with Availability Doc and closes it.
		/// </summary>		
		public void CloseAllViews(CGAVDocument doc)
		{
			//iterate through all views and call update.
			Hashtable h = CGDocumentManager.Current.AvailabilityViews;
			
			CGAVDocument d;
			int nTempCount = h.Count;
			do
			{
				nTempCount = h.Count;
				foreach (CGAVView v in h.Keys)
				{
					d = (CGAVDocument) h[v];
					if (d == doc)
					{
						v.Close();
						break;
					}
				}
			} while ((h.Count > 0) && (nTempCount != h.Count));


		}

		private void mnuRPMSServer_Click(object sender, EventArgs e)
		{
			//Warn that changing servers will close all schedules
			if (MessageBox.Show("Are you sure you want to close all schedules and connect to a different VistA server?", "Clinical Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				return;

			//Reconnect to RPMS and recreate all global recordsets
			try
			{
				m_bExitOK = false;
				bool bRetry = true;
				BMXNetConnectInfo tmpInfo;
				do
				{
					tmpInfo = m_ConnectInfo;
					try
					{
						tmpInfo.ChangeServerInfo();
						bRetry = false;
					}
					catch (Exception ex)
					{
						if (ex.Message == "User cancelled.")
						{
							bRetry = false;
							return;
						}
						if (MessageBox.Show("Unable to connect to VistA.  " + ex.Message , "Clinical Scheduling", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
						{
							bRetry = true;
						}
						else
						{
							bRetry = false;
							return;
						}
					}
				} while (bRetry == true);

				CloseAll();
				m_bExitOK = true;
				m_ConnectInfo = tmpInfo;

				this.InitializeApp();

				//Create a new document
				CGDocument doc = new CGDocument();
				doc.DocManager = _current;
				doc.OnNewDocument();

			}
			catch (Exception ex)
			{
				throw ex;
			}
	
		}

		private void mnuRPMSLogin_Click(object sender, EventArgs e)
		{
			//Warn that changing login will close all schedules
			if (MessageBox.Show("Are you sure you want to close all schedules and login to VistA?", "Clinical Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				return;

			//Reconnect to RPMS and recreate all global recordsets
			try
			{
				m_bExitOK = false;
				CloseAll();
				m_bExitOK = true;
				_current.m_ConnectInfo = new BMXNet.BMXNetConnectInfo();
				this.InitializeApp(true);
				//Create a new document
				CGDocument doc = new CGDocument();
				doc.DocManager = _current;
				doc.OnNewDocument();
			}
			catch (Exception ex)
			{
				throw ex;
			}
	
		}

		delegate void CloseAllDelegate(string sMsg);
		
		private void CloseAll(string sMsg)
		{
			if (sMsg == "")
			{
				sMsg = "Scheduling System Shutting Down Immediately for Maintenance.";
			}

			MessageBox.Show(sMsg, "Clinical Scheduling Administrator -- System Shutdown Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			CloseAll();
		}

		private void CloseAll()
		{
			//Close all documents, views and connections
			Hashtable h = CGDocumentManager.Current.Views;
			int nTempCount = h.Count;
			do
			{
				nTempCount = h.Count;
				foreach (CGView v in h.Keys)
				{
					v.Close();
					break;
				}
			} while ((h.Count > 0) && (nTempCount != h.Count));

			h = CGDocumentManager.Current.AvailabilityViews;
			nTempCount = h.Count;
			do
			{
				nTempCount = h.Count;
				foreach (CGAVView v in h.Keys)
				{
					v.Close();
					break;
				}
			} while ((h.Count > 0) && (nTempCount != h.Count));

		}

		delegate DataTable RPMSDataTableDelegate(string CommandString, string TableName);

		public DataTable RPMSDataTable(string sSQL, string sTableName)
		{
			//Retrieves a recordset from RPMS
			string			sErrorMessage = "";
			try
			{
				System.IntPtr pHandle = this.Handle;
				DataTable dtOut;
				RPMSDataTableDelegate rdtd = new RPMSDataTableDelegate(ConnectInfo.RPMSDataTable);
				dtOut = (DataTable) this.Invoke(rdtd, new object[] {sSQL, sTableName});
				return dtOut;
			}
			catch (Exception ex)
			{
				sErrorMessage = "CGDocumentManager.RPMSDataTable error: " + ex.Message;
				throw ex;
			}
		}

		public void ChangeDivision(System.Windows.Forms.Form frmCaller)
		{
			this.ConnectInfo.ChangeDivision(frmCaller);
			foreach (CGView v in _views.Keys)
			{
				v.InitializeDocView(v.Document.DocName);
				v.Document.RefreshDocument();
			}
		}

		public void ViewRefresh()
		{
			foreach (CGView v in _views.Keys)
			{
				try
				{
					v.Document.RefreshDocument();
				}
				catch (Exception ex)
				{
					Debug.Write("CGDocumentManager.ViewRefresh Exception: " + ex.Message + "\n");
				}
				finally
				{
				}
			}
			Debug.Write("DocManager refreshed all views.\n");
		}

		#endregion Methods & Events

	}
}
