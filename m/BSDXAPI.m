BSDXAPI	; IHS/LJF,HMW,MAW & VEN/SMH - SCHEDULING APIs ; 7/10/12 5:58pm
	;;1.7T2;BSDX;;Jul 11, 2012;Build 18
	; Licensed under LGPL  
	;
	; Orignal routine is BSDAPI by IHS/LJF, HMW, and MAW
	; mods (many) by WV/SMH
	; Move to BSDX namespace as BSDXAPI from BSDAPI by WV/SMH
	; Change history is located in BSDXAPI1 (to save space).
	;
MAKE1(DFN,CLIN,TYP,DATE,LEN,INFO)	; Simplified PEP w/ parameters for $$MAKE - making appointment
	; Call like this for DFN 23435 having an appointment at Hospital Location 33
	; have 3 (scheduled) or 4 (walkin) appt at Dec 20, 2009 @ 10:11:59 for 30 minutes appt
	; for Baby foxes hallucinations.
	; S RESULT=$$MAKE1^BSDXAPI(23435,33,(3 or 4),3091220.221159,30,"I see Baby foxes")
	N BSDR
	S BSDR("PAT")=DFN       ;DFN
	S BSDR("CLN")=CLIN      ;Hosp Loc IEN
	S BSDR("TYP")=TYP       ;3 sched or 4 walkin
	S BSDR("ADT")=DATE      ;Appointment date in FM format
	S BSDR("LEN")=LEN       ;Appt len upto 240 (min)
	S BSDR("OI")=INFO     ;Reason for appt - up to 150 char
	S BSDR("USR")=DUZ       ;Person who made appt - current user
	Q $$MAKE(.BSDR)
	;
