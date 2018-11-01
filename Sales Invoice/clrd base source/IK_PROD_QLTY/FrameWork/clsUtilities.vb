Imports System.Reflection
Imports System.IO
Imports System.Text

Public Class clsUtilities

    Dim v_RetVal, v_ErrCode As Integer
    Dim v_ErrMsg As String
    Dim DB_Restart As Boolean = False


#Region "   --      Start Up      --    "

    Sub StartUp()
        Try
            SetApplication()

            If Not SetConnectionContext() = 0 Then
                oApplication.MessageBox("Failed setting a connection to DI API")
                End
            End If

            SAPXML("Menu.xml")

            SystemUDFs()
            'Dim oMenuItem As SAPbouiCOM.MenuItem
            'Dim oMenus As SAPbouiCOM.Menus
            'oMenus = oApplication.Menus
            'oMenuItem = oApplication.Menus.Item("IK_QLT")
            'Dim path As String = Application.StartupPath & "\Quality.jpg"
            'oMenuItem.Image = path


            'IsNull/IfNull Condition for Query
            If oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014 Then
                _str_IsNull = "IsNull"
            ElseIf oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB Then
                _str_IsNull = "IfNull"
            End If

            'Logged In User
            _str_LogInUser = oCompany.UserName.ToString().Trim()

            'Table Creation
            Me.IK_Table()

            oApplication.StatusBar.SetText("Ikyam Add-On Connected Successfully...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

#Region "   -- Company Connection --    "

    Public Sub SetApplication()
        Try
            Dim oGUI As New SAPbouiCOM.SboGuiApi
            oGUI.AddonIdentifier = ""
            oGUI.Connect(Environment.GetCommandLineArgs.GetValue(1).ToString())
            oApplication = oGUI.GetApplication()
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        Finally
        End Try
    End Sub
    Public Function ConnectionContext() As Integer
        Try
            Dim strErrorCode As String
            If oCompany.Connected = True Then oCompany.Disconnect()

            oApplication.StatusBar.SetText("Addon is Connecting to the Company --> [" + oCompany.CompanyDB + "], Please Wait ..........", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            strErrorCode = oCompany.Connect
            ConnectionContext = strErrorCode
            Dim s As String
            s = oCompany.GetLastErrorDescription
            If strErrorCode = 0 Then
                oApplication.StatusBar.SetText("Connecion Established  !!! ", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                System.Media.SystemSounds.Asterisk.Play()
                'AddLogo()
                Return 0
            End If
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        Finally
        End Try
    End Function
    Public Function CookieConnect() As Integer
        Try
            Dim strCkie, strContext As String
            oCompany = New SAPbobsCOM.Company
            Debug.Print(oCompany.CompanyDB)
            strCkie = oCompany.GetContextCookie()
            strContext = oApplication.Company.GetConnectionContext(strCkie)
            CookieConnect = oCompany.SetSboLoginContext(strContext)
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        Finally
        End Try
    End Function
    Private Function SetConnectionContext() As Integer
        oCompany = oApplication.Company.GetDICompany
    End Function

#End Region

#Region "   -- UDF & UDT Creation --    "

    Sub IK_Table()

        UDF_Configuration_Table()

        If Check_UDF_Configuration_Table_For_FieldCreation() = True Then
            Exit Sub
        End If

        'User Configuration Table for BLVL
        'IK_UserConfiguration()
        IK_SalesInvoice()

        IK_SALMSTR()
        'Creating Formatted Search Query Category,Creating Query and Assigning Query
        CreateCategory()
        QueryWrite()
        AssignFS()

        'Creating Additional Tables For Customization
        'ExecuteAdditionalScripts()

        Update_UDF_Configuration_Table()

    End Sub


#Region "--Sales Invoice--"
    Sub IK_SalesInvoice()
        Me.AddTable("IKOINV", " IKOINV Sales Invoice PARENT ", SAPbobsCOM.BoUTBTableType.bott_Document)
        Me.AddTable("IKINV1", " IK_INV1 Sales Invoice CHILD ", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

        Me.AddColumns("@IKOINV", "CardCode", "Customer", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Me.AddColumns("@IKOINV", "CardName", "Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 30)
        Dim ValidValues(,) As String = {{"", ""}}
        Dim DefaultVal = New String(,) {{"", ""}}
        Me.AddColumns("@IKOINV", "CntctCod", "Contact Person", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", Nothing, ValidValues, 0, DefaultVal)
        Me.AddColumns("@IKOINV", "NumAtCar", "Ref No.", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Dim ValidValues1(,) As String = {{"Local Currency ", "Local Currency"}, {"System Currency", "System Currency"}, {"BP Currency", "BP Currency"}}
        Dim DefaultVal1 = New String(,) {{"Local Currency ", "Local Currency"}}
        Me.AddColumns("@IKOINV", "CurSou", "Currency Source", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", Nothing, ValidValues1, 0, DefaultVal1)
        Me.AddColumns("@IKOINV", "DocCur", "Document Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Dim ValidValues2(,) As String = {{"", ""}}
        Dim DefaultVal2 = New String(,) {{"", ""}}
        Me.AddColumns("@IKOINV", "GSTTraTy", "Transcation Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", Nothing, ValidValues2, 0, DefaultVal2)
        Me.AddColumns("@IKOINV", "PlaOfSup", "Place Of Supply", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Dim ValidValues3(,) As String = {{"", ""}}
        Dim DefaultVal3 = New String(,) {{"", ""}}
        Me.AddColumns("@IKOINV", "Series", "Series", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", Nothing, ValidValues3, 0, DefaultVal3)
        Me.AddColumns("@IKOINV", "DocNum", "Doc Number", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Me.AddColumns("@IKOINV", "Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKOINV", "DocDate", "Posting Date", SAPbobsCOM.BoFieldTypes.db_Date)
        Me.AddColumns("@IKOINV", "DocDueDa", "Due Date", SAPbobsCOM.BoFieldTypes.db_Date)
        Me.AddColumns("@IKOINV", "TaxDate", "Document Date", SAPbobsCOM.BoFieldTypes.db_Date)
        Dim ValidValues4(,) As String = {{"", ""}}
        Dim DefaultVal4 = New String(,) {{"", ""}}
        Me.AddColumns("@IKOINV", "SipCode", "Sales Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", Nothing, ValidValues4, 0, DefaultVal4)
        Me.AddColumns("@IKOINV", "OwnerCo", "Owner", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKOINV", "Order", "Payment Order Run", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKOINV", "Comment", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, 100)
        Me.AddColumns("@IKOINV", "TotBeDis", "Total Before Discount", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "DisPre", "Discount", SAPbobsCOM.BoFieldTypes.db_Numeric, 3)
        Me.AddColumns("@IKOINV", "DisAmo", "Discount Amount", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "TotDPay", "Total Down Payment", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "Freig", "Freight", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "Round", "Rounding", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "Tax", "Tax", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "WTax", "WTax Amount", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "Total", "Total", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "AplAmo", "Applied Amount", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKOINV", "BalDue", "Balance Due", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)

        Me.AddColumns("@IKINV1", "ItemCode", "Item No.", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "SubCatNu", "BP Catalogue No.", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "Dscrip", "Item Dscription", SAPbobsCOM.BoFieldTypes.db_Alpha, 60)
        Me.AddColumns("@IKINV1", "Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Numeric, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKINV1", "UnitPri", "Unit Price", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Quantity)
        Me.AddColumns("@IKINV1", "DisPrcen", "Discount %", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Quantity)
        Me.AddColumns("@IKINV1", "TaxCode", "VAT Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Me.AddColumns("@IKINV1", "WtLiable", "WTAX Liable", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "TotalLC", "Total (LC)", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Price)
        Me.AddColumns("@IKINV1", "WhsCode", "Whse", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "UomCode", "UoM Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "Text", "Item Details", SAPbobsCOM.BoFieldTypes.db_Alpha, 30)
        Me.AddColumns("@IKINV1", "LocCode", "Loc", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "BlAgrNo", "Blanket Agreement No.", SAPbobsCOM.BoFieldTypes.db_Alpha, 50)
        Me.AddColumns("@IKINV1", "IKQRTRL", "Qty in Rolls", SAPbobsCOM.BoFieldTypes.db_Numeric, 10)
        'Me.AddColumns("@IKINV1", "", "", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "PckDet", "Packing Details", SAPbobsCOM.BoFieldTypes.db_Alpha, 50)
        'Me.AddColumns("@IKINV1", "", "", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "CogsOcr", "COGS Machine", SAPbobsCOM.BoFieldTypes.db_Alpha, 20)
        Me.AddColumns("@IKINV1", "TaxOnly", "Tax Only", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "HSN", "HSN", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)
        Me.AddColumns("@IKINV1", "SAC", "SAC", SAPbobsCOM.BoFieldTypes.db_Alpha, 10)

        If Not Me.UDOExists("IKOINVOBJ") Then
            Dim findAliasNDescription = New String(,) {{"DocEntry", "DocEntry"}}
            Me.registerUDO("IKOINVOBJ", "Sales Invoice OBJECT", SAPbobsCOM.BoUDOObjType.boud_Document, findAliasNDescription, "IKOINV", "IKINV1", "", "", SAPbobsCOM.BoYesNoEnum.tNO, SAPbobsCOM.BoYesNoEnum.tNO)
            findAliasNDescription = Nothing

        End If

    End Sub


    Sub IK_SALMSTR()
        Me.AddTable("IK_SALMSTR", "IK-> sales master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
        'Me.AddColumns("@IK_PLNT", "Location", "Location", SAPbobsCOM.BoFieldTypes.db_Alpha, 50)
        If Not Me.UDOExists("IK_SALMST") Then
            Dim findAliasNDescription = New String(,) {{"Code", "Code"}, {"Name", "Name"}}
            Me.registerUDO("IK_SALMST", "IK-> sales master obj", SAPbobsCOM.BoUDOObjType.boud_MasterData, findAliasNDescription, "IK_SALMSTR", "", "", "", SAPbobsCOM.BoYesNoEnum.tNO, SAPbobsCOM.BoYesNoEnum.tYES, SAPbobsCOM.BoYesNoEnum.tNO)
            findAliasNDescription = Nothing
        End If
    End Sub
#End Region



#End Region

#Region "   -- UDF Configur. Table --   "

    Sub UDF_Configuration_Table()
        Me.AddTable("IK_UDF_CONFIG", "UDF CONFIGURATION TABLE", SAPbobsCOM.BoUTBTableType.bott_NoObject)
    End Sub
    Sub Update_UDF_Configuration_Table()
        Dim strQuery As String = ""
        strQuery = "Insert into ""@IK_UDF_CONFIG"" (""Code"",""Name"") Values('QUALITY','QUALITY')"
        Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery(strQuery)
    End Sub
    Function Check_UDF_Configuration_Table_For_FieldCreation() As Boolean
        Dim strQuery As String = ""
        strQuery = "Select Count(*) ""Code"" From ""@IK_UDF_CONFIG"""
        Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery(strQuery)
        If oRS.RecordCount > 0 Then
            If CDbl(oRS.Fields.Item("Code").Value) > 0 Then
                Return True
            End If
        End If
        Return False
    End Function

#End Region

#Region "   -- Creating System UDFs --    "

    Sub SystemUDFs()
        '-----------------------------OWOR And Wor1-------------------------------------------'
        Me.AddColumns("WOR1", "SetupHrs", "Setuphours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "LabourHrs", "labour hours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "TotalHrs", "totalhours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "ProcesCstHr", "processcst hrs", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "ProcessCst", "processCst", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "Opertor", "Opertor", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "StartTime", "StartTime", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "EndTime", "EndTime", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "ATotalHrs", "ATotalHrs", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "DimenRpt", "DimenRpt", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("WOR1", "Finished", "Finished", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("OWOR", "BatchNo", "batchnum", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("OWOR", "Spllns", "batchnum", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)


        '----------------------------------OITT And ITT1------------------------------------'



        Me.AddColumns("ITT1", "SetupHrs", "Setuphours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "LabourHrs", "labour hours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "TotalHrs", "totalhours", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "ProcesCstHr", "processcst hrs", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "ProcessCst", "processCst", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "Opertor", "Opertor", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "StartTime", "StartTime", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "EndTime", "EndTime", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "ATotalHrs", "ATotalHrs", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "DimenRpt", "DimenRpt", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("ITT1", "Finished", "Finished", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("OITT", "BatchNo", "batchnum", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
        Me.AddColumns("OITT", "Spllns", "batchnum", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None)
    End Sub

#End Region

#Region "   -- Creating Userdefined Table --    "

#Region "   -- User Configuration --    "

    'Sub IK_UserConfiguration()
    '    AddTable("IK_UCFG", "IK-> User Configuration", SAPbobsCOM.BoUTBTableType.bott_MasterData)


    '    If Not UDOExists("IK_UCFG") Then
    '        Dim findAliasNDescription = New String(,) {{"Code", "Code"}, {"Name", "Name"}}
    '        registerUDO("IK_UCFG", "User Configuration", SAPbobsCOM.BoUDOObjType.boud_MasterData, findAliasNDescription, "IK_UCFG", "IK_CFG1", "", "", SAPbobsCOM.BoYesNoEnum.tNO, SAPbobsCOM.BoYesNoEnum.tNO)
    '        findAliasNDescription = Nothing
    '    End If
    'End Sub

#End Region





#Region "   -- Common Tables --    "



#End Region

#End Region

#Region "   -- Additional Scripts --   "


    Sub ExecuteAdditionalScripts()
        Try
            Try
                Dim File As New FileInfo((Application.StartupPath).ToString() & "\Scripts\CreateScript_DispatchParametersTable.sql")
                Dim script As String = File.OpenText().ReadToEnd()
                Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oRS.DoQuery(script)
            Catch ex As Exception
            End Try
            Try
                Dim File As New FileInfo((Application.StartupPath).ToString() & "\Scripts\CreateScript_ParametersTable.sql")
                Dim script As String = File.OpenText().ReadToEnd()
                Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oRS.DoQuery(script)
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "   -- DataBase Creation --     "
    Function TableExists(ByVal TableName As String) As Boolean
        Dim oTables As SAPbobsCOM.UserTablesMD
        Dim oFlag As Boolean
        oTables = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
        oFlag = oTables.GetByKey(TableName)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(oTables)
        Return oFlag
    End Function
    Function ColumnExists(ByVal TableName As String, ByVal FieldID As String) As Boolean
        Try
            Dim rs As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim oFlag As Boolean = True
            rs.DoQuery("Select 1 from ""CUFD"" Where ""TableID""='" & Trim(TableName) & "' and ""AliasID""='" & Trim(FieldID) & "'")
            If rs.EoF Then oFlag = False
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rs)
            rs = Nothing
            GC.Collect()
            Return oFlag
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Public Function UDOExists(ByVal code As String) As Boolean
        GC.Collect()
        Dim v_UDOMD As SAPbobsCOM.UserObjectsMD
        Dim v_ReturnCode As Boolean
        v_UDOMD = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
        v_ReturnCode = v_UDOMD.GetByKey(code)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UDOMD)
        v_UDOMD = Nothing
        Return v_ReturnCode
    End Function
    Function AddTable(ByVal TableName As String, ByVal TableDescription As String, ByVal TableType As SAPbobsCOM.BoUTBTableType) As Boolean
        Try
            GC.Collect()
            If Not Me.TableExists(TableName) Then
                Dim v_UserTableMD As SAPbobsCOM.UserTablesMD
                oApplication.StatusBar.SetText("Creating Table " & TableName & " ...................", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                v_UserTableMD = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
                v_UserTableMD.TableName = TableName
                v_UserTableMD.TableDescription = TableDescription
                v_UserTableMD.TableType = TableType
                v_RetVal = v_UserTableMD.Add()
                If v_RetVal <> 0 Then
                    oCompany.GetLastError(v_ErrCode, v_ErrMsg)
                    oApplication.StatusBar.SetText("Failed to Create Table " & TableName & v_ErrCode & " " & v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD)
                    v_UserTableMD = Nothing
                    GC.Collect()
                    Return False
                Else
                    oApplication.StatusBar.SetText("[@" & TableName & "] - " & TableDescription & " created successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD)
                    v_UserTableMD = Nothing
                    GC.Collect()
                    DB_Restart = True
                    Return True
                End If
            Else
                GC.Collect()
                Return False
            End If
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function AddColumns(ByVal TableName As String, ByVal Name As String, ByVal Description As String, ByVal Type As SAPbobsCOM.BoFieldTypes, Optional ByVal Size As Long = 0, Optional ByVal SubType As SAPbobsCOM.BoFldSubTypes = SAPbobsCOM.BoFldSubTypes.st_None, Optional ByVal LinkedTable As String = "", Optional ByVal LinkedUDO As String = "", Optional ByVal LinkedSysObjects As String = "", Optional ByVal Token As Hashtable = Nothing, Optional ByVal ValidValues As String(,) = Nothing, Optional ByVal iCount As Integer = 0, Optional ByVal DefaultValues As String(,) = Nothing) As Boolean
        Try
            If Not Me.ColumnExists(TableName, Name) Then
                Dim v_UserField As SAPbobsCOM.UserFieldsMD
                v_UserField = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)
                v_UserField.TableName = TableName
                v_UserField.Name = Name
                v_UserField.Description = Description
                v_UserField.Type = Type
                If Type <> SAPbobsCOM.BoFieldTypes.db_Date Then
                    If Size <> 0 Then
                        If Type = SAPbobsCOM.BoFieldTypes.db_Numeric Then
                            v_UserField.EditSize = Size
                        Else
                            v_UserField.Size = Size
                        End If
                    End If
                End If
                If SubType <> SAPbobsCOM.BoFldSubTypes.st_None Then
                    v_UserField.SubType = SubType
                End If
                If LinkedTable <> "" Then v_UserField.LinkedTable = LinkedTable

                If LinkedUDO <> "" Then v_UserField.LinkedUDO = LinkedUDO

                If Not (ValidValues Is Nothing) Then
                    If ValidValues.GetLength(0) > 0 Then
                        For i As Integer = 0 To ValidValues.GetLength(0) - 1
                            v_UserField.ValidValues.SetCurrentLine(i)
                            v_UserField.ValidValues.Value = ValidValues(i, 0)
                            v_UserField.ValidValues.Description = ValidValues(i, 1)
                            v_UserField.ValidValues.Add()
                        Next
                        If Not (DefaultValues) Is Nothing Then
                            If DefaultValues.Length > 0 Then
                                v_UserField.DefaultValue = DefaultValues(0, 0)
                            Else
                                v_UserField.DefaultValue = ValidValues(1, 0)
                            End If
                        End If
                    End If
                End If
                v_RetVal = v_UserField.Add()
                If v_RetVal <> 0 Then
                    oCompany.GetLastError(v_ErrCode, v_ErrMsg)
                    oApplication.StatusBar.SetText("Failed to add UserField " & Description & " - " & v_ErrCode & " " & v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField)
                    v_UserField = Nothing
                    Return False
                Else
                    oApplication.StatusBar.SetText("[@" & TableName & "] - " & Description & " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField)
                    v_UserField = Nothing
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function registerUDO(ByVal UDOCode As String, ByVal UDOName As String, ByVal UDOType As SAPbobsCOM.BoUDOObjType, ByVal findAliasNDescription As String(,), ByVal parentTableName As String, Optional ByVal childTable1 As String = "", Optional ByVal childTable2 As String = "", Optional ByVal childTable3 As String = "", Optional ByVal LogOption As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO, Optional ByVal DefaultForm As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO, Optional ByVal MenuForm As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO, Optional ByVal caption As String = "", Optional ByVal fatherMenuUID As String = "", Optional ByVal position As String = "", Optional ByVal menuUID As String = "", Optional ByVal childTable4 As String = "", Optional ByVal childTable5 As String = "", Optional ByVal childTable6 As String = "", Optional ByVal childTable7 As String = "", Optional ByVal childTable8 As String = "", Optional ByVal childTable9 As String = "", Optional ByVal childTable10 As String = "", Optional ByVal childTable11 As String = "") As Boolean
        Dim actionSuccess As Boolean = False
        Try
            registerUDO = False
            Dim v_udoMD As SAPbobsCOM.UserObjectsMD
            v_udoMD = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            v_udoMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO
            v_udoMD.CanClose = SAPbobsCOM.BoYesNoEnum.tNO
            v_udoMD.CanCreateDefaultForm = DefaultForm
            v_udoMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES
            v_udoMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES
            v_udoMD.CanLog = LogOption
            v_udoMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES
            v_udoMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES
            v_udoMD.Code = UDOCode
            v_udoMD.Name = UDOName
            v_udoMD.TableName = parentTableName

            If DefaultForm = SAPbobsCOM.BoYesNoEnum.tYES & MenuForm = SAPbobsCOM.BoYesNoEnum.tYES Then
                v_udoMD.MenuItem = SAPbobsCOM.BoYesNoEnum.tYES
                v_udoMD.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO
                v_udoMD.MenuCaption = caption
                v_udoMD.FatherMenuID = fatherMenuUID
                v_udoMD.Position = position
                v_udoMD.MenuUID = menuUID
            End If

            If DefaultForm = SAPbobsCOM.BoYesNoEnum.tYES & MenuForm = SAPbobsCOM.BoYesNoEnum.tNO Then
                v_udoMD.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO
            End If

            If LogOption = SAPbobsCOM.BoYesNoEnum.tYES Then
                v_udoMD.LogTableName = "A" & parentTableName
            End If
            v_udoMD.ObjectType = UDOType
            For i As Int16 = 0 To findAliasNDescription.GetLength(0) - 1
                If i > 0 Then
                    v_udoMD.FindColumns.Add()
                    v_udoMD.FormColumns.Add()
                End If

                v_udoMD.FindColumns.ColumnAlias = findAliasNDescription(i, 0)
                v_udoMD.FindColumns.ColumnDescription = findAliasNDescription(i, 1)

                v_udoMD.FormColumns.FormColumnAlias = findAliasNDescription(i, 0)
                v_udoMD.FormColumns.FormColumnDescription = findAliasNDescription(i, 1)
            Next
            If childTable1 <> "" Then
                v_udoMD.ChildTables.TableName = childTable1
                v_udoMD.ChildTables.Add()
            End If
            If childTable2 <> "" Then
                v_udoMD.ChildTables.TableName = childTable2
                v_udoMD.ChildTables.Add()
            End If
            If childTable3 <> "" Then
                v_udoMD.ChildTables.TableName = childTable3
                v_udoMD.ChildTables.Add()
            End If
            If childTable4 <> "" Then
                v_udoMD.ChildTables.TableName = childTable4
                v_udoMD.ChildTables.Add()
            End If
            If childTable5 <> "" Then
                v_udoMD.ChildTables.TableName = childTable5
                v_udoMD.ChildTables.Add()
            End If
            If childTable6 <> "" Then
                v_udoMD.ChildTables.TableName = childTable6
                v_udoMD.ChildTables.Add()
            End If
            If childTable7 <> "" Then
                v_udoMD.ChildTables.TableName = childTable7
                v_udoMD.ChildTables.Add()
            End If
            If childTable8 <> "" Then
                v_udoMD.ChildTables.TableName = childTable8
                v_udoMD.ChildTables.Add()
            End If
            If childTable9 <> "" Then
                v_udoMD.ChildTables.TableName = childTable9
                v_udoMD.ChildTables.Add()
            End If
            If childTable10 <> "" Then
                v_udoMD.ChildTables.TableName = childTable10
                v_udoMD.ChildTables.Add()
            End If
            If childTable11 <> "" Then
                v_udoMD.ChildTables.TableName = childTable11
                v_udoMD.ChildTables.Add()
            End If

            If v_udoMD.Add() = 0 Then
                DB_Restart = True
                registerUDO = True
                oApplication.StatusBar.SetText("Successfully Registered UDO >" & UDOCode & ">" & UDOName & " >" & oCompany.GetLastErrorDescription, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Else
                oApplication.StatusBar.SetText("Failed to Register UDO >" & UDOCode & ">" & UDOName & " >" & oCompany.GetLastErrorDescription, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                registerUDO = False
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_udoMD)
            v_udoMD = Nothing
            GC.Collect()
        Catch ex As Exception
        End Try
    End Function
#End Region

#Region "   -- Functions --     "

    Public Function GetNextDocNumOD(ByVal FieldName As String, ByVal TableName As String) As Integer
        Try
            Dim GetDocNum As String = "Select " + _str_IsNull + "(Max(Cast(""" + FieldName + """ As Integer )),0) + 1 From """ & TableName & """"
            Dim oRsGetDocNum As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRsGetDocNum.DoQuery(GetDocNum)

            Return oRsGetDocNum.Fields.Item(0).Value
        Catch ex As Exception
            oCompany.objApplication.StatusBar.SetText("DN: " + ex.Message)
        End Try
    End Function
    Function AddQuery(ByVal QCategory As String, ByVal QName As String, ByVal Query As String) As Boolean
        Try
            Dim _rs As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim UQ As SAPbobsCOM.UserQueries = ((oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserQueries)))
            _rs.DoQuery("select ""CategoryId"" from OQCN where ""CatName"" ='" + QCategory + "'")
            UQ.QueryCategory = Convert.ToInt32(_rs.Fields.Item(0).Value.ToString())
            UQ.QueryDescription = QName
            UQ.Query = Query
            UQ.Add()
            Return True
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function AddQCategory(ByVal QCategory As String) As Boolean
        Try
            Dim QC As SAPbobsCOM.QueryCategories = (oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQueryCategories))
            QC.Name = QCategory
            QC.Permissions = "YYYYYYYYYYYYYYY"
            QC.Add()
            Return True
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function SetFormattedSearch(ByVal FormType As String, ByVal ItemUID As String, ByVal ColUID As String, ByVal QCategory As String, ByVal QueryName As String, ByVal Refresh As Boolean, ByVal FieldID As String, ByVal FrcRefresh As Boolean, ByVal ByField As Boolean) As Boolean
        Try
            Dim FS As SAPbobsCOM.FormattedSearches = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oFormattedSearches)
            Dim _rs As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            Dim _bool As Boolean = False
            If (ColUID = "") Then
                ColUID = "-1"
            End If
            Dim _str_Select As String
            _str_Select = "Select ""IndexID"" From CSHS where ""FormID"" ='" + FormType + "' and ""ItemID"" ='" + ItemUID + "' and ""ColID"" ='" + ColUID + "'"
            _rs.DoQuery("Select ""IndexID"" From CSHS where ""FormID"" ='" + FormType + "' and ""ItemID"" ='" + ItemUID + "' and ""ColID"" ='" + ColUID + "'")
            If (Convert.ToInt32(_rs.Fields.Item(0).Value.ToString()) > 0) Then

                _bool = True
                FS.GetByKey(Convert.ToInt32(_rs.Fields.Item(0).Value.ToString()))
            End If
            _rs.DoQuery("Select ""IntrnalKey"" From OUQR INNER JOIN OQCN ON OUQR.""QCategory"" = OQCN.""CategoryId"" Where ""QName"" ='" + QueryName + "' And ""CatName"" ='" + QCategory + "'")
            FS.Action = SAPbobsCOM.BoFormattedSearchActionEnum.bofsaQuery
            FS.FormID = FormType
            FS.ItemID = ItemUID
            If (ColUID = "") Then
                ColUID = "-1"
            End If
            FS.ColumnID = ColUID
            FS.QueryID = Convert.ToInt32(_rs.Fields.Item(0).Value.ToString())

            If (Refresh) Then
                FS.Refresh = SAPbobsCOM.BoYesNoEnum.tYES
            Else
                FS.Refresh = SAPbobsCOM.BoYesNoEnum.tNO
            End If
            FS.FieldID = FieldID

            If (FrcRefresh) Then
                FS.ForceRefresh = SAPbobsCOM.BoYesNoEnum.tYES
            Else
                FS.ForceRefresh = SAPbobsCOM.BoYesNoEnum.tNO

                If (ByField) Then
                    FS.ByField = SAPbobsCOM.BoYesNoEnum.tYES
                Else
                    FS.ByField = SAPbobsCOM.BoYesNoEnum.tNO
                End If
            End If
            Dim lRetCode As Integer
            If (_bool) Then
                lRetCode = FS.Update()
            Else
                lRetCode = FS.Add()
            End If
            Return True
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function getNextSeriesVal(ByVal udoID As String) As Integer
        Try
            Dim seriesService As SAPbobsCOM.SeriesService
            Dim v_CompanyService As SAPbobsCOM.CompanyService
            Dim objectType As SAPbobsCOM.DocumentTypeParams
            Dim crmSeries As SAPbobsCOM.Series
            v_CompanyService = oCompany.GetCompanyService
            seriesService = v_CompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.SeriesService)
            objectType = seriesService.GetDataInterface(SAPbobsCOM.SeriesServiceDataInterfaces.ssdiDocumentTypeParams)
            objectType.Document = udoID
            crmSeries = seriesService.GetDefaultSeries(objectType)
            Return crmSeries.NextNumber
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Function
    Function keygen(ByVal vtablename As String, Optional ByVal prefix As String = "DOC-") As String()

        Dim str(3) As String
        Dim Query As String
        Try
            'Query = "SELECT MAX(CAST(""Code"" AS int)) AS ""code"" FROM [" + vtablename + "]"
            Query = "Select IfNull(Max(Cast(""Code"" As Integer)),0) ""Code"" From """ + vtablename + """"
            Dim v_recordset As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            v_recordset.DoQuery(Query)
            v_recordset.MoveFirst()
            Dim code As Integer = v_recordset.Fields.Item("Code").Value.ToString
            If code > 0 Then
                code += 1
                Dim docid As String = prefix
                If code.ToString.Length < 7 Then
                    For count As Integer = 0 To 6 - code.ToString.Length
                        docid += "0"
                    Next
                End If
                docid += code.ToString
                str(0) = code
                str(1) = docid
                str(2) = docid
            Else
                str(0) = "1"
                str(1) = prefix + "0000001"
                str(2) = prefix + "0000001"
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_recordset)
            v_recordset = Nothing
            GC.Collect()
        Catch ex As Exception
            oApplication.MessageBox(ex.Message)
        End Try
        keygen = str
    End Function
    Function CreateCategory() As Boolean
        AddQCategory("FS(Formatted Search)")
    End Function
    Function QueryWrite() As Boolean
        Try
            AddQuery("FS(Formatted Search)", "IK_FMS_LOC", "Select ""Code"",""Location"" From OLCT")
            AddQuery("FS(Formatted Search)", "IK_FMS_LOCATION", "Select ""Location"" From OLCT")
            AddQuery("FS(Formatted Search)", "IK_FMS_RESOURCE_LOC", "Select ""Code"" From OLCT Where ""Location"" = $[ORSC.U_Location]")
            AddQuery("FS(Formatted Search)", "IK_FMS_PLNT", "Select ""Code"",""Name"" From ""@IK_PLNT""")
            AddQuery("FS(Formatted Search)", "IK_FMS_LOC_PLNT", "SELECT T1.""Code"",T1.""Location"" FROM ""@IK_PLNT"" T0 INNER JOIN OLCT T1 ON T0.""U_Location"" = T1.""Code"" WHERE T0.""Code"" = $[$3.U_Plant.0]")
            AddQuery("FS(Formatted Search)", "IK_FMS_DVSN", "SELECT ""Code"",""Name"" FROM ""@IK_DVSN""")
            AddQuery("FS(Formatted Search)", "IK_FMS_PLNT_DVSN", "SELECT ""U_Plant"" FROM ""@IK_DVSN"" Where ""Code"" = $[$3.U_Division.0]")
            AddQuery("FS(Formatted Search)", "AlternativeBOM", "SELECT T0.""Code"" ""Alternative BOM Code"", T0.""U_ABomC"" ""BOM Item Code"", T0.""U_ABomD"" ""BOM Item Name"" FROM ""@IK_ABOM"" T0 WHERE T0.""U_LocatnC"" = $[OWOR.U_LocatnC] And T0.""U_PlantC"" = $[OWOR.U_PlantC] And  T0.""U_DivisnC"" = $[OWOR.U_DivisnC] And  T0.""U_PrdLneC"" = $[OWOR.U_PrdLneC]")
            AddQuery("FS(Formatted Search)", "AlternativeUoM", "SELECT T0.""UomCode"", T0.""UomName"" FROM OUOM T0")
        Catch ex As Exception
        End Try
    End Function
    Function AssignFS() As Boolean
        Try
            SetFormattedSearch("IK_PLNT", "3", "U_Location", "FS(Formatted Search)", "IK_FMS_LOC", False, "", False, False)
            SetFormattedSearch("IK_DVSN", "3", "U_Plant", "FS(Formatted Search)", "IK_FMS_PLNT", False, "", False, False)
            SetFormattedSearch("IK_DVSN", "3", "U_Location", "FS(Formatted Search)", "IK_FMS_LOC_PLNT", True, "U_Plant", True, False)
            SetFormattedSearch("IK_PRDL", "3", "U_Division", "FS(Formatted Search)", "IK_FMS_DVSN", False, "", False, False)
            SetFormattedSearch("IK_PRDL", "3", "U_Plant", "FS(Formatted Search)", "IK_FMS_PLNT_DVSN", True, "U_Division", True, False)
            SetFormattedSearch("IK_PRDL", "3", "U_Location", "FS(Formatted Search)", "IK_FMS_LOC_DVSN", True, "U_Division", True, False)
            SetFormattedSearch("65211", "U_AltBOM", "", "FS(Formatted Search)", "AlternativeBOM", False, "", False, False)
            SetFormattedSearch("65211", "U_AltUoM", "", "FS(Formatted Search)", "AlternativeUoM", False, "", False, False)
            'SetFormattedSearch("Form", "GRIDPL", "VNDRName", "FS(Formatted Search)", "VNDRName", True, "VNDRCode", True, False)
        Catch ex As Exception
        End Try
    End Function
    Shared Function IsSuperUser() As Boolean
        Dim oRecordset As SAPbobsCOM.Recordset
        Dim ls_Query As String
        IsSuperUser = False
        oRecordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ls_Query = "Select * from ""OUSR"" Where ""SUPERUSER"" = 'Y' and ""USER_Code"" = '" + oApplication.Company.UserName + "'"
        oRecordset.DoQuery(ls_Query)
        If oRecordset.RecordCount > 0 Then
            IsSuperUser = True
        Else
            IsSuperUser = False
        End If
        Return IsSuperUser
    End Function
    Function keygencode(ByVal vtablename As String, Optional ByVal prefix As String = "DOC-") As String

        Dim str As String = ""
        Dim Query As String
        Try
            Query = "SELECT MAX(CAST(""Code"" AS int)) AS ""code"" FROM """ + vtablename + """"
            Dim v_recordset As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            v_recordset.DoQuery(Query)
            v_recordset.MoveFirst()
            Dim code As Integer = v_recordset.Fields.Item("code").Value.ToString
            If code > 0 Then
                code += 1
                Dim docid As String = prefix
                If code.ToString.Length < 6 Then
                    For count As Integer = 0 To 5 - code.ToString.Length
                        docid += "0"
                    Next
                End If
                docid += code.ToString
                str = code
            Else
                str = "1"
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_recordset)
            v_recordset = Nothing
            GC.Collect()
        Catch ex As Exception
            oApplication.MessageBox(ex.Message)
        End Try
        keygencode = str
    End Function

#End Region

#Region "   -- Printing Common Functions --     "

    Public Function AlignLeft(ByVal vStr As String, ByVal vSpace As Integer) As String
        If Len(Trim(vStr)) > vSpace Then '//if the string length is greater than the space you mention
            AlignLeft = Left(vStr, vSpace - 3) & "..."
            Exit Function
        End If

        AlignLeft = vStr & Space(vSpace - Len(Trim(vStr)))
    End Function
    Public Function AlignRight(ByVal vNumber As String, ByVal vSpace As Integer) As String
        AlignRight = Space(vSpace - Len(Trim(vNumber))) & vNumber
    End Function
    Public Function RepeatString(ByVal vStr As String, ByVal vSpace As Integer) As String
        Dim x As Integer

        For x = 1 To vSpace
            RepeatString = RepeatString & vStr
        Next x
    End Function
    'Public Sub Print(ByVal Report As String, ByRef PrinterName As String)
    '    Dim pd As New PrintDialog()
    '    pd.PrinterSettings = New PrinterSettings()
    '    If (pd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
    '        PrinterName = pd.PrinterSettings.PrinterName
    '        RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, Report)
    '    End If
    'End Sub
    Private Sub ChangeFont()
        Try

            Dim SR As New StreamReader("C:\Report.txt") 'Input File Name

            Dim NewStrLineData As String
            NewStrLineData = ""
            'Read the contents of the input file line by line
            Dim NewFont As Font
            NewFont = New Font("Symbol", 10, FontStyle.Regular)
            Do While SR.Peek <> -1
                NewStrLineData += SR.ReadLine.ToUpper
            Loop
            SR.Close()
            Dim SW As New StreamWriter("C:\Report.txt") 'Output File Name
            SW.WriteLine(NewStrLineData, NewFont)
            SW.Close()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region

#Region "   -- Common Functions --      "

    Function RupeesToWord(ByVal MyNumber)
        Dim Temp
        Dim Rupees, Paisa As String
        Dim DecimalPlace, iCount
        Dim Hundreds, Words As String
        Dim place(9) As String
        place(0) = " Thousand "
        place(2) = " Lakh "
        place(4) = " Crore "
        place(6) = " Arab "
        place(8) = " Kharab "
        On Error Resume Next
        ' Convert MyNumber to a string, trimming extra spaces.
        MyNumber = Trim(Str(MyNumber))

        ' Find decimal place.
        DecimalPlace = InStr(MyNumber, ".")

        ' If we find decimal place...
        If DecimalPlace > 0 Then
            ' Convert Paisa
            Temp = Left(Mid(MyNumber, DecimalPlace + 1) & "00", 2)
            Paisa = " and " & ConvertTens(Temp) & " Paisa"

            ' Strip off paisa from remainder to convert.
            MyNumber = Trim(Left(MyNumber, DecimalPlace - 1))
        End If

        '===============================================================
        Dim TM As String  ' If MyNumber between Rs.1 To 99 Only.
        TM = Right(MyNumber, 2)

        If Len(MyNumber) > 0 And Len(MyNumber) <= 2 Then
            If Len(TM) = 1 Then
                Words = ConvertDigit(TM)
                RupeesToWord = "Rupees " & Words & Paisa & " Only"

                Exit Function

            Else
                If Len(TM) = 2 Then
                    Words = ConvertTens(TM)
                    RupeesToWord = "Rupees " & Words & Paisa & " Only"
                    Exit Function

                End If
            End If
        End If
        '===============================================================


        ' Convert last 3 digits of MyNumber to ruppees in word.
        Hundreds = ConvertHundreds(Right(MyNumber, 3))
        ' Strip off last three digits
        MyNumber = Left(MyNumber, Len(MyNumber) - 3)

        iCount = 0
        Do While MyNumber <> ""
            'Strip last two digits
            Temp = Right(MyNumber, 2)
            If Len(MyNumber) = 1 Then


                If Trim(Words) = "Thousand" Or
                Trim(Words) = "Lakh  Thousand" Or
                Trim(Words) = "Lakh" Or
                Trim(Words) = "Crore" Or
                Trim(Words) = "Crore  Lakh  Thousand" Or
                Trim(Words) = "Arab  Crore  Lakh  Thousand" Or
                Trim(Words) = "Arab" Or
                Trim(Words) = "Kharab  Arab  Crore  Lakh  Thousand" Or
                Trim(Words) = "Kharab" Then

                    Words = ConvertDigit(Temp) & place(iCount)
                    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

                Else

                    Words = ConvertDigit(Temp) & place(iCount) & Words
                    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

                End If
            Else

                If Trim(Words) = "Thousand" Or
                   Trim(Words) = "Lakh  Thousand" Or
                   Trim(Words) = "Lakh" Or
                   Trim(Words) = "Crore" Or
                   Trim(Words) = "Crore  Lakh  Thousand" Or
                   Trim(Words) = "Arab  Crore  Lakh  Thousand" Or
                   Trim(Words) = "Arab" Then


                    Words = ConvertTens(Temp) & place(iCount)


                    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
                Else

                    '=================================================================
                    ' if only Lakh, Crore, Arab, Kharab

                    If Trim(ConvertTens(Temp) & place(iCount)) = "Lakh" Or
                       Trim(ConvertTens(Temp) & place(iCount)) = "Crore" Or
                       Trim(ConvertTens(Temp) & place(iCount)) = "Arab" Then

                        Words = Words
                        MyNumber = Left(MyNumber, Len(MyNumber) - 2)
                    Else
                        Words = ConvertTens(Temp) & place(iCount) & Words
                        MyNumber = Left(MyNumber, Len(MyNumber) - 2)
                    End If

                End If
            End If

            iCount = iCount + 2
        Loop

        RupeesToWord = "Rupees " & Words & Hundreds & Paisa & " Only"
    End Function

    ' Conversion for hundreds
    '*****************************************
    Private Function ConvertHundreds(ByVal MyNumber)
        Dim Result As String

        ' Exit if there is nothing to convert.
        If Val(MyNumber) = 0 Then Exit Function

        ' Append leading zeros to number.
        MyNumber = Right("000" & MyNumber, 3)

        ' Do we have a hundreds place digit to convert?
        If Left(MyNumber, 1) <> "0" Then
            Result = ConvertDigit(Left(MyNumber, 1)) & " Hundreds "
        End If

        ' Do we have a tens place digit to convert?
        If Mid(MyNumber, 2, 1) <> "0" Then
            Result = Result & ConvertTens(Mid(MyNumber, 2))
        Else
            ' If not, then convert the ones place digit.
            Result = Result & ConvertDigit(Mid(MyNumber, 3))
        End If

        ConvertHundreds = Trim(Result)
    End Function

    ' Conversion for tens
    '*****************************************
    Private Function ConvertTens(ByVal MyTens)
        Dim Result As String

        ' Is value between 10 and 19?
        If Val(Left(MyTens, 1)) = 1 Then
            Select Case Val(MyTens)
                Case 10 : Result = "Ten"
                Case 11 : Result = "Eleven"
                Case 12 : Result = "Twelve"
                Case 13 : Result = "Thirteen"
                Case 14 : Result = "Fourteen"
                Case 15 : Result = "Fifteen"
                Case 16 : Result = "Sixteen"
                Case 17 : Result = "Seventeen"
                Case 18 : Result = "Eighteen"
                Case 19 : Result = "Nineteen"
                Case Else
            End Select
        Else
            ' .. otherwise it's between 20 and 99.
            Select Case Val(Left(MyTens, 1))
                Case 2 : Result = "Twenty "
                Case 3 : Result = "Thirty "
                Case 4 : Result = "Forty "
                Case 5 : Result = "Fifty "
                Case 6 : Result = "Sixty "
                Case 7 : Result = "Seventy "
                Case 8 : Result = "Eighty "
                Case 9 : Result = "Ninety "
                Case Else
            End Select

            ' Convert ones place digit.
            Result = Result & ConvertDigit(Right(MyTens, 1))
        End If

        ConvertTens = Result
    End Function
    Private Function ConvertDigit(ByVal MyDigit)
        Select Case Val(MyDigit)
            Case 1 : ConvertDigit = "One"
            Case 2 : ConvertDigit = "Two"
            Case 3 : ConvertDigit = "Three"
            Case 4 : ConvertDigit = "Four"
            Case 5 : ConvertDigit = "Five"
            Case 6 : ConvertDigit = "Six"
            Case 7 : ConvertDigit = "Seven"
            Case 8 : ConvertDigit = "Eight"
            Case 9 : ConvertDigit = "Nine"
            Case Else : ConvertDigit = ""
        End Select
    End Function
    Private Function DEC4(ByVal Str As String) As String
        Dim s As String = Trim(Str)
        Dim output As String
        If s.Contains(".") = True Then
            output = s.Split(".")(1)
            For i As Integer = 2 - s.Split(".")(1).Length To 1 Step -1
                output = output & "0"
            Next
            output = s.Split(".")(0) & "." & output
        Else
            output = s & ".00"
        End If
        Return output
    End Function
    Sub SAPXML(ByVal path As String, Optional ByVal CHILD_FORM As String = "")
        Try
            Dim xmldoc As New Xml.XmlDocument
            Dim Streaming As System.IO.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("IK_PROD_QLTY." + path)
            Dim StreamRead As New System.IO.StreamReader(Streaming, True)
            xmldoc.LoadXml(StreamRead.ReadToEnd)
            StreamRead.Close()
            If Not xmldoc.SelectSingleNode("//form") Is Nothing Then
                If Trim(CHILD_FORM).Equals("") = True Then
                    Dim r As New Random
                    xmldoc.SelectSingleNode("//form").Attributes.GetNamedItem("uid").Value = xmldoc.SelectSingleNode("//form").Attributes.GetNamedItem("uid").Value & "_" & r.Next(100)
                Else
                    xmldoc.SelectSingleNode("//form").Attributes.GetNamedItem("uid").Value = CHILD_FORM
                End If
            End If

            oApplication.LoadBatchActions(xmldoc.InnerXml)

        Catch ex As Exception
            oApplication.MessageBox(ex.Message)
        End Try
    End Sub
    Sub GetSeries(ByVal FormUID As String, ByVal ItemUID As String, ByVal ObjectType As String)
        Try
            Dim objForm As SAPbouiCOM.Form = oApplication.Forms.Item(FormUID)
            Dim objCombo As SAPbouiCOM.ComboBox = objForm.Items.Item(ItemUID).Specific
            Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery("Select ""Series"",""SeriesName"" from ""NNM1"" Where ""ObjectCode""='" & Trim(ObjectType) & "'")
            If objCombo.ValidValues.Count = 0 Then
                For Row As Integer = 1 To oRS.RecordCount
                    objCombo.ValidValues.Add(oRS.Fields.Item("Series").Value, oRS.Fields.Item("SeriesName").Value)
                    oRS.MoveNext()
                Next
            End If
            oRS.DoQuery("Select ""DfltSeries"" from ONNM Where ""ObjectCode""='" & Trim(ObjectType) & "'")

            If objCombo.ValidValues.Count > 0 Then objCombo.Select(Trim(oRS.Fields.Item("DfltSeries").Value), SAPbouiCOM.BoSearchKey.psk_ByValue)
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
    Sub ShowReport(ByVal rptName As String, ByVal SourceXML As String)
        Try

            Dim oSubReport As CrystalDecisions.CrystalReports.Engine.SubreportObject
            Dim rptSubReportDoc As CrystalDecisions.CrystalReports.Engine.ReportDocument
            Dim rptView As New CrystalDecisions.Windows.Forms.CrystalReportViewer
            Dim rptPath As String = System.Windows.Forms.Application.StartupPath & "\" & rptName
            Dim rptDoc As New CrystalDecisions.CrystalReports.Engine.ReportDocument
            rptDoc.Load(rptPath)
            For Each oMainReportTable As CrystalDecisions.CrystalReports.Engine.Table In rptDoc.Database.Tables
                oMainReportTable.Location = System.IO.Path.GetTempPath() & SourceXML
            Next
            For Each rptSection As CrystalDecisions.CrystalReports.Engine.Section In rptDoc.ReportDefinition.Sections
                For Each rptObject As CrystalDecisions.CrystalReports.Engine.ReportObject In rptSection.ReportObjects
                    If rptObject.Kind = CrystalDecisions.Shared.ReportObjectKind.SubreportObject Then
                        oSubReport = rptObject
                        rptSubReportDoc = oSubReport.OpenSubreport(oSubReport.SubreportName)
                        For Each oSubTable As CrystalDecisions.CrystalReports.Engine.Table In rptSubReportDoc.Database.Tables
                            oSubTable.Location = System.IO.Path.GetTempPath() & SourceXML
                        Next
                    End If
                Next
            Next

            'rptDoc.PrintOptions.PaperSize = CType(rawKind, CrystalDecisions.Shared.PaperSize)
            Dim oThread As New System.Threading.Thread(AddressOf ReportThread)
            oThread.SetApartmentState(System.Threading.ApartmentState.STA)
            oThread.Start(rptDoc)

        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
    Public Sub ReportThread(ByVal rptdoc As Object)
        Try
            Dim rptForm1 As New rptForm
            rptForm1.CrystalReportViewer1.ReportSource = rptdoc
            rptForm1.ShowDialog()
        Catch ex As Exception
            oApplication.MessageBox(ex.Message.ToString)
        End Try
    End Sub
    Sub FilterWarehouse(ByVal FormUID As String, ByVal CFL_Id As String)
        Try
            Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim emptyConds As New SAPbouiCOM.Conditions
            oCFL = oApplication.Forms.Item(FormUID).ChooseFromLists.Item(CFL_Id)
            oCFL.SetConditions(emptyConds)
            oCons = oCFL.GetConditions()
            oRS.DoQuery("Select DISTINCT ""U_WhsCode"" from ""@GEN_USR_WHS"" Where ""U_UserID""='" & Trim(oCompany.UserName) & "'")
            For i As Integer = 1 To oRS.RecordCount
                If i > 1 Then
                    oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR
                End If
                oCon = oCons.Add()
                oCon.Alias = "WhsCode"
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                oCon.CondVal = oRS.Fields.Item("U_WhsCode").Value
                oRS.MoveNext()
            Next
            If oRS.RecordCount = 0 Then
                oCon = oCons.Add()
                oCon.Alias = "WhsCode"
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                oCon.CondVal = "-1"
            End If
            oCFL.SetConditions(oCons)
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Sub FilterTaxCode(ByVal FormUID As String, ByVal CFL_Id As String, ByVal WhsCode As String, Optional ByVal SalesTax As String = "No")
        Try
            Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim emptyConds As New SAPbouiCOM.Conditions
            oCFL = oApplication.Forms.Item(FormUID).ChooseFromLists.Item(CFL_Id)
            oCFL.SetConditions(emptyConds)
            oCons = oCFL.GetConditions()
            oRS.DoQuery("Select DISTINCT T0.""Code"" from ""OSTC"" T0 INNER JOIN ""OWHS"" T1 ON " + _str_IsNull + "(T0.""U_State"",'')=T1.""State"" Where T1.""WhsCode""='" & Trim(WhsCode) & "' and " + _str_IsNull + "(T0.""U_SalesTax"",'No')='" & SalesTax.Trim & "' " _
                        & " UNION Select DISTINCT T0.""Code"" from ""OSTC"" T0 Where " + _str_IsNull + "(T0.""U_State"",'')='' and " + _str_IsNull + "(T0.""U_SalesTax"",'No')='" & SalesTax.Trim & "'")
            For i As Integer = 1 To oRS.RecordCount
                If i > 1 Then
                    oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR
                End If
                oCon = oCons.Add()
                oCon.Alias = "Code"
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                oCon.CondVal = oRS.Fields.Item("Code").Value
                oRS.MoveNext()
            Next
            If oRS.RecordCount = 0 Then
                oCon = oCons.Add()
                oCon.Alias = "Code"
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                oCon.CondVal = "-1"
            End If
            oCFL.SetConditions(oCons)
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Sub ExecuteStoredProcedures()
        Try
            'Dim file As New FileInfo((Application.StartupPath).ToString() & "\SPs\Drop_[GET_Customer].sql")
            'Dim script As String = file.OpenText().ReadToEnd()
            'Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'oRS.DoQuery(script)

            'file = New FileInfo((Application.StartupPath).ToString() & "\SPs\Create_[GET_Customer].sql")
            'script = file.OpenText().ReadToEnd()
            'oRS.DoQuery(script)

        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region "   -- Amount In Words --   "

    Function SpellNumber(ByVal MyNumber As String, Optional ByVal incRupees As Boolean = True) As String
        Dim Crores, Lakhs, Rupees, Paise, Temp
        Dim DecimalPlace As Long, Count As Long
        Dim myLakhs, myCrores As String
        Dim Place(9) As String
        Place(2) = " Thousand " : Place(3) = " Million "
        Place(4) = " Billion " : Place(5) = " Trillion "
        ' String representation of amount.
        MyNumber = Trim(Str(MyNumber))
        ' Position of decimal place 0 if none.
        DecimalPlace = InStr(MyNumber, ".")
        ' Convert Paise and set MyNumber to Rupees amount.
        If DecimalPlace > 0 Then
            Paise = GetTens(Left(Mid(MyNumber, DecimalPlace + 1) & "00", 2))
            MyNumber = Trim(Left(MyNumber, DecimalPlace - 1))
        Else
            Paise = ""
        End If
        myCrores = MyNumber \ 10000000
        myLakhs = (MyNumber - myCrores * 10000000) \ 100000
        MyNumber = MyNumber - myCrores * 10000000 - myLakhs * 100000
        Count = 1
        Do While myCrores <> ""
            Temp = GetHundreds(Right(myCrores, 3))
            If Temp <> "" Then Crores = Temp & Place(Count) & Crores
            If Len(myCrores) > 3 Then
                myCrores = Left(myCrores, Len(myCrores) - 3)
            Else
                myCrores = ""
            End If
            Count = Count + 1
        Loop
        Count = 1
        Do While myLakhs <> ""
            Temp = GetHundreds(Right(myLakhs, 3))
            If Temp <> "" Then Lakhs = Temp & Place(Count) & Lakhs
            If Len(myLakhs) > 3 Then
                myLakhs = Left(myLakhs, Len(myLakhs) - 3)
            Else
                myLakhs = ""
            End If
            Count = Count + 1
        Loop
        Count = 1
        Do While MyNumber <> ""
            Temp = GetHundreds(Right(MyNumber, 3))
            If Temp <> "" Then Rupees = Temp & Place(Count) & Rupees
            If Len(MyNumber) > 3 Then
                MyNumber = Left(MyNumber, Len(MyNumber) - 3)
            Else
                MyNumber = ""
            End If
            Count = Count + 1
        Loop
        Select Case Crores
            Case "" : Crores = ""
            Case "One" : Crores = " One Crore "
            Case Else : Crores = Crores & " Crores "
        End Select
        Select Case Lakhs
            Case "" : Lakhs = ""
            Case "One" : Lakhs = " One Lakh "
            Case Else : Lakhs = Lakhs & " Lakhs "
        End Select
        Select Case Rupees
            Case "" : Rupees = "Zero "
            Case "One" : Rupees = "One "
            Case Else
                Rupees = Rupees
        End Select
        Select Case Paise
            Case "" : Paise = " Only "
            Case "One" : Paise = " Ten Paise Only "
            Case Else : Paise = " and " & Paise & " Paise Only "
        End Select
        Return (IIf(incRupees, "Rupees ", "") & Crores & Lakhs & Rupees & Paise)
    End Function
    ' Converts a number from 100-999 into text
    Function GetHundreds(ByVal MyNumber)
        Dim Result As String = ""
        If Val(MyNumber) = 0 Then Exit Function
        MyNumber = Right("000" & MyNumber, 3)
        ' Convert the hundreds place.
        If Mid(MyNumber, 1, 1) <> "0" Then
            Result = GetDigit(Mid(MyNumber, 1, 1)) & " Hundred "
        End If
        ' Convert the tens and ones place.
        If Mid(MyNumber, 2, 1) <> "0" Then
            Result = Result & GetTens(Mid(MyNumber, 2))
        Else
            Result = Result & GetDigit(Mid(MyNumber, 3))
        End If
        GetHundreds = Result
    End Function
    ' Converts a number from 10 to 99 into text.
    Function GetTens(ByVal TensText)
        Dim Result As String
        Result = "" ' Null out the temporary function value.
        If Val(Left(TensText, 1)) = 1 Then ' If value between 10-19...
            Select Case Val(TensText)
                Case 10 : Result = "Ten"
                Case 11 : Result = "Eleven"
                Case 12 : Result = "Twelve"
                Case 13 : Result = "Thirteen"
                Case 14 : Result = "Fourteen"
                Case 15 : Result = "Fifteen"
                Case 16 : Result = "Sixteen"
                Case 17 : Result = "Seventeen"
                Case 18 : Result = "Eighteen"
                Case 19 : Result = "Nineteen"
                Case Else
            End Select
        Else ' If value between 20-99...
            Select Case Val(Left(TensText, 1))
                Case 2 : Result = "Twenty "
                Case 3 : Result = "Thirty "
                Case 4 : Result = "Forty "
                Case 5 : Result = "Fifty "
                Case 6 : Result = "Sixty "
                Case 7 : Result = "Seventy "
                Case 8 : Result = "Eighty "
                Case 9 : Result = "Ninety "
                Case Else
            End Select
            Result = Result & GetDigit _
            (Right(TensText, 1)) ' Retrieve ones place.
        End If
        GetTens = Result
    End Function
    ' Converts a number from 1 to 9 into text.
    Function GetDigit(ByVal Digit)
        Select Case Val(Digit)
            Case 1 : GetDigit = "One"
            Case 2 : GetDigit = "Two"
            Case 3 : GetDigit = "Three"
            Case 4 : GetDigit = "Four"
            Case 5 : GetDigit = "Five"
            Case 6 : GetDigit = "Six"
            Case 7 : GetDigit = "Seven"
            Case 8 : GetDigit = "Eight"
            Case 9 : GetDigit = "Nine"
            Case Else : GetDigit = ""
        End Select
    End Function

#End Region

End Class
