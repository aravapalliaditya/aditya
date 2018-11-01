
/*
Created By			: Vijeesh P.S
Created Date		: 27/09/2017
Tables Used			: 
Notes				: Sub Level BOM Selectio (10 Levels)
Execution Sample	: Call IK_MULTI_BOM ('HP6')
Drop Procedure		: Drop Procedure IK_MULTI_BOM;
Update Reason		: 
Updated By			: 
Update Date			: 
*/

Create Procedure IK_MULTI_BOM
(IN FatherItemCode nVarchar(50))
LANGUAGE SQLSCRIPT SQL SECURITY INVOKER 
As
Begin

Declare v_index Int = 0;

Create Local Temporary Table #BOM
(
"PARENT" nVarchar(50),
"CHILD" nVarchar(50),
"LEVEL" Integer
);

Insert Into #BOM
(
"PARENT",
"CHILD",
"LEVEL"
)
Select T0."Father",T0."Code",0 "LEVEL"
From ITT1 T0 Where T0."Father" = :FatherItemCode;

While :v_index <= 10 DO

Insert Into #BOM
(
"PARENT",
"CHILD",
"LEVEL"
)
Select T0."Father",T0."Code",(T1."LEVEL" + 1)
From ITT1 T0 Inner Join #BOM T1 On T0."Father" = T1."CHILD";

v_index = :v_index + 1;
End While;

Select Distinct "PARENT" From #BOM;

Drop Table #BOM;

End;