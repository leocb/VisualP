Imports System.ComponentModel

 Public Class BaseOut
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Dim MyLineAnchor As Point
    Dim Tipo As IOTypes
    Public CanvasRef As Canvas
    Public Linhas As New List(Of Linha)
    Dim isArray As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(ByVal Tipo As IOTypes, ByVal Descricao As String, ByRef CanvasRef As Canvas, ByVal isArray As Boolean)

        InitializeComponent()

        OutExec.StrokeThickness = 1
        OutValue.StrokeThickness = 1

        OutDescricao.Content = Descricao
        Me.Tipo = Tipo
        Me.CanvasRef = CanvasRef
        Me.isArray = isArray

        UpdateConnectionStyle()
    End Sub
    Public Sub New(ByVal Tipo As IOTypes, ByVal Descricao As String, ByRef CanvasRef As Canvas)

        InitializeComponent()

        OutExec.StrokeThickness = 1
        OutValue.StrokeThickness = 1

        OutDescricao.Content = Descricao
        Me.Tipo = Tipo
        Me.CanvasRef = CanvasRef
        Me.isArray = False

        UpdateConnectionStyle()
    End Sub

#Region "Dependency Properties"
    Public Shared AnchorPropertyX As DependencyProperty =
         DependencyProperty.Register("AnchorX", GetType(Double), GetType(BaseOut), New FrameworkPropertyMetadata(CDbl(0), FrameworkPropertyMetadataOptions.None))
    Public Shared AnchorPropertyY As DependencyProperty =
         DependencyProperty.Register("AnchorY", GetType(Double), GetType(BaseOut), New FrameworkPropertyMetadata(CDbl(0), FrameworkPropertyMetadataOptions.None))

    Public Property AnchorX() As Double
        Get
            Return GetValue(AnchorPropertyX)
        End Get
        Set(ByVal value As Double)
            SetValue(AnchorPropertyX, value)
        End Set
    End Property
    Public Property AnchorY() As Double
        Get
            Return GetValue(AnchorPropertyY)
        End Get
        Set(ByVal value As Double)
            SetValue(AnchorPropertyY, value)
        End Set
    End Property
#End Region

