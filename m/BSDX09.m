BSDX09	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ;  ; 6/21/12 11:03am
	;;1.7;BSDX;;Jun 01, 2013;Build 24
	; Licensed under LGPL
	;
	; Change Log:
	; UJO/TH - v 1.3 on 3100714 - Extra Demographics:
	; - Email
	; - Cell Phone
	; - Country
	; - + refactoring of routine
	; 
	; UJO/TH - v 1.3 on 3100715 - Change SSN to PID and get PID field instead
	;
	   ; UJO/TH - v 1.42 on 3101020 - Add Sex field.
	;
GETREGA(BSDXRET,BSDXPAT)	       ;EP
	;
	; See below for the returned fields
	;
	;For patient with ien BSDXPAT
	;K ^BSDXTMP($J)
	S BSDXERR=""
	S BSDXRET="^BSDXTMP("_$J_")"
	;
	N OUT S OUT=$NA(^BSDXTMP($J,0))
	S $P(@OUT,U,1)="T00030IEN"
	S $P(@OUT,U,2)="T00030STREET"
	S $P(@OUT,U,3)="T00030CITY"
	S $P(@OUT,U,4)="T00030STATE"
	S $P(@OUT,U,5)="T00030ZIP"
	S $P(@OUT,U,6)="T00030NAME"
	S $P(@OUT,U,7)="D00030DOB"
	S $P(@OUT,U,8)="T00030PID"
	S $P(@OUT,U,9)="T00030HRN"
	S $P(@OUT,U,10)="T00030HOMEPHONE"
	S $P(@OUT,U,11)="T00030OFCPHONE"
	S $P(@OUT,U,12)="T00030MSGPHONE"
	S $P(@OUT,U,13)="T00030NOK NAME"
	S $P(@OUT,U,14)="T00030RELATIONSHIP"
	S $P(@OUT,U,15)="T00030PHONE"
	S $P(@OUT,U,16)="T00030STREET"
	S $P(@OUT,U,17)="T00030CITY"
	S $P(@OUT,U,18)="T00030STATE"
	S $P(@OUT,U,19)="T00030ZIP"
	S $P(@OUT,U,20)="D00030DATAREVIEWED"
	S $P(@OUT,U,21)="T00030RegistrationComments"
	S $P(@OUT,U,22)="T00050EMAIL ADDRESS"
	S $P(@OUT,U,23)="T00020PHONE NUMBER [CELLULAR]"
	S $P(@OUT,U,24)="T00030COUNTRY"
	S $P(@OUT,U,25)="T00030SEX"
	S $E(@OUT,$L(@OUT)+1)=$C(30)
	;
	;
	N BSDXNOD,BSDXNAM,Y,U
	S U="^"
	S BSDXY="ERROR"
	K NAME
	I '+BSDXPAT S ^BSDXTMP($J,1)=$C(31) Q
	I '$D(^DPT(+BSDXPAT,0)) S ^BSDXTMP($J,1)=$C(31) Q
	S BSDXY=""
	S $P(BSDXY,U)=BSDXPAT
	;//smh S $P(BSDXY,U,23)=""
	S $P(BSDXY,U,21)=""
	S BSDXNOD=^DPT(+BSDXPAT,0)
	S $P(BSDXY,"^",6)=$P(BSDXNOD,U) ;NAME
	S $P(BSDXY,"^",8)=$$GET1^DIQ(2,BSDXPAT,"PRIMARY LONG ID") ;PID
	S Y=$P(BSDXNOD,U,3) I Y]""  X ^DD("DD") S Y=$TR(Y,"@"," ")
	S $P(BSDXY,"^",7)=Y ;DOB
	S $P(BSDXY,"^",9)=""
	I $D(DUZ(2)) I DUZ(2)>0 S $P(BSDXY,"^",9)=$P($G(^AUPNPAT(BSDXPAT,41,DUZ(2),0)),U,2) ;HRN
	D MAIL
	D PHONE
	D NOK
	D DATAREV
	;/smh D MEDICARE
	D REGCMT
	S $P(BSDXY,"^",22)=$$GET1^DIQ(2,BSDXPAT,"EMAIL ADDRESS")
	S $P(BSDXY,"^",23)=$$GET1^DIQ(2,BSDXPAT,"PHONE NUMBER [CELLULAR]")
	S $P(BSDXY,"^",24)=$$GET1^DIQ(2,BSDXPAT,"COUNTRY:DESCRIPTION")
	S $P(BSDXY,"^",25)=$$GET1^DIQ(2,BSDXPAT,"SEX")
	N BSDXBEG,BSDXEND,BSDXLEN,BSDXI
	S BSDXLEN=$L(BSDXY)
	S BSDXBEG=0,BSDXI=2
	F  D  Q:BSDXEND=BSDXLEN
	. S BSDXEND=BSDXBEG+100
	. S:BSDXEND>BSDXLEN BSDXEND=BSDXLEN
	. S BSDXI=BSDXI+1
	. S ^BSDXTMP($J,BSDXI)=$E(BSDXY,BSDXBEG,BSDXEND)
	. S BSDXBEG=BSDXBEG+101
	S ^BSDXTMP($J,BSDXI+1)=$C(30)_$C(31)
	Q
	;
