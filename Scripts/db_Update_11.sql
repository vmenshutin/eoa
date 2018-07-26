create procedure eoa_transfer 
@stockcode varchar(50),
@reference varchar(50),
@ref1 varchar(20),
@quantity int,
@location varchar(50),
@toLocation varchar(50),
@insertIntoHdr BIT,
@transtype int
as
BEGIN

declare @periodSeqno int;
declare @hdrSeqno int;
declare @unitPrice float;
declare @sessionId int;

SET @periodSeqno = (SELECT MAX(SEQNO) FROM PERIOD_STATUS WHERE LEDGER = 'S');
SET @unitPrice = (SELECT AVECOST FROM STOCK_ITEMS WHERE STOCKCODE = @stockcode);

if @insertIntoHdr = 1
begin
	INSERT INTO STOCK_TRANS_HDR (
	TRANSDATE,PERIOD_SEQNO,TRANSTYPE,REFERENCE,STAFFNO,NARRATIVE_SEQNO)
	VALUES(
		DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())), -- TRANSDATE -- today date
		@periodSeqno, -- PERIOD_SEQNO -- the last “s” ledger in dbo.period_status
		@transtype, -- TRANSTYPE
		@reference, -- REFERENCE
		1, -- STAFFNO -- STAFF.STAFFNO - default to 1
		-1 -- NARRATIVE_SEQNO -- SALESORD_LINES.NARRATIVE_SEQNO --->>> ???
	)
end

SET @hdrSeqno = (SELECT MAX(SEQNO) FROM STOCK_TRANS_HDR);
SET @sessionId = (SELECT MAX(SESSION_ID)+1 FROM STOCK_TRANS);

INSERT INTO STOCK_TRANS (
TRANSDATE,STOCKCODE,TRANSTYPE,REF1,REF2,QUANTITY,
UNITPRICE,UNITCOST,LOCATION,TOLOCATION, FROM_LEDGER, 
GLBRANCH, GLACC, GLSUBACC, PERIOD_SEQNO,FROM_HDR,POST_TO_GL,PLU,SESSION_ID)
VALUES (
	DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())), -- TRANSDATE -- today date
	@stockcode, -- STOCKCODE
	@transtype, -- TRANSTYPE
	@ref1, -- REF1 -- reference
	@reference, -- REF2 -- static
	@quantity, -- QUANTITY -- negative value of X_ACTION
	@unitPrice, -- UNITPRICE -- dbo.stock_items avecost
	@unitPrice, -- UNITCOST -- dbo.stock_items avecost
	SUBSTRING(@location, 0, CHARINDEX(' ', @location, 0)), -- LOCATION -- current location of a selected item in bottom-right grid
	SUBSTRING(@toLocation, 0, CHARINDEX(' ', @location, 0)), -- TOLOCATION -- what is selected in a dropdown by the Transfer button
	's', -- FROM_LEDGER -- static
	0, -- GLBRANCH -- static
	5030, -- GLACC -- static
	0, -- GLSUBACC -- static
	@periodSeqno, -- PERIOD_SEQNO -- the last “s” ledger in dbo.period_status
	@hdrSeqno, -- FROM_HDR -- dbo.stock_trans_hdr seqno
	'Y', -- POST_TO_GL -- static
	@stockcode, -- PLU -- stockcode again
	@sessionId -- SESSION_ID -- sequential number assigned to all dbo.stock_trans lines created by that "transfer" transaction
)

END

GO

create procedure eoa_duplicate @seqno int, @quantity int, @location varchar(50), @reference varchar(50), @addReference BIT
as
BEGIN

