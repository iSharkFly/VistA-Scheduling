BSDXUT	; VEN/SMH - Unit Tests for Scheduling GUI ; 6/29/12 12:20pm
	;;1.7;BSDX;;Oct 04, 2012;Build 25
	; Licensed under LGPL
	;
	; Change Log:
	; June 21 2012: Initial Version
	;
EN	; Run all Unit Tests
	D UT07
	QUIT
UT07	; Unit Tests for BSDX07 - Assumes you have Patients with DFNs 1,2,3,4,5
	; HLs/Resources are created as part of the UT
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
	N ZZZ,DFN
	; Test for normality:
	S DFN=3
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	; Does Appt exist?
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-1" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-2"
	I '$D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-3"
	I '$$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error Making Appt-4"
	;
	; Do it again for a different patient
	S DFN=2
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-5" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-6"
	I '$D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-7"
	I '$$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error Making Appt-8"
	;
	; Again for a different patient (4)
	S DFN=4
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-9" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-10"
	I '$D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-11"
	I '$$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error Making Appt-12"
	;
	; Delete appointment set for Patient 4 (made above)
	N BSDX,DFN
	S DFN=4
	S BSDX("PAT")=DFN
	S BSDX("CLN")=HLIEN
	S BSDX("ADT")=APPTTIME
	D ROLLBACK^BSDX07(APPID,.BSDX)
	I +$G(^BSDXAPPT(APPID,0)) W "Error in deleting appointment-1",!
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error in deleting appointment-2",!
	I $$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error in deleting appointment-3",!
	;
	; Again for a different patient (5)
	S DFN=5
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-13" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-14"
	I '$D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-15"
	I '$$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error Making Appt-16"
	; Now cancel that appointment
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",1,"Sam's Cancel Note")
	; Now make it again
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-17" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-18"
	I '$D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-19"
	I '$$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error Making Appt-20"
	;
	; Delete appointment set for Patient 1 (not made)... needs to not crash
	D
	. N $ET S $ET="D ^%ZTER S $EC="""" W ""Failure to del non-existent appt"",!"
	. N BSDX
	. S BSDX("PAT")=1
	. S BSDX("CLN")=HLIEN
	. S BSDX("ADT")=APPTTIME
	. D ROLLBACK^BSDX07(APPID,.BSDX)
	;
	; Test for bad start date
	D APPADD^BSDX07(.ZZZ,2100123,3100123.3,2,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-2 W "Error in -2",!
	; Test for bad end date
	D APPADD^BSDX07(.ZZZ,3100123,2100123.3,2,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-3 W "Error in -3",!
	; Test for end date without time - obsolete
	; Test for mumps error
	N BSDXDIE S BSDXDIE=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,1,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-100 W "Error in -100: M Error",!
	K BSDXDIE
	; Test for TRESTART -- retired in v 1.7
	; Test for non-numeric patient
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,"CAT,DOG",RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-5 W "Error in -5",!
	; Test for a non-existent patient
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,8989898989,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-6 W "Error in -6",!
	; Test for a non-existent resource name
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,3,"lkajsflkjsadf",30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-7 W "Error in -7",!
	; Test for corrupted resource
	; Can't test for -8 since it requires DB corruption
	; Test for inability to add appointment to BSDX Appointment (-9)
	; Also requires something wrong in the DB
	; Test for inability to add appointment to 2,44
	; Test by creating a duplicate appointment
	; Get start and end times
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,3,RESNAM,30,"Sam's Note",1)
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,3,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-10 W "Error in -10",!
	;
	; Test that ROLLBACK^BSDX07 occurs properly in various places
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	S DFN=4
	N BSDXSIMERR1 S BSDXSIMERR1=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I +APPID W "Error in deleting appointment-4",!
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error in deleting appointment-5",!
	I $$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error in deleting appointment-6",!
	;
	K BSDXSIMERR1
	N BSDXSIMERR2 S BSDXSIMERR2=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I +APPID W "Error in deleting appointment-7",!
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error in deleting appointment-8",!
	I $$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error in deleting appointment-9",!
	;
	K BSDXSIMERR2
	N BSDXSIMERR4 S BSDXSIMERR4=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I +APPID W "Error in deleting appointment-16",!
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error in deleting appointment-17",!
	I $$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error in deleting appointment-18",!
	;
	K BSDXSIMERR4
	N BSDXSIMERR5 S BSDXSIMERR5=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I +APPID W "Error in deleting appointment-19",!
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error in deleting appointment-20",!
	I $$SCIEN^BSDXAPI(DFN,HLIEN,APPTTIME) W "Error in deleting appointment-21",!
	;
	; Okay now we do UTs for an unlinked resource (not linked to PIMS)
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
	; Test for normality:
	S DFN=3
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	; Does Appt exist?
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-101" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-102"
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-103"
	;
	; Again for a different patient (4)
	S DFN=4
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	I 'APPID W "Error Making Appt-104" QUIT
	I +^BSDXAPPT(APPID,0)'=APPTTIME W "Error Making Appt-105"
	I $D(^DPT(DFN,"S",APPTTIME)) W "Error Making Appt-106"
	;
	; Delete appointment set for Patient 4 (made above)
	N BSDX,DFN
	S DFN=4
	D ROLLBACK^BSDX07(APPID)
	I +$G(^BSDXAPPT(APPID,0)) W "Error in deleting appointment-1",!
	;
	; Duplicate appointments... This is SUPPOSED to fail for now (v1.7)
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,3,RESNAM,30,"Sam's Note",1)
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,3,RESNAM,30,"Sam's Note",1)
	I +$P(^BSDXTMP($J,1),U,2)'=-10 W "Error in -10 in Unlinked Section (existing bug)",!
	;
	; Test that ROLLBACK^BSDX07 occurs properly in various places
	N TIMES S TIMES=$$TIMES^BSDXUT ; appt time^end time
	N APPTTIME S APPTTIME=$P(TIMES,U)
	N ENDTIME S ENDTIME=$P(TIMES,U,2)
	S DFN=4
	N BSDXSIMERR1 S BSDXSIMERR1=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I +APPID W "Error in deleting appointment-101",!
	;
	; These are never triggered, so we should still have an appointment
	K BSDXSIMERR1
	N BSDXSIMERR2 S BSDXSIMERR2=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I '+APPID W "Error in deleting appointment-102",!
	;
	K BSDXSIMERR2
	N BSDXSIMERR4 S BSDXSIMERR4=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I '+APPID W "Error in deleting appointment-103",!
	;
	K BSDXSIMERR4
	N BSDXSIMERR5 S BSDXSIMERR5=1
	D APPADD^BSDX07(.ZZZ,APPTTIME,ENDTIME,DFN,RESNAM,30,"Sam's Note",1)
	N APPID S APPID=$O(^BSDXAPPT("B",APPTTIME,""))
	I '+APPID W "Error in deleting appointment-104",!
	QUIT
	; 
UTCR(RESNAM)	; $$ - Create Unit Test Clinic and Resource Pair ; Private
	; Input: Resource Name By Value
	; Output: -1^Error or HLIEN^RESIEN for Success (file 44 IEN^file 9002018.1 IEN)
	; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
	N HLIEN S HLIEN=$$UTCR44(RESNAM)
	I +HLIEN=-1 QUIT HLIEN
	;
	N RESIEN S RESIEN=$$UTCRRES(RESNAM,HLIEN)
	I +RESIEN=-1 QUIT RESIEN
	E  QUIT HLIEN_U_RESIEN
	;
UTCR44(HLNAME)	; $$ - Create Unit Test Clinic in File 44; Private ; TESTING ONLY CODE
	; Output: -1^Error or IEN for Success
	; Input: Hosp Location Name by Value
	; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
	;
	I $D(^SC("B",HLNAME)) Q $O(^(HLNAME,""))
	;
	N SAM
	S SAM(44,"?+1,",.01)=HLNAME            ; Name
	S SAM(44,"?+1,",2)="C"                 ; Type = Clinic
	S SAM(44,"?+1,",2.1)=1                 ; Type Extension (not used)
	S SAM(44,"?+1,",3.5)=$O(^DG(40.8,0))   ; Division (not yet used)
	S SAM(44,"?+1,",8)=295                 ; Stop Code Number (not used)
	S SAM(44,"?+1,",9)="M"          ; Service (not used)
	S SAM(44,"?+1,",1912)=15               ; Length of Appt (not used)
	S SAM(44,"?+1,",1917)=4                ; Display increments per hour (not used)
	S SAM(44,"?+1,",1918)=8                ; Overbooks/day max (not used)
	S SAM(44,"?+1,",2000.5)=0              ; Require Action Profiles: Yes (not used)
	S SAM(44,"?+1,",2001)=999              ; Allowable consecutive no-shows (not used)
	S SAM(44,"?+1,",2002)=999              ; Max # days for Future Booking (not used)
	S SAM(44,"?+1,",2005)=365              ; Max # days for Auto Rebook (not used)
	S SAM(44,"?+1,",2502)="N"             ; Non-Count Clinic (not used)
	S SAM(44,"?+1,",2504)="Y"            ; Clinic meets at this Facility? (not used)
	S SAM(44,"?+1,",2507)=9              ; Appointment Type (not used)
	;
	N BSDXERR,BSDXIEN
	D UPDATE^DIE("",$NA(SAM),$NA(BSDXIEN),$NA(BSDXERR))
	Q $S($D(BSDXERR):-1_U_BSDXERR("DIERR",1,"TEXT",1),1:BSDXIEN(1))
	;
UTCRRES(NAME,HLIEN)	; $$ - Create Unit Test Resource in 9002018.1 (BSDX RESOURCE); Private
	; Input: Hospital Location IEN
	; Output: -1^Error or IEN for Success
	; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
	I $D(^BSDXRES("B",NAME)) Q $O(^(NAME,""))
	S HLIEN=$G(HLIEN) ; If we don't send one in
	N RES ; garbage variable
	D RSRC^BSDX16(.RES,"|"_NAME_"||"_HLIEN)
	N RTN S RTN=@$Q(^BSDXTMP($J,0)) ; return array next value
	Q $S(RTN=0:-1_U_RTN,1:+RTN) ; 0 means an error has occurred; 1 means IEN returned
	;
TIMES()	; $$ - Create a next available appointment time^ending time; Private
	; Output: appttime^endtime
	N NOW S NOW=$$NOW^XLFDT() ; Now time
	N LAST S LAST=$O(^BSDXAPPT("B"," "),-1) ; highest time in file
	N TIME2USE S TIME2USE=$S(NOW>LAST:NOW,1:LAST) ; Which time to use?
	S TIME2USE=$E(TIME2USE,1,12) ; Strip away seconds
	N APPTIME S APPTIME=$$FMADD^XLFDT(TIME2USE,0,0,15,0) ; Add 15 min
	N ENDTIME S ENDTIME=$$FMADD^XLFDT(APPTIME,0,0,15,0) ; Add 15 more min
	Q APPTIME_U_ENDTIME ; quit with apptime^endtime
	;
TIMEHL(HLIEN)	; $$ - Create a next available appointment time^ending time by HL; Private
	; Input: HLIEN
	; Output: Next available appointment time for the HLIEN
	N LAST S LAST=$O(^SC(HLIEN,"S",""),-1)
	Q $$FMADD^XLFDT(LAST,1,0,15,0) ; Add 1 day and 15 minutes
