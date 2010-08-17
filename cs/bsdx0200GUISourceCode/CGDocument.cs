using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Contains the array of appointments and availabily that make up the document class
	/// </summary>
	public class CGDocument : System.Object
	{
		
		public CGDocument()
		{
			m_appointments = new CGAppointments();
			m_pAvArray = new ArrayList();
			m_sResourcesArray = new ArrayList();
		}

		#region Member Variables
		public int				m_nColumnCount; //todo: this should point to the view's member for column count
		public int				m_nTimeUnits;
		private string			m_sDocName;
		public ArrayList		m_sResourcesArray; //keeps the resources
		public ScheduleType		m_ScheduleType;
		private DateTime		m_dSelectedDate; //Holds the user's selection from the dtpicker
		private DateTime		m_dStartDate; //Beginning date of document data
		private DateTime		m_dEndDate; //Ending date of document data
		public CGAppointments	m_appointments;
		private ArrayList		m_pAvArray;
		private CGDocumentManager m_DocManager;
		private DateTime		m_dLastRefresh = DateTime.Now;

		#endregion

		#region Properties

		/// <summary>
		/// Returns the latest refresh time for this document
		/// </summary>
		public DateTime LastRefreshed
		{
			get
			{
				return m_dLastRefresh;
			}			
		}

		/// <summary>
		/// The list of Resource names
		/// </summary>
		public ArrayList Resources
		{
			get
			{
				return this.m_sResourcesArray;
			}
			set
			{
				this.m_sResourcesArray = value;
			}
		}

		/// <summary>
		/// The array of CGAvailabilities that contains appt type and slots
		/// </summary>
		public ArrayList AvailabilityArray
		{
			get
			{
				return this.m_pAvArray;
			}
		}

		public CGDocumentManager DocManager
		{
			get
			{
				return m_DocManager;
			}
			set
			{
				m_DocManager = value;
			}
		}

		/// <summary>
		/// Contains the hashtable of appointments
		/// </summary>
		public CGAppointments Appointments
		{
			get
			{
				return m_appointments;
			}
		}

		/// <summary>
		/// Holds the date selected by the user in CGView.dateTimePicker1
		/// </summary>
		public DateTime SelectedDate
		{
			get
			{
				return this.m_dSelectedDate;
			}
			set
			{
				this.m_dSelectedDate = value;
				bool bRet = false;
				if (m_sResourcesArray.Count == 1)
				{
					bRet = this.WeekNeedsRefresh(1, m_dSelectedDate, out this.m_dStartDate, out this.m_dEndDate);
				}
				else
				{
					this.m_dStartDate = m_dSelectedDate;
					this.m_dEndDate = m_dSelectedDate;
					this.m_dEndDate = this.m_dEndDate.AddHours(23);
					this.m_dEndDate = this.m_dEndDate.AddMinutes(59);
					this.m_dEndDate = this.m_dEndDate.AddSeconds(59);
				}

				bRet = RefreshSchedule();
			}
		}

			/// <summary>
			/// Contains the beginning date of the appointment document
			/// </summary>
			public DateTime StartDate
		{
			get
			{
				return this.m_dStartDate;
			}
		}
		
		public string DocName
		{
			get
			{
				return this.m_sDocName;
			}
			set
			{
				this.m_sDocName = value;
			}
		}

		#endregion

		#region Methods

		public void UpdateAllViews()
		{
			//iterate through all views and call update.
			Hashtable h = CGDocumentManager.Current.Views;
			
			CGDocument d;
			foreach (CGView v in h.Keys)
			{
				d = (CGDocument) h[v];
				if (d == this)
				{
					v.UpdateArrays();
				}
			}

		}

		/// <summary>
		/// Update schedule based on info in RPMS
		/// </summary>
		private bool RefreshDaysSchedule()
		{
			try
			{
				string				sPatientName;
				string				sPatientID;
				DateTime			dStart;
				DateTime			dEnd;
				DateTime			dCheckIn;
				DateTime			dAuxTime;
				int					nKeyID;
				string				sNote;
				string				sResource;
				bool				bNoShow = false;
				string				sNoShow = "0";
				string				sHRN = "";
				int					nAccessTypeID; //used in autorebook
				string				sWalkIn = "0";
				bool				bWalkIn;
				CGAppointment		pAppointment;
				CGDocumentManager	pApp = CGDocumentManager.Current;
				DataTable			rAppointmentSchedule;

                //Nice to know that it gets set here!!!
				m_dLastRefresh = DateTime.Now;

				this.m_appointments.ClearAllAppointments();

                //  calls RPC to get appointments
				rAppointmentSchedule = CGSchedLib.CreateAppointmentSchedule(m_DocManager, m_sResourcesArray, this.m_dStartDate, this.m_dEndDate);
				
                // Datatable dumper into Debug Log (nice to know that this exists)
                CGSchedLib.OutputArray(rAppointmentSchedule, "rAppointmentSchedule");
				
                
                foreach (DataRow r in rAppointmentSchedule.Rows) 
				{
				
					if (r["APPOINTMENTID"].ToString() == "0")
					{
						string sMsg = r["NOTE"].ToString();
						throw new BMXNetException(sMsg);
					}
					nKeyID = Convert.ToInt32(r["APPOINTMENTID"].ToString());
					sResource =  r["RESOURCENAME"].ToString();
					sPatientName =r["PATIENTNAME"].ToString();
					sPatientID =r["PATIENTID"].ToString();
					dStart = (DateTime) r["START_TIME"];
					dEnd = (DateTime) r["END_TIME"];
					dCheckIn = new DateTime();
					dAuxTime = new DateTime();

					if (r["CHECKIN"].GetType() != typeof(System.DBNull))
						dCheckIn = (DateTime) r["CHECKIN"];
					if (r["AUXTIME"].GetType() != typeof(System.DBNull))
						dCheckIn = (DateTime) r["AUXTIME"];
					sNote = r["NOTE"].ToString();
					sNoShow = r["NOSHOW"].ToString();
					bNoShow = (sNoShow == "1")?true: false;
					sHRN = r["HRN"].ToString();
					nAccessTypeID = (int) r["ACCESSTYPEID"];
					sWalkIn = r["WALKIN"].ToString();
					bWalkIn = (sWalkIn == "1")?true: false;

					pAppointment = new CGAppointment();
					pAppointment.CreateAppointment(dStart, dEnd, sNote, nKeyID, sResource);
					pAppointment.PatientName = sPatientName;
					pAppointment.PatientID = Convert.ToInt32(sPatientID);
					if (dCheckIn.Ticks > 0)
						pAppointment.CheckInTime = dCheckIn;
					if (dAuxTime.Ticks > 0)
						pAppointment.AuxTime = dAuxTime;
					pAppointment.NoShow = bNoShow;
					pAppointment.HealthRecordNumber = sHRN;
					pAppointment.AccessTypeID = nAccessTypeID;
					pAppointment.WalkIn = bWalkIn;
					this.m_appointments.AddAppointment(pAppointment);

				}
			
				return true;
			}
			catch(Exception Ex)
			{
				Debug.Write("CGDocument.RefreshDaysSchedule error: " + Ex.Message + "\n");
				return false;
			}
		}


		public void OnNewDocument()
		{
			/*
			 * TEST EXCEPTION -- REMOVE AFTER TESTING
			 */
			//throw new Exception("Simulated Uncaught Exception");
			/*
			 * TEST EXCEPTION -- REMOVE AFTER TESTING
			 */

			//Open an empty document
			m_sResourcesArray.Clear();
			m_ScheduleType = ScheduleType.Resource;

			//Set initial From and To dates based on current day
			//			DateTime dDate = new DateTime(2001,12,05);  //Testing line
			DateTime dDate = DateTime.Today;
            
            //smh: Question: Where does bRet get used? It's a useless varaible so far.
			bool bRet = this.WeekNeedsRefresh(2,dDate, out this.m_dStartDate, out this.m_dEndDate);

			//Create new View
			CGView view = new CGView();
			view.InitializeDocView(this, 
				this.DocManager,
				m_dStartDate, 
				this.Appointments, 
				DocManager.WindowText);

			view.Show();
			view.SyncTree();
			view.Activate();
			this.UpdateAllViews();
		}

		private void SetDate(DateTime dDate)
		{
			bool bRet = false;
			if (m_ScheduleType == ScheduleType.Resource)
			{
				bRet = this.WeekNeedsRefresh(2,dDate, out this.m_dStartDate, out this.m_dEndDate);
			}
			else
			{
				this.m_dStartDate = dDate;
				this.m_dEndDate = dDate;
				this.m_dEndDate = this.m_dEndDate.AddHours(23);
				this.m_dEndDate = this.m_dEndDate.AddMinutes(59);
				this.m_dEndDate = this.m_dEndDate.AddSeconds(59);
			}

			bRet = RefreshSchedule();
			this.UpdateAllViews();
		}

		public void RefreshDocument()
		{
			bool bRet = RefreshSchedule();
			this.UpdateAllViews();
		}

		public void OnOpenDocument() 
		{
			try
			{
				//Create new Document
				m_ScheduleType = (m_sResourcesArray.Count == 1) ? ScheduleType.Resource: ScheduleType.Clinic;
				bool bRet = false;

				//Set initial From and To dates based on current day
				DateTime dDate = DateTime.Today;
				if (m_ScheduleType == ScheduleType.Resource)
				{
					bRet = this.WeekNeedsRefresh(1,dDate, out this.m_dStartDate, out this.m_dEndDate);
				}
				else
				{
					this.m_dStartDate = dDate;
					this.m_dEndDate = dDate;
					this.m_dEndDate = this.m_dEndDate.AddHours(23);
					this.m_dEndDate = this.m_dEndDate.AddMinutes(59);
					this.m_dEndDate = this.m_dEndDate.AddSeconds(59);
				}

				bRet = RefreshSchedule();

				CGView view = null;
				//If this document already has a view, the use it
				Hashtable h = CGDocumentManager.Current.Views;		
				CGDocument d;
				bool bReuseView = false;
				foreach (CGView v in h.Keys)
				{
					d = (CGDocument) h[v];
					if (d == this)
					{
						view = v;
						bReuseView = true;
						v.InitializeDocView(this.DocName);
						break;
					}
				}

				//Otherwise, create new View
				if (bReuseView == false)
				{
					view = new CGView();

					view.InitializeDocView(this, 
						this.DocManager,
						m_dStartDate, 
						this.Appointments, 
						this.DocName);

					view.Show();
					view.SyncTree();

				}
				this.UpdateAllViews();
			}
			catch (BMXNetException bmxEx)
			{
				throw bmxEx;
			}
			catch (Exception ex)
			{
				throw new BMXNet.BMXNetException("ClinicalScheduling.OnOpenDocument error:  " + ex.Message);
			}
		}

		private bool RefreshSchedule()
		{
			try
			{
				bool bRet = this.RefreshAvailabilitySchedule();
				if (bRet == false)
				{
					return bRet;
				}
				bRet = this.RefreshDaysSchedule();
				return bRet;
			}
			catch (ApplicationException aex)
			{
				Debug.Write("CGDocument.RefreshSchedule Application Error:  " + aex.Message + "\n");
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show("CGDocument.RefreshSchedule error:  " + ex.Message + "\n");
				return false;
			}
		}

		private bool RefreshAvailabilitySchedule()
		{
			try
			{
				if (this.m_DocManager.ConnectInfo.Connected == false)
				{
					m_DocManager.ConnectInfo.LoadConnectInfo();
				}
				System.IntPtr pHandle = m_DocManager.Handle;

				m_pAvArray.Clear();

				ArrayList saryApptTypes = new ArrayList();
				int nApptTypeID = 0;

				//Refresh Availability schedules
				DataTable rAvailabilitySchedule;
				rAvailabilitySchedule = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_sResourcesArray, this.m_dStartDate, this.m_dEndDate, saryApptTypes,/**/ m_ScheduleType, "0");
				CGSchedLib.OutputArray(rAvailabilitySchedule, "rAvailabilitySchedule");

				//Refresh Type Schedule
				string sResourceName = "";
				DataTable rTypeSchedule = new DataTable();;
				for (int j = 0; j < m_sResourcesArray.Count; j++)
				{
					sResourceName = m_sResourcesArray[j].ToString();
					DataTable dtTemp = CGSchedLib.CreateAssignedTypeSchedule(m_DocManager, sResourceName, this.m_dStartDate, this.m_dEndDate, m_ScheduleType);
					CGSchedLib.OutputArray(dtTemp, "dtTemp");
					if (j == 0)
					{
						rTypeSchedule = dtTemp;
					}
					else
					{
						rTypeSchedule = CGSchedLib.UnionBlocks(rTypeSchedule, dtTemp);
					}
				}
				CGSchedLib.OutputArray(rTypeSchedule, "rTypeSchedule");

				DateTime dStart;
				DateTime dEnd;
				DateTime dTypeStart;
				DateTime dTypeEnd;
				int nSlots;
				Rectangle	crRectA = new Rectangle(0,0,1,0);
				Rectangle	crRectB= new Rectangle(0,0,1,0);
				bool	bIsect;
				string sResourceList;
				string sAccessRuleList;


				foreach (DataRow rTemp in rAvailabilitySchedule.Rows)
				{
					//get StartTime, EndTime and Slots 
					dStart = (DateTime) rTemp["START_TIME"];
					dEnd = (DateTime) rTemp["END_TIME"];

					//TODO: Fix this slots datatype problem
					string sSlots = rTemp["SLOTS"].ToString();
					nSlots = Convert.ToInt16(sSlots);

					sResourceList = rTemp["RESOURCE"].ToString();
					sAccessRuleList = rTemp["ACCESS_TYPE"].ToString();

					string sNote = rTemp["NOTE"].ToString();

					if ((nSlots < -1000)||(sAccessRuleList == ""))
					{
						nApptTypeID = 0;
					}
					else 
					{
						foreach (DataRow rType in rTypeSchedule.Rows)
						{
						
							dTypeStart = (DateTime) rType["StartTime"];
							dTypeEnd = (DateTime) rType["EndTime"];
							//if start & end times overlap, then
							string sTypeResource = rType["ResourceName"].ToString();
							if ((dTypeStart.DayOfYear == dStart.DayOfYear)  && (sResourceList == sTypeResource))
							{
								crRectA.Y = GetTotalMinutes(dStart);
								crRectA.Height = GetTotalMinutes(dEnd) - crRectA.Top;
								crRectB.Y = GetTotalMinutes(dTypeStart);
								crRectB.Height = GetTotalMinutes(dTypeEnd) - crRectB.Top;
								bIsect = crRectA.IntersectsWith(crRectB);
								if (bIsect == true) 
								{
									//TODO: This code:
									//	nApptTypeID = (int) rType["AppointmentTypeID"];
									//Causes this exception:
									//Unhandled Exception: System.InvalidCastException: Specified cast is not valid.
									string sTemp = rType["AppointmentTypeID"].ToString();
									nApptTypeID = Convert.ToInt16(sTemp);
									break;
								}
							}
						}//end foreach datarow rType
					}

					AddAvailability(dStart, dEnd, nApptTypeID, nSlots, false, sResourceList, sAccessRuleList, sNote);
				}//end foreach datarow rTemp

				return true;
			}
			catch (Exception ex)
			{
				Debug.Write("CGDocument.RefreshAvailabilitySchedule error: " + ex.Message + "\n");
				return false;
			}
		}

		private int GetTotalMinutes(DateTime dDate)
		{
			return ((dDate.Hour * 60) + dDate.Minute);
		}

		public int AddAvailability(DateTime StartTime, DateTime EndTime, int nType, int nSlots, bool UpdateView, string sResourceList, string sAccessRuleList, string sNote)
		{
			//adds it to the object array
			//Returns the index in the array

			CGAvailability pNewAv = new CGAvailability();
			pNewAv.Create(StartTime, EndTime, nType, nSlots, sResourceList, sAccessRuleList);

			pNewAv.Note = sNote;

			//Look up the color and type name using the AppointmentTypes datatable
			DataTable dtType = this.m_DocManager.GlobalDataSet.Tables["AccessTypes"];
			DataRow dRow = dtType.Rows.Find(nType.ToString());
			if (dRow != null)
			{
				string sColor = dRow["DISPLAY_COLOR"].ToString();
				pNewAv.DisplayColor = sColor;
				string sTemp = dRow["RED"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				int nRed = Convert.ToInt16(sTemp);
				pNewAv.Red = nRed;
				sTemp = dRow["GREEN"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				int nGreen = Convert.ToInt16(sTemp);
				pNewAv.Green = nGreen;
				sTemp = dRow["BLUE"].ToString();
				sTemp = (sTemp == "")?"0":sTemp;
				int nBlue = Convert.ToInt16(sTemp);
				pNewAv.Blue = nBlue;

				string sName = dRow["ACCESS_TYPE_NAME"].ToString();
				pNewAv.AccessTypeName = sName;
			}

			int nIndex = 0;
			nIndex = m_pAvArray.Add(pNewAv);
			if (UpdateView == true) 
			{
				this.UpdateAllViews();
			}
			return nIndex;			
		}


		public void AddResource(string sResource)
		{
			//TODO:  Test that resource is not currently in list, that it IS a resource, etc
			this.m_sResourcesArray.Add(sResource);
			this.UpdateAllViews();
		}

		public void ClearResources()
		{
			this.m_sResourcesArray.Clear();
		}

		public int SlotsAvailable(DateTime dSelStart, DateTime dSelEnd, string sResource, out string sAccessType, out string sAvailabilityMessage)
		{
			sAccessType = "";
			sAvailabilityMessage = "";
			DateTime dStart;
			DateTime dEnd;
			int nAvailableSlots = 999;
			int nSlots = 0;
			int i = 0;
			CGAvailability pAv;
			Rectangle crRectA = new Rectangle(0,0,1,0);
			Rectangle crRectB = new Rectangle(0,0,1,0);
			bool bIsect;
			crRectB.Y = GetTotalMinutes(dSelStart);
			crRectB.Height = GetTotalMinutes(dSelEnd)- crRectB.Y;

			//			//loop thru m_pAvArray
			//			//Compare the start time and end time of eachblock
			while (i < m_pAvArray.Count) 
			{
				pAv =  (CGAvailability) m_pAvArray[i];
				dStart = pAv.StartTime;
				dEnd = pAv.EndTime;
				if ((sResource == pAv.ResourceList) && 
					((dSelStart.Date == dStart.Date) || (dSelStart.Date == dEnd.Date)))
				{
					crRectA.Y = (dStart.Date < dSelStart.Date) ? 0 : GetTotalMinutes(dStart);
					crRectA.Height = (dEnd.Date > dSelEnd.Date) ? 1440 : GetTotalMinutes(dEnd);
					crRectA.Height = crRectA.Height - crRectA.Y;
					bIsect = crRectA.IntersectsWith(crRectB);
					if (bIsect != false) 
					{
						nSlots = pAv.Slots;
						if (nSlots < 1) 
						{
							nAvailableSlots = 0;
							break;
						}
						if (nSlots < nAvailableSlots) 
						{
							nAvailableSlots = nSlots;
							sAccessType = pAv.AccessTypeName;
							sAvailabilityMessage = pAv.Note;
							
						}
					}
				}
				i++;
			}
			if (nAvailableSlots == 999) 
			{
				nAvailableSlots = 0;
			}
			return nAvailableSlots;
		}		

		/// <summary>
		/// Given a selected date,
		/// Calculates StartDay and End Day and returns them in output params.  
		/// nWeeks == number of Weeks to display
		/// nColumnCount is number of days displayed per week.  
        /// If 5 columns, begin on Second Day of Week
		/// If 7 Columns, begin on First Day of Week
        /// (this is a change from the hardcoded behavior for US-based calendars)
		/// 
		/// Returns TRUE if the document's data needs refreshing based on 
		/// this newly selected date.
		/// </summary>
		public bool WeekNeedsRefresh(int nWeeks, DateTime SelectedDate, 
			out DateTime WeekStartDay, out DateTime WeekEndDay)
		{
			DateTime OldStartDay = m_dStartDate;
			DateTime OldEndDay = m_dEndDate;
            // Week start based on thread locale
            int nStartWeekDay = (int)System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            // Current Day
            int nWeekDay = (int) SelectedDate.DayOfWeek; //0 == Sunday

			// this offset gets approrpriate day based on locale.
            int nOff = (nStartWeekDay + 1) % 7;
			TimeSpan ts = new TimeSpan(nWeekDay - nOff,0,0,0); //d,h,m,s

            // if ts is negative, we will jump to the next week in the logic.
            // to show the correct week, add 7. Confusing, I know.
            if (ts < new TimeSpan() ) ts = ts + new TimeSpan(7,0,0,0);

			if (m_nColumnCount == 1) // if one column start and end on the same day.
			{
				ts = new TimeSpan(0,23,59,59);
				WeekStartDay = SelectedDate;
                WeekEndDay = WeekStartDay + ts;
			}
            else if (m_nColumnCount == 5 || m_nColumnCount == 0) // if 5 column start (or default) at the 2nd day of this week and end in 4:23:59:59 days.
            {
                // if picked day is start of week (Sunday in US), start in the next day since that's the first day of work week
                // else, just substract the calculated time span to get to the start of week (first work day)
                WeekStartDay = (nWeekDay == nStartWeekDay) ? SelectedDate + new TimeSpan(1,0,0,0): SelectedDate - ts;
                // End day calculation
                int nEnd = 3;
                ts = new TimeSpan((7 * nWeeks) - nEnd, 23, 59, 59);
                WeekEndDay = WeekStartDay + ts;
            }
            else // if 7 column start at the 1st day of this week and end in 6:23:59:59 days.
            {
                // if picked day is start of week, use that. Otherwise, go to the fist work day and substract one to get to start of week.
                WeekStartDay = (nWeekDay == nStartWeekDay) ? SelectedDate : SelectedDate - ts - new TimeSpan(1,0,0,0);
                // End day calculation
                int nEnd = 1;
                ts = new TimeSpan((7 * nWeeks) - nEnd, 23, 59, 59);
                WeekEndDay = WeekStartDay + ts;
            }

			bool bRet = (( WeekStartDay.Date != OldStartDay.Date) || (WeekEndDay.Date != OldEndDay.Date));
			return bRet;
		}

		/// <summary>
		/// Calls RPMS to create appointment then 
		/// adds appointment to the m_appointments collection
		/// Returns the IEN of the appointment in the RPMS BSDX APPOINTMENT file.
		/// </summary>
		/// <param name="rApptInfo"></param>
		/// <returns></returns>
		public int CreateAppointment(CGAppointment rApptInfo)
		{
			return CreateAppointment(rApptInfo, false);
		}
		
		/// <summary>
		/// Use this overload to create a walkin appointment
		/// </summary>
		/// <param name="rApptInfo"></param>
		/// <param name="bWalkin"></param>
		/// <returns></returns>
		public int CreateAppointment(CGAppointment rApptInfo, bool bWalkin)
		{
			string sStart;
			string sEnd;
			string sPatID;
			string sResource;
			string sNote;
			string sLen;
			string sApptID;

			//sStart = rApptInfo.StartTime.ToString("M-d-yyyy@HH:mm");
			//sEnd = rApptInfo.EndTime.ToString("M-d-yyyy@HH:mm");

            // i18n code -- Use culture neutral FMDates
            sStart = FMDateTime.Create(rApptInfo.StartTime).FMDateString;
            sEnd = FMDateTime.Create(rApptInfo.EndTime).FMDateString;

			TimeSpan sp = rApptInfo.EndTime - rApptInfo.StartTime;
			sLen = sp.TotalMinutes.ToString();
			sPatID = rApptInfo.PatientID.ToString();
			sNote = rApptInfo.Note;
			sResource = rApptInfo.Resource;
			if (bWalkin == true)
			{
				sApptID = "WALKIN";
			}
			else
			{
				sApptID = rApptInfo.AccessTypeID.ToString();
			}

			CGAppointment aCopy = new CGAppointment();
			aCopy.CreateAppointment(rApptInfo.StartTime, rApptInfo.EndTime, sNote, 0, sResource);
			aCopy.PatientID = rApptInfo.PatientID;
			aCopy.PatientName = rApptInfo.PatientName;
			aCopy.HealthRecordNumber = rApptInfo.HealthRecordNumber;
			aCopy.AccessTypeID = rApptInfo.AccessTypeID;

			string sSql = "BSDX ADD NEW APPOINTMENT^" + sStart + "^" + sEnd + "^" + sPatID + "^" + sResource + "^" + sLen + "^" + sNote + "^" + sApptID ;
			System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "NewAppointment");
			int nApptID;

			Debug.Assert(dtAppt.Rows.Count == 1);
			DataRow r = dtAppt.Rows[0];
			nApptID =Convert.ToInt32(r["APPOINTMENTID"]);
			string sErrorID;
			sErrorID = r["ERRORID"].ToString();
			if ((sErrorID != "")||(nApptID < 1))
				throw new Exception(sErrorID);
			aCopy.AppointmentKey = nApptID;
			this.m_appointments.AddAppointment(aCopy);

			bool bRet = RefreshAvailabilitySchedule();

			UpdateAllViews();

			return nApptID;
		}

		public void EditAppointment(CGAppointment pAppt, string sNote)
		{
			try
			{
				int nApptID = pAppt.AppointmentKey;
				string sSql = "BSDX EDIT APPOINTMENT^" + nApptID.ToString() + "^" + sNote;

				System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "EditAppointment");

				Debug.Assert(dtAppt.Rows.Count == 1);
				DataRow r = dtAppt.Rows[0];
				string sErrorID = r["ERRORID"].ToString();
				if (sErrorID == "-1")
					pAppt.Note = sNote;
				
				if (this.m_appointments.AppointmentTable.ContainsKey(nApptID))
				{
					bool bRet = RefreshAvailabilitySchedule();
					UpdateAllViews();
				}
			}
			catch (Exception ex)
			{
				Debug.Write("CGDocument.EditAppointment Failed:  " + ex.Message);
			}
		}

		public void CheckInAppointment(int nApptID, DateTime dCheckIn,
			string ClinicStopIEN,
			string ProviderIEN,
			string PrintRouteSlip,
			string PCCClinicIEN,
			string PCCFormIEN,
			string PCCOutGuide
			)
		{
			string sCheckIn = dCheckIn.ToString("M-d-yyyy@HH:mm");

			string sSql = "BSDX CHECKIN APPOINTMENT^" + nApptID.ToString() + "^" + sCheckIn + "^";
			sSql += ClinicStopIEN + "^" + ProviderIEN + "^" + PrintRouteSlip + "^";
			sSql += PCCClinicIEN + "^" + PCCFormIEN + "^" + PCCOutGuide;

			System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "CheckInAppointment");

			Debug.Assert(dtAppt.Rows.Count == 1);
			DataRow r = dtAppt.Rows[0];
			string sErrorID = r["ERRORID"].ToString();
				
			if (this.m_appointments.AppointmentTable.ContainsKey(nApptID))
			{
				bool bRet = RefreshSchedule();
				UpdateAllViews();
			}
		}

		public string DeleteAppointment(int nApptID)
		{
			return DeleteAppointment(nApptID, true, 0, "");
		}

		public string DeleteAppointment(int nApptID, bool bClinicCancelled, int nReason, string sRemarks)
		{
			//Returns "" if deletion successful
			//Otherwise, returns reason for failure

			string sClinicCancelled = (bClinicCancelled == true)?"C":"PC";
			string sReasonID = nReason.ToString();
			string sSql = "BSDX CANCEL APPOINTMENT^" + nApptID.ToString();
			sSql += "^" + sClinicCancelled;
			sSql += "^" + sReasonID;
			sSql += "^" + sRemarks;
			DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "DeleteAppointment");

			Debug.Assert(dtAppt.Rows.Count == 1);
			DataRow r = dtAppt.Rows[0];
			string sErrorID = r["ERRORID"].ToString();
			if (sErrorID != "")
				return sErrorID;
				
			if (this.m_appointments.AppointmentTable.ContainsKey(nApptID))
			{
				this.m_appointments.RemoveAppointment(nApptID);
				bool bRet = RefreshAvailabilitySchedule();
				UpdateAllViews();
			}
			return "";
		}

		public string AutoRebook(CGAppointment a, int nSearchType, int nMinimumDays, int nMaximumDays, out CGAppointment aRebook)
		{
			//If successful Returns "1" and new start date and time returned in aRebook
			//Otherwise, returns error message
			
			CGAppointment aCopy = new CGAppointment();
			aCopy.CreateAppointment(a.StartTime, a.EndTime, a.Note, 0, a.Resource);
			aCopy.PatientID = a.PatientID;
			aCopy.PatientName = a.PatientName;
			aCopy.HealthRecordNumber = a.HealthRecordNumber;
			aCopy.AccessTypeID = a.AccessTypeID;
			aRebook = aCopy;

			//Determine Rebook access type
			//nSearchType = -1: use current, -2: use any non-zero type, >0 use this access type id
			int nAVType = 0;

			switch (nSearchType)
			{
				case -1:
					nAVType = a.AccessTypeID;
					break;
				case -2:
					nAVType = 0;
					break;
				default:
					nAVType = nSearchType;
					break;
			}

			int nSlots = 0;
			string sSlots = "";
			int nAccessTypeID;  //To compare with nAVType

			DateTime dResult = new DateTime(); //StartTime of access block to autorebook into

			//Next two are empty, but needed to pass to CreateAvailabilitySchedule
			ArrayList alAccessTypes = new ArrayList();
			string sSearchInfo = "";

			//Find the StartTime of first availability block of this type for this clinic
			//between nMinimumDays and nMaximumDays

			string sAVStart = a.StartTime.AddDays(nMinimumDays).ToString("M/d/yyyy@H:mm");

			//dtAVEnd is the last day to search
			DateTime dtAVEnd = a.StartTime.AddDays(nMinimumDays + nMaximumDays);
			string sAVEnd = dtAVEnd.ToString("M/d/yyyy@H:mm");

			//Increment start day to search a week (or so) at a time
			//30 is a test increment.  Need to test different values for performance
			int nIncrement = (nMaximumDays < 30)?nMaximumDays:30;

			//nCount and nCountEnd are the 'moving' counters 
			//that I add to start and end to get the bracket
			//At the beginning of the DO loop, nCount and nCountEnd are already set
			bool bFinished = false;
			bool bFound = false;

			DateTime dStart = a.StartTime.AddDays(nMinimumDays);
            // v 1.3 i18n support - FM Date passed insated of American Date
            string sStart = FMDateTime.Create(dStart).DateOnly.FMDateString;
			DateTime dEnd = dStart.AddDays(nIncrement);
			do
			{	
				string sSql = "BSDX REBOOK NEXT BLOCK^" + sStart + "^" + a.Resource + "^" + nAVType.ToString();
				DataTable dtNextBlock = this.DocManager.RPMSDataTable(sSql, "NextBlock");
				Debug.Assert(dtNextBlock.Rows.Count == 1);
				DataRow drNextBlockRow = dtNextBlock.Rows[0];
				
				object oNextBlock;
				oNextBlock = drNextBlockRow["NEXTBLOCK"];
				if (oNextBlock.GetType() == typeof(System.DBNull))
					break;
				DateTime dNextBlock = (DateTime) drNextBlockRow["NEXTBLOCK"];
				if (dNextBlock > dtAVEnd)
				{
					break;
				}

				dStart = dNextBlock;
				dEnd = dStart.AddDays(nIncrement);
				if (dEnd > dtAVEnd)
					dEnd = dtAVEnd;

				DataTable dtResult = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, this.Resources, dStart, dEnd, alAccessTypes, ScheduleType.Resource, sSearchInfo);
				//Loop thru dtResult looking for a slot having the required availability type.
				//If found, set bFinished = true;	
				foreach (DataRow dr in dtResult.Rows)
				{

					sSlots = dr["SLOTS"].ToString();
					if (sSlots == "")
						sSlots = "0";
					nSlots = Convert.ToInt16(sSlots);
					if (nSlots > 0)
					{
						nAccessTypeID = 0;  //holds the access type id of the availability block
						if (dr["ACCESS_TYPE"].ToString() != "") 
							nAccessTypeID =Convert.ToInt16(dr["ACCESS_TYPE"].ToString());
						if ((nSearchType == -2) && (nAccessTypeID > 0)) //Match on any non-zero type
						{
							bFinished = true;
							bFound = true;
							dResult = (DateTime) dr["START_TIME"];
							break;
						}
						if (nAccessTypeID == nAVType)
						{
							bFinished = true;
							bFound = true;
							dResult = (DateTime) dr["START_TIME"];
							break;
						}
					}
				}
				dStart = dEnd.AddDays(1);
				dEnd = dStart.AddDays(nIncrement);
				if (dEnd > dtAVEnd)
					dEnd = dtAVEnd;
			} while (bFinished == false);

			if (bFound == true)
			{
				aCopy.StartTime = dResult;
				aCopy.EndTime = dResult.AddMinutes(a.Duration);
				//Create the appointment
				//Set the AUTOREBOOKED flag 
				//and store the "AutoRebooked To DateTime" 
				//in each autorebooked appointment
				this.CreateAppointment(aCopy);
				SetAutoRebook(a, dResult);
				return "1";
			}
			else
			{
				return "0";
			}
		}

		private void SetAutoRebook(CGAppointment a, DateTime dtRebookedTo)
		{
			string sApptKey = a.AppointmentKey.ToString();
			//string sRebookedTo = dtRebookedTo.ToString("M/d/yyyy@HH:mm");
            // i18n
            string sRebookedTo = FMDateTime.Create(dtRebookedTo).FMDateString;
			string sSql = "BSDX REBOOK SET^" + sApptKey + "^" + sRebookedTo;
			System.Data.DataTable dtRebook = m_DocManager.RPMSDataTable(sSql, "AutoRebook");

		}

		public string AppointmentNoShow(int nApptID, bool bNoShow)
		{
			/*
			 * BSDX NOSHOW RPC Returns 1  in ERRORID if  successfully sets NOSHOW flag in BSDX APPOINTMENT and, if applicable, File 2
			 *Otherwise, returns 0 for failure and errormessage in ERRORTXT
			 *THIS routine returns "" if success or the message in ERRORTEXT if failed
			 *Exceptions should be caught by caller
			 *
			 */
			
			string sTest = bNoShow.ToString();
			string sNoShow = (bNoShow == true)?"1":"0";
			string sSql = "BSDX NOSHOW^" + nApptID.ToString();
			sSql += "^";
			sSql += sNoShow;

			DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "AppointmentNoShow");

			Debug.Assert(dtAppt.Rows.Count == 1);
			DataRow r = dtAppt.Rows[0];
			string sErrorID = r["ERRORID"].ToString();
			if (sErrorID != "1")
			{
				return r["ERRORTEXT"].ToString();
			}

			bool bRet = RefreshSchedule();
			
			return sErrorID;
		}

		#endregion Methods

	}//End class
}//End namespace
