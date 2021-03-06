-- add custom columns to SALESORD_HDR

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DISPATCHSTATUS') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_DISPATCHSTATUS varchar(20) NULL
END

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DISPATCHMETHOD') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_DISPATCHMETHOD varchar(20) NULL
END

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DUETIME') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_DUETIME datetime NULL
END

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DIFOT_TIMESTAMP') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_DIFOT_TIMESTAMP datetime NULL
END

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_PICKDATE') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_PICKDATE datetime NULL
END 

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_CARRIER') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	ADD X_CARRIER varchar(100) NULL
END

GO

-- add custom fields to DR_TRANS

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_SCHEDULE_TIMESTAMP') 
BEGIN 
	ALTER TABLE DR_TRANS 
	ADD X_SCHEDULE_TIMESTAMP datetime NULL
END

GO

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_TIMESTAMP') 
BEGIN 
	ALTER TABLE DR_TRANS 
	ADD X_DIFOT_TIMESTAMP datetime NULL
END

GO

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DISPATCHMETHOD') 
BEGIN 
	ALTER TABLE DR_TRANS 
	ADD X_DISPATCHMETHOD varchar(20) NULL
END

GO

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_STATUS') 
BEGIN 
	ALTER TABLE DR_TRANS 
	ADD X_DIFOT_STATUS varchar(100) NULL
END

GO

IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_NOTE') 
BEGIN 
	ALTER TABLE DR_TRANS 
	ADD X_DIFOT_NOTE varchar(100) NULL
END

GO

-- add custom fields to SALESORD_LINES

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'SALESORD_LINES' AND  COLUMN_NAME = 'X_ACTION') 
BEGIN 
	ALTER TABLE SALESORD_LINES 
	ADD X_ACTION INT NULL
END

GO