if @addReference = 1
begin
insert into SALESORD_LINES (
	ACCNO,
	HDR_SEQNO,
	STOCKCODE,
	DESCRIPTION,
	ORD_QUANT,
	SUP_QUANT,
	INV_QUANT,
	UNITPRICE,
	DISCOUNT,
	ANALYSIS,
	LOCATION,
	SUPPLY_NOW,
	INVOICE_NOW,
	JOBCODE,
	BATCHCODE,
	SUBCODE,
	BRANCHNO,
	LAST_SUP,
	LAST_INV,
	TAXRATE,
	TAXRATE_NO,
	LINETAX_OVERRIDE,
	LINETAX_OVERRIDDEN,
	SERIALNO,
	RELEASE_QUANT,
	BINCODE,
	LSTATUS,
	LISTPRICE,
	CONTRACT_HDR,
	LINKED_STOCKCODE,
	LINKED_QTY,
	BKORD_QUANT,
	PICK_NOW,
	PICKED_QUANT,
	LAST_PICKED,
	RELEASE_NOW,
	LAST_RELEASED,
	NARRATIVE_SEQNO,
	PRICE_OVERRIDDEN,
	KITCODE,
	HDR_STATUS,
	LINETYPE,
	KITSEQNO,
	BOMTYPE,
	SHOWLINE,
	LINKEDSTATUS,
	BOMPRICING,
	HIDDEN_SELL,
	CORRECTION_QUANT,
	CUSTORDERNO,
	DUEDATE,
	SUPPLIERNO,
	PURCHORDNO,
	ORIGINAL_KEY,
	X_SO_STATUS,
	X_DISP_SO_STATUS,
	X_VALADD,
	X_DIFOT,
	OPPLINEID,
	X_AVECOST,
	BKORD_BATCHNO,
	BSOLP_BATCHNO,
	X_ACTION
)
select ACCNO, -- copy
	HDR_SEQNO, -- copy
	'', -- STOCKCODE static
	@reference, -- DESCRIPTION passed in
	0, -- ORD_QUANT static
	0, -- SUP_QUANT static
	0, -- INV_QUANT static
	0, -- UNITPRICE static
	0, -- DISCOUNT static
	0, -- ANALYSIS static
	SUBSTRING(@location, 0, CHARINDEX(' ', @location, 0)), -- LOCATION passed in 
	0, -- SUPPLY_NOW static
	0, -- INVOICE_NOW static
	'', -- JOBCODE static
	'', -- BATCHCODE static
	0, -- SUBCODE static
	0, -- BRANCHNO static
	0, -- LAST_SUP static
	0, -- LAST_INV static
	0, -- TAXRATE static
	0, -- TAXRATE_NO static
	0, -- LINETAX_OVERRIDE static
	'N', -- LINETAX_OVERRIDDEN static
	'', -- SERIALNO static
	0, -- RELEASE_QUANT static
	'', -- BINCODE static
	0, -- LSTATUS static
	0, -- LISTPRICE static
	0, -- CONTRACT_HDR static
	'', -- LINKED_STOCKCODE static
	1, -- LINKED_QTY static 
	0, -- BKORD_QUANT static
	0, -- PICK_NOW static
	0, -- PICKED_QUANT static
	0, -- LAST_PICKED static
	0, -- RELEASE_NOW static
	0, -- LAST_RELEASED static
	-1, -- NARRATIVE_SEQNO static
	'N', -- PRICE_OVERRIDDEN static
	'', -- KITCODE static
	HDR_STATUS, -- copied
	4, -- LINETYPE static
	-1, -- KITSEQNO static
	'N', -- BOMTYPE static
	'Y', -- SHOWLINE static
	'S', -- LINKEDSTATUS static
	'N', -- BOMPRICING static
	0, -- HIDDEN_SELL static
	0, -- CORRECTION_QUANT static
	'', -- CUSTERORDERNO static
	DUEDATE, -- copied
	0, -- SUPPLIERNO static
	0, -- PURCHORDNO static
	NULL, -- ORIGINAL_KEY static
	NULL, -- X_SO_STATUS static
	NULL, -- X_DISP_SO_STATUS static
	NULL, -- X_VALADD static
	NULL, -- X_DIFOT static
	-1, -- OPPLINEID static
	NULL, -- X_AVECOST static
	0, -- BKORD_BATCHNO static
	-1, -- BSOLP_BATCHNO static
	NULL -- X_ACTION static
