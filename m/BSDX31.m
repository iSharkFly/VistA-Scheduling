BSDX31	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 6/27/12 4:57pm
	;;1.7T1;BSDX;;Jul 06, 2012;Build 18
	; Licensed under LGPL
	; Change Log:
	; v1.42 3101023 WV/SMH - Change transaction to restartable. 
	; v1.42 3101206 UJO/SMH - Extensive refactoring
	; v1.7  3120626 VEN/SMH - Removed transactions; extensive refactoring
	;                       - Moved APTNS (whatever it was) to BSDXAPI1 
	;                         as $$NOSHOW
	;                       - Made BSDXNOS extrinsic.
	;                       - Moved Unit Tests to BSDXUT1
	;                       - BSDXNOS deletes no-show rather than file 0 for
	;                         undoing a no show
	; 
	; Error Reference:
	; -1: zero or null Appt ID
	; -2: Invalid APPT ID (doesn't exist in ^BSDXAPPT)
	; -3: No-show flag is invalid
	; -4: Filing of No-show in ^BSDXAPPT failed
	; -5: Filing of No-show in ^DPT failed (BSDXAPI error)
	; -6: Invalid Resource ID
	; -100: M Error
	;
	;
NOSHOWD(BSDXY,BSDXAPTID,BSDXNS)	;EP
	;Entry point for debugging
	;
	; D DEBUG^%Serenji("NOSHOW^BSDX31(.BSDXY,BSDXAPTID,BSDXNS)")
	Q
	;
NOSHOW(BSDXY,BSDXAPTID,BSDXNS)	        ;EP - No show a patient
	; Called by RPC: BSDX NOSHOW
	; Sets appointment noshow flag in BSDX APPOINTMENT file and "S" node in File 2
	;
	; Parameters:
	; BSDXY: Global Return
	; BSDXAPTID is entry number in BSDX APPOINTMENT file
	; BSDXNS = 1: NOSHOW, 0: CANCEL NOSHO
	; 
	; Returns ADO.net record set with fields
	; - ERRORID; ERRORTEXT
	; ERRORID of 1 is okay
	; Anything else is an error.
	;
	; Return Array; set and clear
	S BSDXY=$NA(^BSDXTMP($J))
	K ^BSDXTMP($J)
	;
	; $ET
	N $ET S $ET="G ETRAP^BSDX31"
	;
	; Basline vars
	D ^XBKVAR  ; Set up baseline variables (DUZ, DUZ(2)) if they don't exist
	;
	; Counter
	N BSDXI S BSDXI=0
	;
	; Header Node
	S ^BSDXTMP($J,BSDXI)="I00100ERRORID^T00030ERRORTEXT"_$C(30)
	;
	;;;test for error. See if %ZTER works
	I $G(BSDXDIE) N X S X=1/0
	;;;TEST
	;
	; Turn off SDAM APPT PROTOCOL BSDX Entries
	N BSDXNOEV S BSDXNOEV=1 ;Don't execute protocol
	;
	; Appointment ID check
	I '+BSDXAPTID D ERR(-1,"BSDX31: Invalid Appointment ID") Q
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(-2,"BSDX31: Invalid Appointment ID") Q
	;
	; Noshow value check - Must be 1 or 0
	S BSDXNS=+BSDXNS
	I BSDXNS'=1&(BSDXNS'=0) D ERR(-3,"BSDX31: Invalid No Show value") Q
	;
	; Get Some data
	N BSDXNOD S BSDXNOD=^BSDXAPPT(BSDXAPTID,0) ; Node
	N BSDXPATID S BSDXPATID=$P(BSDXNOD,U,5) ; DFN
	N BSDXSTART S BSDXSTART=$P(BSDXNOD,U)  ; Start Date/Time
	N BSDXRES S BSDXRES=$P(BSDXNOD,U,7) ; Resource ID
	;
	; Check if Resource ID is missing or invalid
	I BSDXRES="" D ERR(-6,"BSDX31: Invalid Resource") QUIT
	I '$D(^BSDXRES(BSDXRES,0)) D ERR(-6,"BSDX31: Invalid Resource") QUIT
	;
	; Get the Hospital Location
	N BSDXRESNOD S BSDXRESNOD=^BSDXRES(BSDXRES,0)
	N BSDXLOC S BSDXLOC=$P(BSDXRESNOD,U,4) ;HOSPITAL LOCATION
	I BSDXLOC,'$D(^SC(BSDXLOC,0)) S BSDXLOC="" ; Unlink it if it doesn't exist
	; I can go and then delete it from ^BSDXRES like Mailman code which tries
	; to be too helpful... but I will postpone that until this is a need.
	;
	; Check if it's okay to no-show patient.
	N BSDXERR S BSDXERR=0 ; Error variable
	I BSDXLOC S BSDXERR=$$NOSHOWCK^BSDXAPI1(BSDXPATID,BSDXLOC,BSDXSTART,BSDXNS)
	I BSDXERR D ERR(-5,"BSDX31: "_$P(BSDXERR,U,2)) QUIT
	;
	; Simulated Error
	I $G(BSDXSIMERR1) D ERR(-4,"BSDX31: Simulated Error") QUIT
	; Edit BSDX APPOINTMENT entry No-show field
	; Failure Analysis: If we fail here, no rollback needed, as this is the 1st
	; call
	N BSDXMSG S BSDXMSG=$$BSDXNOS(BSDXAPTID,BSDXNS)
	I BSDXMSG D ERR(-4,"BSDX31: "_$P(BSDXMSG,U,2)) QUIT
	;
	; Edit File 2 "S" node entry
	; Failure Analysis: If we fail here, we need to rollback the BSDX
	; Apptointment Entry
	N BSDXERR S BSDXERR=0 ; Error variable
	; If HL exist, (resource is linked to PIMS), file no show in File 2
	I BSDXLOC S BSDXERR=$$NOSHOW^BSDXAPI1(BSDXPATID,BSDXLOC,BSDXSTART,BSDXNS)
	I BSDXERR D  QUIT
	. D ERR(-5,"BSDX31: "_$P(BSDXERR,U,2))
	. N % S %=$$BSDXNOS(BSDXAPTID,'BSDXNS) ; no error checking for filer
	;
	; Return data in ADO.net table
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="1^"_$C(30) ; 1 means everything okay
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	QUIT
	;
BSDXNOS(BSDXAPTID,BSDXNS)	; $$ Private; File/unfile noshow in ^BSDXAPPT
	; in v1.7 I delete the no-show value rather than file zero
	N BSDXFDA,BSDXIENS,BSDXMSG
	N BSDXVALUE ; What to file: 1 or delete it.
	I BSDXNS S BSDXVALUE=1
	E  S BSDXVALUE="@"
	S BSDXIENS=BSDXAPTID_","
	S BSDXFDA(9002018.4,BSDXIENS,.1)=BSDXVALUE ;NOSHOW 1 or 0
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	QUIT:$D(BSDXMSG) -1_U_BSDXMSG("DIERR",1,"TEXT",1)
	QUIT 0
	;
NOSEVT(BSDXPAT,BSDXSTART,BSDXSC)	   ;EP Called by BSDX NOSHOW APPOINTMENT event
	;when appointments NOSHOW via PIMS interface.
	;Propagates NOSHOW to BSDXAPPT and raises refresh event to running GUI clients
	;
	Q:+$G(BSDXNOEV)
	Q:'+$G(BSDXSC)
	Q:$G(SDATA("AFTER","STATUS"))["AUTO RE-BOOK"
	N BSDXSTAT,BSDXFOUND,BSDXRES
	S BSDXSTAT=1
	S:$G(SDATA("BEFORE","STATUS"))["NO-SHOW" BSDXSTAT=0
	S BSDXFOUND=0
	I $D(^BSDXRES("ALOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ALOC",BSDXSC,0)) S BSDXFOUND=$$NOSEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)
	I BSDXFOUND D NOSEVT3(BSDXRES) Q
	I $D(^BXDXRES("ASSOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ASSOC",BSDXSC,0)) S BSDXFOUND=$$NOSEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)
	I BSDXFOUND D NOSEVT3(BSDXRES)
	Q
	;
NOSEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)	;
	;Get appointment id in BSDXAPT
	;If found, call BSDXNOS(BSDXAPPT) and return 1
	;else return 0
	N BSDXFOUND,BSDXAPPT,BSDXNOD
	S BSDXFOUND=0
	Q:'+$G(BSDXRES) BSDXFOUND
	Q:'$D(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART)) BSDXFOUND
	S BSDXAPPT=0 F  S BSDXAPPT=$O(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART,BSDXAPPT)) Q:'+BSDXAPPT  D  Q:BSDXFOUND
	. S BSDXNOD=$G(^BSDXAPPT(BSDXAPPT,0)) Q:BSDXNOD=""
	. I $P(BSDXNOD,U,5)=BSDXPAT,$P(BSDXNOD,U,12)="" S BSDXFOUND=1 Q
	I BSDXFOUND,+$G(BSDXAPPT) N BSDXMSG S BSDXMSG=$$BSDXNOS(BSDXAPPT,BSDXSTAT)
	I BSDXMSG D ^%ZTER ; Last ditch error handling. This is supposed to be silently called from the protocol file.
	Q BSDXFOUND
	;
