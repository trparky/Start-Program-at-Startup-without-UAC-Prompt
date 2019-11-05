Public Class Popout_Description
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub

    Private Sub Popout_Description_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.popoutDescriptionSize = Me.Size
    End Sub

    Private Sub Popout_Description_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPopoutDescription.SelectionStart = txtPopoutDescription.Text.Length
    End Sub
End Class