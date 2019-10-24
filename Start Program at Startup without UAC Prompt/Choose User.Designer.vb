<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Choose_User
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.listUsers = New System.Windows.Forms.ListView()
        Me.btnChooseUser = New System.Windows.Forms.Button()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'listUsers
        '
        Me.listUsers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.listUsers.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.listUsers.HideSelection = False
        Me.listUsers.Location = New System.Drawing.Point(12, 12)
        Me.listUsers.Name = "listUsers"
        Me.listUsers.Size = New System.Drawing.Size(433, 202)
        Me.listUsers.TabIndex = 0
        Me.listUsers.UseCompatibleStateImageBehavior = False
        Me.listUsers.View = System.Windows.Forms.View.Details
        '
        'btnChooseUser
        '
        Me.btnChooseUser.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnChooseUser.Enabled = False
        Me.btnChooseUser.Location = New System.Drawing.Point(12, 220)
        Me.btnChooseUser.Name = "btnChooseUser"
        Me.btnChooseUser.Size = New System.Drawing.Size(115, 23)
        Me.btnChooseUser.TabIndex = 1
        Me.btnChooseUser.Text = "Choose User"
        Me.btnChooseUser.UseVisualStyleBackColor = True
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "User Name"
        Me.ColumnHeader1.Width = 183
        '
        'Choose_User
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(456, 251)
        Me.Controls.Add(Me.btnChooseUser)
        Me.Controls.Add(Me.listUsers)
        Me.Name = "Choose_User"
        Me.Text = "Choose User..."
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents listUsers As ListView
    Friend WithEvents btnChooseUser As Button
    Friend WithEvents ColumnHeader1 As ColumnHeader
End Class
