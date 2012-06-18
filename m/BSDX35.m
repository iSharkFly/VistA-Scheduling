BSDX35	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 6/18/12 2:27pm
	;;1.7T1;BSDX;;Aug 31, 2011;Build 18
	; Licensed under LGPL  
	;
	;
	Q
	;
RSRCLTRD(BSDXY,BSDXLIST)	;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("RSRCLTR^BSDX35(.BSDXY,BSDXLIST)")
	Q
	;
RSRCLTR(BSDXY,BSDXLIST)	;EP
	;
	;Return recordset of RESOURCES and associated LETTERS
	;Used in generating rebook letters for a clinic
	;BSDXLIST is a |-delimited list of BSDX RESOURCE iens.  (The last |-piece is null, so discard it.)
	;Called by BSDX RESOURCE LETTERS
	;
	;
	S X="ERROR^BSDX35",@^%ZOSF("TRAP")
	S BSDXY="^BSDXTMP("_$J_")"
	N BSDXIEN,BSDX,BSDXLTR,BSDXNOS,BSDXCAN,BSDXIEN1
	S BSDXI=0
	S ^BSDXTMP($J,BSDXI)="I00010RESOURCEID^T00030RESOURCE_NAME^T00030LETTER_TEXT^T00030NO_SHOW_LETTER^T00030CLINIC_CANCELLATION_LETTER"_$C(30)
	;
	;
	;If BSDXLIST is a list of resource NAMES, look up each name and convert to IEN
	F BSDXJ=1:1:$L(BSDXLIST,"|")-1 S BSDX=$P(BSDXLIST,"|",BSDXJ) D  S $P(BSDXLIST,"|",BSDXJ)=BSDY
	. S BSDY=""
	. I BSDX]"",$D(^BSDXRES(BSDX,0)) S BSDY=BSDX Q
	. I BSDX]"",$D(^BSDXRES("B",BSDX)) S BSDY=$O(^BSDXRES("B",BSDX,0)) Q
	. Q
	;
	;Get letter text from wp fields
	S BSDXIEN=0
	F BSDX=1:1:$L(BSDXLIST,"|")-1 S BSDXIEN=$P(BSDXLIST,"|",BSDX) D
	. Q:'$D(^BSDXRES(BSDXIEN))
	. S BSDXNAM=$P(^BSDXRES(BSDXIEN,0),U)
	. S BSDXLTR=""
	. I $D(^BSDXRES(BSDXIEN,1)) D
	. . S BSDXIEN1=0 F  S BSDXIEN1=$O(^BSDXRES(BSDXIEN,1,BSDXIEN1)) Q:'+BSDXIEN1  D
	. . . S BSDXLTR=BSDXLTR_$G(^BSDXRES(BSDXIEN,1,BSDXIEN1,0))
	. . . S BSDXLTR=BSDXLTR_$C(13)_$C(10)
	. S BSDXNOS=""
	. I $D(^BSDXRES(BSDXIEN,12)) D
	. . S BSDXIEN1=0 F  S BSDXIEN1=$O(^BSDXRES(BSDXIEN,12,BSDXIEN1)) Q:'+BSDXIEN1  D
	. . . S BSDXNOS=BSDXNOS_$G(^BSDXRES(BSDXIEN,12,BSDXIEN1,0))
	. . . S BSDXNOS=BSDXNOS_$C(13)_$C(10)
	. S BSDXCAN=""
	. I $D(^BSDXRES(BSDXIEN,13)) D
	. . S BSDXIEN1=0 F  S BSDXIEN1=$O(^BSDXRES(BSDXIEN,13,BSDXIEN1)) Q:'+BSDXIEN1  D
	. . . S BSDXCAN=BSDXCAN_$G(^BSDXRES(BSDXIEN,13,BSDXIEN1,0))
	. . . S BSDXCAN=BSDXCAN_$C(13)_$C(10)
	. S BSDXI=BSDXI+1
	. S ^BSDXTMP($J,BSDXI)=BSDXIEN_U_BSDXNAM_U_BSDXLTR_U_BSDXNOS_U_BSDXCAN_$C(30)
	;
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
ERROR	;
	D ERR("RPMS Error")
	Q
	;
