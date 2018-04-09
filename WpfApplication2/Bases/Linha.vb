Imports System.ComponentModel
Imports System.Windows.Controls

 Public Class Linha
    Inherits Control
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Variaveis"
    Public LinhaPath As New Path()
    Dim CompIni As BaseOut
    Dim CompFin As BaseIn
    Dim ParentCompIni As Object
    Dim ParentCompFin As Object
    Dim CompIniIndex As Integer
    Dim CompFinIndex As Integer
    Dim CanvasRef As Canvas
    Dim Tipo As IOTypes
    Dim MouseLocked As Boolean
    Dim PontoDesejado As PontoDaLinha
#End Region
#Region "Dependency Property/Event Definitions"

    Public Shared PosIniXP As DependencyProperty =
         DependencyProperty.Register("PosIniX", GetType(Double), GetType(Linha),
                                     New FrameworkPropertyMetadata(
                                         CDbl(0), FrameworkPropertyMetadataOptions.None, New PropertyChangedCallback(AddressOf OnCurrentReadingChanged))
                                     )
    Public Shared PosIniYP As DependencyProperty =
         DependencyProperty.Register("PosIniY", GetType(Double), GetType(Linha),
                                     New FrameworkPropertyMetadata(
                                         CDbl(0), FrameworkPropertyMetadataOptions.None, New PropertyChangedCallback(AddressOf OnCurrentReadingChanged))
                                     )
    Public Shared PosFinXP As DependencyProperty =
         DependencyProperty.Register("PosFinX", GetType(Double), GetType(Linha),
                                     New FrameworkPropertyMetadata(
                                         CDbl(0), FrameworkPropertyMetadataOptions.None, New PropertyChangedCallback(AddressOf OnCurrentReadingChanged))
                                     )
    Public Shared PosFinYP As DependencyProperty =
         DependencyProperty.Register("PosFinY", GetType(Double), GetType(Linha),
                                     New FrameworkPropertyMetadata(
                                         CDbl(0), FrameworkPropertyMetadataOptions.None, New PropertyChangedCallback(AddressOf OnCurrentReadingChanged))
                                     )

    Private Shared Sub OnCurrentReadingChanged(ByVal d As DependencyObject,
    ByVal e As DependencyPropertyChangedEventArgs)
        Dim MeLinha As Linha = DirectCast(d, Linha)
        MeLinha.AtualizarLinha()
    End Sub

    Public Property PosIniX() As Double
        Get
            Return CDbl(GetValue(PosIniXP))
        End Get
        Set(ByVal value As Double)
            SetValue(PosIniXP, value)
        End Set
    End Property
    Public Property PosIniY() As Double
        Get
            Return CDbl(GetValue(PosIniYP))
        End Get
        Set(ByVal value As Double)
            SetValue(PosIniYP, value)
        End Set
    End Property
    Public Property PosFinX() As Double
        Get
            Return CDbl(GetValue(PosFinXP))
        End Get
        Set(ByVal value As Double)
            SetValue(PosFinXP, value)
        End Set
    End Property
    Public Property PosFinY() As Double
        Get
            Return CDbl(GetValue(PosFinYP))
        End Get
        Set(ByVal value As Double)
            SetValue(PosFinYP, value)
        End Set
    End Property

#End Region
#Region "Bindings"

    Dim CIniPosXBind As Binding = New Binding("AnchorX")
    Dim CIniPosYBind As Binding = New Binding("AnchorY")
    Dim CFinPosXBind As Binding = New Binding("AnchorX")
    Dim CFinPosYBind As Binding = New Binding("AnchorY")

#End Region

    Public Sub New(ByVal Tipo As IOTypes, ByRef CanvasRef As Canvas)
        Me.Tipo = Tipo
        Me.CanvasRef = CanvasRef

#Region "Line style"
        Select Case Tipo
            Case IOTypes.TExecution 'BRANCO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 220, 220, 220))

            Case IOTypes.TBoolean   'VERMELHO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 32))

            Case IOTypes.TChar      'ROSA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 255))

            Case IOTypes.TString    'AMARELO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 255, 32))

            Case IOTypes.TFloat     'VERDE
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 200, 32))

            Case IOTypes.TInteger   'AZUL
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 139, 255))

            Case IOTypes.TAny   'CINZA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 128, 128))

            Case IOTypes.TFloatOrInt   'TEAL
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 255, 164))

            Case IOTypes.TCharOrString   'LARANJA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 128, 32))

        End Select

        LinhaPath.StrokeThickness = 4