from SALESORD_LINES
where seqno = @seqno
end

insert into SALESORD_LINES (
	ACCNO,
	HDR_SEQNO,
	STOCKCODE,
	DESCRIPTION,
	ORD_QUANT,
	SUP_QUANT,
	INV_QUANT,
	UNITPRICE,
	DISCOUNT,
	ANALYSIS,
	LOCATION,
	SUPPLY_NOW,
	INVOICE_NOW,
	JOBCODE,
	BATCHCODE,
	SUBCODE,
	BRANCHNO,
	LAST_SUP,
	LAST_INV,
	TAXRATE,
	TAXRATE_NO,
	LINETAX_OVERRIDE,
	LINETAX_OVERRIDDEN,
	SERIALNO,
	RELEASE_QUANT,
	BINCODE,
	LSTATUS,
	LISTPRICE,
	SOLINEID,
	CONTRACT_HDR,
	LINKED_STOCKCODE,
	LINKED_QTY,
	BKORD_QUANT,
	PICK_NOW,
	PICKED_QUANT,
	LAST_PICKED,
	RELEASE_NOW,
	LAST_RELEASED,
	NARRATIVE_SEQNO,
	PRICE_OVERRIDDEN,
	KITCODE,
	HDR_STATUS,
	LINETYPE,
	KITSEQNO,
	BOMTYPE,
	SHOWLINE,
	LINKEDSTATUS,
	BOMPRICING,
	HIDDEN_SELL,
	CORRECTION_QUANT,
	CUSTORDERNO,
	DUEDATE,
	SUPPLIERNO,
	PURCHORDNO,
	ORIGINAL_KEY,
	X_SO_STATUS,
	X_DISP_SO_STATUS,
	X_VALADD,
	X_DIFOT,
	OPPLINEID,
	X_AVECOST,
	BKORD_BATCHNO,
	BSOLP_BATCHNO,
	X_ACTION
)
select ACCNO,
	HDR_SEQNO,
	STOCKCODE,
	DESCRIPTION,
	@quantity,
	0,
	0,
	UNITPRICE,
	DISCOUNT,
	ANALYSIS,
	SUBSTRING(@location, 0, CHARINDEX(' ', @location, 0)),
	0,
	0,
	JOBCODE,
	BATCHCODE,
	SUBCODE,
	BRANCHNO,
	0,
	0,
	TAXRATE,
	TAXRATE_NO,
	LINETAX_OVERRIDE,
	LINETAX_OVERRIDDEN,
	SERIALNO,
	0,
	BINCODE,
	LSTATUS,
	LISTPRICE,
	SOLINEID,
	CONTRACT_HDR,
	LINKED_STOCKCODE,
	LINKED_QTY,
	0,
	0,
	0,
	0,
	0,
	0,
	NARRATIVE_SEQNO,
	PRICE_OVERRIDDEN,
	KITCODE,
	HDR_STATUS,
	LINETYPE,
	KITSEQNO,
	BOMTYPE,
	SHOWLINE,
	LINKEDSTATUS,
	BOMPRICING,
	HIDDEN_SELL,
	CORRECTION_QUANT,
	CUSTORDERNO,
	DUEDATE,
	SUPPLIERNO,
	PURCHORDNO,
	ORIGINAL_KEY,
	X_SO_STATUS,
	X_DISP_SO_STATUS,
	X_VALADD,
	X_DIFOT,
	OPPLINEID,
	X_AVECOST,
	BKORD_BATCHNO,
	BSOLP_BATCHNO,
	X_ACTION
from SALESORD_LINES
where seqno = @seqno

update SALESORD_LINES
SET ORD_QUANT = ORD_QUANT - @quantity
where seqno = @seqno

END

GO