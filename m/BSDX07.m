BSDX07	; VW/UJO/SMH - WINDOWS SCHEDULING RPCS  ; 7/9/12 4:02pm
	;;1.7T1;BSDX;;Jul 06, 2012;Build 18
	; Licensed under LGPL
	;
	; Change Log:
	; UJO/SMH
	; v1.3 July 13 2010 - Add support i18n - Dates input as FM dates, not US.
	; v1.42 Oct 22 2010 - Transaction now restartable by providing arguments
	; v1.42 Oct 30 2010 - Extensive refactoring.
	; v1.5  Mar 15 2011 - End time does not have to have time anymore.
	;      It could be midnight of the next day
	; v1.6 Apr 11 2011 - Support for Scheduling Radiology Exams...
	; v1.7 Jun 20 2012 - Refactoring to remove transactions - many changes
	;                  - AVUPDT moved to AVUPDTMK in BSDXAPI1
	;
	; Error Reference:
	; -1: Patient Record is locked. This means something is wrong!!!!
	; -2: Start Time is not a valid Fileman date
	; -3: End Time is not a valid Fileman date
	; v1.5:obsolete::-4: End Time does not have time inside of it.
	; -5: BSDXPATID is not numeric
	; -6: Patient Does not exist in ^DPT
	; -7: Resource Name does not exist in B index of BSDX RESOURCE
	; -8: Resouce doesn't exist in ^BSDXRES
	; -9: Couldn't add appointment to BSDX APPOINTMENT
	; -10: Couldn't add appointment to files 2 and/or 44
	; -100: Mumps Error
	;
APPADDD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)	   ;EP
	;Entry point for debugging
	; D DEBUG^%Serenji("APPADD^BSDX07(.BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)")
	Q
	;
