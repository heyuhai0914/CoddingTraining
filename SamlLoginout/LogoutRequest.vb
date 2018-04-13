Imports System.IO
Imports System.IO.Compression
Imports System.Xml

Public Class LoginoutRequest

    Private id As String
    Private issue_instant As String
    Private iniFile As String = "\coda.ini" 'INIファイル名
    Private Const UnderLine As String = "_"

    Public Sub New(ByVal dir As String)
        id = UnderLine & System.Guid.NewGuid().ToString()
        issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        iniFile = dir + iniFile
    End Sub

    ''' <summary>
    ''' ログアウトのリクエストを生成
    ''' </summary>
    ''' <returns></returns>
    Public Function getRequest() As String

        Dim AzureLoginNM As StringBuilder = New StringBuilder(1024)
        Dim Issuer As StringBuilder = New StringBuilder(1024)
        IniFileHandler.GetPrivateProfileString("Coda", "AzureLoginNM", "default", AzureLoginNM, AzureLoginNM.Capacity, iniFile)
        IniFileHandler.GetPrivateProfileString("Coda", "Issuer", "default", Issuer, Issuer.Capacity, iniFile)

        Using sw As New StringWriter
            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.OmitXmlDeclaration = False

            Using xw = XmlWriter.Create(sw, settings)

                xw.WriteStartDocument()
                xw.WriteStartElement("samlp", "LogoutRequest", "urn:oasis:names:tc:SAML:2.0:protocol")
                xw.WriteAttributeString("xmlns", "urn:oasis:names:tc:SAML:2.0:metadata")
                xw.WriteAttributeString("ID", id)
                xw.WriteAttributeString("Version", "2.0")
                xw.WriteAttributeString("IssueInstant", issue_instant)
                xw.WriteAttributeString("AssertionConsumerServiceURL", "https://localhost:44314/home/loginOut")
                xw.WriteStartElement("Issuer", "urn:oasis:names:tc:SAML:2.0:assertion")
                '識別子
                xw.WriteString(Issuer.ToString())
                xw.WriteEndElement()
                xw.WriteStartElement("NameID", "urn:oasis:names:tc:SAML:2.0:assertion")
                'TODO サインアウトしているユーザーの NameIDと厳密に一致する必要がある
                xw.WriteString(AzureLoginNM.ToString())
                xw.WriteEndElement()
                xw.WriteEndElement()
                xw.Flush()
                Dim toEncodeAsBytes As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes((sw.ToString.Replace("utf-16", "utf-8")))

                Dim ms As MemoryStream = New MemoryStream()
                Dim CompressedStream As DeflateStream = New DeflateStream(ms, CompressionMode.Compress, True)
                CompressedStream.Write(toEncodeAsBytes, 0, toEncodeAsBytes.Length)
                CompressedStream.Close()

                Dim destination As Byte() = ms.ToArray()
                ms.Close()

                Return System.Convert.ToBase64String(destination)

            End Using

        End Using

    End Function

End Class