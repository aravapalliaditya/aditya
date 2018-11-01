Public Class clsSalesInvoice
#Region "---DECLARATION---"
    Dim objForM, OBJSUBFORM As SAPbouiCOM.Form
    Dim objUtilities As New clsUtilities
    Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
    Dim oCFL As SAPbouiCOM.ChooseFromList
    Dim objMatrix As SAPbouiCOM.Matrix
    Dim oGrid As SAPbouiCOM.Grid
    Dim oDBs_Head As SAPbouiCOM.DBDataSource
    Dim oDBs_Detail As SAPbouiCOM.DBDataSource
    Dim sum As Double
    Dim XYZ As Integer
    Dim StringVal As String
    Dim Row As Integer
    Dim bool As Boolean
#End Region
#Region "--Create Form--"
    Sub CreateForm()
        Try

            objUtilities.SAPXML("SalesInvoice.xml")
            objForM = oApplication.Forms.GetForm("SAINV", oApplication.Forms.ActiveForm.TypeCount)
            oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
            objMatrix = objForM.Items.Item("56").Specific
            'oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")

            'objMatrix.AddRow()
            'objMatrix.FlushToDataSource()
            'oDBs_Detail.SetValue("LineId", 0, objMatrix.VisualRowCount)
            'objMatrix.SetLineData(objMatrix.VisualRowCount)
            Me.DefaultForm(objForM.UniqueID)

        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub

#End Region

