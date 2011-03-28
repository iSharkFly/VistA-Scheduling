BSDX02	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 3/21/11 11:49am
	;;1.5V3;BSDX;;Mar 16, 2011
	   ; 
	   ; Change Log
	   ; July 15 2010: UJO/SMH - Pass FM dates in instead of US dates for i18n
	; March 21 2011: UJO/SMH (v 1.5) - Return new fields: Patient SEX, PID, and DOB
	;
	;
CRSCHD(BSDXY,BSDXRES,BSDXSTART,BSDXEND)	;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("CRSCH^BSDX02(.BSDXY,BSDXRES,BSDXSTART,BSDXEND)")
	Q
	;
CRSCH(BSDXY,BSDXRES,BSDXSTART,BSDXEND,BSDXWKIN)	    ;
	;Called by BSDX CREATE APPT SCHEDULE
	;Create Resource Appointment Schedule recordset
	;On error, returns 0 in APPOINTMENTID field and error text in NOTE field
	;
	;$O Thru ^BSDXAPPT("ARSRC", RESOURCE, STARTTIME, APPTID)
	;BMXRES is a | delimited list of resource names
	;BSDXWKIN - If 1, then return walkins, otherwise skip them
	;9-27-2004 Added walkin to returned datatable
	;TODO: Change BSDXRES from names to IDs
	;
	N BSDXERR,BSDXIEN,BSDXDEPD,BSDXDEPN,BSDXRESD,BSDXI,BSDXJ,BSDXRESN,BSDXS,BSDXAD,BSDXZ,BSDXQ,BSDXNOD
	N BSDXPAT,BSDXNOT,BSDXZPCD,BSDXPCD
	K ^BSDXTMP($J)
	S BSDXERR=""
	S BSDXY="^BSDXTMP("_$J_")"
	S ^BSDXTMP($J,0)="I00020APPOINTMENTID^D00030START_TIME^D00030END_TIME^D00030CHECKIN^D00030AUXTIME^I00020PATIENTID^T00030PATIENTNAME^T00030RESOURCENAME^I00005NOSHOW^T00020HRN^I00005ACCESSTYPEID^I00005WALKIN^T00250NOTE^T00006SEX^T00040PID^D00030DOB"_$C(30)
	D ^XBKVAR S X="ETRAP^BSDX02",@^%ZOSF("TRAP")
	;
	; S %DT="T",X=BSDXSTART D ^%DT S BSDXSTART=Y
	; I BSDXSTART=-1 S ^BSDXTMP($J,1)=$C(31) Q
	; S %DT="T",X=BSDXEND D ^%DT S BSDXEND=Y
	; I BSDXEND=-1 S ^BSDXTMP($J,1)=$C(31) Q
	   ;
	S BSDXI=0
	D STRES
	;
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
STRES	;
	F BSDXJ=1:1:$L(BSDXRES,"|") S BSDXRESN=$P(BSDXRES,"|",BSDXJ) D
	. Q:BSDXRESN=""
	. Q:'$D(^BSDXRES("B",BSDXRESN))
	. S BSDXRESD=$O(^BSDXRES("B",BSDXRESN,0))
	. Q:'+BSDXRESD
	. Q:'$D(^BSDXAPPT("ARSRC",BSDXRESD))
	. S BSDXS=BSDXSTART-.0001
	. F  S BSDXS=$O(^BSDXAPPT("ARSRC",BSDXRESD,BSDXS)) Q:'+BSDXS  Q:BSDXS>BSDXEND  D
	. . S BSDXAD=0 F  S BSDXAD=$O(^BSDXAPPT("ARSRC",BSDXRESD,BSDXS,BSDXAD)) Q:'+BSDXAD  D STCOMM(BSDXAD,BSDXRESN)
	Q
	;