MAKE(BSDR)	;PEP; call to store appt made
	;
	; Make call using: S ERR=$$MAKE^BSDXAPI(.ARRAY)
	;
	; Input Array -
	; BSDR("PAT") = ien of patient in file 2
	; BSDR("CLN") = ien of clinic in file 44
	; BSDR("TYP") = 3 for scheduled appts, 4 for walkins
	; BSDR("ADT") = appointment date and time
	; BSDR("LEN") = appointment length in minutes (*1.42 limit removed)
	; BSDR("OI")  = reason for appt - up to 150 characters
	; BSDR("USR") = user who made appt
	;
	;Output: error status and message
	;   = 0 or null:  everything okay
	;   = 1^message:  error and reason
	;
	N BSDXMKCK S BSDXMKCK=$$MAKECK(.BSDR) ; Check if we can make appointment
	I BSDXMKCK Q BSDXMKCK ; If we can't, quit with the reason why.
	;
	;Otherwise, we continue
	;
	N BSDXFDA,BSDXIENS,BSDXMSG ; FILE/UPDATE^DIE variables
	;
	I $D(^DPT(BSDR("PAT"),"S",BSDR("ADT"),0)),$P(^(0),U,2)["C" D
	. ; "un-cancel" existing appt in file 2
	. S BSDXIENS=BSDR("ADT")_","_BSDR("PAT")_","
	. S BSDXFDA(2.98,BSDXIENS,".01")=BSDR("CLN")
	. S BSDXFDA(2.98,BSDXIENS,"3")=""
	. S BSDXFDA(2.98,BSDXIENS,"9")=BSDR("TYP")
	. S BSDXFDA(2.98,BSDXIENS,"9.5")=9
	. S BSDXFDA(2.98,BSDXIENS,"14")=""
	. S BSDXFDA(2.98,BSDXIENS,"15")=""
	. S BSDXFDA(2.98,BSDXIENS,"16")=""
	. S BSDXFDA(2.98,BSDXIENS,"17")="@" ; v 1.7; cancellation remarks were left over
	. S BSDXFDA(2.98,BSDXIENS,"19")=""
	. S BSDXFDA(2.98,BSDXIENS,"20")=$$NOW^XLFDT
	. D FILE^DIE("","BSDXFDA","BSDXMSG")
	Q:$D(BSDXMSG) 1_U_"Fileman edit to DPT error: Patient="_BSDR("PAT")_" Appt="_BSDR("ADT")_" Error="_BSDXMSG("DIERR",1,"TEXT",1)
	;
	Q:$G(BSDXSIMERR2) 1_U_$NA(BSDXSIMERR2) ; Unit Test line
	;
	E  D  ; File new appointment/edit existing appointment in file 2
	. S BSDXIENS="?+2,"_BSDR("PAT")_","
	. S BSDXIENS(2)=BSDR("ADT")
	. S BSDXFDA(2.98,BSDXIENS,.01)=BSDR("CLN")
	. S BSDXFDA(2.98,BSDXIENS,"9")=BSDR("TYP")
	. S BSDXFDA(2.98,BSDXIENS,"9.5")=9
	. S BSDXFDA(2.98,BSDXIENS,"20")=$$NOW^XLFDT
	. D UPDATE^DIE("","BSDXFDA","BSDXIENS","BSDXMSG")
	Q:$D(BSDXMSG) 1_U_"FileMan add to DPT error: Patient="_BSDR("PAT")_" Appt="_BSDR("ADT")_" Error="_BSDXMSG("DIERR",1,"TEXT",1)
	;
	Q:$G(BSDXSIMERR3) 1_U_$NA(BSDXSIMERR3) ; Unit Test line
	;
	; add appt to file 44. This adds it to the FIRST subfile (Appointment)
	N DIC,DA,Y,X,DD,DO,DLAYGO,DINUM
	I '$D(^SC(BSDR("CLN"),"S",0)) S ^SC(BSDR("CLN"),"S",0)="^44.001DA^^"
	I '$D(^SC(BSDR("CLN"),"S",BSDR("ADT"),0)) D  I Y<1 Q 1_U_"Error adding date to file 44: Clinic="_BSDR("CLN")_" Date="_BSDR("ADT")
	. S DIC="^SC("_BSDR("CLN")_",""S"",",DA(1)=BSDR("CLN"),(X,DINUM)=BSDR("ADT")
	. S DIC("P")="44.001DA",DIC(0)="L",DLAYGO=44.001
	. S Y=1 I '$D(@(DIC_X_")")) D FILE^DICN
	;
	Q:$G(BSDXSIMERR4) 1_U_$NA(BSDXSIMERR4) ; Unit Test line
	;
	; add appt for file 44, second subfile (Appointment/Patient)
	; Sep 28 2010: Changed old style API to new style API. Keep for reference //smh
	;K DIC,DA,X,Y,DLAYGO,DD,DO,DINUM
	;S DIC="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	;S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),X=BSDR("PAT")
	;S DIC("DR")="1///"_BSDR("LEN")_";3///"_$E($G(BSDR("OI")),1,150)_";7///`"_BSDR("USR")_";8///"_$P($$NOW^XLFDT,".")
	;S DIC("P")="44.003PA",DIC(0)="L",DLAYGO=44.003
	;D FILE^DICN
	;
	N BSDXIENS S BSDXIENS="?+1,"_BSDR("ADT")_","_BSDR("CLN")_","
	N BSDXFDA
	S BSDXFDA(44.003,BSDXIENS,.01)=BSDR("PAT")
	S BSDXFDA(44.003,BSDXIENS,1)=BSDR("LEN")
	S BSDXFDA(44.003,BSDXIENS,3)=$E($G(BSDR("OI")),1,150)
	S BSDXFDA(44.003,BSDXIENS,7)=BSDR("USR")
	S BSDXFDA(44.003,BSDXIENS,8)=$P($$NOW^XLFDT,".")
	N BSDXERR
	D UPDATE^DIE("","BSDXFDA","","BSDXERR")
	;
	I $D(BSDXERR) Q 1_U_"Error adding appt to file 44: Clinic="_BSDR("CLN")_" Date="_BSDR("ADT")_" Patient="_BSDR("PAT")_" Error: "_BSDXERR("DIERR",1,"TEXT",1)
	;
	;Q:$G(BSDXSIMERR5) 1_U_$NA(BSDXSIMERR5) ; Unit Test line
	S:$G(BSDXSIMERR5) X=1/0
	;
	; Update the Availablilities ; Doesn't fail. Global reads and sets.
	D AVUPDTMK^BSDXAPI1(BSDR("CLN"),BSDR("ADT"),BSDR("LEN"),BSDR("PAT"))
	;
	; call event driver
	NEW DFN,SDT,SDCL,SDDA,SDMODE
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2
	S SDDA=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	D MAKE^SDAMEVT(DFN,SDT,SDCL,SDDA,SDMODE)
	Q 0
	;
MAKECK(BSDR)	; $$ - Is it okay to make an appointment? ; PEP
	; Input: Same as $$MAKE
	; Output: 1^error or 0 for success
	; NB: This subroutine saves no data. Only checks whether it's okay.
	;
	I '$D(^DPT(+$G(BSDR("PAT")),0)) Q 1_U_"Patient not on file: "_$G(BSDR("PAT"))
	I '$D(^SC(+$G(BSDR("CLN")),0)) Q 1_U_"Clinic not on file: "_$G(BSDR("CLN"))
	I ($G(BSDR("TYP"))<3)!($G(BSDR("TYP"))>4) Q 1_U_"Appt Type error: "_$G(BSDR("TYP"))
	I $G(BSDR("ADT")) S BSDR("ADT")=+$E(BSDR("ADT"),1,12)  ;remove seconds
	I $G(BSDR("ADT"))'?7N.1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	;
	; Appt Length check removed in v 1.5
	;
	I '$D(^VA(200,+$G(BSDR("USR")),0)) Q 1_U_"User Who Made Appt Error: "_$G(BSDR("USR"))
	; More verbose error message in v1.5
	; Following block to give an error message to user if there is already an appointment for patient. More verbose than others.
	N BSDXERR ; place to store error message
	I $D(^DPT(BSDR("PAT"),"S",BSDR("ADT"),0)),$P(^(0),U,2)'["C" DO  QUIT BSDXERR  ; If there's an appt in the "S" node of file 2 and it's not cancelled
	. S BSDXERR=1_U_"Patient "_$P(^DPT(BSDR("PAT"),0),U)_" ("_BSDR("PAT")_") "
	. S BSDXERR=BSDXERR_"already has appt at "_$$FMTE^XLFDT(BSDR("ADT"))
	. N BSDXSCIEN S BSDXSCIEN=$P(^DPT(BSDR("PAT"),"S",BSDR("ADT"),0),U)  ; Clinic IEN in ^SC (0 piece of 0 node of "S" multiple in file 2)
	. N BSDXSCNAM S BSDXSCNAM=$P(^SC(BSDXSCIEN,0),U) ; PIMS Name of Clinic
	. S BSDXERR=BSDXERR_$C(13,10)_"PIMS clinic: "_BSDXSCNAM ; tell the user of the PIMS clinic
	. I $D(^BSDXRES("ALOC",BSDXSCIEN)) DO  ; if the Clinic is linked to a BSDX Resource (we find out using the index ALOC in the BSDX RESOURCE file)
	. . N BSDXRESIEN S BSDXRESIEN=$O(^BSDXRES("ALOC",BSDXSCIEN,""))
	. . QUIT:'BSDXRESIEN  ; Safeguard if index is corrupt
	. . N BSDXRESNAM S BSDXRESNAM=$P(^BSDXRES(BSDXRESIEN,0),U)
	. . S BSDXERR=BSDXERR_$C(13,10)_"Scheduling GUI clinic: "_BSDXRESNAM ; tell the user of the BSDX clinic
	Q 0
	;
UNMAKE(BSDR)	; Reverse Make - Private $$
	; Only used in Emergiencies where Fileman data filing fails.
	; If previous data exists, which caused an error, it's destroyed.
	; NB: ^DIK stops for nobody
	; NB: If Patient Appointment previously existed as cancelled, it's removed.
	; How can I tell if one previously existed when data is in an intermediate
	; State? Can I restore it if the other file failed? Restoration can cause
	; another error. If I restore the global, there will be cross-references
	; missing (ASDCN specifically).
	;
	; Input: Same array as $$MAKE
	; Output: Always 0
	NEW DIK,DA
	S DIK="^DPT("_BSDR("PAT")_",""S"","
	S DA(1)=BSDR("PAT"),DA=BSDR("ADT")
	D ^DIK
	;
	N IEN S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I 'IEN QUIT 0
	;
	NEW DIK,DA
	S DIK="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),DA=IEN
	D ^DIK
	QUIT 0
	;
CHECKIN1(DFN,CLIN,APDATE)	; Simplified PEP w/ parameters for $$CHECKIN - Checking in
	; Call like this for DFN 23435 checking in now at Hospital Location 33
	; for appt at Dec 20, 2009 @ 10:11:59 
	; S RESULT=$$CHECKIN1^BSDXAPI(23435,33,3091220.221159)
	N BSDR
	S BSDR("PAT")=DFN          ;DFN
	S BSDR("CLN")=CLIN         ;Hosp Loc IEN
	S BSDR("ADT")=APDATE       ;Appt Date
	S BSDR("CDT")=$$NOW^XLFDT  ;Check-in date defaults to now
	S BSDR("USR")=DUZ          ;Check-in user defaults to current
	Q $$CHECKIN(.BSDR)
	;
CHECKIN(BSDR)	;EP; call to add checkin info to appt; IHS/ITSC/LJF 12/23/2004 PATCH 1002
	;
	; Make call by using:  S ERR=$$CHECKIN^BSDXAPI(.ARRAY)
	;
	; Input array -
	;  BSDR("PAT") = ien of patient in file 2
	;  BSDR("CLN") = ien of clinic in file 44
	;  BSDR("ADT") = appt date/time
	;  BSDR("CDT") = checkin date/time
	;  BSDR("USR") = checkin user
	;
	; Output value -
	;              = 0 means everything worked
	;              = 1^message means error with reason message
	;
	I $G(BSDXDIE2) N X S X=1/0
	;
	N BSDXERR S BSDXERR=$$CHECKICK(.BSDR)
	I BSDXERR Q BSDXERR
	;
	; find ien for appt in file 44
	NEW IEN,DIE,DA,DR
	S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	;
	; remember before status
	; Failure analysis: Only ^TMP global is set here.
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL,SDMODE
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2,SDDA=IEN
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; set checkin; Old Code -- keep for ref VEN/SMH 3 Jul 2012
	; S DIE="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	; S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),DA=IEN
	; S DR="309///"_BSDR("CDT")_";302///`"_BSDR("USR")_";305///"_$$NOW^XLFDT
	; D ^DIE
	;
	I $D(BSDXSIMERR3) Q 1_U_"Simulated Error"
	;
	; Failure analysis: If this fails, no other changes were made in this routine
	N BSDXIENS S BSDXIENS=IEN_","_BSDR("ADT")_","_BSDR("CLN")_","
	N BSDXFDA
	S BSDXFDA(44.003,BSDXIENS,309)=BSDR("CDT")
	S BSDXFDA(44.003,BSDXIENS,302)=BSDR("USR")
	S BSDXFDA(44.003,BSDXIENS,305)=$$NOW^XLFDT()
	N BSDXERR
	D UPDATE^DIE("","BSDXFDA","BSDXERR")
	;
	I $D(BSDXERR) Q 1_U_"Error checking in appointment to file 44. Error: "_BSDXERR("DIERR",1,"TEXT",1)
	;
	; set after status
	S SDDA=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D AFTER^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; Point of no Return
	; call event driver
	D EVT^SDAMEVT(.SDATA,4,SDMODE,SDCIHDL)
	Q 0
	;
CHECKIC1(DFN,CLIN,APDATE)	; Simplified PEP w/ parameters for $$CHECKICK -
	; Check-in Check
	; Call like this for DFN 23435 checking in now at Hospital Location 33
	; for appt at Dec 20, 2009 @ 10:11:59 
	; S RESULT=$$CHECKIC1^BSDXAPI(23435,33,3091220.221159)
	N BSDR
	S BSDR("PAT")=DFN          ;DFN
	S BSDR("CLN")=CLIN         ;Hosp Loc IEN
	S BSDR("ADT")=APDATE       ;Appt Date
	S BSDR("CDT")=$$NOW^XLFDT  ;Check-in date defaults to now
	S BSDR("USR")=DUZ          ;Check-in user defaults to current
	Q $$CHECKICK(.BSDR)
	;
CHECKICK(BSDR)	; $$ PEP; - Is it okay to check-in patient?
	; Input: Same as $$CHECKIN
	; Output: 0 if okay or 1^message if error
	;
	I $G(BSDXSIMERR2) Q 1_U_"Simulated Error"
	;
	I '$D(^DPT(+$G(BSDR("PAT")),0)) Q 1_U_"Patient not on file: "_$G(BSDR("PAT"))
	I '$D(^SC(+$G(BSDR("CLN")),0)) Q 1_U_"Clinic not on file: "_$G(BSDR("CLN"))
	I $G(BSDR("ADT")) S BSDR("ADT")=+$E(BSDR("ADT"),1,12)  ;remove seconds
	I $G(BSDR("ADT"))'?7N.1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	I $G(BSDR("CDT")) S BSDR("CDT")=+$E(BSDR("CDT"),1,12)  ;remove seconds
	I $G(BSDR("CDT"))'?7N.1".".4N Q 1_U_"Checkin Date/Time error: "_$G(BSDR("CDT"))
	I '$D(^VA(200,+$G(BSDR("USR")),0)) Q 1_U_"User Who Made Appt Error: "_$G(BSDR("USR"))
	;
	; find ien for appt in file 44
	N IEN S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I 'IEN Q 1_U_"Error trying to find appointment for checkin: Patient="_BSDR("PAT")_" Clinic="_BSDR("CLN")_" Appt="_BSDR("ADT")
	Q 0
	;
CANCEL1(DFN,CLIN,TYP,APDATE,REASON,INFO)	; PEP w/ parameters for $$CANCEL - cancelling appointment
	; Call like this for DFN 23435 cancelling an appointment at Hospital Location 33,
	; cancellation initiated by patient ("PC" rather than clinic "C"),
	; cancelling appt at Dec 20, 2009 @ 10:11:59 because of reason 1 in file 409.2 IEN (weather)
	; because foxes come out during bad weather.
	; S RESULT=$$CANCEL1^BSDXAPI(23435,33,"PC",3091220.221159,1,"Afraid of foxes")
	N BSDR
	S BSDR("PAT")=DFN
	S BSDR("CLN")=CLIN
	S BSDR("TYP")=TYP
	S BSDR("ADT")=APDATE
	S BSDR("CDT")=$$NOW^XLFDT
	S BSDR("USR")=DUZ
	S BSDR("CR")=REASON
	S BSDR("NOT")=INFO
	Q $$CANCEL(.BSDR)
	;
CANCEL(BSDR)	;PEP; called to cancel appt
	;
	; Make call using: S ERR=$$CANCEL^BSDXAPI(.ARRAY)
	;
	; Input Array -
	; BSDR("PAT") = ien of patient in file 2
	; BSDR("CLN") = ien of clinic in file 44
	; BSDR("TYP") = C for canceled by clinic; PC for patient canceled
	; BSDR("ADT") = appointment date and time
	; BSDR("CDT") = cancel date and time
	; BSDR("USR") = user who canceled appt
	; BSDR("CR")  = cancel reason - pointer to file 409.2
	; BSDR("NOT") = cancel remarks - optional notes to 160 characters
	;
	;Output: error status and message
	;   = 0 or null:  everything okay
	;   = 1^message:  error and reason
	;
	; Okay to Cancel? Call Cancel Check.
	N BSDXCANCK S BSDXCANCK=$$CANCELCK(.BSDR)
	I BSDXCANCK Q BSDXCANCK
	;
	; BSDX 1.5 3110125
	; UJO/SMH - Add ability to remove check-in if the patient is checked in
	; VEN/SMH on 3120625/v1.7 - PIMS doesn't care if patient is already checked in
	; Lets you remove appointment anyways! Not like RPMS.
	; Plus... deleting checkin affects S node on 44, which is DELETED anyways!
	;
	; remember before status
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCPHDL,SDMODE
	NEW IEN S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2,SDDA=IEN
	S SDCPHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCPHDL)
	; NB: Here only ^TMP globals are set with before values.
	;
	; get user who made appt and date appt made from ^SC
	;    because data in ^SC will be deleted
	; Appointment Length: ditto
	NEW USER,DATE
	S USER=$P($G(^SC(SDCL,"S",SDT,1,IEN,0)),U,6)
	S DATE=$P($G(^SC(SDCL,"S",SDT,1,IEN,0)),U,7)
	N BSDXLEN S BSDXLEN=$$APPLEN(DFN,SDCL,SDT) ; appt length
	;
	; update file 2 info --old code; keep for reference
	;NEW DIE,DA,DR
	;S DIE="^DPT("_DFN_",""S"",",DA(1)=DFN,DA=SDT
	;S DR="3///"_BSDR("TYP")_";14///`"_BSDR("USR")_";15///"_BSDR("CDT")_";16///`"_BSDR("CR")_";19///`"_USER_";20///"_DATE
	;S:$G(BSDR("NOT"))]"" DR=DR_";17///"_$E(BSDR("NOT"),1,160)
	;D ^DIE
	N BSDXIENS S BSDXIENS=SDT_","_DFN_","
	N BSDXFDA
	S BSDXFDA(2.98,BSDXIENS,3)=BSDR("TYP")
	S BSDXFDA(2.98,BSDXIENS,14)=BSDR("USR")
	S BSDXFDA(2.98,BSDXIENS,15)=BSDR("CDT")
	S BSDXFDA(2.98,BSDXIENS,16)=BSDR("CR")
	S BSDXFDA(2.98,BSDXIENS,19)=USER
	S BSDXFDA(2.98,BSDXIENS,20)=DATE
	S:$G(BSDR("NOT"))]"" BSDXFDA(2.98,BSDXIENS,17)=$E(BSDR("NOT"),1,160)
	N BSDXERR
	D FILE^DIE("","BSDXFDA","BSDXERR")
	I $D(BSDXERR) Q 1_U_"Cannot cancel appointment in File 2"
	; Failure point 1: If we fail here, nothing has happened yet.
	;
	; delete data in ^SC -- this does not (typically) fail. Fileman won't stop
	NEW DIK,DA
	S DIK="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),DA=IEN
	D ^DIK
	; Failure point 2: not expected to happen here
	;
	; Update PIMS availability -- this doesn't fail. Global gets/sets only.
	D AVUPDTCN^BSDXAPI1(SDCL,SDT,BSDXLEN)
	;
	; call event driver -- point of no return
	D CANCEL^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDMODE,SDCPHDL)
	;
	Q 0
	;
