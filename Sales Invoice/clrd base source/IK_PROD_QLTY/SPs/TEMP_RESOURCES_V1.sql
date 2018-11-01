
create Table "TEMP_RESOURCES_V1"
(
"OprtCod" NVARCHAR(50),
"OprtNm" NVARCHAR(100),
"ResTyp"  NVARCHAR(50),
"ResCd"  NVARCHAR(50),
"ResNm" NVARCHAR(100),
"MouldN" NVARCHAR(100),
"MouldC"  NVARCHAR(50),
"PrdQty" Decimal(19,2),
"AccQty" Decimal(19,2),
"RejQty"Decimal(19,2),
"UOM" NVARCHAR(50),
"TSMins" INTEGER,
"BaseLine" NVARCHAR(10),
"LineId" NVARCHAR(10),
"ReasonC" NVARCHAR(50),
"Reason" NVARCHAR(100),
"FrmTm" NVARCHAR(100),
"ToTm"  NVARCHAR(50),
"TotTm"  NVARCHAR(50),
"SbRsn" NVARCHAR(100),
"FreeTxt" NCLOB,
"MACId"  NVARCHAR(100)
)