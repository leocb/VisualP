Imports System.Text.RegularExpressions
Imports System.IO
Public Class CanvasControl
#Disable Warning BC42104 ' Variable is used before it has been assigned a value

#Region "Variaveis locais"
    'Components
    Dim ComponentMoving As New List(Of Object)
    Dim ComponentMovingMouseOffset As New List(Of Point)
    Dim CreateCompPos As Point
    Dim funcBegin As ComponentsLogic
    Dim funcEnd As ComponentsLogic
    Dim CompNameCounter As ULong = 0
    Public LinhaNameCounter As ULong = 0

    'Canvas
    Dim ObjectsOnCanvas As New Dictionary(Of String, Object)
    Public LinhasOnCanvas As New List(Of Linha)
    Dim EnableSnapping As Boolean = True '<-----------------CHANGE THIS TO APP SETTINGS
    Dim ViewportClass As New CustomProp
    Dim MouseOffsetCanvas As Point
    Dim CanvasCurrLocation As Point
    Dim LinhaMoving As Linha

    'Canvas Scaling
    Dim LayoutScale As Double = 1
    Dim LayoutScaleSteps() As Double = New Double() {0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3}
    Dim LayoutScaleIndex As Short = 7

    'Components Selection
    Dim isSelecting As Boolean
    Dim Marker As New List(Of Object)
    Dim MouseOffsetSelectRect As New Point(Double.NaN, Double.NaN)

    'Vars
    Dim VarsCM As New ContextMenu
    Dim VarSelectedName As String
    Public VarsDict As New Dictionary(Of String, VarsStruct)

    'Control
    Private _textCode As TextBox
    Public Property TextCode As TextBox
        Get
            Return _textCode
        End Get
        Set(value As TextBox)
            _textCode = value
        End Set
    End Property
    Public Identacao As Integer


#End Region

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub CanvasControl_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        ViewportClass.setVPCoords(New Rect(0, 0, BigGridSize, BigGridSize))
        DataContext = ViewportClass
        LinhaMoving = Nothing

#Region "Adiciona o ExecLogicFuncBegin e End, obrigatórios"
        'BEGIN
        funcBegin = New ComponentsLogic(ComponentsCanvas, ComponentType.EBegin)
        AddToCanvas(funcBegin)
        AddHandler funcBegin.MouseDown, AddressOf MoveControleDOWN

        'END
        funcEnd = New ComponentsLogic(ComponentsCanvas, ComponentType.EEnd)
        AddToCanvas(funcEnd)
        AddHandler funcEnd.MouseDown, AddressOf MoveControleDOWN

        'Posições
        funcBegin.SetValue(Canvas.TopProperty, CDbl((BigGridSize * 1 \ GridSize) * GridSize))
        funcEnd.SetValue(Canvas.TopProperty, CDbl((BigGridSize * 1 \ GridSize) * GridSize))
        funcEnd.SetValue(Canvas.LeftProperty, CDbl((BigGridSize * 3 \ GridSize) * GridSize))


#End Region

        'Handlers para os eventos da linha
        Me.AddHandler(BaseIn.LineGrabEvent, New RoutedEventHandler(AddressOf ReceiveBaseInMouseUpDown))
        Me.AddHandler(BaseOut.LineGrabEvent, New RoutedEventHandler(AddressOf ReceiveBaseOutMouseUpDown))

        'Send the focus to the keyPresses textbox handler
        KeyboardFocuser.Focus()

        'Create the main Context Menu
#Region "CONTEXT MENU ----------------------------------------------------------------------------"
        'Create Context Menu
        Dim CanvasCM As New ContextMenu
        'Create menu items

        Dim CanvasMI As New MenuItem
        Dim CanvasSMI As New MenuItem
        Dim CanvasSSMI As New MenuItem
        Dim CanvasCMSeparator As New Separator

#Region "'------------------------------------------------- EXEC FLOW"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Execution Flow"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "If"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "For"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "While"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Switch"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Compare Integer/Float"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)

        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- BOOLEAN OPERATORS"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Boolean Operators"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Equal to (==)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Not equal (!=)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "And"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Or"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Xor"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Nor"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Negate"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)

        CanvasSMI.Header = "Boolean"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Greater than (>)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Greater or equal to (>=)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Less or equal to (<=)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Less than (<)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)

        CanvasSMI.Header = "Integer/Float"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Longer than (>)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Longer or equal to (>=)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Shorter or equal to (<=)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "Shorter than (<)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)

        CanvasSMI.Header = "String"
        CanvasMI.Items.Add(CanvasSMI)

        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- MATH OPERATORS"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Math Operators"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Add (+)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Subtract (-)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Divide (/)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Multiply (*)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Mod (%)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Absolute (Abs)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Round (Round to Closest)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Ceil (Round Up)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Floor (Round Down)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Increment (++)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Decrement (--)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Bit Shift Right (>>)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Bit Shift Left (<<)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Inverse (*-1)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Power (^)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Square root (sqrt)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Clamp (Min and Max)"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)

        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- IO OPERATORS"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Input/Output"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Receive Input"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Print Text"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)

        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- CONVERTERS"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Converters"

        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Integer (FTI)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To String (FTS)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Char (FTC)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem

        CanvasSMI.Header = "From Float"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Float (ITF)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To String (ITS)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Char (ITC)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem

        CanvasSMI.Header = "From Integer"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Integer (STI)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Float (STF)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Char (STC)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem

        CanvasSMI.Header = "From String"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Integer (CTI)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Float (CTF)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To String (CTS)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem

        CanvasSMI.Header = "From Char"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasSMI = New MenuItem
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Integer (BTI)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Float (BTF)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To String (BTS)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem
        CanvasSSMI.Header = "To Char (BTC)"
        AddHandler(CanvasSSMI.Click), AddressOf CanvasContextMenu
        CanvasSMI.Items.Add(CanvasSSMI)
        CanvasSSMI = New MenuItem

        CanvasSMI.Header = "From Boolean"
        CanvasMI.Items.Add(CanvasSMI)


        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- CONSTANTS"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Constants"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Constant Boolean"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Constant Integer"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Constant Float"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Constant String"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasSMI = New MenuItem
        CanvasSMI.Header = "Constant Char"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)

        CanvasCM.Items.Add(CanvasMI)
