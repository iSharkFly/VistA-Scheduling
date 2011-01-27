BSDX08	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 1/25/11 12:39pm
	;;1.5;BSDX;;Jan 25, 2011
	; 
	; Original by HMW. New Written by Sam Habiel. Licensed under LGPL.
	; 
	; Change History
	; 3101022 UJO/SMH v1.42
	;  - Transaction now restartable. Thanks to 
	;   --> Zach Gonzalez and Rick Marshall for fix.
	;  - Extra TROLLBACK in Lock Statement when lock fails.
	;   --> Removed--Rollback is already in ERR tag.
	;  - Added new statements to old SD code in AVUPDT to obviate
	;   --> need to restore variables in transaction
	;  - Refactored this chunk of code. Don't really know whether it 
	;   --> worked in the first place. Waiting for bug report to know.
	;  - Refactored all of APPDEL.
	; 
	; 3111125 UJO/SMH v1.5
	;  - Added ability to remove checked in appointments. Added a couple
	;    of units tests for that under UT2.
	;  - Minor reformatting because of how KIDS adds tabs.
	; 
	; Error Reference:
	;  -1~BSDX08: Appt record is locked. Please contact technical support.
	;  -2~BSDX08: Invalid Appointment ID
	;  -3~BSDX08: Invalid Appointment ID
	;  -4~BSDX08: Cancelled appointment does not have a Resouce ID  
	;  -5~BSDX08: Resouce ID does not exist in BSDX RESOURCE
	;  -6~BSDX08: Invalid Hosp Location stored in Database
	;  -7~BSDX08: Patient does not have an appointment in PIMS Clinic
	;  -8^BSDX08: Unable to find associated PIMS appointment for this patient
	;  -9^BSDX08: BSDXAPI returned an error: (error)
	;  -100~BSDX08 Error: (Mumps Error)
	;
APPDELD(BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT)	;EP
	;Entry point for debugging
	D DEBUG^%Serenji("APPDEL^BSDX08(.BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT)")
	Q
	;
UT	; Unit Tests
	; Test 1: Make normal appointment and cancel it. See if every thing works
	N ZZZ
	D APPADD^BSDX07(.ZZZ,3110123.2,3110123.3,4,"Dr Office",10,"Sam's Note",1)
	S APPID=+$P(^BSDXTMP($J,1),U)
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",1,"Sam's Cancel Note")
	I $P(^BSDXAPPT(APPID,0),U,12)'>0 W "Error in Cancellation-1"
	I $O(^SC(2,"S",3110123.2,1,0))]"" W "Error in Cancellation-2"
	I $P(^DPT(4,"S",3110123.2,0),U,2)'="PC" W "Error in Cancellation-3"
	I ^DPT(4,"S",3110123.2,"R")'="Sam's Cancel Note" W "Error in Cancellation-4"
	;
	; Test 2: Check for -1
	; Make appt
	D APPADD^BSDX07(.ZZZ,3110125.2,3110125.3,4,"Dr Office",10,"Sam's Note",1)
	; Lock the node in another job
	S APPID=+$P(^BSDXTMP($J,1),U)
	; W "Lock ^BSDXAPPT("_APPID_") in another session. You have 10 seconds." H 10
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",1,"Sam's Cancel Note")
	;
	; Test 3: Check for -100
	S bsdxdie=1
	D APPADD^BSDX07(.ZZZ,3110126.2,3110126.3,4,"Dr Office",10,"Sam's Note",1)
	S APPID=+$P(^BSDXTMP($J,1),U)
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",1,"Reasons")
	I $P(^BSDXTMP($J,1),"~")'=-100 W "Error in -100",!
	K bsdxdie
	;
	; Test 4: Restartable transaction
	S bsdxrestart=1
	D APPADD^BSDX07(.ZZZ,3110128.2,3110128.3,4,"Dr Office",10,"Sam's Note",1)
	S APPID=+$P(^BSDXTMP($J,1),U)
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",1,"Reasons")
	I $P(^DPT(4,"S",3110128.2,0),U,2)'="PC" W "Error in Restartable Transaction",!
	;
	; Test 5: for invalid Appointment ID (-2 and -3)
	D APPDEL^BSDX08(.ZZZ,0,"PC",1,"Reasons")
	I $P(^BSDXTMP($J,1),"~")'=-2 W "Error in -2",!
	D APPDEL^BSDX08(.ZZZ,999999,"PC",1,"Reasons")
	I $P(^BSDXTMP($J,1),"~")'=-3 W "Error in -3",!