APPADD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID,BSDXRADEXAM)	;Private EP
	;
	;Called by RPC: BSDX ADD NEW APPOINTMENT
	;
	;Add new appointment to 3 files
	; - BSDX APPOINTMENT
	; - Hosp Location Appointment SubSubfile if Resource is linked to clinic
	; - Patient Appointment Subfile if Resource is linked to clinic
	;
	;Paramters:
	;BSDXY: Global Return (RPC must be set to Global Array)
	;BSDXSTART: FM Start Date
	;BSDXEND: FM End Date
	;BSDXPATID: Patient DFN
	;BSDXRES is ResourceName in BSDX RESOURCE file (not IEN)
	;BSDXLEN is the appointment duration in minutes
	;BSDXNOTE is the Appiontment Note
	;BSDXATID is used for 2 purposes:
	; if BSDXATID = "WALKIN" then BSDAPI is called to create a walkin appt.
	; if BSDXATID = a number, then it is the access type id (used for rebooking)
	;BSDXRADEXAM is used to store the Radiology Exam to which this appointment is tied to (optional)
	;
	;Return:
	; ADO.net Recordset having fields:
	; AppointmentID and ErrorNumber
	;
	; TODO: Specifying BSDXLEN and BSDXEND is redundant. For future programmers
	; to sort out. Needs changes on client.
	;
	;Test lines:
	;BSDX ADD NEW APPOINTMENT^3091122.0930^3091122.1000^370^Dr Office^30^EXAM^WALKIN
	;
	; Deal with optional arguments
	S BSDXRADEXAM=$G(BSDXRADEXAM)
	;
	; Return Array; set Return and clear array
	S BSDXY=$NA(^BSDXTMP($J))
	K ^BSDXTMP($J)
	;
	; $ET
	N $ET S $ET="G ETRAP^BSDX07"
	;
	; Counter
	N BSDXI S BSDXI=0
	;
	; Lock BSDX node, only to synchronize access to the globals.
	; It's not expected that the error will ever happen as no filing
	; is supposed to take 5 seconds.
	L +^BSDXPAT(BSDXPATID):5 I '$T D ERR(BSDXI,"-1~Patient record is locked. Please contact technical support.") Q
	;
	; Header Node
	S ^BSDXTMP($J,BSDXI)="I00020APPOINTMENTID^T00100ERRORID"_$C(30)
	;
	; Turn off SDAM APPT PROTOCOL BSDX Entries
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute BSDX ADD APPOINTMENT protocol
	;
	; Set Error Message to be empty
	N BSDXERR S BSDXERR=0
	;
	;;;test for error. See if %ZTER works
	I $G(BSDXDIE) N X S X=1/0
	;;;test
	;
	; -- Start and End Date Processing --
	; If C# sends the dates with extra zeros, remove them
	S BSDXSTART=+BSDXSTART,BSDXEND=+BSDXEND
	; Are the dates valid? Must be FM Dates > than 2010
	I BSDXSTART'>3100000 D ERR(BSDXI,"-2~BSDX07 Error: Invalid Start Time") Q
	I BSDXEND'>3100000 D ERR(BSDXI,"-3~BSDX07 Error: Invalid End Time") Q
	;
	;; If Ending date doesn't have a time, this is an error --rm 1.5
	; I $L(BSDXEND,".")=1 D ERR(BSDXI,"-4~BSDX07 Error: Invalid End Time") Q
	;
	; If the Start Date is greater than the end date, swap dates
	N BSDXTMP
	I BSDXSTART>BSDXEND S BSDXTMP=BSDXEND,BSDXEND=BSDXSTART,BSDXSTART=BSDXTMP
	;
	; Check if the patient exists:
	; - DFN valid number?
	; - Valid Patient in file 2?
	I '+BSDXPATID D ERR(BSDXI,"-5~BSDX07 Error: Invalid Patient ID") Q 
	I '$D(^DPT(BSDXPATID,0)) D ERR(BSDXI,"-6~BSDX07 Error: Invalid Patient ID") Q
	;
	;Validate Resource entry
	I '$D(^BSDXRES("B",BSDXRES)) D ERR(BSDXI,"-7~BSDX07 Error: Invalid Resource ID") Q
	N BSDXRESD ; Resource IEN
	S BSDXRESD=$O(^BSDXRES("B",BSDXRES,0))
	N BSDXRNOD ; Resouce zero node
	S BSDXRNOD=$G(^BSDXRES(BSDXRESD,0))
	I BSDXRNOD="" D ERR(BSDXI,"-8~BSDX07 Error: invalid Resource entry.") Q
	;
	; Walk-in (Unscheduled) Appointment?
	N BSDXWKIN S BSDXWKIN=0
	I BSDXATID="WALKIN" S BSDXWKIN=1
	; Reset Access Type ID if it doesn't say "WALKIN" and isn't a number
	I BSDXATID'?.N&(BSDXATID'="WALKIN") S BSDXATID=""
	;
	; Now, check if PIMS has any issues with us making the appt using MAKECK
	N BSDXSCD S BSDXSCD=$P(BSDXRNOD,U,4)  ; Hosp Location IEN
	N BSDXERR S BSDXERR=0 ; Variable to hold value of $$MAKE and $$MAKECK
	N BSDXC ; Array to send to MAKE and MAKECK APIs
	; Only if we have a valid Hosp Location
	I +BSDXSCD,$D(^SC(BSDXSCD,0)) D 
	. S BSDXC("PAT")=BSDXPATID
	. S BSDXC("CLN")=BSDXSCD
	. S BSDXC("TYP")=3 ;3 for scheduled appts, 4 for walkins
	. S:BSDXWKIN BSDXC("TYP")=4
	. S BSDXC("ADT")=BSDXSTART
	. S BSDXC("LEN")=BSDXLEN
	. S BSDXC("OI")=$E($G(BSDXNOTE),1,150) ;File 44 has 150 character limit on OTHER field
	. S BSDXC("OI")=$TR(BSDXC("OI"),";"," ") ;No semicolons allowed by MAKE^BSDXAPI
	. S BSDXC("OI")=$$STRIP(BSDXC("OI")) ;Strip control characters from note
	. S BSDXC("USR")=DUZ
	. S BSDXERR=$$MAKECK^BSDXAPI(.BSDXC)
	I BSDXERR D ERR(BSDXI,"-10~BSDX07 Error: MAKECK^BSDXAPI returned error code: "_BSDXERR) Q  ; no need for roll back
	;
	; Done with all checks, let's make appointment in BSDX APPOINTMENT
	N BSDXAPPTID
	S BSDXAPPTID=$$BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID,BSDXRADEXAM)
	I 'BSDXAPPTID D ERR(BSDXI,"-9~BSDX07 Error: Unable to add appointment to BSDX APPOINTMENT file.") Q  ; no roll back needed! No appts made.
	I BSDXNOTE]"" D BSDXWP(BSDXAPPTID,BSDXNOTE) ; no error checks are made here
	; I don't think it's important b/c users can detect right away if the WP
	; filing fails.
	;
	I $G(BSDXSIMERR1) D ERR(BSDXI,"-11~BSDX07 Error: Simulated Error"),ROLLBACK(BSDXAPPTID,.BSDXC) Q  ; UT Line
	;
	; Only if we have a valid Hosp Loc can we make an appointment in 2/44
	; Use BSDXC array from before.
	; FYI: $$MAKE itself calls $$MAKECK to check again for being okay.
	; If an error happens here, we roll back both ^BSDXAPPT and 2/44 by deleting
	N BSDXERR S BSDXERR=0 ; Variable to hold value of $$MAKE and $$MAKECK
	I +BSDXSCD,$D(^SC(BSDXSCD,0)) S BSDXERR=$$MAKE^BSDXAPI(.BSDXC)
	I BSDXERR D ERR(BSDXI,"-10~BSDX07 Error: MAKE^BSDXAPI returned error code: "_BSDXERR),ROLLBACK(BSDXAPPTID,.BSDXC) Q
	;
	; Unlock
	L -^BSDXPAT(BSDXPATID)
	;
	;Return Recordset
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXAPPTID_"^"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
STRIP(BSDXZ)	   ;Replace control characters with spaces
	N BSDXI
	F BSDXI=1:1:$L(BSDXZ) I (32>$A($E(BSDXZ,BSDXI))) S BSDXZ=$E(BSDXZ,1,BSDXI-1)_" "_$E(BSDXZ,BSDXI+1,999)
	Q BSDXZ
	;
BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID,BSDXRADEXAM)	 ;ADD BSDX APPOINTMENT ENTRY
	;Returns ien in BSDXAPPT or 0 if failed
	;Create entry in BSDX APPOINTMENT
	N BSDXAPPTID,BSDXFDA
	S BSDXFDA(9002018.4,"+1,",.01)=BSDXSTART
	S BSDXFDA(9002018.4,"+1,",.02)=BSDXEND
	S BSDXFDA(9002018.4,"+1,",.05)=BSDXPATID
	S BSDXFDA(9002018.4,"+1,",.07)=BSDXRESD
	S BSDXFDA(9002018.4,"+1,",.08)=$G(DUZ)
	S BSDXFDA(9002018.4,"+1,",.09)=$$NOW^XLFDT
	S:BSDXATID="WALKIN" BSDXFDA(9002018.4,"+1,",.13)="y"
	S:BSDXATID?.N BSDXFDA(9002018.4,"+1,",.06)=BSDXATID
	S BSDXFDA(9002018.4,"+1,",.14)=$G(BSDXRADEXAM)
	N BSDXIEN,BSDXMSG
	D UPDATE^DIE("","BSDXFDA","BSDXIEN","BSDXMSG")
	S BSDXAPPTID=+$G(BSDXIEN(1))
	Q BSDXAPPTID
	;
BSDXWP(BSDXAPPTID,BSDXNOTE)	;
	;Add WP field
	N BSDXMSG
	I BSDXNOTE]"" S BSDXNOTE(.5)=BSDXNOTE,BSDXNOTE=""
	I $D(BSDXNOTE(0)) S BSDXNOTE(.5)=BSDXNOTE(0) K BSDXNOTE(0)
	I $D(BSDXNOTE(.5)) D
	. D WP^DIE(9002018.4,BSDXAPPTID_",",1,"","BSDXNOTE","BSDXMSG")
	Q
	;
