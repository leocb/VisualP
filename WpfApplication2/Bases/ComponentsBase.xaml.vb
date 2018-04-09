 Public Class ComponentsBase
    Dim Inputs As New List(Of BaseIn)
    Dim Outputs As New List(Of BaseOut)
    Dim CountAddIn As Integer
    Dim CountAddOut As Integer
    Dim TitlebarVisibility As Visibility
    Dim EnableAddInput As Boolean
    Dim EnableAddOutput As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal Name As String, ByVal Description As String, ByVal BlockType As BlockTypes, ByVal MaxNumValues As Integer, ByRef Inputs As List(Of BaseIn), ByRef Outputs As List(Of BaseOut))
        InitializeComponent()
        CompleteCreating(Name, Description, BlockType, MaxNumValues, Inputs, Outputs, False)

    End Sub
    Public Sub New(ByVal Name As String, ByVal Description As String, ByVal BlockType As BlockTypes, ByVal MaxNumValues As Integer, ByRef Inputs As List(Of BaseIn), ByRef Outputs As List(Of BaseOut), ByVal TitlebarVisibility As Visibility, ByVal EnableAddInput As Boolean, ByVal EnableAddOutput As Boolean)
        InitializeComponent()

        '"Uncommon features
        Me.TitlebarVisibility = TitlebarVisibility
        'Me.EnableAddInput = EnableAddInput
        'Me.EnableAddOutput = EnableAddOutput

        If TitlebarVisibility = Visibility.Hidden Then
            ComponentName.Visibility = Visibility.Hidden
            'ComponentDescription.Visibility = Visibility.Hidden
            BigLabel.Visibility = Visibility.Visible
            BigLabel.Content = Name.ToUpper
            MaxNumValues -= 1
        End If

        'If EnableAddInput Then
        '    AddInputBtn.Visibility = Visibility.Visible
        'End If

        'If EnableAddOutput Then
        '    AddOutputBtn.Visibility = Visibility.Visible
        'End If

        'If EnableAddInput Or EnableAddOutput Then MaxNumValues += 1

        CompleteCreating(Name, Description, BlockType, MaxNumValues, Inputs, Outputs, True)

    End Sub

    Private Sub ComponentsBase_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        RecalculateWidth()
    End Sub

    Private Sub CompleteCreating(ByVal Name As String, ByVal Description As String, ByVal BlockType As BlockTypes, ByVal MaxNumValues As Integer, ByRef Inputs As List(Of BaseIn), ByRef Outputs As List(Of BaseOut), ByVal Extra As Boolean)

        'Nome e Descrição
        ComponentName.Content = Name
        'ComponentDescription.Content = ""


        'Entradas e Saidas
        Me.Inputs = Inputs
        Me.Outputs = Outputs

        'Tamanho
        Me.Height = MaxNumValues * 32 + 33

#Region "Style"
        Select Case BlockType
            Case BlockTypes.BTFlow 'Blue
                ComponentName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(204, 61, 95, 184))
                canvas.Background = New SolidColorBrush(Color.FromArgb(204, 40, 40, 50))
            Case BlockTypes.BTInOut 'purple
                ComponentName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(204, 172, 56, 167))
                canvas.Background = New SolidColorBrush(Color.FromArgb(204, 50, 40, 50))
            Case BlockTypes.BTLogic 'Red
                ComponentName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(204, 184, 61, 61))
                canvas.Background = New SolidColorBrush(Color.FromArgb(204, 50, 40, 40))
            Case BlockTypes.BTOperator 'green
                ComponentName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(204, 61, 184, 72))
                canvas.Background = New SolidColorBrush(Color.FromArgb(204, 40, 50, 40))
            Case BlockTypes.BTConverter 'gray
                ComponentName.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(204, 110, 110, 110))
                canvas.Background = New SolidColorBrush(Color.FromArgb(204, 40, 40, 40))
            Case BlockTypes.BTNone 'Sem Fundo
                ComponentName.Background = Nothing
                canvas.Background = Nothing
        End Select
