BSDX01	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 7/6/12 10:52am
	;;1.7T2;BSDX;;Jul 11, 2012;Build 18
	; Licensed under LGPL
	;
SUINFOD(BSDXY,BSDXDUZ)	;EP Debugging entry point
	;D DEBUG^%Serenji("SUINFO^BSDX01(.BSDXY,BSDXDUZ)")
	;
	Q
	;
SUINFO(BSDXY,BSDXDUZ)	 ;EP
	;Called by BSDX SCHEDULING USER INFO
	;Returns ADO Recordset having column MANAGER
	;MANAGER = YES if user has keys BSDXZMGR or XUPROGMODE
	;
	N BSDXMGR,BSDXERR
	K ^BSDXTMP($J)
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S BSDXERR=""
	S ^BSDXTMP($J,BSDXI)="T00010MANAGER"_$C(30)
	;Check SECURITY KEY file for BSDXZMGR or XUPROGMODE keys
	I '+BSDXDUZ S BSDXDUZ=DUZ
	S BSDXMGR=$$APSEC("BSDXZMGR",BSDXDUZ)
	S BSDXMGR=$S(BSDXMGR=1:"YES",1:"NO")
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXMGR_$C(30)
	S ^BSDXTMP($J,BSDXI+1)=$C(31)_BSDXERR
	Q
DEPUSRD(BSDXY,BSDXDUZ)	;EP Debugging entry point
	;
	;
	;D DEBUG^%Serenji("DEPUSR^BSDX01(.BSDXY,BSDXDUZ)")
	;
	Q
	;
DEPUSR(BSDXY,BSDXDUZ)	 ;EP
	;Called by BSDX RESOURCE GROUPS BY USER
	;Returns ADO Recordset with all ACTIVE resource group names to which user has access
	;based on entries in BSDX RESOURCE USER file (Say this again for myself: Groups ONLY!!)
	;If BSDXDUZ=0 then returns all department names for current DUZ
	   ;if not linked, always returned.
	;If user BSDXDUZ possesses the key BSDXZMGR or XUPROGMODE
	;then ALL resource group names are returned regardless of whether any active resources
	;are associated with the group or not.
	;
	;
	N BSDXERR,BSDXRET,BSDXIEN,BSDXRES,BSDXDEP,BSDXDDR,BSDXDEPN,BSDXRDAT,BSDXRNOD,BSDXI
	N BSDXMGR,BSDXNOD
	K ^BSDXTEMP($J)
	K ^BSDXTMP($J)
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S BSDXERR=""
	S ^BSDXTMP($J,BSDXI)="I00020RESOURCE_GROUPID^T00030RESOURCE_GROUP"_$C(30)
	I '+BSDXDUZ S BSDXDUZ=DUZ
	;Check SECURITY KEY file for BSDXZMGR or XUPROGMODE keys
	S BSDXMGR=$$APSEC("BSDXZMGR",BSDXDUZ)
	;
	;User does not have BSDXZMGR or XUPROGMODE keys, so
	;$O THRU AC XREF OF BSDX RESOURCE USER
	I 'BSDXMGR,$D(^BSDXRSU("AC",BSDXDUZ)) S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXRSU("AC",BSDXDUZ,BSDXIEN)) Q:'+BSDXIEN  D
	. S BSDXRES=$P(^BSDXRSU(BSDXIEN,0),U)
	. Q:'$D(^BSDXDEPT("AB",BSDXRES))  ; If not part of a group, quit ("AB" is the whole file index for the resource multiple in Group file)
	   . ; Q:'$$INDIV2(BSDXRES)  ; If not in the same division as user, quit
	. S BSDXRNOD=^BSDXRES(BSDXRES,0)
	. ;QUIT if the resource is inactive
	. Q:$P(BSDXRNOD,U,2)=1
	. S BSDXDEP=0 F  S BSDXDEP=$O(^BSDXDEPT("AB",BSDXRES,BSDXDEP)) Q:'+BSDXDEP  D
	. . Q:'$D(^BSDXDEPT(BSDXDEP,0))
	. . Q:$D(^BSDXTEMP($J,BSDXDEP))
	. . S ^BSDXTEMP($J,BSDXDEP)=""
	. . S BSDXDEPN=$P(^BSDXDEPT(BSDXDEP,0),U)
	. . S BSDXI=BSDXI+1
	. . S ^BSDXTMP($J,BSDXI)=BSDXDEP_U_BSDXDEPN_$C(30)
	. . Q
	. Q
	;
	;User does have BSDXZMGR or XUPROGMODE keys, so
	;$O THRU BSDX RESOURCE GROUP file directly
	I BSDXMGR S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXDEPT(BSDXIEN)) Q:'+BSDXIEN  D
	. Q:'$D(^BSDXDEPT(BSDXIEN,0))
	. S BSDXNOD=^BSDXDEPT(BSDXIEN,0)
	. S BSDXDEPN=$P(BSDXNOD,U)
	. S BSDXI=BSDXI+1
	. S ^BSDXTMP($J,BSDXI)=BSDXIEN_U_BSDXDEPN_$C(30)
	. Q
	;
	S ^BSDXTMP($J,BSDXI+1)=$C(31)_BSDXERR
	Q
	;
	;
