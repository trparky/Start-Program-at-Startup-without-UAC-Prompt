<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtTaskName = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtParameters = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtEXEPath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.listTasks = New System.Windows.Forms.ListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.GetStatusOfTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StopStartTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.CreateShortcutToTaskOnDesktopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportTaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.chkEnabled = New System.Windows.Forms.CheckBox()
        Me.saveTask = New System.Windows.Forms.SaveFileDialog()
        Me.importTask = New System.Windows.Forms.OpenFileDialog()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnStopStartTask = New System.Windows.Forms.Button()
        Me.btnDeleteTasks = New System.Windows.Forms.Button()
        Me.btnEditTask = New System.Windows.Forms.Button()
        Me.btnGetTaskStatus = New System.Windows.Forms.Button()
        Me.btnImportTask = New System.Windows.Forms.Button()
        Me.btnExportTask = New System.Windows.Forms.Button()
        Me.btnCreateShortcutOnDesktop = New System.Windows.Forms.Button()
        Me.btnAbout = New System.Windows.Forms.Button()
        Me.btnCheckForUpdates = New System.Windows.Forms.Button()
        Me.btnImportCollectionOfTasks = New System.Windows.Forms.Button()
        Me.btnExportAllTasks = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.chkUseSSL = New System.Windows.Forms.CheckBox()
        Me.imgLock = New System.Windows.Forms.PictureBox()
        Me.btnCancelEditTask = New System.Windows.Forms.Button()
        Me.btnCreateTask = New System.Windows.Forms.Button()
        Me.btnBrowseForExecutable = New System.Windows.Forms.Button()
        Me.linkWhatIsAParameter = New System.Windows.Forms.LinkLabel()
        Me.chkDelayExecution = New System.Windows.Forms.CheckBox()
        Me.lblHowManyMinutes = New System.Windows.Forms.Label()
        Me.txtDelayMinutes = New System.Windows.Forms.TextBox()
        Me.lblLastRanOn = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.imgLock, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(79, 179)
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(437, 20)
        Me.txtDescription.TabIndex = 48
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(10, 182)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(63, 13)
        Me.Label6.TabIndex = 47
        Me.Label6.Text = "Description:"
        '
        'txtTaskName
        '
        Me.txtTaskName.Location = New System.Drawing.Point(93, 154)
        Me.txtTaskName.Name = "txtTaskName"
        Me.txtTaskName.Size = New System.Drawing.Size(569, 20)
        Me.txtTaskName.TabIndex = 46
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(11, 8)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(625, 52)
        Me.Label5.TabIndex = 45
        Me.Label5.Text = resources.GetString("Label5.Text")
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(10, 157)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 13)
        Me.Label4.TabIndex = 44
        Me.Label4.Text = "Name of Task:"
        '
        'txtParameters
        '
        Me.txtParameters.Location = New System.Drawing.Point(168, 123)
        Me.txtParameters.Name = "txtParameters"
        Me.txtParameters.Size = New System.Drawing.Size(428, 20)
        Me.txtParameters.TabIndex = 43
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 117)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(152, 26)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "Executable Parameters" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Leave blank if there are none)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'txtEXEPath
        '
        Me.txtEXEPath.Location = New System.Drawing.Point(116, 84)
        Me.txtEXEPath.Name = "txtEXEPath"
        Me.txtEXEPath.ReadOnly = True
        Me.txtEXEPath.Size = New System.Drawing.Size(400, 20)
        Me.txtEXEPath.TabIndex = 40
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 13)
        Me.Label2.TabIndex = 39
        Me.Label2.Text = "Path to Executable:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 284)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(122, 13)
        Me.Label1.TabIndex = 38
        Me.Label1.Text = "Currently Installed Tasks"
        '
        'listTasks
        '
        Me.listTasks.ContextMenuStrip = Me.ContextMenuStrip1
        Me.listTasks.FormattingEnabled = True
        Me.listTasks.Location = New System.Drawing.Point(11, 300)
        Me.listTasks.Name = "listTasks"
        Me.listTasks.Size = New System.Drawing.Size(650, 108)
        Me.listTasks.TabIndex = 37
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteTaskToolStripMenuItem, Me.EditTaskToolStripMenuItem, Me.ToolStripMenuItem1, Me.GetStatusOfTaskToolStripMenuItem, Me.StopStartTaskToolStripMenuItem, Me.ToolStripMenuItem2, Me.CreateShortcutToTaskOnDesktopToolStripMenuItem, Me.ToolStripMenuItem3, Me.ExportTaskToolStripMenuItem, Me.ImportTaskToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(197, 176)
        '
        'DeleteTaskToolStripMenuItem
        '
        Me.DeleteTaskToolStripMenuItem.Enabled = False
        Me.DeleteTaskToolStripMenuItem.Name = "DeleteTaskToolStripMenuItem"
        Me.DeleteTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.DeleteTaskToolStripMenuItem.Text = "&Delete Task"
        '
        'EditTaskToolStripMenuItem
        '
        Me.EditTaskToolStripMenuItem.Enabled = False
        Me.EditTaskToolStripMenuItem.Name = "EditTaskToolStripMenuItem"
        Me.EditTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.EditTaskToolStripMenuItem.Text = "&Edit Task"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(193, 6)
        '
        'GetStatusOfTaskToolStripMenuItem
        '
        Me.GetStatusOfTaskToolStripMenuItem.Enabled = False
        Me.GetStatusOfTaskToolStripMenuItem.Name = "GetStatusOfTaskToolStripMenuItem"
        Me.GetStatusOfTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.GetStatusOfTaskToolStripMenuItem.Text = "&Get Status of Task"
        '
        'StopStartTaskToolStripMenuItem
        '
        Me.StopStartTaskToolStripMenuItem.Enabled = False
        Me.StopStartTaskToolStripMenuItem.Name = "StopStartTaskToolStripMenuItem"
        Me.StopStartTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.StopStartTaskToolStripMenuItem.Text = "Stop/Start Task"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(193, 6)
        '
        'CreateShortcutToTaskOnDesktopToolStripMenuItem
        '
        Me.CreateShortcutToTaskOnDesktopToolStripMenuItem.Enabled = False
        Me.CreateShortcutToTaskOnDesktopToolStripMenuItem.Name = "CreateShortcutToTaskOnDesktopToolStripMenuItem"
        Me.CreateShortcutToTaskOnDesktopToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.CreateShortcutToTaskOnDesktopToolStripMenuItem.Text = "&Create Shortcut to Task"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(193, 6)
        '
        'ExportTaskToolStripMenuItem
        '
        Me.ExportTaskToolStripMenuItem.Enabled = False
        Me.ExportTaskToolStripMenuItem.Name = "ExportTaskToolStripMenuItem"
        Me.ExportTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ExportTaskToolStripMenuItem.Text = "&Export Task"
        '
        'ImportTaskToolStripMenuItem
        '
        Me.ImportTaskToolStripMenuItem.Name = "ImportTaskToolStripMenuItem"
        Me.ImportTaskToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ImportTaskToolStripMenuItem.Text = "&Import Task"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'chkEnabled
        '
        Me.chkEnabled.AutoSize = True
        Me.chkEnabled.Location = New System.Drawing.Point(526, 181)
        Me.chkEnabled.Name = "chkEnabled"
        Me.chkEnabled.Size = New System.Drawing.Size(136, 17)
        Me.chkEnabled.TabIndex = 59
        Me.chkEnabled.Text = "At User Logon Enabled"
        Me.ToolTip1.SetToolTip(Me.chkEnabled, "Makes it so that this task starts at user logon.")
        Me.chkEnabled.UseVisualStyleBackColor = True
        '
        'importTask
        '
        Me.importTask.FileName = "OpenFileDialog2"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnStopStartTask, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.btnDeleteTasks, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnEditTask, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnGetTaskStatus, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.btnImportTask, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.btnExportTask, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.btnCreateShortcutOnDesktop, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.btnAbout, 1, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.btnCheckForUpdates, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.btnImportCollectionOfTasks, 1, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.btnExportAllTasks, 0, 6)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(11, 414)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 8
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(652, 192)
        Me.TableLayoutPanel1.TabIndex = 62
        '
        'btnStopStartTask
        '
        Me.btnStopStartTask.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStopStartTask.Enabled = False
        Me.btnStopStartTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStopStartTask.Location = New System.Drawing.Point(329, 43)
        Me.btnStopStartTask.Name = "btnStopStartTask"
        Me.btnStopStartTask.Size = New System.Drawing.Size(320, 22)
        Me.btnStopStartTask.TabIndex = 65
        Me.btnStopStartTask.Text = "Start/Stop Task"
        Me.btnStopStartTask.UseVisualStyleBackColor = True
        '
        'btnDeleteTasks
        '
        Me.btnDeleteTasks.Enabled = False
        Me.btnDeleteTasks.Image = CType(resources.GetObject("btnDeleteTasks.Image"), System.Drawing.Image)
        Me.btnDeleteTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteTasks.Location = New System.Drawing.Point(3, 3)
        Me.btnDeleteTasks.Name = "btnDeleteTasks"
        Me.btnDeleteTasks.Size = New System.Drawing.Size(320, 22)
        Me.btnDeleteTasks.TabIndex = 50
        Me.btnDeleteTasks.Text = "Delete Task"
        Me.btnDeleteTasks.UseVisualStyleBackColor = True
        '
        'btnEditTask
        '
        Me.btnEditTask.Enabled = False
        Me.btnEditTask.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.edit
        Me.btnEditTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnEditTask.Location = New System.Drawing.Point(329, 3)
        Me.btnEditTask.Name = "btnEditTask"
        Me.btnEditTask.Size = New System.Drawing.Size(320, 22)
        Me.btnEditTask.TabIndex = 51
        Me.btnEditTask.Text = "Edit Task"
        Me.btnEditTask.UseVisualStyleBackColor = True
        '
        'btnGetTaskStatus
        '
        Me.btnGetTaskStatus.Enabled = False
        Me.btnGetTaskStatus.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.status
        Me.btnGetTaskStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnGetTaskStatus.Location = New System.Drawing.Point(3, 43)
        Me.btnGetTaskStatus.Name = "btnGetTaskStatus"
        Me.btnGetTaskStatus.Size = New System.Drawing.Size(320, 22)
        Me.btnGetTaskStatus.TabIndex = 58
        Me.btnGetTaskStatus.Text = "Get Status of Task"
        Me.btnGetTaskStatus.UseVisualStyleBackColor = True
        '
        'btnImportTask
        '
        Me.btnImportTask.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.import1
        Me.btnImportTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnImportTask.Location = New System.Drawing.Point(329, 71)
        Me.btnImportTask.Name = "btnImportTask"
        Me.btnImportTask.Size = New System.Drawing.Size(320, 22)
        Me.btnImportTask.TabIndex = 56
        Me.btnImportTask.Text = "Import Task"
        Me.btnImportTask.UseVisualStyleBackColor = True
        '
        'btnExportTask
        '
        Me.btnExportTask.Enabled = False
        Me.btnExportTask.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.save
        Me.btnExportTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnExportTask.Location = New System.Drawing.Point(3, 71)
        Me.btnExportTask.Name = "btnExportTask"
        Me.btnExportTask.Size = New System.Drawing.Size(320, 22)
        Me.btnExportTask.TabIndex = 55
        Me.btnExportTask.Text = "Export Task"
        Me.btnExportTask.UseVisualStyleBackColor = True
        '
        'btnCreateShortcutOnDesktop
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.btnCreateShortcutOnDesktop, 2)
        Me.btnCreateShortcutOnDesktop.Enabled = False
        Me.btnCreateShortcutOnDesktop.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.shortcut
        Me.btnCreateShortcutOnDesktop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreateShortcutOnDesktop.Location = New System.Drawing.Point(3, 99)
        Me.btnCreateShortcutOnDesktop.Name = "btnCreateShortcutOnDesktop"
        Me.btnCreateShortcutOnDesktop.Size = New System.Drawing.Size(646, 22)
        Me.btnCreateShortcutOnDesktop.TabIndex = 54
        Me.btnCreateShortcutOnDesktop.Text = "Create Shortcut to Task"
        Me.btnCreateShortcutOnDesktop.UseVisualStyleBackColor = True
        '
        'btnAbout
        '
        Me.btnAbout.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAbout.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.info_blue
        Me.btnAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnAbout.Location = New System.Drawing.Point(329, 167)
        Me.btnAbout.Name = "btnAbout"
        Me.btnAbout.Size = New System.Drawing.Size(320, 22)
        Me.btnAbout.TabIndex = 59
        Me.btnAbout.Text = "About"
        Me.btnAbout.UseVisualStyleBackColor = True
        '
        'btnCheckForUpdates
        '
        Me.btnCheckForUpdates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCheckForUpdates.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.refresh
        Me.btnCheckForUpdates.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCheckForUpdates.Location = New System.Drawing.Point(3, 167)
        Me.btnCheckForUpdates.Name = "btnCheckForUpdates"
        Me.btnCheckForUpdates.Size = New System.Drawing.Size(320, 22)
        Me.btnCheckForUpdates.TabIndex = 53
        Me.btnCheckForUpdates.Text = "Check for Updates"
        Me.btnCheckForUpdates.UseVisualStyleBackColor = True
        '
        'btnImportCollectionOfTasks
        '
        Me.btnImportCollectionOfTasks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImportCollectionOfTasks.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.import1
        Me.btnImportCollectionOfTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnImportCollectionOfTasks.Location = New System.Drawing.Point(329, 139)
        Me.btnImportCollectionOfTasks.Name = "btnImportCollectionOfTasks"
        Me.btnImportCollectionOfTasks.Size = New System.Drawing.Size(320, 22)
        Me.btnImportCollectionOfTasks.TabIndex = 66
        Me.btnImportCollectionOfTasks.Text = "Import Task Collection File"
        Me.btnImportCollectionOfTasks.UseVisualStyleBackColor = True
        '
        'btnExportAllTasks
        '
        Me.btnExportAllTasks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExportAllTasks.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.save
        Me.btnExportAllTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnExportAllTasks.Location = New System.Drawing.Point(3, 139)
        Me.btnExportAllTasks.Name = "btnExportAllTasks"
        Me.btnExportAllTasks.Size = New System.Drawing.Size(320, 22)
        Me.btnExportAllTasks.TabIndex = 67
        Me.btnExportAllTasks.Text = "Export All Tasks to Task Collection File"
        Me.btnExportAllTasks.UseVisualStyleBackColor = True
        '
        'chkUseSSL
        '
        Me.chkUseSSL.AutoSize = True
        Me.chkUseSSL.Location = New System.Drawing.Point(10, 612)
        Me.chkUseSSL.Name = "chkUseSSL"
        Me.chkUseSSL.Size = New System.Drawing.Size(68, 17)
        Me.chkUseSSL.TabIndex = 63
        Me.chkUseSSL.Text = "Use SSL"
        Me.chkUseSSL.UseVisualStyleBackColor = True
        '
        'imgLock
        '
        Me.imgLock.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.locked
        Me.imgLock.Location = New System.Drawing.Point(77, 612)
        Me.imgLock.Name = "imgLock"
        Me.imgLock.Size = New System.Drawing.Size(15, 20)
        Me.imgLock.TabIndex = 64
        Me.imgLock.TabStop = False
        '
        'btnCancelEditTask
        '
        Me.btnCancelEditTask.Enabled = False
        Me.btnCancelEditTask.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.removeSmall
        Me.btnCancelEditTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancelEditTask.Location = New System.Drawing.Point(336, 232)
        Me.btnCancelEditTask.Name = "btnCancelEditTask"
        Me.btnCancelEditTask.Size = New System.Drawing.Size(324, 23)
        Me.btnCancelEditTask.TabIndex = 52
        Me.btnCancelEditTask.Text = "Cancel Edit"
        Me.btnCancelEditTask.UseVisualStyleBackColor = True
        '
        'btnCreateTask
        '
        Me.btnCreateTask.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.save
        Me.btnCreateTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreateTask.Location = New System.Drawing.Point(9, 232)
        Me.btnCreateTask.Name = "btnCreateTask"
        Me.btnCreateTask.Size = New System.Drawing.Size(321, 23)
        Me.btnCreateTask.TabIndex = 49
        Me.btnCreateTask.Text = "Create Task"
        Me.btnCreateTask.UseVisualStyleBackColor = True
        '
        'btnBrowseForExecutable
        '
        Me.btnBrowseForExecutable.Image = Global.Start_Program_at_Startup_without_UAC_Prompt.My.Resources.Resources.folder_explore
        Me.btnBrowseForExecutable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBrowseForExecutable.Location = New System.Drawing.Point(522, 82)
        Me.btnBrowseForExecutable.Name = "btnBrowseForExecutable"
        Me.btnBrowseForExecutable.Size = New System.Drawing.Size(140, 23)
        Me.btnBrowseForExecutable.TabIndex = 41
        Me.btnBrowseForExecutable.Text = "Browse for Executable"
        Me.btnBrowseForExecutable.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnBrowseForExecutable.UseVisualStyleBackColor = True
        '
        'linkWhatIsAParameter
        '
        Me.linkWhatIsAParameter.AutoSize = True
        Me.linkWhatIsAParameter.Location = New System.Drawing.Point(602, 126)
        Me.linkWhatIsAParameter.Name = "linkWhatIsAParameter"
        Me.linkWhatIsAParameter.Size = New System.Drawing.Size(68, 13)
        Me.linkWhatIsAParameter.TabIndex = 65
        Me.linkWhatIsAParameter.TabStop = True
        Me.linkWhatIsAParameter.Text = "What is this?"
        '
        'chkDelayExecution
        '
        Me.chkDelayExecution.AutoSize = True
        Me.chkDelayExecution.Enabled = False
        Me.chkDelayExecution.Location = New System.Drawing.Point(11, 209)
        Me.chkDelayExecution.Name = "chkDelayExecution"
        Me.chkDelayExecution.Size = New System.Drawing.Size(146, 17)
        Me.chkDelayExecution.TabIndex = 66
        Me.chkDelayExecution.Text = "Delay Startup Execution?"
        Me.chkDelayExecution.UseVisualStyleBackColor = True
        '
        'lblHowManyMinutes
        '
        Me.lblHowManyMinutes.AutoSize = True
        Me.lblHowManyMinutes.Location = New System.Drawing.Point(165, 210)
        Me.lblHowManyMinutes.Name = "lblHowManyMinutes"
        Me.lblHowManyMinutes.Size = New System.Drawing.Size(115, 13)
        Me.lblHowManyMinutes.TabIndex = 67
        Me.lblHowManyMinutes.Text = "By how many minutes?"
        Me.lblHowManyMinutes.Visible = False
        '
        'txtDelayMinutes
        '
        Me.txtDelayMinutes.Location = New System.Drawing.Point(286, 207)
        Me.txtDelayMinutes.Name = "txtDelayMinutes"
        Me.txtDelayMinutes.Size = New System.Drawing.Size(37, 20)
        Me.txtDelayMinutes.TabIndex = 68
        Me.txtDelayMinutes.Visible = False
        '
        'lblLastRanOn
        '
        Me.lblLastRanOn.AutoSize = True
        Me.lblLastRanOn.Location = New System.Drawing.Point(329, 210)
        Me.lblLastRanOn.Name = "lblLastRanOn"
        Me.lblLastRanOn.Size = New System.Drawing.Size(66, 13)
        Me.lblLastRanOn.TabIndex = 70
        Me.lblLastRanOn.Text = "Last Ran At:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(675, 641)
        Me.Controls.Add(Me.lblLastRanOn)
        Me.Controls.Add(Me.txtDelayMinutes)
        Me.Controls.Add(Me.lblHowManyMinutes)
        Me.Controls.Add(Me.chkDelayExecution)
        Me.Controls.Add(Me.linkWhatIsAParameter)
        Me.Controls.Add(Me.imgLock)
        Me.Controls.Add(Me.chkUseSSL)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.chkEnabled)
        Me.Controls.Add(Me.btnCancelEditTask)
        Me.Controls.Add(Me.btnCreateTask)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtTaskName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtParameters)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btnBrowseForExecutable)
        Me.Controls.Add(Me.txtEXEPath)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.listTasks)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Form1"
        Me.Text = "Start Program at Startup with Admin Privileges without UAC Prompt"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.imgLock, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCreateShortcutOnDesktop As System.Windows.Forms.Button
    Friend WithEvents btnCheckForUpdates As System.Windows.Forms.Button
    Friend WithEvents btnCancelEditTask As System.Windows.Forms.Button
    Friend WithEvents btnEditTask As System.Windows.Forms.Button
    Friend WithEvents btnDeleteTasks As System.Windows.Forms.Button
    Friend WithEvents btnCreateTask As System.Windows.Forms.Button
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtTaskName As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtParameters As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnBrowseForExecutable As System.Windows.Forms.Button
    Friend WithEvents txtEXEPath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents listTasks As System.Windows.Forms.ListBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnExportTask As System.Windows.Forms.Button
    Friend WithEvents saveTask As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnImportTask As System.Windows.Forms.Button
    Friend WithEvents importTask As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnGetTaskStatus As System.Windows.Forms.Button
    Friend WithEvents chkEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents GetStatusOfTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StopStartTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CreateShortcutToTaskOnDesktopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ImportTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportTaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents chkUseSSL As CheckBox
    Friend WithEvents btnAbout As Button
    Friend WithEvents imgLock As PictureBox
    Friend WithEvents btnStopStartTask As Button
    Friend WithEvents linkWhatIsAParameter As LinkLabel
    Friend WithEvents btnImportCollectionOfTasks As Button
    Friend WithEvents btnExportAllTasks As Button
    Friend WithEvents chkDelayExecution As CheckBox
    Friend WithEvents lblHowManyMinutes As Label
    Friend WithEvents txtDelayMinutes As TextBox
    Friend WithEvents lblLastRanOn As Label
End Class
