using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using IndianHealthService.BMXNet;
using System.Linq;
using System.Collections.Generic;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Contains the array of appointments and availabily that make up the document class
    /// </summary>
    public class CGDocument
    {
        #region Member Variables
        public int m_nColumnCount;                 //todo: this should point to the view's member for column count
        public int m_nTimeUnits;                   //?
        private string m_sDocName;                     //Document Name ?
        public ArrayList m_sResourcesArray;              //keeps the resources
        public ScheduleType m_ScheduleType;                 //Either a Resource or a Clinic (Group of Resources)
        private DateTime m_dSelectedDate;                //Holds the user's selection from the dtpicker
        private DateTime m_dStartDate;                   //Beginning date of document data
        private DateTime m_dEndDate;                     //Ending date of document data
        public CGAppointments m_appointments;                 //Appointment List
        private ArrayList m_pAvArray;                     //Availability List
        private CGDocumentManager m_DocManager;                 //Holds a reference to the document manager
        private DateTime m_dLastRefresh = DateTime.Now;  //Holds last refersh date
        #endregion

        /// <summary>
        /// Constructor. Initialize State Data:
        /// 3 Arrays (Appointments, Availabilities, and Resources)
        /// Schedule Type is Resource not Clinic
        /// Selected Date is Today
        /// Start Date is Today
        /// End Date is 7 days from Today
        /// </summary>
        public CGDocument()
        {
            m_appointments = new CGAppointments();      // Holds Appointments
            m_pAvArray = new ArrayList();               // Holds Availabilites
            m_sResourcesArray = new ArrayList();        // Holds Resources
            m_ScheduleType = ScheduleType.Resource;     // Default Schedule Type is a Resource
            m_dSelectedDate = DateTime.Today;           // Default Selected Date is Today
            m_dStartDate = DateTime.Today;              // Default Start Date is Today
            m_dEndDate = DateTime.Today.AddDays(7);     // Default End Date is 7 days from Today.
        }


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
                d = (CGDocument)h[v];
                if (d == this)
                {
                    v.UpdateArrays();
                }
            }

        }

        /// <summary>
        /// Update schedule based on info in RPMS
        /// <returns>Clears and repopluates m_appointments</returns>
        /// </summary>
        private bool RefreshAppointments()
        {
            try
            {
                string sPatientName;
                string sPatientID;
                DateTime dStart;
                DateTime dEnd;
                DateTime dCheckIn;
                DateTime dAuxTime;
                int nKeyID;
                string sNote;
                string sResource;
                bool bNoShow = false;
                string sNoShow = "0";
                string sHRN = "";
                int nAccessTypeID; //used in autorebook
                string sWalkIn = "0";
                bool bWalkIn;
                CGAppointment pAppointment;
                CGDocumentManager pApp = CGDocumentManager.Current;
                DataTable rAppointmentSchedule;

                //Nice to know that it gets set here!!!
                m_dLastRefresh = DateTime.Now;

                //Clear appointments associated with this document
                this.m_appointments.ClearAllAppointments();

                //  calls RPC to get appointments
                rAppointmentSchedule = CGSchedLib.CreateAppointmentSchedule(m_DocManager, m_sResourcesArray, this.m_dStartDate, this.m_dEndDate);

                // loop through datatable: Create CGAppointment and add to CGAppointments
                foreach (DataRow r in rAppointmentSchedule.Rows)
                {

                    if (r["APPOINTMENTID"].ToString() == "0")
                    {
                        string sMsg = r["NOTE"].ToString();
                        throw new BMXNetException(sMsg);
                    }
                    nKeyID = Convert.ToInt32(r["APPOINTMENTID"].ToString());
                    sResource = r["RESOURCENAME"].ToString();
                    sPatientName = r["PATIENTNAME"].ToString();
                    sPatientID = r["PATIENTID"].ToString();
                    dStart = (DateTime)r["START_TIME"];
                    dEnd = (DateTime)r["END_TIME"];
                    dCheckIn = new DateTime();
                    dAuxTime = new DateTime();

                    if (r["CHECKIN"].GetType() != typeof(System.DBNull))
                        dCheckIn = (DateTime)r["CHECKIN"];
                    if (r["AUXTIME"].GetType() != typeof(System.DBNull))
                        dCheckIn = (DateTime)r["AUXTIME"];
                    sNote = r["NOTE"].ToString();
                    sNoShow = r["NOSHOW"].ToString();
                    bNoShow = (sNoShow == "1") ? true : false;
                    sHRN = r["HRN"].ToString();
                    nAccessTypeID = (int)r["ACCESSTYPEID"];
                    sWalkIn = r["WALKIN"].ToString();
                    bWalkIn = (sWalkIn == "1") ? true : false;
                    int? RadiologyExamIEN = r["RADIOLOGY_EXAM"] as Int32?; //new in v 1.6 - Get Radiology Exam

                    Patient pt = new Patient()
                    {
                        DFN = Convert.ToInt32(sPatientID),
                        Name = sPatientName,
                        DOB = (DateTime)r["DOB"],
                        ID = r["PID"].ToString(), 
                        HRN = sHRN,
                        Sex = r["SEX"].ToString() == "MALE" ? Sex.Male : Sex.Female
                    };

                    pAppointment = new CGAppointment();
                    pAppointment.Patient = pt;
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
                    pAppointment.RadiologyExamIEN = RadiologyExamIEN;
                    this.m_appointments.AddAppointment(pAppointment);

                }

                return true;
            }
            catch (Exception Ex)
            {
                Debug.Write("CGDocument.RefreshDaysSchedule error: " + Ex.Message + "\n");
                return false;
            }
        }


        public bool IsRefreshNeeded()
        {
            if (m_sResourcesArray.Count == 0) return false;
            return this.WeekNeedsRefresh(1, m_dSelectedDate, out this.m_dStartDate, out this.m_dEndDate);
        }

        //sam: This is a test that duplicates RefreshDocument, but without the UpdateAllViews,
        // as that has to be done synchornously.
        //XXX: Needs to be refactored obviously, but now for testing.
        //XXX: Tested extensively enough. Less refactoring now. 2011-01-26
        public void RefreshDocumentAsync()
        {
            Debug.WriteLine("IN REFERSH DOCUMENT ASYNC\n\n");

            bool bRet = false;
            if (m_sResourcesArray.Count == 0)
                return;
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

        
        public void RefreshDocument()
        {
            bool bRet = false;
            if (m_sResourcesArray.Count == 0)
                return;
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
            
            this.UpdateAllViews();
        }

        public void OnOpenDocument(DateTime dDate)
        {
            try
            {
                this.SelectedDate = dDate.Date;

                m_ScheduleType = (m_sResourcesArray.Count == 1) ? ScheduleType.Resource : ScheduleType.Clinic;
                bool bRet = false;
                
                if (m_ScheduleType == ScheduleType.Resource)
                {
                    bRet = this.WeekNeedsRefresh(1, dDate, out this.m_dStartDate, out this.m_dEndDate);
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
                //SAM: Why do this again???
                //SAM: I think it's not needed; b/c 
                Hashtable h = CGDocumentManager.Current.Views;
                CGDocument d;
                bool bReuseView = false;
                foreach (CGView v in h.Keys)
                {
                    d = (CGDocument)h[v];
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

        /// <summary>
        /// Refreshes Availablility and Schedules from RPMS.
        /// </summary>
        /// <returns>Success or Failure. Should be always Success.</returns>
        public bool RefreshSchedule()
        {
            this.RefreshAvailabilitySchedule();
            this.RefreshAppointments();
            return true;
        }

        private bool RefreshAvailabilitySchedule()
        {
            try
            {
                ArrayList saryApptTypes = new ArrayList();
                
                //Refresh Availability schedules
                DataTable rAvailabilitySchedule;
                rAvailabilitySchedule = CGSchedLib.CreateAvailabilitySchedule(m_DocManager, m_sResourcesArray, this.m_dStartDate, this.m_dEndDate, saryApptTypes,/**/ m_ScheduleType, "0");

                ////NEW
                //NOTE: This lock makes sure that availabilities aren't queried for slots when the array is an intermediate
                //state. The other place that has this lock is SlotsAvailable function.
                lock (this.m_pAvArray)
                {
                    m_pAvArray.Clear();
                    foreach (DataRow rTemp in rAvailabilitySchedule.Rows)
                    {
                        DateTime dStart = (DateTime)rTemp["START_TIME"];
                        DateTime dEnd = (DateTime)rTemp["END_TIME"];

                        //TODO: Fix this slots datatype problem
                        string sSlots = rTemp["SLOTS"].ToString();
                        int nSlots = Convert.ToInt16(sSlots);

                        string sResourceList = rTemp["RESOURCE"].ToString();
                        string sAccessRuleList = rTemp["ACCESS_TYPE"].ToString();
                        string sNote = rTemp["NOTE"].ToString();

                        int nApptTypeID;

                        if ((nSlots < -1000) || (sAccessRuleList == ""))
                        {
                            nApptTypeID = 0;
                        }
                        else
                        {
                            nApptTypeID = Int32.Parse(rTemp["ACCESS_TYPE"].ToString());
                        }

                        AddAvailability(dStart, dEnd, nApptTypeID, nSlots, sResourceList, sAccessRuleList, sNote);
                    }
                }
                return true;
                
                /* NOT USED
                //Refresh Type Schedule
                string sResourceName = "";
                DataTable rTypeSchedule = new DataTable(); ;
                for (int j = 0; j < m_sResourcesArray.Count; j++)
                {
                    sResourceName = m_sResourcesArray[j].ToString();
                    DataTable dtTemp = CGSchedLib.CreateAssignedTypeSchedule(m_DocManager, sResourceName, this.m_dStartDate, this.m_dEndDate, m_ScheduleType);

                    if (j == 0)
                    {
                        rTypeSchedule = dtTemp;
                    }
                    else
                    {
                        rTypeSchedule = CGSchedLib.UnionBlocks(rTypeSchedule, dtTemp);
                    }
                }

                DateTime dStart;
                DateTime dEnd;
                DateTime dTypeStart;
                DateTime dTypeEnd;
                int nSlots;
                Rectangle crRectA = new Rectangle(0, 0, 1, 0);
                Rectangle crRectB = new Rectangle(0, 0, 1, 0);
                bool bIsect;
                string sResourceList;
                string sAccessRuleList;

                //smh: moved clear availabilities down here.
                //smh: Temporary solution to make sure that people don't touch the availability table at the same time!!!
                //NOTE: This lock makes sure that availabilities aren't queried for slots when the array is an intermediate
                //state. The other place that has this lock is SlotsAvailable function.
                lock (this.m_pAvArray)
                {
                    m_pAvArray.Clear();

                    foreach (DataRow rTemp in rAvailabilitySchedule.Rows)
                    {
                        //get StartTime, EndTime and Slots 
                        dStart = (DateTime)rTemp["START_TIME"];
                        dEnd = (DateTime)rTemp["END_TIME"];

                        //TODO: Fix this slots datatype problem
                        string sSlots = rTemp["SLOTS"].ToString();
                        nSlots = Convert.ToInt16(sSlots);

                        sResourceList = rTemp["RESOURCE"].ToString();
                        sAccessRuleList = rTemp["ACCESS_TYPE"].ToString();

                        string sNote = rTemp["NOTE"].ToString();

                        if ((nSlots < -1000) || (sAccessRuleList == ""))
                        {
                            nApptTypeID = 0;
                        }
                        else
                        {
                            foreach (DataRow rType in rTypeSchedule.Rows)
                            {

                                dTypeStart = (DateTime)rType["StartTime"];
                                dTypeEnd = (DateTime)rType["EndTime"];
                                //if start & end times overlap, then
                                string sTypeResource = rType["ResourceName"].ToString();
                                if ((dTypeStart.DayOfYear == dStart.DayOfYear) && (sResourceList == sTypeResource))
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


                        //AddAvailability(dStart, dEnd, nApptTypeID, nSlots, sResourceList, sAccessRuleList, sNote);
                    }//end foreach datarow rTemp
                }//end lock
                return true;
             */
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

        /// <summary>
        /// Adds Availability to Availability Array held by document
        /// </summary>
        /// <param name="StartTime">Self-Explan</param>
        /// <param name="EndTime">Self-Explan</param>
        /// <param name="nType"></param>
        /// <param name="nSlots"></param>
        /// <param name="UpdateView"></param>
        /// <param name="sResourceList"></param>
        /// <param name="sAccessRuleList"></param>
        /// <param name="sNote"></param>
        /// <returns></returns>
        public int AddAvailability(DateTime StartTime, DateTime EndTime, int nType, int nSlots, string sResourceList, string sAccessRuleList, string sNote)
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
                sTemp = (sTemp == "") ? "0" : sTemp;
                int nRed = Convert.ToInt16(sTemp);
                pNewAv.Red = nRed;
                sTemp = dRow["GREEN"].ToString();
                sTemp = (sTemp == "") ? "0" : sTemp;
                int nGreen = Convert.ToInt16(sTemp);
                pNewAv.Green = nGreen;
                sTemp = dRow["BLUE"].ToString();
                sTemp = (sTemp == "") ? "0" : sTemp;
                int nBlue = Convert.ToInt16(sTemp);
                pNewAv.Blue = nBlue;

                string sName = dRow["ACCESS_TYPE_NAME"].ToString();
                pNewAv.AccessTypeName = sName;
            }

            int nIndex = 0;
            nIndex = m_pAvArray.Add(pNewAv);

            return nIndex;
        }


        public void AddResource(string sResource)
        {
            //TODO:  Test that resource is not currently in list, that it IS a resource, etc
            this.m_sResourcesArray.Add(sResource);
        }

        /// <summary>
        /// Gets number of slots left in a certain selection on the grid. Used in Multiple Places.
        /// </summary>
        /// <param name="dSelStart">Selection Start Date</param>
        /// <param name="dSelEnd">Selection End Date</param>
        /// <param name="sResource">Resource Name</param>
        /// <param name="sAccessType">Out: Name of Access Type</param>
        /// <param name="sAvailabilityMessage">Out: Access Note</param>
        /// <returns>Number of slots</returns>
        public int SlotsAvailable(DateTime dSelStart, DateTime dSelEnd, string sResource, int viewTimeScale, out CGAvailability resultantAV)
        {

            resultantAV = null;

            double slotsAvailable = 0;      //defalut return value

            double effectiveSlotsAvailable = 0;    //Slots available based on the time scale.


            //NOTE: What's this lock? This lock makes sure that nobody is editing the availability array
            //when we are looking at it. Since the availability array could potentially be updated on
            //a different thread, we are can be potentially left stuck with an empty array.
            //
            //The other place that uses this lock is the RefershAvailabilitySchedule method
            //
            //This is a temporary fix until I figure out how to divorce the availbilities here from those drawn
            //on the calendar. Appointments are cloned b/c they are in an object that supports that; and b/c I
            //don't need to suddenly query them at runtime like I do with Availabilities.

            //Let's Try Linq
            lock (this.m_pAvArray)
            {
                //This foreach loop looks for an availability that overlaps where the user clicked.
                //Availabilites cannot overlap each other (enforced at the DB level)
                //If selection hits multiple blocks, get the block with the most slots (reflected by the sorting here)
                List<CGAvailability> lstAV = (from avail in this.m_pAvArray.Cast<CGAvailability>()
                           where (sResource == avail.ResourceList && CalendarGrid.TimesOverlap(dSelStart, dSelEnd, avail.StartTime, avail.EndTime))
                           select avail).ToList();

                //if we don't have any availabilities, then return with zero slots.
                if (lstAV.Count == 0) return 0;
                
                CGAvailability pAV;

                //if there is just one, that's it.
                if (lstAV.Count == 1) pAV = lstAV.First();
                //otherwise...
                else
                {
                    //if availabilities are contigous to each other, need to join them together.

                    //First, are they the same?
                    string firsttype = lstAV.First().AccessTypeName;
                    bool bAllSameType = lstAV.All(av => av.AccessTypeName == firsttype);

                    //Second are they ALL contigous?
                    DateTime lastEndTime = DateTime.Today; //bogus value to please compiler who wants it assigned.
                    int index = 0;

                    bool bContigous = lstAV.OrderBy(av => av.StartTime)
                       .All(av =>
                       {
                           index++;
                           if (index == 1)
                           {
                               lastEndTime = av.EndTime;
                               return true;
                           }
                           if (av.StartTime == lastEndTime)
                           {
                               lastEndTime = av.EndTime;
                               return true;
                           }

                           return false;
                       });



                    if (bContigous && bAllSameType)
                    {
                        var enumAVOrdered = lstAV.OrderBy(av => av.StartTime);

                        pAV = new CGAvailability
                        {
                            StartTime = enumAVOrdered.First().StartTime,
                            EndTime = enumAVOrdered.Last().EndTime,
                            Slots = enumAVOrdered.Sum(av => av.Slots),
                            AccessTypeName = enumAVOrdered.First().AccessTypeName,
                            Note = enumAVOrdered.First().Note
                        };
                    }
                    else
                    {
                        pAV = lstAV.OrderByDescending(av => av.Slots).First();
                    }
                }

                resultantAV = pAV; // out var
                
                slotsAvailable = pAV.Slots;
                
                //Subtract total slots current appointments take up.
                slotsAvailable -= (from appt in this.Appointments.AppointmentTable.Values.Cast<CGAppointment>()
                                   //If the resource is the same and the user selection overlaps, then...
                                   where (sResource == appt.Resource && CalendarGrid.TimesOverlap(pAV.StartTime, pAV.EndTime, appt.StartTime, appt.EndTime))
                                   // if appt starttime is before avail start time, only count against the avail starting from the availability start time
                                   let startTimeToCountAgainstBlock = appt.StartTime < pAV.StartTime ? pAV.StartTime : appt.StartTime
                                   // if appt endtime is after the avail ends, only count against the avail up to where the avail ends
                                   let endTimeToCountAgainstBlock = appt.EndTime > pAV.EndTime ? pAV.EndTime : appt.EndTime
                                   // theoretical minutes per slot for the availability
                                   let minPerSlot = (pAV.EndTime - pAV.StartTime).TotalMinutes / pAV.Slots
                                   // how many minutes does this appointment take away from the slot
                                   let minPerAppt = (endTimeToCountAgainstBlock - startTimeToCountAgainstBlock).TotalMinutes
                                   // how many slots the appointment takes up using this availability's scale
                                   let slotsConsumed = minPerAppt / minPerSlot
                                   select slotsConsumed)
                                   // add up SlotsConsumed to substract from slotsAvailable
                                   .Sum();

                //theoretical minutes per slot for the availability
                double minPerSlot2 = (pAV.EndTime - pAV.StartTime).TotalMinutes / pAV.Slots;
                
                //Convert it to the view's time scale.
                effectiveSlotsAvailable = (minPerSlot2 / viewTimeScale) * slotsAvailable;

            }
            
            //round it down.
            return (int)effectiveSlotsAvailable;

            /* OLD ALGOTHRIM 2

            lock (this.m_pAvArray)
            {
                //This foreach loop looks for an availability that overlaps where the user clicked.
                //There can only be one, as availabilites cannot overlap each other (enforced at the DB level)
                //Therefore, we loop, and once we find it, we break.
                foreach (CGAvailability pAV in this.m_pAvArray)
                {
                    //If the resource is the same and the user selection overlaps, then...
                    if (sResource == pAV.ResourceList && CalendarGrid.TimesOverlap(dSelStart, dSelEnd, pAV.StartTime, pAV.EndTime))
                    {
                        slotsAvailable = pAV.Slots;         //variable now holds the total number of slots
                        sAccessType = pAV.AccessTypeName;   //Access Name
                        sAvailabilityMessage = pAV.Note;    //Access Block Note
                        
                        //Here we substract each appointment weight in slots from slotsAvailable
                        foreach (DictionaryEntry apptDict in this.m_appointments.AppointmentTable)
                        {
                            CGAppointment appt = (CGAppointment)apptDict.Value;
                            //If the appointment is in the same resource and overlaps with this availablity
                            if (sResource == appt.Resource && CalendarGrid.TimesOverlap(pAV.StartTime, pAV.EndTime, appt.StartTime, appt.EndTime))
                            {
                                // if appt starttime is before avail start time, only count against the avail starting from the availability start time
                                DateTime startTimeToCountAgainstBlock = appt.StartTime < pAV.StartTime ? pAV.StartTime : appt.StartTime;
                                // if appt endtime is after the avail ends, only count against the avail up to where the avail ends
                                DateTime endTimeToCountAgainstBlock = appt.EndTime > pAV.EndTime ? pAV.EndTime : appt.EndTime;
                                // theoretical minutes per slot for the availability
                                double minPerSlot = (pAV.EndTime - pAV.StartTime).TotalMinutes/pAV.Slots;
                                // how many minutes does this appointment take away from the slot
                                double minPerAppt = (endTimeToCountAgainstBlock - startTimeToCountAgainstBlock).TotalMinutes;
                                // how many slots the appointment takes up using this availability's scale
                                double slotsConsumed = minPerAppt / minPerSlot;
                                // subscract that from the total slots for the availability
                                slotsAvailable -= slotsConsumed;
                            } //end if
                            //now go to the next appointment in foreach
                        } 
                        // As I said above, we can match only one availability. Once we found it and substracted from it; we are done.
                        break;
                    }
                }
                // We return the integer portion of the variable for display to the user or for calculations.
                // That means, if 2.11 slots are left, the user sees 2. If 2.66, still 2.
                return (int)slotsAvailable;
            }
            */

            /* ORIGINAL ALGORITHM
            sAccessType = "";
            sAvailabilityMessage = "";
            DateTime dStart;
            DateTime dEnd;
            int nAvailableSlots = 999;
            int nSlots = 0;
            int i = 0;
            CGAvailability pAv;
            Rectangle crRectA = new Rectangle(0, 0, 1, 0);
            Rectangle crRectB = new Rectangle(0, 0, 1, 0);
            bool bIsect;
            crRectB.Y = GetTotalMinutes(dSelStart);
            crRectB.Height = GetTotalMinutes(dSelEnd) - crRectB.Y;

            //NOTE: What's this lock? This lock makes sure that nobody is editing the availability array
            //when we are looking at it. Since the availability array could potentially be updated on
            //a different thread, we are can be potentially left stuck with an empty array.
            //
            //The other place that uses this lock is the RefershAvailabilitySchedule method
            //
            //This is a temporary fix until I figure out how to divorce the availbilities here from those drawn
            //on the calendar. Appointments are cloned b/c they are in an object that supports that; and b/c I
            //don't need to suddenly query them at runtime like I do with Availabilities.

            lock (this.m_pAvArray)
            {
                //loop thru m_pAvArray
                //Compare the start time and end time of eachblock
                while (i < m_pAvArray.Count)
                {
                    pAv = (CGAvailability)m_pAvArray[i];
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
                        }//end if
                    }//end if
                    i++;
                }//end while
            }//end lock

            if (nAvailableSlots == 999)
            {
                nAvailableSlots = 0;
            }
            return nAvailableSlots;
            */
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
            int nWeekDay = (int)SelectedDate.DayOfWeek; //0 == Sunday

            // this offset gets approrpriate day based on locale.
            int nOff = (nStartWeekDay + 1) % 7;
            TimeSpan ts = new TimeSpan(nWeekDay - nOff, 0, 0, 0); //d,h,m,s

            // if ts is negative, we will jump to the next week in the logic.
            // to show the correct week, add 7. Confusing, I know.
            if (ts < new TimeSpan()) ts = ts + new TimeSpan(7, 0, 0, 0);

            if (m_nColumnCount == 1) // if one column start and end on the same day.
            {
                ts = new TimeSpan(0, 23, 59, 59);
                WeekStartDay = SelectedDate;
                WeekEndDay = WeekStartDay + ts;
            }
            else if (m_nColumnCount == 5 || m_nColumnCount == 0) // if 5 column start (or default) at the 2nd day of this week and end in 4:23:59:59 days.
            {
                // if picked day is start of week (Sunday in US), start in the next day since that's the first day of work week
                // else, just substract the calculated time span to get to the start of week (first work day)
                WeekStartDay = (nWeekDay == nStartWeekDay) ? SelectedDate + new TimeSpan(1, 0, 0, 0) : SelectedDate - ts;
                // End day calculation
                int nEnd = 3;
                ts = new TimeSpan((7 * nWeeks) - nEnd, 23, 59, 59);
                WeekEndDay = WeekStartDay + ts;
            }
            else // if 7 column start at the 1st day of this week and end in 6:23:59:59 days.
            {
                // if picked day is start of week, use that. Otherwise, go to the fist work day and substract one to get to start of week.
                WeekStartDay = (nWeekDay == nStartWeekDay) ? SelectedDate : SelectedDate - ts - new TimeSpan(1, 0, 0, 0);
                // End day calculation
                int nEnd = 1;
                ts = new TimeSpan((7 * nWeeks) - nEnd, 23, 59, 59);
                WeekEndDay = WeekStartDay + ts;
            }

            bool bRet = ((WeekStartDay.Date != OldStartDay.Date) || (WeekEndDay.Date != OldEndDay.Date));
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
            // i18n code -- Use culture neutral FMDates
            string sStart = FMDateTime.Create(rApptInfo.StartTime).FMDateString;
            string sEnd = FMDateTime.Create(rApptInfo.EndTime).FMDateString;

            TimeSpan sp = rApptInfo.EndTime - rApptInfo.StartTime;
            string sLen = sp.TotalMinutes.ToString();
            string sPatID = rApptInfo.PatientID.ToString();
            string sNote = stripC30C31(rApptInfo.Note);
            string sResource = rApptInfo.Resource;

            string sApptID;

            if (bWalkin == true)
            {
                sApptID = "WALKIN";
            }
            else
            {
                sApptID = rApptInfo.AccessTypeID.ToString();
            }

            string sSql = "BSDX ADD NEW APPOINTMENT^" + sStart + "^" + sEnd + "^" + sPatID + "^" + sResource + "^" + sLen + "^" + sNote + "^" + sApptID + "^" + rApptInfo.RadiologyExamIEN;
            System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "NewAppointment");

            Debug.Assert(dtAppt.Rows.Count == 1);
            DataRow r = dtAppt.Rows[0];
            int nApptID = Convert.ToInt32(r["APPOINTMENTID"]);
            string sErrorID;
            sErrorID = r["ERRORID"].ToString();
            if ((sErrorID != "") || (nApptID < 1))
            {
                throw new Exception(sErrorID);
            }

            //next line is probably done elsewhere
            rApptInfo.WalkIn = bWalkin ? true : false;
            rApptInfo.AppointmentKey = nApptID;

            this.m_appointments.AddAppointment(rApptInfo);

            //Have make appointment from CGView responsible for requesting an update for the avialability.
            //bool bRet = RefreshAvailabilitySchedule();

            //Sam: don't think this is needed as it is called from CGView.
            //Make CGView responsible for all drawing.
            //UpdateAllViews();

            return nApptID;
        }

        /// <summary>
        /// Replaces ascii 30 and 31 as they are used as delimiters. Funny users can crash the program.
        /// </summary>
        /// <param name="s">Input String</param>
        /// <returns>Output Stripped String</returns>
        public string stripC30C31(string s)
        {
            if (s != null && s.Length > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length);
                foreach (char c in s)
                {
                    sb.Append(((c==30)||(c==31)) ? ' ' : c);
                }
                s = sb.ToString();
            }

            return s;
        }

        public void EditAppointment(CGAppointment pAppt, string sNote)
        {
            try
            {
                int nApptID = pAppt.AppointmentKey;
                string sSql = "BSDX EDIT APPOINTMENT^" + nApptID.ToString() + "^" + stripC30C31(sNote);

                System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "EditAppointment");

                Debug.Assert(dtAppt.Rows.Count == 1);
                DataRow r = dtAppt.Rows[0];
                string sErrorID = r["ERRORID"].ToString();
                if (sErrorID == "-1")
                    pAppt.Note = sNote;
            }
            catch (Exception ex)
            {
                Debug.Write("CGDocument.EditAppointment Failed:  " + ex.Message);
            }
        }

        public void CheckInAppointment(int nApptID, DateTime dCheckIn)
        {
            string sCheckIn = FMDateTime.Create(dCheckIn).FMDateString;

            string sSql = "BSDX CHECKIN APPOINTMENT^" + nApptID.ToString() + "^" + sCheckIn; // +"^";
            //sSql += ClinicStopIEN + "^" + ProviderIEN + "^" + PrintRouteSlip + "^";
            //sSql += PCCClinicIEN + "^" + PCCFormIEN + "^" + PCCOutGuide;

            System.Data.DataTable dtAppt = m_DocManager.RPMSDataTable(sSql, "CheckInAppointment");

            Debug.Assert(dtAppt.Rows.Count == 1);
            DataRow r = dtAppt.Rows[0];
            string sErrorID = r["ERRORID"].ToString();
        }

        public string DeleteAppointment(int nApptID)
        {
            return DeleteAppointment(nApptID, true, 0, "");
        }

        public string DeleteAppointment(int nApptID, bool bClinicCancelled, int nReason, string sRemarks)
        {
            //Returns "" if deletion successful
            //Otherwise, returns reason for failure

            string sClinicCancelled = (bClinicCancelled == true) ? "C" : "PC";
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
                
                // View responsible for deciding to redraw the grid; not the document now.
                //bool bRet = RefreshAvailabilitySchedule();
                //UpdateAllViews();
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
            int nIncrement = (nMaximumDays < 30) ? nMaximumDays : 30;

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
                DateTime dNextBlock = (DateTime)drNextBlockRow["NEXTBLOCK"];
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
                            nAccessTypeID = Convert.ToInt16(dr["ACCESS_TYPE"].ToString());
                        if ((nSearchType == -2) && (nAccessTypeID > 0)) //Match on any non-zero type
                        {
                            bFinished = true;
                            bFound = true;
                            dResult = (DateTime)dr["START_TIME"];
                            break;
                        }
                        if (nAccessTypeID == nAVType)
                        {
                            bFinished = true;
                            bFound = true;
                            dResult = (DateTime)dr["START_TIME"];
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

        public string AppointmentNoShow(CGAppointment a, bool bNoShow)
        {
            /*
             * BSDX NOSHOW RPC Returns 1  in ERRORID if  successfully sets NOSHOW flag in BSDX APPOINTMENT and, if applicable, File 2
             *Otherwise, returns negative numbers for failure and errormessage in ERRORTXT
             *
             */
            int nApptID = a.AppointmentKey;
            string sTest = bNoShow.ToString();
            string sNoShow = (bNoShow == true) ? "1" : "0";
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

            //All okay over here... then set appointment noshow or not no show...
            a.NoShow = bNoShow ? true : false;

            //bool bRet = RefreshSchedule();

            return sErrorID;
        }

        public bool AppointmentUndoCheckin(CGAppointment a, out string msg)
        {
            msg = "";
            //zero good, anything else bad
            DataTable dt = CGDocumentManager.Current.DAL.RemoveCheckIn(a.AppointmentKey);
            if (dt.Rows[0][0].ToString() == "0")
            {
                a.CheckInTime = DateTime.MinValue; // remove check-in time.
                return true;
            }

            else
            {
                msg = dt.Rows[0][0].ToString();
                return false;
            }
        }

        //DON'T USE OBVIOUSLY. JUST FOR TESTING.
        public void ThrowException()
        {
            throw new Exception("Hello World. I am an Exception.");
        }

        #endregion Methods

    }//End class
}//End namespace
