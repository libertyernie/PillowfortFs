namespace PillowfortFs

open System
open System.IO
open System.Net
open System.Text
open System.Text.RegularExpressions

type PillowfortClientFactoryException(message: string) =
    inherit ApplicationException(message)

module PillowfortClientFactory =
    let private pillowfail str = raise (PillowfortClientFactoryException str)

    let private ua = "PillowfortFs/0.1 (https://github.com/libertyernie)"

    let private get_authenticity_token cookies = async {
        let req = WebRequest.CreateHttp("https://pillowfort.io/users/sign_in", CookieContainer = cookies, UserAgent = ua)
        
        use! response = req.AsyncGetResponse()
        use sr = new StreamReader(response.GetResponseStream())
        let! contents = sr.ReadToEndAsync() |> Async.AwaitTask
        
        let m = Regex.Match(contents, """<input type="hidden" name="authenticity_token" value="([^"]+)" """)

        if not m.Success then
            pillowfail "No authenticity_token found"

        return m.Groups.[1].Value
    }

    let AsyncLogin username password = async {
        // Create a container to keep cookies between requests
        let cookies = new CookieContainer()

        // Get the server-generated token for the form submission
        let! authenticity_token = get_authenticity_token cookies
        
        let req = WebRequest.CreateHttp("https://pillowfort.io/users/sign_in", CookieContainer = cookies, UserAgent = ua, Method = "POST", ContentType = "application/x-www-form-urlencoded")

        let parameters = [
            sprintf "%s=%s" "utf8" (WebUtility.UrlEncode "✓")
            sprintf "%s=%s" "authenticity_token" (WebUtility.UrlEncode authenticity_token)
            sprintf "%s=%s" "user[email]" (WebUtility.UrlEncode username)
            sprintf "%s=%s" "user[password]" (WebUtility.UrlEncode password)
            sprintf "%s=%s" "user[remember_me]" "0"
            "commit=Log+in"
        ]
        let requestBody = String.concat "&" parameters

        // Write to request body
        // I do this in a separate scope so the stream is automatically flushed and can't be used later.
        do! async {
            use! reqStream = req.GetRequestStreamAsync() |> Async.AwaitTask

            use sw = new StreamWriter(reqStream, Encoding.UTF8)
            do! requestBody |> sw.WriteLineAsync |> Async.AwaitTask
        }
        
        // Send request and read from response body
        use! resp = req.AsyncGetResponse()
        if resp.ResponseUri.AbsolutePath <> "/" then
            pillowfail "Did not redirect to / as expected"

        let cookie = cookies.GetCookies(new Uri("https://pillowfort.io")).["_Pillowfort_session"]
        if isNull cookie then
            pillowfail "_Pillowfort_session cookie not found"

        return new PillowfortClient(Cookie = cookie.Value)
    }

    let CreateClient u p = AsyncLogin u p |> Async.RunSynchronously
    let CreateClientAsync u p = AsyncLogin u p |> Async.StartAsTask