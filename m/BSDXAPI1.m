BSDXAPI1 ; VEN/SMH - SCHEDULING APIs - Continued!!! ; 6/27/12 4:45pm
	;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	; Licensed under LGPL  
	;
NOSHOW(PAT,CLINIC,DATE,NSFLAG) ; $$ PEP; No-show Patient at appt date (new in v1.7)
	; PAT = DFN
	; CLINIC = SC IEN
	; DATE = FM Date/Time of Appointment
	; NSFLAG = truthy value to add no-show, or falsy to remove (use 1 or 0 pls!)
	; 1^error for failure, 0 for success
	; Code follows EN1^SDN
	;
	; Check for failure conditions first before doing this. No globals set here
	N NOSHOWCK S NOSHOWCK=$$NOSHOWCK(PAT,CLINIC,DATE,NSFLAG)
	I NOSHOWCK Q NOSHOWCK
	;
	; Set up Protocol Driver
	N SDNSHDL,SDDA S SDNSHDL=$$HANDLE^SDAMEVT(1) S SDDA=$$SCIEN^BSDXAPI(PAT,CLINIC,DATE)
	N SDATA
	D BEFORE^SDAMEVT(.SDATA,PAT,DATE,CLINIC,SDDA,SDNSHDL) ; Only ^TMP set here.
	;
	; Simulated Errors
	Q:$D(BSDXSIMERR2) 1_U_"Simulated Error"
	;
	; Edit the ^DPT( "S" node entry - Noshow or undo noshow
	; Failure analysis: if we fail here, we presume no change happened in 
	; ^DPT(DA,"S", and so we just have to roll back ^BSDXAPPT
	N BSDXIENS S BSDXIENS=DATE_","_PAT_","
	N BSDXFDA
	I +NSFLAG D
	. S BSDXFDA(2.98,BSDXIENS,3)="N"
	. S BSDXFDA(2.98,BSDXIENS,14)=DUZ
	. S BSDXFDA(2.98,BSDXIENS,15)=$$NOW^XLFDT()
	E  D
	. S BSDXFDA(2.98,BSDXIENS,3)="@"
	. S BSDXFDA(2.98,BSDXIENS,14)="@"
	. S BSDXFDA(2.98,BSDXIENS,15)="@"
	N BSDXMSG
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	Q:$D(BSDXMSG) 1_U_"Fileman edit to DPT error: Patient="_PAT_" Appt="_DATE_" Error="_BSDXMSG("DIERR",1,"TEXT",1)
	; 
	; This M error trigger tests if ^BSDXAPPT rolls back. 
	; I won't try to roll back ^DPT(,"S"
	; The M error is caused here, so if I try to rollback, I can cause another
	; error. Infinite Errors then.
	I $D(BSDXSIMERR3) N X S X=1/0
	;
	; Run the event driver
	D NOSHOW^SDAMEVT(.SDATA,PAT,DATE,CLINIC,SDDA,0,SDNSHDL)
	Q 0
	;
NOSHOWCK(PAT,CLINIC,DATE,NSFLAG) ; $$ PEP; No-show Check
	; TODO: Not all appointments can be no showed.
	; Check the code in SDAMN 
	; S SDSTB=$$STATUS^SDAM1(DFN,SDT,SDCL,$G(^DPT(DFN,"S",SDT,0))) ; before status  
	; Q:'$$CHK ; Checks $D(^SD(409.63,"ANS",1,+SDSTB))
	QUIT 0
