Imports Microsoft.Win32.TaskScheduler
Imports IWshRuntimeLibrary

Public Class Form1
    Private Const strTaskFolderName As String = "Run at User Logon with Administrator Privileges"
    Private Const DoubleCRLF As String = vbCrLf & vbCrLf

    Private Sub newFileDeleterThreadSub()
        SearchForProcessAndKillIt($"{Application.ExecutablePath}.new.exe", False)
        IO.File.Delete($"{Application.ExecutablePath}.new.exe")
    End Sub

    Private Function verifyWindowLocation(point As Point) As Point
        Return If(point.X < 0 Or point.Y < 0, New Point(0, 0), point)
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblLastRanOn.Text = Nothing
        Location = verifyWindowLocation(My.Settings.mainWindowPosition)

        If IO.File.Exists($"{Application.ExecutablePath}.new.exe") Then Threading.ThreadPool.QueueUserWorkItem(AddressOf newFileDeleterThreadSub)

        Try
            Using taskService As New TaskService
                If Not taskService.RootFolder.SubFolders.Exists(strTaskFolderName) Then
                    taskService.RootFolder.CreateFolder(strTaskFolderName)
                End If
            End Using
        Catch ex As Exception
        End Try

        refreshTasks()
    End Sub

    Private Sub createShortcut(locationOfShortcut As String, pathToExecutable As String, iconPath As String, Title As String, Optional arguments As String = Nothing)
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
                .IconLocation = $"{iconPath}, 0"
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

    Private Function getActionEXEPath(ByRef task As Task) As String
        For Each action As Action In task.Definition.Actions
            If action.ActionType = TaskActionType.Execute Then
                Return DirectCast(action, ExecAction).Path.Replace("""", "")
            End If
        Next
        Return Nothing
    End Function

    Sub refreshTasks()
        listTasks.Items.Clear()

        Using taskService As New TaskService
            Dim strEXEPath As String
            For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
                checkTaskPrioritySettings(task)
                strEXEPath = getActionEXEPath(task)

                If Not String.IsNullOrWhiteSpace(strEXEPath) AndAlso Not IO.File.Exists(strEXEPath) Then
                    MsgBox($"WARNING! The task named ""{task.Name}"" has an invalid executable path. Please fix this.", MsgBoxStyle.Critical, Text)
                End If

                listTasks.Items.Add(task.Name)
            Next
        End Using

        btnExportAllTasks.Enabled = listTasks.Items.Count <> 0
        disableButtons()
    End Sub

    Sub disableButtons()
        DeleteTaskToolStripMenuItem.Enabled = False
        EditTaskToolStripMenuItem.Enabled = False
        StopStartTaskToolStripMenuItem.Enabled = False
        GetStatusOfTaskToolStripMenuItem.Enabled = False
        CreateShortcutToTaskOnDesktopToolStripMenuItem.Enabled = False
        StopStartTaskToolStripMenuItem.Enabled = False
        ExportTaskToolStripMenuItem.Enabled = False
    End Sub

    Sub enableButtons()
        DeleteTaskToolStripMenuItem.Enabled = True
        EditTaskToolStripMenuItem.Enabled = True
        StopStartTaskToolStripMenuItem.Enabled = True
        GetStatusOfTaskToolStripMenuItem.Enabled = True
        CreateShortcutToTaskOnDesktopToolStripMenuItem.Enabled = True
        StopStartTaskToolStripMenuItem.Enabled = True
        ExportTaskToolStripMenuItem.Enabled = True
    End Sub

    Private Sub listTasks_Click(sender As Object, e As EventArgs) Handles listTasks.Click
        If listTasks.SelectedItems.Count > 0 Then
            enableButtons()

            Dim taskObject As Task = Nothing
            Using taskService As New TaskService
                If getTaskObject(taskService, listTasks.Text, taskObject) Then
                    Dim lastRunTime As Date = taskObject.LastRunTime.ToUniversalTime.ToLocalTime
                    lblLastRanOn.Text = $"Task Last Ran At: {lastRunTime.ToLongTimeString} on {lastRunTime.ToLongDateString}"
                    lastRunTime = Nothing

                    If taskObject.State = TaskState.Running Then
                        StopStartTaskToolStripMenuItem.Text = "&Stop Task"
                        StopStartTaskToolStripMenuItem.Image = My.Resources.stop_sign
                    Else
                        StopStartTaskToolStripMenuItem.Text = "&Start Task"
                        StopStartTaskToolStripMenuItem.Image = My.Resources.start
                    End If
                End If
            End Using
        End If
    End Sub

    Private Sub btnBrowseForExecutable_Click(sender As Object, e As EventArgs) Handles btnBrowseForExecutable.Click
        OpenFileDialog1.Title = "Select a program to run with Windows Scheduler"
        OpenFileDialog1.Filter = "All Supported Types|*.exe;*.bat|Program Executable File|*.exe|Batch File|*.bat"
        OpenFileDialog1.FileName = Nothing
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then txtEXEPath.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub btnCreateTask_Click(sender As Object, e As EventArgs) Handles btnCreateTask.Click
        If String.IsNullOrEmpty(txtTaskName.Text.Trim) Then
            MsgBox("You must provide a name for this task.", MsgBoxStyle.Critical, Text)
            Exit Sub
        End If
        If String.IsNullOrEmpty(txtEXEPath.Text.Trim) Then
            MsgBox("You must provide a path to an executable for this task.", MsgBoxStyle.Critical, Text)
            Exit Sub
        End If
        If Not IO.File.Exists(txtEXEPath.Text) Then
            MsgBox("Executable Path Not Found.", MsgBoxStyle.Critical, Text)
            Exit Sub
        End If
        If chkRunAsSpecificUser.Checked And String.IsNullOrWhiteSpace(txtRunAsUser.Text) Then
            MsgBox("You have your task setup to run as a specific user but you didn't select one yet, please do so now.", MsgBoxStyle.Critical, Text)
            Exit Sub
        End If

        If btnCreateTask.Text.Equals("Save Changes to Task", StringComparison.OrdinalIgnoreCase) Then deleteTask(txtTaskName.Text)

        Dim intDelayedMinutes As Integer = 0
        If chkDelayExecution.Checked Then
            If Not Integer.TryParse(txtDelayMinutes.Text, intDelayedMinutes) Then intDelayedMinutes = 0
        End If

        Dim strUserName As String = Nothing
        If chkRunAsSpecificUser.Checked Then strUserName = txtRunAsUser.Text

        If addTask(txtTaskName.Text, txtDescription.Text, txtEXEPath.Text, txtParameters.Text, chkEnabled.Checked, intDelayedMinutes, strUserName, ChkRequireElevation.Checked) Then
            Dim strPathToAutoShortcut As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Start {txtTaskName.Text}.lnk")

            If btnCreateTask.Text.Equals("Save Changes to Task", StringComparison.OrdinalIgnoreCase) Then
                If chkEnabled.Checked Then : MsgBox("Task changes saved.", MsgBoxStyle.Information, Text)
                Else
                    If Not IO.File.Exists(strPathToAutoShortcut) Then
                        autoCreateDesktopShortcut(txtTaskName.Text, strPathToAutoShortcut)
                        MsgBox($"Task changes saved.{DoubleCRLF}User Logon Startup is disabled so a shortcut to run it has been created on your desktop.", MsgBoxStyle.Information, Text)
                    Else : MsgBox("Task changes saved.", MsgBoxStyle.Information, Text)
                    End If
                End If

                btnCreateTask.Text = "Create Task"
            ElseIf btnCreateTask.Text.Equals("Create Task", StringComparison.OrdinalIgnoreCase) Then
                If chkEnabled.Checked Then : MsgBox("New task saved.", MsgBoxStyle.Information, Text)
                Else
                    autoCreateDesktopShortcut(txtTaskName.Text, strPathToAutoShortcut)
                    MsgBox($"New task saved.{DoubleCRLF}User Logon Startup is disabled so a shortcut to run it has been created on your desktop.", MsgBoxStyle.Information, Text)
                End If
            End If

            refreshTasks()
        End If

        txtTaskName.Text = Nothing
        txtEXEPath.Text = Nothing
        txtParameters.Text = Nothing
        txtDescription.Text = Nothing
        txtDelayMinutes.Text = Nothing
        txtDelayMinutes.Visible = False
        lblHowManyMinutes.Visible = False
        chkDelayExecution.Enabled = False
        chkDelayExecution.Checked = False
        chkEnabled.Checked = False
        lblLastRanOn.Text = Nothing
        btnCancelEditTask.Enabled = False
        btnChooseUser.Enabled = False
        txtRunAsUser.Enabled = False
        txtRunAsUser.Text = Nothing
        chkRunAsSpecificUser.Checked = False
    End Sub

    Private Sub autoCreateDesktopShortcut(strTaskName As String, strPathToAutoShortcut As String)
        Dim exePath As String = Nothing
        If Not isThisAValidExecutableTask(strTaskName, exePath) Then Exit Sub
        If Not IO.File.Exists(strPathToAutoShortcut) Then
            createShortcut(strPathToAutoShortcut, "schtasks", exePath, strTaskName, $"/run /TN ""\{strTaskFolderName}\{strTaskName}""")
        End If
    End Sub

    Private Function getTaskObject(ByRef taskServiceObject As TaskService, nameOfTask As String, ByRef taskObject As Task) As Boolean
        Try
            taskObject = taskServiceObject.GetTask($"{strTaskFolderName}\{nameOfTask}")
            Return taskObject IsNot Nothing
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>Deletes a scheduled task.</summary>
    ''' <param name="taskName">The task name to be deleted.</param>
    Private Sub deleteTask(taskName As String)
        Try
            Using taskServiceObject As New TaskService()
                taskServiceObject.RootFolder.SubFolders(strTaskFolderName).DeleteTask(taskName, False)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub editTask()
        Using taskService As New TaskService
            Dim actions As ActionCollection
            Dim execAction As ExecAction
            Dim task As Task = Nothing

            If getTaskObject(taskService, listTasks.Text, task) Then
                ChkRequireElevation.Checked = task.Definition.Principal.RunLevel = TaskRunLevel.Highest
                actions = task.Definition.Actions
                txtTaskName.Text = task.Name
                txtDescription.Text = task.Definition.RegistrationInfo.Description
                chkEnabled.Checked = False
                chkDelayExecution.Enabled = False
                lblHowManyMinutes.Visible = False
                txtDelayMinutes.Visible = False
                txtDelayMinutes.Text = Nothing

                chkRunAsSpecificUser.Checked = False
                btnChooseUser.Enabled = False
                txtRunAsUser.Enabled = False
                txtRunAsUser.Text = Nothing

                For Each trigger As Trigger In task.Definition.Triggers
                    If trigger.Enabled And trigger.TriggerType = TaskTriggerType.Logon Then
                        chkEnabled.Checked = True
                        chkDelayExecution.Enabled = True

                        Dim logonTriggerObject As LogonTrigger = DirectCast(trigger, LogonTrigger)

                        If logonTriggerObject.Delay.Minutes <> 0 Then
                            chkDelayExecution.Checked = True
                            lblHowManyMinutes.Visible = True
                            txtDelayMinutes.Visible = True
                            txtDelayMinutes.Text = DirectCast(trigger, LogonTrigger).Delay.Minutes
                        End If

                        If Not String.IsNullOrEmpty(logonTriggerObject.UserId) Then
                            chkRunAsSpecificUser.Checked = True
                            btnChooseUser.Enabled = True
                            txtRunAsUser.Enabled = True
                            txtRunAsUser.Text = logonTriggerObject.UserId
                        End If
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
            ToolTip.SetToolTip(txtTaskName, "Disabled in Edit Mode")
        End Using
    End Sub

    Private Sub EditTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditTaskToolStripMenuItem.Click
        editTask()
    End Sub

    Private Sub btnCancelEditTask_Click(sender As Object, e As EventArgs) Handles btnCancelEditTask.Click
        txtDescription.Text = Nothing
        txtEXEPath.Text = Nothing
        txtParameters.Text = Nothing
        txtTaskName.Text = Nothing
        txtDelayMinutes.Text = Nothing
        txtDelayMinutes.Visible = False
        lblHowManyMinutes.Visible = False
        lblLastRanOn.Text = Nothing
        chkEnabled.Checked = False
        chkDelayExecution.Enabled = False
        chkDelayExecution.Checked = False
        chkRunAsSpecificUser.Checked = False
        btnChooseUser.Enabled = False
        txtRunAsUser.Enabled = False
        txtRunAsUser.Text = Nothing

        btnCreateTask.Text = "Create Task"
        btnCancelEditTask.Enabled = False
        txtTaskName.ReadOnly = False
        ToolTip.SetToolTip(txtTaskName, "")
        refreshTasks()
        disableButtons()
    End Sub

    Private Sub DeleteTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTaskToolStripMenuItem.Click
        If MsgBox($"Are you sure you want to delete the task named ""{listTasks.Text}""?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Text) = MsgBoxResult.Yes Then
            deleteTask(listTasks.Text)
            btnCancelEditTask.PerformClick()
            refreshTasks()
            MsgBox("Task Deleted.", MsgBoxStyle.Information, Text)
        End If
    End Sub

    Private Sub btnCheckForUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim checkForUpdatesObject As New checkForUpdates.CheckForUpdatesClass(Me)
                                                   checkForUpdatesObject.checkForUpdates(True)
                                               End Sub)
        btnCheckForUpdates.Enabled = False
    End Sub

    Function isThisAValidExecutableTask(strTaskName As String, ByRef exePath As String) As Boolean
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

    Private Sub CreateShortcutToTaskOnDesktopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateShortcutToTaskOnDesktopToolStripMenuItem.Click
        Dim exePath As String = Nothing
        If Not isThisAValidExecutableTask(exePath) Then
            MsgBox("Something went wrong.", MsgBoxStyle.Critical, Text)
            Exit Sub
        End If

        SaveFileDialog1.Title = "Save Shortcut To..."
        SaveFileDialog1.FileName = $"{listTasks.Text}.lnk"
        SaveFileDialog1.Filter = "Shortcut|*.lnk"

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim fileInfo As New IO.FileInfo(SaveFileDialog1.FileName)
            Dim locationOfShortcut As String = IO.Path.Combine(fileInfo.DirectoryName, $"{listTasks.Text}.lnk")

            createShortcut(locationOfShortcut, "schtasks", exePath, listTasks.Text, $"/run /TN ""\{strTaskFolderName}\{listTasks.Text}""")

            MsgBox("Shortcut Created Successfully.", MsgBoxStyle.Information, "Create Shortcut to Task")
        End If
    End Sub

    Private Sub ExportTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTaskToolStripMenuItem.Click
        saveTask.Title = "Save as Task File"
        saveTask.Filter = "JSON Task File|*.task|Legacy XML Task File|*.taskx"
        saveTask.FileName = $"{listTasks.Text}.task"

        If saveTask.ShowDialog() = DialogResult.OK Then
            Using taskService As New TaskService
                Dim task As Task = Nothing

                If getTaskObject(taskService, listTasks.Text, task) Then
                    Dim savedTask As New classTask() With {.startup = False, .delayedMinutes = 0}
                    Dim actions As ActionCollection = task.Definition.Actions
                    Dim execAction As ExecAction

                    savedTask.requireElevation = task.Definition.Principal.RunLevel = TaskRunLevel.Highest

                    If task.Definition.Triggers.Count <> 0 Then
                        If task.Definition.Triggers(0).TriggerType = TaskTriggerType.Logon Then
                            Dim logonTriggerObject As LogonTrigger = DirectCast(task.Definition.Triggers(0), LogonTrigger)

                            savedTask.userName = If(String.IsNullOrEmpty(logonTriggerObject.UserId), Nothing, logonTriggerObject.UserId)
                            savedTask.delayedMinutes = logonTriggerObject.Delay.Minutes
                            savedTask.startup = True
                        End If
                    End If

                    savedTask.taskName = task.Name
                    savedTask.taskDescription = task.Definition.RegistrationInfo.Description

                    For Each action As Action In actions
                        If action.ActionType = TaskActionType.Execute Then
                            execAction = DirectCast(action, ExecAction)

                            savedTask.taskEXE = execAction.Path.Replace("""", "")
                            savedTask.taskParameters = execAction.Arguments

                            action.Dispose()
                            execAction.Dispose()

                            Exit For
                        End If
                    Next

                    actions.Dispose()

                    Using streamWriter As New IO.StreamWriter(saveTask.FileName)
                        If New IO.FileInfo(saveTask.FileName).Extension.Equals(".taskx") Then
                            Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(savedTask.GetType)
                            xmlSerializerObject.Serialize(streamWriter, savedTask)
                        Else
                            Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                            streamWriter.Write(json.Serialize(savedTask))
                        End If
                    End Using

                    MsgBox("Task exported.", MsgBoxStyle.Information, Text)
                End If
            End Using
        End If
    End Sub

    Private Function doesUserExistOnThisSystem(strUserName As String) As Boolean
        If String.IsNullOrWhiteSpace(strUserName) Then Return False

        Using usersSearcher As New Management.ManagementObjectSearcher("SELECT Caption FROM Win32_UserAccount")
            For Each user As Management.ManagementObject In usersSearcher.Get()
                If strUserName.Equals(user("Caption").ToString, StringComparison.OrdinalIgnoreCase) Then Return True
            Next
        End Using

        Return False
    End Function

    Private Sub ImportTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportTaskToolStripMenuItem.Click
        importTask.Title = "Import Task from Task File"
        importTask.FileName = Nothing
        importTask.Filter = "Task File|*.task;*.taskx"

        If importTask.ShowDialog() = DialogResult.OK Then
            Dim savedTask As New classTask()
            Dim strDataFromFile As String
            Dim strFileExtension As String = New IO.FileInfo(importTask.FileName).Extension

            Using streamReader As New IO.StreamReader(importTask.FileName)
                strDataFromFile = streamReader.ReadToEnd.Trim
            End Using

            If strDataFromFile.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) Or strFileExtension.Equals(".taskx", StringComparison.OrdinalIgnoreCase) Then
                Using memoryStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(strDataFromFile))
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(savedTask.GetType)
                    savedTask = xmlSerializerObject.Deserialize(memoryStream)
                End Using

                ' Rewrites the file as a JSON file if the file being loaded is a task file.
                If strFileExtension.Equals(".task", StringComparison.OrdinalIgnoreCase) Then
                    Using streamWriter As New IO.StreamWriter(importTask.FileName)
                        Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                        streamWriter.Write(json.Serialize(savedTask))
                    End Using
                End If
            Else
                Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                savedTask = json.Deserialize(Of classTask)(strDataFromFile)
            End If

            If Not doesUserExistOnThisSystem(savedTask.userName) Then savedTask.userName = Nothing

            If addTask(savedTask.taskName, savedTask.taskDescription, savedTask.taskEXE, savedTask.taskParameters, savedTask.startup, savedTask.delayedMinutes, savedTask.userName, ChkRequireElevation.Checked) Then
                refreshTasks()
                MsgBox("Task imported successfully.", MsgBoxStyle.Information, Text)
            End If
        End If
    End Sub

    Function addTask(strTaskName As String, strTaskDescription As String, strExecutablePath As String, strCommandLineParameters As String, boolStartup As Boolean, intDelayedMinutes As Integer, strUserName As String, boolRequireElevation As Boolean) As Boolean
        ' We trim the variable values here.
        strTaskName = strTaskName.Trim
        strTaskDescription = strTaskDescription.Trim
        strExecutablePath = strExecutablePath.Trim

        ' Not all tasks are going to have command line parameters so we need to check if the strCommandLineParameters variable isn't Null
        ' before attempting to trim the value of it. If we don't check first we're going to have a NullReferenceException and that's bad.
        If Not String.IsNullOrEmpty(strCommandLineParameters) Then strCommandLineParameters = strCommandLineParameters.Trim

        If Not IO.File.Exists(strExecutablePath) Then
            If strExecutablePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) Then
                MsgBox("Task batch file path not found.", MsgBoxStyle.Critical, Text)
            Else
                MsgBox("Executable path not found.", MsgBoxStyle.Critical, Text)
            End If
            Return False
        End If

        Using taskService As New TaskService()
            Dim newTask As TaskDefinition = taskService.NewTask

            newTask.RegistrationInfo.Description = strTaskDescription

            If boolStartup Then
                Dim logonTriggerObject As New LogonTrigger()
                If intDelayedMinutes <> 0 Then logonTriggerObject.Delay = New TimeSpan(0, intDelayedMinutes, 0)
                If Not String.IsNullOrWhiteSpace(strUserName) Then logonTriggerObject.UserId = strUserName
                newTask.Triggers.Add(logonTriggerObject)
            End If

            Dim exeFileInfo As New IO.FileInfo(strExecutablePath)

            With newTask
                If String.IsNullOrEmpty(strCommandLineParameters) Then
                    ' If strCommandLineParameters is Null then we make sure we pass a Null value to the following function call. 
                    .Actions.Add(New ExecAction($"""{strExecutablePath}""", Nothing, exeFileInfo.DirectoryName))
                Else
                    ' In this case, strCommandLineParameters isn't Null so we pass it to the following function call.
                    .Actions.Add(New ExecAction($"""{strExecutablePath}""", strCommandLineParameters, exeFileInfo.DirectoryName))
                End If

                If boolRequireElevation Then .Principal.RunLevel = TaskRunLevel.Highest
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

            taskService.RootFolder.SubFolders(strTaskFolderName).RegisterTaskDefinition(strTaskName, newTask)

            newTask.Dispose()
            Return True
        End Using
    End Function

    Private Sub GetStatusOfTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetStatusOfTaskToolStripMenuItem.Click
        If boolIsTaskRunning(listTasks.Text) Then
            MsgBox("The task is running.", MsgBoxStyle.Information, Text)
        Else
            MsgBox("The task is NOT running.", MsgBoxStyle.Information, Text)
        End If
    End Sub

    Private Sub StopStartTaskToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopStartTaskToolStripMenuItem.Click
        Using taskService As New TaskService
            Dim taskObject As Task = Nothing
            Dim boolIsItRunning As Boolean = False

            boolIsItRunning = boolIsTaskRunning(listTasks.Text, taskService, taskObject)

            If taskObject IsNot Nothing Then
                If boolIsItRunning Then
                    taskObject.Stop()
                Else
                    taskObject.Run()
                End If

                taskObject.Dispose()
            End If
        End Using
    End Sub

    Function boolIsTaskRunning(strTaskName As String, ByRef taskService As TaskService, ByRef taskObject As Task) As Boolean
        Return getTaskObject(taskService, strTaskName, taskObject) AndAlso taskObject.State = TaskState.Running
    End Function

    Function boolIsTaskRunning(strTaskName As String) As Boolean
        Using taskService As New TaskService
            Dim task As Task = Nothing

            If getTaskObject(taskService, strTaskName, task) Then
                Return task.State = TaskState.Running
            Else
                Return False
            End If
        End Using
    End Function

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        Dim stringBuilder As New Text.StringBuilder
        stringBuilder.AppendLine(checkForUpdates.strProgramName)
        stringBuilder.AppendLine($"Version {checkForUpdates.strDisplayVersionString}")
        stringBuilder.AppendLine("Written by Tom Parkison.")
        stringBuilder.AppendLine("Copyright Thomas Parkison 2015-2024.")

        MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, $"About {checkForUpdates.strProgramName}")
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.mainWindowPosition = Location
    End Sub

    Private Sub linkWhatIsAParameter_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkWhatIsAParameter.LinkClicked
        Dim stringBuilder As New Text.StringBuilder()
        stringBuilder.AppendLine("This is a set of parameters that are used by the program you're setting up for this task or program to run. This field isn't for this program.")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("For instance, some programs may accept ""/minimize"" or ""-hide"". You'll have to consult the documentation for the program you're working with.")

        MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Text)
    End Sub

    Private Sub btnExportAllTasks_Click(sender As Object, e As EventArgs) Handles btnExportAllTasks.Click
        saveTask.FileName = Nothing
        saveTask.Title = "Save as Task Collection File"
        saveTask.Filter = "JSON Task Collection File|*.ctask|Legacy XML Task Collection File|*.ctaskx"

        If saveTask.ShowDialog() = DialogResult.OK Then
            Dim collectionOfTasks As New List(Of classTask)
            Dim savedTask As classTask
            Dim actions As ActionCollection
            Dim execAction As ExecAction
            Dim logonTriggerObject As LogonTrigger

            Using taskService As New TaskService
                For Each task As Task In taskService.RootFolder.SubFolders(strTaskFolderName).Tasks
                    savedTask = New classTask With {.startup = False, .delayedMinutes = 0}
                    actions = task.Definition.Actions

                    savedTask.requireElevation = task.Definition.Principal.RunLevel = TaskRunLevel.Highest

                    If task.Definition.Triggers.Count <> 0 Then
                        If task.Definition.Triggers(0).TriggerType = TaskTriggerType.Logon Then
                            logonTriggerObject = DirectCast(task.Definition.Triggers(0), LogonTrigger)

                            savedTask.userName = If(String.IsNullOrEmpty(logonTriggerObject.UserId), Nothing, logonTriggerObject.UserId)
                            savedTask.delayedMinutes = logonTriggerObject.Delay.Minutes
                            savedTask.startup = True
                        End If
                    End If

                    savedTask.taskName = task.Name
                    savedTask.taskDescription = task.Definition.RegistrationInfo.Description

                    For Each action As Action In actions
                        If action.ActionType = TaskActionType.Execute Then
                            execAction = DirectCast(action, ExecAction)

                            savedTask.taskEXE = execAction.Path.Replace("""", "")
                            savedTask.taskParameters = execAction.Arguments

                            action.Dispose()
                            execAction.Dispose()

                            Exit For
                        End If
                    Next

                    actions.Dispose()

                    collectionOfTasks.Add(savedTask)
                Next
            End Using

            Using streamWriter As New IO.StreamWriter(saveTask.FileName)
                If New IO.FileInfo(saveTask.FileName).Extension.Equals(".ctaskx", StringComparison.OrdinalIgnoreCase) Then
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(collectionOfTasks.GetType)
                    xmlSerializerObject.Serialize(streamWriter, collectionOfTasks)
                Else
                    Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                    streamWriter.Write(json.Serialize(collectionOfTasks))
                End If
            End Using

            MsgBox("Tasks exported.", MsgBoxStyle.Information, Text)
        End If
    End Sub

    Private Sub btnImportCollectionOfTasks_Click(sender As Object, e As EventArgs) Handles btnImportCollectionOfTasks.Click
        importTask.Title = "Import Tasks from Task Collection File"
        importTask.FileName = Nothing
        importTask.Filter = "Task Collection File|*.ctask;*.ctaskx"

        If importTask.ShowDialog() = DialogResult.OK Then
            Dim collectionOfTasks As New List(Of classTask)
            Dim strDataFromFile As String
            Dim strFileExtension As String = New IO.FileInfo(importTask.FileName).Extension

            Using streamReader As New IO.StreamReader(importTask.FileName)
                strDataFromFile = streamReader.ReadToEnd.Trim
            End Using

            Try
                If strDataFromFile.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) Or strFileExtension.Equals(".ctaskx", StringComparison.OrdinalIgnoreCase) Then
                    Using memoryStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(strDataFromFile))
                        Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(collectionOfTasks.GetType)
                        collectionOfTasks = xmlSerializerObject.Deserialize(memoryStream)
                    End Using

                    ' Rewrites the file as a JSON file if the file being loaded is a ctask file.
                    If strFileExtension.Equals(".ctask", StringComparison.OrdinalIgnoreCase) Then
                        Using streamWriter As New IO.StreamWriter(importTask.FileName)
                            Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                            streamWriter.Write(json.Serialize(collectionOfTasks))
                        End Using
                    End If
                Else
                    Dim json As New Web.Script.Serialization.JavaScriptSerializer()
                    collectionOfTasks = json.Deserialize(Of List(Of classTask))(strDataFromFile)
                End If
            Catch ex As Exception
                MsgBox("There was an error attempting to import the chosen task collection file, task import has been halted.", MsgBoxStyle.Critical, Text)
                Exit Sub
            End Try

            For Each savedTask As classTask In collectionOfTasks
                If Not doesUserExistOnThisSystem(savedTask.userName) Then savedTask.userName = Nothing
                addTask(savedTask.taskName, savedTask.taskDescription, savedTask.taskEXE, savedTask.taskParameters, savedTask.startup, savedTask.delayedMinutes, savedTask.userName, savedTask.requireElevation)
            Next

            collectionOfTasks.Clear()
            refreshTasks()

            MsgBox("Tasks imported successfully.", MsgBoxStyle.Information, Text)
        End If
    End Sub

    Private Sub chkDelayExecution_Click(sender As Object, e As EventArgs) Handles chkDelayExecution.Click
        lblHowManyMinutes.Visible = chkDelayExecution.Checked
        txtDelayMinutes.Visible = chkDelayExecution.Checked
    End Sub

    Private Sub chkEnabled_Click(sender As Object, e As EventArgs) Handles chkEnabled.Click
        chkDelayExecution.Enabled = chkEnabled.Checked

        If Not chkEnabled.Checked Then
            chkDelayExecution.Checked = False
            lblHowManyMinutes.Visible = False
            txtDelayMinutes.Visible = False
            txtDelayMinutes.Text = Nothing
        End If
    End Sub

    Private Sub chkRunAsSpecificUser_Click(sender As Object, e As EventArgs) Handles chkRunAsSpecificUser.Click
        If chkRunAsSpecificUser.Checked Then
            btnChooseUser.Enabled = True
            txtRunAsUser.Enabled = True
        Else
            btnChooseUser.Enabled = False
            txtRunAsUser.Enabled = False
            txtRunAsUser.Text = Nothing
        End If
    End Sub

    Private Sub btnChooseUser_Click(sender As Object, e As EventArgs) Handles btnChooseUser.Click
        Using Choose_User As New Choose_User With {.StartPosition = FormStartPosition.CenterParent}
            Choose_User.ShowDialog()
            If Not String.IsNullOrEmpty(Choose_User.strSelectedUser) Then txtRunAsUser.Text = Choose_User.strSelectedUser
        End Using
    End Sub

    Private Sub activateOrDeactivateCreateTaskButton()
        btnCreateTask.Enabled = Not String.IsNullOrEmpty(txtEXEPath.Text.Trim) AndAlso IO.File.Exists(txtEXEPath.Text) AndAlso Not String.IsNullOrEmpty(txtTaskName.Text.Trim)
    End Sub

    Private Sub txtEXEPath_TextChanged(sender As Object, e As EventArgs) Handles txtEXEPath.TextChanged
        activateOrDeactivateCreateTaskButton()
    End Sub

    Private Sub txtTaskName_TextChanged(sender As Object, e As EventArgs) Handles txtTaskName.TextChanged
        activateOrDeactivateCreateTaskButton()
    End Sub

    Private Sub txtDescription_TextChanged(sender As Object, e As EventArgs) Handles txtDescription.TextChanged
        activateOrDeactivateCreateTaskButton()
    End Sub

    Private Sub btnPopout_Click(sender As Object, e As EventArgs) Handles btnPopout.Click
        Using Popout_Description As New Popout_Description() With {.StartPosition = FormStartPosition.CenterParent, .Size = My.Settings.popoutDescriptionSize}
            Popout_Description.txtPopoutDescription.Text = txtDescription.Text
            Popout_Description.ShowDialog()
            If Popout_Description.userResponse = Popout_Description.userResponseEnum.save Then txtDescription.Text = Popout_Description.txtPopoutDescription.Text
        End Using
    End Sub

    Private Sub listTasks_KeyUp(sender As Object, e As KeyEventArgs) Handles listTasks.KeyUp
        If e.KeyCode = Keys.Delete AndAlso MsgBox($"Are you sure you want to delete the task named ""{listTasks.Text}""?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Text) = MsgBoxResult.Yes Then
            deleteTask(listTasks.Text)
            refreshTasks()
            MsgBox("Task Deleted.", MsgBoxStyle.Information, Text)
        ElseIf e.KeyCode = Keys.Enter Then
            editTask()
        End If
    End Sub
End Class