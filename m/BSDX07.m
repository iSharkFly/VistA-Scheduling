BSDX07	; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS  ; 3/15/11 11:50am
	   ;;1.5V3;BSDX;;Mar 16, 2011
	   ;
	   ; Change Log:
	   ; UJO/SMH
	   ; v1.3 July 13 2010 - Add support i18n - Dates input as FM dates, not US.
	   ; v1.42 Oct 22 2010 - Transaction now restartable by providing arguments
	   ;   thanks to Rick Marshall and Zach Gonzalez at Oroville.
	   ; v1.42 Oct 30 2010 - Extensive refactoring.
	   ; v1.5  Mar 15 2011 - End time does not have to have time anymore.
	   ;      It could be midnight of the next day
	   ;
	   ; Error Reference:
	   ; -1: Patient Record is locked. This means something is wrong!!!!
	   ; -2: Start Time is not a valid Fileman date
	   ; -3: End Time is not a valid Fileman date
	   ; v1.5:obsolete::-4: End Time does not have time inside of it.
	   ; -5: BSDXPATID is not numeric
	   ; -6: Patient Does not exist in ^DPT
	   ; -7: Resource Name does not exist in B index of BSDX RESOURCE
	   ; -8: Resouce doesn't exist in ^BSDXRES
	   ; -9: Couldn't add appointment to BSDX APPOINTMENT
	   ; -10: Couldn't add appointment to files 2 and/or 44
	   ; -100: Mumps Error
	
APPADDD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)	   ;EP
	   ;Entry point for debugging
	   D DEBUG^%Serenji("APPADD^BSDX07(.BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)")
	   Q
	   ;
