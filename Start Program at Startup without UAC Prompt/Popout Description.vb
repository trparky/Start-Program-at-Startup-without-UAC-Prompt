Public Class Popout_Description
    Public Enum userResponseEnum As Short
        save
        dontSave
        null
    End Enum

    Public userResponse As userResponseEnum = userResponseEnum.null
    Private boolTextChanged As Boolean

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        userResponse = userResponseEnum.save
        Close()
    End Sub

    Private Sub Popout_Description_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.popoutDescriptionSize = Size
    End Sub

    Private Sub Popout_Description_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPopoutDescription.SelectionStart = txtPopoutDescription.Text.Length
        boolTextChanged = False
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        userResponse = userResponseEnum.dontSave
        Close()
    End Sub

    Private Sub Popout_Description_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If userResponse = userResponseEnum.null And boolTextChanged Then
            If MsgBox("Do you want to save your changes?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Save Changes?") = MsgBoxResult.Yes Then
                userResponse = userResponseEnum.save
            Else
                userResponse = userResponseEnum.dontSave
            End If
        End If
    End Sub

    Private Sub txtPopoutDescription_TextChanged(sender As Object, e As EventArgs) Handles txtPopoutDescription.TextChanged
        boolTextChanged = True
    End Sub
End Class