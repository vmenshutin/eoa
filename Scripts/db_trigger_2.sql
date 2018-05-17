create TRIGGER EOA_SCHEDULE ON SALESORD_HDR
AFTER UPDATE 
as begin
SET NOCOUNT ON;

if UPDATE (X_DISPATCHMETHOD)
begin

declare @inserted_order_seqno int;
declare @inserted_despatch_method varchar(50);
declare @inserted_status int;

select @inserted_order_seqno = i.SEQNO from inserted i;
select @inserted_despatch_method = i.X_DISPATCHMETHOD from inserted i;
select @inserted_status = i.STATUS from inserted i;

  ALTER TABLE SALESORD_HDR DISABLE TRIGGER status_is_scheduled

  UPDATE sales
	 set sales.X_DISPATCHSTATUS = 'Sc'
		,sales.X_PICKDATE = 
		(CASE
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Friday' then DateAdd(dd,3,CURRENT_TIMESTAMP)
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Friday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Friday' then DateAdd(dd,4,CURRENT_TIMESTAMP)
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,3,CURRENT_TIMESTAMP)
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Friday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Friday' then DateAdd(dd,3,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Friday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Sunday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('16:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Sunday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then CURRENT_TIMESTAMP
			
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Friday' then DateAdd(dd,3,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Friday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Sunday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Sunday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then CURRENT_TIMESTAMP
			
			
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Friday' then DateAdd(dd,3,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Friday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Saturday' then DateAdd(dd,2,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) = 'Sunday' then DateAdd(dd,1,CURRENT_TIMESTAMP)
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) and datename(weekday,CURRENT_TIMESTAMP) <> 'Sunday' and datename(weekday,CURRENT_TIMESTAMP) <> 'Saturday' then CURRENT_TIMESTAMP
			
			when X_DISPATCHMETHOD = 'P' and @inserted_status <> 2  then DUEDATE
			
			else X_PICKDATE
			end
		)
		FROM SALESORD_HDR sales
		where sales.SEQNO = @inserted_order_seqno
			and sales.X_DISPATCHMETHOD <> '(n)'
			and sales.X_DISPATCHMETHOD <> ''
			and sales.X_DISPATCHMETHOD IS NOT NULL
			and sales.STATUS <> 2
			
			
	UPDATE sales
	 set sales.X_DUETIME = 
		(CASE
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) < CAST('07:00:00' as time) then CAST('07:00:00' as time)
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) > CAST('16:00:00' as time)  then CAST('07:00:00' as time) 
			when X_DISPATCHMETHOD = 'E1' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('7:00:00' as time) and CAST(CURRENT_TIMESTAMP AS time) < CAST('16:00:00' as time) then DATEADD(MI, -DATEPART(MI, CURRENT_TIMESTAMP), DATEADD(HH, 2, CURRENT_TIMESTAMP))

			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and (CAST(CURRENT_TIMESTAMP AS time) < CAST('10:00:00' as time) or CAST(CURRENT_TIMESTAMP AS time) >= CAST('15:00:00' as time)) then CAST('11:00:00' as time)
			when X_DISPATCHMETHOD = 'E4' and @inserted_status <> 2 and CAST(CURRENT_TIMESTAMP AS time) >= CAST('10:00:00' as time) and CAST(CURRENT_TIMESTAMP AS time) < CAST('15:00:00' as time) then CAST('17:00:00' as time)
			
			when X_DISPATCHMETHOD = 'I' and @inserted_status <> 2 then DATEADD(HH, 16, DATEDIFF(dd, 0, X_PICKDATE))
			
			when X_DISPATCHMETHOD = 'P' and @inserted_status <> 2 then CAST('17:00:00' as time)
			--when X_DISPATCHMETHOD = 'G' and @inserted_status <> 2 then CAST('09:00:00' as time)	
			when X_DISPATCHMETHOD = 'N' and @inserted_status <> 2 then CAST('16:00:00' as time)					
			
			else X_DUETIME
			end
		)
		FROM SALESORD_HDR sales
		where sales.SEQNO = @inserted_order_seqno
			and sales.X_DISPATCHMETHOD <> '(n)'
			and sales.X_DISPATCHMETHOD <> ''
			and sales.X_DISPATCHMETHOD IS NOT NULL
			and sales.STATUS <> 2
			
	if (@inserted_despatch_method <> '' and @inserted_status <> 2 and @inserted_despatch_method is not null)
	 		 exec update_difot_timestamp @inserted_order_seqno
	end
	
	ALTER TABLE SALESORD_HDR ENABLE TRIGGER status_is_scheduled
	
end

go