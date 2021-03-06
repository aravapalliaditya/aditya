USE [IreTex]
GO
/****** Object:  StoredProcedure [dbo].[SBO_SP_TransactionNotification]    Script Date: 7/13/2017 10:54:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[SBO_SP_TransactionNotification] 

@object_type nvarchar(30), 				-- SBO Object Type
@transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
@num_of_cols_in_key int,
@list_of_key_cols_tab_del nvarchar(255),
@list_of_cols_val_tab_del nvarchar(255)

AS

begin

-- Return values
declare @error  int				-- Result (0 for no error)
declare @error_message nvarchar (200) 		-- Error string to be displayed

Declare @IsQualityNeeded As nVarchar(5)
Declare @GRPOType As nVarchar(5)
Declare @WhseSubType As nVarchar(10)
Declare @InvoiceType As nVarchar(5)
Declare @VisOrder As Int
Declare @RQCClear As nVarchar(5)
Declare @InvBaseEntry As nVarchar(10)
Declare @InvBaseLine As Int

select @error = 0
select @error_message = N'Ok'

--------------------------------------------------------------------------------------------------------------------------------

--	ADD	YOUR	CODE	HERE


IF(@object_type ='59' AND @transaction_type IN ('A','U'))
BEGIN
IF EXISTS (SELECT * FROM OIGN T0
INNER JOIN IGN1 T1 ON T0.DocEntry=T1.DocEntry
WHERE T0.DocEntry =@list_of_cols_val_tab_del AND ISNULL(U_Shift,'')='' AND T1.BaseType='202')
BEGIN
Select @error = 1, @error_message = 'Shifts is mandatory in Receipt'
END
END
--------------------------------------------------------------------------------------------------------------------------------
-- VALIDATION FOR RESOURCE MASTER
--------------------------------------------------------------------------------------------------------------------------------
--If @object_type = '290' And (@transaction_type = 'A' Or @transaction_type = 'U')
--Begin
--Declare @LocationCode As nVarchar(50)
--Declare @PlantCode As nVarchar(50)
--Declare @DivsionCode As nVarchar(50)
--Declare @ProductLine As nVarchar(50)
--Declare @IsPartOfVM As nVarchar(4)
--Declare @ResourceSubType As nVarchar(10)
--Declare @IsVM As nVarchar(4)
--Declare @VM As nVarchar(50)
 
--Select @LocationCode = IsNull(T0."U_LocatnC",''),@PlantCode = IsNull(T0."U_PlantC",''),@DivsionCode = IsNull(T0."U_DivisnC",'')
--,@ProductLine = IsNull(T0."U_PrdLneC",''),@IsPartOfVM = IsNull(T0."U_IsPVMac",'N'),@IsVM = IsNull(T0."U_IsVMachine",'N')
--,@VM = IsNull(T0."U_VMachinC",'')
--From ORSC T0 Where T0."ResCode" = @list_of_cols_val_tab_del

--If @ProductLine = ''
--	Begin
--	Set @error = '-1001001';
--	Set @error_message = 'Product Line Should not be Blank';
--	End
--If @DivsionCode = ''
--Begin
--	Set @error = '-1001001';
--	Set @error_message = 'Division Should not be Blank';
--End
--If @PlantCode = ''
--Begin
--	Set @error = '-1001001';
--	Set @error_message = 'Plant Should not be Blank';
--End
--If @LocationCode = ''
--Begin
--	Set @error = '-1001001';
--	Set @error_message = 'Location Should not be Blank';
--End
--If @IsPartOfVM = ''
--Begin
--	If @VM = ''
--	Begin
--		Set @error = '-1001001';
--		Set @error_message = 'Virtual Machine Should not be Blank';
--	End
--End
--End
--------------------------------------------------------------------------------------------------------------------------------
-- VALIDATION FOR AP INVOICE
--------------------------------------------------------------------------------------------------------------------------------
If @object_type = '18' And (@transaction_type = 'A' Or @transaction_type = 'U')
Begin

Select @InvoiceType = T0.DocType  From OPCH T0 Where T0.DocEntry = @list_of_cols_val_tab_del

If @InvoiceType = 'I'
Begin
	Select @IsQualityNeeded = IsNull(T2.U_IsReqrd,'N'),@VisOrder = T1.VisOrder,@InvBaseEntry = IsNull(T1.BaseEntry,''),@InvBaseLine = T1.BaseLine
	From OPCH T0 Inner Join PCH1 T1 On T0.DocEntry = T1.DocEntry Inner Join OITM T2 On T1.ItemCode = T2.ItemCode
	Where T0.DocEntry = @list_of_cols_val_tab_del
	If @InvBaseEntry = '' And @IsQualityNeeded = 'Y'
	Begin
		Set @error = '-1001001'
		Set @error_message = N'[Row No. -> '+ convert(nvarchar(10),(@VisOrder+1))+'] The Selected ItemCode is Needed Quality Check, Cannot Post Direct A/P Invoice'
	End
	If @InvBaseEntry <> ''
	Begin
		Select @RQCClear = IsNull(T1.U_RCClear,'N') From OPDN T0 Inner Join PDN1 T1 On T0.DocEntry = T1.DocEntry Where T0.DocEntry = @InvBaseEntry And T1.LineNum = @InvBaseLine
		If @RQCClear = 'N'
		Begin
			Set @error = '-1001001'
			Set @error_message = N'[Row No. -> '+ convert(nvarchar(10),(@VisOrder+1))+'] The Selected ItemCode is Needed Quality Check, Cannot Post A/P Invoice'
		End
	End
End

End
--------------------------------------------------------------------------------------------------------------------------------
-- VALIDATION FOR GRPO
--------------------------------------------------------------------------------------------------------------------------------
If @object_type = '20' And (@transaction_type = 'A' Or @transaction_type = 'U')
Begin

Select @GRPOType = T0.DocType  From OPDN T0 Where T0.DocEntry = @list_of_cols_val_tab_del

If @GRPOType = 'I'
Begin
	Select @IsQualityNeeded = IsNull(T2.U_IsReqrd,'N'),@WhseSubType = IsNull(T3.U_TypeWH,''),@VisOrder = T1.VisOrder
	From OPDN T0 Inner Join PDN1 T1 On T0.DocEntry = T1.DocEntry 
	Inner Join OITM T2 On T1.ItemCode = T2.ItemCode
	Inner Join OWHS T3 On T1.WhsCode = T3.WhsCode
	Where T0.DocEntry = @list_of_cols_val_tab_del

	If @InvBaseEntry <> 'QC' And @IsQualityNeeded = 'Y'
	Begin
		Set @error = '-1001001'
		Set @error_message = N'[Row No. -> '+ convert(nvarchar(10),(@VisOrder+1))+'] The Selected ItemCode is Needed Quality Check, Please Select QC Warehouse'
	End
End

End
--------------------------------------------------------------------------------------------------------------------------------

-- Select the return values
select @error, @error_message

end