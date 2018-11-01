create procedure main_search_orders @sessionId int, @searchstring varchar (100), @includeAll bit
as

BEGIN

SELECT @searchstring = RTRIM(@searchstring) + '%';

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
		where (@includeAll = 1 or (sales.STATUS >= 0 AND sales.STATUS < 2))
			and items.STATUS <> 'L'
			and lines.PICK_NOW >= 0
			and ((CONVERT(varchar(10), sales.SEQNO) like @searchstring
			or (sales.ADDRESS1 + ' ' + sales.ADDRESS2 + ' ' + sales.ADDRESS3 + ' ' + sales.ADDRESS4 + ' ' + sales.ADDRESS5 + ' ' + sales.ADDRESS6) like @searchstring))
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
where (@includeAll = 1 or (so.STATUS >= 0 AND so.STATUS < 2))
and so.X_DISPATCHSTATUS IS NOT NULL
and (CONVERT(varchar(10), so.SEQNO) like @searchstring
	or accs.NAME like @searchstring
	or (so.ADDRESS1 + ' ' + so.ADDRESS2 + ' ' + so.ADDRESS3 + ' ' + so.ADDRESS4 + ' ' + so.ADDRESS5 + ' ' + so.ADDRESS6) like @searchstring)

END

GO

create procedure secondary_search_orders @sessionId int, @searchstring varchar (100), @includeAll bit
as

BEGIN

SELECT @searchstring = RTRIM(@searchstring) + '%';

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
	 end) as 'STOCKCHECK'
	,MAX(lines.PICK_NOW) AS 'PICKNOW_MAX'
 from SALESORD_HDR sales
	 left join DR_ACCS accs
		on sales.ACCNO = accs.ACCNO
	left join SALESORD_LINES lines
		on sales.SEQNO = lines.HDR_SEQNO
	left join STOCK_ITEMS items
		on lines.STOCKCODE = items.STOCKCODE
	left join STOCK_LOC_INFO locinfo
		on lines.STOCKCODE = locinfo.STOCKCODE
		and lines.LOCATION = locinfo.LOCATION
		where (@includeAll = 1 or (sales.STATUS >= 0 AND sales.STATUS < 2))
			and items.STATUS <> 'L'
			and ((CONVERT(varchar(10), sales.SEQNO) like @searchstring
			or accs.NAME like @searchstring
			or (sales.ADDRESS1 + ' ' + sales.ADDRESS2 + ' ' + sales.ADDRESS3 + ' ' + sales.ADDRESS4 + ' ' + sales.ADDRESS5 + ' ' + sales.ADDRESS6) like @searchstring))
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
([#],[ACCOUNTNAME],[STOCK],[STATUS],[METHOD],[DUEDATE],[DUETIME],[PICKDATE],[LAST_SCHEDULED],[DIFOT_TIMESTAMP],[ADDRESS1],[ADDRESS2],[REFERENCE],[CUSTORDERNO],[X_PROJECTNAME],[SESSIONID])

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
		,@sessionId

from SALESORD_HDR so
left join DR_ACCS accs
	on so.ACCNO = accs.ACCNO
left join stocktable stock
	on so.SEQNO = stock.SALES_SEQNO
left join TimeStamps difot
	on so.SEQNO = difot.SEQNO
where (@includeAll = 1 or (so.STATUS >= 0 AND so.STATUS < 2))
and (so.X_DISPATCHSTATUS IS NULL or so.X_DISPATCHSTATUS = '')
and (CONVERT(varchar(10), so.SEQNO) like @searchstring
	or accs.NAME like @searchstring
	or (so.ADDRESS1 + ' ' + so.ADDRESS2 + ' ' + so.ADDRESS3 + ' ' + so.ADDRESS4 + ' ' + so.ADDRESS5 + ' ' + so.ADDRESS6) like @searchstring)

END

GO