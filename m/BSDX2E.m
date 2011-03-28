BSDX2E	;IHS/OIT/MJL - ENVIRONMENT CHECK FOR WINDOWS SCHEDULING [3/16/11 9:54am]
	;;1.5V3;BSDX;;Mar 16, 2011
	;
	S LINE="",$P(LINE,"*",81)=""
	S XPDNOQUE="NO QUE"  ;NO QUEUING ALLOWED
	S XPDABORT=0
	I '$G(DUZ) D SORRY("DUZ UNDEFINED OR 0") Q
	;
	I '$L($G(DUZ(0))) D SORRY("DUZ(0) UNDEFINED OR NULL") Q
	;
	D HOME^%ZIS,DT^DICRW
	S X=$P($G(^VA(200,DUZ,0)),U)
	I $G(X)="" W !,$$C^XBFUNC("Who are you????") D SORRY("Unknown User") Q
	;
VERSION	;
	W !,$$C^XBFUNC("Hello, "_$P(X,",",2)_" "_$P(X,","))
	W !!,$$C^XBFUNC("Checking Environment for Install of Version "_$P($T(+2),";",3)_" of "_$P($T(+2),";",4)_".")
	;
	Q:'$$VERCHK("VA FILEMAN",22)
	Q:'$$VERCHK("KERNEL",8)
	Q:'$$VERCHK("XB",3)
	;Is the PIMS requirement present?
	Q:'$$VERCHK("SD",5.3)
	; Q:'$$PATCHCK("PIMS*5.3*1003") D
	Q:'$$VERCHK("BMX",2)
	;
OTHER	;
	;Other checks
	;
ENVOK	; If this is just an environ check, end here.
	W !!,$$C^XBFUNC("ENVIRONMENT OK.")
	;
	; The following line prevents the "Disable Options..." and "Move
	; Routines..." questions from being asked during the install.
	I $G(XPDENV)=1 S (XPDDIQ("XPZ1"),XPDDIQ("XPZ2"))=0
	;
	;
	;VERIFY BACKUPS HAVE BEEN DONE
	;W !!
	;S DIR(0)="Y"
	;S DIR("B")="NO"
	;S DIR("A")="Has a SUCCESSFUL system backup been performed??"
	;D ^DIR
	;I $D(DIRUT)!($G(Y)=0) S XPDABORT=1 S XPX="BACKUP" D SORRY Q
	;S ^TMP("BPCPRE",$J,"BACKUPS CONFIRMED BY "_DUZ)=$H
	;
	Q
	;
