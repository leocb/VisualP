Imports System.IO
Imports Microsoft.Win32
Imports System.Text.RegularExpressions

Public Class Main
    Public Shared ReadOnly mnNew As RoutedCommand = New RoutedCommand()
    Public Shared ReadOnly mnOpen As RoutedCommand = New RoutedCommand()
    Public Shared ReadOnly mnSave As RoutedCommand = New RoutedCommand()
    Public Shared ReadOnly mnSaveAs As RoutedCommand = New RoutedCommand()
    Public Shared ReadOnly mnClose As RoutedCommand = New RoutedCommand()

    Dim Paths As New Dictionary(Of TabItem, String)



    Private Sub Main_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'open via .vps file
        If (AppDomain.CurrentDomain.SetupInformation.ActivationArguments IsNot Nothing AndAlso AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData IsNot Nothing AndAlso AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Any()) Then
            For Each s As String In AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData
                Dim UriAddr As New Uri(s)
                CustomLoad(UriAddr.LocalPath, Regex.Replace(UriAddr.LocalPath, ".*\\", ""))
            Next
            Exit Sub
        End If

        'normal program open
        CreateNewTab(New CanvasControl())
    End Sub

    Private Sub MnFileNew_Click(sender As Object, e As RoutedEventArgs)
        CreateNewTab(New CanvasControl())
    End Sub
    Private Sub MnFileOpen_Click(sender As Object, e As RoutedEventArgs)
        Dim OpenFileDialog As New OpenFileDialog()
        OpenFileDialog.Filter = "Visual Programming Script (*.vps)|*.vps"

        If (OpenFileDialog.ShowDialog = True) Then
            If Paths.ContainsValue(OpenFileDialog.FileName) Then

                Dim pair As KeyValuePair(Of TabItem, String)
                Dim SelectTab As New TabItem

                For Each pair In Paths
                    If pair.Value = OpenFileDialog.FileName Then
                        SelectTab = pair.Key
                    End If
                Next

                If (MsgBox("File " & OpenFileDialog.SafeFileName & " is already loaded" & vbNewLine & "Do you want to reload the file?", vbYesNo, "Warning") = MsgBoxResult.Yes) Then
                    Paths.Remove(TabControl.SelectedItem)
                    TabControl.Items.Remove(SelectTab)
                    CustomLoad(OpenFileDialog.FileName, OpenFileDialog.SafeFileName)

                Else
                    TabControl.SelectedItem = SelectTab

                End If
            Else

                CustomLoad(OpenFileDialog.FileName, OpenFileDialog.SafeFileName)
            End If
        End If
    End Sub
    Private Sub MnFileSave_Click(sender As Object, e As RoutedEventArgs)
        Dim Path As String = ""
        Dim CurrTab As TabItem = TabControl.SelectedItem
        Paths.TryGetValue(CurrTab, Path)

        If Not Path = "" Then
            CustomSave(TabControl.SelectedContent.children(0), Path)

        Else
            Dim SaveFileDialog As New SaveFileDialog()
            SaveFileDialog.Filter = "Visual Programming Script (*.vps)|*.vps"

            If (SaveFileDialog.ShowDialog() = True) Then
                Path = SaveFileDialog.FileName
                Paths.Item(CurrTab) = Path

                CustomSave(TabControl.SelectedContent.children(0), Path)

                CurrTab.Header = SaveFileDialog.SafeFileName
            End If
        End If

    End Sub
    Private Sub MnFileSaveAs_Click(sender As Object, e As RoutedEventArgs)
        Dim Path As String = ""
        Dim CurrTab As TabItem = TabControl.SelectedItem
        Dim SaveFileDialog As New SaveFileDialog()
        SaveFileDialog.Filter = "Visual Programming Script (*.vps)|*.vps"

        If (SaveFileDialog.ShowDialog() = True) Then
            Path = SaveFileDialog.FileName
            Paths.Item(CurrTab) = Path

            CustomSave(TabControl.SelectedContent.children(0), Path)

            CurrTab.Header = SaveFileDialog.SafeFileName
        End If
    End Sub
    Private Sub MnFileClose_Click(sender As Object, e As RoutedEventArgs)
        Paths.Remove(TabControl.SelectedItem)
        TabControl.Items.Remove(TabControl.SelectedItem)
    End Sub
    Private Sub MnExit_Click(sender As Object, e As RoutedEventArgs) Handles MnExit.Click
        End
    End Sub

    Private Sub CreateNewTab(CanvasControl As CanvasControl, Optional Path As String = "", Optional TabHeader As String = "New Script")
        If CanvasControl IsNot Nothing Then
            Dim TI As New TabItem()
            Dim Grid As New Grid
            Dim TC As New TextBox
            Dim GS As New GridSplitter
            Dim CC As CanvasControl = CanvasControl
            Dim CD1 As New ColumnDefinition
            Dim CD2 As New ColumnDefinition
            CD1.Width = New GridLength(8, GridUnitType.Star)
            CD2.Width = New GridLength() '3, GridUnitType.Star)

            Grid.ColumnDefinitions.Add(CD1)
            Grid.ColumnDefinitions.Add(CD2)
            Grid.Children.Add(CC)
            Grid.Children.Add(GS)
            Grid.Children.Add(TC)

            CC.TextCode = TC

            GS.SetValue(Grid.ColumnProperty, 1)
            GS.SetValue(WidthProperty, CDbl(5))
            GS.SetValue(HeightProperty, Double.NaN)
            GS.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left)

            TC.SetValue(Grid.ColumnProperty, 1)
            TC.SetValue(MarginProperty, New Thickness(5, 0, 0, 0))
            TC.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto)
            TC.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto)
            TC.TextWrapping = TextWrapping.NoWrap
            TC.AcceptsReturn = True


            TI.Content = Grid
            TI.Header = TabHeader

            TabControl.Items.Insert(0, TI)
            TabControl.SelectedIndex = 0

            Paths.Add(TI, Path)
        End If
    End Sub

    Sub CustomSave(CC As CanvasControl, Path As String)
        CC.SaveToFile(Path)
    End Sub

    Sub CustomLoad(Path As String, SafeFileName As String)
        CreateNewTab(LoadFromFile(Path), Path, SafeFileName)
    End Sub
    Public Function LoadFromFile(FilePath As String) As CanvasControl
        Dim File As StreamReader = My.Computer.FileSystem.OpenTextFileReader(FilePath)
        Dim Encrypter As New Simple3Des("PGhhvbjJEieNxOstS7kpwT2RyMvJ9V7X69QFG2wFZj4uoKLC8QqBqSCz94yblYV1")

        Dim MegaString As String = Encrypter.DecryptData(File.ReadToEnd())
        If MegaString = "" Then Return Nothing 'error handling

        Dim LS() As String = MegaString.Split("|")
        File.Close()

        If LS.Count <= 5 Then
            MessageBox.Show("File is corrupted and cannot be loaded" & vbNewLine & "Error 0x00000053", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End If

        Dim NewCC As New CanvasControl

        Dim VarMetaToLoad As Integer = LS(0)
        Dim CompsToLoad As Integer = LS(VarMetaToLoad * 5 + 1)
        Dim LinhasToLoad As Integer = LS(VarMetaToLoad * 5 + 1 + CompsToLoad * 6 + 1)

        '======================================================================================== Recreate var structure
        For i = 1 To 5 * VarMetaToLoad Step 5
            Dim NVS As New VarsStruct
            NVS.VarsCompsOnCanvas = New List(Of Object)
            NVS.ReadableName = LS(i)
            NVS.HashedName = LS(i + 1)
            NVS.Type = LS(i + 2)
            If LS(i + 3) = "true" Then NVS.IsArray = True Else NVS.IsArray = False
            NVS.DefaultValue = LS(i + 4)

            NewCC.ConstructNewVar(NVS) '-------------Add current var structure to the new canvas control
        Next

        '======================================================================================== Recreate components on canvas
        For i = (2 + 5 * VarMetaToLoad) To (1 + 5 * VarMetaToLoad + 6 * CompsToLoad) Step 6
            Dim objToAdd As UIElement
            objToAdd = Nothing

            If LS(i) = GetType(ComponentsLogic).ToString Then '---------------------ADD COMPONENT
                If Not (LS(i + 1) = ComponentType.EBegin Or LS(i + 1) = ComponentType.EEnd) Then 'se for o function begin ou end, nao precisamos adicionar
                    Dim j As New ComponentsLogic(NewCC.ComponentsCanvas, LS(i + 1))
                    objToAdd = j
                End If

            ElseIf LS(i) = GetType(ConstantLogic).ToString Then '-------------------ADD CONSTANT
                Dim j As New ConstantLogic(LS(i + 1), NewCC.ComponentsCanvas)
                j.SetCurrentConstValue(LS(i + 2))
                objToAdd = j

            Else '------------------------------------------------------------------ADD VAR
                Dim VarStruct As New VarsStruct
                NewCC.VarsDict.TryGetValue(LS(i + 1), VarStruct)

                Dim IsSet As Boolean
                If LS(i + 2) = "True" Then IsSet = True Else IsSet = False

                Dim j As New VariablesLogic(NewCC.ComponentsCanvas, VarStruct, IsSet)

                VarStruct.VarsCompsOnCanvas.Add(j)

                objToAdd = j
            End If

            '--------------------------------------------------Add to canvas, rename and reposition
            If objToAdd IsNot Nothing Then
                objToAdd.SetValue(NameProperty, LS(i + 3))
                NewCC.AddToCanvas(objToAdd, LS(i + 3))
                objToAdd.SetValue(Canvas.LeftProperty, CDbl(LS(i + 4)))
                objToAdd.SetValue(Canvas.TopProperty, CDbl(LS(i + 5)))

                AddHandler objToAdd.MouseDown, AddressOf NewCC.MoveControleDOWN

            Else
                If LS(i + 1) = ComponentType.EBegin Then
                    Dim o As UIElement = NewCC.GetObjectOnCanvasByName("Comp0")
                    o.SetValue(Canvas.LeftProperty, CDbl(LS(i + 4)))
                    o.SetValue(Canvas.TopProperty, CDbl(LS(i + 5)))
                End If
                If LS(i + 1) = ComponentType.EEnd Then
                    Dim o As UIElement = NewCC.GetObjectOnCanvasByName("Comp1")
                    o.SetValue(Canvas.LeftProperty, CDbl(LS(i + 4)))
                    o.SetValue(Canvas.TopProperty, CDbl(LS(i + 5)))
                End If
            End If
        Next


        '======================================================================================== Recreate Lines

        'MegaString &= "|" & l.GetTipo
        'MegaString &= "|" & l.GetParentCompIni.name
        'MegaString &= "|" & l.GetCompIni.name
        'MegaString &= "|" & l.GetParentCompFin.name
        'MegaString &= "|" & l.GetCompFin.name

        For i = (3 + 5 * VarMetaToLoad + 6 * CompsToLoad) To (1 + 5 * VarMetaToLoad + 6 * CompsToLoad + 5 * LinhasToLoad) Step 5
            Dim l As Linha

            l = New Linha(LS(i), NewCC.ComponentsCanvas)
            l.SetParentCompIni(NewCC.GetObjectOnCanvasByName(LS(i + 1)))
            l.SetCompIni(NewCC.GetObjectOnCanvasByName(LS(i + 1)).GetOutputByName(LS(i + 2)))
            l.SetParentCompFin(NewCC.GetObjectOnCanvasByName(LS(i + 3)))
            l.SetCompFin(NewCC.GetObjectOnCanvasByName(LS(i + 3)).GetInputByName(LS(i + 4)))

            l.GetCompIni.AddLinha(l)
            l.GetCompFin.AddLinha(l)

            l.StopFollowTheMouse()

            NewCC.ComponentsCanvas.Children.Add(l.LinhaPath) 'Adicionamos o path no canvas (que é o desenho da linha)
            l.AtualizarLinha()

            NewCC.LinhaNameCounter += 1 'Increment line namer
            l.Name = "Ln" & NewCC.LinhaNameCounter.ToString 'Line name (For save porpuses)
            NewCC.LinhasOnCanvas.Add(l) 'Add Line to list (For save)
        Next

        Return NewCC

    End Function
End Class