RESUSRD(BSDXY,BSDXDUZ)	;EP Debugging entry point
	;
	;
	;D DEBUG^%Serenji("RESUSR^BSDX01(.BSDXY,BSDXDUZ)")
	;
	Q
	;
RESUSR(BSDXY,BSDXDUZ)	;EP
	;Returns ADO Recordset with ALL RESOURCE names
	;Inactive RESOURCES are NOT filtered out
	;Called by BSDX RESOURCES BY USER
	;
	N BSDXERR,BSDXRET,BSDXIEN,BSDXRES,BSDXDEP,BSDXDDR,BSDXDEPN,BSDXRDAT,BSDXRNOD,BSDXI,BSDX,BSDXLTR
	N BSDXNOS,BSDXCAN
	K ^BSDXTMP($J)
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S BSDXERR=""
	S ^BSDXTMP($J,BSDXI)="I00010RESOURCEID^T00030RESOURCE_NAME^T00010INACTIVE^I00010TIMESCALE^I00010HOSPITAL_LOCATION_ID^T00030LETTER_TEXT^T00030NO_SHOW_LETTER"
	S ^BSDXTMP($J,BSDXI)=^(BSDXI)_"^T00030CLINIC_CANCELLATION_LETTER^I00010VIEW^I00010OVERBOOK^I00010MODIFY_SCHEDULE^I00010MODIFY_APPOINTMENTS"_$C(30)
	I '+BSDXDUZ S BSDXDUZ=DUZ
	;$O THRU AC XREF OF BSDX RESOURCE USER
	;Rmoved these lines in order to just return all resource names
	;I $D(^BSDXRSU("AC",BSDXDUZ)) S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXRSU("AC",BSDXDUZ,BSDXIEN)) Q:'+BSDXIEN  D
	;. S BSDXRES=$P(^BSDXRSU(BSDXIEN,0),U)
	;
	;$O THRU BSDX RESOURCE File
	S BSDXRES=0 F  S BSDXRES=$O(^BSDXRES(BSDXRES)) Q:'+BSDXRES  D
	. Q:'$D(^BSDXRES(BSDXRES,0))
	. S BSDXRNOD=^BSDXRES(BSDXRES,0)
	. N BSDXSC S BSDXSC=$P(BSDXRNOD,U,4)  ; Hospital Location
	   . ;Q:$P(BSDXRNOD,U,2)=1  ;Inactive resources not filtered
	. ;S BSDXRDAT=$P(BSDXRNOD,U,1,4)
	. ;I '$$INDIV(BSDXSC) QUIT  ; If not in division, quit
	   . K BSDXRDAT
	. F BSDX=1:1:4 S $P(BSDXRDAT,U,BSDX)=$P(BSDXRNOD,U,BSDX)
	. S BSDXRDAT=BSDXRES_U_BSDXRDAT
	. ;Get letter text from wp field
	. S BSDXLTR=""
	. I $D(^BSDXRES(BSDXRES,1)) D
	. . S BSDXIEN=0
	. . F  S BSDXIEN=$O(^BSDXRES(BSDXRES,1,BSDXIEN)) Q:'+BSDXIEN  D
	. . . S BSDXLTR=BSDXLTR_$G(^BSDXRES(BSDXRES,1,BSDXIEN,0))
	. . . S BSDXLTR=BSDXLTR_$C(13)_$C(10)
	. S BSDXNOS=""
	. I $D(^BSDXRES(BSDXRES,12)) D
	. . S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXRES(BSDXRES,12,BSDXIEN)) Q:'+BSDXIEN  D
	. . . S BSDXNOS=BSDXNOS_$G(^BSDXRES(BSDXRES,12,BSDXIEN,0))
	. . . S BSDXNOS=BSDXNOS_$C(13)_$C(10)
	. S BSDXCAN=""
	. I $D(^BSDXRES(BSDXRES,13)) D
	. . S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXRES(BSDXRES,13,BSDXIEN)) Q:'+BSDXIEN  D
	. . . S BSDXCAN=BSDXCAN_$G(^BSDXRES(BSDXRES,13,BSDXIEN,0))
	. . . S BSDXCAN=BSDXCAN_$C(13)_$C(10)
	. N BSDXACC,BSDXMGR
	. S BSDXACC="0^0^0^0"
	. S BSDXMGR=$O(^DIC(19.1,"B","BSDXZMGR",0))
	. I +BSDXMGR,$D(^VA(200,BSDXDUZ,51,BSDXMGR)) S BSDXACC="1^1^1^1"
	. S BSDXMGR=$O(^DIC(19.1,"B","XUPROGMODE",0))
	. I +BSDXMGR,$D(^VA(200,BSDXDUZ,51,BSDXMGR)) S BSDXACC="1^1^1^1"
	. I BSDXACC="0^0^0^0" D
	. . N BSDXNOD,BSDXRUID
	. . S BSDXRUID=0
	. . ;Get entry for this user and resource
	. . F  S BSDXRUID=$O(^BSDXRSU("AC",BSDXDUZ,BSDXRUID)) Q:'+BSDXRUID  I $D(^BSDXRSU(BSDXRUID,0)),$P(^(0),U)=BSDXRES Q
	. . Q:'+BSDXRUID
	. . S $P(BSDXACC,U)=1
	. . S BSDXNOD=$G(^BSDXRSU(BSDXRUID,0))
	. . S $P(BSDXACC,U,2)=+$P(BSDXNOD,U,3)
	. . S $P(BSDXACC,U,3)=+$P(BSDXNOD,U,4)
	. . S $P(BSDXACC,U,4)=+$P(BSDXNOD,U,5)
	. S BSDXRDAT=BSDXRDAT_U_BSDXLTR_U_BSDXNOS_U_BSDXCAN_U_BSDXACC
	. S BSDXI=BSDXI+1
	. S ^BSDXTMP($J,BSDXI)=BSDXRDAT_$C(30)
	S ^BSDXTMP($J,BSDXI+1)=$C(31)_BSDXERR
	Q
	;
