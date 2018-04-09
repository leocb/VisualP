Public Module GlobalEnumsAndValues
    'CONSTANTS
    Public Const HalfGridSize As Double = 16
    Public Const GridSize As Double = 32
    Public Const BigGridSize As Double = 256

    'STRUCTURES
    Structure VarsStruct
        Dim VarsCompsOnCanvas As List(Of Object)
        Dim ReadableName As String
        Dim HashedName As String
        Dim Type As IOTypes
        Dim IsArray As Boolean
        Dim DefaultValue As String
    End Structure

    'ENUMS
    Public Enum PontoDaLinha
        PontoInicial
        PontoFinal
    End Enum

    Public Enum IOTypes
        TExecution 'Linha indicando o caminho
        TBoolean    'True / False
        TInteger    'Numero Inteiro
        TFloat      'Numero Real
        TString     'Texto
        TChar       'Letra
        TAny        'Qualquer
        TFloatOrInt 'Numero real ou inteiro
        TCharOrString 'Caracter ou Sequencia de Caracteres
    End Enum
    Public Enum BlockTypes
        BTFlow       'Redireciona o caminho da logica
        BTLogic      'Operação Logica
        BTOperator   'Operação Aritimética
        BTInOut      'Entra e Saida de dados ou começo e fim de função
        BTConverter 'Conversão de tipos de dados
        BTNone      'Sem fundo
    End Enum

    Public Enum ComponentType
#Region "Exec Logic"
        EIf
        EFor
        EWhile
        ESwitch
        ECompareInt
        EBegin
        EEnd
#End Region
#Region "Logic"
        LFIGreater
        LFIGreaterEqual
        LANYEqual
        LFILessEqual
        LFILess
        LANYNotEqual
        LBAnd
        LBOr
        LBXor
        LBNor
        LBNegate
        LSLonger
        LSLongerEqual
        LSShorter
        LSShorterEqual
#End Region
#Region "Math"
        MAdd
        MSubtract
        MDivide
        MMultiply
        MMod
        MAbsolute
        MRound
        MCeil
        MFloor
        MIncrement
        MDecrement
        MBitShiftRight
        MBitShiftLeft
        MInverse
        MPower
        MSquareRoot
        MClamp
#End Region
#Region "InputOutput"
        IOReceiveInput
        IOPrintText
#End Region
#Region "Converters"
        CStringFloat
        CStringInteger
        CStringChar
        CCharString
        CCharFloat
        CCharInteger
        CFloatString
        CFloatInteger
        CFloatChar
        CIntegerString
        CIntegerFloat
        CIntegerChar
        CBoolString
        CBoolFloat
        CBoolChar
        CBoolInteger
#End Region
    End Enum
End Module
