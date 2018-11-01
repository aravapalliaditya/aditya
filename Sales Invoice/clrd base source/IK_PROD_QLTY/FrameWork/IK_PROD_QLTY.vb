Imports System
Imports System.IO
Imports System.Data.SqlClient

Module IK_PROD_QLTY
    Public WithEvents oApplication As SAPbouiCOM.Application
    Public oCompany As SAPbobsCOM.Company
    Dim objUtilities As New clsUtilities
    Public _str_IsNull, _str_LogInUser As String

#Region "   -- Start Up --     "
    Sub Main()
        objUtilities.StartUp()
        System.Windows.Forms.Application.Run()

    End Sub
#End Region

#Region "   -- Class Declaration --     "

#Region "   -- Sales Order --    "

    Dim objSalOrder As New clsProdOrdr


#End Region

#Region "   -- Sales Invoice --    "

    Dim objSalesInvoice As New clsSalesInvoice

#End Region

#End Region

#Region "   -- Application Events --     "

    Private Sub oApplication_AppEvent(ByVal EventType As SAPbouiCOM.BoAppEventTypes) Handles oApplication.AppEvent
        Select Case EventType
            Case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged, SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged, SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition, SAPbouiCOM.BoAppEventTypes.aet_ShutDown
                objUtilities.SAPXML("RemoveMenu.xml")
                oCompany.Disconnect()
                Application.Exit()
                End
        End Select
    End Sub
    'Private Sub oApplication_FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean) Handles oApplication.FormDataEvent
    '    Try
    '        Select Case BusinessObjectInfo.FormTypeEx

    '            '================ Sales Invoicet Data Load ================		
    '            Case "SAINV"
    '                objSalesInvoice.FormDataEvent(BusinessObjectInfo, BubbleEvent)

    '                '    '================ Quality Module ================
    '                '    Case "IK_PRMS"
    '                '        objParameters.FormDataEvent(BusinessObjectInfo, BubbleEvent)


    '        End Select

    '    Catch ex As Exception
    '        oApplication.SetStatusBarMessage(ex.Message)
    '    End Try
    'End Sub
    Private Sub oApplication_ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean) Handles oApplication.ItemEvent
        Try
            Select Case pVal.FormTypeEx
                '================ Sales Invoicet ITEM ================		
                Case "SAINV"
                    objSalesInvoice.ItemEvent(FormUID, pVal, BubbleEvent)

                    '================ Production Module ================
                    'Case "139"
                    '    objSalOrder.ItemEvent(FormUID, pVal, BubbleEvent)
                    'Case "65211"
                    '    objProdOrder.ItemEvent(FormUID, pVal, BubbleEvent)

            End Select

        Catch ex As Exception
            oApplication.SetStatusBarMessage(ex.Message)
        End Try
    End Sub
    Private Sub oApplication_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles oApplication.MenuEvent
        Try
            Dim objForm As SAPbouiCOM.Form
            Try
                objForm = oApplication.Forms.ActiveForm
            Catch ex As Exception
            End Try
            Try
                Try
                    objForm = oApplication.Forms.ActiveForm
                Catch ex As Exception
                End Try
                Select Case pVal.MenuUID
                    '================ Sales Invoicet menu ================		
                    Case "OINVMEC"
                        objSalesInvoice.MenuEvent(pVal, BubbleEvent)

                    Case "1282"
                        objSalesInvoice.MenuEvent(pVal, BubbleEvent)


                        ''================ Quality Module ================
                        'Case "2050"
                        '    objProdOrder.menuEvent(pVal, BubbleEvent)
                        '    'Paramaeter Master


                End Select
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try
    End Sub
    Private Sub oApplication_RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean) Handles oApplication.RightClickEvent
        Try
            Dim oForm As SAPbouiCOM.Form = oApplication.Forms.ActiveForm
            Select Case oForm.TypeEx

                ''================ Quality Module ================
                'Case "IK_QLTP"
                '    objQualityPlan.RightClickEvent(eventInfo, BubbleEvent)

                Case Else
                    If oApplication.Menus.Exists("Close") = True Then oApplication.Menus.RemoveEx("Close")

            End Select
        Catch ex As Exception
            oApplication.SetStatusBarMessage(ex.Message)
        End Try
    End Sub

#End Region

End Module
