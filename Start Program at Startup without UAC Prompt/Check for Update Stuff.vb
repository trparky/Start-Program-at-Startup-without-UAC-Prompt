Imports System.IO
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.Xml

Public Enum versionPieces As Short
    major = 0
    minor = 1
    build = 2
    revision = 3
End Enum

Module Check_for_Update_Stuff
    Private Const programZipFileURL = "www.toms-world.org/download/Start Program at Startup without UAC Prompt.zip"
    Private Const programZipFileSHA1URL = "www.toms-world.org/download/Start Program at Startup without UAC Prompt.zip.sha1"

    Private Const programFileNameInZIP As String = "Start Program at Startup without UAC Prompt.exe"

    Public Const programUpdateCheckerXMLPath As String = "www.toms-world.org/updates/start_program_at_startup_without_uac_prompt.xml"
    Public Const programName As String = "Start Program at Startup with Admin Privileges without UAC Prompt"

    Private versionInfo As String() = Application.ProductVersion.Split(".")
    Private shortBuild As Short = Short.Parse(versionInfo(versionPieces.build).Trim)

    Private versionStringWithoutBuild As String = String.Format("{0}.{1}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor))
    Public strFullVersionString As String = String.Format("{0}.{1} Build {2}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor), versionInfo(versionPieces.build))

    Public windowObject As Form1

    Public boolWinXP As Boolean = False

    Private Function SHA160(ByRef memoryStream As MemoryStream) As String
        Using SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider
            memoryStream.Position = 0
            Dim Output As Byte() = SHA1Engine.ComputeHash(memoryStream)
            memoryStream.Position = 0
            Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        End Using
    End Function

    Public Function verifyChecksum(urlOfChecksumFile As String, ByRef memoryStream As MemoryStream, boolGiveUserAnErrorMessage As Boolean) As Boolean
        Dim checksumFromWeb As String = Nothing

        If Not createNewHTTPHelperObject().getWebData(urlOfChecksumFile, checksumFromWeb, False) Then
            If boolGiveUserAnErrorMessage Then MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, programName)
            Return False
        Else
            ' Checks to see if we have a valid SHA1 file.
            If Text.RegularExpressions.Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{40})") Then
                checksumFromWeb = Text.RegularExpressions.Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{40})").Groups(1).Value().ToLower.Trim()

                If SHA160(memoryStream).Equals(checksumFromWeb, StringComparison.OrdinalIgnoreCase) Then : Return True
                Else
                    If boolGiveUserAnErrorMessage Then MsgBox("There was an error in the download, checksums don't match. Update process aborted.", MsgBoxStyle.Critical, programName)
                    Return False
                End If
            Else
                If boolGiveUserAnErrorMessage Then MsgBox("Invalid SHA1 file detected. Update process aborted.", MsgBoxStyle.Critical, programName)
                Return False
            End If
        End If
    End Function

    Private Sub extractFileFromZIPFile(memoryStream As MemoryStream, fileToExtract As String, fileToWriteExtractedFileTo As String)
        memoryStream.Position = 0

        Using zipFileObject As New Compression.ZipArchive(memoryStream)
            Dim zipFileEntry As Compression.ZipArchiveEntry = zipFileObject.GetEntry(fileToExtract)

            If zipFileEntry IsNot Nothing Then
                Using zipFileEntryIOStream As Stream = zipFileEntry.Open()
                    Using fileStream As New FileStream(fileToWriteExtractedFileTo, FileMode.Create)
                        zipFileEntryIOStream.CopyTo(fileStream)
                    End Using
                End Using
            End If
        End Using
    End Sub

    Private Sub downloadAndPerformUpdate()
        Dim fileInfo As New FileInfo(Application.ExecutablePath)
        Dim newExecutableName As String = fileInfo.Name & ".new.exe"

        Using memoryStream As New MemoryStream
            If Not createNewHTTPHelperObject().downloadFile(programZipFileURL, memoryStream, False) Then
                MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, programName)
                Exit Sub
            End If

            If Not verifyChecksum(programZipFileSHA1URL, memoryStream, True) Then
                MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, programName)
                Exit Sub
            End If

            extractFileFromZIPFile(memoryStream, programFileNameInZIP, newExecutableName)
        End Using

        Dim startInfo As New ProcessStartInfo With {
            .FileName = newExecutableName,
            .Arguments = "-update"
        }
        If Not canIWriteToTheCurrentDirectory() Then startInfo.Verb = "runas"
        Process.Start(startInfo)

        Process.GetCurrentProcess.Kill()
    End Sub

    Private Function createHTTPUserAgentHeaderString() As String
        Return String.Format("{0} version {1} on {2}", programName, strFullVersionString, getFullOSVersionString())
    End Function

    Public Function createNewHTTPHelperObject() As httpHelper
        Dim httpHelper As New httpHelper With {
            .useHTTPCompression = True,
            .setProxyMode = True,
            .useSystemProxy = True,
            .setUserAgent = createHTTPUserAgentHeaderString()
        }
        httpHelper.addHTTPHeader("PROGRAM_NAME", programName)
        httpHelper.addHTTPHeader("PROGRAM_VERSION", strFullVersionString)
        httpHelper.addHTTPHeader("OPERATING_SYSTEM", getFullOSVersionString())

        If File.Exists("tom") Then httpHelper.addPOSTData("dontcount", "True")

        httpHelper.setURLPreProcessor = Function(ByVal strURLInput As String) As String
                                            Try
                                                If strURLInput.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                                                    Return strURLInput
                                                Else
                                                    Return If(My.Settings.boolUseSSL, "https://" & strURLInput, "http://" & strURLInput)
                                                End If
                                            Catch ex As Exception
                                                Return strURLInput
                                            End Try
                                        End Function

        Return httpHelper
    End Function

    Private Function getFullOSVersionString() As String
        Dim strOSName As String

        Try
            Dim intOSMajorVersion As Integer = Environment.OSVersion.Version.Major
            Dim intOSMinorVersion As Integer = Environment.OSVersion.Version.Minor

            If intOSMajorVersion = 5 And intOSMinorVersion = 0 Then
                strOSName = "Windows 2000"
            ElseIf intOSMajorVersion = 5 And intOSMinorVersion = 1 Then
                strOSName = "Windows XP"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 0 Then
                strOSName = "Windows Vista"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 1 Then
                strOSName = "Windows 7"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 2 Then
                strOSName = "Windows 8"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 3 Then
                strOSName = "Windows 8.1"
            ElseIf intOSMajorVersion = 10 Then
                strOSName = "Windows 10"
            Else
                strOSName = String.Format("Windows NT {0}.{1}", intOSMajorVersion, intOSMinorVersion)
            End If

            strOSName &= If(Environment.Is64BitOperatingSystem, " 64-bit", " 32-bit")
        Catch ex As Exception
            Try
                Return "Unknown Windows Operating System (" & Environment.OSVersion.VersionString & ")"
            Catch ex2 As Exception
                Return "Unknown Windows Operating System"
            End Try
        End Try

        Return strOSName
    End Function

    Public Sub checkForUpdates()
        If Not checkForInternetConnection() Then
            MsgBox("No Internet connection detected.", MsgBoxStyle.Information, programName)
        Else
            Try
                Dim xmlData As String = Nothing

                If createNewHTTPHelperObject().getWebData(programUpdateCheckerXMLPath, xmlData, False) Then
                    Dim remoteVersion As String = Nothing
                    Dim remoteBuild As String = Nothing
                    Dim response As processUpdateXMLResponse = processUpdateXMLData(xmlData, remoteVersion, remoteBuild)

                    If response = processUpdateXMLResponse.newVersion Then
                        If MsgBox(String.Format("An update to {2} (version {0} Build {1}) is available to be downloaded, do you want to download and update to this new version?", remoteVersion, remoteBuild, programName), MsgBoxStyle.Question + MsgBoxStyle.YesNo, programName) = MsgBoxResult.Yes Then
                            downloadAndPerformUpdate()
                        Else
                            MsgBox("The update will not be downloaded.", MsgBoxStyle.Information, programName)
                        End If
                    ElseIf response = processUpdateXMLResponse.noUpdateNeeded Then
                        MsgBox("You already have the latest version, there is no need to update this program.", MsgBoxStyle.Information, programName)
                    ElseIf response = processUpdateXMLResponse.parseError Or response = processUpdateXMLResponse.exceptionError Then
                        MsgBox("There was an error when trying to parse response from server.", MsgBoxStyle.Critical, programName)
                    ElseIf response = processUpdateXMLResponse.newerVersionThanWebSite Then
                        MsgBox("This is weird, you have a version that's newer than what's listed on the web site.", MsgBoxStyle.Information, programName)
                    End If
                Else
                    windowObject.btnCheckForUpdates.Invoke(Sub() windowObject.btnCheckForUpdates.Enabled = True)
                    MsgBox("There was an error checking for updates.", MsgBoxStyle.Information, programName)
                End If
            Catch ex As Exception
                ' Ok, we crashed but who cares.  We give an error message.
            Finally
                windowObject = Nothing
            End Try
        End If
    End Sub

    Public Function checkForInternetConnection() As Boolean
        Return My.Computer.Network.IsAvailable
    End Function

    Enum processUpdateXMLResponse As Short
        noUpdateNeeded
        newVersion
        newerVersionThanWebSite
        parseError
        exceptionError
    End Enum

    ''' <summary>This parses the XML updata data and determines if an update is needed.</summary>
    ''' <param name="xmlData">The XML data from the web site.</param>
    ''' <returns>A Boolean value indicating if the program has been updated or not.</returns>
    Public Function processUpdateXMLData(ByVal xmlData As String, ByRef remoteVersion As String, ByRef remoteBuild As String) As processUpdateXMLResponse
        Try
            Dim xmlDocument As New XmlDocument() ' First we create an XML Document Object.
            xmlDocument.Load(New IO.StringReader(xmlData)) ' Now we try and parse the XML data.

            Dim xmlNode As XmlNode = xmlDocument.SelectSingleNode("/xmlroot")

            remoteVersion = xmlNode.SelectSingleNode("version").InnerText.Trim
            remoteBuild = xmlNode.SelectSingleNode("build").InnerText.Trim
            Dim shortRemoteBuild As Short

            ' This checks to see if current version and the current build matches that of the remote values in the XML document.
            If remoteVersion.Equals(versionStringWithoutBuild) And remoteBuild.Equals(shortBuild.ToString) Then
                ' Both the remoteVersion and the remoteBuild equals that of the current version,
                ' therefore we return a sameVersion value indicating no update is required.
                Return processUpdateXMLResponse.noUpdateNeeded
            Else
                ' First we do a check of the version, if it's not equal we simply return a newVersion value.
                If Not remoteVersion.Equals(versionStringWithoutBuild) Then
                    ' We return a newVersion value indicating that there is a new version to download and install.
                    Return processUpdateXMLResponse.newVersion
                Else
                    ' Now let's do some sanity checks here. 
                    If Short.TryParse(remoteBuild, shortRemoteBuild) Then
                        If shortRemoteBuild < shortBuild Then
                            ' This is weird, the remote build is less than the current build so we return a newerVersionThanWebSite value.
                            Return processUpdateXMLResponse.newerVersionThanWebSite
                        ElseIf shortRemoteBuild.Equals(shortBuild) Then
                            ' The build numbers match, therefore therefore we return a sameVersion value.
                            Return processUpdateXMLResponse.noUpdateNeeded
                        End If
                    Else
                        ' Something went wrong, we couldn't parse the value of the remoteBuild number so we return a parseError value.
                        Return processUpdateXMLResponse.parseError
                    End If

                    ' We return a newVersion value indicating that there is a new version to download and install.
                    Return processUpdateXMLResponse.newVersion
                End If
            End If
        Catch ex As Exception
            ' Something went wrong so we return a exceptionError value.
            Return processUpdateXMLResponse.exceptionError
        End Try
    End Function
End Module