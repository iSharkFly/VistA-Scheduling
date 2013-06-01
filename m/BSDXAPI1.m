BSDXAPI1	; VEN/SMH - SCHEDULING APIs - Continued!!! ; 7/9/12 2:22pm
	;;1.7;BSDX;;Jun 01, 2013;Build 24
	; Licensed under LGPL  
	;
	; Change History (BSDXAPI and BSDXAPI1)
	; Pre 1.42:
	; - Simplified entry points (MAKE1, CANCEL1, CHECKIN1)
	; 2010-11-5: (1.42)
	; - Fixed errors having to do uncanceling patient appointments if it was 
	;   a patient cancelled appointment.
	; - Use new style Fileman API for storing appointments in file 44 in 
	;   $$MAKE due to problems with legacy API.
	; 2010-11-12: (1.42)
	; - Changed ="C" to ["C" in SCIEN. Cancelled appointments can be "PC" as 
	;   well. 
	; 2010-12-5 (1.42)
	; Added an entry point to update the patient note in file 44.
	; 2010-12-6 (1.42)
	; MAKE1 incorrectly put info field in BSDR("INFO") rather than BSDR("OI")
	; 2010-12-8 (1.42)
	; Removed restriction on max appt length. Even though this restriction
	; exists in fileman (120 minutes), PIMS ignores it. Therefore, I 
	; will ignore it here too.
	; 2011-01-25 (v.1.5)
	; Added entry point $$RMCI to remove checked in appointments.
	; In $$CANCEL, if the appointment is checked in, delete check-in rather than
	;  spitting an error message to the user saying 'Delete the check-in'
	; Changed all lines that look like this:
	;  I $G(BSDR("ADT"))'?7N1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	; to:
	;  I $G(BSDR("ADT"))'?7N.1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	; to allow for date at midnight which does not have a dot at the end.
	; 2011-01-26 (v.1.5)
	; More user friendly message if patient already has appointment in $$MAKE:
	;  Spits out pt name and user friendly date.
	; 2012-06-18 (v 1.7)
	; Removing transacions. Means that code SHOULD NOT fail. Took all checks
	;  out for making an appointment to MAKECK. We call this first to make sure
	; that the appointment is okay to make before committing to make it. We
	; still have the provision to delete the data though if we fail when we 
	; actually make the appointment.
	; CANCELCK exists for the same purpose.
	; CHECKINK ditto
	; New API: $$NOSHOW^BSDXAPI1 for no-showing patients
	; Moved RMCI from BSDXAPI to BSDXAPI1 because BSDXAPI1 is getting larger
	;  than 20000 characters.
	; Added RMCICK (Remove check-in check)
	; Moved Availability update EPs in BSDX07 and BSDX08 b/c they really
	; belong to PIMS, not to the Scheduling GUI. $$MAKE and $$CANCEL now
	; call the EPs here.
	; Cancel and Remove-Check-in now check to see if the patient is checked-out
	; If the patient is checked out, then we fail to cancel/no-show.
	; UPDATENOTE was renamed to UPDATENT and moved to BSDXAPI1.
	;
NOSHOW(PAT,CLINIC,DATE,NSFLAG)	; $$ PEP; No-show Patient at appt date (new in v1.7)
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
	; I won't try to roll back ^DPT(,"S" because
	; the M error is caused here, so if I try to rollback, I can cause another
	; error. Infinite Errors then.
	I $D(BSDXSIMERR3) N X S X=1/0
	;
	; Run the event driver
	D NOSHOW^SDAMEVT(.SDATA,PAT,DATE,CLINIC,SDDA,0,SDNSHDL)
	Q 0
	;
NOSHOWCK(PAT,CLINIC,DATE,NSFLAG)	; $$ PEP; No-show Check
	; TODO: Not all appointments can be no showed.
	; Check the code in SDAMN 
	; S SDSTB=$$STATUS^SDAM1(DFN,SDT,SDCL,$G(^DPT(DFN,"S",SDT,0))) ; before status  
	; Q:'$$CHK ; Checks $D(^SD(409.63,"ANS",1,+SDSTB))
	QUIT 0
	;
RMCI(PAT,CLINIC,DATE)	 ;PEP; -- Remove Check-in; $$
	; PAT = DFN
	; CLINIC = SC IEN
	; DATE = FM Date/Time of Appointment
	;
	; Returns:
	; 0 if okay
	; -1 if failure
	;
	; Call like this: $$RMCI(233,33,3110102.1130)
	;
	; Check to see if we can remove the check-in
	N BSDXERR S BSDXERR=$$RMCICK(PAT,CLINIC,DATE)
	I BSDXERR Q BSDXERR
	;
	; Move my variables into the ones used by SDAPIs (just a convenience)
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL,SDMODE
	S DFN=PAT,SDT=DATE,SDCL=CLINIC,SDMODE=2,SDDA=$$SCIEN^BSDXAPI(DFN,SDCL,SDT)
	;
	I SDDA<1 QUIT 0    ; Appt cancelled; cancelled appts rm'ed from file 44
	;
	; remember before status
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; M Error Test - Simulate behavior when an M error occurs
	I $G(BSDXDIE2) N X S X=1/0
	; 
	; Simulate a failure to file the data in Fileman
	I $D(BSDXSIMERR3) Q 1_U_"Simulated Error"
	;
	; remove check-in using filer.
	N BSDXIENS S BSDXIENS=SDDA_","_DATE_","_CLINIC_","
	N BSDXFDA
	S BSDXFDA(44.003,BSDXIENS,309)="@" ; CHECKED-IN
	S BSDXFDA(44.003,BSDXIENS,302)="@" ; CHECK IN USER
	S BSDXFDA(44.003,BSDXIENS,305)="@" ; CHECK IN ENTERED
	N BSDXERR
	D FILE^DIE("","BSDXFDA","BSDXERR")
	I $D(BSDXERR) QUIT "-1~Can't file for Pat "_PAT_" in Clinic "_CLINIC_" at "_DATE_". Fileman reported an error: "_BSDXERR("DIERR",1,"TEXT",1)
	;
	; set after status
	; S SDDA=$$SCIEN(DFN,SDCL,SDT) ;smh -why is this here? SDDA won't change.
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D AFTER^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; call event driver
	D EVT^SDAMEVT(.SDATA,4,SDMODE,SDCIHDL)
	QUIT 0
	;
RMCICK(PAT,CLINIC,DATE)	;PEP; Can you remove a check-in for this patient?
	; PAT - DFN by value
	; CLINIC - ^SC ien by value
	; DATE - Appointment Date
	; Output: 0 if okay or 1 if error
	;
	; Error for Unit Tests
	I $G(BSDXSIMERR2) Q 1_U_"Simulated Error"
	;
	; Get appointment IEN in ^SC(DA(2),"S",DA(1),1,
	N SCIEN S SCIEN=$$SCIEN^BSDXAPI(PAT,CLINIC,DATE)
	;
	; If not there, it has been cancelled. Okay to Remove Check-in.
	I 'SCIEN QUIT 0
	;
	; Check if checked out
	I $$CO^BSDXAPI(PAT,CLINIC,DATE,SCIEN) Q 1_U_"Appointment Already Checked Out"
	;
	QUIT 0
	;
UPDATENT(PAT,CLINIC,DATE,NOTE)	; PEP; Update Note in ^SC for patient's appointment @ DATE
	; PAT = DFN
	; CLINIC = SC IEN
	; DATE = FM Date/Time of Appointment
	;
	; Returns:
	; 0 if okay
	; -1 if failure
	;
	; ERROR SIMULATION
	I $G(BSDXSIMERR1) QUIT "-1~Simulated Error"
	;
	N SCIEN S SCIEN=$$SCIEN^BSDXAPI(PAT,CLINIC,DATE) ; ien of appt in ^SC
	I SCIEN<1 QUIT 0    ; Appt cancelled; cancelled appts rm'ed from file 44
	N BSDXIENS S BSDXIENS=SCIEN_","_DATE_","_CLINIC_","
	N BSDXFDA S BSDXFDA(44.003,BSDXIENS,3)=$E(NOTE,1,150)
	N BSDXERR
	D FILE^DIE("","BSDXFDA","BSDXERR")
	I $D(BSDXERR) QUIT "-1~Can't file for Pat "_PAT_" in Clinic "_CLINIC_" at "_DATE_". Fileman reported an error: "_BSDXERR("DIERR",1,"TEXT",1)
	QUIT 0
	;
AVUPDTCN(BSDXSCD,BSDXSTART,BSDXLEN)	;Update PIMS Clinic availability for cancel
	; NB: VEN/SMH: This code has never been tested. It's here for its
	; presumptive function, but I don't know whether it works accurately!
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
AVUPDTMK(BSDXSCD,BSDXSTART,BSDXLEN,BSDXPATID)	; Update RPMS Clinic availability for Make
	;SEE SDM1
	N Y,DFN
	N SL,STARTDAY,X,SC,SB,HSI,SI,STR,SDDIF,SDMAX,SDDATE,SDDMAX,SDSDATE,CCXN,MXOK,COV,SDPROG
	N X1,SDEDT,X2,SD,SM,SS,S,SDLOCK,ST,I
	S Y=BSDXSCD,DFN=BSDXPATID
	S SL=$G(^SC(+Y,"SL")),X=$P(SL,U,3),STARTDAY=$S($L(X):X,1:8),SC=Y,SB=STARTDAY-1/100,X=$P(SL,U,6),HSI=$S(X=1:X,X:X,1:4),SI=$S(X="":4,X<3:4,X:X,1:4),STR="#@!$* XXWVUTSRQPONMLKJIHGFEDCBA0123456789jklmnopqrstuvwxyz",SDDIF=$S(HSI<3:8/HSI,1:2) K Y
	;Determine maximum days for scheduling
	S SDMAX(1)=$P($G(^SC(+SC,"SDP")),U,2) S:'SDMAX(1) SDMAX(1)=365
	S (SDMAX,SDDMAX)=$$FMADD^XLFDT(DT,SDMAX(1))
	S SDDATE=BSDXSTART
	S SDSDATE=SDDATE,SDDATE=SDDATE\1
1	;L  Q:$D(SDXXX)  S CCXN=0 K MXOK,COV,SDPROT Q:DFN<0  S SC=+SC
	Q:$D(SDXXX)  S CCXN=0 K MXOK,COV,SDPROT Q:DFN<0  S SC=+SC
	S X1=DT,SDEDT=365 S:$D(^SC(SC,"SDP")) SDEDT=$P(^SC(SC,"SDP"),"^",2)
	S X2=SDEDT D C^%DTC S SDEDT=X
	S Y=BSDXSTART
EN1	S (X,SD)=Y,SM=0 D DOW
S	I '$D(^SC(SC,"ST",$P(SD,"."),1)) S SS=+$O(^SC(+SC,"T"_Y,SD)) Q:SS'>0  Q:^(SS,1)=""  S ^SC(+SC,"ST",$P(SD,"."),1)=$E($P($T(DAY),U,Y+2),1,2)_" "_$E(SD,6,7)_$J("",SI+SI-6)_^(1),^(0)=$P(SD,".")
	S S=BSDXLEN
	;Check if BSDXLEN evenly divisible by appointment length
	S RPMSL=$P(SL,U)
	I BSDXLEN<RPMSL S BSDXLEN=RPMSL
	I BSDXLEN#RPMSL'=0 D
	. S BSDXINC=BSDXLEN\RPMSL
	. S BSDXINC=BSDXINC+1
	. S BSDXLEN=RPMSL*BSDXINC
	S SL=S_U_$P(SL,U,2,99)
SC	S SDLOCK=$S('$D(SDLOCK):1,1:SDLOCK+1) Q:SDLOCK>9
	L +^SC(SC,"ST",$P(SD,"."),1):5 G:'$T SC
	S SDLOCK=0,S=^SC(SC,"ST",$P(SD,"."),1)
	S I=SD#1-SB*100,ST=I#1*SI\.6+($P(I,".")*SI),SS=SL*HSI/60*SDDIF+ST+ST
	I (I<1!'$F(S,"["))&(S'["CAN") L -^SC(SC,"ST",$P(SD,"."),1) Q
	I SM<7 S %=$F(S,"[",SS-1) S:'%!($P(SL,"^",6)<3) %=999 I $F(S,"]",SS)'<%!(SDDIF=2&$E(S,ST+ST+1,SS-1)["[") S SM=7
	;
SP	I ST+ST>$L(S),$L(S)<80 S S=S_" " G SP
	S SDNOT=1
	S ABORT=0
	F I=ST+ST:SDDIF:SS-SDDIF D  Q:ABORT
	. S ST=$E(S,I+1) S:ST="" ST=" "
	. S Y=$E(STR,$F(STR,ST)-2)
	. I S["CAN"!(ST="X"&($D(^SC(+SC,"ST",$P(SD,"."),"CAN")))) S ABORT=1 Q
	. I Y="" S ABORT=1 Q
	. S:Y'?1NL&(SM<6) SM=6 S ST=$E(S,I+2,999) S:ST="" ST=" " S S=$E(S,1,I)_Y_ST
	. Q
	S ^SC(SC,"ST",$P(SD,"."),1)=S
	L -^SC(SC,"ST",$P(SD,"."),1)
	Q
DAY	;;^SUN^MON^TUES^WEDNES^THURS^FRI^SATUR
	;
DOW	S %=$E(X,1,3),Y=$E(X,4,5),Y=Y>2&'(%#4)+$E("144025036146",Y)
	F %=%:-1:281 S Y=%#4=1+1+Y
	S Y=$E(X,6,7)+Y#7
	Q
	;
