BSDX25	; VW/UJO/SMH - WINDOWS SCHEDULING RPCS ; 4/28/11 10:24am
	;;1.6T1;BSDX;;May 11, 2011
	; Licensed under LGPL
	;
	; Change Log:
	; 3110106: SMH -> Changed Check-in EP - Removed unused paramters. Will change C#
	;
	;
UT	; Unit Tests
	; Make appointment, checkin, then uncheckin
	N ZZZ
	N APPTTIME S APPTTIME=$E($$NOW^XLFDT(),1,12)
	D APPADD^BSDX07(.ZZZ,APPTTIME,APPTTIME+.0001,3,"Dr Office",30,"Sam's Note",1)
	N APPTID S APPTID=+^BSDXTMP($J,1)
	N HL S HL=$$GET1^DIQ(9002018.4,APPTID,".07:.04","I")
	D CHECKIN^BSDX25(.ZZZ,APPTID,$$NOW^XLFDT())
	IF '$P(^BSDXAPPT(APPTID,0),U,3) WRITE "ERROR IN CHECKIN 1",!
	IF '+$G(^SC(HL,"S",APPTTIME,1,1,"C")) WRITE "ERROR IN CHECKIN 2",!
	D RMCI^BSDX25(.ZZZ,APPTID)
	IF $P(^BSDXAPPT(APPTID,0),U,3) WRITE "ERROR IN UNCHECKIN 1",!
	IF $G(^SC(HL,"S",APPTTIME,1,1,"C")) WRITE "ERROR IN UNCHECKIN 2",!
	D RMCI^BSDX25(.ZZZ,APPTID)  ; again, test sanity in repeat
	IF $P(^BSDXAPPT(APPTID,0),U,3) WRITE "ERROR IN UNCHECKIN 1",!
	IF $G(^SC(HL,"S",APPTTIME,1,1,"C")) WRITE "ERROR IN UNCHECKIN 2",!
	; now test various error conditions
	; Test Error 1
	D RMCI^BSDX25(.ZZZ,)
	IF +^BSDXTMP($J,1)'=-1 WRITE "ERROR IN ETest 1",!
	; Test Error 2
	D RMCI^BSDX25(.ZZZ,234987234398)
	IF +^BSDXTMP($J,1)'=-2 WRITE "ERROR IN Etest 2",!
	; Tests for 3 to 5 difficult to produce
	; Error tests follow: Mumps error test; Transaction restartability
	N bsdxdie S bsdxdie=1
	D RMCI^BSDX25(.ZZZ,APPTID)
	IF +^BSDXTMP($J,1)'=-20 WRITE "ERROR IN Etest 3",!
	K bsdxdie
	N bsdxrestart S bsdxrestart=1
	D RMCI^BSDX25(.ZZZ,APPTID)
	IF +^BSDXTMP($J,1)'=0 WRITE "Error in Etest 4",!
	QUIT
CHECKIND(BSDXY,BSDXAPTID,BSDXCDT,BSDXCC,BSDXPRV,BSDXROU,BSDXVCL,BSDXVFM,BSDXOG)	;EP
	;Entry point for debugging
	;
	;I +$G(^BSDXDBUG("BREAK","CHECKIN")),+$G(^BSDXDBUG("BREAK"))=DUZ D DEBUG^%Serenji("CHECKIN^BSDX25(.BSDXY,BSDXAPTID,BSDXCDT,BSDXCC,BSDXPRV,BSDXROU,BSDXVCL,BSDXVFM,BSDXOG)",$P(^BSDXDBUG("BREAK"),U,2))
	Q
	;
