CREATE PROCEDURE get_sales_order_report @seqno int
as
begin
	select so.SEQNO
			,accs.NAME AS ACCS_NAME
			,accs.ADDRESS1 as ACCS_ADDRESS1
			,accs.ADDRESS2 as ACCS_ADDRESS2
			,accs.ADDRESS3 as ACCS_ADDRESS3
			,accs.ADDRESS4 as ACCS_ADDRESS4
			,accs.ADDRESS5 as ACCS_ADDRESS5
			,accs.POST_CODE
			,so.ADDRESS1
			,so.ADDRESS2
			,so.ADDRESS3
			,so.ADDRESS4
			,so.ADDRESS5
			,so.ADDRESS6
			,so.DUEDATE
			,so.REFERENCE
			,so.CUSTORDERNO
			,so.X_PROJECTNAME
			,nar.NARRATIVE
	from SALESORD_HDR so
	left join DR_ACCS accs
		on so.ACCNO = accs.ACCNO
	left join NARRATIVES nar
		on so.NARRATIVE_SEQNO = nar.SEQNO
	where so.SEQNO = @seqno
end

go

CREATE PROCEDURE eoa_force_fully_processed @seqno int
AS
BEGIN

	--- 1 ---

	UPDATE SALESORD_HDR SET
	STATUS=2,
	BACKORDER='N',
	ORDSTATUS=160,
	FINALISATION_DATE=format(getdate(), 'dd-MMM-yyyy HH:mm:ss.mmm')
	WHERE SEQNO=@seqno

	--- 2 ---

	UPDATE SALESORD_LINES SET
	HDR_STATUS = 2
	,CORRECTION_QUANT = ORD_QUANT - SUP_QUANT
	,BKORD_QUANT = 0
	WHERE (HDR_SEQNO = @seqno)

	--- 3 ---

	DECLARE @x_unreleased char(1)
	DECLARE @x_backordered char(1)
	DECLARE @x_unsupplied char(1)
	DECLARE @x_uninvoiced char(1)
	DECLARE @x_unpicked char(1)

	EXECUTE SALESORD_LINESTATUSES @seqno,
	@UNRELEASED=@x_unreleased OUTPUT,
	@BACKORDERED=@x_backordered OUTPUT,
	@UNSUPPLIED=@x_unsupplied OUTPUT,
	@UNINVOICED=@x_uninvoiced OUTPUT,
	@UNPICKED=@x_unpicked OUTPUT

	UPDATE SALESORD_HDR SET
	HAS_UNRELEASED=@x_unreleased,
	HAS_BACKORDERS=@x_backordered,
	HAS_UNSUPPLIED=@x_unsupplied,
	HAS_UNINVOICED=@x_uninvoiced,
	HAS_UNPICKED=@x_unpicked,
	LAST_UPDATED=format(getdate(), 'dd-MMM-yyyy HH:mm:ss.mmm')
	WHERE SEQNO=@seqno

	--- 4 ---

	UPDATE STOCK_SERIALNOS SET ISASSIGNED = 'N'
	WHERE ((ISASSIGNED = 'o') AND (ASSIGNED_SEQNO = @seqno))

	--- 5 ---

	UPDATE SALESORD_HDR SET
	PROCESSFINALISATION=2
	WHERE SEQNO=@seqno

	--- 6 ---

	INSERT INTO REASON_EVENTS
	(REASONSEQNO, CLASSNO, FROM_LEDGER, FROM_HEADER, LINE_SOURCE,
	PHYS_STAFF, PHYS_BRANCH, MEMO, POSTTIME, OLD_VAL, NEW_VAL,SUPER_STAFF,
	CR_ACCNO, DR_ACCNO, PROSPECT_SEQNO, CONTACT_SEQNO, STOCKCODE, SERIALNO,SU_SEQNO, REF1)
	VALUES
	(	-1, -- REASONSEQNO
		 152, -- CLASSNO
		 'o', -- FROM_LEDGER
		 @seqno, -- FROM_HEADER
		 -1, -- LINE_SOURCE
		 1, -- PHYS_STAFF
		 0, -- PHYS_BRANCH
		 'Order #: ' + CAST(@seqno as varchar) + '. Set to fully processed from EOA', -- MEMO
		 format(getdate(), 'yyyy-MM-dd HH:mm:ss'), -- POSTTIME
		 20, -- OLD_VAL
		 160, -- NEW_VAL
		 -1, -- SUPER_STAFF
		 -1, -- CR_ACCNO
		 0, -- DR_ACCNO
		 -1, -- PROSPECT_SEQNO
		 -1, -- CONTACT_SEQNO
		 '', -- STOCKCODE
		 '', -- SERIALNO
		 -1, -- SU_SEQNO
		 '' -- REF1
	)

	--- 7 ---

	Insert into SALESORDHIST
	(HEADER_SOURCE_SEQ, SALESNO, HISTDATETIME, PHYS_STAFF, PHYS_BRANCH, SUBJECT, NOTE, EVENT_TYPE)
	Values 
	(	@seqno, -- HEADER_SOURCE_SEQ
		1, -- SALESNO
		format(getdate(), 'yyyy-MM-dd HH:mm:ss'), -- HISTDATETIME
		1, -- PHYS_STAFF
		0, -- PHYS_BRANCH
		'Sales Order Short or Over Supplied', -- SUBJECT
		'Order #: ' + CAST(@seqno as varchar) + '. Set to fully processed from EOA', -- NOTE
		'H' -- EVENT_TYPE
	)

END

GO