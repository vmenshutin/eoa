SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EOA_NARRATIVE](
    [SEQNO] [int] NOT NULL,
    [NARRATIVE] [varchar](4096) NULL,
    [NARRATIVE_SEQNO] [int] NULL,
    [SESSION_ID] int not null
    
PRIMARY KEY CLUSTERED 
(
    [SEQNO] ASC, SESSION_ID
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-------------------------------------------------------------

CREATE PROCEDURE EOA_get_narrative @seqno int, @sessionId int
AS

BEGIN

delete
from [dbo].[EOA_NARRATIVE]
where SESSION_ID = @sessionId;

INSERT INTO [dbo].[EOA_NARRATIVE]
select sales.SEQNO
		,nar.NARRATIVE
		,sales.NARRATIVE_SEQNO
		,@sessionId
	from SALESORD_HDR sales 
		left join NARRATIVES nar
			on sales.NARRATIVE_SEQNO = nar.SEQNO
			where sales.SEQNO = @seqno

END

GO

-------------------------------------------------------------

create PROCEDURE eoa_update_narrative @narrative varchar(4096), @sessionId int 
AS

BEGIN

	declare @narrativeSeqno int;
	set @narrativeSeqno = (SELECT TOP 1 NARRATIVE_SEQNO FROM EOA_NARRATIVE where SESSION_ID = @sessionId);

	IF (@narrativeSeqno <> -1)
	BEGIN
		UPDATE NARRATIVES
		SET NARRATIVES.NARRATIVE = @narrative
		FROM NARRATIVES
		JOIN EOA_NARRATIVE temp
			ON NARRATIVES.SEQNO = temp.NARRATIVE_SEQNO	
			WHERE temp.SESSION_ID = @sessionId
	END
	
	ELSE
	
	BEGIN
		declare @maxNar int;
		set @maxNar  = (select max(SEQNO) from narratives);
		SET IDENTITY_INSERT NARRATIVES ON

		insert into NARRATIVES (SEQNO, NARRATIVE)
		values (@maxNar + 1, @narrative)
						
		SET IDENTITY_INSERT NARRATIVES OFF
						
		UPDATE SALESORD_HDR 
			SET SALESORD_HDR.NARRATIVE_SEQNO = @maxNar + 1
		FROM SALESORD_HDR 
		JOIN EOA_NARRATIVE temp
			ON SALESORD_HDR.SEQNO=temp.SEQNO
			WHERE temp.SESSION_ID = @sessionId
	END		
END
	
GO