SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EOA_DIFOT](
    [INVNO] varchar(20) NOT NULL,
    [#] [int] null,
    [ACCOUNTNAME] varchar(255) null,
    [X_DIFOT_STATUS] varchar(100) null,
    [X_DIFOT_NOTE] varchar(100) null,
    [X_LEAD_TIME] datetime null,
    [X_SCHEDULE_TIMESTAMP] datetime null,
    [X_DIFOT_TIMESTAMP] datetime null,
    [X_DESPATCH_METHOD] varchar(20) null,
    SESSIONID int not null
    
PRIMARY KEY CLUSTERED 
(
    [INVNO] ASC, SESSIONID
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

create procedure eoa_query_difot_items @dateFrom datetime, @dateTo datetime, @sessionId int
as
begin

	delete
	from [dbo].[EOA_DIFOT]
	where SESSIONID = @sessionId;

	INSERT INTO [EOA_DIFOT]
	([INVNO] , [#], [ACCOUNTNAME], [X_DESPATCH_METHOD], [X_LEAD_TIME] , [X_SCHEDULE_TIMESTAMP],
		[X_DIFOT_STATUS] , [X_DIFOT_TIMESTAMP], [X_DIFOT_NOTE], [SESSIONID])

	select trans.INVNO
			,sales.SEQNO
			,accs.NAME
			,trans.X_DISPATCHMETHOD
			,trans.POSTTIME
			,trans.X_SCHEDULE_TIMESTAMP
			,case
				when LOWER(trans.X_DIFOT_STATUS) <> 'shipped late' and LOWER(trans.X_DIFOT_STATUS) <> 'difot'  then null
				else LOWER(trans.X_DIFOT_STATUS)
			end
			--,trans.X_DIFOT_STATUS
			,trans.X_DIFOT_TIMESTAMP
			,trans.X_DIFOT_NOTE
			,@sessionId
	FROM DR_TRANS trans
	RIGHT JOIN SALESORD_HDR sales
		ON trans.REF1 = CONVERT(varchar(10), sales.SEQNO)
	left join DR_ACCS accs
			on sales.ACCNO = accs.ACCNO		
		where trans.REF3 = 'Invoice' and 
		CONVERT(date, POSTTIME) >= @dateFrom and CONVERT(date, POSTTIME) <= @dateTo
	ORDER BY trans.INVNO DESC
end

go

create procedure eoa_query_difot_items_secondary @dateFrom datetime, @dateTo datetime, @searchstring varchar (100), @sessionId int
as
begin

	SELECT @searchstring = RTRIM(@searchstring) + '%';

	delete
	from [dbo].[EOA_DIFOT]
	where SESSIONID = @sessionId;

	INSERT INTO [EOA_DIFOT]
	([INVNO] , [#], [ACCOUNTNAME], [X_DESPATCH_METHOD], [X_LEAD_TIME] , [X_SCHEDULE_TIMESTAMP],
		[X_DIFOT_STATUS] , [X_DIFOT_TIMESTAMP], [X_DIFOT_NOTE], [SESSIONID])

	select trans.INVNO
			,sales.SEQNO
			,accs.NAME
			,trans.X_DISPATCHMETHOD
			,trans.POSTTIME
			,trans.X_SCHEDULE_TIMESTAMP
			,case
				when LOWER(trans.X_DIFOT_STATUS) <> 'shipped late' and LOWER(trans.X_DIFOT_STATUS) <> 'difot'  then null
				else LOWER(trans.X_DIFOT_STATUS)
			end
			--,trans.X_DIFOT_STATUS
			,trans.X_DIFOT_TIMESTAMP
			,trans.X_DIFOT_NOTE
			,@sessionId
	FROM DR_TRANS trans
	RIGHT JOIN SALESORD_HDR sales
		ON trans.REF1 = CONVERT(varchar(10), sales.SEQNO)
	left join DR_ACCS accs
			on sales.ACCNO = accs.ACCNO		
		where trans.REF3 = 'Invoice' and 
		CONVERT(date, POSTTIME) >= @dateFrom and CONVERT(date, POSTTIME) <= @dateTo
		and (trans.INVNO like @searchstring
		or sales.SEQNO like @searchstring
		or accs.NAME like @searchstring
		or trans.X_DIFOT_NOTE like @searchstring)
	ORDER BY trans.INVNO DESC
end

go

---------------------------------------------------------------------------

CREATE PROCEDURE eoa_update_difot_X_DIFOT_TIMESTAMP @sessionId int, @invno varchar(20)
AS
	
		UPDATE DR_TRANS 
		SET DR_TRANS.X_DIFOT_TIMESTAMP = temp.X_DIFOT_TIMESTAMP
	FROM DR_TRANS 
		JOIN EOA_DIFOT temp
			ON DR_TRANS.INVNO = temp.INVNO
			WHERE temp.SESSIONID = @sessionId
			and temp.INVNO = @invno
			
GO


CREATE PROCEDURE eoa_update_difot_DIFOT_FAKE @sessionId int, @invno varchar(20)
AS
	
		UPDATE DR_TRANS 
		SET DR_TRANS.X_DIFOT_STATUS = temp.X_DIFOT_STATUS
	FROM DR_TRANS 
		JOIN EOA_DIFOT temp
			ON DR_TRANS.INVNO = temp.INVNO
			WHERE temp.SESSIONID = @sessionId
			and temp.INVNO = @invno
			
GO

CREATE PROCEDURE eoa_update_difot_X_DIFOT_NOTE @sessionId int, @invno varchar(20)
AS
	
		UPDATE DR_TRANS 
		SET DR_TRANS.X_DIFOT_NOTE = temp.X_DIFOT_NOTE
	FROM DR_TRANS 
		JOIN EOA_DIFOT temp
			ON DR_TRANS.INVNO = temp.INVNO
			WHERE temp.SESSIONID = @sessionId
			and temp.INVNO = @invno
			
GO