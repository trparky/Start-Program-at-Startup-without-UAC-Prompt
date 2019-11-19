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

    ''' <summary>This function operates a lot like Replace() but is case-InSeNsItIvE.</summary>
    ''' <param name="source">The source String, aka the String where the data will be replaced in.</param>
    ''' <param name="replace">What you want to replace in the String.</param>
    ''' <param name="replaceWith">What you want to replace with in the String.</param>
    ''' <param name="boolEscape">This is an optional parameter, the default is True. This parameter gives you far more control over how the function works. With this parameter set to True the function automatically properly escapes the "replace" parameter for use in the RegEx replace function that operates inside this function. If this parameter is set to False it is up to you, the programmer, to properly escape the value of the "replace" parameter or this function will throw an exception.</param>
    ''' <return>Returns a String value.</return>
    <Extension()>
    Public Function caseInsensitiveReplace(source As String, replace As String, replaceWith As String, Optional boolEscape As Boolean = True) As String
        If boolEscape Then replace = Regex.Escape(replace)
        Return Regex.Replace(source, replace, replaceWith, RegexOptions.IgnoreCase)
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
            Using searcher As New Management.ManagementObjectSearcher("root\CIMV2", String.Format("Select * FROM Win32_Process WHERE ProcessId={0}", PID))
                Return If(searcher.Get.Count = 0, False, True)
            End Using
        Catch ex3 As Runtime.InteropServices.COMException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub killProcess(PID As Integer)
        If doesPIDExist(PID) Then Process.GetProcessById(PID).Kill()
        If doesPIDExist(PID) Then killProcess(PID)
    End Sub

    Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
        Dim fullFileName As String = If(boolFullFilePathPassed, strFileName, New IO.FileInfo(strFileName).FullName)
        Dim wmiQuery As String = String.Format("Select ExecutablePath, ProcessId FROM Win32_Process WHERE ExecutablePath = '{0}'", fullFileName.addSlashes())

        Try
            Using searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)
                For Each queryObj As Management.ManagementObject In searcher.Get()
                    killProcess(Integer.Parse(queryObj("ProcessId").ToString))
                Next
            End Using
        Catch err As Exception
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
End Module