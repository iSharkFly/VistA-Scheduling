/* Main Class...:
 * Original Author: Horace Whitt
 * Current Author and Maintainer: Sam Habiel
 * License: LGPL. http://www.gnu.org/licenses/lgpl-2.1.html
*/

using System;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;
using IndianHealthService.BMXNet;
using IndianHealthService.BMXNet.WinForm;
using IndianHealthService.BMXNet.WinForm.Configuration; //grrrr... too many namespaces here...
using Mono.Options;
using System.Runtime.InteropServices;
using System.Globalization;

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

        //Globalization Object (tied to command line parameter /culture)
        private string                      m_CultureName = "";

        //Data Access Layer
        private DAL                         _dal = null;

		//M Connection member variables
		private DataSet									m_dsGlobal = null;      // Holds all user data

        //Custom Printing
        private Printing                          m_PrintingObject = null; 
		#endregion

        #region Properties

        public WinFramework WinFramework { get; private set; }  // Login Manager
        public RemoteSession RemoteSession { get; private set; } // Data Sesssion against the RPMS/VISTA server
        public RPCLogger RPCLogger { get; private set; }         // Logger for RPCs

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
        /// User Preferences Auto Property
        /// </summary>
        public UserPreferences UserPreferences { get; private set; }
 
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

        public Printing PrintingObject
        {
            get
            {
                return this.m_PrintingObject;
            }
        }
        #endregion

        /// <summary>
        /// Private constructor for singleton instance.
        /// </summary>
		private CGDocumentManager()
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
        /// /culture or -culture = Culture Name for UI Culture if you wish to override the Windows Culture
        /// </param>
        /// <remarks>
        /// Encoding decision is complex. This is the order of priority:
        /// - If the M DB runs in UTF-8, that's what we are going to use.
        /// - If that's not so, /e sets the default encoding. If /e is a non-existent encoding, move to next step.
        /// - If /e is not supplied or is not recognized, the default encoding is the Windows default Encoding for the user.
        /// </remarks>
        [STAThread()]
        static void Main(string[] args)
        {
            //Application wide error handler for unhandled errors (later I figure out that's only for WinForm ex'es)
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.ThreadException += new ThreadExceptionEventHandler(App_ThreadException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App_DomainException);

#if DEBUG
            // Print console messages to console if launched from console
            // Note: Imported From kernel32.dll
            AttachConsole(ATTACH_PARENT_PROCESS);
#endif

#if TRACE
            DateTime startLoadTime = DateTime.Now;
#endif

            //Store a class instance of manager. Actual constructor does nothing.
            _current = new CGDocumentManager();

            //Get command line options; store in private class wide variables
            var opset = new OptionSet() {
                { "s=", s => _current.m_Server = s },
                { "p=", p => _current.m_Port = int.Parse(p) },
                { "a=", a => _current.m_AccessCode = a },
                { "v=", v => _current.m_VerifyCode = v },
                { "e=", e => _current.m_Encoding = e},
                { "culture=", culture => _current.m_CultureName = culture }
            };

            opset.Parse(args);
            
            //Init app. Catch Login Exceptions if they happen.
            bool isEverythingOkay = false;
            try
            {
                isEverythingOkay = _current.InitializeApp();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Booboo: An Error Happened: " + ex.Message);
                return; // exit application
            }
            

            //if something yucky happened, break out.
            if (!isEverythingOkay) return;

            //Create the first empty document
            //A document holds the resources, appointments, and availabilites
            //SAM: Good place for break point
            CGDocument doc = new CGDocument();
            doc.DocManager = _current;

            //Create new View
            //A view is a specific arrangement of appointments and availabilites that constitute a document
            CGView view = new CGView();
            view.InitializeDocView(doc, _current, doc.StartDate, _current.WindowText);

            //Handle Message Queue
            Application.DoEvents();

            //test
            //doc.ThrowException();
            //test

#if TRACE
            DateTime EndLoadTime = DateTime.Now;
            TimeSpan LoadTime = EndLoadTime - startLoadTime;
            Debug.Write("Load Time for GUI is " + LoadTime.Seconds + " s & " + LoadTime.Milliseconds + " ms\n");
#endif
            
            view.Show();
            view.Activate();
            
            Application.Run();
        }

        /// <summary>
        /// Exception handler for application errors. Only for WinForm Errors.
        /// </summary>
        /// <remarks>Never tested. I can't get an error to go here!</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void App_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string msg = "A problem has occured in this applicaton. \r\n\r\n" +
                "\t" + e.Exception.Message + "\r\n\r\n" +
                "Would you like to continue the application?";

            DialogResult res = MessageBox.Show(msg, "Unexpected Error", MessageBoxButtons.YesNo);

            if (res == DialogResult.Yes) return;
            else Application.Exit();
        }

        /// <summary>
        /// If we are here, we are dead meat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void App_DomainException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Looks like we lost our connection with the server\nClick OK to terminate the application.");
            }
            else
            {
                Exception ex = e.ExceptionObject as Exception;

                string msg = "A problem has occured in this applicaton. \r\n\r\n" +
                    "\t" + ex.InnerException.Message;

                MessageBox.Show(msg, "Unexpected Error");
            }
        }// here application terminates

        #region BMXNet Event Handler
        private void CDocMgrEventHandler(Object obj, RemoteEventArgs e)
		{
			if (e.EventType == "BSDX CALL WORKSTATIONS")
			{
				string sParam = "";
				string sDelim="~";
				sParam += this.RemoteSession.User.Name + sDelim;
				sParam += this.m_sHandle + sDelim;
				sParam += Application.ProductVersion + sDelim;
				sParam += this._views.Count.ToString();
				_current.RemoteSession.EventServices.TriggerEvent("BSDX WORKSTATION REPORT", sParam, true);
			}
			if (e.EventType == "BSDX ADMIN MESSAGE")
			{
				string sMsg = e.EventType;
				ShowAdminMsgDelegate samd = new ShowAdminMsgDelegate(ShowAdminMsg);
                samd.Invoke(sMsg);
			}
			if (e.EventType == "BSDX ADMIN SHUTDOWN")
			{
				string sMsg = e.Details;
				CloseAllDelegate cad = new CloseAllDelegate(CloseAll);
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

        /// <summary>
        /// See InitializeApp(bool) below
        /// </summary>
		private bool InitializeApp()
		{
			return InitializeApp(false);
		}

		/// <summary>
		/// Does a million things:
        /// 1. Starts Connection and displays log-in dialogs
        /// 2. Starts Splash screen
        /// 3. Loads data tables
		/// </summary>
		/// <param name="bReLogin">Is the User logging in again from a currently running instance?
        /// If so, display a dialog to collect access and verify codes.</param>
        private bool InitializeApp(bool bReLogin)
		{
            //Note: There are 2 splashes -- one for being the parent of the log in forms
            // the next is invoked async and updated async while the GUI is loading
            // The reason is b/c an async form cannot be the parent of another that lies on the main thread

            RPCLogger = new RPCLogger();

            DSplash firstSplash = new DSplash();

            firstSplash.Show();
            
            /* IMPORTANT NOTE
             * LOGIN CODE IS COPIED ALMOST VERBATIM FROM THE SCHEMABUILDER APPLICAITON;
             * THE ONLY ONE I CAN FIND WHICH RELIES ON BMX 4 NEW WAYS WHICH I CAN'T FIGURE OUT
             */
            LoginProcess login;
            this.WinFramework = WinFramework.CreateWithNetworkBroker(true, RPCLogger);
            
            if (bReLogin) // if logging in again...
            {
                this.WinFramework.LoadConnectionSpecs(LocalPersistentStore.CreateDefaultStorage(true), "BSDX");
                login = this.WinFramework.CreateLoginProcess();
                login.AttemptUserInputLogin("Clincal Scheduling Log-in", 3, true, firstSplash);
                goto DoneTrying;
            }

            // If server,port,ac,vc are supplied on command line, then try to connect...
            else if (!String.IsNullOrEmpty(m_Server) && m_Port != 0 && !String.IsNullOrEmpty(m_AccessCode) && !String.IsNullOrEmpty(m_VerifyCode))
            {
                RpmsConnectionSpec spec = new RpmsConnectionSpec();
                spec.IsDefault = true;
                spec.Name = "Command Line Server";
                spec.Port = m_Port;
                spec.Server = m_Server;
                spec.UseWindowsAuthentication = false; //for now
                spec.UseDefaultNamespace = true; //for now
                login = this.WinFramework.CreateLoginProcess();
                login.AutoSetDivisionToLastLookup = false;
                login.AttemptAccessVerifyLogin(spec, m_AccessCode, m_VerifyCode);
                goto DoneTrying;
            }
            
            // if only server, port is supplied, then use these instead
            else if (!String.IsNullOrEmpty(m_Server) && m_Port != 0)
            {
                RpmsConnectionSpec spec = new RpmsConnectionSpec();
                spec.IsDefault = true;
                spec.Name = "Command Line Server";
                spec.Port = m_Port;
                spec.Server = m_Server;
                spec.UseWindowsAuthentication = false; //for now
                spec.UseDefaultNamespace = true; //for now

                RpmsConnectionSettings cxnSettings = new RpmsConnectionSettings
                {
                    CommandLineConnectionSpec = spec
                };

                this.WinFramework.ConnectionSettings = cxnSettings;

                login = this.WinFramework.CreateLoginProcess();
                login.AutoSetDivisionToLastLookup = false;
                //test
                //spec.UseWindowsAuthentication = true;
                login.AttemptUserInputLogin("Clinical Scheduling Log-in", 3, false, firstSplash);
                //login.AttemptWindowsAuthLogin();
                //test
                goto DoneTrying;
            }

            // if nothing is supplied, fall back on the original dialog
            else
            {
                this.WinFramework.LoadConnectionSpecs(LocalPersistentStore.CreateDefaultStorage(true), "BSDX");
                login = this.WinFramework.CreateLoginProcess();
                login.AutoSetDivisionToLastLookup = false;
                login.AttemptUserInputLogin("Clincal Scheduling Log-in", 3, true, firstSplash);

                goto DoneTrying;
            }

DoneTrying:
            if (!login.WasSuccessful)
            {
                return false;
            }

            LocalSession local = this.WinFramework.LocalSession;

            if ((this.WinFramework.Context.User.Division == null) && !this.WinFramework.AttemptUserInputSetDivision("Set Initial Division", firstSplash))
            {
                return false;
            }

            

            this.RemoteSession = this.WinFramework.PrimaryRemoteSession;

            //Tie delegate to Events generated by BMX.
            this.RemoteSession.EventServices.RpmsEvent += this.CDocMgrEventHandler;
            //Disable polling
            this.RemoteSession.EventServices.IsEventPollingEnabled = false;

            //Second splash screens
            //Show a splash screen while initializing; define delegates to remote thread
            DSplash secondSplash = new DSplash();
            DSplash.dSetStatus setStatusDelegate = new DSplash.dSetStatus(secondSplash.SetStatus);
            DSplash.dAny closeSplashDelegate = new DSplash.dAny(secondSplash.RemoteClose);
            DSplash.dProgressBarSet setMaxProgressDelegate = new DSplash.dProgressBarSet(secondSplash.RemoteProgressBarMaxSet);
            DSplash.dProgressBarSet setProgressDelegate = new DSplash.dProgressBarSet(secondSplash.RemoteProgressBarValueSet);

            //Start new thread for the Splash screen.
            Thread threadSplash = new Thread(new ParameterizedThreadStart(frm => ((DSplash)frm).ShowDialog()));
            threadSplash.IsBackground = true; //expendable thread -- exit even if still running.
            threadSplash.Name = "Splash Thread";
            threadSplash.Start(secondSplash);

            firstSplash.Close(); // close temporary splash now that the new one is up and running

            //There are 21 steps to load the application. That's max for the progress bar.
            setMaxProgressDelegate(21);

            // smh--not used: System.Configuration.ConfigurationManager.GetSection("appSettings");
            setStatusDelegate("Connecting to VISTA");

            
            /*
            //Try to connect using supplied values for Server and Port
            //Why am I doing this? The library BMX net uses prompts for access and verify code
            //whether you can connect or not. Not good. So I test first whether
            //we can connect at all by doing a simple connection and disconnect.
            //TODO: Make this more robust by sending a TCPConnect message and seeing if you get a response
            if (m_Server != "" && m_Port != 0)
            {
                System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
                try
                {
                    tcpClient.Connect(m_Server, m_Port); // open it
                    tcpClient.Close();                  // then close it
                }
                catch (System.Net.Sockets.SocketException)
                {
                    m_ds.RemoteMsgBox("Can't connect to server! Network Error");
                    return false;
                }
            }


            bool bRetry = true;

            // Do block is Log-in logic
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
                    // Checks the passed parameters stored in the class variables
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
                    m_ds.RemoteMsgBox("Cannot connect to VistA. Network Error");
                }
                catch (BMXNetException ex)
                {
                    if (m_ds.RemoteMsgBox("Unable to connect to VistA.  " + ex.Message, "Clinical Scheduling", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    {
                        bRetry = true;
                        //I hate this, but this is how the library is designed. It throws an error if the user cancels. XXX: Won't fix library until BMX 4.0 port.
                        try { _current.m_ConnectInfo.ChangeServerInfo(); }
                        catch (Exception) 
                        {
                            closeSplashDelegate();
                            bRetry = false;
                            return false; //tell main that it's a no go.
                        }
                    }
                    else
                    {
                        closeSplashDelegate();
                        bRetry = false;
                        return false; //tell main that it's a no go.
                    }
                }
			}while (bRetry == true);
            */

            //Printing Custom DLL. Perfect place for code injection!!!
            //*************************************************
            string DllLocation = string.Empty;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Printing\");
            if (di.Exists)
            {
                System.IO.FileInfo[] rgFiles = di.GetFiles("*.dll");
                
                foreach (System.IO.FileInfo fi in rgFiles)
                {
                    DllLocation = fi.FullName;
                }
            }

            PrintingCreator Creator = null;
            if (DllLocation == string.Empty)
            {
                this.m_PrintingObject = new Printing();
            }
            else
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(DllLocation);
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass == true & type.BaseType == typeof(PrintingCreator))
                    {
                        Creator = (PrintingCreator)Activator.CreateInstance(type);
                        break;
                    }
                }
                this.m_PrintingObject = Creator.PrintFactory();
            }
           //************************************************
            
            //User Interface Culture (m_CultureName is set from the command line flag /culture)
            //
            //If passed, set that try that culture; fail over to Invariant Culture
            if (m_CultureName != String.Empty)
            {
                try { Thread.CurrentThread.CurrentUICulture = new CultureInfo(m_CultureName); }
                catch (CultureNotFoundException) { Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture; }
            }
            //otherwise, use the Current Computer Culture, EVEN IF (!!) the UI Culture is different.
            //this allows localization even if Windows still displays messages in English.
            else
            {
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }

            _dal = new DAL(RemoteSession);   // Data access layer
            
            //Create global dataset
			_current.m_dsGlobal = new DataSet("GlobalDataSet");

			//Version info
            // Table #1
            setProgressDelegate(1);
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
            if (!(x.Major.ToString() == sMajor && x.Minor.ToString() == sMinor))
            {
                MessageBox.Show(
                    "Server runs version " + sMajor + "." + sMinor + "\r\n" +
                    "You are running " + x.ToString() + "\r\n\r\n" +
                    "Major, Minor and Build versions must match",
                    "Version Mismatch");
                closeSplashDelegate();
                return false;
            }
 

            //Change encoding
            // Call #2
            setProgressDelegate(2);
            setStatusDelegate("Setting encoding...");
            //PORT TODO: Set encoding
            if (m_Encoding == String.Empty)
            {
                string utf8_server_support = RemoteSession.TransmitRPC("BMX UTF-8", "");
                
                if (utf8_server_support == "1")
                    RemoteSession.ConnectionEncoding = System.Text.UTF8Encoding.UTF8;
                
            }
			
            //Set application context
            // Call #3
            setProgressDelegate(3);
			setStatusDelegate("Setting Application Context to BSDXRPC...");
			RemoteSession.AppContext = "BSDXRPC";

            //User Preferences Object
            setProgressDelegate(4); //next number is 6 b/c two calls
            setStatusDelegate("Getting User Preferences from the Server...");

            _current.UserPreferences = new UserPreferences(); // Constructor Does the calling to do that...
            
            //Load global recordsets
            string statusConst = "Loading VistA data tables...";
			setStatusDelegate(statusConst);

            string sCommandText;

            //Schedule User Info
            // Table #4
            setProgressDelegate(6);
            setStatusDelegate(statusConst + " Schedule User");
            DataTable dtUser = _dal.GetUserInfo(RemoteSession.User.Duz);
            dtUser.TableName = "SchedulingUser";
            m_dsGlobal.Tables.Add(dtUser);
            Debug.Assert(dtUser.Rows.Count == 1);

            // Only one row and one column named "MANAGER". Set local var m_bSchedManager to true if Manager.
            DataRow rUser = dtUser.Rows[0];
            Object oUser = rUser["MANAGER"];
            string sUser = oUser.ToString();
            m_bSchedManager = (sUser == "YES") ? true : false;

            //Get Access Types
            // Table #5
            setProgressDelegate(7);
            setStatusDelegate(statusConst + " Access Types");
            DataTable dtAccessTypes = _dal.GetAccessTypes(m_dsGlobal, "AccessTypes");

            //Get Access Groups
            // Table #6
            setProgressDelegate(8);
            setStatusDelegate(statusConst + " Access Groups");
            LoadAccessGroupsTable();

            //Build Primary Key for AccessGroup table
            DataTable dtGroups = m_dsGlobal.Tables["AccessGroup"];
            DataColumn dcKey = dtGroups.Columns["ACCESS_GROUP"];
            DataColumn[] dcKeys = new DataColumn[1];
            dcKeys[0] = dcKey;
            dtGroups.PrimaryKey = dcKeys;

            //Get Access Group Types (Combines Access Types and Groups)
            //Optimization Note: Can eliminate Access type and Access Group Table
            // But they are heavily referenced throughout the code.
            // Table #7
            setProgressDelegate(9);
            setStatusDelegate(statusConst + " Access Group Types");
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
            // Table #8
            // What shows up on the tree. The groups the user has access to.
            setProgressDelegate(10);
            setStatusDelegate(statusConst + " Resource Groups By User");
            LoadResourceGroupTable();

            //Resources by user
            // Table #9
            // Individual Resources
            setProgressDelegate(11);
            setStatusDelegate(statusConst + " Resources By User");
            LoadBSDXResourcesTable();

            //Build Primary Key for Resources table
            DataColumn[] dc = new DataColumn[1];
            dc[0] = m_dsGlobal.Tables["Resources"].Columns["RESOURCEID"];
            m_dsGlobal.Tables["Resources"].PrimaryKey = dc;

            //GroupResources table
            // Table #10
            // Resource Groups and Indivdual Resources together
            setProgressDelegate(12);
            setStatusDelegate(statusConst + " Group Resources");
            LoadGroupResourcesTable();

            //Build Primary Key for ResourceGroup table
            dc = new DataColumn[1];
            dc[0] = m_dsGlobal.Tables["ResourceGroup"].Columns["RESOURCE_GROUP"];
            m_dsGlobal.Tables["ResourceGroup"].PrimaryKey = dc;

            //Build Data Relationships between ResourceGroup and GroupResources tables
            dr = new DataRelation("GroupResource",	//Relation Name
                m_dsGlobal.Tables["ResourceGroup"].Columns["RESOURCE_GROUP"],	//Parent
                m_dsGlobal.Tables["GroupResources"].Columns["RESOURCE_GROUP"]);	//Child

            m_dsGlobal.Relations.Add(dr);

            //HospitalLocation table
            //Table #11
            setProgressDelegate(13);
            setStatusDelegate(statusConst + " Clinics");
            //cmd.CommandText = "SELECT BMXIEN 'HOSPITAL_LOCATION_ID', NAME 'HOSPITAL_LOCATION', DEFAULT_PROVIDER, STOP_CODE_NUMBER, INACTIVATE_DATE, REACTIVATE_DATE FROM HOSPITAL_LOCATION";
            sCommandText = "BSDX HOSPITAL LOCATION";
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "HospitalLocation");
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

            //Build ScheduleUser table
            //Table #12
            setProgressDelegate(14);
            setStatusDelegate(statusConst + " Schedule User");
            this.LoadScheduleUserTable();

            //Build Primary Key for ScheduleUser table
            dc = new DataColumn[1];
            dtTemp = m_dsGlobal.Tables["ScheduleUser"];
            dc[0] = dtTemp.Columns["USERID"];
            m_dsGlobal.Tables["ScheduleUser"].PrimaryKey = dc;

            //Build ResourceUser table
            //Table #13
            //Acess to Resources by [this] User
            setProgressDelegate(15);
            setStatusDelegate(statusConst + " Resource User");
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
            //Table #14
            //TODO: Lazy load the provider table; no need to load in advance.
            setProgressDelegate(16);
            setStatusDelegate(statusConst + " Providers");
            sCommandText = "SELECT BMXIEN, NAME FROM NEW_PERSON WHERE INACTIVE_DATE = '' AND BMXIEN > 1";
            RemoteSession.TableFromSQL(sCommandText, m_dsGlobal, "Provider");
            Debug.Write("LoadGlobalRecordsets -- Provider loaded\n");

            //Build the HOLIDAY table
            //Table #15
            setProgressDelegate(17);
            setStatusDelegate(statusConst + " Holiday");
            sCommandText = "SELECT NAME, DATE FROM HOLIDAY WHERE INTERNAL[DATE] > '" + FMDateTime.Create(DateTime.Today).DateOnly.FMDateString + "'";
            RemoteSession.TableFromSQL(sCommandText, m_dsGlobal, "HOLIDAY");
            Debug.Write("LoadingGlobalRecordsets -- Holidays loaded\n");


            //Save the xml schema
            //m_dsGlobal.WriteXmlSchema(@"..\..\csSchema20060526.xsd");
            //----------------------------------------------

            setStatusDelegate("Setting Receive Timeout");
            _current.RemoteSession.ReceiveTimeout = 30000; //30-second timeout

