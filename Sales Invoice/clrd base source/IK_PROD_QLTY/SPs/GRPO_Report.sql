
Declare @FDate DateTime
Declare @TDate DateTime

/* SELECT FROM [dbo].[OPDN] S0 WHERE */ SET  @FDate = /* S0.DocDate*/ '[%0]'
/* SELECT FROM [dbo].[OPDN] S1 WHERE */ SET  @TDate = /* S1.DocDate*/ '[%1]'


Select
'Inward Inspection' "Type"
,T0."DocNum" "Inward/Release QC No"
,T3."DocNum" "GRPO No"
,T3."DocDate" "GRPO Date"
,T1."U_AccQty" "Accepted Qty"
,T1."U_RejQty" "Rejected Qty"
,T1."U_InspBy" "Inspected By"
,T1."U_ApprvBy" "Approved By"
From "@IK_IWIS" T0 Inner Join "@IK_WIS1" T1 On T0."DocEntry" = T1."DocEntry"
Inner Join PDN1 T2 On T1."U_BDcEntry" = T2."DocEntry" And T1."U_BDcLine" = T2."LineNum"
Inner Join OPDN T3 On T2."DocEntry" = T3."DocEntry"
Where
T3."DocDate" >= @FDate And T3."DocDate" <= @TDate

Union

Select
'Release QC' "Type"
,T0."DocNum" "Inward/Release QC No"
,T3."DocNum" "GRPO No"
,T3."DocDate" "GRPO Date"
,T1."U_AccQty" "Accepted Qty"
,T1."U_RejQty" "Rejected Qty"
,T1."U_InspBy" "Inspected By"
,T1."U_ApprvBy" "Approved By"
From "@IK_RLQC" T0 Inner Join "@IK_LQC1" T1 On T0."DocEntry" = T1."DocEntry"
Inner Join PDN1 T2 On T1."U_GRPOE" = T2."DocEntry" And T1."U_GRPOL" = T2."LineNum"
Inner Join OPDN T3 On T2."DocEntry" = T3."DocEntry"
Where
T3."DocDate" >= @FDate And T3."DocDate" <= @TDate