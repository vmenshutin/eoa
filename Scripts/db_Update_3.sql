CREATE PROCEDURE update_difot_timestamp @seqno int
AS
	declare @maxNar int;
	declare @maxNum int;
	declare @method varchar(5);
	DECLARE @status varchar(5);
	
	set @method = (SELECT X_DISPATCHMETHOD FROM SALESORD_HDR WHERE SEQNO = @seqno);
	set @status = (SELECT X_DISPATCHSTATUS FROM SALESORD_HDR WHERE SEQNO = @seqno);
	
	if (@method is null and @status = 'W')
	begin
		set @method = 'W';
	end
	
	if (@method is null and @status = 'TA')
	begin
		set @method = 'TA';
	end
	
	set @maxNar  = (select max(SEQNO) from SALESORDHIST);
	set @maxNum  = (select max(HISTORYNO) from SALESORDHIST where HEADER_SOURCE_SEQ = @seqno);
	if ((exists (SELECT SEQNO from SALESORDHIST where HEADER_SOURCE_SEQ = @seqno and HISTORYNO = @maxNum AND (EVENT_TYPE = 'S' or EVENT_TYPE = '+'))) or ISNULL(@maxNum,0) <= 0)
		set @maxNum = ISNULL(@maxNum + 1, 0);
	
	SET IDENTITY_INSERT SALESORDHIST ON

	insert into SALESORDHIST (SEQNO, 
							  LINES_SOURCE_SEQ, 
							  HEADER_SOURCE_SEQ, 
							  HISTORYNO,
							  EVENT_TYPE
							  ,HISTDATETIME
							  ,DR_INVLINES_SEQ
							  ,ST_TRANS_SEQ
							  ,MANIFEST_NO
							  ,FILEURL
							  ,QTY
							  ,BSOLP_BATCHNO
							  ,SALESNO
							  ,PHYS_STAFF
							  ,PHYS_BRANCH)
	VALUES ( ISNULL(@maxNar + 1,1), 0, @seqno, ISNULL(@maxNum,0), '+', CURRENT_TIMESTAMP, null, null, null, @method, null, null, null, null, null)
			
	SET IDENTITY_INSERT SALESORDHIST OFF
	
	UPDATE sales
		set sales.X_DIFOT_TIMESTAMP = (
			case
			 when sales.X_DISPATCHMETHOD <> 'C' AND sales.X_DISPATCHMETHOD <> 'P' then DATEADD(MI, DATEPART(MINUTE, sales.X_DUETIME),(DATEADD(HH, DATEPART(HOUR, sales.X_DUETIME), (DATEADD(DD, 0, DATEDIFF(dd, 0, sales.X_PICKDATE))))))
			 when sales.X_DISPATCHMETHOD = 'P' AND (sales.X_DUETIME IS NULL OR sales.X_DUETIME = '') AND sales.X_PICKDATE IS NOT NULL AND sales.X_PICKDATE <> '' then DATEADD(HH, 17, sales.X_PICKDATE)
			 when sales.X_DISPATCHMETHOD = 'P' AND sales.X_DUETIME IS NOT NULL AND sales.X_DUETIME <> '' AND (sales.X_PICKDATE IS NULL OR sales.X_PICKDATE = '') then DATEADD(MI, DATEPART(MI,sales.X_DUETIME) , DATEADD(HH, DATEPART(HH,sales.X_DUETIME),sales.DUEDATE))
			 when sales.X_DISPATCHMETHOD = 'P' AND (sales.X_DUETIME IS NULL OR sales.X_DUETIME = '') AND (sales.X_PICKDATE IS NULL OR sales.X_PICKDATE = '') then DATEADD(HH, 17, sales.DUEDATE)
			 when sales.X_DISPATCHMETHOD = 'P' AND (sales.X_DUETIME IS NOT NULL AND sales.X_DUETIME <> '') AND (sales.X_PICKDATE IS NOT NULL AND sales.X_PICKDATE <> '') then DATEADD(MI, DATEPART(MI,sales.X_DUETIME) , DATEADD(HH, DATEPART(HH,sales.X_DUETIME),sales.X_PICKDATE))
			 else sales.X_DIFOT_TIMESTAMP
			end) 
		FROM SALESORD_HDR sales
		WHERE sales.SEQNO = @seqno	
GO