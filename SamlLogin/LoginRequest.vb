Imports System.IO
Imports System.IO.Compression
Imports System.Xml

Public Class LoginRequest

    Private Const UnderLine As String = "_"
    Private iniFile As String = "\coda.ini" 'INIファイル名
    Private id As String                'ID
    Private issue_instant As String     'IssueInstant

    Public Sub New(ByVal dir As String)
        id = UnderLine & System.Guid.NewGuid().ToString()
        issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        iniFile = dir + iniFile
    End Sub

    Public Function GetRequest() As String

        Dim ReplayURL As StringBuilder = New StringBuilder(1024)
        Dim Issuer As StringBuilder = New StringBuilder(1024)
        Dim RedirectURL As StringBuilder = New StringBuilder(1024)
        IniFileHandler.GetPrivateProfileString("Coda", "ReplayURL", "default", ReplayURL, ReplayURL.Capacity, iniFile)
        IniFileHandler.GetPrivateProfileString("Coda", "Issuer", "default", Issuer, Issuer.Capacity, iniFile)
        IniFileHandler.GetPrivateProfileString("Coda", "RedirectURL", "default", RedirectURL, RedirectURL.Capacity, iniFile)

        Using sw As New StringWriter
            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.OmitXmlDeclaration = False
            settings.Encoding = System.Text.Encoding.UTF8

            Using xw = XmlWriter.Create(sw, settings)
                xw.WriteStartDocument()
                xw.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol")
                xw.WriteAttributeString("AssertionConsumerServiceURL", ReplayURL.ToString())
                xw.WriteAttributeString("Destination", RedirectURL.ToString())
                xw.WriteAttributeString("ForceAuthn", "true")
                xw.WriteAttributeString("ID", id)
                xw.WriteAttributeString("IsPassive", "false")
                xw.WriteAttributeString("IssueInstant", issue_instant)
                xw.WriteAttributeString("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST")
                xw.WriteAttributeString("Version", "2.0")
                xw.WriteStartElement("samlp", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion")
                xw.WriteString(Issuer.ToString())
                xw.WriteEndElement()
                xw.WriteEndElement()
                xw.WriteEndDocument()
                xw.Flush()

                'Dim str As String = ""
                'str &= "<?xml version=""1.0"" encoding=""UTF-8""?>"
                'str &= "<saml:AuthnRequest "
                'str &= "AssertionConsumerServiceURL=""https://localhost:44314/"" "
                'str &= "Destination=""https://login.microsoftonline.com/855256f7-1cde-412c-987f-be7f874f9475/saml2"" "
                'str &= "ID=""a4fe279a4b047hf14ccbbhgfaci530g"" "
                'str &= "IssueInstant=""2018-04-10T12:02:31.578Z"" "
                'str &= "ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"" "
                'str &= "Version=""2.0"" "
                'str &= "xmlns:saml=""urn:oasis:names:tc:SAML:2.0:protocol"">"
                'str &= "<saml:Issuer "
                'str &= "xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"">https://localhost:44314/"
                'str &= "</saml:Issuer>"
                'str &= "</saml:AuthnRequest>"
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
