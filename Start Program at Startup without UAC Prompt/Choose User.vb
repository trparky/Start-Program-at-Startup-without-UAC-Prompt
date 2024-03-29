﻿Public Class Choose_User
    Public Property strSelectedUser As String

    Private Sub Choose_User_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Using usersSearcher As New Management.ManagementObjectSearcher("SELECT LocalAccount, Disabled, Lockout, SIDType, Name, Caption FROM Win32_UserAccount")
                For Each user As Management.ManagementObject In usersSearcher.Get()
                    If CBool(user("LocalAccount")) AndAlso Not CBool(user("Disabled")) AndAlso Not CBool(user("Lockout")) AndAlso Integer.Parse(user("SIDType").ToString()) = 1 AndAlso Not user("Name").ToString.Equals("HomeGroupUser$", StringComparison.OrdinalIgnoreCase) Then
                        listUsers.Items.Add(user("Caption").ToString)
                    End If
                Next
            End Using
        Catch ex As Exception
            MsgBox("Unable to enumerate users on current system.", MsgBoxStyle.Critical, "Choose User...")
            Close()
        End Try
    End Sub

    Private Sub listUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listUsers.SelectedIndexChanged
        btnChooseUser.Enabled = listUsers.SelectedItems.Count <> 0
    End Sub

    Private Sub btnChooseUser_Click(sender As Object, e As EventArgs) Handles btnChooseUser.Click
        strSelectedUser = listUsers.SelectedItems(0).Text
        Close()
    End Sub

    Private Sub listUsers_KeyUp(sender As Object, e As KeyEventArgs) Handles listUsers.KeyUp
        If listUsers.SelectedItems.Count = 1 AndAlso e.KeyCode = Keys.Enter Then btnChooseUser.PerformClick()
    End Sub
End Class