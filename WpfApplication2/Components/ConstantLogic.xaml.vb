Imports System.Text.RegularExpressions
Public Class ConstantLogic
    Dim CanvasRef As Canvas
    Dim Tipo As IOTypes
    Public Base As ComponentsBase
    Public Inputs As New List(Of BaseIn)
    Public Outputs As New List(Of BaseOut)

    Public Sub New(ByVal Tipo As IOTypes, ByRef CanvasRef As Canvas)
        InitializeComponent()
        Me.CanvasRef = CanvasRef
        Me.Tipo = Tipo

        If Tipo = IOTypes.TBoolean Then
            OutText.Visibility = Visibility.Hidden
            checkBool.Visibility = Visibility.Visible
        End If
        If Tipo = IOTypes.TChar Then
            OutText.MaxLength = 1
        End If

        Outputs.Add(New BaseOut(Tipo, "", CanvasRef))

        Base = New ComponentsBase("                           ", "", BlockTypes.BTNone, 1, Inputs, Outputs, Visibility.Hidden, False, False)

        Me.Width = Base.Width
        Me.Height = Base.Height
        ThisCanvas.Children.Add(Base)
    End Sub

    Private Sub OutText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles OutText.TextChanged
        If Tipo = IOTypes.TFloat Then
            If (Not OutText.Text = Regex.Replace(OutText.Text, "\s*[^0-9\.]|\.(?=(.*\.))", String.Empty)) Then
                Dim CursorPos As Integer = OutText.CaretIndex
                OutText.Text = Regex.Replace(OutText.Text, "\s*[^0-9\.]|\.(?=(.*\.))", String.Empty)
                OutText.CaretIndex = CursorPos - 1
            End If
        ElseIf Tipo = IOTypes.TInteger Then
            If (Not OutText.Text = Regex.Replace(OutText.Text, "\D", String.Empty)) Then
                Dim CursorPos As Integer = OutText.CaretIndex
                OutText.Text = Regex.Replace(OutText.Text, "\D", String.Empty)
                OutText.CaretIndex = CursorPos - 1
            End If
        ElseIf Tipo = IOTypes.TChar Then
            If (Not OutText.Text = Regex.Replace(OutText.Text, "\s", String.Empty)) Then
                Dim CursorPos As Integer = OutText.CaretIndex
                OutText.Text = Regex.Replace(OutText.Text, "\s", String.Empty)
                OutText.CaretIndex = CursorPos - 1
            End If
        End If
    End Sub

    Private Sub checkBool_Click(sender As Object, e As RoutedEventArgs) Handles checkBool.Click
        If checkBool.IsChecked Then
            checkBool.Content = "True"
        Else
            checkBool.Content = "False"
        End If
    End Sub

    Public Function GetCurrentConstValue() As String
        If Tipo = IOTypes.TBoolean Then
            If checkBool.IsChecked Then Return "True" Else Return "False"
        Else
            Return OutText.Text
        End If
    End Function

    Public Sub SetCurrentConstValue(Value As String)
        If Tipo = IOTypes.TBoolean Then
            If Value = "True" Then
                checkBool.IsChecked = True
                checkBool.Content = "True"
            Else
                checkBool.IsChecked = False
                checkBool.Content = "False"
            End If
        Else
            OutText.Text = Value
        End If
    End Sub

    Public Function GetLogic() As String
        Return ""
    End Function
    Public Sub SetMarker(b As Boolean)
        Base.Marker(b)
    End Sub
    Public Function GetBase() As Object
        Return Base
    End Function
    Public Function GetTipo() As IOTypes
        Return Me.Tipo
    End Function
    Public Sub RemoveConnections()
        Base.RemoveAllConnections()
    End Sub
    Public Function GetInputsList() As List(Of BaseIn)
        Return Inputs
    End Function
    Public Function GetOutputsList() As List(Of BaseOut)
        Return Outputs
    End Function

    Private Sub ConstantLogic_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Select Case Tipo
            Case IOTypes.TBoolean   'VERMELHO
                ConstantName.Content = "Boolean"
                ConstantName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(30, 172, 56, 56))

            Case IOTypes.TChar      'ROSA
                ConstantName.Content = "Char"
                ConstantName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(30, 172, 56, 167))

            Case IOTypes.TString    'AMARELO
                ConstantName.Content = "String"
                ConstantName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(30, 220, 202, 19))

            Case IOTypes.TFloat     'VERDE
                ConstantName.Content = "Float"
                ConstantName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(30, 61, 184, 72))

            Case IOTypes.TInteger   'AZUL
                ConstantName.Content = "Integer"
                ConstantName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(30, 61, 95, 184))
        End Select
    End Sub

    Public Function GetInputByName(name As String) As BaseIn
        For Each i As BaseIn In Inputs
            If i.Name = name Then Return i
        Next
        Return Nothing
    End Function
    Public Function GetOutputByName(name As String) As BaseOut
        For Each o As BaseOut In Outputs
            If o.Name = name Then Return o
        Next
        Return Nothing
    End Function
End Class
