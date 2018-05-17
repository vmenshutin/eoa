CREATE TRIGGER EOA_CLEAR_DATE_AFTER_INSERT ON SALESORD_HDR
AFTER INSERT
AS
BEGIN

SET NOCOUNT ON;

declare @insertedseqno int;
select @insertedseqno = i.SEQNO from inserted i;

UPDATE sales
SET sales.X_DISPATCHSTATUS = NULL
	,sales.X_DISPATCHMETHOD = NULL
	,sales.X_DUETIME = NULL
	,sales.X_DIFOT_TIMESTAMP = NULL
	,sales.X_PICKDATE = NULL
	FROM SALESORD_HDR sales
	WHERE sales.SEQNO = @insertedseqno			
END

GO