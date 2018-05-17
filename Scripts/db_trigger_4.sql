create TRIGGER DIFOT ON DR_TRANS
AFTER INSERT 
  as 
  BEGIN
	
	SET NOCOUNT ON;
	
	declare @inserted_trans_seqno int;
	declare @inserted_trans_ref3 varchar(50);
	declare @inserted_trans_ref1 varchar(50);
	
	select @inserted_trans_seqno = i.SEQNO from inserted i;
	select @inserted_trans_ref3 = i.REF3 from inserted i;
	select @inserted_trans_ref1 = i.REF1 from inserted i;
	
	if (@inserted_trans_ref3 = 'Invoice' and @inserted_trans_ref1 is not null and @inserted_trans_ref1 <> '') 
	BEGIN
	
	WITH TimeStamps as
	(SELECT HEADER_SOURCE_SEQ SEQNO
			,MAX(HISTDATETIME) MaxDate
			FROM SALESORDHIST
			WHERE SALESORDHIST.EVENT_TYPE = '+'
			OR SALESORDHIST.EVENT_TYPE = 'N'
			GROUP BY HEADER_SOURCE_SEQ
	 )		
	
	UPDATE trans
		SET trans.X_DIFOT_STATUS = 
		(CASE
			WHEN CAST(trans.POSTTIME AS DATE) <= CAST(sales.X_DIFOT_TIMESTAMP AS DATE) THEN 'difot'
		    when CAST(trans.POSTTIME AS DATE) > CAST(sales.X_DIFOT_TIMESTAMP AS DATE) then 'shipped late'
			else trans.X_DIFOT_STATUS
		 END
		)
		, trans.X_SCHEDULE_TIMESTAMP = TimeStamps.MaxDate
		, trans.X_DIFOT_TIMESTAMP = sales.X_DIFOT_TIMESTAMP
		, trans.X_DISPATCHMETHOD = sales.X_DISPATCHMETHOD
		, trans.X_DIFOT_NOTE = sales.X_CARRIER
		FROM DR_TRANS trans
		RIGHT JOIN SALESORD_HDR sales
			ON trans.REF1 = CONVERT(varchar(10), sales.SEQNO)
			left join TimeStamps
				on sales.SEQNO = TimeStamps.SEQNO
		WHERE trans.SEQNO = @inserted_trans_seqno;
		
		UPDATE sales
		SET sales.X_DISPATCHSTATUS = NULL
			,sales.X_DISPATCHMETHOD = '(n)'
			,sales.X_DUETIME = NULL
			,sales.X_DIFOT_TIMESTAMP = NULL
			,sales.X_PICKDATE = NULL
			,sales.X_CARRIER = NULL
			
		FROM SALESORD_HDR sales
		WHERE CONVERT(varchar(10), sales.SEQNO) = @inserted_trans_ref1
			
	END	
	 
  END
      
GO
    