#End Region
    End Sub

    Public Sub New()

    End Sub
    Public Sub AtualizarLinha()

        Dim PIX, PIY, PFX, PFY As Double
        PIX = PosIniX
        PIY = PosIniY
        PFX = PosFinX
        PFY = PosFinY

        If MouseLocked Then
            If PontoDesejado = PontoDaLinha.PontoInicial Then
                PIX = Mouse.GetPosition(CanvasRef).X + 1
                PIY = Mouse.GetPosition(CanvasRef).Y
            ElseIf PontoDesejado = PontoDaLinha.PontoFinal Then
                PFX = Mouse.GetPosition(CanvasRef).X - 1
                PFY = Mouse.GetPosition(CanvasRef).Y
            End If
        End If


        'Distance between ini and fin / 2
        Dim Dist As Integer = (Math.Sqrt(Math.Pow((PFX - PIX), 2) + Math.Pow(PFY - PIY, 2))) / 2
        'control point distances from their points
        Dim CP1 As Integer = PIX + Dist
        Dim CP2 As Integer = PFX - Dist

        'Set new pos data
        LinhaPath.Data = Geometry.Parse("M " & Math.Round(PIX) & "," & Math.Round(PIY) & 'Inicial
                                       " C " & Math.Round(CP1) & "," & Math.Round(PIY) & 'Control point 1
                                         " " & Math.Round(CP2) & "," & Math.Round(PFY) & 'Control point 2
                                         " " & Math.Round(PFX) & "," & Math.Round(PFY))  'Final
    End Sub

    Public Sub SetCompIni(ByRef CompIni As BaseOut)
        BindingOperations.ClearBinding(Me, PosIniXP)
        BindingOperations.ClearBinding(Me, PosIniYP)
        Me.CompIni = CompIni
        If CompIni IsNot Nothing Then
            CIniPosXBind = New Binding("AnchorX")
            CIniPosYBind = New Binding("AnchorY")
            CIniPosXBind.Source = CompIni
            CIniPosYBind.Source = CompIni
            BindingOperations.SetBinding(Me, PosIniXP, CIniPosXBind)
            BindingOperations.SetBinding(Me, PosIniYP, CIniPosYBind)
        End If
        If CompFin Is Nothing Then
            Me.MouseLocked = True
            Me.PontoDesejado = PontoDaLinha.PontoFinal
            Me.AtualizarLinha()
        End If
    End Sub
    Public Sub SetCompFin(ByRef CompFin As BaseIn)
        BindingOperations.ClearBinding(Me, PosFinXP)
        BindingOperations.ClearBinding(Me, PosFinYP)
        Me.CompFin = CompFin
        If CompFin IsNot Nothing Then
            CFinPosXBind = New Binding("AnchorX")
            CFinPosYBind = New Binding("AnchorY")
            CFinPosXBind.Source = CompFin
            CFinPosYBind.Source = CompFin
            BindingOperations.SetBinding(Me, PosFinXP, CFinPosXBind)
            BindingOperations.SetBinding(Me, PosFinYP, CFinPosYBind)
        End If
        If CompIni Is Nothing Then
            Me.MouseLocked = True
            Me.PontoDesejado = PontoDaLinha.PontoInicial
            Me.AtualizarLinha()
        End If
    End Sub

    Public Function GetTipo() As IOTypes
        Return Me.Tipo
    End Function

    '------------ GET/SET Comps
    Public Function GetCompIni() As BaseOut
        Return CompIni
    End Function
    Public Function GetCompFin() As Basein
        Return CompFin
    End Function
    Sub SetParentCompIni(ParentCompIni As Object)
        Me.ParentCompIni = ParentCompIni
    End Sub
    Sub SetParentCompFin(ParentCompFin As Object)
        Me.ParentCompFin = ParentCompFin
    End Sub

    '------------ GET Parent Comps
    Function GetParentCompIni() As Object
        Return Me.ParentCompIni
    End Function
    Function GetParentCompFin() As Object
        Return Me.ParentCompFin
    End Function

    '------------ GET/SET Linha Index (useful?)
    Public Sub SetCompIniIndex(index As Integer)
        Me.CompIniIndex = index
    End Sub
    Public Function GetCompIniIndex() As Integer
        Return Me.CompIniIndex
    End Function
    Public Sub SetCompFinIndex(index As Integer)
        Me.CompFinIndex = index
    End Sub
    Public Function GetCompFinIndex() As Integer
        Return Me.CompFinIndex
    End Function

    Public Sub SetStyle(Tipo As IOTypes)
        Select Case Tipo
            Case IOTypes.TExecution 'BRANCO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 220, 220, 220))

            Case IOTypes.TBoolean   'VERMELHO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 32))

            Case IOTypes.TChar      'ROSA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 255))

            Case IOTypes.TString    'AMARELO
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 255, 32))

            Case IOTypes.TFloat     'VERDE
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 200, 32))

            Case IOTypes.TInteger   'AZUL
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 139, 255))

            Case IOTypes.TAny   'CINZA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 128, 128))

            Case IOTypes.TFloatOrInt   'TEAL
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 255, 164))

            Case IOTypes.TCharOrString   'LARANJA
                LinhaPath.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 128, 32))

        End Select
    End Sub
#Region "Follow mouse"
    Public Sub StartFollowTheMouse(ByVal PontoDesejado As PontoDaLinha)
        Me.PontoDesejado = PontoDesejado
        MouseLocked = True
        AtualizarLinha()
    End Sub
    Public Sub StopFollowTheMouse()
        MouseLocked = False
        If CompIni IsNot Nothing And CompFin IsNot Nothing Then
            BindingOperations.SetBinding(Me, PosIniXP, CIniPosXBind)
            BindingOperations.SetBinding(Me, PosIniYP, CIniPosYBind)
            BindingOperations.SetBinding(Me, PosFinXP, CFinPosXBind)
            BindingOperations.SetBinding(Me, PosFinYP, CFinPosYBind)
        Else
            BindingOperations.ClearBinding(Me, PosIniXP)
            BindingOperations.ClearBinding(Me, PosIniYP)
            BindingOperations.ClearBinding(Me, PosFinXP)
            BindingOperations.ClearBinding(Me, PosFinYP)

        End If
    End Sub
#End Region

End Class
