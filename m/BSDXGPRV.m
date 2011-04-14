BSDXGPRV	; WV/SMH - WINDOWS SCHEDULING RPCS ; 11/2/10 4:27pm
	;;1.5T1;BSDX;;Apr 06, 2011
	;
	;
ERROR	;
	D ERR("RPMS Error")
	Q
	;
ERR(BSDXERR)	;Error processing
	D ^%ZTER
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=BSDXERR
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	Q
	;
PD(BSDXY,HLIEN)	;EP Debugging entry point
	;
	D DEBUG^%Serenji("P^BSDXGPRV(.BSDXY,HLIEN)","192.168.254.130")
	;
	Q
	;
P(BSDXY,HLIEN)	; Public Entry point; Get Providers for Hosp Location
	; Input: HLIEN - Hospital Location IEN
	; Output: ADO Datatable with columns:
	; - HOSPITAL_LOCATION_ID, BMXIEN, PROV_NAME, DEFAULT
	; If there are providers in the PROVIDER multiple of file 44 
	; (Hospital Location) return them;
	; If no providers in PROVIDER multiple of file 44, return nothing
	; Called by BSDX HOSP LOC PROVIDERS
	;
	S BSDXI=0
	I '$D(^SC(HLIEN,0)) D ERR("HOSPITAL LOCATION NOT FOUND") QUIT
	D ^XBKVAR 
	N $ET S $ET="G ERROR^BSDXGPRV"
	K ^BSDXTMP($J)
	S BSDXY=$NA(^BSDXTMP($J))
	S $P(^BSDXTMP($J,BSDXI),U,1)="I00020HOSPITAL_LOCATION_ID"
	S $P(^BSDXTMP($J,BSDXI),U,2)="I00020BMXIEN"
	S $P(^BSDXTMP($J,BSDXI),U,3)="T00030NAME"
	S $P(^BSDXTMP($J,BSDXI),U,4)="T00005DEFAULT"
	S ^BSDXTMP($J,BSDXI)=^BSDXTMP($J,BSDXI)_$C(30)
	;
	N OUTPUT
	D GETS^DIQ(44,HLIEN_",","2600*","IE","OUTPUT") ; Provider Multiple
	; No results
	I '$D(OUTPUT) S ^BSDXTMP($J,BSDXI+1)=$C(31) QUIT
	; if results, get them
	N I S I=""
	F  S I=$O(OUTPUT(44.1,I)) Q:I=""  D
	. S BSDXI=BSDXI+1
	. S $P(^BSDXTMP($J,BSDXI),U,1)=HLIEN                  ; HL IEN
	. S $P(^BSDXTMP($J,BSDXI),U,2)=$P(OUTPUT(44.1,I,.01,"I"),",") ; PROV IEN
	. S $P(^BSDXTMP($J,BSDXI),U,3)=$E(OUTPUT(44.1,I,.01,"E"),1,30) ; PROV NAME
	. S $P(^BSDXTMP($J,BSDXI),U,4)=OUTPUT(44.1,I,.02,"E") ; Default - YES, NO
	. S ^BSDXTMP($J,BSDXI)=^BSDXTMP($J,BSDXI)_$C(30)
	S BSDXI=BSDXI+1
	S ^BSDXTMP($J,BSDXI)=$C(31)
	QUIT
