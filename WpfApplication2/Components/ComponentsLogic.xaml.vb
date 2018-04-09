Public Class ComponentsLogic
    Dim CanvasRef As Canvas
    Public Base As ComponentsBase
    Public Inputs As New List(Of BaseIn)
    Public Outputs As New List(Of BaseOut)
    Dim ComponentType As ComponentType

    Public Sub New(ByRef CanvasRef As Canvas, ByVal ComponentType As ComponentType)
        InitializeComponent()
        Me.ComponentType = ComponentType
        Me.CanvasRef = CanvasRef

        'Construct the Component Interface:
        Select Case (ComponentType)
#Region "------------- Exec Logic"
            Case ComponentType.EBegin

                Outputs.Add(New BaseOut(IOTypes.TExecution, "Begin", CanvasRef))

                Base = New ComponentsBase("Function Start", "", BlockTypes.BTFlow, 1, Inputs, Outputs)

            Case ComponentType.EEnd

                Inputs.Add(New BaseIn(IOTypes.TExecution, "End", CanvasRef))

                Base = New ComponentsBase("Function End", "", BlockTypes.BTFlow, 1, Inputs, Outputs)

            Case ComponentType.EIf
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "True", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TExecution, "False", CanvasRef))

                Base = New ComponentsBase("If", "Logic condition test", BlockTypes.BTFlow, 2, Inputs, Outputs)


            Case ComponentType.EFor
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TInteger, "From", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TInteger, "To", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "Loop", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TInteger, "Index", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TExecution, "Completed", CanvasRef))

                Base = New ComponentsBase("For", "", BlockTypes.BTFlow, 3, Inputs, Outputs)


            Case ComponentType.EWhile
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "Loop", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TExecution, "Completed", CanvasRef))

                Base = New ComponentsBase("While", "", BlockTypes.BTFlow, 2, Inputs, Outputs)


            Case ComponentType.ESwitch
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TAny, "Any", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "Case [0]", CanvasRef))

                Base = New ComponentsBase("Switch", "", BlockTypes.BTFlow, 1, Inputs, Outputs, Visibility.Visible, False, True)


            Case ComponentType.ECompareInt
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Value", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "CompareTo", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "Smaller (<)", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TExecution, "Equal (==)", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TExecution, "Greater (>)", CanvasRef))

                Base = New ComponentsBase("Compare Integer/Float", "", BlockTypes.BTFlow, 3, Inputs, Outputs, Visibility.Visible, False, False)
#End Region
#Region "------------- Input/Output"
            Case ComponentType.IOReceiveInput
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "", CanvasRef))
                Outputs.Add(New BaseOut(IOTypes.TString, "", CanvasRef))

                Base = New ComponentsBase("Receive Input", "", BlockTypes.BTInOut, 2, Inputs, Outputs)


            Case ComponentType.IOPrintText
                Inputs.Add(New BaseIn(IOTypes.TExecution, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TExecution, "", CanvasRef))

                Base = New ComponentsBase("Print Text", "", BlockTypes.BTInOut, 2, Inputs, Outputs)

#End Region
#Region "------------- Math Logic"
            Case ComponentType.MAdd
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Add", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.MSubtract
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Subtract", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.MDivide
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Divide", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MMultiply
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Multiply", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.MMod
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Mod", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MAbsolute
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Absolute", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MRound
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Round", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MCeil
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Ceil", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MFloor
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Floor", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MIncrement
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Increment", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MDecrement
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Decrement", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MBitShiftRight
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Value", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TInteger, "Amount", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase(">>", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MBitShiftLeft
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Value", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TInteger, "Amount", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("<<", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MInverse
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Inverse", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MPower
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Power", "", BlockTypes.BTOperator, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MSquareRoot
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Square Root", "", BlockTypes.BTOperator, 1, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.MClamp
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Value", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Min", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "Max", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloatOrInt, "", CanvasRef))

                Base = New ComponentsBase("Clamp", "", BlockTypes.BTOperator, 3, Inputs, Outputs, Visibility.Hidden, False, False)