UT2	; More unit Tests
	;
	; Test 6: for Cancelling walkin and checked-in appointments 
	S BSDXSTART=$E($$NOW^XLFDT,1,12),BSDXEND=BSDXSTART+.0001
	D APPADD^BSDX07(.ZZZ,BSDXSTART,BSDXEND,4,"Dr Office",10,"Sam's Note",1) ; Add appt
	S APPID=+$P(^BSDXTMP($J,1),U)
	I APPID=0 W "Error in test 6",!
	D CHECKIN^BSDX25(.ZZZ,APPID,$$NOW^XLFDT) ; check-in
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",10,"Cancel Note") ; Delete appt
	I $P(^BSDXTMP($J,1),$C(30))'="" W "Error in test 6",!
	;
	; Test 7: for cancelling walkin and checked-in appointments
	S BSDXSTART=$E($$NOW^XLFDT,1,12)+.0001,BSDXEND=BSDXSTART+.0001
	D APPADD^BSDX07(.ZZZ,BSDXSTART,BSDXEND,4,"Dr Office",10,"Sam's Note",1) ; Add appt
	S APPID=+$P(^BSDXTMP($J,1),U)
	I APPID=0 W "Error in test 6",!
	D CHECKIN^BSDX25(.ZZZ,APPID,$$NOW^XLFDT) ; Checkin
	S BSDXRES=$O(^BSDXRES("B","Dr Office",""))
	S BSDXCLN=$P(^BSDXRES(BSDXRES,0),U,4)
	S BSDXRESULT=$$RMCI^BSDXAPI(4,BSDXCLN,BSDXSTART) ; remove checkin
	D APPDEL^BSDX08(.ZZZ,APPID,"PC",10,"Cancel Note") ; delete appt
	I $P(^BSDXTMP($J,1),$C(30))'="" W "Error in test 6",!
	QUIT
