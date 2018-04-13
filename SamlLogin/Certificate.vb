Imports System.Security.Cryptography.X509Certificates

Public Class Certificate

    Public cert As X509Certificate2

    Public Sub LoadCertificate(ByVal certificate As String)
        cert = New X509Certificate2()
        cert.Import(certificate)
    End Sub

    Public Sub LoadCertificate(ByVal certificate As Byte())
        cert = New X509Certificate2()
        cert.Import(certificate)
    End Sub

    Private Function StringToByteArray(ByVal st As String) As Byte()
        Dim bytes As Byte() = New Byte(Len(st)) {}

        For i As Integer = 0 To Len(st)
            bytes(i) = Convert.ToByte(st(i))
        Next

        Return bytes
    End Function


End Class
