BSDX26	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 6/25/12 4:29pm
	;;1.7T1;BSDX;;Jul 06, 2012;Build 18
	; Licensed under LGPL
	; Change History:
	; 3101023 - UJO/SMH - Addition of restartable transaction; relocation of tx.
	; 3101205 - UJO/SMH - Extensive refactoring.
	; 3120625 - VEN/SMH - Removal of Transactions, reloation of UTs to BSDXUT1
	;
	; Error Reference:
	; -1: Appt ID is not a number
	; -2: Appt IEN is not in ^BSDXAPPT
	; -3: FM Failure to file WP field in ^BSDXAPPT
	; -4: BSDXAPI reports failure to change note field in ^SC
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
	I '+BSDXAPTID D ERR(BSDXI,"-1~BSDX26: Invalid Appointment ID") QUIT
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR(BSDXI,"-2~BSDX26: Invalid Appointment ID") QUIT
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
	I $D(BSDXNOTE(.5)) D
	. D WP^DIE(9002018.4,BSDXAPTID_",",1,"","BSDXNOTE","BSDXMSG")
	;
	; Error handling. No need for rollback since nothing else changed.
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
	; AND restore the original note
	I BSDXRES<0 D ERR(BSDXI,"-4~BSDX26: BSDXAPI reports an error: "_BSDXRES),ROLLBACK(BSDXAPTID) QUIT
	;
	;Return Recordset indicating success
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
	S $EC=""
	I '$D(BSDXI) N BSDXI S BSDXI=0
	D ERR(BSDXI,"-100~BSDX26 Error: "_$G(%ZTERZE))
	QUIT
