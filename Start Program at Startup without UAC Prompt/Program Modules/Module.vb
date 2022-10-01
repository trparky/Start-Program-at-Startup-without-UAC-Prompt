Imports System.Runtime.CompilerServices
Imports System.Security.Principal
Imports System.Text.RegularExpressions

Module Globals
    Public Const DoubleCRLF As String = vbCrLf & vbCrLf

    Public Function areWeAnAdministrator() As Boolean
        Try
            Dim principal As New WindowsPrincipal(WindowsIdentity.GetCurrent())
            Return principal.IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module