ADDEVT(BSDXPATID,BSDXSTART,BSDXSC,BSDXSCDA)	;EP
	;Called by BSDX ADD APPOINTMENT protocol
	;BSDXSC=IEN of clinic in ^SC
	;BSDXSCDA=IEN for ^SC(BSDXSC,"S",BSDXSTART,1,BSDXSCDA). Use to get Length & Note
	;
	N BSDXNOD,BSDXLEN,BSDXAPPTID,BSDXNODP,BSDXWKIN,BSDXRES,BSDXNOTE,BSDXEND
	Q:+$G(BSDXNOEV)
	I $D(^BSDXRES("ALOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ALOC",BSDXSC,0))
	E  I $D(^BSDXRES("ASSOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ASSOC",BSDXSC,0))
	Q:'+$G(BSDXRES)
	S BSDXNOD=$G(^SC(BSDXSC,"S",BSDXSTART,1,BSDXSCDA,0))
	Q:BSDXNOD=""
	S BSDXNODP=$G(^DPT(BSDXPATID,"S",BSDXSTART,0))
	S BSDXWKIN=""
	S:$P(BSDXNODP,U,7)=4 BSDXWKIN="WALKIN" ;Purpose of Visit field of DPT Appointment subfile
	S BSDXLEN=$P(BSDXNOD,U,2)
	Q:'+BSDXLEN
	S BSDXEND=$$FMADD^XLFDT(BSDXSTART,0,0,BSDXLEN,0)
	S BSDXAPPTID=$$BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXWKIN)
	Q:'+BSDXAPPTID
	S BSDXNOTE=$P(BSDXNOD,U,4)
	I BSDXNOTE]"" D BSDXWP(BSDXAPPTID,BSDXNOTE)
	D ADDEVT3(BSDXRES)
	Q
	;
ADDEVT3(BSDXRES)	   ;
	;Call RaiseEvent to notify GUI clients
	N BSDXRESN
	S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	Q:BSDXRESN=""
	S BSDXRESN=$P(BSDXRESN,"^")
	;D EVENT^BSDX23("SCHEDULE-"_BSDXRESN,"","","")
	D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	Q
	;
ROLLBACK(BSDXAPPTID,BSDXC)	; Private EP; Roll back appointment set
	; DO NOT USE except as an emergency measure - only if unforseen error occurs
	; Input: 
	; Appointment ID to remove from ^BSDXAPPT
	; BSDXC array (see array format in $$MAKE^BSDXAPI)
	N %
	D BSDXDEL^BSDX07(BSDXAPPTID)
	S:$D(BSDXC) %=$$UNMAKE^BSDXAPI(.BSDXC) ; rtn value always 0
	QUIT
	;
BSDXDEL(BSDXAPPTID)	;Private EP ; Deletes appointment BSDXAPPTID from ^BSDXAPPT
	; DO NOT USE except in emergencies to roll back an appointment set
	N DA,DIK
	S DIK="^BSDXAPPT(",DA=BSDXAPPTID
	D ^DIK
	Q
	;
ERR(BSDXI,BSDXERR)	 ;Error processing - different from error trap.
	; Unlock first
	L -^BSDXPAT(BSDXPATID)
	; If last line is $C(31), we are done. No more errors to send to client.
	I ^BSDXTMP($J,$O(^BSDXTMP($J," "),-1))=$C(31) QUIT
	S BSDXI=BSDXI+1
	S BSDXERR=$TR(BSDXERR,"^","~")
	S ^BSDXTMP($J,BSDXI)="0^"_BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
ETRAP	  ;EP Error trap entry
	N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	D ^%ZTER
	;
	I +$G(BSDXAPPTID) D ROLLBACK(BSDXAPPTID,.BSDXC) ; Rollback if BSDXAPPTID exists
	;
	; Log error message and send to client
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(BSDXI,"-100~BSDX07 Error: "_$G(%ZTERZE))
	Q:$Q 1_U_"Mumps Error" Q
	;