STCOMM(BSDXAD,BSDXRESN)	     ;
	;BSDXAD is the appointment IEN
	N BSDXC,BSDXQ,BSDXZ,BSDXSUBC,BSDXHRN,BSDXPATD,BSDXATID,BSDXISWK
	Q:'$D(^BSDXAPPT(BSDXAD,0))
	S BSDXNOD=^BSDXAPPT(BSDXAD,0)
	Q:$P(BSDXNOD,U,12)]""  ;CANCELLED
	S BSDXISWK=0
	S:$P(BSDXNOD,U,13)="y" BSDXISWK=1
	I +$G(BSDXWKIN) Q:BSDXISWK  ;Don't return walkins if appt is WALKIN and BSDXWKIN is 1
	S BSDXZ=BSDXAD_"^"
	F BSDXQ=1:1:4 D
	. S Y=$P(BSDXNOD,U,BSDXQ)
	. X ^DD("DD") S Y=$TR(Y,"@"," ")
	. S BSDXZ=BSDXZ_Y_"^"
	S BSDXPATD=$P(BSDXNOD,U,5)
	S BSDXZ=BSDXZ_BSDXPATD_"^" ;PATIENT ID
	S BSDXPAT=""
	I BSDXPATD]"",$D(^DPT(BSDXPATD,0)) S BSDXPAT=$P(^DPT(BSDXPATD,0),U)
	S BSDXZ=BSDXZ_BSDXPAT_"^" ;PATIENT NAME
	S BSDXZ=BSDXZ_BSDXRESN_"^" ;RESOURCENAME
	S BSDXZ=BSDXZ_+$P(BSDXNOD,U,10)_"^" ;NOSHOW
	S BSDXHRN=""
	I $D(DUZ(2)),DUZ(2)>0 S BSDXHRN=$P($G(^AUPNPAT(BSDXPATD,41,DUZ(2),0)),U,2) ;HRN
	S BSDXZ=BSDXZ_BSDXHRN_"^"
	S BSDXATID=$P(BSDXNOD,U,6)
	S:'+BSDXATID BSDXATID=0 ;UNKNOWN TYPE
	S BSDXZ=BSDXZ_BSDXATID_"^"_BSDXISWK_"^"
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXZ
	;NOTE
	S BSDXNOT="",BSDXQ=0 F  S BSDXQ=$O(^BSDXAPPT(BSDXAD,1,BSDXQ)) Q:'+BSDXQ  D
	. S BSDXNOT=$G(^BSDXAPPT(BSDXAD,1,BSDXQ,0))
	. S:$E(BSDXNOT,$L(BSDXNOT)-1,$L(BSDXNOT))'=" " BSDXNOT=BSDXNOT_" "
	. S BSDXI=BSDXI+1
	. S ^BSDXTMP($J,BSDXI)=BSDXNOT
	S ^BSDXTMP($J,BSDXI)=^BSDXTMP($J,BSDXI)_U ; Add "^" to separate note from next fields.
	S BSDXI=BSDXI+1
	; new code for V1.5. Extra fields to return.
	N SEX S SEX=$$GET1^DIQ(2,BSDXPATD,.02)  ; SEX
	N PID S PID=$$GET1^DIQ(2,BSDXPATD,.363) ; PRIMARY LONG ID
	; Note strange way I retrieve the value. B/c DOB Output Transform
	; Outputs it in MM/DD/YYYY format, which is ambigous for C#.
	N DOB S DOB=$$FMTE^XLFDT($$GET1^DIQ(2,BSDXPATD,.03,"I"))  ; DOB
	S ^BSDXTMP($J,BSDXI)=SEX_U_PID_U_DOB_$C(30)
	; end new code
	Q
	;
ERR(BSDXI,BSDXERR)	;Error processing
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="0^^^^^^^^^^^"_BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
ETRAP	;EP Error trap entry
	D ^%ZTER
	I '$D(BSDXI) N BSDXI S BSDXI=999999
	S BSDXI=BSDXI+1
	D ERR(BSDXI,"BSDX31 Error: "_$G(%ZTERROR))
	Q
