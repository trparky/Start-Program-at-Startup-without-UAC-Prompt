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

        If chkUseSSL.Checked Then
            imgLock.Image = My.Resources.locked
        Else
            imgLock.Image = My.Resources.unlocked
        End If

        If IO.File.Exists(Application.ExecutablePath & ".new.exe") Then
            Threading.ThreadPool.QueueUserWorkItem(AddressOf newFileDeleterThreadSub)
        End If

        Dim taskService As New TaskService
        Dim boolDoesOurFolderExist As Boolean = False

        For Each folder As TaskFolder In taskService.RootFolder.SubFolders
            If folder.Name.Equals(strTaskFolderName, StringComparison.OrdinalIgnoreCase) Then
                boolDoesOurFolderExist = True
                Exit For
            End If
        Next

        If Not boolDoesOurFolderExist Then taskService.RootFolder.CreateFolder(strTaskFolderName)

        taskService.Dispose()
        taskService = Nothing

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

    Sub refreshTasks()
        listTasks.Items.Clear()
        Dim taskService As New TaskService

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            listTasks.Items.Add(task.Name)
        Next

        taskService.Dispose()
        taskService = Nothing
    End Sub

    Private Sub listTasks_Click(sender As Object, e As EventArgs) Handles listTasks.Click
        If listTasks.SelectedItems.Count > 0 Then
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

        addTask(txtTaskName.Text, txtDescription.Text, txtEXEPath.Text, txtParameters.Text)

        Dim strPathToAutoShortcut As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Start " & txtTaskName.Text & ".lnk")

        If btnCreateTask.Text.Equals("Save Changes to Task", StringComparison.OrdinalIgnoreCase) Then
            If chkEnabled.Checked Then
                MsgBox("Task changes saved.", MsgBoxStyle.Information, Me.Text)
            Else
                If Not IO.File.Exists(strPathToAutoShortcut) Then
                    autoCreateDesktopShortcut(txtTaskName.Text, strPathToAutoShortcut)
                    MsgBox("Task changes saved." & vbCrLf & vbCrLf & "User Logon Startup is disabled so a shortcut to run it has been created on your desktop.", MsgBoxStyle.Information, Me.Text)
                Else
                    MsgBox("Task changes saved.", MsgBoxStyle.Information, Me.Text)
                End If
            End If

            btnCreateTask.Text = "Create Task"
        ElseIf btnCreateTask.Text.Equals("Create Task", StringComparison.OrdinalIgnoreCase) Then
            If chkEnabled.Checked Then
                MsgBox("New task saved.", MsgBoxStyle.Information, Me.Text)
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
        If Not isThisAValidExecutableTask(strTaskName, exePath) Then
            Exit Sub
        End If

        If Not IO.File.Exists(strPathToAutoShortcut) Then
            createShortCut(strPathToAutoShortcut, "schtasks", exePath, strTaskName, String.Format("/run /TN {0}\{2}\{1}{0}", Chr(34), strTaskName, strTaskFolderName))
        End If
    End Sub

    Private Sub btnEditTask_Click(sender As Object, e As EventArgs) Handles btnEditTask.Click
        Dim taskService As New TaskService
        Dim actions As ActionCollection
        Dim execAction As ExecAction

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            If task.Name.Equals(listTasks.Text, StringComparison.OrdinalIgnoreCase) Then
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
            End If
        Next

        btnCreateTask.Text = "Save Changes to Task"
        btnCancelEditTask.Enabled = True
        txtTaskName.ReadOnly = True
        ToolTip1.SetToolTip(txtTaskName, "Disabled in Edit Mode")
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
    End Sub

    Private Sub btnDeleteTasks_Click(sender As Object, e As EventArgs) Handles btnDeleteTasks.Click
        Dim taskService As New TaskService
        taskService.RootFolder.SubFolders(strTaskFolderName).DeleteTask(listTasks.Text)
        taskService.Dispose()
        taskService = Nothing

        refreshTasks()

        MsgBox("Task Deleted.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub btnCheckForUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        windowObject = Me
        Threading.ThreadPool.QueueUserWorkItem(AddressOf checkForUpdates)
        btnCheckForUpdates.Enabled = False
    End Sub

    Function isThisAValidExecutableTask(ByVal strTaskName As String, ByRef exePath As String) As Boolean
        Dim taskService As New TaskService
        Dim actions As ActionCollection

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            If task.Name.Equals(strTaskName, StringComparison.OrdinalIgnoreCase) Then
                actions = task.Definition.Actions

                For Each action As Action In actions
                    If action.ActionType = TaskActionType.Execute Then
                        exePath = DirectCast(action, ExecAction).Path.Replace("""", "")
                        Return True
                    End If
                Next
            End If
        Next

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
            Dim taskService As New TaskService
            Dim actions As ActionCollection
            Dim execAction As ExecAction

            Dim savedTask As New classTask()

            For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
                If task.Name.Equals(listTasks.Text, StringComparison.OrdinalIgnoreCase) Then
                    actions = task.Definition.Actions

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
                End If
            Next

            taskService.Dispose()
            taskService = Nothing

            Dim streamWriter As New StreamWriter(saveTask.FileName)
            Dim xmlSerializerObject As New XmlSerializer(savedTask.GetType)
            xmlSerializerObject.Serialize(streamWriter, savedTask)
            streamWriter.Close()
            streamWriter.Dispose()

            MsgBox("Task exported.", MsgBoxStyle.Information, Me.Text)
        End If
    End Sub

    Private Sub btnImportTask_Click(sender As Object, e As EventArgs) Handles btnImportTask.Click
        importTask.Title = "Import Task from Task File"
        importTask.FileName = Nothing
        importTask.Filter = "Task File|*.task"

        If importTask.ShowDialog() = DialogResult.OK Then
            Dim streamReader As New StreamReader(importTask.FileName)
            Dim savedTask As New classTask()
            Dim xmlSerializerObject As New XmlSerializer(savedTask.GetType)
            savedTask = xmlSerializerObject.Deserialize(streamReader)
            streamReader.Close()
            streamReader.Dispose()

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
        taskParameters = taskParameters.Trim

        If Not IO.File.Exists(taskEXEPath) Then
            MsgBox("Executable path not found.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        Dim taskService As TaskService = New TaskService()
        Dim newTask As TaskDefinition = taskService.NewTask

        newTask.RegistrationInfo.Description = taskDescription

        If chkEnabled.Checked Then newTask.Triggers.Add(New LogonTrigger)

        Dim exeFileInfo As New FileInfo(taskEXEPath)

        newTask.Actions.Add(New ExecAction(Chr(34) & taskEXEPath & Chr(34), taskParameters, exeFileInfo.DirectoryName))

        newTask.Principal.RunLevel = TaskRunLevel.Highest
        newTask.Settings.Compatibility = TaskCompatibility.V2_1
        newTask.Settings.AllowDemandStart = True
        newTask.Settings.DisallowStartIfOnBatteries = False
        newTask.Settings.RunOnlyIfIdle = False
        newTask.Settings.StopIfGoingOnBatteries = False
        newTask.Settings.AllowHardTerminate = False
        newTask.Settings.UseUnifiedSchedulingEngine = True
        newTask.Settings.ExecutionTimeLimit = Nothing
        newTask.Principal.LogonType = TaskLogonType.InteractiveToken

        taskService.RootFolder.SubFolders(strTaskFolderName).RegisterTaskDefinition(taskName, newTask)

        newTask.Dispose()
        taskService.Dispose()
        newTask = Nothing
        taskService = Nothing
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
        If chkUseSSL.Checked Then
            imgLock.Image = My.Resources.locked
        Else
            imgLock.Image = My.Resources.unlocked
        End If

        My.Settings.boolUseSSL = chkUseSSL.Checked
    End Sub

    Private Sub btnStopStartTask_Click(sender As Object, e As EventArgs) Handles btnStopStartTask.Click
        Dim taskService As New TaskService
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

        taskService.Dispose()
    End Sub

    Function boolIsTaskRunning(ByVal strTaskName As String, ByRef taskService As TaskService, ByRef taskObject As Task) As Boolean
        Dim taskObjectToWorkOn As Task = Nothing

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            If task.Name.Equals(strTaskName, StringComparison.OrdinalIgnoreCase) Then
                taskObject = task

                If task.State = TaskState.Running Then
                    Return True
                Else
                    Return False
                End If
            End If

            task.Dispose()
        Next

        Return False
    End Function

    Function boolIsTaskRunning(strTaskName As String) As Boolean
        Dim taskService As New TaskService

        For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
            If task.Name.Equals(strTaskName, StringComparison.OrdinalIgnoreCase) Then
                If task.State = TaskState.Running Then
                    taskService.Dispose()
                    task.Dispose()
                    Return True
                End If
            End If

            task.Dispose()
        Next

        taskService.Dispose()
        Return False
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