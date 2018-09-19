-- salesord_hdr

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DISPATCHSTATUS') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_DISPATCHSTATUS
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DISPATCHMETHOD') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_DISPATCHMETHOD
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DUETIME') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_DUETIME
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_DIFOT_TIMESTAMP') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_DIFOT_TIMESTAMP
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_PICKDATE') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_PICKDATE
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_HDR' AND  COLUMN_NAME = 'X_CARRIER') 
BEGIN 
	ALTER TABLE SALESORD_HDR 
	DROP COLUMN X_CARRIER
END

go

-- DR_TRANS

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_SCHEDULE_TIMESTAMP') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_SCHEDULE_TIMESTAMP
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_TIMESTAMP') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_DIFOT_TIMESTAMP
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DISPATCHMETHOD') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_DISPATCHMETHOD
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_STATUS') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_DIFOT_STATUS
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_LEAD_TIME') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_LEAD_TIME
END

go

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DR_TRANS' AND  COLUMN_NAME = 'X_DIFOT_NOTE') 
BEGIN 
	ALTER TABLE DR_TRANS 
	DROP COLUMN X_DIFOT_NOTE
END

go

-- SALESORD_LINES

IF EXISTS( SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SALESORD_LINES' AND  COLUMN_NAME = 'X_ACTION') 
BEGIN 
	ALTER TABLE SALESORD_LINES
	DROP COLUMN X_ACTION
END

go

-- drop tables -----------------------------------------------------------------------------

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_SALESORD_MAIN')
    BEGIN
		drop table EOA_SALESORD_MAIN
    END

go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_SALESORD_SECONDARY')
    BEGIN
		drop table EOA_SALESORD_SECONDARY
    END

go
    
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_SESSIONS')
    BEGIN
		drop table EOA_SESSIONS
    END
    
go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_SO_ITEM_DETAILS')
    BEGIN
		drop table EOA_SO_ITEM_DETAILS
    END
    
go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_DIFOT')
    BEGIN
		drop table EOA_DIFOT
    END
    
go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'X_DESPATCHMETHODS')
    BEGIN
		drop table X_DESPATCHMETHODS
    END
    
go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_SETTINGS')
    BEGIN
		drop table EOA_SETTINGS
    END
    
go

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'EOA_NARRATIVE')
    BEGIN
		drop table EOA_NARRATIVE
    END
    
go

-- drop stored procedures ------------------------------------------------------------------ 
    
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'query_salesorders_main')
	DROP PROCEDURE query_salesorders_main
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'query_salesorders_secondary')
	DROP PROCEDURE query_salesorders_secondary
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'clear_obsolete_sessions')
	DROP PROCEDURE clear_obsolete_sessions
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_authentificate')
	DROP PROCEDURE eoa_authentificate
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'update_difot_timestamp')
	DROP PROCEDURE update_difot_timestamp
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_secondary_update_STATUS')
	DROP PROCEDURE so_secondary_update_STATUS
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_secondary_update_METHOD')
	DROP PROCEDURE so_secondary_update_METHOD
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_secondary_update_DUEDATE')
	DROP PROCEDURE so_secondary_update_DUEDATE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_secondary_update_DUETIME')
	DROP PROCEDURE so_secondary_update_DUETIME
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_secondary_update_PICKDATE')
	DROP PROCEDURE so_secondary_update_PICKDATE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_main_update_STATUS')
	DROP PROCEDURE so_main_update_STATUS
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_main_update_METHOD')
	DROP PROCEDURE so_main_update_METHOD
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_main_update_DUEDATE')
	DROP PROCEDURE so_main_update_DUEDATE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_main_update_DUETIME')
	DROP PROCEDURE so_main_update_DUETIME
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_main_update_PICKDATE')
	DROP PROCEDURE so_main_update_PICKDATE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'so_update_CARRIER')
	DROP PROCEDURE so_update_CARRIER
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'secondary_search_orders')
	DROP PROCEDURE secondary_search_orders
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_fetch_so_item_details')
	DROP PROCEDURE eoa_fetch_so_item_details
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_so_item_details_update_PICK_NOW')
	DROP PROCEDURE eoa_so_item_details_update_PICK_NOW
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_so_item_details_update_LOCATION_FAKE')
	DROP PROCEDURE eoa_so_item_details_update_LOCATION_FAKE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_so_item_details_update_X_ACTION')
	DROP PROCEDURE eoa_so_item_details_update_X_ACTION
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_update_difot_X_DIFOT_TIMESTAMP')
	DROP PROCEDURE eoa_update_difot_X_DIFOT_TIMESTAMP
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_update_difot_DIFOT_FAKE')
	DROP PROCEDURE eoa_update_difot_DIFOT_FAKE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_update_difot_X_DIFOT_NOTE')
	DROP PROCEDURE eoa_update_difot_X_DIFOT_NOTE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_query_difot_items')
	DROP PROCEDURE eoa_query_difot_items
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_query_difot_items_secondary')
	DROP PROCEDURE eoa_query_difot_items_secondary
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_process_pick')
	DROP PROCEDURE eoa_process_pick
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_pick_all')
	DROP PROCEDURE eoa_pick_all
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'EOA_get_narrative')
	DROP PROCEDURE EOA_get_narrative
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_update_narrative')
	DROP PROCEDURE eoa_update_narrative
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_transfer')
	DROP PROCEDURE eoa_transfer
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_duplicate')
	DROP PROCEDURE eoa_duplicate
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'get_sales_order_report')
	DROP PROCEDURE get_sales_order_report
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'eoa_force_fully_processed')
	DROP PROCEDURE eoa_force_fully_processed
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'get_stockcodes_by_purchord_hdr_seqno')
	DROP PROCEDURE get_stockcodes_by_purchord_hdr_seqno
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'get_stockcodes_by_salesord_hdr_seqno')
	DROP PROCEDURE get_stockcodes_by_salesord_hdr_seqno
GO

-- drop triggers ------------------------------------------------------------------------------

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'status_is_scheduled' AND type = 'TR')
BEGIN
    DROP TRIGGER status_is_scheduled
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'EOA_SCHEDULE' AND type = 'TR')
BEGIN
    DROP TRIGGER EOA_SCHEDULE
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'EOA_CLEAR_SCHEDULING_DATA' AND type = 'TR')
BEGIN
    DROP TRIGGER EOA_CLEAR_SCHEDULING_DATA
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'DIFOT' AND type = 'TR')
BEGIN
    DROP TRIGGER DIFOT
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'EOA_CLEAR_DATE_AFTER_INSERT' AND type = 'TR')
BEGIN
    DROP TRIGGER EOA_CLEAR_DATE_AFTER_INSERT
END
GO