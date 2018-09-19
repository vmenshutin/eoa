create procedure get_stockcodes_by_purchord_hdr_seqno @seqno int
as
begin

select 
items.STOCKCODE,
items.DESCRIPTION,
items.BARCODE1
from PURCHORD_LINES lines
left join STOCK_ITEMS items
	on lines.STOCKCODE = items.STOCKCODE
where lines.HDR_SEQNO = @seqno
	and lines.STOCKCODE IS NOT NULL
	and lines.STOCKCODE <> ''

end
go

create procedure get_stockcodes_by_salesord_hdr_seqno @seqno int
as
begin

select 
items.STOCKCODE,
items.DESCRIPTION,
items.BARCODE1
from SALESORD_LINES lines
left join STOCK_ITEMS items
	on lines.STOCKCODE = items.STOCKCODE
where lines.HDR_SEQNO = @seqno
	and lines.STOCKCODE IS NOT NULL
	and lines.STOCKCODE <> ''

end
go