NOSEVT3(BSDXRES)	   ;
	;Call RaiseEvent to notify GUI clients
	;
	N BSDXRESN
	S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	Q:BSDXRESN=""
	S BSDXRESN=$P(BSDXRESN,"^")
	D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	Q
	;
	;
ERR(BSDXERID,ERRTXT)	   ;Error processing
	; If last line is $C(31), we are done. No more errors to send to client.
	I ^BSDXTMP($J,$O(^BSDXTMP($J," "),-1))=$C(31) QUIT
	S BSDXI=BSDXI+1
	S ERRTXT=$TR(ERRTXT,"^","~")
	S ^BSDXTMP($J,BSDXI)=BSDXERID_"^"_ERRTXT_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	QUIT
	;
ETRAP	  ;EP Error trap entry
	N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	D ^%ZTER
	S $EC="" ; Clear Error
	I $G(BSDXAPTID),$D(BSDXNS) N % S %=$$BSDXNOS(BSDXAPTID,'BSDXNS) ; Reverse No-Show status (whatever it was)
	; Send to client
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(-100,"BSDX31 Error: "_$G(%ZTERZE))
	Q:$Q 100_U_"Mumps Error" Q
	;
IMHERE(BSDXRES)	;EP
	;Entry point for BSDX IM HERE remote procedure
	S BSDXRES=1
	Q
	;
