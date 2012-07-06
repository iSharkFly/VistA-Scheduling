BSDX28	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 7/6/12 10:57am
	;;1.7T1;BSDX;;Jul 06, 2012;Build 18
	; Licensed under LGPL
	; Change Log:
	; HMW 3050721 Added test for inactivated record
	; V1.3 WV/SMH 3100714 
	; - add PID search
	; - return PID instead of SSN (change header and logic)
	; - Change Error trap to new style.
	;
PTLOOKRS(BSDXY,BSDXP,BSDXC)	 ;EP Patient Lookup
	;
	;Find up to BSDXC patients matching BSDXP*
	;Supports DOB Lookup, Primary Long ID lookup
	;
	N $ET S $ET="G ERROR^BSDX28"
	   ; rm ctrl chars
	S BSDXP=$TR(BSDXP,$C(13),"")
	S BSDXP=$TR(BSDXP,$C(10),"")
	S BSDXP=$TR(BSDXP,$C(9),"")
	   ; num of pts to find
	S:BSDXC="" BSDXC=10
	N BSDXHRN,BSDXZ,BSDXDLIM,BSDXRET,BSDXDPT,BSDXRET,BSDXIEN,BSDXFILE
	N BSDXIENS,BSDXFIELDS,BSDXFLAGS,BSDXVALUE,BSDXNUMBER,BSDXINDEXES,BSDXSCREEN
	N BSDXTARG,BSDXMSG,BSDXRSLT
	S BSDXDLIM="^"
	S BSDXRET="T00030NAME^T00030HRN^T00030PID^D00030DOB^T00030IEN"_$C(30)
	I '+$G(DUZ) S BSDXY=BSDXRET_$C(31) Q
	I '$D(DUZ(2)) S BSDXY=BSDXRET_$C(31) Q
DFN	;If DFN is passed as `nnnn, just return that patient
	I $E(BSDXP)="`" DO  SET BSDXY=BSDXRET_$C(31) QUIT
	. N BSDXIEN S BSDXIEN=$E(BSDXP,2,99)
	. I BSDXIEN'=+BSDXIEN QUIT  ; BSDXIEN must be numeric
	. N NAME S NAME=$P(^DPT(BSDXIEN,0),U)
	. N HRN S HRN=$P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,2)
	. N PID S PID=$P(^DPT(BSDXIEN,.36),U,3)
	. N DOB S DOB=$$FMTE^XLFDT($P(^DPT(BSDXIEN,0),U,3))
	. S BSDXRET=BSDXRET_NAME_U_HRN_U_PID_U_DOB_U_BSDXIEN_$C(30)
PID	;PID Lookup
	; If this ID exists, go get it. If "UJOPID" index doesn't exist,
	; won't work anyways.
	I $D(^DPT("UJOPID",BSDXP)) DO  SET BSDXY=BSDXRET_$C(31) QUIT
	. S BSDXIEN=$O(^DPT("UJOPID",BSDXP,""))
	. Q:'$D(^DPT(BSDXIEN,0))
	. S BSDXDPT=$G(^DPT(BSDXIEN,0))
	. S BSDXZ=$P(BSDXDPT,U) ;NAME
	. S BSDXHRN=$P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,2) ;CHART
	. I BSDXHRN="" Q  ;NO CHART AT THIS DUZ2
	. ; Inactivated Chart get an *
	. I $P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,3) S BSDXHRN=BSDXHRN_"(*)" Q
	. S $P(BSDXZ,BSDXDLIM,2)=BSDXHRN
	. S $P(BSDXZ,BSDXDLIM,3)=$P(^DPT(BSDXIEN,.36),U,3) ;PID
	. S Y=$P(BSDXDPT,U,3) X ^DD("DD")
	. S $P(BSDXZ,BSDXDLIM,4)=Y ;DOB
	. S $P(BSDXZ,BSDXDLIM,5)=BSDXIEN
	. S BSDXRET=BSDXRET_BSDXZ_$C(30)
	;
DOB	;DOB Lookup
	I +DUZ(2),((BSDXP?1.2N1"/"1.2N1"/"1.4N)!(BSDXP?1.2N1" "1.2N1" "1.4N)!(BSDXP?1.2N1"-"1.2N1"-"1.4N)) D  S BSDXY=BSDXRET_$C(31) Q
	. S X=BSDXP S %DT="P" D ^%DT S BSDXP=Y Q:'+Y
	. Q:'$D(^DPT("ADOB",BSDXP))
	. S BSDXIEN=0 F  S BSDXIEN=$O(^DPT("ADOB",BSDXP,BSDXIEN)) Q:'+BSDXIEN  D
	. . Q:'$D(^DPT(BSDXIEN,0))
	. . S BSDXDPT=$G(^DPT(BSDXIEN,0))
	. . S BSDXZ=$P(BSDXDPT,U) ;NAME
	. . S BSDXHRN=$P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,2) ;CHART
	. . I BSDXHRN="" Q  ;NO CHART AT THIS DUZ2
	. . I $P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,3) S BSDXHRN=BSDXHRN_"(*)" Q  ;HMW 20050721 Record Inactivated
	. . S $P(BSDXZ,BSDXDLIM,2)=BSDXHRN
	   . . S $P(BSDXZ,BSDXDLIM,3)=$P(^DPT(BSDXIEN,.36),U,3) ;PID
	. . S Y=$P(BSDXDPT,U,3) X ^DD("DD")
	. . S $P(BSDXZ,BSDXDLIM,4)=Y ;DOB
	. . S $P(BSDXZ,BSDXDLIM,5)=BSDXIEN
	. . S BSDXRET=BSDXRET_BSDXZ_$C(30)
	. . Q
	. Q
	;
