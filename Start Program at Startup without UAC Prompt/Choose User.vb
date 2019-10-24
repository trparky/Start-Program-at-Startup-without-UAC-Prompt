Public Class Choose_User
    Public Property strSelectedUser As String

    Private Sub Choose_User_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Using usersSearcher As New Management.ManagementObjectSearcher("SELECT Caption FROM Win32_UserAccount")
            For Each user As Management.ManagementObject In usersSearcher.Get()
                listUsers.Items.Add(user("Caption").ToString)
            Next
        End Using
    End Sub

    Private Sub listUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listUsers.SelectedIndexChanged
        btnChooseUser.Enabled = If(listUsers.SelectedItems.Count = 0, False, True)
    End Sub

    Private Sub btnChooseUser_Click(sender As Object, e As EventArgs) Handles btnChooseUser.Click
        strSelectedUser = listUsers.SelectedItems(0).Text
        Me.Close()
    End Sub
End Class