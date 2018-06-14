SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EOA_SETTINGS](
    [PRINTER_NAME] varchar(255) NOT NULL,
	[LABEL_PRINTER] varchar(255) NULL,
	[30_LABEL_PRINTER] varchar(255) NULL,
	[PICK_LABEL_PRINTER] varchar(255) NULL
)

GO

INSERT INTO EOA_SETTINGS
VALUES ('', 'Microsoft Print to PDF', 'Microsoft Print to PDF', 'Microsoft Print to PDF');

GO

SET ANSI_PADDING OFF
GO

CREATE PROCEDURE eoa_process_pick @seqno int
as begin

UPDATE lines
set lines.BKORD_QUANT = lines.UNSUP_QUANT - lines.PICK_NOW
	,lines.SUPPLY_NOW = lines.PICK_NOW
from [SALESORD_LINES] lines
	left join STOCK_ITEMS items
		on lines.STOCKCODE = items.STOCKCODE 
			where lines.HDR_SEQNO = @seqno
			and lines.STOCKCODE <> '' and lines.STOCKCODE IS NOT NULL

end

GO

CREATE PROCEDURE eoa_pick_all @seqno int
as
begin

UPDATE SALESORD_LINES 
SET PICK_NOW = UNSUP_QUANT
FROM SALESORD_LINES lines
	WHERE HDR_SEQNO = @seqno
	and STOCKCODE IS NOT NULL
	AND STOCKCODE <> ''

end
go