Imports System.Text.RegularExpressions
 Public Class VariablesLogic
    Dim CanvasRef As Canvas
    Public Base As ComponentsBase
    Public Inputs As New List(Of BaseIn)
    Public Outputs As New List(Of BaseOut)
    Public VarStruct As VarsStruct
    Dim isSet As Boolean


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(ByRef CanvasRef As Canvas, ByVal VarStruct As VarsStruct, ByVal isSet As Boolean)
        InitializeComponent()
        Me.CanvasRef = CanvasRef
        Me.VarStruct = VarStruct
        Me.isSet = isSet


        'Construct the Component Interface:

        Dim TitleText As String = VarStruct.ReadableName
        TitleText = Regex.Replace(TitleText, "([a-z])([A-Z])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([a-z])([0-9])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([A-Z])([0-9])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([0-9])(\D)", "$1 $2")

        If isSet Then
            Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
            Inputs.Add(New BaseIn(VarStruct.Type, "Value", CanvasRef))

            Outputs.Add(New BaseOut(IOTypes.TExecution, "", CanvasRef))
            Outputs.Add(New BaseOut(VarStruct.Type, "", CanvasRef))

            Base = New ComponentsBase("Set " & TitleText, "", BlockTypes.BTConverter, 2, Inputs, Outputs)

        Else
            Outputs.Add(New BaseOut(VarStruct.Type, "", CanvasRef))

            Base = New ComponentsBase(TitleText.ToUpper, "", BlockTypes.BTConverter, 1, Inputs, Outputs, Visibility.Hidden, False, False)
        End If

        CompleteCreating()

    End Sub
    Private Sub CompleteCreating()
        Me.Width = Base.Width
        Me.Height = Base.Height
        ThisCanvas.Children.Add(Base)
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
    Public Sub UpdateVarStruct(VarStruct As VarsStruct)
        Me.VarStruct = VarStruct
        RemoveConnections()

        Dim TitleText As String = VarStruct.ReadableName
        TitleText = Regex.Replace(TitleText, "([a-z])([A-Z])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([a-z])([0-9])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([A-Z])([0-9])", "$1 $2")
        TitleText = Regex.Replace(TitleText, "([0-9])(\D)", "$1 $2")

        If isSet Then
            Inputs = New List(Of BaseIn)
            Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
            Inputs.Add(New BaseIn(VarStruct.Type, "Value", CanvasRef, VarStruct.IsArray))

            Outputs = New List(Of BaseOut)
            Outputs.Add(New BaseOut(IOTypes.TExecution, "", CanvasRef))
            Outputs.Add(New BaseOut(VarStruct.Type, "", CanvasRef, VarStruct.IsArray))

            Base.UpdateName(TitleText)
            Base.UpdateInputOutputTypes(Inputs, Outputs)
        Else
            Inputs = New List(Of BaseIn)
            Outputs = New List(Of BaseOut)
            Outputs.Add(New BaseOut(VarStruct.Type, "", CanvasRef, VarStruct.IsArray))

            Base.UpdateName(TitleText.ToUpper)
            Base.UpdateInputOutputTypes(Inputs, Outputs)
        End If
    End Sub
    Public Sub RemoveConnections()
        Base.RemoveAllConnections()
    End Sub
    Public Function GetVarKey() As String
        Return VarStruct.ReadableName
    End Function
    Public Function GetIsSetAsString() As String
        If isSet Then Return "True" Else Return "False"
    End Function

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