APPDEL(BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT)	       ;EP
	;Called by RPC: BSDX CANCEL APPOINTMENT
	;Cancels existing appointment in BSDX APPOINTMENT and 44/2 subfiles
	;Input Parameters:
	; - BSDXAPTID is entry number in BSDX APPOINTMENT file
	; - BSDXTYP is C for clinic-cancelled and PC for patient cancelled
	; - BSDXCR is pointer to CANCELLATION REASON File (409.2)
	; - BSDXNOT is user note
	;
	; Returns error code in recordset field ERRORID. Empty string is success.
	; Returns Global Array. Must use this type in RPC.
	;
	; Return Array: set Return and clear array
	S BSDXY=$NA(^BSDXTMP($J))
	K ^BSDXTMP($J)
	;
	; Set min DUZ vars if they don't exist
	D ^XBKVAR
	;
	; $ET
	N $ET S $ET="G ETRAP^BSDX08"
	;
	; Counter
	N BSDXI S BSDXI=0
	; Header Node
	S ^BSDXTMP($J,BSDXI)="T00100ERRORID"_$C(30)
	;
	; Lock BSDX node, only to synchronize access to the globals.
	; It's not expected that the error will ever happen as no filing
	; is supposed to take 5 seconds.
	L +^BSDXAPPT(BSDXAPTID):5 I '$T D ERR(BSDXI,"-1~BSDX08: Appt record is locked. Please contact technical support.") Q
	;
	;Restartable Transaction; restore paramters when starting.
	; (Params restored are what's passed here + BSDXI)
	TSTART (BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT,BSDXI):T="BSDX CANCEL APPOINTEMENT^BSDX08"
	;
	; Turn off SDAM APPT PROTOCOL BSDX Entries
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute BSDX CANCEL APPOINTMENT protocol
	;
	;;;test for error inside transaction. See if %ZTER works
	I $G(bsdxdie) S X=1/0
	;;;test
	;;;test for TRESTART
	I $G(bsdxrestart) K bsdxrestart TRESTART
	;;;test
	;
	; Check appointment ID and whether it exists
	I '+BSDXAPTID D ERR(BSDXI,"-2~BSDX08: Invalid Appointment ID") Q
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(BSDXI,"-3~BSDX08: Invalid Appointment ID") Q
	;
	; Start Processing:
	; First, add cancellation date to appt entry in BSDX APPOINTMENT
	N BSDXNOD S BSDXNOD=^BSDXAPPT(BSDXAPTID,0) ; BSDX Appt Node
	N BSDXPATID S BSDXPATID=$P(BSDXNOD,U,5) ; Patient ID
	N BSDXSTART S BSDXSTART=$P(BSDXNOD,U) ; Start Time
	D BSDXCAN(BSDXAPTID)  ; Add a cancellation date in BSDX APPOINTMENT
	;
	; Second, cancel appt in "S" nodes in file 2 and 44, then update Legacy PIMS Availability
	N BSDXSC1 S BSDXSC1=$P(BSDXNOD,U,7) ;RESOURCEID
	; If the resouce id doesn't exist...
	I BSDXSC1="" D ERR(BSDXI,"-4~BSDX08: Cancelled appointment does not have a Resouce ID") QUIT
	I '$D(^BSDXRES(BSDXSC1,0)) D ERR(BSDXI,"-5~BSDX08: Resouce ID does not exist in BSDX RESOURCE") QUIT
	; Get zero node of resouce
	S BSDXNOD=^BSDXRES(BSDXSC1,0)
	; Get Hosp location
	N BSDXLOC S BSDXLOC=$P(BSDXNOD,U,4)
	; Error indicator for Hosp Location filing for getting out of routine
	N BSDXERR S BSDXERR=0
	; Only file in 2/44 if there is an associated hospital location
	I BSDXLOC D  QUIT:BSDXERR  
	. I '$D(^SC(BSDXLOC,0)) S BSDXERR=1 D ERR(BSDXI,"-6~BSDX08: Invalid Hosp Location stored in Database") QUIT
	. ; Get the IEN of the appointment in the "S" node of ^SC
	. N BSDXSCIEN
	. S BSDXSCIEN=$$SCIEN^BSDXAPI(BSDXPATID,BSDXLOC,BSDXSTART)
	. I BSDXSCIEN="" S BSDXERR=1 D ERR(BSDXI,"-7~BSDX08: Patient does not have an appointment in PIMS Clinic") QUIT
	. ; Get the appointment node
	. S BSDXNOD=$G(^SC(BSDXLOC,"S",BSDXSTART,1,BSDXSCIEN,0))
	. I BSDXNOD="" S BSDXERR=1 D ERR(BSDXI,"-8^BSDX08: Unable to find associated PIMS appointment for this patient") QUIT
	. N BSDXLEN S BSDXLEN=$P(BSDXNOD,U,2)
	. ; Cancel through BSDXAPI
	. N BSDXZ
	. D APCAN(.BSDXZ,BSDXLOC,BSDXPATID,BSDXSTART)
	. I +BSDXZ>0 S BSDXERR=1 D ERR(BSDXI,"-9^BSDX08: BSDXAPI returned an error: "_$P(BSDXZ,U,2)) QUIT
	. ; Update Legacy PIMS clinic Availability
	. D AVUPDT(BSDXLOC,BSDXSTART,BSDXLEN)
	;
	TCOMMIT
	L -^BSDXAPPT(BSDXAPTID)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=""_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
AVUPDT(BSDXSCD,BSDXSTART,BSDXLEN)	;Update Legacy PIMS Clinic availability
	;See SDCNP0
	N SD,S  ; Start Date
	S (SD,S)=BSDXSTART 
	N I ; Clinic IEN in 44
	S I=BSDXSCD
	; if day has no schedule in legacy PIMS, forget about this update.
	Q:'$D(^SC(I,"ST",SD\1,1))
	N SL ; Clinic characteristics node (length of appt, when appts start etc)
	S SL=^SC(I,"SL")
	N X ; Hour Clinic Display Begins
	S X=$P(SL,U,3)
	N STARTDAY ; When does the day start?
	S STARTDAY=$S($L(X):X,1:8) ; If defined, use it; otherwise, 8am
	N SB ; ?? Who knows? Day Start - 1 divided by 100.
	S SB=STARTDAY-1/100
	S X=$P(SL,U,6) ; Now X is Display increments per hour
	N HSI ; Slots per hour, try 1
	S HSI=$S(X:X,1:4) ; if defined, use it; otherwise, 4
	N SI ; Slots per hour, try 2
	S SI=$S(X="":4,X<3:4,X:X,1:4) ; If slots "", or less than 3, then 4
	N STR ; ??
	S STR="#@!$* XXWVUTSRQPONMLKJIHGFEDCBA0123456789jklmnopqrstuvwxyz"
	N SDDIF ; Slots per hour diff??
	S SDDIF=$S(HSI<3:8/HSI,1:2)
	S SL=BSDXLEN ; Dammit, reusing variable; SL now Appt Length from GUI
	S S=^SC(I,"ST",SD\1,1) ; reusing var again; S now Day Pattern from PIMS
	N Y ; Hours since start of Date
	S Y=SD#1-SB*100 ;SD#1=FM Time portion; -SB minus start of day; conv to hrs
	N ST  ; ??
	; Y#1 -> Minutes; *SI -> * Slots per hour; \.6 trunc min to hour
	; Y\1 -> Hours since start of day; * SI: * slots
	S ST=Y#1*SI\.6+(Y\1*SI) 
	N SS ; how many slots are supposed to be taken by appointment
	S SS=SL*HSI/60 ; (nb: try SL: 30 min; HSI: 4 slots)
	N I
	I Y'<1 D  ; If Hours since start of Date is greater than 1
	. ; loop through pattern. Tired of documenting.
	. F I=ST+ST:SDDIF D  Q:Y=""  Q:SS'>0
	. . S Y=$E(STR,$F(STR,$E(S,I+1))) Q:Y=""  
	. . S S=$E(S,1,I)_Y_$E(S,I+2,999)
	. . S SS=SS-1 
	. . Q:SS'>0
	S ^SC(BSDXSCD,"ST",SD\1,1)=S  ; new pattern; global set
	Q
	;
APCAN(BSDXZ,BSDXLOC,BSDXDFN,BSDXSD)	        ;
	;Cancel appointment for patient BSDXDFN in clinic BSDXSC1
	;at time BSDXSD
	N BSDXC,%H
	S BSDXC("PAT")=BSDXPATID
	S BSDXC("CLN")=BSDXLOC
	S BSDXC("TYP")=BSDXTYP
	S BSDXC("ADT")=BSDXSD
	S %H=$H D YMD^%DTC
	S BSDXC("CDT")=X+%
	S BSDXC("NOT")=BSDXNOT
	S:'+$G(BSDXCR) BSDXCR=11 ;Other
	S BSDXC("CR")=BSDXCR
	S BSDXC("USR")=DUZ
	;
	S BSDXZ=$$CANCEL^BSDXAPI(.BSDXC)
	Q
	;
BSDXCAN(BSDXAPTID)	;
	;Cancel BSDX APPOINTMENT entry
	N %DT,X,BSDXDATE,Y,BSDXIENS,BSDXFDA,BSDXMSG
	S %DT="XT",X="NOW" D ^%DT ; X ^DD("DD")
	S BSDXDATE=Y
	S BSDXIENS=BSDXAPTID_","
	S BSDXFDA(9002018.4,BSDXIENS,.12)=BSDXDATE
	K BSDXMSG
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	Q
	;
CANEVT(BSDXPAT,BSDXSTART,BSDXSC)	;EP Called by BSDX CANCEL APPOINTMENT event
	;when appointments cancelled via PIMS interface.
	;Propagates cancellation to BSDXAPPT and raises refresh event to running GUI clients
	N BSDXFOUND,BSDXRES
	Q:+$G(BSDXNOEV)
	Q:'+$G(BSDXSC)
	S BSDXFOUND=0
	I $D(^BSDXRES("ALOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ALOC",BSDXSC,0)) S BSDXFOUND=$$CANEVT1(BSDXRES,BSDXSTART,BSDXPAT)
	I BSDXFOUND D CANEVT3(BSDXRES) Q
	I $D(^BXDXRES("ASSOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ASSOC",BSDXSC,0)) S BSDXFOUND=$$CANEVT1(BSDXRES,BSDXSTART,BSDXPAT)
	I BSDXFOUND D CANEVT3(BSDXRES)
	Q
	;
CANEVT1(BSDXRES,BSDXSTART,BSDXPAT)	;
	;Get appointment id in BSDXAPT
	;If found, call BSDXCAN(BSDXAPPT) and return 1
	;else return 0
	N BSDXFOUND,BSDXAPPT
	S BSDXFOUND=0
	Q:'+BSDXRES BSDXFOUND
	Q:'$D(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART)) BSDXFOUND
	S BSDXAPPT=0 F  S BSDXAPPT=$O(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART,BSDXAPPT)) Q:'+BSDXAPPT  D  Q:BSDXFOUND
	. S BSDXNOD=$G(^BSDXAPPT(BSDXAPPT,0)) Q:BSDXNOD=""
	. I $P(BSDXNOD,U,5)=BSDXPAT,$P(BSDXNOD,U,12)="" S BSDXFOUND=1 Q
	I BSDXFOUND,+$G(BSDXAPPT) D BSDXCAN(BSDXAPPT)
	Q BSDXFOUND
	;