DEPRESD(BSDXY,BSDXDUZ)	;EP Debugging entry point
	;
	;
	;D DEBUG^%Serenji("DEPRES^BSDX01(.BSDXY,BSDXDUZ)")
	;
	Q
	;
DEPRES(BSDXY,BSDXDUZ)	;EP
	;Called by BSDX GROUP RESOURCE
	;Returns ADO Recordset with all ACTIVE GROUP/RESOURCE combinations
	;to which user has access based on entries in BSDX RESOURCE USER file
	;If BSDXDUZ=0 then returns all ACTIVE GROUP/RESOURCE combinations for current DUZ
	;If user BSDXDUZ possesses the key BSDXZMGR or XUPROGMODE
	;then ALL ACTIVE resource group names are returned
	;
	N BSDXERR,BSDXRET,BSDXIEN,BSDXRES,BSDXDEP,BSDXDDR,BSDXDEPN,BSDXRDAT,BSDXRNOD,BSDXI
	N BSDXRESN,BSDXMGR,BSDXRESD,BSDXNOD,BSDXSUBID
	K ^BSDXTEMP($J)
	K ^BSDXTMP($J)
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S BSDXERR=""
	S ^BSDXTMP($J,BSDXI)="I00020RESOURCE_GROUPID^T00030RESOURCE_GROUP^I00020RESOURCE_GROUP_ITEMID^T00030RESOURCE_NAME^I00020RESOURCEID"_$C(30)
	I '+BSDXDUZ S BSDXDUZ=DUZ
	;Check SECURITY KEY file for BSDXZMGR or XUPROGMODE keys
	S BSDXMGR=$$APSEC("BSDXZMGR",BSDXDUZ)
	;
	;User does not have BSDXZMGR or XUPROGMODE keys, so
	;$O THRU AC XREF OF BSDX RESOURCE USER
	I 'BSDXMGR,$D(^BSDXRSU("AC",BSDXDUZ))  S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXRSU("AC",BSDXDUZ,BSDXIEN)) Q:'+BSDXIEN  D
	. S BSDXRES=$P(^BSDXRSU(BSDXIEN,0),U)
	. Q:'$D(^BSDXDEPT("AB",BSDXRES))  ; Quit if Resource isn't part of any Group
	   . ;Q:'$$INDIV2(BSDXRES)  ; Quit if Resource isn't in same division as user.
	. S BSDXRNOD=$G(^BSDXRES(BSDXRES,0))
	. Q:BSDXRNOD=""
	. ;QUIT if the resource is inactive
	. Q:$P(BSDXRNOD,U,2)=1
	. S BSDXRESN=$P(BSDXRNOD,U)
	. S BSDXDEP=0 F  S BSDXDEP=$O(^BSDXDEPT("AB",BSDXRES,BSDXDEP)) Q:'+BSDXDEP  D
	. . Q:'$D(^BSDXDEPT(BSDXDEP,0))
	. . S BSDXDEPN=$P(^BSDXDEPT(BSDXDEP,0),U)
	. . S BSDXSUBID=$O(^BSDXDEPT(BSDXDEP,1,"B",BSDXRES,0))
	. . S BSDXI=BSDXI+1
	. . S ^BSDXTMP($J,BSDXI)=BSDXDEP_U_BSDXDEPN_U_BSDXSUBID_U_BSDXRESN_U_BSDXRES_$C(30)
	. Q
	;
	;User does have BSDXZMGR or XUPROGMODE keys, so
	;$O THRU BSDX RESOURCE GROUP file directly
	I BSDXMGR S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXDEPT(BSDXIEN)) Q:'+BSDXIEN  D
	. Q:'$D(^BSDXDEPT(BSDXIEN,0))
	. S BSDXNOD=^BSDXDEPT(BSDXIEN,0)
	. S BSDXDEPN=$P(BSDXNOD,U)
	. S BSDXRES=0 F  S BSDXRES=$O(^BSDXDEPT(BSDXIEN,1,BSDXRES)) Q:'+BSDXRES  D
	. . N BSDXRESD
	. . Q:'$D(^BSDXDEPT(BSDXIEN,1,BSDXRES,0))  ; Quit if zero node is invalid in multiple
	. . S BSDXRESD=$P(^BSDXDEPT(BSDXIEN,1,BSDXRES,0),"^")
	. . Q:'$D(^BSDXRES(BSDXRESD,0))  ; Quit if zero node of resouce file is invalid
	   . . ;Q:'$$INDIV2(BSDXRESD)  ; Quit if resource is not in the same division
	. . S BSDXRNOD=$G(^BSDXRES(BSDXRESD,0))
	. . Q:BSDXRNOD=""
	. . ;QUIT if the resource is inactive
	. . Q:$P(BSDXRNOD,U,2)=1
	. . S BSDXRESN=$P(BSDXRNOD,U)
	. . S BSDXI=BSDXI+1
	. . S ^BSDXTMP($J,BSDXI)=BSDXIEN_U_BSDXDEPN_U_BSDXRES_U_BSDXRESN_U_BSDXRESD_$C(30)
	. . Q
	. Q
	;
	S ^BSDXTMP($J,BSDXI+1)=$C(31)_BSDXERR
	Q
	;