UT	; Unit Tests
	   N ZZZ
	   ; Test for bad start date
	   D APPADD(.ZZZ,2100123,3100123.3,2,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-2 W "Error in -2",!
	   ; Test for bad end date
	   D APPADD(.ZZZ,3100123,2100123.3,2,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-3 W "Error in -3",!
	   ; Test for end date without time
	   D APPADD(.ZZZ,3100123.1,3100123,2,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-4 W "Error in -4",!
	   ; Test for mumps error
	   S bsdxdie=1
	   D APPADD(.ZZZ,3100123.09,3100123.093,2,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-100 W "Error in -100: M Error",!
	   K bsdxdie
	   ; Test for TRESTART
	   s bsdxrestart=1
	   D APPADD(.ZZZ,3100123.09,3100123.093,3,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=0&(+$P(^BSDXTMP($J,1),U,2)'=-10) W "Error in TRESTART",!
	   k bsdxrestart
	   ; Test for non-numeric patient
	   D APPADD(.ZZZ,3100123.09,3100123.093,"CAT,DOG","Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-5 W "Error in -5",!
	   ; Test for a non-existent patient
	   D APPADD(.ZZZ,3100123.09,3100123.093,8989898989,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-6 W "Error in -6",!
	   ; Test for a non-existent resource name
	   D APPADD(.ZZZ,3100123.09,3100123.093,3,"lkajsflkjsadf",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-7 W "Error in -7",!
	   ; Test for corrupted resource
	   ; Can't test for -8 since it requires DB corruption
	   ; Test for inability to add appointment to BSDX Appointment
	   ; Also requires something wrong in the DB
	   ; Test for inability to add appointment to 2,44
	   ; Test by creating a duplicate appointment
	   D APPADD(.ZZZ,3100123.09,3100123.093,3,"Dr Office",30,"Sam's Note",1)
	   D APPADD(.ZZZ,3100123.09,3100123.093,3,"Dr Office",30,"Sam's Note",1)
	   I +$P(^BSDXTMP($J,1),U,2)'=-10 W "Error in -10",!
	   ; Test for normality:
	   D APPADD(.ZZZ,3110123.09,3110123.093,3,"Dr Office",30,"Sam's Note",1)
	   ; Does Appt exist?
	   N APPID S APPID=+$P(^BSDXTMP($J,1),U)
	   I 'APPID W "Error Making Appt-1" QUIT
	   I +^BSDXAPPT(APPID,0)'=3110123.09 W "Error Making Appt-2"
	   I '$D(^DPT(3,"S",3110123.09)) W "Error Making Appt-3"
	   I '$D(^SC(2,"S",3110123.09)) W "Error Making Appt-4"
	   QUIT
	   ; 
APPADD(BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID)	;EP
	   ;Called by RPC: BSDX ADD NEW APPOINTMENT
	   ;
	   ;Add new appointment to 3 files
	   ; - BSDX APPOINTMENT
	   ; - Hosp Location Appointment SubSubfile if Resource is linked to clinic
	   ; - Patient Appointment Subfile if Resource is linked to clinic
	   ;
	   ;Paramters:
	   ;BSDXY: Global Return (RPC must be set to Global Array)
	   ;BSDXSTART: FM Start Date
	   ;BSDXEND: FM End Date
	   ;BSDXPATID: Patient DFN
	   ;BSDXRES is ResourceName in BSDX RESOURCE file (not IEN)
	   ;BSDXLEN is the appointment duration in minutes
	   ;BSDXNOTE is the Appiontment Note
	   ;BSDXATID is used for 2 purposes:
	   ; if BSDXATID = "WALKIN" then BSDAPI is called to create a walkin appt.
	   ; if BSDXATID = a number, then it is the access type id (used for rebooking)
	   ;
	   ;Return:
	   ; ADO.net Recordset having fields:
	   ; AppointmentID and ErrorNumber
	   ;
	   ;Test lines:
	   ;BSDX ADD NEW APPOINTMENT^3091122.0930^3091122.1000^370^Dr Office^30^EXAM^WALKIN
	   ;
	   ; Return Array; set Return and clear array
	   S BSDXY=$NA(^BSDXTMP($J))
	   K ^BSDXTMP($J)
	   ; $ET
	   N $ET S $ET="G ETRAP^BSDX07"
	   ; Counter
	   N BSDXI S BSDXI=0
	   ; Lock BSDX node, only to synchronize access to the globals.
	   ; It's not expected that the error will ever happen as no filing
	   ; is supposed to take 5 seconds.
	   L +^BSDXAPPT(BSDXPATID):5 I '$T D ERR(BSDXI,"-1~Patient record is locked. Please contact technical support.") Q
	   ; Header Node
	   S ^BSDXTMP($J,BSDXI)="I00020APPOINTMENTID^T00100ERRORID"_$C(30)
	   ;Restartable Transaction; restore paramters when starting.
	   ; (Params restored are what's passed here + BSDXI)
	   TSTART (BSDXY,BSDXSTART,BSDXEND,BSDXPATID,BSDXRES,BSDXLEN,BSDXNOTE,BSDXATID,BSDXI):T="BSDX ADD NEW APPOINTMENT^BSDX07"
	   ;
	   ; Turn off SDAM APPT PROTOCOL BSDX Entries
	   N BSDXNOEV
	   S BSDXNOEV=1 ;Don't execute BSDX ADD APPOINTMENT protocol
	   ;
	   ; Set Error Message to be empty
	   N BSDXERR S BSDXERR=0
	   ;
	   ;;;test for error inside transaction. See if %ZTER works
	   I $G(bsdxdie) S X=1/0
	   ;;;test
	   ;;;test for TRESTART
	   I $G(bsdxrestart) K bsdxrestart TRESTART
	   ;;;test
	   ;
	   ; -- Start and End Date Processing --
	   ; If C# sends the dates with extra zeros, remove them
	   S BSDXSTART=+BSDXSTART,BSDXEND=+BSDXEND
	   ; Are the dates valid? Must be FM Dates > than 2010
	   I BSDXSTART'>3100000 D ERR(BSDXI,"-2~BSDX07 Error: Invalid Start Time") Q
	   I BSDXEND'>3100000 D ERR(BSDXI,"-3~BSDX07 Error: Invalid End Time") Q
	   ;
	   ;; If Ending date doesn't have a time, this is an error --rm 1.5
	   ; I $L(BSDXEND,".")=1 D ERR(BSDXI,"-4~BSDX07 Error: Invalid End Time") Q
	   ;
	   ; If the Start Date is greater than the end date, swap dates
	   N BSDXTMP
	   I BSDXSTART>BSDXEND S BSDXTMP=BSDXEND,BSDXEND=BSDXSTART,BSDXSTART=BSDXTMP
	   ;
	   ; Check if the patient exists:
	   ; - DFN valid number?
	   ; - Valid Patient in file 2?
	   I '+BSDXPATID D ERR(BSDXI,"-5~BSDX07 Error: Invalid Patient ID") Q 
	   I '$D(^DPT(BSDXPATID,0)) D ERR(BSDXI,"-6~BSDX07 Error: Invalid Patient ID") Q
	   ;
	   ;Validate Resource entry
	   I '$D(^BSDXRES("B",BSDXRES)) D ERR(BSDXI,"-7~BSDX07 Error: Invalid Resource ID") Q
	   N BSDXRESD ; Resource IEN
	   S BSDXRESD=$O(^BSDXRES("B",BSDXRES,0))
	   N BSDXRNOD ; Resouce zero node
	   S BSDXRNOD=$G(^BSDXRES(BSDXRESD,0))
	   I BSDXRNOD="" D ERR(BSDXI,"-8~BSDX07 Error: invalid Resource entry.") Q
	   ;
	   ; Walk-in (Unscheduled) Appointment?
	   N BSDXWKIN S BSDXWKIN=0
	   I BSDXATID="WALKIN" S BSDXWKIN=1
	   ; Reset Access Type ID if it doesn't say "WALKIN" and isn't a number
	   I BSDXATID'?.N&(BSDXATID'="WALKIN") S BSDXATID=""
	   ;
	   ; Done with all checks, let's make appointment in BSDX APPOINTMENT
	   N BSDXAPPTID
	   S BSDXAPPTID=$$BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID)
	   I 'BSDXAPPTID D ERR(BSDXI,"-9~BSDX07 Error: Unable to add appointment to BSDX APPOINTMENT file.") Q
	   I BSDXNOTE]"" D BSDXWP(BSDXAPPTID,BSDXNOTE)
	   ;
	   ; Then Create Subfiles in 2/44 Appointment
	   N BSDXSCD S BSDXSCD=$P(BSDXRNOD,U,4)  ; Hosp Location IEN
	   ; Only if we have a valid Hosp Loc can we make an appointment
	   I +BSDXSCD,$D(^SC(BSDXSCD,0)) D  I +BSDXERR D ERR(BSDXI,"-10~BSDX07 Error: MAKE^BSDXAPI returned error code: "_BSDXERR) Q
	   . N BSDXC
	   . S BSDXC("PAT")=BSDXPATID
	   . S BSDXC("CLN")=BSDXSCD
	   . S BSDXC("TYP")=3 ;3 for scheduled appts, 4 for walkins
	   . S:BSDXWKIN BSDXC("TYP")=4
	   . S BSDXC("ADT")=BSDXSTART
	   . S BSDXC("LEN")=BSDXLEN
	   . S BSDXC("OI")=$E($G(BSDXNOTE),1,150) ;File 44 has 150 character limit on OTHER field
	   . S BSDXC("OI")=$TR(BSDXC("OI"),";"," ") ;No semicolons allowed by MAKE^BSDXAPI
	   . S BSDXC("OI")=$$STRIP(BSDXC("OI")) ;Strip control characters from note
	   . S BSDXC("USR")=DUZ
	   . S BSDXERR=$$MAKE^BSDXAPI(.BSDXC)
	   . Q:BSDXERR
	   . ;Update RPMS Clinic availability
	   . D AVUPDT(BSDXSCD,BSDXSTART,BSDXLEN)
	   . Q
	   ;
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
STRIP(BSDXZ)	   ;Replace control characters with spaces
	   N BSDXI
	   F BSDXI=1:1:$L(BSDXZ) I (32>$A($E(BSDXZ,BSDXI))) S BSDXZ=$E(BSDXZ,1,BSDXI-1)_" "_$E(BSDXZ,BSDXI+1,999)
	   Q BSDXZ
	   ;
BSDXADD(BSDXSTART,BSDXEND,BSDXPATID,BSDXRESD,BSDXATID)	 ;ADD BSDX APPOINTMENT ENTRY
	   ;Returns ien in BSDXAPPT or 0 if failed
	   ;Create entry in BSDX APPOINTMENT
	   N BSDXAPPTID
	   S BSDXFDA(9002018.4,"+1,",.01)=BSDXSTART
	   S BSDXFDA(9002018.4,"+1,",.02)=BSDXEND
	   S BSDXFDA(9002018.4,"+1,",.05)=BSDXPATID
	   S BSDXFDA(9002018.4,"+1,",.07)=BSDXRESD
	   S BSDXFDA(9002018.4,"+1,",.08)=$G(DUZ)
	   S BSDXFDA(9002018.4,"+1,",.09)=$$NOW^XLFDT
	   S:BSDXATID="WALKIN" BSDXFDA(9002018.4,"+1,",.13)="y"
	   S:BSDXATID?.N BSDXFDA(9002018.4,"+1,",.06)=BSDXATID
	   N BSDXIEN,BSDXMSG
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
ADDEVT3(BSDXRES)	   ;
	   ;Call RaiseEvent to notify GUI clients
	   N BSDXRESN
	   S BSDXRESN=$G(^BSDXRES(BSDXRES,0))
	   Q:BSDXRESN=""
	   S BSDXRESN=$P(BSDXRESN,"^")
	   ;D EVENT^BSDX23("SCHEDULE-"_BSDXRESN,"","","")
	   D EVENT^BMXMEVN("BSDX SCHEDULE",BSDXRESN)
	   Q
	   ;
ERR(BSDXI,BSDXERR)	 ;Error processing
	   S BSDXI=BSDXI+1
	   S BSDXERR=$TR(BSDXERR,"^","~")
	   I $TL>0 TROLLBACK
	   S ^BSDXTMP($J,BSDXI)="0^"_BSDXERR_$C(30)
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   L -^BSDXAPPT(BSDXPATID)
	   Q
	   ;
ETRAP	  ;EP Error trap entry
	   N $ET S $ET="D ^%ZTER HALT"  ; Emergency Error Trap
	   ; Rollback, otherwise ^XTER will be empty from future rollback
	   I $TL>0 TROLLBACK 
	   D ^%ZTER
	   S $EC=""  ; Clear Error
	   ; Log error message and send to client
	   I '$D(BSDXI) N BSDXI S BSDXI=0
	   D ERR(BSDXI,"-100~BSDX07 Error: "_$G(%ZTERZE))
	   Q
	   ;
DAY	;;^SUN^MON^TUES^WEDNES^THURS^FRI^SATUR
	   ;
DOW	S %=$E(X,1,3),Y=$E(X,4,5),Y=Y>2&'(%#4)+$E("144025036146",Y)
	   F %=%:-1:281 S Y=%#4=1+1+Y
	   S Y=$E(X,6,7)+Y#7
	   Q
	   ;
AVUPDT(BSDXSCD,BSDXSTART,BSDXLEN)	  ;Update RPMS Clinic availability
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
1	  ;L  Q:$D(SDXXX)  S CCXN=0 K MXOK,COV,SDPROT Q:DFN<0  S SC=+SC
	   Q:$D(SDXXX)  S CCXN=0 K MXOK,COV,SDPROT Q:DFN<0  S SC=+SC
	   S X1=DT,SDEDT=365 S:$D(^SC(SC,"SDP")) SDEDT=$P(^SC(SC,"SDP"),"^",2)
	   S X2=SDEDT D C^%DTC S SDEDT=X
	   S Y=BSDXSTART
EN1	S (X,SD)=Y,SM=0 D DOW
S	  I '$D(^SC(SC,"ST",$P(SD,"."),1)) S SS=+$O(^SC(+SC,"T"_Y,SD)) Q:SS'>0  Q:^(SS,1)=""  S ^SC(+SC,"ST",$P(SD,"."),1)=$E($P($T(DAY),U,Y+2),1,2)_" "_$E(SD,6,7)_$J("",SI+SI-6)_^(1),^(0)=$P(SD,".")
	   S S=BSDXLEN
	   ;Check if BSDXLEN evenly divisible by appointment length
	   S RPMSL=$P(SL,U)
	   I BSDXLEN<RPMSL S BSDXLEN=RPMSL
	   I BSDXLEN#RPMSL'=0 D
	   . S BSDXINC=BSDXLEN\RPMSL
	   . S BSDXINC=BSDXINC+1
	   . S BSDXLEN=RPMSL*BSDXINC
	   S SL=S_U_$P(SL,U,2,99)
SC	 S SDLOCK=$S('$D(SDLOCK):1,1:SDLOCK+1) Q:SDLOCK>9
	   L +^SC(SC,"ST",$P(SD,"."),1):5 G:'$T SC
	   S SDLOCK=0,S=^SC(SC,"ST",$P(SD,"."),1)
	   S I=SD#1-SB*100,ST=I#1*SI\.6+($P(I,".")*SI),SS=SL*HSI/60*SDDIF+ST+ST
	   I (I<1!'$F(S,"["))&(S'["CAN") L -^SC(SC,"ST",$P(SD,"."),1) Q
	   I SM<7 S %=$F(S,"[",SS-1) S:'%!($P(SL,"^",6)<3) %=999 I $F(S,"]",SS)'<%!(SDDIF=2&$E(S,ST+ST+1,SS-1)["[") S SM=7
	   ;
SP	 I ST+ST>$L(S),$L(S)<80 S S=S_" " G SP
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
