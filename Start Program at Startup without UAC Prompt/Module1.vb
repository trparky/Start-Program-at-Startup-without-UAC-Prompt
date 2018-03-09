Imports System.Runtime.CompilerServices
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.Text.RegularExpressions

Module Module1
    ' PHP like addSlashes and stripSlashes. Call using String.addSlashes() and String.stripSlashes().
    <Extension()>
    Public Function addSlashes(unsafeString As String) As String
        Return Regex.Replace(unsafeString, "([\000\010\011\012\015\032\042\047\134\140])", "\$1")
    End Function

    <Extension()>
    Public Function caseInsensitiveContains(haystack As String, needle As String, Optional boolDoEscaping As Boolean = False) As Boolean
        Try
            If boolDoEscaping Then needle = Regex.Escape(needle)
            Return Regex.IsMatch(haystack, needle, RegexOptions.IgnoreCase)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function doesPIDExist(PID As Integer) As Boolean
        Try
            Dim searcher As New Management.ManagementObjectSearcher("root\CIMV2", String.Format("Select * FROM Win32_Process WHERE ProcessId={0}", PID))

            If searcher.Get.Count = 0 Then
                searcher.Dispose()
                Return False
            Else
                searcher.Dispose()
                Return True
            End If
        Catch ex3 As Runtime.InteropServices.COMException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub killProcess(PID As Integer)
        Debug.Write(String.Format("Killing PID {0}...", PID))

        If doesPIDExist(PID) Then
            Process.GetProcessById(PID).Kill()
        End If

        If doesPIDExist(PID) Then
            killProcess(PID)
        End If
    End Sub

    Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
        Dim fullFileName As String = If(boolFullFilePathPassed, strFileName, New IO.FileInfo(strFileName).FullName)

        Dim wmiQuery As String = String.Format("Select ExecutablePath, ProcessId FROM Win32_Process WHERE ExecutablePath = '{0}'", fullFileName.addSlashes())
        Dim searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)

        Try
            For Each queryObj As Management.ManagementObject In searcher.Get()
                killProcess(Integer.Parse(queryObj("ProcessId").ToString))
            Next
        Catch ex3 As Runtime.InteropServices.COMException
        Catch err As Management.ManagementException
        End Try
    End Sub

    Public Function areWeAnAdministrator() As Boolean
        Try
            Dim principal As WindowsPrincipal = New WindowsPrincipal(WindowsIdentity.GetCurrent())
            Return principal.IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function canIWriteToTheCurrentDirectory() As Boolean
        Return canIWriteThere(New IO.FileInfo(Application.ExecutablePath).DirectoryName)
    End Function

    Private Function canIWriteThere(folderPath As String) As Boolean
        ' We make sure we get valid folder path by taking off the leading slash.
        If folderPath.EndsWith("\") Then folderPath = folderPath.Substring(0, folderPath.Length - 1)
        If String.IsNullOrEmpty(folderPath) Or Not IO.Directory.Exists(folderPath) Then Return False

        If checkByFolderACLs(folderPath) Then
            Try
                Dim strOurTestFileName As String = IO.Path.Combine(folderPath, "test" & randomStringGenerator(10) & ".txt")

                IO.File.Create(strOurTestFileName, 1, IO.FileOptions.DeleteOnClose).Close()
                If IO.File.Exists(strOurTestFileName) Then IO.File.Delete(strOurTestFileName)

                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Private Function checkByFolderACLs(folderPath As String) As Boolean
        If WindowsIdentity.GetCurrent().IsSystem Then Return True

        Try
            Dim dsDirectoryACLs As DirectorySecurity = IO.Directory.GetAccessControl(folderPath)
            Dim strCurrentUserSDDL As String = WindowsIdentity.GetCurrent.User.Value
            Dim ircCurrentUserGroups As IdentityReferenceCollection = WindowsIdentity.GetCurrent.Groups

            Dim arcAuthorizationRules As AuthorizationRuleCollection = dsDirectoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
            Dim fsarDirectoryAccessRights As FileSystemAccessRule

            For Each arAccessRule As AuthorizationRule In arcAuthorizationRules
                If arAccessRule.IdentityReference.Value.Equals(strCurrentUserSDDL, StringComparison.OrdinalIgnoreCase) Or ircCurrentUserGroups.Contains(arAccessRule.IdentityReference) Then
                    fsarDirectoryAccessRights = DirectCast(arAccessRule, FileSystemAccessRule)

                    If fsarDirectoryAccessRights.AccessControlType = AccessControlType.Allow Then
                        If fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.Modify Or fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.WriteData Or fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.FullControl Then
                            Return True
                        End If
                    End If
                End If
            Next

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function randomStringGenerator(length As Integer) As String
        Dim random As Random = New Random()
        Dim builder As New Text.StringBuilder()
        Dim ch As Char
        Dim legalCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890"

        For cntr As Integer = 0 To length
            ch = legalCharacters.Substring(random.Next(0, legalCharacters.Length), 1)
            builder.Append(ch)
        Next

        Return builder.ToString()
    End Function
End Module