CHART ;Chart# Lookup
	I +DUZ(2),BSDXP]"",$D(^AUPNPAT("D",BSDXP)) D  S BSDXY=BSDXRET_$C(31) Q
	. S BSDXIEN=0 F  S BSDXIEN=$O(^AUPNPAT("D",BSDXP,BSDXIEN)) Q:'+BSDXIEN  I $D(^AUPNPAT("D",BSDXP,BSDXIEN,DUZ(2))) D  Q
	. . Q:'$D(^DPT(BSDXIEN,0))
	. . S BSDXDPT=$G(^DPT(BSDXIEN,0))
	. . S BSDXZ=$P(BSDXDPT,U) ;NAME
	. . S BSDXHRN=BSDXP ;CHART
	. . I $D(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),$P(^(0),U,3) S BSDXHRN=BSDXHRN_"(*)" Q  ;HMW 20050721 Record Inactivated
	. . S $P(BSDXZ,BSDXDLIM,2)=BSDXHRN
	   . . S $P(BSDXZ,BSDXDLIM,3)=$P(^DPT(BSDXIEN,.36),U,3) ;PID
	. . S Y=$P(BSDXDPT,U,3) X ^DD("DD")
	. . S $P(BSDXZ,BSDXDLIM,4)=Y ;DOB
	. . S $P(BSDXZ,BSDXDLIM,5)=BSDXIEN
	. . S BSDXRET=BSDXRET_BSDXZ_$C(30)
	. . Q
	. Q
	   ;
SSN	;SSN Lookup
	I (BSDXP?9N)!(BSDXP?3N1"-"2N1"-"4N),$D(^DPT("SSN",BSDXP)) D  S BSDXY=BSDXRET_$C(31) Q
	. S BSDXIEN=0 F  S BSDXIEN=$O(^DPT("SSN",BSDXP,BSDXIEN)) Q:'+BSDXIEN  D  Q
	. . Q:'$D(^DPT(BSDXIEN,0))
	. . S BSDXDPT=$G(^DPT(BSDXIEN,0))
	. . S BSDXZ=$P(BSDXDPT,U) ;NAME
	. . S BSDXHRN=$P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,2) ;CHART
	. . I BSDXHRN="" Q  ;NO CHART AT THIS DUZ2
	. . I $P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,3) S BSDXHRN=BSDXHRN_"(*)" Q  ;HMW 20050721 Record Inactivated
	. . S $P(BSDXZ,BSDXDLIM,2)=BSDXHRN
	   . . S $P(BSDXZ,BSDXDLIM,3)=$P(^DPT(BSDXIEN,.36),U,3) ;PID
	. . S Y=$P(BSDXDPT,U,3) X ^DD("DD")
	. . S $P(BSDXZ,BSDXDLIM,4)=Y ;DOB
	. . S $P(BSDXZ,BSDXDLIM,5)=BSDXIEN
	. . S BSDXRET=BSDXRET_BSDXZ_$C(30)
	. . Q
	. Q
	;
	S BSDXFILE=9000001
	S BSDXIENS=""
	S BSDXFIELDS=".01"
	S BSDXFLAGS="M"
	S BSDXVALUE=BSDXP
	S BSDXNUMBER=BSDXC
	S BSDXINDEXES=""
	S BSDXSCREEN=$S(+DUZ(2):"I $D(^AUPNPAT(Y,41,DUZ(2),0))",1:"")
	S BSDXIDEN=""
	S BSDXTARG="BSDXRSLT"
	S BSDXMSG=""
	D FIND^DIC(BSDXFILE,BSDXIENS,BSDXFIELDS,BSDXFLAGS,BSDXVALUE,BSDXNUMBER,BSDXINDEXES,BSDXSCREEN,BSDXIDEN,BSDXTARG,BSDXMSG)
	I '+$G(BSDXRSLT("DILIST",0)) S BSDXY=BSDXRET_$C(31) Q
	N BSDXCNT S BSDXCNT=2
	F BSDXX=1:1:$P(BSDXRSLT("DILIST",0),U) D
	. S BSDXIEN=BSDXRSLT("DILIST",2,BSDXX)
	. S BSDXZ=BSDXRSLT("DILIST","ID",BSDXX,.01) ;NAME
	. S BSDXHRN=$P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,2) ;CHART
	. I BSDXHRN="" Q  ;NO CHART AT THIS DUZ2
	. I $P($G(^AUPNPAT(BSDXIEN,41,DUZ(2),0)),U,3) S BSDXHRN=BSDXHRN_"(*)" Q  ;HMW 20050721 Record Inactivated
	. S $P(BSDXZ,BSDXDLIM,2)=BSDXHRN
	. S BSDXDPT=$G(^DPT(BSDXIEN,0))
	   . S $P(BSDXZ,BSDXDLIM,3)=$P(^DPT(BSDXIEN,.36),U,3) ;PID
	. S Y=$P(BSDXDPT,U,3) X ^DD("DD")
	. S $P(BSDXZ,BSDXDLIM,4)=Y ;DOB
	. S $P(BSDXZ,BSDXDLIM,5)=BSDXIEN
	. S $P(BSDXRET,$C(30),BSDXCNT)=BSDXZ
	. S BSDXCNT=BSDXCNT+1
	. Q
	S BSDXY=BSDXRET_$C(30)_$C(31)
	Q
	;
ERROR	;
	D ERR("RPMS Error")
	Q
	;
ERR(ERRNO)	;Error processing
	S BSDXRET="T00030NAME^T00030HRN^T00030SSN^D00030DOB^T00030IEN"_$C(30)_"^^^^"_$C(30)_$C(31)
	Q
