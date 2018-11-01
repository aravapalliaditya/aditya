Imports System.Text
Imports System.IO
Imports System.IO.Stream
Imports System.FileStyleUriParser


Friend Class PrintPreview
    Public displayText As String

    Public Sub New(ByVal DisplayStr As String)
        Try
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            displayText = DisplayStr
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub PrintPreview_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            txtPreview.Text = displayText
            Label1.Location = New Point(txtPreview.Left + 10, txtPreview.Height - 28 + txtPreview.Top + 35)
            TxtPath.Location = New Point(Label1.Left + 60, Label1.Height + 5 + Label1.Top - 25)
            BtnSave.Location = New Point(TxtPath.Width + Label1.Width + 20, TxtPath.Height + 5 + TxtPath.Top - 27)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSave.Click

        Try
            Dim Response As String = MsgBox("Do u want to save " & (TxtPath.Text).ToString() & " (" & DateTime.Today.ToString("dd-MM-yyyy") + ").txt file?", MsgBoxStyle.YesNo, "Verify")
            If Response = vbYes Then
                If txtPreview.Text.Equals("") = False Then

                    Dim objFileStream As FileStream
                    Dim objStreamWriter As StreamWriter
                    Dim filename As String = (TxtPath.Text).ToString() & " (" & DateTime.Today.ToString("dd-MM-yyyy") + ").txt"
                    Dim objStringBuilder As StringBuilder = New StringBuilder()
                    'Append the message
                    objStringBuilder.AppendLine(txtPreview.Text)
                    ' objStringBuilder.AppendFormat("{0}{1}", txtPreview.Text, Environment.NewLine)
                    If Directory.Exists(Path.GetDirectoryName(filename)).Equals(filename) = False Then
                        Directory.CreateDirectory(Path.GetDirectoryName(filename))
                    End If
                    If File.Exists(filename) = True Then
                        objFileStream = File.Open(filename, FileMode.Append, FileAccess.Write)
                    Else
                        objFileStream = File.Create(filename)
                    End If
                    objStreamWriter = New StreamWriter(objFileStream)
                    objStreamWriter.Write(objStringBuilder.ToString())
                    objStreamWriter.Close()
                    MsgBox("File  " & filename & "  is Successfully Saved.")
                Else
                    MsgBox("There is no information to Save.")
                End If
            ElseIf Response = vbNo Then
                MsgBox("File not Saved.")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

   
End Class