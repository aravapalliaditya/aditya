Declare @FDate DateTime
Declare @TDate DateTime

/* SELECT FROM [dbo].[OPDN] S0 WHERE */ SET  @FDate = /* S0.DocDate*/ '[%0]'
/* SELECT FROM [dbo].[OPDN] S1 WHERE */ SET  @TDate = /* S1.DocDate*/ '[%1]'

Select
T0."DocNum" "Final Inspection No"
,T0."U_Date" "Final Inspection Date"
,T0."U_ICode" "Final ItemCode"
,T0."U_IName" "Final ItemName"
,T0."U_AccQty" "Accepted Qty" 
,T0."U_RejQty" "Rejected Qty"
,T1."U_ParaC" "Parameter Code"
,T1."U_ParaN" "Parameter Name"
,T1."U_SmplNo" "Sample Size"
From "@IK_FLIS" T0
Inner Join "@IK_LIS1" T1 On T0."DocEntry" = T1."DocEntry"
Where
T0."U_Date" >= @FDate And T0."U_Date" <= @TDate