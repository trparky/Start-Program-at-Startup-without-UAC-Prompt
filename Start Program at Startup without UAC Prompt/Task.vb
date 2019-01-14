Public Class classTask
    Private strTaskName, strTaskDescription, strTaskEXE, strTaskParameters As String
    Private intDelayedMinutes As Integer = 0
    Private boolStartup As Boolean = False

    Public Property startup() As Boolean
        Get
            startup = boolStartup
        End Get
        Set(ByVal Value As Boolean)
            boolStartup = Value
        End Set
    End Property

    Public Property delayedMinutes() As Integer
        Get
            delayedMinutes = intDelayedMinutes
        End Get
        Set(ByVal Value As Integer)
            intDelayedMinutes = Value
        End Set
    End Property

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