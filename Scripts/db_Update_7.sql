SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EOA_SO_ITEM_DETAILS](
	[SEQNO] [int] NOT NULL,
	[LINES_ID] [int] NOT NULL,
	[STOCKCODE] [varchar](23) NULL,
	[DESCRIPTION] [varchar](100) NULL,
	[STOCKCHECK] [varchar](12) NULL,
	[PICK_NOW] [float] NULL,
	[UNSUP_QUANT] [float] NULL,
	[TOTALSTOCK] [float] NULL,
	[FREE] [float] NULL,
	[ALLOCATED] [float] NULL,
	[DUEDATE] [datetime] NULL,
	[LOCATION] [varchar](100) NULL,
	[X_ACTION] [int] NULL,
	[X_HEADING_LINE] char(1) NULL,
	[X_HIDEFROMPICK] char(1) NULL,
	[LINETYPE] INT NOT NULL,
	[SESSIONID] INT NOT NULL
	
PRIMARY KEY CLUSTERED 
(
	[SEQNO] ASC,
	[LINES_ID] ASC,
	[SESSIONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

create PROCEDURE eoa_fetch_so_item_details @sessionId int, @seqno int
AS

BEGIN

delete
from dbo.EOA_SO_ITEM_DETAILS
where SESSIONID = @sessionId;

with qtyTable as (
select
lines.STOCKCODE,
purch_lines.ORD_QUANT - purch_lines.SUP_QUANT as 'ON ORDER',
lines.UNSUP_QUANT as 'OUTSTANDING',
locinfo.QTY as 'ON HAND',
lines.DUEDATE AS 'SALESORD_DUEDATE',
purch_lines.DUEDATE as 'PURCH_DUEDATE'
from SALESORD_HDR sales
left join SALESORD_LINES lines
	on sales.SEQNO = lines.HDR_SEQNO
left join STOCK_LOC_INFO locinfo
	on lines.STOCKCODE = locinfo.STOCKCODE
left join PURCHORD_LINES purch_lines
	on purch_lines.STOCKCODE = lines.STOCKCODE
left join PURCHORD_HDR purch_hdr
	on purch_lines.HDR_SEQNO = purch_hdr.SEQNO
where (lines.HDR_STATUS = 0 or lines.HDR_STATUS = 1)
	and lines.LOCATION = 1
	and locinfo.LOCATION = 1
	and (purch_hdr.STATUS = 0 or purch_hdr.STATUS = 1)
	and purch_lines.LOCATION = 1
	and sales.SEQNO = @seqno
),

freeTable as (
SELECT
STOCKCODE,
SUM([ON ORDER]) AS 'ON ORDER',
SUM([ON HAND]) AS 'ON HAND',
SUM(OUTSTANDING) AS 'OUTSTANDING'
from qtyTable
	WHERE CONVERT(DATE, SALESORD_DUEDATE) < DATEADD(DAY,61,CONVERT(DATE, GETDATE()))
	AND CONVERT(DATE, PURCH_DUEDATE) < DATEADD(DAY,15,CONVERT(DATE, GETDATE()))
	group by STOCKCODE
),

allocatedTable as (
SELECT
STOCKCODE,
SUM([ON ORDER]) AS 'ON ORDER',
SUM([ON HAND]) AS 'ON HAND',
SUM(OUTSTANDING) AS 'OUTSTANDING'
from qtyTable
	WHERE CONVERT(DATE, SALESORD_DUEDATE) <= CONVERT(DATE, GETDATE())
	AND CONVERT(DATE, PURCH_DUEDATE) <= CONVERT(DATE, SALESORD_DUEDATE)
	group by STOCKCODE
)

INSERT INTO dbo.EOA_SO_ITEM_DETAILS
select distinct 
	sales.SEQNO
	,lines.SEQNO
	,lines.STOCKCODE
	,lines.[DESCRIPTION]
	,case
			when locinfo.QTY > 0 and (lines.PICK_NOW > locinfo.QTY) and items.STATUS <> 'L' and items.STOCKCODE is NOt null and items.STOCKCODE <> '' then 'NIS'
			when lines.PICK_NOW = 0 and locinfo.QTY > 0 and (UNSUP_QUANT > locinfo.QTY) and items.STATUS <> 'L' and items.STOCKCODE is NOt null and items.STOCKCODE <> '' then 'NIS'
			when (lines.UNSUP_QUANT < 0 or lines.PICK_NOW < 0) and items.STATUS <> 'L' and items.STOCKCODE is NOt null and items.STOCKCODE <> '' then 'TR'
			when locinfo.QTY = 0 and UNSUP_QUANT <> 0 and items.STATUS <> 'L' and items.STOCKCODE is NOt null and items.STOCKCODE <> '' then 'OOS'
			when lines.UNSUP_QUANT = 0 and items.STATUS <> 'L' and items.STOCKCODE is NOt null and items.STOCKCODE <> '' then 'Sh'
			when items.STATUS = 'L' or items.STOCKCODE is null or items.STOCKCODE = '' then ''
			else 'IS'
     end as 'STOCKCHECK'
     ,lines.PICK_NOW
     ,lines.UNSUP_QUANT
     ,locinfo.QTY
	 ,free.[ON ORDER] + free.[ON HAND] - free.OUTSTANDING as 'FREE'
	 ,allocated.[ON ORDER] + allocated.[ON HAND] - allocated.OUTSTANDING as 'ALLOCATED'
	 ,lines.DUEDATE
	 ,CONCAT(stocklocations.LOCNO, ' ', stocklocations.LCODE)
	 ,lines.X_ACTION	
	 ,items.X_HEADING_LINE
	 ,items.X_HIDEFROMPICK
	 ,lines.LINETYPE
     ,@sessionId
	 from SALESORD_HDR sales
		left join SALESORD_LINES lines
			on sales.SEQNO = lines.HDR_SEQNO
		left join STOCK_ITEMS items
			on lines.STOCKCODE = items.STOCKCODE
		left join STOCK_LOC_INFO locinfo
			on lines.STOCKCODE = locinfo.STOCKCODE
			and lines.LOCATION = locinfo.LOCATION
		left join STOCK_LOCATIONS stocklocations
			on lines.LOCATION = stocklocations.LOCNO
		left join freeTable free
			on lines.STOCKCODE = free.STOCKCODE
		left join allocatedTable allocated
			on lines.STOCKCODE = allocated.STOCKCODE
			WHERE lines.SEQNO is not null
			and sales.SEQNO = @seqno

END
			
GO

CREATE PROCEDURE eoa_so_item_details_update_PICK_NOW @sessionId int, @seqno int, @lines_id int
AS

begin

UPDATE SALESORD_LINES 
	SET SALESORD_LINES.PICK_NOW = temp.PICK_NOW
FROM SALESORD_LINES lines
	JOIN EOA_SO_ITEM_DETAILS temp
		ON lines.HDR_SEQNO = temp.SEQNO
		and lines.STOCKCODE = temp.STOCKCODE
		and lines.SEQNO = temp.LINES_ID
		WHERE temp.SESSIONID = @sessionId
		and temp.SEQNO = @seqno
		and temp.LINES_ID = @lines_id
			
end

go

CREATE PROCEDURE eoa_so_item_details_update_LOCATION_FAKE @sessionId int, @seqno int, @lines_id int
AS

begin

UPDATE SALESORD_LINES 
	SET SALESORD_LINES.LOCATION = SUBSTRING(temp.LOCATION, 0, CHARINDEX(' ', temp.LOCATION, 0))
FROM SALESORD_LINES lines
	JOIN EOA_SO_ITEM_DETAILS temp
		ON lines.HDR_SEQNO = temp.SEQNO
		and lines.STOCKCODE = temp.STOCKCODE
		and lines.SEQNO = temp.LINES_ID
		WHERE temp.SESSIONID = @sessionId
		and temp.SEQNO = @seqno
		and temp.LINES_ID = @lines_id
			
end

go

CREATE PROCEDURE eoa_so_item_details_update_X_ACTION @sessionId int, @seqno int, @lines_id int
AS

begin

UPDATE SALESORD_LINES
	SET SALESORD_LINES.X_ACTION = temp.X_ACTION
FROM SALESORD_LINES lines
	JOIN EOA_SO_ITEM_DETAILS temp
		ON lines.HDR_SEQNO = temp.SEQNO
		and lines.STOCKCODE = temp.STOCKCODE
		and lines.SEQNO = temp.LINES_ID
		WHERE temp.SESSIONID = @sessionId
		and temp.SEQNO = @seqno
		and temp.LINES_ID = @lines_id
			
end

go

CREATE PROCEDURE eoa_so_item_details_update_DUEDATE_FAKE @sessionId int, @seqno int, @lines_id int
AS

begin

UPDATE SALESORD_LINES 
	SET SALESORD_LINES.DUEDATE = temp.DUEDATE
FROM SALESORD_LINES lines
	JOIN EOA_SO_ITEM_DETAILS temp
		ON lines.HDR_SEQNO = temp.SEQNO
		and lines.STOCKCODE = temp.STOCKCODE
		and lines.SEQNO = temp.LINES_ID
		WHERE temp.SESSIONID = @sessionId
		and temp.SEQNO = @seqno
		and temp.LINES_ID = @lines_id
			
end

go