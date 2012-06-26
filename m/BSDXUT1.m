BSDXUT1 ; VEN/SMH - Unit Tests for Scheduling GUI - cont. ; 6/25/12 4:13pm
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
	;
UT26	; Unit Tests - BSDX26
	;
	; Test 1: Make sure this damn thing works
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
	; Get start and end times
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	;
	; Make appt
	N ZZZ,DFN
	S DFN=3
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	;
	; Now edit the note - basic test
	N %H S %H=$H
	N NOTE S NOTE="New Note "_%H
	D EDITAPT^BSDX26(.ZZZ,APPID,NOTE)
	I ^BSDXAPPT(APPID,1,1,0)'=NOTE W "ERROR 1",!
	I $P(^SC(HLIEN,"S",APPTTIME,1,1,0),U,4)'=NOTE W "Error in HL Section",!
	;
	; Test 2: Test Error -1
	; -1 --> ApptID not a number
	N ZZZ
	N NOTE S NOTE="Nothing important"
	D EDITAPT^BSDX26(.ZZZ,"BLAHBLAH",NOTE)
	I +^BSDXTMP($J,1)'=-1 W "ERROR IN -1",!
	;
	; Test 3: Test Error -2
	; -2 --> ApptID not in ^BSDXAPPT
	D EDITAPT^BSDX26(.ZZZ,298734322,NOTE)
	I +^BSDXTMP($J,1)'=-2 W "ERROR IN -2",!
	;
	; Test 4: M Error
	N BSDXDIE S BSDXDIE=1
	D EDITAPT^BSDX26(.ZZZ,188,NOTE)
	I +^BSDXTMP($J,1)'=-100 W "ERROR IN -100",!
	K BSDXDIE
	; Test 5: Trestart -- retired in v1.7
	;
	; Test 6: UTs for an unlinked resource (not linked to PIMS)
	N RESNAM S RESNAM="UTCLINICUL" ; Unlinked Clinic
	N RESIEN
	D
	. N $ET S $ET="D ^%ZTER B"
	. S RESIEN=$$UTCRRES^BSDXUT(RESNAM)
	. I RESIEN<0 S $EC=",U1," ; not supposed to happen - hard crash if so
	;
	; Get start and end times
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	;
	N ZZZ,DFN
	S DFN=3
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	; Now edit the note - basic test
	N %H S %H=$H
	N NOTE S NOTE="New Note "_%H
	D EDITAPT^BSDX26(.ZZZ,APPID,NOTE)
	I ^BSDXAPPT(APPID,1,1,0)'=NOTE W "ERROR 2",!
	;
	; Test 7: Simulated failure in BSDXAPI
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
	; Get start and end times
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	;
	; Make appt
	N ZZZ,DFN
	S DFN=3
	N ORIGNOTE S ORIGNOTE="Sam's Note"
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,ORIGNOTE,1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	;
	; Create the error condition
	N BSDXSIMERR1 S BSDXSIMERR1=1
	;
	; Try to edit the note. Should still be "Sam's Note"
	N %H S %H=$H
	N NOTE S NOTE="New Note "_%H
	D EDITAPT^BSDX26(.ZZZ,APPID,NOTE)
	I +^BSDXTMP($J,1)'=-4 W "Simulated error not triggered",!
	I ^BSDXAPPT(APPID,1,1,0)'=ORIGNOTE ZWRITE ^(*) W "ERROR 3",!
	I $P(^SC(HLIEN,"S",APPTTIME,1,1,0),U,4)'=ORIGNOTE W "ERROR 4",!
	QUIT
