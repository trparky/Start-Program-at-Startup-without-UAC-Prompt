﻿Imports System.Runtime.CompilerServices
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

    ''' <summary>This function uses an IndexOf call to do a case-insensitive search. This function operates a lot like Contains().</summary>
    ''' <param name="needle">The String containing what you want to search for.</param>
    ''' <return>Returns a Boolean value.</return>
    <Extension()>
    Public Function caseInsensitiveContains(haystack As String, needle As String) As Boolean
        Dim index As Integer = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase)
        Return If(index = -1, False, True)
    End Function

    ''' <summary>This function uses a RegEx search to do a case-insensitive search but with more power and more responsibilities. This function operates a lot like Contains().</summary>
    ''' <param name="needle">The String containing what you want to search for.</param>
    ''' <param name="boolDoEscaping">This tells the function if it should add slashes where appropriate to the "needle" String.</param>
    ''' <return>Returns a Boolean value.</return>
    <Extension()>
    Public Function caseInsensitiveContains(haystack As String, needle As String, Optional boolDoEscaping As Boolean = False) As Boolean
        Try
            If boolDoEscaping Then needle = Regex.Escape(needle)
            Return Regex.IsMatch(haystack, needle, RegexOptions.IgnoreCase)
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>Checks to see if a Process ID or PID exists on the system.</summary>
    ''' <param name="PID">The PID of the process you are checking the existance of.</param>
    ''' <param name="processObject">If the PID does exist, the function writes back to this argument in a ByRef way a Process Object that can be interacted with outside of this function.</param>
    ''' <returns>Return a Boolean value. If the PID exists, it return a True value. If the PID doesn't exist, it returns a False value.</returns>
    Private Function DoesProcessIDExist(ByVal PID As Integer, ByRef processObject As Process) As Boolean
        Try
            processObject = Process.GetProcessById(PID)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub KillProcess(processID As Integer)
        Dim processObject As Process = Nothing

        ' First we are going to check if the Process ID exists.
        If doesProcessIDExist(processID, processObject) Then
            Try
                processObject.Kill() ' Yes, it does so let's kill it.
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.

        '' Now we are going to check again if the Process ID exists and if it does, we're going to attempt to kill it again.
        If doesProcessIDExist(processID, processObject) Then
            Try
                processObject.Kill()
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        Threading.Thread.Sleep(250) ' We're going to sleep (again) to give the system some time to kill the process.
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