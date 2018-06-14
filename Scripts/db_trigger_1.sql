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