ERR(ERRNO)	;Error processing
	S:'$D(BSDXI) BSDXI=999
	I +ERRNO S BSDXERR=ERRNO+134234112 ;vbObjectError
	E  S BSDXERR=ERRNO
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="^^^^"_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
UTCR(RESNAM) ; $$ - Create Unit Test Clinic and Resource Pair ; Private
 ; Input: Resource Name By Value
 ; Output: -1^Error or HLIEN^RESIEN for Success (file 44 IEN^file 9002018.1 IEN)
 ; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
 N HLIEN S HLIEN=$$UTCR44(RESNAM)
 I +HLIEN=-1 QUIT HLIEN
 ;
 N RESIEN S RESIEN=$$UTCRRES(RESNAM,HLIEN)
 I +RESIEN=-1 QUIT RESIEN
 E  QUIT HLIEN_U_RESIEN
 ;
UTCR44(HLNAME) ; $$ - Create Unit Test Clinic in File 44; Private ; TESTING ONLY CODE
 ; Output: -1^Error or IEN for Success
 ; Input: Hosp Location Name by Value
 ; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
 ;
 I $D(^SC("B",HLNAME)) Q $O(^(HLNAME,""))
 ;
 N SAM
 S SAM(44,"?+1,",.01)=HLNAME            ; Name
 S SAM(44,"?+1,",2)="C"                 ; Type = Clinic
 S SAM(44,"?+1,",2.1)=1                 ; Type Extension (not used)
 S SAM(44,"?+1,",3.5)=$O(^DG(40.8,0))   ; Division (not yet used)
 S SAM(44,"?+1,",8)=295                 ; Stop Code Number (not used)
 S SAM(44,"?+1,",9)="M"          ; Service (not used)
 S SAM(44,"?+1,",1912)=15               ; Length of Appt (not used)
 S SAM(44,"?+1,",1917)=4                ; Display increments per hour (not used)
 S SAM(44,"?+1,",1918)=8                ; Overbooks/day max (not used)
 S SAM(44,"?+1,",2000.5)=0              ; Require Action Profiles: Yes (not used)
 S SAM(44,"?+1,",2001)=999              ; Allowable consecutive no-shows (not used)
 S SAM(44,"?+1,",2002)=999              ; Max # days for Future Booking (not used)
 S SAM(44,"?+1,",2005)=365              ; Max # days for Auto Rebook (not used)
 S SAM(44,"?+1,",2502)="N"             ; Non-Count Clinic (not used)
 S SAM(44,"?+1,",2504)="Y"            ; Clinic meets at this Facility? (not used)
 S SAM(44,"?+1,",2507)=9              ; Appointment Type (not used)
 ;
 N BSDXERR,BSDXIEN
 D UPDATE^DIE("",$NA(SAM),$NA(BSDXIEN),$NA(BSDXERR))
 Q $S($D(BSDXERR):-1_U_BSDXERR("DIERR",1,"TEXT",1),1:BSDXIEN(1))
 ;
UTCRRES(NAME,HLIEN) ; $$ - Create Unit Test Resource in 9002018.1 (BSDX RESOURCE); Private
 ; Input: Hospital Location IEN
 ; Output: -1^Error or IEN for Success
 ; DO NOT USE IN A PRODUCTION ENVIRONTMENT. INTENDED FOR TESTING ONLY
 I $D(^BSDXRES("B",NAME)) Q $O(^(NAME,""))
 D RSRC^BSDX16(.RES,"|"_NAME_"||"_HLIEN)
 N RTN S RTN=@$Q(^BSDXTMP($J,0)) ; return array next value
 Q $S(RTN=0:-1_U_RTN,1:+RTN) ; 0 means an error has occurred; 1 means IEN returned
 ;
TIMES() ; $$ - Create a next available appointment time^ending time; Private
 N NOW S NOW=$$NOW^XLFDT() ; Now time
 N LAST S LAST=$O(^BSDXAPPT("B"," "),-1) ; highest time in file
 N TIME2USE S TIME2USE=$S(NOW>LAST:NOW,1:LAST) ; Which time to use?
 S TIME2USE=$E(TIME2USE,1,12) ; Strip away seconds
 N APPTIME S APPTIME=$$FMADD^XLFDT(TIME2USE,0,0,15,0) ; Add 15 min
 N ENDTIME S ENDTIME=$$FMADD^XLFDT(APPTIME,0,0,15,0) ; Add 15 more min
 Q APPTIME_U_ENDTIME ; quit with apptime^endtime