VERCHK(XPXPKG,XVRMIN)	;
	S X=$$VERSION^XPDUTL(XPXPKG)
	W !!,$$C^XBFUNC("Need at least "_XPXPKG_" "_XVRMIN_"....."_XPXPKG_" "_$S(X'="":X,1:"Is Not")_" Present")
	I X<XVRMIN  D SORRY(XPXPKG_" "_XVRMIN_" Is Not Installed") Q 0
	Q 1
	;
PATCHCK(XPXPCH)	;
	S X=$$PATCH^XPDUTL(XPXPCH)
	W !!,$$C^XBFUNC("Need "_XPXPCH_"....."_XPXPCH_" "_$S(X:"Is",1:"Is Not")_" Present")
	Q X
	;
V0200	;EP Version 1.5 PostInit
	;Add Protocol items to SDAM APPOINTMENT EVENTS protocol
	;Remove protocols known to cause problems from SDAM APPOINTMENT EVENTS
	;Set Default Values for Parameters
	N BSDXDA,BSDXFDA,BSDXDA1,BSDXSEQ,BSDXDAT,BSDXNOD,BSDXIEN,BSDXMSG
	;
	; 1st, add the BSDX event protocols
	; Get SDAM APPOINTMENT EVENTS IEN in 101
	S BSDXDA=$O(^ORD(101,"B","SDAM APPOINTMENT EVENTS",0))
	Q:'+BSDXDA
	; Add each of those protocols unless they already exist.
	S BSDXDAT="BSDX ADD APPOINTMENT;10.2^BSDX CANCEL APPOINTMENT;10.4^BSDX CHECKIN APPOINTMENT;10.6^BSDX NOSHOW APPOINTMENT;10.8"
	; For each
	F J=1:1:$L(BSDXDAT,U) D
	. K BSDXIEN,BSDXMSG,BSDXFDA
	. ; Get Item
	. S BSDXNOD=$P(BSDXDAT,U,J)
	. ; Get Item Name (BSDX ADD APPOINTMENT)
	. S BSDXDA1=$P(BSDXNOD,";")
	. ; Get Item Sequence (10.2)
	. S BSDXSEQ=$P(BSDXNOD,";",2)
	. ; Get Item Reference (Item is already in the protocol file)
	. S BSDXDA1=$O(^ORD(101,"B",BSDXDA1,0))
	. ; Quit if not found
	. Q:'+BSDXDA1
	. ; Quit if already exists in the SDAM protocol
	. Q:$D(^ORD(101,BSDXDA,10,"B",BSDXDA1))
	. ; Go ahead and save it.
	. S BSDXFDA(101.01,"+1,"_BSDXDA_",",".01")=BSDXDA1
	. S BSDXFDA(101.01,"+1,"_BSDXDA_",","3")=BSDXSEQ
	. D UPDATE^DIE("","BSDXFDA","BSDXIEN","BSDXMSG")
	. ; Error message
	. I $D(BSDXMSG) W $C(7),"Error: ",BSDXMSG("DIERR",1,"TEXT",1)
	;
	; Remove nassssty protocols ORU PATIENT MOVMT and DVBA C&P SCHD EVENT
	; SDAM APPOINTMENT EVENTS IENS for use in FIND1^DIC
	N SDEVTIENS S SDEVTIENS=","_BSDXDA_","
	; Subfile entry for ORU...
	N ORUIEN S ORUIEN=$$FIND1^DIC(101.01,SDEVTIENS,"","ORU PATIENT MOVMT")
	; Subfile entry for DVBA...
	N DVBAIEN S DVBAIEN=$$FIND1^DIC(101.01,SDEVTIENS,"","DVBA C&P SCHD EVENT")
	; Deletion code
	N BSDXFDA,BSDXMSG
	S:ORUIEN>0 BSDXFDA(101.01,ORUIEN_SDEVTIENS,.01)="@"
	S:DVBAIEN>0 BSDXFDA(101.01,DVBAIEN_SDEVTIENS,.01)="@"
	D:$D(BSDXFDA) FILE^DIE("","BSDXFDA","BSDXMSG")
	; If error
	I $D(BSDXMSG) W $C(7),"Error: ",BSDXMSG("DIERR",1,"TEXT",1)
	;
	;
	; Now put in the default values for parameters
	; BSDX AUTO PRINT RS as false
	; BSDX AUTO PRINT AS as false
	;
	N BSDXERR
	D PUT^XPAR("PKG","BSDX AUTO PRINT RS",1,0,.BSDXERR)
	I $G(BSDXERR) W $C(7),"Error: ",BSDXERR
	D PUT^XPAR("PKG","BSDX AUTO PRINT AS",1,0,.BSDXERR)
	I $G(BSDXERR) W $C(7),"Error: ",BSDXERR
	QUIT
	;
SORRY(XPX)	;
	K DIFQ
	S XPDABORT=1
	W !,$$C^XBFUNC($P($T(+2),";",3)_" of "_$P($T(+2),";",4)_" Cannot Be Installed!")
	W !,$$C^XBFUNC("Reason: "_XPX_".")
	W *7,!!!,$$C^XBFUNC("Sorry....something is wrong with your environment")
	W !,$$C^XBFUNC("Aborting "_XPDNM_" install!")
	W !,$$C^XBFUNC("Correct error and reinstall otherwise")
	W !,$$C^XBFUNC("please print/capture this screen and notify")
	W !,$$C^XBFUNC("technical support")
	W !!,LINE
	D BMES^XPDUTL("Sorry....something is wrong with your environment")
	D BMES^XPDUTL("Enviroment ERROR "_$G(XPX))
	D BMES^XPDUTL("Aborting "_XPDNM_" install!")
	D BMES^XPDUTL("Correct error and reinstall otherwise")
	D BMES^XPDUTL("please print/capture this screen and notify")
	D BMES^XPDUTL("technical support")
	Q
	;
