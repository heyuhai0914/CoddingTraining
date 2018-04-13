Imports System.Security.Cryptography.Xml
Imports System.Xml

Public Class LoginResponse

    'TODO 署名キーを設定　複数があった場合？
    Private Const CertificateStr As String = "-----BEGIN CERTIFICATE-----\nMIIDBTCCAe2gAwIBAgIQev76BWqjWZxChmKkGqoAfDANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTE4MDIxODAwMDAwMFoXDTIwMDIxOTAwMDAwMFowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMgmGiRfLh6Fdi99XI2VA3XKHStWNRLEy5Aw\n-----END CERTIFICATE-----"
    Private Const GlobalID As String = "GlobalID"
    Dim xmlDoc As XmlDocument
    Dim accountSettings As Object
    Dim certificate As Certificate

    Public Sub New()
        'ByVal accountSettings As Object)
        'accountSettings = accountSettings
        certificate = New Certificate()
        'certificate.LoadCertificate(CertificateStr)

    End Sub

    Public Sub LoadXml(ByVal xml As String)
        xmlDoc = New XmlDocument()
        xmlDoc.PreserveWhitespace = True
        xmlDoc.XmlResolver = Nothing
        xmlDoc.LoadXml(xml)

        'GIDの取得をテストのため追加
        Dim manager As XmlNamespaceManager = New XmlNamespaceManager(xmlDoc.NameTable)
        manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim node As XmlNode = xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement", manager)
        Dim elem As XmlElement = xmlDoc.CreateElement("Attribute")
        elem.SetAttribute("Name", "GlobalID")
        Dim elem2 As XmlElement = xmlDoc.CreateElement("AttributeValue")
        elem2.InnerText = "1234567890"
        elem.AppendChild(elem2)
        node.AppendChild(elem)

    End Sub

    Public Sub LoadXmlFromBase64(ByVal response As String)
        Dim enc As New System.Text.ASCIIEncoding()
        LoadXml(enc.GetString(Convert.FromBase64String(response)))
    End Sub

    Public Function IsValid() As Boolean
        Dim status As Boolean = True

        Dim signedXml As New SignedXml(xmlDoc)

        Dim manager = New XmlNamespaceManager(xmlDoc.NameTable)
        manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim nodeList As XmlNodeList = xmlDoc.SelectNodes("//ds:Signature", manager)

        signedXml.LoadXml(nodeList(0))

        'status &= signedXml.CheckSignature(certificate.cert, True)

        'Dim notBefore As DateTime? = Me.NotBefore()
        'status &= notBefore Is Nothing OrElse notBefore <= DateTime.Now

        'Dim notOnOrAfter As DateTime? = Me.NotOnOrAfter
        'status &= notOnOrAfter Is Nothing OrElse notOnOrAfter <= DateTime.Now

        Return status
    End Function


    Public Function NotBefore() As DateTime?

        Dim manager As XmlNamespaceManager = New XmlNamespaceManager(xmlDoc.NameTable)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim nodes = xmlDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:Conditions", manager)

        Dim value As Object = Nothing

        If nodes.Count() > 0 AndAlso nodes(0) IsNot Nothing AndAlso nodes(0).Attributes IsNot Nothing AndAlso nodes(0).Attributes("NotBefore") IsNot Nothing Then

            value = nodes(0).Attributes("NotBefore").Value

        End If
        Return If(value IsNot Nothing, DateTime.Parse(value), Nothing)


    End Function

    Public Function NotOnOrAfter() As DateTime?

        Dim manager As XmlNamespaceManager = New XmlNamespaceManager(xmlDoc.NameTable)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim nodes = xmlDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:Conditions", manager)

        Dim value As Object = Nothing

        If nodes.Count() > 0 AndAlso nodes(0) IsNot Nothing AndAlso nodes(0).Attributes IsNot Nothing AndAlso nodes(0).Attributes("NotOnOrAfter") IsNot Nothing Then

            value = nodes(0).Attributes("NotOnOrAfter").Value

        End If
        Return If(value IsNot Nothing, DateTime.Parse(value), Nothing)

    End Function

    Public Function GetNameID() As String

        Dim manager As XmlNamespaceManager = New XmlNamespaceManager(xmlDoc.NameTable)
        manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim node As XmlNode = xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", manager)

        Return node.InnerText

    End Function


    ''' <summary>
    ''' GIDを取得
    ''' </summary>
    ''' <returns></returns>
    Public Function GetGID() As String

        Dim manager As XmlNamespaceManager = New XmlNamespaceManager(xmlDoc.NameTable)

        manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl)
        manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion")
        manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol")

        Dim node As XmlNode = xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement", manager)

        For Each element As XmlElement In node.ChildNodes()
            If element.GetAttribute("Name") = GlobalID Then
                Return element.ChildNodes(0).InnerText
            End If
        Next
        Return String.Empty

    End Function

End Class
