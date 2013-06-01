BSDX26	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 7/10/12 10:27am
	;;1.7;BSDX;;Jun 01, 2013;Build 24
	; Licensed under LGPL
	; Change History:
	; 3101023 - UJO/SMH - Addition of restartable transaction; relocation of tx.
	; 3101205 - UJO/SMH - Extensive refactoring.
	; 3120625 - VEN/SMH - Removal of Transactions, reloation of UTs to BSDXUT1
	;
	; Error Reference:
	; 1: Appt ID is not a number
	; 2: Appt IEN is not in ^BSDXAPPT
	; 3: FM Failure to file WP field in ^BSDXAPPT
	; 4: BSDXAPI reports failure to change note field in ^SC
	; 5: Failure to acquire lock on ^BSDXAPPT(APPTID)
	; 100: Mumps Error
	; 
	; NB: Normally I use negative numbers for errors; this routine returns
	;     -1 as a successful result! So I needed to use +ve numbers.
	;
EDITAPTD(BSDXY,BSDXAPTID,BSDXNOTE)	 ;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("EDITAPT^BSDX26(.BSDXY,BSDXAPTID,BSDXNOTE)")
	Q
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
	;
	;;;test for error. See if %ZTER works
	I $G(BSDXDIE) S X=1/0
	;
	; Validate Appointment ID
	I '+BSDXAPTID D ERR(BSDXI,"1~BSDX26: Invalid Appointment ID") QUIT
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(BSDXI,"2~BSDX26: Invalid Appointment ID") QUIT
	;
	; Lock BSDX node, only to synchronize access to the globals.
	; It's not expected that the error will ever happen as no filing
	; is supposed to take 5 seconds.
	L +^BSDXAPPT(BSDXAPTID):5 E  D ERR(BSDXI,"5~BSDX08: Appt record is locked. Please contact technical support.") QUIT
	;
	; Put the WP in decendant fields from the root to file as a WP field
	S BSDXNOTE(.5)=BSDXNOTE,BSDXNOTE=""
	I $D(BSDXNOTE(0)) S BSDXNOTE(.5)=BSDXNOTE(0) K BSDXNOTE(0)
	;
	N BSDXMSG ; Message in case of error in filing.
	;
	; Save Before State in case we need it for rollback
	K ^TMP($J)
	M ^TMP($J,"BEFORE","BSDXAPPT")=^BSDXAPPT(BSDXAPTID)
	;
	; Update note in BSDX APPOINTMENT
	I $D(BSDXNOTE(.5)) D
	. D WP^DIE(9002018.4,BSDXAPTID_",",1,"","BSDXNOTE","BSDXMSG")
	;
	; Error handling. No need for rollback since nothing else changed.
	I $D(BSDXMSG) D ERR(BSDXI,"3~BSDX26: Fileman failure to file data into 9002018.4") QUIT
	;
	; Now file in file 44:
	N PTIEN S PTIEN=$$GET1^DIQ(9002018.4,BSDXAPTID,".05","I") ; Patient IEN
	N HLIEN S HLIEN=$$GET1^DIQ(9002018.4,BSDXAPTID,".07:.04","I") ; HL Location IEN pointed to by Resource ID
	N DATE S DATE=+^BSDXAPPT(BSDXAPTID,0) ; Date of APPT
	N BSDXRES S BSDXRES=0 ; Result
	; Update Note only if we have a linked hospital location.
	I HLIEN S BSDXRES=$$UPDATENT^BSDXAPI1(PTIEN,HLIEN,DATE,BSDXNOTE(.5))
	; If we get an error (denoted by -1 in BSDXRES), return error to client
	; AND restore the original note
	I BSDXRES D ERR(BSDXI,"4~BSDX26: BSDXAPI reports an error: "_BSDXRES),ROLLBACK(BSDXAPTID) QUIT
	;
	;Return Recordset indicating success
	L -^BSDXAPPT(BSDXAPTID)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="-1"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	;
	K ^TMP($J) ; Done; remove TMP data
	QUIT
	;
ROLLBACK(BSDXAPTID)	; Rollback note to original in ^BSDXAPPT
	M ^BSDXAPPT(BSDXAPTID)=^TMP($J,"BEFORE","BSDXAPPT")
	K ^TMP($J)
	QUIT
	;
ERR(BSDXI,BSDXERR)	 ;Error processing
	; Unlock first
	L:$D(BSDXAPTID) -^BSDXAPPT(BSDXAPTID)
	; If last line is $C(31), we are done. No more errors to send to client.
	I ^BSDXTMP($J,$O(^BSDXTMP($J," "),-1))=$C(31) QUIT
	S BSDXI=BSDXI+1
	S BSDXERR=$TR(BSDXERR,"^","~")
	S ^BSDXTMP($J,BSDXI)=BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	QUIT
	;
ETRAP	  ;EP Error trap entry
	N $ET S $ET="D ^%ZTER HALT" ; Emergency Error Trap
	D ^%ZTER
	;
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(BSDXI,"100~BSDX26 Error: "_$G(%ZTERZE))
	QUIT
