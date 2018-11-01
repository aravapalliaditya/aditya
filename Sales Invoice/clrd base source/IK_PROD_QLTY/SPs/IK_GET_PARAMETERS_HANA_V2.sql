
/*
Created By			: Vijeesh P.S
Created Date		: 03/08/2017
Tables Used			: 
Notes				: Parameter Selection
Execution Sample	: Call IK_GET_PARAMETERS ('5','10701-004000000023-000')
Drop Procedure		: Drop Procedure IK_GET_PARAMETERS
Update Reason		: 
Updated By			: 
Update Date			: 
*/

CREATE PROCEDURE IK_GET_PARAMETERS
(IN NoOfSamples nVarchar(10),
IN ItemCode nVarchar(50)
)
LANGUAGE SQLSCRIPT SQL SECURITY INVOKER 
As
Begin

Declare vLoop1 Integer = 1;

Create Column Table IK_TMP_PARAMETERS_V1
(
"SAMNO" Integer,
"ITEMC" nVarchar(50),
"QCCLEAR" nVarchar(5),
"PARAC" nVarchar(50),
"PARAN" nVarchar(100),
"PARAMTYP" nVarchar(50),
"UOM" nVarchar(15),
"SREADNGV" nVarchar(3),
"SREADNG" Decimal(19,2),
"TOLPLUS" Decimal(19,2),
"TOLMINUS" Decimal(19,2),
"INSTRMNT" nVarchar(50),
"ACTREAD" Decimal(19,2),
"ACTREADV" nVarchar(3)
);

While :vLoop1 <= :NoOfSamples Do

	Insert Into IK_TMP_PARAMETERS_V1
	(
	"SAMNO",
	"ITEMC",
	"QCCLEAR",
	"PARAC",
	"PARAN",
	"PARAMTYP",
	"UOM",
	"SREADNGV",
	"SREADNG",
	"TOLPLUS",
	"TOLMINUS",
	"INSTRMNT",
	"ACTREAD",
	"ACTREADV"
	)
	Select
	:vLoop1 "SAMNO"
	,:ItemCode "ITEMC"
	,'' "QCCLEAR"
	,T1."U_ParaC" "PARAC"
	,T1."U_ParaN" "PARAN"
	,T1."U_PrmTyp" "PARAMTYP"
	,T1."U_UoM" "UOM"
	,T1."U_SReadngV" "SREADNGV" 
	,T1."U_SReadng" "SREADNG" 
	,T1."U_TolPlus" "TOLPLUS"  
	,T1."U_TolMinus" "TOLMINUS"
	,T1."U_Instrmnt" "INSTRMNT"
	,0.00 "ACTREAD"
	,' ' "ACTREADV"
	From "@IK_QLTP" T0 Inner Join "@IK_LTP1" T1 On T0."Code" = T1."Code"
	Where T0."U_MacCode" = :ItemCode And IfNull(T1."U_IsADspt",'N') = 'N';
	
vLoop1 := vLoop1 + 1;

End While;

Select "SAMNO","ITEMC","PARAC","PARAN","PARAMTYP","UOM","SREADNGV","SREADNG","TOLPLUS","TOLMINUS","QCCLEAR","INSTRMNT","ACTREAD","ACTREADV"
From IK_TMP_PARAMETERS_V1;

Drop Table IK_TMP_PARAMETERS_V1;

End
