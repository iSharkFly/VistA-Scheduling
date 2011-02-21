using System;
using System.Data;
//using System.Data.OleDb;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using IndianHealthService.BMXNet;

namespace IndianHealthService.ClinicalScheduling
{
	public enum ScheduleType 
	{
		Resource,
		Clinic
	}

	/// <summary>
	/// CGSchedLib contains static functions that are called from throughout the 
	/// scheduling application.
	/// </summary>
	public static class CGSchedLib
	{
        /// <summary>
        /// Gets appointments from VISTA to display in Grid
        /// </summary>
        /// <param name="docManager"></param>
        /// <param name="saryResNames"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
		public static DataTable CreateAppointmentSchedule(CGDocumentManager docManager, ArrayList saryResNames, DateTime StartTime, DateTime EndTime)
		{
			string sResName = "";
			for  (int i = 0; i < saryResNames.Count; i++)
			{
				sResName += saryResNames[i];
				if ((i+1) < saryResNames.Count)
					sResName += "|";
			}
			
			
            string sStart = FMDateTime.Create(StartTime).DateOnly.FMDateString;
            string sEnd = FMDateTime.Create(EndTime).FMDateString;
			string sSql = "BSDX CREATE APPT SCHEDULE^" + sResName + "^" + sStart + "^" + sEnd ;
			DataTable dtRet = docManager.RPMSDataTable(sSql, "AppointmentSchedule");
			return dtRet;
			
		}


        /// <summary>
        /// Gets all Availabilities and Appointments, then substracts Appointments from availabilities.
        /// </summary>
        /// <param name="docManager">God Class</param>
        /// <param name="saryResourceNames">Resource Array (ArrayList)</param>
        /// <param name="StartTime">Self-Explanatory</param>
        /// <param name="EndTime">Self-Explanatory</param>
        /// <param name="saryApptTypes">Array of Access Type IDs</param>
        /// <param name="stType"></param>
        /// <param name="sSearchInfo"></param>
        /// <returns></returns>
		public static DataTable CreateAvailabilitySchedule(CGDocumentManager docManager, 
			ArrayList saryResourceNames, DateTime StartTime, DateTime EndTime, 
			ArrayList saryApptTypes,/**/ ScheduleType stType, string sSearchInfo) 
		{
			DataTable rsOut;
			rsOut = new DataTable("AvailabilitySchedule");

			DataTable rsSlotSchedule;
			DataTable rsApptSchedule;
			DataTable rsTemp1;

			int nSize = saryResourceNames.Count;
			if (nSize == 0) 
			{
				return rsOut;
			}
			
			string sResName;
            //TODO: Optimize: no need to keep looping through resources.
            // for each resource
			for (int i = 0; i < nSize; i++) 
			{
				sResName = saryResourceNames[i].ToString();
                //Gets all the slots (or Availabities, or AV Blocks if you like)
				rsSlotSchedule = CGSchedLib.CreateAssignedSlotSchedule(docManager, sResName, StartTime, EndTime, saryApptTypes,/**/ stType, sSearchInfo);
				
                //if we have slots
				if (rsSlotSchedule.Rows.Count > 0 ) 
				{
                    // Get appointment count to substract from the slots
					rsApptSchedule = CGSchedLib.CreateAppointmentSlotSchedule(docManager, sResName, StartTime, EndTime, stType);

                    // Perform the substraction
					rsTemp1 = CGSchedLib.SubtractSlotsRS2(rsSlotSchedule, rsApptSchedule, sResName);

				}
                //otherwise, just return the slot schedule we have.
				else 
				{
					rsTemp1 = rsSlotSchedule;

				}

                // if only one resource was passed in, its availablility is what we want
				if (i == 0) 
				{
					rsOut = rsTemp1;

				} 
                // if more than one resource, merge them together
				else 
				{
					rsOut = CGSchedLib.UnionBlocks(rsTemp1, rsOut);
				}
			}
			return rsOut;
		}		


