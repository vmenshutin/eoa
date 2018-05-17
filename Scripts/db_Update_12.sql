CREATE PROCEDURE get_sales_order_report @seqno int
as
begin
	select so.SEQNO
			,accs.NAME AS ACCS_NAME
			,accs.ADDRESS1 as ACCS_ADDRESS1
			,accs.ADDRESS2 as ACCS_ADDRESS2
			,accs.ADDRESS3 as ACCS_ADDRESS3
			,accs.ADDRESS4 as ACCS_ADDRESS4
			,accs.ADDRESS5 as ACCS_ADDRESS5
			,accs.POST_CODE
			,so.ADDRESS1
			,so.ADDRESS2
			,so.ADDRESS3
			,so.ADDRESS4
			,so.ADDRESS5
			,so.ADDRESS6
			,so.DUEDATE
			,so.REFERENCE
			,so.CUSTORDERNO
			,so.X_PROJECTNAME
			,nar.NARRATIVE
	from SALESORD_HDR so
	left join DR_ACCS accs
		on so.ACCNO = accs.ACCNO
	left join NARRATIVES nar
		on so.NARRATIVE_SEQNO = nar.SEQNO
	where so.SEQNO = @seqno
end

go