MAIL	N BSDXST
	Q:'$D(^DPT(+BSDXPAT,.11))
	S BSDXNOD=^DPT(+BSDXPAT,.11)
	Q:BSDXNOD=""
	S $P(BSDXY,"^",2)=$E($P(BSDXNOD,U),1,50) ;STREET
	S $P(BSDXY,"^",3)=$P(BSDXNOD,U,4) ;CITY
	S BSDXST=$P(BSDXNOD,U,5)
	I +BSDXST,$D(^DIC(5,+BSDXST,0)) S BSDXST=$P(^DIC(5,+BSDXST,0),U,2)
	S $P(BSDXY,"^",4)=BSDXST ;STATE
	S $P(BSDXY,"^",5)=$P(BSDXNOD,U,6) ;ZIP
	Q
	;
PHONE	;PHONE 10,11,12 HOME,OFC,MSG
	I $D(^DPT(+BSDXPAT,.13)) D
	. S BSDXNOD=^DPT(+BSDXPAT,.13)
	. S $P(BSDXY,U,10)=$P(BSDXNOD,U,1)
	. S $P(BSDXY,U,11)=$P(BSDXNOD,U,2)
	I $D(^DPT(+BSDXPAT,.121)) D
	. S BSDXNOD=^DPT(+BSDXPAT,.121)
	. S $P(BSDXY,U,12)=$P(BSDXNOD,U,10)
	Q
	;
NOK	;NOK
	;   13 NOK NAME^RELATIONSHIP^PHONE^STREET^CITY^STATE^ZIP
	N Y,BSDXST
	I $D(^DPT(+BSDXPAT,.21)) D
	. S BSDXNOD=^DPT(+BSDXPAT,.21)
	. S $P(BSDXY,U,13)=$P(BSDXNOD,U,1)
	. S $P(BSDXY,U,14)=$$VAL^XBDIQ1(9000001,BSDXPAT,2802)
	. S $P(BSDXY,U,15)=$P(BSDXNOD,U,9)
	. S $P(BSDXY,U,16)=$P(BSDXNOD,U,3)
	. S $P(BSDXY,U,17)=$P(BSDXNOD,U,6)
	. S BSDXST=$P(BSDXNOD,U,7)
	. I +BSDXST D
	. . I $D(^DIC(5,+BSDXST,0)) S BSDXST=$P(^DIC(5,+BSDXST,0),U,2),$P(BSDXY,U,18)=BSDXST
	. S $P(BSDXY,U,19)=$P(BSDXNOD,U,8)
	Q
	;
DATAREV	S $P(BSDXY,U,20)=$P($$VAL^XBDIQ1(9000001,BSDXPAT,16651),"@")
	Q
	;
