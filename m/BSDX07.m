BSDX07	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ;  ; 7/18/10 2:11pm
	;;1.4;BSDX;;Sep 07, 2010
	   ;
	   ; Change Log:
	   ; UJO/SMH
	   ; v1.3 July 13 2010 - Add support i18n - Dates input as FM dates, not US.
	;
	;
APPADDD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)	;EP
	;Entry point for debugging
	;
	I +$G(^HWDEBUG("BREAK","APPADD")),+$G(^HWDEBUG("BREAK"))=DUZ D DEBUG^%Serenji("APPADD^BSDX07(.BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)",$P(^HWDEBUG("BREAK"),U,2))
	E  G ENDBG
	Q
	;
APPADD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)	;EP
	;Called by BSDX ADD NEW APPOINTMENT
	;Add new appointment
	;BSDXRES is ResourceName
	;BSDXLEN is the appointment duration in minutes
	;BSDXATID is used for 2 purposes:
	; if BSDXATID = "WALKIN" then BSDAPI is called to create a walkin appt.
	; if BSDXATID = a number, then it is the access type id (used for rebooking)
	;
	;Create entry in BSDX APPOINTMENT
	;Returns recordset having fields
	; AppointmentID and ErrorNumber
	;
	;Test lines:
ENDBG	;BSDX ADD NEW APPOINTMENT^3091122.0930^3091122.1000^370^2^PEDIATRICIAN,DEMO^EXAM^SCRATCH NOTE
	;
	N BSDXERR,BSDXIEN,BSDXDEP,BSDXI,BSDXJ,BSDXAPPTI,BSDXDJ,BSDXRESD,BSDXRNOD,BSDXSCD,BSDXC,BSDXERR,BSDXWKIN
	N BSDXNOEV
	S BSDXNOEV=1 ;Don't execute BSDX ADD APPOINTMENT protocol
	K ^BSDXTMP($J)
	S X="ETRAP^BSDX07",@^%ZOSF("TRAP")
	S BSDXERR=0
	S BSDXI=0
	S BSDXY="^BSDXTMP("_$J_")"
	S ^BSDXTMP($J,BSDXI)="I00020APPOINTMENTID^T00020ERRORID"_$C(30)
	S BSDXI=BSDXI+1
	;
	;Lock BSDX node
	L +^BSDXAPPT(BSDXPATID):5 I '$T D ERR(BSDXI+1,"Another user is working with this patient's record.  Please try again later") Q
	;
	TSTART
	   ; v1.3 - date passed in as FM Date, not US date.
	;Check input data for errors
	; S:BSDXSTART["@0000" BSDXSTART=$P(BSDXSTART,"@")
	; S:BSDXEND["@0000" BSDXEND=$P(BSDXEND,"@")
	; S %DT="T",X=BSDXSTART D ^%DT S BSDXSTART=Y
	; I BSDXSTART=-1 D ERR(BSDXI+1,"BSDX07 Error: Invalid Start Time") Q
	; S %DT="T",X=BSDXEND D ^%DT S BSDXEND=Y
	; I BSDXEND=-1 D ERR(BSDXI+1,"BSDX07 Error: Invalid End Time") Q
	   ;
	   ; If C# sends the dates with extra zeros, remove them
	S BSDXSTART=+BSDXSTART,BSDXEND=+BSDXEND
	   ;
	   I $L(BSDXEND,".")=1 D ERR(BSDXI+1,"BSDX07 Error: Invalid End Time") Q
	I BSDXSTART>BSDXEND S BSDXTMP=BSDXEND,BSDXEND=BSDXSTART,BSDXSTART=BSDXTMP
	I '+BSDXPATID,'$D(^DPT(BSDXPATID,0)) D ERR(BSDXI+1,"BSDX07 Error: Invalid Patient ID") Q
	;Validate Resource entry
	S BSDXERR=0 K BSDXRESD
	I '$D(^BSDXRES("B",BSDXRES)) D ERR(BSDXI+1,"BSDX07 Error: Invalid Resource ID") Q
	S BSDXRESD=$O(^BSDXRES("B",BSDXRES,0))
	S BSDXWKIN=0
	I BSDXATID="WALKIN" S BSDXWKIN=1
	I BSDXATID'?.N&(BSDXATID'="WALKIN") S BSDXATID=""
	;
	S BSDXAPPTID=$$BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID)
	I 'BSDXAPPTID D ERR(BSDXI+1,"BSDX07 Error: Unable to add appointment to BSDX APPOINTMENT file.") Q
	I BSDXNOTE]"" D BSDXWP(BSDXAPPTID,BSDXNOTE)
	;
	;Create RPMS Appointment
	S BSDXRNOD=$G(^BSDXRES(BSDXRESD,0))
	;I BSDXRNOD="" D ERR(BSDXI+1,"BSDX07 Error: Unable to add appointment -- invalid Resource entry."),BSDXDEL(BSDXAPPTID) Q
	I BSDXRNOD="" D ERR(BSDXI+1,"BSDX07 Error: Unable to add appointment -- invalid Resource entry.") Q
	S BSDXSCD=$P(BSDXRNOD,U,4)
	;I +BSDXSCD,$D(^SC(BSDXSCD,0)) D  I +BSDXERR D ERR(BSDXI+1,"BSDX07 Error: Unable to make appointment.  MAKE^BSDAPI returned error code: "_BSDXERR),BSDXDEL(BSDXAPPTID) Q
	I +BSDXSCD,$D(^SC(BSDXSCD,0)) D  I +BSDXERR D ERR(BSDXI+1,"BSDX07 Error: Unable to make appointment.  MAKE^BSDAPI returned error code: "_BSDXERR) Q
	. S BSDXC("PAT")=BSDXPATID
	. S BSDXC("CLN")=BSDXSCD
	. S BSDXC("TYP")=3 ;3 for scheduled appts, 4 for walkins
	. S:BSDXWKIN BSDXC("TYP")=4
	. S BSDXC("ADT")=BSDXSTART
	. S BSDXC("LEN")=BSDXLEN
	. S BSDXC("OI")=$E($G(BSDXNOTE),1,150) ;File 44 has 150 character limit on OTHER field
	. S BSDXC("OI")=$TR(BSDXC("OI"),";"," ") ;No semicolons allowed by MAKE^BSDAPI
	. S BSDXC("OI")=$$STRIP(BSDXC("OI")) ;Strip control characters from note
	. S BSDXC("USR")=DUZ
	. S BSDXERR=$$MAKE^BSDXAPI(.BSDXC)
	. Q:BSDXERR
	. D AVUPDT(BSDXSCD,BSDXSTART,BSDXLEN)
	. ;L
	. Q
	;
	;Update RPMS Clinic availability
	;Return Recordset
	TCOMMIT
	L -^BSDXAPPT(BSDXPATID)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXAPPTID_"^"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