#Region "------MENU EVENT----"
    Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case "OINVMEC"

                        CreateForm()

                    Case "1282"
                        If objForM.TypeEx = "OINVMEC" And pVal.BeforeAction = False Then
                            DefaultForm(objForM.UniqueID)
                        End If
                    Case "1281"
                        If objForM.TypeEx = "OINVMEC" Then
                            objForM.EnableMenu("1282", True)
                        End If

                        'Case "OINVMEC"
                        '    Me.CreateForm()
                        'Case "1282"
                        '    Me.DefaultForm(objForM.UniqueID)
                        'Case "1284"
                        '    'objForM.Close()
                End Select
            End If
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
#End Region
#Region "--item event ---"
    Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        objForM = oApplication.Forms.GetForm("SAINV", oApplication.Forms.ActiveForm.TypeCount)
        oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
        oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
        Try
            Select Case pVal.EventType
                Case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE
                    If bool = True Then
                        bool = False
                        objMatrix = objForM.Items.Item("56").Specific
                        For i As Integer = 1 To objMatrix.VisualRowCount - 1
                            objMatrix.Columns.Item("V_1").Cells.Item(i).Specific.value = StringVal
                        Next
                    End If
                Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT

                    objForM = oApplication.Forms.Item(pVal.FormUID)
                    Try
                        oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
                        If pVal.ItemUID = "30" And pVal.BeforeAction = False And pVal.FormMode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                            oDBs_Head.SetValue("U_Series", 0, objForM.BusinessObject.GetNextSerialNumber(objForM.Items.Item("30").Specific.Selected.Value, "IKOINVOBJ"))
                        End If
                    Catch ex As Exception

                    End Try

                Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                    objForM = oApplication.Forms.Item(pVal.FormUID)
                    If pVal.BeforeAction = False Then
                        ChooseFromListCondition(FormUID, pVal)
                    End If
                Case SAPbouiCOM.BoEventTypes.et_VALIDATE
                    objForM = oApplication.Forms.Item(pVal.FormUID)
                    oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
                    oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
                    objMatrix = objForM.Items.Item("56").Specific
                    If pVal.ItemUID = "56" And (pVal.ColUID = "V_18" Or pVal.ColUID = "V_17" Or pVal.ColUID = "V_16") And pVal.Before_Action = False And pVal.InnerEvent = False Then
                        Me.fieldtotal(FormUID, pVal)
                        Me.sumtotal(FormUID, pVal)
                    End If


                Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                    Try
                        ' objForM = oApplication.Forms.Item(pVal.FormUID)
                        objForM = oApplication.Forms.GetForm("SAINV", oApplication.Forms.ActiveForm.TypeCount)
                        objMatrix = objForM.Items.Item("56").Specific
                        If pVal.ItemUID = "1" And pVal.BeforeAction = True And (pVal.FormMode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or pVal.FormMode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                            If Me.Validation(FormUID, pVal) = False Then
                                BubbleEvent = False
                                Exit Sub
                            End If
                        End If
                        If pVal.ItemUID = "1" And pVal.Before_Action = False And pVal.Action_Success = True And pVal.FormMode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                            objForM = oApplication.Forms.Item(pVal.FormUID)
                            Me.DefaultForm(FormUID)
                        End If
                        If pVal.ItemUID = "1" And pVal.ActionSuccess = False And pVal.BeforeAction = True Then
                            If objMatrix.VisualRowCount <> 1 Then
                                objMatrix.DeleteRow(objMatrix.VisualRowCount)
                            End If
                        End If

                    Catch ex As Exception
                        oApplication.StatusBar.SetText(ex.Message)
                    End Try

            End Select
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
#End Region
    '#Region "--DataEvent"
    '    Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
    '        Try
    '            Select Case BusinessObjectInfo.EventType
    '                Case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD, SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE
    '                    If BusinessObjectInfo.ActionSuccess = True Then
    '                        objForM = oApplication.Forms.Item(BusinessObjectInfo.FormUID)
    '                        objForM.EnableMenu("1282", True)
    '                    End If
    '            End Select
    '        Catch ex As Exception
    '            objForM.Freeze(False)
    '        End Try
    '    End Sub
    '#End Region
#Region "--METHODS---"
    Sub DefaultForm(ByVal FormUID As String)
        Try
            objForM = oApplication.Forms.Item(FormUID)
            Dim oDBs_Head As SAPbouiCOM.DBDataSource = objForM.DataSources.DBDataSources.Item("@IKOINV")
            Dim oDBs_Detail As SAPbouiCOM.DBDataSource = objForM.DataSources.DBDataSources.Item("@IKINV1")
            objForM.EnableMenu("1283", False)
            oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
            oDBs_Head.SetValue("U_DocDate", 0, DateTime.Now.ToString("yyyyMMdd"))
            oDBs_Head.SetValue("U_TaxDate", 0, DateTime.Now.ToString("yyyyMMdd"))
            objUtilities.GetSeries(FormUID, "30", "IKOINVOBJ")
            oDBs_Head.SetValue("U_Series", 0, objForM.BusinessObject.GetNextSerialNumber(objForM.Items.Item("30").Specific.Selected.Value, "IKOINVOBJ"))
            AutogenDocNum()
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
            objMatrix.AddRow()
            objMatrix.FlushToDataSource()
            oDBs_Detail.SetValue("LineId", 0, objMatrix.VisualRowCount)
            objMatrix.SetLineData(objMatrix.VisualRowCount)
            objForM.DataBrowser.BrowseBy = "38"
            'AddChooseFromListItem(objForM.UniqueID)
        Catch EX As Exception
        End Try
    End Sub
    Sub AutogenDocNum()
        Dim txtDocNum = objForM.Items.Item("38").Specific
        txtDocNum.Value = ""
        Try
            Dim GetDocNum As String = "Select ""NextNumber"" from ""NNM1"" where ""ObjectCode"" ='IKOINVOBJ'"
            Dim oRsGetDocNum As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRsGetDocNum.DoQuery(GetDocNum)
            txtDocNum.Value = oRsGetDocNum.Fields.Item("NextNumber").Value
        Catch ex As Exception
            oCompany.objApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
    Sub ChooseFromListCondition(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent)

        objForM = oApplication.Forms.Item(pVal.FormUID)
        Dim sCFL_ID As String
        oCFLEvento = pVal
        sCFL_ID = oCFLEvento.ChooseFromListUID
        oCFL = objForM.ChooseFromLists.Item(sCFL_ID)
        Dim oDataTable As SAPbouiCOM.DataTable
        oDataTable = oCFLEvento.SelectedObjects
        objForM = oApplication.Forms.Item(pVal.FormUID)
        oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
        oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
        objMatrix = objForM.Items.Item("56").Specific
        If Not (oDataTable Is Nothing) And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE And pVal.BeforeAction = False And pVal.ActionSuccess = True Then
            If pVal.FormMode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then objForM.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE

            If (oCFL.UniqueID = "CFL_OCRD" Or oCFL.UniqueID = "CFL_OCRD1") Then
                oDBs_Head.SetValue("U_CardCode", 0, oDataTable.GetValue("CardCode", 0))
                oDBs_Head.SetValue("U_CardName", 0, oDataTable.GetValue("CardName", 0))
                FillContactPerson(FormUID, "27")
                Dim oRecordSet As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                If oDataTable.GetValue("State2", 0) <> "" Then
                    Dim strQry As String = "Select T1.""Location""  From ""OCRD"" T0 Inner Join ""OLCT"" T1 On T0.""State2"" = T1.""State""   Where T0.""CardCode"" = '" + oDBs_Head.GetValue("U_CardCode", 0) + "' "
                    oRecordSet.DoQuery(strQry)
                    oDBs_Head.SetValue("U_PlaOfSup", oDBs_Detail.Offset, oRecordSet.Fields.Item("Location").Value)
                End If
                Dim today As DateTime = DateTime.Today
                Dim dueDate As DateTime = today.AddDays(29)
                oDBs_Head.SetValue("U_DocDueDa", 0, dueDate.ToString("yyyyMMdd"))
            End If
            If (oCFL.UniqueID = "CFL_OCST") Then
                oDBs_Head.SetValue("U_PlaOfSup", 0, oDataTable.GetValue("Name", 0))
            End If
            If (oCFL.UniqueID = "CFL_SAL2") Then
                Try
                    oDBs_Head.SetValue("U_Comment", 0, oDataTable.GetValue("Code", 0))
                    objMatrix = objForM.Items.Item("56").Specific
                    'objMatrix.Columns.Item("V_1").Cells.Item(1).Specific.value = oDataTable.GetValue("Name", 0)
                    bool = True
                    StringVal = oDataTable.GetValue("Name", 0)
                Catch ex As Exception
                End Try
            End If
            If (oCFL.UniqueID = "CFL_OHEM") Then
                oDBs_Head.SetValue("U_OwnerCo", 0, oDataTable.GetValue("firstName", 0))
            End If
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_21" And oCFL.UniqueID = "CFL_OITM" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            objMatrix.FlushToDataSource()
            oDBs_Detail.SetValue("LineId", oDBs_Detail.Offset, pVal.Row)
            oDBs_Detail.SetValue("U_ItemCode", oDBs_Detail.Offset, oDataTable.GetValue("ItemCode", 0))

            Dim oRecordSet As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strQry1 As String = "Select T1.""Substitute""  From ""OITM"" T0 Left Join ""OSCN"" T1 On T0.""ItemCode"" = T1.""ItemCode""   Where T0.""ItemCode"" = '" + oDBs_Detail.GetValue("U_ItemCode", oDBs_Detail.Offset) + "' "
            oRecordSet.DoQuery(strQry1)
            If oRecordSet.Fields.Item("Substitute").Value <> "" Then
                oDBs_Detail.SetValue("U_SubCatNu", oDBs_Detail.Offset, oRecordSet.Fields.Item("Substitute").Value)
            End If

            oDBs_Detail.SetValue("U_Dscrip", oDBs_Detail.Offset, oDataTable.GetValue("ItemName", 0))
            oDBs_Detail.SetValue("U_Quantity", oDBs_Detail.Offset, 1)
            If oDataTable.GetValue("WTLiable", 0) = "Y" Then
                oDBs_Detail.SetValue("U_WtLiable", oDBs_Detail.Offset, "Yes")
            End If
            If oDataTable.GetValue("WTLiable", 0) = "N" Then
                oDBs_Detail.SetValue("U_WtLiable", oDBs_Detail.Offset, "No")
            End If


                oDBs_Detail.SetValue("U_WhsCode", oDBs_Detail.Offset, oDataTable.GetValue("DfltWH", 0))
            If oDataTable.GetValue("DfltWH", 0) <> "" Then
                Dim strQry As String = "Select T1.""Location""  From ""OWHS"" T0 Inner Join ""OLCT"" T1 On T0.""Location"" = T1.""Code""   Where T0.""WhsCode"" = '" + oDBs_Detail.GetValue("U_WhsCode", oDBs_Detail.Offset) + "' "
                oRecordSet.DoQuery(strQry)
                oDBs_Detail.SetValue("U_LocCode", oDBs_Detail.Offset, oRecordSet.Fields.Item("Location").Value)
            End If
            If oDataTable.GetValue("ChapterID", 0) <> 0 Then
                Dim strQry As String = "Select T1.""ChapterID""  From ""OITM"" T0 Inner Join ""OCHP"" T1 On T0.""ChapterID"" = T1.""AbsEntry""   Where T0.""ItemCode"" = '" + oDBs_Detail.GetValue("U_ItemCode", oDBs_Detail.Offset) + "' "
                oRecordSet.DoQuery(strQry)
                oDBs_Detail.SetValue("U_HSN", oDBs_Detail.Offset, oRecordSet.Fields.Item("ChapterID").Value)
            End If
            If oDataTable.GetValue("UgpEntry", 0) = -1 Then
                oDBs_Detail.SetValue("U_UomCode", oDBs_Detail.Offset, "Manual")
            End If
            objMatrix.SetLineData(pVal.Row)
            Dim rn As Integer = objMatrix.VisualRowCount
            If objMatrix.Columns.Item("V_21").Cells.Item(objMatrix.VisualRowCount).Specific.value <> "" Then
                objMatrix.AddRow()
                objMatrix.FlushToDataSource()
                Me.SetNewLine_Matrix(FormUID, objMatrix.VisualRowCount)
            End If
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_20" And oCFL.UniqueID = "CFL_OSCN" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            oDBs_Detail.SetValue("SubCatNu", oDBs_Detail.Offset, oDataTable.GetValue("Substitute", 0))
            objMatrix.SetLineData(pVal.Row)
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_19" And oCFL.UniqueID = "CFL_OITM1" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            objMatrix.FlushToDataSource()
            oDBs_Detail.SetValue("LineId", oDBs_Detail.Offset, pVal.Row)
            oDBs_Detail.SetValue("U_ItemCode", oDBs_Detail.Offset, oDataTable.GetValue("ItemCode", 0))
            oDBs_Detail.SetValue("U_Dscrip", oDBs_Detail.Offset, oDataTable.GetValue("ItemName", 0))
            oDBs_Detail.SetValue("U_Quantity", oDBs_Detail.Offset, 1)
            If oDataTable.GetValue("WTLiable", 0) = "Y" Then
                oDBs_Detail.SetValue("U_WtLiable", oDBs_Detail.Offset, "Yes")
            End If
            If oDataTable.GetValue("WTLiable", 0) = "N" Then
                oDBs_Detail.SetValue("U_WtLiable", oDBs_Detail.Offset, "No")
            End If
            Dim oRecordSet As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oDBs_Detail.SetValue("U_WhsCode", oDBs_Detail.Offset, oDataTable.GetValue("DfltWH", 0))
            If oDataTable.GetValue("DfltWH", 0) <> "" Then
                Dim strQry As String = "Select T1.""Location""  From ""OWHS"" T0 Inner Join ""OLCT"" T1 On T0.""Location"" = T1.""Code""   Where T0.""WhsCode"" = '" + oDBs_Detail.GetValue("U_WhsCode", oDBs_Detail.Offset) + "' "
                oRecordSet.DoQuery(strQry)
                oDBs_Detail.SetValue("U_LocCode", oDBs_Detail.Offset, oRecordSet.Fields.Item("Location").Value)
            End If
            If oDataTable.GetValue("ChapterID", 0) <> 0 Then
                Dim strQry As String = "Select T1.""ChapterID""  From ""OITM"" T0 Inner Join ""OCHP"" T1 On T0.""ChapterID"" = T1.""AbsEntry""   Where T0.""ItemCode"" = '" + oDBs_Detail.GetValue("U_ItemCode", oDBs_Detail.Offset) + "' "
                oRecordSet.DoQuery(strQry)
                oDBs_Detail.SetValue("U_HSN", oDBs_Detail.Offset, oRecordSet.Fields.Item("ChapterID").Value)
            End If
            If oDataTable.GetValue("UgpEntry", 0) = -1 Then
                oDBs_Detail.SetValue("U_UomCode", oDBs_Detail.Offset, "Manual")
            End If
            objMatrix.SetLineData(pVal.Row)
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_15" And oCFL.UniqueID = "CFL_OSTC" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            oDBs_Detail.SetValue("U_TaxCode", oDBs_Detail.Offset, oDataTable.GetValue("Code", 0))

            objMatrix.SetLineData(pVal.Row)
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_12" And oCFL.UniqueID = "CFL_OWHS" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            Dim oRecordSet As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oDBs_Detail.SetValue("U_WhsCode", oDBs_Detail.Offset, oDataTable.GetValue("WhsCode", 0))
            Dim strQry As String = "Select T1.""Location""  From ""OWHS"" T0 Inner Join ""OLCT"" T1 On T0.""Location"" = T1.""Code""   Where T0.""WhsCode"" = '" + oDBs_Detail.GetValue("U_WhsCode", oDBs_Detail.Offset) + "' "
            oRecordSet.DoQuery(strQry)
            oDBs_Detail.SetValue("U_LocCode", oDBs_Detail.Offset, oRecordSet.Fields.Item("Location").Value)
            objMatrix.SetLineData(pVal.Row)
        End If
        If pVal.ItemUID = "56" And pVal.ColUID = "V_3" And oCFL.UniqueID = "CFL_OOCR" Then
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            oDBs_Detail.SetValue("U_CogsOcr", oDBs_Detail.Offset, oDataTable.GetValue("OcrCode", 0))
            objMatrix.SetLineData(pVal.Row)
        End If

        If pVal.ItemUID = "56" And pVal.ColUID = "V_1" And oCFL.UniqueID = "CFL_SALMST" Then
            oDBs_Head.SetValue("U_Comment", 0, oDataTable.GetValue("Code", 0))
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = pVal.Row - 1
            oDBs_Detail.SetValue("U_HSN", oDBs_Detail.Offset, oDataTable.GetValue("Name", 0))
            objMatrix.SetLineData(pVal.Row)
        End If
    End Sub
    Sub FillContactPerson(ByVal FormUID As String, ByVal ItemUID As String)
        Try

            Dim objForm As SAPbouiCOM.Form = oApplication.Forms.GetForm("SAINV", oApplication.Forms.ActiveForm.TypeCount)
            objMatrix = objForm.Items.Item("56").Specific
            Dim objCombo As SAPbouiCOM.ComboBox

            objCombo = objForm.Items.Item("27").Specific

            'objRsrcMatrix.Columns.Item(ItemUID).Cells.Item(objRsrcMatrix.RowCount).Specific

            Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim qry As String = "SELECT CntctPrsn from OCRD where CardCode='" + oDBs_Head.GetValue("U_CardCode", 0) + "' "
            oRS.DoQuery(qry)

            If objCombo.ValidValues.Count > 0 Then
                For i As Integer = objCombo.ValidValues.Count - 1 To 0 Step -1
                    '1 To objCombo.ValidValues.Count  '- 1 To 0 Step -1
                    objCombo.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index)
                Next
            End If
            If objCombo.ValidValues.Count = 0 Then
                'objCombo.ValidValues.Add("", "")

                For Row As Integer = 1 To oRS.RecordCount

                    objCombo.ValidValues.Add(oRS.Fields.Item("CntctPrsn").Value, oRS.Fields.Item("CntctPrsn").Value)
                    oRS.MoveNext()
                Next
            End If
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
    Sub SetNewLine_Matrix(ByVal FormUID As String, ByVal Row As Integer)
        Try
            objForM = oApplication.Forms.Item(FormUID)
            oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
            oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Detail.Offset = Row - 1
            oDBs_Detail.SetValue("LineId", oDBs_Detail.Offset, Row)
            oDBs_Detail.SetValue("U_ItemCode", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_SubCatNu", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_Dscrip", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_Quantity", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_UnitPri", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_DisPrcen", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_WtLiable", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_TotalLC", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_WhsCode", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_UomCode", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_LocCode", oDBs_Detail.Offset, "")
            oDBs_Detail.SetValue("U_HSN", oDBs_Detail.Offset, "")
            'oDBs_Detail.SetValue("U_DisPrcen", oDBs_Detail.Offset, "")
            'oDBs_Detail.SetValue("U_loc", oDBs_Detail.Offset, "")

            objMatrix.SetLineData(Row)
            objMatrix.FlushToDataSource()
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
        End Try
    End Sub
    Sub fieldtotal(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent)
        Try
            objForM = oApplication.Forms.Item(FormUID)
            oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
            objMatrix = objForM.Items.Item("56").Specific
            objMatrix.FlushToDataSource()
            objMatrix.GetLineData(pVal.Row)
            oDBs_Detail.Offset = pVal.Row - 1

            Dim TotLC As Double
            TotLC = oDBs_Detail.GetValue("U_Quantity", oDBs_Detail.Offset) * oDBs_Detail.GetValue("U_UnitPri", oDBs_Detail.Offset)
            Dim Discount As Double = (oDBs_Detail.GetValue("U_DisPrcen", oDBs_Detail.Offset) / 100) * TotLC
            oDBs_Detail.SetValue("U_TotalLC", oDBs_Detail.Offset, TotLC - Discount)
            objMatrix.SetLineData(pVal.Row)

        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)

        End Try
    End Sub
    Sub sumtotal(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent)
        Try
            objForM = oApplication.Forms.Item(FormUID)
            'objForM.Freeze(True)
            Dim oRecordSet As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim ProdPrc As Double = 0
            objMatrix = objForM.Items.Item("56").Specific
            oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")
            oDBs_Detail = objForM.DataSources.DBDataSources.Item("@IKINV1")
            For j As Integer = 1 To objMatrix.VisualRowCount
                objMatrix.FlushToDataSource()
                oDBs_Detail.Offset = j - 1
                objMatrix.GetLineData(j)
                ProdPrc = ProdPrc + oDBs_Detail.GetValue("U_TotalLC", oDBs_Detail.Offset)
            Next
            oDBs_Head.SetValue("U_Total", 0, ProdPrc)
            'objForM.Freeze(False)
        Catch ex As Exception
            'objForM.Freeze(True)
        End Try
    End Sub

