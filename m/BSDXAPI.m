BSDXAPI	; IHS/ANMC/LJF & VW/SMH - SCHEDULING APIs ; 6/20/12 12:40pm
	;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	; Licensed under LGPL  
	;
	;Orignal routine is BSDAPI by IHS/LJF, HMW, and MAW
	; mods (many) by WV/SMH
	;Move to BSDX namespace as BSDXAPI from BSDAPI by WV/SMH
	; Change History:
	; 2010-11-5: (1.42)
	; - Fixed errors having to do uncanceling patient appointments if it was a patient cancelled appointment.
	; - Use new style Fileman API for storing appointments in file 44 in $$MAKE due to problems with legacy API.
	; 2010-11-12: (1.42)
	; - Changed ="C" to ["C" in SCIEN. Cancelled appointments can be "PC" as well. 
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
	; actually make the appointment
	;
MAKE1(DFN,CLIN,TYP,DATE,LEN,INFO)	; Simplified PEP w/ parameters for $$MAKE - making appointment
	; Call like this for DFN 23435 having an appointment at Hospital Location 33
	; have 3 (scheduled) or 4 (walkin) appt at Dec 20, 2009 @ 10:11:59 for 30 minutes appt
	; for Baby foxes hallucinations.
	; S RESULT=$$MAKE1^BSDXAPI(23435,33,(3 or 4),3091220.221159,30,"I see Baby foxes")
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
	N DIC,DA,Y,X,DD,DO,DLAYGO
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
	; call event driver
	NEW DFN,SDT,SDCL,SDDA,SDMODE
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2
	S SDDA=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	D MAKE^SDAMEVT(DFN,SDT,SDCL,SDDA,SDMODE)
	Q 0
	;
MAKECK(BSDR) ; $$ - Is it okay to make an appointment? ; PEP
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
UNMAKE(BSDR) ; Reverse Make - Private $$
	; Only used in Emergiencies where Fileman data filing fails.
	; If previous data exists, which caused an error, it's destroyed.
	; NB: ^DIK stops for nobody
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
	I '$D(^DPT(+$G(BSDR("PAT")),0)) Q 1_U_"Patient not on file: "_$G(BSDR("PAT"))
	I '$D(^SC(+$G(BSDR("CLN")),0)) Q 1_U_"Clinic not on file: "_$G(BSDR("CLN"))
	I $G(BSDR("ADT")) S BSDR("ADT")=+$E(BSDR("ADT"),1,12)  ;remove seconds
	I $G(BSDR("ADT"))'?7N.1".".4N Q 1_U_"Appt Date/Time error: "_$G(BSDR("ADT"))
	I $G(BSDR("CDT")) S BSDR("CDT")=+$E(BSDR("CDT"),1,12)  ;remove seconds
	I $G(BSDR("CDT"))'?7N.1".".4N Q 1_U_"Checkin Date/Time error: "_$G(BSDR("CDT"))
	I '$D(^VA(200,+$G(BSDR("USR")),0)) Q 1_U_"User Who Made Appt Error: "_$G(BSDR("USR"))
	;
	; find ien for appt in file 44
	NEW IEN,DIE,DA,DR
	S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I 'IEN Q 1_U_"Error trying to find appointment for checkin: Patient="_BSDR("PAT")_" Clinic="_BSDR("CLN")_" Appt="_BSDR("ADT")
	;
	; remember before status
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2,SDDA=IEN
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; set checkin
	S DIE="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),DA=IEN
	S DR="309///"_BSDR("CDT")_";302///`"_BSDR("USR")_";305///"_$$NOW^XLFDT
	D ^DIE
	;
	; set after status
	S SDDA=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D AFTER^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; call event driver
	D EVT^SDAMEVT(.SDATA,4,SDMODE,SDCIHDL)
	Q 0
	;
CANCEL1(DFN,CLIN,TYP,APDATE,REASON,INFO)	; PEP w/ parameters for $$CANCEL - cancelling appointment
	; Call like this for DFN 23435 cancelling an appointment at Hospital Location 33,
	; cancellation initiated by patient ("PC" rather than clinic "C"),
	; cancelling appt at Dec 20, 2009 @ 10:11:59 because of reason 1 in file 409.2 IEN (weather)
	; because foxes come out during bad weather.
	; S RESULT=$$CANCEL1^BSDXAPI(23435,33,"PC",3091220.221159,1,"Afraid of foxes")
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
	NEW IEN,DIE,DA,DR
	S IEN=$$SCIEN(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I 'IEN Q 1_U_"Error trying to find appointment for cancel: Patient="_BSDR("PAT")_" Clinic="_BSDR("CLN")_" Appt="_BSDR("ADT")
	;
	; BSDX 1.5 3110125
	; UJO/SMH - Add ability to remove check-in if the patient is checked in
	; I $$CI(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"),IEN) Q 1_U_"Patient already checked in; cannot cancel until checkin deleted: Patient="_BSDR("PAT")_" Clinic="_BSDR("CLN")_" Appt="_BSDR("ADT")
	; Remove check-in if the patient is checked in.
	N BSDXRESULT S BSDXRESULT=0 ; Result; should be zero if success; -1 + message if failure
	I $$CI(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"),IEN) SET BSDXRESULT=$$RMCI(BSDR("PAT"),BSDR("CLN"),BSDR("ADT"))
	I BSDXRESULT Q BSDXRESULT
	;
	; remember before status
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCPHDL
	S DFN=BSDR("PAT"),SDT=BSDR("ADT"),SDCL=BSDR("CLN"),SDMODE=2,SDDA=IEN
	S SDCPHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCPHDL)
	;
	; get user who made appt and date appt made from ^SC
	;    because data in ^SC will be deleted
	NEW USER,DATE
	S USER=$P($G(^SC(SDCL,"S",SDT,1,IEN,0)),U,6)
	S DATE=$P($G(^SC(SDCL,"S",SDT,1,IEN,0)),U,7)
	;
	; update file 2 info
	NEW DIE,DA,DR
	S DIE="^DPT("_DFN_",""S"",",DA(1)=DFN,DA=SDT
	S DR="3///"_BSDR("TYP")_";14///`"_BSDR("USR")_";15///"_BSDR("CDT")_";16///`"_BSDR("CR")_";19///`"_USER_";20///"_DATE
	S:$G(BSDR("NOT"))]"" DR=DR_";17///"_$E(BSDR("NOT"),1,160)
	D ^DIE
	;
	; delete data in ^SC
	NEW DIK,DA
	S DIK="^SC("_BSDR("CLN")_",""S"","_BSDR("ADT")_",1,"
	S DA(2)=BSDR("CLN"),DA(1)=BSDR("ADT"),DA=IEN
	D ^DIK
	;
	; call event driver
	D CANCEL^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDMODE,SDCPHDL)
	Q 0
	;
CI(PAT,CLINIC,DATE,SDIEN)	;PEP; -- returns 1 if appt already checked-in
	NEW X
	S X=$G(SDIEN)   ;ien sent in call
	I 'X S X=$$SCIEN(PAT,CLINIC,DATE) I 'X Q 0
	S X=$P($G(^SC(CLINIC,"S",DATE,1,X,"C")),U)
	Q $S(X:1,1:0)
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
	; Move my variables into the ones used by SDAPIs (just a convenience)
	NEW SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL
	S DFN=PAT,SDT=DATE,SDCL=CLINIC,SDMODE=2,SDDA=$$SCIEN(DFN,SDCL,SDT)
	;
	I SDDA<1 QUIT 0    ; Appt cancelled; cancelled appts rm'ed from file 44
	;
	; remember before status
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D BEFORE^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; remove check-in using filer.
	N BSDXIENS S BSDXIENS=SDDA_","_DATE_","_CLINIC_","
	S BSDXFDA(44.003,BSDXIENS,309)="@" ; CHECKED-IN
	S BSDXFDA(44.003,BSDXIENS,302)="@" ; CHECK IN USER
	S BSDXFDA(44.003,BSDXIENS,305)="@" ; CHECK IN ENTERED
	N BSDXERR
	D FILE^DIE("","BSDXFDA","BSDXERR")
	I $D(BSDXERR) QUIT "-1~Can't file for Pat "_PAT_" in Clinic "_CLINIC_" at "_DATE_". Fileman reported an error: "_BSDXERR("DIERR",1,"TEXT",1)
	;
	; set after status
	S SDDA=$$SCIEN(DFN,SDCL,SDT)
	S SDCIHDL=$$HANDLE^SDAMEVT(1),SDATA=SDDA_U_DFN_U_SDT_U_SDCL
	D AFTER^SDAMEVT(.SDATA,DFN,SDT,SDCL,SDDA,SDCIHDL)
	;
	; call event driver
	D EVT^SDAMEVT(.SDATA,4,SDMODE,SDCIHDL)
	QUIT 0
	;
SCIEN(PAT,CLINIC,DATE)	;PEP; returns ien for appt in ^SC
	NEW X,IEN
	S X=0 F  S X=$O(^SC(CLINIC,"S",DATE,1,X)) Q:'X  Q:$G(IEN)  D
	. Q:$P($G(^SC(CLINIC,"S",DATE,1,X,0)),U,9)["C"  ;cancelled
	 . I +$G(^SC(CLINIC,"S",DATE,1,X,0))=PAT S IEN=X
	Q $G(IEN)
	;
APPTYP(PAT,DATE)	;PEP; -- returns type of appt (scheduled or walk-in)
	NEW X S X=$P($G(^DPT(PAT,"S",DATE,0)),U,7)
	Q $S(X=3:"SCHED",X=4:"WALK-IN",1:"??")
	;
CO(PAT,CLINIC,DATE,SDIEN)	;PEP; -- returns 1 if appt already checked-out
	NEW X
	S X=$G(SDIEN)   ;ien sent in call
	I 'X S X=$$SCIEN(PAT,CLINIC,DATE) I 'X Q 0
	S X=$P($G(^SC(CLINIC,"S",DATE,1,X,"C")),U,3)
	Q $S(X:1,1:0)
	;
UPDATENT(PAT,CLINIC,DATE,NOTE)	; PEP; Update Note in ^SC for patient's appointment @ DATE
	; PAT = DFN
	; CLINIC = SC IEN
	; DATE = FM Date/Time of Appointment
	;
	; Returns:
	; 0 if okay
	; -1 if failure
	N SCIEN S SCIEN=$$SCIEN(PAT,CLINIC,DATE) ; ien of appt in ^SC
	I SCIEN<1 QUIT 0    ; Appt cancelled; cancelled appts rm'ed from file 44
	N BSDXIENS S BSDXIENS=SCIEN_","_DATE_","_CLINIC_","
	S BSDXFDA(44.003,BSDXIENS,3)=$E(NOTE,1,150)
	N BSDXERR
	D FILE^DIE("","BSDXFDA","BSDXERR")
	I $D(BSDXERR) QUIT "-1~Can't file for Pat "_PAT_" in Clinic "_CLINIC_" at "_DATE_". Fileman reported an error: "_BSDXERR("DIERR",1,"TEXT",1)
	QUIT 0