CHECKIN(BSDXY,BSDXAPTID,BSDXCDT)	; ,BSDXCC,BSDXPRV,BSDXROU,BSDXVCL,BSDXVFM,BSDXOG)	;EP Check in appointment
	; Private to GUI; use BSDXAPI for general API to checkin patients
	; Parameters:
	; BSDXY: Global Out
	; BSDXAPTID: Appointment ID in ^BSDXAPPT
	; BSDXCDT: Checkin Date --> Changed
	; BSDXCC: Clinic Stop IEN (not used)
	; BSDXPRV: Provider IEN (not used)
	; BSDXROU: Print Routing Slip? (not used)
	; BSDXVCL: PCC+ Clinic IEN (not used)
	; BSDXVFM: PCC+ Form IEN (not used)
	; BSDXOG: PCC+ Outguide (true or false)
	;
	; Output:
	; ADO.net table with 1 column ErrorID, 1 row result
	; - 0 if all okay
	; - Another number or text if not
	
	N BSDXNOD,BSDXPATID,BSDXSTART,DIK,DA,BSDXID,BSDXI,BSDXZ,BSDXIENS,BSDXVEN
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute protocol
	;
	D ^XBKVAR S X="ERROR^BSDX25",@^%ZOSF("TRAP")
	S BSDXI=0
	K ^BSDXTMP($J)
	S BSDXY="^BSDXTMP("_$J_")"
	S ^BSDXTMP($J,0)="T00020ERRORID"_$C(30)
	I '+BSDXAPTID D ERR("BSDX25: Invalid Appointment ID") Q
	I '$D(^BSDXAPPT(BSDXAPTID,0)) D ERR("BSDX08: Invalid Appointment ID") Q
	; Remove Date formatting v.1.5. Client will send date as FM Date.
	;S:BSDXCDT["@0000" BSDXCDT=$P(BSDXCDT,"@")
	;S %DT="T",X=BSDXCDT D ^%DT S BSDXCDT=Y
	   S BSDXCDT=+BSDXCDT  ; Strip off zeros if C# sends them
	I BSDXCDT=-1 D ERR(70) Q
	I BSDXCDT>$$NOW^XLFDT S BSDXCDT=$$NOW^XLFDT
	;Checkin BSDX APPOINTMENT entry
	D BSDXCHK(BSDXAPTID,BSDXCDT)
	S BSDXNOD=^BSDXAPPT(BSDXAPTID,0)
	S BSDXPATID=$P(BSDXNOD,U,5)
	S BSDXSTART=$P(BSDXNOD,U)
	;
	S BSDXSC1=$P(BSDXNOD,U,7) ;RESOURCEID
	I BSDXSC1]"",$D(^BSDXRES(BSDXSC1,0)) D  I +$G(BSDXZ) D ERR($P(BSDXZ,U,2)) Q
	. S BSDXNOD=^BSDXRES(BSDXSC1,0)
	. S BSDXSC1=$P(BSDXNOD,U,4) ;HOSPITAL LOCATION
	. I BSDXSC1]"",$D(^SC(BSDXSC1,0)) D APCHK(.BSDXZ,BSDXSC1,BSDXPATID,BSDXCDT,BSDXSTART)
	;
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="0"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
BSDXCHK(BSDXAPTID,BSDXCDT)	;
	;
	S BSDXIENS=BSDXAPTID_","
	S BSDXFDA(9002018.4,BSDXIENS,.03)=BSDXCDT
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	Q
	;
APCHK(BSDXZ,BSDXSC1,BSDXDFN,BSDXCDT,BSDXSTART)	        ;
	;Checkin appointment for patient BSDXDFN in clinic BSDXSC1
	;at time BSDXSTART
	S BSDXZ=$$CHECKIN1^BSDXAPI(BSDXDFN,BSDXSC1,BSDXSTART)
	Q
	;