#Region "Validation Forms"

    'Default Validation method
    Function Validation(ByVal FormUID As String, ByVal pval As SAPbouiCOM.ItemEvent)
        Try
            objForM = oApplication.Forms.Item(FormUID)
            oDBs_Head = objForM.DataSources.DBDataSources.Item("@IKOINV")

            If Trim(oDBs_Head.GetValue("U_CardCode", 0)).Equals("") = True Then
                oApplication.StatusBar.SetText("CardCode is Mandatory", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
                Exit Function
            End If
            If Trim(oDBs_Head.GetValue("U_CardName", 0)) = "" Then
                oApplication.StatusBar.SetText("CardName is Mandatory ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
                Exit Function
            End If

            Dim objMatrix_Operations As SAPbouiCOM.Matrix = objForM.Items.Item("56").Specific
            ' Dim objMatrix_Resources As SAPbouiCOM.Matrix = objForM.Items.Item("Mtx_Rsrc").Specific

            If objMatrix_Operations.VisualRowCount - 1 = 0 Then
                oApplication.StatusBar.SetText("No Operations defined", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                For Row As Integer = 1 To objMatrix_Operations.VisualRowCount - 1
                    If Trim(objMatrix_Operations.Columns.Item("V_21").Cells.Item(Row).Specific.Value).Equals("") = True Then
                        oApplication.StatusBar.SetText("Operations -> Row [ " & Row & " ] - Operation Code should not be left blank", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                        'ElseIf Trim(objMatrix_Operations.Columns.Item("C_OprNam").Cells.Item(Row).Specific.Value).Equals("") = True Then
                        '    oApplication.StatusBar.SetText("Operations -> Row [ " & Row & " ] - Operation Name should not be left blank", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    Return False
                        'ElseIf CDbl(objMatrix_Operations.Columns.Item("C_Qty").Cells.Item(Row).Specific.Value) <= 0 Then
                        '    oApplication.StatusBar.SetText("Operations -> Row [ " & Row & " ] - Quantity should be entered", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    Return False
                    End If
                Next
            End If

            Return True
        Catch ex As Exception
            oApplication.StatusBar.SetText(ex.Message)
            Return False
        End Try
    End Function

#End Region

#End Region


End Class