BSDXDEL(BSDXAPPTID)	;Deletes appointment BSDXAPPTID from BSDXAPPOINTMETN
	N DA,DIK
	S DIK="^BSDXAPPT(",DA=BSDXAPPTID
	D ^DIK
	Q
	;
STRIP(BSDXZ)	;Replace control characters with spaces
	N BSDXI
	F BSDXI=1:1:$L(BSDXZ) I (32>$A($E(BSDXZ,BSDXI))) S BSDXZ=$E(BSDXZ,1,BSDXI-1)_" "_$E(BSDXZ,BSDXI+1,999)
	Q BSDXZ
	;
BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID)	;ADD BSDX APPOINTMENT ENTRY
	;Returns ien in BSDXAPPT or 0 if failed
	;Create entry in BSDX APPOINTMENT
	N BSDXAPPTID
	S BSDXFDA(9002018.4,"+1,",.01)=BSDXSTART
	S BSDXFDA(9002018.4,"+1,",.02)=BSDXEND
	S BSDXFDA(9002018.4,"+1,",.05)=BSDXPATID
	S BSDXFDA(9002018.4,"+1,",.07)=BSDXRESD
	S BSDXFDA(9002018.4,"+1,",.08)=$G(DUZ)
	;S BSDXFDA(9002018.4,"+1,",.09)=$G(DT) ;MJL 1/25/2007
	S BSDXFDA(9002018.4,"+1,",.09)=$$NOW^XLFDT
	S:BSDXATID="WALKIN" BSDXFDA(9002018.4,"+1,",.13)="y"
	S:BSDXATID?.N BSDXFDA(9002018.4,"+1,",.06)=BSDXATID
	K BSDXIEN,BSDXMSG
	D UPDATE^DIE("","BSDXFDA","BSDXIEN","BSDXMSG")
	S BSDXAPPTID=+$G(BSDXIEN(1))
	Q BSDXAPPTID
	;