REGCMT	N BSDXI,BSDXM,BSDXR
	S BSDXR=""
	D ENP^XBDIQ1(9000001,BSDXPAT,1301,"BSDXM(")
	S BSDXI=0 F  S BSDXI=$O(BSDXM(1301,BSDXI)) Q:'+BSDXI  D
	. S BSDXR=BSDXR_" "_BSDXM(1301,BSDXI)
	; S $P(BSDXY,U,23)=$TR($E(BSDXR,1,1024),U," ") ; MJL 1/17/2007 //smh
	S $P(BSDXY,U,21)=$TR($E(BSDXR,1,1024),U," ") ;
	Q
	;
GETMCAID(BSDXY,BSDXPAT)	; not in wv
	;Returns PATIENTIEN^ENTRY#^MEDICAID#^SUBENTRY#^ELIG.BEGIN^ELIG.END |
	;File is not dinum
	N C,N,ASDGX,BSDXM,BSDXBLD,BSDXCNT
	N BSDXIEN
	S BSDXBLD=""
	S BSDXIEN=0
	S BSDXCNT=1
	F  S BSDXIEN=$O(^AUPNMCD("B",BSDXPAT,BSDXIEN)) Q:'+BSDXIEN  D
	. S BSDXNUM=$$VAL^XBDIQ1(9000004,BSDXIEN,.03) ;MCAID#
	. D ENPM^XBDIQ1(9000004.11,BSDXIEN_",0",".01:.02","ASDGX(")
	. S C=1,N=0,BSDXM=""
	. F  S N=$O(ASDGX(N)) Q:'N  D
	. . S $P(BSDXY,"|",C)=BSDXPAT_U_BSDXIEN_U_BSDXNUM_U_N_U_ASDGX(N,.01)_U_ASDGX(N,.02)
	. . S C=C+1
	. . Q
	. Q
	Q
	;
MEDICARE	; not in WV
	S $P(BSDXY,U,21)=$$VAL^XBDIQ1(9000003,BSDXPAT,.03)
	S $P(BSDXY,U,22)=$$VAL^XBDIQ1(9000003,BSDXPAT,.04)
	Q
	;
GETMCARE(BSDXY,BSDXPAT)	     ;
	;Returns IEN^MEDICARE#^SUFFIX^SUBENTRY#^TYPE^ELIG.BEGIN^ELIG.END |
	;File is dinum
	;
	N ASDGX,C,N,BSDXNUM,BSDXSUF,BSDXBLD
	S BSDXNUM=$$VAL^XBDIQ1(9000003,BSDXPAT,.03)
	S BSDXSUF=$$VAL^XBDIQ1(9000003,BSDXPAT,.04)
	D ENPM^XBDIQ1(9000003.11,BSDXPAT_",0",".01:.03","ASDGX(")
	S C=1,N=0,BSDXBLD=""
	F  S N=$O(ASDGX(N)) Q:'N  D
	. S $P(BSDXY,"|",C)=BSDXPAT_U_BSDXNUM_U_BSDXSUF_U_N_U_ASDGX(N,.03)_U_ASDGX(N,.01)_U_ASDGX(N,.02)
	. S C=C+1
	. Q
	Q
	;
GETPVTIN(BSDXY,BSDXPAT)	;
	;Returns IEN^SUBENTRY^INSURER^POLICYNUMBER^ELIG.BEGIN^ELIG.END|...
	;File is dinum
	;
	N ASDGX,C,N
	D ENPM^XBDIQ1(9000006.11,BSDXPAT_",0",".01;.02;.06;.07","ASDGX(")
	S C=1,N=0
	F  S N=$O(ASDGX(N)) Q:'N  D
	. S $P(BSDXY,"|",C)=BSDXPAT_U_N_U_ASDGX(N,.01)_U_ASDGX(N,.02)_U_ASDGX(N,.06)_U_ASDGX(N,.07)
	. S C=C+1
	. Q
	Q
	;
DFN(FILE,BSDXPAT)	; -- returns ien for file
	I FILE'[9000004 Q BSDXPAT
	Q +$O(^AUPNMCD("B",BSDXPAT,0))
