SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
CREATE TABLE EOA_SESSIONS(
    SESSIONID [int] NOT NULL,
    TIME_STAMP [datetime] NULL
    
PRIMARY KEY CLUSTERED 
(
    SESSIONID
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'clear_obsolete_sessions')
	DROP PROCEDURE clear_obsolete_sessions
GO

create procedure clear_obsolete_sessions
as
begin

delete EOA_SALESORD_MAIN
	from EOA_SESSIONS ses
		left join EOA_SALESORD_MAIN SO
		on ses.SESSIONID = so.SESSIONID
		where CAST(ses.TIME_STAMP as DATE) <= CAST(DATEADD(dd, -1, DATEDIFF(dd, 0, GETDATE())) as DATE)
		
delete EOA_SALESORD_SECONDARY
	from EOA_SESSIONS ses
		left join EOA_SALESORD_SECONDARY SO
		on ses.SESSIONID = so.SESSIONID
		where CAST(ses.TIME_STAMP as DATE) <= CAST(DATEADD(dd, -1, DATEDIFF(dd, 0, GETDATE())) as DATE)	

delete
	from EOA_SESSIONS
		where CAST(TIME_STAMP as DATE) <= CAST(DATEADD(dd, -1, DATEDIFF(dd, 0, GETDATE())) as DATE)

end

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_authentificate')
	DROP PROCEDURE eoa_authentificate
GO

create procedure eoa_authentificate @sessionId int
as
begin

INSERT INTO EOA_SESSIONS (SESSIONID, TIME_STAMP)
	VALUES (@sessionId, CURRENT_TIMESTAMP);

end

GO