#if DEBUG
            _current.RemoteSession.ReceiveTimeout = 600000; //longer timeout for debugging
#endif 
			// Event Subsriptions
            setStatusDelegate("Subscribing to Server Events");
            //Table #16
            setProgressDelegate(18);
            _current.RemoteSession.EventServices.Subscribe("BSDX SCHEDULE");
			//Table #17
            setProgressDelegate(19);
            _current.RemoteSession.EventServices.Subscribe("BSDX CALL WORKSTATIONS");
			//Table #18
            setProgressDelegate(20);
            _current.RemoteSession.EventServices.Subscribe("BSDX ADMIN MESSAGE");
			//Table #19
            setProgressDelegate(21);
            _current.RemoteSession.EventServices.Subscribe("BSDX ADMIN SHUTDOWN");

			_current.RemoteSession.EventServices.EventPollingInterval = 5000; //in milliseconds
			_current.RemoteSession.EventServices.IsEventPollingEnabled = true;
			
            //PORT TODO: No Autofire in BMX 4.0
            //_current.RemoteSession.EventServices. = 12; //AutoFire every 12*5 seconds

            //Close Splash Screen
            closeSplashDelegate();

            return true;
			
		}



		public void LoadAccessGroupsTable()
		{
			string sCommandText = "SELECT * FROM BSDX_ACCESS_GROUP";
			RemoteSession.TableFromSQL(sCommandText, m_dsGlobal, "AccessGroup");
			Debug.Write("LoadGlobalRecordsets -- AccessGroups loaded\n");
		}

		public void LoadAccessGroupTypesTable()
		{
			string sCommandText = "BSDX GET ACCESS GROUP TYPES";
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "AccessGroupType");
			Debug.Write("LoadGlobalRecordsets -- AccessGroupTypes loaded\n");
		}

		public void LoadBSDXResourcesTable()
		{
			string sCommandText = "BSDX RESOURCES^" + RemoteSession.User.Duz;
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "Resources");
			Debug.Write("LoadGlobalRecordsets -- Resources loaded\n");
		}
		
		public void LoadResourceGroupTable()
		{
			//ResourceGroup Table (Resource Groups by User)
			//Table "ResourceGroup" contains all resource group names
			//to which user has access
			//Fields are: RESOURCE_GROUPID, RESOURCE_GROUP
			string sCommandText = "BSDX RESOURCE GROUPS BY USER^" + RemoteSession.User.Duz;
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "ResourceGroup");
			Debug.Write("LoadGlobalRecordsets -- ResourceGroup loaded\n");
		}

		public void LoadGroupResourcesTable()
		{
			//Table "GroupResources" contains all active GROUP/RESOURCE combinations
			//to which user has access based on entries in BSDX RESOURCE USER file
			//If user has BSDXZMGR or XUPROGMODE keys, then ALL Group/Resource combinstions
			//are returned.
			//Fields are: RESOURCE_GROUPID, RESOURCE_GROUP, RESOURCE_GROUP_ITEMID, RESOURCE_NAME, RESOURCE_ID
			string sCommandText = "BSDX GROUP RESOURCE^" + RemoteSession.User.Duz;
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "GroupResources");
			Debug.Write("LoadGlobalRecordsets -- GroupResources loaded\n");
		}

		public void LoadScheduleUserTable()
		{
			//Table "ScheduleUser" contains an entry for each user in File 200 (NEW PERSON)
			//who possesses the BSDXZMENU security key.
			string sCommandText = "BSDX SCHEDULE USER";
            RemoteSession.TableFromCommand(sCommandText, m_dsGlobal, "ScheduleUser");
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
                sCommandText += String.Format(" WHERE INTERNAL[USERNAME] = {0}", RemoteSession.User.Duz);
            }

            RemoteSession.TableFromSQL(sCommandText, m_dsGlobal, "ResourceUser");
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
				RemoteSession.EventServices.IsEventPollingEnabled = false;
				RemoteSession.EventServices.Unsubscribe("BSDX SCHEDULE");
                RemoteSession.Close();
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
                RemoteSession.Close();
				Application.Exit();
			}
		}

        /// <summary>
        /// Not used
        /// </summary>
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

        /// <summary>
        /// Accomplishes Changing the Server to which you connect
        /// </summary>
        /// <remarks>
        /// Parameter relog-in for InitializeApp forces initialize app to use
        /// 1. The server the user just picked and then BMX saved off to User Preferences
        /// 2. A new access and verify code pair
        /// </remarks>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
		private void mnuRPMSServer_Click(object sender, EventArgs e)
		{
			//Warn that changing servers will close all schedules
			if (MessageBox.Show("Are you sure you want to close all schedules and connect to a different VistA server?", "Clinical Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				return;

			//Reconnect to RPMS and recreate all global recordsets
			try
			{
                // Close All, but tell the Close All method not to call Applicaiton.Exit since we still plan to continue.
                // Close All does not call Application.Exit, but CGView_Close handler does
				m_bExitOK = false;
                CloseAll();
                m_bExitOK = true;
                
                //Used in Do loop
                //bool bRetry = true;
				
                /*// Do Loop to deal with changing the server and the vagaries of user choices.
				do
				{
					try
					{
                        //ChangeServerInfo does not re-login the user
                        //It only changes the saved server information in the %APPDATA% folder
                        //so it can be re-used when BMX tries to log in again.
                        //Access and Verify code are prompted for in InitializeApp
                        LoginProcess login = this.WinFramework.CreateLoginProcess();
                        login.AttemptUserInputLogin("ReLog-in", 3, true, null);
						bRetry = false;
					}
					catch (Exception ex)
					{
						if (ex.Message == "User cancelled.")
						{
							bRetry = false;
                            Application.Exit();
							return;
						}
						if (MessageBox.Show("Unable to connect to VistA.  " + ex.Message , "Clinical Scheduling", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
						{
							bRetry = true;
						}
						else
						{
							bRetry = false;
                            Application.Exit();
							return;
						}
					}
				} while (bRetry == true);
                */
                
                //Parameter for initialize app tells it that this is a re-login and forces a new access and verify code.
                bool isEverythingOkay = this.InitializeApp(true);

                //if an error occurred, break out. This time we need to call Application.Exit since it's already running.
                if (!isEverythingOkay)
                {
                    Application.Exit();
                    return;
                }

                //Otherwise, everything is okay. So open document and view, then show and activate view.
                CGDocument doc = new CGDocument();
                doc.DocManager = _current;

                CGView view = new CGView();
                view.InitializeDocView(doc, _current, doc.StartDate, _current.WindowText);

                view.Show();
                view.Activate();

                //Application.Run need not be called b/c it is already running.
			}
			catch (Exception ex)
			{
				throw ex;
			}
	
		}

        /// <summary>
        /// Accomplishes Re-login into RPMS/VISTA. Now all logic is in this event handler.
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
		private void mnuRPMSLogin_Click(object sender, EventArgs e)
		{
            mnuRPMSServer_Click(sender, e);
            
            /* v 1.7 to support BMX 4 -- commented out -- smh
            //Warn that changing login will close all schedules
			if (MessageBox.Show("Are you sure you want to close all schedules and login to VistA?", "Clinical Scheduling", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				return;

			//Reconnect to RPMS and recreate all global recordsets
			try
			{
                // Close All, but tell the Close All method not to call Applicaiton.Exit since we still plan to continue.
                // Close All does not call Application.Exit, but CGView_Close handler does
				m_bExitOK = false;
				CloseAll();
				m_bExitOK = true;

                LoginProcess login = this.WinFramework.CreateLoginProcess();
                login.AttemptUserInputLogin("Clincal Scheduling", 3, true, null);
                //m_ConnectInfo.bmxNetLib.StartLog();    //This line turns on logging of messages

                if (!login.WasSuccessful)
                {
                    return;
                }

                LocalSession local = this.WinFramework.LocalSession;

                if ((this.WinFramework.Context.User.Division == null) && !this.WinFramework.AttemptUserInputSetDivision("Set Initial Division", null))
                {
                    return;
                }

                this.RemoteSession = this.WinFramework.PrimaryRemoteSession;

                //Parameter for initialize app tells it that this is a re-login and forces a new access and verify code.
                bool isEverythingOkay = this.InitializeApp(true);

                //if an error occurred, break out. This time we need to call Application.Exit since it's already running.
                if (!isEverythingOkay)
                {
                    Application.Exit();
                    return;
                }

                //Otherwise, everything is okay. So open document and view, then show and activate view.
                CGDocument doc = new CGDocument();
                doc.DocManager = _current;

                CGView view = new CGView();
                view.InitializeDocView(doc, _current, doc.StartDate, _current.WindowText);

                view.Show();
                view.Activate();

                //Application.Run need not be called b/c it is already running.
			}
			catch (Exception ex)
			{
				throw ex;
			}
	        */
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

			try
			{
				//System.IntPtr pHandle = this.Handle;
				RPMSDataTableDelegate rdtd = new RPMSDataTableDelegate(RemoteSession.TableFromCommand);
				//dtOut = (DataTable) this.Invoke(rdtd, new object[] {sSQL, sTableName});
                dtOut = RemoteSession.TableFromCommand(sSQL);
                dtOut.TableName = sTableName;

			}

			catch (Exception ex)
			{
				sErrorMessage = "CGDocumentManager.RPMSDataTable error: " + ex.Message;
				throw ex;
			}

            return dtOut;

		}

		public void ChangeDivision(System.Windows.Forms.Form frmCaller)
		{
            WinFramework.AttemptUserInputSetDivision("Change Division", frmCaller);

            RemoteSession = WinFramework.PrimaryRemoteSession;

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
