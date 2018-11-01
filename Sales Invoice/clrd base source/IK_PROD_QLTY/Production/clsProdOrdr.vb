Public Class clsProdOrdr
#Region "Declaration"
    Dim oDBs_Detail As SAPbouiCOM.DBDataSource
    Dim objForm, objSubForm As SAPbouiCOM.Form
    Dim objMatrix As SAPbouiCOM.Matrix
    Dim oDataTable As SAPbouiCOM.DataTable
    Dim oDBs_Head As SAPbouiCOM.DataTable
#End Region

#Region "events"
    Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            objForm = oApplication.Forms.GetForm("139", oApplication.Forms.ActiveForm.TypeCount)
            Select Case pVal.EventType
                Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                    Try
                        objMatrix = objForm.Items.Item("1320002138").Specific
                        If pVal.ItemUID = "1320002139" And pVal.BeforeAction = False And pVal.ActionSuccess = True Then
                            Dim i As Integer = objMatrix.VisualRowCount
                            If objMatrix.VisualRowCount = 1 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "CDWG"
                                CompareString(FormUID, pVal)
                            ElseIf objMatrix.VisualRowCount = 2 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "PFD"
                                CompareString(FormUID, pVal)
                            ElseIf objMatrix.VisualRowCount = 3 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "FMEA"
                                CompareString(FormUID, pVal)
                            ElseIf objMatrix.VisualRowCount = 4 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "CTRP"
                                CompareString(FormUID, pVal)
                            ElseIf objMatrix.VisualRowCount = 5 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "INSPLN"
                                CompareString(FormUID, pVal)
                            ElseIf objMatrix.VisualRowCount = 6 Then
                                objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value = "INPRODWG"
                                CompareString(FormUID, pVal)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
            End Select
        Catch ex As Exception
        End Try
    End Sub
#End Region
    Sub CompareString(ByVal FormUID As String, ByRef pval As SAPbouiCOM.ItemEvent)
        Try
            Dim odbs_detail As SAPbouiCOM.DBDataSource
            objForm = oApplication.Forms.GetForm("139", oApplication.Forms.ActiveForm.TypeCount)
            objMatrix = objForm.Items.Item("1320002138").Specific
            Dim i As Integer = objMatrix.VisualRowCount
            ' Dim oDataTable As SAPbouiCOM.DataTable =
            odbs_detail = objForm.DataSources.DBDataSources.Item("ATC1")
            ' oDataTable = objMatrix.Columns.Item("256000011").Cells.Item(i).Specific
            Dim String1 As String = objMatrix.Columns.Item("256000011").Cells.Item(i).Specific.value 'odbs_detail.GetValue("256000011", pval.Row)
            Dim String2 As String = objMatrix.Columns.Item("1320000004").Cells.Item(i).Specific.value 'odbs_detail.GetValue("256000011", pval.Row)

            For j As Integer = 1 To objMatrix.VisualRowCount

                If Not String2.StartsWith(String1, StringComparison.OrdinalIgnoreCase) Then
                    oApplication.StatusBar.SetText("FileNmae shuld be Starts with " + String1, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

                End If

            Next
        Catch ex As Exception
        End Try

    End Sub
End Class
