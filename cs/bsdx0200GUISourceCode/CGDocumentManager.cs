using System;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;
using IndianHealthService.BMXNet;
using Mono.Options;
using System.Runtime.InteropServices;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Main Worker. Handles sub-forms.
	/// </summary>
	public class CGDocumentManager //: System.Windows.Forms.Form
	{
		#region Member Variables

		private static CGDocumentManager	_current;
		private Hashtable					_views = new Hashtable();       //Returns the list of currently opened documents
		private Hashtable					m_AVViews = new Hashtable();    // List of currently opened CGAVViews
		private string						m_sWindowText = "Clinical Scheduling"; //Default Window Text
		private bool						m_bSchedManager = false;    // Do you have the XUPROGMODE or BSDXZMGR?
		private bool						m_bExitOK = true;           // Okay to exit program? Used to control Re-logins. Default true.
        public string                       m_sHandle = "0";            // Not Used
        
        //Connection variables (tied to command line parameters /a /v /s /p /e)
        private string                      m_AccessCode="";
        private string                      m_VerifyCode="";
        private string                      m_Server="";
        private int                         m_Port=0;
        private string                      m_Encoding="";  //Encoding is "" by default;

        //Data Access Layer
        private DAL                         _dal = null;

		//M Connection member variables
		private DataSet									m_dsGlobal = null;      // Holds all user data
		private BMXNetConnectInfo						m_ConnectInfo = null;   // Connection to VISTA object
        private BMXNetConnectInfo.BMXNetEventDelegate CDocMgrEventDelegate;     // Delegate to respond to messages from VISTA. Responds to event: BMXNetConnectInfo.BMXNetEvent

		#endregion

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

        public DAL DAL
        {
            get { return this._dal; }
        }


        #endregion

        /// <summary>
        /// Constructor. Does absolutely nothing at this point.
        /// </summary>
		public CGDocumentManager()
		{
        }


#if DEBUG
        //To write to the console
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
#endif
        /// <summary>
        /// Main Entry Point
        /// </summary>
        /// <param name="args">We accept the following Arguments:
        /// /s or -s = Server ip address or name
        /// /p or -p = port number (must be numeric)
        /// /a or -a = Access Code
        /// /v or -v = Verify Code
        /// /e or -e = Encoding (name of encoding as known to windows, such as windows-1256)
        /// </param>
        /// <remarks>
        /// Encoding decision is complex. This is the order of priority:
        /// - If the M DB runs in UTF-8, that's what we are going to use.
        /// - If that's not so, /e sets the default encoding. If /e is a non-existent encoding, move forward.
        /// - If /e is not supplied or is not recognized, the default encoding is the Windows default Encoding for the user.
        /// </remarks>
        [STAThread()]
        static void Main(string[] args)
        {
#if DEBUG
            // Print console messages to console if launched from console
            // Note: Imported From kernel32.dll
            AttachConsole(ATTACH_PARENT_PROCESS);
#endif
            //Store a class instance of manager. Actual constructor does nothing.
            _current = new CGDocumentManager();

            //Get command line options; store in private variables
            var opset = new OptionSet() {
                { "s=", s => _current.m_Server = s },
                { "p=", p => _current.m_Port = int.Parse(p) },
                { "a=", a => _current.m_AccessCode = a },
                { "v=", v => _current.m_VerifyCode = v },
                { "e=", e => _current.m_Encoding = e}
            };

            opset.Parse(args);

            
            _current.InitializeApp();

            //Create the first empty document
            CGDocument doc = new CGDocument();
            doc.DocManager = _current;
            doc.OnNewDocument();
            Application.DoEvents();

            //Run the application
            Application.Run();
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
				//this.Invoke(samd, new object [] {sMsg});
                samd.Invoke(sMsg);
			}
			if (e.BMXEvent == "BSDX ADMIN SHUTDOWN")
			{
				string sMsg = e.BMXParam;
				CloseAllDelegate cad = new CloseAllDelegate(CloseAll);
				//this.Invoke(cad, new object [] {sMsg});
                cad.Invoke(sMsg);
			}
		}
		
		delegate void ShowAdminMsgDelegate(string sMsg);

		private void ShowAdminMsg(string sMsg)
		{
			MessageBox.Show(sMsg, "Message from Scheduling Administrator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion  BMXNet Event Handler


		#region Methods & Events

        
		private void StartSplash(object form)
		{
            ((DSplash)form).ShowDialog();
		}

        /// <summary>
        /// See InitializeApp(bool) below
        /// </summary>
		private void InitializeApp()
		{
			InitializeApp(false);
		}

		/// <summary>
		/// Does a million things:
        /// 1. Starts Connection and displays log-in dialogs
        /// 2. Starts Splash screen
        /// 3. Loads data tables
		/// </summary>
		/// <param name="bReLogin">Is the User logging in again from a currently running instance?
        /// If so, display a dialog to collect access and verify codes.</param>
        private void InitializeApp(bool bReLogin)
		{
            //Set M connection info
            m_ConnectInfo = new BMXNetConnectInfo(m_Encoding); // Encoding is "" unless passed in command line
            _dal = new DAL(m_ConnectInfo);   // Data access layer
            //m_ConnectInfo.bmxNetLib.StartLog();    //This line turns on logging of messages
            
            //Create a delegate to process events raised by BMX.
            CDocMgrEventDelegate = new BMXNetConnectInfo.BMXNetEventDelegate(CDocMgrEventHandler);
            //Tie delegate to Events generated by BMX.
            m_ConnectInfo.BMXNetEvent += CDocMgrEventDelegate;
            //Disable polling (But does this really work???? I don't see how it gets disabled)
            m_ConnectInfo.EventPollingEnabled = false;

            //Show a splash screen while initializing
            DSplash m_ds = new DSplash();
            DSplash.dSetStatus setStatusDelegate = new DSplash.dSetStatus(m_ds.SetStatus);
            DSplash.dAny closeSplashDelegate = new DSplash.dAny(m_ds.RemoteClose);
            DSplash.dAny hideSplashDelegate = new DSplash.dAny(m_ds.RemoteHide);

            Thread threadSplash = new Thread(new ParameterizedThreadStart(StartSplash));
            threadSplash.IsBackground = true; //expendable -- exit even if still running.
            threadSplash.Start(m_ds);

            
          	//m_ds.SetStatus("Loading Configuration Settings...");
            //m_ds.Refresh();
			//this.Activate();
            // smh--not used System.Configuration.ConfigurationManager.GetSection("appSettings");
            setStatusDelegate("Connecting to VISTA");
            //m_ds.Refresh();
			bool bRetry = true;

            //Try to connect using supplied values for Server and Port
            //Why am I doing this? The library BMX net uses prompts for access and verify code
            //whether you can connect or not. Not good. So I test first whether
            //we can connect at all by doing a simple connection and disconnect.
            //TODO: Make this more robust by sending a TCPConnect message and seeing if you get a response.

            //m_ds.Refresh();

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
                // login crap
                try
                {
                    // Not my code
                    if (bReLogin == true)
                    {
                        //Prompt for Access and Verify codes
                        _current.m_ConnectInfo.LoadConnectInfo("", "");
                    }
                    // My code -- buts looks so ugly!
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
                    //m_ds.Close();
                    if (MessageBox.Show("Unable to connect to VistA.  " + ex.Message, "Clinical Scheduling", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    {
                        bRetry = true;
                        _current.m_ConnectInfo.ChangeServerInfo();
                    }
                    else
                    {
                        closeSplashDelegate();
                        bRetry = false;
                        throw ex;
                    }
                }
			}while (bRetry == true);
            
            //Create global dataset
			_current.m_dsGlobal = new DataSet("GlobalDataSet");

			//Version info
            //m_ds.Activate();
			setStatusDelegate("Getting Version Info from Server...");

            DataTable ver = _dal.GetVersion("BSDX");
            ver.TableName = "VersionInfo";
            m_dsGlobal.Tables.Add(ver);
                
			//How to extract the version numbers:
            DataTable dtVersion = m_dsGlobal.Tables["VersionInfo"];
            Debug.Assert(dtVersion.Rows.Count == 1);
            DataRow rVersion = dtVersion.Rows[0];
            string sMajor = rVersion["MAJOR_VERSION"].ToString();
            string sMinor = rVersion["MINOR_VERSION"].ToString();
            string sBuild = rVersion["BUILD"].ToString();
            decimal fBuild = Convert.ToDecimal(sBuild);

            //Make sure that the server is running the same version the client is.
            Version x = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            //if version numbers mismatch, don't continue.
            //TODO: For future: Include in v. 1.5
            /*
            if (!(x.Major.ToString() == sMajor && x.Minor.ToString() + x.Build.ToString() == sMinor))
            {
                MessageBox.Show(
                    "Server runs version " + sMajor + "." + sMinor + "\r\n" +
                    "You are running " + x.ToString() + "\r\n\r\n" +
                    "Major, Minor and Build versions must match",
                    "Version Mismatch");
                m_ds.Close();
                return;
            }
            */
 

            //Change encoding
            setStatusDelegate("Setting encoding...");

            if (m_Encoding == String.Empty)
            {
                string utf8_server_support = m_ConnectInfo.bmxNetLib.TransmitRPC("BMX UTF-8", "");
                if (utf8_server_support == "1")
                    m_ConnectInfo.bmxNetLib.Encoder = System.Text.UTF8Encoding.UTF8;
            }
			//Set application context
			setStatusDelegate("Setting Application Context to BSDXRPC...");
			m_ConnectInfo.AppContext = "BSDXRPC";
	
			//Load global recordsets
            string statusConst = "Loading VistA data tables...";
			setStatusDelegate(statusConst);

            string sCommandText;

            setStatusDelegate(statusConst + " Schedule User");
            //Schedule User Info
            DataTable dtUser = _dal.GetUserInfo(m_ConnectInfo.DUZ);
            dtUser.TableName = "SchedulingUser";
            m_dsGlobal.Tables.Add(dtUser);
            Debug.Assert(dtUser.Rows.Count == 1);

            // Only one row and one column named "MANAGER". Set local var m_bSchedManager to true if Manager.
            DataRow rUser = dtUser.Rows[0];
            Object oUser = rUser["MANAGER"];
            string sUser = oUser.ToString();
            m_bSchedManager = (sUser == "YES") ? true : false;

            setStatusDelegate(statusConst + " Access Types");
            //Get Access Types
            DataTable dtAccessTypes = _dal.GetAccessTypes();
            dtAccessTypes.TableName = "AccessTypes";
            m_dsGlobal.Tables.Add(dtAccessTypes);

            setStatusDelegate(statusConst + " Access Groups");
            //AccessGroups
            LoadAccessGroupsTable();

            //Build Primary Key for AccessGroup table
            DataTable dtGroups = m_dsGlobal.Tables["AccessGroup"];
            DataColumn dcKey = dtGroups.Columns["ACCESS_GROUP"];
            DataColumn[] dcKeys = new DataColumn[1];
            dcKeys[0] = dcKey;
            dtGroups.PrimaryKey = dcKeys;

            setStatusDelegate(statusConst + " Access Group Types");
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

            setStatusDelegate(statusConst + " Resource Groups By User");
            //ResourceGroup Table (Resource Groups by User)
            LoadResourceGroupTable();

            setStatusDelegate(statusConst + " Resources By User");
            //Resources by user
            LoadBSDXResourcesTable();

            //Build Primary Key for Resources table
            DataColumn[] dc = new DataColumn[1];
            dc[0] = m_dsGlobal.Tables["Resources"].Columns["RESOURCEID"];
            m_dsGlobal.Tables["Resources"].PrimaryKey = dc;

            setStatusDelegate(statusConst + " Group Resources");
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

            setStatusDelegate(statusConst + " Clinics");
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

            //Build Data Relationships between Resources and HospitalLocation tables
            dr = new DataRelation("HospitalLocationResource",	//Relation Name
                m_dsGlobal.Tables["HospitalLocation"].Columns["HOSPITAL_LOCATION_ID"],	//Parent
                m_dsGlobal.Tables["Resources"].Columns["HOSPITAL_LOCATION_ID"], false);	//Child
            m_dsGlobal.Relations.Add(dr);

            setStatusDelegate(statusConst + " Schedule User");
            //Build ScheduleUser table
            this.LoadScheduleUserTable();

            //Build Primary Key for ScheduleUser table
            dc = new DataColumn[1];
            dtTemp = m_dsGlobal.Tables["ScheduleUser"];
            dc[0] = dtTemp.Columns["USERID"];
            m_dsGlobal.Tables["ScheduleUser"].PrimaryKey = dc;

            setStatusDelegate(statusConst + " Resource User");
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

            setStatusDelegate(statusConst + " Providers");
            //Build active provider table
            sCommandText = "SELECT BMXIEN, NAME FROM NEW_PERSON WHERE INACTIVE_DATE = '' AND BMXIEN > 1";
            ConnectInfo.RPMSDataTable(sCommandText, "Provider", m_dsGlobal);
            Debug.Write("LoadGlobalRecordsets -- Provider loaded\n");

            setStatusDelegate(statusConst + " Clinic Stops");
            //Build the CLINIC_STOP table
            // sCommandText = "SELECT BMXIEN, CODE, NAME FROM CLINIC_STOP"; //SMH
            sCommandText = "SELECT BMXIEN, AMIS_REPORTING_STOP_CODE, NAME FROM CLINIC_STOP";
            ConnectInfo.RPMSDataTable(sCommandText, "ClinicStop", m_dsGlobal);
            Debug.Write("LoadGlobalRecordsets -- ClinicStop loaded\n");

            setStatusDelegate(statusConst + " Holiday");
            //Build the HOLIDAY table
            sCommandText = "SELECT NAME, DATE FROM HOLIDAY WHERE DATE > '" + DateTime.Today.ToShortDateString() + "'";
            ConnectInfo.RPMSDataTable(sCommandText, "HOLIDAY", m_dsGlobal);
            Debug.Write("LoadingGlobalRecordsets -- Holidays loaded\n");


            //Save the xml schema
            //m_dsGlobal.WriteXmlSchema(@"..\..\csSchema20060526.xsd");
            //----------------------------------------------
            

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

            //Close Splash Screen
            closeSplashDelegate();
			
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
            string sCommandText = @"SELECT BMXIEN RESOURCEUSER_ID, RESOURCENAME, INTERNAL[RESOURCENAME] RESOURCEID, OVERBOOK, MODIFY_SCHEDULE, MODIFY_APPOINTMENTS, USERNAME, INTERNAL[USERNAME] USERID FROM BSDX_RESOURCE_USER"; // WHERE INTERNAL[INSTITUTION]=" + m_ConnectInfo.DUZ2;
            
            if (!bAllUsers)
            {
                sCommandText += String.Format(" WHERE INTERNAL[USERNAME] = {0}", m_ConnectInfo.DUZ);
            }

			ConnectInfo.RPMSDataTable(sCommandText, "ResourceUser", m_dsGlobal);
			Debug.Write("LoadGlobalRecordsets -- ResourceUser loaded\n");
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

        /// <summary>
        /// Removes view and Handles Disconnection from Database if no views are left.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
				//_current.m_ConnectInfo = new BMXNet.BMXNetConnectInfo();//smh redundant
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

		public delegate DataTable RPMSDataTableDelegate(string CommandString, string TableName);

		public DataTable RPMSDataTable(string sSQL, string sTableName)
		{
			//Retrieves a recordset from RPMS
			string			sErrorMessage = "";
            DataTable dtOut;

#if TRACE
            DateTime sendTime = DateTime.Now;
#endif
			try
			{
				//System.IntPtr pHandle = this.Handle;
				RPMSDataTableDelegate rdtd = new RPMSDataTableDelegate(ConnectInfo.RPMSDataTable);
				//dtOut = (DataTable) this.Invoke(rdtd, new object[] {sSQL, sTableName});
                dtOut = rdtd.Invoke(sSQL, sTableName);
			}

			catch (Exception ex)
			{
				sErrorMessage = "CGDocumentManager.RPMSDataTable error: " + ex.Message;
				throw ex;
			}

#if TRACE
            DateTime receiveTime = DateTime.Now;
            TimeSpan executionTime = receiveTime - sendTime;
            Debug.Write("CGDocumentManager::RPMSDataTable Execution Time: " + executionTime.Milliseconds + " ms.\n");
#endif

            return dtOut;

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