#End Region

        'Nomes dos IOs e Adiciona nos canvas
        Dim i As Integer

        'INPUTS
        If Extra And TitlebarVisibility = Visibility.Hidden Then i = -1 Else i = 0
        For Each BaseIO As BaseIn In Inputs
            i += 1
            BaseIO.Name = "In" & i
            BaseIO.SetValue(Canvas.TopProperty, CDbl(i * GridSize))
            Me.canvas.Children.Add(BaseIO)
        Next BaseIO
        If Extra And AddInputBtn.Visibility = Visibility.Visible Then AddInputBtn.SetValue(Canvas.TopProperty, CDbl((i + 1) * GridSize))

        'OUTPUTS
        If Extra And TitlebarVisibility = Visibility.Hidden Then i = -1 Else i = 0
        For Each BaseIO As BaseOut In Outputs
            i += 1
            BaseIO.Name = "out" & i
            BaseIO.SetValue(Canvas.TopProperty, CDbl(i * GridSize))
            Me.canvas.Children.Add(BaseIO)
        Next BaseIO
        If Extra And AddOutputBtn.Visibility = Visibility.Visible Then AddOutputBtn.SetValue(Canvas.TopProperty, CDbl((i + 1) * GridSize))

    End Sub
    Public Sub Marker(ByVal Activate As Boolean)
        If Activate Then
            Border.StrokeThickness = 1
            Border.Stroke.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(255, 255, 255, 255))
        Else
            Border.StrokeThickness = 1
            Border.Stroke.SetValue(SolidColorBrush.ColorProperty, Color.FromArgb(178, 15, 15, 15))
        End If
    End Sub
    Public Sub RemoveAllConnections()
        For Each BaseIO As BaseIn In Inputs
            If BaseIO.GetLinhasList.Count > 0 Then
                For Each linha As Linha In BaseIO.GetLinhasList
                    BaseIO.CanvasRef.Children.Remove(linha.LinhaPath)
                    If linha.GetCompIni() IsNot Nothing Then linha.GetCompIni().RemoveLinhaByItem(linha)
                Next
                BaseIO.RemoveAllLinhas()
            End If
        Next
        For Each BaseIO As BaseOut In Outputs
            If BaseIO.GetLinhasList.Count > 0 Then
                For Each linha As Linha In BaseIO.GetLinhasList
                    BaseIO.CanvasRef.Children.Remove(linha.LinhaPath)
                    If linha.GetCompFin() IsNot Nothing Then linha.GetCompFin().RemoveLinhaByItem(linha)
                Next
                BaseIO.RemoveAllLinhas()
            End If
        Next
    End Sub

    'Private Sub AddInputBtn_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles AddInputBtn.MouseDown

    '    If e.LeftButton = MouseButtonState.Pressed Then
    '        CountAddIn += 1
    '        If CountAddIn > 0 Then
    '            Inputs.Add(New BaseIn(Inputs(0).GetTipo, "", Inputs(0).CanvasRef))
    '            Me.canvas.Children.Add(Inputs.Last)
    '            Inputs.Last.SetValue(Canvas.TopProperty, CDbl((Inputs(Inputs.Count - 2)).GetValue(Canvas.TopProperty) + GridSize))
    '        End If
    '    End If
    '    If e.RightButton = MouseButtonState.Pressed Then
    '        If CountAddIn > 0 Then
    '            CountAddIn -= 1
    '            Me.canvas.Children.Remove(Inputs.Last)
    '            RemoveLineFromSingleIO(Inputs.Last)
    '            Inputs.Remove(Inputs.Last)
    '        End If
    '    End If
    '    AddInputBtn.SetValue(Canvas.TopProperty, CDbl(Inputs.Last.GetValue(Canvas.TopProperty) + GridSize))
    '    RecalculateHeight()
    '    e.Handled = True
    'End Sub
    'Private Sub AddOutputBtn_Click(sender As Object, e As MouseButtonEventArgs) Handles AddOutputBtn.MouseDown

    '    If e.LeftButton = MouseButtonState.Pressed Then
    '        CountAddOut += 1
    '        If CountAddOut > 0 Then
    '            Outputs.Add(New BaseOut(Outputs(0).GetTipo, "", Outputs(0).CanvasRef))
    '            Me.canvas.Children.Add(Outputs.Last)
    '            Outputs.Last.SetValue(Canvas.TopProperty, CDbl((Outputs(Outputs.Count - 2)).GetValue(Canvas.TopProperty) + GridSize))
    '        End If
    '    End If
    '    If e.RightButton = MouseButtonState.Pressed Then
    '        If CountAddOut > 0 Then
    '            CountAddOut -= 1
    '            Me.canvas.Children.Remove(Outputs.Last)
    '            RemoveLineFromSingleIO(Outputs.Last)
    '            Outputs.Remove(Outputs.Last)
    '        End If
    '    End If
    '    AddOutputBtn.SetValue(Canvas.TopProperty, CDbl(Outputs.Last.GetValue(Canvas.TopProperty) + GridSize))
    '    RecalculateHeight()
    '    e.Handled = True
    'End Sub
    'Public Sub RemoveLineFromSingleIO(IO As Object)

    '    Dim Entrada = TryCast(IO, BaseIn)
    '    Dim Saida = TryCast(IO, BaseOut)
    '    Dim IsEntrada As Boolean

    '    If Entrada IsNot Nothing Then 'Valida o tipo certo
    '        IsEntrada = True
    '    ElseIf Saida IsNot Nothing Then
    '        IsEntrada = False
    '    End If

    '    If IsEntrada Then
    '        If Inputs(Inputs.IndexOf(IO)).GetLinhasList.Count > 0 Then
    '            For Each linha As Linha In Inputs(Inputs.IndexOf(IO)).GetLinhasList
    '                linha.GetCompIni.CanvasRef.Children.Remove(linha.LinhaPath)
    '                If linha.GetCompIni() IsNot Nothing Then linha.GetCompIni.RemoveLinhaByItem(linha)
    '            Next



    '            If Inputs(Inputs.IndexOf(IO)).Linha.GetCompIni() IsNot Nothing Then Inputs(Inputs.IndexOf(IO)).Linha.GetCompIni().SetLinha(Nothing)
    '            RemoveLine(Inputs(Inputs.IndexOf(IO)))
    '        End If
    '    Else
    '        If Outputs(Outputs.IndexOf(IO)).Linha IsNot Nothing Then
    '            If Outputs(Outputs.IndexOf(IO)).Linha.GetCompFin() IsNot Nothing Then Outputs(Outputs.IndexOf(IO)).Linha.GetCompFin().SetLinha(Nothing)
    '            RemoveLine(Outputs(Outputs.IndexOf(IO)))
    '        End If

    '    End If
    'End Sub
    Private Sub RecalculateHeight()
        Dim Plusses As Integer
        If TitlebarVisibility = Visibility.Visible Then Plusses += 32
        ' If EnableAddInput Or EnableAddOutput Then Plusses += 32
        If Outputs.Count > Inputs.Count Then
            Me.Height = Outputs.Count * GridSize + Plusses + 1
        Else
            Me.Height = Inputs.Count * GridSize + Plusses + 1
        End If

        Me.UpdateLayout()
    End Sub
    Private Sub RecalculateWidth()

        If BigLabel.Visibility = Visibility.Visible Then
            BigLabel.Width = Double.NaN
            Dim plusses As Integer
            If Inputs.Count Then plusses += GridSize
            If Outputs.Count Then plusses += GridSize

            Me.UpdateLayout()

            Dim MinimumWidth As Double = GridSize * 4 + 1
            Dim CalculatedWidth As Double = Math.Ceiling((BigLabel.ActualWidth + plusses) / GridSize) * GridSize + 1
            If CalculatedWidth < MinimumWidth Then Me.Width = MinimumWidth Else Me.Width = CalculatedWidth
            BigLabel.Width = Me.Width

            For Each o As BaseOut In Outputs
                o.SetValue(Canvas.LeftProperty, CDbl(Me.Width - o.ActualWidth))
            Next
        End If
    End Sub
    Public Sub UpdateName(ByVal Name As String)
        ComponentName.Content = Name
        BigLabel.Content = Name
        RecalculateWidth()
    End Sub
    Public Sub UpdateInputOutputTypes(ByVal NewInputs As List(Of BaseIn), ByVal NewOutputs As List(Of BaseOut))
        For Each o As BaseIn In NewInputs
            Inputs.Item(NewInputs.IndexOf(o)).UpdateTipo(o.GetTipo, o.GetIsArray)
        Next
        For Each o As BaseOut In NewOutputs
            Outputs.Item(NewOutputs.IndexOf(o)).UpdateTipo(o.GetTipo, o.GetIsArray)
        Next
    End Sub

End Class
