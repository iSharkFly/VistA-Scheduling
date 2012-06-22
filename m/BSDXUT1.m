BSDXUT1 ; VEN/SMH - Unit Tests for Scheduling GUI - cont. ; 6/22/12 1:44pm
	;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	;
	;
UT29 ; Unit Test for BSDX29
	; HLs/Resources are created as part of the UT
	; Patients 1,2,3,4,5 must exist
	;
	I '$$TM^%ZTLOAD() W "Cannot test. Taskman is not running!" QUIT
	;
	; Set-up - Create Clinics
	N RESNAM S RESNAM="UTCLINIC"
	N HLRESIENS ; holds output of UTCR^BSDXUT - HL IEN^Resource IEN
	D
	. N $ET S $ET="D ^%ZTER B"
	. S HLRESIENS=$$UTCR^BSDXUT(RESNAM)
	. I HLRESIENS<0 S $EC=",U1," ; not supposed to happen - hard crash if so
	;
	N HLIEN,RESIEN
	S HLIEN=$P(HLRESIENS,U)
	S RESIEN=$P(HLRESIENS,U,2)
	;
	; Turn off SDAM APPT PROTOCOL BSDX Entries
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute BSDX ADD APPOINTMENT protocol
	;
	; Create a bunch of appointments in PIMS (25 actually)
	N DFN
	N BSDXAPPT,BSDXDATE
	N BSDXI
	F BSDXI=1:1:5 D
	. N APPTTIME S APPTTIME=$$TIMEHL^BSDXUT(HLIEN) ; appt time
	. F DFN=1,2,3,4,5 D
	. . N % S %=$$MAKE1^BSDXAPI(DFN,HLIEN,3,APPTTIME,15,"Sam Test Appt"_DFN)
	. . I % W "Error in $$MAKE1^BSDXAPI for TIME "_APPTTIME_" for DFN "_DFN,!,%,!
	. . E  S BSDXAPPT(DFN,APPTTIME)="",BSDXDATE(APPTTIME)=""
	;
	; Check that appointments are not in ^BSDXAPPT
	N DFN,APPTTIME S (DFN,APPTTIME)=""
	F  S DFN=$O(BSDXAPPT(DFN)) Q:'DFN  D
	. F  S APPTTIME=$O(BSDXAPPT(DFN,APPTTIME)) Q:'APPTTIME  D
	. . I $D(^BSDXAPPT("APAT",DFN,APPTTIME)) W "Appt for "_DFN_" @ "_APPTTIME_" present",!
	;
	; Now, copy those appointments using BSDX29 to ^BSDXAPPT
	N FIRSTDATE S FIRSTDATE=$O(BSDXDATE(""))
	N LASTDATE S LASTDATE=$O(BSDXDATE(""),-1)
	N ZZZ ; garbage
	D BSDXCP^BSDX29(.ZZZ,RESIEN,HLIEN,FIRSTDATE,LASTDATE)
	I +^BSDXTMP($J,1)=0 W "Error... task not created",! QUIT
	;
	W "Waiting for 5 seconds for it to finish",! HANG 5
	N DFN,APPTTIME S (DFN,APPTTIME)=""
	F  S DFN=$O(BSDXAPPT(DFN)) Q:'DFN  D
	. F  S APPTTIME=$O(BSDXAPPT(DFN,APPTTIME)) Q:'APPTTIME  D
	. . I '$D(^BSDXAPPT("APAT",DFN,APPTTIME)) W "Appt for "_DFN_" @ "_APPTTIME_" missing",!
	;
	; Do all of this again making sure that events execute.
	K BSDXNOEV
	;
	; Create a bunch of appointments in PIMS (25 actually)
	N DFN
	N BSDXAPPT,BSDXDATE
	N BSDXI
	F BSDXI=1:1:5 D
	. N APPTTIME S APPTTIME=$$TIMEHL^BSDXUT(HLIEN) ; appt time
	. F DFN=1,2,3,4,5 D
	. . N % S %=$$MAKE1^BSDXAPI(DFN,HLIEN,3,APPTTIME,15,"Sam Test Appt"_DFN)
	. . I % W "Error in $$MAKE1^BSDXAPI for TIME "_APPTTIME_" for DFN "_DFN,!,%,!
	. . E  S BSDXAPPT(DFN,APPTTIME)="",BSDXDATE(APPTTIME)=""
	;
	; Check that appointments are in ^BSDXAPPT (different from last time)
	N DFN,APPTTIME S (DFN,APPTTIME)=""
	F  S DFN=$O(BSDXAPPT(DFN)) Q:'DFN  D
	. F  S APPTTIME=$O(BSDXAPPT(DFN,APPTTIME)) Q:'APPTTIME  D
	. . I '$D(^BSDXAPPT("APAT",DFN,APPTTIME)) W "Appt for "_DFN_" @ "_APPTTIME_" present",!
	;
	; Now, copy those appointments using BSDX29 to ^BSDXAPPT
	N FIRSTDATE S FIRSTDATE=$O(BSDXDATE(""))
	N LASTDATE S LASTDATE=$O(BSDXDATE(""),-1)
	N ZZZ ; garbage
	D BSDXCP^BSDX29(.ZZZ,RESIEN,HLIEN,FIRSTDATE,LASTDATE)
	I +^BSDXTMP($J,1)=0 W "Error... task not created",! QUIT
	;
	W "Waiting for 5 seconds for it to finish",! HANG 5
	W ^BSDXTMP("BSDXCOPY",+^BSDXTMP($J,1)),!
	W "Last line should say 0",!
	QUIT