RMCI(BSDXY,BSDXAPPTID)	; EP - Remove Check-in from BSDX APPT and 2/44
	; Called by RPC [Fill in later]
	; 
	; Parameters to pass:
	; APPTID: IEN in file BSDX APPOINTMENT
	;
	; Return in global array:
	; Record set with Column ERRORID; value of 0 AOK; other value 
	;  --> means that something went wrong
	; 
	; Error Reference:
	; -1~Invalid Appointment ID (not passed)
	; -2~Invalid Appointment ID (Doesn't exist in ^BSDXAPPT)
	; -3~DB has corruption. Call Tech Support. (Resource ID doesn't exist in BSDXAPPT)
	; -4~DB has corruption. Call Tech Support. (Resource ID in BSDXAPPT doesnt exist in BSDXRES)
	; -5~BSDXAPI Error. Message depends on error.
	; -20~Mumps Error
	; 
	N BSDXNOEV S BSDXNOEV=1 ;Don't execute protocol
	;
	N $ET S $ET="G ERROR^BSDX25" ; Error Trap
	;
	; Set return variable and kill contents
	S BSDXY=$NAME(^BSDXTMP($J))
	K @BSDXY
	; 
	N BSDXI S BSDXI=0 ; Initialize Counter
	;
	S ^BSDXTMP($J,BSDXI)="T00020ERRORID"_$C(30) ; Header of ADO recordset
	;
	TSTART (BSDXI):SERIAL ; Perform Autolocking
	;
	;;;test
	I $g(bsdxdie) S X=8/0
	;;;
	I $g(bsdxrestart) k bsdxrestart TRESTART
	;;;test
	;
	; Check for Appointment ID (passed and exists in file)
	I '+$G(BSDXAPPTID) D ERR("-1~Invalid Appointment ID") QUIT
	I '$D(^BSDXAPPT(BSDXAPPTID,0)) D ERR("-2~Invalid Appointment ID") QUIT
	;
	; Remove checkin from BSDX APPOINTMENT entry
	D BSDXCHK(BSDXAPPTID,"@")
	;
	; Now, remove checkin from PIMS files 2/44
	N BSDXNOD S BSDXNOD=^BSDXAPPT(BSDXAPPTID,0)
	N BSDXPATID S BSDXPATID=$P(BSDXNOD,U,5)	; DFN
	N BSDXSTART S BSDXSTART=$P(BSDXNOD,U)	; Start Date
	N BSDXSC1 S BSDXSC1=$P(BSDXNOD,U,7) ; Resource ID
	; 
	; If the resource doesn't exist, error out. DB is corrupt.
	I 'BSDXSC1 D ERR("-3~DB has corruption. Call Tech Support.") QUIT
	I '$D(^BSDXRES(BSDXSC1,0)) D ERR("-4~DB has corruption. Call Tech Support.") QUIT 
	;
	N BSDXNOD S BSDXNOD=^BSDXRES(BSDXSC1,0) ; Resource 0 node
	S BSDXSC1=$P(BSDXNOD,U,4) ;HOSPITAL LOCATION
	;
	N BSDXZ ; Scratch variable to hold error message
	I BSDXSC1]"",$D(^SC(BSDXSC1,0)) S BSDXZ=$$RMCI^BSDXAPI(BSDXPATID,BSDXSC1,BSDXSTART)
	I +$G(BSDXZ) D ERR("-5~"_$P(BSDXZ,U,2)) QUIT
	; 
	TCOMMIT  ; Save Data into Globals
	;
	; Return ADO recordset
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="0"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
CHKEVT(BSDXPAT,BSDXSTART,BSDXSC)	;EP Called by BSDX CHECKIN APPOINTMENT event
	;when appointments CHECKIN via PIMS interface.
	;Propagates CHECKIN to BSDXAPPT and raises refresh event to running GUI clients
	;
	Q:+$G(BSDXNOEV)
	Q:'+$G(BSDXSC)
	N BSDXSTAT,BSDXFOUND,BSDXRES
	S BSDXSTAT=""
	S:$G(SDATA("AFTER","STATUS"))["CHECKED IN" BSDXSTAT=$P(SDATA("AFTER","STATUS"),"^",4)
	S BSDXFOUND=0
	I $D(^BSDXRES("ALOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ALOC",BSDXSC,0)) S BSDXFOUND=$$CHKEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)
	I BSDXFOUND D CHKEVT3(BSDXRES) Q
	I $D(^BXDXRES("ASSOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ASSOC",BSDXSC,0)) S BSDXFOUND=$$CHKEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)
	I BSDXFOUND D CHKEVT3(BSDXRES)
	Q
	;
CHKEVT1(BSDXRES,BSDXSTART,BSDXPAT,BSDXSTAT)	;
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
	I BSDXFOUND,+$G(BSDXAPPT) D BSDXCHK(BSDXAPPT,BSDXSTAT)
	Q BSDXFOUND
	;
CHKEVT3(BSDXRES)	;
	;Call RaiseEvent to notify GUI clients
	;
	N BSDXRESN
	S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	Q:BSDXRESN=""
	S BSDXRESN=$P(BSDXRESN,"^")
	D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	Q
	;
ERROR	;
	S $ETRAP="D ^%ZTER HALT"  ; Emergency Error Trap for the wise
	   ; Rollback, otherwise ^XTER will be empty from future rollback
	   I $TL>0 TROLLBACK
	   D ^%ZTER
	   S $EC=""  ; Clear Error
	   ; Log error message and send to client
	D ERR("-20~Mumps Error")
	Q
	;
ERR(BSDXERR)	;Error processing
	I $TLEVEL>0 TROLLBACK
	S BSDXERR=$G(BSDXERR)
	S BSDXERR=$P(BSDXERR,"~")_"~"_$TEXT(+0)_":"_$P(BSDXERR,"~",2) ; Append Routine Name
	S BSDXI=$G(BSDXI)+1
	S ^BSDXTMP($J,BSDXI)=BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	QUIT