-- create EOA_SALESORD_MAIN table

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
CREATE TABLE EOA_SALESORD_MAIN (
    [#] [int] NOT NULL,
    [ACCOUNTNAME] [varchar](60) NULL,
    [STOCK] [varchar](30) NULL,
    [STATUS] [varchar](30) NULL,
    [METHOD] [varchar](30) NULL,
    [PICKDATE] [datetime] NULL,
    [DUETIME] [datetime] NULL,
    [DUEDATE] [datetime] NULL,
    [LAST_SCHEDULED] [datetime] NULL,
    [DIFOT_TIMESTAMP] [datetime] NULL,
    [ADDRESS1] [varchar](200) NULL,
    [ADDRESS2] [varchar](200) NULL,
    [REFERENCE] [varchar](200) NULL,
	[CUSTORDERNO] [varchar](20) NULL,
	[X_PROJECTNAME] [varchar](100) NULL,
	[X_CARRIER] [varchar](100) NULL,
    [SESSIONID] int not null
    
PRIMARY KEY CLUSTERED 
(
    [#] ASC,SESSIONID
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- create EOA_SALESORD_SECONDARY table

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
CREATE TABLE EOA_SALESORD_SECONDARY (
    [#] [int] NOT NULL,
    [ACCOUNTNAME] [varchar](60) NULL,
    [STOCK] [varchar](30) NULL,
    [STATUS] [varchar](30) NULL,
    [METHOD] [varchar](30) NULL,
    [PICKDATE] [datetime] NULL,
    [DUETIME] [datetime] NULL,
    [DUEDATE] [datetime] NULL,
    [LAST_SCHEDULED] [datetime] NULL,
    [DIFOT_TIMESTAMP] [datetime] NULL,
    [ADDRESS1] [varchar](200) NULL,
    [ADDRESS2] [varchar](200) NULL,
    [REFERENCE] [varchar](200) NULL,
	[CUSTORDERNO] [varchar](20) NULL,
	[X_PROJECTNAME] [varchar](100) NULL,
	[X_CARRIER] [varchar](100) NULL,
    [SESSIONID] int not null
    
PRIMARY KEY CLUSTERED 
(
    [#] ASC,SESSIONID
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- create stored procedure for EOA_SALESORD_MAIN

create procedure query_salesorders_main @sessionId int, @filter varchar(10)
as

BEGIN

delete
from EOA_SALESORD_MAIN
where SESSIONID = @sessionId;

with stocktable as 
(select sales.SEQNO AS 'SALES_SEQNO'
	,SUM(lines.PICK_NOW) AS 'PICKNOW_TOTAL'
	,MIN(lines.PICK_NOW) AS 'PICKNOW_MIN'
	,MAX(lines.PICK_NOW) AS 'PICKNOW_MAX'
	,SUM((lines.UNITPRICE - lines.DISCOUNT*lines.UNITPRICE/100)*lines.PICK_NOW) AS 'UNITPRICE_TOTAL'
	,sum (case
		when lines.PICK_NOW > 0 and (lines.PICK_NOW > locinfo.QTY) then 1
		else 0
	 end) as 'STOCKCHECK'
 from SALESORD_HDR sales
	left join SALESORD_LINES lines
		on sales.SEQNO = lines.HDR_SEQNO
	left join STOCK_ITEMS items
		on lines.STOCKCODE = items.STOCKCODE
	left join STOCK_LOC_INFO locinfo
		on lines.STOCKCODE = locinfo.STOCKCODE
		and lines.LOCATION = locinfo.LOCATION
		where sales.STATUS >= 0 AND sales.STATUS < 2
			and items.STATUS <> 'L'
			and lines.PICK_NOW >= 0
		GROUP BY sales.SEQNO),
		
TimeStamps as
	(SELECT HEADER_SOURCE_SEQ SEQNO
        ,MAX(HISTDATETIME) MaxDate
        FROM SALESORDHIST
        WHERE SALESORDHIST.EVENT_TYPE = '+'
        OR SALESORDHIST.EVENT_TYPE = 'N'
        GROUP BY HEADER_SOURCE_SEQ
	 )

insert into EOA_SALESORD_MAIN
([#],[ACCOUNTNAME],[STOCK],[STATUS],[METHOD],[DUEDATE],[DUETIME],[PICKDATE],[LAST_SCHEDULED],[DIFOT_TIMESTAMP],[ADDRESS1],[ADDRESS2],[REFERENCE],[CUSTORDERNO],[X_PROJECTNAME],[X_CARRIER],[SESSIONID])

select so.SEQNO
		,accs.NAME
		,CASE 
			 WHEN PICKNOW_TOTAL > 0 AND STOCKCHECK = 0 AND (so.X_DISPATCHSTATUS <> 'W' or so.X_DISPATCHSTATUS is null) THEN 'IS'
			 WHEN PICKNOW_TOTAL IS NULL AND (so.X_DISPATCHSTATUS <> 'W' or so.X_DISPATCHSTATUS is null) THEN 'IS'
			 WHEN PICKNOW_TOTAL > 0 AND STOCKCHECK = 0 AND so.X_DISPATCHSTATUS = 'W' THEN 'NowIS'
			 WHEN PICKNOW_TOTAL IS NULL AND so.X_DISPATCHSTATUS = 'W' THEN 'NowIS'
			 WHEN PICKNOW_TOTAL = 0 AND PICKNOW_MIN = 0 AND PICKNOW_MAX = 0 AND so.X_DISPATCHSTATUS is not null THEN 'NPQ'
			 ELSE 'NIS'
		  END
		,so.X_DISPATCHSTATUS
		,so.X_DISPATCHMETHOD
		,so.DUEDATE
		,so.X_DUETIME
		,so.X_PICKDATE
		,difot.MaxDate
		,so.X_DIFOT_TIMESTAMP
		,so.ADDRESS1 + ' ' + so.ADDRESS2
		,so.ADDRESS3 + ' ' + so.ADDRESS4 + ' ' + so.ADDRESS5 + ' ' + so.ADDRESS6
		,so.REFERENCE
		,so.CUSTORDERNO
		,so.X_PROJECTNAME
		,so.X_CARRIER
		,@sessionId

from SALESORD_HDR so
left join DR_ACCS accs
	on so.ACCNO = accs.ACCNO
left join stocktable stock
	on so.SEQNO = stock.SALES_SEQNO
left join TimeStamps difot
	on so.SEQNO = difot.SEQNO
where so.STATUS >= 0 AND so.STATUS < 2
and so.X_DISPATCHSTATUS IS NOT NULL
and so.X_DISPATCHSTATUS = (CASE @filter WHEN 'All' THEN so.X_DISPATCHSTATUS ELSE @filter END)

END

GO

-- create stored procedure for EOA_SALESORD_SECONDARY

create procedure query_salesorders_secondary @sessionId int
as

BEGIN

delete
from EOA_SALESORD_SECONDARY
where SESSIONID = @sessionId;

with stocktable as 
(select sales.SEQNO AS 'SALES_SEQNO'
	,SUM(lines.PICK_NOW) AS 'PICKNOW_TOTAL'
	,SUM((lines.UNITPRICE - lines.DISCOUNT*lines.UNITPRICE/100)*lines.PICK_NOW) AS 'UNITPRICE_TOTAL'
	,sum (case
		when lines.PICK_NOW > 0 and (lines.PICK_NOW > locinfo.QTY) then 1
		else 0
	 end) as 'STOCKCHECK',
	 MAX(lines.PICK_NOW) AS 'PICKNOW_MAX'
 from SALESORD_HDR sales
	left join SALESORD_LINES lines
		on sales.SEQNO = lines.HDR_SEQNO
	left join STOCK_ITEMS items
		on lines.STOCKCODE = items.STOCKCODE
	left join STOCK_LOC_INFO locinfo
		on lines.STOCKCODE = locinfo.STOCKCODE
		and lines.LOCATION = locinfo.LOCATION
	where sales.STATUS >= 0 AND sales.STATUS < 2
		and items.STATUS <> 'L'
	GROUP BY sales.SEQNO),
		
TimeStamps as
	(SELECT HEADER_SOURCE_SEQ SEQNO
        ,MAX(HISTDATETIME) MaxDate
        FROM SALESORDHIST
        WHERE SALESORDHIST.EVENT_TYPE = '+'
        OR SALESORDHIST.EVENT_TYPE = 'N'
        GROUP BY HEADER_SOURCE_SEQ
	 )
		
insert into EOA_SALESORD_SECONDARY
([#],[ACCOUNTNAME],[STOCK],[STATUS],[METHOD],[DUEDATE],[DUETIME],[PICKDATE],[LAST_SCHEDULED],[DIFOT_TIMESTAMP],[ADDRESS1],[ADDRESS2],[REFERENCE],[CUSTORDERNO],[X_PROJECTNAME],[X_CARRIER],[SESSIONID])

select so.SEQNO
		,accs.NAME
		,CASE 
			 WHEN PICKNOW_TOTAL > 0 AND STOCKCHECK = 0 THEN 'IS'
			 WHEN PICKNOW_TOTAL = 0 THEN 'NPQ'
			 WHEN PICKNOW_TOTAL IS NULL THEN 'IS'
			 WHEN PICKNOW_TOTAL < 0 AND PICKNOW_MAX <= 0 THEN 'TR'
			 ELSE 'NIS'
		 END
		,so.X_DISPATCHSTATUS
		,so.X_DISPATCHMETHOD
		,so.DUEDATE
		,so.X_DUETIME
		,so.X_PICKDATE
		,difot.MaxDate
		,so.X_DIFOT_TIMESTAMP
		,so.ADDRESS1 + ' ' + so.ADDRESS2
		,so.ADDRESS3 + ' ' + so.ADDRESS4 + ' ' + so.ADDRESS5 + ' ' + so.ADDRESS6
		,so.REFERENCE
		,so.CUSTORDERNO
		,so.X_PROJECTNAME
		,so.X_CARRIER
		,@sessionId

from SALESORD_HDR so
left join DR_ACCS accs
	on so.ACCNO = accs.ACCNO
left join stocktable stock
	on so.SEQNO = stock.SALES_SEQNO
left join TimeStamps difot
	on so.SEQNO = difot.SEQNO
where so.STATUS >= 0 AND so.STATUS < 2
and (so.X_DISPATCHSTATUS IS NULL or so.X_DISPATCHSTATUS = '')

END

GO

IF NOT EXISTS( SELECT * 
	FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'X_DESPATCHMETHODS') 
BEGIN 
	CREATE TABLE X_DESPATCHMETHODS
	(EMPTY_FIELD varchar(1) null
	,METHOD varchar(30) not null PRIMARY KEY)
	
	insert into X_DESPATCHMETHODS (EMPTY_FIELD, METHOD)
	values ('', 'P')

	insert into X_DESPATCHMETHODS (EMPTY_FIELD, METHOD)
	values ('', 'E1')
	
	insert into X_DESPATCHMETHODS (EMPTY_FIELD, METHOD)
	values ('', 'E4')
	
	insert into X_DESPATCHMETHODS (EMPTY_FIELD, METHOD)
	values ('', 'N')

	insert into X_DESPATCHMETHODS (EMPTY_FIELD, METHOD)
	values ('', '(n)')
END

GO