CANCELCK(BSDR)	; $$ PEP; Okay to Cancel Appointment?
	; Input: .BSDR array as documented in $$CANCEL
	; Output: 0 or 1^Error message
	I '$D(^DPT(+$G(BSDR("PAT")),0)) Q 1_U_"Patient not on file: "_$G(BSDR("PAT"))
	I '$D(^SC(+$G(BSDR("CLN")),0)) Q 1_U_"Clinic not on file: "_$G(BSDR("CLN"))
	I ($G(BSDR("TYP"))'="C"),($G(BSDR("TYP"))'="PC") Q 1_U_"Cancel Status error: "_$G(BSDR("TYP"))
	I $G(BSDR("ADT")) S BSDR("ADT")=+$E(BSDR("ADT"),1,12)  ;remove seconds
	I $G(BSDR("ADT"))'?7N.1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	I $G(BSDR("CDT")) S BSDR("CDT")=+$E(BSDR("CDT"),1,12)  ;remove seconds
	I $G(BSDR("CDT"))'?7N.1".".4N Q 1_U_"Cancel Date/Time error: "_$G(BSDR("CDT"))
	I '$D(^VA(200,+$G(BSDR("USR")),0)) Q 1_U_"User Who Canceled Appt Error: "_$G(BSDR("USR"))
	I '$D(^SD(409.2,+$G(BSDR("CR")))) Q 1_U_"Cancel Reason error: "_$G(BSDR("CR"))
	;
	NEW IEN S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I 'IEN Q 1_U_"Error trying to find appointment for cancel: Patient="_BSDR("PAT")_" Clinic="_BSDR("CLN")_" Appt="_BSDR("ADT")
	;
	; Check-out check. New in v1.7
	I $$CO(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"),IEN) Q 1_U_"Cannot delete. Appointment has already been checked-out!"
	Q 0
	;
CI(PAT,CLINIC,DATE,SDIEN)	;PEP; -- returns 1 if appt already checked-in
	NEW X
	S X=$G(SDIEN)   ;ien sent in call
	I 'X S X=$$SCIEN(PAT,CLINIC,DATE) I 'X Q 0
	S X=$P($G(^SC(CLINIC,"S",DATE,1,X,"C")),U)
	Q $S(X:1,1:0)
	;
CO(PAT,CLINIC,DATE,SDIEN)	;PEP; -- returns 1 if appt already checked-out
	NEW X
	S X=$G(SDIEN)   ;ien sent in call
	I 'X S X=$$SCIEN(PAT,CLINIC,DATE) I 'X Q 0
	S X=$P($G(^SC(CLINIC,"S",DATE,1,X,"C")),U,3)
	Q $S(X:1,1:0)
	;
SCIEN(PAT,CLINIC,DATE)	;PEP; returns ien for appt in ^SC
	NEW X,IEN
	S X=0 F  S X=$O(^SC(CLINIC,"S",DATE,1,X)) Q:'X  Q:$G(IEN)  D
	. Q:$P($G(^SC(CLINIC,"S",DATE,1,X,0)),U,9)["C"  ;cancelled
	 . I +$G(^SC(CLINIC,"S",DATE,1,X,0))=PAT S IEN=X
	Q $G(IEN)
	;
APPLEN(PAT,CLINIC,DATE)	; $$ PEP; returns an appointment's length
	; Get either the appointment length or zero
	N SCIEN S SCIEN=$$SCIEN(PAT,CLINIC,DATE)
	Q:SCIEN $P(^SC(CLINIC,"S",DATE,1,SCIEN,0),U,2)
	Q 0
APPTYP(PAT,DATE)	;PEP; -- returns type of appt (scheduled or walk-in)
	NEW X S X=$P($G(^DPT(PAT,"S",DATE,0)),U,7)
	Q $S(X=3:"SCHED",X=4:"WALK-IN",1:"??")
	;
