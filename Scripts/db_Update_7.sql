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

CREATE PROCEDURE eoa_fetch_so_item_details @sessionId int, @seqno int
AS

BEGIN

delete
from dbo.EOA_SO_ITEM_DETAILS
where SESSIONID = @sessionId;

with onHandTable as (
	SELECT lines.SEQNO, SUM(loc_info.QTY) AS ON_HAND
	FROM SALESORD_HDR orders
		LEFT JOIN SALESORD_LINES lines
			ON orders.SEQNO = lines.HDR_SEQNO
		LEFT JOIN STOCK_LOC_INFO loc_info
			ON lines.STOCKCODE = loc_info.STOCKCODE
			AND lines.LOCATION = loc_info.LOCATION
		WHERE orders.SEQNO = @seqno
		GROUP BY lines.SEQNO
),

onPurchaseOrderTable as (
	select sales.SEQNO, sum(purch.ORD_QUANT - purch.SUP_QUANT) as ON_PURCHASE_ORDER
		from SALESORD_HDR orders
			LEFT JOIN SALESORD_LINES sales
				on orders.SEQNO = sales.HDR_SEQNO
			LEFT JOIN PURCHORD_LINES purch
				on sales.STOCKCODE = purch.STOCKCODE
				and sales.LOCATION = purch.LOCATION
			LEFT JOIN PURCHORD_HDR hdr
				on purch.HDR_SEQNO = hdr.SEQNO
		WHERE (hdr.STATUS = 0 or hdr.STATUS = 1)
			AND CONVERT(DATE, purch.DUEDATE) <= CONVERT(DATE, sales.DUEDATE)
			AND orders.SEQNO = @seqno
		GROUP BY sales.SEQNO

),

onSalesOrderTable as (
	SELECT lines.SEQNO, sum(lines2.UNSUP_QUANT) as ON_SALES_ORDER
	from SALESORD_HDR orders
		left join SALESORD_LINES lines
			ON orders.SEQNO = lines.HDR_SEQNO
		left join SALESORD_LINES lines2
			ON lines.STOCKCODE = lines2.STOCKCODE
			AND lines.LOCATION = lines2.LOCATION
		WHERE (lines2.HDR_STATUS = 0 OR lines2.HDR_STATUS = 1)	
			and CONVERT(DATE, lines2.DUEDATE) <= CONVERT(DATE, lines.DUEDATE)
			AND orders.SEQNO = @seqno
		GROUP BY lines.SEQNO
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
	 , case
			when items.STATUS = 'L' or items.STOCKCODE is null or items.STOCKCODE = '' then NULL
			else ((IIF(onHand.ON_HAND IS NOT NULL, onHand.ON_HAND, 0)) + (IIF(onPurchaseOrder.ON_PURCHASE_ORDER IS NOT NULL, onPurchaseOrder.ON_PURCHASE_ORDER, 0)) - (IIF(onSalesOrder.ON_SALES_ORDER IS NOT NULL, onSalesOrder.ON_SALES_ORDER, 0)))
	 end as 'ALLOCATED'
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
		left join onHandTable onHand
			on lines.SEQNO = onHand.SEQNO
		left join onPurchaseOrderTable onPurchaseOrder
			on lines.SEQNO = onPurchaseOrder.SEQNO
		left join onSalesOrderTable onSalesOrder
			on lines.SEQNO = onSalesOrder.SEQNO
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