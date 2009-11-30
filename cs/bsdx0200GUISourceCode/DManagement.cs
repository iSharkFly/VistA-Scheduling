using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using IndianHealthService.BMXNet;
using System.Diagnostics;
using System.Threading;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Summary description for DManagement.
	/// </summary>
	public class DManagement : System.Windows.Forms.Form
	{

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TabPage tpResources;
		private System.Windows.Forms.TabPage tpAccessTypes;
		private System.Windows.Forms.GroupBox grpDescription;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Panel pnlPageBottom;
		private System.Windows.Forms.TabControl tabMain;
		private System.Windows.Forms.Panel pnlAddEdit;
		private System.Windows.Forms.Button cmdAddResource;
		private System.Windows.Forms.Button cmdChangeResource;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.Panel pnlResources;
		private System.Windows.Forms.DataGrid grdResources;
		private System.Windows.Forms.Panel pnlAddEditAT;
		private System.Windows.Forms.Button cmdChangeAT;
		private System.Windows.Forms.Button cmdAddAT;
		private System.Windows.Forms.Panel pnlDescriptionAT;
		private System.Windows.Forms.GroupBox grpDescriptionAT;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tpAccessGroups;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.TabPage tpResourceGroups;
		private System.Windows.Forms.Panel pnlAddEditResourceGroups;
		private System.Windows.Forms.Panel pnlDescriptionResourceGroup;
		private System.Windows.Forms.GroupBox grpDescriptionResourceGroup;
		private System.Windows.Forms.Label lblDescriptionResourceGroup;
		private System.Windows.Forms.Panel pnlAddEditAccessGroup;
		private System.Windows.Forms.Button cmdRemoveAccessGroup;
		private System.Windows.Forms.Button cmdAddAccessGroup;
		private System.Windows.Forms.Panel pnlDescriptionAccessGroups;
		private System.Windows.Forms.GroupBox grpDescriptionAccessGroups;
		private System.Windows.Forms.Label lblDescriptionAccessGroups;
		private System.Windows.Forms.DataGrid grdAccessGroups;
		private System.Windows.Forms.Button cmdRemoveUser;
		private System.Windows.Forms.DataGrid grdResourceGroup;
		private System.Windows.Forms.Button cmdRemoveResourceGroup;
		private System.Windows.Forms.Button cmdAddResourceGroup;
		private System.Windows.Forms.Button cmdChangeResourceGroup;
		private System.Windows.Forms.Button cmdChangeAccessGroup;
		private System.Windows.Forms.TabPage tpTransferAppts;
		private System.Windows.Forms.Panel pnlCmdXfer;
		private System.Windows.Forms.Panel pnlDescriptionXfer;
		private System.Windows.Forms.Label lblDescriptionXfer;
		private System.Windows.Forms.GroupBox grpDescriptionXfer;
		private System.Windows.Forms.DataGrid grdAccessTypes;

		#region Fields
		private DataTable			m_dtResources;
		private DataView			m_dvResources;
		private DataTable			m_dtHospLoc;
		private DataView			m_dvHospLoc;
		private DataTable			m_dtResourceGroup;
		private DataView			m_dvResourceGroup;
		private DataTable			m_dtAccessTypes;
		private DataView			m_dvAccessTypes;
		private DataTable			m_dtAccessGroup;
		private DataView			m_dvAccessGroup;
		private DataTable			m_dtAccessGroupType;
		private DataView			m_dvAccessGroupType;
		private int					m_nATRow;
		private int					m_nResourceRow;
		private int					m_nResourceGroupRow;
		private int					m_nAccessGroupRow;
		private int					m_nResourceID;
		private int					m_nResourceGroupID;
		private int					m_nAccessGroupID;
		private DataSet				m_dsGlobal;
		private CGDocumentManager	m_DocManager;
		private bool				m_bEditUsers;
		private string				m_sMember;
		private string				m_sGroupMember;
		private string				m_sAccessGroupMember;
		private bool				m_bEditGroupItems;
		private bool				m_bEditAccessGroupItems;
		private string				m_sResourceGroupName;
        private string              m_sAccessGroupName;
        private DataTable           m_dtWSGrid;
        private System.Windows.Forms.Button cmdCopyAppts;
		private System.Windows.Forms.ComboBox cboRPMSClinic;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker dtpBegin;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dtpEnd;
		private System.Windows.Forms.ComboBox cboBSDXClinic;
		private System.Windows.Forms.TabPage tpWorkStations;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cmdWorkStationsMessage;
		private System.Windows.Forms.Button cmdWorkStationsShutdown;
		private System.Windows.Forms.Button cmdWorkStationsRefresh;
		private System.Windows.Forms.Panel pnlWorkstations;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblWorkstations;
		private System.Windows.Forms.DataGrid grdWorkStations;
		private System.Windows.Forms.TextBox txtSendMessage;

		#endregion Fields

		#region Initialization

		public DManagement()
		{
			InitializeComponent();

			m_nATRow = -1;
			m_nResourceRow = -1;
			m_nResourceGroupRow = -1;
			m_nAccessGroupRow = -1;
			m_nResourceID = 0;
			m_sMember = "Resource";
			m_sGroupMember = "Group";
			m_sAccessGroupMember = "Group";

		}

		public void InitializeDialog(CGDocumentManager docManager)
		{
			//System.IntPtr pHandle = this.Handle;
            //this.m_sMgrHandle = pHandle.ToString()
			this.m_DocManager = docManager;
			this.m_dsGlobal = m_DocManager.GlobalDataSet;

			MgrEventDelegate = new BMXNetConnectInfo.BMXNetEventDelegate(MgrEventHandler);
			m_DocManager.ConnectInfo.BMXNetEvent += MgrEventDelegate;
			m_DocManager.ConnectInfo.SubscribeEvent("BSDX WORKSTATION REPORT");
			m_dtWSGrid = new DataTable("WSGrid");
			m_dtWSGrid.Columns.Add("UserName", typeof(System.String));
			m_dtWSGrid.Columns.Add("Handle", typeof(System.String));
			m_dtWSGrid.Columns.Add("Version", typeof(System.String));
			m_dtWSGrid.Columns.Add("Views", typeof(System.String));

			this.grdWorkStations.DataSource = m_dtWSGrid;

			//Resources
			m_dtResources = m_dsGlobal.Tables["Resources"];
			m_dvResources = new DataView(m_dtResources);
			m_dvResources.Sort = "RESOURCE_NAME ASC";
			this.grdResources.DataSource = m_dvResources;

			//Reload ResourceUser table with all users
			m_dsGlobal.Tables["ResourceUser"].Clear();
			m_DocManager.LoadResourceUserTable(true);

			//Create DataGridTableStyle for ResourceUser table
			DataGridTableStyle tsRU = new DataGridTableStyle();
			tsRU.MappingName = "ResourceUser";
			tsRU.ReadOnly = true;
			// Add RESOURCEID column style.
			DataGridColumnStyle colRUID = new DataGridTextBoxColumn();
			colRUID.MappingName = "RESOURCEID";
			colRUID.HeaderText = "Resource ID";
			colRUID.Width = 0;
			tsRU.GridColumnStyles.Add(colRUID);
			// Add RESOURCEUSER_ID column style.
			DataGridColumnStyle colRUserID = new DataGridTextBoxColumn();
			colRUserID.MappingName = "RESOURCEUSER_ID";
			colRUserID.HeaderText = "ResourceUser ID";
			colRUserID.Width = 15;
			tsRU.GridColumnStyles.Add(colRUserID);
			// Add USERNAME column style.
			DataGridColumnStyle colRUName = new DataGridTextBoxColumn();
			colRUName.MappingName = "USERNAME";
			colRUName.HeaderText = "Resource User Name";
			colRUName.Width = 250;
			tsRU.GridColumnStyles.Add(colRUName);

			grdResources.TableStyles.Add(tsRU);

			//Create DataGridTableStyle for Resources table
			DataGridTableStyle tsResource = new DataGridTableStyle();
			tsResource.MappingName = "Resources";
			tsResource.ReadOnly = true;

			// Add RESOURCEID column style.
			DataGridColumnStyle colResID = new DataGridTextBoxColumn();
			colResID.MappingName = "RESOURCEID";
			colResID.HeaderText = "Resource ID";
			colResID.Width = 0;
			tsResource.GridColumnStyles.Add(colResID);

			// Add RESOURCE_NAME column style.
			DataGridColumnStyle colResName = new DataGridTextBoxColumn();
			colResName.MappingName = "RESOURCE_NAME";
			colResName.HeaderText = "Resource Name";
			colResName.Width = 250;
			tsResource.GridColumnStyles.Add(colResName);
			grdResources.TableStyles.Add(tsResource);

			//ResourceGroup
			m_dtResourceGroup = m_dsGlobal.Tables["ResourceGroup"];
			m_dvResourceGroup = new DataView(m_dtResourceGroup);
			this.grdResourceGroup.DataSource = m_dvResourceGroup;

			//Create DataGridTableStyle for ResourceGroup table
			DataGridTableStyle tsResourceGroup = new DataGridTableStyle();
			tsResourceGroup.MappingName = "ResourceGroup";
			tsResourceGroup.ReadOnly = true;
			// Add RESOURCE_GROUPID column style.
			DataGridColumnStyle colResGroupID = new DataGridTextBoxColumn();
			colResGroupID.MappingName = "RESOURCE_GROUPID";
			colResGroupID.HeaderText = "GroupID";
			colResGroupID.Width = 50;
			tsResourceGroup.GridColumnStyles.Add(colResGroupID);
			// Add RESOURCE_GROUP column style.
			DataGridColumnStyle colResGroup = new DataGridTextBoxColumn();
			colResGroup.MappingName = "RESOURCE_GROUP";
			colResGroup.HeaderText = "Group";
			colResGroup.Width = 250;
			tsResourceGroup.GridColumnStyles.Add(colResGroup);
			grdResourceGroup.TableStyles.Add(tsResourceGroup);

			//Create DataGridTableStyle for GroupResources table
			DataGridTableStyle tsGroupResources = new DataGridTableStyle();
			tsGroupResources.MappingName = "GroupResources";
			tsGroupResources.ReadOnly = true;
			// Add RESOURCE_GROUPID column style.
			DataGridColumnStyle colResGroupID2 = new DataGridTextBoxColumn();
			colResGroupID2.MappingName = "RESOURCE_GROUPID";
			colResGroupID2.HeaderText = "Resource GroupID";
			colResGroupID2.Width = 50;
			tsGroupResources.GridColumnStyles.Add(colResGroupID2);
			// Add RESOURCE_NAME column style.
			DataGridColumnStyle colGroupRes = new DataGridTextBoxColumn();
			colGroupRes.MappingName = "RESOURCE_NAME";
			colGroupRes.HeaderText = "Resource Name";
			colGroupRes.Width = 250;
			tsGroupResources.GridColumnStyles.Add(colGroupRes);


			// Add RESOURCE_GROUP_ITEMID column style.
			DataGridColumnStyle colResGroupItemID = new DataGridTextBoxColumn();
			colResGroupItemID.MappingName = "RESOURCE_GROUP_ITEMID";
			colResGroupItemID.HeaderText = "Resource ItemID";
			colResGroupItemID.Width = 50;
			tsGroupResources.GridColumnStyles.Add(colResGroupItemID);
			grdResourceGroup.TableStyles.Add(tsGroupResources);


			//Access Types
			m_dtAccessTypes = m_dsGlobal.Tables["AccessTypes"];
			m_dvAccessTypes = new DataView(m_dtAccessTypes);
			this.grdAccessTypes.DataSource = m_dvAccessTypes;

			// Create DataGridTableStyle for AccessTypes table  
			DataGridTableStyle tsAT = new DataGridTableStyle();
			tsAT.MappingName = "AccessTypes";
			tsAT.ReadOnly = true;

			// Add ACCESS_TYPE_NAME column style.
			DataGridColumnStyle colATName = new DataGridTextBoxColumn();
			colATName.MappingName = "ACCESS_TYPE_NAME";
			colATName.HeaderText = "Access Type";
			colATName.Width = 250;
			tsAT.GridColumnStyles.Add(colATName);
      
			// Add INACTIVE column style.
			DataGridColumnStyle colATInactive = new DataGridTextBoxColumn();
			colATInactive.MappingName = "INACTIVE";
			colATInactive.HeaderText = "Inactive?";
			colATInactive.Width = 100;
			tsAT.GridColumnStyles.Add(colATInactive);

			grdAccessTypes.TableStyles.Add(tsAT);


			//Access Groups
			m_dtAccessGroup = m_dsGlobal.Tables["AccessGroup"];
			m_dvAccessGroup = new DataView(m_dtAccessGroup);
			this.grdAccessGroups.DataSource = m_dvAccessGroup;

			// Create DataGridTableStyle for AccessGroup table  
			DataGridTableStyle tsAG = new DataGridTableStyle();
			tsAG.MappingName = "AccessGroup";
			tsAG.ReadOnly = true;

			// Add BMXIEN column style.
			DataGridColumnStyle colAGID = new DataGridTextBoxColumn();
			colAGID.MappingName = "BMXIEN";
			colAGID.HeaderText = "Access Group ID";
			colAGID.Width = 50;
			tsAG.GridColumnStyles.Add(colAGID);
      
			// Add ACCESS_GROUP column style.
			DataGridColumnStyle colAGNAME = new DataGridTextBoxColumn();
			colAGNAME.MappingName = "ACCESS_GROUP";
			colAGNAME.HeaderText = "Access Group";
			colAGNAME.Width = 150;
			tsAG.GridColumnStyles.Add(colAGNAME);

			grdAccessGroups.TableStyles.Add(tsAG);

			//Access Group Types
			m_dtAccessGroupType = m_dsGlobal.Tables["AccessGroupType"];
			m_dvAccessGroupType = new DataView(m_dtAccessGroupType);

			// Create DataGridTableStyle for AccessGroupType table  
			DataGridTableStyle tsAGTP = new DataGridTableStyle();
			tsAGTP.MappingName = "AccessGroupType";
			tsAGTP.ReadOnly = true;

			// 0 Add ACCESS_GROUP_TYPEID column style.
			DataGridColumnStyle colAGTPID = new DataGridTextBoxColumn();
			colAGTPID.MappingName = "ACCESS_GROUP_TYPEID";
			colAGTPID.HeaderText = "Access Group Type ID";
			colAGTPID.Width = 0;
			tsAGTP.GridColumnStyles.Add(colAGTPID);
      
			// 1 Add ACCESS_GROUP_ID column style.
			DataGridColumnStyle colAGTPGroupID = new DataGridTextBoxColumn();
			colAGTPGroupID.MappingName = "ACCESS_GROUP_ID";
			colAGTPGroupID.HeaderText = "Access Group ID";
			colAGTPGroupID.Width = 0;
			tsAGTP.GridColumnStyles.Add(colAGTPGroupID);

			// 2 Add ACCESS_GROUP column style.
			DataGridColumnStyle colAGTPGroup = new DataGridTextBoxColumn();
			colAGTPGroup.MappingName = "ACCESS_GROUP";
			colAGTPGroup.HeaderText = "Access Group";
			colAGTPGroup.Width = 0;
			tsAGTP.GridColumnStyles.Add(colAGTPGroup);

			// 3 Add ACCESS_TYPE_ID column style.
			DataGridColumnStyle colAGTPTypeID = new DataGridTextBoxColumn();
			colAGTPTypeID.MappingName = "ACCESS_TYPE_ID";
			colAGTPTypeID.HeaderText = "Access TypeID";
			colAGTPTypeID.Width = 0;
			tsAGTP.GridColumnStyles.Add(colAGTPTypeID);

			// 4 Add ACCESS_TYPE column style.
			DataGridColumnStyle colAGTPType = new DataGridTextBoxColumn();
			colAGTPType.MappingName = "ACCESS_TYPE";
			colAGTPType.HeaderText = "Access Type";
			colAGTPType.Width = 150;
			tsAGTP.GridColumnStyles.Add(colAGTPType);

			grdAccessGroups.TableStyles.Add(tsAGTP);


			//Find out if there are any grdResources rows and
			//enable command buttons accordingly
			int nRows = this.grdResources.VisibleRowCount;
			if (nRows == 0)
			{
				this.cmdChangeResource.Enabled = false;
				this.cmdRemoveUser.Enabled = false;
			}
			else
			{
				grdResources.CurrentCell = new DataGridCell(0, 0);
				this.cmdChangeResource.Enabled = true;
				this.cmdRemoveUser.Enabled = true;

			}

			//Copy Appointments TabPage
			m_dtHospLoc = m_dsGlobal.Tables["HospitalLocation"];
			m_dvHospLoc = new DataView(m_dtHospLoc);
			m_dvHospLoc.Sort = "HOSPITAL_LOCATION ASC";
			cboRPMSClinic.DataSource = m_dvHospLoc;
			cboRPMSClinic.DisplayMember = "HOSPITAL_LOCATION";
			cboRPMSClinic.ValueMember = "HOSPITAL_LOCATION_ID";
			cboBSDXClinic.DataSource = m_dvResources;
			cboBSDXClinic.DisplayMember = "RESOURCE_NAME";
			cboBSDXClinic.ValueMember = "RESOURCEID";

		}

		private void DManagement_Load(object sender, System.EventArgs e)
		{
			this.cmdChangeResource.Enabled = false;
			this.cmdRemoveUser.Enabled = false;
			//Select the grid's zeroeth row
			if (m_dvResources.Count > 0)
			{
				this.grdResources.CurrentCell = new DataGridCell(0,0);
				this.grdResources.Select(0);
				this.m_nResourceRow=0;
				Object dgItem = grdResources[0,0];
				this.m_nResourceID = Convert.ToInt16(dgItem);
				this.cmdChangeResource.Enabled = true;
				this.cmdRemoveUser.Enabled = true;
			}

			this.cmdChangeResourceGroup.Enabled = false;
			this.cmdRemoveResourceGroup.Enabled = false;
			if (this.m_dvResourceGroup.Count > 0)
			{
				this.m_nResourceGroupRow = 0;
				this.cmdChangeResourceGroup.Enabled = true;
				this.cmdRemoveResourceGroup.Enabled = true;
			}
			
			this.cmdChangeAccessGroup.Enabled = false;
			this.cmdRemoveAccessGroup.Enabled = false;
			if (this.m_dvAccessGroup.Count > 0)
			{
				this.m_nAccessGroupRow = 0;
				this.cmdChangeAccessGroup.Enabled = true;
				this.cmdRemoveAccessGroup.Enabled = true;
			}

			this.cmdChangeAT.Enabled = false;
			if (this.m_dvAccessTypes.Count > 0)
			{
				this.m_nATRow = 0;
				this.cmdChangeAT.Enabled = true;
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion Initialization

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DManagement));
            this.pnlPageBottom = new System.Windows.Forms.Panel();
            this.cmdClose = new System.Windows.Forms.Button();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tpResources = new System.Windows.Forms.TabPage();
            this.pnlResources = new System.Windows.Forms.Panel();
            this.grdResources = new System.Windows.Forms.DataGrid();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.grpDescription = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnlAddEdit = new System.Windows.Forms.Panel();
            this.cmdRemoveUser = new System.Windows.Forms.Button();
            this.cmdChangeResource = new System.Windows.Forms.Button();
            this.cmdAddResource = new System.Windows.Forms.Button();
            this.tpResourceGroups = new System.Windows.Forms.TabPage();
            this.grdResourceGroup = new System.Windows.Forms.DataGrid();
            this.pnlDescriptionResourceGroup = new System.Windows.Forms.Panel();
            this.grpDescriptionResourceGroup = new System.Windows.Forms.GroupBox();
            this.lblDescriptionResourceGroup = new System.Windows.Forms.Label();
            this.pnlAddEditResourceGroups = new System.Windows.Forms.Panel();
            this.cmdChangeResourceGroup = new System.Windows.Forms.Button();
            this.cmdRemoveResourceGroup = new System.Windows.Forms.Button();
            this.cmdAddResourceGroup = new System.Windows.Forms.Button();
            this.tpAccessTypes = new System.Windows.Forms.TabPage();
            this.grdAccessTypes = new System.Windows.Forms.DataGrid();
            this.pnlDescriptionAT = new System.Windows.Forms.Panel();
            this.grpDescriptionAT = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAddEditAT = new System.Windows.Forms.Panel();
            this.cmdChangeAT = new System.Windows.Forms.Button();
            this.cmdAddAT = new System.Windows.Forms.Button();
            this.tpAccessGroups = new System.Windows.Forms.TabPage();
            this.grdAccessGroups = new System.Windows.Forms.DataGrid();
            this.pnlDescriptionAccessGroups = new System.Windows.Forms.Panel();
            this.grpDescriptionAccessGroups = new System.Windows.Forms.GroupBox();
            this.lblDescriptionAccessGroups = new System.Windows.Forms.Label();
            this.pnlAddEditAccessGroup = new System.Windows.Forms.Panel();
            this.cmdChangeAccessGroup = new System.Windows.Forms.Button();
            this.cmdRemoveAccessGroup = new System.Windows.Forms.Button();
            this.cmdAddAccessGroup = new System.Windows.Forms.Button();
            this.tpTransferAppts = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpBegin = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBSDXClinic = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboRPMSClinic = new System.Windows.Forms.ComboBox();
            this.pnlDescriptionXfer = new System.Windows.Forms.Panel();
            this.grpDescriptionXfer = new System.Windows.Forms.GroupBox();
            this.lblDescriptionXfer = new System.Windows.Forms.Label();
            this.pnlCmdXfer = new System.Windows.Forms.Panel();
            this.cmdCopyAppts = new System.Windows.Forms.Button();
            this.tpWorkStations = new System.Windows.Forms.TabPage();
            this.grdWorkStations = new System.Windows.Forms.DataGrid();
            this.pnlWorkstations = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblWorkstations = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSendMessage = new System.Windows.Forms.TextBox();
            this.cmdWorkStationsMessage = new System.Windows.Forms.Button();
            this.cmdWorkStationsShutdown = new System.Windows.Forms.Button();
            this.cmdWorkStationsRefresh = new System.Windows.Forms.Button();
            this.pnlPageBottom.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tpResources.SuspendLayout();
            this.pnlResources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResources)).BeginInit();
            this.pnlDescription.SuspendLayout();
            this.grpDescription.SuspendLayout();
            this.pnlAddEdit.SuspendLayout();
            this.tpResourceGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResourceGroup)).BeginInit();
            this.pnlDescriptionResourceGroup.SuspendLayout();
            this.grpDescriptionResourceGroup.SuspendLayout();
            this.pnlAddEditResourceGroups.SuspendLayout();
            this.tpAccessTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAccessTypes)).BeginInit();
            this.pnlDescriptionAT.SuspendLayout();
            this.grpDescriptionAT.SuspendLayout();
            this.pnlAddEditAT.SuspendLayout();
            this.tpAccessGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAccessGroups)).BeginInit();
            this.pnlDescriptionAccessGroups.SuspendLayout();
            this.grpDescriptionAccessGroups.SuspendLayout();
            this.pnlAddEditAccessGroup.SuspendLayout();
            this.tpTransferAppts.SuspendLayout();
            this.pnlDescriptionXfer.SuspendLayout();
            this.grpDescriptionXfer.SuspendLayout();
            this.pnlCmdXfer.SuspendLayout();
            this.tpWorkStations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdWorkStations)).BeginInit();
            this.pnlWorkstations.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPageBottom
            // 
            this.pnlPageBottom.Controls.Add(this.cmdClose);
            this.pnlPageBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPageBottom.Location = new System.Drawing.Point(0, 454);
            this.pnlPageBottom.Name = "pnlPageBottom";
            this.pnlPageBottom.Size = new System.Drawing.Size(704, 40);
            this.pnlPageBottom.TabIndex = 3;
            // 
            // cmdClose
            // 
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdClose.Location = new System.Drawing.Point(600, 8);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(96, 24);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Text = "Close";
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tpResources);
            this.tabMain.Controls.Add(this.tpResourceGroups);
            this.tabMain.Controls.Add(this.tpAccessTypes);
            this.tabMain.Controls.Add(this.tpAccessGroups);
            this.tabMain.Controls.Add(this.tpTransferAppts);
            this.tabMain.Controls.Add(this.tpWorkStations);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(704, 454);
            this.tabMain.TabIndex = 4;
            this.tabMain.Click += new System.EventHandler(this.tabMain_Click);
            // 
            // tpResources
            // 
            this.tpResources.Controls.Add(this.pnlResources);
            this.tpResources.Controls.Add(this.pnlDescription);
            this.tpResources.Controls.Add(this.pnlAddEdit);
            this.tpResources.Location = new System.Drawing.Point(4, 22);
            this.tpResources.Name = "tpResources";
            this.tpResources.Size = new System.Drawing.Size(696, 428);
            this.tpResources.TabIndex = 0;
            this.tpResources.Text = "Resources and Users";
            // 
            // pnlResources
            // 
            this.pnlResources.Controls.Add(this.grdResources);
            this.pnlResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResources.Location = new System.Drawing.Point(0, 0);
            this.pnlResources.Name = "pnlResources";
            this.pnlResources.Size = new System.Drawing.Size(696, 316);
            this.pnlResources.TabIndex = 2;
            // 
            // grdResources
            // 
            this.grdResources.DataMember = "";
            this.grdResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdResources.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdResources.Location = new System.Drawing.Point(0, 0);
            this.grdResources.Name = "grdResources";
            this.grdResources.ReadOnly = true;
            this.grdResources.Size = new System.Drawing.Size(696, 316);
            this.grdResources.TabIndex = 0;
            this.grdResources.CurrentCellChanged += new System.EventHandler(this.grdResources_CurrentCellChanged);
            this.grdResources.Navigate += new System.Windows.Forms.NavigateEventHandler(this.grdResources_Navigate);
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.grpDescription);
            this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescription.Location = new System.Drawing.Point(0, 316);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Size = new System.Drawing.Size(696, 72);
            this.pnlDescription.TabIndex = 1;
            // 
            // grpDescription
            // 
            this.grpDescription.Controls.Add(this.lblDescription);
            this.grpDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescription.Location = new System.Drawing.Point(0, 0);
            this.grpDescription.Name = "grpDescription";
            this.grpDescription.Size = new System.Drawing.Size(696, 72);
            this.grpDescription.TabIndex = 0;
            this.grpDescription.TabStop = false;
            this.grpDescription.Text = "Description";
            // 
            // lblDescription
            // 
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 16);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(690, 53);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = resources.GetString("lblDescription.Text");
            // 
            // pnlAddEdit
            // 
            this.pnlAddEdit.Controls.Add(this.cmdRemoveUser);
            this.pnlAddEdit.Controls.Add(this.cmdChangeResource);
            this.pnlAddEdit.Controls.Add(this.cmdAddResource);
            this.pnlAddEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAddEdit.Location = new System.Drawing.Point(0, 388);
            this.pnlAddEdit.Name = "pnlAddEdit";
            this.pnlAddEdit.Size = new System.Drawing.Size(696, 40);
            this.pnlAddEdit.TabIndex = 0;
            // 
            // cmdRemoveUser
            // 
            this.cmdRemoveUser.Location = new System.Drawing.Point(304, 8);
            this.cmdRemoveUser.Name = "cmdRemoveUser";
            this.cmdRemoveUser.Size = new System.Drawing.Size(128, 24);
            this.cmdRemoveUser.TabIndex = 3;
            this.cmdRemoveUser.Text = "&Remove User";
            this.cmdRemoveUser.Visible = false;
            this.cmdRemoveUser.Click += new System.EventHandler(this.cmdRemoveUser_Click);
            // 
            // cmdChangeResource
            // 
            this.cmdChangeResource.Enabled = false;
            this.cmdChangeResource.Location = new System.Drawing.Point(160, 8);
            this.cmdChangeResource.Name = "cmdChangeResource";
            this.cmdChangeResource.Size = new System.Drawing.Size(128, 24);
            this.cmdChangeResource.TabIndex = 1;
            this.cmdChangeResource.Text = "&Change...";
            this.cmdChangeResource.Click += new System.EventHandler(this.cmdChangeResource_Click);
            // 
            // cmdAddResource
            // 
            this.cmdAddResource.Location = new System.Drawing.Point(16, 8);
            this.cmdAddResource.Name = "cmdAddResource";
            this.cmdAddResource.Size = new System.Drawing.Size(128, 24);
            this.cmdAddResource.TabIndex = 0;
            this.cmdAddResource.Text = "&Add...";
            this.cmdAddResource.Click += new System.EventHandler(this.cmdAddResource_Click);
            // 
            // tpResourceGroups
            // 
            this.tpResourceGroups.Controls.Add(this.grdResourceGroup);
            this.tpResourceGroups.Controls.Add(this.pnlDescriptionResourceGroup);
            this.tpResourceGroups.Controls.Add(this.pnlAddEditResourceGroups);
            this.tpResourceGroups.Location = new System.Drawing.Point(4, 22);
            this.tpResourceGroups.Name = "tpResourceGroups";
            this.tpResourceGroups.Size = new System.Drawing.Size(579, 362);
            this.tpResourceGroups.TabIndex = 4;
            this.tpResourceGroups.Text = "Resource Groups";
            // 
            // grdResourceGroup
            // 
            this.grdResourceGroup.AccessibleName = "DataGrid";
            this.grdResourceGroup.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
            this.grdResourceGroup.DataMember = "";
            this.grdResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdResourceGroup.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdResourceGroup.Location = new System.Drawing.Point(0, 0);
            this.grdResourceGroup.Name = "grdResourceGroup";
            this.grdResourceGroup.ReadOnly = true;
            this.grdResourceGroup.Size = new System.Drawing.Size(579, 250);
            this.grdResourceGroup.TabIndex = 4;
            this.grdResourceGroup.CurrentCellChanged += new System.EventHandler(this.grdResourceGroup_CurrentCellChanged);
            this.grdResourceGroup.Navigate += new System.Windows.Forms.NavigateEventHandler(this.grdResourceGroup_Navigate);
            // 
            // pnlDescriptionResourceGroup
            // 
            this.pnlDescriptionResourceGroup.Controls.Add(this.grpDescriptionResourceGroup);
            this.pnlDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescriptionResourceGroup.Location = new System.Drawing.Point(0, 250);
            this.pnlDescriptionResourceGroup.Name = "pnlDescriptionResourceGroup";
            this.pnlDescriptionResourceGroup.Size = new System.Drawing.Size(579, 72);
            this.pnlDescriptionResourceGroup.TabIndex = 3;
            // 
            // grpDescriptionResourceGroup
            // 
            this.grpDescriptionResourceGroup.Controls.Add(this.lblDescriptionResourceGroup);
            this.grpDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescriptionResourceGroup.Location = new System.Drawing.Point(0, 0);
            this.grpDescriptionResourceGroup.Name = "grpDescriptionResourceGroup";
            this.grpDescriptionResourceGroup.Size = new System.Drawing.Size(579, 72);
            this.grpDescriptionResourceGroup.TabIndex = 0;
            this.grpDescriptionResourceGroup.TabStop = false;
            this.grpDescriptionResourceGroup.Text = "Description";
            // 
            // lblDescriptionResourceGroup
            // 
            this.lblDescriptionResourceGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescriptionResourceGroup.Location = new System.Drawing.Point(3, 16);
            this.lblDescriptionResourceGroup.Name = "lblDescriptionResourceGroup";
            this.lblDescriptionResourceGroup.Size = new System.Drawing.Size(573, 53);
            this.lblDescriptionResourceGroup.TabIndex = 0;
            this.lblDescriptionResourceGroup.Text = resources.GetString("lblDescriptionResourceGroup.Text");
            // 
            // pnlAddEditResourceGroups
            // 
            this.pnlAddEditResourceGroups.Controls.Add(this.cmdChangeResourceGroup);
            this.pnlAddEditResourceGroups.Controls.Add(this.cmdRemoveResourceGroup);
            this.pnlAddEditResourceGroups.Controls.Add(this.cmdAddResourceGroup);
            this.pnlAddEditResourceGroups.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAddEditResourceGroups.Location = new System.Drawing.Point(0, 322);
            this.pnlAddEditResourceGroups.Name = "pnlAddEditResourceGroups";
            this.pnlAddEditResourceGroups.Size = new System.Drawing.Size(579, 40);
            this.pnlAddEditResourceGroups.TabIndex = 2;
            // 
            // cmdChangeResourceGroup
            // 
            this.cmdChangeResourceGroup.Enabled = false;
            this.cmdChangeResourceGroup.Location = new System.Drawing.Point(160, 8);
            this.cmdChangeResourceGroup.Name = "cmdChangeResourceGroup";
            this.cmdChangeResourceGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdChangeResourceGroup.TabIndex = 2;
            this.cmdChangeResourceGroup.Text = "&Change Group";
            this.cmdChangeResourceGroup.Click += new System.EventHandler(this.cmdChangeResourceGroup_Click);
            // 
            // cmdRemoveResourceGroup
            // 
            this.cmdRemoveResourceGroup.Enabled = false;
            this.cmdRemoveResourceGroup.Location = new System.Drawing.Point(304, 8);
            this.cmdRemoveResourceGroup.Name = "cmdRemoveResourceGroup";
            this.cmdRemoveResourceGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdRemoveResourceGroup.TabIndex = 1;
            this.cmdRemoveResourceGroup.Text = "&Remove Group";
            this.cmdRemoveResourceGroup.Click += new System.EventHandler(this.cmdRemoveResourceGroup_Click);
            // 
            // cmdAddResourceGroup
            // 
            this.cmdAddResourceGroup.Location = new System.Drawing.Point(16, 8);
            this.cmdAddResourceGroup.Name = "cmdAddResourceGroup";
            this.cmdAddResourceGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdAddResourceGroup.TabIndex = 0;
            this.cmdAddResourceGroup.Text = "&Add Group";
            this.cmdAddResourceGroup.Click += new System.EventHandler(this.cmdAddResourceGroup_Click);
            // 
            // tpAccessTypes
            // 
            this.tpAccessTypes.Controls.Add(this.grdAccessTypes);
            this.tpAccessTypes.Controls.Add(this.pnlDescriptionAT);
            this.tpAccessTypes.Controls.Add(this.pnlAddEditAT);
            this.tpAccessTypes.Location = new System.Drawing.Point(4, 22);
            this.tpAccessTypes.Name = "tpAccessTypes";
            this.tpAccessTypes.Size = new System.Drawing.Size(579, 362);
            this.tpAccessTypes.TabIndex = 2;
            this.tpAccessTypes.Text = "Access Types";
            // 
            // grdAccessTypes
            // 
            this.grdAccessTypes.DataMember = "";
            this.grdAccessTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdAccessTypes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdAccessTypes.Location = new System.Drawing.Point(0, 0);
            this.grdAccessTypes.Name = "grdAccessTypes";
            this.grdAccessTypes.ReadOnly = true;
            this.grdAccessTypes.RowHeadersVisible = false;
            this.grdAccessTypes.Size = new System.Drawing.Size(579, 250);
            this.grdAccessTypes.TabIndex = 3;
            this.grdAccessTypes.CurrentCellChanged += new System.EventHandler(this.grdAccessTypes_CurrentCellChanged);
            // 
            // pnlDescriptionAT
            // 
            this.pnlDescriptionAT.Controls.Add(this.grpDescriptionAT);
            this.pnlDescriptionAT.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescriptionAT.Location = new System.Drawing.Point(0, 250);
            this.pnlDescriptionAT.Name = "pnlDescriptionAT";
            this.pnlDescriptionAT.Size = new System.Drawing.Size(579, 72);
            this.pnlDescriptionAT.TabIndex = 2;
            // 
            // grpDescriptionAT
            // 
            this.grpDescriptionAT.Controls.Add(this.label1);
            this.grpDescriptionAT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescriptionAT.Location = new System.Drawing.Point(0, 0);
            this.grpDescriptionAT.Name = "grpDescriptionAT";
            this.grpDescriptionAT.Size = new System.Drawing.Size(579, 72);
            this.grpDescriptionAT.TabIndex = 0;
            this.grpDescriptionAT.TabStop = false;
            this.grpDescriptionAT.Text = "Description";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(573, 53);
            this.label1.TabIndex = 1;
            this.label1.Text = " Use the Access Types panel to define the kinds of access available for schedulin" +
                "g at this facility.  Common types of access include Walkin, Scheduled, Same Day," +
                " and Dental Expanded Functions.";
            // 
            // pnlAddEditAT
            // 
            this.pnlAddEditAT.Controls.Add(this.cmdChangeAT);
            this.pnlAddEditAT.Controls.Add(this.cmdAddAT);
            this.pnlAddEditAT.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAddEditAT.Location = new System.Drawing.Point(0, 322);
            this.pnlAddEditAT.Name = "pnlAddEditAT";
            this.pnlAddEditAT.Size = new System.Drawing.Size(579, 40);
            this.pnlAddEditAT.TabIndex = 1;
            // 
            // cmdChangeAT
            // 
            this.cmdChangeAT.Location = new System.Drawing.Point(160, 8);
            this.cmdChangeAT.Name = "cmdChangeAT";
            this.cmdChangeAT.Size = new System.Drawing.Size(128, 24);
            this.cmdChangeAT.TabIndex = 1;
            this.cmdChangeAT.Text = "&Change Access Type";
            this.cmdChangeAT.Click += new System.EventHandler(this.cmdChangeAT_Click);
            // 
            // cmdAddAT
            // 
            this.cmdAddAT.Location = new System.Drawing.Point(16, 8);
            this.cmdAddAT.Name = "cmdAddAT";
            this.cmdAddAT.Size = new System.Drawing.Size(128, 24);
            this.cmdAddAT.TabIndex = 0;
            this.cmdAddAT.Text = "&Add Access Type";
            this.cmdAddAT.Click += new System.EventHandler(this.cmdAddAT_Click);
            // 
            // tpAccessGroups
            // 
            this.tpAccessGroups.Controls.Add(this.grdAccessGroups);
            this.tpAccessGroups.Controls.Add(this.pnlDescriptionAccessGroups);
            this.tpAccessGroups.Controls.Add(this.pnlAddEditAccessGroup);
            this.tpAccessGroups.Location = new System.Drawing.Point(4, 22);
            this.tpAccessGroups.Name = "tpAccessGroups";
            this.tpAccessGroups.Size = new System.Drawing.Size(579, 362);
            this.tpAccessGroups.TabIndex = 1;
            this.tpAccessGroups.Text = "Access Groups";
            // 
            // grdAccessGroups
            // 
            this.grdAccessGroups.AccessibleName = "DataGrid";
            this.grdAccessGroups.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
            this.grdAccessGroups.DataMember = "";
            this.grdAccessGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdAccessGroups.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdAccessGroups.Location = new System.Drawing.Point(0, 0);
            this.grdAccessGroups.Name = "grdAccessGroups";
            this.grdAccessGroups.ReadOnly = true;
            this.grdAccessGroups.Size = new System.Drawing.Size(579, 250);
            this.grdAccessGroups.TabIndex = 5;
            this.grdAccessGroups.CurrentCellChanged += new System.EventHandler(this.grdAccessGroups_CurrentCellChanged);
            this.grdAccessGroups.Navigate += new System.Windows.Forms.NavigateEventHandler(this.grdAccessGroups_Navigate);
            // 
            // pnlDescriptionAccessGroups
            // 
            this.pnlDescriptionAccessGroups.Controls.Add(this.grpDescriptionAccessGroups);
            this.pnlDescriptionAccessGroups.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescriptionAccessGroups.Location = new System.Drawing.Point(0, 250);
            this.pnlDescriptionAccessGroups.Name = "pnlDescriptionAccessGroups";
            this.pnlDescriptionAccessGroups.Size = new System.Drawing.Size(579, 72);
            this.pnlDescriptionAccessGroups.TabIndex = 4;
            // 
            // grpDescriptionAccessGroups
            // 
            this.grpDescriptionAccessGroups.Controls.Add(this.lblDescriptionAccessGroups);
            this.grpDescriptionAccessGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescriptionAccessGroups.Location = new System.Drawing.Point(0, 0);
            this.grpDescriptionAccessGroups.Name = "grpDescriptionAccessGroups";
            this.grpDescriptionAccessGroups.Size = new System.Drawing.Size(579, 72);
            this.grpDescriptionAccessGroups.TabIndex = 0;
            this.grpDescriptionAccessGroups.TabStop = false;
            this.grpDescriptionAccessGroups.Text = "Description";
            // 
            // lblDescriptionAccessGroups
            // 
            this.lblDescriptionAccessGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescriptionAccessGroups.Location = new System.Drawing.Point(3, 16);
            this.lblDescriptionAccessGroups.Name = "lblDescriptionAccessGroups";
            this.lblDescriptionAccessGroups.Size = new System.Drawing.Size(573, 53);
            this.lblDescriptionAccessGroups.TabIndex = 0;
            this.lblDescriptionAccessGroups.Text = resources.GetString("lblDescriptionAccessGroups.Text");
            // 
            // pnlAddEditAccessGroup
            // 
            this.pnlAddEditAccessGroup.Controls.Add(this.cmdChangeAccessGroup);
            this.pnlAddEditAccessGroup.Controls.Add(this.cmdRemoveAccessGroup);
            this.pnlAddEditAccessGroup.Controls.Add(this.cmdAddAccessGroup);
            this.pnlAddEditAccessGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAddEditAccessGroup.Location = new System.Drawing.Point(0, 322);
            this.pnlAddEditAccessGroup.Name = "pnlAddEditAccessGroup";
            this.pnlAddEditAccessGroup.Size = new System.Drawing.Size(579, 40);
            this.pnlAddEditAccessGroup.TabIndex = 3;
            // 
            // cmdChangeAccessGroup
            // 
            this.cmdChangeAccessGroup.Enabled = false;
            this.cmdChangeAccessGroup.Location = new System.Drawing.Point(160, 8);
            this.cmdChangeAccessGroup.Name = "cmdChangeAccessGroup";
            this.cmdChangeAccessGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdChangeAccessGroup.TabIndex = 3;
            this.cmdChangeAccessGroup.Text = "&Change Group";
            this.cmdChangeAccessGroup.Click += new System.EventHandler(this.cmdChangeAccessGroup_Click);
            // 
            // cmdRemoveAccessGroup
            // 
            this.cmdRemoveAccessGroup.Enabled = false;
            this.cmdRemoveAccessGroup.Location = new System.Drawing.Point(304, 8);
            this.cmdRemoveAccessGroup.Name = "cmdRemoveAccessGroup";
            this.cmdRemoveAccessGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdRemoveAccessGroup.TabIndex = 1;
            this.cmdRemoveAccessGroup.Text = "&Remove Group";
            this.cmdRemoveAccessGroup.Click += new System.EventHandler(this.cmdRemoveAccessGroup_Click);
            // 
            // cmdAddAccessGroup
            // 
            this.cmdAddAccessGroup.Location = new System.Drawing.Point(16, 8);
            this.cmdAddAccessGroup.Name = "cmdAddAccessGroup";
            this.cmdAddAccessGroup.Size = new System.Drawing.Size(128, 24);
            this.cmdAddAccessGroup.TabIndex = 0;
            this.cmdAddAccessGroup.Text = "&Add Group";
            this.cmdAddAccessGroup.Click += new System.EventHandler(this.cmdAddAccessGroup_Click);
            // 
            // tpTransferAppts
            // 
            this.tpTransferAppts.Controls.Add(this.label5);
            this.tpTransferAppts.Controls.Add(this.dtpEnd);
            this.tpTransferAppts.Controls.Add(this.label4);
            this.tpTransferAppts.Controls.Add(this.dtpBegin);
            this.tpTransferAppts.Controls.Add(this.label3);
            this.tpTransferAppts.Controls.Add(this.cboBSDXClinic);
            this.tpTransferAppts.Controls.Add(this.label2);
            this.tpTransferAppts.Controls.Add(this.cboRPMSClinic);
            this.tpTransferAppts.Controls.Add(this.pnlDescriptionXfer);
            this.tpTransferAppts.Controls.Add(this.pnlCmdXfer);
            this.tpTransferAppts.Location = new System.Drawing.Point(4, 22);
            this.tpTransferAppts.Name = "tpTransferAppts";
            this.tpTransferAppts.Size = new System.Drawing.Size(579, 362);
            this.tpTransferAppts.TabIndex = 5;
            this.tpTransferAppts.Text = "Copy Appointments";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "End with appointments on:";
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(152, 128);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(232, 20);
            this.dtpEnd.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Start with appointments on:";
            // 
            // dtpBegin
            // 
            this.dtpBegin.Location = new System.Drawing.Point(152, 93);
            this.dtpBegin.Name = "dtpBegin";
            this.dtpBegin.Size = new System.Drawing.Size(232, 20);
            this.dtpBegin.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Copy To Windows Scheduling Resource:";
            // 
            // cboBSDXClinic
            // 
            this.cboBSDXClinic.Location = new System.Drawing.Point(152, 48);
            this.cboBSDXClinic.Name = "cboBSDXClinic";
            this.cboBSDXClinic.Size = new System.Drawing.Size(232, 21);
            this.cboBSDXClinic.TabIndex = 5;
            this.cboBSDXClinic.Text = "cboBSDXClinic";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Copy From RPMS Clinic:";
            // 
            // cboRPMSClinic
            // 
            this.cboRPMSClinic.Location = new System.Drawing.Point(152, 16);
            this.cboRPMSClinic.Name = "cboRPMSClinic";
            this.cboRPMSClinic.Size = new System.Drawing.Size(232, 21);
            this.cboRPMSClinic.TabIndex = 3;
            this.cboRPMSClinic.Text = "cboRPMSClinic";
            // 
            // pnlDescriptionXfer
            // 
            this.pnlDescriptionXfer.Controls.Add(this.grpDescriptionXfer);
            this.pnlDescriptionXfer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDescriptionXfer.Location = new System.Drawing.Point(0, 250);
            this.pnlDescriptionXfer.Name = "pnlDescriptionXfer";
            this.pnlDescriptionXfer.Size = new System.Drawing.Size(579, 72);
            this.pnlDescriptionXfer.TabIndex = 2;
            // 
            // grpDescriptionXfer
            // 
            this.grpDescriptionXfer.Controls.Add(this.lblDescriptionXfer);
            this.grpDescriptionXfer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDescriptionXfer.Location = new System.Drawing.Point(0, 0);
            this.grpDescriptionXfer.Name = "grpDescriptionXfer";
            this.grpDescriptionXfer.Size = new System.Drawing.Size(579, 72);
            this.grpDescriptionXfer.TabIndex = 0;
            this.grpDescriptionXfer.TabStop = false;
            this.grpDescriptionXfer.Text = "Description";
            // 
            // lblDescriptionXfer
            // 
            this.lblDescriptionXfer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescriptionXfer.Location = new System.Drawing.Point(3, 16);
            this.lblDescriptionXfer.Name = "lblDescriptionXfer";
            this.lblDescriptionXfer.Size = new System.Drawing.Size(573, 53);
            this.lblDescriptionXfer.TabIndex = 0;
            this.lblDescriptionXfer.Text = resources.GetString("lblDescriptionXfer.Text");
            // 
            // pnlCmdXfer
            // 
            this.pnlCmdXfer.Controls.Add(this.cmdCopyAppts);
            this.pnlCmdXfer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCmdXfer.Location = new System.Drawing.Point(0, 322);
            this.pnlCmdXfer.Name = "pnlCmdXfer";
            this.pnlCmdXfer.Size = new System.Drawing.Size(579, 40);
            this.pnlCmdXfer.TabIndex = 1;
            // 
            // cmdCopyAppts
            // 
            this.cmdCopyAppts.Location = new System.Drawing.Point(16, 8);
            this.cmdCopyAppts.Name = "cmdCopyAppts";
            this.cmdCopyAppts.Size = new System.Drawing.Size(128, 24);
            this.cmdCopyAppts.TabIndex = 0;
            this.cmdCopyAppts.Text = "&Copy Appointments";
            this.cmdCopyAppts.Click += new System.EventHandler(this.cmdCopyAppts_Click);
            // 
            // tpWorkStations
            // 
            this.tpWorkStations.Controls.Add(this.grdWorkStations);
            this.tpWorkStations.Controls.Add(this.pnlWorkstations);
            this.tpWorkStations.Controls.Add(this.panel1);
            this.tpWorkStations.Location = new System.Drawing.Point(4, 22);
            this.tpWorkStations.Name = "tpWorkStations";
            this.tpWorkStations.Size = new System.Drawing.Size(579, 362);
            this.tpWorkStations.TabIndex = 6;
            this.tpWorkStations.Text = "WorkStations";
            // 
            // grdWorkStations
            // 
            this.grdWorkStations.AccessibleName = "DataGrid";
            this.grdWorkStations.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
            this.grdWorkStations.DataMember = "";
            this.grdWorkStations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdWorkStations.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grdWorkStations.Location = new System.Drawing.Point(0, 0);
            this.grdWorkStations.Name = "grdWorkStations";
            this.grdWorkStations.ReadOnly = true;
            this.grdWorkStations.Size = new System.Drawing.Size(579, 250);
            this.grdWorkStations.TabIndex = 6;
            // 
            // pnlWorkstations
            // 
            this.pnlWorkstations.Controls.Add(this.groupBox1);
            this.pnlWorkstations.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlWorkstations.Location = new System.Drawing.Point(0, 250);
            this.pnlWorkstations.Name = "pnlWorkstations";
            this.pnlWorkstations.Size = new System.Drawing.Size(579, 72);
            this.pnlWorkstations.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblWorkstations);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(579, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Description";
            // 
            // lblWorkstations
            // 
            this.lblWorkstations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWorkstations.Location = new System.Drawing.Point(3, 16);
            this.lblWorkstations.Name = "lblWorkstations";
            this.lblWorkstations.Size = new System.Drawing.Size(573, 53);
            this.lblWorkstations.TabIndex = 0;
            this.lblWorkstations.Text = resources.GetString("lblWorkstations.Text");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSendMessage);
            this.panel1.Controls.Add(this.cmdWorkStationsMessage);
            this.panel1.Controls.Add(this.cmdWorkStationsShutdown);
            this.panel1.Controls.Add(this.cmdWorkStationsRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 322);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(579, 40);
            this.panel1.TabIndex = 4;
            // 
            // txtSendMessage
            // 
            this.txtSendMessage.Location = new System.Drawing.Point(440, 8);
            this.txtSendMessage.Name = "txtSendMessage";
            this.txtSendMessage.Size = new System.Drawing.Size(248, 20);
            this.txtSendMessage.TabIndex = 4;
            // 
            // cmdWorkStationsMessage
            // 
            this.cmdWorkStationsMessage.Location = new System.Drawing.Point(160, 8);
            this.cmdWorkStationsMessage.Name = "cmdWorkStationsMessage";
            this.cmdWorkStationsMessage.Size = new System.Drawing.Size(128, 24);
            this.cmdWorkStationsMessage.TabIndex = 3;
            this.cmdWorkStationsMessage.Text = "Send &Message";
            this.cmdWorkStationsMessage.Click += new System.EventHandler(this.cmdWorkStationsMessage_Click);
            // 
            // cmdWorkStationsShutdown
            // 
            this.cmdWorkStationsShutdown.Location = new System.Drawing.Point(304, 8);
            this.cmdWorkStationsShutdown.Name = "cmdWorkStationsShutdown";
            this.cmdWorkStationsShutdown.Size = new System.Drawing.Size(128, 24);
            this.cmdWorkStationsShutdown.TabIndex = 1;
            this.cmdWorkStationsShutdown.Text = "&Stop Workstations";
            this.cmdWorkStationsShutdown.Click += new System.EventHandler(this.cmdWorkStationsShutdown_Click);
            // 
            // cmdWorkStationsRefresh
            // 
            this.cmdWorkStationsRefresh.Location = new System.Drawing.Point(16, 8);
            this.cmdWorkStationsRefresh.Name = "cmdWorkStationsRefresh";
            this.cmdWorkStationsRefresh.Size = new System.Drawing.Size(128, 24);
            this.cmdWorkStationsRefresh.TabIndex = 0;
            this.cmdWorkStationsRefresh.Text = "&Refresh";
            this.cmdWorkStationsRefresh.Click += new System.EventHandler(this.cmdWorkStationsRefresh_Click);
            // 
            // DManagement
            // 
            this.AcceptButton = this.cmdClose;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(704, 494);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlPageBottom);
            this.Name = "DManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scheduling Management";
            this.Load += new System.EventHandler(this.DManagement_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DManagement_Closing);
            this.pnlPageBottom.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tpResources.ResumeLayout(false);
            this.pnlResources.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResources)).EndInit();
            this.pnlDescription.ResumeLayout(false);
            this.grpDescription.ResumeLayout(false);
            this.pnlAddEdit.ResumeLayout(false);
            this.tpResourceGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResourceGroup)).EndInit();
            this.pnlDescriptionResourceGroup.ResumeLayout(false);
            this.grpDescriptionResourceGroup.ResumeLayout(false);
            this.pnlAddEditResourceGroups.ResumeLayout(false);
            this.tpAccessTypes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdAccessTypes)).EndInit();
            this.pnlDescriptionAT.ResumeLayout(false);
            this.grpDescriptionAT.ResumeLayout(false);
            this.pnlAddEditAT.ResumeLayout(false);
            this.tpAccessGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdAccessGroups)).EndInit();
            this.pnlDescriptionAccessGroups.ResumeLayout(false);
            this.grpDescriptionAccessGroups.ResumeLayout(false);
            this.pnlAddEditAccessGroup.ResumeLayout(false);
            this.tpTransferAppts.ResumeLayout(false);
            this.pnlDescriptionXfer.ResumeLayout(false);
            this.grpDescriptionXfer.ResumeLayout(false);
            this.pnlCmdXfer.ResumeLayout(false);
            this.tpWorkStations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdWorkStations)).EndInit();
            this.pnlWorkstations.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region TabMain
		private void tabMain_Click(object sender, System.EventArgs e)
		{
			TabPage tp = tabMain.SelectedTab;
			if (tp.Text == "Resource Groups")
			{
				//20041109 Added below
				InitResourceGroupsPage();
			}
			//20041109 Added below
			if (tp.Text == "Access Groups")
			{
				InitAccessGroupsPage();
			}
		
		}
		#endregion TabMain

		#region Resources

		private void cmdAddResource_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (m_bEditUsers == true)
				{
					AddResourceUser();
					return;
				}

				DResource dRes = new DResource();
				dRes.InitializePage(-1, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to add new resource
				bool bInactive = dRes.Inactive;
				string sInactive = (bInactive == true)?"YES":"NO";
				string sResourceName = dRes.ResourceName;
				int nHospLocID = dRes.HospitalLocationID;
				string sHospLocID = nHospLocID.ToString();
				int nResourceID = dRes.ResourceID;
				string sResourceID = nResourceID.ToString();

				int nTimeScale = dRes.TimeScale;
				string sTimeScale = nTimeScale.ToString();
				string sLetterText = dRes.LetterText;

				string sNoShowLetter = dRes.NoShowLetterText;
				string sCancellationLetter = dRes.CancellationLetterText;

				string sSql = "BSDX ADD/EDIT RESOURCE^" + sResourceID + "|" + sResourceName + "|" + sInactive + "|" + sHospLocID + "|" + sTimeScale + "|" + sLetterText + "|" + sNoShowLetter + "|" + sCancellationLetter;
				DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "Resource");

				Debug.Assert(dtRes.Rows.Count == 1);
				if (dtRes.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdAddResource_Click: Unable to add new Resource.");
				}
				DataRow dr = dtRes.Rows[0];
				int nErrorID = (int) dr["RESOURCEID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}
				m_dsGlobal.Tables["GroupResources"].Clear();
				m_dsGlobal.Tables["ResourceUser"].Clear();
				m_dsGlobal.Tables["Resources"].Clear();
				m_DocManager.LoadBSDXResourcesTable();

				m_DocManager.LoadGroupResourcesTable();
			
				m_DocManager.LoadResourceUserTable();

				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdChangeResource_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (m_bEditUsers == true)
				{
					ChangeResourceUser();
					return;
				}
			
				Object oSelectedResourceID = grdResources[grdResources.CurrentRowIndex, 0];
				int nSelectedResourceID = Convert.ToInt16(oSelectedResourceID);

				DResource dRes = new DResource();
				dRes.InitializePage(nSelectedResourceID, this.m_dsGlobal);

				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to change data for resource
				bool bInactive = dRes.Inactive;
				string sInactive = (bInactive == true)?"YES":"NO";
				string sResourceName = dRes.ResourceName;
				int nHospLocID = dRes.HospitalLocationID;
				string sHospLocID = nHospLocID.ToString();
				int nResourceID = dRes.ResourceID;
				string sResourceID = nResourceID.ToString();

				int nTimeScale = dRes.TimeScale;
				string sTimeScale = nTimeScale.ToString();
				string sLetterText = dRes.LetterText;

				string sNoShowLetter = dRes.NoShowLetterText;
				string sCancellationLetter = dRes.CancellationLetterText;

				//Replace all crlfs with "@~" character combination
				//sLetterText = sLetterText.Replace("\r\n","@~");

				string sSql = "BSDX ADD/EDIT RESOURCE^" + sResourceID + "|" + sResourceName + "|" + sInactive + "|" + sHospLocID + "|" + sTimeScale + "|" + sLetterText + "|" + sNoShowLetter + "|" + sCancellationLetter;
				DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "Resource");

				if (dtRes.Rows.Count != 1)
				{
					Exception ex = new Exception("Unable to update Resource file");
					throw ex;
				}
				DataRow rw = dtRes.Rows[0];
				string sError = rw["ERRORTEXT"].ToString();
				if (sError != "")
				{
					Exception ex = new Exception(sError);
					throw ex;
				}

				m_dsGlobal.Tables["ResourceUser"].Clear();

				m_dsGlobal.Tables["Resources"].Clear();
				m_DocManager.LoadBSDXResourcesTable();
				m_DocManager.LoadResourceUserTable();

				m_dsGlobal.Tables["GroupResources"].Clear();
				m_DocManager.LoadGroupResourcesTable();
			
				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void grdResources_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataGridCell dgCell;
			dgCell = this.grdResources.CurrentCell;
			m_nResourceRow = dgCell.RowNumber;
			this.grdResources.Select(m_nResourceRow);
			if (m_sMember == "Resource")
			{
				Object dgItem = grdResources[dgCell.RowNumber, 0];
				this.m_nResourceID = Convert.ToInt16(dgItem);
				Debug.Write("m_nResourceID changed to " + m_nResourceID.ToString() + "\n");
			}
			this.cmdChangeResource.Enabled = true;
		}
		private void grdResources_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
			m_sMember = grdResources.DataMember.ToString();
			if (m_sMember == "")
				m_sMember = "Resource";

			if (m_sMember == "ResourceUser")
			{
				m_bEditUsers = true;
				this.cmdAddResource.Text = "Add User...";
				this.cmdChangeResource.Text = "Change User...";
				this.cmdRemoveUser.Visible = true;
				this.lblDescription.Text = "Define the users who can create appointments and establish availability for a particular resource.  Users must first be given basic access to the RPMS Scheduling package using standard RPMS menu and key management before they can be selected here and assigned to a resource.  Click the left-pointing arrow near the upper right of the window to go back to the list of resources.";
				int nRows = this.grdResources.VisibleRowCount;
				if (nRows == 0)
				{
					this.cmdChangeResource.Enabled = false;
					this.cmdRemoveUser.Enabled = false;
				}
				else
				{
					grdResources.CurrentCell = new DataGridCell(0, 0);
					this.cmdChangeResource.Enabled = true;
					this.cmdRemoveUser.Enabled = true;
				}
			}
			else
			{
				m_bEditUsers = false;
				this.cmdAddResource.Text = "Add ...";
				this.cmdChangeResource.Text = "Change...";
				this.cmdRemoveUser.Visible = false;
				this.lblDescription.Text="Use the resources panel to define the set of clinical entities available for scheduling at this facility.  Resources may include care providers (for example, dentists and physicians) or other kinds of scheduled services, facilities or equipment.  Click the small + sign next to the resource name to manage the users who can schedule this resource.";
				this.cmdChangeResource.Enabled = true;
			}
		}

		#endregion Resources

		#region ResourceUser

		private void AddResourceUser()
		{
			DResourceUser dRes = new DResourceUser();

			//Find out if there are any rows here
			int nRows = this.grdResources.VisibleRowCount;
			if (nRows == 0)
			{
				this.cmdChangeResource.Enabled = false;
				this.cmdRemoveUser.Enabled = false;
			}

			dRes.InitializePage(-1, this.m_dsGlobal);
			if (dRes.ShowDialog(this) == DialogResult.Cancel)
			{
				return;
			}
			//Call RPMS to add new Resource User
			int nUserID = dRes.UserID;
			string sUserID = nUserID.ToString();
			bool bOverbook = dRes.Overbook;
			string sOverbook = (bOverbook == true)?"YES":"NO";
			bool bModifySchedule = dRes.ModifySchedule;
			string sModifySchedule = (bModifySchedule == true)?"YES":"NO";
			bool bAppointments = dRes.Appoinmtments;
			string sAppointments = (bAppointments == true)?"YES":"NO";

			string sSql = "BSDX ADD/EDIT RESOURCEUSER^" + "0" + "|" + sOverbook + "|" + sModifySchedule + "|" + m_nResourceID + "|" + sUserID + "|" + sAppointments;
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResourceUser");

			m_dsGlobal.Tables["ResourceUser"].Clear();
			m_DocManager.LoadResourceUserTable(true);
			
			m_DocManager.UpdateViews();
		}


		private void ChangeResourceUser()
		{
			Object oSelectedResourceUserID = grdResources[grdResources.CurrentCell.RowNumber, 1];
			int nSelectedResourceUserID = Convert.ToInt16(oSelectedResourceUserID);
			DResourceUser dRes = new DResourceUser();
			dRes.InitializePage(nSelectedResourceUserID, this.m_dsGlobal);
			if (dRes.ShowDialog(this) != DialogResult.OK)
			{
				return;
			}
			//Call RPMS to change Resource User
			int nUserID = dRes.UserID;
			string sUserID = nUserID.ToString();
			bool bOverbook = dRes.Overbook;
			string sOverbook = (bOverbook == true)?"YES":"NO";
			bool bModifySchedule = dRes.ModifySchedule;
			string sModifySchedule = (bModifySchedule == true)?"YES":"NO";
			bool bAppointments = dRes.Appoinmtments;
			string sAppointments = (bAppointments == true)?"YES":"NO";

			string sSql = "BSDX ADD/EDIT RESOURCEUSER^" + nSelectedResourceUserID.ToString() + "|" + sOverbook + "|" + sModifySchedule + "|" + m_nResourceID + "|" + sUserID + "|" + sAppointments ;
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResourceUser");

			m_dsGlobal.Tables["ResourceUser"].Clear();
			m_DocManager.LoadResourceUserTable(true);
			
			m_DocManager.UpdateViews();
		}


		private void cmdRemoveUser_Click(object sender, System.EventArgs e)
		{
			Object oSelectedResourceUserID = grdResources[grdResources.CurrentCell.RowNumber, 1];
			int nSelectedResourceUserID = Convert.ToInt16(oSelectedResourceUserID);

			string sSql = "BSDX DELETE RESOURCEUSER^" + nSelectedResourceUserID.ToString();
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResUsr");

			m_dsGlobal.Tables["ResourceUser"].Clear();
			m_DocManager.LoadResourceUserTable(true);
			
			m_DocManager.UpdateViews();
			int nRows = this.grdResources.VisibleRowCount;
			if (nRows == 0)
			{
				this.cmdChangeResource.Enabled = false;
				this.cmdRemoveUser.Enabled = false;
			}
			else
			{
				grdResources.Select(0);
				this.cmdChangeResource.Enabled = true;
				this.cmdRemoveUser.Enabled = true;
			}
		}

		#endregion ResourceUser

		#region ResourceGroups

		//20041109 Added below
		private void InitResourceGroupsPage()
		{
			this.cmdChangeResourceGroup.Enabled = false;
			this.cmdRemoveResourceGroup.Enabled = false;
			if (this.m_dvResourceGroup.Count > 0)
			{
				this.m_nResourceGroupRow = 0;

				this.grdResourceGroup.CurrentCell = new DataGridCell(0,0);
				this.grdResourceGroup.Select(0);
				this.m_nResourceGroupRow = 0;
				object dgItem = this.grdResourceGroup[0,0];
				this.m_nResourceGroupID = Convert.ToInt16(dgItem);
				dgItem = grdResourceGroup[0,1];
				this.m_sResourceGroupName = dgItem.ToString();

				this.cmdChangeResourceGroup.Enabled = true;
				this.cmdRemoveResourceGroup.Enabled = true;
			}		
		}

		private void cmdAddResourceGroup_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (m_bEditGroupItems == true)
				{
					AddResourceGroupItem();
					return;
				}

				DResourceGroup dRes = new DResourceGroup();
				dRes.InitializePage(-1, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to add new resource

				string sResGroupName = dRes.ResourceGroupName;

				string sSql = "BSDX ADD/EDIT RESOURCE GROUP^0|" + sResGroupName;
				DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "ResourceGroup");
				
				Debug.Assert(dtRes.Rows.Count == 1);
				if (dtRes.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdAddResource_Click: Unable to add new Resource Group.");
				}
				DataRow dr = dtRes.Rows[0];
				int nErrorID = (int) dr["RESOURCEGROUPID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_dsGlobal.Tables["GroupResources"].Clear();
				m_DocManager.LoadGroupResourcesTable();
				m_DocManager.LoadResourceGroupTable();
			
				m_DocManager.UpdateViews();

				//20041109 Added below
				InitResourceGroupsPage();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void grdResourceGroup_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataGridCell dgCell;
			dgCell = grdResourceGroup.CurrentCell;
			m_nResourceGroupRow = dgCell.RowNumber;
			grdResourceGroup.Select(m_nResourceGroupRow);
			if (m_sGroupMember == "Group")
			{
				Object dgItem = grdResourceGroup[dgCell.RowNumber, 1];
				m_sResourceGroupName = dgItem.ToString();
				dgItem = grdResourceGroup[dgCell.RowNumber, 0];
				m_nResourceGroupID = Convert.ToInt16(dgItem);
				Debug.Write("m_nResourceGroupID changed to " + m_nResourceGroupID.ToString() + "\n");
			}
			this.cmdRemoveResourceGroup.Enabled = true;
			this.cmdChangeResourceGroup.Enabled = true;
		}
		

		private void grdResourceGroup_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
			m_sGroupMember = grdResourceGroup.DataMember.ToString();
			if (m_sGroupMember == "")
				m_sGroupMember = "Group";

			if (m_sGroupMember == "GroupResource")
			{
				m_bEditGroupItems = true;
				cmdAddResourceGroup.Text = "Add Resource";
				this.cmdRemoveResourceGroup.Text = "Remove Resource";
				this.cmdChangeResourceGroup.Visible = false;
				this.cmdRemoveResourceGroup.Visible = true;
				this.lblDescriptionResourceGroup.Text = "Define the Resource which will be a part of this group.  Click the left-pointing arrow near the upper right of the window to go back to the list of Resource Groups.";
				int nRows = this.grdResourceGroup.VisibleRowCount;
				if (nRows == 0)
				{
					this.cmdRemoveResourceGroup.Enabled = false;
					this.cmdChangeResourceGroup.Visible = false;
				}
				else
				{
					grdResourceGroup.CurrentCell = new DataGridCell(0, 0);
					this.cmdChangeResourceGroup.Visible = false;
					this.cmdRemoveResourceGroup.Enabled = true;
				}
			}
			else
			{
				m_bEditGroupItems = false;
				this.cmdAddResourceGroup.Text = "&Add Group";
				this.cmdRemoveResourceGroup.Text = "&Remove Group";
				this.cmdChangeResourceGroup.Visible = true;
				this.lblDescriptionResourceGroup.Text="Use this panel to organize Resources into useful groups.  Resource Groups may include departments, clinics or any other collection of resources.  Resource groups will be visible to all scheduling users.";
			}		
		}

		private void cmdRemoveResourceGroup_Click(object sender, System.EventArgs e)
		{
			if (m_bEditGroupItems == true)
			{
				RemoveResourceGroupItem();
				return;
			}
		
			string sSql = "BSDX DELETE RESOURCE GROUP^" + m_sResourceGroupName;
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResGrp");

			m_dsGlobal.Tables["GroupResources"].Clear();
			m_DocManager.LoadGroupResourcesTable();
			m_DocManager.LoadResourceGroupTable();

			DataTable dt = m_dsGlobal.Tables["ResourceGroup"];
			DataRow dr = dt.Rows.Find(m_sResourceGroupName);
			dr.Delete();
			dr.AcceptChanges();
			
			m_DocManager.UpdateViews();

			//20041109 Added below
			InitResourceGroupsPage();

			//20041109 Removed below
//			int nRows = this.grdResourceGroup.VisibleRowCount;
//			if (nRows == 0)
//			{
//				this.cmdRemoveResourceGroup.Enabled = false;
//			}
//			else
//			{
//				grdResourceGroup.Select(0);
//				this.cmdRemoveResourceGroup.Enabled = true;
//			}

		}

		private void AddResourceGroupItem()
		{
			DResourceGroupItem dRes = new DResourceGroupItem();
			dRes.InitializePage(-1, this.m_dsGlobal);
			if (dRes.ShowDialog(this) == DialogResult.Cancel)
			{
				return;
			}

			//Call RPMS to add new resource

			int nResID = dRes.ResourceID;

			string sSql = "BSDX ADD RES GROUP ITEM^" + m_nResourceGroupID.ToString() + "^" + nResID.ToString();
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "ResourceGroupItem");

			this.cmdRemoveResourceGroup.Enabled = true;

			m_dsGlobal.Tables["GroupResources"].Clear();
			m_DocManager.LoadGroupResourcesTable();
			
			m_DocManager.UpdateViews();

		}

		private void RemoveResourceGroupItem()
		{
			Object oSelectedResourceName = this.grdResourceGroup[grdResourceGroup.CurrentCell.RowNumber, 1];
			string sSelectedResourceName = oSelectedResourceName.ToString();

			Object oSelectedResourceGroupID = this.grdResourceGroup[grdResourceGroup.CurrentCell.RowNumber, 0];
			int nSelectedResourceGroupID = Convert.ToInt16(oSelectedResourceGroupID);

			Object oSelectedResourceItemID = this.grdResourceGroup[grdResourceGroup.CurrentCell.RowNumber, 2];
			int nSelectedResourceItemID = Convert.ToInt16(oSelectedResourceItemID);
			
			string sSql = "BSDX DELETE RES GROUP ITEM^" + nSelectedResourceGroupID.ToString() + "^" + nSelectedResourceItemID.ToString();
			DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResGrpItem");

			m_dsGlobal.Tables["GroupResources"].Clear();
			m_DocManager.LoadGroupResourcesTable();
			
			m_DocManager.UpdateViews();
			int nRows = this.grdResourceGroup.VisibleRowCount;
			if (nRows == 0)
			{
				this.cmdRemoveResourceGroup.Enabled = false;
			}
			else
			{
				grdResourceGroup.Select(0);
				this.cmdRemoveResourceGroup.Enabled = true;
			}
		}

		private void cmdChangeResourceGroup_Click(object sender, System.EventArgs e)
		{
			try
			{
				int nRows = this.grdResourceGroup.VisibleRowCount;
				if (nRows == 0)
				{
					Debug.Assert(false, "This code shouldn't execute.");
					m_sResourceGroupName = "";
					this.cmdChangeResource.Enabled = false;
					this.cmdRemoveResourceGroup.Enabled = false;
					return;
				}
				else
				{
					DataGridCell dgCell;
					dgCell = grdResourceGroup.CurrentCell;
					Object dgItem = grdResourceGroup[dgCell.RowNumber, 1];
					m_sResourceGroupName = dgItem.ToString();
					this.cmdChangeResource.Enabled = true;
					this.cmdRemoveResourceGroup.Enabled = true;
				}

				DataTable dt = m_dsGlobal.Tables["ResourceGroup"];
				DataRow dr = dt.Rows.Find(m_sResourceGroupName);
				int nRGID = Convert.ToInt16(dr["RESOURCE_GROUPID"]);

				DResourceGroup dRes = new DResourceGroup();
				dRes.ResourceGroupName = m_sResourceGroupName;
				dRes.InitializePage(nRGID, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to change resource group

				string sResGroupName = dRes.ResourceGroupName;
			
				string sSql = "BSDX ADD/EDIT RESOURCE GROUP^" + nRGID.ToString() + "|" + sResGroupName;
				DataTable dtRes = m_DocManager.RPMSDataTable(sSql, "TempResGrp");

				Debug.Assert(dtRes.Rows.Count == 1);
				if (dtRes.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdAddResource_Click: Unable to edit Resource Group.");
				}
				dr = dtRes.Rows[0];
				int nErrorID = (int) dr["RESOURCEGROUPID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_sResourceGroupName = sResGroupName;
				m_DocManager.LoadResourceGroupTable();
				m_dsGlobal.Tables["GroupResources"].Clear();
				m_DocManager.LoadGroupResourcesTable();


				dr.Delete();
				dr.AcceptChanges();
			
				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#endregion ResourceGroups

		#region AccessTypes

		private void grdAccessTypes_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataGridCell myCell;
			myCell = this.grdAccessTypes.CurrentCell;
			m_nATRow = myCell.RowNumber;
			grdAccessTypes.Select(m_nATRow);
			this.cmdChangeAT.Enabled = true;
		}

		private void cmdChangeAT_Click(object sender, System.EventArgs e)
		{
			try
			{
				DAccessType dAT = new DAccessType();
				dAT.InitializePage(m_nATRow, this.m_dsGlobal);

				if (dAT.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}
				//Call RPMS to change data for access type
				bool bInactive = dAT.Inactive;
				string sInactive = (bInactive == true)?"YES":"NO";
				string sAccessTypeName = dAT.AccessTypeName;
				string sColor = dAT.DisplayColor;
				string sRed = dAT.Red.ToString();
				string sBlue = dAT.Blue.ToString();
				string sGreen = dAT.Green.ToString();
				string sIEN = dAT.AccessIEN;
			
			
				string sSql = "BSDX ADD/EDIT ACCESS TYPE^" + sIEN + "|" + sAccessTypeName + "|" + sInactive + "|" + sColor + "|" + sRed + "|" + sGreen + "|" + sBlue;
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AccessType");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdChangeAT_Click: Unable to add new Access Type.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSTYPEID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_DocManager.LoadAccessTypesTable();
				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cmdAddAT_Click(object sender, System.EventArgs e)
		{
			try
			{
				DAccessType dAT = new DAccessType();
				dAT.InitializePage(-1, this.m_dsGlobal);
				if (dAT.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to add new access type
				bool bInactive = dAT.Inactive;
				string sInactive = (bInactive == true)?"YES":"NO";
				string sAccessTypeName = dAT.AccessTypeName;
				string sColor = dAT.DisplayColor;
				string sRed = dAT.Red.ToString();
				string sBlue = dAT.Blue.ToString();
				string sGreen = dAT.Green.ToString();

				string sSql = "BSDX ADD/EDIT ACCESS TYPE^0|" + sAccessTypeName + "|" + sInactive + "|" + sColor + "|" + sRed + "|" + sGreen + "|" + sBlue;
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AccessType");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdAddAT_Click: Unable to add new Resource.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSTYPEID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_DocManager.LoadAccessTypesTable();
				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#endregion AccessTypes

		#region AccessGroups

		private void InitAccessGroupsPage()
		{
			this.cmdChangeAccessGroup.Enabled = false;
			this.cmdRemoveAccessGroup.Enabled = false;
			if (this.m_dvAccessGroup.Count > 0)
			{
				this.m_nAccessGroupRow = 0;

				this.grdAccessGroups.CurrentCell = new DataGridCell(0,0);
				this.grdAccessGroups.Select(0);
				this.m_nAccessGroupRow = 0;
				object dgItem = this.grdAccessGroups[0,0];
				this.m_nAccessGroupID = Convert.ToInt16(dgItem);
				dgItem = grdAccessGroups[0,1];
				this.m_sAccessGroupName = dgItem.ToString();
				this.cmdChangeAccessGroup.Enabled = true;
				this.cmdRemoveAccessGroup.Enabled = true;
			}
		}

		private void grdAccessGroups_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataGridCell dgCell;
			dgCell = grdAccessGroups.CurrentCell;
			m_nAccessGroupRow = dgCell.RowNumber;
			grdAccessGroups.Select(m_nAccessGroupRow);
			if (m_sAccessGroupMember == "Group")
			{
				Object dgItem = grdAccessGroups[dgCell.RowNumber, 0];
				m_sAccessGroupName = dgItem.ToString();
				dgItem = grdAccessGroups[dgCell.RowNumber, 0];
				m_nAccessGroupID = Convert.ToInt16(dgItem);
				Debug.Write("m_nAccessGroupID changed to " + m_nAccessGroupID.ToString() + "\n");
				dgItem = grdAccessGroups[dgCell.RowNumber, 1];
				m_sAccessGroupName = dgItem.ToString();
				Debug.Write("m_sAccessGroupName changed to " + m_sAccessGroupName + "\n");
			}
			cmdRemoveAccessGroup.Enabled = true;
			cmdChangeAccessGroup.Enabled = true;
		}

		private void grdAccessGroups_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
			m_sAccessGroupMember = grdAccessGroups.DataMember.ToString();
			if (m_sAccessGroupMember == "")
			{
				m_sAccessGroupMember = "Group";
			}

			if (m_sAccessGroupMember == "AccessGroupType")
			{
				m_bEditAccessGroupItems = true;
				this.cmdAddAccessGroup.Text = "&Add Type";
				this.cmdRemoveAccessGroup.Text = "&Remove Type";
				cmdChangeAccessGroup.Visible = false;
				this.lblDescriptionAccessGroups.Text = "Define the Access Type which will be a part of this group.  Click the left-pointing arrow near the upper right of the window to go back to the list of Access Groups.";
				int nRows = this.grdAccessGroups.VisibleRowCount;
				if (nRows == 0)
				{
					this.cmdRemoveAccessGroup.Enabled = false;
					cmdChangeAccessGroup.Visible = false;
				}
				else
				{
					grdAccessGroups.CurrentCell = new DataGridCell(0, 0);
					cmdChangeAccessGroup.Visible = false;
					this.cmdRemoveAccessGroup.Enabled = true;
				}
			}
			else
			{
				m_bEditAccessGroupItems = false;
				this.cmdAddAccessGroup.Text = "&Add Group";
				this.cmdRemoveAccessGroup.Text = "&Remove Group";
				cmdChangeAccessGroup.Visible = true;
				this.lblDescriptionAccessGroups.Text="Use this panel to organize Access Types into convenient groups.  Access Groups are useful when selecting the Access Type (Walk-in, Same-Day, etc) to use when setting up the schedule for a resource.  Access Type Groups will be visible to all scheduling users.";
			}			
		}

		private void cmdAddAccessGroup_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (m_bEditAccessGroupItems == true)
				{
					AddAccessGroupItem();
					return;
				}

				DAccessGroup dRes = new DAccessGroup();
				dRes.InitializePage(-1, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to add new Acccess Group
				string sAccessGroupName = dRes.AccessGroupName;
				string sSql = "BSDX ADD/EDIT ACCESS GROUP^0|" + sAccessGroupName;
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AccessGroup");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdAddAccessGroup_Click: Unable to add new Resource.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSGROUPID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_dsGlobal.Tables["AccessGroupType"].Clear();
				m_dsGlobal.Tables["AccessGroup"].Clear();
				m_DocManager.LoadAccessGroupsTable();
				m_DocManager.LoadAccessGroupTypesTable();
			
				m_DocManager.UpdateViews();
				
				//20041109 Added
				InitAccessGroupsPage();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		private void cmdRemoveAccessGroup_Click(object sender, System.EventArgs e)
		{
			if (m_bEditAccessGroupItems == true)
			{
				RemoveAccessGroupItem();
				return;
			}
		
			string sSql = "BSDX DELETE ACCESS GROUP^" + this.m_nAccessGroupID;
			DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AccessGroup");

			m_dsGlobal.Tables["AccessGroupType"].Clear();
			m_dsGlobal.Tables["AccessGroup"].Clear();
			m_DocManager.LoadAccessGroupsTable();
			m_DocManager.LoadAccessGroupTypesTable();
			m_DocManager.UpdateViews();

			//20041109 Added
			InitAccessGroupsPage();	
		}

		private void cmdChangeAccessGroup_Click(object sender, System.EventArgs e)
		{
			try
			{
				int nRows = this.grdAccessGroups.VisibleRowCount;
				int nAccessGroupID;
				if (nRows == 0)
				{
					Debug.Assert(false, "This code shouldn't execute.");
					return;
				}
				else
				{
					DataGridCell dgCell;
					dgCell = grdAccessGroups.CurrentCell;
					Object dgItem = grdAccessGroups[dgCell.RowNumber, 1];
					m_sAccessGroupName = dgItem.ToString();
					dgItem = grdAccessGroups[dgCell.RowNumber, 0];
					nAccessGroupID = Convert.ToInt16(dgItem);
				}

				DAccessGroup dRes = new DAccessGroup();
				dRes.AccessGroupName = m_sAccessGroupName;
				dRes.InitializePage(nAccessGroupID, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to change resource group

				string sAccessGroupName = dRes.AccessGroupName;
			
				string sSql = "BSDX ADD/EDIT ACCESS GROUP^" + nAccessGroupID.ToString() + "|" + sAccessGroupName;
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "TempAccGrp");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:cmdChangeAccessGroup_Click: Unable to add new Resource.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSGROUPID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_sAccessGroupName = sAccessGroupName;
				m_dsGlobal.Tables["AccessGroupType"].Clear();
				m_dsGlobal.Tables["AccessGroup"].Clear();
				m_DocManager.LoadAccessGroupsTable();
				m_DocManager.LoadAccessGroupTypesTable();
			
				m_DocManager.UpdateViews();	
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void AddAccessGroupItem()
		{
			try
			{
				DAccessGroupItem dRes = new DAccessGroupItem();
				dRes.InitializePage(-1, this.m_dsGlobal);
				if (dRes.ShowDialog(this) == DialogResult.Cancel)
				{
					return;
				}

				//Call RPMS to add new AccessGroupItem

				int nAccessTypeID = dRes.AccessTypeID;

				string sSql = "BSDX ADD ACCESS GROUP ITEM^" + m_nAccessGroupID.ToString() + "^" + nAccessTypeID.ToString();
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AccessGroupItem");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:AddAccessGroupItem: Unable to add new Access Group Item.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSGROUPTYPEID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}


				this.cmdRemoveAccessGroup.Enabled = true;

				m_dsGlobal.Tables["AccessTypes"].Clear();
				m_dsGlobal.Tables["AccessGroupType"].Clear();
				m_DocManager.LoadAccessTypesTable();
				m_DocManager.LoadAccessGroupTypesTable();
			
				m_DocManager.UpdateViews();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void RemoveAccessGroupItem()
		{
			try
			{
				Object oSelectedAccessGroupID = this.grdAccessGroups[grdAccessGroups.CurrentCell.RowNumber, 1];
				int nSelectedAccessGroupID = Convert.ToInt16(oSelectedAccessGroupID);

				Object oSelectedAccessGroupItemID = this.grdAccessGroups[grdAccessGroups.CurrentCell.RowNumber, 3];
				int nSelectedAccessGroupItemID = Convert.ToInt16(oSelectedAccessGroupItemID);
			
				string sSql = "BSDX DELETE ACCESS GROUP ITEM^" + nSelectedAccessGroupID.ToString() + "^" + nSelectedAccessGroupItemID.ToString();
				DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "TempAccGrpItem");

				Debug.Assert(dtAppt.Rows.Count == 1);
				if (dtAppt.Rows.Count != 1)
				{
					throw new Exception("DManagement:RemoveAccessGroupItem: Unable to add new Access Group Item.");
				}
				DataRow dr = dtAppt.Rows[0];
				int nErrorID = (int) dr["ACCESSGROUPTYPEID"];
				if (nErrorID == 0)
				{
					throw new Exception((string) dr["ERRORTEXT"]);
				}

				m_dsGlobal.Tables["AccessGroupType"].Clear();
				m_DocManager.LoadAccessGroupTypesTable();
			
				m_DocManager.UpdateViews();
				int nRows = this.grdAccessGroups.VisibleRowCount;
				if (nRows == 0)
				{
					this.cmdRemoveAccessGroup.Enabled = false;
				}
				else
				{
					grdResourceGroup.Select(0);
					this.cmdRemoveAccessGroup.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#endregion AccessGroups

		#region TransferAppts

		private void cmdCopyAppts_Click(object sender, System.EventArgs e)
		{
			try
			{
				//Show a modeless progress dialog
				int nHospLocationID = Convert.ToInt32(this.cboRPMSClinic.SelectedValue);
				int nResourceID = Convert.ToInt32(this.cboBSDXClinic.SelectedValue);
				
				ThreadApptCopy tac = new ThreadApptCopy(this.dtpBegin.Value, this.dtpEnd.Value, 
					nHospLocationID.ToString(), cboRPMSClinic.Text, 
					nResourceID.ToString(), cboBSDXClinic.Text, 
					this.m_DocManager);

				Thread threadApptCopy = new Thread(new System.Threading.ThreadStart(tac.ThreadApptCopyProc));
				//threadApptCopy.ApartmentState = ApartmentState.STA;
                threadApptCopy.SetApartmentState(ApartmentState.STA);
				threadApptCopy.Start();

			}
			catch(Exception ex)
			{
				MessageBox.Show(this,ex.Message,"IHS Clinical Scheduling");
			}
		
		}


		public class ThreadApptCopy
		{
			private						DateTime m_dtBegin;
			private						DateTime m_dtEnd;
			private	string				m_HospLocationID;
			private string				m_HospLocationName;
			private string				m_ResourceID;
			private string				m_ResourceName;
			private CGDocumentManager	m_DocManager;

			public ThreadApptCopy(DateTime dtBegin, DateTime EndDate, 
				string HospLocationID, string HospLocationName, 
				string ResourceID, string ResourceName, 
				CGDocumentManager DocManager)
			{
				m_dtBegin = dtBegin;
				m_dtEnd = EndDate;
				m_HospLocationID = HospLocationID;
				m_HospLocationName = HospLocationName;
				m_ResourceID = ResourceID;
				m_ResourceName = ResourceName;
				m_DocManager = DocManager;
			}

			public void ThreadApptCopyProc()
			{
				DCopyAppts dCopy = new DCopyAppts();
				dCopy.InitializePage(m_dtBegin, m_dtEnd, 
					m_HospLocationID, m_HospLocationName, 
					m_ResourceID, m_ResourceName, 
					m_DocManager);
				dCopy.ShowDialog();
			}
		}

		#endregion TransferAppts

        #region Workstations
        private void cmdWorkStationsRefresh_Click(object sender, System.EventArgs e)
		{
			this.m_dtWSGrid.Clear();
			this.m_DocManager.ConnectInfo.RaiseEvent("BSDX CALL WORKSTATIONS", "A", true);
		}

		private BMXNetConnectInfo.BMXNetEventDelegate	MgrEventDelegate;
		delegate void UpdateWorkstationGridDelegate(string sParam);

		private void MgrEventHandler(Object obj, BMXNet.BMXNetEventArgs e)
		{
			try
			{
				if (e.BMXEvent == "BSDX WORKSTATION REPORT")
				{
					Debug.Write("DManagement Got Workstation Report\n");
					UpdateWorkstationGridDelegate uWSGd = new UpdateWorkstationGridDelegate(UpdateWorkstationGrid);
                    if (this.InvokeRequired == true) //ensures that handle is created
                    {
                        this.Invoke(uWSGd, new object[] { e.BMXParam });
                    }
                    else
                    {
                        UpdateWorkstationGrid(e.BMXParam);
                    }
				}
			}
			catch (Exception ex)
			{
				Debug.Write("MgrEventHandler exception: " + ex.Message + "\n");
			}
		}

		private void UpdateWorkstationGrid(string sParam)
		{
			string sDelim = "~";
			DataRow dr = this.m_dtWSGrid.NewRow();
			dr["UserName"] = BMXNetLib.Piece(sParam,sDelim,1);
			dr["Handle"] = BMXNetLib.Piece(sParam,sDelim,2);
			dr["Version"] = BMXNetLib.Piece(sParam,sDelim,3);		
			dr["Views"] = BMXNetLib.Piece(sParam,sDelim,4);
			m_dtWSGrid.Rows.Add(dr);
		}

		private void DManagement_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			m_DocManager.ConnectInfo.UnSubscribeEvent("BSDX WORKSTATION REPORT");
		}

		private void cmdWorkStationsMessage_Click(object sender, System.EventArgs e)
		{
			string sMessage;
			dInputText dlg = new dInputText();
			dlg.DialogTitle = "IHS Clinical Scheduling - Send Message to Scheduling Clients.";

			if (dlg.ShowDialog(this) != DialogResult.OK)
				return;

			sMessage = dlg.TextValue;

			if (sMessage == "")
				return;

			this.m_DocManager.ConnectInfo.RaiseEvent("BSDX ADMIN MESSAGE", sMessage, false);
		}

		private void cmdWorkStationsShutdown_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to shut down all IHS Clincal Scheduling clients?" ,"IHS Clinical Scheduling Client Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			this.m_DocManager.ConnectInfo.RaiseEvent("BSDX ADMIN SHUTDOWN", txtSendMessage.Text, false);
        }
        #endregion Workstations

    }
}
