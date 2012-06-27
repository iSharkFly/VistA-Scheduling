BSDX08	; VW/UJO/SMH - WINDOWS SCHEDULING RPCS ; 6/26/12 10:49am
	;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	; 
	; Original by HMW. New Written by Sam Habiel. Licensed under LGPL.
	; 
	; Change History
	; 3101022 UJO/SMH v1.42
	;  - Transaction work. As of v 1.7, all work here has been superceded
	;  - Refactoring of AVUPDT - never tested though.
	;  - Refactored all of APPDEL.
	; 
	; 3111125 UJO/SMH v1.5
	;  - Added ability to remove checked in appointments. Added a couple
	;    of units tests for that under UT2.
	; 
	; 3120625 VEN/SMH v1.7
	;  - Transactions removed. Code refactored to work w/o txns.
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
	;  -10^BSDX08: $$BSDXCAN failed (Fileman filing error)
	;  -100~BSDX08 Error: (Mumps Error)
	;
APPDELD(BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT)	;EP
	;Entry point for debugging
	;D DEBUG^%Serenji("APPDEL^BSDX08(.BSDXY,BSDXAPTID,BSDXTYP,BSDXCR,BSDXNOT)")
	Q
	;
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
	;
	; Header Node
	S ^BSDXTMP($J,BSDXI)="T00100ERRORID"_$C(30)
	;
	; Lock BSDX node, only to synchronize access to the globals.
	; It's not expected that the error will ever happen as no filing
	; is supposed to take 5 seconds.
	L +^BSDXAPPT(BSDXAPTID):5 I '$T D ERR(BSDXI,"-1~BSDX08: Appt record is locked. Please contact technical support.") Q
	;
	; Turn off SDAM APPT PROTOCOL BSDX Entries
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute BSDX CANCEL APPOINTMENT protocol
	;
	;;;test for error inside transaction. See if %ZTER works
	I $G(BSDXDIE1) N X S X=1/0
	;
	; Check appointment ID and whether it exists
	I '+BSDXAPTID D ERR(BSDXI,"-2~BSDX08: Invalid Appointment ID") Q
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(BSDXI,"-3~BSDX08: Invalid Appointment ID") Q
	; 
	; Start Processing:
	; First, get data
	N BSDXNOD S BSDXNOD=^BSDXAPPT(BSDXAPTID,0) ; BSDX Appt Node
	N BSDXPATID S BSDXPATID=$P(BSDXNOD,U,5) ; Patient ID
	N BSDXSTART S BSDXSTART=$P(BSDXNOD,U) ; Start Time
	;
	; Check the resource ID and whether it exists
	N BSDXSC1 S BSDXSC1=$P(BSDXNOD,U,7) ;RESOURCEID
	; If the resource id doesn't exist...
	I BSDXSC1="" D ERR(BSDXI,"-4~BSDX08: Cancelled appointment does not have a Resouce ID") QUIT
	I '$D(^BSDXRES(BSDXSC1,0)) D ERR(BSDXI,"-5~BSDX08: Resouce ID does not exist in BSDX RESOURCE") QUIT
	;
	;
	; Check if PIMS will let us cancel the appointment using $$CANCELCK^BSDXAPI
	; Get zero node of resouce
	N BSDXNOD S BSDXNOD=^BSDXRES(BSDXSC1,0)
	; Get Hosp location
	N BSDXLOC S BSDXLOC=$P(BSDXNOD,U,4)
	; Error indicator
	N BSDXERR S BSDXERR=0
	; 
	N BSDXC ; Array to pass to BSDXAPI
	;
	I BSDXLOC D 
	. S BSDXC("PAT")=BSDXPATID
	. S BSDXC("CLN")=BSDXLOC
	. S BSDXC("TYP")=BSDXTYP
	. S BSDXC("ADT")=BSDXSTART
	. S BSDXC("CDT")=$$NOW^XLFDT()
	. S BSDXC("NOT")=BSDXNOT
	. S:'+$G(BSDXCR) BSDXCR=11 ;Other
	. S BSDXC("CR")=BSDXCR
	. S BSDXC("USR")=DUZ
	. ;
	. S BSDXERR=$$CANCELCK^BSDXAPI(.BSDXC) ; 0 or 1^error message
	; If error, quit. No need to rollback as no changes took place.
	I BSDXERR D ERR(BSDXI,"-9~BSDX08: BSDXAPI reports that "_$P(BSDXERR,U,2)) QUIT
	;
	I $G(BSDXDIE2) N X S X=1/0
	;
	; Now cancel the appointment for real
	; BSDXAPPT First; no need for rollback if error occured.
	N BSDXERR S BSDXERR=$$BSDXCAN(BSDXAPTID)  ; Add a cancellation date in BSDX APPOINTMENT
	I BSDXERR D ERR(BSDXI,"$$BSDXCAN failed (Fileman filing error): "_$P(BSDXERR,U,2)) QUIT
	;
	; Then PIMS: 
	; cancel appt in "S" nodes in file 2 and 44, then update Legacy PIMS Availability
	; If error happens, must rollback ^BSDXAPPT
	I BSDXLOC D  QUIT:BSDXERR
	. N BSDXLEN S BSDXLEN=$$APPLEN^BSDXAPI(BSDXPATID,BSDXLOC,BSDXSTART) ; appt length
	. S BSDXERR=$$CANCEL^BSDXAPI(.BSDXC) ; Cancel through BSDXAPI
	. ; Rollback BSDXAPPT if error occurs
	. ; TODO: If an M error occurs in BSDXAPI, ETRAP gets called, ^BSDXTMP is
	. ;       populated, then the output of $$CANCEL is the output of ETRAP.
	. ;       Then, we see that BSDXERR is true, and we do another write,
	. ;       which deletes the information we had in ^BSDXTMP. What to do???
	. I BSDXERR D ERR(BSDXI,"-9^BSDX08: BSDXAPI returned an error: "_$P(BSDXERR,U,2)),ROLLBACK(BSDXAPTID)  QUIT
	. ;
	. ; Update Legacy PIMS clinic Availability ; no failure expected here.
	. D AVUPDT(BSDXLOC,BSDXSTART,BSDXLEN)
	;
	;
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
BSDXCAN(BSDXAPTID)	; $$; Private; Cancel BSDX APPOINTMENT entry
	; Input: Appt IEN in ^BSDXAPPT
	; Output: 0 for success and 1^Msg for failure
	N BSDXDATE,BSDXIENS,BSDXFDA,BSDXMSG
	S BSDXDATE=$$NOW^XLFDT()
	S BSDXIENS=BSDXAPTID_","
	S BSDXFDA(9002018.4,BSDXIENS,.12)=BSDXDATE
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	I $D(BSDXMSG) Q 1_U_BSDXMSG("DIERR",1,"TEXT",1)
	QUIT 0
	;
ROLLBACK(BSDXAPTID)  ; Proc; Private; Rollback cancellation
	; Input same as $$BSDXCAN
	N BSDXIENS S BSDXIENS=BSDXAPTID_","
	N BSDXFDA S BSDXFDA(9002018.4,BSDXIENS,.12)="@"
	N BSDXMSG
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	;I $D(BSDXMSG)  ; Not sure what to do. We are already handling an error.
	QUIT
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
	. N BSDXNOD
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
	S ^BSDXTMP($J,BSDXI)=BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	L -^BSDXAPPT(BSDXAPTID)
	QUIT
	;
ETRAP	;EP Error trap entry
	N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	D ^%ZTER
	S $EC=""  ; Clear Error
	; Roll back BSDXAPPT; 
	; TODO: What if a Mumps error happens in fileman in BSDXAPI? The Scheduling files can potentially be out of sync
	D:$G(BSDXAPTID) ROLLBACK(BSDXAPTID)
	; Log error message and send to client
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(BSDXI,"-100~BSDX08 Error: "_$G(%ZTERZE))
	Q:$Q 1_U_"-100~Mumps Error" Q
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