#End Region
#Region "'------------------------------------------------- VARIABLES"
        CanvasMI = New MenuItem
        CanvasMI.Header = "Variables"

        CanvasSMI = New MenuItem
        CanvasSMI.Header = "+New"
        AddHandler(CanvasSMI.Click), AddressOf CanvasContextMenu
        CanvasMI.Items.Add(CanvasSMI)
        CanvasMI.Items.Add(CanvasCMSeparator)

        CanvasCM.Items.Add(CanvasMI)
#End Region

        '================================================= ADD the contextmenu to the canvas
        ParentCanvas.ContextMenu = CanvasCM
#End Region

    End Sub

    'CONTEXT MENU EVENT HANDLER
    Sub CanvasContextMenu(sender As Object, ByVal e As EventArgs)
        Dim Addeer As UIElement

        Select Case (sender.header.ToString)
#Region "'------------------------------------------------- EXEC FLOW"
            Case "If"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.EIf)
            Case "For"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.EFor)
            Case "While"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.EWhile)
            Case "Switch"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.ESwitch)
            Case "Compare Integer/Float"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.ECompareInt)
#End Region
#Region "'------------------------------------------------- BOOLEAN OPERATORS"
            Case "Equal to (==)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LANYEqual)
            Case "Not equal (!=)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LANYNotEqual)
            Case "And"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LBAnd)
            Case "Or"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LBOr)
            Case "Xor"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LBXor)
            Case "Nor"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LBNor)
            Case "Negate"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LBNegate)
            Case "Greater than (>)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LFIGreater)
            Case "Greater or equal to (>=)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LFIGreaterEqual)
            Case "Less or equal to (<=)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LFILess)
            Case "Less than (<)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LFILessEqual)
            Case "Longer than (>)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LSLonger)
            Case "Longer or equal to (>=)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LSLongerEqual)
            Case "Shorter or equal to (<=)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LSShorter)
            Case "Shorter than (<)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.LSShorterEqual)
#End Region
#Region "'------------------------------------------------- MATH OPERATORS"
            Case "Add (+)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MAdd)
            Case "Subtract (-)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MSubtract)
            Case "Divide (/)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MDivide)
            Case "Multiply (*)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MMultiply)
            Case "Mod (%)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MMod)
            Case "Absolute (Abs)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MAbsolute)
            Case "Round (Round to Closest)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MRound)
            Case "Ceil (Round Up)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MCeil)
            Case "Floor (Round Down"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MFloor)
            Case "Increment (++)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MIncrement)
            Case "Decrement (--)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MDecrement)
            Case "Bit Shift Right (>>)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MBitShiftRight)
            Case "Bit Shift Left (<<)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MBitShiftLeft)
            Case "Inverse (*-1)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MInverse)
            Case "Power (^)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MPower)
            Case "Square root (sqrt)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MSquareRoot)
            Case "Clamp (Min and Max)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.MClamp)
#End Region
#Region "'------------------------------------------------- IO OPERATORS"
            Case "Receive Input"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.IOReceiveInput)
            Case "Print Text"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.IOPrintText)
#End Region
#Region "'------------------------------------------------- CONVERTERS"
            Case "To Integer (FTI)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CFloatInteger)
            Case "To String (FTS)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CFloatString)
            Case "To Char (FTC)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CFloatChar)
            Case "To Float (ITF)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CIntegerFloat)
            Case "To String (ITS)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CIntegerString)
            Case "To Char (ITC)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CIntegerChar)
            Case "To Integer (STI)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CStringInteger)
            Case "To Float (STF)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CStringFloat)
            Case "To Char (STC)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CStringChar)
            Case "To Float (CTF)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CCharFloat)
            Case "To Integer (CTI)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CCharInteger)
            Case "To String (CTS)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CCharString)
            Case "To Float (BTF)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CBoolFloat)
            Case "To Integer (BTI)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CBoolInteger)
            Case "To String (BTS)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CBoolString)
            Case "To Char (BTC)"
                Addeer = New ComponentsLogic(ComponentsCanvas, ComponentType.CBoolChar)
#End Region
#Region "'------------------------------------------------- CONSTANTS"
            Case "Constant Boolean"
                Addeer = New ConstantLogic(IOTypes.TBoolean, ComponentsCanvas)
            Case "Constant Integer"
                Addeer = New ConstantLogic(IOTypes.TInteger, ComponentsCanvas)
            Case "Constant Float"
                Addeer = New ConstantLogic(IOTypes.TFloat, ComponentsCanvas)
            Case "Constant String"
                Addeer = New ConstantLogic(IOTypes.TString, ComponentsCanvas)
            Case "Constant Char"
                Addeer = New ConstantLogic(IOTypes.TChar, ComponentsCanvas)
#End Region
#Region "'------------------------------------------------- VARIABLES"
            Case "+New"
                Dim CreateNewVar As New CreateNewVar(Me)
                CreateNewVar.Owner = Application.Current.MainWindow
                CreateNewVar.ShowDialog()