APSEC(BSDXKEY,BSDXDUZ)	;EP - Return TRUE (1) if user has keys BSDXKEY or XUPROGMODE, otherwise, returns FALSE (0)
	;
	N BSDXIEN,BSDXPROG,BSDXPKEY
	I '$G(BSDXDUZ) Q 0
	;
	;Test for programmer mode key
	S BSDXPROG=0
	I $D(^DIC(19.1,"B","XUPROGMODE")) D
	. S BSDXPKEY=$O(^DIC(19.1,"B","XUPROGMODE",0))
	. I '+BSDXPKEY Q
	. I '$D(^VA(200,BSDXDUZ,51,BSDXPKEY,0)) Q
	. S BSDXPROG=1
	I BSDXPROG Q 1
	;
	I BSDXKEY="" Q 0
	I '$D(^DIC(19.1,"B",BSDXKEY)) Q 0
	S BSDXIEN=$O(^DIC(19.1,"B",BSDXKEY,0))
	I '+BSDXIEN Q 0
	I '$D(^VA(200,BSDXDUZ,51,BSDXIEN,0)) Q 0
	Q 1
SP(BSDXY,PARAM,YESNO)	; Save Param at User Level - EP
	; Called by RPC: BSDX SET PARAM
	; Input:
	; - Param: Name of Parameter (prog name of course)
	; - Yes/No: 1 or 0
	; Output: Error Code as string; 0 is good
	;
	; Security Protection
	IF $EXTRACT(PARAM,1,4)'="BSDX" S BSDXY="-1^BSDX Params only allowed" QUIT
	;
	N ERROR
	D PUT^XPAR("USR",PARAM,1,YESNO,.ERROR)
	S BSDXY=$G(ERROR)
	QUIT
	;
