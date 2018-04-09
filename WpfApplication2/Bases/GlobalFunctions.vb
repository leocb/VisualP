Imports System.ComponentModel
Imports System.Security.Cryptography

Public Module GlobalFunctions
    'Propriedades customizadas
    Function HashString(InputString As String) As String
        'Hash the varName for our internal code using a non-destructive method (can be reversed)
        'This prevents the user from using reserved names (if, else, ...)
        'without the need for a full list of reserved words. This is not shown to the user at design time.
        Dim AllChars As New List(Of Char) From {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
        Dim HashedName As String = "V"
        For i = 0 To InputString.Length - 1
            HashedName &= Hex(AllChars.IndexOf(InputString.Chars(i))).PadLeft(2, "0")
        Next
        Return HashedName
    End Function
    Public Class CustomProp
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Sub NotifyPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        'VARS
        Dim VPCoords As New Rect(0, 0, 0, 0)

        Public Function setVPCoords(Coords As Rect)
            VPCoords = Coords
            NotifyPropertyChanged("ViewportCoordinates")
            NotifyPropertyChanged("ViewportCoordinatesX")
            NotifyPropertyChanged("ViewportCoordinatesY")
            Return 0
        End Function
        Public ReadOnly Property ViewportCoordinates As Rect
            Get
                Return VPCoords
            End Get
        End Property
        Public ReadOnly Property ViewportCoordinatesX As Double
            Get
                Return VPCoords.X
            End Get
        End Property
        Public ReadOnly Property ViewportCoordinatesY As Double
            Get
                Return VPCoords.Y
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Class Taken from https://msdn.microsoft.com/en-us/library/ms172831.aspx
    ''' </summary>
    Public NotInheritable Class Simple3Des
        Private TripleDes As New TripleDESCryptoServiceProvider
        Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()

            Dim sha1 As New SHA1CryptoServiceProvider

            ' Hash the key.
            Dim keyBytes() As Byte =
                System.Text.Encoding.Unicode.GetBytes(key)
            Dim hash() As Byte = sha1.ComputeHash(keyBytes)

            ' Truncate or pad the hash.
            ReDim Preserve hash(length - 1)
            Return hash
        End Function

        Sub New(ByVal key As String)
            ' Initialize the crypto provider.
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize \ 8)
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize \ 8)
        End Sub

        Public Function EncryptData(ByVal plaintext As String) As String

            ' Convert the plaintext string to a byte array.
            Dim plaintextBytes() As Byte =
                System.Text.Encoding.Unicode.GetBytes(plaintext)

            ' Create the stream.
            Dim ms As New System.IO.MemoryStream
            ' Create the encoder to write to the stream.
            Dim encStream As New CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

            ' Use the crypto stream to write the byte array to the stream.
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
            encStream.FlushFinalBlock()

            ' Convert the encrypted stream to a printable string.
            Return Convert.ToBase64String(ms.ToArray)
        End Function

        Public Function DecryptData(ByVal encryptedtext As String) As String
            ' Convert the encrypted text string to a byte array.
            Dim encryptedBytes() As Byte
            Try
                encryptedBytes = Convert.FromBase64String(encryptedtext)
            Catch
                MessageBox.Show("File is corrupted and cannot be loaded" & vbNewLine & "Error 0x00000052", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                Return ""
            End Try

            ' Create the stream.
            Dim ms As New System.IO.MemoryStream
            ' Create the decoder to write to the stream.
            Dim decStream As New CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

            ' Use the crypto stream to write the byte array to the stream.
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
            decStream.FlushFinalBlock()

            ' Convert the plaintext stream to a string.
            Return System.Text.Encoding.Unicode.GetString(ms.ToArray)
        End Function
    End Class

    ''' <summary>
    ''' Class translated to VB from http://www.infragistics.com/community/blogs/blagunas/archive/2013/05/29/find-the-parent-control-of-a-specific-type-in-wpf-and-silverlight.aspx
    ''' Returns the parent object of the specified type, if a parent cannot be found, returns Nothing
    ''' </summary>
    ''' <param name="child">Object from where to begin the search</param>
    ''' <param name="typeToFind">Type of parent to be found</param>
    ''' <returns></returns>
    Public Function FindParent(child As DependencyObject, typeToFind As Type) As DependencyObject

        Dim parent As DependencyObject
        If VisualTreeHelper.GetParent(child) IsNot Nothing Then
            parent = VisualTreeHelper.GetParent(child)
        Else
            Return Nothing
        End If

        Dim FoundType As Type = parent.GetType

        If parent.GetType = typeToFind Then
            Return parent
        Else
            Return FindParent(parent, typeToFind)
        End If
    End Function

    Public Function FindCompConnectedToThisLine(ByRef BaseInOut As Object) As DependencyObject
        Dim returnValue As Object
        Debug.Print("ComponentsLogic")
        returnValue = FindParent(BaseInOut, GetType(ComponentsLogic))
        If IsNothing(returnValue) Then Debug.Print("NOTHING") Else Debug.Print(returnValue.ToString)
        If returnValue IsNot Nothing Then Return returnValue

        Debug.Print("ConstantLogic")
        returnValue = FindParent(BaseInOut, GetType(ConstantLogic))
        If IsNothing(returnValue) Then Debug.Print("NOTHING") Else Debug.Print(returnValue.ToString)
        If returnValue IsNot Nothing Then Return returnValue

        Debug.Print("VariablesLogic")
        returnValue = FindParent(BaseInOut, GetType(VariablesLogic))
        If IsNothing(returnValue) Then Debug.Print("NOTHING") Else Debug.Print(returnValue.ToString)
        If returnValue IsNot Nothing Then Return returnValue

        Return Nothing
    End Function


End Module