#End Region
#Region "------------- Converters"
            Case ComponentType.CStringFloat
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloat, "", CanvasRef))

                Base = New ComponentsBase("From String to Float", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CStringInteger
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TInteger, "", CanvasRef))

                Base = New ComponentsBase("From String to Integer", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CStringChar
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TChar, "", CanvasRef))

                Base = New ComponentsBase("From String to Char", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CCharString
                Inputs.Add(New BaseIn(IOTypes.TChar, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TString, "", CanvasRef))

                Base = New ComponentsBase("From Char to String", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CCharFloat
                Inputs.Add(New BaseIn(IOTypes.TChar, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloat, "", CanvasRef))

                Base = New ComponentsBase("From Char to Float", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CCharInteger
                Inputs.Add(New BaseIn(IOTypes.TChar, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TInteger, "", CanvasRef))

                Base = New ComponentsBase("From Char to Integer", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CFloatString
                Inputs.Add(New BaseIn(IOTypes.TFloat, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TString, "", CanvasRef))

                Base = New ComponentsBase("From Float to String", "", BlockTypes.BTConverter, 1, Inputs, Outputs)

            Case ComponentType.CFloatInteger
                Inputs.Add(New BaseIn(IOTypes.TFloat, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TInteger, "", CanvasRef))

                Base = New ComponentsBase("From Float to Integer", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CFloatChar
                Inputs.Add(New BaseIn(IOTypes.TFloat, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TChar, "", CanvasRef))

                Base = New ComponentsBase("From Float to Char", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CIntegerString
                Inputs.Add(New BaseIn(IOTypes.TInteger, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TString, "", CanvasRef))

                Base = New ComponentsBase("From Integer to String", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CIntegerFloat
                Inputs.Add(New BaseIn(IOTypes.TInteger, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloat, "", CanvasRef))

                Base = New ComponentsBase("From Integer to Float", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CIntegerChar
                Inputs.Add(New BaseIn(IOTypes.TInteger, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TChar, "", CanvasRef))

                Base = New ComponentsBase("From Integer to Char", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CBoolString
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TString, "", CanvasRef))

                Base = New ComponentsBase("From Boolean to String", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CBoolFloat
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TFloat, "", CanvasRef))

                Base = New ComponentsBase("From Boolean to Float", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CBoolChar
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TChar, "", CanvasRef))

                Base = New ComponentsBase("From Boolean to Char", "", BlockTypes.BTConverter, 1, Inputs, Outputs)


            Case ComponentType.CBoolInteger
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TInteger, "", CanvasRef))

                Base = New ComponentsBase("From Boolean to Integer", "", BlockTypes.BTConverter, 1, Inputs, Outputs)
#End Region
#Region "------------- Boolean"
            Case ComponentType.LFIGreater
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase(">", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LSLonger
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase(">", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LFIGreaterEqual
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase(">=", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LSLongerEqual
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase(">=", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LANYEqual
                Inputs.Add(New BaseIn(IOTypes.TAny, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TAny, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("==", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LFILessEqual
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("<=", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LSShorterEqual
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("<=", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LFILess
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TFloatOrInt, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("<", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LSShorter
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TString, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("<", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LANYNotEqual
                Inputs.Add(New BaseIn(IOTypes.TAny, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TAny, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("!=", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, False, False)


            Case ComponentType.LBAnd
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("And", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.LBOr
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("Or", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.LBXor
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("Xor", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.LBNor
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("Nor", "", BlockTypes.BTLogic, 2, Inputs, Outputs, Visibility.Hidden, True, False)


            Case ComponentType.LBNegate
                Inputs.Add(New BaseIn(IOTypes.TBoolean, "", CanvasRef))

                Outputs.Add(New BaseOut(IOTypes.TBoolean, "", CanvasRef))

                Base = New ComponentsBase("Negate", "", BlockTypes.BTLogic, 1, Inputs, Outputs, Visibility.Hidden, False, False)

#End Region
        End Select

        Me.Width = Base.Width
        Me.Height = Base.Height
        ThisCanvas.Children.Add(Base)

    End Sub

    Public Function GetLogic() As String
        Dim t As String
        Const nl As String = vbNewLine

        t = "N.I."

        ''For i = 0 To CanvasRef.

        Select Case (ComponentType)
            Case ComponentType.EBegin
                t = "Main()" & nl
                t += "{" & nl
                t += GetOutputLogic(0)
                t += "}"

            Case ComponentType.EEnd
                t = ""

            Case ComponentType.EIf
                t = "if(" & GetInputLogic(1) & ")" & nl
                t += "{" & nl
                t += GetOutputLogic(0)
                t += "}" & nl
                t += "else" & nl
                t += "{" & nl
                t += GetOutputLogic(1)
                t += "}" & nl

        End Select
        Return t
    End Function

    Private Function GetOutputLogic(i As Integer) As String
        Dim t As String
        Dim o As Object
        Dim l As Linha

        l = Outputs(i).GetLinhaByIndex()
        If l IsNot Nothing Then o = l.GetParentCompFin Else t += ""
        If o IsNot Nothing Then t += o.GetLogic() Else t += ""

        Return t

    End Function
    Private Function GetInputLogic(i As Integer) As String
        Dim t As String
        Dim o As Object
        Dim l As Linha

        l = Inputs(i).GetLinhaByIndex()
        If l IsNot Nothing Then o = l.GetParentCompFin Else t += ""
        If o IsNot Nothing Then t += o.GetLogic() Else t += ""

        Return t

    End Function

    Public Sub SetMarker(b As Boolean)
        Base.Marker(b)
    End Sub
    Public Function GetBase() As Object
        Return Base
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
    Public Function GetComponentType() As ComponentType
        Return ComponentType
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
