BSDX31	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 12/6/10 12:39pm
	   ;;1.5V3;BSDX;;Mar 16, 2011
	   ; Change Log:
	   ; v1.42 Oct 23 2010 WV/SMH
	   ; - Change transaction to restartable. Thanks to Zach Gonzalez
	   ; --> and Rick Marshall for their help.
	   ; v1.42 Dec 6 2010: Extensive refactoring
	   ; 
	   ; Error Reference:
	   ; -1: zero or null Appt ID
	   ; -2: Invalid APPT ID (doesn't exist in ^BSDXAPPT)
	   ; -3: No-show flag is invalid
	   ; -4: Filing of No-show in ^BSDXAPPT failed
	   ; -5: Filing of No-show in ^DPT failed (BSDXAPI error)
	   ; -100: M Error
	   ;
	   ;
NOSHOWD(BSDXY,BSDXAPTID,BSDXNS)	;EP
	   ;Entry point for debugging
	   ;
	   D DEBUG^%Serenji("NOSHOW^BSDX31(.BSDXY,BSDXAPTID,BSDXNS)")
	   Q
	   ;
UT	; Unit Tests
	   ; Test 1: Sanity Check
	   N ZZZ ; Garbage return variable
	   N DATE S DATE=$$NOW^XLFDT()
	   S DATE=$E(DATE,1,12) ; Just get minutes b/c of HL file input transform
	   D APPADD^BSDX07(.ZZZ,DATE,DATE+.0001,3,"Dr Office",30,"Old Note",1)
	   N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	   D NOSHOW(.ZZZ,APPID,1)
	   I $P(^BSDXAPPT(APPID,0),U,10)'=1 W "ERROR T1",! B
	   I $P(^DPT(3,"S",DATE,0),U,2)'="N" W "ERROR T1",! B
	   ; Test 2: Undo noshow
	   D NOSHOW(.ZZZ,APPID,0)
	   I $P(^BSDXAPPT(APPID,0),U,10)'="0" W "ERROR T2",! B
	   I $P(^DPT(3,"S",DATE,0),U,2)'="" W "ERROR T2",! B
	   ; Test 3: -1
	   D NOSHOW(.ZZZ,"",0)
	   I $P(^BSDXTMP($J,1),U)'=-1 W "ERROR T3",! B
	   ; Test 4: -2
	   D NOSHOW(.ZZZ,2938748233,0)
	   I $P(^BSDXTMP($J,1),U)'=-2 W "ERROR T4",! B
	   ; Test 5: -3
	   D NOSHOW(.ZZZ,APPID,3)
	   I $P(^BSDXTMP($J,1),U)'=-3 W "ERROR T5",! B
	   ; Test 6: Mumps error (-100)
	   s bsdxdie=1
	   D NOSHOW(.ZZZ,APPID,1)
	   I $P(^BSDXTMP($J,1),U)'=-100 W "ERROR T6",! B
	   k bsdxdie
	   ; Test 7: Restartable transaction
	   s bsdxrestart=1
	   D NOSHOW(.ZZZ,APPID,1)
	   I $P(^BSDXAPPT(APPID,0),U,10)'=1 W "ERROR T7",! B
	   QUIT
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
	   ; $ET
	   N $ET S $ET="G ETRAP^BSDX31"
	   ; Basline vars
	   D ^XBKVAR  ; Set up baseline variables (DUZ, DUZ(2)) if they don't exist
	   ; Counter
	   N BSDXI S BSDXI=0
	   ; Header Node
	   S ^BSDXTMP($J,BSDXI)="I00100ERRORID^T00030ERRORTEXT"_$C(30)
	   ; Begin transaction
	   TSTART (BSDXI,BSDXY,BSDXAPTID,BSDXNS):T="BSDX NOSHOW CANCEL^BSDX29"
	   ;;;test for error inside transaction. See if %ZTER works
	   I $G(bsdxdie) S X=1/0
	   ;;;TEST
	   ;;;test for TRESTART
	   I $G(bsdxrestart) K bsdxrestart TRESTART
	   ;;;test
	   ; Turn off SDAM APPT PROTOCOL BSDX Entries
	   N BSDXNOEV S BSDXNOEV=1 ;Don't execute protocol
	   ; Appointment ID check
	   I '+BSDXAPTID D ERR(-1,"BSDX31: Invalid Appointment ID") Q
	   I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(-2,"BSDX31: Invalid Appointment ID") Q
	   ; Noshow value check - Must be 1 or 0
	   S BSDXNS=+BSDXNS
	   I BSDXNS'=1&(BSDXNS'=0) D ERR(-3,"BSDX31: Invalid No Show value") Q
	   ; Get Some data
	   N BSDXNOD S BSDXNOD=^BSDXAPPT(BSDXAPTID,0) ; Node
	   N BSDXPATID S BSDXPATID=$P(BSDXNOD,U,5) ; DFN
	   N BSDXSTART S BSDXSTART=$P(BSDXNOD,U)  ; Start Date/Time
	   ; Edit BSDX APPOINTMENT entry
	   N BSDXMSG  ; 
	   D BSDXNOS(BSDXAPTID,BSDXNS,.BSDXMSG)  ;Edit BSDX APPOINTMENT entry NOSHOW field 
	   I $D(BSDXMSG("DIERR")) S BSDXMSG=$G(BSDXMSG("DIERR",1,"TEXT",1)) D ERR(-4,"BSDX31: "_BSDXMSG) Q
	   ; Edit File 2 "S" node entry
	   N BSDXZ,BSDXERR ; Error variables to control looping
	   S BSDXSC1=$P(BSDXNOD,U,7) ;RESOURCEID
	   ; If Resource ID exists, and HL exists (means that Resource is linked), No show in File 2
	   I BSDXSC1]"",$D(^BSDXRES(BSDXSC1,0)) D  I $G(BSDXZ)]"" S BSDXERR="BSDX31: APNOSHO Returned: "_BSDXZ D ERR(-5,BSDXERR) Q
	   . S BSDXNOD=^BSDXRES(BSDXSC1,0)
	   . S BSDXSC1=$P(BSDXNOD,U,4) ;HOSPITAL LOCATION
	   . I BSDXSC1]"",$D(^SC(BSDXSC1,0)) D APNOSHO(.BSDXZ,BSDXSC1,BSDXPATID,BSDXSTART,BSDXNS)
	   ;
	   TCOMMIT
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)="1^"_$C(30) ; 1 means everything okay
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   QUIT
	   ;