CANEVT3(BSDXRES)	;
	;Call RaiseEvent to notify GUI clients
	;
	N BSDXRESN
	S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	Q:BSDXRESN=""
	S BSDXRESN=$P(BSDXRESN,"^")
	;D EVENT^BSDX23("SCHEDULE-"_BSDXRESN,"","","")
	D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	Q
	;
ERR(BSDXI,BSDXERR)	;Error processing
	S BSDXI=BSDXI+1
	S BSDXERR=$TR(BSDXERR,"^","~")
	I $TL>0 TROLLBACK
	S ^BSDXTMP($J,BSDXI)=BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	L -^BSDXAPPT(BSDXAPTID)
	QUIT
	;
ETRAP	;EP Error trap entry
	N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	; Rollback, otherwise ^XTER will be empty from future rollback
	I $TL>0 TROLLBACK 
	D ^%ZTER
	S $EC=""  ; Clear Error
	; Log error message and send to client
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(BSDXI,"-100~BSDX08 Error: "_$G(%ZTERZE))
	QUIT
	;
	;;;NB: This is code that is unused in both original and port.
	; ; If not appt in the "S" node is found in ^SC then check associated RPMS Clinic Multiple
	; I BSDXSCIEN="" D  I 'BSDXZ Q  ;Q:BSDXZ
	; . S BSDXERR="BSDX08: Unable to find associated RPMS appointment for this patient. "
	; . S BSDXZ=1
	; . ; Check if there are associated RPMS clinics. (not currently used) Does the multiple exist? No, then quit
	; . I '$D(^BSDXRES(BSDXSC1,20)) S BSDXZ=0 QUIT
	; . ; Loop through the multiple. Get Location and then the ^SC "S" node IEN.
	; . N BSDX1 S BSDX1=0
	; . F  S BSDX1=$O(^BSDXRES(BSDXSC1,20,BSDX1)) Q:'+BSDX1  Q:BSDXZ=0  D
	; . . Q:'$D(^BSDXRES(BSDXSC1,20,BSDX1,0))
	; . . S BSDXLOC=$P(^BSDXRES(BSDXSC1,20,BSDX1,0),U)
	; . . S BSDXSCIEN=$$SCIEN^BSDXAPI(BSDXPATID,BSDXLOC,BSDXSTART) I +BSDXSCIEN S BSDXZ=0 Q