        /* NOT USED ANYMORE!!!
		public static DataTable CreateAssignedTypeSchedule(CGDocumentManager docManager, string sResourceName, DateTime StartTime, DateTime EndTime, ScheduleType stType)
		{

            string sStart = FMDateTime.Create(StartTime).DateOnly.FMDateString;
			string sEnd = FMDateTime.Create(EndTime).DateOnly.FMDateString;
            string sSql = "BSDX TYPE BLOCKS OVERLAP^" + sStart + "^" + sEnd + "^" + sResourceName;

			DataTable rs = docManager.RPMSDataTable(sSql, "AssignedTypeSchedule");

			if (rs.Rows.Count == 0)
				return rs;

			DataTable rsCopy = new DataTable("rsCopy");			
			DataColumn dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "StartTime";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "EndTime";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.Int16");
			dCol.ColumnName = "AppointmentTypeID";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add(dCol);

			dCol = new DataColumn();
            dCol.DataType = Type.GetType("System.Int32");
            dCol.ColumnName = "AvailabilityID";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.String");
			dCol.ColumnName = "ResourceName";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add(dCol);


			DateTime dLastEnd;
			DateTime dStart;
			DateTime dEnd;
			DataRow rNew; //Temporary Holding place for first or last row

            // Get Last Date from final row
			rNew = rs.Rows[rs.Rows.Count - 1];
			dLastEnd = (DateTime) rNew["EndTime"];
            // Get First Date from first row
			rNew = rs.Rows[0];
			dStart = (DateTime) rNew["StartTime"];

			long UNSPECIFIED_TYPE = 10;
			long UNSPECIFIED_ID = 0;

			//if first block in vgetrows starts later than StartTime,
			// then pad with a new block

            // if First Row time is later than the StartTime param (should always be true)
            // then make a new row whose time starts from StartTime and ends with dStart
			if (dStart > StartTime) 
			{
				dEnd = dStart;
				rNew = rsCopy.NewRow();
				rNew["StartTime"] = StartTime;
				rNew["EndTime"] = dEnd;
				rNew["AppointmentTypeID"] = UNSPECIFIED_TYPE;
				rNew["AvailabilityID"] = UNSPECIFIED_ID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add(rNew);
			}	
			
			//if first block start time is < StartTime then trim (shouldn't happen)
			if (dStart < StartTime) 
			{
				rNew = rs.Rows[0];
				rNew["StartTime"] = StartTime;
				dStart = StartTime;
			}	

			int nAppointmentTypeID;
			int nAvailabilityID;

            //dStart holds the first date for the availabilities returned from RPMS
			dEnd = dStart;
			foreach (DataRow rEach in rs.Rows)
			{
				dStart = (DateTime) rEach["StartTime"];
				if (dStart > dEnd) 
				{
					rNew = rsCopy.NewRow();
					rNew["StartTime"] = dEnd;
					rNew["EndTime"] = dStart;
					rNew["AppointmentTypeID"] = 0;
					rNew["AvailabilityID"] = UNSPECIFIED_ID;
					rNew["ResourceName"] = sResourceName;
					rsCopy.Rows.Add(rNew);
				}

                //dEnd now EndTime for AV Block
				dEnd = (DateTime) rEach["EndTime"];

                // if dEnd is greater than endime, set dEnd to be the same as EndTime.
                if (dEnd > EndTime) { dEnd = EndTime; }


				nAppointmentTypeID = (int) rEach["AppointmentTypeID"];
				nAvailabilityID = (int) rEach["AvailabilityID"];
				
				rNew = rsCopy.NewRow();
				rNew["StartTime"] = dStart;
				rNew["EndTime"] = dEnd;
				rNew["AppointmentTypeID"] = nAppointmentTypeID;
				rNew["AvailabilityID"] = nAvailabilityID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add(rNew);
			}

			//Pad the end if necessary
			if (dLastEnd < EndTime) 
			{
				rNew = rsCopy.NewRow();
				rNew["StartTime"] = dLastEnd;
				rNew["EndTime"] = EndTime;
				rNew["AppointmentTypeID"] = UNSPECIFIED_TYPE;
				rNew["AvailabilityID"] = UNSPECIFIED_ID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add(rNew);		
			}
			
			return rsCopy;
		}
        */

