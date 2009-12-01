using System;
using System.Collections;
using System.Data;
//using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	/// <summary>
	/// Contains array of availability blocks for a scheduling resource
	/// </summary>
	public class CGAVDocument : System.Object
	{
		public CGAVDocument()
		{
			m_AVBlocks = new CGAppointments();
			m_sResourcesArray = new ArrayList();
		}

		#region Member Variables

		public int				m_nColumnCount; //todo: this should point to the view's member for column count
		public int				m_nTimeUnits;
		public string			m_sSecondary;
		private string			m_sDocName;
		public ArrayList		m_sResourcesArray;
		public ScheduleType		m_ScheduleType;
		private DateTime		m_dSelectedDate; //Holds the user's selection from the dtpicker
		private DateTime		m_dStartDate; //Beginning date of document data
		private DateTime		m_dEndDate; //Ending date of document data
		public CGAppointments	m_AVBlocks;
		private CGDocumentManager m_DocManager;
		private int				m_nDocID;	//Resource IEN in ^BSDXRES

		#endregion

		#region Properties

		/// <summary>
		/// Resource IEN in ^BSDXRES
		/// </summary>
		public int ResourceID
		{
			get
			{
				return m_nDocID;
			}
			set
			{
				m_nDocID = value;
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
		/// Contains the hashtable of Availability Blocks
		/// </summary>
		public CGAppointments AVBlocks
		{
			get
			{
				return m_AVBlocks;
			}
			set
			{
				m_AVBlocks = value;
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
				if (m_ScheduleType == ScheduleType.Resource)
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

				bRet = this.RefreshDaysSchedule();
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

		public void ChangeAppointmentTime(CGAppointment pAppt, DateTime dNewStart, DateTime dNewEnd, string sResource)
		{
			try 
			{
				DateTime dOldStart = pAppt.StartTime;
				DateTime dOldEnd = pAppt.EndTime;
				if ((dOldStart == dNewStart) && (dOldEnd == dNewEnd)) 
				{ //no change in time
					return;
				}

				foreach (CGAppointment a in m_AVBlocks.AppointmentTable.Values)
				{
					DateTime sStart2 = a.StartTime;
					DateTime sEnd2 = a.EndTime;
					if ((a.AppointmentKey != pAppt.AppointmentKey) && (CGSchedLib.TimesOverlap(dNewStart, dNewEnd, a.StartTime, a.EndTime)))
					{
						MessageBox.Show("Access blocks may not overlap.","Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}

				this.DeleteAvailability(pAppt.AppointmentKey);
				pAppt.StartTime = dNewStart;
				pAppt.EndTime = dNewEnd;
				pAppt.Resource = sResource;
				this.CreateAppointment(pAppt);
				
			}
			catch(Exception e)
			{
				MessageBox.Show("CGDocument::ChangeAppointmentTime failed: " + e.Message);
				return;
			}
		}

		public void DeleteAvailability(int nApptID)
		{
			if (this.m_AVBlocks.AppointmentTable.ContainsKey(nApptID))
			{
				string sSql = "BSDX CANCEL AVAILABILITY^" + nApptID.ToString();
				DataTable dtAppt =m_DocManager.RPMSDataTable(sSql, "DeleteAvailability");
				int nErrorID;				
				Debug.Assert(dtAppt.Rows.Count == 1);
				DataRow r = dtAppt.Rows[0];
				nErrorID = Convert.ToInt32(r["ERRORID"]);
				this.m_AVBlocks.RemoveAppointment(nApptID);
				UpdateAllViews();
			}		
		}

		/// <summary>
		/// Called by LoadTemplate to create Access Block
		/// Returns the IEN of the availability block in the RPMS BSDX AVAILABILITY file.
		/// </summary>
		public int CreateAppointmentAuto(CGAppointment rApptInfo)
		{
			try
			{
				string sStart;
				string sEnd;
				string sResource;
				string sNote;
				string sTypeID;
				string sSlots;

				sStart = rApptInfo.StartTime.ToString("M-d-yyyy@HH:mm");
				sEnd = rApptInfo.EndTime.ToString("M-d-yyyy@HH:mm");
				sNote = rApptInfo.Note;
				sResource = rApptInfo.Resource;
				sTypeID = rApptInfo.AccessTypeID.ToString();
				sSlots = rApptInfo.Slots.ToString();

				CGAppointment aCopy = new CGAppointment();
				aCopy.CreateAppointment(rApptInfo.StartTime, rApptInfo.EndTime, sNote, 0, sResource);
				aCopy.AccessTypeID = rApptInfo.AccessTypeID;
				aCopy.Slots = rApptInfo.Slots;
				aCopy.IsAccessBlock = true;

				string sSql = "BSDX ADD NEW AVAILABILITY^" + sStart + "^" + sEnd + "^" + sTypeID + "^" + sResource + "^" +  sSlots + "^" + sNote;
				DataTable dtAppt =m_DocManager.RPMSDataTable(sSql, "NewAvailability");

				int nApptID;
				int nErrorID;

				Debug.Assert(dtAppt.Rows.Count == 1);
				DataRow r = dtAppt.Rows[0];
				nApptID =Convert.ToInt32(r["AVAILABILITYID"]);
				nErrorID = Convert.ToInt32(r["ERRORID"]);
				if (nErrorID != -1)
				{
					throw new Exception("RPMS Error");
				}
				Debug.Write("CreateAvailabilityAuto -- new AV block created\n");

				UpdateAllViews();

				aCopy.AppointmentKey = nApptID;
				return nApptID;
			}
			catch (Exception ex)
			{
				Debug.Write("CGAVDocument.CreateAppointmentAuto Failed: " + ex.Message);
				return -1;
			}		
		}


		public int CreateAppointment(CGAppointment rApptInfo)
		{
			try
			{
				string sStart;
				string sEnd;
				string sResource;
				string sNote;
				string sTypeID;
				string sSlots;

				sStart = rApptInfo.StartTime.ToString("M-d-yyyy@HH:mm");
				sEnd = rApptInfo.EndTime.ToString("M-d-yyyy@HH:mm");
				sNote = rApptInfo.Note;
				sResource = rApptInfo.Resource;
				sTypeID = rApptInfo.AccessTypeID.ToString();
				sSlots = rApptInfo.Slots.ToString();

				CGAppointment aCopy = new CGAppointment();
				aCopy.CreateAppointment(rApptInfo.StartTime, rApptInfo.EndTime, sNote, 0, sResource);
				aCopy.AccessTypeID = rApptInfo.AccessTypeID;
				aCopy.Slots = rApptInfo.Slots;
				aCopy.IsAccessBlock = true;

				aCopy.AccessTypeName = this.AccessNameFromID(aCopy.AccessTypeID);

				foreach (CGAppointment a in this.m_AVBlocks.AppointmentTable.Values)
				{
					DateTime sStart2 = a.StartTime;
					DateTime sEnd2 = a.EndTime;
					if (CGSchedLib.TimesOverlap(aCopy.StartTime, aCopy.EndTime, a.StartTime, a.EndTime))
					{
						//						throw new Exception("Access blocks may not overlap.");
						MessageBox.Show("Access blocks may not overlap.","Clinical Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return -1;
					}
						
				}

				string sSql = "BSDX ADD NEW AVAILABILITY^" + sStart + "^" + sEnd + "^" + sTypeID + "^" + sResource + "^" +  sSlots + "^" + sNote;
				DataTable dtAppt =m_DocManager.RPMSDataTable(sSql, "NewAvailability");

				int nApptID;
				int nErrorID;

				Debug.Assert(dtAppt.Rows.Count == 1);
				DataRow r = dtAppt.Rows[0];
				nApptID =Convert.ToInt32(r["AVAILABILITYID"]);
				nErrorID = Convert.ToInt32(r["ERRORID"]);
				if (nErrorID != -1)
				{
					throw new Exception("RPMS Error");
				}
				Debug.Write("CreateAvailability -- new AV block created\n");

				aCopy.AppointmentKey = nApptID;
			
				this.m_AVBlocks.AddAppointment(aCopy);

				UpdateAllViews();

				return nApptID;
			}
			catch (Exception ex)
			{
				Debug.Write("CGAVDocument.CreateAppointment Failed: " + ex.Message);
				return -1;
			}
		}

		private int GetTotalMinutes(DateTime dDate)
		{
			return ((dDate.Hour * 60) + dDate.Minute);
		}

		private string AccessNameFromID(int nAccessTypeID)
		{
			DataView dvTypes = new DataView(this.DocManager.GlobalDataSet.Tables["AccessTypes"]);
			dvTypes.Sort = "BMXIEN ASC";
			int nRow = dvTypes.Find(nAccessTypeID);
			DataRowView drv = dvTypes[nRow];
			string sAccessTypeName = drv["ACCESS_TYPE_NAME"].ToString();
			return sAccessTypeName;		
		}


		/// <summary>
		/// Update availability block schedule based on info in RPMS
		/// </summary>
		public bool RefreshDaysSchedule()
		{
			DateTime dStart;
			DateTime dEnd;
			int nKeyID;
			string sNote;
			string sResource;
			string sAccessType;
			string sSlots;
			CGAppointment	pAppointment;
			CGDocumentManager pApp = CGDocumentManager.Current;
			DataTable rAppointmentSchedule;

			this.m_AVBlocks.ClearAllAppointments();

			ArrayList apptTypeIDs = new ArrayList();

			rAppointmentSchedule = CGSchedLib.CreateAssignedSlotSchedule(m_DocManager, (string) m_sResourcesArray[0], this.m_dStartDate, this.m_dEndDate, apptTypeIDs,/* */ this.m_ScheduleType, "0");
			CGSchedLib.OutputArray(rAppointmentSchedule, "rAppointmentSchedule");
			foreach (DataRow r in rAppointmentSchedule.Rows) 
			{
				nKeyID = Convert.ToInt32(r["AVAILABILITYID"].ToString());
				if (nKeyID > 0) 
				{
					dStart = (DateTime) r["START_TIME"];
					dEnd = (DateTime) r["END_TIME"];
					sNote = r["NOTE"].ToString();
					sResource = r["RESOURCE"].ToString();
					sAccessType = r["ACCESS_TYPE"].ToString();
					sSlots = r["SLOTS"].ToString();

					pAppointment = new CGAppointment();
					pAppointment.CreateAppointment(dStart, dEnd, sNote, nKeyID, sResource);
					pAppointment.AccessTypeID = Convert.ToInt16(sAccessType);
					pAppointment.Slots = Convert.ToInt16(sSlots);
					pAppointment.IsAccessBlock = true;
					pAppointment.AccessTypeName = this.AccessNameFromID(pAppointment.AccessTypeID);

					this.m_AVBlocks.AddAppointment(pAppointment);
				}
			}		
			return true;
		}

		/// <summary>
		/// Given a selected date,
		/// Calculates StartDay and End Day and returns them in output params.  
		/// nWeeks == number of Weeks to display
		/// nColumnCount is number of days displayed per week.  If 5 columns, begin on
		/// Monday, if 7 Columns, begin on Sunday
		/// 
		/// Returns TRUE if the document's data needs refreshing based on 
		/// this newly selected date.
		/// </summary>
		public bool WeekNeedsRefresh(int nWeeks, DateTime SelectedDate, 
			out DateTime WeekStartDay, out DateTime WeekEndDay)
		{
			DateTime OldStartDay = m_dStartDate;
			DateTime OldEndDay = m_dEndDate;
			int nWeekDay = (int) SelectedDate.DayOfWeek; //0 == Sunday

			int nOff = 1;
			TimeSpan ts = new TimeSpan(nWeekDay - nOff,0,0,0); //d,h,m,s

			if (m_nColumnCount == 1) 
			{
				ts = new TimeSpan(0,23,59,59);
				WeekStartDay = SelectedDate;
			}
			else 
			{
				WeekStartDay = SelectedDate - ts;
				if (m_nColumnCount == 7)
				{
					ts = new TimeSpan(1,0,0,0);
					WeekStartDay -= ts;
				}
				int nEnd = (m_nColumnCount == 7) ? 1 : 3;
				ts = new TimeSpan((7* nWeeks) - nEnd, 23, 59,59);
			}
			WeekEndDay = WeekStartDay + ts;
			bool bRet = (( WeekStartDay.Date != OldStartDay.Date) || (WeekEndDay.Date != OldEndDay.Date));
			return bRet;
		}

		public void OnOpenDocument() 
		{
			//Create new Document
			//			DateTime dStart;
			//			DateTime dEnd;

			Debug.Assert(m_sResourcesArray.Count > 0);

			m_sSecondary = "";

			m_ScheduleType = (m_sResourcesArray.Count == 1) ? ScheduleType.Resource: ScheduleType.Clinic;
		
			bool bRet = false;
			//Set initial From and To dates based on current day
			//			DateTime dDate = new DateTime(2001,12,05); //test line
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

			bRet = this.RefreshDaysSchedule();

			CGAVView view = null;
			//If this document already has a view, the use it
			Hashtable h = CGDocumentManager.Current.AvailabilityViews;		
			CGAVDocument d;
			bool bReuseView = false;
			foreach (CGAVView v in h.Keys)
			{
				d = (CGAVDocument) h[v];
				if (d == this)
				{
					view = v;
					bReuseView = true;
					break;
				}
			}

			//Otherwise, create new View
			if (bReuseView == false)
			{
				view = new CGAVView();

				view.DocManager = this.DocManager;
				view.StartDate = m_dStartDate;
			
				//Assign the document to the view
				view.Document = this;

				//Link the calendargrid's appointments table to the document's table
				view.AVBlocks = this.AVBlocks;
			
				view.Text = "Edit Availability - " + this.DocName;
				view.Show();
			}

			this.UpdateAllViews();

		}

		public void AddResource(string sResource)
		{
			//TODO:  Test that resource is not currently in list, that it IS a resource, etc
			this.m_sResourcesArray.Add(sResource);
			this.UpdateAllViews();
		}

		/// <summary>
		/// Calls each AVview associated with this AVdocument and tells it to update itself
		/// </summary>
		public void UpdateAllViews()
		{
			//iterate through all views and call update.
			Hashtable h = CGDocumentManager.Current.AvailabilityViews;
			
			CGAVDocument d;
			foreach (CGAVView v in h.Keys)
			{
				d = (CGAVDocument) h[v];
				if (d == this)
				{
					v.UpdateArrays();
				}
			}

		}

		#endregion
	
	}//End class
}
