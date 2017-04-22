Public Class classTask
    Private strTaskName, strTaskDescription, strTaskEXE, strTaskParameters As String

    Public Property taskName() As String
        Get
            taskName = strTaskName
        End Get
        Set(ByVal Value As String)
            strTaskName = Value
        End Set
    End Property

    Public Property taskDescription() As String
        Get
            taskDescription = strTaskDescription
        End Get
        Set(ByVal Value As String)
            strTaskDescription = Value
        End Set
    End Property

    Public Property taskEXE() As String
        Get
            taskEXE = strTaskEXE
        End Get
        Set(ByVal Value As String)
            strTaskEXE = Value
        End Set
    End Property

    Public Property taskParameters() As String
        Get
            taskParameters = strTaskParameters
        End Get
        Set(ByVal Value As String)
            strTaskParameters = Value
        End Set
    End Property
End Class