        /// <summary>
        /// Gets the Availability Slots from the Server
        /// </summary>
        /// <param name="docManager">God Class</param>
        /// <param name="sResourceName">Resource for which to get slots</param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="rsaryApptTypeIDs">Access Type IDs to retrieve</param>
        /// <param name="stType">Not used</param>
        /// <param name="sSearchInfo">If performing a slot search (i.e. for empty appointments), has search info here. Used by Find Appointments</param>
        /// <returns>DataTable with the following Columns:
        /// D00030START_TIME^D00030END_TIME^I00010SLOTS^T00030RESOURCE^T00010ACCESS_TYPE^T00250NOTE^I00030AVAILABILITYID
        /// </returns>
		public static DataTable CreateAssignedSlotSchedule(CGDocumentManager docManager, string sResourceName, DateTime StartTime, 
            DateTime EndTime, ArrayList rsaryApptTypeIDs, /**/ ScheduleType stType, string sSearchInfo) 
		{
            //Appointment type ids is now always "" so that all appointment types are returned.
			string sApptTypeIDs = "";
			
            //flatten types by '|'
			int nSize = rsaryApptTypeIDs.Count;  //nSize is used to decide where to put the '|' sent in the RPC as we flatten sApptTypeIDs
			for (int i=0; i < nSize; i++) 
			{
				sApptTypeIDs += rsaryApptTypeIDs[i];
				if (i < (nSize-1))
					sApptTypeIDs += "|";
			}	
	
            string sStart = FMDateTime.Create(StartTime).DateOnly.FMDateString;
            string sEnd = FMDateTime.Create(EndTime).FMDateString;
			string sSql = "BSDX CREATE ASGND SLOT SCHED^" + sResourceName + "^" + sStart + "^" + sEnd + "^" + sApptTypeIDs + "^" + sSearchInfo; //+ "^" + sSTType ;
			DataTable dtRet = docManager.RPMSDataTable(sSql, "AssignedSlotSchedule");

            return dtRet;
		}

		public static DataTable CreateCopyTable()
		{
			DataTable dtCopy = new DataTable("dtCopy");			
			DataColumn dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "START_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "END_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.Int16");
			dCol.ColumnName = "SLOTS";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.String");
			dCol.ColumnName = "RESOURCE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.String");
			dCol.ColumnName = "ACCESS_TYPE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.String");
			dCol.ColumnName = "NOTE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);