BSDXWP(BSDXAPPTID,BSDXNOTE)	;
	;Add WP field
	I BSDXNOTE]"" S BSDXNOTE(.5)=BSDXNOTE,BSDXNOTE=""
	I $D(BSDXNOTE(0)) S BSDXNOTE(.5)=BSDXNOTE(0) K BSDXNOTE(0)
	I $D(BSDXNOTE(.5)) D
	. D WP^DIE(9002018.4,BSDXAPPTID_",",1,"","BSDXNOTE","BSDXMSG")
	Q
	;
ADDEVT(BSDXPATID,BSDXSTART,BSDXSC,BSDXSCDA)	;EP
	;Called by BSDX ADD APPOINTMENT protocol
	;BSDXSC=IEN of clinic in ^SC
	;BSDXSCDA=IEN for ^SC(BSDXSC,"S",BSDXSTART,1,BSDXSCDA). Use to get Length & Note
	;
	N BSDXNOD,BSDXLEN,BSDXAPPTID,BSDXNODP,BSDXWKIN,BSDXRES
	Q:+$G(BSDXNOEV)
	I $D(^BSDXRES("ALOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ALOC",BSDXSC,0))
	E  I $D(^BSDXRES("ASSOC",BSDXSC)) S BSDXRES=$O(^BSDXRES("ASSOC",BSDXSC,0))
	Q:'+$G(BSDXRES)
	S BSDXNOD=$G(^SC(BSDXSC,"S",BSDXSTART,1,BSDXSCDA,0))
	Q:BSDXNOD=""
	S BSDXNODP=$G(^DPT(BSDXPATID,"S",BSDXSTART,0))
	S BSDXWKIN=""
	S:$P(BSDXNODP,U,7)=4 BSDXWKIN="WALKIN" ;Purpose of Visit field of DPT Appointment subfile
	S BSDXLEN=$P(BSDXNOD,U,2)
	Q:'+BSDXLEN
	S BSDXEND=$$FMADD^XLFDT(BSDXSTART,0,0,BSDXLEN,0)
	S BSDXAPPTID=$$BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXWKIN)
	Q:'+BSDXAPPTID
	S BSDXNOTE=$P(BSDXNOD,U,4)
	I BSDXNOTE]"" D BSDXWP(BSDXAPPTID,BSDXNOTE)
	D ADDEVT3(BSDXRES)
	Q
	;
ADDEVT3(BSDXRES)	;
	;Call RaiseEvent to notify GUI clients
	N BSDXRESN
	S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	Q:BSDXRESN=""
	S BSDXRESN=$P(BSDXRESN,"^")
	;D EVENT^BSDX23("SCHEDULE-"_BSDXRESN,"","","")
	D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	Q
	;
ERR(BSDXI,BSDXERR)	;Error processing
	D ^%ZTER ;XXX: remove after we figure out the cause of error
	   S BSDXI=BSDXI+1
	S BSDXERR=$TR(BSDXERR,"^","~")
	TROLLBACK
	S ^BSDXTMP($J,BSDXI)="0^"_BSDXERR_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	L
	Q
	;
ETRAP	;EP Error trap entry
	D ^%ZTER
	I '$D(BSDXI) N BSDXI S BSDXI=999999
	S BSDXI=BSDXI+1
	D ERR(BSDXI,"BSDX07 Error: "_$G(%ZTERROR))
	Q
	;
DAY	;;^SUN^MON^TUES^WEDNES^THURS^FRI^SATUR
	;
DOW	S %=$E(X,1,3),Y=$E(X,4,5),Y=Y>2&'(%#4)+$E("144025036146",Y)
	F %=%:-1:281 S Y=%#4=1+1+Y
	S Y=$E(X,6,7)+Y#7
	Q
	;
AVUPDT(BSDXSCD,BSDXSTART,BSDXLEN)	;Update RPMS Clinic availability
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
