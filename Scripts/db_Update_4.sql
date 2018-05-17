-- update scripts

CREATE PROCEDURE so_secondary_update_STATUS @sessionId int, @seqno int
AS	
UPDATE SALESORD_HDR 
SET SALESORD_HDR.X_DISPATCHSTATUS = eoa.[STATUS]
FROM SALESORD_HDR 
JOIN EOA_SALESORD_SECONDARY eoa
	ON SALESORD_HDR.SEQNO=eoa.#
	WHERE eoa.SESSIONID = @sessionId
	and eoa.# = @seqno	
GO

CREATE PROCEDURE so_secondary_update_METHOD @sessionId int, @seqno int
AS	
UPDATE SALESORD_HDR 
SET SALESORD_HDR.X_DISPATCHMETHOD = eoa.METHOD
FROM SALESORD_HDR 
JOIN EOA_SALESORD_SECONDARY eoa
	ON SALESORD_HDR.SEQNO=eoa.#
	WHERE eoa.SESSIONID = @sessionId
	and eoa.# = @seqno	
GO

CREATE PROCEDURE so_secondary_update_DUEDATE @sessionId int, @seqno int
AS	
UPDATE SALESORD_HDR 
SET SALESORD_HDR.DUEDATE = eoa.DUEDATE
FROM SALESORD_HDR 
JOIN EOA_SALESORD_SECONDARY eoa
	ON SALESORD_HDR.SEQNO=eoa.#
	WHERE eoa.SESSIONID = @sessionId
	and eoa.# = @seqno	
GO

CREATE PROCEDURE so_secondary_update_DUETIME @sessionId int, @seqno int
AS	
UPDATE SALESORD_HDR 
SET SALESORD_HDR.X_DUETIME = eoa.DUETIME
FROM SALESORD_HDR 
JOIN EOA_SALESORD_SECONDARY eoa
	ON SALESORD_HDR.SEQNO=eoa.#
	WHERE eoa.SESSIONID = @sessionId
	and eoa.# = @seqno	
GO

CREATE PROCEDURE so_secondary_update_PICKDATE @sessionId int, @seqno int
AS	
UPDATE SALESORD_HDR 
SET SALESORD_HDR.X_PICKDATE = eoa.PICKDATE
FROM SALESORD_HDR 
JOIN EOA_SALESORD_SECONDARY eoa
	ON SALESORD_HDR.SEQNO=eoa.#
	WHERE eoa.SESSIONID = @sessionId
	and eoa.# = @seqno	
GO