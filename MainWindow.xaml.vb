Imports MahApps.Metro.Controls
Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Collections.Concurrent
Imports System.Management
Imports System.Net




Public Class MainWindow
    Inherits MetroWindow

    Public Shared serverlist As List(Of DataServerList) = New List(Of DataServerList)
    Dim inputFile As String = "\\gsodata2\PSS Data\~Programs\~PSC Scripts\Server Storage Stats\input files\target servers.txt"
    Dim myservers As DataServerList = New DataServerList()
    Dim selectedList As DataServerList = New DataServerList()
    Dim WithEvents BackgroundWorker1 As BackgroundWorker
    Dim lstMyObject As List(Of Object) = New List(Of Object)()
    Dim _globallinecount As Integer
    Dim _shareName As List(Of String) = New List(Of String)


    Private FullPathsToPrograms As List(Of String) = New List(Of String)

    Private Sub Window_Closed(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Application.Current.Shutdown()
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.EventArgs)
        serverlist.Clear()
        ReadInputFile()
        ListShares()
        TB_ServerList.Text = "\\gsodata2\PSS Data\~Programs\~PSC Scripts\Server Storage Stats\input files\target servers.txt"
        rb_seperate.IsChecked = True
    End Sub


    Private Sub BT_Go_Click(sender As Object, e As RoutedEventArgs) Handles BT_Go.Click

        Dim search1 As String = inputText.Text
        'Dim dirs As String = IO.Path.GetFullPath(IO.Path.Combine(path1, path2))
        Dim dirs As List(Of String) = New List(Of String)
        Dim searchPattern As String = "*" & inputText.Text & "*"
        Dim searchResults() As String = Split(searchPattern, vbCrLf)

        If (rb_seperate.IsChecked) Then
            If (search1.Contains("-") Or search1.Contains("_")) Then
                MsgBox("Search by matter instead")
            Else

            End If
            ' For Each search In searchResults
            For i As Integer = 0 To searchResults.Count - 1
                For Each foo In serverlist
                    Dim servers As String = "\\" + foo.serverName.ToString
                    Dim tempServer = "\\" + "lvddata2"
                    For x As Integer = 0 To _shareName.Count - 1
                        Dim path = IO.Path.Combine(tempServer, _shareName(x))
                        dirs.Add(path)
                    Next
                Next
                Dim resultingPath As List(Of String) = dirs.Distinct().ToList()
                For Each result In resultingPath
                    CollectFiles(result, searchResults(i))
                Next
            Next
            showResults()
        Else
            For Each search In searchResults
                For Each foo In serverlist
                    Dim servers As String = "\\" + foo.serverName.ToString
                    Dim tempServer = "\\" + "lvddata2"
                    For i As Integer = 0 To _shareName.Count - 1
                        Dim path = IO.Path.Combine(tempServer, _shareName(i))
                        dirs.Add(path)
                    Next
                Next
                Dim resultingPath As List(Of String) = dirs.Distinct().ToList()
                Dim splitMatters() As String = Split(search, "_")
                Dim firstMatter = splitMatters(0)
                Dim secondMatter = splitMatters(1)

                For Each result In resultingPath
                    Dim matterCollection As List(Of String) = CollectFiles(result, firstMatter)
                    For Each col In matterCollection
                        CollectMatter(col, secondMatter)
                    Next
                Next
            Next
            showResults()
        End If
    End Sub

    Public Function ReadInputFile() As Integer
        Dim linecount As Integer = 0
        Using sr As New StreamReader(inputFile)

            While Not sr.EndOfStream
                Dim line As String = sr.ReadLine
                Dim skipLine = sr.ReadLine
                Dim columns As String() = skipLine.Split(",")
                myservers.serverName = columns(1)
                serverlist.Add(New DataServerList() With {
                            .serverid = columns(0),
                            .serverName = columns(1),
                            .IsChecked = True
                            })
            End While
            listView1.ItemsSource = serverlist
        End Using
        Return 1
    End Function

    Public Sub ListShares()
        Dim Path As ManagementPath = New ManagementPath
        Dim Shares As ManagementClass = Nothing
        Dim CO As ConnectionOptions = New ConnectionOptions
        CO.Impersonation = ImpersonationLevel.Impersonate
        CO.Authentication = AuthenticationLevel.Default
        CO.EnablePrivileges = True
        CO.Username = "cmsvc"
        CO.Password = "cm2016lw!"
        Path.Server = "LVDDATA2" ' use . for local server, else server name
        'For Each foo In serverlist
        '    ' Path.Server = foo.serverName
        '    Dim newServer = foo.serverName
        '    Path.Server = newServer

        '    Dim myServers As List(Of String) = New List(Of String)
        '    myServers.Add(newServer)
        '    Dim resultingPath As List(Of String) = myServers.Distinct().ToList()

        '    For Each newpath In resultingPath
        Path.NamespacePath = "\root\cimv2"
        Path.RelativePath = "Win32_Share"

        Dim password As String = "password"
        Dim username As String = "username"
        Dim domain As String = "domain"

        Dim theNetworkCredential As NetworkCredential = New NetworkCredential(username, password, domain)
        Dim theNetcache As CredentialCache = New CredentialCache()
        CredentialCache.DefaultNetworkCredentials.Domain = domain
        CredentialCache.DefaultNetworkCredentials.UserName = username
        CredentialCache.DefaultNetworkCredentials.Password = password


        Dim lNetWorkCredential As List(Of System.Net.NetworkCredential) = New System.Collections.Generic.List(Of System.Net.NetworkCredential)
        lNetWorkCredential.Add(theNetworkCredential)

        Dim Scope As ManagementScope = New ManagementScope(Path, CO)
        Scope.Connect()

        'If (Scope.IsConnected) Then
        '    MsgBox("Connected")
        'Else
        '    MsgBox("Error Connecting")
        'End If


        Dim os As ManagementClass = New ManagementClass("Win32_OperatingSystem")
        Dim inParams As ManagementBaseObject = os.GetMethodParameters("Win32Shutdown")
        inParams("Flags") = "2"
        inParams("Reserved") = "0"

        Dim Options As ObjectGetOptions = New ObjectGetOptions(Nothing, New TimeSpan(0, 0, 0, 5), True)
        Try
            Shares = New ManagementClass(Scope, Path, Options)
            Dim MOC As ManagementObjectCollection = Shares.GetInstances()
            For Each mo As ManagementObject In MOC
                '  Console.WriteLine("{0} - {1} - {2}", mo("Name"), mo("Description"), mo("Path"))
                Dim shareName = mo("name")
                Dim startingShare1 = "CMA"
                Dim startingShare2 = "CMI"
                Dim startingShare3 = "CMD"
                If InStr(1, shareName, startingShare1) = 1 Or InStr(1, shareName, startingShare2) = 1 Or InStr(1, shareName, startingShare3) = 1 Then
                    Console.WriteLine(shareName)
                    _shareName.Add(shareName)
                    'For Each share In _shareName
                    '    MsgBox(share)
                    'Next
                End If
            Next

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        Finally

            If Shares IsNot Nothing Then
                Shares.Dispose()
            End If
        End Try
    End Sub


    Public Function CollectMatter(ByVal dir As String, ByVal pattern As String)
        Dim output = "\\lvdiprodata\EXPORTS01\PS100000\Erin Development Projects\Output\output_matter.txt"
        Dim directory As DirectoryInfo = New DirectoryInfo(dir)
        Dim dire = Directory.GetDirectories(pattern)
        Dim sb As New StringBuilder()
        Dim queue As ConcurrentQueue(Of String) = New ConcurrentQueue(Of String)
        For Each result In dire.[Select](Function(file) file.FullName)
            queue.Enqueue(result)
            Dim underscore = result.Replace("__", "_")
            resultsBox.Items.Add(underscore)

            FullPathsToPrograms.Add(result)
            For Each item As Object In resultsBox.Items
                item.Replace("__", "_")
                sb.AppendFormat("{0} {1}", item, Environment.NewLine)
            Next

            Dim writer As New System.IO.StreamWriter(output, False)
            writer.Write(sb.ToString())
            writer.WriteLine()
            writer.Close()
        Next

        Return queue.AsEnumerable().ToList()
    End Function



    Public Function CollectFiles(ByVal directory As String, ByVal pattern As String) As List(Of String)
        Dim queue As ConcurrentQueue(Of String) = New ConcurrentQueue(Of String)()
        InternalCollectFiles(directory, pattern, queue)
        Return queue.AsEnumerable().ToList()
    End Function

    Private Sub InternalCollectFiles(ByVal dir As String, ByVal pattern As String, ByVal queue As ConcurrentQueue(Of String))
        Dim directory As DirectoryInfo = New DirectoryInfo(dir)
        Dim dire = directory.GetDirectories(pattern)
        Dim sb As New StringBuilder()
        Dim output = "\\lvdiprodata\EXPORTS01\PS100000\Erin Development Projects\Output\output.txt"

        For Each result In dire.[Select](Function(file) file.FullName)
            queue.Enqueue(result)
            Dim underscore = result.Replace("_", "__")
            resultsBox.Items.Add(underscore)

            FullPathsToPrograms.Add(result)

            For Each item As Object In resultsBox.Items
                item.Replace("__", "_")
                sb.AppendFormat("{0} {1}", item, Environment.NewLine)
            Next

            Dim writer As New System.IO.StreamWriter(output, False)
            writer.Write(sb.ToString())
            writer.WriteLine()
            writer.Close()
        Next
    End Sub


    Private Sub BT_Input_Click(sender As Object, e As RoutedEventArgs) Handles BT_Input.Click
        Dim dlg As New Microsoft.Win32.OpenFileDialog

        dlg.DefaultExt = ".txt" ' Default file extension
        dlg.Filter = "Server target list (.txt)|*.txt" ' Filter files by extension

        ' Show open file dialog box
        dlg.Multiselect = True
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results
        If result = True Then
            TB_ServerList.Text = dlg.FileName
        End If
    End Sub

    Private Sub BT_Output_Click(sender As Object, e As RoutedEventArgs) Handles BT_Output.Click
        Dim dlg As New Microsoft.Win32.SaveFileDialog
        dlg.InitialDirectory = Path.GetDirectoryName(Directory.GetCurrentDirectory)
        dlg.FileName = "ClientInventory_Report.csv"
        dlg.DefaultExt = ".csv" ' Default file extension
        dlg.Filter = "Comma Delimited (.csv)|*.csv" ' Filter files by extension

        ' Show open file dialog box
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results
        If result = True Then
            ' Open document
            TB_Output.Text = dlg.FileName
        End If
    End Sub

    Private Sub ImagePanel_Drop(ByVal sender As Object, ByVal e As DragEventArgs)

    End Sub


    Private Sub chkSelectAll_Checked(ByVal sender As Object, e As RoutedEventArgs)
        'For i As Integer = 0 To listView1.Items.Count - 1
        '    listView1.SelectedItems.Add(listView1.Items(i))
        '    TryCast(listView1.Items(i), DataServerList).IsChecked = True
        'Next
        Dim checkBox = TryCast(sender, CheckBox)
        If checkBox Is Nothing Then Return

        For Each item In listView1.ItemsSource
            item.IsChecked = True
        Next

    End Sub

    Private Sub chkUnselectAll_Unchecked(ByVal sender As Object, e As RoutedEventArgs)

        Dim checkBox = TryCast(sender, CheckBox)
        If checkBox Is Nothing Then Return

        For Each item In serverlist
            item.IsChecked = checkBox.IsChecked.Value = False
        Next
    End Sub

    Public Sub checkbox_IsChecked()
        If listView1.Items.Count > 0 Then

            For i As Integer = 0 To listView1.Items.Count - 1
                TryCast(listView1.Items(i), DataServerList).IsChecked = True
            Next
            listView1.UpdateLayout()
        End If
    End Sub

    Public Sub checkbox_IsUnchecked()
        If listView1.Items.Count > 0 Then

            For i As Integer = 0 To listView1.Items.Count - 1
                TryCast(listView1.Items(i), DataServerList).IsChecked = False
            Next
            listView1.UpdateLayout()
        End If

    End Sub


    Private Sub showResults()
        FirstGrid.Visibility = Visibility.Hidden
        SecondGrid.Visibility = Visibility.Hidden
        ThirdGrid.Visibility = Visibility.Visible
    End Sub


    Private Sub viewCustomized_Click(sender As Object, e As RoutedEventArgs) Handles viewCustomized.Click
        SecondGrid.Visibility = Visibility.Hidden
        viewCustomized.Visibility = Visibility.Hidden

        ' customizeBox.Visibility = Visibility.Visible
        listView1.Visibility = Visibility.Visible
        selectAllCheckbox.Visibility = Visibility.Visible
        closeCustomized.Visibility = Visibility.Visible
    End Sub

    Private Sub closeCustomized_Click(sender As Object, e As RoutedEventArgs) Handles closeCustomized.Click
        SecondGrid.Visibility = Visibility.Visible
        viewCustomized.Visibility = Visibility.Visible

        ' customizeBox.Visibility = Visibility.Hidden
        listView1.Visibility = Visibility.Hidden
        selectAllCheckbox.Visibility = Visibility.Hidden
        closeCustomized.Visibility = Visibility.Hidden
    End Sub

    Private Sub closeResults_Click(sender As Object, e As RoutedEventArgs) Handles closeResults.Click
        If (resultsBox.Items IsNot Nothing) Then
            '  MsgBox("Results Successfully written to file.")
        End If
        resultsBox.Items.Clear()
        FirstGrid.Visibility = Visibility.Visible
        SecondGrid.Visibility = Visibility.Visible

        ThirdGrid.Visibility = Visibility.Hidden
    End Sub

    Private Sub resultsBox_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles resultsBox.MouseDoubleClick

        Dim fullpath As String = FullPathsToPrograms(resultsBox.SelectedIndex)

        Dim MyInstance As New System.Diagnostics.Process
        Dim info As ProcessStartInfo = New ProcessStartInfo((fullpath))
        MyInstance.StartInfo = info
        MyInstance.Start()
    End Sub

    Private Sub listView1_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' selectedList = serverlist(listView1.SelectedIndex)


        '   selectedList = serverlist(listView1.SelectedIndex)
        ' MessageBox.Show("Selected server location = " & selectedList.serverid & vbCrLf & "Selected Server name = " & selectedList.serverName)

        '  GetSubDir(sender, selectedList.serverid)
    End Sub


End Class