GP(BSDXY,PARAM)	; Get Param - EP
	; Called by RPC: BSDX GET PARAM
	; Input: Name of Parameter
	; Output: Value of parameter: 0 or 1, for now.
	;
	S BSDXY=$$GET^XPAR("USR^LOC^SYS^PKG",PARAM,1,"I")
	QUIT
	;
INDIV(BSDXSC)	; PEP - Is ^SC clinic in the same DUZ(2) as user?
	; Input: BSDXSC - Hospital Location IEN
	; Output: True or False
	I '+BSDXSC QUIT 1  ;If not tied to clinic, yes
	I '$D(^SC(BSDXSC,0)) QUIT 1 ; If Clinic does not exist, yes
	; Jump to Division:Medical Center Division:Inst File Pointer for
	; Institution IEN (and get its internal value)
	N DIV S DIV=$$GET1^DIQ(44,BSDXSC_",","3.5:.07","I")
	I DIV="" Q 1 ; If clinic has no division, consider it avial to user.
	I DIV=DUZ(2) Q 1 ; If same, then User is in same Div as Clinic
	E  Q 0 ; Otherwise, no
INDIV2(BSDXRES)	; PEP - Is Resource in the same DUZ(2) as user?
	; Input BSDXRES - BSDX RESOURCE IEN
	; Output: True of False
	Q $$INDIV($P($G(^BSDXRES(BSDXRES,0)),U,4)) ; Extract Hospital Location and send to $$INDIV
UTINDIV	; Unit Test $$INDIV
	W "Testing if they are the same",!
	S DUZ(2)=67
	I '$$INDIV(1) W "ERROR",!
	I '$$INDIV(2) W "ERROR",!
	W "Testing if Div not defined in 44, should be true",!
	I '$$INDIV(3) W "ERROR",!
	W "Testing empty string. Should be true",!
	I '$$INDIV("") W "ERROR",!
	W "Testing if they are different",!
	S DUZ(2)=899
	I $$INDIV(1) W "ERROR",!
	I $$INDIV(2) W "ERROR",!
	QUIT
UTINDIV2	; Unit Test $$INDIV2
	W "Testing if they are the same",!
	S DUZ(2)=69
	I $$INDIV2(22)'=0 W "ERROR",!
	I $$INDIV2(25)'=1 W "ERROR",!
	I $$INDIV2(26)'=1 W "ERROR",!
	I $$INDIV2(27)'=1 W "ERROR",!
	QUIT
	;