#Region "Routed Event for click management"

    ' Create a custom routed event by first registering a RoutedEventID
    ' This event uses the bubbling routing strategy
    Public Shared ReadOnly LineGrabEvent As RoutedEvent = EventManager.RegisterRoutedEvent("LineGrab", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(BaseOut))

    ' Provide CLR accessors for the event
    Public Custom Event LineGrab As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(LineGrabEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(LineGrabEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    ' This method raises the Tap event
    Private Sub RaiseLineGrab(sender As Object)
        Dim newEventArgs As New RoutedEventArgs(BaseOut.LineGrabEvent, sender)
        Me.RaiseEvent(newEventArgs)
    End Sub

    ' raise the event
    Private Shadows Sub OutValue_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles OutValue.MouseDown
        If e.ChangedButton = MouseButton.Left Then
            Me.RaiseLineGrab(Me)
            e.Handled = True
        End If
        If e.ChangedButton = MouseButton.Right Then
            e.Handled = True
        End If
    End Sub
    Private Shadows Sub OutExec_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles OutExec.MouseDown
        If e.ChangedButton = MouseButton.Left Then
            Me.RaiseLineGrab(Me)
            e.Handled = True
        End If
        If e.ChangedButton = MouseButton.Right Then
            e.Handled = True
        End If
    End Sub
    Private Shadows Sub OutValue_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles OutValue.MouseUp
        If e.ChangedButton = MouseButton.Left Then
            Me.RaiseLineGrab(Me)
            e.Handled = True
        End If
        If e.ChangedButton = MouseButton.Right Then
            Me.RemoveAllLinhas()
            e.Handled = True
        End If
    End Sub
    Private Shadows Sub OutExec_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles OutExec.MouseUp
        If e.ChangedButton = MouseButton.Left Then
            Me.RaiseLineGrab(Me)
            e.Handled = True
        End If
        If e.ChangedButton = MouseButton.Right Then
            Me.RemoveAllLinhas()
            e.Handled = True
        End If
    End Sub
#End Region

    Public Sub LayoutUpdatedHandler(sender As Object, e As EventArgs) Handles Me.LayoutUpdated 'Quanto o layout é atualizado, recalculamos a posição
        Dim prevAx As Double = AnchorX
        Dim prevAy As Double = AnchorY
        Dim center As Point
        Dim centerRelativeToAncestor As Point

        If Me.Tipo = IOTypes.TExecution Then
            center = New Point(OutExec.ActualWidth / 2 + OutExec.GetValue(Canvas.LeftProperty), OutExec.ActualHeight / 2 + OutExec.GetValue(Canvas.TopProperty))
        Else
            center = New Point(OutValue.ActualWidth / 2 + OutValue.GetValue(Canvas.LeftProperty), OutValue.ActualHeight / 2 + OutValue.GetValue(Canvas.TopProperty))
        End If

        If Me.IsLoaded And Me.IsEnabled And Me.IsVisible Then
            centerRelativeToAncestor = Me.TransformToAncestor(Me.CanvasRef).Transform(center)

            If Not centerRelativeToAncestor.X = prevAx Or Not centerRelativeToAncestor.Y = prevAy Then 'Só Recalculamos a linha se a posição for diferente
                AnchorX = centerRelativeToAncestor.X
                AnchorY = centerRelativeToAncestor.Y
                NotifyPropertyChanged("AnchorX")
                NotifyPropertyChanged("AnchorY")
            End If
            'If Not IsNothing(CanvasRef) Then Debug.Print("update " & AnchorX & " | " & AnchorY & " | " & Me.TransformToAncestor(Me.CanvasRef).Transform(center).ToString)
        End If
    End Sub


#Region "Connection Manager"
    Public Sub AddLinha(ByRef Linha As Linha)
        If Me.Tipo = IOTypes.TExecution Then RemoveAllLinhas() 'if execution, remove all line before add the new one
        Linha.SetCompFinIndex(Linhas.Count)
        Me.Linhas.Add(Linha)
        UpdateConnectionStyle()
    End Sub
    Public Function GetLinhaByIndex(Optional index As Integer = 0) As Linha
        If Me.Tipo = IOTypes.TExecution Then index = 0 'If execution, always return the first (and only) line
        Return Me.Linhas.Item(index)
    End Function
    Public Function GetLinhasList() As List(Of Linha)
        Return Me.Linhas
    End Function
    Public Function GetTipo() As IOTypes
        Return Me.Tipo
    End Function
    Public Function GetIsArray() As Boolean
        Return Me.isArray
    End Function
    Public Sub RemoveLinhaByItem(ByRef Linha As Linha)
        Linhas.Remove(Linha)
        UpdateConnectionStyle()
    End Sub
    Public Sub RemoveAllLinhas()
        For Each l As Linha In Linhas
            If CanvasRef.Children.Contains(l.LinhaPath) Then
                Debug.Print("HAS")
                CanvasRef.Children.Remove(l.LinhaPath)
                l.GetCompFin.RemoveLinhaByItem(l)
                l = Nothing
            End If
        Next
        Linhas.Clear()
        UpdateConnectionStyle()
        'CanvasRef.CanvasCleanup()
    End Sub
#End Region
    Private Sub UpdateConnectionStyle()
#Region "Connection Style -> -NOT- line style"
        If Linhas.Count > 0 Then
            Select Case Tipo
                Case IOTypes.TExecution
                    OutExec.Fill = New SolidColorBrush(Color.FromArgb(255, 220, 220, 220))
                    OutExec.Stroke = New SolidColorBrush(Color.FromArgb(255, 180, 180, 180))
                    OutExec.Visibility = Visibility.Visible
                    OutValue.Visibility = vbHidden

                Case IOTypes.TBoolean   'VERMELHO
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 255, 32, 32))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 32, 32))

                Case IOTypes.TChar      'ROSA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 255, 32, 255))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 32, 128))

                Case IOTypes.TString    'AMARELO
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 255, 255, 32))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 128, 32))

                Case IOTypes.TFloat     'VERDE
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 32, 200, 32))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 128, 32))

                Case IOTypes.TInteger   'AZUL
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 32, 139, 255))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 64, 128))

                Case IOTypes.TAny   'CINZA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 128, 128, 128))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 64, 64, 64))

                Case IOTypes.TFloatOrInt   'TEAL
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 32, 255, 164))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 128, 80))

                Case IOTypes.TCharOrString   'LARANJA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(255, 255, 128, 32))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 64, 32))
            End Select
        Else
            Select Case Tipo
                Case IOTypes.TExecution
                    OutExec.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutExec.Stroke = New SolidColorBrush(Color.FromArgb(255, 220, 220, 220))
                    OutExec.Visibility = Visibility.Visible
                    OutValue.Visibility = vbHidden

                Case IOTypes.TBoolean   'VERMELHO
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 32))

                Case IOTypes.TChar      'ROSA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 32, 255))

                Case IOTypes.TString    'AMARELO
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 255, 32))

                Case IOTypes.TFloat     'VERDE
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 200, 32))

                Case IOTypes.TInteger   'AZUL
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 139, 255))

                Case IOTypes.TAny   'CINZA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 128, 128, 128))

                Case IOTypes.TFloatOrInt   'TEAL
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 32, 255, 164))

                Case IOTypes.TCharOrString   'LARANJA
                    OutValue.Fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    OutValue.Stroke = New SolidColorBrush(Color.FromArgb(255, 255, 128, 32))
            End Select
        End If
#End Region
    End Sub

    Public Function ReturnGetLogicParent() As Object
        Return Me.Parent
    End Function

    Public Sub UpdateTipo(newType As IOTypes, isArray As Boolean)
        Tipo = newType
        Me.isArray = isArray
        RemoveAllLinhas()
    End Sub
End Class
