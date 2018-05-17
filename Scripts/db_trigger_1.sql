IF EXISTS (SELECT name FROM sysobjects WHERE name = 'status_is_scheduled' AND type = 'TR')
BEGIN
    DROP TRIGGER status_is_scheduled
END
GO

create TRIGGER status_is_scheduled ON SALESORD_HDR
AFTER UPDATE
as begin

	IF UPDATE (X_DISPATCHSTATUS)
	BEGIN
		SET NOCOUNT ON;

		Declare @status varchar(2)
		Declare @seqno int
		
		SELECT 
			@status = inserted.X_DISPATCHSTATUS,
			@seqno = inserted.SEQNO
		FROM
			inserted
		
		if @status = 'W' or @status = 'TA'
		begin
			UPDATE SALESORD_HDR
				SET X_DISPATCHMETHOD = NULL,
					X_PICKDATE = NULL,
					X_DUETIME = NULL
			where SEQNO = @seqno
		end
		
		if @status = 'Sc' or @status = 'W' or @status = 'TA'
		begin
			exec update_difot_timestamp @seqno
		end
		
	END
end

go