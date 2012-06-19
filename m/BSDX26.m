BSDX26	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 6/18/12 5:33pm
	   ;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	   ; Licensed under LGPL
	   ; Change History:
	   ; 3101023 - UJO/SMH - Addition of restartable transaction; relocation of tx.
	   ; --> Thanks to Zach Gonzalez and Rick Marshall
	   ; 3101205 - UJO/SMH - Extensive refactoring.
	   ;
	   ; Error Reference:
	   ; -1: Appt ID is not a number
	   ; -2: Appt IEN is not in ^BSDXAPPT
	   ; -3: FM Failure to file WP field in ^BSDXAPPT
	   ;
EDITAPTD(BSDXY,BSDXAPTID,BSDXNOTE)	 ;EP
	   ;Entry point for debugging
	   ;
	   D DEBUG^%Serenji("EDITAPT^BSDX26(.BSDXY,BSDXAPTID,BSDXNOTE)")
	   Q
UT	; Unit Tests
	   ; Test 1: Make sure this damn thing works
	   N ZZZ
	   N %H S %H=$H
	   N NOTE S NOTE="New Note "_%H
	   D EDITAPT(.ZZZ,188,NOTE)
	   I ^BSDXAPPT(188,1,1,0)'=NOTE W "ERROR",! B
	   ; Test 2: Test Errors -1 and -2
	   N ZZZ
	   N NOTE S NOTE="Nothing important"
	   D EDITAPT(.ZZZ,"BLAHBLAH",NOTE)
	   I +^BSDXTMP($J,1)'=-1 W "ERROR IN -1",! B
	   D EDITAPT(.ZZZ,298734322,NOTE)
	   I +^BSDXTMP($J,1)'=-2 W "ERROR IN -2",! B
	   ; Test 4: M Error
	   N bsdxdie S bsdxdie=1
	   D EDITAPT(.ZZZ,188,NOTE)
	   I +^BSDXTMP($J,1)'=-100 W "ERROR IN -100",! B
	   k bsdxdie
	   ; Test 5: Trestart
	   N bsdxrestart S bsdxrestart=1
	   N %H S %H=$H
	   N NOTE S NOTE="New Note "_%H
	   D EDITAPT(.ZZZ,188,NOTE)
	   I ^BSDXAPPT(188,1,1,0)'=NOTE W "ERROR in TRESTART",! B
	   ; Test 6: for Hosp Location Update
	   N DATE S DATE=$$NOW^XLFDT()
	   S DATE=$E(DATE,1,12) ; Just get minutes b/c of HL file input transform
	   D APPADD^BSDX07(.ZZZ,DATE,DATE+.001,3,"Dr Office",30,"Old Note",1)
	   N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	   D EDITAPT(.ZZZ,APPID,"New Note")
	   I ^BSDXAPPT(APTID,1,1,0)'="New Note" W "Error in HL Section",! B
	   I $P(^SC(2,"S",DATE,1,1,0),U,4)'="New Note" W "Error in HL Section",! B
	   QUIT
	   ;
EDITAPT(BSDXY,BSDXAPTID,BSDXNOTE)	  ;EP Edit appointment (only note text can be edited)
	   ; Called by RPC: BSDX EDIT APPOINTMENT
	   ;
	   ; Edits Appointment Text in BSDX APPOINTMENT file & Hosp Location (44) file
	   ;
	   ; Parameters:
	   ; - BSDXY: Global Return (RPC must be set to Global Array)
	   ; - BSDXAPTID: Appointment IEN in BSDX APPOINTMENT
	   ; - BSDXNOTE: New note
	   ;
	   ; Return:
	   ; ADO.net Recordset having 1 field: ERRORID
	   ; If Okay: -1; otherwise, positive integer with message
	   ;
	   ; Return Array; set Return and clear array
	   S BSDXY=$NA(^BSDXTMP($J))
	   K ^BSDXTMP($J)
	   ; ET
	   N $ET S $ET="G ETRAP^BSDX26"
	   ; Set up basic DUZ variables
	   D ^XBKVAR
	   ; Counter
	   N BSDXI S BSDXI=0
	   ; Header Node
	   S ^BSDXTMP($J,BSDXI)="T00100ERRORID"_$C(30)
	   ; Restartable txn for GT.M. Restored vars are Params + BSDXI.
	   TSTART (BSDXY,BSDXAPTID,BSDXNOTE,BSDXI):T="BSDX EDIT APPOINTMENT^BSDX26"
	   ;
	   ;;;test for error inside transaction. See if %ZTER works
	   I $G(bsdxdie) S X=1/0
	   ;;;test
	   ;;;test for TRESTART
	   I $G(bsdxrestart) K bsdxrestart TRESTART
	   ;;;test
	   ;
	   ; Validate Appointment ID
	   I '+BSDXAPTID D ERR(BSDXI,"-1~BSDX26: Invalid Appointment ID") QUIT
	   I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(BSDXI,"-2~BSDX26: Invalid Appointment ID") QUIT
	   ; Put the WP in decendant fields from the root to file as a WP field
	   S BSDXNOTE(.5)=BSDXNOTE,BSDXNOTE=""
	   I $D(BSDXNOTE(0)) S BSDXNOTE(.5)=BSDXNOTE(0) K BSDXNOTE(0)
	   N BSDXMSG ; Message in case of error in filing.
	   I $D(BSDXNOTE(.5)) D
	   . D WP^DIE(9002018.4,BSDXAPTID_",",1,"","BSDXNOTE","BSDXMSG")
	   I $D(BSDXMSG) D ERR(BSDXI,"-3~BSDX26: Fileman failure to file data into 9002018.4") QUIT
	   ;
	   ; Now file in file 44:
	   N PTIEN S PTIEN=$$GET1^DIQ(9002018.4,BSDXAPTID,".05","I") ; Patient IEN
	   N HLIEN S HLIEN=$$GET1^DIQ(9002018.4,BSDXAPTID,".07:.04","I") ; HL Location IEN pointed to by Resource ID
	   N DATE S DATE=+^BSDXAPPT(BSDXAPTID,0) ; Date of APPT
	   N BSDXRES S BSDXRES=0 ; Result
	   ; Update Note only if we have a linked hospital location.
	   I HLIEN S BSDXRES=$$UPDATENT^BSDXAPI(PTIEN,HLIEN,DATE,BSDXNOTE(.5))
	   ; If we get an error (denoted by -1 in BSDXRES), return error to client
	   I BSDXRES<0 D ERR(BSDXI,"-4~BSDX26: BSDXAPI reports an error: "_BSDXRES) QUIT
	   ;Return Recordset
	   TCOMMIT
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)="-1"_$C(30)
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   QUIT
	   ;
ERR(BSDXI,BSDXERR)	 ;Error processing
	   S BSDXI=BSDXI+1
	   S BSDXERR=$TR(BSDXERR,"^","~")
	   I $TL>0 TROLLBACK
	   S ^BSDXTMP($J,BSDXI)=BSDXERR_$C(30)
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   QUIT
	   ;
ETRAP	  ;EP Error trap entry
	   N $ET S $ET="D ^%ZTER HALT" ; Emergency Error Trap
	   I $TL>0 TROLLBACK
	   D ^%ZTER
	   S $EC=""
	   I '$D(BSDXI) N BSDXI S BSDXI=0
	   D ERR(BSDXI,"-100~BSDX26 Error: "_$G(%ZTERZE))
	   Q
