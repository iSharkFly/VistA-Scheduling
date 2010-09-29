BSDX33	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 7/15/10 12:33pm
	;;1.41;BSDX;;Sep 29, 2010
	   ; Mods by WV/STAR
	   ;
	   ; Change Log:
	   ; July 13, 2010
	   ; v 1.3 adds fixes Rebooking behavior in application (see RBNEXT)
	   ; also adds i18 support - Dates passed in FM format from application
	   ; in tag SETRBK and RBNEXT
	;
	;
	Q
RBNEXTD(BSDXY,BSDXDATE,BSDXRES,BSDXTPID)	;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("RBNEXT^BSDX33(.BSDXY,BSDXDATE,BSDXRES,BSDXTPID)")
	Q
	;
RBNEXT(BSDXY,BSDXDATE,BSDXRES,BSDXTPID)	;EP
	;Called by BSDX REBOOK NEXT BLOCK to find
	;the next ACCESS BLOCK in resource BSDXRES after BSDXDATE
	;Returns 1 in ERRORID and date in NEXTBLOCK if a block was found or NULL in NEXTBLOCK of no date found
	;Otherwise, returns 0 and error message in ERRORTEXT
	;If BSDXTPID = 0 then any access type match
	;
	S X="ERROR2^BSDX33",@^%ZOSF("TRAP")
	N BSDXI,BSDXIENS,%DT,BSDXMSG,Y,BSDXRESD,BSDXFND,BSDXIEN,BSDXNOD,BSDXATID
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S ^BSDXTMP($J,BSDXI)="I00020ERRORID^D00010NEXTBLOCK^T00030ERRORTEXT"_$C(30)
	;
	I BSDXRES="" D ERR2("BSDX REBOOK NEXT BLOCK: Invalid resource name") Q
	I '$D(^BSDXRES("B",BSDXRES)) D ERR2("BSDX REBOOK NEXT BLOCK: Invalid resource name") Q
	S BSDXRESD=$O(^BSDXRES("B",BSDXRES,0))
	I '+BSDXRESD D ERR2("BSDX REBOOK NEXT BLOCK: Invalid resource name") Q
	;
	   ; i18n fix
	   ; S X=BSDXDATE,%DT="XT" D ^%DT
	; I Y=-1 D ERR2(1,"BSDX REBOOK NEXT BLOCK: Invalid datetime") Q
	;
	   ; S BSDXDATE=$P(Y,".")
	;
	S BSDXFND=0
	F  S BSDXDATE=$O(^BSDXAB("ARSCT",BSDXRESD,BSDXDATE)) Q:'+BSDXDATE  D  Q:BSDXFND
	. S BSDXIEN=0 F  S BSDXIEN=$O(^BSDXAB("ARSCT",BSDXRESD,BSDXDATE,BSDXIEN)) Q:'+BSDXIEN  D  Q:BSDXFND
	. . Q:'$D(^BSDXAB(BSDXIEN,0))
	. . S BSDXNOD=^BSDXAB(BSDXIEN,0)
	. . Q:+$P(BSDXNOD,U,4)=0  ;Slots
	. . S BSDXATID=$P(BSDXNOD,U,5)
	. . I BSDXTPID=0!(BSDXATID=BSDXTPID) S BSDXFND=$P(BSDXNOD,U,2) Q
	;
	I BSDXFND=0 S BSDXFND=""
	E  S Y=BSDXFND X ^DD("DD") S BSDXFND=Y
	S BSDXI=BSDXI+1
	   ;//smh - bug (V 1.3): Need to replace @ in FM date for C# to recognize it
	   S BSDXFND=$TR(BSDXFND,"@"," ")
	   ;//smh end fix
	S ^BSDXTMP($J,BSDXI)="1^"_BSDXFND_"^"_$C(30)_$C(31)
	Q
SETRBKD(BSDXY,BSDXAPPT,BSDXDATE)	;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("SETRBK^BSDX33(.BSDXY,BSDXAPPT,BSDXDATE)")
	Q
	;
SETRBK(BSDXY,BSDXAPPT,BSDXDATE)	;EP
	;
	;Sets rebook date into appointment
	;BSDXAPPT - Appointment ID
	;BSDXDATE - Rebook Datetime in internal format
	;Called by BSDX REBOOK SET
	;
	;ErrorID:
	; 0 if a problem.  Message in ERRORTEXT
	; 1 if OK
	;
	S X="ERROR^BSDX33",@^%ZOSF("TRAP")
	N BSDXI,BSDXIENS,%DT,BSDXMSG,Y
	S BSDXY="^BSDXTMP("_$J_")"
	S BSDXI=0
	S ^BSDXTMP($J,BSDXI)="I00020ERRORID^T00030ERRORTEXT"_$C(30)
	;
	I '+BSDXAPPT
	I '$D(^BSDXAPPT(BSDXAPPT,0)) D ERR(1,"BSDX REBOOK SET: Invalid appointment ID") Q
	; i18n (v 1.3)
	   ;S X=BSDXDATE,%DT="XT" D ^%DT
	;I Y=-1 D ERR(1,"BSDX REBOOK SET: Invalid rebook datetime") Q
	;S BSDXDATE=Y
	S BSDXIENS=BSDXAPPT_","
	S BSDXFDA(9002018.4,BSDXIENS,.11)=+BSDXDATE
	;
	K BSDXMSG
	D FILE^DIE("","BSDXFDA","BSDXMSG")
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)="1^"_$C(31)
	;
	Q
	;
ERR(BSDXERID,ERRTXT)	;Error processing
	S:'+$G(BSDXI) BSDXI=999999
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXERID_"^"_ERRTXT_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
ERROR	;
	D ^%ZTER
	I '+$G(BSDXI) N BSDXI S BSDXI=999999
	S BSDXI=BSDXI+1
	D ERR(0,"BSDX33 M Error: <"_$G(%ZTERROR)_">")
	Q
	;
ERR2(BSDXERID,ERRTXT)	;Error processing
	S:'+$G(BSDXI) BSDXI=999999
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXERID_"^^"_ERRTXT_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
ERROR2	;
	D ^%ZTER
	I '+$G(BSDXI) N BSDXI S BSDXI=999999
	S BSDXI=BSDXI+1
	D ERR2(0,"BSDX33 M Error: <"_$G(%ZTERROR)_">")
	Q
