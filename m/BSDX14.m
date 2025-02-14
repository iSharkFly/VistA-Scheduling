BSDX14	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 4/28/11 10:19am
	;;1.7;BSDX;;Jun 01, 2013;Build 24
	; Licensed under LGPL
	;
	;
ACCTYPD(BSDXY,BSDXVAL)	;EP
	;Entry point for debugging
	;
	;D DEBUG^%Serenji("ACCTYP^BSDX14(.BSDXY,BSDXVAL)")
	Q
	;
ACCTYP(BSDXY,BSDXVAL)	;EP
	;Called by BSDX ADD/EDIT ACCESS TYPE
	;Add/Edit ACCESS TYPE entry
	;BSDXVAL is IEN|NAME|INACTIVE|COLOR|RED|GREEN|BLUE
	;If IEN=0 Then this is a new ACCTYPE
	;Test Line:
	;D ACCTYP^BSDX14(.RES,"0|ORAL HYGIENE|false|Red")
	;
	S X="ERROR^BSDX14",@^%ZOSF("TRAP")
	N BSDXIENS,BSDXFDA,BSDXIEN,BSDXMSG,BSDX,BSDXNAM
	S BSDXY="^BSDXTMP("_$J_")"
	S ^BSDXTMP($J,0)="I00020ACCESSTYPEID^T00030ERRORTEXT"_$C(30)
	I BSDXVAL="" D ERR(0,"BSDX14: Invalid null input Parameter") Q
	S BSDXIEN=$P(BSDXVAL,"|")
	I +BSDXIEN D
	. S BSDX="EDIT"
	. S BSDXIENS=BSDXIEN_","
	E  D
	. S BSDX="ADD"
	. S BSDXIENS="+1,"
	;
	S BSDXNAM=$P(BSDXVAL,"|",2)
	I BSDXNAM="" D ERR(0,"BSDX14: Invalid null Access Type name.") Q
	;
	;Prevent adding entry with duplicate name
	I $D(^BSDXTYPE("B",BSDXNAM)),$O(^BSDXTYPE("B",BSDXNAM,0))'=BSDXIEN D  Q
	. D ERR(0,"BSDX14: Cannot have two Access Types with the same name.")
	. Q
	;
	S BSDXINA=$P(BSDXVAL,"|",3)
	S BSDXINA=$S(BSDXINA="YES":1,1:0)
	;
	S BSDXFDA(9002018.35,BSDXIENS,.01)=$P(BSDXVAL,"|",2) ;NAME
	S BSDXFDA(9002018.35,BSDXIENS,.02)=BSDXINA ;INACTIVE
	S BSDXFDA(9002018.35,BSDXIENS,.04)=$P(BSDXVAL,"|",4) ;COLOR
	S BSDXFDA(9002018.35,BSDXIENS,.05)=$P(BSDXVAL,"|",5) ;RED
	S BSDXFDA(9002018.35,BSDXIENS,.06)=$P(BSDXVAL,"|",6) ;GREEN
	S BSDXFDA(9002018.35,BSDXIENS,.07)=$P(BSDXVAL,"|",7) ;BLUE
	K BSDXMSG
	I BSDX="ADD" D
	. K BSDXIEN
	. D UPDATE^DIE("","BSDXFDA","BSDXIEN","BSDXMSG")
	. S BSDXIEN=+$G(BSDXIEN(1))
	E  D
	. D FILE^DIE("","BSDXFDA","BSDXMSG")
	S ^BSDXTMP($J,1)=$G(BSDXIEN)_"^-1"_$C(30)_$C(31)
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
	D ERR(0,"BSDX14 M Error: <"_$G(%ZTERROR)_">")
	Q
