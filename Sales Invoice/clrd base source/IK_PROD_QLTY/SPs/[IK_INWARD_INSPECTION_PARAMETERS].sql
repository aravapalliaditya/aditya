USE [IreTex]
GO
/****** Object:  StoredProcedure [dbo].[IK_INWARD_INSPECTION_PARAMETERS]    Script Date: 7/13/2017 11:59:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
/*
Created By			: Vijeesh P.S
Created Date		: `13/07/2017
Tables Used			: 
Notes				: Parameter Selection
Execution Sample	: Exec IK_INWARD_INSPECTION_PARAMETERS '4','TRW00006','8','1','5','','0','2_CHN001_DIV001_PRL001_5_0_TRW00006'
Update Reason		: 
Updated By			: 
Update Date			: 
*/
-- =============================================

ALTER Proc [dbo].[IK_INWARD_INSPECTION_PARAMETERS] (@NoOfSamples As nVarchar(10),@ItemCode As nVarchar(50),@InwardNo As nVarchar(15),@InwardLine nVarchar(10),@GRPODocE As nVarchar(10),@GRPODocN As nVarchar(15),@GRPODocL As nVarchar(10),@MacID As nVarchar(Max))
As
Begin

-- =======================================================================================================================================
-- PARAMETERS FOR INWARD INSPECTION SCREEN                           
-- =======================================================================================================================================

If Exists( Select * from sysobjects where name = 'IK_TMP_PARAMETERS')
Begin    
Drop Table IK_TMP_PARAMETERS
End  

Create Table IK_TMP_PARAMETERS
(
Num Integer identity(1,1),
"INWDOCE" nVarchar(10),
"INWDOCN" nVarchar(15),
"INWDOCL" nVarchar(10),
"GRPODOCE" nVarchar(10),
"GRPODOCN" nVarchar(15),
"GRPODOCL" nVarchar(10),
"SAMNO" Integer,
"ITEMC" nVarchar(50),
"QCCLEAR" nVarchar(5),
"PARAC" nVarchar(50),
"PARAN" nVarchar(100),
"UOM" nVarchar(15),
"SREADNG" Decimal(19,2),
"TOLPLUS" Decimal(19,2),
"TOLMINUS" Decimal(19,2),
"INSTRMNT" nVarchar(50),
"ACTREAD" Decimal(19,2),
"REMARKS" nVarchar(100),
"MACID" nVarchar(100)
)

-- =======================================================================================================================================
-- LOOP BASED ON SAMPLE SIZE                           
-- =======================================================================================================================================
Declare @Loop1 Int
Set @Loop1 = 1

While @Loop1 <= @NoOfSamples
Begin

	Insert Into IK_TMP_PARAMETERS
	Select '            ' "INWDOCE"
	,@InwardNo "INWDOCN"
	,@InwardLine "INWDOCL"
	,@GRPODocE "GRPODOCE"
	,@GRPODocN "GRPODOCN" 
	,@GRPODocL "GRPODOCL"
	, @Loop1 "SAMNO"
	,@ItemCode "ITEMC"
	,'' "QCCLEAR"
	,T1."U_ParaC" "PARAC"
	,T1."U_ParaN" "PARAN"
	,T1."U_UoM" "UOM"
	,T1."U_SReadng" "SREADNG" 
	,T1."U_TolPlus" "TOLPLUS"  
	,T1."U_TolMinus" "TOLMINUS"
	,T1."U_Instrmnt" "INSTRMNT"
	,0.00 "ACTREAD"
	,'                                                                                                                                                                                                       ' "Remarks"
	,@MacID "MacID"  
	From "@IK_QLTP" T0 Inner Join "@IK_LTP1" T1 On T0."Code" = T1."Code"
	Where T0."U_MacCode" = @ItemCode And IsNull(T1."U_IsADspt",'N') = 'N'

Set @Loop1 = @Loop1 + 1

End

Select "INWDOCE","INWDOCN" "InwardInspection No","INWDOCL","GRPODOCE","GRPODOCN","GRPODOCL","SAMNO" "Sample Size","ITEMC" "Item Code","PARAC" "Parameter Code"
,"PARAN" "Parameter Name","UOM","SREADNG" "Std.Reading","TOLPLUS" "Tolerance (+)","TOLMINUS" "Tolerance (-)","QCCLEAR" "QC Clearance","INSTRMNT" "Instrument"
,"ACTREAD" "Actual Reading","REMARKS" "Remarks","MACID" "MacID"
From IK_TMP_PARAMETERS

End
