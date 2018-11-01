
CREATE PROCEDURE IK_GET_PARAMETERS_STEEL
(IN NoOfSamples nVarchar(10),
IN ItemCode nVarchar(50)
)
LANGUAGE SQLSCRIPT SQL SECURITY INVOKER 
As
Begin

	Select
	'1' "SAMNO"
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

End