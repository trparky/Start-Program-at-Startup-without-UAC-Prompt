﻿Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Security.AccessControl
Imports System.Security.Principal

Namespace checkForUpdates
    Module checkForUpdatesModule
        ' Change these variables whenever you import this module into a program's code to handle software updates.
        Public Const strMessageBoxTitleText As String = "Start Program at Startup without UAC Prompt"
        Public Const strProgramName As String = "Start Program at Startup without UAC Prompt"
        ' Change these variables whenever you import this module into a program's code to handle software updates.

        Public versionString As String
        Public versionInfo As String() = Application.ProductVersion.Split(".")
        Public strDisplayVersionString As String = $"{versionInfo(VersionPieces.major)}.{versionInfo(VersionPieces.minor)} Build {versionInfo(VersionPieces.build)}"

        Sub New()
            versionString = $"{versionInfo(VersionPieces.major)}.{versionInfo(VersionPieces.minor)} Build {versionInfo(VersionPieces.build)}"
            If File.Exists("tom") Then strDisplayVersionString &= $" (Update {versionInfo(VersionPieces.revision)})"
        End Sub
    End Module

    Class CheckForUpdatesClass
        ' Change these variables whenever you import this module into a program's code to handle software updates.
        Private Const updaterURL As String = "https://www.toms-world.org/download/updater.exe"
        Private Const updaterSHA256URL As String = "https://www.toms-world.org/download/updater.exe.sha2"
        Private Const programUpdateCheckerXMLFile As String = "https://www.toms-world.org/updates/start_program_at_startup_without_uac_prompt.xml"
        Private Const programCode As String = "startprogramewithnouac"
        ' Change these variables whenever you import this module into a program's code to handle software updates.

        Public windowObject As Form1
        Private ReadOnly shortBuild As Short = Short.Parse(versionInfo(VersionPieces.build).Trim)
        Private ReadOnly versionStringWithoutBuild As Double = Double.Parse($"{versionInfo(VersionPieces.major)}.{versionInfo(VersionPieces.minor)}")
        Private ReadOnly longInternalVersion As Long = Long.Parse(versionInfo(VersionPieces.revision))

        Public Sub New(inputWindowObject As Form1)
            windowObject = inputWindowObject
        End Sub

        Private Shared Function ExtractFileFromZIPFile(ByRef memoryStream As MemoryStream, fileToExtract As String, fileToWriteExtractedFileTo As String) As Boolean
            Try
                Using zipFileObject As New Compression.ZipArchive(memoryStream, Compression.ZipArchiveMode.Read)
                    Using fileStream As New FileStream(fileToWriteExtractedFileTo, FileMode.Create)
                        zipFileObject.GetEntry(fileToExtract).Open().CopyTo(fileStream)
                        Return True ' Extraction of file was successful, return True.
                    End Using
                End Using
                Return False ' Something went wrong, return False.
            Catch ex As Exception
                Return False ' Something went wrong, return False.
            End Try
        End Function

        Enum ProcessUpdateXMLResponse As Byte
            noUpdateNeeded
            newVersion
            newerVersionThanWebSite
            parseError
            exceptionError
        End Enum

        ''' <summary>This parses the XML updata data and determines if an update is needed.</summary>
        ''' <param name="xmlData">The XML data from the web site.</param>
        ''' <returns>A Boolean value indicating if the program has been updated or not.</returns>
        Private Function ProcessUpdateXMLData(xmlData As String, ByRef remoteVersion As String, ByRef remoteBuild As String) As ProcessUpdateXMLResponse
            Try
                Dim xmlDocument As New XmlDocument() ' First we create an XML Document Object.
                xmlDocument.Load(New StringReader(xmlData)) ' Now we try and parse the XML data.
                Dim xmlNode As XmlNode = xmlDocument.SelectSingleNode("/xmlroot")

                remoteVersion = xmlNode.SelectSingleNode("version").InnerText.Trim
                remoteBuild = xmlNode.SelectSingleNode("build").InnerText.Trim

                Dim longInternalVersionFromXML As Long = 0
                If xmlNode.SelectSingleNode("internalversion") IsNot Nothing Then
                    If Long.TryParse(xmlNode.SelectSingleNode("internalversion").InnerText.Trim, longInternalVersionFromXML) Then
                        If longInternalVersionFromXML = longInternalVersion Then ' If the internal version from the XML file matches the internal version from the program itself, we return a noUpdateNeeded value.
                            Return ProcessUpdateXMLResponse.noUpdateNeeded
                        ElseIf longInternalVersionFromXML > longInternalVersion Then ' If the internal version from the XML file is greater than the internal version from the program itself, we return a newVersion value.
                            Return ProcessUpdateXMLResponse.newVersion
                        ElseIf longInternalVersionFromXML < longInternalVersion Then
                            Return ProcessUpdateXMLResponse.newerVersionThanWebSite ' If the internal version from the XML file is less than the internal version from the program itself, we return a newerVersionThanWebSite value.
                        End If
                    Else
                        Return ProcessUpdateXMLResponse.parseError ' Something went wrong, so we return a parseError value.
                    End If
                Else
                    Return ProcessUpdateXMLResponse.exceptionError ' Something really went wrong, so we return a exceptionError value.
                End If
            Catch ex As Exception
                ' Something went wrong so we return an exceptionError value.
                Return ProcessUpdateXMLResponse.exceptionError
            End Try

            ' We return a noUpdateNeeded flag.
            Return ProcessUpdateXMLResponse.noUpdateNeeded
        End Function

        Private Shared Function CheckFolderPermissionsByACLs(folderPath As String) As Boolean
            Try
                Dim directoryACLs As DirectorySecurity = Directory.GetAccessControl(folderPath)
                Dim directoryAccessRights As FileSystemAccessRule

                For Each rule As AuthorizationRule In directoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
                    If rule.IdentityReference.Value.Equals(WindowsIdentity.GetCurrent.User.Value, StringComparison.OrdinalIgnoreCase) Then
                        directoryAccessRights = DirectCast(rule, FileSystemAccessRule)

                        If directoryAccessRights.AccessControlType = AccessControlType.Allow AndAlso directoryAccessRights.FileSystemRights = (FileSystemRights.Read Or FileSystemRights.Modify Or FileSystemRights.Write Or FileSystemRights.FullControl) Then
                            Return True
                        End If
                    End If
                Next

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function CreateNewHTTPHelperObject() As HttpHelper
            Dim httpHelper As New HttpHelper With {
                .SetUserAgent = CreateHTTPUserAgentHeaderString(),
                .UseHTTPCompression = True,
                .SetProxyMode = True
            }
            httpHelper.AddHTTPHeader("PROGRAM_NAME", strProgramName)
            httpHelper.AddHTTPHeader("PROGRAM_VERSION", versionString)
            httpHelper.AddHTTPHeader("OPERATING_SYSTEM", GetFullOSVersionString())

            Return httpHelper
        End Function

        Private Shared Function SHA256ChecksumStream(ByRef stream As Stream) As String
            Using SHA256Engine As New Security.Cryptography.SHA256CryptoServiceProvider
                Return BitConverter.ToString(SHA256Engine.ComputeHash(stream)).ToLower().Replace("-", "").Trim
            End Using
        End Function

        Private Function VerifyChecksum(urlOfChecksumFile As String, ByRef memStream As MemoryStream, ByRef httpHelper As HttpHelper, boolGiveUserAnErrorMessage As Boolean) As Boolean
            Dim checksumFromWeb As String = Nothing
            memStream.Position = 0

            Try
                If httpHelper.GetWebData(urlOfChecksumFile, checksumFromWeb) Then
                    Dim regexObject As New Regex("([a-zA-Z0-9]{64})")

                    ' Checks to see if we have a valid SHA256 file.
                    If regexObject.IsMatch(checksumFromWeb) Then
                        ' Now that we have a valid SHA256 file we need to parse out what we want.
                        checksumFromWeb = regexObject.Match(checksumFromWeb).Groups(1).Value.Trim()

                        ' Now we do the actual checksum verification by passing the name of the file to the SHA256() function
                        ' which calculates the checksum of the file on disk. We then compare it to the checksum from the web.
                        If SHA256ChecksumStream(memStream).Equals(checksumFromWeb, StringComparison.OrdinalIgnoreCase) Then
                            Return True ' OK, things are good; the file passed checksum verification so we return True.
                        Else
                            ' The checksums don't match. Oops.
                            If boolGiveUserAnErrorMessage Then
                                windowObject.Invoke(Sub() MsgBox("There was an error in the download, checksums don't match. Update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                            End If

                            Return False
                        End If
                    Else
                        If boolGiveUserAnErrorMessage Then
                            windowObject.Invoke(Sub() MsgBox("Invalid SHA2 file detected. Update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                        End If

                        Return False
                    End If
                Else
                    If boolGiveUserAnErrorMessage Then
                        windowObject.Invoke(Sub() MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                    End If

                    Return False
                End If
            Catch ex As Exception
                If boolGiveUserAnErrorMessage Then
                    windowObject.Invoke(Sub() MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                End If

                Return False
            End Try
        End Function

        Private Sub DownloadAndPerformUpdate()
            Dim httpHelper As HttpHelper = CreateNewHTTPHelperObject()

            Using memoryStream As New MemoryStream()
                If Not httpHelper.DownloadFile(updaterURL, memoryStream, False) Then
                    windowObject.Invoke(Sub() MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                    Exit Sub
                End If

                If Not VerifyChecksum(updaterSHA256URL, memoryStream, httpHelper, True) Then
                    windowObject.Invoke(Sub() MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                    Exit Sub
                End If

                memoryStream.Position = 0

                Using fileStream As New FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "updater.exe"), FileMode.OpenOrCreate)
                    memoryStream.CopyTo(fileStream)
                End Using
            End Using

            Dim startInfo As New ProcessStartInfo With {
                .FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "updater.exe"),
                .Arguments = $"--programcode={programCode}"
            }
            If Not CheckFolderPermissionsByACLs(AppDomain.CurrentDomain.BaseDirectory) Then startInfo.Verb = "runas"
            Process.Start(startInfo)

            Process.GetCurrentProcess.Kill()
        End Sub

        ''' <summary>Creates a User Agent String for this program to be used in HTTP requests.</summary>
        ''' <returns>String type.</returns>
        Private Shared Function CreateHTTPUserAgentHeaderString() As String
            Dim versionInfo As String() = Application.ProductVersion.Split(".")
            Dim versionString As String = $"{versionInfo(0)}.{versionInfo(1)} Build {versionInfo(2)}"
            Return $"{strProgramName} version {versionString} on {GetFullOSVersionString()}"
        End Function

        Private Shared Function GetFullOSVersionString() As String
            Try
                Dim intOSMajorVersion As Integer = Environment.OSVersion.Version.Major
                Dim intOSMinorVersion As Integer = Environment.OSVersion.Version.Minor
                Dim dblDOTNETVersion As Double = Double.Parse($"{Environment.Version.Major}.{Environment.Version.Minor}")
                Dim strOSName As String

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
                ElseIf intOSMajorVersion = 11 Then
                    strOSName = "Windows 11"
                Else
                    strOSName = $"Windows NT {intOSMajorVersion}.{intOSMinorVersion}"
                End If

                Return $"{strOSName} {If(Environment.Is64BitOperatingSystem, "64", "32")}-bit (Microsoft .NET {dblDOTNETVersion })"
            Catch ex As Exception
                Try
                    Return $"Unknown Windows Operating System ({Environment.OSVersion.VersionString})"
                Catch ex2 As Exception
                    Return "Unknown Windows Operating System"
                End Try
            End Try
        End Function

        Private Function BackgroundThreadMessageBox(strMsgBoxPrompt As String, strMsgBoxTitle As String) As MsgBoxResult
            If windowObject.InvokeRequired Then
                Return windowObject.Invoke(New Func(Of MsgBoxResult)(Function() MsgBox(strMsgBoxPrompt, MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMsgBoxTitle)))
            Else
                Return MsgBox(strMsgBoxPrompt, MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMsgBoxTitle)
            End If
        End Function

        Public Sub CheckForUpdates(Optional boolShowMessageBox As Boolean = True)
            windowObject.Invoke(Sub()
                                    windowObject.btnCheckForUpdates.Enabled = False
                                End Sub)

            If Not My.Computer.Network.IsAvailable Then
                windowObject.Invoke(Sub() MsgBox("No Internet connection detected.", MsgBoxStyle.Information, strMessageBoxTitleText))
            Else
                Try
                    Dim xmlData As String = Nothing
                    Dim httpHelper As HttpHelper = CreateNewHTTPHelperObject()

                    If httpHelper.GetWebData(programUpdateCheckerXMLFile, xmlData, False) Then
                        Dim remoteVersion As String = Nothing
                        Dim remoteBuild As String = Nothing
                        Dim response As ProcessUpdateXMLResponse = ProcessUpdateXMLData(xmlData, remoteVersion, remoteBuild)

                        If response = ProcessUpdateXMLResponse.newVersion Then
                            If BackgroundThreadMessageBox($"An update to {strProgramName} (version {remoteVersion} Build {remoteBuild}) is available to be downloaded, do you want to download and update to this new version?", strMessageBoxTitleText) = MsgBoxResult.Yes Then
                                DownloadAndPerformUpdate()
                            Else
                                windowObject.Invoke(Sub() MsgBox("The update will not be downloaded.", MsgBoxStyle.Information, strMessageBoxTitleText))
                            End If
                        ElseIf response = ProcessUpdateXMLResponse.noUpdateNeeded AndAlso boolShowMessageBox Then
                            windowObject.Invoke(Sub() MsgBox("You already have the latest version, there is no need to update this program.", MsgBoxStyle.Information, strMessageBoxTitleText))
                        ElseIf (response = ProcessUpdateXMLResponse.parseError Or response = ProcessUpdateXMLResponse.exceptionError) AndAlso boolShowMessageBox Then
                            windowObject.Invoke(Sub() MsgBox("There was an error when trying to parse the response from the server.", MsgBoxStyle.Critical, strMessageBoxTitleText))
                        ElseIf response = ProcessUpdateXMLResponse.newerVersionThanWebSite AndAlso boolShowMessageBox Then
                            windowObject.Invoke(Sub() MsgBox("This is weird, you have a version that's newer than what's listed on the web site.", MsgBoxStyle.Information, strMessageBoxTitleText))
                        End If
                    Else
                        If boolShowMessageBox Then windowObject.Invoke(Sub() MsgBox("There was an error checking for updates.", MsgBoxStyle.Information, strMessageBoxTitleText))
                    End If
                Catch ex As Exception
                    ' Ok, we crashed but who cares.
                Finally
                    windowObject.Invoke(Sub()
                                            windowObject.btnCheckForUpdates.Enabled = True
                                        End Sub)
                    windowObject = Nothing
                End Try
            End If
        End Sub
    End Class

    Public Enum VersionPieces As Short
        major = 0
        minor = 1
        build = 2
        revision = 3
    End Enum
End Namespace