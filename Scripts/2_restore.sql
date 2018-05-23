UPDATE sales
SET sales.X_DISPATCHSTATUS = btemp.X_DISPATCHSTATUS
	,sales.X_DISPATCHMETHOD = btemp.X_DISPATCHMETHOD
	,sales.X_DUETIME = btemp.X_DUETIME
	,sales.X_DIFOT_TIMESTAMP = btemp.X_DIFOT_TIMESTAMP
	,sales.X_PICKDATE = btemp.X_PICKDATE
	,sales.X_CARRIER = btemp.X_CARRIER
FROM SALESORD_HDR sales
	LEFT JOIN EOA_BACKUP_1 btemp
		ON sales.SEQNO = btemp.SEQNO
		
GO

UPDATE trans
SET trans.X_DIFOT_STATUS = btemp.X_DIFOT_STATUS
	,trans.X_SCHEDULE_TIMESTAMP = btemp.X_SCHEDULE_TIMESTAMP
	,trans.X_DIFOT_TIMESTAMP = btemp.X_DIFOT_TIMESTAMP
	,trans.X_DISPATCHMETHOD = btemp.X_DISPATCHMETHOD
	,trans.X_DIFOT_NOTE = btemp.X_DIFOT_NOTE
FROM DR_TRANS trans
	LEFT JOIN EOA_BACKUP_2 btemp
		ON trans.SEQNO = btemp.SEQNO
		
GO

UPDATE lines
SET lines.X_ACTION = btemp.X_ACTION
FROM SALESORD_LINES lines
	LEFT JOIN EOA_BACKUP_4 btemp
		ON lines.SEQNO = btemp.SEQNO
		
GO
		
IF NOT EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'EOA_BACKUP_3' AND COLUMN_NAME = 'LABEL_PRINTER') 
BEGIN 
	ALTER TABLE EOA_BACKUP_3 
	ADD LABEL_PRINTER varchar(255) NULL
END

GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_BACKUP_3')
BEGIN
	DELETE FROM EOA_SETTINGS
	
	INSERT INTO EOA_SETTINGS
	SELECT * FROM EOA_BACKUP_3

	UPDATE EOA_SETTINGS
	SET LABEL_PRINTER = 'Microsoft Print to PDF'
	WHERE LABEL_PRINTER IS NULL
END

GO