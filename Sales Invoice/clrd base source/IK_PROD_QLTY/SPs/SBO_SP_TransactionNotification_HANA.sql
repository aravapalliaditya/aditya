CREATE PROCEDURE SBO_SP_TransactionNotification
(
	in object_type nvarchar(30), 				-- SBO Object Type
	in transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
	in num_of_cols_in_key int,
	in list_of_key_cols_tab_del nvarchar(255),
	in list_of_cols_val_tab_del nvarchar(255)
)
LANGUAGE SQLSCRIPT
AS

-- Drop Procedure SBO_SP_TransactionNotification
-- Return values
error  int;				-- Result (0 for no error)
error_message nvarchar (200); 		-- Error string to be displayed

vIsQualityNeeded nVarchar(5);
vGRPOType nVarchar(5);
vWhseSubType nVarchar(10);
vInvoiceType nVarchar(5);
vVisOrder Int;
vRQCClear nVarchar(5);
vInvBaseEntry nVarchar(10);
vInvBaseLine Int;

begin

error := 0;
error_message := N'Ok';
--------------------------------------------------------------------------------------------------------------------------------
-- VALIDATION FOR GRPO
--------------------------------------------------------------------------------------------------------------------------------
If :object_type='20' And (:transaction_type='A' or :transaction_type ='U') Then

Select T0."DocType" Into vGRPOType From OPDN T0 Where T0."DocEntry" = :list_of_cols_val_tab_del;

If vGRPOType = 'I' Then
	Select IfNull(T2."U_IsReqrd",'N'),IfNull(T3."U_TypeWH",''),T1."VisOrder"
	Into vIsQualityNeeded,vWhseSubType,vVisOrder
	From OPDN T0 Inner Join PDN1 T1 On T0."DocEntry" = T1."DocEntry" 
	Inner Join OITM T2 On T1."ItemCode" = T2."ItemCode"
	Inner Join OWHS T3 On T1."WhsCode" = T3."WhsCode"
	Where T0."DocEntry" = :list_of_cols_val_tab_del;

	If vWhseSubType <> 'QC' And vIsQualityNeeded = 'Y' Then
		error := -1001001;
		error_message := '[Row No. -> '|| Cast((vVisOrder+1)As Char)||'] The Selected ItemCode is Needed Quality Check, Please Select QC Warehouse';
	End If;
End If;

End If;
--------------------------------------------------------------------------------------------------------------------------------

-- Select the return values
select :error, :error_message FROM dummy;

end;