GETRADEX(BSDXY,DFN,SCIEN)	; Get All Pending and On Hold Radiology Exams for Patient; RPC EP; UJO/SMH new in v 1.6
	; RPC: BSDX GET RAD EXAM FOR PT; Return: Global Array
	;
	; Input: DFN - you should know; SCIEN - IEN of Hospital Location
	; Output: ADO Datatable with the following columns:
	; - BMXIEN: Radiology Exam IEN in file 75.1 (RAD/NUC MED ORDERS)
	; - STATUS: Pending Or Hold Status
	; - PROCEDURE: Text Procedure Name
	; - REQUEST_DATE: Date Procedure was requested
	;
	; Error Processing: Silent failure. 
	;
	S BSDXY=$NA(^BMXTEMP($J))
	K @BSDXY
	;
	N BSDXI S BSDXI=0
	S @BSDXY@(BSDXI)="I00015BMXIEN^T00015STATUS^T00100PROCEDURE^D00030REQUEST_DATE"_$C(30)
	;
	N BSDXRLIEN S BSDXRLIEN=$ORDER(^RA(79.1,"B",SCIEN,""))  ; IEN of HL in file 79.1, to get Radiology Imaging IEN
	I 'BSDXRLIEN GOTO END
	;
	N BSDXOUT,BSDXERR ; Out, Error
	;
	; File 75.1 = RAD/NUC MED ORDERS
	; Fields 5 = Request Status; 2 = Procedure; 16 = Requested Entered Date Time
	; Filter Field: First piece is DFN, 5th piece is 3 or 5 (Status of Pending Or Hold); 20th piece is Radiology Location requested
	D LIST^DIC(75.1,"","@;5;2;16","P","","","","B","I $P(^(0),U)=DFN&(35[$P(^(0),U,5))&($P(^(0),U,20)=BSDXRLIEN)","","BSDXOUT","BSDXERR")
	;
	IF $DATA(BSDXERR) GOTO END
	;
	I +BSDXOUT("DILIST",0)>0 FOR BSDXI=1:1:+BSDXOUT("DILIST",0) DO  ; if we have data, fetch the data in each row and store it in the return variable
	. N BMXIEN,BMXSTAUS,BMXPROC,BMXDATE ; Proc IEN, Proc Status, Proc Name
	. S BMXIEN=$P(BSDXOUT("DILIST",BSDXI,0),U) ; IEN
	. S BMXSTATUS=$P(BSDXOUT("DILIST",BSDXI,0),U,2) ; Status
	. S BMXPROC=$P(BSDXOUT("DILIST",BSDXI,0),U,3) ; Procedure Name
	. S BMXDATE=$TR($P(BSDXOUT("DILIST",BSDXI,0),U,4),"@"," ") ; Request Entered Date Time
	. S @BSDXY@(BSDXI)=BMXIEN_U_BMXSTATUS_U_BMXPROC_U_BMXDATE_$C(30)
END	; Errors Jump Here...
	S @BSDXY@(BSDXI+1)=$C(31)
	QUIT
	;
SCHRAEX(BSDXY,RADFN,RAOIFN,RAOSCH)	; Schedule a Radiology Exam; RPC EP; UJO/SMH new in v 1.6
	; RPC: BSDX SCHEDULE RAD EXAM; Return: Single Value
	;
	; Input: 
	; - RADFN -> DFN
	; - RAOIFN -> Radiology Order IEN in file 75.1
	; - RAOSCH -> Scheduled Time for Exam
	; Output: Always "1"
	;
	S RAOSCH=+RAOSCH ; Strip the trailing zeros from the Fileman Date produced by C#
	N RAOSTS S RAOSTS=8  ; Status of Scheduled
	D ^RAORDU  ; API in Rad expects RADFN, RAOIFN, RAOSCH, and RAOSTS
	S BSDXY=1 ; Success
	QUIT
	;
HOLDRAEX(BSDXY,RADFN,RAOIFN)	; Hold a Radiology Exam; RPC EP; UJO/SMH new in v 1.6
	; RPC: BSDX HOLD RAD EXAM; Return: Single Value
	;
	; Input:
	; - RADFN -> DFN
	; - RAOIFN -> Radiology Order IEN in file 75.1
	; Output: 1 OR 0 for success or failure.
	; Can we hold?
	N CANHOLD
	D CANHOLD(.CANHOLD,RAOIFN)
	I 'CANHOLD S BSDXY=0 QUIT
	;
	N RAOSTS S RAOSTS=3  ; Status of Hold
	N RAOREA ; Reason, stored in file 75.2
	I $D(^RA(75.2,100)) S RAOREA=100  ; Custom site Reason
	E  I $D(^RA(75.2,20))  S RAOREA=20 ; Reason: Exam Cancelled
	E   ; Else is empty. I won't set RAOREA at all.
	D ^RAORDU
	S BSDXY=1 ; Success
	QUIT
	;
CANHOLD(BSDXY,RAOIFN)	; Can we hold this Exam? RPC EP; UJO/SMH new in 1.6
	; RPC: BSDX CAN HOLD RAD EXAM; Return: Single Value
	;
	; Input:
	; - RAOIFN -> Radiology Order IEN in file 75.1
	; Output: 0 or 1 for false or true
	;
	N STATUS S STATUS=$$GET1^DIQ(75.1,RAOIFN,"REQUEST STATUS","I")
	; 1 = discontinued; 2 = Complete; 6 = Active
	; if any one of these, cannot hold exam; otherwise, we can
	I 126[STATUS S BSDXY=0 QUIT
	ELSE  S BSDXY=1 QUIT
	QUIT