APNOSHO(BSDXZ,BSDXSC1,BSDXDFN,BSDXSD,BSDXNS)	           ;
	   ; update file 2 info
	   ;Set noshow for patient BSDXDFN in clinic BSDXSC1
	   ;at time BSDXSD
	   N BSDXC,%H,BSDXCDT,BSDXIEN
	   N BSDXIENS,BSDXFDA,BSDXMSG
	   S %H=$H D YMD^%DTC
	   S BSDXCDT=X+%
	   ;
	   S BSDXIENS=BSDXSD_","_BSDXDFN_","
	   I +BSDXNS D
	   . S BSDXFDA(2.98,BSDXIENS,3)="N"
	   . S BSDXFDA(2.98,BSDXIENS,14)=DUZ
	   . S BSDXFDA(2.98,BSDXIENS,15)=BSDXCDT
	   E  D
	   . S BSDXFDA(2.98,BSDXIENS,3)=""
	   . S BSDXFDA(2.98,BSDXIENS,14)=""
	   . S BSDXFDA(2.98,BSDXIENS,15)=""
	   K BSDXIEN
	   D UPDATE^DIE("","BSDXFDA","BSDXIEN","BSDXMSG")
	   S BSDXZ=$G(BSDXMSG("DIERR",1,"TEXT",1))
	   Q
	   ;
BSDXNOS(BSDXAPTID,BSDXNS,BSDXMSG)	  ;
	   ;
	   N BSDXFDA,BSDXIENS
	   S BSDXIENS=BSDXAPTID_","
	   S BSDXFDA(9002018.4,BSDXIENS,.1)=BSDXNS ;NOSHOW
	   D FILE^DIE("","BSDXFDA","BSDXMSG")
	   QUIT
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
	   N BSDXFOUND,BSDXAPPT
	   S BSDXFOUND=0
	   Q:'+$G(BSDXRES) BSDXFOUND
	   Q:'$D(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART)) BSDXFOUND
	   S BSDXAPPT=0 F  S BSDXAPPT=$O(^BSDXAPPT("ARSRC",BSDXRES,BSDXSTART,BSDXAPPT)) Q:'+BSDXAPPT  D  Q:BSDXFOUND
	   . S BSDXNOD=$G(^BSDXAPPT(BSDXAPPT,0)) Q:BSDXNOD=""
	   . I $P(BSDXNOD,U,5)=BSDXPAT,$P(BSDXNOD,U,12)="" S BSDXFOUND=1 Q
	   I BSDXFOUND,+$G(BSDXAPPT) D BSDXNOS(BSDXAPPT,BSDXSTAT)
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
	   S BSDXI=BSDXI+1
	   S ERRTXT=$TR(ERRTXT,"^","~")
	   I $TL>0 TROLLBACK
	   S ^BSDXTMP($J,BSDXI)=BSDXERID_"^"_ERRTXT_$C(30)
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   QUIT
	   ;
ETRAP	  ;EP Error trap entry
	   N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	   ; Rollback, otherwise ^XTER will be empty from future rollback
	   I $TL>0 TROLLBACK
	   D ^%ZTER
	   S $EC="" ; Clear Error
	   ; Send to client
	   I '$D(BSDXI) N BSDXI S BSDXI=0
	   D ERR(-100,"BSDX31 Error: "_$G(%ZTERZE))
	   QUIT
	   ;
IMHERE(BSDXRES)	;EP
	   ;Entry point for BSDX IM HERE remote procedure
	   S BSDXRES=1
	   Q
	   ;
