BSDX05	 ; IHS/OIT/HMW - WINDOWS SCHEDULING RPCS ; 4/28/11 10:16am
	   ;;1.7T2;BSDX;;Jul 11, 2012;Build 18
	   ; Licensed under LGPL
	   ;
	   ; Change Log:
	   ; UJO/SMH - July 11 2010: pass FM Dates for Start and End rather than US Dates
	   ; UJO/SMH - Dec 8 2010: In STCOMM, the logic was that an appointment 
	   ;  that was a walk-in didn't count towards slot calculations.
	   ;  I checked PIMS, and Walk-ins do indeed count towards slot calculations.
	   ;  Therefore, I commented this line out:
	   ;    ;Q:$P(BSDXNOD,U,13)="y"  ;WALKIN
	   ;
APBLKOV(BSDXY,BSDXSTART,BSDXEND,BSDXRES)	    ;EP
	   ;Called by BSDX APPT BLOCKS OVERLAP
	      ; July 11 2010 - pass FM Dates for Start and End rather than US Dates
	   ;(Duplicates old qryAppointmentBlocksOverlapB)
	   ;BSDXRES is resource name
	   ;
	   ;Test lines:
	   ;D APBLKOV^BSDX05(.RES,"11-8-2000","11-8-2004","WHITT") ZW RES
	   ;BSDX APPT BLOCKS OVERLAP^11-8-2000^11-8-2004^WHITT
	   ;S ^HW("BSDXD05")=BSDXSTART_U_BSDXEND_U_BSDXRES
	   ;
	   N BSDXERR,BSDXIEN,BSDXDEP,BSDXBS,BSDXI,BSDXNEND,BSDXNSTART,BSDXPEND,BSDXRESD,BSDXRESN,BSDXS,BSDXAD,BSDXNOD
	   K ^BSDXTMP($J)
	   S BSDXERR=""
	   S BSDXY="^BSDXTMP("_$J_")"
	   S ^BSDXTMP($J,0)="D00030START_TIME^D00030END_TIME"_$C(30)
	   D
	   . S BSDXBS=0
	   . S BSDXEND=BSDXEND+.9999 ;Go to end of day
	   . S BSDXRESN=BSDXRES
	   . Q:BSDXRESN=""
	   . Q:'$D(^BSDXRES("B",BSDXRESN))
	   . S BSDXRESD=$O(^BSDXRES("B",BSDXRESN,0))
	   . Q:'+BSDXRESD
	   . Q:'$D(^BSDXAPPT("ARSRC",BSDXRESD))
	   . D STRES(BSDXRESD,BSDXSTART,BSDXEND)
	   . Q
	   ;
	   S BSDXI=$G(BSDXI)+1
	   S ^BSDXTMP($J,BSDXI)=$C(31)
	   Q
	   ;
STRES(BSDXRESD,BSDXSTART,BSDXEND)	  ;
	   ;$O THRU "ARSRC" XREF OF ^BSDXAPPT
	   ;Start at the beginning of the day -- appts can't overlap days
	   S BSDXS=$P(BSDXSTART,"."),BSDXS=BSDXS-.0001
	   S BSDXI=0
	   F  S BSDXS=$O(^BSDXAPPT("ARSRC",BSDXRESD,BSDXS)) Q:'+BSDXS  Q:BSDXS>BSDXEND  D
	   . S BSDXAD=0 F  S BSDXAD=$O(^BSDXAPPT("ARSRC",BSDXRESD,BSDXS,BSDXAD)) Q:'+BSDXAD  D STCOMM(BSDXAD) ;BSDXAD Is the AppointmentID
	   . Q
	   Q
	   ;
STCOMM(BSDXAD)	 ;
	   S BSDXNEND=0,BSDXNSTART=0,BSDXPEND=0
	   Q:'$D(^BSDXAPPT(BSDXAD,0))
	   S BSDXNOD=^BSDXAPPT(BSDXAD,0)
	   Q:$P(BSDXNOD,U,10)=1  ;NO-SHOW Flag
	   Q:$P(BSDXNOD,U,12)]""  ;CANCELLED APPT
	   ; Q:$P(BSDXNOD,U,13)="y"  ;WALKIN -- new in V 1.42. See top comments.
	   S BSDXNSTART=$P(BSDXNOD,U)
	   S BSDXNEND=$P(BSDXNOD,U,2)
	   I BSDXNEND'>BSDXSTART Q  ;End is less than start
	   S Y=BSDXNSTART X ^DD("DD") S BSDXNSTART=$TR(Y,"@"," ")
	   S Y=BSDXNEND X ^DD("DD") S BSDXNEND=$TR(Y,"@"," ")
	   S BSDXI=BSDXI+1
	   S ^BSDXTMP($J,BSDXI)=BSDXNSTART_U_BSDXNEND_$C(30)
	   Q