#End Region
        End Select

        If Addeer IsNot Nothing Then
            AddToCanvas(Addeer)
            AddHandler Addeer.MouseDown, AddressOf MoveControleDOWN
        End If
    End Sub

    'CONTROL THE LINE -------------------------------------------------------------------------
    '*Dois handlers de evento da linha estao no canvasMouseMove e canvasMouseUp
    Private Sub ReceiveBaseInMouseUpDown(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BaseInOutMouseUpDown(sender, e)
    End Sub
    Private Sub ReceiveBaseOutMouseUpDown(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BaseInOutMouseUpDown(sender, e)
    End Sub
    Private Sub BaseInOutMouseUpDown(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim Entrada = TryCast(e.OriginalSource, BaseIn)
        Dim Saida = TryCast(e.OriginalSource, BaseOut)
        Dim IsEntrada As Boolean

        If Entrada IsNot Nothing Then 'Valida o tipo certo
            IsEntrada = True
        ElseIf Saida IsNot Nothing Then
            IsEntrada = False
        End If

        If LinhaMoving Is Nothing Then '==================== 1--Linha nova =======================================================================
            If e.OriginalSource.GetLinhasList.count > 0 Then '1.1-Linha Existente (ja conectada a algo)-------------------------------------------
                If (Not IsEntrada And e.OriginalSource.GetTipo = IOTypes.TExecution) Or  'saida e tipo execucao só pode 1 linha
                   (IsEntrada And Not e.OriginalSource.GetTipo = IOTypes.TExecution) Then  'entrada e qualquer tipo que nao seja execucao só pode 1 linha

                    LinhaMoving = e.OriginalSource.GetLinhaByIndex() 'Pega a linha de onde clicamos
                    If Not IsEntrada Then LinhaMoving.GetCompFin().RemoveLinhaByItem(e.OriginalSource.GetLinhaByIndex()) Else LinhaMoving.GetCompIni().RemoveLinhaByItem(e.OriginalSource.GetLinhaByIndex()) 'remove a ref da linha do comp de origem
                    If Not IsEntrada Then LinhaMoving.SetCompFin(Nothing) Else LinhaMoving.SetCompIni(Nothing) 'remove a ref do comp de dentro da linha
                    If Not IsEntrada Then LinhaMoving.SetParentCompFin(Nothing) Else LinhaMoving.SetParentCompIni(Nothing) 'remove a ref do comp pai de dentro da linha
                    If Not IsEntrada Then LinhaMoving.StartFollowTheMouse(PontoDaLinha.PontoFinal) Else LinhaMoving.StartFollowTheMouse(PontoDaLinha.PontoInicial) 'Começa a seguir o mouse
                    LinhaMoving.SetStyle(e.OriginalSource.GetTipo)

                Else 'se a entrada/saida permite varias linhas, tratar como uma  nova linha
                    GoTo linhanova
                End If
            Else '-------------------------------------------1.2-Linha Nova (Nao havia nada conectado ao comp)-------------------------------------
linhanova:
                Dim NovaLinha As New Linha(e.OriginalSource.GetTipo, ComponentsCanvas) 'Criamos uma nova instancia
                If IsEntrada Then NovaLinha.SetCompFin(e.OriginalSource) Else NovaLinha.SetCompIni(e.OriginalSource) 'Setamos a ref do comp de origem na linha
                If IsEntrada Then NovaLinha.SetParentCompFin(FindCompConnectedToThisLine(e.OriginalSource)) Else NovaLinha.SetParentCompIni(FindCompConnectedToThisLine(e.OriginalSource)) 'Setamos a ref do comp de origem na linha
                e.OriginalSource.AddLinha(NovaLinha) 'Seta a ref da linha no comp de origem
                LinhaMoving = NovaLinha 'coloca a ref da linha estamos movendo na var global
                ComponentsCanvas.Children.Add(NovaLinha.LinhaPath) 'Adicionamos o path no canvas (que é o desenho da linha)

                LinhaNameCounter += 1 'Increment line namer
                NovaLinha.Name = "Ln" & LinhaNameCounter.ToString 'Line name (For save porpuses)
                LinhasOnCanvas.Add(NovaLinha) 'Add Line to list (For save)
            End If



        Else '======================================================== 2--Linha existente sendo solta ===========================================

            If ((LinhaMoving.GetCompIni Is Nothing And Not IsEntrada) Or 'lado oposto (saida->entrada) ou
            (IsEntrada And LinhaMoving.GetCompFin Is Nothing)) And       'lado oposto (entrada->saida) e
            (e.OriginalSource.GetTipo = LinhaMoving.GetTipo Or            'TIPOS:                                                                           'tipos iguais ou
            (e.OriginalSource.GetTipo = IOTypes.TFloatOrInt And (LinhaMoving.GetTipo = IOTypes.TFloat Or LinhaMoving.GetTipo = IOTypes.TInteger)) Or        'linha = int or float ou
            (LinhaMoving.GetTipo = IOTypes.TFloatOrInt And (e.OriginalSource.GetTipo = IOTypes.TFloat Or e.OriginalSource.GetTipo = IOTypes.TInteger)) Or   'objeto = int or float ou
            (e.OriginalSource.GetTipo = IOTypes.TCharOrString And (LinhaMoving.GetTipo = IOTypes.TChar Or LinhaMoving.GetTipo = IOTypes.TString)) Or        'linha = char ou string ou
            (LinhaMoving.GetTipo = IOTypes.TCharOrString And (e.OriginalSource.GetTipo = IOTypes.TChar Or e.OriginalSource.GetTipo = IOTypes.TString)) Or   'objeto = char ou string ou
            (e.OriginalSource.GetTipo = IOTypes.TAny And Not LinhaMoving.GetTipo = IOTypes.TExecution) Or
            (LinhaMoving.GetTipo = IOTypes.TAny And Not e.OriginalSource.GetTipo = IOTypes.TExecution)) Then                                                'linha ou objeto = any

                '------------------------------------------------------2.1--Soltou em lugar valido------------------------------------------------

                'Verifica se já há alguma linha connectado ali
                If e.OriginalSource.GetLinhasList.count > 0 Then
                    If (Not IsEntrada And e.OriginalSource.GetTipo = IOTypes.TExecution) Or  'saida e tipo execucao só pode 1 linha
                       (IsEntrada And Not e.OriginalSource.GetTipo = IOTypes.TExecution) Then  'entrada e qualquer tipo que nao seja execucao só pode 1 linha

                        '----------------------------------------------2.1.1-Valido, porem já tem alguma coisa ligada ali-------------------------
                        EliminarLinha(e.OriginalSource.GetLinhaByIndex()) 'temos que remover a linha antiga, entao pegamos a ref dela dentro do comp
                    End If
                End If
                '------------------------------------------------------2.1.2-Valido e Vazio-------------------------------------------------------
                e.OriginalSource.AddLinha(LinhaMoving) 'Setamos a ref da linha no comp de destino
                If IsEntrada Then LinhaMoving.SetCompFin(e.OriginalSource) Else LinhaMoving.SetCompIni(e.OriginalSource) 'Setamos a ref do comp de destino na linha
                If IsEntrada Then LinhaMoving.SetParentCompFin(FindCompConnectedToThisLine(e.OriginalSource)) Else LinhaMoving.SetParentCompIni(FindCompConnectedToThisLine(e.OriginalSource)) 'Setamos a ref do comp de destino na linha
                LinhaMoving.StopFollowTheMouse() 'Paramos de seguir o mouse
                LinhaMoving.AtualizarLinha() 'Ataualizamos a linha para ela dar um "snap" no lugar correto

                '------------------------------------------------------2.1.3-Valido, porem Tipos Diferentes---------------------------------------
                '(definir a cor da linha de acordo com o tipo predominante)
                '(Any < IorF/SorC < todos os outros)
                Dim CIniTipo As IOTypes = LinhaMoving.GetCompIni.GetTipo
                Dim CFinTipo As IOTypes = LinhaMoving.GetCompFin.GetTipo
                If Not CIniTipo = CFinTipo Then
                    If CIniTipo = IOTypes.TFloatOrInt Or CIniTipo = IOTypes.TCharOrString Then
                        If CFinTipo = IOTypes.TAny Then
                            LinhaMoving.SetStyle(CIniTipo)
                        Else
                            LinhaMoving.SetStyle(CFinTipo)
                        End If
                    ElseIf CFinTipo = IOTypes.TFloatOrInt Or CFinTipo = IOTypes.TCharOrString Then
                        If CIniTipo = IOTypes.TAny Then
                            LinhaMoving.SetStyle(CFinTipo)
                        Else
                            LinhaMoving.SetStyle(CIniTipo)
                        End If
                    ElseIf CFinTipo = IOTypes.TAny Then
                        LinhaMoving.SetStyle(CIniTipo)
                    ElseIf CIniTipo = IOTypes.TAny Then
                        LinhaMoving.SetStyle(CFinTipo)
                    End If
                End If

                LinhaMoving = Nothing 'Removemos a ref da var global


            Else '-----------------------------------------------------2.2--Soltou em lugar invalido----------------------------------------------
                EliminarLinha(LinhaMoving) 'removemos a linha
            End If
        End If '----- end line connection logic (PHEW!)


        e.Handled = True 'Informamos ao comp que disparou o evento que já cuidamos do evento,
        'isso evita que o evento "suba" para niveis mais altos da arvore de componentes, economizando processamento


        UpdateCodeText()

    End Sub
    Private Sub EliminarLinha(ByRef Linha As Linha)
        If Linha.GetCompIni() IsNot Nothing Then Linha.GetCompIni().RemoveLinhaByItem(Linha) 'removemos a ref da linha nos comps
        If Linha.GetCompFin() IsNot Nothing Then Linha.GetCompFin().RemoveLinhaByItem(Linha)
        ComponentsCanvas.Children.Remove(Linha.LinhaPath) 'removemos o path do canvas (parte grafica)
        Linha.StopFollowTheMouse() 'paramos de seguir o mouse
        LinhasOnCanvas.Remove(Linha)
        Linha = Nothing 'Removemos a ref da var global
    End Sub

    'CANVAS SCALING ---------------------------------------------------------------------------
    Private Sub Canvas_MouseWheel(sender As Object, e As MouseWheelEventArgs) Handles ParentCanvas.MouseWheel
        Dim ScaleTransform As New ScaleTransform

        'Set the size
        LayoutScaleIndex += (e.Delta / Math.Abs(e.Delta))
        LayoutScaleIndex = Math.Max(0, Math.Min(LayoutScaleIndex, LayoutScaleSteps.Count - 1))
        LayoutScale = LayoutScaleSteps(LayoutScaleIndex)

        'zoom around the mouse
        Dim PosBeforeScale As New Point(Mouse.GetPosition(ComponentsCanvas).X, Mouse.GetPosition(ComponentsCanvas).Y)

        'Transform to the set size
        ScaleTransform.ScaleX = LayoutScale
        ScaleTransform.ScaleY = LayoutScale
        Me.ParentCanvas.LayoutTransform = ScaleTransform
        UpdateLayout()
        UpdateLayout()

        'zoom around the mouse
        Dim PosAfterScale As New Point(Mouse.GetPosition(ComponentsCanvas).X, Mouse.GetPosition(ComponentsCanvas).Y)

        CanvasCurrLocation = New Point(ViewportClass.ViewportCoordinates.Left, ViewportClass.ViewportCoordinates.Top)
        ViewportClass.setVPCoords(New Rect((PosAfterScale.X - PosBeforeScale.X) + CanvasCurrLocation.X, (PosAfterScale.Y - PosBeforeScale.Y) + CanvasCurrLocation.Y, BigGridSize, BigGridSize))
    End Sub
    'MOVE CANVAS AROUND!-----------------------------------------------------------------------
    Private Sub Canvas_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles ParentCanvas.MouseDown
        If IsNothing(Mouse.Captured) Then

            KeyboardFocuser.Focus()

            MouseOffsetCanvas = Mouse.GetPosition(ParentCanvas)
            CanvasCurrLocation = New Point(ViewportClass.ViewportCoordinates.Left, ViewportClass.ViewportCoordinates.Top)


            If e.LeftButton = MouseButtonState.Pressed Then 'Deselect/SelectRect
                MouseOffsetSelectRect = Mouse.GetPosition(ComponentsCanvas)
                If Not Keyboard.IsKeyDown(Key.LeftShift) And Not Keyboard.IsKeyDown(Key.LeftCtrl) Then
                    ClearMarker()
                End If
                If LinhaMoving Is Nothing Then
                    DrawSelectRect()
                End If
                ParentCanvas.CaptureMouse()
                e.Handled = True
            End If

            If e.MiddleButton = MouseButtonState.Pressed Then 'Canvas Pan
                ParentCanvas.CaptureMouse()
                e.Handled = True
            End If

            If e.RightButton = MouseButtonState.Pressed Then 'Adicionar componente
                CreateCompPos = Mouse.GetPosition(ComponentsCanvas)
                ParentCanvas.ContextMenu.IsOpen = True
                e.Handled = True
            End If

        End If
    End Sub
    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles ParentCanvas.MouseMove
        If e.MiddleButton = MouseButtonState.Pressed AndAlso Not MouseOffsetCanvas = New Point(0, 0) Then
            ViewportClass.setVPCoords(New Rect((Mouse.GetPosition(ParentCanvas).X - MouseOffsetCanvas.X) + CanvasCurrLocation.X, (Mouse.GetPosition(ParentCanvas).Y - MouseOffsetCanvas.Y) + CanvasCurrLocation.Y, BigGridSize, BigGridSize))
            e.Handled = True
        End If

        If e.LeftButton = MouseButtonState.Pressed Then 'DraggingLineUpdate/ComponentMove/DrawSelectRect
            If LinhaMoving IsNot Nothing Then LinhaMoving.AtualizarLinha() ' atualiza o path da linha se estivermos movendo uma
#Region "Components movement"
            If Not isSelecting Then 'Movendo Componentes
                If ComponentMoving.Count > 0 Then
                    For Each o As Object In Marker
                        Dim MouseCompOffset As Point = ComponentMovingMouseOffset(Marker.IndexOf(o))
                        Dim ComponentMove As Object = ComponentMoving(Marker.IndexOf(o))
                        o.SetValue(Canvas.LeftProperty, CDbl(Mouse.GetPosition(ComponentsCanvas).X - MouseCompOffset.X))
                        o.SetValue(Canvas.TopProperty, CDbl(Mouse.GetPosition(ComponentsCanvas).Y - MouseCompOffset.Y))
                    Next
#End Region
                Else 'Seleção de componentes (SelectRect)
                    If LinhaMoving Is Nothing Then
                        DrawSelectRect()
                    End If
                End If
            End If
            e.Handled = True
        End If
    End Sub
    Private Sub Canvas_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles ParentCanvas.MouseUp
        If e.MiddleButton = MouseButtonState.Released Then
            MouseOffsetCanvas = New Point(0, 0)
        End If

        If e.LeftButton = MouseButtonState.Released Then 'ReleaseCompMove/ReleaseMovingLine/SelectRect
            If Not isSelecting Then
#Region "Components movement"
                If ComponentMoving.Count > 0 Then
                    For Each o As Object In ComponentMoving
                        Dim releasePos As Point = Mouse.GetPosition(ComponentsCanvas)
                        Dim MouseCompOffset As Point = ComponentMovingMouseOffset(ComponentMoving.IndexOf(o))

                        If o.getValue(Canvas.LeftProperty) < 0 Then releasePos.X = releasePos.X - MouseCompOffset.X - HalfGridSize _
                                            Else releasePos.X = releasePos.X - MouseCompOffset.X + HalfGridSize

                        If o.getValue(Canvas.TopProperty) < 0 Then releasePos.Y = releasePos.Y - MouseCompOffset.Y - HalfGridSize _
                                            Else releasePos.Y = releasePos.Y - MouseCompOffset.Y + HalfGridSize

                        If EnableSnapping Then
                            o.SetValue(Canvas.LeftProperty, CDbl((releasePos.X \ GridSize) * GridSize))
                            o.SetValue(Canvas.TopProperty, CDbl((releasePos.Y \ GridSize) * GridSize))
                        End If
                    Next
                    ComponentMoving.Clear()
                    ComponentMovingMouseOffset.Clear()
                End If
#End Region
                If LinhaMoving IsNot Nothing Then
                    EliminarLinha(LinhaMoving) 'Eliminamos a linha se soltarmos ela no canvas
                End If
            End If
            MouseOffsetSelectRect = New Point(vbNull, vbNull)
            If SelectRect.Visibility = Visibility.Visible Then 'Are we selecting with the rectangle?
                SelectComponentsInsideSelectRect()
            End If
            e.Handled = True
        End If

        ParentCanvas.ReleaseMouseCapture()


    End Sub

    'MOVE COMPONENTS AROUND!-------------------------------------------------------------------
    '*Components selection is a hybrid between components events and canvas events
    public Sub MoveControleDOWN(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            If Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.LeftCtrl) Then
                isSelecting = True
            Else
                isSelecting = False
            End If

            If Not Keyboard.IsKeyDown(Key.LeftShift) And Not Keyboard.IsKeyDown(Key.LeftCtrl) And Marker.IndexOf(sender) = -1 Then
                ClearMarker()
            End If

            BringToFront(sender) 'Bring to front

            If Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.LeftCtrl) Or Marker.Count = 0 Then
                SetMarker(sender)
            End If
            ComponentMoving.Clear()
            ComponentMovingMouseOffset.Clear()

            For Each o As Object In Marker
                ComponentMoving.Add(o)
                ComponentMovingMouseOffset.Add(Mouse.GetPosition(o)) 'move init
            Next
            ParentCanvas.CaptureMouse() 'lock mouse to component
        End If
    End Sub

    'KEY PRESSES-------------------------------------------------------------------------------
    Private Sub KeyboardFocuser_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles KeyboardFocuser.PreviewKeyDown
        If (e.Key = Key.Delete) Then 'Delete marked components
            DeleteMarked()
        End If
        ' e.Handled = True 'Stops the event from reaching the textbox
    End Sub

    Private Sub KeyboardFocuser_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles KeyboardFocuser.PreviewTextInput
        e.Handled = True
    End Sub

    'COMMON FUNCTIONS AND SUBS-----------------------------------------------------------------
    'Markers
    Private Sub ClearMarker()
        If Marker.Count > 0 Then
            For Each o As Object In Marker
                o.SetMarker(False)
            Next
        End If
        Marker.Clear()
    End Sub
    Private Sub SetMarker(ByRef sender As Object)
        If Marker.IndexOf(sender) = -1 Then
            Marker.Add(sender)
            If Marker.Count > 0 Then
                For Each o As Object In Marker
                    o.SetMarker(True)
                Next
            End If
        ElseIf Keyboard.IsKeyDown(Key.LeftShift) And Not Keyboard.IsKeyDown(Key.LeftCtrl) Then
            Marker.Remove(sender)
            sender.SetMarker(False)
        End If
    End Sub
    Private Sub DeleteMarked()
        If Marker.Count > 0 Then
            For Each o As Object In Marker
                RemoveFromCanvas(o)
            Next
        End If
        Marker.Clear()
    End Sub
    Private Sub BringToFront(ByRef sender As Object)
        ComponentsCanvas.Children.Remove(sender)
        ComponentsCanvas.Children.Add(sender)
    End Sub

    'Add/Remove from Canvas
    Public Sub AddToCanvas(ByRef NewComponent As Object, Optional name As String = "")
        Panel.SetZIndex(NewComponent, 10)

        If Not name = "" Then
            NewComponent.name = name
        Else
            While (ObjectsOnCanvas.ContainsKey("Comp" & CompNameCounter.ToString))
                CompNameCounter += 1
            End While

            NewComponent.Name = "Comp" & CompNameCounter.ToString
            CompNameCounter += 1
        End If

        ComponentsCanvas.Children.Add(NewComponent)
        ObjectsOnCanvas.Add(NewComponent.Name, NewComponent)
        If EnableSnapping Then

            If CreateCompPos.X < 0 Then CreateCompPos.X = CreateCompPos.X - HalfGridSize - NewComponent.ActualWidth / 2 Else CreateCompPos.X = CreateCompPos.X + HalfGridSize - NewComponent.ActualWidth / 2
            If CreateCompPos.Y < 0 Then CreateCompPos.Y = CreateCompPos.Y - HalfGridSize - NewComponent.ActualHeight / 2 Else CreateCompPos.Y = CreateCompPos.Y + HalfGridSize - NewComponent.ActualHeight / 2

            NewComponent.SetValue(Canvas.LeftProperty, CDbl((CreateCompPos.X \ GridSize) * GridSize))
            NewComponent.SetValue(Canvas.TopProperty, CDbl((CreateCompPos.Y \ GridSize) * GridSize))

        End If
    End Sub
    Public Sub RemoveFromCanvas(ByRef Component As Object)
        If Component IsNot funcBegin And Component IsNot funcEnd Then
            ComponentsCanvas.Children.Remove(Component)
            ObjectsOnCanvas.Remove(Component.name)
            Component.RemoveConnections()
        End If
        Component.setMarker(False)
        Dim IsVar = TryCast(Component, VariablesLogic)
        If IsVar IsNot Nothing Then
            RemoveVarCompFromDict(Component)
        End If
        Component = Nothing
    End Sub
    Public Function GetObjectOnCanvasByName(name As String) As Object
        Dim o As New Object
        If (ObjectsOnCanvas.TryGetValue(name, o)) Then
            Return o
        Else
            Return Nothing
        End If
    End Function

    'Select Rect
    Public Sub DrawSelectRect()
        'Makes life easier
        Dim SelectRectPos1 As Point = MouseOffsetSelectRect
        Dim SelectRectPos2 As Point = Mouse.GetPosition(ComponentsCanvas)

        'Only draw if pos1 != 0,0
        If Not SelectRectPos1 = New Point(vbNull, vbNull) Then
            'Only show the rect after a set treshold
            If Not SelectRect.IsVisible Then
                If Math.Abs(Math.Abs(SelectRectPos1.X) - Math.Abs(SelectRectPos2.X)) > 5 Or Math.Abs(Math.Abs(SelectRectPos1.Y) - Math.Abs(SelectRectPos2.Y)) > 5 Then
                    SelectRect.Visibility = Visibility.Visible
                ElseIf SelectRect.IsVisible Then
                    SelectRect.Visibility = Visibility.Hidden
                End If
            End If

            Dim temp As Double 'Inverts the positions so no negative width and height are passed to the rect
            If SelectRectPos1.X > SelectRectPos2.X Then
                temp = SelectRectPos1.X
                SelectRectPos1.X = SelectRectPos2.X
                SelectRectPos2.X = temp
            End If
            If SelectRectPos1.Y > SelectRectPos2.Y Then
                temp = SelectRectPos1.Y
                SelectRectPos1.Y = SelectRectPos2.Y
                SelectRectPos2.Y = temp
            End If

            'Set the rect position and size
            SelectRect.SetValue(Canvas.LeftProperty, SelectRectPos1.X)
            SelectRect.SetValue(Canvas.TopProperty, SelectRectPos1.Y)
            SelectRect.Width = SelectRectPos2.X - SelectRectPos1.X
            SelectRect.Height = SelectRectPos2.Y - SelectRectPos1.Y
        End If
    End Sub
    Public Sub SelectComponentsInsideSelectRect()
        Dim RectMinimos As New Point(SelectRect.GetValue(Canvas.LeftProperty), SelectRect.GetValue(Canvas.TopProperty))
        Dim RectMaximos As New Point(SelectRect.GetValue(Canvas.LeftProperty) + SelectRect.Width, SelectRect.GetValue(Canvas.TopProperty) + SelectRect.Height)

        For Each objeto As Object In ObjectsOnCanvas.Values
            Dim ObjetoMinimos As New Point(objeto.GetValue(Canvas.LeftProperty), objeto.GetValue(Canvas.TopProperty))
            Dim ObjetoMaximos As New Point(ObjetoMinimos.X + objeto.GetBase.ActualWidth, ObjetoMinimos.Y + objeto.GetBase.ActualHeight)

            If (
                (RectMinimos.X < ObjetoMinimos.X And RectMaximos.X > ObjetoMinimos.X) Or ' Dentro do lado esquerdo
                (RectMinimos.X < ObjetoMaximos.X And RectMaximos.X > ObjetoMaximos.X) Or ' Dentro do lado direito
                (RectMinimos.X > ObjetoMinimos.X And RectMaximos.X < ObjetoMaximos.X)    ' Entre os dois lados
                ) And (
                (RectMinimos.Y < ObjetoMinimos.Y And RectMaximos.Y > ObjetoMinimos.Y) Or ' Dentro do lado de cima
                (RectMinimos.Y < ObjetoMaximos.Y And RectMaximos.Y > ObjetoMaximos.Y) Or ' Dentro do lado de baixo
                (RectMinimos.Y > ObjetoMinimos.Y And RectMaximos.Y < ObjetoMaximos.Y)    ' Entre os dois lados
                ) Then

                SetMarker(objeto)
            End If
        Next
        SelectRect.Visibility = Visibility.Hidden
    End Sub

    'Generate code
    Private Sub UpdateCodeText()
        TextCode.Text = GetCodeFromNodes()
    End Sub
    Private Function GetCodeFromNodes() As String
        Return "" 'funcBegin.GetLogic()
    End Function

    'VAR CONTROLS ------------------------------------------------------------------------------
    Public Function CheckIfVarExists(VarName As String) As Boolean
        Return VarsDict.ContainsKey(VarName)
    End Function
    Sub ConstructNewVar(VarStruct As VarsStruct)

        Dim NewVarStruct As New VarsStruct
        NewVarStruct.VarsCompsOnCanvas = New List(Of Object)
        NewVarStruct.HashedName = HashString(VarStruct.ReadableName)
        NewVarStruct.ReadableName = VarStruct.ReadableName
        NewVarStruct.Type = VarStruct.Type
        NewVarStruct.DefaultValue = VarStruct.DefaultValue
        NewVarStruct.IsArray = VarStruct.IsArray

        VarsDict.Add(VarStruct.ReadableName, NewVarStruct)

        RebuildCanvasVarsContextMenu()

    End Sub
    'Var Controls for context menu
    Sub RebuildCanvasVarsContextMenu()
        Dim CanvasCM As ContextMenu = ParentCanvas.ContextMenu
        Dim CanvasCMItem As New MenuItem
        Dim CanvasCMSubItem As New MenuItem
        Dim CanvasCMSeparator As New Separator


        CanvasCMItem.Header = "Variables"
        CanvasCMSubItem = New MenuItem
        CanvasCMSubItem.Header = "+New"
        AddHandler(CanvasCMSubItem.Click), AddressOf CanvasContextMenu
        CanvasCMItem.Items.Add(CanvasCMSubItem)

        CanvasCMItem.Items.Add(CanvasCMSeparator)

        For Each o As VarsStruct In VarsDict.Values
            CanvasCMSubItem = New MenuItem
            CanvasCMSubItem.Header = o.ReadableName
            AddHandler CanvasCMSubItem.Click, AddressOf ShowVarContextMenu
            CanvasCMItem.Items.Add(CanvasCMSubItem)
        Next

        CanvasCM.Items.Item(6) = CanvasCMItem  'Variables submenu
        ParentCanvas.ContextMenu = CanvasCM

    End Sub
    Sub ShowVarContextMenu(sender As Object, ByVal e As EventArgs)
        VarSelectedName = sender.header.ToString
        CreateVarContextMenu()
    End Sub 'VarSelectedName here
    Sub CreateVarContextMenu()
        Dim ThisVarStruct As New VarsStruct
        VarsDict.TryGetValue(VarSelectedName, ThisVarStruct)
        VarsCM = New ContextMenu
#Region "Context Menu"
        'Create menu items
        Dim VarsMI As New MenuItem
        Dim VarsSMI As New MenuItem
        Dim VarsSeparator As New Separator

        VarsMI = New MenuItem
        VarsMI.Header = "Get Value"
        AddHandler(VarsMI.Click), AddressOf VarGetCreateComponent
        VarsCM.Items.Add(VarsMI)

        VarsMI = New MenuItem
        VarsMI.Header = "Set Value"
        AddHandler(VarsMI.Click), AddressOf VarSetCreateComponent
        VarsCM.Items.Add(VarsMI)


        VarsSeparator = New Separator
        VarsCM.Items.Add(VarsSeparator)


        VarsMI = New MenuItem
        VarsMI.Header = "Name: " & VarSelectedName
        AddHandler(VarsMI.Click), AddressOf VarChangeStructure
        VarsCM.Items.Add(VarsMI)

        VarsMI = New MenuItem
        Select Case ThisVarStruct.Type
            Case IOTypes.TBoolean
                VarsMI.Header = "Type: Boolean"
            Case IOTypes.TString
                VarsMI.Header = "Type: String"
            Case IOTypes.TChar
                VarsMI.Header = "Type: Char"
            Case IOTypes.TInteger
                VarsMI.Header = "Type: Integer"
            Case IOTypes.TFloat
                VarsMI.Header = "Type: Float"
        End Select
        AddHandler(VarsMI.Click), AddressOf VarChangeStructure
        VarsCM.Items.Add(VarsMI)

        VarsMI = New MenuItem
        If ThisVarStruct.IsArray Then VarsMI.Header = "Is Array: Yes" Else VarsMI.Header = "Is Array: No"
        AddHandler(VarsMI.Click), AddressOf VarChangeStructure
        VarsCM.Items.Add(VarsMI)

        VarsMI = New MenuItem
        VarsMI.Header = "Default: " & ThisVarStruct.DefaultValue
        AddHandler(VarsMI.Click), AddressOf VarChangeStructure
        VarsCM.Items.Add(VarsMI)
#End Region

        VarsCM.StaysOpen = True
        VarsCM.IsOpen = True

    End Sub
    'Add Var to canvas
    Sub VarGetCreateComponent(sender As Object, ByVal e As EventArgs)
        VarCreateComponent(False)
    End Sub
    Sub VarSetCreateComponent(sender As Object, ByVal e As EventArgs)
        VarCreateComponent(True)
    End Sub
    Sub VarCreateComponent(isSet As Boolean)
        Dim ThisVarStruct As New VarsStruct
        VarsDict.TryGetValue(VarSelectedName, ThisVarStruct)
        Dim ThisVarNewComp As New VariablesLogic(ComponentsCanvas, ThisVarStruct, isSet)
        ThisVarStruct.VarsCompsOnCanvas.Add(ThisVarNewComp)
        AddHandler ThisVarNewComp.MouseDown, AddressOf MoveControleDOWN
        AddToCanvas(ThisVarNewComp)
    End Sub
    'Remove Var from canvas
    Sub RemoveVarCompFromDict(ByRef varComp As VariablesLogic)
        VarSelectedName = varComp.VarStruct.ReadableName
        Dim ThisVarStruct As New VarsStruct
        VarsDict.TryGetValue(VarSelectedName, ThisVarStruct)
        ThisVarStruct.VarsCompsOnCanvas.Remove(varComp)
    End Sub
    'Var values updaters
    Sub VarChangeStructure(sender As Object, ByVal e As EventArgs)
        Dim VarStruct As New VarsStruct
        VarsDict.TryGetValue(VarSelectedName, VarStruct)
        Dim CreateNewVar As New CreateNewVar(Me, VarStruct)
        CreateNewVar.Owner = Application.Current.MainWindow
        CreateNewVar.ShowDialog()
    End Sub
    Public Sub UpdateVarStruct(VarStruct As VarsStruct)
        Dim ExistingVarStruct As New VarsStruct
        VarsDict.TryGetValue(VarSelectedName, ExistingVarStruct)
        ExistingVarStruct.HashedName = HashString(VarStruct.ReadableName)
        ExistingVarStruct.ReadableName = VarStruct.ReadableName
        ExistingVarStruct.Type = VarStruct.Type
        ExistingVarStruct.DefaultValue = VarStruct.DefaultValue
        ExistingVarStruct.IsArray = VarStruct.IsArray
        For Each o As VariablesLogic In ExistingVarStruct.VarsCompsOnCanvas
            o.UpdateVarStruct(ExistingVarStruct)
        Next
        VarsDict.Remove(VarSelectedName)
        VarsDict.Add(ExistingVarStruct.ReadableName, ExistingVarStruct)
        RebuildCanvasVarsContextMenu()
    End Sub


    'SAVE/LOAD ---------------------------------------------------------------------------------
    Public Sub SaveToFile(FilePath As String, Optional AutoOverwriteFile As Boolean = False)

        Dim File As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(FilePath, False)
        Dim Encrypter As New Simple3Des("PGhhvbjJEieNxOstS7kpwT2RyMvJ9V7X69QFG2wFZj4uoKLC8QqBqSCz94yblYV1")

        Dim MegaString As String

        MegaString = VarsDict.Count.ToString
        For Each v As VarsStruct In VarsDict.Values
            MegaString &= "|" & v.ReadableName
            MegaString &= "|" & v.HashedName
            MegaString &= "|" & v.Type
            MegaString &= "|" & v.IsArray
            MegaString &= "|" & v.DefaultValue
        Next

        MegaString &= "|" & ObjectsOnCanvas.Count.ToString
        For Each o As Object In ObjectsOnCanvas.Values

            If o.GetType Is GetType(ComponentsLogic) Then
                MegaString &= "|" & o.GetType.ToString
                MegaString &= "|" & o.GetComponentType
                MegaString &= "|" & "Filler"
                MegaString &= "|" & o.name.ToString
                MegaString &= "|" & o.GetValue(Canvas.LeftProperty).ToString
                MegaString &= "|" & o.GetValue(Canvas.TopProperty).ToString
            End If

            If o.GetType Is GetType(ConstantLogic) Then
                MegaString &= "|" & o.GetType.ToString
                MegaString &= "|" & o.GetTipo
                MegaString &= "|" & o.GetCurrentConstValue
                MegaString &= "|" & o.name.ToString
                MegaString &= "|" & o.GetValue(Canvas.LeftProperty).ToString
                MegaString &= "|" & o.GetValue(Canvas.TopProperty).ToString
            End If

            If o.GetType Is GetType(VariablesLogic) Then
                MegaString &= "|" & o.GetType.ToString
                MegaString &= "|" & o.GetVarKey
                MegaString &= "|" & o.GetIsSetAsString
                MegaString &= "|" & o.name.ToString
                MegaString &= "|" & o.GetValue(Canvas.LeftProperty).ToString
                MegaString &= "|" & o.GetValue(Canvas.TopProperty).ToString
            End If
        Next


        'Cleanup LinhasOnCanvas before saving the linhas
        Dim LineToBeRemoved As New List(Of Linha)
        For Each l As Linha In LinhasOnCanvas
            If Not ComponentsCanvas.Children.Contains(l.LinhaPath) Then 'Se a linhapath nao esta no canvas, adicione a fila de remoção
                LineToBeRemoved.Add(l)
            End If
        Next
        For Each l As Linha In LineToBeRemoved
            LinhasOnCanvas.Remove(l)
        Next


        MegaString &= "|" & LinhasOnCanvas.Count.ToString
        For Each l As Linha In LinhasOnCanvas
            MegaString &= "|" & l.GetTipo
            MegaString &= "|" & l.GetParentCompIni.name
            MegaString &= "|" & l.GetCompIni.name
            MegaString &= "|" & l.GetParentCompFin.name
            MegaString &= "|" & l.GetCompFin.name
        Next

        File.Write(Encrypter.EncryptData(MegaString))
        File.Close()

    End Sub
End Class