			return dtCopy;
		}

        /// <summary>
        /// This gets a datatable which shows the appointments between start and end time, one row per appointment
        /// </summary>
        /// <param name="docManager"></param>
        /// <param name="sResourceName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="stType"></param>
        /// <returns>DataTable with 4 columns: START_TIME, END_TIME, SLOTS, RESOURCE </returns>
		public static DataTable CreateAppointmentSlotSchedule(CGDocumentManager docManager, string sResourceName, DateTime StartTime, DateTime EndTime, ScheduleType stType)
		{
            //Change Dates to FM Format
            string sStart = FMDateTime.Create(StartTime).DateOnly.FMDateString;
			string sEnd = FMDateTime.Create(EndTime).DateOnly.FMDateString;
			
            string sSTType = (stType == ScheduleType.Resource ? "ST_RESOURCE" : "ST_CLINIC");
			string sSql = "BSDX APPT BLOCKS OVERLAP^" + sStart + "^" + sEnd + "^" + sResourceName ;//+ "^"  + sSTType;

            //This gets you a table with 2 columns containing start and end time for each appt
            //Each appt gets its own row
			DataTable dtRet = docManager.RPMSDataTable(sSql, "AppointmentSlotSchedule");
			
			if (dtRet.Rows.Count < 1)
				return dtRet;
			
			//Create CDateTimeArray & load records from rsOut
			int nRC = dtRet.Rows.Count;     // nRC is row count from Appointments table just retrieved
			ArrayList cdtArray = new ArrayList();
			cdtArray.Capacity = (nRC * 2); //new ArrayList has capacity double that of appointment table
			DateTime v;
			int i = 0;

			//for each row in the Appointment Table, ArrayList cdtArray gets 2 entries: Start and End times
            foreach (DataRow r in dtRet.Rows) 
			{
				v = (DateTime) r[dtRet.Columns["START_TIME"]];
				cdtArray.Add(v);
				v = (DateTime) r[dtRet.Columns["END_TIME"]];
				cdtArray.Add(v);
			}

            //Sort start and end times (for use in ScheduleFromArray method)
			cdtArray.Sort();

			//Create a CTimeBlockArray and load it from rsOut
		
            //Now, create a new ArrayList with the size of the appointment table to hold availabilities
			ArrayList ctbAppointments = new ArrayList(nRC);
			CGAvailability cTB;
			i = 0;
            //For each appointment, create an availability
			foreach (DataRow r in dtRet.Rows) 
			{
				cTB = new CGAvailability();
				cTB.StartTime = (DateTime) r[dtRet.Columns["START_TIME"]];
				cTB.EndTime = (DateTime) r[dtRet.Columns["END_TIME"]];
				ctbAppointments.Add(cTB);
			}			
			
			//Create a TimeBlock Array from the data in the DateTime array
			ArrayList ctbApptSchedule = new ArrayList();

            //Convert Appointments to Availabilities, where all appointments become squeezed together.
			ScheduleFromArray(cdtArray, StartTime, EndTime, ref ctbApptSchedule);

            /*So far, we have the following:
             * dtRet -> List of Appointments Start and End times, one row per appointment
             * cdtArray -> Linear 1 dimensional Array of dtRet Start and End times, sorted
             * ctbAppointments -> Arraylist of dtRet as Availabilities
             * ctbApptSchedule -> Arraylist of dtRet as Availabilities with no overlapping boundaries
             *      (overlaps in appointments get converted into availabilties themselves: so
             *      2 appts as 10:10-10:30 and 10:20-10:40 get converted into 10:10-10:20,
             *      10:20-10:30, 10:30-10:40).
            */

            //Find number of TimeBlocks in ctbApptSchedule that overlap the TimeBlocks in ctbAppointments
			ArrayList ctbApptSchedule2 = new ArrayList();
			CGAvailability cTB2;
			int nSlots = 0;
			for (i=0; i< ctbApptSchedule.Count; i++)    //for each non-overlapping appts as availabilities
			{
				cTB = (CGAvailability) ctbApptSchedule[i];
                //How many times does the non-overlapping part show up in an appointment slot?
				nSlots = BlocksOverlap(cTB, ctbAppointments);   
				cTB2 = new CGAvailability();
				cTB2.Create(cTB.StartTime, cTB.EndTime, nSlots);
				ctbApptSchedule2.Add(cTB2);
			}
			
			ConsolidateBlocks(ctbApptSchedule2);
			
			DataTable dtCopy = new DataTable("dtCopy");			
			DataColumn dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "START_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.DateTime");
			dCol.ColumnName = "END_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.Int16");
			dCol.ColumnName = "SLOTS";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);
			
			dCol = new DataColumn();
			dCol.DataType = Type.GetType("System.String");
			dCol.ColumnName = "RESOURCE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add(dCol);

			for (int k=0; k < ctbApptSchedule2.Count; k++) 
			{
				cTB = (CGAvailability) ctbApptSchedule2[k];
				DataRow newRow;
				newRow = dtCopy.NewRow();
				newRow["START_TIME"] = cTB.StartTime;
				newRow["END_TIME"] = cTB.EndTime;
				newRow["SLOTS"] = cTB.Slots;
				newRow["RESOURCE"] = sResourceName;
				dtCopy.Rows.Add(newRow);
			}

			return dtCopy;

		}

		public static int BlocksOverlap(CGAvailability rBlock, ArrayList rTBArray)
		{
			//this overload implements default false for bCountSlots
			return CGSchedLib.BlocksOverlap(rBlock, rTBArray, false);
		}
		public static int BlocksOverlap(CGAvailability rBlock, ArrayList rTBArray, bool bCountSlots)
		{
			//If bCountSlots == true, then returns
			//sum of Slots in overlapping blocks 
			//instead of the count of overlapping blocks

			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			int nSlots;
			int nCount = 0;
			CGAvailability cBlock;

			dStart1 = rBlock.StartTime;
			dEnd1 = rBlock.EndTime;

			for (int j=0; j< rTBArray.Count; j++) 
			{
				cBlock = (CGAvailability) rTBArray[j];
				dStart2 = cBlock.StartTime;
				dEnd2 = cBlock.EndTime;
				nSlots = cBlock.Slots;
				if (TimesOverlap(dStart1, dEnd1, dStart2, dEnd2)) 
				{
					if (bCountSlots == true) 
					{
						nCount += nSlots;
					}
					else 
					{
						nCount++;
					}
				}
			}

			return nCount;
		}

		/// <summary>
		/// Does an appointment overlap with another appointment in the same day?
		/// </summary>
		/// <param name="dStart1">Start Time of First Appt</param>
		/// <param name="dEnd1">End Time of First Appt</param>
        /// <param name="dStart2">Start Time of Second Appt</param>
		/// <param name="dEnd2">End Time of Second Appt</param>
		/// <returns>true or false</returns>
        /// <remarks>Draws 2 rectangles and sees if they overlap using minutes from 1980 as the start point</remarks>
		public static bool TimesOverlap(DateTime dStart1, DateTime dEnd1, DateTime dStart2, DateTime dEnd2)
		{
			Rectangle rect1 = new Rectangle();
			Rectangle rect2 = new Rectangle();
			rect1.X = 0;
			rect2.X = 0;
			rect1.Width = 1;
			rect2.Width = 1;

			rect1.Y = CGSchedLib.MinSince80(dStart1);
			rect1.Height = CGSchedLib.MinSince80(dEnd1) - rect1.Y;
			rect2.Y = CGSchedLib.MinSince80(dStart2);
			rect2.Height = CGSchedLib.MinSince80(dEnd2) - rect2.Y;
			bool bRet = rect2.IntersectsWith(rect1);
			return bRet;
		}

		public static void ConsolidateBlocks(ArrayList rTBArray)
		{
			//TODO: Test this function

			int j = 0;
			bool bDirty = false;
			CGAvailability cBlockA;
			CGAvailability cBlockB;
			CGAvailability cTemp1;
			if (rTBArray.Count < 2)
				return;
			do 
			{
				bDirty = false;
				for (j = 0; j < (rTBArray.Count - 1); j++) //TODO: why minus 1?
				{
					cBlockA = (CGAvailability) rTBArray[j];
					cBlockB = (CGAvailability) rTBArray[j+1];
					if ((cBlockA.EndTime == cBlockB.StartTime) 
						&& (cBlockA.Slots == cBlockB.Slots)
						&& (cBlockA.ResourceList == cBlockB.ResourceList)
						&& (cBlockA.AccessRuleList == cBlockB.AccessRuleList)
						) 
					{
						cTemp1 = new CGAvailability();
						cTemp1.StartTime = cBlockA.StartTime;
						cTemp1.EndTime = cBlockB.EndTime;
						cTemp1.Slots = cBlockA.Slots;
						cTemp1.AccessRuleList = cBlockA.AccessRuleList;
						cTemp1.ResourceList = cBlockA.ResourceList;
						rTBArray.Insert(j, cTemp1);
						rTBArray.RemoveRange(j+1, 2);
						bDirty = true;
						break;
					}
				}
			}
			while (!((bDirty == false) || (rTBArray.Count == 1)));
		}

        /// <summary>
        /// My guess is that this calculates remaining slots
        /// </summary>
        /// <param name="rsBlocks1"></param>
        /// <param name="rsBlocks2"></param>
        /// <param name="sResource"></param>
        /// <returns></returns>
		public static DataTable SubtractSlotsRS2(DataTable rsBlocks1, DataTable rsBlocks2, string sResource)
		{
			//Subtract slots in rsBlocks2 from rsBlocks1
			//7-16-01 SUBCLINIC data field in rsBlocks1 persists thru routine.
			//7-18-01 RESOURCE and ACCESS_TYPE fields presisted

			if ((rsBlocks2.Rows.Count == 0) || (rsBlocks1.Rows.Count == 0))
				return rsBlocks1;


			//Create an array of the start and end times of blocks2
			ArrayList cdtArray = new ArrayList(2*(rsBlocks1.Rows.Count + rsBlocks2.Rows.Count));

			foreach (DataRow r in rsBlocks1.Rows) 
			{
				cdtArray.Add(r[rsBlocks1.Columns["START_TIME"]]);
				cdtArray.Add(r[rsBlocks1.Columns["END_TIME"]]);
			}

			foreach (DataRow r in rsBlocks2.Rows) 
			{
				cdtArray.Add(r[rsBlocks2.Columns["START_TIME"]]);
				cdtArray.Add(r[rsBlocks2.Columns["END_TIME"]]);
			}

			cdtArray.Sort();

			ArrayList ctbReturn = new ArrayList();
			DateTime cDate = new DateTime();
			ScheduleFromArray(cdtArray, cDate, cDate, ref ctbReturn);

			//Set up return table
			DataTable rsCopy = CGSchedLib.CreateCopyTable();	//TODO: There's a datatable method that does this.	
			long nSlots = 0;
			CGAvailability cTB;

			for (int j=0; j < (ctbReturn.Count -1); j++) //TODO: why minus 1?
			{
				cTB = (CGAvailability) ctbReturn[j];
				nSlots = SlotsInBlock(cTB, rsBlocks1) - SlotsInBlock(cTB, rsBlocks2);
				string sResourceList = "";
				string sAccessRuleList = "";
				string sNote = "";

				if (nSlots > 0) 
				{
					bool bRet = ResourceRulesInBlock(cTB, rsBlocks1, ref sResourceList, ref sAccessRuleList, ref sNote);
				}
				DataRow newRow;
				newRow = rsCopy.NewRow();
				newRow["START_TIME"] = cTB.StartTime;
				newRow["END_TIME"] = cTB.EndTime;
				newRow["SLOTS"] = nSlots;
				//Subclinic, Access Rule and Resource are null in subtractedSlot sets
				newRow["RESOURCE"] = sResource;
				newRow["ACCESS_TYPE"] = sAccessRuleList;

				newRow["NOTE"] = sNote;

				rsCopy.Rows.Add(newRow);
			}
			return rsCopy;

		}

		public static DataTable UnionBlocks(DataTable rs1, DataTable rs2)
		{
			//Test input tables
			Debug.Assert(rs1 != null);
			Debug.Assert(rs2 != null);
			
			DataTable rsCopy;
			DataRow dr;
			if (rs1.Columns.Count >= rs2.Columns.Count)
			{
				rsCopy = rs1.Copy();
				foreach (DataRow dr2 in rs2.Rows)
				{
					dr = rsCopy.NewRow();
					dr.ItemArray = dr2.ItemArray;
					//dr["START_TIME"] = dr2["START_TIME"];
					//dr["END_TIME"] = dr2["END_TIME"];
					//dr["SLOTS"] = dr2["SLOTS"];
					rsCopy.Rows.Add(dr);
				}			
			}
			else
			{
				rsCopy = rs2.Copy();
				foreach (DataRow dr2 in rs1.Rows)
				{
					dr = rsCopy.NewRow();
					dr.ItemArray = dr2.ItemArray;
					rsCopy.Rows.Add(dr);
				}			
			}
			return rsCopy;
		}

		public static DataTable IntersectBlocks(DataTable rs1, DataTable rs2)
		{
			DataTable rsCopy = CGSchedLib.CreateCopyTable();

			//Test input tables

			if ((rs1 == null) || (rs2 == null))
				return rsCopy;

			if ((rs1.Rows.Count == 0) || (rs2.Rows.Count == 0))
				return rsCopy;

			int nSlots1 = 0;
			int nSlots2 = 0;
			int nSlots3 = 0;
			DateTime dStart1;
			DateTime dStart2;
			DateTime dStart3;
			DateTime dEnd1;
			DateTime dEnd2;
			DateTime dEnd3;
			string sClinic;
			string sClinic2;
//			Rectangle rect1 = new Rectangle();
//			Rectangle rect2 = new Rectangle();
//			rect1.X = 0;
//			rect2.X = 0;
//			rect1.Width = 1;
//			rect2.Width = 1;

			//			DataColumn cSlots = rs1.Columns["SLOTS"];
			foreach (System.Data.DataRow r1 in rs1.Rows)
			{
				nSlots1 = (int) r1[rs1.Columns["SLOTS"]];
				if (nSlots1 > 0) 
				{
					dStart1 = (DateTime) r1[rs1.Columns["START_TIME"]];
					dEnd1 = (DateTime) r1[rs1.Columns["END_TIME"]];
					sClinic = r1[rs1.Columns["SUBCLINIC"]].ToString();
					if (sClinic == "NULL")
						sClinic = "";
//					rect1.Y = CGSchedLib.MinSince80(dStart1);
//					rect1.Height = CGSchedLib.MinSince80(dEnd1) - rect1.Y;
					foreach (System.Data.DataRow r2 in rs2.Rows) 
					{
						nSlots2 = (int) r2[rs2.Columns["SLOTS"]];

						if (nSlots2 > 0) 
						{
							dStart2 = (DateTime) r2[rs2.Columns["START_TIME"]];
							dEnd2 = (DateTime) r2[rs2.Columns["END_TIME"]];
							sClinic2 = r2[rs2.Columns["SUBCLINIC"]].ToString();
//							rect2.Y = CGSchedLib.MinSince80(dStart2);
//							rect2.Height = CGSchedLib.MinSince80(dEnd2) - rect2.Y;
							if (
								/*(rect2.IntersectsWith(rect1) == true)*/
								(CGSchedLib.TimesOverlap(dStart1, dEnd1, dStart2, dEnd2) == true)
								&& 
								((sClinic == sClinic2) || (sClinic == "NONE") || (sClinic2 == "NONE"))
								)
							{
								dStart3 = (dStart1 >= dStart2) ? dStart1 : dStart2 ;
								dEnd3 = (dEnd1 >= dEnd2) ? dEnd2 : dEnd1;
								nSlots3 = (nSlots1 >= nSlots2) ? nSlots2 : nSlots1 ;

								DataRow newRow;
								newRow = rsCopy.NewRow();
								newRow["START_TIME"] = dStart3;
								newRow["END_TIME"] = dEnd3;
								newRow["SLOTS"] = nSlots3;
								newRow["SUBCLINIC"] = (sClinic == "NONE") ? sClinic2 : sClinic;
								//Access Rule and Resource are null in interesected sets
								newRow["ACCESS_TYPE"] = "";
								newRow["RESOURCE"] = "";
								rsCopy.Rows.Add(newRow);
							}
						}//nSlots2 > 0
					}//foreach r2 in rs2.rows					
				}//nSlots1 > 0
			}//foreach r1 in rs1.rows
			return rsCopy;
		}//end IntersectBlocks

        /// <summary>
        /// Number of minutes since Jan 1 1980
        /// </summary>
        /// <param name="d">Date to compare</param>
        /// <returns>Minutes as integer</returns>
		public static int MinSince80(DateTime d)
		{
			//Returns the total minutes between d and 1 Jan 1980
			DateTime y = new DateTime(1980,1,1,0,0,0);
			Debug.Assert(d > y);
			TimeSpan ts = d - y;
			//Assure ts.TotalMinutes within int range so that cast on next line works
			Debug.Assert(ts.TotalMinutes < 2147483646); 
			int nMinutes = (int) ts.TotalMinutes;
			return nMinutes;
		}

        /// <summary>
        /// Converts an Array of Times like this:
        /// 10:00 10:00 10:20 10:20 10:30 10:30 10:30 10:40
        /// To an array of availabilities like this:
        /// 12:00-10:00 10:00-10:20 10:20-10:30 10:30-10:40
        /// Where the 12:00 comes from the start time
        /// </summary>
        /// <param name="cdtArray">ArrayList containing start and end times of Appointments combmined and sorted</param>
        /// <param name="dStartTime">Start Time for Schedule</param>
        /// <param name="dEndTime">End Time for Schedule</param>
        /// <param name="rTBArray">Output of Availabilities: Pass Empty</param>
		public static void ScheduleFromArray(ArrayList cdtArray, DateTime dStartTime, DateTime dEndTime, 
            ref ArrayList rTBArray)
		{
			int j = 0;
			CGAvailability cTB;

			if (cdtArray.Count == 0)
				return;

			Debug.Assert(cdtArray.Count > 0);
			Debug.Assert(cdtArray[0].GetType() == typeof(DateTime));

			//If StartTime passed in, then adjust for it
			if (dStartTime.Ticks > 0) 
			{
				// if first Array Entry greater than start time, then create an availability
                // with the start time as dStartTime, and end time as the first array entry, and 0 slots
                // then add this to the output array
                if ((DateTime) cdtArray[0] > dStartTime) 
				{
					cTB = new CGAvailability();
					cTB.Create(dStartTime, (DateTime) cdtArray[0], 0);
					rTBArray.Add(cTB);
				}

                // if first Array Entry less than start time (shouldn't happen),
                // convert all input array's times less than the start time to all be the start time
				if ((DateTime) cdtArray[0] < dStartTime) 
				{
					for (j = 0; j < cdtArray.Count; j++) 
					{
						if ((DateTime) cdtArray[j] < dStartTime)
							cdtArray[j] = dStartTime;
					}
				}
			}

			//Trim the end if necessary
            //If end time is passed, set all the times in the original array greater
            //than the end time to be the end time (Shouldn't happen).
			if (dEndTime.Ticks > 0) 
			{
				for (j = 0; j < cdtArray.Count; j++) 
				{
					if ((DateTime) cdtArray[j] > dEndTime)
						cdtArray[j] = dEndTime;
				}
			}

			//build the schedule in rTBArray
			DateTime dTemp = new DateTime(); //hold previous appt time.
			DateTime dStart;
			DateTime dEnd;
			int k = 0;
            //for each time in appointment array
			for (j = 0; j < (cdtArray.Count -1); j++) // -1 b/c k below starts with j+1.
			{
				if ((DateTime) cdtArray[j] != dTemp) 
				{
					dStart =(DateTime) cdtArray[j];
					dTemp = dStart;
                    
                    //for each time in appointment array, starting with the next one
					for (k = j+1; k < cdtArray.Count; k++) 
					{
						dEnd = new DateTime();
						if ((DateTime) cdtArray[k] != dStart) //if the next time isn't the same
						{
							dEnd = (DateTime) cdtArray[k]; // set the end to be the next time
						}
						if (dEnd.Ticks > 0) //make a new availability and add it to the output array.
						{
							cTB = new CGAvailability();
							cTB.Create(dStart, dEnd, 0);
							rTBArray.Add(cTB);
							break;
						}
                        // if the end time is still empty, loop
					}
				}
			}

		}//end ScheduleFromArray

		//long CResourceLink::SlotsInBlock(CTimeBlock &rTimeBlock, _RecordsetPtr rsBlock)
		public static int SlotsInBlock(CGAvailability rTimeBlock, DataTable rsBlock)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			int nSlots = 0;

			if (rsBlock.Rows.Count == 0)
				return nSlots;

			Rectangle rect1 = new Rectangle();
			Rectangle rect2 = new Rectangle();
			rect1.X = 0;
			rect2.X = 0;
			rect1.Width = 1;
			rect2.Width = 1;

			dStart1 = rTimeBlock.StartTime;
			dEnd1 = rTimeBlock.EndTime;
			rect1.Y = CGSchedLib.MinSince80(dStart1);
			rect1.Height = CGSchedLib.MinSince80(dEnd1) - rect1.Y;

            //for each row in the availability schedule
			foreach (DataRow r in rsBlock.Rows) 
			{
				dStart2 = (DateTime) r[rsBlock.Columns["START_TIME"]];
				dEnd2 = (DateTime) r[rsBlock.Columns["END_TIME"]];

				rect2.Y = CGSchedLib.MinSince80(dStart2);
				rect2.Height = CGSchedLib.MinSince80(dEnd2) - rect2.Y;
				if (rect2.IntersectsWith(rect1) == true)
				{
					string sSlots =  r[rsBlock.Columns["SLOTS"]].ToString();
					nSlots = System.Convert.ToInt16(sSlots);
//					nSlots = (int) r[rsBlock.Columns["SLOTS"]];
					break;
				}
			}
			return nSlots;
		}//end SlotsInBlock

		public static string ClinicInBlock(CGAvailability rTimeBlock, DataTable rsBlock)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			string sClinic = "";

			if (rsBlock.Rows.Count == 0)
				return sClinic;

			Rectangle rect1 = new Rectangle();
			Rectangle rect2 = new Rectangle();
			rect1.X = 0;
			rect2.X = 0;
			rect1.Width = 1;
			rect2.Width = 1;

			dStart1 = rTimeBlock.StartTime;
			dEnd1 = rTimeBlock.EndTime;
			rect1.Y = CGSchedLib.MinSince80(dStart1);
			rect1.Height = CGSchedLib.MinSince80(dEnd1) - rect1.Y;

			foreach (DataRow r in rsBlock.Rows) 
			{
				dStart2 = (DateTime) r[rsBlock.Columns["START_TIME"]];
				dEnd2 = (DateTime) r[rsBlock.Columns["END_TIME"]];

				rect2.Y = CGSchedLib.MinSince80(dStart2);
				rect2.Height = CGSchedLib.MinSince80(dEnd2) - rect2.Y;
				if (rect2.IntersectsWith(rect1) == true)
				{
					sClinic =  r[rsBlock.Columns["SUBCLINIC"]].ToString();
					break;
				}
			}
			return sClinic;
		}//end ClinicInBlock

		public static bool ResourceRulesInBlock(CGAvailability rTimeBlock, DataTable rsBlock, ref string sResourceList, ref string sAccessRuleList, ref string sNote)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			string sResource;
			string sAccessRule;

			if (rsBlock.Rows.Count == 0)
				return true;

			Rectangle rect1 = new Rectangle();
			Rectangle rect2 = new Rectangle();
			rect1.X = 0;
			rect2.X = 0;
			rect1.Width = 1;
			rect2.Width = 1;

			dStart1 = rTimeBlock.StartTime;
			dEnd1 = rTimeBlock.EndTime;
			rect1.Y = CGSchedLib.MinSince80(dStart1);
			rect1.Height = CGSchedLib.MinSince80(dEnd1) - rect1.Y;

			foreach (DataRow r in rsBlock.Rows) 
			{
				dStart2 = (DateTime) r[rsBlock.Columns["START_TIME"]];
				dEnd2 = (DateTime) r[rsBlock.Columns["END_TIME"]];

				rect2.Y = CGSchedLib.MinSince80(dStart2);
				rect2.Height = CGSchedLib.MinSince80(dEnd2) - rect2.Y;
				if (rect2.IntersectsWith(rect1) == true)
				{
					sResource = r[rsBlock.Columns["RESOURCE"]].ToString();
					if (sResource == "NULL")
						sResource = "";
					if (sResource != "") 
					{
						if (sResourceList == "") 
						{
							sResourceList += sResource;
						}
						else 
						{
							sResourceList += "^" + sResource;
						}
					}
					sAccessRule = r[rsBlock.Columns["ACCESS_TYPE"]].ToString();
					if (sAccessRule == "0")
						sAccessRule = "";
					if (sAccessRule != "") 
					{
						if (sAccessRuleList == "") 
						{
							sAccessRuleList += sAccessRule;
						}
						else 
						{
							sAccessRuleList += "^" + sAccessRule;
						}
					}
					sNote = r[rsBlock.Columns["NOTE"]].ToString();

				}
			}
			return true;
		}//End ResourceRulesInBlock

	
	}
}
