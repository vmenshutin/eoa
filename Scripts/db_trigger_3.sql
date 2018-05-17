create TRIGGER EOA_CLEAR_SCHEDULING_DATA ON SALESORD_HDR
  AFTER UPDATE
  as begin
  SET NOCOUNT ON;
		
  if UPDATE (X_DISPATCHMETHOD)
  begin
	  UPDATE SALESORD_HDR
		 set X_DISPATCHSTATUS = 
			 (CASE	
				WHEN X_DISPATCHMETHOD = '(n)' then NULL
				else X_DISPATCHSTATUS
			  end
			  )
			  ,X_DISPATCHMETHOD = 
			 (CASE	
				WHEN X_DISPATCHMETHOD = '(n)'  then NULL
				else X_DISPATCHMETHOD
			  end
			  )
			  ,X_PICKDATE = 
			  (CASE	
				WHEN X_DISPATCHMETHOD = '(n)' then NULL
				else X_PICKDATE
			  end
			  )
			  ,X_DUETIME = 
			  (CASE	
				WHEN X_DISPATCHMETHOD = '(n)' then NULL
				else X_DUETIME
			  end
			  )			  
		end
	
	end
	
go				 