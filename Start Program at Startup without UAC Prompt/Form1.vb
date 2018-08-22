Imports Microsoft.Win32.TaskScheduler
Imports System.Text
Imports IWshRuntimeLibrary
Imports System.IO
Imports System.Xml.Serialization

Public Class Form1
    Private Const Tabs As String = vbTab
    Private Const strTaskFolderName As String = "Run at User Logon with Administrator Privileges"

    Private Sub newFileDeleterThreadSub()
        searchForProcessAndKillIt(Application.ExecutablePath & ".new.exe", False)
        IO.File.Delete(Application.ExecutablePath & ".new.exe")
    End Sub

    Private Function verifyWindowLocation(point As Point) As Point
        Return If(point.X < 0 Or point.Y < 0, New Point(0, 0), point)
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = verifyWindowLocation(My.Settings.mainWindowPosition)
        chkUseSSL.Checked = My.Settings.boolUseSSL
        imgLock.Image = If(chkUseSSL.Checked, My.Resources.locked, My.Resources.unlocked)

        If IO.File.Exists(Application.ExecutablePath & ".new.exe") Then Threading.ThreadPool.QueueUserWorkItem(AddressOf newFileDeleterThreadSub)

        Using taskService As New TaskService
            Dim boolDoesOurFolderExist As Boolean = False

            For Each folder As TaskFolder In taskService.RootFolder.SubFolders
                If folder.Name.Equals(strTaskFolderName, StringComparison.OrdinalIgnoreCase) Then
                    boolDoesOurFolderExist = True
                    Exit For
                End If
            Next

            If Not boolDoesOurFolderExist Then taskService.RootFolder.CreateFolder(strTaskFolderName)
        End Using

        refreshTasks()
    End Sub

    Private Sub createShortCut(ByVal locationOfShortcut As String, pathToExecutable As String, iconPath As String, ByVal Title As String, Optional arguments As String = Nothing)
        Try
            Dim WshShell As New WshShell
            ' short cut files have a .lnk extension
            Dim shortCut As IWshShortcut = DirectCast(WshShell.CreateShortcut(locationOfShortcut), IWshShortcut)

            ' set the shortcut properties
            With shortCut
                .TargetPath = pathToExecutable

                If Not String.IsNullOrEmpty(arguments) Then .Arguments = arguments

                .WindowStyle = 1I
                .Description = Title
                .IconLocation = iconPath & ", 0"
                .WindowStyle = 7
                .Save() ' save the shortcut file
            End With
        Catch ex As Exception
        End Try
    End Sub

    Sub checkTaskPrioritySettings(ByRef task As Task)
        Try
            If task.Definition.Settings.Priority <> ProcessPriorityClass.Normal Then
                task.Definition.Settings.Priority = ProcessPriorityClass.Normal
                task.RegisterChanges()
            End If
        Catch ex As Exception
            ' We don't care if we crash here but we need to do it silently.
        End Try
    End Sub

    Sub refreshTasks()
        listTasks.Items.Clear()
        Dim taskService As New TaskService

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            checkTaskPrioritySettings(task)
            listTasks.Items.Add(task.Name)
        Next

        taskService.Dispose()
        taskService = Nothing
        disableButtons()
    End Sub

    Sub disableButtons()
        btnDeleteTasks.Enabled = False
        btnEditTask.Enabled = False
        btnCreateShortcutOnDesktop.Enabled = False
        btnExportTask.Enabled = False
        btnStopStartTask.Enabled = False
        btnGetTaskStatus.Enabled = False

        DeleteTaskToolStripMenuItem.Enabled = False
        EditTaskToolStripMenuItem.Enabled = False
        StopStartTaskToolStripMenuItem.Enabled = False
        GetStatusOfTaskToolStripMenuItem.Enabled = False
        CreateShortcutToTaskOnDesktopToolStripMenuItem.Enabled = False
        StopStartTaskToolStripMenuItem.Enabled = False
        ExportTaskToolStripMenuItem.Enabled = False
        ImportTaskToolStripMenuItem.Enabled = False
    End Sub

    Sub enableButtons()
        btnDeleteTasks.Enabled = True
        btnEditTask.Enabled = True
        btnCreateShortcutOnDesktop.Enabled = True
        btnExportTask.Enabled = True
        btnStopStartTask.Enabled = True
        btnGetTaskStatus.Enabled = True

        DeleteTaskToolStripMenuItem.Enabled = True
        EditTaskToolStripMenuItem.Enabled = True
        StopStartTaskToolStripMenuItem.Enabled = True
        GetStatusOfTaskToolStripMenuItem.Enabled = True
        CreateShortcutToTaskOnDesktopToolStripMenuItem.Enabled = True
        StopStartTaskToolStripMenuItem.Enabled = True
        ExportTaskToolStripMenuItem.Enabled = True
        ImportTaskToolStripMenuItem.Enabled = True
    End Sub

    Private Sub listTasks_Click(sender As Object, e As EventArgs) Handles listTasks.Click
        If listTasks.SelectedItems.Count > 0 Then
            enableButtons()

            If boolIsTaskRunning(listTasks.Text) Then
                btnStopStartTask.Text = "Stop Task"
                StopStartTaskToolStripMenuItem.Text = "&Stop Task"
                btnStopStartTask.Image = My.Resources.stop_sign
                StopStartTaskToolStripMenuItem.Image = My.Resources.stop_sign
            Else
                btnStopStartTask.Text = "Start Task"
                StopStartTaskToolStripMenuItem.Text = "&Start Task"
                btnStopStartTask.Image = My.Resources.start
                StopStartTaskToolStripMenuItem.Image = My.Resources.start
            End If
        End If
    End Sub

    Private Sub btnBrowseForExecutable_Click(sender As Object, e As EventArgs) Handles btnBrowseForExecutable.Click
        OpenFileDialog1.Title = "Select a program to run with Windows Scheduler"
        OpenFileDialog1.Filter = "Program|*.exe"
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        txtEXEPath.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub btnCreateTask_Click(sender As Object, e As EventArgs) Handles btnCreateTask.Click
        If String.IsNullOrEmpty(txtTaskName.Text.Trim) Then
            MsgBox("You must provide a name for this task.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If
        If String.IsNullOrEmpty(txtEXEPath.Text.Trim) Then
            MsgBox("You must provide a path to an executable for this task.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If
        If Not IO.File.Exists(txtEXEPath.Text) Then
            MsgBox("Executable Path Not Found.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        If btnCreateTask.Text.Equals("Save Changes to Task", StringComparison.OrdinalIgnoreCase) Then deleteTask(txtTaskName.Text)

        addTask(txtTaskName.Text, txtDescription.Text, txtEXEPath.Text, txtParameters.Text)

        Dim strPathToAutoShortcut As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Start " & txtTaskName.Text & ".lnk")

        If btnCreateTask.Text.Equals("Save Changes to Task", StringComparison.OrdinalIgnoreCase) Then
            If chkEnabled.Checked Then : MsgBox("Task changes saved.", MsgBoxStyle.Information, Me.Text)
            Else
                If Not IO.File.Exists(strPathToAutoShortcut) Then
                    autoCreateDesktopShortcut(txtTaskName.Text, strPathToAutoShortcut)
                    MsgBox("Task changes saved." & vbCrLf & vbCrLf & "User Logon Startup is disabled so a shortcut to run it has been created on your desktop.", MsgBoxStyle.Information, Me.Text)
                Else : MsgBox("Task changes saved.", MsgBoxStyle.Information, Me.Text)
                End If
            End If

            btnCreateTask.Text = "Create Task"
        ElseIf btnCreateTask.Text.Equals("Create Task", StringComparison.OrdinalIgnoreCase) Then
            If chkEnabled.Checked Then : MsgBox("New task saved.", MsgBoxStyle.Information, Me.Text)
            Else
                autoCreateDesktopShortcut(txtTaskName.Text, strPathToAutoShortcut)
                MsgBox("New task saved." & vbCrLf & vbCrLf & "User Logon Startup is disabled so a shortcut to run it has been created on your desktop.", MsgBoxStyle.Information, Me.Text)
            End If
        End If

        txtTaskName.Text = Nothing
        txtEXEPath.Text = Nothing
        txtParameters.Text = Nothing
        txtDescription.Text = Nothing
        refreshTasks()
    End Sub

    Private Sub autoCreateDesktopShortcut(strTaskName As String, strPathToAutoShortcut As String)
        Dim exePath As String = Nothing
        If Not isThisAValidExecutableTask(strTaskName, exePath) Then Exit Sub
        If Not IO.File.Exists(strPathToAutoShortcut) Then
            createShortCut(strPathToAutoShortcut, "schtasks", exePath, strTaskName, String.Format("/run /TN {0}\{2}\{1}{0}", Chr(34), strTaskName, strTaskFolderName))
        End If
    End Sub

    Private Function getTaskObject(ByRef taskServiceObject As TaskService, ByVal nameOfTask As String, ByRef taskObject As Task) As Boolean
        Try
            taskObject = taskServiceObject.GetTask(strTaskFolderName & "\" & nameOfTask)
            Return If(taskObject Is Nothing, False, True)
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>Deletes a scheduled task.</summary>
    ''' <param name="taskName">The task name to be deleted.</param>
    Private Sub deleteTask(taskName As String)
        Try
            Using taskServiceObject As TaskService = New TaskService()
                taskServiceObject.RootFolder.SubFolders(strTaskFolderName).DeleteTask(taskName, False)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnEditTask_Click(sender As Object, e As EventArgs) Handles btnEditTask.Click
        Using taskService As New TaskService
            Dim actions As ActionCollection
            Dim execAction As ExecAction
            Dim task As Task = Nothing

            If getTaskObject(taskService, listTasks.Text, task) Then
                actions = task.Definition.Actions
                txtTaskName.Text = task.Name
                txtDescription.Text = task.Definition.RegistrationInfo.Description
                chkEnabled.Checked = False

                For Each trigger As Trigger In task.Definition.Triggers
                    If trigger.Enabled And trigger.TriggerType = TaskTriggerType.Logon Then
                        chkEnabled.Checked = True
                    End If
                Next

                For Each action As Action In actions
                    If action.ActionType = TaskActionType.Execute Then
                        execAction = DirectCast(action, ExecAction)

                        txtEXEPath.Text = execAction.Path.Replace("""", "")
                        txtParameters.Text = execAction.Arguments

                        Exit For
                    End If
                Next

                task.Dispose()
            End If

            btnCreateTask.Text = "Save Changes to Task"
            btnCancelEditTask.Enabled = True
            txtTaskName.ReadOnly = True
            ToolTip1.SetToolTip(txtTaskName, "Disabled in Edit Mode")
        End Using
    End Sub

    Private Sub btnCancelEditTask_Click(sender As Object, e As EventArgs) Handles btnCancelEditTask.Click
        txtDescription.Text = Nothing
        txtEXEPath.Text = Nothing
        txtParameters.Text = Nothing
        txtTaskName.Text = Nothing

        btnCreateTask.Text = "Create Task"
        btnCancelEditTask.Enabled = False
        txtTaskName.ReadOnly = False
        ToolTip1.SetToolTip(txtTaskName, "")
        refreshTasks()
        disableButtons()
    End Sub

    Private Sub btnDeleteTasks_Click(sender As Object, e As EventArgs) Handles btnDeleteTasks.Click
        deleteTask(listTasks.Text)
        refreshTasks()
        MsgBox("Task Deleted.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub btnCheckForUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        windowObject = Me
        Threading.ThreadPool.QueueUserWorkItem(AddressOf checkForUpdates)
        btnCheckForUpdates.Enabled = False
    End Sub

    Function isThisAValidExecutableTask(ByVal strTaskName As String, ByRef exePath As String) As Boolean
        Using taskService As New TaskService
            Dim task As Task = Nothing

            If getTaskObject(taskService, strTaskName, task) Then
                Dim actions As ActionCollection = task.Definition.Actions

                For Each action As Action In actions
                    If action.ActionType = TaskActionType.Execute Then
                        exePath = DirectCast(action, ExecAction).Path.Replace("""", "")
                        Return True
                    End If
                Next
            Else : Return False
            End If
        End Using

        Return False
    End Function

    Function isThisAValidExecutableTask(ByRef exePath As String) As Boolean
        Return isThisAValidExecutableTask(listTasks.Text, exePath)
    End Function

    Private Sub btnCreateShortcutOnDesktop_Click(sender As Object, e As EventArgs) Handles btnCreateShortcutOnDesktop.Click
        Dim exePath As String = Nothing
        If Not isThisAValidExecutableTask(exePath) Then
            MsgBox("Something went wrong.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        SaveFileDialog1.Title = "Save Shortcut To..."
        SaveFileDialog1.FileName = listTasks.Text & ".lnk"
        SaveFileDialog1.Filter = "Shortcut|*.lnk"

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim fileInfo As New FileInfo(SaveFileDialog1.FileName)
            Dim locationOfShortcut As String = Path.Combine(fileInfo.DirectoryName, listTasks.Text & ".lnk")
            fileInfo = Nothing

            createShortCut(locationOfShortcut, "schtasks", exePath, listTasks.Text, String.Format("/run /TN {0}\{2}\{1}{0}", Chr(34), listTasks.Text, strTaskFolderName))
            locationOfShortcut = Nothing
            exePath = Nothing

            MsgBox("Shortcut Created Successfully.", MsgBoxStyle.Information, "Create Shortcut to Task")
        End If
    End Sub

    Private Sub btnExportTask_Click(sender As Object, e As EventArgs) Handles btnExportTask.Click
        saveTask.Title = "Save as Task File"
        saveTask.Filter = "Task File|*.task"

        If saveTask.ShowDialog() = DialogResult.OK Then
            Using taskService As New TaskService
                Dim task As Task = Nothing

                If getTaskObject(taskService, listTasks.Text, task) Then
                    Dim savedTask As New classTask()
                    Dim actions As ActionCollection = task.Definition.Actions
                    Dim execAction As ExecAction

                    savedTask.taskName = task.Name
                    savedTask.taskDescription = task.Definition.RegistrationInfo.Description

                    For Each action As Action In actions
                        If action.ActionType = TaskActionType.Execute Then
                            execAction = DirectCast(action, ExecAction)

                            savedTask.taskEXE = execAction.Path.Replace("""", "")
                            savedTask.taskParameters = execAction.Arguments

                            action.Dispose()
                            execAction.Dispose()

                            execAction = Nothing
                            action = Nothing

                            Exit For
                        End If
                    Next

                    actions.Dispose()
                    actions = Nothing

                    Using streamWriter As New StreamWriter(saveTask.FileName)
                        Dim xmlSerializerObject As New XmlSerializer(savedTask.GetType)
                        xmlSerializerObject.Serialize(streamWriter, savedTask)
                    End Using

                    MsgBox("Task exported.", MsgBoxStyle.Information, Me.Text)
                End If
            End Using
        End If
    End Sub

    Private Sub btnImportTask_Click(sender As Object, e As EventArgs) Handles btnImportTask.Click
        importTask.Title = "Import Task from Task File"
        importTask.FileName = Nothing
        importTask.Filter = "Task File|*.task"

        If importTask.ShowDialog() = DialogResult.OK Then
            Dim savedTask As New classTask()

            Using streamReader As New StreamReader(importTask.FileName)
                Dim xmlSerializerObject As New XmlSerializer(savedTask.GetType)
                savedTask = xmlSerializerObject.Deserialize(streamReader)
            End Using

            addTask(savedTask.taskName, savedTask.taskDescription, savedTask.taskEXE, savedTask.taskParameters)

            savedTask = Nothing
            refreshTasks()

            MsgBox("Task imported successfully.", MsgBoxStyle.Information, Me.Text)
        End If
    End Sub

    Sub addTask(taskName As String, taskDescription As String, taskEXEPath As String, taskParameters As String)
        taskName = taskName.Trim
        taskDescription = taskDescription.Trim
        taskEXEPath = taskEXEPath.Trim

        If Not String.IsNullOrEmpty(taskParameters) Then taskParameters = taskParameters.Trim

        If Not IO.File.Exists(taskEXEPath) Then
            MsgBox("Executable path not found.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        Using taskService As TaskService = New TaskService()
            Dim newTask As TaskDefinition = taskService.NewTask

            newTask.RegistrationInfo.Description = taskDescription

            If chkEnabled.Checked Then newTask.Triggers.Add(New LogonTrigger)
            Dim exeFileInfo As New FileInfo(taskEXEPath)

            With newTask
                If taskParameters Is Nothing Then
                    .Actions.Add(New ExecAction(Chr(34) & taskEXEPath & Chr(34), Nothing, exeFileInfo.DirectoryName))
                Else
                    .Actions.Add(New ExecAction(Chr(34) & taskEXEPath & Chr(34), taskParameters, exeFileInfo.DirectoryName))
                End If

                .Principal.RunLevel = TaskRunLevel.Highest
                .Settings.Compatibility = TaskCompatibility.V2
                .Settings.AllowDemandStart = True
                .Settings.DisallowStartIfOnBatteries = False
                .Settings.RunOnlyIfIdle = False
                .Settings.StopIfGoingOnBatteries = False
                .Settings.AllowHardTerminate = False
                .Settings.UseUnifiedSchedulingEngine = True
                .Settings.ExecutionTimeLimit = Nothing
                .Settings.Priority = ProcessPriorityClass.Normal
                .Principal.LogonType = TaskLogonType.InteractiveToken
            End With

            taskService.RootFolder.SubFolders(strTaskFolderName).RegisterTaskDefinition(taskName, newTask)

            newTask.Dispose()
            newTask = Nothing
        End Using
    End Sub

    Private Sub btnGetTaskStatus_Click(sender As Object, e As EventArgs) Handles btnGetTaskStatus.Click
        If boolIsTaskRunning(listTasks.Text) Then
            MsgBox("The task is running.", MsgBoxStyle.Information, Me.Text)
        Else
            MsgBox("The task is NOT running.", MsgBoxStyle.Information, Me.Text)
        End If
    End Sub

    Private Sub DeleteTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTaskToolStripMenuItem.Click
        btnDeleteTasks.PerformClick()
    End Sub

    Private Sub EditTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditTaskToolStripMenuItem.Click
        btnEditTask.PerformClick()
    End Sub

    Private Sub GetStatusOfTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetStatusOfTaskToolStripMenuItem.Click
        btnGetTaskStatus.PerformClick()
    End Sub

    Private Sub StopStartTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopStartTaskToolStripMenuItem.Click
        btnStopStartTask.PerformClick()
    End Sub

    Private Sub CreateShortcutToTaskOnDesktopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateShortcutToTaskOnDesktopToolStripMenuItem.Click
        btnCreateShortcutOnDesktop.PerformClick()
    End Sub

    Private Sub ExportTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTaskToolStripMenuItem.Click
        btnExportTask.PerformClick()
    End Sub

    Private Sub ImportTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportTaskToolStripMenuItem.Click
        btnImportTask.Enabled = True
    End Sub

    Private Sub chkUseSSL_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseSSL.CheckedChanged
        imgLock.Image = If(chkUseSSL.Checked, My.Resources.locked, My.Resources.unlocked)
        My.Settings.boolUseSSL = chkUseSSL.Checked
    End Sub

    Private Sub btnStopStartTask_Click(sender As Object, e As EventArgs) Handles btnStopStartTask.Click
        Using taskService As New TaskService
            Dim taskObject As Task = Nothing
            Dim boolIsItRunning As Boolean = False

            boolIsItRunning = boolIsTaskRunning(listTasks.Text, taskService, taskObject)

            If taskObject IsNot Nothing Then
                If boolIsItRunning Then
                    taskObject.Stop()
                    btnStopStartTask.Image = My.Resources.start
                Else
                    taskObject.Run()
                    btnStopStartTask.Image = My.Resources.stop_sign
                End If

                taskObject.Dispose()
            End If
        End Using
    End Sub

    Function boolIsTaskRunning(ByVal strTaskName As String, ByRef taskService As TaskService, ByRef taskObject As Task) As Boolean
        Return If(getTaskObject(taskService, strTaskName, taskObject), If(taskObject.State = TaskState.Running, True, False), False)
    End Function

    Function boolIsTaskRunning(strTaskName As String) As Boolean
        Using taskService As New TaskService
            Dim task As Task = Nothing
            Return If(getTaskObject(taskService, strTaskName, task), If(task.State = TaskState.Running, True, False), False)
        End Using
    End Function

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        Dim stringBuilder As New StringBuilder
        stringBuilder.AppendLine(programName)
        stringBuilder.AppendLine("Version " & strFullVersionString)
        stringBuilder.AppendLine("Written by Tom Parkison.")

        MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, "About " & programName)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.mainWindowPosition = Me.Location
    End Sub
End Class