Public Class HomeController
    Inherits System.Web.Mvc.Controller


    ''' <summary>
    ''' aadログイン画面へリダイレクト処理
    ''' </summary>
    ''' <returns></returns>

    Function Index() As ActionResult

        If IsNothing(Request.Form("SAMLResponse")) Then
            Response.Cache.SetCacheability(HttpCacheability.NoCache)

            Dim lgr As LoginRequest = New LoginRequest(Request.MapPath("").ToString())
            Response.Redirect("https://login.microsoftonline.com/855256f7-1cde-412c-987f-be7f874f9475/saml2" + "?SAMLRequest=" + HttpUtility.UrlEncode(lgr.GetRequest()))

        Else
            Me.About()
        End If

        Return View()
    End Function

    Protected Sub About()

        Dim response As LoginResponse = New LoginResponse()

        response.LoadXmlFromBase64(Request.Form("SAMLResponse"))
        If response.IsValid() Then

            Dim gid As String = response.GetGID()
            'メイン画面にリダイレクト
            'GIDによりメニュー制御処理を行う
            ViewData("Message") = "Login Success"
            ViewData("Gid") = gid
        Else
            'TODO 仕様更新次第
            '無効の場合、エラー画面に、無効であることを伝えるメッセージを表示するようにする、
            'メッセージは設定ファイルで変更可能
        End If

    End Sub

    ''' <summary>
    ''' ログアウトイベント
    ''' </summary>
    ''' <returns></returns>
    Function LoginOut() As ActionResult

        Dim logout As LoginoutRequest = New LoginoutRequest(Request.MapPath("").ToString())
        'https://coda.sony.jp/logout をクリックするときに、当該メソッドが呼ばれる

        Response.Redirect("https://login.microsoftonline.com/855256f7-1cde-412c-987f-be7f874f9475/saml2" + "?SAMLRequest=" + Server.UrlEncode(logout.getRequest()))
        Return View()
    End Function

    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."

        Return View()
    End Function
End Class
