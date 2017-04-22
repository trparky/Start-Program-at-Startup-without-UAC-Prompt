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

    Private Const zipFileName As String = "Start Program at Startup without UAC Prompt.zip"
    Private Const programFileNameInZIP As String = "Start Program at Startup without UAC Prompt.exe"

    Private Const webSiteURL As String = "www.toms-world.org/blog/start-program-at-startup-without-uac-prompt"

    Public Const programUpdateCheckerXMLPath As String = "www.toms-world.org/updates/start_program_at_startup_without_uac_prompt.xml"
    Public Const programName As String = "Start Program at Startup with Admin Privileges without UAC Prompt"

    Private versionInfo As String() = Application.ProductVersion.Split(".")
    Private shortMajor As Short = Short.Parse(versionInfo(versionPieces.major).Trim)
    Private shortMinor As Short = Short.Parse(versionInfo(versionPieces.minor).Trim)
    Private shortBuild As Short = Short.Parse(versionInfo(versionPieces.build).Trim)

    Private versionStringWithoutBuild As String = String.Format("{0}.{1}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor))
    Public strFullVersionString As String = String.Format("{0}.{1} Build {2}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor), versionInfo(versionPieces.build))

    Public windowObject As Form1

    Public boolWinXP As Boolean = False

    Private Function SHA160(ByRef memoryStream As MemoryStream) As String
        Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider

        memoryStream.Position = 0
        Dim Output As Byte() = SHA1Engine.ComputeHash(memoryStream)
        memoryStream.Position = 0

        Dim result As String = BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        SHA160 = result
    End Function

    Public Function verifyChecksum(urlOfChecksumFile As String, ByRef memoryStream As MemoryStream, boolGiveUserAnErrorMessage As Boolean) As Boolean
        Dim checksumFromWeb As String = Nothing

        If Not createNewHTTPHelperObject().getWebData(urlOfChecksumFile, checksumFromWeb, False) Then
            If boolGiveUserAnErrorMessage = True Then
                MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Add Adobe Flash to Microsoft EMET")
            End If

            Return False
        Else
            ' Checks to see if we have a valid SHA1 file.
            If Text.RegularExpressions.Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{40})") = True Then
                checksumFromWeb = Text.RegularExpressions.Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{40})").Groups(1).Value().ToLower.Trim()

                If SHA160(memoryStream) = checksumFromWeb Then
                    Return True
                Else
                    If boolGiveUserAnErrorMessage Then
                        MsgBox("There was an error in the download, checksums don't match. Update process aborted.", MsgBoxStyle.Critical, "Add Adobe Flash to Microsoft EMET")
                    End If

                    Return False
                End If
            Else
                If boolGiveUserAnErrorMessage Then
                    MsgBox("Invalid SHA1 file detected. Update process aborted.", MsgBoxStyle.Critical, "Add Adobe Flash to Microsoft EMET")
                End If

                Return False
            End If
        End If
    End Function

    Private Sub extractFileFromZIPFile(memoryStream As MemoryStream, fileToExtract As String, fileToWriteExtractedFileTo As String)
        memoryStream.Position = 0
        Dim zipFileObject As New ICSharpCode.SharpZipLib.Zip.ZipFile(memoryStream)
        Dim zipFileEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry = zipFileObject.GetEntry(fileToExtract)

        If zipFileEntry IsNot Nothing Then
            Dim fileStream As New FileStream(fileToWriteExtractedFileTo, FileMode.Create)
            zipFileObject.GetInputStream(zipFileEntry).CopyTo(fileStream)
            fileStream.Close()
            fileStream.Dispose()
        End If

        zipFileObject.Close()
    End Sub

    Private Sub downloadAndPerformUpdate()
        Dim fileInfo As New FileInfo(Application.ExecutablePath)
        Dim newExecutableName As String = fileInfo.Name & ".new.exe"
        Dim memoryStream As New MemoryStream

        If Not createNewHTTPHelperObject().downloadFile(programZipFileURL, memoryStream, False) Then
            MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, programName)

            memoryStream.Close()
            memoryStream.Dispose()
            Exit Sub
        End If

        If Not verifyChecksum(programZipFileSHA1URL, memoryStream, True) Then
            MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, programName)

            memoryStream.Close()
            memoryStream.Dispose()
            Exit Sub
        End If

        fileInfo = Nothing

        extractFileFromZIPFile(memoryStream, programFileNameInZIP, newExecutableName)
        memoryStream.Close()
        memoryStream.Dispose()

        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = newExecutableName
        startInfo.Arguments = "-update"
        If canIWriteToTheCurrentDirectory() = False Then startInfo.Verb = "runas"
        Process.Start(startInfo)

        Process.GetCurrentProcess.Kill()
    End Sub

    Private Function canIWriteToTheCurrentDirectory() As Boolean
        Return canIWriteThere(New IO.FileInfo(Application.ExecutablePath).DirectoryName)
    End Function

    Private Function canIWriteThere(folderPath As String) As Boolean
        ' We make sure we get valid folder path by taking off the leading slash.
        If folderPath.EndsWith("\") Then
            folderPath = folderPath.Substring(0, folderPath.Length - 1)
        End If

        If String.IsNullOrEmpty(folderPath) = True Or Directory.Exists(folderPath) = False Then
            Return False
        End If

        If checkByFolderACLs(folderPath) = True Then
            Try
                File.Create(Path.Combine(folderPath, "test.txt"), 1, FileOptions.DeleteOnClose).Close()
                If File.Exists(Path.Combine(folderPath, "test.txt")) Then File.Delete(Path.Combine(folderPath, "test.txt"))
                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Private Function checkByFolderACLs(folderPath As String) As Boolean
        Try
            Dim directoryACLs As DirectorySecurity = Directory.GetAccessControl(folderPath)
            Dim directoryUsers As String = WindowsIdentity.GetCurrent.User.Value
            Dim directoryAccessRights As FileSystemAccessRule
            Dim fileSystemRights As FileSystemRights

            For Each rule As AuthorizationRule In directoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
                If rule.IdentityReference.Value = directoryUsers Then
                    directoryAccessRights = DirectCast(rule, FileSystemAccessRule)

                    If directoryAccessRights.AccessControlType = Security.AccessControl.AccessControlType.Allow Then
                        fileSystemRights = directoryAccessRights.FileSystemRights

                        If fileSystemRights = (FileSystemRights.Read Or FileSystemRights.Modify Or FileSystemRights.Write Or FileSystemRights.FullControl) Then
                            Return True
                        End If
                    End If
                End If
            Next

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function createHTTPUserAgentHeaderString() As String
        Return String.Format("{0} version {1} on {2}", programName, strFullVersionString, getFullOSVersionString())
    End Function

    Public Function createNewHTTPHelperObject() As httpHelper
        Dim httpHelper As New httpHelper
        httpHelper.setUserAgent = createHTTPUserAgentHeaderString()
        httpHelper.addHTTPHeader("PROGRAM_NAME", programName)
        httpHelper.addHTTPHeader("PROGRAM_VERSION", strFullVersionString)
        httpHelper.addHTTPHeader("OPERATING_SYSTEM", getFullOSVersionString())
        httpHelper.useHTTPCompression = True
        httpHelper.setProxyMode = True
        httpHelper.useSystemProxy = True

        If File.Exists("tom") = True Then
            httpHelper.addPOSTData("dontcount", "True")
        End If

        httpHelper.setURLPreProcessor = Function(ByVal strURLInput As String) As String
                                            Try
                                                If strURLInput.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                                                    Return strURLInput
                                                Else
                                                    If My.Settings.boolUseSSL Then
                                                        Return "https://" & strURLInput
                                                    Else
                                                        Return "http://" & strURLInput
                                                    End If
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

            If Environment.Is64BitOperatingSystem Then
                strOSName &= " 64-bit"
            Else
                strOSName &= " 32-bit"
            End If
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
        If checkForInternetConnection() = False Then
            MsgBox("No Internet connection detected.", MsgBoxStyle.Information, windowObject.Text)
        Else
            'Debug.WriteLine("internet connection detected")
            Try
                Dim xmlData As String = Nothing

                If createNewHTTPHelperObject().getWebData(programUpdateCheckerXMLPath, xmlData, False) Then
                    If processUpdateXMLData(xmlData) Then
                        downloadAndPerformUpdate()
                    Else
                        windowObject.btnCheckForUpdates.Invoke(Sub() windowObject.btnCheckForUpdates.Enabled = True)
                        MsgBox("You already have the latest version.", MsgBoxStyle.Information, windowObject.Text)
                    End If
                Else
                    windowObject.btnCheckForUpdates.Invoke(Sub() windowObject.btnCheckForUpdates.Enabled = True)
                    MsgBox("There was an error checking for updates.", MsgBoxStyle.Information, "Add Adobe Flash to Microsoft EMET")
                End If
            Catch ex As Exception
                ' Ok, we crashed but who cares.  We give an error message.
                'MsgBox("Error while checking for new version.", MsgBoxStyle.Information, Me.Text)
            Finally
                windowObject = Nothing
            End Try
        End If
    End Sub

    Public Function checkForInternetConnection() As Boolean
        Return My.Computer.Network.IsAvailable
    End Function

    ''' <summary>This parses the XML updata data and determines if an update is needed.</summary>
    ''' <param name="xmlData">The XML data from the web site.</param>
    ''' <returns>A Boolean value indicating if the program has been updated or not.</returns>
    Private Function processUpdateXMLData(ByVal xmlData As String) As Boolean
        Try
            Dim xmlDocument As New XmlDocument() ' First we create an XML Document Object.
            xmlDocument.Load(New StringReader(xmlData)) ' Now we try and parse the XML data.

            Dim xmlNode As XmlNode = xmlDocument.SelectSingleNode("/xmlroot")

            Dim remoteVersion As String = xmlNode.SelectSingleNode("version").InnerText.Trim
            Dim remoteBuild As String = xmlNode.SelectSingleNode("build").InnerText.Trim
            Dim shortRemoteBuild As Short

            ' This checks to see if current version and the current build matches that of the remote values in the XML document.
            If remoteVersion.Equals(versionStringWithoutBuild) And remoteBuild.Equals(shortBuild.ToString) Then
                If Short.TryParse(remoteBuild, shortRemoteBuild) And remoteVersion.Equals(versionStringWithoutBuild) Then
                    If shortRemoteBuild < shortBuild Then
                        ' This is weird, the remote build is less than the current build. Something went wrong. So to be safe we're going to return a False value indicating that there is no update to download. Better to be safe.
                        Return False
                    End If
                End If

                ' OK, they match so there's no update to download and update to therefore we return a False value.
                Return False
            Else
                ' We return a True value indicating that there is a new version to download and install.
                Return True
            End If
        Catch ex As XPath.XPathException
            ' Something went wrong so we return a False value.
            Return False
        Catch ex As XmlException
            ' Something went wrong so we return a False value.
            Return False
        Catch ex As Exception
            ' Something went wrong so we return a False value.
            Return False
        End Try
    End Function
End Module