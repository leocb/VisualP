Imports System.Text.RegularExpressions
 Public Class CreateNewVar
    Dim Type As IOTypes = IOTypes.TBoolean
    Dim CanvasControl As CanvasControl
    Dim VarStruct As VarsStruct
    Dim FromExisting As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(ByRef CanvasControl As CanvasControl)
        InitializeComponent()

        Me.CanvasControl = CanvasControl
        FromExisting = False
    End Sub

    Public Sub New(ByRef CanvasControl As CanvasControl, ByVal VarStruct As VarsStruct)
        InitializeComponent()

        lblWarning.Content = "Warning: Changing any value will reset all connections to this var"
        Me.CanvasControl = CanvasControl
        Me.VarStruct = VarStruct
        FromExisting = True

    End Sub

    Private Sub txtDefault_LostKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs) Handles txtDefault.LostKeyboardFocus
        Select Case Type
            Case IOTypes.TBoolean
                If Not (txtDefault.Text.ToUpper = "TRUE" Or txtDefault.Text.ToUpper = "FALSE") Then
                    txtDefault.Text = "False"
                End If
            Case IOTypes.TInteger
                txtDefault.Text = Regex.Replace(txtDefault.Text, "\D", String.Empty)
            Case IOTypes.TFloat
                txtDefault.Text = Regex.Replace(txtDefault.Text, "\s*[^0-9\.]|\.(?=(.*\.))", String.Empty)
            Case IOTypes.TChar
                txtDefault.Text = Regex.Replace(txtDefault.Text, "\s", String.Empty)
                txtDefault.Text = Regex.Match(txtDefault.Text, "^.").ToString()
            Case IOTypes.TString
                Exit Select
        End Select
    End Sub

    'Only allow valid caracters to be typed
    Private Sub txtVarName_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtVarName.TextChanged
        Dim CursorPos As Integer = txtVarName.CaretIndex
        If (Not txtVarName.Text = Regex.Replace(txtVarName.Text, "^\s*\d+|\W", String.Empty)) Then
            txtVarName.Text = Regex.Replace(txtVarName.Text, "^\s*\d+|\W", String.Empty)
            txtVarName.CaretIndex = CursorPos - 1
        End If
    End Sub

    Private Sub cbType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbType.SelectionChanged
        If Me.IsLoaded Then
            Select Case (cbType.SelectedIndex)
                Case 0
                    Type = IOTypes.TBoolean
                    txtDefault.ToolTip = "Only ""True"" or ""False"" are accepted"
                    txtDefault.Text = "False"
                Case 1
                    Type = IOTypes.TInteger
                    txtDefault.ToolTip = "Any integer numbers are accepted"
                    txtDefault.Text = "0"
                Case 2
                    Type = IOTypes.TFloat
                    txtDefault.ToolTip = "Any real numbers are accepted"
                    txtDefault.Text = "0.0"
                Case 3
                    Type = IOTypes.TChar
                    txtDefault.ToolTip = "Only a single character is accepted"
                    txtDefault.Text = ""
                Case 4
                    Type = IOTypes.TString
                    txtDefault.ToolTip = "Any text is accepted"
                    txtDefault.Text = ""
            End Select
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As RoutedEventArgs) Handles btnOK.Click
        If Not txtVarName.Text = "" Then
            Dim VarExists As Boolean
            If Not FromExisting Then
                VarExists = CanvasControl.CheckIfVarExists(txtVarName.Text)
            End If
            If Not VarExists Then
                VarStruct = New VarsStruct
                VarStruct.ReadableName = txtVarName.Text
                VarStruct.Type = Type
                VarStruct.DefaultValue = txtDefault.Text
                VarStruct.IsArray = checkIsArray.IsChecked
                If Not FromExisting Then
                    CanvasControl.ConstructNewVar(VarStruct)
                    Me.Close()
                Else
                    CanvasControl.UpdateVarStruct(VarStruct)
                    Me.Close()
                End If

            Else
                MsgBox("The variable name """ & txtVarName.Text & """ is already in use.")
                txtVarName.Focus()
            End If
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub CreateNewVar_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        If FromExisting Then
            txtVarName.Text = VarStruct.ReadableName
            Select Case (VarStruct.Type)
                Case IOTypes.TBoolean
                    cbType.SelectedIndex = 0
                Case IOTypes.TInteger
                    cbType.SelectedIndex = 1
                Case IOTypes.TFloat
                    cbType.SelectedIndex = 2
                Case IOTypes.TChar
                    cbType.SelectedIndex = 3
                Case IOTypes.TString
                    cbType.SelectedIndex = 4
            End Select
            Me.CanvasControl = CanvasControl
            txtDefault.Text = VarStruct.DefaultValue
            checkIsArray.IsChecked = VarStruct.IsArray
        End If

        txtVarName.Focus()
    End Sub
End Class
